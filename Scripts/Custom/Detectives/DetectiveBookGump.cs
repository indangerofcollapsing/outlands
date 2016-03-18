/***************************************************************************
 *                           DetectivesBookGump.cs
 *                            ------------------
 *   begin                : February 2010
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/
using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Commands;
using Server.Items;
using Server.Custom;

namespace Server.Gumps
{
    public class DetectiveBookGump : Gump
    {
        private static readonly int m_EntriesPerPage = 8;

        public Mobile m_From;
        public ClueBook m_Book;
        public PageType m_PageType;
        public int m_Page;

        public enum PageType
        {
            Index,
            Detailed
        }

        public DetectiveBookGump(Mobile from, ClueBook book, PageType pageType = PageType.Index, int page = 0)
            : base(100, 50)
        {
            m_From = from;
            m_Book = book;
            m_Page = page;
            m_PageType = pageType;

            #region static items
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            AddPage(0);
            AddImage(0, 0, 11058);
            #endregion

            var data = m_Book.Data;
            var entries = data.Entries;

            if (m_PageType == PageType.Index)
            {
                AddHtml(53, 8, 146, 26, @"<center><h3><BASEFONT COLOR=#480000>CLUE", false, false);
                AddHtml(210, 8, 146, 26, @"<center><h3><BASEFONT COLOR=#480000>BOOK", false, false);

                int start = m_Page * m_EntriesPerPage * 2;
                int count = Math.Max(0, entries.Count - start);

                if (m_Page > 0)
                    AddButton(51, 8, 2235, 2235, 1, GumpButtonType.Reply, 0);
                if (count > m_EntriesPerPage * 2)
                    AddButton(321, 8, 2236, 2236, 2, GumpButtonType.Reply, 0);

                int firstPageCount = Math.Min(m_EntriesPerPage, count);

                int index;

                for (int i = 0; i < firstPageCount; i++)
                {
                    index = start + i;

                    if (index >= entries.Count) 
                        break;

                    var entry = entries[index];
                    
                    if (entry.Killer == null) continue;

                    AddButton(55, 37 + i * 20, 2118, 2117, 10 + i, GumpButtonType.Reply, 0);
                    AddHtml(71, 36 + i * 20, 128, 16, entry.Killer.RawName, false, false);
                }

                int secondPageCount = Math.Min(m_EntriesPerPage, Math.Max(0, count - m_EntriesPerPage));

                for (int i = 0; i < secondPageCount; i++)
                {
                    index = start + i + m_EntriesPerPage;

                    if (index >= entries.Count) 
                        break;

                    var entry = entries[index];

                    if (entry.Killer == null) continue;

                    AddButton(210, 37 + i * 20, 2118, 2117, 10 + m_EntriesPerPage + i, GumpButtonType.Reply, 0);
                    AddHtml(226, 36 + i * 20, 128, 16, entry.Killer.RawName, false, false);
                }
            }
            else if (m_PageType == PageType.Detailed)
            {
                AddButton(51, 8, 2235, 2235, 5, GumpButtonType.Reply, 0);

                if (entries.Count <= m_Page)
                    return;

                var entry = entries[m_Page];

                Mobile killer = entry.Killer;

                if (killer == null)
                    return;

                int footprints, weapon, blood;
                entry.GetEvidence(out footprints, out weapon, out blood);

                AddHtml(90, 15, 257, 17, String.Format("<h3>{0}</h3>", killer.RawName), (bool)false, (bool)false);

                AddButton(54, 43, 1210, 1209, 1, GumpButtonType.Reply, 0);
                AddLabel(75, 40, 0, @"Footprints");
                AddLabel(90, 60, 681, footprints.ToString());

                AddButton(56, 83, 1210, 1209, 2, GumpButtonType.Reply, 0);
                AddLabel(75, 80, 0, @"Murder Weapon");
                AddLabel(90, 100, 681, weapon.ToString());

                AddButton(56, 123, 1210, 1209, 3, GumpButtonType.Reply, 0);
                AddLabel(75, 120, 0, @"Blood");
                AddLabel(90, 140, 681, blood.ToString());

                AddButton(259, 101, 4502, 4502, 4, GumpButtonType.Reply, 0);
                AddHtml(210, 87, 139, 22, @"<center>Create Wanted Note", (bool)false, (bool)false);
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (info.ButtonID == 0)
                return;

            int ID = info.ButtonID;

            if (m_PageType == PageType.Detailed)
            {
                if (ID == 1)
                    m_Book.Extract(from, m_Page, Clue.ClueType.Footsteps);
                else if (ID == 2)
                    m_Book.Extract(from, m_Page, Clue.ClueType.MurderWeapon);
                else if (ID == 3)
                    m_Book.Extract(from, m_Page, Clue.ClueType.Blood);
                else if (ID == 4)
                    m_Book.CreateWantedNote(from, m_Page);

                if (ID == 5)
                    from.SendGump(new DetectiveBookGump(from, m_Book, PageType.Index, 0));
                else
                    from.SendGump(new DetectiveBookGump(from, m_Book, PageType.Detailed, m_Page));
            }
            else if (m_PageType == PageType.Index)
            {
                if (ID == 1) // page up
                    from.SendGump(new DetectiveBookGump(from, m_Book, PageType.Index, Math.Max(0, --m_Page)));
                else if (ID == 2) //page down
                    from.SendGump(new DetectiveBookGump(from, m_Book, PageType.Index, Math.Max(0, ++m_Page)));
                else
                {
                    int index = Math.Max(0, ID - 10) + m_Page * m_EntriesPerPage * 2;
                    from.SendGump(new DetectiveBookGump(from, m_Book, PageType.Detailed, index));
                }

            }
        }
    }
}