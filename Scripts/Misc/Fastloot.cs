using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Server.Gumps;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom
{
    public static class AntiFastLoot
    {
        public class LootEntry
        {
            public Mobile Looter { get; set; }
            public int Count { get; set; }
            public TimeSpan Fastest { get; set; }

            public LootEntry(Mobile who)
            {
                Looter = who;
                Fastest = TimeSpan.FromSeconds(1.0);
            }
        }

        public static void Initialize()
        {
            CommandSystem.Register("fastloot", AccessLevel.Counselor, LootCommandHandler);
            CommandSystem.Register("clearfastloot", AccessLevel.Counselor, ClearLootCommandHandler);
        }

        public static void LootCommandHandler(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            m_Table.Sort((a, b) => b.Count.CompareTo(a.Count));

            from.CloseGump(typeof(FastlootGump));
            from.SendGump(new FastlootGump());
        }

        public static void ClearLootCommandHandler(CommandEventArgs e)
        {
            m_Table.Clear();
        }

        private static List<LootEntry> m_Table = new List<LootEntry>();

        public static void RegisterFastloot(Mobile from, TimeSpan lootTime)
        {
            if (from == null || !(from is PlayerMobile))
                return;

            LootEntry entry = FindLooter(from);

            entry.Count++;

            if (lootTime < entry.Fastest)
                entry.Fastest = lootTime;
        }

        public static LootEntry FindLooter(Mobile from)
        {
            for (int i = 0; i < m_Table.Count; i++)
            {
                if (from == m_Table[i].Looter)
                    return m_Table[i];
            }

            var entry = new LootEntry(from);
            m_Table.Add(entry);
            return entry;
        }

        private class FastlootGump : Gump
        {
            public FastlootGump()
                : base(100, 100)
            {
                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = true;
                this.AddPage(0);
                AddBackground(0, 0, 300, 500, 0xE10);
                AddAlphaRegion(15, 15, 270, 470);

                string strHTML = "<BASEFONT COLOR=WHITE><b>FAST LOOTERS</b><br><br>";

                int count = Math.Min(m_Table.Count, 25);

                for (int i = 0; i < count; i++)
                {
                    var entry = m_Table[i];

                    strHTML += String.Format("{0}. {1} @ {2:0.0}ms<br>", entry.Count, entry.Looter.RawName, entry.Fastest.TotalMilliseconds);
                }

                this.AddHtml(20, 20, 260, 460, strHTML, (bool)false, (bool)true);
            }
        }
    }
}
