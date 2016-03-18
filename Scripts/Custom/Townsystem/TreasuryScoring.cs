using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 *	Manager, tracker and gumps for OCB capture scores and rewards.
 * 
 */

using Server.Mobiles;
using Server.Network;
using Server.Items;
using Server;
using Server.Gumps;

namespace Server.Custom.Townsystem
{
    public class TreasuryKeyGoodiebag : MetalBox
    {
        public override string DefaultName
        {
            get
            {
                return "A gift from your militia leaders, for valor and courage shown in battle";
            }
        }

        [Constructable]
        public TreasuryKeyGoodiebag(PlayerMobile winner, int goldamount)
        {
            Hue = Utility.RandomNondyedHue();
            Weight = 1.0;
            DropItem(SkillScroll.Generate(winner, 120.0, 1));
            if (goldamount > 0)
                DropItem(new Gold(goldamount));
            else
            {
                Item i = Loot.RandomReagent();
                i.Amount = Utility.Random(75, 100);
                DropItem(i);
            }
        }

        public TreasuryKeyGoodiebag(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    ///////////////////////////////////////////////////////////////
    // REWARD ITEMS
    ///////////////////////////////////////////////////////////////
    public abstract class TreasuryReward : Item
    {
        public TreasuryReward(Serial s) : base(s) { }
        public TreasuryReward(int itemId) : base(itemId) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); }
        public override bool DisplayLootType { get { return true; } }


        public string m_GumpSpecialEffectDescription;
        public int m_KeyCost;
        public string m_BlessedToLabel;

        public abstract TreasuryReward CreateItem(PlayerMobile purchaser);
        public abstract double ModifyFactionSkillLossAmount(double val);
        public abstract double ModifyFactionSkillLossDuration(double val);
        public virtual double GetAdditionalGoldDropMultiplier() { return 1.0; }
        public abstract OCBRankingSystem.OCBRank RequiredRank();

        public override bool CanEquip(Mobile m)
        {
            PlayerMobile pm = m as PlayerMobile;
            if (pm == null)
                return base.CanEquip(m);

            if (pm.TownsystemPlayerState == null || pm.TownsystemPlayerState == null || (int)pm.TownsystemPlayerState.AllianceRank > (int)RequiredRank())
            {
                pm.SendMessage(String.Format("You must have reached an OCB ranking of {0} or higher to purchase this item", OCBRankingSystem.s_RankNameToString[(int)RequiredRank()]));
                return false;
            }
            else
                return base.CanEquip(m);
        }

        public override void LabelLootTypeTo(Mobile to)
        {
            // blessed to specific person
            Mobile bfor = BlessedFor;
            if (bfor != null)
            {
                LabelTo(to, String.Format("(blessed for {0})", bfor.Name));
            }
            else
            {
                base.LabelLootTypeTo(to);
            }
        }
    }

