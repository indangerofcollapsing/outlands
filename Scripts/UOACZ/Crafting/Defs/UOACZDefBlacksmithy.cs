using System;
using Server.Items;
using Server.Mobiles;
using Server.Custom;

namespace Server.Engines.Craft
{
    public class UOACZDefBlacksmithy : CraftSystem
    {
        #region Base

        public override SkillName MainSkill
        {
            get { return SkillName.Blacksmith; }
        }

        public override int GumpTitleNumber
        {
            get { return 1044002; } // <CENTER>BLACKSMITHY MENU</CENTER>
        }

        public static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new UOACZDefBlacksmithy();

                return m_CraftSystem;
            }
        }

        public override CraftECA ECA { get { return CraftECA.ChanceMinusSixtyToFourtyFive; } }

        public override double GetChanceAtMin(CraftItem item)
        {
            return 0.0; // 0%
        }

        public UOACZDefBlacksmithy()
            : base(1, 2, 1.75)// base( 1, 4, 4.3 )
        {
            /*
			
            base( MinCraftEffect, MaxCraftEffect, Delay )
			
            MinCraftEffect	: The minimum number of time the mobile will play the craft effect
            MaxCraftEffect	: The maximum number of time the mobile will play the craft effect
            Delay			: The delay between each craft effect
			
            Example: (3, 6, 1.7) would make the mobile do the PlayCraftEffect override
            function between 3 and 6 time, with a 1.7 second delay each time.
			
            */
        }

        public static Type typeofAnvil = typeof(AnvilAttribute);
        public static Type typeofForge = typeof(ForgeAttribute);

        public static void CheckAnvilAndForge(Mobile from, int range, out bool anvil, out bool forge)
        {
            anvil = false;
            forge = false;

            Map map = from.Map;

            if (map == null)
                return;

            IPooledEnumerable eable = map.GetItemsInRange(from.Location, range);

            foreach (Item item in eable)
            {
                Type type = item.GetType();

                bool isAnvil = (type.IsDefined(typeofAnvil, false) || item.ItemID == 4015 || item.ItemID == 4016 || item.ItemID == 0x2DD5 || item.ItemID == 0x2DD6);
                bool isForge = (type.IsDefined(typeofForge, false) || item.ItemID == 4017 || (item.ItemID >= 6522 && item.ItemID <= 6569) || item.ItemID == 0x2DD8);

                if (isAnvil || isForge)
                {
                    if ((from.Z + 16) < item.Z || (item.Z + 16) < from.Z || !from.InLOS(item))
                        continue;

                    anvil = anvil || isAnvil;
                    forge = forge || isForge;

                    if (anvil && forge)
                        break;
                }
            }

            eable.Free();

            for (int x = -range; (!anvil || !forge) && x <= range; ++x)
            {
                for (int y = -range; (!anvil || !forge) && y <= range; ++y)
                {
                    StaticTile[] tiles = map.Tiles.GetStaticTiles(from.X + x, from.Y + y, true);

                    for (int i = 0; (!anvil || !forge) && i < tiles.Length; ++i)
                    {
                        int id = tiles[i].ID;

                        bool isAnvil = (id == 4015 || id == 4016 || id == 0x2DD5 || id == 0x2DD6);
                        bool isForge = (id == 4017 || (id >= 6522 && id <= 6569) || id == 0x2DD8);

                        if (isAnvil || isForge)
                        {
                            if ((from.Z + 16) < tiles[i].Z || (tiles[i].Z + 16) < from.Z || !from.InLOS(new Point3D(from.X + x, from.Y + y, tiles[i].Z + (tiles[i].Height / 2) + 1)))
                                continue;

                            anvil = anvil || isAnvil;
                            forge = forge || isForge;
                        }
                    }
                }
            }
        }

        public override int CanCraft(Mobile from, BaseTool tool, Type itemType)
        {
            if (tool == null || tool.Deleted || tool.UsesRemaining < 0)
                return 1044038; // You have worn out your tool!
            else if (!BaseTool.CheckTool(tool, from))
                return 1048146; // If you have a tool equipped, you must use that tool.
            else if (!BaseTool.CheckAccessible(tool, from))
                return 1044263; // The tool must be on your person to use.

            bool anvil, forge;
            CheckAnvilAndForge(from, 2, out anvil, out forge);

            if (anvil && forge)
                return 0;

            return 1044267; // You must be near an anvil and a forge to smith items.
        }

        public override void PlayCraftEffect(Mobile from)
        {
            // no animation, instant sound
            //if ( from.Body.Type == BodyType.Human && !from.Mounted )
            //	from.Animate( 9, 5, 1, true, false, 0 );
            //new InternalTimer( from ).Start();

            from.PlaySound(0x2A);
        }

        // Delay to synchronize the sound with the hit on the anvil
        public class InternalTimer : Timer
        {
            public Mobile m_From;

            public InternalTimer(Mobile from)
                : base(TimeSpan.FromSeconds(0.7))
            {
                m_From = from;
            }

            protected override void OnTick()
            {
                m_From.PlaySound(0x2A);
            }
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

            //One Handed Weapons
            AddCraft(typeof(UOACZDagger), "One Handed Weapons", "Dagger", 5, 30, typeof(IronIngot), 1044036, 5, 1044037);
            AddCraft(typeof(Kryss), "One Handed Weapons", "Kryss", 10, 35, typeof(IronIngot), 1044036, 10, 1044037);
            AddCraft(typeof(WarFork), "One Handed Weapons", "Warfork", 10, 35, typeof(IronIngot), 1044036, 10, 1044037);
            AddCraft(typeof(Cutlass), "One Handed Weapons", "Cutlass", 10, 35, typeof(IronIngot), 1044036, 10, 1044037);
            AddCraft(typeof(Katana), "One Handed Weapons", "Katana", 10, 35, typeof(IronIngot), 1044036, 10, 1044037);
            AddCraft(typeof(Longsword), "One Handed Weapons", "Longsword", 10, 35, typeof(IronIngot), 1044036, 10, 1044037);
            AddCraft(typeof(Scimitar), "One Handed Weapons", "Scimitar", 10, 35, typeof(IronIngot), 1044036, 10, 1044037);
            AddCraft(typeof(VikingSword), "One Handed Weapons", "Viking Sword", 10, 35, typeof(IronIngot), 1044036, 10, 1044037);
            AddCraft(typeof(HammerPick), "One Handed Weapons", "Hammerpick", 10, 35, typeof(IronIngot), 1044036, 10, 1044037);
            AddCraft(typeof(Mace), "One Handed Weapons", "Mace", 10, 35, typeof(IronIngot), 1044036, 10, 1044037);
            AddCraft(typeof(Maul), "One Handed Weapons", "Maul", 10, 35, typeof(IronIngot), 1044036, 10, 1044037);
            AddCraft(typeof(WarMace), "One Handed Weapons", "War Mace", 10, 35, typeof(IronIngot), 1044036, 10, 1044037);
            AddCraft(typeof(WarAxe), "One Handed Weapons", "War Axe", 10, 35, typeof(IronIngot), 1044036, 10, 1044037);
            
            //Two Handed Weapons
            AddCraft(typeof(ShortSpear), "Two Handed Weapons", "Short Spear", 10, 35, typeof(IronIngot), 1044036, 15, 1044037);
            AddCraft(typeof(Spear), "Two Handed Weapons", "Spear", 10, 35, typeof(IronIngot), 1044036, 15, 1044037);
            AddCraft(typeof(Pitchfork), "Two Handed Weapons", "Pitchfork", 10, 35, typeof(IronIngot), 1044036, 15, 1044037);
            AddCraft(typeof(Axe), "Two Handed Weapons", "Axe", 10, 35, typeof(IronIngot), 1044036, 15, 1044037);
            AddCraft(typeof(BattleAxe), "Two Handed Weapons", "Battle Axe", 10, 35, typeof(IronIngot), 1044036, 15, 1044037);
            AddCraft(typeof(DoubleAxe), "Two Handed Weapons", "Double Axe", 10, 35, typeof(IronIngot), 1044036, 15, 1044037);
            AddCraft(typeof(ExecutionersAxe), "Two Handed Weapons", "Executioner's Axe", 10, 35, typeof(IronIngot), 1044036, 15, 1044037);
            AddCraft(typeof(LargeBattleAxe), "Two Handed Weapons", "Large Battle Axe", 10, 35, typeof(IronIngot), 1044036, 15, 1044037);
            AddCraft(typeof(TwoHandedAxe), "Two Handed Weapons", "Two Handed Axe", 10, 35, typeof(IronIngot), 1044036, 15, 1044037);
            AddCraft(typeof(Bardiche), "Two Handed Weapons", "Bardiche", 10, 35, typeof(IronIngot), 1044036, 15, 1044037);
            AddCraft(typeof(Halberd), "Two Handed Weapons", "Halberd", 10, 35, typeof(IronIngot), 1044036, 15, 1044037);
            AddCraft(typeof(WarHammer), "Two Handed Weapons", "Warhammer", 10, 35, typeof(IronIngot), 1044036, 15, 1044037);

            //Shields
            AddCraft(typeof(Buckler), "Shields", "Buckler", 15, 40, typeof(IronIngot), 1044036, 10, 1044037);
            AddCraft(typeof(MetalShield), "Shields", "Metal Shield", 20, 45, typeof(IronIngot), 1044036, 15, 1044037);
            AddCraft(typeof(BronzeShield), "Shields", "Bronze Shield", 25, 50, typeof(IronIngot), 1044036, 15, 1044037);            
            AddCraft(typeof(MetalKiteShield), "Shields", "Metal Kite Shield", 30, 55, typeof(IronIngot), 1044036, 15, 1044037);
            AddCraft(typeof(HeaterShield), "Shields", "Heater Shield", 35, 60, typeof(IronIngot), 1044036, 20, 1044037);           

            //Ringmail
            AddCraft(typeof(RingmailGloves), "Ringmail Armor", "Ringmail Gloves", 15, 40, typeof(IronIngot), 1044036, 5, 1044037);
            AddCraft(typeof(RingmailArms), "Ringmail Armor", "Ringmail Arms", 15, 40, typeof(IronIngot), 1044036, 10, 1044037);
            AddCraft(typeof(RingmailLegs), "Ringmail Armor", "Ringmail Legs", 15, 40, typeof(IronIngot), 1044036, 10, 1044037);
            AddCraft(typeof(RingmailChest), "Ringmail Armor", "Ringmail Chest", 15, 40, typeof(IronIngot), 1044036, 15, 1044037);

            //Chainmail
            AddCraft(typeof(ChainCoif), "Chainmail Armor", "Chainmail Coif", 25, 50, typeof(IronIngot), 1044036, 5, 1044037);
            AddCraft(typeof(ChainLegs), "Chainmail Armor", "Chainmail Legs", 25, 50, typeof(IronIngot), 1044036, 10, 1044037);
            AddCraft(typeof(ChainChest), "Chainmail Armor", "Chainmail Chest", 25, 50, typeof(IronIngot), 1044036, 15, 1044037);

            //Platemail
            AddCraft(typeof(PlateGorget), "Platemail Armor", "Platemail Gorget", 35, 60, typeof(IronIngot), 1044036, 5, 1044037);
            AddCraft(typeof(PlateGloves), "Platemail Armor", "Platemail Gloves", 35, 60, typeof(IronIngot), 1044036, 5, 1044037);
            AddCraft(typeof(PlateHelm), "Platemail Armor", "Platemail Helm", 35, 60, typeof(IronIngot), 1044036, 5, 1044037);
            AddCraft(typeof(PlateArms), "Platemail Armor", "Platemail Arms", 35, 60, typeof(IronIngot), 1044036, 10, 1044037);
            AddCraft(typeof(PlateLegs), "Platemail Armor", "Platemail Legs", 35, 60, typeof(IronIngot), 1044036, 15, 1044037);
            AddCraft(typeof(PlateChest), "Platemail Armor", "Platemail Chest", 35, 60, typeof(IronIngot), 1044036, 20, 1044037);
            AddCraft(typeof(FemalePlateChest), "Platemail Armor", "Female Platemail Chest", 35, 60, typeof(IronIngot), 1044036, 20, 1044037);

            //Other Helmets
            AddCraft(typeof(Bascinet), "Other Helmets", "Bascinet", 5, 30, typeof(IronIngot), 1044036, 5, 1044037);
            AddCraft(typeof(CloseHelm), "Other Helmets", "Close Helm", 35, 60, typeof(IronIngot), 1044036, 5, 1044037);
            AddCraft(typeof(Helmet), "Other Helmets", "Helmet", 35, 60, typeof(IronIngot), 1044036, 5, 1044037);
            AddCraft(typeof(NorseHelm), "Other Helmets", "Norse Helm", 35, 60, typeof(IronIngot), 1044036, 5, 1044037);                                   

            // Set the overridable material
            SetSubRes(typeof(IronIngot), 1044022);

            // Add every material you want the player to be able to choose from
            // This will override the overridable material
            AddSubRes(typeof(IronIngot), 1044022, 00.0, 1044036, 1044267);
            AddSubRes(typeof(DullCopperIngot), 1044023, 65.0, 1044036, 1044268);
            AddSubRes(typeof(ShadowIronIngot), 1044024, 70.0, 1044036, 1044268);
            AddSubRes(typeof(CopperIngot), 1044025, 75.0, 1044036, 1044268);
            AddSubRes(typeof(BronzeIngot), 1044026, 80.0, 1044036, 1044268);
            AddSubRes(typeof(GoldIngot), 1044027, 85.0, 1044036, 1044268);
            AddSubRes(typeof(AgapiteIngot), 1044028, 90.0, 1044036, 1044268);
            AddSubRes(typeof(VeriteIngot), 1044029, 95.0, 1044036, 1044268);
            AddSubRes(typeof(ValoriteIngot), 1044030, 99.0, 1044036, 1044268);
            AddSubRes(typeof(LuniteIngot), "LUNITE", 100.0, 1044268);
            
            Recycle = true;
            Repair = true;
            MarkOption = true;
        }
    }
}