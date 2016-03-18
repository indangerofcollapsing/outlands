using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Multis;
using Server.Custom.Townsystem;
using Server.Mobiles;
using Server.Custom.Items.Totem;

namespace Server.Custom.Paladin
{

    public class MilitiaRewardEntry 
    {
        public int KeyCost = 0;
        public bool DyeTownColor = false;
        public Type Reward;
        public int ItemID;
        public int Hue;
        public string Name;

        public MilitiaRewardEntry(string name, Type reward, int itemID, int hue, int keyCost, bool dye = false) 
        {
            Name = name;
            Reward = reward;
            ItemID = itemID;
            Hue = hue;
            KeyCost = keyCost;
            DyeTownColor = dye;
        }

        public Item Create(Town town)
        {
            Item reward = Activator.CreateInstance(Reward) as Item;
            if (reward == null) return null;

            if (reward is FurnitureDyeTub)
            {
                ((FurnitureDyeTub)reward).UsesRemaining = 5;
                ((FurnitureDyeTub) reward).DyedHue = town.HomeFaction.Definition.HuePrimary;
            }

            if (DyeTownColor)
                reward.Hue = town.HomeFaction.Definition.HuePrimary;

            return reward;
        }
    }

    public class MilitiaVendorGump : Gump
    {
        private static List<MilitiaRewardEntry> Rewards = new List<MilitiaRewardEntry>()
        {
            new MilitiaRewardEntry("Furniture Dye Tub", typeof(FurnitureDyeTub), 0xFAB, 0, 10000, true),
            new MilitiaRewardEntry("Infiltration Dust", typeof(InfiltrationDust), 3983, 0, 20),
            new MilitiaRewardEntry("Baron's Ring", typeof(BaronsRing), 0x108a, 0, 1000),
            new MilitiaRewardEntry("Flimsy Baron's Ring", typeof(FlimsyBaronsRing), 0x108a, 0, 750),
            new MilitiaRewardEntry("Bronze Statue Maker", typeof(BronzeStatueMaker), 0x32F0, 2967, 30000),
            new MilitiaRewardEntry("Jade Statue Maker", typeof(JadeStatueMaker), 0x32F0, 2963, 60000),
            new MilitiaRewardEntry("Marble Statue Maker", typeof(MarbleStatueMaker), 0x32F0, 2959, 90000),
            new MilitiaRewardEntry("Finisher Totem", typeof(FinisherTotem), 0x2F5B, 0, 3000, true),
            new MilitiaRewardEntry("a militia robe", typeof(Server.Custom.Townsystem.Items.MilitiaRobe), 7939, 0, 1000, true),
            new MilitiaRewardEntry("a militia cloak", typeof(Server.Custom.Townsystem.Items.MilitiaCloak), 5397, 0, 1000, true),
            new MilitiaRewardEntry("a militia skirt", typeof(Server.Custom.Townsystem.Items.MilitiaSkirt), 5398, 0, 1000, true),
            new MilitiaRewardEntry("a militia doublet", typeof(Server.Custom.Townsystem.Items.MilitiaDoublet), 8059, 0, 1000, true),
            new MilitiaRewardEntry("a militia headband", typeof(Server.Custom.Townsystem.Items.MilitiaHeadband), 5440, 0, 1000, true),
            new MilitiaRewardEntry("a militia sash", typeof(Server.Custom.Townsystem.Items.MilitiaSash), 5441, 0, 1000, true),
            new MilitiaRewardEntry("a militia half apron", typeof(Server.Custom.Townsystem.Items.MilitiaHalfApron), 5435, 0, 1000, true),
            new MilitiaRewardEntry("a militia kilt", typeof(Server.Custom.Townsystem.Items.MilitiaKilt), 5431, 0, 1000, true),
            new MilitiaRewardEntry("a militia wizard hat", typeof(Server.Custom.Townsystem.Items.MilitiaWizardHat), 5912, 0, 1000, true),
            new MilitiaRewardEntry("a militia tunic", typeof(Server.Custom.Townsystem.Items.MilitiaTunic), 8097, 0, 1000, true),
            new MilitiaRewardEntry("a militia lantern", typeof(Server.Custom.Townsystem.Items.MilitiaLantern), 2597, 0, 20000, true),
        };

        public int m_PageNo = 0;

