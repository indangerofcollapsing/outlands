using System;
using Server.Items;
using Server.Mobiles;
using Server.Spells;

using Server.Custom;

namespace Server.Engines.Craft
{
    public class DefInscription : CraftSystem
    {
        public override SkillName MainSkill
        {
            get { return SkillName.Inscribe; }
        }

        public override int GumpTitleNumber
        {
            get { return 1044009; } // <CENTER>INSCRIPTION MENU</CENTER>
        }

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new DefInscription();

                return m_CraftSystem;
            }
        }

        public override double GetChanceAtMin(CraftItem item)
        {
            return 0.0; // 0%
        }

        private DefInscription()
            : base(1, 1, 1.25)// base( 1, 1, 3.0 )
        {
        }

        public override int CanCraft(Mobile from, BaseTool tool, Type typeItem)
        {
            if (tool == null || tool.Deleted || tool.UsesRemaining < 0)
                return 1044038; // You have worn out your tool!

            else if (!BaseTool.CheckAccessible(tool, from))
                return 1044263; // The tool must be on your person to use.

            if (typeItem != null)
            {
                object item = Activator.CreateInstance(typeItem);

                if (item is SpellScroll)
                {
                    SpellScroll scroll = (SpellScroll)item;
                    Spellbook book = Spellbook.Find(from, scroll.SpellID);

                    bool hasSpell = (book != null && book.HasSpell(scroll.SpellID));

                    if (scroll.SpellID >= 48 && scroll.SpellID <= 63)
                    {
                        Item[] packScrolls = from.Backpack.FindItemsByType(typeof(SpellScroll));

                        for (int i = 0; i < packScrolls.Length; i++)
                        {
                            SpellScroll spellScroll = (SpellScroll)packScrolls[i];

                            if (spellScroll != null && spellScroll.SpellID == scroll.SpellID && spellScroll.MasterStatus > 0)
                            {
                                scroll.Delete();
                                return 0;
                            }
                        }

                        scroll.Delete();
                        from.SendMessage("You must have a master copy of the scroll in your pack in order to inscribe this spell");

                        return 1042404;
                    }

                    else
                    {
                        scroll.Delete();
                        return (hasSpell ? 0 : 1042404); // null : You don't have that spell!
                    }
                }

                else if (item is Item)
                {
                    ((Item)item).Delete();
                }
            }

            return 0;
        }

        public override void PlayCraftEffect(Mobile from)
        {
            from.PlaySound(0x249);
        }

        private static Type typeofSpellScroll = typeof(SpellScroll);

        public override int PlayEndingEffect(Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item)
        {
            if (toolBroken)
                from.SendLocalizedMessage(1044038); // You have worn out your tool

            if (!typeofSpellScroll.IsAssignableFrom(item.ItemType)) //  not a scroll
            {
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

            else
            {
                Type[] types = Loot.RegularScrollTypes;
                int ID = Array.IndexOf(types, item.ItemType);

                if (ID >= 48 && ID <= 63)
                {
                    Item[] packScrolls = from.Backpack.FindItemsByType(typeof(SpellScroll));

                    for (int i = 0; i < packScrolls.Length; i++)
                    {
                        SpellScroll ss = (SpellScroll)packScrolls[i];

                        if (ss != null && ss.SpellID == ID && ss.MasterStatus > 0)
                        {
                            if (--ss.UsesRemaining <= 0)
                            {
                                from.SendMessage("The master scroll is destroyed during the inscription process.");
                                ss.Delete();
                            }

                            break;
                        }
                    }
                }

                if (failed)
                    return 501630; // You fail to inscribe the scroll, and the scroll is ruined.
                else                                   
                    return 501629; // You inscribe the spell and put the scroll in your backpack.                
            }
        }

        private int m_Circle, m_Mana;

        private enum Reg { BlackPearl, Bloodmoss, Garlic, Ginseng, MandrakeRoot, Nightshade, SulfurousAsh, SpidersSilk }

        private Type[] m_RegTypes = new Type[]
            {
                typeof( BlackPearl ),
                typeof( Bloodmoss ),
                typeof( Garlic ),
                typeof( Ginseng ),
                typeof( MandrakeRoot ),
                typeof( Nightshade ),
                typeof( SulfurousAsh ),
                typeof( SpidersSilk )
            };

        private int m_Index;

        private void AddSpell(Type type, params Reg[] regs)
        {
            double minSkill, maxSkill;

            switch (m_Circle)
            {
                default:
                case 0: minSkill = -25.0; maxSkill = 25.0; break;
                case 1: minSkill = -10.8; maxSkill = 39.2; break;
                case 2: minSkill = 03.5; maxSkill = 53.5; break;
                case 3: minSkill = 17.8; maxSkill = 67.8; break;
                case 4: minSkill = 32.1; maxSkill = 82.1; break;
                case 5: minSkill = 46.4; maxSkill = 96.4; break;
                case 6: minSkill = 60.7; maxSkill = 110.7; break;
                case 7: minSkill = 75.0; maxSkill = 125.0; break;
            }

            string groupName = "Circle 1-6 Spells";

            if (m_Circle >= 6)
                groupName = "Circle 7-8 Spells";

            int index = AddCraft(1,type, groupName, 1044381 + m_Index++, minSkill, maxSkill, m_RegTypes[(int)regs[0]], 1044353 + (int)regs[0], 1, 1044361 + (int)regs[0]);

            double ChanceOffset = 20.0, ChanceLength = 100.0 / 7.5;

            double minimumMagery = (m_Circle + 1) * ChanceLength - ChanceOffset;

            AddSkill(index, SkillName.Magery, minimumMagery, minimumMagery);

            for (int i = 1; i < regs.Length; ++i)
                AddRes(index, m_RegTypes[(int)regs[i]], 1044353 + (int)regs[i], 1, 1044361 + (int)regs[i]);

            AddRes(index, typeof(BlankScroll), 1044377, 1, 1044378);

            SetManaReq(index, m_Mana);
        }

        private void AddNecroSpell(int spell, int mana, double minSkill, Type type, params Type[] regs)
        {
            int id = CraftItem.ItemIDOf(regs[0]);

            int index = AddCraft(1,type, 1061677, 1060509 + spell, minSkill, minSkill + 1.0, regs[0], id < 0x4000 ? 1020000 + id : 1078872 + id, 1, 501627);

            for (int i = 1; i < regs.Length; ++i)
            {
                id = CraftItem.ItemIDOf(regs[i]);
                AddRes(index, regs[i], id < 0x4000 ? 1020000 + id : 1078872 + id, 1, 501627);
            }

            AddRes(index, typeof(BlankScroll), 1044377, 1, 1044378);

            SetManaReq(index, mana);
        }

        public override void InitCraftList()
        {
            int index;

            m_Circle = 0;
            m_Mana = 2;

            //Utility
            index = AddCraft(1, typeof(Spellbook), "Utility", "Spellbook", 25.0, 50.0, typeof(BlankScroll), 1044377, 2, 1044378);
            ForceNonExceptional(index);

            index = AddCraft(1, typeof(Runebook), "Utility", "Runebook", 45.0, 95.0, typeof(BlankScroll), 1044377, 8, 1044378);
            AddRes(index, typeof(RecallScroll), 1044445, 1, 1044253);
            AddRes(index, typeof(ResurrectionScroll), 1044439, 1, 1044253);
            AddCraft(1, typeof(Engines.BulkOrders.BulkOrderBook), 1044294, 1028793, 65.0, 115.0, typeof(BlankScroll), 1044377, 10, 1044378);

            index = AddCraft(1, typeof(AncientMystery.AncientMysteryScroll), "Utility", "Ancient Mystery Scroll", 80.0, 120.0, typeof(BlankScroll), 1044377, 25, 1044378);
            AddRes(index, typeof(GhoulHide), "Ghoul Hide", 2, "You do not have the neccesary crafting component needed to make this");

            //Circle 1-6 Spells
            AddSpell(typeof(ReactiveArmorScroll), Reg.Garlic, Reg.SpidersSilk, Reg.SulfurousAsh);
            AddSpell(typeof(ClumsyScroll), Reg.Bloodmoss, Reg.Nightshade);
            AddSpell(typeof(CreateFoodScroll), Reg.Garlic, Reg.Ginseng, Reg.MandrakeRoot);
            AddSpell(typeof(FeeblemindScroll), Reg.Nightshade, Reg.Ginseng);
            AddSpell(typeof(HealScroll), Reg.Garlic, Reg.Ginseng, Reg.SpidersSilk);
            AddSpell(typeof(MagicArrowScroll), Reg.SulfurousAsh);
            AddSpell(typeof(NightSightScroll), Reg.SpidersSilk, Reg.SulfurousAsh);
            AddSpell(typeof(WeakenScroll), Reg.Garlic, Reg.Nightshade);

            m_Circle = 1;
            m_Mana = 3;

            AddSpell(typeof(AgilityScroll), Reg.Bloodmoss, Reg.MandrakeRoot);
            AddSpell(typeof(CunningScroll), Reg.Nightshade, Reg.MandrakeRoot);
            AddSpell(typeof(CureScroll), Reg.Garlic, Reg.Ginseng);
            AddSpell(typeof(HarmScroll), Reg.Nightshade, Reg.SpidersSilk);
            AddSpell(typeof(MagicTrapScroll), Reg.Garlic, Reg.SpidersSilk, Reg.SulfurousAsh);
            AddSpell(typeof(MagicUnTrapScroll), Reg.Bloodmoss, Reg.SulfurousAsh);
            AddSpell(typeof(ProtectionScroll), Reg.Garlic, Reg.Ginseng, Reg.SulfurousAsh);
            AddSpell(typeof(StrengthScroll), Reg.Nightshade, Reg.MandrakeRoot);

            m_Circle = 2;
            m_Mana = 4;

            AddSpell(typeof(BlessScroll), Reg.Garlic, Reg.MandrakeRoot);
            AddSpell(typeof(FireballScroll), Reg.BlackPearl);
            AddSpell(typeof(MagicLockScroll), Reg.Bloodmoss, Reg.Garlic, Reg.SulfurousAsh);
            AddSpell(typeof(PoisonScroll), Reg.Nightshade);
            AddSpell(typeof(TelekinisisScroll), Reg.Bloodmoss, Reg.MandrakeRoot);
            AddSpell(typeof(TeleportScroll), Reg.Bloodmoss, Reg.MandrakeRoot);
            AddSpell(typeof(UnlockScroll), Reg.Bloodmoss, Reg.SulfurousAsh);
            AddSpell(typeof(WallOfStoneScroll), Reg.Bloodmoss, Reg.Garlic);

            m_Circle = 3;
            m_Mana = 6;

            AddSpell(typeof(ArchCureScroll), Reg.Garlic, Reg.Ginseng, Reg.MandrakeRoot);
            AddSpell(typeof(ArchProtectionScroll), Reg.Garlic, Reg.Ginseng, Reg.MandrakeRoot, Reg.SulfurousAsh);
            AddSpell(typeof(CurseScroll), Reg.Garlic, Reg.Nightshade, Reg.SulfurousAsh);
            AddSpell(typeof(FireFieldScroll), Reg.BlackPearl, Reg.SpidersSilk, Reg.SulfurousAsh);
            AddSpell(typeof(GreaterHealScroll), Reg.Garlic, Reg.SpidersSilk, Reg.MandrakeRoot, Reg.Ginseng);
            AddSpell(typeof(LightningScroll), Reg.MandrakeRoot, Reg.SulfurousAsh);
            AddSpell(typeof(ManaDrainScroll), Reg.BlackPearl, Reg.SpidersSilk, Reg.MandrakeRoot);
            AddSpell(typeof(RecallScroll), Reg.BlackPearl, Reg.Bloodmoss, Reg.MandrakeRoot);

            m_Circle = 4;
            m_Mana = 7;

            AddSpell(typeof(BladeSpiritsScroll), Reg.BlackPearl, Reg.Nightshade, Reg.MandrakeRoot);
            AddSpell(typeof(DispelFieldScroll), Reg.BlackPearl, Reg.Garlic, Reg.SpidersSilk, Reg.SulfurousAsh);
            AddSpell(typeof(IncognitoScroll), Reg.Bloodmoss, Reg.Garlic, Reg.Nightshade);
            AddSpell(typeof(MagicReflectScroll), Reg.Garlic, Reg.MandrakeRoot, Reg.SpidersSilk);
            AddSpell(typeof(MindBlastScroll), Reg.BlackPearl, Reg.MandrakeRoot, Reg.Nightshade, Reg.SulfurousAsh);
            AddSpell(typeof(ParalyzeScroll), Reg.Garlic, Reg.MandrakeRoot, Reg.SpidersSilk);
            AddSpell(typeof(PoisonFieldScroll), Reg.BlackPearl, Reg.Nightshade, Reg.SpidersSilk);
            AddSpell(typeof(SummonCreatureScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk);

            m_Circle = 5;
            m_Mana = 10;

            AddSpell(typeof(DispelScroll), Reg.Garlic, Reg.MandrakeRoot, Reg.SulfurousAsh);
            AddSpell(typeof(EnergyBoltScroll), Reg.BlackPearl, Reg.Nightshade);
            AddSpell(typeof(ExplosionScroll), Reg.Bloodmoss, Reg.MandrakeRoot);
            AddSpell(typeof(InvisibilityScroll), Reg.Bloodmoss, Reg.Nightshade);
            AddSpell(typeof(MarkScroll), Reg.Bloodmoss, Reg.BlackPearl, Reg.MandrakeRoot);
            AddSpell(typeof(MassCurseScroll), Reg.Garlic, Reg.MandrakeRoot, Reg.Nightshade, Reg.SulfurousAsh);
            AddSpell(typeof(ParalyzeFieldScroll), Reg.BlackPearl, Reg.Ginseng, Reg.SpidersSilk);
            AddSpell(typeof(RevealScroll), Reg.Bloodmoss, Reg.SulfurousAsh);

            //Circle 7-8 Spells
            m_Circle = 6;
            m_Mana = 20;

            AddSpell(typeof(ChainLightningScroll), Reg.BlackPearl, Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SulfurousAsh);
            AddSpell(typeof(EnergyFieldScroll), Reg.BlackPearl, Reg.MandrakeRoot, Reg.SpidersSilk, Reg.SulfurousAsh);
            AddSpell(typeof(FlamestrikeScroll), Reg.SpidersSilk, Reg.SulfurousAsh);
            AddSpell(typeof(GateTravelScroll), Reg.BlackPearl, Reg.MandrakeRoot, Reg.SulfurousAsh);
            AddSpell(typeof(ManaVampireScroll), Reg.BlackPearl, Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk);
            AddSpell(typeof(MassDispelScroll), Reg.BlackPearl, Reg.Garlic, Reg.MandrakeRoot, Reg.SulfurousAsh);
            AddSpell(typeof(MeteorSwarmScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SulfurousAsh, Reg.SpidersSilk);
            AddSpell(typeof(PolymorphScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk);

            m_Circle = 7;
            m_Mana = 25;

            AddSpell(typeof(EarthquakeScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.Ginseng, Reg.SulfurousAsh);
            AddSpell(typeof(EnergyVortexScroll), Reg.BlackPearl, Reg.Bloodmoss, Reg.MandrakeRoot, Reg.Nightshade);
            AddSpell(typeof(ResurrectionScroll), Reg.Bloodmoss, Reg.Garlic, Reg.Ginseng);
            AddSpell(typeof(SummonAirElementalScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk);
            AddSpell(typeof(SummonDaemonScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk, Reg.SulfurousAsh);
            AddSpell(typeof(SummonEarthElementalScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk);
            AddSpell(typeof(SummonFireElementalScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk, Reg.SulfurousAsh);
            AddSpell(typeof(SummonWaterElementalScroll), Reg.Bloodmoss, Reg.MandrakeRoot, Reg.SpidersSilk);

            MarkOption = true;
        }
    }
}