using System;
using Server.Items;
using Server.Custom;

namespace Server.Engines.Craft
{
    public class DefCooking : CraftSystem
    {
        public override SkillName MainSkill
        {
            get { return SkillName.Cooking; }
        }

        public override int GumpTitleNumber
        {
            get { return 1044003; } // <CENTER>COOKING MENU</CENTER>
        }

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new DefCooking();

                return m_CraftSystem;
            }
        }

        public override CraftECA ECA { get { return CraftECA.ChanceMinusSixtyToFourtyFive; } }

        public override double GetChanceAtMin(CraftItem item)
        {
            return 0.0; // 0%
        }

        private DefCooking() : base(1, 1, 1.25)
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

        public override bool ConsumeOnFailure(Mobile from, Type resourceType, CraftItem craftItem)
        {
            if (resourceType == typeof(Server.Custom.Items.RareBerries))
                return false;

            else
                return base.ConsumeOnFailure(from, resourceType, craftItem);
        }

        public override void PlayCraftEffect(Mobile from)
        {
            from.PlaySound(0x4BA);
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

            index = AddCraft(1, typeof(Dough), 1044495, 1024157, 0.0, 100.0, typeof(SackFlour), 1044468, 1, 1044253);
            AddRes(index, typeof(BaseBeverage), 1046458, 1, 1044253);

            /*
            index = AddCraft(1, typeof(SackFlour), 1044495, 1024153, 0.0, 100.0, typeof(WheatSheaf), 1044489, 2, 1044490);
            SetNeedMill(index, true);

            index = AddCraft(1, typeof(Dough), 1044495, 1024157, 0.0, 100.0, typeof(SackFlour), 1044468, 1, 1044253);
            AddRes(index, typeof(BaseBeverage), 1046458, 1, 1044253);

            index = AddCraft(1, typeof(SweetDough), 1044495, 1041340, 0.0, 100.0, typeof(Dough), 1044469, 1, 1044253);
            AddRes(index, typeof(JarHoney), 1044472, 1, 1044253);

            index = AddCraft(1, typeof(CakeMix), 1044495, 1041002, 0.0, 100.0, typeof(SackFlour), 1044468, 1, 1044253);
            AddRes(index, typeof(SweetDough), 1044475, 1, 1044253);

            index = AddCraft(1, typeof(CookieMix), 1044495, 1024159, 0.0, 100.0, typeof(JarHoney), 1044472, 1, 1044253);
            AddRes(index, typeof(SweetDough), 1044475, 1, 1044253);

            if (Core.ML)
            {
                index = AddCraft(1, typeof(CocoaButter), 1044495, 1079998, 0.0, 100.0, typeof(CocoaPulp), 1080530, 1, 1044253);
                SetItemHue(index, 0x457);
                SetNeededExpansion(index, Expansion.ML);
                SetNeedOven(index, true);

                index = AddCraft(1, typeof(CocoaLiquor), 1044495, 1079999, 0.0, 100.0, typeof(CocoaPulp), 1080530, 1, 1044253);
                AddRes(index, typeof(EmptyPewterBowl), 1025629, 1, 1044253);
                SetItemHue(index, 0x46A);
                SetNeededExpansion(index, Expansion.ML);
                SetNeedOven(index, true);
            }

            index = AddCraft(1, typeof(UnbakedQuiche), 1044496, 1041339, 0.0, 100.0, typeof(Dough), 1044469, 1, 1044253);
            AddRes(index, typeof(Eggs), 1044477, 1, 1044253);

            // TODO: This must also support chicken and lamb legs
            index = AddCraft(1, typeof(UnbakedMeatPie), 1044496, 1041338, 0.0, 100.0, typeof(Dough), 1044469, 1, 1044253);
            AddRes(index, typeof(RawRibs), 1044482, 1, 1044253);

            index = AddCraft(1, typeof(UncookedSausagePizza), 1044496, 1041337, 0.0, 100.0, typeof(Dough), 1044469, 1, 1044253);
            AddRes(index, typeof(Sausage), 1044483, 1, 1044253);

            index = AddCraft(1, typeof(UncookedCheesePizza), 1044496, 1041341, 0.0, 100.0, typeof(Dough), 1044469, 1, 1044253);
            AddRes(index, typeof(CheeseWheel), 1044486, 1, 1044253);

            index = AddCraft(1, typeof(UnbakedFruitPie), 1044496, 1041334, 0.0, 100.0, typeof(Dough), 1044469, 1, 1044253);
            AddRes(index, typeof(Pear), 1044481, 1, 1044253);

            index = AddCraft(1, typeof(UnbakedPeachCobbler), 1044496, 1041335, 0.0, 100.0, typeof(Dough), 1044469, 1, 1044253);
            AddRes(index, typeof(Peach), 1044480, 1, 1044253);

            index = AddCraft(1, typeof(UnbakedApplePie), 1044496, 1041336, 0.0, 100.0, typeof(Dough), 1044469, 1, 1044253);
            AddRes(index, typeof(Apple), 1044479, 1, 1044253);

            index = AddCraft(1, typeof(UnbakedPumpkinPie), 1044496, 1041342, 0.0, 100.0, typeof(Dough), 1044469, 1, 1044253);
            AddRes(index, typeof(Pumpkin), 1044484, 1, 1044253);

            if (Core.SE)
            {
                index = AddCraft(1, typeof(GreenTea), 1044496, 1030315, 80.0, 130.0, typeof(GreenTeaBasket), 1030316, 1, 1044253);
                AddRes(index, typeof(BaseBeverage), 1046458, 1, 1044253);
                SetNeededExpansion(index, Expansion.SE);
                SetNeedOven(index, true);

                index = AddCraft(1, typeof(WasabiClumps), 1044496, 1029451, 70.0, 120.0, typeof(BaseBeverage), 1046458, 1, 1044253);
                AddRes(index, typeof(WoodenBowlOfPeas), 1025633, 3, 1044253);
                SetNeededExpansion(index, Expansion.SE);

                index = AddCraft(1, typeof(SushiRolls), 1044496, 1030303, 90.0, 120.0, typeof(BaseBeverage), 1046458, 1, 1044253);
                AddRes(index, typeof(RawFishSteak), 1044476, 10, 1044253);
                SetNeededExpansion(index, Expansion.SE);

                index = AddCraft(1, typeof(SushiPlatter), 1044496, 1030305, 90.0, 120.0, typeof(BaseBeverage), 1046458, 1, 1044253);
                AddRes(index, typeof(RawFishSteak), 1044476, 10, 1044253);
                SetNeededExpansion(index, Expansion.SE);
            }

            if (Core.SE)
            {
                index = AddCraft(1, typeof(EggBomb), 1044496, 1030249, 90.0, 120.0, typeof(Eggs), 1044477, 1, 1044253);
                AddRes(index, typeof(SackFlour), 1044468, 3, 1044253);
                SetNeededExpansion(index, Expansion.SE);
            }

            index = AddCraft(1, typeof(BreadLoaf), 1044497, 1024156, 0.0, 100.0, typeof(Dough), 1044469, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(1, typeof(Cookies), 1044497, 1025643, 0.0, 100.0, typeof(CookieMix), 1044474, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(1, typeof(Cake), 1044497, 1022537, 0.0, 100.0, typeof(CakeMix), 1044471, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(1, typeof(Muffins), 1044497, 1022539, 0.0, 100.0, typeof(SweetDough), 1044475, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(1, typeof(Quiche), 1044497, 1041345, 0.0, 100.0, typeof(UnbakedQuiche), 1044518, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(1, typeof(MeatPie), 1044497, 1041347, 0.0, 100.0, typeof(UnbakedMeatPie), 1044519, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(1, typeof(SausagePizza), 1044497, 1044517, 0.0, 100.0, typeof(UncookedSausagePizza), 1044520, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(1, typeof(CheesePizza), 1044497, 1044516, 0.0, 100.0, typeof(UncookedCheesePizza), 1044521, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(1, typeof(FruitPie), 1044497, 1041346, 0.0, 100.0, typeof(UnbakedFruitPie), 1044522, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(1, typeof(PeachCobbler), 1044497, 1041344, 0.0, 100.0, typeof(UnbakedPeachCobbler), 1044523, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(1, typeof(ApplePie), 1044497, 1041343, 0.0, 100.0, typeof(UnbakedApplePie), 1044524, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(1, typeof(PumpkinPie), 1044497, 1041348, 0.0, 100.0, typeof(UnbakedPumpkinPie), 1046461, 1, 1044253);
            SetNeedOven(index, true);

            if (Core.SE)
            {
                index = AddCraft(1, typeof(MisoSoup), 1044497, 1030317, 60.0, 110.0, typeof(RawFishSteak), 1044476, 1, 1044253);
                AddRes(index, typeof(BaseBeverage), 1046458, 1, 1044253);
                SetNeededExpansion(index, Expansion.SE);
                SetNeedOven(index, true);

                index = AddCraft(1, typeof(WhiteMisoSoup), 1044497, 1030318, 60.0, 110.0, typeof(RawFishSteak), 1044476, 1, 1044253);
                AddRes(index, typeof(BaseBeverage), 1046458, 1, 1044253);
                SetNeededExpansion(index, Expansion.SE);
                SetNeedOven(index, true);

                index = AddCraft(1, typeof(RedMisoSoup), 1044497, 1030319, 60.0, 110.0, typeof(RawFishSteak), 1044476, 1, 1044253);
                AddRes(index, typeof(BaseBeverage), 1046458, 1, 1044253);
                SetNeededExpansion(index, Expansion.SE);
                SetNeedOven(index, true);

                index = AddCraft(1, typeof(AwaseMisoSoup), 1044497, 1030320, 60.0, 110.0, typeof(RawFishSteak), 1044476, 1, 1044253);
                AddRes(index, typeof(BaseBeverage), 1046458, 1, 1044253);
                SetNeededExpansion(index, Expansion.SE);
                SetNeedOven(index, true);
            }

            index = AddCraft(1, typeof(CookedBird), 1044498, 1022487, 0.0, 100.0, typeof(RawBird), 1044470, 1, 1044253);
            SetNeedHeat(index, true);
            SetUseAllRes(index, true);

            index = AddCraft(1, typeof(ChickenLeg), 1044498, 1025640, 0.0, 100.0, typeof(RawChickenLeg), 1044473, 1, 1044253);
            SetNeedHeat(index, true);
            SetUseAllRes(index, true);

            index = AddCraft(1, typeof(FishSteak), 1044498, 1022427, 0.0, 120.0, typeof(RawFishSteak), 1044476, 1, 1044253);
            SetNeedHeat(index, true);
            SetUseAllRes(index, true);

            index = AddCraft(1, typeof(FishSteak), 1044498, "fish steak (single)", 0.0, 120.0, typeof(RawFishSteak), 1044476, 1, 1044253);
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(FriedEggs), 1044498, 1022486, 0.0, 100.0, typeof(Eggs), 1044477, 1, 1044253);
            SetNeedHeat(index, true);
            SetUseAllRes(index, true);

            index = AddCraft(1, typeof(LambLeg), 1044498, 1025642, 0.0, 100.0, typeof(RawLambLeg), 1044478, 1, 1044253);
            SetNeedHeat(index, true);
            SetUseAllRes(index, true);

            index = AddCraft(1, typeof(Ribs), 1044498, 1022546, 0.0, 120.0, typeof(RawRibs), 1044485, 1, 1044253);
            SetNeedHeat(index, true);
            SetUseAllRes(index, true);

            index = AddCraft(1, typeof(Ribs), 1044498, "cut of ribs (single)", 0.0, 120.0, typeof(RawRibs), 1044485, 1, 1044253);
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(Lobster), 1044498, "lobster", 0.0, 100.0, typeof(RawLobster), "Raw Lobster", 1, 1044253);
            SetNeedHeat(index, true);
            SetUseAllRes(index, true);

            index = AddCraft(1, typeof(Crab), 1044498, "crab", 0.0, 100.0, typeof(RawCrab), "Raw Crab", 1, 1044253);
            SetNeedHeat(index, true);
            SetUseAllRes(index, true);

            //Specialty Items
            index = AddCraft(1, typeof(Server.Custom.Items.VeterinarySalts), "Specialty Items", "Veterinary Salts", 88.0, 100.0, typeof(Server.Custom.Items.RareBerries), "rare berries", 1, "You do not have the neccessary rare berries required to cook this.");
            AddRes(index, typeof(Bottle), "bottle", 1, "You do not have the neccesary bottle required to cook this.");
            SetNeedHeat(index, true);

            //Luthius Expansion            
            index = AddCraft(1, typeof(FaintLure), "Specialty Items", "Faint Lure", 40, 80, typeof(RawFishSteak), "Raw Fish Steak", 100, "You do not have the neccessary raw fish steaks required to cook this.");
            AddRes(index, typeof(CockatriceEgg), "Cockatrice Egg", 1, "You do not have the neccesary crafting component needed to make this");

            index = AddCraft(1, typeof(PotentLure), "Specialty Items", "Potent Lure", 60, 100, typeof(RawFishSteak), "Raw Fish Steak", 150, "You do not have the neccessary raw fish steaks required to cook this.");
            AddRes(index, typeof(CockatriceEgg), "Cockatrice Egg", 1, "You do not have the neccesary crafting component needed to make this");

            index = AddCraft(1, typeof(IrresistibleLure), "Specialty Items", "Irresistable Lure", 80, 120, typeof(RawFishSteak), "Raw Fish Steak", 200, "You do not have the neccessary raw fish steaks required to cook this.");
            AddRes(index, typeof(CockatriceEgg), "Cockatrice Egg", 1, "You do not have the neccesary crafting component needed to make this");

            //Kin Paints
            index = AddCraft(1, typeof(TribalKinPaint), "Kin Paints", "tribal kin paint", 60.0, 65.0, typeof(Bloodroot), "bloodroot", 1, "You do not have the neccessary bloodroot required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(DrowKinPaint), "Kin Paints", "drow kin paint", 60.0, 65.0, typeof(StrangeMushrooms), "strange mushrooms", 1, "You do not have the neccessary strange mushrooms required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UndeadKinPaint), "Kin Paints", "undead kin paint", 60.0, 65.0, typeof(Bonemeal), "bonemeal", 1, "You do not have the neccessary bonemeal required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(GreenOrcKinPaint), "Kin Paints", "green orc kin paint", 60.0, 65.0, typeof(CaveMoss), "cave moss", 1, "You do not have the neccessary cave moss required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(BrownOrcKinPaint), "Kin Paints", "brown orc kin paint", 60.0, 65.0, typeof(CaveMoss), "cave moss", 1, "You do not have the neccessary cave moss required to cook this.");
            SetNeedHeat(index, true);
            */
        }        
    }
}