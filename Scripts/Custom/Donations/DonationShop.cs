using Server.Gumps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Donations
{
    public class DonationShop : Gump
    {
        private int m_Page;
        private int m_Category;

        public enum Buttons
        {
            pageDown = 99,
            pageUp = 98,
            Donate = 100,
        }

        public DonationShop(int category = 0, int page = 0)
            : base(0, 50)
        {
            if (page < 0)
                return;

            m_Category = category;
            m_Page = page;

            #region static content
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
            this.AddPage(0);
            this.AddBackground(3, 0, 774, 432, 9200);
            this.AddHtml(7, 17, 764, 30, @"<center><h3>Donation Shop : http://uoancorp.com</h3></center>", (bool)true, (bool)false);
            this.AddImage(8, 64, 3003);
            this.AddImageTiled(11, 64, 212, 346, 3004);
            this.AddImage(223, 65, 3005);
            this.AddImage(223, 163, 3005);
            this.AddImage(8, 166, 3003);
            this.AddImageTiled(242, 64, 525, 346, 3004);
            this.AddImage(767, 65, 3005);
            this.AddImage(767, 163, 3005);
            this.AddImage(239, 64, 3003);
            this.AddImage(239, 166, 3003);
            this.AddHtml(53, 67, 88, 25, @"Categories", (bool)false, (bool)false);

            #endregion


            #region Categories

            int count = 0;
            foreach (string catName in Donations.DragonTable.Keys)
            {
                this.AddButton(15, 100 + 20 * count, 4005, 4007, 1 + count, GumpButtonType.Reply, 0);
                this.AddLabel(50, 100 + 20 * count++, 0, catName);
            }

            this.AddButton(15, 100 + 20 * count, 4005, 4007, (int)Buttons.Donate, GumpButtonType.Reply, 0);
            this.AddLabel(50, 100 + 20 * count++, 0, "Donate Now");


            #endregion

            #region Items

            if (Donations.DragonTable.Count <= category)
                return;

            var Items = Donations.DragonTable[category] as List<DonationItem>;

            if (Items == null)
                return;

            int itemCount = Items.Count - m_Page * 3;

            if (itemCount > 3)
                this.AddButton(749, 383, 252, 253, (int)Buttons.pageDown, GumpButtonType.Reply, 0);

            if (m_Page > 0)
                this.AddButton(747, 67, 250, 251, (int)Buttons.pageUp, GumpButtonType.Reply, 0);

            if (itemCount > 3)
                itemCount = 3;

            for (int i = 0; i < itemCount; i++)
            {
                int index = i + m_Page * 3;

                if (Items.Count <= index)
                    return;

                DonationItem di = Items[index];

                this.AddLabel(260, 69 + 110 * i, 0, di.Name);

                this.AddImage(256, 90 + 110 * i, 2328);
                this.AddItem(275 + di.XOffset, 98 + 110 * i + di.YOffset, di.ItemID, di.ItemHue);

                this.AddLabel(340, 96 + 110 * i, 238, di.Limited ? "Limited" : "");
                this.AddLabel(340, 122 + 110 * i, 0, String.Format("Cost: {0}", di.Cost));

                this.AddHtml(429, 85 + 110 * i, 253, 67, di.Description, false, false);

                this.AddLabel(698, 115 + 110 * i, 0, @"Purchase");
                this.AddButton(712, 133 + 110 * i, 1154, 1155, 50 + i, GumpButtonType.Reply, 0);

                if (i < 2)
                    this.AddImageTiled(308, 172 + 110 * i, 407, 4, 9351);
            }

            #endregion
        }

        public override void OnResponse(Network.NetState sender, RelayInfo info)
        {
            Mobile m = sender.Mobile;
            int buttonID = info.ButtonID;

            if (buttonID == 0)
                return;
            if (buttonID == (int)Buttons.Donate)
                m.LaunchBrowser("http://uoancorp.com/index.php/support/donate");
            else if (buttonID == (int)Buttons.pageUp)
                m.SendGump(new DonationShop(m_Category, --m_Page));
            else if (buttonID == (int)Buttons.pageDown)
                m.SendGump(new DonationShop(m_Category, ++m_Page));
            else if (buttonID < 50) //category
            {
                int category = buttonID - 1;
                m.SendGump(new DonationShop(category, 0));
            }
            else //purchase
            {

                if (m_Category >= Donations.DragonTable.Count)
                {
                    m.SendMessage("That category is no longer available in the donation shop.");
                    m.SendGump(new DonationShop());
                    return;
                }

                var itemList = Donations.DragonTable[m_Category] as List<DonationItem>;

                if (itemList == null)
                    return;

                int buyIndex = buttonID - 50 + m_Page * 3;

                if (itemList.Count <= buyIndex)
                    return;

                DonationItem di = itemList[buyIndex];

                if (di == null)
                    return;

                Donations.PurchaseItem(m, di, this);
            }
        }
    }
}
