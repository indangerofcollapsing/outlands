using System;
using Server.Items;
using Server.Multis;
using Server.Custom.Pirates;
using Server.Multis.Deeds;
using Server.Custom.Items;
using Server.Multis;
using Server.Custom;

namespace Server.Engines.Craft
{
    public class DefCarpentry : CraftSystem
    {
        public override SkillName MainSkill
        {
            get { return SkillName.Carpentry; }
        }

        public override int GumpTitleNumber
        {
            get { return 1044004; } // <CENTER>CARPENTRY MENU</CENTER>
        }

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new DefCarpentry();

                return m_CraftSystem;
            }
        }

        public override double GetChanceAtMin(CraftItem item)
        {
            return 0.5; // 50%
        }

        private DefCarpentry()
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

        public override void InitCraftList()
        {
            int index = -1;

            //Fletching
            index = AddCraft(typeof(Kindling), "Fletching", "Kindling", 0.0, 00.0, typeof(Board), 1044041, 1, 1044351);
            SetUseAllRes(index, true);

            index = AddCraft(typeof(Shaft), "Fletching", "Shaft", 0.0, 40.0, typeof(Board), 1044041, 1, 1044351);
            SetUseAllRes(index, true);

            index = AddCraft(typeof(Arrow), "Fletching", "Arrow", 0.0, 40.0, typeof(Shaft), 1044560, 1, 1044561);
            AddRes(index, typeof(Feather), 1044562, 1, 1044563);
            SetUseAllRes(index, true);

            index = AddCraft(typeof(Bolt), "Fletching", "Bolt", 0.0, 40.0, typeof(Shaft), 1044560, 1, 1044561);
            AddRes(index, typeof(Feather), 1044562, 1, 1044563);
            SetUseAllRes(index, true);

            AddCraft(typeof(Bow), "Fletching", "Bow", 30.0, 70.0, typeof(Board), 1044041, 7, 1044351);
            AddCraft(typeof(Crossbow), "Fletching", "Crossbow", 60.0, 100.0, typeof(Board), 1044041, 7, 1044351);
            AddCraft(typeof(HeavyCrossbow), "Fletching", "Heavy Crossbow", 80.0, 120.0, typeof(Board), 1044041, 10, 1044351);

            //Weapons and Shields
            AddCraft(typeof(Club), "Weapons and Shields", "Club", 65, 100, typeof(Board), 1044041, 10, 1044351);
            AddCraft(typeof(ShepherdsCrook), "Weapons and Shields", "Shepherds Crook", 78.9, 103.9, typeof(Board), 1044041, 7, 1044351);
            AddCraft(typeof(QuarterStaff), "Weapons and Shields", "Quarter Staff", 73.6, 98.6, typeof(Board), 1044041, 6, 1044351);
            AddCraft(typeof(GnarledStaff), "Weapons and Shields", "Gnarled Staff", 78.9, 103.9, typeof(Board), 1044041, 7, 1044351);
            AddCraft(typeof(BlackStaff), "Weapons and Shields", "Black Staff", 81.5, 105.0, typeof(Board), 1044041, 9, 1044351);
            AddCraft(typeof(WoodenShield), "Weapons and Shields", "Wooden Shield", 52.6, 77.6, typeof(Board), 1044041, 9, 1044351);
            AddCraft(typeof(WoodenKiteShield), "Weapons and Shields", "Wooden Kite Shield", 75, 100, typeof(Board), 1044041, 12, 1044351);

            //Instruments
            index = AddCraft(typeof(Harp), "Instruments", "Harp", 78.9, 103.9, typeof(Board), 1044041, 35, 1044351);
            AddRes(index, typeof(Cloth), 1044286, 15, 1044287);

            index = AddCraft(typeof(LapHarp), "Instruments", "Lap Harp", 63.1, 88.1, typeof(Board), 1044041, 20, 1044351);
            AddRes(index, typeof(Cloth), 1044286, 10, 1044287);

            index = AddCraft(typeof(Drums), "Instruments", "Drums", 57.8, 82.8, typeof(Board), 1044041, 20, 1044351);
            AddRes(index, typeof(Cloth), 1044286, 10, 1044287);

            index = AddCraft(typeof(Lute), "Instruments", "Lute", 68.4, 93.4, typeof(Board), 1044041, 25, 1044351);
            AddRes(index, typeof(Cloth), 1044286, 10, 1044287);

            index = AddCraft(typeof(Tambourine), "Instruments", "Tambourine", 57.8, 82.8, typeof(Board), 1044041, 15, 1044351);
            AddRes(index, typeof(Cloth), 1044286, 10, 1044287);

            index = AddCraft(typeof(TambourineTassel), "Instruments", "Tassled Tambourine", 57.8, 82.8, typeof(Board), 1044041, 15, 1044351);
            AddRes(index, typeof(Cloth), 1044286, 15, 1044287);

            AddCraft(typeof(BambooFlute), "Instruments", "Bamboo Flute", 80.0, 105.0, typeof(Board), 1044041, 15, 1044351);

            // Furniture
            AddCraft(typeof(FootStool), "Furniture", "Foot Stool", 11.0, 36.0, typeof(Board), 1044041, 9, 1044351);
            AddCraft(typeof(Stool), "Furniture", "Stool", 11.0, 36.0, typeof(Board), 1044041, 9, 1044351);
            AddCraft(typeof(BambooChair), "Furniture", "Bamboo Chair", 21.0, 46.0, typeof(Board), 1044041, 13, 1044351);
            AddCraft(typeof(WoodenChair), "Furniture", "Wooden Chair", 21.0, 46.0, typeof(Board), 1044041, 13, 1044351);
            AddCraft(typeof(FancyWoodenChairCushion), "Furniture", "Fancy Wooden Chair Cushion", 42.1, 67.1, typeof(Board), 1044041, 15, 1044351);
            AddCraft(typeof(WoodenChairCushion), "Furniture", "Wooden Chair Cushion", 42.1, 67.1, typeof(Board), 1044041, 13, 1044351);
            AddCraft(typeof(WoodenBench), "Furniture", "Wooden Bench", 52.6, 77.6, typeof(Board), 1044041, 17, 1044351);
            AddCraft(typeof(WoodenThrone), "Furniture", "Wooden Throne", 52.6, 77.6, typeof(Board), 1044041, 17, 1044351);
            AddCraft(typeof(Throne), "Furniture", "Throne", 73.6, 98.6, typeof(Board), 1044041, 19, 1044351);
            AddCraft(typeof(Nightstand), "Furniture", "Nightstand", 42.1, 67.1, typeof(Board), 1044041, 17, 1044351);
            AddCraft(typeof(WritingTable), "Furniture", "Writing Table", 63.1, 88.1, typeof(Board), 1044041, 17, 1044351);
            AddCraft(typeof(YewWoodTable), "Furniture", "Yew Wood Table", 63.1, 88.1, typeof(Board), 1044041, 23, 1044351);
            AddCraft(typeof(LargeTable), "Furniture", "Large Table", 84.2, 109.2, typeof(Board), 1044041, 27, 1044351);

            //Containers
            index = AddCraft(typeof(Keg), "Containers", "Keg", 57.8, 82.8, typeof(BarrelStaves), 1044288, 3, 1044253);
            AddRes(index, typeof(BarrelHoops), 1044289, 1, 1044253);
            AddRes(index, typeof(BarrelLid), 1044251, 1, 1044253);

            AddCraft(typeof(WoodenBox), "Containers", "Wooden Box", 21.0, 46.0, typeof(Board), 1044041, 10, 1044351);
            AddCraft(typeof(SmallCrate), "Containers", "Small Crate", 10.0, 35.0, typeof(Board), 1044041, 8, 1044351);
            AddCraft(typeof(MediumCrate), "Containers", "Medium Crate", 31.0, 56.0, typeof(Board), 1044041, 15, 1044351);
            AddCraft(typeof(LargeCrate), "Containers", "Large Crate", 47.3, 72.3, typeof(Board), 1044041, 18, 1044351);
            AddCraft(typeof(WoodenChest), "Containers", "Wooden Chest", 73.6, 98.6, typeof(Board), 1044041, 20, 1044351);
            AddCraft(typeof(EmptyBookcase), "Containers", "Empty Bookcase", 31.5, 56.5, typeof(Board), 1044041, 25, 1044351);
            AddCraft(typeof(Armoire), "Containers", "Armoire", 84.2, 109.2, typeof(Board), 1044041, 35, 1044351);
            AddCraft(typeof(FancyArmoire), "Containers", "Fancy Armoire", 84.2, 109.2, typeof(Board), 1044041, 35, 1044351);

            //Add-Ons
            AddCraft(typeof(DartBoardSouthDeed), "Add-Ons", "Dartboard South Deed", 15.7, 40.7, typeof(Board), 1044041, 5, 1044351);
            AddCraft(typeof(DartBoardEastDeed), "Add-Ons", "Dartboard East Deed", 15.7, 40.7, typeof(Board), 1044041, 5, 1044351);
            AddCraft(typeof(BallotBoxDeed), "Add-Ons", "Ballot Box Deed", 47.3, 72.3, typeof(Board), 1044041, 5, 1044351);

            index = AddCraft(typeof(SmallBedSouthDeed), "Add-Ons", "Small Bed South Deed", 94.7, 119.8, typeof(Board), 1044041, 100, 1044351);
            AddSkill(index, SkillName.Tailoring, 75.0, 80.0);
            AddRes(index, typeof(Cloth), 1044286, 100, 1044287);

            index = AddCraft(typeof(SmallBedEastDeed), "Add-Ons", "Small Bed East Deed", 94.7, 119.8, typeof(Board), 1044041, 100, 1044351);
            AddSkill(index, SkillName.Tailoring, 75.0, 80.0);
            AddRes(index, typeof(Cloth), 1044286, 100, 1044287);

            index = AddCraft(typeof(LargeBedSouthDeed), "Add-Ons", "Large Bed South Deed", 94.7, 119.8, typeof(Board), 1044041, 150, 1044351);
            AddSkill(index, SkillName.Tailoring, 75.0, 80.0);
            AddRes(index, typeof(Cloth), 1044286, 150, 1044287);

            index = AddCraft(typeof(LargeBedEastDeed), "Add-Ons", "Large Bed East Deed", 94.7, 119.8, typeof(Board), 1044041, 150, 1044351);
            AddSkill(index, SkillName.Tailoring, 75.0, 80.0);
            AddRes(index, typeof(Cloth), 1044286, 150, 1044287);

            index = AddCraft(typeof(PentagramDeed), "Add-Ons", "Pentagram Deed", 100.0, 125.0, typeof(Board), 1044041, 100, 1044351);
            AddSkill(index, SkillName.Magery, 75.0, 80.0);
            AddRes(index, typeof(IronIngot), 1044036, 40, 1044037);

            index = AddCraft(typeof(AbbatoirDeed), "Add-Ons", "Abbatoir Deed", 100.0, 125.0, typeof(Board), 1044041, 100, 1044351);
            AddSkill(index, SkillName.Magery, 50.0, 55.0);
            AddRes(index, typeof(IronIngot), 1044036, 40, 1044037);

            index = AddCraft(typeof(TrainingDummyEastDeed), "Add-Ons", "Training Dummy East Deed", 68.4, 93.4, typeof(Board), 1044041, 55, 1044351);
            AddSkill(index, SkillName.Tailoring, 50.0, 55.0);
            AddRes(index, typeof(Cloth), 1044286, 60, 1044287);

            index = AddCraft(typeof(TrainingDummySouthDeed), "Add-Ons", "Training Dummy South Deed", 68.4, 93.4, typeof(Board), 1044041, 55, 1044351);
            AddSkill(index, SkillName.Tailoring, 50.0, 55.0);
            AddRes(index, typeof(Cloth), 1044286, 60, 1044287);

            index = AddCraft(typeof(PickpocketDipEastDeed), "Add-Ons", "Pickpocket Dip East Deed", 73.6, 98.6, typeof(Board), 1044041, 65, 1044351);
            AddSkill(index, SkillName.Tailoring, 50.0, 55.0);
            AddRes(index, typeof(Cloth), 1044286, 60, 1044287);

            index = AddCraft(typeof(PickpocketDipSouthDeed), "Add-Ons", "Pickpocket Dip South Deed", 73.6, 98.6, typeof(Board), 1044041, 65, 1044351);
            AddSkill(index, SkillName.Tailoring, 50.0, 55.0);
            AddRes(index, typeof(Cloth), 1044286, 60, 1044287);

            index = AddCraft(typeof(SmallForgeDeed), "Add-Ons", "Small Forge Deed", 73.6, 98.6, typeof(Board), 1044041, 5, 1044351);
            AddSkill(index, SkillName.Blacksmith, 75.0, 80.0);
            AddRes(index, typeof(IronIngot), 1044036, 75, 1044037);

            index = AddCraft(typeof(LargeForgeEastDeed), "Add-Ons", "Large Forge East Deed", 78.9, 103.9, typeof(Board), 1044041, 5, 1044351);
            AddSkill(index, SkillName.Blacksmith, 80.0, 85.0);
            AddRes(index, typeof(IronIngot), 1044036, 100, 1044037);

            index = AddCraft(typeof(LargeForgeSouthDeed), "Add-Ons", "Large Forge South Deed", 78.9, 103.9, typeof(Board), 1044041, 5, 1044351);
            AddSkill(index, SkillName.Blacksmith, 80.0, 85.0);
            AddRes(index, typeof(IronIngot), 1044036, 100, 1044037);

            index = AddCraft(typeof(AnvilEastDeed), "Add-Ons", "Anvil East Deed", 73.6, 98.6, typeof(Board), 1044041, 5, 1044351);
            AddSkill(index, SkillName.Blacksmith, 75.0, 80.0);
            AddRes(index, typeof(IronIngot), 1044036, 150, 1044037);

            index = AddCraft(typeof(AnvilSouthDeed), "Add-Ons", "Anvil South Deed", 73.6, 98.6, typeof(Board), 1044041, 5, 1044351);
            AddSkill(index, SkillName.Blacksmith, 75.0, 80.0);
            AddRes(index, typeof(IronIngot), 1044036, 150, 1044037);

            index = AddCraft(typeof(Dressform), "Add-Ons", "Dressform", 63.1, 88.1, typeof(Board), 1044041, 25, 1044351);
            AddSkill(index, SkillName.Tailoring, 65.0, 70.0);
            AddRes(index, typeof(Cloth), 1044286, 10, 1044287);

            index = AddCraft(typeof(SpinningwheelEastDeed), "Add-Ons", "Spinning Wheel East Deed", 73.6, 98.6, typeof(Board), 1044041, 75, 1044351);
            AddSkill(index, SkillName.Tailoring, 65.0, 70.0);
            AddRes(index, typeof(Cloth), 1044286, 25, 1044287);

            index = AddCraft(typeof(SpinningwheelSouthDeed), "Add-Ons", "Spinning Wheel South Deed", 73.6, 98.6, typeof(Board), 1044041, 75, 1044351);
            AddSkill(index, SkillName.Tailoring, 65.0, 70.0);
            AddRes(index, typeof(Cloth), 1044286, 25, 1044287);

            index = AddCraft(typeof(LoomEastDeed), "Add-Ons", "Loom East Deed", 84.2, 109.2, typeof(Board), 1044041, 85, 1044351);
            AddSkill(index, SkillName.Tailoring, 65.0, 70.0);
            AddRes(index, typeof(Cloth), 1044286, 25, 1044287);

            index = AddCraft(typeof(LoomSouthDeed), "Add-Ons", "Loom South Deed", 84.2, 109.2, typeof(Board), 1044041, 85, 1044351);
            AddSkill(index, SkillName.Tailoring, 65.0, 70.0);
            AddRes(index, typeof(Cloth), 1044286, 25, 1044287);

            index = AddCraft(typeof(StoneOvenEastDeed), "Add-Ons", "Stone Oven East Deed", 68.4, 93.4, typeof(Board), 1044041, 85, 1044351);
            AddSkill(index, SkillName.Tinkering, 50.0, 55.0);
            AddRes(index, typeof(IronIngot), 1044036, 125, 1044037);

            index = AddCraft(typeof(StoneOvenSouthDeed), "Add-Ons", "Stone Oven South Deed", 68.4, 93.4, typeof(Board), 1044041, 85, 1044351);
            AddSkill(index, SkillName.Tinkering, 50.0, 55.0);
            AddRes(index, typeof(IronIngot), 1044036, 125, 1044037);

            index = AddCraft(typeof(FlourMillEastDeed), "Add-Ons", "Flour Mill East Deed", 94.7, 119.7, typeof(Board), 1044041, 100, 1044351);
            AddSkill(index, SkillName.Tinkering, 50.0, 55.0);
            AddRes(index, typeof(IronIngot), 1044036, 50, 1044037);

            index = AddCraft(typeof(FlourMillSouthDeed), "Add-Ons", "Flour Mill South Deed", 94.7, 119.7, typeof(Board), 1044041, 100, 1044351);
            AddSkill(index, SkillName.Tinkering, 50.0, 55.0);
            AddRes(index, typeof(IronIngot), 1044036, 50, 1044037);

            AddCraft(typeof(WaterTroughEastDeed), "Add-Ons", "Water Trough East Deed", 94.7, 119.7, typeof(Board), 1044041, 150, 1044351);
            AddCraft(typeof(WaterTroughSouthDeed), "Add-Ons", "Water Trough South Deed", 94.7, 119.7, typeof(Board), 1044041, 150, 1044351);

            //Houses
            index = AddCraft(typeof(SmallStoneTempleHouseDeed), "Houses", "Small Stone Temple ", 100, 105, typeof(SmallStoneTempleHousePlans), "Paladin House Plans", 1, "You do not have the plans necessary to construct this home.");
            AddRes(index, typeof(ArchitectsPen), "Architect's Pen", 1, "You are missing the Architect's Pen required to draft the deed");
            AddSkill(index, SkillName.Blacksmith, 75.0, 75.0);
            AddSkill(index, SkillName.Tinkering, 75.0, 75.0);

            index = AddCraft(typeof(ArbiterEstateDeed), "Houses", "Arbiters Estate", 100, 120, typeof(ArbitersEstateHousePlans), "Arbiters Estate House Plans", 1, "You do not have the plans necessary to construct this home.");
            AddRes(index, typeof(ArchitectsPen), "Architect's Pen", 1, "You are missing the Architect's Pen required to draft the deed");
            AddSkill(index, SkillName.Blacksmith, 75.0, 75.0);
            AddSkill(index, SkillName.Tinkering, 75.0, 75.0);

            index = AddCraft(typeof(SandstoneSpaHouseDeed), "Houses", "Sandstone Spa", 100, 110, typeof(SandstoneSpaHousePlans), "Lanistas Retreat House Plans", 1, "You do not have the plans necessary to construct this home.");
            AddRes(index, typeof(ArchitectsPen), "Architect's Pen", 1, "You are missing the Architect's Pen required to draft the deed");
            AddSkill(index, SkillName.Blacksmith, 75.0, 75.0);
            AddSkill(index, SkillName.Tinkering, 75.0, 75.0);

            index = AddCraft(typeof(MagistrateHouseDeed), "Houses", "Magistrates House", 100, 110, typeof(MagistratesHousePlans), "Magistrates House Plans", 1, "You do not have the plans necessary to construct this home.");
            AddRes(index, typeof(ArchitectsPen), "Architect's Pen", 1, "You are missing the Architect's Pen required to draft the deed");
            AddSkill(index, SkillName.Blacksmith, 75.0, 75.0);
            AddSkill(index, SkillName.Tinkering, 75.0, 75.0);

            //Ship Items and Upgrades  
            index = AddCraft(typeof(FishingPole), "Ship Items", "Fishing Pole", 68.4, 93.4, typeof(Board), 1044041, 5, 1044351); //This is in the categor of Other during AoS
            AddSkill(index, SkillName.Tailoring, 40.0, 45.0);
            AddRes(index, typeof(Cloth), 1044286, 5, 1044287);

            index = AddCraft(typeof(ShipParts), "Ship Items", "Ship Parts", 50, 60, typeof(Board), "Board", 100, "You do not have the necessary boards to construct the ship parts.");
            AddRes(index, typeof(Cloth), "Cloth", 50, "You do not have the necessary cloth to construct the ship parts.");
            AddRes(index, typeof(IronIngot), "Iron Ingot", 50, "You do not have the necessary boards to construct the ship parts.");

            index = AddCraft(typeof(ShipRepairTool), "Ship Items", "Ship Repair Tool", 50, 75, typeof(Board), "Board", 25, "You do not have the neccesary boards to construct a ship repair tool.");
            AddRes(index, typeof(Nails), "Nails", 5, "You do not have the neccessary nails to construct a ship repair tool");
            AddRes(index, typeof(Hammer), "Hammer", 1, "You require a hammer to construct a ship repair tool");
            AddRes(index, typeof(Saw), "Saw", 1, "You require a saw to construct a ship repair tool");

            index = AddCraft(typeof(DockDeed), "Ship Items", "Guild Dock Deed", 95, 100, typeof(Board), "Board", 300, "You do not have the necessary boards to construct the guild dock deed.");

            index = AddCraft(typeof(SmallBoatDeed), "Ship Items", "Small Boat Deed", 70.0, 75.0, typeof(ShipParts), "Ship Parts", 10, "You do not have the neccessary number of ship parts to construct that ship.");
            index = AddCraft(typeof(SmallDragonBoatDeed), "Ship Items", "Small Dragon Boat Deed", 70.0, 75.0, typeof(ShipParts), "Ship Parts", 11, "You do not have the neccessary number of ship parts to construct that ship.");
            index = AddCraft(typeof(MediumBoatDeed), "Ship Items", "Medium Boat Deed", 80.0, 85.0, typeof(ShipParts), "Ship Parts", 12, "You do not have the neccessary number of ship parts to construct that ship.");
            index = AddCraft(typeof(MediumDragonBoatDeed), "Ship Items", "Medium Dragon Boat Deed", 80.0, 85.0, typeof(ShipParts), "Ship Parts", 13, "You do not have the neccessary number of ship parts to construct that ship.");
            index = AddCraft(typeof(LargeBoatDeed), "Ship Items", "Large Boat Deed", 85, 90.0, typeof(ShipParts), "Ship Parts", 16, "You do not have the neccessary number of ship parts to construct that ship.");
            index = AddCraft(typeof(LargeDragonBoatDeed), "Ship Items", "Large Dragon Boat Deed ", 85.0, 90.0, typeof(ShipParts), "Ship Parts", 17, "You do not have the neccessary number of ship parts to construct that ship.");
            index = AddCraft(typeof(CarrackBoatDeed), "Ship Items", "Carrack Deed ", 90.0, 95.0, typeof(ShipParts), "Ship Parts", 20, "You do not have the neccessary number of ship parts to construct that ship.");
            index = AddCraft(typeof(GalleonBoatDeed), "Ship Items", "Galleon Deed", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 30, "You do not have the neccessary number of ship parts to construct that ship.");

            index = AddCraft(typeof(SmallFishermansBoatDeed), "Ship Items", "Small Fisherman's Boat Deed", 65.0, 70.0, typeof(ShipParts), "Ship Parts", 5, "You do not have the neccessary number of ship parts to construct that ship.");
            index = AddCraft(typeof(MediumFishermansBoatDeed), "Ship Items", "Medium Fisherman's Boat Deed", 75.0, 80.0, typeof(ShipParts), "Ship Parts", 6, "You do not have the neccessary number of ship parts to construct that ship.");
            index = AddCraft(typeof(LargeFishermansBoatDeed), "Ship Items", "Large Fisherman's Boat Deed", 85, 90.0, typeof(ShipParts), "Ship Parts", 8, "You do not have the neccessary number of ship parts to construct that ship.");
            index = AddCraft(typeof(CarrackFishermansBoatDeed), "Ship Items", "Fisherman's Carrack Deed ", 90.0, 95.0, typeof(ShipParts), "Ship Parts", 10, "You do not have the neccessary number of ship parts to construct that ship.");
            index = AddCraft(typeof(GalleonFishermansBoatDeed), "Ship Items", "Fisherman's Galleon Deed", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 15, "You do not have the neccessary number of ship parts to construct that ship.");

            index = AddCraft(typeof(CrowsNestUpgrade), "Ship Items", "Ship Upgrade: Crows Nest", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 1, "You do not have the neccessary number of ship parts to construct that upgrade.");
            index = AddCraft(typeof(ExpandedHoldUpgrade), "Ship Items", "Ship Upgrade: Expanded Hold", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 1, "You do not have the neccessary number of ship parts to construct that upgrade.");
            index = AddCraft(typeof(FishingTrawlerUpgrade), "Ship Items", "Ship Upgrade: Fishing Trawler", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 1, "You do not have the neccessary number of ship parts to construct that upgrade.");
            index = AddCraft(typeof(SecureHoldUpgrade), "Ship Items", "Ship Upgrade: Secure Hold", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 1, "You do not have the neccessary number of ship parts to construct that upgrade.");

            index = AddCraft(typeof(BoardingHooksUpgrade), "Ship Items", "Ship Upgrade: Boarding Hooks", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 2, "You do not have the neccessary number of ship parts to construct that upgrade.");
            index = AddCraft(typeof(ExceptionalRigging), "Ship Items", "Ship Upgrade: Exceptional Rigging", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 2, "You do not have the neccessary number of ship parts to construct that upgrade.");
            index = AddCraft(typeof(MastercraftCannonsUpgrade), "Ship Items", "Ship Upgrade: Mastercraft Cannons", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 2, "You do not have the neccessary number of ship parts to construct that upgrade.");
            index = AddCraft(typeof(ReinforcedHullUpgrade), "Ship Items", "Ship Upgrade: Reinforced Hull", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 2, "You do not have the neccessary number of ship parts to construct that upgrade.");

            index = AddCraft(typeof(BritainNavyPaintUpgrade), "Ship Items", "Ship Paint: Britain Naval Forces", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 3, "You do not have the neccessary number of ship parts to construct that upgrade.");
            index = AddCraft(typeof(FishermanPaintUpgrade), "Ship Items", "Ship Paint: Fisherman", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 3, "You do not have the neccessary number of ship parts to construct that upgrade.");
            index = AddCraft(typeof(PiratePaintUpgrade), "Ship Items", "Ship Paint: Pirate", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 3, "You do not have the neccessary number of ship parts to construct that upgrade.");
            index = AddCraft(typeof(GhostShipPaintUpgrade), "Ship Items", "Ship Paint: Undead", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 3, "You do not have the neccessary number of ship parts to construct that upgrade.");
            index = AddCraft(typeof(OrcPaintUpgrade), "Ship Items", "Ship Paint: Orc", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 3, "You do not have the neccessary number of ship parts to construct that upgrade.");
            index = AddCraft(typeof(OrghereimPaintUpgrade), "Ship Items", "Ship Paint: Orghereim", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 3, "You do not have the neccessary number of ship parts to construct that upgrade.");

            index = AddCraft(typeof(BritainNavyThemeUpgrade), "Ship Items", "Ship Theme: Britain Naval Forces", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 4, "You do not have the neccessary number of ship parts to construct that upgrade.");
            index = AddCraft(typeof(FishermanThemeUpgrade), "Ship Items", "Ship Theme: Fisherman", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 4, "You do not have the neccessary number of ship parts to construct that upgrade.");
            index = AddCraft(typeof(PirateThemeUpgrade), "Ship Items", "Ship Theme: Pirate", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 4, "You do not have the neccessary number of ship parts to construct that upgrade.");
            index = AddCraft(typeof(GhostShipThemeUpgrade), "Ship Items", "Ship Theme: Undead", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 4, "You do not have the neccessary number of ship parts to construct that upgrade.");

            //Misc
            index = AddCraft(typeof(CarpentryDungeonMould), "Misc", "Carpentry Dungeon Mould", 90, 105, typeof(Board), "Board", 250, "You do not have the neccessary boards to construct that.");
            AddRes(index, typeof(GhoulHide), "Ghoul Hide", 3, "You do not have the crafting components required to make this.");
            AddRes(index, typeof(ShatteredCrystal), "Shattered Crystal", 3, "You do not have the crafting components required to make this.");
            AddRes(index, typeof(TrollFat), "Troll Fat", 3, "You do not have the crafting components required to make this.");
            //AddRes(index, typeof(CockatriceEgg), "Cockatrice Egg", 3, "You do not have the crafting components required to make this.");

            index = AddCraft(typeof(CampingFirepit), "Misc", "Camping Firepit", 80.0, 120, typeof(Board), 1044041, 50, 1044351);
            AddRes(index, typeof(IronIngot), "Iron Ingot", 25, 1044037);
            AddRes(index, typeof(TrollFat), "Troll Fat", 1, "You do not have the neccesary crafting component needed to make this");

            AddCraft(typeof(BarrelStaves), "Misc", "Barrel Staves", 00.0, 25.0, typeof(Board), 1044041, 5, 1044351);
            AddCraft(typeof(BarrelLid), "Misc", "Barrel Lid", 11.0, 36.0, typeof(Board), 1044041, 4, 1044351);
            AddCraft(typeof(ShortMusicStand), "Misc", "Short Music Stand", 78.9, 103.9, typeof(Board), 1044041, 15, 1044351);
            AddCraft(typeof(TallMusicStand), "Misc", "Tall Music Stand", 81.5, 106.5, typeof(Board), 1044041, 20, 1044351);
            AddCraft(typeof(Easle), "Misc", "Easle", 86.8, 111.8, typeof(Board), 1044041, 20, 1044351);

            //Hue Plating
            /*
            AddCraft(typeof(RegularWoodHuePlating), "Misc", "regular wood hue plating", 70, 120, typeof(Board), "Board", 25, "You do not have the neccesary boards to construct that.");

            index = AddCraft(typeof(OakWoodHuePlating), "Misc", "oak wood hue plating", 70, 120, typeof(OakBoard), "oak board", 25, "You do not have the neccesary boards to construct that.");
            AddRes(index, typeof(Ghostweed), "ghostweed", 5, "You do not have the neccessary crafting components to construct that.");

            index = AddCraft(typeof(AshWoodHuePlating), "Other", "ash wood hue plating", 70, 120, typeof(AshBoard), "ash board", 25, "You do not have the neccesary boards to construct that.");
            AddRes(index, typeof(Snakeskin), "creepervine", 5, "You do not have the neccessary crafting components to construct that.");

            index = AddCraft(typeof(YewWoodHuePlating), "Other", "yew wood hue plating", 70, 120, typeof(YewBoard), "yew board", 25, "You do not have the neccesary boards to construct that.");
            AddRes(index, typeof(Quartzstone), "quartzstone", 5, "You do not have the neccessary crafting components to construct that.");

            index = AddCraft(typeof(HeartWoodHuePlating), "Other", "heartwood hue plating", 70, 120, typeof(HeartwoodBoard), "heartwood board", 25, "You do not have the neccesary boards to construct that.");
            AddRes(index, typeof(Creepervine), "creepervine", 5, "You do not have the neccessary crafting components to construct that.");

            index = AddCraft(typeof(BloodWoodHuePlating), "Other", "bloodwood hue plating", 70, 120, typeof(BloodwoodBoard), "bloodwood board", 25, "You do not have the neccesary boards to construct that.");
            AddRes(index, typeof(FireEssence), "fire essence", 5, "You do not have the neccessary crafting components to construct that.");

            index = AddCraft(typeof(FrostWoodHuePlating), "Other", "frostwood hue plating", 70, 120, typeof(FrostwoodBoard), "frostwood board", 25, "You do not have the neccesary boards to construct that.");
            AddRes(index, typeof(BluecapMushroom), "bluecap mushroom", 5, "You do not have the neccessary crafting components to construct that.");
            */

            Recycle = true;
            MarkOption = true;
            Repair = true;

            SetSubRes(typeof(Board), "Board");

            // Add every material you want the player to be able to choose from
            // This will override the overridable material	TODO: Verify the required skill amount
            AddSubRes(typeof(Board), "Board", 00.0, 1072652);
            AddSubRes(typeof(OakBoard), "Oak Board", 65.0, 1072652);
            AddSubRes(typeof(AshBoard), "Ash Board", 80.0, 1072652);
            AddSubRes(typeof(YewBoard), "Yew Board", 95.0, 1072652);
            AddSubRes(typeof(HeartwoodBoard), "Heartwood Board", 100, 1072652);
            AddSubRes(typeof(BloodwoodBoard), "Bloodwood Board", 100, 1072652);
            AddSubRes(typeof(FrostwoodBoard), "Frostwood Board", 100, 1072652);
        }
    }
}