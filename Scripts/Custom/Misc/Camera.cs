/***************************************************************************
 *                                 Camera.cs
 *                            -------------------
 *   begin                : February 9, 2011
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/

/***************************************************************************
 *
 * This script enables the use of a 'Camera' (i.e. Hidden Mobile) to
 * teleport randomly to predetermined HotSpots.  In addition, it has an
 * option to also teleport to a players upon death, to keep the Camera
 * entertaining.
 * 
 * Default Values: 
 * Player death cam enabled.
 * 1% chance to view player death for 30 seconds.
 * 5 Minutes per hotspot (hotspots are random).
 *
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Server.Commands;
using Server.Items;
using Server.Mobiles;

namespace Server.Custom
{
    public static class Camera
    {
        #region Variables    

        private static double DeathCamSwitch = 1.0; //Percent chance to switch to the latest death (0.01 = 1%)
        private static readonly TimeSpan HotSpotShuffleLength = TimeSpan.FromMinutes(1.0); //How long should the camera stay at a hotspot?
        private static readonly TimeSpan DeathShuffleLength = TimeSpan.FromSeconds(90); // How long should the camera look at the latest death?

        private static Point3D[] HotSpots
        {
            get
            {
                return new Point3D[] { 
                    new Point3D(1435, 1697, 0),  //Britain West Bank
                    new Point3D(1428, 1685, 20),  // Britain Casino
                    new Point3D(5405, 91, 10),   //Shame Entrance
                    new Point3D(5568, 1861, 0),  //Covetous 3
                    new Point3D(5770, 19, 0),	 //Shame blood elementals
                    new Point3D(5394, 1253, 5),  // 1v1 arena
                    new Point3D(5434, 1253, 10), // 1v1 arena
                    //new Point3D(1490, 1797, 8),// 2v2 arena
                    new Point3D(5243, 985, -40), // destard
                    new Point3D(2504, 563, 0),   // minoc
                    new Point3D(2892, 692, 0),   // vesper
                    new Point3D(2235, 1205, 0),  // cove
                    new Point3D(657, 817, 0),    // yew
                    new Point3D(3652, 2599, 0),  // ocllo
                    new Point3D(3729, 2162, 20), // magincia
                    new Point3D(3016, 3364, 15), // serps
                    new Point3D(3765, 1308, 0),  // nujel'm
                    new Point3D(4454, 1165, 0),  // moon glow
                    new Point3D(600, 2136, 0),   // skara brae
                    new Point3D(1321, 3780, 0),  // jhelom
                    new Point3D(1823, 2823, 0),  // trinsic
                    //new Point3D(5869, 1192, 5) // tournament area
                };
            }
        }

        #endregion

        #region Camera Methods and Timers
        private static InternalTimer m_Timer;
        private static PlayerDeathEventHandler m_Handler = new PlayerDeathEventHandler(EventSink_PlayerDeath);
        private static bool m_Handled = false;

        public static Mobile CameraMobile { get; set; }
        public static DateTime NextShuffle { get; set; }

        public static void Initialize()
        {
            CommandSystem.Register("RegisterCamera", AccessLevel.Counselor, new CommandEventHandler(RegisterCamera_OnCommand));
            CommandSystem.Register("UnregisterCamera", AccessLevel.Counselor, new CommandEventHandler(UnregisterCamera_OnCommand));
            CommandSystem.Register("SetCamDeathPercent", AccessLevel.Counselor, new CommandEventHandler(SetCamDeathPercent_OnCommand));
        }

        [Usage("RegisterCamera")]
        [Description("Registers the Camera mobile")]
        public static void RegisterCamera_OnCommand(CommandEventArgs e)
        {
            StartCamera(e.Mobile);
        }

        [Usage("RegisterCamera")]
        [Description("Unregisters the Camera")]
        public static void UnregisterCamera_OnCommand(CommandEventArgs e)
        {
            EndCamera(e.Mobile);
        }

        [Usage("SetCamDeathPercent")]
        [Description("SetCamDeathPercent")]
        public static void SetCamDeathPercent_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            double d;

            if (e.Length == 1 && Double.TryParse(e.GetString(0).Trim(), out d))
            {
                from.SendMessage("The death camera percentage is now set at {0:0.00}.", d);
                DeathCamSwitch = d;
            }
            else
            {
                e.Mobile.SendMessage(0x25, "Bad Format: SetCamDeathPercent <0.10>");
            }
        }

        public static void StartCamera(Mobile m)
        {
            if (CameraMobile != null)
                EndCamera(CameraMobile);

            CameraMobile = m;
            CameraMobile.BodyValue = 0;
            CameraMobile.Hidden = true;
            CameraMobile.Frozen = true;
            CameraMobile.Blessed = true;
            ((PlayerMobile)CameraMobile).IsDeathCam = true;

            CameraMobile.SendMessage("This mobile is now registered as the camera.");

			NextShuffle = DateTime.UtcNow;

            m_Timer = new InternalTimer();
            m_Timer.Start();

            if (!m_Handled)
            {
                EventSink.PlayerDeath += m_Handler;
                m_Handled = true;
            }
        }

        public static bool AnkhNear(Point3D p, Map map)
        {
            if (map == null || map == Map.Internal)
                return false;

            IPooledEnumerable eable = map.GetObjectsInRange(p,4);

            bool near = false;

            Item i;
            foreach (object o in eable)
            {
                if (o is Item)
                {
                    i = (Item)o;

                    if (i.ItemID > 1 && i.ItemID < 6)
                    {
                        near = true;
                        break;
                    }
                }
            }

            eable.Free();
            return near;
        }

        public static void EndCamera(Mobile m)
        {
            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }

            if (m_Handled)
            {
                EventSink.PlayerDeath -= m_Handler;
                m_Handled = false;
            }

            if (m == null || m != CameraMobile)
                return;

            CameraMobile.BodyValue = 400;
            CameraMobile.Frozen = false;
            ((PlayerMobile)CameraMobile).IsDeathCam = false;

            CameraMobile.SendMessage("This mobile is no longer registered as the camera.");

            CameraMobile = null;
        }

        public static void Shuffle()
        {
			if (NextShuffle > DateTime.UtcNow)
                return;

            if (!ValidCamera())
            {
                EndCamera(CameraMobile);
                return;
            }

            Point3D[] locations = HotSpots;
            Point3D newLocation = locations[Utility.Random(locations.Length)];

            CameraMobile.MoveToWorld(newLocation, Map.Felucca);

			NextShuffle = DateTime.UtcNow + HotSpotShuffleLength;
        }

        public static void EventSink_PlayerDeath(PlayerDeathEventArgs e)
        {
            if (e == null)
                return;

            if (!ValidCamera())
		    {
                EndCamera(CameraMobile);
		        return;
		    }

            Mobile killed = e.Mobile;

            if (killed == null || killed.Map == null || killed.Map == Map.Internal)
                return;

            if (Utility.RandomDouble() > DeathCamSwitch)
                return;

            if (AnkhNear(killed.Location, killed.Map))
                return;

            CameraMobile.MoveToWorld(killed.Location, killed.Map);
			NextShuffle = DateTime.UtcNow + DeathShuffleLength;
        }

        public static bool ValidCamera()
        {
            return (CameraMobile != null && !CameraMobile.Deleted && CameraMobile.NetState != null);
        }

        private class InternalTimer : Timer
        {
            public InternalTimer()
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(20))
            {
                Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                Shuffle();
            }
        }
        #endregion
    }
}
