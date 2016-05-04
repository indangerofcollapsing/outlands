using System;
using Server.Items;
using Server.Mobiles;
using Server.Custom;

namespace Server.Engines.Craft
{
    public class DefBlacksmithy : CraftSystem
    {
        public override SkillName MainSkill
        {
            get { return SkillName.Blacksmith; }
        }

        public override int GumpTitleNumber
        {
            get { return 1044002; } // <CENTER>BLACKSMITHY MENU</CENTER>
        }

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new DefBlacksmithy();

                return m_CraftSystem;
            }
        }

        public override CraftECA ECA { get { return CraftECA.ChanceMinusSixtyToFourtyFive; } }

        public override double GetChanceAtMin(CraftItem item)
        {
            return 0.0; // 0%
        }

        private DefBlacksmithy()
            : base(1, 2, 1.75)// base( 1, 4, 4.3 )
        {
        }

        private static Type typeofAnvil = typeof(AnvilAttribute);
        private static Type typeofForge = typeof(ForgeAttribute);

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
            from.PlaySound(0x2A);
        }

        private class InternalTimer : Timer
        {
            private Mobile m_From;

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

        public override void InitCraftList()
        {
            int index = -1;

            //Ringmail
            AddCraft(1, typeof(RingmailGorget), "Ringmail Armor", "Ringmail Gorget", 5.0, 55.0, typeof(IronIngot), 1044036, 4, 1044037);
            AddCraft(1, typeof(RingmailGloves), "Ringmail Armor", "Ringmail Gloves", 10.0, 60.0, typeof(IronIngot), 1044036, 6, 1044037);
            AddCraft(1, typeof(RingmailHelm), "Ringmail Armor", "Ringmail Coif", 15.0, 65.0, typeof(IronIngot), 1044036, 8, 1044037);  
            AddCraft(1, typeof(RingmailArms), "Ringmail Armor", "Ringmail Arms", 20.0, 70.0, typeof(IronIngot), 1044036, 8, 1044037);
            AddCraft(1, typeof(RingmailLegs), "Ringmail Armor", "Ringmail Legs", 25.0, 75.0, typeof(IronIngot), 1044036, 10, 1044037);
            AddCraft(1, typeof(RingmailChest), "Ringmail Armor", "Ringmail Chest", 30.0, 80.0, typeof(IronIngot), 1044036, 14, 1044037);

            //Chainmail
            AddCraft(1, typeof(ChainmailGorget), "Chainmail Armor", "Chainmail Gorget", 25.0, 75.0, typeof(IronIngot), 1044036, 6, 1044037);
            AddCraft(1, typeof(ChainmailGloves), "Chainmail Armor", "Chainmail Gloves", 30.0, 80.0, typeof(IronIngot), 1044036, 8, 1044037);
            AddCraft(1, typeof(ChainmailCoif), "Chainmail Armor", "Chainmail Coif", 35.0, 85.0, typeof(IronIngot), 1044036, 10, 1044037);
            AddCraft(1, typeof(ChainmailArms), "Chainmail Armor", "Chainmail Arms", 40.0, 90.0, typeof(IronIngot), 1044036, 10, 1044037);
            AddCraft(1, typeof(ChainmailLegs), "Chainmail Armor", "Chainmail Legs", 45.0, 95.0, typeof(IronIngot), 1044036, 12, 1044037);
            AddCraft(1, typeof(ChainmailChest), "Chainmail Armor", "Chainmail Chest", 50.0, 100.0, typeof(IronIngot), 1044036, 16, 1044037);

            //Platemail
            AddCraft(1, typeof(PlateGorget), "Platemail Armor", "Plate Gorget", 45.0, 95.0, typeof(IronIngot), 1044036, 6, 1044037);
            AddCraft(1, typeof(PlateGloves), "Platemail Armor", "Plate Gloves", 50.0, 100.0, typeof(IronIngot), 1044036, 8, 1044037);
            AddCraft(1, typeof(PlateHelm), "Platemail Armor", "Plate Helm", 55.0, 105.0, typeof(IronIngot), 1044036, 12, 1044037);
            AddCraft(1, typeof(PlateArms), "Platemail Armor", "Plate Arms", 60.0, 110.0, typeof(IronIngot), 1044036, 12, 1044037);
            AddCraft(1, typeof(PlateLegs), "Platemail Armor", "Plate Legs", 65.0, 115.0, typeof(IronIngot), 1044036, 14, 1044037);
            AddCraft(1, typeof(PlateChest), "Platemail Armor", "Plate Chest", 70.0, 120.0, typeof(IronIngot), 1044036, 18, 1044037);
            AddCraft(1, typeof(FemalePlateChest), "Platemail Armor", "Female Plate Chest", 70.0, 120.0, typeof(IronIngot), 1044036, 18, 1044037);

            //Helmets
            AddCraft(1, typeof(Bascinet), "Other Helmets", "Bascinet", 55.0, 105.0, typeof(IronIngot), 1044036, 12, 1044037);
            AddCraft(1, typeof(CloseHelm), "Other Helmets", "Close Helm", 55.0, 105.0, typeof(IronIngot), 1044036, 12, 1044037);
            AddCraft(1, typeof(NorseHelm), "Other Helmets", "Norse Helm", 55.0, 105.0, typeof(IronIngot), 1044036, 12, 1044037);

            //Shields
            AddCraft(1, typeof(Buckler), "Shields", "Buckler", -10.0, 40.0, typeof(IronIngot), 1044036, 8, 1044037);
            AddCraft(1, typeof(MetalShield), "Shields", "Metal Shield", 10.0, 60.0, typeof(IronIngot), 1044036, 10, 1044037);
            AddCraft(1, typeof(BronzeShield), "Shields", "Bronze Shield", 30.0, 80.0, typeof(IronIngot), 1044036, 12, 1044037);
            AddCraft(1, typeof(MetalKiteShield), "Shields", "Metal Kite Shield", 50.0, 100.0, typeof(IronIngot), 1044036, 16, 1044037);
            AddCraft(1, typeof(HeaterShield), "Shields", "Heater Shield", 70.0, 120.0, typeof(IronIngot), 1044036, 18, 1044037);

            //Fencing
            AddCraft(1, typeof(Dagger), "Fencing Weapons", "Dagger", -10.0, 40.0, typeof(IronIngot), 1044036, 4, 1044037);
            AddCraft(1, typeof(Kryss), "Fencing Weapons", "Kryss", 20.0, 70.0, typeof(IronIngot), 1044036, 8, 1044037);
            AddCraft(1, typeof(WarFork), "Fencing Weapons", "War Fork", 20.0, 70.0, typeof(IronIngot), 1044036, 8, 1044037);
            AddCraft(1, typeof(ShortSpear), "Fencing Weapons", "Short Spear", 20.0, 70.0, typeof(IronIngot), 1044036, 8, 1044037);

            AddCraft(1, typeof(Pitchfork), "Fencing Weapons", "Pitchfork", 40.0, 90.0, typeof(IronIngot), 1044036, 12, 1044037);
            AddCraft(1, typeof(Spear), "Fencing Weapons", "Spear", 40.0, 90.0, typeof(IronIngot), 1044036, 12, 1044037);

            //Macing
            AddCraft(1, typeof(HammerPick), "Macing Weapons", "Hammer Pick", 20.0, 70.0, typeof(IronIngot), 1044036, 8, 1044037);
            AddCraft(1, typeof(WarAxe), "Macing Weapons", "War Axe", 20.0, 70.0, typeof(IronIngot), 1044036, 8, 1044037);
            AddCraft(1, typeof(Mace), "Macing Weapons", "Mace", 20.0, 70.0, typeof(IronIngot), 1044036, 8, 1044037);
            AddCraft(1, typeof(Maul), "Macing Weapons", "Maul", 20.0, 70.0, typeof(IronIngot), 1044036, 8, 1044037);
            AddCraft(1, typeof(WarMace), "Macing Weapons", "War Mace", 20.0, 70.0, typeof(IronIngot), 1044036, 8, 1044037);

            AddCraft(1, typeof(WarHammer), "Macing Weapons", "Warhammer", 40.0, 90.0, typeof(IronIngot), 1044036, 12, 1044037);

            //Swordsmanship			
            AddCraft(1, typeof(Cutlass), "Swordmanship Weapons", "Cutlass", 20.0, 70.0, typeof(IronIngot), 1044036, 8, 1044037);
            AddCraft(1, typeof(Katana), "Swordmanship Weapons", "Katana", 20.0, 70.0, typeof(IronIngot), 1044036, 8, 1044037);
            AddCraft(1, typeof(Scimitar), "Swordmanship Weapons", "Scimitar", 20.0, 70.0, typeof(IronIngot), 1044036, 8, 1044037);
            AddCraft(1, typeof(Broadsword), "Swordmanship Weapons", "Broadsword", 20.0, 70.0, typeof(IronIngot), 1044036, 8, 1044037);
            AddCraft(1, typeof(Longsword), "Swordmanship Weapons", "Longsword", 20.0, 70.0, typeof(IronIngot), 1044036, 8, 1044037);
            AddCraft(1, typeof(VikingSword), "Swordmanship Weapons", "Viking Sword", 20.0, 70.0, typeof(IronIngot), 1044036, 8, 1044037);

            AddCraft(1, typeof(Axe), "Swordmanship Weapons", "Axe", 40.0, 90.0, typeof(IronIngot), 1044036, 12, 1044037);
            AddCraft(1, typeof(BattleAxe), "Swordmanship Weapons", "Battle Axe", 40.0, 90.0, typeof(IronIngot), 1044036, 12, 1044037);
            AddCraft(1, typeof(DoubleAxe), "Swordmanship Weapons", "Double Axe", 40.0, 90.0, typeof(IronIngot), 1044036, 12, 1044037);
            AddCraft(1, typeof(ExecutionersAxe), "Swordmanship Weapons", "Executioner's Axe", 40.0, 90.0, typeof(IronIngot), 1044036, 12, 1044037);
            AddCraft(1, typeof(LargeBattleAxe), "Swordmanship Weapons", "Large Battle Axe", 40.0, 90.0, typeof(IronIngot), 1044036, 12, 1044037);
            AddCraft(1, typeof(TwoHandedAxe), "Swordmanship Weapons", "Two Handed Axe", 40.0, 90.0, typeof(IronIngot), 1044036, 12, 1044037);
            AddCraft(1, typeof(Bardiche), "Swordmanship Weapons", "Bardiche", 40.0, 90.0, typeof(IronIngot), 1044036, 12, 1044037);
            AddCraft(1, typeof(Halberd), "Swordmanship Weapons", "Halberd", 40.0, 90.0, typeof(IronIngot), 1044036, 12, 1044037);

            //Other Items
            index = AddCraft(1, typeof(BlacksmithyDungeonMould), "Misc", "Blacksmithing Dungeon Mould", 90, 105, typeof(IronIngot), "Iron Ingot", 200, 1044037);
            AddRes(index, typeof(FireEssence), "Fire Essence", 3, "You do not have the crafting components required to make this.");
            AddRes(index, typeof(Quartzstone), "Quartzstone", 3, "You do not have the crafting components required to make this.");
            AddRes(index, typeof(ObsidianShard), "Obsidian Shard", 3, "You do not have the crafting components required to make this.");
            //AddRes(index, typeof(LuniteHeart), "Lunite Heart", 3, "You do not have the crafting components required to make this.");

            /*
            AddCraft(1, typeof(IronOreHuePlating), "Other", "iron ore hue plating", 70, 120, typeof(IronIngot), "iron ingot", 25, "You do not have the neccesary ingots to construct that.");

            index = AddCraft(1, typeof(DullCopperOreHuePlating), "Other", "dull copper ore hue plating", 70, 120, typeof(DullCopperIngot), "dull copper ingot", 25, "You do not have the neccesary ingots to construct that.");
            AddRes(index, typeof(Ghostweed), "ghostweed", 5, "You do not have the neccessary crafting components to construct that.");

            index = AddCraft(1, typeof(ShadowIronOreHuePlating), "Other", "shadow iron ore hue plating", 70, 120, typeof(ShadowIronIngot), "shadow iron ingot", 25, "You do not have the neccesary ingots to construct that.");
            AddRes(index, typeof(ObsidianShard), "obsidian shard", 5, "You do not have the neccessary crafting components to construct that.");

            index = AddCraft(1, typeof(CopperOreHuePlating), "Other", "copper ore hue plating", 70, 120, typeof(CopperIngot), "copper ingot", 25, "You do not have the neccesary ingots to construct that.");
            AddRes(index, typeof(Snakeskin), "snakeskin", 5, "You do not have the neccessary crafting components to construct that.");

            index = AddCraft(1, typeof(BronzeOreHuePlating), "Other", "bronze ore hue plating", 70, 120, typeof(BronzeIngot), "bronze ingot", 25, "You do not have the neccesary ingots to construct that.");
            AddRes(index, typeof(Quartzstone), "quartzstone", 5, "You do not have the neccessary crafting components to construct that.");

            index = AddCraft(1, typeof(GoldOreHuePlating), "Other", "gold ore hue plating", 70, 120, typeof(GoldIngot), "gold ingot", 25, "You do not have the neccesary ingots to construct that.");
            AddRes(index, typeof(TrollFat), "troll fat", 5, "You do not have the neccessary crafting components to construct that.");

            index = AddCraft(1, typeof(AgapiteOreHuePlating), "Other", "agapite ore hue plating", 70, 120, typeof(AgapiteIngot), "agapite ingot", 25, "You do not have the neccesary ingots to construct that.");
            AddRes(index, typeof(GhoulHide), "ghoul hide", 5, "You do not have the neccessary crafting components to construct that.");

            index = AddCraft(1, typeof(VeriteOreHuePlating), "Other", "verite ore hue plating", 70, 120, typeof(VeriteIngot), "verite ingot", 25, "You do not have the neccesary ingots to construct that.");
            AddRes(index, typeof(ShatteredCrystal), "shattered crystal", 5, "You do not have the neccessary crafting components to construct that.");

            index = AddCraft(1, typeof(ValoriteOreHuePlating), "Other", "valorite ore hue plating", 70, 120, typeof(ValoriteIngot), "valorite ingot", 25, "You do not have the neccesary ingots to construct that.");
            AddRes(index, typeof(BluecapMushroom), "bluecap mushroom", 5, "You do not have the neccessary crafting components to construct that.");

            index = AddCraft(1, typeof(LuniteOreHuePlating), "Other", "lunite ore hue plating", 70, 120, typeof(LuniteIngot), "lunite ingot", 25, "You do not have the neccesary ingots to construct that.");
            AddRes(index, typeof(LuniteHeart), "lunite heart", 5, "You do not have the neccessary crafting components to construct that.");
            */

            // Set the overridable material
            SetSubRes(typeof(IronIngot), "Iron");

            //Add every material you want the player to be able to choose from
            //This will override the overridable material
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
            Repair = true;
            MarkOption = true;
        }
    }

    public class ForgeAttribute : Attribute
    {
        public ForgeAttribute()
        {
        }
    }

    public class AnvilAttribute : Attribute
    {
        public AnvilAttribute()
        {
        }
    }
}