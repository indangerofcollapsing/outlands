using System;
using System.Collections.Generic;
using Server;
using Server.Custom.Townsystem;
using System.Collections;

namespace Server.Gumps
{
    public class OutcastGump : Gump
    {
        private Town m_Town;
        private List<OutcastEntry> m_List;
        private int m_Page;

        public OutcastGump(Town town)
            : this(town, 0)
        {
        }

        public OutcastGump(Town town, int page)
            : base(100, 100)
        {
            m_Town = town;
            m_Page = page;
            m_List = town.OutcastEntries;
            

            #region static content
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
            this.AddPage(0);
            this.AddImage(0, 0, 5400);
            this.AddImage(152, 31, 5399);
            this.AddLabel(110, 137, 0, @"Read");
            this.AddLabel(315, 137, 0, @"Vote");
            this.AddButton(1, 153, 5397, 5398, (int)Buttons.btnPostOutcast, GumpButtonType.Reply, 0);
            this.AddButton(378, 172, 5395, 5396, (int)Buttons.btnViewExiles, GumpButtonType.Reply, 0);

            if (m_Page > 0)
                this.AddButton(362, 114, 9770, 9772, (int)Buttons.btnScrollUp, GumpButtonType.Reply, 0);

            #endregion  

            Queue _queue = new Queue();
            for (int i = 0; i < m_List.Count; i++)
            {
                if (m_List[i].Outcast == null || m_List[i].Outcast.Deleted || m_List[i].Poster == null || m_List[i].Poster.Deleted || m_List[i].StartDate + TimeSpan.FromDays(14) < DateTime.Now)
                    _queue.Enqueue(m_List[i]);
            }

            while (_queue.Count > 0)
                m_List.Remove((OutcastEntry)_queue.Dequeue());
            m_List.Sort();


            int count = m_List.Count - m_Page*9;

            if (count <= 0)
                return;

            if (count > 9)
            {
                this.AddButton(362, 342, 9771, 9773, (int)Buttons.btnScrollDown, GumpButtonType.Reply, 0);
                count = 9;
            }

            for (int i = 0; i < count; i++)
            {
                this.AddButton(117, 157 + i * 20, 5411, 5411, 10 + i, GumpButtonType.Reply, 0);
                this.AddButton(315, 156 + i * 20, 55, 55, 20 + i, GumpButtonType.Reply, 0);
                this.AddButton(332, 156 + i * 20, 56, 56, 30 + i, GumpButtonType.Reply, 0);
                this.AddLabel(348, 155 + i * 20, 0, m_List[i + page * 9].Count.ToString());
                this.AddHtml(135, 158 + i * 20, 176, 18, String.Format("{0}. {1}: {2}", i + 1 + page * 9, m_List[i + page * 9].Outcast.RawName, m_List[i + page * 9].Title), (bool)false, (bool)false);
            }
        }

        public enum Buttons
        {
            btnCancel,
            btnPostOutcast,
            btnViewExiles,
            btnScrollUp,
            btnScrollDown,
        }

        public override void OnResponse(Network.NetState sender, RelayInfo info)
        {
            try 
            {
                base.OnResponse(sender, info);
                Mobile from = sender.Mobile;
    				bool gm_or_higher = from.AccessLevel >= AccessLevel.GameMaster;

                int id = info.ButtonID;

                if (id == (int)Buttons.btnPostOutcast)
                {
                    if (!gm_or_higher && Town.CheckCitizenship(from) != m_Town)
                        from.SendMessage(String.Format("You must be a citizen of {0} to post outcast entries!", m_Town.Definition.FriendlyName));
                    else
    					from.SendGump(new OutcastEntryGump(from, m_Town));
                }
                else if (id == (int)Buttons.btnViewExiles)
                {
                    if (m_Town.Exiles.Count > 0)
                        from.SendGump(new ExileListGump(m_Town, 0));
                    else
                        from.SendMessage("This town currently has no exiled players.");
                }
                else if (id == (int)Buttons.btnScrollUp)
                {
                    from.SendGump(new OutcastGump(m_Town, m_Page - 1));
                }
                else if (id == (int)Buttons.btnScrollDown)
                {
                    from.SendGump(new OutcastGump(m_Town, m_Page + 1));
                }
                else if (id >= 30) // vote down
                {
                    int ID = id - 30 + m_Page * 9;
                    if (ID >= m_List.Count || ID < 0)
                    {
                        from.SendMessage("That post has been removed.");
                    }
    				else if (!gm_or_higher && Town.CheckCitizenship(from) != m_Town)
                    {
                        from.SendMessage(String.Format("You must be a citizen of {0} to vote for outcasts!", m_Town.Definition.FriendlyName));
                    }
    				else if (!gm_or_higher && m_List[ID].Voters.Contains(from))
                    {
                        from.SendMessage("You have already voted for this outcast.");
                        from.SendGump(new OutcastGump(m_Town, m_Page));
                    }
                    else
                    {
                        m_List[ID].Voters.Add(from);
                        m_List[ID].Count--;
                        ReOrder(-1, ID);
                        from.SendMessage(String.Format("You have voted against the exile of {0}.", m_List[ID].Outcast.Name));
                        from.SendGump(new OutcastGump(m_Town, m_Page));
                    }

                }
                else if (id >= 20) //vote up
                {
                    int ID = id - 20 + m_Page * 9;

                    if (ID >= m_List.Count || ID < 0)
                    {
                        from.SendMessage("That post has been removed.");
                    }
    				else if (!gm_or_higher && Town.CheckCitizenship(from) != m_Town)
                    {
                        from.SendMessage(String.Format("You must be a citizen of {0} to vote for outcasts!", m_Town.Definition.FriendlyName));
                    }
    				else if (!gm_or_higher && m_List[ID].Voters.Contains(from))
                    {
                        from.SendMessage("You have already voted for this outcast.");
                        from.SendGump(new OutcastGump(m_Town, m_Page));
                    }
                    else
                    {
                        m_List[ID].Voters.Add(from);
                        m_List[ID].Count++;
                        ReOrder(1, ID);
                        from.SendMessage(String.Format("You have voted to exile {0}.", m_List[ID].Outcast.RawName));
                        from.SendGump(new OutcastGump(m_Town, m_Page));
                    }
                }
                else if (id >= 10)
                {
                    int ID = id - 10 + m_Page * 9;

                    if (ID >= m_List.Count || ID < 0)
                    {
                        from.SendMessage("That post has been removed.");
                    }
                    else
                    {
                        from.SendGump(new OutcastEntryGump(from, m_List[ID]));
                    }
                }
            }
            catch(Exception ex) {
                Console.WriteLine("Outcast gump error: {0}", ex.Message);
            }
        }

