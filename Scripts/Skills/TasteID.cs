using System;
using Server.Targeting;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using System.Globalization;
using Server.Gumps;

namespace Server.SkillHandlers
{
	public class TasteID
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.TasteID].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse( Mobile from )
		{
            from.Target = new InternalTarget();
            from.SendMessage("What food, potion, or player would you like to inspect.");

			return TimeSpan.FromSeconds( 2.0 );
		}

		[PlayerVendorTarget]
		private class InternalTarget : Target
		{
			public InternalTarget() :  base ( 2, false, TargetFlags.None )
			{
				AllowNonlocal = true;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
                if (targeted is Mobile && from != targeted)
                {
                    from.SendMessage("You can only guess to as their current level of hunger.");
                    return;
                }

                else if (targeted is Mobile && from == targeted)
                {
                    from.SendSound(0x055);

                    from.CloseGump(typeof(HungerGump));
                    from.SendGump(new HungerGump(from));
                }

                else if (targeted is Food)
                {
                    Food food = (Food)targeted;

                    from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.TasteIDCooldown * 1000);

                    if (from.CheckTargetSkill(SkillName.TasteID, food, 0, 100, 1.0))
                    {
                        if (food.Poison != null)
                        {
                            if (food.Poison == Poison.Lesser)
                                from.SendMessage("It appears to be coated in a lesser poison.");

                            else if (food.Poison == Poison.Regular)
                                from.SendMessage("It appears to be coated in a regular poison.");

                            else if (food.Poison == Poison.Greater)
                                from.SendMessage("It appears to be coated in a greater poison.");

                            else if (food.Poison == Poison.Deadly)
                                from.SendMessage("It appears to be coated in a deadly poison.");

                            else if (food.Poison == Poison.Lethal)
                                from.SendMessage("It appears to be coated in a lethal poison.");                            
                        }

                        else
                            from.SendMessage("It appears to be devoid of any form of poison.");                 
                    }

                    else
                        from.SendMessage("You are uncertain as to the safety of this food item.");

                    from.SendSound(0x055);

                    from.CloseGump(typeof(FoodGump));
                    from.SendGump(new FoodGump(from, food));
                }

                else if (targeted is CustomAlchemyPotion)
                {
                    CustomAlchemyPotion customAlchemyPotion = targeted as CustomAlchemyPotion;

                    if (customAlchemyPotion.Identified)
                    {
                        from.SendMessage("That potion's composition has already been determined.");
                        return;
                    }

                    string name = customAlchemyPotion.GetPotionName();

                    switch (customAlchemyPotion.EffectPotency)
                    {
                        case CustomAlchemy.EffectPotencyType.Target:
                            break;

                        case CustomAlchemy.EffectPotencyType.SmallAoE:
                            if (from.Skills.TasteID.Value < 50)
                            {
                                from.SendMessage("You must have at least 50 Taste Identification skill to analyze that potion's composition.");
                                return;
                            }
                        break;

                        case CustomAlchemy.EffectPotencyType.MediumAoE:
                            if (from.Skills.TasteID.Value < 75)
                            {
                                from.SendMessage("You must have at least 75 Taste Identification skill to analyze that potion's composition.");
                                return;
                            }
                        break;

                        case CustomAlchemy.EffectPotencyType.LargeAoE:
                            if (from.Skills.TasteID.Value < 100)
                            {
                                from.SendMessage("You must have at least 100 Taste Identification skill to analyze that potion's composition.");
                                return;
                            }
                        break;
                    }

                    from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.TasteIDCooldown * 1000);

                    if (from.CheckTargetSkill(SkillName.TasteID, 0, 100, 1.0))
                    {
                        customAlchemyPotion.Identified = true;
                        from.SendMessage("You analyze the potion and determine its composition: " + name + ".");

                        from.SendSound(0x5AF);
                    }

                    else
                    {
                        from.SendMessage("You are not certain of the potion's composition.");
                        from.SendSound(0x5AD);
                    }
                }

                else if (targeted is Food)
                {                                      
                }

                else if (targeted is BasePotion)
                {
                    BasePotion potion = (BasePotion)targeted;

                    potion.SendLocalizedMessageTo(from, 502813); // You already know what kind of potion that is.
                    potion.SendLocalizedMessageTo(from, potion.LabelNumber);
                }

                else if (targeted is PotionKeg)
                {
                    PotionKeg keg = (PotionKeg)targeted;

                    if (keg.Held <= 0)
                        keg.SendLocalizedMessageTo(from, 502228); // There is nothing in the keg to taste!

                    else
                    {
                        keg.SendLocalizedMessageTo(from, 502229); // You are already familiar with this keg's contents.
                        keg.SendLocalizedMessageTo(from, keg.LabelNumber);
                    }
                }

                else
                {
                    from.SendMessage("That is not something you can inspect.");
                    return;
                }
			}

			protected override void OnTargetOutOfRange( Mobile from, object targeted )
			{
                from.SendMessage("You are too far away to inspect that.");
                return;
			}
		}
	}

    public class FoodGump : Gump
    {
        public Mobile m_From;
        public Food m_Food;

        public FoodGump(Mobile from, Food food): base(150, 150)
        {
            if (from == null || food == null) return;
            if (from.Deleted || !from.Alive || food.Deleted) return;

            m_From = from;
            m_Food = food;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int textHue = 2036;

            AddPage(0);       

            AddImage(140, 4, 103);
            AddImage(140, 101, 103);
            AddImage(140, 144, 103);
            AddImage(5, 143, 103);
            AddImage(5, 4, 103);
            AddImage(5, 101, 103);
            AddImage(15, 103, 3604, 2052);
            AddImage(140, 104, 3604, 2052);
            AddImage(15, 14, 3604, 2052);
            AddImage(141, 14, 3604, 2052); 
            AddItem(69, 111, 2942);
            AddItem(43, 92, 2944);
            AddItem(24, 77, 2943);
            AddItem(6, 94, 2943);
            AddItem(26, 110, 2944);
            AddItem(51, 128, 2942);
            AddItem(42, 105, 3125);
            AddItem(22, 82, 3126);

            string foodNameText = food.DisplayName;

            if (foodNameText == null)
                foodNameText = "";

            foodNameText = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(foodNameText);

            string satisfactionText = Food.GetSatisfactionText(food.Satisfaction);

            if (satisfactionText == null)
                satisfactionText = "";

            satisfactionText = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(satisfactionText);
            int satisfactionTextHue = Food.GetSatisfactionHue(food.Satisfaction);

            string decayExpiration = "Expires in " + Utility.CreateTimeRemainingString(DateTime.UtcNow, food.DecayExpiration, true, true, true, true, false);

            if (food.DecayExpiration < DateTime.UtcNow)
                decayExpiration = "Expires shortly";

            double hitsRegenChance = Food.GetFoodHitsRegenChance(food.Satisfaction);
            double stamRegenChance = Food.GetFoodStamRegenChance(food.Satisfaction);
            double manaRegenChance = Food.GetFoodManaRegenChance(food.Satisfaction);

            //Guide
            AddButton(18, 15, 2094, 2095, 1, GumpButtonType.Reply, 0);
            AddLabel(16, -3, 149, "Guide");

            //Item
            AddItem(37 + food.IconOffsetX, 95 + food.IconOffsetX, food.IconItemId, food.IconItemHue);

            //Description
            AddLabel(114, 15, 2550, foodNameText);
            AddLabel(112, 35, satisfactionTextHue, satisfactionText);

            if (food.Decays)
                AddLabel(80, 55, 1101, decayExpiration);

            //Properties            
            AddLabel(112, 95, textHue, "Bites Remaining:");
            AddLabel(220, 95, 2603, food.Charges.ToString());            

            AddLabel(145, 115, textHue, "Fill Factor:");
            AddLabel(220, 115, 2603, food.FillFactor.ToString());

            AddLabel(120, 135, textHue, "Stam Regained:");
            AddLabel(220, 135, 2603, food.MinStaminaRegained.ToString() + "-" + food.MaxStaminaRegained.ToString());

            //Regen
            AddLabel(34, 190, 149, "Satisfaction Regen Boost Chances");
            AddLabel(20, 210, textHue, "Hits:");
            AddLabel(55, 210, 2603, Utility.CreateDecimalPercentageString(hitsRegenChance, 1));

            AddLabel(107, 210, textHue, "Stam:");
            AddLabel(147, 210, 2603, Utility.CreateDecimalPercentageString(stamRegenChance, 1));

            AddLabel(193, 210, textHue, "Mana:");
            AddLabel(234, 210, 2603, Utility.CreateDecimalPercentageString(manaRegenChance, 1));
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_From == null) return;

            bool closeGump = true;

            switch (info.ButtonID)
            {
                //Guide
                case 1:
                    closeGump = false;
                break;
            }

            if (!closeGump)
            {
                m_From.CloseGump(typeof(FoodGump));
                m_From.SendGump(new FoodGump(m_From, m_Food));
            }
        }
    }

    public class HungerGump : Gump
    {
        public PlayerMobile m_Player;

        public HungerGump(Mobile from): base(150, 150)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player == null) return;
            if (player.Deleted || !player.Alive) return;

            m_Player = player;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int textHue = 2036;

            AddPage(0);

            AddImage(140, 4, 103);
            AddImage(140, 101, 103);
            AddImage(140, 144, 103);
            AddImage(5, 143, 103);
            AddImage(5, 4, 103);
            AddImage(5, 101, 103);
            AddImage(15, 103, 3604, 2052);
            AddImage(140, 104, 3604, 2052);
            AddImage(15, 14, 3604, 2052);
            AddImage(141, 14, 3604, 2052);
            AddItem(134, 48, 2449);
            AddItem(109, 51, 2542);
            AddItem(113, 77, 2519);
            AddItem(133, 91, 2550);
            AddItem(90, 63, 2548);
            AddItem(140, 97, 2552);
            AddItem(145, 55, 2444);
            AddItem(115, 75, 2496);
            AddItem(131, 41, 3191);
            AddItem(131, 57, 4161);

            double hitsRegenChance = Food.GetFoodHitsRegenChance(m_Player.SatisfactionLevel);
            double stamRegenChance = Food.GetFoodStamRegenChance(m_Player.SatisfactionLevel);
            double manaRegenChance = Food.GetFoodManaRegenChance(m_Player.SatisfactionLevel);

            string satisfactionText = Food.GetPlayerSatisfactionText(player.SatisfactionLevel);
            int satisfactionTextHue = Food.GetSatisfactionHue(m_Player.SatisfactionLevel);

            //Guide
            AddButton(18, 15, 2094, 2095, 1, GumpButtonType.Reply, 0);
            AddLabel(16, -3, 149, "Guide");

            AddLabel(87, 15, 2550, "Your Hunger Status");

            AddLabel(70, 111, textHue, "Current Fill Level:");
            AddLabel(189, 111, 2600, m_Player.Hunger.ToString() + "/" + Food.MaxHunger.ToString());

            AddLabel(69, 131, textHue, "Satisfaction Level:");
            AddLabel(189, 131, satisfactionTextHue, satisfactionText);

            if (m_Player.SatisfactionExpiration > DateTime.UtcNow)
            {
                string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, m_Player.SatisfactionExpiration, true, true, true, true, true);

                AddLabel(40, 151, textHue, "Satisfaction Expires In:");
                AddLabel(189, 151, 2401, timeRemaining);
            }

            //Satisfaction
            AddLabel(34, 190, 149, "Satisfaction Regen Boost Chances");

            AddLabel(20, 210, textHue, "Hits:");
            AddLabel(55, 210, 2603, Utility.CreateDecimalPercentageString(hitsRegenChance, 1));

            AddLabel(107, 210, textHue, "Stam:");
            AddLabel(147, 210, 2603, Utility.CreateDecimalPercentageString(stamRegenChance, 1));

            AddLabel(193, 210, textHue, "Mana:");
            AddLabel(234, 210, 2603, Utility.CreateDecimalPercentageString(manaRegenChance, 1));
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null) return;

            bool closeGump = true;

            switch (info.ButtonID)
            {
                //Guide
                case 1:
                    closeGump = false;
                break;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(HungerGump));
                m_Player.SendGump(new HungerGump(m_Player));
            }
        }
    }
}