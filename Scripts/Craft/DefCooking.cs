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
            string missingIngredients = "You do not have all of the ingredients neccessary to make that.";

            int index = -1;

            //Ingredients
            index = AddCraft(5, typeof(Dough), "Ingredients", "Dough", 0.0, 25.0, typeof(SackOfFlour), "Sack of Flour", 1, missingIngredients);
            AddRes(index, typeof(BaseBeverage), "Water", 1, missingIngredients);

            //Meagre Food
            index = AddCraft(1, typeof(BreadRoll), "Meagre Food", "Bread Roll", 0.0, 100.0, typeof(Dough), "Dough", 1, missingIngredients);
            AddRes(index, typeof(BaseBeverage), "Water", 1, missingIngredients);

            AddCraft(1, typeof(CookedEggs), "Meagre Food", "Cooked Eggs", 0.0, 100.0, typeof(Eggs), "Eggs", 3, missingIngredients);
            AddCraft(1, typeof(CookedRibs), "Meagre Food", "Cooked Ribs", 0.0, 100.0, typeof(RawRibs), "Raw Ribs", 1, missingIngredients);
            AddCraft(1, typeof(CookedFishsteak), "Meagre Food", "Cooked Fishsteak", 0.0, 100.0, typeof(RawFishSteak), "Raw Fishsteak", 1, missingIngredients);
            AddCraft(1, typeof(CookedDrumstick), "Adequate Food", "Cooked Drumstick", 50.0, 120.0, typeof(RawDrumstick), "Raw Drumstick", 1, missingIngredients);
            
            index = AddCraft(1, typeof(GardenSalad), "Meagre Food", "Garden Salad", 0.0, 100.0, typeof(Lettuce), "Lettuce", 1, missingIngredients);
            AddRes(index, typeof(Carrot), "Carrot", 1, missingIngredients);
            AddRes(index, typeof(Turnip), "Turnip", 1, missingIngredients);

            index = AddCraft(1, typeof(FruitDish), "Meagre Food", "Fruit Dish", 0.0, 100.0, typeof(Pear), "Pear", 1, missingIngredients);
            AddRes(index, typeof(Grapes), "Grapes", 1, missingIngredients);
            AddRes(index, typeof(Honeydew), "Honeydew", 1, missingIngredients);

            //Adequate Food
            AddCraft(1, typeof(CookedSteaks), "Adequate Food", "Cooked Steaks", 50.0, 120.0, typeof(RawSteaks), "Raw Steaks", 1, missingIngredients);
            AddCraft(1, typeof(CookedHam), "Adequate Food", "Cooked Ham", 50.0, 120.0, typeof(RawHam), "Raw Ham", 1, missingIngredients);
            AddCraft(1, typeof(CookedBird), "Adequate Food", "Cooked Bird", 50.0, 120.0, typeof(RawBird), "Raw Bird", 1, missingIngredients);
            AddCraft(1, typeof(CookedFishFillet), "Adequate Food", "Cooked Fish Fillet", 50.0, 120.0, typeof(RawFishFillet), "Raw Fish Fillet", 1, missingIngredients);
            AddCraft(1, typeof(CookedLobster), "Adequate Food", "Cooked Lobster", 50.0, 120.0, typeof(RawLobster), "Raw Lobster", 1, missingIngredients);
            AddCraft(1, typeof(CookedCrab), "Adequate Food", "Cooked Crab", 50.0, 120.0, typeof(RawCrab), "Raw Crab", 1, missingIngredients);
            AddCraft(1, typeof(CookedMeatShank), "Adequate Food", "Cooked Meat Shank", 50.0, 120.0, typeof(RawMeatShank), "Raw Meat Shank", 1, missingIngredients);
            AddCraft(1, typeof(CookedBacon), "Adequate Food", "Cooked Bacon", 50.0, 120.0, typeof(RawBacon), "Raw Bacon", 1, missingIngredients);
            AddCraft(1, typeof(CookedSausage), "Adequate Food", "Cooked Sausage", 50.0, 120.0, typeof(RawSausage), "Raw Sausage", 1, missingIngredients);
            AddCraft(1, typeof(SearedMushrooms), "Adequate Food", "Seared Mushrooms", 50.0, 120.0, typeof(WildMushroom), "Wild Mushroom", 1, missingIngredients);

            index = AddCraft(1, typeof(HoneyBreadLoaves), "Adequate Food", "Honey Bread Loaves", 50.0, 120.0, typeof(Dough), "Dough", 1, missingIngredients);
            AddRes(index, typeof(JarOfHoney), "Jar Of Honey", 1, missingIngredients);
            AddRes(index, typeof(BaseBeverage), "Water", 1, missingIngredients);
            
            index = AddCraft(1, typeof(CheesePastry), "Adequate Food", "Cheese Pastry", 50.0, 120.0, typeof(Dough), "Dough", 1, missingIngredients);
            AddRes(index, typeof(Eggs), "Eggs", 3, missingIngredients);
            AddRes(index, typeof(CheeseWedge), "Cheese Wedge", 1, missingIngredients);

            index = AddCraft(1, typeof(Cornbread), "Adequate Food", "Cornbread", 50.0, 120.0, typeof(Dough), "Dough", 1, missingIngredients);
            AddRes(index, typeof(Corn), "Corn", 3, missingIngredients);
            AddRes(index, typeof(BaseBeverage), "Water", 1, missingIngredients);

            //Appetizing Food
            index = AddCraft(1, typeof(FigSweetrolls), "Appetizing Food", "Fig Sweetrolls", 75.0, 120.0, typeof(Dough), "Dough", 2, missingIngredients);
            AddRes(index, typeof(Figs), "Figs", 2, missingIngredients);
            AddRes(index, typeof(BaseBeverage), "Water", 1, missingIngredients);

            index = AddCraft(1, typeof(CoconutShortbread), "Adequate Food", "Coconut Shortbread", 75.0, 120.0, typeof(Dough), "Dough", 2, missingIngredients);
            AddRes(index, typeof(Coconut), "Coconut", 1, missingIngredients);
            AddRes(index, typeof(BaseBeverage), "Water", 1, missingIngredients);

            index = AddCraft(1, typeof(PumpkinSoup), "Appetizing Food", "Pumpkin Soup", 75.0, 120.0, typeof(Pumpkin), "Pumpkin", 3, missingIngredients);
            AddRes(index, typeof(Squash), "Squash", 2, missingIngredients);

            index = AddCraft(1, typeof(MushroomSoup), "Appetizing Food", "Mushroom Soup", 75.0, 120.0, typeof(WildMushroom), "Wild Mushroom", 3, missingIngredients);
            AddRes(index, typeof(Onion), "Onion", 2, missingIngredients);  

            index = AddCraft(1, typeof(CabbageStew), "Appetizing Food", "Cabbage Stew", 75.0, 120.0, typeof(Cabbage), "Cabbage", 3, missingIngredients);
            AddRes(index, typeof(Gourd), "Gourd", 2, missingIngredients);
            
            index = AddCraft(1, typeof(FruitPie), "Appetizing Food", "Fruit Pie", 75.0, 120.0, typeof(Dough), "Dough", 2, missingIngredients);
            AddRes(index, typeof(Apple), "Apple", 1, missingIngredients);
            AddRes(index, typeof(Peach), "Peach", 1, missingIngredients);
            AddRes(index, typeof(BaseBeverage), "Water", 1, missingIngredients);            

            index = AddCraft(1, typeof(MeatPie), "Appetizing Food", "Meat Pie", 75.0, 120.0, typeof(Dough), "Dough", 2, missingIngredients);
            AddRes(index, typeof(RawRibs), "Raw Ribs", 2, missingIngredients);
            AddRes(index, typeof(RawSausage), "Raw Sausage", 1, missingIngredients);
            AddRes(index, typeof(BaseBeverage), "Water", 1, missingIngredients);

            index = AddCraft(1, typeof(FishPie), "Appetizing Food", "Fish Pie", 75.0, 120.0, typeof(Dough), "Dough", 2, missingIngredients);
            AddRes(index, typeof(RawFishSteak), "Raw Fishsteak", 2, missingIngredients);
            AddRes(index, typeof(RawFishFillet), "Raw Fish Fillet", 1, missingIngredients);
            AddRes(index, typeof(BaseBeverage), "Water", 1, missingIngredients);

            index = AddCraft(1, typeof(ChickenPotPie), "Appetizing Food", "Chicken Pot Pie", 75.0, 120.0, typeof(Dough), "Dough", 2, missingIngredients);
            AddRes(index, typeof(RawDrumstick), "Raw Drumstick", 1, missingIngredients);
            AddRes(index, typeof(Carrot), "Carrot", 1, missingIngredients);
            AddRes(index, typeof(BaseBeverage), "Water", 1, missingIngredients);

            index = AddCraft(1, typeof(CrabPuffs), "Appetizing Food", "Crab Puffs", 75.0, 120.0, typeof(Dough), "Dough", 2, missingIngredients);
            AddRes(index, typeof(RawCrab), "Raw Crab", 2, missingIngredients);

            index = AddCraft(1, typeof(LobsterRolls), "Appetizing Food", "Lobster Rolls", 75.0, 120.0, typeof(Dough), "Dough", 2, missingIngredients);
            AddRes(index, typeof(RawLobster), "Raw Lobster", 2, missingIngredients);

            //Delectable Food
            index = AddCraft(1, typeof(BeefStew), "Delectable Food", "Beef Stew", 99.9, 120.0, typeof(RawRibs), "Raw Ribs", 1, missingIngredients);
            AddRes(index, typeof(RawSteaks), "Raw Steaks", 1, missingIngredients);
            AddRes(index, typeof(RawMeatShank), "Raw Meat Shank", 1, missingIngredients);
            AddRes(index, typeof(RawBacon), "Raw Bacon", 1, missingIngredients);

            index = AddCraft(1, typeof(RoastPig), "Delectable Food", "Roast Pig", 99.9, 120.0, typeof(RawHam), "Raw Ham", 4, missingIngredients);
            AddRes(index, typeof(Apple), "Apple", 1, missingIngredients);

            index = AddCraft(1, typeof(LemonLimeSearedFish), "Delectable Food", "Lemon-Lime Seared Fish", 99.9, 120.0, typeof(RawFishFillet), "Raw Fish Fillet", 3, missingIngredients);
            AddRes(index, typeof(Lemon), "Lemon", 1, missingIngredients);
            AddRes(index, typeof(Lemon), "Lime", 1, missingIngredients);

            index = AddCraft(1, typeof(BananaCake), "Delectable Food", "Banana Cake", 99.9, 120.0, typeof(Dough), "Dough", 5, missingIngredients);
            AddRes(index, typeof(Eggs), "Eggs", 5, missingIngredients);
            AddRes(index, typeof(Banana), "Banana", 3, missingIngredients);            
            AddRes(index, typeof(BaseBeverage), "Water", 1, missingIngredients);
        }        
    }
}