using System;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Custom;

namespace Server.Engines.Craft
{
    public class DefTinkering : CraftSystem
    {
        public override SkillName MainSkill
        {
            get { return SkillName.Tinkering; }
        }

        public override int GumpTitleNumber
        {
            get { return 1044007; } // <CENTER>TINKERING MENU</CENTER>
        }

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new DefTinkering();

                return m_CraftSystem;
            }
        }

        private DefTinkering()
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

            return 0;
        }

        private static Type[] m_TinkerColorables = new Type[]
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

        public void AddJewelrySet(GemType gemType, string name, Type itemType)
        {
            int offset = (int)gemType - 1;

            //Jewelry
            int index = AddCraft(1, typeof(GoldRing), "Jewelry", name + " Ring", 40.0, 90.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddRes(index, itemType, 1044231 + offset, 1, 1044240);

            index = AddCraft(1, typeof(SilverBeadNecklace), "Jewelry", name + " Silver Necklace", 40.0, 90.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddRes(index, itemType, 1044231 + offset, 1, 1044240);

            index = AddCraft(1, typeof(GoldNecklace), "Jewelry", name + " Necklace", 40.0, 90.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddRes(index, itemType, 1044231 + offset, 1, 1044240);

            index = AddCraft(1, typeof(GoldEarrings), "Jewelry", name + " Earrings", 40.0, 90.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddRes(index, itemType, 1044231 + offset, 1, 1044240);

            index = AddCraft(1, typeof(GoldBeadNecklace), "Jewelry", name + " Gold Necklace", 40.0, 90.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddRes(index, itemType, 1044231 + offset, 1, 1044240);

            index = AddCraft(1, typeof(GoldBracelet), "Jewelry", name + " Bracelet", 40.0, 90.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddRes(index, itemType, 1044231 + offset, 1, 1044240);
        }

        public override void InitCraftList()
        {
            int index = -1;

            //Utility            
            
            index = AddCraft(1, typeof(PotionKeg), "Utility", "Potion Keg", 75.0, 100.0, typeof(Keg), 1044255, 1, 1044253);            
            AddRes(index, typeof(BarrelLid), 1044251, 1, 1044253);
            AddRes(index, typeof(BarrelTap), 1044252, 1, 1044253);

            AddCraft(1, typeof(Sextant), "Utility", "Sextant", 0.0, 0.0, typeof(SextantParts), 1044175, 1, 1044253);
            AddCraft(1, typeof(Key), "Utility", "Key", 20.0, 70.0, typeof(IronIngot), 1044036, 3, 1044037);
            AddCraft(1, typeof(KeyRing), "Utility", "Key Ring", 10.0, 60.0, typeof(IronIngot), 1044036, 2, 1044037);
            
            AddCraft(1, typeof(Lantern), "Utility", "Lantern", 30.0, 80.0, typeof(IronIngot), 1044036, 2, 1044037);

            //Tools
            AddCraft(1, typeof(Lockpick), "Tools", "Lockpick", 45.0, 95.0, typeof(IronIngot), 1044036, 1, 1044037);
            AddCraft(1, typeof(Scissors), "Tools", "Scissors", 5.0, 55.0, typeof(IronIngot), 1044036, 2, 1044037);

            AddCraft(1, typeof(Shovel), "Tools", "Shovel", 37.5, 90.0, typeof(IronIngot), 1044036, 4, 1044037);
            AddCraft(1, typeof(Pickaxe), "Tools", "Pickaxe", 40.0, 90.0, typeof(IronIngot), 1044036, 4, 1044037);
            AddCraft(1, typeof(Hatchet), "Tools", "Hatchet", 30.0, 80.0, typeof(IronIngot), 1044036, 4, 1044037);

            AddCraft(1, typeof(Tongs), "Tools", "Tongs", 35.0, 85.0, typeof(IronIngot), 1044036, 1, 1044037);
            AddCraft(1, typeof(SledgeHammer), "Tools", "Sledgehammer", 40.0, 90.0, typeof(IronIngot), 1044036, 4, 1044037);
            AddCraft(1, typeof(SmithHammer), "Tools", "Smith's Hammer", 40.0, 90.0, typeof(IronIngot), 1044036, 4, 1044037);

            AddCraft(1, typeof(MortarPestle), "Tools", "Mortar and Pestle", 20.0, 70.0, typeof(IronIngot), 1044036, 3, 1044037);
            AddCraft(1, typeof(TinkerTools), "Tools", "Tinker Tools", 10.0, 60.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(1, typeof(SewingKit), "Tools", "Sewing Kit", 10.0, 70.0, typeof(IronIngot), 1044036, 2, 1044037);
            
            AddCraft(1, typeof(Skillet), "Tools", "Skillet", 30.0, 80.0, typeof(IronIngot), 1044036, 4, 1044037);
            AddCraft(1, typeof(RollingPin), "Tools", "Rolling Pin", 0.0, 50.0, typeof(Board), 1044041, 5, 1044351);

            AddCraft(1, typeof(MapmakersPen), "Tools", "Mapmaker's Pen", 25.0, 75.0, typeof(IronIngot), 1044036, 1, 1044037);
            AddCraft(1, typeof(ScribesPen), "Tools", "Scribe's Pen", 25.0, 75.0, typeof(IronIngot), 1044036, 1, 1044037);            

            AddCraft(1, typeof(Scorp), "Tools", "Scorp", 30.0, 80.0, typeof(IronIngot), 1044036, 2, 1044037);            
            AddCraft(1, typeof(DrawKnife), "Tools", "Drawing Knife", 30.0, 80.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(1, typeof(Saw), "Tools", "Saw", 30.0, 80.0, typeof(IronIngot), 1044036, 4, 1044037);
            AddCraft(1, typeof(DovetailSaw), "Tools", "Dovetail Saw", 30.0, 80.0, typeof(IronIngot), 1044036, 4, 1044037);
            AddCraft(1, typeof(Froe), "Tools", "Froe", 30.0, 80.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(1, typeof(Hammer), "Tools", "Hammer", 30.0, 80.0, typeof(IronIngot), 1044036, 1, 1044037);
            AddCraft(1, typeof(Inshave), "Tools", "Inshave", 30.0, 80.0, typeof(IronIngot), 1044036, 2, 1044037);

            //Parts
            AddCraft(1, typeof(BarrelTap), "Parts", "Barrel Tap", 35.0, 85.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(1, typeof(BarrelHoops), "Parts", "Barrel Hoops", -15.0, 35.0, typeof(IronIngot), 1044036, 5, 1044037);

            AddCraft(1, typeof(JointingPlane), "Parts", "Jointing Plane", 0.0, 50.0, typeof(Board), 1044041, 4, 1044351);
            AddCraft(1, typeof(MouldingPlane), "Parts", "Moulding Plane", 0.0, 50.0, typeof(Board), 1044041, 4, 1044351);
            AddCraft(1, typeof(SmoothingPlane), "Parts", "Smoothing Plane", 0.0, 50.0, typeof(Board), 1044041, 4, 1044351);
            AddCraft(1, typeof(ClockFrame), "Parts", "Clock Frame", 0.0, 50.0, typeof(Board), 1044041, 6, 1044351);
            AddCraft(1, typeof(Axle), "Parts", "Axle", -25.0, 25.0, typeof(Board), 1044041, 2, 1044351);
            AddCraft(1, typeof(Gears), "Parts", "Gears", 5.0, 55.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(1, typeof(ClockParts), "Parts", "Clock Parts", 25.0, 75.0, typeof(IronIngot), 1044036, 1, 1044037);            
            AddCraft(1, typeof(Springs), "Parts", "Springs", 5.0, 55.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(1, typeof(SextantParts), "Parts", "Sextant Parts", 30.0, 80.0, typeof(IronIngot), 1044036, 4, 1044037);            
            AddCraft(1, typeof(Hinge), "Parts", "Hinge", 5.0, 55.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(1, typeof(Nails), "Parts", "Nails", 0.0, 25.0, typeof(IronIngot), 1044036, 2, 1044037);

            index = AddCraft(1, typeof(AxleGears), "Parts", "Axel Gears", 0.0, 0.0, typeof(Axle), 1044169, 1, 1044253);
            AddRes(index, typeof(Gears), 1044254, 1, 1044253);

            index = AddCraft(1, typeof(ClockParts), "Parts", "Clock Parts", 0.0, 0.0, typeof(AxleGears), 1044170, 1, 1044253);
            AddRes(index, typeof(Springs), 1044171, 1, 1044253);

            index = AddCraft(1, typeof(SextantParts), "Parts", "Sextant Parts", 0.0, 0.0, typeof(AxleGears), 1044170, 1, 1044253);
            AddRes(index, typeof(Hinge), 1044172, 1, 1044253);

            //Decorative
            AddCraft(1, typeof(Candelabra), "Decorative", "Candelabra", 55.0, 105.0, typeof(IronIngot), 1044036, 4, 1044037);
            AddCraft(1, typeof(HeatingStand), "Decorative", "Heating Stand", 60.0, 130.0, typeof(IronIngot), 1044036, 4, 1044037);
            AddCraft(1, typeof(Scales), "Decorative", "Scales", 60.0, 130.0, typeof(IronIngot), 1044036, 4, 1044037);
            AddCraft(1, typeof(Globe), "Decorative", "Globe", 55.0, 105.0, typeof(IronIngot), 1044036, 4, 1044037);

            index = AddCraft(1, typeof(ClockRight), "Decorative", "Clock", 0.0, 0.0, typeof(ClockFrame), 1044174, 1, 1044253);
            AddRes(index, typeof(ClockParts), 1044173, 1, 1044253);

            index = AddCraft(1, typeof(ClockLeft), "Decorative", "Clock", 0.0, 0.0, typeof(ClockFrame), 1044174, 1, 1044253);
            AddRes(index, typeof(ClockParts), 1044173, 1, 1044253);           

            //Utensils
            AddCraft(1, typeof(ButcherKnife), "Utensils", "Butcher Knife", 25.0, 75.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(1, typeof(SkinningKnife), "Utensils", "Skinning Knife", 25.0, 75.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(1, typeof(Cleaver), "Utensils", "Cleaver", 20.0, 70.0, typeof(IronIngot), 1044036, 3, 1044037);
            AddCraft(1, typeof(SpoonLeft), "Utensils", "Spoon Left", 0.0, 50.0, typeof(IronIngot), 1044036, 1, 1044037);
            AddCraft(1, typeof(SpoonRight), "Utensils", "Spoon Right", 0.0, 50.0, typeof(IronIngot), 1044036, 1, 1044037);
            AddCraft(1, typeof(ForkLeft), "Utensils", "Fork Left", 0.0, 50.0, typeof(IronIngot), 1044036, 1, 1044037);
            AddCraft(1, typeof(ForkRight), "Utensils", "Fork Right", 0.0, 50.0, typeof(IronIngot), 1044036, 1, 1044037);
            AddCraft(1, typeof(KnifeLeft), "Utensils", "Knife Left", 0.0, 50.0, typeof(IronIngot), 1044036, 1, 1044037);
            AddCraft(1, typeof(KnifeRight), "Utensils", "Knife Right", 0.0, 50.0, typeof(IronIngot), 1044036, 1, 1044037);
            AddCraft(1, typeof(Plate), "Utensils", "Plate", 0.0, 50.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(1, typeof(Goblet), "Utensils", "Goblet", 10.0, 60.0, typeof(IronIngot), 1044036, 2, 1044037);
            AddCraft(1, typeof(PewterMug), "Utensils", "Pewter Mug", 10.0, 60.0, typeof(IronIngot), 1044036, 2, 1044037);

            //Jewelry
            AddJewelrySet(GemType.StarSapphire, "Star Sapphire", typeof(StarSapphire));
            AddJewelrySet(GemType.Emerald, "Emerald", typeof(Emerald));
            AddJewelrySet(GemType.Sapphire, "Sapphire", typeof(Sapphire));
            AddJewelrySet(GemType.Ruby, "Ruby", typeof(Ruby));
            AddJewelrySet(GemType.Citrine, "Citrine", typeof(Citrine));
            AddJewelrySet(GemType.Amethyst, "Amethyst", typeof(Amethyst));
            AddJewelrySet(GemType.Tourmaline, "Tourmaline", typeof(Tourmaline));
            AddJewelrySet(GemType.Amber, "Amber", typeof(Amber));
            AddJewelrySet(GemType.Diamond, "Diamond", typeof(Diamond));

            // Traps
            index = AddCraft(1, typeof(DartTrapCraft), "Traps and Bombs", "Dart Trap", 30.0, 80.0, typeof(IronIngot), 1044036, 1, 1044037);
            AddRes(index, typeof(Bolt), "Bolt", 1, 1044253);

            index = AddCraft(1, typeof(PoisonTrapCraft), "Traps and Bombs", "Poison Trap", 30.0, 80.0, typeof(IronIngot), 1044036, 1, 1044037);
            AddRes(index, typeof(BasePoisonPotion), "Poison Potion", 1, 1044253);

            index = AddCraft(1, typeof(ExplosionTrapCraft), "Traps and Bombs", "Explosion Trap", 55.0, 105.0, typeof(IronIngot), 1044036, 1, 1044037);
            AddRes(index, typeof(BaseExplosionPotion), "Explosion Potion", 1, 1044253);

            // Set the overridable material
            SetSubRes(typeof(IronIngot), "Iron");

            // Add every material you want the player to be able to choose from
            // This will override the overridable material
            AddSubRes(typeof(IronIngot), "Iron", 00.0, 1044268);
            AddSubRes(typeof(DullCopperIngot), "Dull Copper", 65.0, 1044268);
            AddSubRes(typeof(ShadowIronIngot), "Shadow Iron", 70.0, 1044268);
            AddSubRes(typeof(CopperIngot), "Copper", 75.0, 1044268);
            AddSubRes(typeof(BronzeIngot), "Bronze", 80.0, 1044268);
            AddSubRes(typeof(GoldIngot), "Gold", 85.0, 1044268);
            AddSubRes(typeof(AgapiteIngot), "Agapite", 90.0, 1044268);
            AddSubRes(typeof(VeriteIngot), "Verite", 95.0, 1044268);
            AddSubRes(typeof(ValoriteIngot), "Valorite", 99.0, 1044268);
            AddSubRes(typeof(LuniteIngot), "Lunite", 100.0, 1044268);

            Recycle = true;
            MarkOption = true;
            Repair = true;
        }
    }

    public abstract class TrapCraft : CustomCraft
    {
        private LockableContainer m_Container;

        public LockableContainer Container { get { return m_Container; } }

        public abstract TrapType TrapType { get; }

        public TrapCraft(Mobile from, CraftItem craftItem, CraftSystem craftSystem, Type typeRes, BaseTool tool, int quality): base(from, craftItem, craftSystem, typeRes, tool, quality)
        {
        }

        private int Verify(LockableContainer container)
        {
            if (container == null || container.KeyValue == 0)
                return 1005638; // You can only trap lockable chests.

            if (From.Map != container.Map || !From.InRange(container.GetWorldLocation(), 2))
                return 500446; // That is too far away.

            if (!container.IsAccessibleTo(From))
                return 502946; // That belongs to someone else.

            if (container.Locked)
                return 502943; // You can only trap an unlocked object.

            if (container.TrapType != TrapType.None)
                return 502945; // You can only place one trap on an object at a time.

            return 0;
        }

        private bool Acquire(object target, out int message)
        {
            LockableContainer container = target as LockableContainer;

            message = Verify(container);

            if (message > 0)
                return false;

            else
            {
                m_Container = container;
                return true;
            }
        }

        public override void EndCraftAction()
        {
            From.SendLocalizedMessage(502921); // What would you like to set a trap on?
            From.Target = new ContainerTarget(this);
        }

        private class ContainerTarget : Target
        {
            private TrapCraft m_TrapCraft;

            public ContainerTarget(TrapCraft trapCraft)
                : base(-1, false, TargetFlags.None)
            {
                m_TrapCraft = trapCraft;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                int message;

                if (m_TrapCraft.Acquire(targeted, out message))
                    m_TrapCraft.CraftItem.CompleteCraft(m_TrapCraft.Quality, false, m_TrapCraft.From, m_TrapCraft.CraftSystem, m_TrapCraft.TypeRes, m_TrapCraft.Tool, m_TrapCraft);

                else
                    Failure(message);
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                if (cancelType == TargetCancelType.Canceled)
                    Failure(0);
            }

            private void Failure(int message)
            {
                Mobile from = m_TrapCraft.From;
                BaseTool tool = m_TrapCraft.Tool;

                if (tool != null && !tool.Deleted && tool.UsesRemaining > 0)
                    from.SendGump(new CraftGump(from, m_TrapCraft.CraftSystem, tool, message));

                else if (message > 0)
                    from.SendLocalizedMessage(message);
            }
        }

        public override Item CompleteCraft(out int message)
        {
            message = Verify(this.Container);

            if (message == 0)
            {
                int trapLevel = (int)(From.Skills.Tinkering.Value / 10);

                Container.TrapType = this.TrapType;
                Container.TrapPower = trapLevel * 9;
                Container.TrapLevel = trapLevel;
                Container.Trapper = From;

                message = 1010360; //You arm the trap and carefully hide it from view.
            }

            return null;
        }
    }

    [CraftItemID(0x1BFC)]
    public class DartTrapCraft : TrapCraft
    {
        public override TrapType TrapType { get { return TrapType.DartTrap; } }

        public DartTrapCraft(Mobile from, CraftItem craftItem, CraftSystem craftSystem, Type typeRes, BaseTool tool, int quality)
            : base(from, craftItem, craftSystem, typeRes, tool, quality)
        {
        }
    }

    [CraftItemID(0x113E)]
    public class PoisonTrapCraft : TrapCraft
    {
        public override TrapType TrapType { get { return TrapType.PoisonTrap; } }

        public PoisonTrapCraft(Mobile from, CraftItem craftItem, CraftSystem craftSystem, Type typeRes, BaseTool tool, int quality)
            : base(from, craftItem, craftSystem, typeRes, tool, quality)
        {
        }
    }

    [CraftItemID(0x370C)]
    public class ExplosionTrapCraft : TrapCraft
    {
        public override TrapType TrapType { get { return TrapType.ExplosionTrap; } }

        public ExplosionTrapCraft(Mobile from, CraftItem craftItem, CraftSystem craftSystem, Type typeRes, BaseTool tool, int quality)
            : base(from, craftItem, craftSystem, typeRes, tool, quality)
        {
        }
    }
}