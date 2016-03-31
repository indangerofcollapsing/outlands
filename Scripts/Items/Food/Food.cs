using System;
using System.Collections;
using Server.Network;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Custom;
using Server.Spells;
using Server.Mobiles;

namespace Server.Items
{
    public class FoodDecayTimer : Timer
    {
        public static void Initialize()
        {
            new FoodDecayTimer().Start();
        }

        public FoodDecayTimer(): base(TimeSpan.FromMinutes(60), TimeSpan.FromMinutes(60))
        {
            Priority = TimerPriority.OneMinute;
        }

        protected override void OnTick()
        {
            CheckFoodDecay();
        }

        public static void CheckFoodDecay()
        {
            Queue m_Queue = new Queue();

            foreach (Item item in World.Items.Values)
            {
                if (!(item is Food))
                    continue;

                Food food = item as Food;

                if (!food.Decays) continue;
                if (food.Map == null || food.Map == Map.Internal) continue;

                Mobile parent = food.RootParentEntity as Mobile;

                if (parent != null)
                {
                    //Refresh Expiration
                    if ((parent is BaseCreature && !(parent is PlayerVendor)) || parent.AccessLevel > AccessLevel.Player)
                    {                        
                        food.DecayExpiration = DateTime.UtcNow + food.DecayDuration;
                        continue;
                    }
                }

                m_Queue.Enqueue(food);
            }

            while (m_Queue.Count > 0)
            {
                Item item = (Item)m_Queue.Dequeue();
                item.Delete();
            }
        }
    }

	public abstract class Food : Item
	{
        public enum SatisfactionLevelType
        {
            None,
            Paltry,
            Meagre,
            Adequate,
            Appetizing,
            Delectable
        }

        public static int CharacterCreationHunger = 30;
        public static int MaxHunger = 60;        

        public static double HungerThirstTickDuration = 5; //In Minutes
        public static int HungerThirstLostPerTick = 5;

        public virtual int FillFactor { get { return 6; } }
        public virtual SatisfactionLevelType Satisfaction { get { return SatisfactionLevelType.Paltry; } }
        public virtual TimeSpan SatisfactionDuration { get { return TimeSpan.FromMinutes(30); } }        
        public virtual int MaxCharges { get { return 1; } }
        public virtual bool Decays { get { return false; } }
        public virtual TimeSpan DecayDuration { get { return TimeSpan.FromDays(7); } }

        public virtual int MinStaminaRegained { get { return 10; } }
        public virtual int MaxStaminaRegained { get { return 20; } }        

        public static bool AllowInPlayerVsPlayerCombat = true;

        private int m_Charges = 1;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get { return m_Charges; }
            set { m_Charges = value; }
        }

        private DateTime m_DecayExpiration = DateTime.UtcNow + TimeSpan.FromDays(1000);
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime DecayExpiration
        {
            get { return m_DecayExpiration; }
            set { m_DecayExpiration = value; }
        }

        private Poison m_Poison;
        [CommandProperty(AccessLevel.GameMaster)]
        public Poison Poison
        {
            get { return m_Poison; }
            set { m_Poison = value; }
        }

        private Mobile m_Poisoner;
		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Poisoner
		{
			get { return m_Poisoner; }
			set { m_Poisoner = value; }
		}                

		public Food( int itemID ) : this( 1, itemID )
		{
		}

		public Food( int amount, int itemID ) : base( itemID )
		{
			Stackable = true;
			Amount = amount;

            Charges = MaxCharges;
            DecayExpiration = DateTime.UtcNow + DecayDuration;
		}

		public Food( Serial serial ) : base( serial )
		{
		}

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (m_Charges > 1)
                LabelTo(from, "(" + m_Charges.ToString() + " bites remaining)");

            if (Decays)
            {
                string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, m_DecayExpiration, true, true, true, true, false);

                if (m_DecayExpiration <= DateTime.UtcNow)
                    LabelTo(from, "[will expire shortly]");

