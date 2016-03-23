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

            //Luthius Expansion            
            index = AddCraft(typeof(CampingFirepit), 1044294, "camping firepit", 80.0, 120, typeof(Board), 1044041, 50, 1044351);
            AddRes(index, typeof(IronIngot), 1044036, 25, 1044037);
            AddRes(index, typeof(TrollFat), "troll fat", 1, "You do not have the neccesary crafting component needed to make this");            
            
            // Furniture
            AddCraft(typeof(FootStool), 1044291, 1022910, 11.0, 36.0, typeof(Board), 1044041, 9, 1044351);
            AddCraft(typeof(Stool), 1044291, 1022602, 11.0, 36.0, typeof(Board), 1044041, 9, 1044351);
            AddCraft(typeof(BambooChair), 1044291, 1044300, 21.0, 46.0, typeof(Board), 1044041, 13, 1044351);
            AddCraft(typeof(WoodenChair), 1044291, 1044301, 21.0, 46.0, typeof(Board), 1044041, 13, 1044351);
            AddCraft(typeof(FancyWoodenChairCushion), 1044291, 1044302, 42.1, 67.1, typeof(Board), 1044041, 15, 1044351);
            AddCraft(typeof(WoodenChairCushion), 1044291, 1044303, 42.1, 67.1, typeof(Board), 1044041, 13, 1044351);
            AddCraft(typeof(WoodenBench), 1044291, 1022860, 52.6, 77.6, typeof(Board), 1044041, 17, 1044351);
            AddCraft(typeof(WoodenThrone), 1044291, 1044304, 52.6, 77.6, typeof(Board), 1044041, 17, 1044351);
            AddCraft(typeof(Throne), 1044291, 1044305, 73.6, 98.6, typeof(Board), 1044041, 19, 1044351);
            AddCraft(typeof(Nightstand), 1044291, 1044306, 42.1, 67.1, typeof(Board), 1044041, 17, 1044351);
            AddCraft(typeof(WritingTable), 1044291, 1022890, 63.1, 88.1, typeof(Board), 1044041, 17, 1044351);
            AddCraft(typeof(YewWoodTable), 1044291, 1044307, 63.1, 88.1, typeof(Board), 1044041, 23, 1044351);
            AddCraft(typeof(LargeTable), 1044291, 1044308, 84.2, 109.2, typeof(Board), 1044041, 27, 1044351);
            
            // Containers
            AddCraft(typeof(WoodenBox), 1044292, 1023709, 21.0, 46.0, typeof(Board), 1044041, 10, 1044351);
            AddCraft(typeof(SmallCrate), 1044292, 1044309, 10.0, 35.0, typeof(Board), 1044041, 8, 1044351);
            AddCraft(typeof(MediumCrate), 1044292, 1044310, 31.0, 56.0, typeof(Board), 1044041, 15, 1044351);
            AddCraft(typeof(LargeCrate), 1044292, 1044311, 47.3, 72.3, typeof(Board), 1044041, 18, 1044351);
            AddCraft(typeof(WoodenChest), 1044292, 1023650, 73.6, 98.6, typeof(Board), 1044041, 20, 1044351);
            AddCraft(typeof(EmptyBookcase), 1044292, 1022718, 31.5, 56.5, typeof(Board), 1044041, 25, 1044351);
            AddCraft(typeof(FancyArmoire), 1044292, 1044312, 84.2, 109.2, typeof(Board), 1044041, 35, 1044351);
            AddCraft(typeof(Armoire), 1044292, 1022643, 84.2, 109.2, typeof(Board), 1044041, 35, 1044351);
            
            index = AddCraft(typeof(Keg), 1044292, 1023711, 57.8, 82.8, typeof(BarrelStaves), 1044288, 3, 1044253);
            AddRes(index, typeof(BarrelHoops), 1044289, 1, 1044253);
            AddRes(index, typeof(BarrelLid), 1044251, 1, 1044253);
            
            // Staves and Shields
            AddCraft(typeof(ShepherdsCrook), Core.ML ? 1044566 : 1044295, 1023713, 78.9, 103.9, typeof(Board), 1044041, 7, 1044351);
            AddCraft(typeof(QuarterStaff), Core.ML ? 1044566 : 1044295, 1023721, 73.6, 98.6, typeof(Board), 1044041, 6, 1044351);
            AddCraft(typeof(GnarledStaff), Core.ML ? 1044566 : 1044295, 1025112, 78.9, 103.9, typeof(Board), 1044041, 7, 1044351);
            AddCraft(typeof(WoodenShield), Core.ML ? 1062760 : 1044295, 1027034, 52.6, 77.6, typeof(Board), 1044041, 9, 1044351);
            AddCraft(typeof(Club), 1044295, "Club", 65, 100, typeof(Board), 1044041, 10, 1044351);
            AddCraft(typeof(BlackStaff), 1044295, "Black Staff", 81.5, 105.0, typeof(Board), 1044041, 9, 1044351);
                       
            index = AddCraft(typeof(FishingPole), Core.ML ? 1044294 : 1044295, 1023519, 68.4, 93.4, typeof(Board), 1044041, 5, 1044351); //This is in the categor of Other during AoS
            AddSkill(index, SkillName.Tailoring, 40.0, 45.0);
            AddRes(index, typeof(Cloth), 1044286, 5, 1044287);            

            // Instruments
            index = AddCraft(typeof(LapHarp), 1044293, 1023762, 63.1, 88.1, typeof(Board), 1044041, 20, 1044351);
            AddSkill(index, SkillName.Musicianship, 45.0, 50.0);
            AddRes(index, typeof(Cloth), 1044286, 10, 1044287);

            index = AddCraft(typeof(Harp), 1044293, 1023761, 78.9, 103.9, typeof(Board), 1044041, 35, 1044351);
            AddSkill(index, SkillName.Musicianship, 45.0, 50.0);
            AddRes(index, typeof(Cloth), 1044286, 15, 1044287);

            index = AddCraft(typeof(Drums), 1044293, 1023740, 57.8, 82.8, typeof(Board), 1044041, 20, 1044351);
            AddSkill(index, SkillName.Musicianship, 45.0, 50.0);
            AddRes(index, typeof(Cloth), 1044286, 10, 1044287);

            index = AddCraft(typeof(Lute), 1044293, 1023763, 68.4, 93.4, typeof(Board), 1044041, 25, 1044351);
            AddSkill(index, SkillName.Musicianship, 45.0, 50.0);
            AddRes(index, typeof(Cloth), 1044286, 10, 1044287);

            index = AddCraft(typeof(Tambourine), 1044293, 1023741, 57.8, 82.8, typeof(Board), 1044041, 15, 1044351);
            AddSkill(index, SkillName.Musicianship, 45.0, 50.0);
            AddRes(index, typeof(Cloth), 1044286, 10, 1044287);

            index = AddCraft(typeof(TambourineTassel), 1044293, 1044320, 57.8, 82.8, typeof(Board), 1044041, 15, 1044351);
            AddSkill(index, SkillName.Musicianship, 45.0, 50.0);
            AddRes(index, typeof(Cloth), 1044286, 15, 1044287);
            
            index = AddCraft(typeof(BambooFlute), 1044293, 1030247, 80.0, 105.0, typeof(Board), 1044041, 15, 1044351);
            AddSkill(index, SkillName.Musicianship, 45.0, 50.0);                
            
            // Misc
            index = AddCraft(typeof(SmallBedSouthDeed), 1044290, 1044321, 94.7, 119.8, typeof(Board), 1044041, 100, 1044351);
            AddSkill(index, SkillName.Tailoring, 75.0, 80.0);
            AddRes(index, typeof(Cloth), 1044286, 100, 1044287);

            index = AddCraft(typeof(SmallBedEastDeed), 1044290, 1044322, 94.7, 119.8, typeof(Board), 1044041, 100, 1044351);
            AddSkill(index, SkillName.Tailoring, 75.0, 80.0);
            AddRes(index, typeof(Cloth), 1044286, 100, 1044287);
            
            index = AddCraft(typeof(LargeBedSouthDeed), 1044290, 1044323, 94.7, 119.8, typeof(Board), 1044041, 150, 1044351);
            AddSkill(index, SkillName.Tailoring, 75.0, 80.0);
            AddRes(index, typeof(Cloth), 1044286, 150, 1044287);
            
            index = AddCraft(typeof(LargeBedEastDeed), 1044290, 1044324, 94.7, 119.8, typeof(Board), 1044041, 150, 1044351);
            AddSkill(index, SkillName.Tailoring, 75.0, 80.0);
            AddRes(index, typeof(Cloth), 1044286, 150, 1044287);
            
            AddCraft(typeof(DartBoardSouthDeed), 1044290, 1044325, 15.7, 40.7, typeof(Board), 1044041, 5, 1044351);
            AddCraft(typeof(DartBoardEastDeed), 1044290, 1044326, 15.7, 40.7, typeof(Board), 1044041, 5, 1044351);
            AddCraft(typeof(BallotBoxDeed), 1044290, 1044327, 47.3, 72.3, typeof(Board), 1044041, 5, 1044351);
            
            index = AddCraft(typeof(PentagramDeed), 1044290, 1044328, 100.0, 125.0, typeof(Board), 1044041, 100, 1044351);
            AddSkill(index, SkillName.Magery, 75.0, 80.0);
            AddRes(index, typeof(IronIngot), 1044036, 40, 1044037);
            
            index = AddCraft(typeof(AbbatoirDeed), 1044290, 1044329, 100.0, 125.0, typeof(Board), 1044041, 100, 1044351);
            AddSkill(index, SkillName.Magery, 50.0, 55.0);
            AddRes(index, typeof(IronIngot), 1044036, 40, 1044037);

            index = AddCraft(typeof(TrainingDummyEastDeed), 1044290, 1044335, 68.4, 93.4, typeof(Board), 1044041, 55, 1044351);
            AddSkill(index, SkillName.Tailoring, 50.0, 55.0);
            AddRes(index, typeof(Cloth), 1044286, 60, 1044287);
            
            index = AddCraft(typeof(TrainingDummySouthDeed), 1044290, 1044336, 68.4, 93.4, typeof(Board), 1044041, 55, 1044351);
            AddSkill(index, SkillName.Tailoring, 50.0, 55.0);
            AddRes(index, typeof(Cloth), 1044286, 60, 1044287);
            
            index = AddCraft(typeof(PickpocketDipEastDeed), 1044290, 1044337, 73.6, 98.6, typeof(Board), 1044041, 65, 1044351);
            AddSkill(index, SkillName.Tailoring, 50.0, 55.0);
            AddRes(index, typeof(Cloth), 1044286, 60, 1044287);
            
            index = AddCraft(typeof(PickpocketDipSouthDeed), 1044290, 1044338, 73.6, 98.6, typeof(Board), 1044041, 65, 1044351);
            AddSkill(index, SkillName.Tailoring, 50.0, 55.0);
            AddRes(index, typeof(Cloth), 1044286, 60, 1044287);

            // Blacksmithy - This changed to Anvils and Forges (1111809) for SA
            index = AddCraft(typeof(SmallForgeDeed), 1044296, 1044330, 73.6, 98.6, typeof(Board), 1044041, 5, 1044351);
            AddSkill(index, SkillName.Blacksmith, 75.0, 80.0);
            AddRes(index, typeof(IronIngot), 1044036, 75, 1044037);
            
            index = AddCraft(typeof(LargeForgeEastDeed), 1044296, 1044331, 78.9, 103.9, typeof(Board), 1044041, 5, 1044351);
            AddSkill(index, SkillName.Blacksmith, 80.0, 85.0);
            AddRes(index, typeof(IronIngot), 1044036, 100, 1044037);
            
            index = AddCraft(typeof(LargeForgeSouthDeed), 1044296, 1044332, 78.9, 103.9, typeof(Board), 1044041, 5, 1044351);
            AddSkill(index, SkillName.Blacksmith, 80.0, 85.0);
            AddRes(index, typeof(IronIngot), 1044036, 100, 1044037);
            
            index = AddCraft(typeof(AnvilEastDeed), 1044296, 1044333, 73.6, 98.6, typeof(Board), 1044041, 5, 1044351);
            AddSkill(index, SkillName.Blacksmith, 75.0, 80.0);
            AddRes(index, typeof(IronIngot), 1044036, 150, 1044037);
            
            index = AddCraft(typeof(AnvilSouthDeed), 1044296, 1044334, 73.6, 98.6, typeof(Board), 1044041, 5, 1044351);
            AddSkill(index, SkillName.Blacksmith, 75.0, 80.0);
            AddRes(index, typeof(IronIngot), 1044036, 150, 1044037);

            // Tailoring
            index = AddCraft(typeof(Dressform), 1044298, 1044339, 63.1, 88.1, typeof(Board), 1044041, 25, 1044351);
            AddSkill(index, SkillName.Tailoring, 65.0, 70.0);
            AddRes(index, typeof(Cloth), 1044286, 10, 1044287);
            
            index = AddCraft(typeof(SpinningwheelEastDeed), 1044298, 1044341, 73.6, 98.6, typeof(Board), 1044041, 75, 1044351);
            AddSkill(index, SkillName.Tailoring, 65.0, 70.0);
            AddRes(index, typeof(Cloth), 1044286, 25, 1044287);
            
            index = AddCraft(typeof(SpinningwheelSouthDeed), 1044298, 1044342, 73.6, 98.6, typeof(Board), 1044041, 75, 1044351);
            AddSkill(index, SkillName.Tailoring, 65.0, 70.0);
            AddRes(index, typeof(Cloth), 1044286, 25, 1044287);
            
            index = AddCraft(typeof(LoomEastDeed), 1044298, 1044343, 84.2, 109.2, typeof(Board), 1044041, 85, 1044351);
            AddSkill(index, SkillName.Tailoring, 65.0, 70.0);
            AddRes(index, typeof(Cloth), 1044286, 25, 1044287);
            
            index = AddCraft(typeof(LoomSouthDeed), 1044298, 1044344, 84.2, 109.2, typeof(Board), 1044041, 85, 1044351);
            AddSkill(index, SkillName.Tailoring, 65.0, 70.0);
            AddRes(index, typeof(Cloth), 1044286, 25, 1044287);

            // Cooking
            index = AddCraft(typeof(StoneOvenEastDeed), Core.ML ? 1044298 : 1044299, 1044345, 68.4, 93.4, typeof(Board), 1044041, 85, 1044351);
            AddSkill(index, SkillName.Tinkering, 50.0, 55.0);
            AddRes(index, typeof(IronIngot), 1044036, 125, 1044037);
            
            index = AddCraft(typeof(StoneOvenSouthDeed), Core.ML ? 1044298 : 1044299, 1044346, 68.4, 93.4, typeof(Board), 1044041, 85, 1044351);
            AddSkill(index, SkillName.Tinkering, 50.0, 55.0);
            AddRes(index, typeof(IronIngot), 1044036, 125, 1044037);
            
            index = AddCraft(typeof(FlourMillEastDeed), Core.ML ? 1044298 : 1044299, 1044347, 94.7, 119.7, typeof(Board), 1044041, 100, 1044351);
            AddSkill(index, SkillName.Tinkering, 50.0, 55.0);
            AddRes(index, typeof(IronIngot), 1044036, 50, 1044037);
            
            index = AddCraft(typeof(FlourMillSouthDeed), Core.ML ? 1044298 : 1044299, 1044348, 94.7, 119.7, typeof(Board), 1044041, 100, 1044351);
            AddSkill(index, SkillName.Tinkering, 50.0, 55.0);
            AddRes(index, typeof(IronIngot), 1044036, 50, 1044037);
            
            AddCraft(typeof(WaterTroughEastDeed), 1044298, 1044349, 94.7, 119.7, typeof(Board), 1044041, 150, 1044351);
            AddCraft(typeof(WaterTroughSouthDeed), 1044298, 1044350, 94.7, 119.7, typeof(Board), 1044041, 150, 1044351);

            //House crafting
            index = AddCraft(typeof(SmallStoneTempleHouseDeed), "Large Items", "small stone temple ", 100, 105, typeof(SmallStoneTempleHousePlans), "Paladin House Plans", 1, "You do not have the plans necessary to construct this home.");
            AddRes(index, typeof(ArchitectsPen), "Architect's Pen", 1, "You are missing the Architect's Pen required to draft the deed");
            AddSkill(index, SkillName.Blacksmith, 75.0, 75.0);
            AddSkill(index, SkillName.Tinkering, 75.0, 75.0);
            
            index = AddCraft(typeof(ArbiterEstateDeed), "Large Items", "arbiters estate", 100, 120, typeof(ArbitersEstateHousePlans), "Arbiters Estate House Plans", 1, "You do not have the plans necessary to construct this home.");
            AddRes(index, typeof(ArchitectsPen), "Architect's Pen", 1, "You are missing the Architect's Pen required to draft the deed");
            AddSkill(index, SkillName.Blacksmith, 75.0, 75.0);
            AddSkill(index, SkillName.Tinkering, 75.0, 75.0);
            
            index = AddCraft(typeof(SandstoneSpaHouseDeed), "Large Items", "sandstone spa", 100, 110, typeof(SandstoneSpaHousePlans), "Lanistas Retreat House Plans", 1, "You do not have the plans necessary to construct this home.");
            AddRes(index, typeof(ArchitectsPen), "Architect's Pen", 1, "You are missing the Architect's Pen required to draft the deed");
            AddSkill(index, SkillName.Blacksmith, 75.0, 75.0);
            AddSkill(index, SkillName.Tinkering, 75.0, 75.0);
            
            index = AddCraft(typeof(MagistrateHouseDeed), "Large Items", "magistrates house", 100, 110, typeof(MagistratesHousePlans), "Magistrates House Plans", 1, "You do not have the plans necessary to construct this home.");
            AddRes(index, typeof(ArchitectsPen), "Architect's Pen", 1, "You are missing the Architect's Pen required to draft the deed");
            AddSkill(index, SkillName.Blacksmith, 75.0, 75.0);
            AddSkill(index, SkillName.Tinkering, 75.0, 75.0);

            //Ship Items            
            index = AddCraft(typeof(ShipParts), "Ship Items", "ship parts", 50, 60, typeof(Board), "board", 100, "You do not have the necessary boards to construct the ship parts.");
            AddRes(index, typeof(Cloth), "cloth", 50, "You do not have the necessary cloth to construct the ship parts.");
            AddRes(index, typeof(IronIngot), "iron ingot", 50, "You do not have the necessary boards to construct the ship parts.");

            index = AddCraft(typeof(ShipRepairTool), "Ship Items", "ship repair tool", 50, 75, typeof(Board), "board", 25, "You do not have the neccesary boards to construct a ship repair tool.");
            AddRes(index, typeof(Nails), "nails", 5, "You do not have the neccessary nails to construct a ship repair tool");
            AddRes(index, typeof(Hammer), "hammer", 1, "You require a hammer to construct a ship repair tool");
            AddRes(index, typeof(Saw), "saw", 1, "You require a saw to construct a ship repair tool");

            index = AddCraft(typeof(DockDeed), "Ship Items", "guild dock deed", 95, 100, typeof(Board), "board", 300, "You do not have the necessary boards to construct the guild dock deed.");
            
            //Boats
            index = AddCraft(typeof(SmallBoatDeed), "Ship Items", "small boat deed", 70.0, 75.0, typeof(ShipParts), "Ship Parts", 10, "You do not have the neccessary number of ship parts to construct that ship.");
            index = AddCraft(typeof(SmallDragonBoatDeed), "Ship Items", "small dragon boat deed", 70.0, 75.0, typeof(ShipParts), "Ship Parts", 11, "You do not have the neccessary number of ship parts to construct that ship.");
            index = AddCraft(typeof(MediumBoatDeed), "Ship Items", "medium boat deed", 80.0, 85.0, typeof(ShipParts), "Ship Parts", 12, "You do not have the neccessary number of ship parts to construct that ship.");
            index = AddCraft(typeof(MediumDragonBoatDeed), "Ship Items", "medium dragon boat deed", 80.0, 85.0, typeof(ShipParts), "Ship Parts", 13, "You do not have the neccessary number of ship parts to construct that ship.");
            index = AddCraft(typeof(LargeBoatDeed), "Ship Items", "large boat deed", 85, 90.0, typeof(ShipParts), "Ship Parts", 16, "You do not have the neccessary number of ship parts to construct that ship.");
            index = AddCraft(typeof(LargeDragonBoatDeed), "Ship Items", "large dragon boat deed ", 85.0, 90.0, typeof(ShipParts), "Ship Parts", 17, "You do not have the neccessary number of ship parts to construct that ship.");
            index = AddCraft(typeof(CarrackBoatDeed), "Ship Items", "carrack deed ", 90.0, 95.0, typeof(ShipParts), "Ship Parts", 20, "You do not have the neccessary number of ship parts to construct that ship.");
            index = AddCraft(typeof(GalleonBoatDeed), "Ship Items", "galleon deed", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 30, "You do not have the neccessary number of ship parts to construct that ship.");

            index = AddCraft(typeof(SmallFishermansBoatDeed), "Ship Items", "small fisherman's boat deed", 65.0, 70.0, typeof(ShipParts), "Ship Parts", 5, "You do not have the neccessary number of ship parts to construct that ship.");
            index = AddCraft(typeof(MediumFishermansBoatDeed), "Ship Items", "medium fisherman's boat deed", 75.0, 80.0, typeof(ShipParts), "Ship Parts", 6, "You do not have the neccessary number of ship parts to construct that ship.");
            index = AddCraft(typeof(LargeFishermansBoatDeed), "Ship Items", "large fisherman's boat deed", 85, 90.0, typeof(ShipParts), "Ship Parts", 8, "You do not have the neccessary number of ship parts to construct that ship.");
            index = AddCraft(typeof(CarrackFishermansBoatDeed), "Ship Items", "fisherman's carrack deed ", 90.0, 95.0, typeof(ShipParts), "Ship Parts", 10, "You do not have the neccessary number of ship parts to construct that ship.");
            index = AddCraft(typeof(GalleonFishermansBoatDeed), "Ship Items", "fisherman's galleon deed", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 15, "You do not have the neccessary number of ship parts to construct that ship.");

            //Boat Upgrades               
            index = AddCraft(typeof(CrowsNestUpgrade), "Ship Items", "ship upgrade: crows nest", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 1, "You do not have the neccessary number of ship parts to construct that upgrade.");
            index = AddCraft(typeof(ExpandedHoldUpgrade), "Ship Items", "ship upgrade: expanded hold", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 1, "You do not have the neccessary number of ship parts to construct that upgrade.");
            index = AddCraft(typeof(FishingTrawlerUpgrade), "Ship Items", "ship upgrade: fishing trawler", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 1, "You do not have the neccessary number of ship parts to construct that upgrade.");
            index = AddCraft(typeof(SecureHoldUpgrade), "Ship Items", "ship upgrade: secure hold", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 1, "You do not have the neccessary number of ship parts to construct that upgrade.");

            index = AddCraft(typeof(BoardingHooksUpgrade), "Ship Items", "ship upgrade: boarding hooks", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 2, "You do not have the neccessary number of ship parts to construct that upgrade.");
            index = AddCraft(typeof(ExceptionalRigging), "Ship Items", "ship upgrade: exceptional rigging", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 2, "You do not have the neccessary number of ship parts to construct that upgrade.");
            index = AddCraft(typeof(MastercraftCannonsUpgrade), "Ship Items", "ship upgrade: mastercraft cannons", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 2, "You do not have the neccessary number of ship parts to construct that upgrade.");
            index = AddCraft(typeof(ReinforcedHullUpgrade), "Ship Items", "ship upgrade: reinforced hull", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 2, "You do not have the neccessary number of ship parts to construct that upgrade.");

            index = AddCraft(typeof(BritainNavyPaintUpgrade), "Ship Items", "ship paint: britain naval forces", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 3, "You do not have the neccessary number of ship parts to construct that upgrade.");
            index = AddCraft(typeof(FishermanPaintUpgrade), "Ship Items", "ship paint: fisherman", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 3, "You do not have the neccessary number of ship parts to construct that upgrade.");
            index = AddCraft(typeof(PiratePaintUpgrade), "Ship Items", "ship paint: pirate", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 3, "You do not have the neccessary number of ship parts to construct that upgrade.");
            index = AddCraft(typeof(GhostShipPaintUpgrade), "Ship Items", "ship paint: ghost ship", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 3, "You do not have the neccessary number of ship parts to construct that upgrade.");
            index = AddCraft(typeof(OrcPaintUpgrade), "Ship Items", "ship paint: orc", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 3, "You do not have the neccessary number of ship parts to construct that upgrade.");
            index = AddCraft(typeof(OrghereimPaintUpgrade), "Ship Items", "ship paint: orghereim", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 3, "You do not have the neccessary number of ship parts to construct that upgrade.");

            index = AddCraft(typeof(BritainNavyThemeUpgrade), "Ship Items", "ship theme: britain naval forces", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 4, "You do not have the neccessary number of ship parts to construct that upgrade.");
            index = AddCraft(typeof(FishermanThemeUpgrade), "Ship Items", "ship theme: fisherman", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 4, "You do not have the neccessary number of ship parts to construct that upgrade.");
            index = AddCraft(typeof(PirateThemeUpgrade), "Ship Items", "ship theme: pirate", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 4, "You do not have the neccessary number of ship parts to construct that upgrade.");
            index = AddCraft(typeof(GhostShipThemeUpgrade), "Ship Items", "ship theme: ghost ship", 95.0, 100.0, typeof(ShipParts), "Ship Parts", 4, "You do not have the neccessary number of ship parts to construct that upgrade.");

            AddCraft(typeof(BarrelStaves), "Other", 1027857, 00.0, 25.0, typeof(Board), 1044041, 5, 1044351);
            AddCraft(typeof(BarrelLid), "Other", 1027608, 11.0, 36.0, typeof(Board), 1044041, 4, 1044351);
            AddCraft(typeof(ShortMusicStand), "Other", 1044313, 78.9, 103.9, typeof(Board), 1044041, 15, 1044351);
            AddCraft(typeof(TallMusicStand), "Other", 1044315, 81.5, 106.5, typeof(Board), 1044041, 20, 1044351);
            AddCraft(typeof(Easle), "Other", 1044317, 86.8, 111.8, typeof(Board), 1044041, 20, 1044351);
            
            index = AddCraft(typeof(CarpentryDungeonMould), "Other", "carpentry dungeon mould", 90, 105, typeof(Board), "Board", 250, "You do not have the neccessary boards to construct that.");
            AddRes(index, typeof(GhoulHide), "Ghoul Hide", 3, "You do not have the crafting components required to make this.");
            AddRes(index, typeof(ShatteredCrystal), "Shattered Crystal", 3, "You do not have the crafting components required to make this.");
            AddRes(index, typeof(TrollFat), "Troll Fat", 3, "You do not have the crafting components required to make this.");
            //AddRes(index, typeof(CockatriceEgg), "Cockatrice Egg", 3, "You do not have the crafting components required to make this.");
            
            AddCraft(typeof(RegularWoodHuePlating), "Other", "regular wood hue plating", 70, 120, typeof(Board), "board", 25, "You do not have the neccesary boards to construct that.");
            
            index = AddCraft(typeof(OakWoodHuePlating), "Other", "oak wood hue plating", 70, 120, typeof(OakBoard), "oak board", 25, "You do not have the neccesary boards to construct that.");
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