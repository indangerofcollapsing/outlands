using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server.Accounting;
using Server.Mobiles;
using Server.Network;
using Server.Misc;
using Server.Gumps;
using Server.Commands;

namespace Server.Custom
{
    public static class GoldCoinTracker
    {
        private static void ResetLeaderBoard_OnCommand(CommandEventArgs e)
        {
            // count total amount of valorite coins looted for all accounts
            foreach (Mobile m in World.Mobiles.Values)
            {
                PlayerMobile pm = m as PlayerMobile;
                if (pm == null || pm.AccessLevel > AccessLevel.Player)
                    continue;

                pm.NumGoldCoinsGenerated = 0;
            }
        }

        public static void TrackGoldCoinLoot(int amount, PlayerMobile killer)
        {
            if (killer != null && killer.AccessLevel == AccessLevel.Player)
                killer.NumGoldCoinsGenerated += amount;
        }

        public class Entry : IComparable<Entry>
        {
            public string name;
            public int count;
            public string guild;

            public int CompareTo(Entry other)
            {
                if (other == null)
                    return 1;
                else
                    return other.count - count;
            }
        }

        public class Report
        {
            public List<Entry> m_Top10;
        }

        static Report m_LastReport = null;
        public static Report LastReport { get { return m_LastReport; } }

        public static void Initialize()
        {
            EventSink.ServerStarted += new ServerStartedEventHandler(() => { GenerateReport(); });
            EventSink.WorldSave += new WorldSaveEventHandler((x) => { GenerateReport(); });
            CommandSystem.Register("resetpveleaderboard", AccessLevel.Administrator, new CommandEventHandler(ResetLeaderBoard_OnCommand));
            m_LastReport = new Report();
            m_LastReport.m_Top10 = new List<Entry>();
        }

        public static void GenerateHTML()
        {
        }

        public static void ShowGump(Mobile m)
        {
            m.SendGump(new GoldCoinReportGump(m, m_LastReport));
        }

        public static void GenerateReport()
        {
            List<Entry> allentries = new List<Entry>();

            // count total amount of coins generated for all accounts
            foreach (Mobile m in World.Mobiles.Values)
            {
                PlayerMobile pm = m as PlayerMobile;
                if (pm == null || pm.AccessLevel > AccessLevel.Player)
                    continue;

                if (pm.NumGoldCoinsGenerated > 0)
                {
                    string guildabbrev = pm.Guild != null ? pm.Guild.m_Abbreviation : "";
                    allentries.Add(new Entry() { name = pm.Name, count = pm.NumGoldCoinsGenerated, guild = guildabbrev });
                }
            }

            // get top 10 scorers
            allentries.Sort();
            m_LastReport.m_Top10 = new List<Entry>();
            for (int i = 0; i < Math.Min(10, allentries.Count); ++i)
            {
                m_LastReport.m_Top10.Add(allentries[i]);
            }
        }
    }

    public class GoldCoinReportGump : Server.Gumps.Gump
    {
        public GoldCoinReportGump(Mobile from, GoldCoinTracker.Report report)
            : base(0, 0)
        {
            from.CloseGump(typeof(GoldCoinReportGump));

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            this.AddPage(0);
            this.AddBackground(0, 31, 360, 400, 0x2436);
            this.AddImage(140, 0, 1417);
            this.AddImage(150, 9, 5608);
            this.AddImage(323, 276, 10410);
            this.AddItem(298, 45, 3823);
            this.AddItem(13, 45, 3823);
            this.AddButton(279, 392, 0x81c, 0x81b, 0, GumpButtonType.Reply, 0);


            this.AddImageTiled(20, 140, 325, 3, 0x238e);//horizontal bar

            int label_hue = 53;
            this.AddLabel(130, 93, 53, @"PvE CHAMPIONS");
            this.AddLabel(50, 123, label_hue, @"PLAYER");
            this.AddLabel(155, 123, label_hue, @"GUILD");
            this.AddLabel(210, 123, label_hue, @"COINS");

            int entry_hue = 1153;
            int y = 150;
            int delta = 22;
            for (int i = 0; i < report.m_Top10.Count; ++i)
            {
                string l1 = "#" + (i + 1).ToString();
                AddLabel(20, y + (delta * i), entry_hue, l1);
                AddLabel(50, y + (delta * i), entry_hue, report.m_Top10[i].name);
                AddLabel(155, y + (delta * i), entry_hue, report.m_Top10[i].guild);
                AddLabel(210, y + (delta * i), entry_hue, report.m_Top10[i].count.ToString("#,##0"));
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            sender.Mobile.SendGump(new IPYGump(sender.Mobile));
        }
    }
}
