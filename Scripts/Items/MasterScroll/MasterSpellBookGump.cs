using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Commands;
using Server.Items;

namespace Server.Gumps
{
    public class MasterSpellBookGump : Gump
    {
        private Mobile m_From;
        private MasterSpellBook m_Book;
        private PageType m_PageType;
        private int m_ScrollID;

        public enum PageType
        {
            Index,
            Overview,
            Details
        }


        public MasterSpellBookGump(Mobile from, MasterSpellBook book, PageType pageType = PageType.Index,int scrollID = 0): base(100, 150)
        {
            m_From = from;
            m_PageType = pageType;
            m_Book = book;
            m_ScrollID = scrollID;
            this.Closable = true;
			this.Disposable = true;
			this.Dragable = true;
            this.Resizable = false;

            AddPage(0);

            var scrolls = m_Book.Data;
            switch(m_PageType) 
            {
                case PageType.Overview: 
                    AddImage(0, 0, 1248); 
                    AddHtml(60, 60 , 200, 25, @"<center><h3><u><BASEFONT COLOR=#FFFFFF> Master Scroll Overview", false, false);
                    int count = 0;
                    for (int y = 0 + scrollID; y < scrolls.Count; y++)
                    {
                        if (count < 10)
                        {
                            AddHtml(35, 95 + ((y-scrollID) * 20), 240, 25, @"<center><BASEFONT COLOR=#480000>" + scrolls[y].Name + " - " + scrolls[y].Charges + " charges.", false, false);
                            count++;
                        }
                    }
                    AddButton(125, 320, 242, 243, 5, GumpButtonType.Reply, 0);
                    if (scrolls.Count > 10)
                    {
                        if (scrollID == 0)
                        {
                            AddButton(255, 370, 2085, 2085, 6, GumpButtonType.Reply, 0);
                        }
                        else
                        {
                            AddButton(255, 15, 2084, 2084, 7, GumpButtonType.Reply, 0);
                        }
                    } 
                    break;

                case PageType.Details:
                    
                    AddImage(0, 0, 11049);
                    AddButton(50, 9, 2235, 2235, 5, GumpButtonType.Reply, 0);
                    AddImage(87, 49, 7000 + m_ScrollID);
                    AddHtml(60, 125, 125, 25, @"<center><h3><u><BASEFONT COLOR=#FFFFFF>" + m_Book.getScrollName(m_ScrollID), false, false);
                    AddHtml(60, 150, 125, 25, @"<center><BASEFONT COLOR=#480000>Charges Left: " + m_Book.getMaster(scrollID).Charges, false, false);
                    AddHtml(220, 20, 125, 50, @"<center><h3><u><BASEFONT COLOR=#FFFFFF>Remove Master Scroll?", false, false);
                    AddLabel(237, 87, 2, @"Enter amount");
                    AddAlphaRegion(258, 125, 45, 20);
                    AddTextEntry(258, 125, 45, 20, 0, 0, "", 5);
                    AddButton(250, 170, 247, 249, 6, GumpButtonType.Reply, 0);
                    break;

                default:
                    AddImage(0, 0, 11049);
                    int leftItems = 0;
                    int rightItems = 0;
                    for (int i = 0; i < scrolls.Count; i++)
                    {
                        if (scrolls[i].ID <= 56)
                        {
                            AddButton(55, 33 + (leftItems * 20), 2118, 2117, scrolls[i].ID, GumpButtonType.Reply, 0);
                            AddLabel(73, 30 + (leftItems * 20), 241, ("" + scrolls[i].Name));
                            leftItems++;
                        }
                        else
                        {
                            AddButton(215, 33 + (rightItems * 20), 2118, 2117, scrolls[i].ID, GumpButtonType.Reply, 0);
                            AddLabel(233, 30 + (rightItems * 20), 241, ("" + scrolls[i].Name));
                            rightItems++;
                        }
                    }
                    break;

            }
        }

        

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            int ID = info.ButtonID;

            if (ID == 0)
                return;

            switch(m_PageType)
            {
                case PageType.Overview:
                    if(ID == 5) 
                    {
                        from.CloseGump(typeof(MasterSpellBookGump));
                    }
                    else if (ID >= 6)
                    {
                        from.SendGump(new MasterSpellBookGump(from,m_Book,PageType.Overview,(ID == 6) ? 10 : 0));
                    }
                    break;

                case PageType.Index:
                    from.SendGump(new MasterSpellBookGump(from, m_Book, PageType.Details, ID));
                    break;

                case PageType.Details:
                    if (ID == 5)
                    {
                        from.SendGump(new MasterSpellBookGump(from, m_Book));
                    }
                    else if (ID == 6)
                    {
                        TextRelay entry0 = info.GetTextEntry(0);
			            string text0 = (entry0 == null ? "0" : entry0.Text.Trim());
                        int amount = 0;
                        for (int i = 0; i < text0.Length; i++)
                        {
                            if (!char.IsDigit(text0[i]))
                            {
                                from.SendMessage("Number of charges is invalid.");
                                from.SendGump(new MasterSpellBookGump(from, m_Book));
                                return;
                            }
                        }
                        amount = Convert.ToInt16(text0);
                        m_Book.RemoveMaster(from, m_ScrollID, amount);
                        from.SendGump(new MasterSpellBookGump(from, m_Book));
                    }
                    break;

                default:
                    break;

            }
        }
    }
}