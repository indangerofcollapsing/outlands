/***************************************************************************
 *                         BuccaneersTavernControl.cs
 *                            ------------------
 *   begin                : August 2010
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/
using System;
using System.Collections.Generic;
using System.Collections;
using Server.Mobiles;
using Server.Commands;
using Server.Multis;
using Server.Guilds;
using Server.Network;

namespace Server.Custom.Pirates
{

    static class BuccaneersTavernControl
    {
        #region Variables and Definitions

        //xmin, xmax, ymin, ymax
        private static int[] m_BuccaneersDenTavern1 = new int[] { 2671, 2687, 2232, 2247 };
        private static int[] m_BuccaneersDenTavern2 = new int[] { 2664, 2671, 2232, 2239 };
        private static Point3D m_BuccaneersDenPlatformLocation = new Point3D(2679, 2236, 3);
        public static BuccaneersDenPlatform m_BuccaneersDenPlatform;
        private static BuccsControlTimer m_Timer;
        public static DateTime m_ControlStartTime;
        public static bool m_Controlled = false;
        public static Guild m_ControllingGuild;
        public static int m_CurrentControlLevel;

        private static Rectangle2D m_BuccaneersDenTavernRect1 = new Rectangle2D(new Point2D(2671, 2232), new Point2D(2687, 2247));
        private static Rectangle2D m_BuccaneersDenTavernRect2 = new Rectangle2D(new Point2D(2664, 2232), new Point2D(2671, 2239));

        #endregion

        public static void Initialize()
        {
            CommandSystem.Register("StartBuccsTavern", AccessLevel.GameMaster, new CommandEventHandler(StartBuccsTavern_OnCommand));
            Server.Commands.CommandSystem.Register("ClearBuccsTavern", AccessLevel.GameMaster, new CommandEventHandler(ClearBuccsTavern_OnCommand));
        }

        [Usage("StartBuccsTavern")]
        [Description("Creates the Buccaneer's Tavern Platform and initializes the takeover parameters.")]
        public static void StartBuccsTavern_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            if (e.Length == 0)
            {
                if (m_BuccaneersDenPlatform == null)
                {
                    BuccaneersDenPlatform platform = new BuccaneersDenPlatform();
                    platform.MoveToWorld(m_BuccaneersDenPlatformLocation, Map.Felucca);
                    m_BuccaneersDenPlatform = platform;
                    from.SendMessage(String.Format("Buccaneer's Tavern has been initialized."));
                }
                else
                    from.SendMessage(String.Format("Buccaneer's Tavern has already been initialized. Please clear first."));
            }
            else
                e.Mobile.SendMessage(0x25, "Bad Format: StartBuccsTavern");
        }

        [Usage("ClearBuccsTavern")]
        [Description("Deletes the Buccaneer's Tavern Platform and stops all related quests.")]
        public static void ClearBuccsTavern_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            if (e.Length == 0)
            {
                if (m_BuccaneersDenPlatform != null)
                    m_BuccaneersDenPlatform.Delete();
                if (m_Timer != null)
                    m_Timer.Stop();

                m_BuccaneersDenPlatform = null;

                from.SendMessage(String.Format("Buccaneer's Tavern has stopped."));
            }
            else
                e.Mobile.SendMessage(0x25, "Bad Format: StartBuccsTavern");
        }

        public static bool InControlZone(Mobile m)
        {
            Point3D loc = m.Location;

            if ((loc.X > m_BuccaneersDenTavern1[0] && loc.X < m_BuccaneersDenTavern1[1] && loc.Y > m_BuccaneersDenTavern1[2] && loc.Y < m_BuccaneersDenTavern1[3])||
                (loc.X > m_BuccaneersDenTavern2[0] && loc.X < m_BuccaneersDenTavern2[1] && loc.Y > m_BuccaneersDenTavern2[2] && loc.Y < m_BuccaneersDenTavern2[3]))
            {
                return true;
            }
            

            return false;
                    
        }

        public static List<Mobile> GetMobilesInTavern()
        {
            List<Mobile> mobiles = new List<Mobile>();

            IPooledEnumerable eable = Map.Felucca.GetClientsInBounds(m_BuccaneersDenTavernRect1);

            foreach (NetState ns in eable)
            {
                Mobile m = ns.Mobile;
                if (m.Alive && !mobiles.Contains(m))
                            mobiles.Add(m);

            }
            eable.Free();

            eable = Map.Felucca.GetClientsInBounds(m_BuccaneersDenTavernRect2);
            foreach (NetState ns in eable)
            {
                Mobile m = ns.Mobile;
                if (m.Alive && !mobiles.Contains(m))
                    mobiles.Add(m);

            }
            eable.Free();

            return mobiles;
        }

        public static bool SetControllingGuild(Mobile m)
        {
            if (m.Guild != null) 
            {
                m_ControllingGuild = (Guild)m.Guild;
                return true;
            }
            return false;
        }

        public static void SendControllingGuildMessage(string str)
        {
            if (m_ControllingGuild != null)
                m_ControllingGuild.GuildTextMessage(str);
        }

        public static void SendControllingGuildAlliesMessage(string str)
        {

            //Do work son

        } //NEEDS TO BE DONE

        public static void PlatformOnEnter(Mobile m)
        {
            if (m == null)
                return;

            if (!m_Controlled)
            {
                if (SetControllingGuild(m))
                {
                    SendControllingGuildMessage("Your guild now controls Buccaneer's Tavern!");
                    m_CurrentControlLevel = 1;
                    m_Controlled = true;
                    m_ControlStartTime = DateTime.UtcNow;

                    if (m_Timer != null && m_Timer.Running)
                        m_Timer.Stop();

                    m_Timer = new BuccsControlTimer();
                    m_Timer.Start();
                }
                else
                {
                    m.SendMessage("You must be in a guild to control Buccaneer's Tavern.");
                }
            }
            else if (!(m.Guild == m_ControllingGuild))
                m.SendMessage(String.Format("Buccaneer's Tavern is currently controlled by {0}.", m_ControllingGuild.Name));
            else
                m.SendMessage("Your guild currently controls Buccaneer's Tavern!");
        }

        public static void NewControlLevel(int level)
        {
            switch (level)
            {
                case 2:
                    {
                        m_CurrentControlLevel = 2;
                        SendControllingGuildMessage("Your guild has been in control of Buccaneer's Tavern for 12 hours and can now purchase level two items!");
                    } break;
                case 3:
                    {
                        m_CurrentControlLevel = 3;
                        SendControllingGuildMessage("Your guild has been in control of Buccaneer's Tavern for 72 hours and can now purchase level three items!");
                    } break;
            }
        }

        public static void StartTimer()
        {
            if (m_Timer != null)
                m_Timer.Stop();

            m_Timer = new BuccsControlTimer();
            m_Timer.Start();

        }

        public static void StopCurrentControl()
        {
            m_CurrentControlLevel = 0;
            m_Controlled = false;
            m_ControllingGuild = null;
        }

    }

    class BuccsControlTimer : Timer
    {
        private bool m_ControllingGuildWasPresent;
        private DateTime m_DateTimeLastPresent;
        private int currentLevelRewards;

        public BuccsControlTimer()
            : base(TimeSpan.Zero, TimeSpan.FromSeconds(10))
        {
            m_ControllingGuildWasPresent = true;
            m_DateTimeLastPresent = DateTime.UtcNow;
            currentLevelRewards = 1;
            Priority = TimerPriority.FiveSeconds;
        }

        protected override void OnTick()
        {
            if (!BuccaneersTavernControl.m_Controlled)
                this.Stop();

            if (DateTime.UtcNow > m_DateTimeLastPresent + TimeSpan.FromMinutes(30))
            {
                BuccaneersTavernControl.SendControllingGuildMessage("Your guild no longer controls Buccaneer's Tavern.");
                BuccaneersTavernControl.StopCurrentControl();
                this.Stop();
            }

            if (currentLevelRewards == 1)
            {
                if (DateTime.UtcNow > BuccaneersTavernControl.m_ControlStartTime + TimeSpan.FromHours(12))
                {
                    currentLevelRewards = 2;
                    BuccaneersTavernControl.NewControlLevel(2);
                }
            }
            else if (currentLevelRewards == 2)
            {
                if (DateTime.UtcNow > BuccaneersTavernControl.m_ControlStartTime + TimeSpan.FromHours(72))
                {
                    currentLevelRewards = 3;
                    BuccaneersTavernControl.NewControlLevel(3);
                }
            }

            bool ControllingGuildPresent = false;
            List<Mobile> mobiles = BuccaneersTavernControl.GetMobilesInTavern();
            foreach (Mobile m in mobiles)
            {
                if (((Guild)m.Guild == BuccaneersTavernControl.m_ControllingGuild) &&
                    (((PlayerMobile)m).GameTime > ((PlayerMobile)m).m_TimeSpanResurrected + TimeSpan.FromMinutes(3))) //Player must have been alive for at least 3 minutes
                {
                    ControllingGuildPresent = true;
                    break;
                }
            }

            if (ControllingGuildPresent)
            {
                if (!m_ControllingGuildWasPresent)
                {
                    m_ControllingGuildWasPresent = true;
                    BuccaneersTavernControl.SendControllingGuildMessage("Your guild is now present at Buccaneer's Tavern and has renewed the timeclock.");
                }
                m_DateTimeLastPresent = DateTime.UtcNow;
                

            }
            else
            {
                if (m_ControllingGuildWasPresent)
                {
                    m_ControllingGuildWasPresent = false;
                    BuccaneersTavernControl.SendControllingGuildMessage("Your guild is no longer present at Buccaneer's Tavern.");
                }
                
            }
        }
    }
}

