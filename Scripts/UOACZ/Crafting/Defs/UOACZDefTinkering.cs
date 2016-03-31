using System;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Custom;

namespace Server.Engines.Craft
{
    public class UOACZDefTinkering : CraftSystem
    {
        #region Base

        public override SkillName MainSkill
        {
            get { return SkillName.Tinkering; }
        }

        public override int GumpTitleNumber
        {
            get { return 1044007; } // <CENTER>TINKERING MENU</CENTER>
        }

        public static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new UOACZDefTinkering();

                return m_CraftSystem;
            }
        }

        public UOACZDefTinkering()
            : base(1, 1, 1.25)// base( 1, 1, 3.0 )
        {
        }

        public override double GetChanceAtMin(CraftItem item)
        {
            if (item.NameNumber == 1044258 || item.NameNumber == 1046445) // potion keg and faction trap removal kit
                return 0.5; // 50%

            return 0.0; // 0%
        }

        public override int CanCraft(Mobile from, BaseTool tool, Type itemType)
        {
            if (tool == null || tool.Deleted || tool.UsesRemaining < 0)
                return 1044038; // You have worn out your tool!
            else if (!BaseTool.CheckAccessible(tool, from))
                return 1044263; // The tool must be on your person to use.
            //else if ( itemType != null && ( itemType.IsSubclassOf( typeof( BaseFactionTrapDeed ) ) || itemType == typeof( FactionTrapRemovalKit ) ) && Faction.Find( from ) == null )
            //return 1044573; // You have to be in a faction to do that.

            return 0;
        }

        public static Type[] m_TinkerColorables = new Type[]
			{
				typeof( ForkLeft ), typeof( ForkRight ),
				typeof( SpoonLeft ), typeof( SpoonRight ),
				typeof( KnifeLeft ), typeof( KnifeRight ),
				typeof( Plate ),
				typeof( Goblet ), typeof( PewterMug ),
				typeof( KeyRing ),
				typeof( Candelabra ), typeof( Scales ),
				typeof( Key ), typeof( Globe ),
				typeof( Spyglass ), typeof( Lantern ),
				typeof( HeatingStand )
			};

        public override bool RetainsColorFrom(CraftItem item, Type type)
        {
            if (!type.IsSubclassOf(typeof(BaseIngot)))
                return false;

            type = item.ItemType;

            bool contains = false;

            for (int i = 0; !contains && i < m_TinkerColorables.Length; ++i)
                contains = (m_TinkerColorables[i] == type);

            return contains;
        }

        public override void PlayCraftEffect(Mobile from)
        {
            from.PlaySound(0x3e6);
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

        public override bool ConsumeOnFailure(Mobile from, Type resourceType, CraftItem craftItem)
        {
            return base.ConsumeOnFailure(from, resourceType, craftItem);
        }

        public void AddJewelrySet(GemType gemType, Type itemType)
        {
            int offset = (int)gemType - 1;
        }

        #endregion

        public override void InitCraftList()
        {
            int index = -1;

            //Fortifications
            AddCraft(1, typeof(UOACZBoilingOilCauldronDeed), "Fortifications", "Boiling Oil Cauldron Deed", 25, 50, typeof(IronIngot), 1044036, 100, 1044037);

            //Crafting Tools
            AddCraft(1, typeof(UOACZMortarPestle), "Crafting Tools", "Mortar and Pestle", 5, 30, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(1, typeof(UOACZSaw), "Crafting Tools", "Saw", 5, 30, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(1, typeof(UOACZTongs), "Crafting Tools", "Tongs", 5, 30, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(1, typeof(UOACZSewingKit), "Crafting Tools", "Sewing Kit", 5, 30, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(1, typeof(UOACZSkillet), "Crafting Tools", "Skillet", 5, 30, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(1, typeof(UOACZTinkersTools), "Crafting Tools", "Tinker Tools", 5, 30, typeof(IronIngot), 1044036, 2, 1044037);

            //Utility Tools
            AddCraft(1, typeof(Scissors), "Utility Tools", "Scissors", 5, 30, typeof(IronIngot), 1044036, 1, 1044037);
            AddCraft(1, typeof(UOACZHatchet), "Utility Tools", "Hatchet", 5, 30, typeof(IronIngot), 1044036, 3, 1044037);
            AddCraft(1, typeof(UOACZPickaxe), "Utility Tools", "Pickaxe", 5, 30, typeof(IronIngot), 1044036, 3, 1044037);
            AddCraft(1, typeof(UOACZLockpickKit), "Utility Tools", "Lockpick Kit", 5, 30, typeof(IronIngot), 1044036, 3, 1044037);
            AddCraft(1, typeof(UOACZRepairHammer), "Utility Tools", "Repair Hammer", 5, 30, typeof(IronIngot), 1044036, 5, 1044037);

            //Materials
            AddCraft(1, typeof(UOACZIronWire), "Materials", "Iron Wire", 10, 35, typeof(IronIngot), 1044036, 25, 1044037);          
            AddCraft(1, typeof(Springs), "Materials", "Springs", 5, 30, typeof(IronIngot), 1044036, 1, 1044037);
            AddCraft(1, typeof(Gears), "Materials", "Gears", 5, 30, typeof(IronIngot), 1044036, 1, 1044037);           
                      
            // Set the overridable material
            SetSubRes(typeof(IronIngot), 1044022);

            AddSubRes(typeof(IronIngot), 1044022, 00.0, 1044036, 1044267);
            AddSubRes(typeof(DullCopperIngot), 1044023, 65.0, 1044036, 1044268);
            AddSubRes(typeof(ShadowIronIngot), 1044024, 70.0, 1044036, 1044268);
            AddSubRes(typeof(CopperIngot), 1044025, 75.0, 1044036, 1044268);
            AddSubRes(typeof(BronzeIngot), 1044026, 80.0, 1044036, 1044268);
            AddSubRes(typeof(GoldIngot), 1044027, 85.0, 1044036, 1044268);
            AddSubRes(typeof(AgapiteIngot), 1044028, 90.0, 1044036, 1044268);
            AddSubRes(typeof(VeriteIngot), 1044029, 95.0, 1044036, 1044268);
            AddSubRes(typeof(ValoriteIngot), 1044030, 99.0, 1044036, 1044268);

            MarkOption = true;
            Repair = true;
            CanEnhance = Core.AOS;
        }
    }
}