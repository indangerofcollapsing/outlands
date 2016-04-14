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

        public virtual string DisplayName { get { return "food"; } }
        public virtual SatisfactionLevelType Satisfaction { get { return SatisfactionLevelType.Paltry; } }   
  
        public virtual int IconItemId { get { return ItemID; } }
        public virtual int IconItemHue { get { return Hue; } }
        public virtual int IconOffsetX { get { return 0; } }
        public virtual int IconOffsetY { get { return 0; } }
          
        public virtual int FillFactor { get { return 6; } }
        public virtual bool IsStackable { get { return true; } }   
        public virtual int MaxCharges { get { return 1; } }
        public virtual double WeightPerCharge { get { return .1; } }

        public virtual bool Decays { get { return false; } }
        public virtual TimeSpan DecayDuration { get { return TimeSpan.FromDays(7); } }

        public virtual int MinStaminaRegained { get { return 10; } }
        public virtual int MaxStaminaRegained { get { return 20; } }

        public static TimeSpan SatisfactionDuration = TimeSpan.FromMinutes(60);

        public static bool AllowInPlayerVsPlayerCombat = true;        

        private int m_Charges = 1;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get { return m_Charges; }
            set
            { 
                m_Charges = value;
                Weight = WeightPerCharge * m_Charges;
            }
        }

        private DateTime m_DecayExpiration = DateTime.UtcNow + TimeSpan.FromDays(7);
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

        public Food(int itemID): this(1, itemID)
        {
            Stackable = IsStackable;

            Charges = MaxCharges;
            DecayExpiration = DateTime.UtcNow + DecayDuration;
        }

        public Food(int amount, int itemID): base(itemID)
        {
            Stackable = IsStackable;
            Amount = amount;

            Charges = MaxCharges;
            DecayExpiration = DateTime.UtcNow + DecayDuration;
        }

        public Food(Serial serial): base(serial)
        {
        }
        
        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, DisplayName);

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

		public override void OnDoubleClick( Mobile from )
		{
			if ( !Movable )
				return;

			if ( from.InRange( this.GetWorldLocation(), 1 ) )			
				Eat( from );			
		}

        public static string GetSatisfactionText(SatisfactionLevelType satisfactionLevel)
        {
            switch (satisfactionLevel)
            {
                case SatisfactionLevelType.Paltry: return "paltry"; break;
                case SatisfactionLevelType.Meagre: return "meagre"; break;
                case SatisfactionLevelType.Adequate: return "adequate"; break;
                case SatisfactionLevelType.Appetizing: return "appetizing"; break;
                case SatisfactionLevelType.Delectable: return "delectable"; break;
            }

            return "unappetizing";
        }


        public static int GetSatisfactionHue(SatisfactionLevelType satisfactionLevel)
        {
            switch (satisfactionLevel)
            {
                case SatisfactionLevelType.Paltry: return 2401; break;
                case SatisfactionLevelType.Meagre: return 2655; break;
                case SatisfactionLevelType.Adequate: return 2599; break;
                case SatisfactionLevelType.Appetizing: return 0x3F; break;
                case SatisfactionLevelType.Delectable: return 2607; break;
            }

            return 2401;
        }

        public static string GetPlayerSatisfactionText(SatisfactionLevelType satisfactionLevel)
        {
            switch (satisfactionLevel)
            {
                case SatisfactionLevelType.Paltry: return "Barely"; break;
                case SatisfactionLevelType.Meagre: return "Mildly"; break;
                case SatisfactionLevelType.Adequate: return "Decently"; break;
                case SatisfactionLevelType.Appetizing: return "Greatly"; break;
                case SatisfactionLevelType.Delectable: return "Completely"; break;
            }

            return "None";
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

        public static double GetFoodHitsRegenChance(SatisfactionLevelType satisfaction)
        {
            switch (satisfaction)
            {
                case SatisfactionLevelType.Paltry: return.1; break;
                case SatisfactionLevelType.Meagre: return .2; break;
                case SatisfactionLevelType.Adequate: return .3; break;
                case SatisfactionLevelType.Appetizing: return .4; break;
                case SatisfactionLevelType.Delectable: return .5; break;
            }

            return 0;
        }

        public static double GetFoodStamRegenChance(SatisfactionLevelType satisfaction)
        {
            switch (satisfaction)
            {
                case SatisfactionLevelType.Paltry: return .1; break;
                case SatisfactionLevelType.Meagre: return .2; break;
                case SatisfactionLevelType.Adequate: return .3; break;
                case SatisfactionLevelType.Appetizing: return .4; break;
                case SatisfactionLevelType.Delectable: return .5; break;
            }

            return 0;
        }

        public static double GetFoodManaRegenChance(SatisfactionLevelType satisfaction)
        {
            switch (satisfaction)
            {
                case SatisfactionLevelType.Paltry: return .05; break;
                case SatisfactionLevelType.Meagre: return .1; break;
                case SatisfactionLevelType.Adequate: return .15; break;
                case SatisfactionLevelType.Appetizing: return .2; break;
                case SatisfactionLevelType.Delectable: return .25; break;
            }

            return 0;
        }

        public static void CheckFoodHitsRegen(PlayerMobile player)
        {
            if (player.Region is UOACZRegion) return;
            if (!AllowInPlayerVsPlayerCombat && player.RecentlyInPlayerCombat) return;
            if (player.SatisfactionExpiration <= DateTime.UtcNow) return;

            double chance = GetFoodHitsRegenChance(player.SatisfactionLevel);

            if (Utility.RandomDouble() <= chance)
                player.Hits++;
        }

        public static void CheckFoodStamRegen(PlayerMobile player)
        {
            if (player.Region is UOACZRegion) return;
            if (!AllowInPlayerVsPlayerCombat && player.RecentlyInPlayerCombat) return;
            if (player.SatisfactionExpiration <= DateTime.UtcNow) return;

            double chance = GetFoodStamRegenChance(player.SatisfactionLevel);

            if (Utility.RandomDouble() <= chance)
                player.Stam++;
        }

        public static void CheckFoodManaRegen(PlayerMobile player)
        {
            if (player.Region is UOACZRegion) return;
            if (!AllowInPlayerVsPlayerCombat && player.RecentlyInPlayerCombat) return;
            if (player.SatisfactionExpiration <= DateTime.UtcNow) return;

            double chance = GetFoodManaRegenChance(player.SatisfactionLevel);

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
            writer.Write(m_DecayExpiration);

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
                m_DecayExpiration = reader.ReadDateTime();

                int poisonLevel = reader.ReadInt();
                if (poisonLevel > -1)
                    m_Poison = Poison.GetPoison(poisonLevel);
            }
		}
	}

    public class FoodDecayTimer : Timer
    {
        public static void Initialize()
        {
            new FoodDecayTimer().Start();
        }

        public FoodDecayTimer()
            : base(TimeSpan.FromMinutes(60), TimeSpan.FromMinutes(60))
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
}