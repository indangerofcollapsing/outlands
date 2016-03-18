/***************************************************************************
 *                          PaladinVendorGump.cs
 *                            ------------------
 *   begin                : August 2010
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/
using System;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Multis;
using Server.Custom.Townsystem;
using Scripts.Custom;
using Server.Mobiles;

namespace Server.Custom.Paladin
{
    public class DousingGuildVendorGump : Gump
    {
        public int m_PageNo = 0;
        public int m_GoldItem1 = 1000;
        public int m_CreditsItem1 = 1;

        public int m_GoldItem2 = 1000;
        public int m_CreditsItem2 = 1;


        public DousingGuildVendorGump(int PageNo)
            : base(0, 0)
        {
            m_PageNo = PageNo;
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
            this.AddPage(0);
            this.AddImage(0, 0, 11010);

            string htmlName1 = @"";
            int itemID1 = 0x1;

            string htmlName2 = @"";
            int itemID2 = 0x1;

            int item1Hue = 0;
            int item2Hue = 0;

            switch (PageNo)
            {
                case 1:
                    {
                        htmlName1 = @"<center>Fire Elemental Statue</center>";
                        m_GoldItem1 = 50000;
                        m_CreditsItem1 = 100;
                        itemID1 = 8435;

                        htmlName2 = @"<center>(Title, Suffix) Champion of Fire</center>";
                        m_GoldItem2 = 1;
                        m_CreditsItem2 = 300;
                        itemID2 = 1;
                    } break;
            }

            if (PageNo < 1)
                this.AddButton(356, 102, 2224, 2224, (int)Buttons.btnNext, GumpButtonType.Reply, 0);

            if (m_PageNo > 1)
                this.AddButton(34, 102, 2223, 2223, (int)Buttons.btnPrev, GumpButtonType.Reply, 0);

            
            this.AddButton(78, 170, 1154, 1153, (int)Buttons.btnPurchase1, GumpButtonType.Reply, 0);
            this.AddHtml(52, 12, 145, 44, htmlName1, (bool)false, (bool)false);
            this.AddLabel(111, 171, 0x0, @"Purchase");
            if (m_GoldItem1 > 0)
            {
                this.AddItem(58, 102, 3823);
                this.AddLabel(102, 106, 0x0, String.Format("{0}", m_GoldItem1)); //GOLD
            }
            this.AddItem(100, 61, itemID1, item1Hue); //ITEMID OF PAGE ITEM

            if (m_CreditsItem1 > 0)
                this.AddLabel(60, 131, 0x0, String.Format("Dousing Credits: {0}", m_CreditsItem1));

            if (htmlName2.Length > 0)
            {
                this.AddButton(234, 170, 1154, 1153, (int)Buttons.btnPurchase2, GumpButtonType.Reply, 0);
                this.AddHtml(208, 12, 145, 44, htmlName2, (bool)false, (bool)false);
                if (m_GoldItem2 > 0)
                {
                    this.AddItem(214, 102, 3823); //GOLD IMAGE
                    this.AddLabel(258, 106, 0x0, String.Format("{0}", m_GoldItem2)); //GOLD
                }

                if (m_CreditsItem2 > 0)
                    this.AddLabel(216, 129, 0x0, String.Format("Dousing Credits: {0}", m_CreditsItem2));
                
                this.AddLabel(267, 171, 0x0, @"Purchase");
                this.AddItem(256, 61, itemID2, item2Hue);
            }
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
            var ps = FireDungeonPlayerState.Find(from as PlayerMobile);
            var town = Town.CheckCitizenship(from);
            Item purchased = null;

            switch (info.ButtonID)
            {
                case (int)Buttons.btnPurchase1:
                    {
                        if (TryConsumeResources(from, m_GoldItem1, m_CreditsItem1))
                        {
                            switch (m_PageNo)
                            {
                                case 1:
                                    {
                                        var state = FireDungeonPlayerState.FindOrCreate(from as PlayerMobile);
                                        if (state != null)
                                        {
                                            state.PurchasedFireElementalStatue = true;
                                            purchased = new MonsterStatuette(MonsterStatuetteType.FireElemental);
                                        }

                                        break;
                                    }
                                
                                
                            }

                            if (purchased != null)
                                from.Backpack.DropItem(purchased);

                            from.SendGump(new DousingGuildVendorGump(m_PageNo));
                        }
                    } break;

                case (int)Buttons.btnPurchase2:
                    {
                        if (TryConsumeResources(from, m_GoldItem2, m_CreditsItem2))
                        {
                            switch (m_PageNo)
                            {
                                case 1:
                                    {
                                        var pm = from as PlayerMobile;
                                        if (pm != null)
                                            pm.AddPrefixTitle = "Champion of Fire";

                                        break; 
                                    }
                            }
                        }

                        if (purchased != null)
                            from.Backpack.DropItem(purchased);

                        from.SendGump(new DousingGuildVendorGump(m_PageNo));
                    } break;

                case (int)Buttons.btnNext:
                    {
                        from.SendGump(new DousingGuildVendorGump(++m_PageNo));
                    } break;

                case (int)Buttons.btnPrev:
                    {
                        if (m_PageNo == 1)
                            from.SendGump(new DousingGuildVendorGump(m_PageNo));
                        else
                            from.SendGump(new DousingGuildVendorGump(--m_PageNo));
                    } break;
            }
        }

        public bool TryConsumeResources(Mobile from, int goldAmt, int vendorCredits)
        {
            if (from == null)
                return false;

            Container pack = from.Backpack;
            if (pack == null)
                return false;

            Container gold_container = pack.GetTotal(TotalType.Gold) >= goldAmt ? pack : from.BankBox;
            bool enough_gold = gold_container.GetTotal(TotalType.Gold) >= goldAmt;
            if (enough_gold)
            {
                // try consume valorite/monster coins from bp/bank, if success burn the gold and we're done.

                if (FireDungeonPlayerState.GetDousingPoints(from as PlayerMobile) >= vendorCredits)
                {
                    return gold_container.ConsumeTotal(typeof(Gold), goldAmt) && FireDungeonPlayerState.ConsumeFirePoints(from as PlayerMobile, vendorCredits);
                }
                else
                {
                    from.SendMessage("You do not have enough dousing credits to purchase this item.");
                }
            }
            else
            {
                from.SendMessage("You do not have enough gold to purchase that");
            }
            return false;
        }

    }
}