        private void ReOrder(int change, int id)
        {
            if (change > 0)
            {
                if (id > 0 && Math.Abs(m_List[id].Count) > Math.Abs(m_List[id - 1].Count))
                {
                    OutcastEntry b = m_List[id-1];
                    m_List[id - 1] = m_List[id];
                    m_List[id] = b;
                }
            }
            else
            {
                if (id < m_List.Count-1 && Math.Abs(m_List[id].Count) < Math.Abs(m_List[id + 1].Count))
                {
                    OutcastEntry b = m_List[id + 1];
                    m_List[id + 1] = m_List[id];
                    m_List[id] = b;
                }
            }
        }
    }

    public class OutcastEntryGump : Gump
    {
        private string m_Name;
        private string m_Title;
        private string m_Desc;
        private Mobile m_From;
        private Town m_Town;
        private OutcastEntry m_oe;
        
        public OutcastEntryGump(Mobile from, Town town)
            : this(from,from,town,"<Name>","<Post Title>","<Description>", true)
        {
        }

        public OutcastEntryGump(Mobile from, OutcastEntry oe)
            : this(from, oe.Poster, oe.Town, oe.Outcast.Name, oe.Title, oe.Description, false)
        {
            m_oe = oe;
        }

        public OutcastEntryGump(Mobile from, Mobile poster, Town town, string name, string title, string desc, bool edit)
            : base(50, 50)
        {
            m_Name = name;
            m_Title = title;
            m_Town = town;
            m_From = from;
            m_Desc = desc;
            
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
            this.AddPage(0);
            this.AddImage(214, 154, 5175);
            this.AddImage(7, 154, 5173);
            this.AddImage(7, 4, 5170);
            this.AddImage(45, 4, 5171);
            this.AddImage(214, 4, 5172);
            this.AddImageTiled(45, 42, 173, 333, 5174);
            this.AddImage(7, 42, 5173);
            this.AddImage(7, 260, 5173);
            this.AddImage(214, 42, 5175);
            this.AddImage(214, 260, 5175);
            this.AddImage(7, 372, 5176);
            this.AddImage(44, 372, 5177);
            this.AddImage(214, 372, 5178);
            this.AddLabel(32, 30, 0, @"I wish to Exile");
            this.AddLabel(32, 70, 0, @"From the town of");
            this.AddLabel(60, 90, 5, town.Definition.FriendlyName);
            this.AddLabel(32, 110, 0, @"For the crime of");
            this.AddImageTiled(110, 386, 35, 63, 9004);
            this.AddImage(58, 159, 50);
            this.AddLabel(30, 174, 0, String.Format("Citizens of {0},", town.Definition.FriendlyName));
            this.AddLabel(113, 346, 0, @"Signed,");
            this.AddLabel(127, 363, 0, poster.RawName);

            if (edit)
            {
                this.AddTextEntry(60, 50, 160, 17, 5, (int)Buttons.txtOutcast, name);
                this.AddTextEntry(60, 130, 158, 16, 5, (int)Buttons.txtTitle, title);
                this.AddTextEntry(40, 200, 178, 145, 5, (int)Buttons.txtDesc, desc);
                this.AddButton(203, 398, 2182, 2182, (int)Buttons.btnPost, GumpButtonType.Reply, 0);
            }
            else
            {
                this.AddLabel(60, 50, 5, name);
                this.AddLabel(60, 130, 5, title);
                this.AddHtml(40, 200, 178, 145, String.Format("<BASEFONT COLOR=BLUE>{0}", desc),false,true);

                if (from.AccessLevel >= AccessLevel.GameMaster || from == town.King || from == poster)
                    this.AddButton(37, 358, 5531, 5532, (int)Buttons.btnDeletePost, GumpButtonType.Reply, 0);
            }
        }