                else                 
                    LabelTo(from, "[expires in " + timeRemaining + "]");
            }
        } 

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );

			if ( from.Alive )
				list.Add( new ContextMenus.EatEntry( from, this ) );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !Movable )
				return;

			if ( from.InRange( this.GetWorldLocation(), 1 ) )			
				Eat( from );			
		}

        public bool AttemptEat(Mobile from)
        {
            if (from.Hunger + FillFactor > Food.MaxHunger)
            {
                from.SendMessage("You are too full to eat that.");
                return false;
            }

            from.Hunger += FillFactor;

            PlayerMobile player = from as PlayerMobile;

            if (player != null)
            {
                if (Satisfaction >= player.SatisfactionLevel)
                {
                    player.SatisfactionExpiration = DateTime.UtcNow + SatisfactionDuration;
                    player.SatisfactionLevel = Satisfaction;
                }
            }

            if (from.Stam < from.StamMax)
                from.Stam += Utility.RandomMinMax(MinStaminaRegained, MaxStaminaRegained);

            int iHunger = from.Hunger + FillFactor;

            string satisfiedMessage = "";
            string hungerMessage = "";

            switch (Satisfaction)
            {
                case SatisfactionLevelType.Paltry: satisfiedMessage = "The food is paltry and barely satisfies."; break;
                case SatisfactionLevelType.Meagre: satisfiedMessage = " The food is meagre and mildly satisfies."; break;
                case SatisfactionLevelType.Adequate: satisfiedMessage = "The food is adequate and decently satisfies."; break;
                case SatisfactionLevelType.Appetizing: satisfiedMessage = "The food is appetizing and greatly satisfies."; break;
                case SatisfactionLevelType.Delectable: satisfiedMessage = "The food is delectable and completely satisfies."; break;
            }

            double hungerPercent = (double)iHunger / (double)MaxHunger;

            if (hungerPercent < .25)
                hungerMessage = "You are still very hungry.";

            else if (hungerPercent < .5)
                hungerMessage = "You are still slightly hungry.";

            else if (hungerPercent < .75)
                hungerMessage = "You are somewhat full.";

            else if (hungerPercent < 1.0)
                hungerMessage = "You are very full.";

            else
                hungerMessage = "You are completely full.";

            from.SendMessage(satisfiedMessage);
            from.SendMessage(hungerMessage);

            return true;
        }

		public virtual bool Eat(Mobile from)
		{			
			if (AttemptEat(from))
			{
                //Player Enhancement Customization: Mouthy
                bool mouthy = PlayerEnhancementPersistance.IsCustomizationEntryActive(from, CustomizationType.Mouthy);

                if (from.Body.IsHuman && !from.Mounted)
                    from.Animate(34, 5, 1, true, false, 0);

                Point3D location = from.Location;
                Map map = from.Map;

                if (mouthy)
                {
                    //Chomp
                    from.PlaySound(Utility.RandomList(0x5DA));
                    from.PublicOverheadMessage(MessageType.Regular, 0, false, "*chomps*");

                    for (int a = 1; a < 3; a++)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(a * 1), delegate
                        {
                            if (from == null) return;
                            if (!from.Alive) return;

                            if (from.Body.IsHuman && !from.Mounted)
                                from.Animate(34, 5, 1, true, false, 0);

                            //Chew
                            if (Utility.RandomDouble() <= .75)
                            {
                                from.PlaySound(Utility.RandomList(0x5A9, 0x5AB, 0x03A, 0x03B, 0x03C));
                                from.PublicOverheadMessage(MessageType.Regular, 0, false, "*chews noisily*");
                            }

                            //Spill
                            else
                            {
                                from.PlaySound(Utility.RandomList(0x5D8, 0x5D9, 0x5DB, 0x5A2, 0x580, 0x581));

                                TimedStatic droppedFood = new TimedStatic(2482, 5);

                                switch (Utility.RandomMinMax(1, 3))
                                {
                                    case 1:
                                        droppedFood.Name = "food mush";
                                        droppedFood.ItemID = 2484;
                                        droppedFood.Hue = 2313;
                                    break;

                                    case 2:
                                        droppedFood.Name = "food mush";
                                        droppedFood.ItemID = 2485;
                                        droppedFood.Hue = 2313;
                                    break;

                                    case 3:
                                        droppedFood.Name = "food mush";
                                        droppedFood.ItemID = 2486;
                                        droppedFood.Hue = 2313;
                                    break;
                                }

                                from.PublicOverheadMessage(MessageType.Regular, 0, false, "*spills*");

                                Point3D foodLocation = new Point3D(location.X + Utility.RandomList(-1, 1), location.Y + Utility.RandomList(-1, 1), location.Z);
                                SpellHelper.AdjustField(ref foodLocation, map, 12, false);

                                droppedFood.MoveToWorld(foodLocation, map);
                            }
                        });
                    }
                }

                else                
                    from.PlaySound(Utility.Random(0x3A, 3));   

				if ( m_Poison != null )
					from.ApplyPoison( m_Poisoner, m_Poison );

                if (Stackable)
                    Consume();

                else if (m_Charges > 1)
                    m_Charges--;

                else
                    Delete();

				return true;
			}

			return false;
		}

        public static void CheckFoodHitsRegen(PlayerMobile player)
        {
            if (player.Region is UOACZRegion) return;
            if (!AllowInPlayerVsPlayerCombat && player.RecentlyInPlayerCombat) return;
            if (player.SatisfactionExpiration <= DateTime.UtcNow) return;

            double chance = 0;

            switch (player.SatisfactionLevel)
            {
                case SatisfactionLevelType.Paltry: chance = .1; break;
                case SatisfactionLevelType.Meagre: chance = .2; break;
                case SatisfactionLevelType.Adequate: chance = .3; break;
                case SatisfactionLevelType.Appetizing: chance = .4; break;
                case SatisfactionLevelType.Delectable: chance = .5; break;
            }

            if (Utility.RandomDouble() <= chance)
                player.Hits++;
        }

        public static void CheckFoodStamRegen(PlayerMobile player)
        {
            if (player.Region is UOACZRegion) return;
            if (!AllowInPlayerVsPlayerCombat && player.RecentlyInPlayerCombat) return;
            if (player.SatisfactionExpiration <= DateTime.UtcNow) return;

            double chance = 0;

            switch (player.SatisfactionLevel)
            {
                case SatisfactionLevelType.Paltry: chance = .1; break;
                case SatisfactionLevelType.Meagre: chance = .2; break;
                case SatisfactionLevelType.Adequate: chance = .3; break;
                case SatisfactionLevelType.Appetizing: chance = .4; break;
                case SatisfactionLevelType.Delectable: chance = .5; break;
            }

            if (Utility.RandomDouble() <= chance)
                player.Stam++;
        }

        public static void CheckFoodManaRegen(PlayerMobile player)
        {
            if (player.Region is UOACZRegion) return;
            if (!AllowInPlayerVsPlayerCombat && player.RecentlyInPlayerCombat) return;
            if (player.SatisfactionExpiration <= DateTime.UtcNow) return;

            double chance = 0;

            switch (player.SatisfactionLevel)
            {
                case SatisfactionLevelType.Paltry: chance = .05; break;
                case SatisfactionLevelType.Meagre: chance = .1; break;
                case SatisfactionLevelType.Adequate: chance = .15; break;
                case SatisfactionLevelType.Appetizing: chance = .2; break;
                case SatisfactionLevelType.Delectable: chance = .25; break;
            }

            if (player.Meditating)
                chance *= .5;

            if (Utility.RandomDouble() <= chance)
                player.Mana++;
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

            //Version 0
            writer.Write(m_Charges);            
			writer.Write(m_Poisoner);

            if (m_Poison != null)
                writer.Write(m_Poison.Level);
            else
                writer.Write(-1);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Charges = reader.ReadInt();                
                m_Poisoner = reader.ReadMobile();

                int poisonLevel = reader.ReadInt();
                if (poisonLevel > -1)
                    m_Poison = Poison.GetPoison(poisonLevel);
            }

            //-----
            
            if (m_DecayExpiration > DateTime.UtcNow + TimeSpan.FromDays(365))
                m_DecayExpiration = DateTime.UtcNow + TimeSpan.FromDays(1000);
		}
	}

	public class BreadLoaf : Food
	{
		[Constructable]
		public BreadLoaf() : this( 1 )
		{
		}

		[Constructable]
		public BreadLoaf( int amount ) : base( amount, 0x103B )
		{
			Weight = 1.0;
		}

		public BreadLoaf( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class Bacon : Food
	{
		[Constructable]
		public Bacon() : this( 1 )
		{
		}

		[Constructable]
		public Bacon( int amount ) : base( amount, 0x979 )
		{
			Weight = 1.0;
		}

		public Bacon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class SlabOfBacon : Food
	{
		[Constructable]
		public SlabOfBacon() : this( 1 )
		{
		}

		[Constructable]
		public SlabOfBacon( int amount ) : base( amount, 0x976 )
		{
			Weight = 1.0;
		}

		public SlabOfBacon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );            
            int version = reader.ReadInt();
		}
	}

	public class FishSteak : Food
	{
		[Constructable]
		public FishSteak() : this( 1 )
		{
		}

		[Constructable]
		public FishSteak( int amount ) : base( amount, 0x97B )
		{
            Weight = .1;
		}

		public FishSteak( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CheeseWheel : Food
	{
        [Constructable]
		public CheeseWheel() : this( 1 )
		{
		}

		[Constructable]
		public CheeseWheel( int amount ) : base( amount, 0x97E )
		{
            Weight = .5;
		}

		public CheeseWheel( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CheeseWedge : Food
	{
		[Constructable]
		public CheeseWedge() : this( 1 )
		{
		}

		[Constructable]
		public CheeseWedge( int amount ) : base( amount, 0x97D )
		{
            Weight = .1;
		}

		public CheeseWedge( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CheeseSlice : Food
	{
		[Constructable]
		public CheeseSlice() : this( 1 )
		{
		}

		[Constructable]
		public CheeseSlice( int amount ) : base( amount, 0x97C )
		{
            Weight = 0.1;
		}

		public CheeseSlice( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class FrenchBread : Food
	{
		[Constructable]
		public FrenchBread() : this( 1 )
		{
		}

		[Constructable]
		public FrenchBread( int amount ) : base( amount, 0x98C )
		{
			Weight = 2.0;
		}

		public FrenchBread( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}


	public class FriedEggs : Food
	{
		[Constructable]
		public FriedEggs() : this( 1 )
		{
		}

		[Constructable]
		public FriedEggs( int amount ) : base( amount, 0x9B6 )
		{
			Weight = 1.0;
		}

		public FriedEggs( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CookedBird : Food
	{
		[Constructable]
		public CookedBird() : this( 1 )
		{
		}

		[Constructable]
		public CookedBird( int amount ) : base( amount, 0x9B7 )
		{
			Weight = 1.0;
		}

		public CookedBird( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class RoastPig : Food
	{
        public override int FillFactor { get { return 30; } }
        public override SatisfactionLevelType Satisfaction { get { return SatisfactionLevelType.Delectable; } }
        public override TimeSpan SatisfactionDuration { get { return TimeSpan.FromMinutes(60); } }
        public override int MaxCharges { get { return 5; } }
        public override bool Decays { get { return true; } }
        public override TimeSpan DecayDuration { get { return TimeSpan.FromDays(3); } }

        public override int MinStaminaRegained { get { return 50; } }
        public override int MaxStaminaRegained { get { return 100; } }

		[Constructable]
		public RoastPig() : this( 1 )
		{
		}

		[Constructable]
		public RoastPig( int amount ) : base( amount, 0x9BB )
		{
            Stackable = false;

			Weight = 10.0;
		}

		public RoastPig( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class Sausage : Food
	{
		[Constructable]
		public Sausage() : this( 1 )
		{
		}

		[Constructable]
		public Sausage( int amount ) : base( amount, 0x9C0 )
		{
			Weight = 1.0;
		}

		public Sausage( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class GreenHam : Food
	{
		[Constructable]
		public GreenHam() : this( 1 )
		{
		}

		[Constructable]
		public GreenHam( int amount ) : base( amount, 0x9C9 )
		{
            Hue = 0x301;

			Weight = 1.0;		
		}

		public GreenHam( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class Ham : Food
	{
		[Constructable]
		public Ham() : this( 1 )
		{
		}

		[Constructable]
		public Ham( int amount ) : base( amount, 0x9C9 )
		{
            Hue = 0x301;

			Weight = 1.0;		
		}

		public Ham( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class Cake : Food
	{
		[Constructable]
		public Cake() : base( 0x9E9 )
		{
			Stackable = false;

			Weight = 1.0;
		}

		public Cake( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class Ribs : Food
	{
		[Constructable]
		public Ribs() : this( 1 )
		{
		}

		[Constructable]
		public Ribs( int amount ) : base( amount, 0x9F2 )
		{
			Weight = 1.0;
		}

		public Ribs( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class Cookies : Food
	{
		[Constructable]
		public Cookies() : base( 0x160b )
		{
			Weight = 1.0;
		}

		public Cookies( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class Muffins : Food
	{
		[Constructable]
		public Muffins() : base( 0x9eb )
		{
			Stackable = false;

			Weight = 1.0;
		}

		public Muffins( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CheesePizza : Food
	{
		public override int LabelNumber{ get{ return 1044516; } } // cheese pizza

		[Constructable]
		public CheesePizza() : base( 0x1040 )
		{
			Stackable = false;

			Weight = 1.0;
		}

		public CheesePizza( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class SausagePizza : Food
	{
		public override int LabelNumber{ get{ return 1044517; } } // sausage pizza

		[Constructable]
		public SausagePizza() : base( 0x1040 )
		{
			Stackable = false;

			Weight = 1.0;
		}

		public SausagePizza( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class FruitPie : Food
	{
		public override int LabelNumber{ get{ return 1041346; } } // baked fruit pie

		[Constructable]
		public FruitPie() : base( 0x1041 )
		{
			Stackable = false;

			Weight = 1.0;
		}

		public FruitPie( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class MeatPie : Food
	{
		public override int LabelNumber{ get{ return 1041347; } } // baked meat pie

		[Constructable]
		public MeatPie() : base( 0x1041 )
		{
			Stackable = false;

			Weight = 1.0;
		}

		public MeatPie( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class PumpkinPie : Food
	{
		public override int LabelNumber{ get{ return 1041348; } } // baked pumpkin pie

		[Constructable]
		public PumpkinPie() : base( 0x1041 )
		{
			Stackable = false;

			Weight = 1.0;
		}

		public PumpkinPie( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class ApplePie : Food
	{
		public override int LabelNumber{ get{ return 1041343; } } // baked apple pie

		[Constructable]
		public ApplePie() : base( 0x1041 )
		{
			Stackable = false;

			Weight = 1.0;
		}

		public ApplePie( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

    public class FruitBasket : Food
    {
        [Constructable]
        public FruitBasket()
            : base(1, 0x993)
        {
            Weight = 2.0;
            Stackable = false;
        }

        public FruitBasket(Serial serial)
            : base(serial)
        {
        }

        public override bool Eat(Mobile from)
        {
            if (!base.Eat(from))
                return false;

            from.AddToBackpack(new Basket());
            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

	public class PeachCobbler : Food
	{
		public override int LabelNumber{ get{ return 1041344; } } // baked peach cobbler

		[Constructable]
		public PeachCobbler() : base( 0x1041 )
		{
			Stackable = false;

			Weight = 1.0;
		}

		public PeachCobbler( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class Quiche : Food
	{
		public override int LabelNumber{ get{ return 1041345; } } // baked quiche

		[Constructable]
		public Quiche() : base( 0x1041 )
		{
			Weight = 1.0;
		}

		public Quiche( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class LambLeg : Food
	{
		[Constructable]
		public LambLeg() : this( 1 )
		{
		}

		[Constructable]
		public LambLeg( int amount ) : base( amount, 0x160a )
		{
			Weight = 2.0;
		}

		public LambLeg( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class ChickenLeg : Food
	{
		[Constructable]
		public ChickenLeg() : this( 1 )
		{
		}

		[Constructable]
		public ChickenLeg( int amount ) : base( amount, 0x1608 )
		{
			Weight = 1.0;
		}

		public ChickenLeg( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	[FlipableAttribute( 0xC74, 0xC75 )]
	public class HoneydewMelon : Food
	{
		[Constructable]
		public HoneydewMelon() : this( 1 )
		{
		}

		[Constructable]
		public HoneydewMelon( int amount ) : base( amount, 0xC74 )
		{
			Weight = 1.0;
		}

		public HoneydewMelon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );            
            int version = reader.ReadInt();
		}
	}

	[FlipableAttribute( 0xC64, 0xC65 )]
	public class YellowGourd : Food
	{
		[Constructable]
		public YellowGourd() : this( 1 )
		{
		}

		[Constructable]
		public YellowGourd( int amount ) : base( amount, 0xC64 )
		{
			Weight = 1.0;
		}

		public YellowGourd( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	[FlipableAttribute( 0xC66, 0xC67 )]
	public class GreenGourd : Food
	{
		[Constructable]
		public GreenGourd() : this( 1 )
		{
		}

		[Constructable]
		public GreenGourd( int amount ) : base( amount, 0xC66 )
		{
			Weight = 1.0;
		}

		public GreenGourd( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	[FlipableAttribute( 0xC7F, 0xC81 )]
	public class EarOfCorn : Food
	{
		[Constructable]
		public EarOfCorn() : this( 1 )
		{
		}

		[Constructable]
		public EarOfCorn( int amount ) : base( amount, 0xC81 )
		{
			Weight = 1.0;
		}

		public EarOfCorn( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class Turnip : Food
	{
		[Constructable]
		public Turnip() : this( 1 )
		{
		}

		[Constructable]
		public Turnip( int amount ) : base( amount, 0xD3A )
		{
			Weight = 1.0;
		}

		public Turnip( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class SheafOfHay : Item
	{
		[Constructable]
		public SheafOfHay() : base( 0xF36 )
		{
			Weight = 10.0;
		}

		public SheafOfHay( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}