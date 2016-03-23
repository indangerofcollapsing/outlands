using System;
using Server.Items;
using Server.Custom;

namespace Server.Engines.Craft
{
    public class DefBowFletching : CraftSystem
    {
        public override SkillName MainSkill
        {
            get { return SkillName.Fletching; }
        }

        public override int GumpTitleNumber
        {
            get { return 1044006; } // <CENTER>BOWCRAFT AND FLETCHING MENU</CENTER>
        }

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new DefBowFletching();

                return m_CraftSystem;
            }
        }

        public override double GetChanceAtMin(CraftItem item)
        {
            return 0.5; // 50%
        }

        private DefBowFletching()
            : base(1, 1, 1.25)// base( 1, 2, 1.7 )
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
            //	from.Animate( 33, 5, 1, true, false, 0 );

            from.PlaySound(0x55);
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

        public override CraftECA ECA { get { return CraftECA.FiftyPercentChanceMinusTenPercent; } }

        public override void InitCraftList()
        {
            int index = -1;

            // Materials
            index = AddCraft(typeof(Kindling), 1044457, 1023553, 0.0, 00.0, typeof(Board), 1044041, 1, 1044351);
            SetUseAllRes(index, true);

            index = AddCraft(typeof(Shaft), 1044457, 1027124, 0.0, 40.0, typeof(Board), 1044041, 1, 1044351);
            SetUseAllRes(index, true);

            // Ammunition
            index = AddCraft(typeof(Arrow), 1044565, 1023903, 0.0, 40.0, typeof(Shaft), 1044560, 1, 1044561);
            AddRes(index, typeof(Feather), 1044562, 1, 1044563);
            SetUseAllRes(index, true);

            index = AddCraft(typeof(Bolt), 1044565, 1027163, 0.0, 40.0, typeof(Shaft), 1044560, 1, 1044561);
            AddRes(index, typeof(Feather), 1044562, 1, 1044563);
            SetUseAllRes(index, true);
            
            // Weapons            
            AddCraft(typeof(Bow), 1044566, 1025042, 30.0, 70.0, typeof(Board), 1044041, 7, 1044351);
            AddCraft(typeof(Crossbow), 1044566, 1023919, 60.0, 100.0, typeof(Board), 1044041, 7, 1044351);
            AddCraft(typeof(HeavyCrossbow), 1044566, 1025117, 80.0, 120.0, typeof(Board), 1044041, 10, 1044351);
                  
            //Wood Hue Plating
            AddCraft(typeof(RegularWoodHuePlating), "Wood Hue Plating", "regular wood hue plating", 70, 120, typeof(Board), "board", 25, "You do not have the neccesary boards to construct that.");

            index = AddCraft(typeof(OakWoodHuePlating), "Wood Hue Plating", "oak wood hue plating", 70, 120, typeof(OakBoard), "oak board", 25, "You do not have the neccesary boards to construct that.");
            AddRes(index, typeof(Ghostweed), "ghostweed", 5, "You do not have the neccessary crafting components to construct that.");

            index = AddCraft(typeof(AshWoodHuePlating), "Wood Hue Plating", "ash wood hue plating", 70, 120, typeof(AshBoard), "ash board", 25, "You do not have the neccesary boards to construct that.");
            AddRes(index, typeof(Snakeskin), "creepervine", 5, "You do not have the neccessary crafting components to construct that.");

            index = AddCraft(typeof(YewWoodHuePlating), "Wood Hue Plating", "yew wood hue plating", 70, 120, typeof(YewBoard), "yew board", 25, "You do not have the neccesary boards to construct that.");
            AddRes(index, typeof(Quartzstone), "quartzstone", 5, "You do not have the neccessary crafting components to construct that.");

            index = AddCraft(typeof(HeartWoodHuePlating), "Wood Hue Plating", "heartwood hue plating", 70, 120, typeof(HeartwoodBoard), "heartwood board", 25, "You do not have the neccesary boards to construct that.");
            AddRes(index, typeof(Creepervine), "creepervine", 5, "You do not have the neccessary crafting components to construct that.");

            index = AddCraft(typeof(BloodWoodHuePlating), "Wood Hue Plating", "bloodwood hue plating", 70, 120, typeof(BloodwoodBoard), "bloodwood board", 25, "You do not have the neccesary boards to construct that.");
            AddRes(index, typeof(FireEssence), "fire essence", 5, "You do not have the neccessary crafting components to construct that.");

            index = AddCraft(typeof(FrostWoodHuePlating), "Wood Hue Plating", "frostwood hue plating", 70, 120, typeof(FrostwoodBoard), "frostwood board", 25, "You do not have the neccesary boards to construct that.");
            AddRes(index, typeof(BluecapMushroom), "bluecap mushroom", 5, "You do not have the neccessary crafting components to construct that.");

            index = AddCraft(typeof(BowcraftDungeonMould), "Other", "bowcraft dungeon mould", 90, 105, typeof(Board), "Board", 250, "You do not have the neccessary boards to construct that.");
            AddRes(index, typeof(Creepervine), "Creepervine", 3, "You do not have the crafting components required to make this.");
            AddRes(index, typeof(Ghostweed), "Ghostweed", 3, "You do not have the crafting components required to make this.");
            AddRes(index, typeof(Snakeskin), "Snakeskin", 3, "You do not have the crafting components required to make this.");
            //AddRes(index, typeof(BluecapMushroom), "Bluecap Mushroom", 3, "You do not have the crafting components required to make this.");

            Recycle = true;
            MarkOption = true;
            Repair = true;

            SetSubRes(typeof(Board), 1072643);

            // Add every material you want the player to be able to choose from
            // This will override the overridable material	TODO: Verify the required skill amount
            AddSubRes(typeof(Board), 1072643, 00.0, 1044041, 1072652);
            AddSubRes(typeof(OakBoard), 1072644, 65.0, 1044041, 1072652);
            AddSubRes(typeof(AshBoard), 1072645, 80.0, 1044041, 1072652);
            AddSubRes(typeof(YewBoard), 1072646, 95.0, 1044041, 1072652);
            AddSubRes(typeof(HeartwoodBoard), 1072647, 100.0, 1044041, 1072652);
            AddSubRes(typeof(BloodwoodBoard), 1072648, 100.0, 1044041, 1072652);
            AddSubRes(typeof(FrostwoodBoard), 1072649, 100.0, 1044041, 1072652);
        }
    }
}