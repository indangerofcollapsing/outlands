using System;
using Server.Items;
using Server.Custom;

namespace Server.Engines.Craft
{
    public class UOACZDefTailoring : CraftSystem
    {
        #region Base

        public override SkillName MainSkill
        {
            get { return SkillName.Tailoring; }
        }

        public override int GumpTitleNumber
        {
            get { return 1044005; } // <CENTER>TAILORING MENU</CENTER>
        }

        public static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new UOACZDefTailoring();

                return m_CraftSystem;
            }
        }

        public override CraftECA ECA { get { return CraftECA.ChanceMinusSixtyToFourtyFive; } }

        public override double GetChanceAtMin(CraftItem item)
        {
            return 0.5; // 50%
        }

        public UOACZDefTailoring()
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

        public static Type[] m_TailorColorables = new Type[]
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

        #endregion

        public override void InitCraftList()
        {
            int index = -1;

            //Cloth Bolts and Utility
            AddCraft(1, typeof(UOACZBoltOfCloth), "Cloth Bolts and Utility", "Bolt of Cloth (from wool)", 0, 0, typeof(UOACZWool), "Wool", 1, "You do not have the wool neccessary in order to craft that.");
            AddCraft(1, typeof(UOACZBoltOfCloth), "Cloth Bolts and Utility", "Bolt of Cloth (from cotton)", 0, 0, typeof(UOACZCotton), "Cotton", 1, "You do not have the cotton neccessary in order to craft that.");
            
            AddCraft(1, typeof(UOACZThrowingNet), "Cloth Bolts and Utility", "Throwing Net", 5, 30, typeof(UOACZRope), "Rope", 3, "You do not have the rope neccessary in order to craft that.");
            AddCraft(1, typeof(UOACZRope), "Cloth Bolts and Utility", "Rope (from cloth)", 5, 30, typeof(Cloth), "Cloth", 1, "You do not have the cloth neccessary in order to craft that.");
            AddCraft(1, typeof(UOACZRope), "Cloth Bolts and Utility", "Rope (from intestines)", 5, 30, typeof(UOACZIntestines), "Intestines", 1, "You do not have the intestines neccessary in order to craft that.");
            AddCraft(1, typeof(Bandage), "Cloth Bolts and Utility", "Bandage (from intestine)", 5, 30, typeof(UOACZIntestines), "Intestines", 1, "You do not have the intestines neccessary in order to craft that.");
            AddCraft(1, typeof(Bedroll), "Cloth Bolts and Utility", "Bedroll (from cloth)", 5, 30, typeof(Cloth), "Cloth", 2, "You do not have the cloth neccessary in order to craft that.");
            AddCraft(1, typeof(Bedroll), "Cloth Bolts and Utility", "Bedroll (from leather)", 5, 30, typeof(Leather), "Leather", 2, "You do not have the leather neccessary in order to craft that.");

            AddCraft(1, typeof(Backpack), "Cloth Bolts and Utility", "Backpack", 5, 30, typeof(Leather), "Leather", 1, "You do not have the leather neccessary in order to craft that.");
            AddCraft(1, typeof(Bag), "Cloth Bolts and Utility", "Bag", 5, 30, typeof(Leather), "Leather", 1, "You do not have the leather neccessary in order to craft that.");
            AddCraft(1, typeof(Pouch), "Cloth Bolts and Utility", "Pouch", 5, 30, typeof(Leather), "Leather", 1, "You do not have the leather neccessary in order to craft that.");

            //Leather Armor
            AddCraft(1, typeof(LeatherGorget), "Leather Armor", "Leather Gorget", 5, 30, typeof(Leather), 1044462, 1, 1044463);
            AddCraft(1, typeof(LeatherGloves), "Leather Armor", "Leather Gloves", 5, 30, typeof(Leather), 1044462, 1, 1044463);
            AddCraft(1, typeof(LeatherCap), "Leather Armor", "Leather Cap", 5, 30, typeof(Leather), 1044462, 1, 1044463);
            AddCraft(1, typeof(LeatherArms), "Leather Armor", "Leather Arms", 5, 30, typeof(Leather), 1044462, 1, 1044463);
            AddCraft(1, typeof(LeatherLegs), "Leather Armor", "Leather Legs", 5, 30, typeof(Leather), 1044462, 2, 1044463);
            AddCraft(1, typeof(LeatherShorts), "Leather Armor", "Leather Shorts", 5, 30, typeof(Leather), 1044462, 2, 1044463);
            AddCraft(1, typeof(LeatherSkirt), "Leather Armor", "Leather Skirt", 5, 30, typeof(Leather), 1044462, 2, 1044463);
            AddCraft(1, typeof(LeatherChest), "Leather Armor", "Leather Chest", 5, 30, typeof(Leather), 1044462, 3, 1044463);
            AddCraft(1, typeof(LeatherBustier), "Leather Armor", "Leather Bustier", 5, 30, typeof(Leather), 1044462, 3, 1044463);
            AddCraft(1, typeof(FemaleLeatherChest), "Leather Armor", "Female Leather Chest", 5, 30, typeof(Leather), 1044462, 3, 1044463);

            //Studded Armor
            AddCraft(1, typeof(StuddedGorget), "Studded Leather Armor", "Studded Gorget", 10, 35, typeof(Leather), 1044462, 1, 1044463);
            AddCraft(1, typeof(StuddedGloves), "Studded Leather Armor", "Studded Gloves", 10, 35, typeof(Leather), 1044462, 1, 1044463);
            AddCraft(1, typeof(StuddedArms), "Studded Leather Armor", "Studded Arms", 10, 35, typeof(Leather), 1044462, 2, 1044463);
            AddCraft(1, typeof(StuddedLegs), "Studded Leather Armor", "Studded Legs", 10, 35, typeof(Leather), 1044462, 3, 1044463);
            AddCraft(1, typeof(StuddedChest), "Studded Leather Armor", "Studded Chest", 10, 35, typeof(Leather), 1044462, 4, 1044463);
            AddCraft(1, typeof(StuddedBustier), "Studded Leather Armor", "Studded Bustier", 10, 35, typeof(Leather), 1044462, 4, 1044463);
            AddCraft(1, typeof(FemaleStuddedChest), "Studded Leather Armor", "Female Studded Chest", 10, 35, typeof(Leather), 1044462, 4, 1044463);

            //Bone Armor
            index = AddCraft(1, typeof(UOACZBoneGloves), "Bone Armor", "Bone Gloves", 15, 40, typeof(UOACZCrudeBoneGloves), "Crude Bone Gloves", 1, "You do not have the crude bone gloves neccesary to craft this.");
            AddRes(index, typeof(Leather), "Leather", 1, "You do not have the neccessary leather required to make this.");
            AddRes(index, typeof(UOACZIntestines), "Intestine", 1, "You do not have the neccessary intestine required to make this.");

            index = AddCraft(1, typeof(UOACZBoneHelm), "Bone Armor", "Bone Helm", 15, 40, typeof(UOACZCrudeBoneHelm), "Crude Bone Helm", 1, "You do not have the crude bone helm neccesary to craft this.");
            AddRes(index, typeof(Leather), "Leather", 1, "You do not have the neccessary leather required to make this.");
            AddRes(index, typeof(UOACZIntestines), "Intestine", 1, "You do not have the neccessary intestine required to make this.");

            index = AddCraft(1, typeof(UOACZBoneArms), "Bone Armor", "Bone Arms", 15, 40, typeof(UOACZCrudeBoneArms), "Crude Bone Arms", 1, "You do not have the crude bone arms neccesary to craft this.");
            AddRes(index, typeof(Leather), "Leather", 1, "You do not have the neccessary leather required to make this.");
            AddRes(index, typeof(UOACZIntestines), "Intestine", 1, "You do not have the neccessary intestine required to make this.");

            index = AddCraft(1, typeof(UOACZBoneLegs), "Bone Armor", "Bone Legs", 15, 40, typeof(UOACZCrudeBoneLegs), "Crude Bone Legs", 1, "You do not have the crude bone legs neccesary to craft this.");
            AddRes(index, typeof(Leather), "Leather", 2, "You do not have the neccessary leather required to make this.");
            AddRes(index, typeof(UOACZIntestines), "Intestine", 1, "You do not have the neccessary intestine required to make this.");

            index = AddCraft(1, typeof(UOACZBoneChest), "Bone Armor", "Bone Chest", 15, 40, typeof(UOACZCrudeBoneChest), "Crude Bone Chest", 1, "You do not have the crude bone chest neccesary to craft this.");
            AddRes(index, typeof(Leather), "Leather", 3, "You do not have the neccessary leather required to make this.");
            AddRes(index, typeof(UOACZIntestines), "Intestine", 1, "You do not have the neccessary intestine required to make this.");

            index = AddCraft(1, typeof(UOACZBoneShield), "Bone Armor", "Bone Shield", 15, 40, typeof(UOACZCrudeBoneShield), "Crude Bone Shield", 1, "You do not have the crude bone shield neccesary to craft this.");
            AddRes(index, typeof(Leather), "Leather", 3, "You do not have the neccessary leather required to make this.");
            AddRes(index, typeof(UOACZIntestines), "Intestine", 1, "You do not have the neccessary intestine required to make this.");

            //Reinforced Bone Armor
            index = AddCraft(1, typeof(UOACZReinforcedBoneGloves), "Reinforced Bone Armor", "Reinforced Bone Gloves", 25, 50, typeof(UOACZBoneGloves), "Bone Gloves", 1, "You do not have the bone gloves neccesary to craft this.");
            AddRes(index, typeof(Leather), "Leather", 1, "You do not have the neccessary leather required to make this.");
            AddRes(index, typeof(UOACZIntestines), "Intestine", 1, "You do not have the neccessary intestine required to make this.");

            index = AddCraft(1, typeof(UOACZReinforcedBoneHelm), "Reinforced Bone Armor", "Reinforced Bone Helm", 25, 50, typeof(UOACZBoneHelm), "Bone Helm", 1, "You do not have the bone helm neccesary to craft this.");
            AddRes(index, typeof(Leather), "Leather", 1, "You do not have the neccessary leather required to make this.");
            AddRes(index, typeof(UOACZIntestines), "Intestine", 1, "You do not have the neccessary intestine required to make this.");

            index = AddCraft(1, typeof(UOACZReinforcedBoneArms), "Reinforced Bone Armor", "Reinforced Bone Arms", 25, 50, typeof(UOACZBoneArms), "Bone Arms", 1, "You do not have the bone arms neccesary to craft this.");
            AddRes(index, typeof(Leather), "Leather", 1, "You do not have the neccessary leather required to make this.");
            AddRes(index, typeof(UOACZIntestines), "Intestine", 1, "You do not have the neccessary intestine required to make this.");

            index = AddCraft(1, typeof(UOACZReinforcedBoneLegs), "Reinforced Bone Armor", "Reinforced Bone Legs", 25, 50, typeof(UOACZBoneLegs), "Bone Legs", 1, "You do not have the bone legs neccesary to craft this.");
            AddRes(index, typeof(Leather), "Leather", 2, "You do not have the neccessary leather required to make this.");
            AddRes(index, typeof(UOACZIntestines), "Intestine", 1, "You do not have the neccessary intestine required to make this.");

            index = AddCraft(1, typeof(UOACZReinforcedBoneChest), "Reinforced Bone Armor", "Reinforced Bone Chest", 25, 50, typeof(UOACZBoneChest), "Bone Chest", 1, "You do not have the bone chest neccesary to craft this.");
            AddRes(index, typeof(Leather), "Leather", 3, "You do not have the neccessary leather required to make this.");
            AddRes(index, typeof(UOACZIntestines), "Intestine", 1, "You do not have the neccessary intestine required to make this.");
                        
            //Clothing
            AddCraft(1, typeof(Sandals), 1015288, 1025901, 5, 30, typeof(Leather), 1044462, 1, 1044463);
            AddCraft(1, typeof(Shoes), 1015288, 1025904, 5, 30, typeof(Leather), 1044462, 1, 1044463);
            AddCraft(1, typeof(Boots), 1015288, 1025899, 5, 30, typeof(Leather), 1044462, 1, 1044463);
            AddCraft(1, typeof(ThighBoots), 1015288, 1025906, 5, 30, typeof(Leather), 1044462, 1, 1044463);

            AddCraft(1, typeof(SkullCap), 1011375, 1025444, 5, 30, typeof(Cloth), 1044286, 2, 1044287);
            AddCraft(1, typeof(Bandana), 1011375, 1025440, 5, 30, typeof(Cloth), 1044286, 2, 1044287);
            AddCraft(1, typeof(FloppyHat), 1011375, 1025907, 5, 30, typeof(Cloth), 1044286, 2, 1044287);
            AddCraft(1, typeof(Cap), 1011375, 1025909, 5, 30, typeof(Cloth), 1044286, 2, 1044287);
            AddCraft(1, typeof(WideBrimHat), 1011375, 1025908, 5, 30, typeof(Cloth), 1044286, 2, 1044287);
            AddCraft(1, typeof(StrawHat), 1011375, 1025911, 5, 30, typeof(Cloth), 1044286, 2, 1044287);
            AddCraft(1, typeof(TallStrawHat), 1011375, 1025910, 5, 30, typeof(Cloth), 1044286, 2, 1044287);
            AddCraft(1, typeof(WizardsHat), 1011375, 1025912, 5, 30, typeof(Cloth), 1044286, 2, 1044287);
            AddCraft(1, typeof(Bonnet), 1011375, 1025913, 5, 30, typeof(Cloth), 1044286, 2, 1044287);
            AddCraft(1, typeof(FeatheredHat), 1011375, 1025914, 5, 30, typeof(Cloth), 1044286, 2, 1044287);
            AddCraft(1, typeof(TricorneHat), 1011375, 1025915, 5, 30, typeof(Cloth), 1044286, 2, 1044287);
            AddCraft(1, typeof(JesterHat), 1011375, 1025916, 5, 30, typeof(Cloth), 1044286, 2, 1044287);

            AddCraft(1, typeof(ShortPants), 1015279, 1025422, 5, 30, typeof(Cloth), 1044286, 3, 1044287);
            AddCraft(1, typeof(LongPants), 1015279, 1025433, 5, 30, typeof(Cloth), 1044286, 3, 1044287);
            AddCraft(1, typeof(Kilt), 1015279, 1025431, 5, 30, typeof(Cloth), 1044286, 3, 1044287);
            AddCraft(1, typeof(Skirt), 1015279, 1025398, 5, 30, typeof(Cloth), 1044286, 3, 1044287);

            AddCraft(1, typeof(BodySash), 1015283, 1025441, 5, 30, typeof(Cloth), 1044286, 3, 1044287);
            AddCraft(1, typeof(HalfApron), 1015283, 1025435, 5, 30, typeof(Cloth), 1044286, 3, 1044287);
            AddCraft(1, typeof(FullApron), 1015283, 1025437, 5, 30, typeof(Cloth), 1044286, 3, 1044287);  

            AddCraft(1, typeof(Doublet), 1015269, 1028059, 5, 30, typeof(Cloth), 1044286, 3, 1044287);
            AddCraft(1, typeof(Shirt), 1015269, 1025399, 5, 30, typeof(Cloth), 1044286, 3, 1044287);
            AddCraft(1, typeof(FancyShirt), 1015269, 1027933, 5, 30, typeof(Cloth), 1044286, 3, 1044287);
            AddCraft(1, typeof(Tunic), 1015269, 1028097, 5, 30, typeof(Cloth), 1044286, 3, 1044287);
            AddCraft(1, typeof(Surcoat), 1015269, 1028189, 5, 30, typeof(Cloth), 1044286, 3, 1044287);            
            AddCraft(1, typeof(JesterSuit), 1015269, 1028095, 5, 30, typeof(Cloth), 1044286, 3, 1044287);

            AddCraft(1, typeof(Cloak), 1015269, 1025397, 5, 30, typeof(Cloth), 1044286, 4, 1044287);

            AddCraft(1, typeof(PlainDress), 1015269, 1027937, 5, 30, typeof(Cloth), 1044286, 5, 1044287);
            AddCraft(1, typeof(FancyDress), 1015269, 1027935, 5, 30, typeof(Cloth), 1044286, 5, 1044287);            
            AddCraft(1, typeof(Robe), 1015269, 1027939, 5, 30, typeof(Cloth), 1044286, 5, 1044287);                      

            // Set the overridable material
            SetSubRes(typeof(Leather), 1049150);

            // Add every material you want the player to be able to choose from
            // This will override the overridable material
            AddSubRes(typeof(Leather), 1049150, 00.0, 1044462, 1049311);
            AddSubRes(typeof(SpinedLeather), 1049151, 65.0, 1044462, 1049311);
            AddSubRes(typeof(HornedLeather), 1049152, 80.0, 1044462, 1049311);
            AddSubRes(typeof(BarbedLeather), 1049153, 99.0, 1044462, 1049311);

            MarkOption = true;
            Repair = true;
            CanEnhance = Core.AOS;
        }
    }
}
