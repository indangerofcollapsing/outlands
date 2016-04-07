using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Spells;
using Server.Regions;
using Server.Mobiles;

namespace Server.SkillHandlers
{
    public class Tracking
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.Tracking].Callback = new SkillUseCallback(OnUse);
        }

        public static TimeSpan OnUse(Mobile m)
        {
            m.SendLocalizedMessage(1011350); // What do you wish to track?

            m.CloseGump(typeof(TrackWhatGump));
            m.CloseGump(typeof(TrackWhoGump));
            m.SendGump(new TrackWhatGump(m));

            return TimeSpan.FromSeconds(SkillCooldown.TrackingCooldown);
        }

        public class TrackingInfo
        {
            public Mobile m_Tracker;
            public Mobile m_Target;
            public Point2D m_Location;
            public Map m_Map;

            public TrackingInfo(Mobile tracker, Mobile target)
            {
                m_Tracker = tracker;
                m_Target = target;
                m_Location = new Point2D(target.X, target.Y);
                m_Map = target.Map;
            }
        }

        private static Dictionary<Mobile, TrackingInfo> m_Table = new Dictionary<Mobile, TrackingInfo>();

        public static void AddInfo(Mobile tracker, Mobile target)
        {
            TrackingInfo info = new TrackingInfo(tracker, target);
            
            m_Table[tracker] = info;
        }

        public static void ClearTrackingInfo(Mobile tracker)
        {
            m_Table.Remove(tracker);
        }
    }

    public class TrackWhatGump : Gump
    {
        private Mobile m_From;
        private bool m_Success;

        public TrackWhatGump(Mobile from): base(20, 30)
        {
            m_From = from;            

            AddPage(0);

            AddBackground(0, 0, 440, 135, 5054);

            AddBackground(10, 10, 420, 75, 2620);
            AddBackground(10, 85, 420, 25, 3000);

            AddItem(20, 20, 9682);
            AddButton(20, 110, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(20, 90, 100, 20, 1018087, false, false); // Animals

            AddItem(120, 20, 9607);
            AddButton(120, 110, 4005, 4007, 2, GumpButtonType.Reply, 0);
            AddHtmlLocalized(120, 90, 100, 20, 1018088, false, false); // Monsters

            AddItem(220, 20, 8454);
            AddButton(220, 110, 4005, 4007, 3, GumpButtonType.Reply, 0);
            AddHtmlLocalized(220, 90, 100, 20, 1018089, false, false); // Human NPCs

            AddItem(320, 20, 8455);
            AddButton(320, 110, 4005, 4007, 4, GumpButtonType.Reply, 0);
            AddHtmlLocalized(320, 90, 100, 20, 1018090, false, false); // Players
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info.ButtonID >= 1 && info.ButtonID <= 4)
            {
                if (m_From != null)
                {
                    if (m_From.QuestArrow != null)
                        m_From.QuestArrow.Stop();

                    Tracking.ClearTrackingInfo(m_From);
                }

                bool success = m_From.CheckSkill(SkillName.Tracking, 0.0, 100, 1.0);

                if (!success)
                {
                    m_From.SendMessage("You search for tracks but are uncertain of your findings.");
                    return;
                }

                TrackWhoGump.DisplayTo(m_From, info.ButtonID - 1);
            }
        }
    }

    public delegate bool TrackTypeDelegate(Mobile m);

    public class TrackWhoGump : Gump
    {
        private Mobile m_From;
        private int m_Range;

        private static TrackTypeDelegate[] m_Delegates = new TrackTypeDelegate[]
		{
			new TrackTypeDelegate( IsAnimal ),
			new TrackTypeDelegate( IsMonster ),
			new TrackTypeDelegate( IsHumanNPC ),
			new TrackTypeDelegate( IsPlayer )
		};

        private class InternalSorter : IComparer<Mobile>
        {
            private Mobile m_From;

            public InternalSorter(Mobile from)
            {
                m_From = from;
            }

            public int Compare(Mobile x, Mobile y)
            {
                if (x == null && y == null)
                    return 0;

                else if (x == null)
                    return -1;

                else if (y == null)
                    return 1;

                return m_From.GetDistanceToSqrt(x).CompareTo(m_From.GetDistanceToSqrt(y));
            }
        }

        public static void DisplayTo(Mobile from, int type)
        {
            Map map = from.Map;

            if (map == null)
                return;

            TrackTypeDelegate check = m_Delegates[type];

            int visibleTrackingRange = 20 + (int)Math.Floor((from.Skills[SkillName.Tracking].Value / 100) * 80);
            double detectHiddenScalar = .1 + ((from.Skills[SkillName.DetectHidden].Value / 100) * .9);
            int hiddenTrackingRange = (int)(Math.Floor(visibleTrackingRange * detectHiddenScalar));

            List<Mobile> trackedMobiles = new List<Mobile>();

            IPooledEnumerable nearbyMobiles = from.GetMobilesInRange(visibleTrackingRange);

            foreach (Mobile mobile in nearbyMobiles)
            {
                if (from.Region.IsPartOf(typeof(DungeonRegion)) && from.Region != mobile.Region) continue;
                if (mobile == from) continue;
                if (!mobile.Alive) continue;
                if (mobile.AccessLevel > AccessLevel.Player) continue;
                if (!check(mobile)) continue;
                if (mobile.Hidden && Utility.GetDistance(from.Location, mobile.Location) > hiddenTrackingRange) continue;

                trackedMobiles.Add(mobile);
            }

            nearbyMobiles.Free();

            if (trackedMobiles.Count > 0)
            {
                trackedMobiles.Sort(new InternalSorter(from));

                from.SendGump(new TrackWhoGump(from, trackedMobiles, visibleTrackingRange));
                from.SendLocalizedMessage(1018093); // Select the one you would like to track.
            }

            else
            {
                if (type == 0)
                    from.SendLocalizedMessage(502991); // You see no evidence of animals in the area.

                else if (type == 1)
                    from.SendLocalizedMessage(502993); // You see no evidence of creatures in the area.

                else
                    from.SendLocalizedMessage(502995); // You see no evidence of people in the area.
            }
        }

        private static bool IsAnimal(Mobile m)
        {
            return (!m.Player && m.Body.IsAnimal);
        }

        private static bool IsMonster(Mobile m)
        {
            return (!m.Player && m.Body.IsMonster);
        }

        private static bool IsHumanNPC(Mobile m)
        {
            return (!m.Player && m.Body.IsHuman);
        }

        private static bool IsPlayer(Mobile m)
        {
            return m.Player;
        }

        private List<Mobile> m_List;

        private TrackWhoGump(Mobile from, List<Mobile> list, int range): base(20, 30)
        {
            m_From = from;
            m_List = list;
            m_Range = range;

            AddPage(0);

            AddBackground(0, 0, 440, 155, 5054);

            AddBackground(10, 10, 420, 75, 2620);
            AddBackground(10, 85, 420, 45, 3000);

            if (list.Count > 4)
            {
                AddBackground(0, 155, 440, 155, 5054);

                AddBackground(10, 165, 420, 75, 2620);
                AddBackground(10, 240, 420, 45, 3000);

                if (list.Count > 8)
                {
                    AddBackground(0, 310, 440, 155, 5054);

                    AddBackground(10, 320, 420, 75, 2620);
                    AddBackground(10, 395, 420, 45, 3000);
                }
            }

            for (int i = 0; i < list.Count && i < 12; ++i)
            {
                Mobile m = list[i];

                AddItem(20 + ((i % 4) * 100), 20 + ((i / 4) * 155), ShrinkTable.Lookup(m));
                AddButton(20 + ((i % 4) * 100), 130 + ((i / 4) * 155), 4005, 4007, i + 1, GumpButtonType.Reply, 0);

                if (m.Name != null)
                    AddHtml(20 + ((i % 4) * 100), 90 + ((i / 4) * 155), 90, 40, m.Name, false, false);
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            int index = info.ButtonID - 1;

            if (index >= 0 && index < m_List.Count && index < 12)
            {
                Mobile m = m_List[index];

                m_From.QuestArrow = new TrackArrow(m_From, m, m_Range * 2);
            }
        }
    }

    public class TrackArrow : QuestArrow
    {
        private Mobile m_From;
        private Timer m_Timer;

        public TrackArrow(Mobile from, Mobile target, int range): base(from, target)
        {
            m_From = from;
            m_Timer = new TrackTimer(from, target, range, this);
            m_Timer.Start();
        }

        public override void OnClick(bool rightClick)
        {
            if (rightClick)
            {
                Tracking.ClearTrackingInfo(m_From);

                m_From = null;

                Stop();
            }
        }

        public override void OnStop()
        {
            m_Timer.Stop();

            if (m_From != null)            
                Tracking.ClearTrackingInfo(m_From);
        }
    }

    public class TrackTimer : Timer
    {
        private Mobile m_From, m_Target;
        private int m_Range;
        private int m_LastX, m_LastY;
        private QuestArrow m_Arrow;

        public TrackTimer(Mobile from, Mobile target, int range, QuestArrow arrow): base(TimeSpan.FromSeconds(0.25), TimeSpan.FromSeconds(2.5))
        {
            m_From = from;
            m_Target = target;
            m_Range = range;

            m_Arrow = arrow;

            m_Arrow.m_X = target.Location.X - 2;
            m_Arrow.m_Y = target.Location.Y - 2;
        }

        protected override void OnTick()
        {
            if (!m_Arrow.Running)
            {
                Stop();
                return;
            }

            else if (m_From.NetState == null || !(SpecialAbilities.Exists(m_From)) || !(SpecialAbilities.Exists(m_Target)) || m_From.Map != m_Target.Map || !m_From.InRange(m_Target, m_Range) || (m_Target.AccessLevel > m_From.AccessLevel))
            {
                m_Arrow.Stop();
                Stop();

                if (m_From != null)
                    m_From.SendLocalizedMessage(503177); // You have lost your quarry.

                return;
            }

            if (m_LastX != m_Target.X || m_LastY != m_Target.Y)
            {
                m_LastX = m_Target.X;
                m_LastY = m_Target.Y;

                m_Arrow.Update();
            }
        }
    }
}