using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Spells;
using Server.Regions;

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

        public static double GetStalkingBonus(Mobile tracker, Mobile target)
        {
            TrackingInfo info = null;
            m_Table.TryGetValue(tracker, out info);

            if (info == null || info.m_Target != target || info.m_Map != target.Map)
                return 0.0;

            int xDelta = info.m_Location.X - target.X;
            int yDelta = info.m_Location.Y - target.Y;

            double bonus = Math.Sqrt((xDelta * xDelta) + (yDelta * yDelta));

            m_Table.Remove(tracker);	//Reset as of Pub 40, counting it as bug for Core.SE.

            if (Core.ML)
                return Math.Min(bonus, 10 + tracker.Skills.Tracking.Value / 10);

            return bonus;
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

        public TrackWhatGump(Mobile from)
            : base(20, 30)
        {
            m_From = from;
            m_Success = from.CheckSkill(SkillName.Tracking, 0.0, 21.1, 1.0);

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
                TrackWhoGump.DisplayTo(m_Success, m_From, info.ButtonID - 1);
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

        public static void DisplayTo(bool success, Mobile from, int type)
        {
            if (!success)
            {
                from.SendLocalizedMessage(1018092); // You see no evidence of those in the area.
                return;
            }

            Map map = from.Map;

            if (map == null)
                return;

            TrackTypeDelegate check = m_Delegates[type];

            from.CheckSkill(SkillName.Tracking, 21.1, 100.0, 1.0); // Passive gain

            int range = (4 + (int)(from.Skills[SkillName.Tracking].Value / 5.0)) * 3;

            List<Mobile> list = new List<Mobile>();

            bool inDungeon = from.Region.IsPartOf(typeof(DungeonRegion));                        

            IPooledEnumerable eable = from.GetMobilesInRange(range);

            foreach (Mobile m in eable)
            {
                //If Tracker is in Dungeon, Can Only Track Entities in that Same Dungeon
                if (inDungeon && from.Region != m.Region)                
                    continue;                

                // Ghosts can no longer be tracked 
                if (m != from && (!Core.AOS || m.Alive) && (!m.Hidden || m.AccessLevel == AccessLevel.Player || from.AccessLevel > m.AccessLevel) && check(m) && CheckDifficulty(from, m))
                    list.Add(m);
            }

            eable.Free();

            if (list.Count > 0)
            {
                list.Sort(new InternalSorter(from));

                from.SendGump(new TrackWhoGump(from, list, range));
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

        // Tracking players uses tracking and detect hidden vs. hiding and stealth 
        private static bool CheckDifficulty(Mobile from, Mobile m)
        {
            if (!m.Player)
                return true;

            int tracking = from.Skills[SkillName.Tracking].Fixed;
            int detectHidden = from.Skills[SkillName.DetectHidden].Fixed;

            if (Core.ML && m.Race == Race.Elf)
                tracking /= 2; //The 'Guide' says that it requires twice as Much tracking SKILL to track an elf.  Not the total difficulty to track.

            int hiding = m.Skills[SkillName.Hiding].Fixed;
            int stealth = m.Skills[SkillName.Stealth].Fixed;
            int divisor = hiding + stealth;
            
            int chance;

            if (divisor > 0)
            {
                if (Core.SE)
                    chance = 50 * (tracking * 2 + detectHidden) / divisor;
                else
                    chance = 50 * (tracking + detectHidden + 10 * Utility.RandomMinMax(1, 20)) / divisor;
            }

            else
                chance = 100;

            return chance > Utility.Random(100);
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

        private TrackWhoGump(Mobile from, List<Mobile> list, int range)
            : base(20, 30)
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

                if (Core.SE)
                    Tracking.AddInfo(m_From, m);
            }
        }
    }

    public class TrackArrow : QuestArrow
    {
        private Mobile m_From;
        private Timer m_Timer;

        public TrackArrow(Mobile from, Mobile target, int range)
            : base(from, target)
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
            {
                Tracking.ClearTrackingInfo(m_From);

                m_From.SendLocalizedMessage(503177); // You have lost your quarry.
            }
        }
    }

    public class TrackTimer : Timer
    {
        private Mobile m_From, m_Target;
        private int m_Range;
        private int m_LastX, m_LastY;
        private QuestArrow m_Arrow;

        public TrackTimer(Mobile from, Mobile target, int range, QuestArrow arrow)
            : base(TimeSpan.FromSeconds(0.25), TimeSpan.FromSeconds(2.5))
        {
            m_From = from;
            m_Target = target;
            m_Range = range;

            m_Arrow = arrow;
        }

        protected override void OnTick()
        {
            if (!m_Arrow.Running)
            {
                Stop();
                return;
            }
            else if (m_From.NetState == null || m_From.Deleted || m_Target.Deleted || m_From.Map != m_Target.Map || !m_From.InRange(m_Target, m_Range) || (m_Target.Hidden && m_Target.AccessLevel > m_From.AccessLevel))
            {
                m_Arrow.Stop();
                Stop();
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