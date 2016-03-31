using System;
using Server.Items;
using Server.Custom;

namespace Server.Engines.Craft
{
    public class DefTailoring : CraftSystem
    {
        public override SkillName MainSkill
        {
            get { return SkillName.Tailoring; }
        }

        public override int GumpTitleNumber
        {
            get { return 1044005; } // <CENTER>TAILORING MENU</CENTER>
        }

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new DefTailoring();

                return m_CraftSystem;
            }
        }

        public override CraftECA ECA { get { return CraftECA.ChanceMinusSixtyToFourtyFive; } }

        public override double GetChanceAtMin(CraftItem item)
        {
            return 0.5; // 50%
        }

        private DefTailoring()
            : base(1, 1, 1.25)// base( 1, 1, 4.5 )
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

        private static Type[] m_TailorColorables = new Type[]
			{
				typeof( GozaMatEastDeed ), typeof( GozaMatSouthDeed ),
				typeof( SquareGozaMatEastDeed ), typeof( SquareGozaMatSouthDeed ),
				typeof( BrocadeGozaMatEastDeed ), typeof( BrocadeGozaMatSouthDeed ),
				typeof( BrocadeSquareGozaMatEastDeed ), typeof( BrocadeSquareGozaMatSouthDeed )
			};

        public override bool RetainsColorFrom(CraftItem item, Type type)
        {
            if (type != typeof(Cloth) && type != typeof(UncutCloth))
                return false;

            type = item.ItemType;

            bool contains = false;

            for (int i = 0; !contains && i < m_TailorColorables.Length; ++i)
                contains = (m_TailorColorables[i] == type);

            return contains;
        }

        public override void PlayCraftEffect(Mobile from)
        {
            from.PlaySound(0x248);
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

            //Leather
            AddCraft(1, typeof(LeatherGorget), "Leather Armor", "Leather Gorget", 53.9, 78.9, typeof(Leather), 1044462, 4, 1044463);
            AddCraft(1, typeof(LeatherCap), "Leather Armor", "Leather Cap", 6.2, 31.2, typeof(Leather), 1044462, 2, 1044463);
            AddCraft(1, typeof(LeatherGloves), "Leather Armor", "Leather Gloves", 51.8, 76.8, typeof(Leather), 1044462, 3, 1044463);
            AddCraft(1, typeof(LeatherArms), "Leather Armor", "Leather Arms", 53.9, 78.9, typeof(Leather), 1044462, 4, 1044463);
            AddCraft(1, typeof(LeatherLegs), "Leather Armor", "Leather Legs", 66.3, 91.3, typeof(Leather), 1044462, 10, 1044463);
            AddCraft(1, typeof(LeatherShorts), "Leather Armor", "Leather Shorts", 62.2, 87.2, typeof(Leather), 1044462, 8, 1044463);
            AddCraft(1, typeof(LeatherSkirt), "Leather Armor", "Leather Skirt", 58.0, 83.0, typeof(Leather), 1044462, 6, 1044463);
            AddCraft(1, typeof(LeatherBustierArms), "Leather Armor", "Leather Bustier Arms", 58.0, 83.0, typeof(Leather), 1044462, 6, 1044463);
            AddCraft(1, typeof(FemaleLeatherChest), "Leather Armor", "Female Leather Chest", 62.2, 87.2, typeof(Leather), 1044462, 8, 1044463);
            AddCraft(1, typeof(LeatherChest), "Leather Armor", "Leather Chest", 70.5, 95.5, typeof(Leather), 1044462, 12, 1044463);

            //Studded
            AddCraft(1, typeof(StuddedGorget), "Studded Leather Armor", "Studded Gorget", 60, 90, typeof(Leather), 1044462, 6, 1044463);
            AddCraft(1, typeof(StuddedGloves), "Studded Leather Armor", "Studded Gloves", 55, 85, typeof(Leather), 1044462, 8, 1044463);
            AddCraft(1, typeof(StuddedArms), "Studded Leather Armor", "Studded Arms", 65, 95, typeof(Leather), 1044462, 10, 1044463);
            AddCraft(1, typeof(StuddedLegs), "Studded Leather Armor", "Studded Legs", 65, 95, typeof(Leather), 1044462, 12, 1044463);
            AddCraft(1, typeof(StuddedBustierArms), "Studded Leather Armor", "Studded Bustier Arms", 70, 100, typeof(Leather), 1044462, 8, 1044463);
            AddCraft(1, typeof(StuddedChest), "Studded Leather Armor", "Studded Chest", 75, 105, typeof(Leather), 1044462, 14, 1044463);
            AddCraft(1, typeof(FemaleStuddedChest), "Studded Leather Armor", "Studded Female Chest", 70, 100, typeof(Leather), 1044462, 10, 1044463);

            //Bone Armor
            index = AddCraft(1, typeof(BoneGloves), "Bone Armor", "Bone Gloves", 89.0, 114.0, typeof(Leather), 1044462, 6, 1044463);
            AddRes(index, typeof(Bone), 1049064, 2, 1049063);

            index = AddCraft(1, typeof(BoneHelm), "Bone Armor", "Bone Helm", 85.0, 110.0, typeof(Leather), 1044462, 4, 1044463);
            AddRes(index, typeof(Bone), 1049064, 2, 1049063);

            index = AddCraft(1, typeof(BoneArms), "Bone Armor", "Bone Arms", 92.0, 117.0, typeof(Leather), 1044462, 8, 1044463);
            AddRes(index, typeof(Bone), 1049064, 4, 1049063);

            index = AddCraft(1, typeof(BoneLegs), "Bone Armor", "Bone Legs", 95.0, 120.0, typeof(Leather), 1044462, 10, 1044463);
            AddRes(index, typeof(Bone), 1049064, 6, 1049063);

            index = AddCraft(1, typeof(BoneChest), "Bone Armor", "Bone Chest", 96.0, 121.0, typeof(Leather), 1044462, 12, 1044463);
            AddRes(index, typeof(Bone), 1049064, 10, 1049063);

            //Hats
            AddCraft(1, typeof(SkullCap), "Hats", "Skull Cap", 0.0, 25.0, typeof(Cloth), 1044286, 2, 1044287);
            AddCraft(1, typeof(Bandana), "Hats", "Bandana", 0.0, 25.0, typeof(Cloth), 1044286, 2, 1044287);
            AddCraft(1, typeof(FloppyHat), "Hats", "Floppy Hat", 6.2, 31.2, typeof(Cloth), 1044286, 11, 1044287);
            AddCraft(1, typeof(Cap), "Hats", "Cap", 6.2, 31.2, typeof(Cloth), 1044286, 11, 1044287);
            AddCraft(1, typeof(WideBrimHat), "Hats", "Wide Brim Hat", 6.2, 31.2, typeof(Cloth), 1044286, 12, 1044287);
            AddCraft(1, typeof(StrawHat), "Hats", "Straw Hat", 6.2, 31.2, typeof(Cloth), 1044286, 10, 1044287);
            AddCraft(1, typeof(TallStrawHat), "Hats", "Tall Straw Hat", 6.7, 31.7, typeof(Cloth), 1044286, 13, 1044287);
            AddCraft(1, typeof(Bonnet), "Hats", "Bonnet", 6.2, 31.2, typeof(Cloth), 1044286, 11, 1044287);
            AddCraft(1, typeof(FeatheredHat), "Hats", "Feathered Hat", 6.2, 31.2, typeof(Cloth), 1044286, 12, 1044287);
            AddCraft(1, typeof(TricorneHat), "Hats", "Tricorne Hat", 6.2, 31.2, typeof(Cloth), 1044286, 12, 1044287);
            AddCraft(1, typeof(JesterHat), "Hats", "Jester Hat", 7.2, 32.2, typeof(Cloth), 1044286, 15, 1044287);
            AddCraft(1, typeof(WizardsHat), "Hats", "Wizard's Hat", 7.2, 32.2, typeof(Cloth), 1044286, 15, 1044287);

            //Shirts
            AddCraft(1, typeof(Doublet), "Shirts", "Doublet", 0, 25.0, typeof(Cloth), 1044286, 8, 1044287);
            AddCraft(1, typeof(Shirt), "Shirts", "Shirt", 20.7, 45.7, typeof(Cloth), 1044286, 8, 1044287);
            AddCraft(1, typeof(FancyShirt), "Shirts", "Fancy Shirt", 24.8, 49.8, typeof(Cloth), 1044286, 8, 1044287);
            AddCraft(1, typeof(Tunic), "Shirts", "Tunic", 00.0, 25.0, typeof(Cloth), 1044286, 12, 1044287);
            AddCraft(1, typeof(Surcoat), "Shirts", "Surcoat", 8.2, 33.2, typeof(Cloth), 1044286, 14, 1044287);
            AddCraft(1, typeof(Cloak), "Shirts", "Cloak", 41.4, 66.4, typeof(Cloth), 1044286, 14, 1044287);
            AddCraft(1, typeof(JesterSuit), "Shirts", "Jester Suit", 8.2, 33.2, typeof(Cloth), 1044286, 24, 1044287);
            AddCraft(1, typeof(PlainDress), "Shirts", "Plain Dress", 12.4, 37.4, typeof(Cloth), 1044286, 10, 1044287);
            AddCraft(1, typeof(FancyDress), "Shirts", "Fancy Dress", 33.1, 58.1, typeof(Cloth), 1044286, 12, 1044287);
            AddCraft(1, typeof(Robe), "Shirts", "Robe", 53.9, 78.9, typeof(Cloth), 1044286, 16, 1044287);

            //Pants
            AddCraft(1, typeof(ShortPants), "Pants", "Short Pants", 24.8, 49.8, typeof(Cloth), 1044286, 6, 1044287);
            AddCraft(1, typeof(LongPants), "Pants", "Long Pants", 24.8, 49.8, typeof(Cloth), 1044286, 8, 1044287);
            AddCraft(1, typeof(Kilt), "Pants", "Kilt", 20.7, 45.7, typeof(Cloth), 1044286, 8, 1044287);
            AddCraft(1, typeof(Skirt), "Pants", "Skirt", 29.0, 54.0, typeof(Cloth), 1044286, 10, 1044287);

            //Other Wearables
            AddCraft(1, typeof(Sandals), "Other Wearables", "Sandals", 12.4, 37.4, typeof(Leather), 1044462, 4, 1044463);
            AddCraft(1, typeof(Shoes), "Other Wearables", "Shoes", 16.5, 41.5, typeof(Leather), 1044462, 6, 1044463);
            AddCraft(1, typeof(Boots), "Other Wearables", "Boots", 33.1, 58.1, typeof(Leather), 1044462, 8, 1044463);
            AddCraft(1, typeof(ThighBoots), "Other Wearables", "Thigh Boots", 41.4, 66.4, typeof(Leather), 1044462, 10, 1044463);
            AddCraft(1, typeof(BodySash), "Other Wearables", "Body Sash", 4.1, 29.1, typeof(Cloth), 1044286, 4, 1044287);
            AddCraft(1, typeof(HalfApron), "Other Wearables", "Half Apron", 20.7, 45.7, typeof(Cloth), 1044286, 6, 1044287);
            AddCraft(1, typeof(FullApron), "Other Wearables", "Full Apron", 29.0, 54.0, typeof(Cloth), 1044286, 10, 1044287);

            //Ship Items
            AddCraft(1, typeof(OilCloth), "Misc", "Oil Cloth", 74.6, 99.6, typeof(Cloth), 1044286, 1, 1044287);

            AddCraft(1, typeof(BoardingRope), "Misc", "Boarding Rope", 50, 75, typeof(Cloth), "Cloth", 25, "You do not have the neccesary cloth to construct a boarding rope.");

            index = AddCraft(1, typeof(SpidersilkFishingNet), "Misc", "Spidersilk Fishing Net", 80.0, 120.0, typeof(Cloth), 1044286, 50, 1044287);
            AddRes(index, typeof(SpidersSilk), 1044360, 50, 1044253);
            AddRes(index, typeof(BluecapMushroom), "bluecap mushroom", 1, "You do not have the neccesary crafting component needed to make this");

            // Set the overridable material
            SetSubRes(typeof(Leather), "Leather");

            // Add every material you want the player to be able to choose from
            // This will override the overridable material
            AddSubRes(typeof(Leather), "Leather", 00.0, 1049311);
            AddSubRes(typeof(SpinedLeather), "Spined", 65.0, 1049311);
            AddSubRes(typeof(HornedLeather), "Horned", 80.0, 1049311);
            AddSubRes(typeof(BarbedLeather), "Barbed", 99.0, 1049311);

            Recycle = true;
            MarkOption = true;
            Repair = true;
        }
    }
}
