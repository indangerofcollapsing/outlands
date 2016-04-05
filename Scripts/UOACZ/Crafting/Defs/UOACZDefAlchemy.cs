using System;
using Server.Items;

using Server.Mobiles;

namespace Server.Engines.Craft
{
    public class UOACZDefAlchemy : CraftSystem
    {
        #region Base 

        public override SkillName MainSkill
        {
            get { return SkillName.Alchemy; }
        }

        public override int GumpTitleNumber
        {
            get { return 1044001; } // <CENTER>ALCHEMY MENU</CENTER>
        }

        public static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new UOACZDefAlchemy();

                return m_CraftSystem;
            }
        }

        public override double GetChanceAtMin(CraftItem item)
        {
            return 0.0; // 0%
        }

        public UOACZDefAlchemy()
            : base(1, 1, 1.25)// base( 1, 1, 3.1 )
        {
        }

        public override int CanCraft(Mobile from, BaseTool tool, Type itemType)
        {
            if (tool == null || tool.Deleted || tool.UsesRemaining < 0)
                return 1044038; // You have worn out your tool!

            else if (!BaseTool.CheckAccessible(tool, from))
                return 1044263; // The tool must be on your person to use.

            return 0;
        }

        public override void PlayCraftEffect(Mobile from)
        {
            from.PlaySound(0x242);
        }

        public static Type typeofPotion = typeof(BasePotion);

        public static bool IsPotion(Type type)
        {
            return typeofPotion.IsAssignableFrom(type);
        }

        public override int PlayEndingEffect(Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item)
        {
            if (toolBroken)
                from.SendLocalizedMessage(1044038); // You have worn out your tool

            if (failed)
            {
                if (IsPotion(item.ItemType))
                {
                    from.AddToBackpack(new Bottle());
                    return 500287; // You fail to create a useful potion.
                }
                else
                {
                    return 1044043; // You failed to create the item, and some of your materials are lost.
                }
            }
            else
            {
                from.PlaySound(0x240); // Sound of a filling bottle

                if (IsPotion(item.ItemType))
                {
                    if (quality == -1)
                        return 1048136; // You create the potion and pour it into a keg.
                    else
                        return 500279; // You pour the potion into a bottle...
                }
                else
                {
                    return 1044154; // You create the item.
                }
            }
        }

    #endregion

        public override void InitCraftList()
        {
            int index = -1;

            //Purification
            index = AddCraft(1, typeof(UOACZRawDrumstick), "Purification", "Raw Drumstick", 5, 30, typeof(UOACZCorruptedRawDrumstick), "Corrupted Raw Drumstick", 1, "You do not have the neccessary corrupted raw drumstick required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZRawHam), "Purification", "Raw Ham", 5, 30, typeof(UOACZCorruptedRawHam), "Corrupted Raw Ham", 1, "You do not have the neccessary corrupted raw ham required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZRawMeatScraps), "Purification", "Raw Meat Scraps", 5, 30, typeof(UOACZCorruptedRawMeatScraps), "Corrupted Raw Meat Scraps", 1, "You do not have the neccessary corrupted raw meat scraps required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZRawMeatShank), "Purification", "Raw Meat Shank", 5, 30, typeof(UOACZCorruptedRawMeatShank), "Corrupted Raw Meat Shank", 1, "You do not have the neccessary corrupted raw meat shank required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZRawSteak), "Purification", "Cooked Steak", 5, 30, typeof(UOACZCookedCorruptedSteak), "Cooked Corrupted Steak", 1, "You do not have the neccessary cooked corrupted steak required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZCookedDrumstick), "Purification", "Cooked Drumstick", 5, 30, typeof(UOACZCookedCorruptedDrumstick), "Cooked Corrupted Drumstick", 1, "You do not have the neccessary cooked corrupted drumstick required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZCookedHam), "Purification", "Cooked Ham", 5, 30, typeof(UOACZCookedCorruptedHam), "Cooked Corrupted Ham", 1, "You do not have the neccessary cooked corrupted ham required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZCookedMeatScraps), "Purification", "Cooked Meat Scraps", 5, 30, typeof(UOACZCookedCorruptedMeatScraps), "Cooked Corrupted Meat Scraps", 1, "You do not have the neccessary cooked corrupted meat scraps required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZCookedMeatShank), "Purification", "Cooked Meat Shank", 5, 30, typeof(UOACZCookedCorruptedMeatShank), "Cooked Corrupted Meat Shank", 1, "You do not have the neccessary cooked corrupted meat shank required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZCookedSteak), "Purification", "Cooked Steak", 5, 30, typeof(UOACZCookedCorruptedSteak), "Cooked Corrupted Steak", 1, "You do not have the neccessary cooked corrupted steak required to cook this.");
            SetNeedHeat(index, true);

            //Restoration
            index = AddCraft(1, typeof(HealPotion), "Restoration", "Heal Potion", 5, 30, typeof(Ginseng), 1044356, 1, 1044364);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(1, typeof(GreaterHealPotion), "Restoration", "Greater Heal Potion", 15, 40, typeof(Ginseng), 1044356, 2, 1044364);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(1, typeof(CurePotion), "Restoration", "Cure Potion", 5, 30, typeof(Garlic), 1044355, 1, 1044363);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(1, typeof(GreaterCurePotion), "Restoration", "Greater Cure Potion", 15, 40, typeof(Garlic), 1044355, 2, 1044363);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(1, typeof(RefreshPotion), "Restoration", "Refresh Potion", 5, 30, typeof(BlackPearl), 1044353, 1, 1044361);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(1, typeof(TotalRefreshPotion), "Restoration", "Total Refresh Potion", 15, 40, typeof(BlackPearl), 1044353, 2, 1044361);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            //Enhancement
            index = AddCraft(1, typeof(StrengthPotion), "Enhancement", "Strength Potion", 15, 40, typeof(MandrakeRoot), 1044357, 1, 1044365);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(1, typeof(GreaterStrengthPotion), "Enhancement", "Greater Strength Potion", 25, 50, typeof(MandrakeRoot), 1044357, 2, 1044365);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(1, typeof(AgilityPotion), "Enhancement", "Agility Potion", 15, 40, typeof(Bloodmoss), 1044354, 1, 1044362);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(1, typeof(GreaterAgilityPotion), "Enhancement", "Greater Agility Potion", 25, 50, typeof(Bloodmoss), 1044354, 2, 1044362);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            //NightShade
            index = AddCraft(1, typeof(UOACZLesserTruestrikePotion), "Enhancement", "Lesser Truestrike Potion", 15, 40, typeof(Nightshade), "Nightshade", 1, "You do not have the neccessary nightshade to craft that.");
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(1, typeof(UOACZGreaterTruestrikePotion), "Enhancement", "Greater Truestrike Potion", 25, 50, typeof(Nightshade), "Nightshade", 2, "You do not have the neccessary nightshade to craft that.");
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            //Utility
            index = AddCraft(1, typeof(UOACZLesserConfusionPotion), "Utility", "Lesser Confusion Potion", 15, 40, typeof(SpidersSilk), "Spider's Silk", 1, "You do not have the neccessary spider's silk to craft that.");
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(1, typeof(UOACZGreaterConfusionPotion), "Utility", "Greater Confusion Potion", 25, 50, typeof(SpidersSilk), "Spider's Silk", 2, "You do not have the neccessary spider's silk to craft that.");
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(1, typeof(UOACZOilFlask), "Utility", "Oil Flask", 5, 30, typeof(SulfurousAsh), 1044359, 2, 1044367);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);
        }
    }
}