        public MilitiaVendorGump(int PageNo)
            : base(0, 0)
        {
            m_PageNo = PageNo;
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
            this.AddPage(0);
            this.AddImage(0, 0, 11010);

            var items = Rewards.Skip(PageNo * 2).Take(2).ToList();

            var first = items.First();
            this.AddButton(78, 170, 1154, 1153, (int)Buttons.btnPurchase1, GumpButtonType.Reply, 0);
            this.AddHtml(60, 12, 145, 44, first.Name, false, false);
            this.AddLabel(111, 171, 0x0, @"Purchase");
            if (first.DyeTownColor)
            {
                this.AddLabel(70, 106, 0x0, "Dyed town color");
            }

            this.AddItem(100, 61, first.ItemID, first.Hue); 

            this.AddLabel(60, 131, 0x0, String.Format("Treasury Keys: {0}", first.KeyCost));

            if (items.Count > 1)
            {
                var second = items.Last();

                this.AddButton(234, 170, 1154, 1153, (int)Buttons.btnPurchase2, GumpButtonType.Reply, 0);
                this.AddHtml(208, 12, 145, 44, second.Name, false, false);
                if (second.DyeTownColor)
                {
                    this.AddLabel(225, 106, 0x0, "Dyed town color");
                }

                this.AddLabel(216, 129, 0x0, String.Format("Treasury Keys: {0}", second.KeyCost));

                this.AddLabel(267, 171, 0x0, @"Purchase");
                this.AddItem(256, 61, second.ItemID, second.Hue);
            }

            if (PageNo < (Rewards.Count / 2))
                this.AddButton(356, 102, 2224, 2224, (int)Buttons.btnNext, GumpButtonType.Reply, 0);

            if (m_PageNo > 0)
                this.AddButton(34, 102, 2223, 2223, (int)Buttons.btnPrev, GumpButtonType.Reply, 0);
        }

        public enum Buttons
        {
            btnCancel,
            btnPurchase1,
            btnNext,
            btnPrev,
            btnPurchase2,
        }

        public override void OnResponse(Network.NetState sender, RelayInfo info)
        {
            base.OnResponse(sender, info);

            if (info.ButtonID == 0)
                return;

            Mobile from = sender.Mobile;
            var town = Town.CheckCitizenship(from);
            Item purchased = null;

            switch (info.ButtonID)
            {
                case (int)Buttons.btnPurchase1:
                    {
                        var reward = Rewards.Skip(m_PageNo * 2).Take(1).First();
                        if (reward != null)
                        {
                            if (TryConsumeResources(from, reward.KeyCost))
                            {
                                var item = reward.Create(town);
                                if (item != null)
                                    from.AddToBackpack(item);
                                from.SendMessage("You have purchased {0}", reward.Name);
                            }
                        }
                        from.SendGump(new MilitiaVendorGump(m_PageNo));
                    } break;

                case (int)Buttons.btnPurchase2:
                    {
                        var reward = Rewards.Skip((m_PageNo * 2) + 1).Take(1).First();
                        if (reward != null)
                        {
                            if (TryConsumeResources(from, reward.KeyCost))
                            {
                                var item = reward.Create(town);
                                if (item != null)
                                    from.AddToBackpack(item);
                                from.SendMessage("You have purchased {0}", reward.Name);
                            }
                        }
                        from.SendGump(new MilitiaVendorGump(m_PageNo));
                    } break;

                case (int)Buttons.btnNext:
                    {
                        from.SendGump(new MilitiaVendorGump(++m_PageNo));
                    } break;

                case (int)Buttons.btnPrev:
                    {
                        if (m_PageNo == 0)
                            from.SendGump(new MilitiaVendorGump(m_PageNo));
                        else
                            from.SendGump(new MilitiaVendorGump(--m_PageNo));
                    } break;
            }
        }

        public bool TryConsumeResources(Mobile from, int keyWorth)
        {
            var ps = PlayerState.Find(from);
            var player = from as PlayerMobile;

            if (ps == null)
                from.SendMessage("You must be in a militia to purchase this item.");
            else if (player.TreasuryKeys <= keyWorth)
                from.SendMessage("You do not have enough treasury keys to purchase this item.");
            else
            {
                player.TreasuryKeys -= keyWorth;
                return true;
            }
           
            return false;
        }

    }
}
