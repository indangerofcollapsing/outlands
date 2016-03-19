/***************************************************************************
 *                             DoubloonDocks.cs
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
using Server.Items;
using Server.Guilds;

namespace Server.Custom.Pirates
{
    public enum DoubloonDock { South, West, North };

    static class DoubloonDocks
    {
        #region Spawn Commands
        public static void Initialize()
        {
            CommandSystem.Register("SpawnDoubloonDocks", AccessLevel.GameMaster, new CommandEventHandler(SpawnDoubloonDocks_OnCommand));
            CommandSystem.Register("ClearDoubloonDocks", AccessLevel.GameMaster, new CommandEventHandler(ClearDoubloonDocks_OnCommand));
           
        }

        [Usage("SpawnDoubloonDocks")]
        [Description("SpawnDoubloonDocks")]
        public static void SpawnDoubloonDocks_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            forceRemove = false;
            if (e.Length == 0)
            {
                SpawnDoubloonDocks(DoubloonDock.North);
                SpawnDoubloonDocks(DoubloonDock.South);
                SpawnDoubloonDocks(DoubloonDock.West);
            }
            else
            {
                e.Mobile.SendMessage(0x25, "Bad Format: [SpawnDoubloonDocks");
            }

            from.SendMessage("The doubloon docks have been spawned.");
        }
        [Usage("ClearDoubloonDocks")]
        [Description("ClearDoubloonDocks")]
        public static void ClearDoubloonDocks_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            if (e.Length == 0)
            {
                ClearDoubloonDocks();
            }
            else
            {
                e.Mobile.SendMessage(0x25, "Bad Format: [ClearDoubloonDocks");
            }
            from.SendMessage("The doubloon docks have been deleted.");
        }
        #endregion
        private static bool forceRemove = false;

        #region Variables...

        //DOCK SPAWN LOCATIONS
        private static Point3D m_SouthBuccaneersDockLocation = new Point3D(2704, 2332+8,1-5);
        private static Point3D m_WestBuccaneersDockLocation = new Point3D(2603-8, 2201, -2-5);
        private static Point3D m_NorthBuccaneersDockLocation = new Point3D(2742, 1937-8, 0-5);
        private static Point3D m_SouthBuccaneersDockGuardLocation = new Point3D(2704, 2345, 2);
        private static Point3D m_WestBuccaneersDockGuardLocation = new Point3D(2590, 2201, -1);
        private static Point3D m_NorthBuccaneersDockGuardLocation = new Point3D(2742, 1923, 1);
        

        //CONTROL ZONES
        //xmin, xmax, ymin, ymax
        private static int[] m_SouthBuccaneersControlZone = new int[] {2687, 2714, 2329, 2359};
        private static int[] m_WestBuccaneersControlZone = new int[] {2579,2607,2182, 2218};
        private static int[] m_NorthBuccaneersControlZone = new int[] { 2727, 2759, 1910, 1935 };

        private static int m_DispersedDoubloons = 10; //NUMBER OF DOUBLOONS TO DISPURSE
        public static TimeSpan m_DoubloonDispurseRate = TimeSpan.FromMinutes(10); //FREQUENCY TO DISPURSE DOUBLOONS
        public static TimeSpan m_InitialControlTime = TimeSpan.FromMinutes(10);

        public static BuccsZoneTimer[] m_Timer = new BuccsZoneTimer[3];

        private static DoubloonDockGuard m_NorthGuard;
        private static DoubloonDockGuard m_SouthGuard;
        private static DoubloonDockGuard m_WestGuard;
        private static NorthBuccsDock m_NorthDock;
        private static SouthBuccsDock m_SouthDock;
        private static WestBuccsDock m_WestDock;
        public static List<Guild> m_NorthCombatantGuilds = new List<Guild>();
        public static List<Guild> m_SouthCombatantGuilds = new List<Guild>();
        public static List<Guild> m_WestCombatantGuilds = new List<Guild>();

        #endregion 

        public static void SpawnDoubloonDocks(DoubloonDock dock)
        {
            switch (dock)
            {
                case DoubloonDock.North:
                    {
                        if (m_NorthDock == null || m_NorthDock.Deleted)
                        {
                            NorthBuccsDock d = new NorthBuccsDock();
                            d.MoveToWorld(m_NorthBuccaneersDockLocation, Map.Felucca);
                            m_NorthDock = d;

                        }
                        if (m_NorthGuard == null || m_NorthGuard.Deleted)
                        {
                            SpawnDockGuard(dock);
                        }

                    }break;
                case DoubloonDock.South:
                    {
                        if (m_SouthDock == null || m_SouthDock.Deleted)
                        {
                            SouthBuccsDock d = new SouthBuccsDock();
                            d.MoveToWorld(m_SouthBuccaneersDockLocation, Map.Felucca);
                            m_SouthDock = d;

                        }
                        if (m_SouthGuard == null || m_SouthGuard.Deleted)
                        {
                            SpawnDockGuard(dock);
                        }

                    } break;
                case DoubloonDock.West:
                    {
                        if (m_WestDock == null || m_WestDock.Deleted)
                        {
                            WestBuccsDock d = new WestBuccsDock();
                            d.MoveToWorld(m_WestBuccaneersDockLocation, Map.Felucca);
                            m_WestDock = d;

                        }
                        if (m_WestGuard == null || m_WestGuard.Deleted)
                        {
                            SpawnDockGuard(dock);
                        }

                    } break;
            }
        }

        public static void ClearDoubloonDocks()
        {
            forceRemove = true;

            if (m_NorthDock != null)
                m_NorthDock.Delete();

            if (m_SouthDock != null)
                m_SouthDock.Delete();

            if (m_WestDock != null)
                m_WestDock.Delete();

            if (m_NorthGuard != null)
                m_NorthGuard.Delete();

            if (m_SouthGuard != null)
                m_SouthGuard.Delete();

            if (m_WestGuard != null)
                m_WestGuard.Delete();

            m_NorthCombatantGuilds.Clear();
            m_WestCombatantGuilds.Clear();
            m_SouthCombatantGuilds.Clear();

        }

        public static void SpawnDockGuard(DoubloonDock dock)
        {
            switch (dock)
            {
                case DoubloonDock.North:
                    {
                        if (m_NorthGuard == null || m_NorthGuard.Deleted)
                        {
                            DoubloonDockGuard g = new DoubloonDockGuard(dock);
                            m_NorthGuard = g;
                            g.MoveToWorld(m_NorthBuccaneersDockGuardLocation, Map.Felucca);
                        }
                    } break;
                case DoubloonDock.South:
                    {
                        if (m_SouthGuard == null || m_SouthGuard.Deleted)
                        {
                            DoubloonDockGuard g = new DoubloonDockGuard(dock);
                            m_SouthGuard = g;
                            g.MoveToWorld(m_SouthBuccaneersDockGuardLocation, Map.Felucca);
                        }
                    } break;
                case DoubloonDock.West:
                    {
                        if (m_WestGuard == null || m_WestGuard.Deleted)
                        {
                            DoubloonDockGuard g = new DoubloonDockGuard(dock);
                            m_WestGuard = g;
                            g.MoveToWorld(m_WestBuccaneersDockGuardLocation, Map.Felucca);
                        }
                    } break;
            } 
        }

        public static void RegisterDockCannon(DoubloonDock dock, DoubloonDockGuard g)
        {
            switch (dock)
            {
                case DoubloonDock.North: { if (g != null) m_NorthGuard = g; }break;
                case DoubloonDock.South: { if (g != null) m_SouthGuard = g; } break;
                case DoubloonDock.West: { if (g != null) m_WestGuard = g; } break;
            }
        }

        public static void RegisterDock(DoubloonDock dock, BaseGuildDock d)
        {
            switch (dock)
            {
                case DoubloonDock.North: { if (d != null) m_NorthDock = (NorthBuccsDock)d; } break;
                case DoubloonDock.South: { if (d != null) m_SouthDock = (SouthBuccsDock)d; } break;
                case DoubloonDock.West: { if (d != null) m_WestDock = (WestBuccsDock)d; } break;
            }
        }

        public static void CannonDestroyed(DoubloonDock dock, DoubloonDockGuard guard)
        {
            switch (dock)
            {
                case DoubloonDock.North: { if (m_NorthGuard != null && !m_NorthGuard.Deleted) { m_NorthGuard.Delete(); } } break;
                case DoubloonDock.South: { if (m_SouthGuard != null && !m_SouthGuard.Deleted) { m_SouthGuard.Delete(); } } break;
                case DoubloonDock.West: { if (m_WestGuard != null && !m_WestGuard.Deleted) { m_WestGuard.Delete(); } } break;
            }

            if (!forceRemove)
            {
                m_Timer[(int)dock] = new BuccsZoneTimer(dock);
                m_Timer[(int)dock].Start();
            }
        }

        public static bool CheckDockMulti(Point3D loc, Map map)
        {
			if( map == null || map == Map.Internal )
				return false;

			Sector sector = map.GetSector( loc.X, loc.Y );

			for ( int i = 0; i < sector.Multis.Count; ++i )
			{
				BaseMulti multi = (BaseMulti)sector.Multis[i];

				if ( multi.Contains( loc ) )
				{
					return true;
				}
			}

			return false;
		}

        public static void AddCombatant(DoubloonDock dock, Mobile m)
        {
            switch (dock)
            {
                case DoubloonDock.North: { if (m_NorthGuard != null) m_NorthGuard.Combatant = m; } break;
                case DoubloonDock.South: { if (m_SouthGuard != null) m_SouthGuard.Combatant = m; } break;
                case DoubloonDock.West: { if (m_WestGuard != null) m_WestGuard.Combatant = m; } break;
            }

            if (m.Guild != null)
                AddCombatantGuild(dock, (Guild)m.Guild);

        }

        public static void AddCombatantGuild(DoubloonDock dock, Guild g)
        {
            switch (dock)
            {
                case (DoubloonDock.North):
{
                        if (!m_NorthCombatantGuilds.Contains(g))
                            m_NorthCombatantGuilds.Add(g);
                    } break;
                case (DoubloonDock.South):
                    {
                        if (!m_SouthCombatantGuilds.Contains(g))
                            m_SouthCombatantGuilds.Add(g);
                    } break;
                case (DoubloonDock.West):
                    {
                        if (!m_WestCombatantGuilds.Contains(g))
                            m_WestCombatantGuilds.Add(g);
                    } break;
            }
        }

        public static void DispurseDoubloons(DoubloonDock dock)
        {

            List<Mobile> mList = GetMobilesInControlZone(dock);
            List<BaseBoat> bList = new List<BaseBoat>();
            List<Guild> gList = new List<Guild>();

            switch (dock)
            {
                case DoubloonDock.North: { gList = m_NorthCombatantGuilds; } break;
                case DoubloonDock.South: { gList = m_SouthCombatantGuilds; } break;
                case DoubloonDock.West: { gList = m_WestCombatantGuilds; } break;

            }

            foreach (Mobile m in mList)
            {
                if (m.Guild != null)
                {
                    if (gList.Contains((Guild)m.Guild))
                    {
                        BaseBoat b = BaseBoat.FindBoatAt(m.Location, m.Map);
                        if (b != null)
                        {
                            if (!bList.Contains(b))
                            {
                                bList.Add(b);
                            }
                        }
                        else
                        {
                            m.SendMessage("You must be in a boat to collect doubloons.");
                        }
                        
                    }
                    else
                    {
                        m.SendMessage(String.Format("{0} is not an ally of the controlling guild.",m.Guild.Name));
                    }
                }
                else
                {
                    m.SendMessage("You must be in a guild to collect doubloons.");
                }
            }

            int boatsToGive = bList.Count;
            if (boatsToGive > 0)
            {
                int each = m_DispersedDoubloons/boatsToGive;
                 
                foreach (BaseBoat b in bList)
                {
                    try
                    {
                        Doubloon d = new Doubloon(each);
                        if (b.Hold.TryDropItem(b.Owner, d, true))
                            SendMessageToAllBoatMembers(b, "Doubloons have just been added into the hold of your ship!");
                        else
                            d.Delete();
                    }
                    catch { }
                }
            }
        }

        public static void RespawnDock(DoubloonDock dock)
        {
            switch (dock)
            {
                case DoubloonDock.North:
                    {
                        if (m_NorthGuard != null)
                            m_NorthGuard.Delete();

                        DoubloonDockGuard g = new DoubloonDockGuard(dock);
                        g.MoveToWorld(m_NorthBuccaneersDockGuardLocation, Map.Felucca);
                        m_NorthGuard = g;
                        m_NorthCombatantGuilds.Clear();
                    }break;
                case DoubloonDock.South:
                    {
                        if (m_SouthGuard != null)
                            m_SouthGuard.Delete();

                        DoubloonDockGuard g = new DoubloonDockGuard(dock);
                        g.MoveToWorld(m_SouthBuccaneersDockGuardLocation, Map.Felucca);
                        m_SouthGuard = g;
                        m_SouthCombatantGuilds.Clear();
                    } break;
                case DoubloonDock.West:
                    {
                        if (m_WestGuard != null)
                            m_WestGuard.Delete();

                        DoubloonDockGuard g = new DoubloonDockGuard(dock);
                        g.MoveToWorld(m_WestBuccaneersDockGuardLocation, Map.Felucca);
                        m_WestGuard = g;
                        m_WestCombatantGuilds.Clear();
                    } break;
            }
        }

        public static void SendMessageToAllBoatMembers(BaseBoat b, string str)
        {
            MultiComponentList mcl = b.Components;

			IPooledEnumerable eable = b.Map.GetObjectsInBounds( new Rectangle2D( b.X + mcl.Min.X, b.Y + mcl.Min.Y, mcl.Width, mcl.Height ) );

			foreach ( object o in eable )
				if ( o is Mobile && b.Contains( (Mobile)o ) )
                    ((Mobile)o).SendMessage(str);

        }

        public static bool BoatHasLivePlayers(BaseBoat b)
        {
            MultiComponentList mcl = b.Components;

            IPooledEnumerable eable = b.Map.GetObjectsInBounds(new Rectangle2D(b.X + mcl.Min.X, b.Y + mcl.Min.Y, mcl.Width, mcl.Height));

            bool result = false;
            foreach (object o in eable)
                if (o is Mobile)
                    if (((Mobile)o).Alive && ((Mobile)o).Player && b.Contains((Mobile)o))
                    {
                        result = true;
                        break;
                    }
            
            eable.Free();
            return result;
        }

        public static bool InControlZone(DoubloonDock dock, Point3D loc)
        {
            switch (dock)
            {
                case (DoubloonDock.North):
                    {
                        if (loc.X > m_NorthBuccaneersControlZone[0] && loc.X < m_NorthBuccaneersControlZone[1] &&
                            loc.Y > m_NorthBuccaneersControlZone[2] && loc.Y < m_NorthBuccaneersControlZone[3])
                        {
                            return true;
                        }
                    }break;
                case (DoubloonDock.South):
                    {
                        if (loc.X > m_SouthBuccaneersControlZone[0] && loc.X < m_SouthBuccaneersControlZone[1] &&
                            loc.Y > m_SouthBuccaneersControlZone[2] && loc.Y < m_SouthBuccaneersControlZone[3])
                        {
                            return true;
                        }
                    } break;
                case (DoubloonDock.West):
                    {
                        if (loc.X > m_WestBuccaneersControlZone[0] && loc.X < m_WestBuccaneersControlZone[1] &&
                            loc.Y > m_WestBuccaneersControlZone[2] && loc.Y < m_WestBuccaneersControlZone[3])
                        {
                            return true;
                        }
                    } break;

            }
            return false;

        }

        public static List<Mobile> GetMobilesInControlZone(DoubloonDock dock)
        {
            List<Mobile> mobiles = new List<Mobile>();
            IPooledEnumerable eable = null;

            switch (dock)
            {
                case DoubloonDock.North: { eable = Map.Felucca.GetMobilesInBounds(new Rectangle2D(new Point2D(m_NorthBuccaneersControlZone[0], m_NorthBuccaneersControlZone[2]), new Point2D(m_NorthBuccaneersControlZone[1], m_NorthBuccaneersControlZone[3]))); } break;
                case DoubloonDock.South: { eable = Map.Felucca.GetMobilesInBounds(new Rectangle2D(new Point2D(m_SouthBuccaneersControlZone[0], m_SouthBuccaneersControlZone[2]), new Point2D(m_SouthBuccaneersControlZone[1], m_SouthBuccaneersControlZone[3]))); } break;
                case DoubloonDock.West: { eable = Map.Felucca.GetMobilesInBounds(new Rectangle2D(new Point2D(m_WestBuccaneersControlZone[0], m_WestBuccaneersControlZone[2]), new Point2D(m_WestBuccaneersControlZone[1], m_WestBuccaneersControlZone[3]))); } break;
            }

            foreach (object o in eable)
            {
                if (o != null)
                    if (o is Mobile)
                        if (((Mobile)o).Player && ((Mobile)o).Alive && !mobiles.Contains((Mobile)o))
                            mobiles.Add((Mobile)o);

            }
            eable.Free();

            return mobiles;
        }

    }

    class BuccsZoneTimer : Timer
    {
        private DoubloonDock m_dock;
        private DateTime m_endTime;
        private DateTime m_nextDoubloonDispurseTime;
        private DateTime m_startTime;

        public BuccsZoneTimer(DoubloonDock dock)
            : base(TimeSpan.Zero, TimeSpan.FromSeconds(1))
        {
            m_dock = dock;
            m_endTime = DateTime.UtcNow + TimeSpan.FromMinutes(60);
            m_nextDoubloonDispurseTime = DateTime.UtcNow;
            m_startTime = DateTime.UtcNow + DoubloonDocks.m_InitialControlTime;
            
        }

        protected override void OnTick()
        {
            if (DateTime.UtcNow > m_endTime)
            {
                this.Stop();
                DoubloonDocks.RespawnDock(m_dock);
            }

            if (DateTime.UtcNow > m_startTime)
            {
                bool inZone = false;
                List<Mobile> mList = DoubloonDocks.GetMobilesInControlZone(m_dock);

                switch (m_dock)
                {
                    case DoubloonDock.North:
                        {
                            foreach (Mobile m in mList)
                                if (m != null)
                                    if (DoubloonDocks.m_NorthCombatantGuilds.Contains((Guild)m.Guild)) { inZone = true; break; }

                        } break;
                    case DoubloonDock.South:
                        {
                            foreach (Mobile m in mList)
                                if (m != null)
                                    if (DoubloonDocks.m_SouthCombatantGuilds.Contains((Guild)m.Guild)) { inZone = true; break; }

                        } break;
                    case DoubloonDock.West:
                        {
                            foreach (Mobile m in mList)
                                if (m != null)
                                    if (DoubloonDocks.m_WestCombatantGuilds.Contains((Guild)m.Guild)) { inZone = true; break; }

                        } break;
                }

                if (inZone == false)
                {
                    this.Stop();
                    DoubloonDocks.RespawnDock(m_dock);
                }

                if (DateTime.UtcNow > m_nextDoubloonDispurseTime)
                {
                    DoubloonDocks.DispurseDoubloons(m_dock);
                    m_nextDoubloonDispurseTime = DateTime.UtcNow + DoubloonDocks.m_DoubloonDispurseRate;
                }
            }
        }
    }
}