    public class YewWoodNecklace : TreasuryReward
    {
        public YewWoodNecklace(Serial s) : base(s) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); }
        public override string DefaultName { get { return "Yew Wood Necklace"; } }
        public override double ModifyFactionSkillLossAmount(double val) { return val; }
        public override double ModifyFactionSkillLossDuration(double val) { return val * 0.85; }
        public override OCBRankingSystem.OCBRank RequiredRank() { return OCBRankingSystem.OCBRank.Crusader; }

        public override void OnAdded(object parent)
        {
            base.OnAdded(parent);

            if (parent is Mobile)
                ((Mobile)parent).VirtualArmorMod += 1;
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);

            if (parent is Mobile)
                ((Mobile)parent).VirtualArmorMod -= 1;
        }

        public YewWoodNecklace()
            : base(0x3BB5)
        {
            Hue = 2310;
            m_GumpSpecialEffectDescription = "Faction Statloss Duration: -15%";
            m_KeyCost = 1700;
            Layer = Layer.Neck;
        }

        public override TreasuryReward CreateItem(PlayerMobile purchaser)
        {
            return new YewWoodNecklace();
        }
    }
    public class MinocIronRing : TreasuryReward
    {
        public MinocIronRing(Serial s) : base(s) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); }
        public override double ModifyFactionSkillLossAmount(double val) { return val; }
        public override double ModifyFactionSkillLossDuration(double val) { return val * 0.90; }
        public override OCBRankingSystem.OCBRank RequiredRank() { return OCBRankingSystem.OCBRank.Knight; }

        public override string DefaultName { get { return "Minoc Iron Ring"; } }
        public MinocIronRing()
            : base(0x1f09)
        {
            m_GumpSpecialEffectDescription = "Faction Statloss Duration: -10%";
            m_KeyCost = 900;
            Layer = Layer.Ring;
        }
        public override TreasuryReward CreateItem(PlayerMobile purchaser)
        {
            return new MinocIronRing();
        }
    }

    public class MasterHuntersIronRing : TreasuryReward
    {
        public MasterHuntersIronRing(Serial s) : base(s) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); }
        public override double ModifyFactionSkillLossAmount(double val) { return val; }
        public override double ModifyFactionSkillLossDuration(double val) { return val; }
        public override OCBRankingSystem.OCBRank RequiredRank() { return OCBRankingSystem.OCBRank.Knight; }
        public override double GetAdditionalGoldDropMultiplier() { return 1.15; }

        public static bool IsEquipedOn(Mobile killer)
        {
            if (killer == null) return false;

            var item = killer.FindItemOnLayer(Layer.Ring) as MasterHuntersIronRing;
            return item != null;
        }

        public override string DefaultName { get { return "Master Hunters Iron Ring"; } }
        [Constructable]
        public MasterHuntersIronRing()
            : base(0x1f09)
        {
            m_GumpSpecialEffectDescription = "+15% gold + chance for special loot";
            m_KeyCost = 300;
            Layer = Layer.Ring;
            Hue = 0x43;
        }
        public override TreasuryReward CreateItem(PlayerMobile purchaser)
        {
            return new MasterHuntersIronRing();
        }
    }

    public class HuntersIronRing : TreasuryReward
    {
        public HuntersIronRing(Serial s) : base(s) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); }
        public override double ModifyFactionSkillLossAmount(double val) { return val; }
        public override double ModifyFactionSkillLossDuration(double val) { return val; }
        public override OCBRankingSystem.OCBRank RequiredRank() { return OCBRankingSystem.OCBRank.Squire; }
        public override double GetAdditionalGoldDropMultiplier() { return 1.1; }

        public static bool IsEquipedOn(Mobile mob)
        {
            if (mob == null)
                return false;

            var item = mob.FindItemOnLayer(Layer.Ring) as HuntersIronRing;
            return item != null;
        }

        public override string DefaultName { get { return "Hunters Iron Ring"; } }
        [Constructable]
        public HuntersIronRing()
            : base(0x1f09)
        {
            m_GumpSpecialEffectDescription = "+10% gold in dungeons";
            m_KeyCost = 100;
            Layer = Layer.Ring;
            Hue = 0x43;
        }
        public override TreasuryReward CreateItem(PlayerMobile purchaser)
        {
            return new HuntersIronRing();
        }
    }

    public class BraceletOfVirtues : TreasuryReward
    {
        public BraceletOfVirtues(Serial s) : base(s) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); }
        public override double ModifyFactionSkillLossAmount(double val) { return val * 0.85; }
        public override double ModifyFactionSkillLossDuration(double val) { return val; }
        public override string DefaultName { get { return "Bracelet of Virtues"; } }
        public override OCBRankingSystem.OCBRank RequiredRank() { return OCBRankingSystem.OCBRank.Sentinel; }

        public BraceletOfVirtues()
            : base(0x1f06)
        {
            m_GumpSpecialEffectDescription = "Faction Statloss Intensity: -15%";
            m_KeyCost = 600;
            Layer = Layer.Bracelet;
        }
        public override TreasuryReward CreateItem(PlayerMobile purchaser)
        {
            return new BraceletOfVirtues();
        }
    }

    public class HoodedShroudOfTheChampion : TreasuryReward
    {
        public HoodedShroudOfTheChampion(Serial s) : base(s) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); }
        public override double ModifyFactionSkillLossAmount(double val) { return val; }
        public override double ModifyFactionSkillLossDuration(double val) { return val; }
        public override string DefaultName { get { return "Hooded Shroud of the Champion"; } }
        public override OCBRankingSystem.OCBRank RequiredRank() { return OCBRankingSystem.OCBRank.Champion; }

        public HoodedShroudOfTheChampion()
            : base(0x2683)
        {
            m_GumpSpecialEffectDescription = "Great looks";
            m_KeyCost = 5000;
            Layer = Layer.OuterTorso;
        }
        public override TreasuryReward CreateItem(PlayerMobile purchaser)
        {
            TreasuryReward i = new HoodedShroudOfTheChampion();
            i.Hue = purchaser.TownsystemPlayerState.Town.HomeFaction.Definition.HuePrimary;
            return i;
        }
        public override bool OnEquip(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;
            if (pm.TownsystemPlayerState == null || pm.TownsystemPlayerState.Town == null || pm.TownsystemPlayerState.Town.HomeFaction == null)
            {
                // this should never happen, should have been caught in CanEquip
                Hue = 0x0;
            }
            else
            {
                Hue = pm.TownsystemPlayerState.Town.HomeFaction.Definition.HuePrimary;
            }
            return base.OnEquip(from);
        }
    }

    public class HerosTalisman : TreasuryReward
    {
        public HerosTalisman(Serial s) : base(s) { }
        public override double ModifyFactionSkillLossAmount(double val) { return val; }
        public override double ModifyFactionSkillLossDuration(double val) { return val; }
        public override string DefaultName { get { return "Hero's Talisman"; } }
        public override OCBRankingSystem.OCBRank RequiredRank() { return OCBRankingSystem.OCBRank.Hero; }

        private int m_Hue;
        [Hue, CommandProperty(AccessLevel.GameMaster)]
        public int DyeHue { get { return m_Hue; } set { m_Hue = value; } }

        [Constructable]
        public HerosTalisman(int hue = 0x0)
            : base(0x2F58)
        {
            m_GumpSpecialEffectDescription = "Shades 2Handed weapons when worn";
            m_KeyCost = 4500;
            Layer = Layer.Talisman;
            m_Hue = hue;
            Hue = hue;	// Item.Hue
        }
        public override TreasuryReward CreateItem(PlayerMobile purchaser)
        {
            TreasuryReward i = new HerosTalisman(purchaser.TownsystemPlayerState.Town.HomeFaction.Definition.HuePrimary);
            i.Hue = purchaser.TownsystemPlayerState.Faction.Alliance.BannerHue;
            return i;
        }

        public virtual void HueItem(Item item)
        {
            if (item.Layer == Layer.TwoHanded)
            {
                SetNewAndOriginalHue(item, m_Hue);
            }
        }

        public override bool OnEquip(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;
            if (pm.TownsystemPlayerState == null || pm.TownsystemPlayerState.Town == null || pm.TownsystemPlayerState.Town.HomeFaction == null)
            {
                // this should never happen, should have been caught in CanEquip
                Hue = m_Hue = 0x0;
            }
            else
            {
                Hue = m_Hue = pm.TownsystemPlayerState.Town.HomeFaction.Definition.HuePrimary;
            }
            return base.OnEquip(from);
        }


        protected void SetNewAndOriginalHue(Item item, int new_hue)
        {
            // why this? Setting item.Hue also sets item.OriginalHue
            // when we do this we want the Hue to be the new Hue and OriginalHue to be whatever was before.
            // this is the ONLY place we should have to take this into consideration.
            if (item.Hue != new_hue)
            {
                int old_hue = item.Hue;
                item.Hue = m_Hue;
                item.OriginalHue = old_hue;
            }
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);
            Mobile from = parent as Mobile;
            if (from != null)
            {
                foreach (Item i in from.Items)
                    i.Hue = i.OriginalHue;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write(m_Hue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            m_Hue = reader.ReadInt();
        }
    }

    public class VeteranKnightsTalisman : TreasuryReward
    {
        public VeteranKnightsTalisman(Serial s) : base(s) { }
        public override double ModifyFactionSkillLossAmount(double val) { return val; }
        public override double ModifyFactionSkillLossDuration(double val) { return val; }
        public override string DefaultName { get { return "Veteran Knights's Talisman"; } }
        public override OCBRankingSystem.OCBRank RequiredRank() { return OCBRankingSystem.OCBRank.Sentinel; }

        private int m_Hue;
        [Hue, CommandProperty(AccessLevel.GameMaster)]
        public int DyeHue { get { return m_Hue; } set { m_Hue = value; } }

        [Constructable]
        public VeteranKnightsTalisman(int hue = 0x0)
            : base(0x2F58)
        {
            m_GumpSpecialEffectDescription = "Shades 1Handed weapons when worn";
            m_KeyCost = 3500;
            Layer = Layer.Talisman;
            m_Hue = hue;
            Hue = hue;	// Item.Hue
        }
        public override TreasuryReward CreateItem(PlayerMobile purchaser)
        {
            TreasuryReward i = new VeteranKnightsTalisman(purchaser.TownsystemPlayerState.Town.HomeFaction.Definition.HuePrimary);
            i.Hue = purchaser.TownsystemPlayerState.Town.HomeFaction.Definition.HuePrimary;
            return i;
        }

        public virtual void HueItem(Item item)
        {
            if (item.Layer == Layer.OneHanded)
            {
                SetNewAndOriginalHue(item, m_Hue);
            }
        }

        public override bool OnEquip(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;
            if (pm.TownsystemPlayerState == null || pm.TownsystemPlayerState.Town == null || pm.TownsystemPlayerState.Town.HomeFaction == null)
            {
                // this should never happen, should have been caught in CanEquip
                Hue = m_Hue = 0x0;
            }
            else
            {
                Hue = m_Hue = pm.TownsystemPlayerState.Town.HomeFaction.Definition.HuePrimary;
            }
            return base.OnEquip(from);
        }


        protected void SetNewAndOriginalHue(Item item, int new_hue)
        {
            // why this? Setting item.Hue also sets item.OriginalHue
            // when we do this we want the Hue to be the new Hue and OriginalHue to be whatever was before.
            // this is the ONLY place we should have to take this into consideration.
            if (item.Hue != new_hue)
            {
                int old_hue = item.Hue;
                item.Hue = m_Hue;
                item.OriginalHue = old_hue;
            }
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);
            Mobile from = parent as Mobile;
            if (from != null)
            {
                foreach (Item i in from.Items)
                    i.Hue = i.OriginalHue;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write(m_Hue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            m_Hue = reader.ReadInt();
        }
    }


    ///////////////////////////////////////////////////////////////
    // REWARDS GUMP
    ///////////////////////////////////////////////////////////////
    public class TreasuryKeyGump : Server.Gumps.Gump
    {
        private static List<TreasuryReward> s_AllRewards = new List<TreasuryReward>()
		{
			new HoodedShroudOfTheChampion(),
            new HerosTalisman(),
			new VeteranKnightsTalisman(),
			new YewWoodNecklace(),
			new BraceletOfVirtues(),
			new MinocIronRing(),
			new MasterHuntersIronRing(),
			new HuntersIronRing(),
		};

        private static int ITEMS_PER_PAGE = 4;
        PlayerMobile m_From;
        int m_CurrentPage;
        public TreasuryKeyGump(Mobile from, int page = 0)
            : base(0, 0)
        {
            m_From = from as PlayerMobile;
            m_CurrentPage = Math.Max(page, 0);

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
            this.AddPage(0);
            this.AddImage(23, 44, 206);
            this.AddImageTiled(127, 147, 21, 21, 200);
            this.AddImageTiled(67, 85, 539, 424, 200);
            this.AddImage(66, 44, 201);
            this.AddImage(178, 44, 201);
            this.AddImage(605, 82, 203);
            this.AddImage(605, 465, 205);
            this.AddImage(605, 149, 203);
            this.AddImage(23, 465, 204);
            this.AddImage(604, 44, 207);
            this.AddImage(178, 465, 233);
            this.AddImage(66, 465, 233);
            this.AddImage(23, 88, 202);
            this.AddImage(23, 149, 202);
            this.AddImage(615, 77, 10441);
            this.AddImageTiled(127, 278, 21, 21, 200);

            // header
            this.AddImageTiled(119, 117, 448, 3, 9102);
            this.AddLabel(269, 63, 53, @"Treasury Theft Rewards");
            this.AddLabel(52, 89, 0, @"Item");
            this.AddLabel(304, 89, 0, @"Special Effect");
            this.AddLabel(500, 89, 0, @"Cost");
            this.AddLabel(546, 89, 0, @"Purchase");
            this.AddImageTiled(127, 214, 21, 21, 200);

            // page items
            int nameX = 50;
            int nameY = 131;
            int rankReqX = 58;
            int rankReqY = 150;
            int sfxX = 249;
            int sfxY = 150;
            int costX = 506;
            int costY = 150;
            int gfxX = 83;
            int gfxY = 170;
            int btnX = 563;
            int btnY = 153;
            int keyGfxX = 530;
            int keyGfxY = 154;
            int start = page * ITEMS_PER_PAGE;
            int end = Math.Min(start + ITEMS_PER_PAGE, s_AllRewards.Count);

            int reqRanqHue = 100;

            int ydelta = 0;
            for (int i = start; i < end; ++i)
            {
                TreasuryReward rewitem = s_AllRewards[i];
                // item 1
                AddLabel(nameX, nameY + ydelta, 2036, rewitem.Name);
                AddLabel(rankReqX, rankReqY + ydelta, reqRanqHue, String.Format("Required rank: {0}", OCBRankingSystem.s_RankNameToString[(int)rewitem.RequiredRank()]));
                AddLabel(sfxX, sfxY + ydelta, 53, rewitem.m_GumpSpecialEffectDescription);

                if (rewitem.m_KeyCost > 1000) 
                {
                    double cost = rewitem.m_KeyCost / 1000.0;
                    AddLabel(costX, costY + ydelta, 0, String.Format("{0}k", cost));
                }
                else
                {
                    AddLabel(costX, costY + ydelta, 0, rewitem.m_KeyCost.ToString());
                }
                AddItem(gfxX, gfxY + ydelta, rewitem.ItemID, rewitem.Hue); // reward item
                AddButton(btnX, btnY + ydelta, 4005, 4007, (int)Buttons.BtnPurchaseStart + i, GumpButtonType.Reply, 0);
                AddItem(keyGfxX, keyGfxY + ydelta, 4114); // key gfx

                ydelta += 83;
            }

            // page flipping
            if (page > 0)
                this.AddButton(544, 433, 2223, 2223, (int)Buttons.BtnPrevPage, GumpButtonType.Reply, 0);
            this.AddButton(572, 433, 2224, 2224, (int)Buttons.BtnNextPage, GumpButtonType.Reply, 0);

            // footer
            AddImageTiled(119, 448, 448, 3, 9102); // divider

            AddLabel(167, 454, 0, @"Acquire more keys by participating in militia wars");
            AddLabel(sfxX, 472, 52, String.Format("CURRENT BALANCE: {0}", m_From.TreasuryKeys));
            AddItem(415, 476, 4114);

            AddLabel(btnX - 75, 472, 2036, "Rankings");
            AddButton(btnX, 472, 0xfa5, 0xfa7, (int)Buttons.BtnShowLeaderboards, GumpButtonType.Reply, 0); // complete now
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == (int)Buttons.BtnPrevPage && m_CurrentPage > 0)
            {
                sender.Mobile.SendGump(new TreasuryKeyGump(sender.Mobile, m_CurrentPage - 1));
            }
            else if (info.ButtonID == (int)Buttons.BtnNextPage)
            {
                sender.Mobile.SendGump(new TreasuryKeyGump(sender.Mobile, m_CurrentPage + 1));
            }
            else if (info.ButtonID == (int)Buttons.BtnShowLeaderboards)
            {
                if (sender.Mobile.Player)
                    sender.Mobile.SendGump(new OCBLeaderboards_New(sender.Mobile as PlayerMobile, 0));
            }
            else if (info.ButtonID >= (int)Buttons.BtnPurchaseStart)
            {
                int which_item = info.ButtonID - (int)Buttons.BtnPurchaseStart;
                if (which_item >= 0 && s_AllRewards.Count > which_item)
                {
                    // try purchasing the item
                    PlayerMobile player = sender.Mobile as PlayerMobile;
                    if (player != null)
                    {
                        if (!player.IsInMilitia)
                        {
                            player.SendMessage("You must be a member of an OCB militia to purchase this item");
                            sender.Mobile.SendGump(new TreasuryKeyGump(sender.Mobile, m_CurrentPage));
                            return;
                        }
                        TreasuryReward rew = s_AllRewards[which_item];

                        if ((int)player.TownsystemPlayerState.AllianceRank > (int)rew.RequiredRank())
                        {
                            player.SendMessage(String.Format("You must have reached an OCB ranking of {0} or higher to equip this item", OCBRankingSystem.s_RankNameToString[(int)rew.RequiredRank()]));
                            player.SendGump(new TreasuryKeyGump(player, m_CurrentPage));
                            return;
                        }


                        if (player.TreasuryKeys < rew.m_KeyCost)
                        {
                            player.SendMessage("You can not afford that.");
                            player.SendGump(new TreasuryKeyGump(player, m_CurrentPage));
                        }
                        else
                        {
                            TreasuryReward reward = rew.CreateItem(player);
                            reward.BlessedFor = player;
                            if (player.Backpack.TryDropItem(player, reward, true))
                            {
                                player.SendMessage("Your purchase the item.");
                                player.TreasuryKeys -= rew.m_KeyCost;
                            }
                        }
                    }
                }
            }
            else
            {
                sender.Mobile.SendGump(new IPYGump(sender.Mobile));
                return;
            }
        }

        public enum Buttons
        {
            BtnPrevPage = 1,
            BtnNextPage,
            BtnShowLeaderboards,
            BtnPurchaseStart = 513,
        }
    }
}