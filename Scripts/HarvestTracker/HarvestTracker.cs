/***************************************************************************
 *                            SellingPersistance.cs
 *                          ------------------------
 *   begin                : August 1, 2010
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/
using System;
using Server.Commands;
using System.Collections;
using System.Collections.Generic;
using Server.Network;
using Server.Gumps;
using Server.Mobiles;
using System.IO;

namespace Server.Custom
{

    public enum HarvestType { Lumberjacking, Fishing, Mining };
    public static class HarvestTracker
    {
        #region static

        #region commands
        public static void Initialize()
        {
            CommandSystem.Register("HarvestTracker", AccessLevel.Counselor, new CommandEventHandler(HarvestTracker_OnCommand));
            CommandSystem.Register("WriteHarvestLog", AccessLevel.GameMaster, new CommandEventHandler(WriteHarvestLog_OnCommand));

            CommandSystem.Register("RandomHarvester", AccessLevel.Counselor, new CommandEventHandler(RandomHarvester_OnCommand));
            CommandSystem.Register("RandomFishing", AccessLevel.Counselor, new CommandEventHandler(RandomFishing_OnCommand));
            CommandSystem.Register("RandomLumberjacking", AccessLevel.Counselor, new CommandEventHandler(RandomLumberjacking_OnCommand));
            CommandSystem.Register("RandomMining", AccessLevel.Counselor, new CommandEventHandler(RandomMining_OnCommand));

            if (!Directory.Exists("HarvestLogs"))
            Directory.CreateDirectory("HarvestLogs");

            new HarvestTrackerPersistance();       
        }

        [Usage("HarvestTracker")]
        [Description("HarvestTracker")]
        public static void HarvestTracker_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            from.SendGump(new HarvestTrackerGump(1));
        }
        [Usage("WriteHarvestLog")]
        [Description("WriteHarvestLog")]
        public static void WriteHarvestLog_OnCommand(CommandEventArgs e)
        {
             WriteTopFile();
        }

        [Usage("RandomHarvester")]
        [Description("Jumps Invisibly to a Random Character Harvesting")]
        public static void RandomHarvester_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            List<Mobile> m_Harvesters = new List<Mobile>();

            foreach (KeyValuePair<Mobile, Int32> entry in m_TableOre)
            {
                if (entry.Key != null)
                {
                    if (!entry.Key.Deleted && entry.Key.NetState != null)
                        m_Harvesters.Add(entry.Key);  
                }
            }

            foreach (KeyValuePair<Mobile, Int32> entry in m_TableFish)
            {
                if (entry.Key != null)
                {
                    if (!entry.Key.Deleted && entry.Key.NetState != null)
                        m_Harvesters.Add(entry.Key);  
                }
            }

            foreach (KeyValuePair<Mobile, Int32> entry in m_TableLumber)
            {
                if (entry.Key != null)
                {
                    if (!entry.Key.Deleted && entry.Key.NetState != null)
                        m_Harvesters.Add(entry.Key);  
                }
            }

            if (m_Harvesters.Count <= 0)
            {
                from.SendMessage("No players harvesting found.");
                return;
            }

            Mobile mobileToView = m_Harvesters[Utility.RandomMinMax(0, m_Harvesters.Count - 1)];

            if (mobileToView != null)
            {
                if (!mobileToView.Deleted)
                {
                    from.Hidden = true;
                    from.Location = mobileToView.Location;
                    from.Map = mobileToView.Map;
                }
            }           
        }

        [Usage("RandomFishing")]
        [Description("Jumps Invisibly to a Random Character Fishing")]
        public static void RandomFishing_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            List<Mobile> m_Harvesters = new List<Mobile>();

            foreach (KeyValuePair<Mobile, Int32> entry in m_TableFish)
            {
                if (entry.Key != null)
                {
                    if (!entry.Key.Deleted && entry.Key.NetState != null)
                        m_Harvesters.Add(entry.Key);
                }
            }

            if (m_Harvesters.Count <= 0)
            {
                from.SendMessage("No players fishing found.");
                return;
            }

            Mobile mobileToView = m_Harvesters[Utility.RandomMinMax(0, m_Harvesters.Count - 1)];

            if (mobileToView != null)
            {
                if (!mobileToView.Deleted)
                {
                    from.Hidden = true;
                    from.Location = mobileToView.Location;
                    from.Map = mobileToView.Map;
                }
            }
        }

        [Usage("RandomLumberjacking")]
        [Description("Jumps Invisibly to a Random Character Lumberjacking")]
        public static void RandomLumberjacking_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            List<Mobile> m_Harvesters = new List<Mobile>();

            foreach (KeyValuePair<Mobile, Int32> entry in m_TableLumber)
            {
                if (entry.Key != null)
                {
                    if (!entry.Key.Deleted && entry.Key.NetState != null)
                        m_Harvesters.Add(entry.Key);
                }
            }

            if (m_Harvesters.Count <= 0)
            {
                from.SendMessage("No players lumberjacking found.");
                return;
            }

            Mobile mobileToView = m_Harvesters[Utility.RandomMinMax(0, m_Harvesters.Count - 1)];

            if (mobileToView != null)
            {
                if (!mobileToView.Deleted)
                {
                    from.Hidden = true;
                    from.Location = mobileToView.Location;
                    from.Map = mobileToView.Map;
                }
            }
        }

        [Usage("RandomMining")]
        [Description("Jumps Invisibly to a Random Character Mining")]
        public static void RandomMining_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            List<Mobile> m_Harvesters = new List<Mobile>();

            foreach (KeyValuePair<Mobile, Int32> entry in m_TableOre)
            {
                if (entry.Key != null)
                {
                    if (!entry.Key.Deleted && entry.Key.NetState != null)
                        m_Harvesters.Add(entry.Key);
                }
            }

            if (m_Harvesters.Count <= 0)
            {
                from.SendMessage("No players mining found.");
                return;
            }

            Mobile mobileToView = m_Harvesters[Utility.RandomMinMax(0, m_Harvesters.Count - 1)];

            if (mobileToView != null)
            {
                if (!mobileToView.Deleted)
                {
                    from.Hidden = true;
                    from.Location = mobileToView.Location;
                    from.Map = mobileToView.Map;
                }
            }
        }

        #endregion
                                
        #region variables
        private static DateTime m_DateLastReset;
        private static HarvestTrackerPersistance m_Instance;
        public static Dictionary<Mobile,int> m_TableOre = new Dictionary<Mobile,int>();
        public static Dictionary<Mobile, int> m_TableFish = new Dictionary<Mobile, int>();
        public static Dictionary<Mobile, int> m_TableLumber = new Dictionary<Mobile, int>();
        public static List<Mobile> m_WatchList = new List<Mobile>();
        public static DateTime m_DateLastWroteLog;
        #endregion

        public static void NotifyStaff(Mobile from, HarvestType type)
        {
            string htype = "";
            switch (type)
            {
                case HarvestType.Fishing: { htype = "fishing"; } break;
                case HarvestType.Lumberjacking: { htype = "lumberjacking"; } break;
                case HarvestType.Mining: { htype = "mining"; } break;
            }
            CommandHandlers.BroadcastMessage(AccessLevel.GameMaster,0x482,String.Format("Watchlist player \"{0}\" is currently {1}.",from.Name,htype));
        }

        public static void PlayerHarvest(Mobile from, HarvestType type, Int32 amount)
        {
            if (m_WatchList.Contains(from))
                NotifyStaff(from, type);

			if (m_DateLastWroteLog.Date < DateTime.UtcNow.Date)
            {
                WriteTopFile();
                return;
            }
            
            if (type == HarvestType.Fishing)
            {
                if (m_TableFish.ContainsKey(from))
                    m_TableFish[from] += amount;
                else
                    m_TableFish.Add(from, amount);
            }
            else if (type == HarvestType.Lumberjacking)
            {
                if (m_TableLumber.ContainsKey(from))
                    m_TableLumber[from] += amount;
                else
                    m_TableLumber.Add(from, amount);
            }
            else if (type == HarvestType.Mining)
            {
                if (m_TableOre.ContainsKey(from))
                    m_TableOre[from] += amount;
                else
                    m_TableOre.Add(from, amount);

            }

        }

        public static List<KeyValuePair<Mobile, Int32>> ReturnTop10(HarvestType type)
        {
            switch (type)
            {
                case HarvestType.Fishing:
                    {
                        List<KeyValuePair<Mobile, Int32>> m_List = new List<KeyValuePair<Mobile, Int32>>();
                        foreach (KeyValuePair<Mobile, Int32> entry in m_TableFish)
                        {
                            m_List.Add(entry);
                        }
                        m_List.Sort(delegate(KeyValuePair<Mobile, Int32> x, KeyValuePair<Mobile, Int32> y) { return y.Value.CompareTo(x.Value); });
                        int i = 10;
                        if (m_List.Count < 10)
                            i = m_List.Count;
                        if (m_List.Count > 0)
                            return m_List.GetRange(0, i);
                        else
                            return null;
                    }
                case HarvestType.Lumberjacking:
                    {
                        List<KeyValuePair<Mobile, Int32>> m_List = new List<KeyValuePair<Mobile, Int32>>();
                        foreach (KeyValuePair<Mobile, Int32> entry in m_TableLumber)
                        {
                            m_List.Add(entry);
                        }
                        m_List.Sort(delegate(KeyValuePair<Mobile, Int32> x, KeyValuePair<Mobile, Int32> y) { return y.Value.CompareTo(x.Value); });
                        int i = 10;
                        if (m_List.Count < 10)
                            i = m_List.Count;
                        if (m_List.Count > 0)
                            return m_List.GetRange(0, i);
                        else
                            return null;
                    }
                case HarvestType.Mining:
                    {
                        List<KeyValuePair<Mobile, Int32>> m_List = new List<KeyValuePair<Mobile, Int32>>();
                        foreach (KeyValuePair<Mobile,Int32> entry in m_TableOre)
                        {
                            m_List.Add(entry);
                        }
                        m_List.Sort(delegate(KeyValuePair<Mobile, Int32> x, KeyValuePair<Mobile, Int32> y) { return y.Value.CompareTo(x.Value); });
                        int i = 10;
                        if (m_List.Count < 10)
                            i = m_List.Count;
                        if (m_List.Count > 0)
                            return m_List.GetRange(0, i);
                        else
                            return null;
                    }
            }

            return null;
        }

        private static void Reset()
        {
            m_TableOre.Clear();
            m_TableFish.Clear();
            m_TableLumber.Clear();
			m_DateLastReset = DateTime.UtcNow;
        }

        private static void WriteTopFile()
        {
            try
            {

				StreamWriter sw = new StreamWriter(String.Format("HarvestLogs\\{0}.log", DateTime.UtcNow.ToLongDateString()), true);

                List<KeyValuePair<Mobile, Int32>> fishlist = ReturnTop10(HarvestType.Fishing);
                List<KeyValuePair<Mobile, Int32>> minelist = ReturnTop10(HarvestType.Mining);
                List<KeyValuePair<Mobile, Int32>> loglist = ReturnTop10(HarvestType.Lumberjacking);

                if (fishlist != null)
                {
                    sw.WriteLine("*************Fishing*************");
                    foreach (KeyValuePair<Mobile, Int32> entry in fishlist)
                    {
                        sw.WriteLine(String.Format("({0}){1}: {2}", entry.Key.Account.Username, entry.Key.Name, entry.Value));
                    }
                }
                if (minelist != null)
                {
                    sw.WriteLine("**************Mining**************");
                    foreach (KeyValuePair<Mobile, Int32> entry in minelist)
                    {
                        sw.WriteLine(String.Format("({0}){1}: {2}", entry.Key.Account.Username, entry.Key.Name, entry.Value));
                    }
                }
                if (loglist != null)
                {
                    sw.WriteLine("**********Lumberjacking**********");
                    foreach (KeyValuePair<Mobile, Int32> entry in loglist)
                    {
                        sw.WriteLine(String.Format("({0}){1}: {2}", entry.Key.Account.Username, entry.Key.Name, entry.Value));
                    }
                }
                sw.Close();
				m_DateLastWroteLog = DateTime.UtcNow;
                Reset();
            }
            catch (Exception ex)
            {
                CommandHandlers.BroadcastMessage(AccessLevel.GameMaster, 0x482, String.Format("Error writing logfile:{0}", ex));
            }
        }

        private static string GetAccountFromPlayer(Mobile from)
        {
            return from.Account.Username;
        }
        #endregion

        public class HarvestTrackerGump : Gump
        {
            private Mobile[] m_fishing;
            private Mobile[] m_mining;
            private Mobile[] m_lumber;
            private int m_PageNo;
            private static int noWatchPerPage = 7;

            public HarvestTrackerGump(int PageNo)
                : base(50, 50)
            {
                m_PageNo = PageNo;
                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;
                this.AddPage(0);

                #region BottomScroll
                int yMod = 0;
                if (m_WatchList.Count == 0)
                    yMod = 142;
                //bottom scroll
                this.AddImage(211, 350 - yMod, 5153);
                this.AddImage(536, 363 - yMod, 5155);
                this.AddImage(211, 440 - yMod, 5153);
                this.AddImage(536, 433 - yMod, 5155);
                this.AddImage(536, 543 - yMod, 5158);
                this.AddImage(211, 543 - yMod, 5156);
                this.AddImage(367, 543 - yMod, 5157);
                this.AddImage(249, 543 - yMod, 5157);
                this.AddImageTiled(249, 403 - yMod, 287, 152, 5154);
                //end bottom scroll
                #endregion

                #region Background
                this.AddImage(312, 1, 201);
                this.AddImage(0, 1, 206);
                this.AddImageTiled(41, 42, 703, 318, 200);
                this.AddImage(44, 1, 201);
                this.AddImage(733, 1, 207);
                this.AddImage(0, 45, 202);
                this.AddImage(733, 44, 203);
                this.AddImage(0, 360, 204);
                this.AddImage(733, 360, 205);
                this.AddImage(44, 360, 233);
                this.AddImage(307, 360, 233);


                #endregion


                #region Images
                this.AddImage(363, 35, 70);
                this.AddImage(437, 24, 10451);
                this.AddImage(270, 24, 10451);
                #endregion

                #region Titles and Horizontal Bar
                this.AddLabel(96, 80, 0x0, @"Fishing");
                this.AddLabel(602, 80, 0x0, @"Mining");
                this.AddLabel(332, 80, 0x0, @"Lumberjacking");
                this.AddImage(298, 100, 96);
                this.AddImage(545, 100, 96);
                this.AddImage(45, 100, 96);
                #endregion

                int buttonID = 1;

                #region Fishing
                List<KeyValuePair<Mobile, int>> m_FishingList = new List<KeyValuePair<Mobile, Int32>>();
                m_FishingList = ReturnTop10(HarvestType.Fishing);
                if (m_FishingList != null)
                {
                    Mobile[] fishing = new Mobile[m_FishingList.Count];
                    int[] value1 = new int[m_FishingList.Count];
                    int i1 = 0;
                    foreach (KeyValuePair<Mobile, int> entry in m_FishingList)
                    {
                        fishing[i1] = entry.Key;
                        value1[i1++] = entry.Value;
                    }
                    m_fishing = fishing;

                    int j1 = 0;
                    foreach (Mobile m in fishing)
                    {
                        this.AddButton(22, 110 + j1 * 20, 4007, 4006, buttonID++, GumpButtonType.Reply, 0);
                        this.AddLabel(52, 112 + j1 * 20, 0x0, String.Format("{0}. {1}: {2}", j1 + 1, fishing[j1].Name, value1[j1++]));
                    }
                }
                #endregion

                buttonID = 11;

                #region Lumberjacking
                List<KeyValuePair<Mobile, int>> m_LumberList = new List<KeyValuePair<Mobile, Int32>>();
                m_LumberList = ReturnTop10(HarvestType.Lumberjacking);
                if (m_LumberList != null)
                {
                    Mobile[] lumber = new Mobile[m_LumberList.Count];
                    int[] value2 = new int[m_LumberList.Count];
                    int i2 = 0;
                    foreach (KeyValuePair<Mobile, int> entry in m_LumberList)
                    {
                        lumber[i2] = entry.Key;
                        value2[i2++] = entry.Value;
                    }
                    m_lumber = lumber;

                    int j2 = 0;
                    foreach (Mobile m in lumber)
                    {
                        this.AddButton(275, 110 + j2 * 20, 4007, 4006, buttonID++, GumpButtonType.Reply, 0);
                        this.AddLabel(305, 112 + j2 * 20, 0x0, String.Format("{0}. {1}: {2}", j2 + 1, lumber[j2].Name, value2[j2++]));
                    }
                }
                #endregion

                buttonID = 21;

                #region Mining

                List<KeyValuePair<Mobile, int>> m_MiningList = new List<KeyValuePair<Mobile, Int32>>();
                m_MiningList = ReturnTop10(HarvestType.Mining);
                if (m_MiningList != null)
                {
                    Mobile[] mining = new Mobile[m_MiningList.Count];
                    int[] value3 = new int[m_MiningList.Count];
                    int i3 = 0;
                    foreach (KeyValuePair<Mobile, int> entry in m_MiningList)
                    {
                        mining[i3] = entry.Key;
                        value3[i3++] = entry.Value;
                    }
                    m_mining = mining;

                    int j3 = 0;
                    foreach (Mobile m in mining)
                    {
                        this.AddButton(522, 109 + j3 * 20, 4007, 4006, buttonID++, GumpButtonType.Reply, 0);
                        this.AddLabel(552, 112 + j3 * 20, 0x0, String.Format("{0}. {1}: {2}", j3 + 1, mining[j3].Name, value3[j3++]));
                    }
                }
                #endregion

                buttonID = 31;

                #region Watchlist
                
                //Up & Down Buttons
                if (m_PageNo > 1)
                    this.AddButton(236, 409, 251, 253, 50, GumpButtonType.Reply, 0); //up
                if (m_WatchList.Count > noWatchPerPage*m_PageNo)
                    this.AddButton(236, 534, 253, 252, 51, GumpButtonType.Reply, 0); //down

                if (m_WatchList.Count > 0)
                {
                    int count = m_WatchList.Count;
                    if (count > noWatchPerPage)
                        count = noWatchPerPage;
                    for (int k = 0; k < count; k++)
                    {
                        Mobile m = m_WatchList[k + (m_PageNo - 1) * noWatchPerPage];
                        this.AddButton(257, 405 + k * 20, 4004, 4003, buttonID++, GumpButtonType.Reply, 0); //remove 1
                        this.AddLabel(292, 407 + k * 20, 0x0, String.Format("{0}. {1}: {2}", k + 1, m.Account.Username, m.Name));
                    }
                }
                //People on watchlist
                #endregion
            }

            public override void OnResponse(NetState state, RelayInfo info)
            {
                Mobile from = state.Mobile;
                int index;

                if (info.ButtonID > 0 && info.ButtonID < 11) //fishing
                {
                    index = info.ButtonID - 1;
                    if (m_fishing.Length > index)
                    {
                        Mobile m = m_fishing[index];
                        AddToWatchlist(from, m);
                    }
                }
                else if (info.ButtonID > 10 && info.ButtonID < 21) //lumberjacking
                {
                    index = info.ButtonID - 11;
                    if (m_lumber.Length > index)
                    {
                        Mobile m = m_lumber[index];
                        AddToWatchlist(from, m);
                    }
                }
                else if (info.ButtonID > 20 && info.ButtonID < 31) //mining
                {
                    index = info.ButtonID - 21;
                    if (m_mining.Length > index)
                    {
                        Mobile m = m_mining[index];
                        AddToWatchlist(from, m);
                    }
                }
                else if (info.ButtonID > 30 && info.ButtonID < 50)//remove from watch list
                {
                    index = info.ButtonID - 31;
                    if (m_WatchList.Count > index)
                    {
                        Mobile m = m_WatchList[index];
                        m_WatchList.Remove(m);
                        from.SendMessage(String.Format("{0} has been removed to the Watchlist. You will no longer recieve notifications for this player.", m.Name));
                    }
                    from.SendGump(new HarvestTrackerGump(m_PageNo));
                }
                else if (info.ButtonID == 50) //pageup
                {
                    from.SendGump(new HarvestTrackerGump(--m_PageNo));
                }
                else if (info.ButtonID == 51)//pagedown
                {
                    from.SendGump(new HarvestTrackerGump(++m_PageNo));
                }
            }

            private void AddToWatchlist(Mobile from, Mobile m)
            {
                if (m.Map == Map.Internal)
                {
                    from.SendMessage(String.Format("{0} is not currently logged in.", m.RawName));
                    from.SendGump(new HarvestTrackerGump(m_PageNo));
                }
                else
                {
                    from.SendMessage(String.Format("You have been teleported to {0}.", m.RawName));
                    from.MoveToWorld(m.Location, m.Map);
                }
                /*if (m_WatchList.Contains(m))
                {
                    from.SendMessage(String.Format("{0} is already on the Watchlist.", m.Name));
                    from.SendGump(new HarvestTrackerGump(m_PageNo));
                }
                else
                {
                    from.SendMessage(String.Format("{0} has been added to the Watchlist. Staff will be notified upon any harvest by this player.", m.Name));
                    m_WatchList.Add(m);
                    from.SendGump(new HarvestTrackerGump(m_PageNo));
                }*/
            }
        }

        public class HarvestTrackerPersistance : Item
        {
            public override string DefaultName { get { return "Harvest Tracker - Internal"; } }
            public HarvestTrackerPersistance()
                : base(1)
            {
                Movable = false;
                if (m_Instance == null || m_Instance.Deleted)
                    m_Instance = this;
                else
                    base.Delete();
            }

            public HarvestTrackerPersistance(Serial serial)
                : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
				if (m_DateLastReset.Date < DateTime.UtcNow.Date)
                    Reset();

                base.Serialize(writer);
                writer.WriteEncodedInt((int)1);

                //version 1
                writer.Write(m_TableFish.Count);
                foreach (KeyValuePair<Mobile, Int32> entry in m_TableFish)
                {
                    writer.Write(entry.Key);
                    writer.Write(entry.Value);
                }

                writer.Write(m_TableLumber.Count);
                foreach (KeyValuePair<Mobile, Int32> entry in m_TableLumber)
                {
                    writer.Write(entry.Key);
                    writer.Write(entry.Value);
                }

                writer.Write(m_TableOre.Count);
                foreach (KeyValuePair<Mobile, Int32> entry in m_TableOre)
                {
                    writer.Write(entry.Key);
                    writer.Write(entry.Value);
                }

                //version 0
                writer.Write((DateTime)m_DateLastReset);
                writer.WriteMobileList(m_WatchList);

                
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                int version = reader.ReadEncodedInt();

                switch (version)
                {
                    case 1:
                        {
                            int countFish = reader.ReadInt();
                            for (int i = 0; i < countFish; ++i)
                            {
                                Mobile key = reader.ReadMobile();
                                int value = reader.ReadInt();

                                if (key != null)
                                    m_TableFish.Add(key, value);
                            }

                            int countLumber = reader.ReadInt();
                            for (int i = 0; i < countLumber; ++i)
                            {
                                Mobile key = reader.ReadMobile();
                                int value = reader.ReadInt();

                                if (key != null)
                                    m_TableLumber.Add(key, value);
                            }

                            int countOre = reader.ReadInt();
                            for (int i = 0; i < countOre; ++i)
                            {
                                Mobile key = reader.ReadMobile();
                                int value = reader.ReadInt();

                                if (key != null)
                                    m_TableOre.Add(key, value);
                            }

                            goto case 0;
                        }
                    case 0:
                        {
                            m_DateLastReset = reader.ReadDateTime();
                            ArrayList aL = reader.ReadMobileList();
                            m_WatchList = new List<Mobile>();
                            foreach (Mobile m in aL)
                            {
                                m_WatchList.Add(m);
                            }
                            break;
                        }
                }

                m_Instance = this;
            }
        }
    }



    
}