using System;
using Server.Items;
using Server.Multis;
using Server.Multis.Deeds;
using Server.Custom.Items;
using Server.Multis;
using Server.Custom;

namespace Server.Engines.Craft
{
    public class UOACZDefCarpentry : CraftSystem
    {
        #region Base

        public override SkillName MainSkill
        {
            get { return SkillName.Carpentry; }
        }

        public override int GumpTitleNumber
        {
            get { return 1044004; } // <CENTER>CARPENTRY MENU</CENTER>
        }

        public static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new UOACZDefCarpentry();

                return m_CraftSystem;
            }
        }

        public override double GetChanceAtMin(CraftItem item)
        {
            return 0.0;
        }

        public UOACZDefCarpentry()
            : base(1, 1, 1.25)// base( 1, 1, 3.0 )
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
            // no animation
            //if ( from.Body.Type == BodyType.Human && !from.Mounted )
            //	from.Animate( 9, 5, 1, true, false, 0 );

            from.PlaySound(0x23D);
        }

        public override bool ConsumeOnFailure(Mobile from, Type resourceType, CraftItem craftItem)
        {
            if (resourceType == typeof(ShipParts))
            {
                from.Backpack.ConsumeTotal(resourceType, 1);
                return false;
            }

            if (typeof(BaseHousePlans).IsAssignableFrom(resourceType))
            {
                if (from.Backpack.ConsumeTotal(resourceType, 1))
                    UnfilledHousePlans.Create(from, resourceType);

                return false;
            }

            else if
                (
                resourceType == typeof(SmallStoneTempleHousePlans) ||
                resourceType == typeof(ArbitersEstateHousePlans) ||
                resourceType == typeof(SandstoneSpaHousePlans) ||
                resourceType == typeof(MagistratesHousePlans) ||
                resourceType == typeof(BalconyHousePlans)
                )

                return false;

            return base.ConsumeOnFailure(from, resourceType, craftItem);
        }

        public override int PlayEndingEffect(Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item)
        {
            if (toolBroken)
                from.SendLocalizedMessage(1044038); // You have worn out your tool

            if (failed)
            {
                if (lostMaterial)
                    return 1044043; // You failed to create the item, and some of your materials are lost.
                else
                    return 1044157; // You failed to create the item, but no materials were lost.
            }

            else
            {
                if (quality == 0)
                    return 502785; // You were barely able to make this item.  It's quality is below average.

                else if (makersMark && quality == 2)
                {
                    //Player Enhancement Customization: Artisan
                    if (PlayerEnhancementPersistance.IsCustomizationEntryActive(from, CustomizationType.Artisan))
                        CustomizationAbilities.Artisan(from, from.Location, from.Map);

                    return 1044156; // You create an exceptional quality item and affix your maker's mark.
                }

                else if (quality == 2)
                    return 1044155; // You create an exceptional quality item.

                else
                    return 1044154; // You create the item.
            }
        }

        #endregion

        public override void InitCraftList()
        {
            int index = -1;

            //Utility
            AddCraft(1, typeof(Kindling), "Utility", "Kindling", 0, 0, typeof(Log), "Log", 1, "You do not have the logs neccessary in order to craft that.");
            AddCraft(1, typeof(UOACZBowl), "Utility", "Bowl", 5, 30, typeof(Log), "Log", 5, "You do not have the logs neccessary in order to craft that.");
            AddCraft(1, typeof(UOACZTorch), "Utility", "Torch", 5, 30, typeof(Log), "Log", 5, "You do not have the logs neccessary in order to craft that.");
            AddCraft(1, typeof(UOACZTub), "Utility", "Empty Water Tub", 10, 35, typeof(Log), "Log", 20, "You do not have the logs neccessary in order to craft that.");
            AddCraft(1, typeof(UOACZFishingPole), "Utility", "Fishing Pole", 10, 35, typeof(Log), "Log", 15, "You do not have the logs neccessary in order to craft that.");
            AddCraft(1, typeof(CampingFirepit), "Utility", "Camping Firepit", 25, 50, typeof(Log), "Log", 75, "You do not have the logs neccessary required in order to craft that.");

            //Weapons
            AddCraft(1, typeof(Club), "Weapons and Armor", "Club", 10, 35, typeof(Log), "Log", 5, "You do not have the logs neccessary in order to craft that.");
            AddCraft(1, typeof(WoodenShield), "Weapons and Armor", "Wooden Shield", 10, 35, typeof(Log), "Log", 10, "You do not have the logs neccessary in order to craft that.");
            AddCraft(1, typeof(UOACZWoodenSword), "Weapons and Armor", "Wooden Sword", 15, 40, typeof(Log), "Log", 10, "You do not have the logs neccessary in order to craft that.");
            AddCraft(1, typeof(WoodenKiteShield), "Weapons and Armor", "Wooden Kite Shield", 15, 40, typeof(Log), "Log", 15, "You do not have the logs neccessary in order to craft that.");
            AddCraft(1, typeof(QuarterStaff), "Weapons and Armor", "Quarterstaff", 20, 45, typeof(Log), "Log", 10, "You do not have the logs neccessary in order to craft that.");
            AddCraft(1, typeof(GnarledStaff), "Weapons and Armor", "Gnarled Staff", 20, 45, typeof(Log), "Log", 10, "You do not have the logs neccessary in order to craft that.");
            AddCraft(1, typeof(BlackStaff), "Weapons and Armor", "Black Staff", 20, 45, typeof(Log), "Log", 10, "You do not have the logs neccessary in order to craft that.");
                  
            MarkOption = true;
            Repair = true;
        }
    }
}