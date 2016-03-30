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
	public abstract class Food : Item
	{
        public enum SatisfactionLevel
        {
            None,
            Paltry,
            Meagre,
            Adequate,
            Appetizing,
            Delectable
        }
                
        public virtual SatisfactionLevel Satisfaction { get { return SatisfactionLevel.Paltry; } }
        public virtual int FillFactor { get { return 5; } }

        public static bool AllowInPlayerVsPlayerCombat = true;

        private int m_Charges = 1;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get { return m_Charges; }
            set { m_Charges = value; }
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
		}

		public Food( Serial serial ) : base( serial )
		{
		}

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (m_Charges > 1)
                LabelTo(from, "(" + m_Charges.ToString() + " bites remaining)");

            if (Satisfaction != SatisfactionLevel.None)
                LabelTo(from, "[" + Satisfaction.ToString().ToLower() + "]");
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

		public virtual bool Eat( Mobile from )
		{			
			if ( CanEat( from ) )
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

            double chance = 0;

            switch (player.FoodSatisfaction)
            {
                case SatisfactionLevel.Paltry: chance = .1; break;
                case SatisfactionLevel.Meagre: chance = .2; break;
                case SatisfactionLevel.Adequate: chance = .3; break;
                case SatisfactionLevel.Appetizing: chance = .4; break;
                case SatisfactionLevel.Delectable: chance = .5; break;
            }

            if (Utility.RandomDouble() <= chance)
                player.Hits++;
        }

        public static void CheckFoodStamRegen(PlayerMobile player)
        {
            if (player.Region is UOACZRegion) return;
            if (!AllowInPlayerVsPlayerCombat && player.RecentlyInPlayerCombat) return;

            double chance = 0;

            switch (player.FoodSatisfaction)
            {
                case SatisfactionLevel.Paltry: chance = .1; break;
                case SatisfactionLevel.Meagre: chance = .2; break;
                case SatisfactionLevel.Adequate: chance = .3; break;
                case SatisfactionLevel.Appetizing: chance = .4; break;
                case SatisfactionLevel.Delectable: chance = .5; break;
            }

            if (Utility.RandomDouble() <= chance)
                player.Stam++;
        }

        public static void CheckFoodManaRegen(PlayerMobile player)
        {
            if (player.Region is UOACZRegion) return;
            if (!AllowInPlayerVsPlayerCombat && player.RecentlyInPlayerCombat) return;

            double chance = 0;

            switch (player.FoodSatisfaction)
            {
                case SatisfactionLevel.Paltry: chance = .05; break;
                case SatisfactionLevel.Meagre: chance = .1; break;
                case SatisfactionLevel.Adequate: chance = .15; break;
                case SatisfactionLevel.Appetizing: chance = .2; break;
                case SatisfactionLevel.Delectable: chance = .25; break;
            }

            if (player.Meditating)
                chance *= .5;

            if (Utility.RandomDouble() <= chance)
                player.Mana++;
        }

		public virtual bool CanEat( Mobile from )
		{
			return AttemptEat( from, FillFactor, Satisfaction);
		}

		public static bool AttemptEat( Mobile from, int fillFactor, SatisfactionLevel satisfactionLevel )
		{
            if (from.Hunger + fillFactor >= 20)
            {
                from.SendMessage("You are too full to eat that.");
                return false;
            }

            from.Hunger += fillFactor;

            PlayerMobile player = from as PlayerMobile;

            if (player != null)
            
            if (from.Stam < from.StamMax)
                from.Stam += Utility.RandomMinMax(10, 20);

            int iHunger = from.Hunger + fillFactor;
                        
            string satisfiedMessage = "";
            string hungerMessage = "";

            switch (satisfactionLevel)
            {
                case SatisfactionLevel.Paltry: satisfiedMessage = "The food is paltry and barely satisfies.";  break;
                case SatisfactionLevel.Meagre: satisfiedMessage = " The food is meagre and mildly satisfies."; break;
                case SatisfactionLevel.Adequate: satisfiedMessage = "The food is adequate and decently satisfies."; break;
                case SatisfactionLevel.Appetizing: satisfiedMessage = "The food is appetizing and greatly satisfies."; break;
                case SatisfactionLevel.Delectable: satisfiedMessage = "The food is delectable and completely satisfies."; break;
            }

            if (iHunger < 5)
                hungerMessage = "You are still very hungry.";
                
            else if (iHunger < 10)
                hungerMessage = "You are still slightly hungry.";

            else if (iHunger < 15)
                hungerMessage = "You are somewhat full.";

            else if (iHunger < 20)
                hungerMessage = "You are very full.";

            else
                hungerMessage = "You are completely full.";

            from.SendMessage(satisfiedMessage);
            from.SendMessage(hungerMessage);

			return true;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

            //Version 0
            writer.Write(m_Charges);
            writer.Write(m_Poison.Level);
			writer.Write(m_Poisoner);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Charges = reader.ReadInt();
                m_Poison = Poison.GetPoison(reader.ReadInt());
                m_Poisoner = reader.ReadMobile();
            }
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
		[Constructable]
		public RoastPig() : this( 1 )
		{
		}

		[Constructable]
		public RoastPig( int amount ) : base( amount, 0x9BB )
		{
			Weight = 20.0;
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