        public enum Buttons
        {
            btnCancel,
            txtTitle,
            txtOutcast,
            btnPost,
            txtDesc,
            btnDeletePost
        }

        public override void OnResponse(Network.NetState sender, RelayInfo info)
        {
            base.OnResponse(sender, info);

            Mobile from = sender.Mobile;

            if (info.ButtonID == (int)Buttons.btnPost)
            {
                foreach (TextRelay text in info.TextEntries)
                {
                    if (text.EntryID == (int)Buttons.txtOutcast)
                        m_Name = text.Text;
                    else if (text.EntryID == (int)Buttons.txtTitle)
                        m_Title = text.Text;
                    else if (text.EntryID == (int)Buttons.txtDesc)
                        m_Desc = text.Text;
                }
                
                Mobile outcast = Outcasts.FromName(m_Name);

                if (outcast == null)
                {
                    from.SendMessage(String.Format("The system is unable to find a player by the name of {0}.", m_Name));
                    from.SendGump(new OutcastEntryGump(from, m_From, m_Town, m_Name, m_Title, m_Desc, true));
                    return;
                }
                else if (outcast.AccessLevel > AccessLevel.Player)
                {
                    from.SendMessage("You cannot post outcast entries about staff members.");
                    return;
                }
                
                foreach (OutcastEntry oe in m_Town.OutcastEntries)
                {
                    if (oe.Outcast == outcast)
                    {
                        from.SendMessage("That player is already posted on the outcast board.");
                        return;
                    }
                }

                if (m_Desc == "<Description>" || m_Title == "<Post Title>")
                {
                    from.SendMessage("Please make sure all fields are filled out.");
                    from.SendGump(new OutcastEntryGump(from, m_From, m_Town, m_Name, m_Title, m_Desc, true));
                    return;
                }

                OutcastEntry newEntry = new OutcastEntry(from, outcast, m_Town, m_Title, m_Desc);
                m_Town.OutcastEntries.Add(newEntry);
                from.SendGump(new OutcastGump(m_Town));
            }
            else if (info.ButtonID == (int)Buttons.btnDeletePost && m_oe != null && m_Town.OutcastEntries.Contains(m_oe))
            {
                m_Town.OutcastEntries.Remove(m_oe);
                from.SendMessage("This post has been removed.");
                from.SendGump(new OutcastGump(m_Town));
            }
        }
    }

    public class ExileListGump : Gump
    {
        private int m_Page;
        private Town m_Town;
        
        public ExileListGump(Town town, int page)
            : base(50, 50)
        {
            m_Page = page;
            m_Town = town;

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
            this.AddPage(0);
            this.AddImage(7, 4, 5170);
            this.AddImage(45, 4, 5171);
            this.AddImage(214, 4, 5172);
            this.AddImageTiled(45, 42, 173, 228, 5174);
            this.AddImage(7, 42, 5173);
            this.AddImage(7, 149, 5173);
            this.AddImage(214, 42, 5175);
            this.AddImage(214, 149, 5175);
            this.AddImage(7, 261, 5176);
            this.AddImage(44, 261, 5177);
            this.AddImage(214, 261, 5178);
            this.AddImageTiled(110, 275, 35, 63, 9004);

            int rc = 0;
            while (rc < town.Exiles.Count)
            {
                if (town.Exiles[rc] == null)
                    town.Exiles.RemoveAt(rc);
                else
                    rc++;
            }
            
            List<Mobile> list = town.Exiles;

            int count = list.Count - page * 11;
            if (count > 11)
            {
                this.AddButton(40, 262, 5606, 5602, (int)Buttons.Down, GumpButtonType.Reply, 0);
                count = 11;
            }

            if (page > 0)
                this.AddButton(40, 40, 5604, 5604, (int)Buttons.Up, GumpButtonType.Reply, 0);

            for (int i = 0; i < count; i++)
            {
              int index = i + page * 11;
              if (list.Count > index)
                this.AddLabel(63, 39 + 20 * i, 0, String.Format("{0}. {1}", index + 1, list[index].RawName));
            }
        }

        public enum Buttons
        {
            Cancel,
            Up,
            Down,
        }

        public override void OnResponse(Network.NetState sender, RelayInfo info)
        {
            base.OnResponse(sender, info);
        
            int change = info.ButtonID == 0 ? 0 : info.ButtonID*2 - 3;

            if (change != 0)
                sender.Mobile.SendGump(new ExileListGump(m_Town, m_Page + change));
        
        }
    }
}