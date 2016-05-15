using System;
using Server.Items;
using Server.Custom;

namespace Server.Engines.Craft
{
    public class UOACZDefCooking : CraftSystem
    {
        #region Base

        public override SkillName MainSkill
        {
            get { return SkillName.Cooking; }
        }

        public override int GumpTitleNumber
        {
            get { return 1044003; } // <CENTER>COOKING MENU</CENTER>
        }

        public static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new UOACZDefCooking();

                return m_CraftSystem;
            }
        }

        public override CraftExceptionalChanceMode ECA { get { return CraftExceptionalChanceMode.SuccessChanceDivideByFive; } }

        public override double GetChanceAtMin(CraftItem item)
        {
            return 0.0; // 0%
        }

        public UOACZDefCooking()
            : base(1, 1, 1.25)// base( 1, 1, 1.5 )
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

        #endregion

        public override void InitCraftList()
        {
            int index = -1;

            //Corrupted
            index = AddCraft(1, typeof(UOACZCookedCorruptedDrumstick), "Corrupted Items", "Cooked Corrupted Drumstick", 10, 35, typeof(UOACZCorruptedRawDrumstick), "Corrupted Raw Drumstick", 1, "You do not have the neccessary corrupted raw drumstick required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZCookedCorruptedHam), "Corrupted Items", "Cooked Corrupted Ham", 10, 35, typeof(UOACZCorruptedRawHam), "Corrupted Raw Ham", 1, "You do not have the neccessary corrupted raw ham required to cook this.");
            SetNeedHeat(index, true);           

            index = AddCraft(1, typeof(UOACZCookedCorruptedMeatScraps), "Corrupted Items", "Cooked Corrupted Meat Scraps", 10, 35, typeof(UOACZCorruptedRawMeatScraps), "Corrupted Raw Meat Scraps", 1, "You do not have the neccessary corrupted raw meat scraps required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZCookedCorruptedMeatShank), "Corrupted Items", "Cooked Corrupted Meat Shank", 10, 35, typeof(UOACZCorruptedRawMeatShank), "Corrupted Raw Meat Shank", 1, "You do not have the neccessary corrupted raw meat shank required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZCookedCorruptedSteak), "Corrupted Items", "Cooked Corrupted Steak", 10, 35, typeof(UOACZCorruptedRawSteak), "Corrupted Raw Steak", 1, "You do not have the neccessary corrupted raw steak required to cook this.");
            SetNeedHeat(index, true);

            //Baked Goods
            index = AddCraft(1, typeof(UOACZBowlOfDough), "Baked Goods", "Bowl of Dough", 5, 30, typeof(UOACZSackOfFlour), "Sack of Flour", 1, "You do not have the neccessary sack of flour required to cook this.");
            AddRes(index, typeof(UOACZBowl), "Bowl", 1, "You do not have the neccessary bowl required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZBreadRolls), "Baked Goods", "Bread Rolls", 5, 30, typeof(UOACZDough), "Dough", 3, "You do not have the neccessary dough required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZBreadLoaves), "Baked Goods", "Bread Loaves", 5, 30, typeof(UOACZDough), "Dough", 2, "You do not have the neccessary dough required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZHoneyBread), "Baked Goods", "Honey Bread", 10, 35, typeof(UOACZDough), "Dough", 1, "You do not have the neccessary dough required to cook this.");
            AddRes(index, typeof(UOACZHoney), "Honey", 1, "You do not have the neccessary honey required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZBakedPie), "Baked Goods", "Baked Pie", 15, 40, typeof(UOACZDough), "Dough", 1, "You do not have the neccessary dough required to cook this.");
            AddRes(index, typeof(UOACZFruit), "Fruit", 1, "You do not have the neccessary fruit required to cook this.");
            AddRes(index, typeof(UOACZBowl), "Bowl", 1, "You do not have the neccessary bowl required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZBakedPie), "Baked Goods", "Meat Pie", 15, 40, typeof(UOACZDough), "Dough", 1, "You do not have the neccessary dough required to cook this.");
            AddRes(index, typeof(UOACZRawCutsOfMeat), "Raw Cuts of Meat", 1, "You do not have the neccessary raw cuts of meat required to cook this.");
            AddRes(index, typeof(UOACZBowl), "Bowl", 1, "You do not have the neccessary bowl required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZTrayOfRolls), "Baked Goods", "Tray of Rolls", 10, 35, typeof(UOACZDough), "Dough", 3, "You do not have the neccessary dough required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZFruitCake), "Baked Goods", "Fruit Cake", 25, 50, typeof(UOACZDough), "Dough", 2, "You do not have the neccessary dough required to cook this.");
            AddRes(index, typeof(UOACZFruit), "Fruit", 2, "You do not have the neccessary fruit required to cook this.");
            AddRes(index, typeof(UOACZEggs), "Eggs", 2, "You do not have the neccessary eggs required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZCheesePastry), "Baked Goods", "Cheese Pastry", 10, 35, typeof(UOACZDough), "Dough", 1, "You do not have the neccessary dough required to cook this.");
            AddRes(index, typeof(UOACZEggs), "Eggs", 1, "You do not have the neccessary eggs required to cook this.");
            AddRes(index, typeof(UOACZCheeseWheel), "Cheese", 1, "You do not have the neccessary cheese wheel required to cook this.");            
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZHerbShortbread), "Baked Goods", "Herb Shortbread", 10, 35, typeof(UOACZHerbs), "Herbs", 1, "You do not have the neccessary herbs required to cook this.");
            AddRes(index, typeof(UOACZDough), "Dough", 1, "You do not have the neccessary dough required to cook this.");
            SetNeedHeat(index, true);

            //Meat Items
            index = AddCraft(1, typeof(UOACZCookedBacon), "Meat Items", "Cooked Bacon", 10, 35, typeof(UOACZRawBacon), "Raw Bacon", 1, "You do not have the neccessary raw bacon required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZCookedHam), "Meat Items", "Cooked Ham", 10, 35, typeof(UOACZRawHam), "Raw Ham", 1, "You do not have the neccessary raw ham required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZCookedMeatScraps), "Meat Items", "Cooked Meat Scrap", 10, 35, typeof(UOACZRawMeatScraps), "Raw Meat Scrap", 1, "You do not have the neccessary raw meat scrap required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZCookedMeatShank), "Meat Items", "Cooked Meat Shank", 10, 35, typeof(UOACZRawMeatShank), "Raw Meat Shank", 1, "You do not have the neccessary raw meat shank required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZCookedSausage), "Meat Items", "Cooked Sausage", 10, 35, typeof(UOACZRawSausage), "Raw Sausage", 1, "You do not have the neccessary raw sausage required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZCookedSteak), "Meat Items", "Cooked Steak", 10, 35, typeof(UOACZRawSteak), "Raw Steak", 1, "You do not have the neccessary raw steak required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZMushroomSteak), "Meat Items", "Mushroom Steak", 10, 35, typeof(UOACZRawSteak), "Raw Steak", 1, "You do not have the neccessary raw steak required to cook this.");
            AddRes(index, typeof(UOACZMushrooms), "Mushrooms", 1, "You do not have the neccessary mushrooms required to cook this.");
            SetNeedHeat(index, true);

            //Poultry Items
            index = AddCraft(1, typeof(UOACZCookedBird), "Poultry Items", "Cooked Bird", 10, 35, typeof(UOACZRawBird), "Raw Bird", 1, "You do not have the neccessary raw bird required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZCookedDrumstick), "Poultry Items", "Cooked Drumstick", 10, 35, typeof(UOACZRawDrumstick), "Raw Drumstick", 1, "You do not have the neccessary raw drumstick required to cook this.");
            SetNeedHeat(index, true);

            //Fish Items
            index = AddCraft(1, typeof(UOACZCookedFishsteak), "Fish Items", "Cooked Fishsteak", 10, 35, typeof(UOACZRawFishsteak), "Raw Fishsteak", 1, "You do not have the neccessary raw fishsteak required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZCuredFish), "Fish Items", "Cured Fish", 5, 30, typeof(UOACZRawFishsteak), "Raw Fishsteak", 1, "You do not have the neccessary raw fishsteak required to cook this.");
            //SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZCuredLargeFish), "Fish Items", "Large Cured Fish", 15, 40, typeof(UOACZRawFishsteak), "Raw Fishsteak", 3, "You do not have the neccessary raw fishsteak required to cook this.");
            //SetNeedHeat(index, true);            

            //Meals
            index = AddCraft(1, typeof(UOACZPreparedMeal), "Meals", "Prepared Meal", 10, 35, typeof(UOACZRawCutsOfMeat), "Raw Cuts of Meat", 1, "You do not have the neccessary raw cuts of meat required to cook this.");
            AddRes(index, typeof(UOACZFruit), "Fruit", 1, "You do not have the neccessary fruit required to cook this.");
            AddRes(index, typeof(UOACZVegetable), "Vegetable", 1, "You do not have the neccessary vegetable required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZRoastPig), "Meals", "Roast Pig", 25, 50, typeof(UOACZRawHam), "Raw Ham", 3, "You do not have the neccessary raw ham required to cook this.");
            AddRes(index, typeof(UOACZVegetable), "Vegetable", 1, "You do not have the neccessary vegetable required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZSalad), "Meals", "Salad", 10, 35, typeof(UOACZVegetable), "Vegetable", 2, "You do not have the neccessary vegetable required to cook this.");
            AddRes(index, typeof(UOACZBowl), "Bowl", 1, "You do not have the neccessary bowl required to cook this.");
            //SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZVegetableStew), "Meals", "Vegetable Stew", 10, 35, typeof(UOACZVegetable), "Vegetable", 3, "You do not have the neccessary vegetable required to cook this.");
            AddRes(index, typeof(UOACZBowl), "Bowl", 1, "You do not have the neccessary bowl required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZPumpkinSoup), "Meals", "Pumpkin Soup", 10, 35, typeof(Pumpkin), "Pumpkin", 2, "You do not have the neccessary pumpkin required to cook this.");
            AddRes(index, typeof(UOACZBowl), "Bowl", 1, "You do not have the neccessary bowl required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZMushroomSoup), "Meals", "Mushroom Soup", 10, 35, typeof(UOACZMushrooms), "Mushrooms", 1, "You do not have the neccessary mushrooms required to cook this.");
            AddRes(index, typeof(UOACZBowl), "Bowl", 1, "You do not have the neccessary bowl required to cook this.");
            SetNeedHeat(index, true);

            //Other Items
            index = AddCraft(1, typeof(UOACZCheeseWheel), "Other Items", "Cheese Wheel", 5, 30, typeof(UOACZGlassOfMilk), "Glass of Milk", 1, "You do not have the neccessary glass of milk required to cook this.");
            //SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZCookedEggs), "Other Items", "Cooked Eggs", 5, 30, typeof(UOACZEggs), "Eggs", 1, "You do not have the neccessary eggs required to cook this.");
            SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZBottleOfFruitJuice), "Other Items", "Fruit Juice", 5, 30, typeof(UOACZFruit), "Fruit", 1, "You do not have the neccessary fruit required to cook this.");
            AddRes(index, typeof(Bottle), "Bottle", 1, "You do not have the neccessary bottle required to cook this.");
            //SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZFruitMedley), "Other Items", "Fruit Medley", 10, 35, typeof(UOACZFruit), "Fruit", 3, "You do not have the neccessary fruit required to cook this.");
            AddRes(index, typeof(UOACZBowl), "Bowl", 1, "You do not have the neccessary bowl required to cook this.");
            //SetNeedHeat(index, true);

            index = AddCraft(1, typeof(UOACZHerbBasket), "Other Items", "Herb Basket", 5, 30, typeof(UOACZHerbs), "Herbs", 1, "You do not have the neccessary herbs required to cook this.");
            AddRes(index, typeof(UOACZBowl), "Bowl", 1, "You do not have the neccessary bowl required to cook this.");
            //SetNeedHeat(index, true);            
        }
    }
}