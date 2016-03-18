using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Misc;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Achievements;

namespace Server.Gumps
{
    public class ReportMurdererGump : Gump
    {
        private int m_Idx;

        private Mobile m_Victim;
        private DeathEventType m_DeathEventType;
        private List<Mobile> m_Killers;
        private DateTime m_EventTime;
        private Point3D m_Location;
        private Point3D m_RandomLocation;    
        private Map m_Map;

        public class GumpTimer : Timer
        {
            private int m_Idx;

            private Mobile m_Victim;
            private DeathEventType m_DeathEventType;
            private List<Mobile> m_Killers;
            private DateTime m_EventTime;
            private Point3D m_Location;
            private Point3D m_RandomLocation; 
            private Map m_Map;

            public GumpTimer(Mobile victim, DeathEventType deathEventType, List<Mobile> killers, DateTime eventTime, Point3D location, Point3D randomLocation, Map map): base(TimeSpan.FromSeconds(2.0))
            {
                m_Victim = victim;
                m_Killers = killers;
                m_EventTime = eventTime;
                m_Location = location;
                m_RandomLocation = randomLocation;    
                m_Map = map;
            }

            protected override void OnTick()
            {
                m_Victim.SendGump(new ReportMurdererGump(m_Victim, m_DeathEventType, m_Killers, m_EventTime, m_Location, m_RandomLocation, m_Map));
            }
        }

        public ReportMurdererGump(Mobile victim, DeathEventType deathEventType, List<Mobile> killers, DateTime eventTime, Point3D location, Point3D randomLocation, Map map): this(victim, deathEventType, killers, eventTime, location, randomLocation, map, 0)
        {
        }

        private ReportMurdererGump(Mobile victim, DeathEventType deathEventType, List<Mobile> killers, DateTime eventTime, Point3D location, Point3D randomLocation, Map map, int idx)
            : base(0, 0)
        {
            m_Victim = victim;
            m_DeathEventType = deathEventType;
            m_Killers = killers;
            m_EventTime = eventTime;
            m_Location = location;
            m_RandomLocation = randomLocation;     
            m_Map = map;

            m_Idx = idx;

            BuildGump();
        }

        private void BuildGump()
        {
            AddBackground(265, 205, 320, 290, 5054);
            Closable = false;
            Resizable = false;

            AddPage(0);

            AddImageTiled(225, 175, 50, 45, 0xCE);   //Top left corner
            AddImageTiled(267, 175, 315, 44, 0xC9);  //Top bar
            AddImageTiled(582, 175, 43, 45, 0xCF);   //Top right corner
            AddImageTiled(225, 219, 44, 270, 0xCA);  //Left side
            AddImageTiled(582, 219, 44, 270, 0xCB);  //Right side
            AddImageTiled(225, 489, 44, 43, 0xCC);   //Lower left corner
            AddImageTiled(267, 489, 315, 43, 0xE9);  //Lower Bar
            AddImageTiled(582, 489, 43, 43, 0xCD);   //Lower right corner

            AddPage(1);

            AddHtml(260, 234, 300, 140, ((Mobile)m_Killers[m_Idx]).Name, false, false); // Player's Name
            AddHtmlLocalized(260, 254, 300, 140, 1049066, false, false); // Would you like to report...

            AddButton(260, 300, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(300, 300, 300, 50, 1046362, false, false); // Yes

            AddButton(360, 300, 0xFA5, 0xFA7, 2, GumpButtonType.Reply, 0);
            AddHtmlLocalized(400, 300, 300, 50, 1046363, false, false); // No      
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            switch (info.ButtonID)
            {
                //Report Murder
                case 1:
                    {
                        int killerCount = m_Killers.Count;

                        PlayerMobile killer = m_Killers[m_Idx] as PlayerMobile;
                        PlayerMobile victim = from as PlayerMobile;

                        PaladinEvents.ReportMurder(killer, victim, m_Killers.Count, m_Location, m_Map);

                        DeathEventEntry deathEventEntry = new DeathEventEntry(m_Victim, m_DeathEventType, m_Killers, m_EventTime, m_Location, m_RandomLocation, m_Map);
                        PaladinEvents.AddDeathEventEntry(deathEventEntry);

                        //Add Additional Recall Restriction Minutes for Extra Killers (Criminal Timer Takes Care of First Killer)
                        if (DateTime.UtcNow < m_EventTime + TimeSpan.FromSeconds(PaladinEvents.recallRestrictionVictimTimeout))
                        {
                            if (killerCount > 1)
                            {
                                int duration = (int)(((double)killerCount - 1) * (double)PaladinEvents.recallRestrictionDurationPerKiller);
                                
                                if (killer.RecallRestrictionExpiration < DateTime.UtcNow)
                                    killer.RecallRestrictionExpiration = DateTime.UtcNow + TimeSpan.FromSeconds(duration);
                                
                                else
                                    killer.RecallRestrictionExpiration = killer.RecallRestrictionExpiration + TimeSpan.FromSeconds(duration);
                            }
                        }

                        break;
                    }

                //Ignore The Murder
                case 2:
                {
                    break;
                }
            }


            m_Idx++;

            if (m_Idx < m_Killers.Count)
                from.SendGump(new ReportMurdererGump(from, m_DeathEventType, m_Killers, m_EventTime, m_Location, m_RandomLocation, m_Map, m_Idx));
        }
    }
}