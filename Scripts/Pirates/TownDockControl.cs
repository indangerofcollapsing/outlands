/***************************************************************************
 *                            TownDockControl.cs
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

namespace Server.Custom.Pirates
{
    public enum Towns
    {
        Britain, Trinsic
    };

   

    static class TownDockControl
    {
        #region Spawn Commands
        public static void Initialize()
        {
            //CommandSystem.Register("SpawnCannons", AccessLevel.GameMaster, new CommandEventHandler(SpawnCannons_OnCommand));
        }

        [Usage("SpawnCannons")]
        [Description("SpawnCannons")]
        public static void SpawnCannons_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            if (e.Length == 0)
            {
                SpawnCannons(Towns.Britain);
            }
            else
            {
                e.Mobile.SendMessage(0x25, "Bad Format: YewJail");
            }
        }
        #endregion

        #region Town Cannon Spawn Locations
        private static List<Point3D> m_BritainCannonLocations = new List<Point3D> 
        { 
            //
        };

        private static List<Point3D> m_TrinsicCannonLocations = new List<Point3D> 
        { 
            new Point3D(2757,2166,-2), new Point3D(2751,2158,-2), new Point3D(2751,2175,-2)
        };

        #endregion

        private static Point3D m_SouthBucaneersDockLocation = new Point3D(2704, 2332, 2);
        private static Point3D m_WestBuccaneersDockLocation = new Point3D(2603, 2201, -2);
        private static Point3D m_NorthBuccaneersDockLocation = new Point3D(2742, 1937, 0);
        
        #region Town Control Zones

        //xmin, xmax, ymin, ymax
        private static int[] m_BritainControlZone = new int[] { 1458, 1508, 1741, 1781 };
        private static int[] m_TrinsicControlZone = new int[] {};

        #endregion


        private static Dictionary<Point3D, TurnableCannonGuard> m_BritainCannonGuards = new Dictionary<Point3D, TurnableCannonGuard>();
        private static Dictionary<Point3D, TurnableCannonGuard> m_TrinsicCannonGuards = new Dictionary<Point3D, TurnableCannonGuard>();

        public static List<BaseBoat> m_BritainCombatantShips = new List<BaseBoat>();

        public static void SpawnCannons(Towns town)
        {
            switch (town)
            {
                case Towns.Britain:
                    {
                        foreach (Point3D p in m_BritainCannonLocations)
                        {
                            if (!m_BritainCannonGuards.ContainsKey(p))
                            {
                                TurnableCannonGuard g = new TurnableCannonGuard(Towns.Britain);
                                m_BritainCannonGuards.Add(p, g);
                                g.MoveToWorld(p, Map.Felucca);
                            }
                        }

                    }break;

                case Towns.Trinsic:
                    {
                        foreach (Point3D p in m_TrinsicCannonLocations)
                        {
                            if (!m_TrinsicCannonGuards.ContainsKey(p))
                            {
                                TurnableCannonGuard g = new TurnableCannonGuard(Towns.Trinsic);
                                m_TrinsicCannonGuards.Add(p, g);
                                g.MoveToWorld(p, Map.Felucca);
                            }
                        }

                    } break;

            }
        }

        public static void RegisterTownCannon(Towns town, Point3D p, TurnableCannonGuard g)
        {
            switch (town)
            {
                case Towns.Britain: { if (!m_BritainCannonGuards.ContainsKey(p)) m_BritainCannonGuards.Add(p, g); }break;
                case Towns.Trinsic: { if (!m_TrinsicCannonGuards.ContainsKey(p)) m_TrinsicCannonGuards.Add(p, g); } break;
            }

        }

        public static void CannonDestroyed(Towns town, TurnableCannonGuard guard)
        {
            switch (town)
            {
                case Towns.Britain:
                    {
                        if (m_BritainCannonGuards.ContainsKey(guard.Location))
                        {
                            m_BritainCannonGuards.Remove(guard.Location);
                            if (m_BritainCannonGuards.Count == 0) 
                                AllCannonsDestroyed(Towns.Britain);
                        }
                    }break;
                case Towns.Trinsic:
                    {
                        if (m_TrinsicCannonGuards.ContainsKey(guard.Location))
                        {
                            m_TrinsicCannonGuards.Remove(guard.Location);
                            if (m_TrinsicCannonGuards.Count == 0)
                                AllCannonsDestroyed(Towns.Trinsic);
                        }
                    } break;
            } 
        }

        public static BaseGuildDock CheckDockMulti(Point3D loc, Map map)
        {
			if( map == null || map == Map.Internal )
				return null;

			Sector sector = map.GetSector( loc.X, loc.Y );

			for ( int i = 0; i < sector.Multis.Count; ++i )
			{
				BaseMulti multi = (BaseMulti)sector.Multis[i];

				if ( multi is BaseGuildDock && multi.Contains( loc ) )
				{
					return ((BaseGuildDock)multi);
				}
			}

			return null;
		}

        public static ControlZoneTimer m_Timer;

        public static void AllCannonsDestroyed(Towns town)
        {
            switch (town)
            {
                case Towns.Britain:
                    {
                        m_Timer = new ControlZoneTimer(Towns.Britain);
                        m_Timer.Start();
                    } break;
                case Towns.Trinsic:
                    {
                        m_Timer = new ControlZoneTimer(Towns.Trinsic);
                        m_Timer.Start();
                    } break;
            }

        }

        public static void AddCombatant(Towns town, Mobile m)
        {
            switch (town)
            {
                case Towns.Britain:
                    {
                        foreach (TurnableCannonGuard g in m_BritainCannonGuards.Values)
                        {
                            g.Combatant = m;
                        }

                        BaseBoat b = BaseBoat.FindBoatAt(m.Location, m.Map);
                        if (b != null)
                        {
                            AddCombatantShip(Towns.Britain, b);
                        }
                    }break;

            }

        }

        public static void AddCombatantShip(Towns town, BaseBoat b)
        {
            switch (town)
            {
                case (Towns.Britain):
                    {
                        if (!m_BritainCombatantShips.Contains(b))
                            m_BritainCombatantShips.Add(b);

                    }break;

            }

        }

        public static void SendMessageToAllBoatMembers(BaseBoat b, string str)
        {
            MultiComponentList mcl = b.Components;

			IPooledEnumerable eable = b.Map.GetObjectsInBounds( new Rectangle2D( b.X + mcl.Min.X, b.Y + mcl.Min.Y, mcl.Width, mcl.Height ) );

			foreach ( object o in eable )
				if ( o is Mobile && b.Contains( (Mobile)o ) )
                    ((Mobile)o).SendMessage(str);

            eable.Free();
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

        public static bool InControlZone(Towns town, Point3D loc)
        {
            switch (town)
            {
                case (Towns.Britain):
                    {
                        if (loc.X > m_BritainControlZone[0] && loc.X < m_BritainControlZone[1] && 
                            loc.Y > m_BritainControlZone[2] && loc.Y < m_BritainControlZone[3])
                        {
                            return true;
                        }
                    }break;

            }
            return false;

        }
    }

    class ControlZoneTimer : Timer
    {
        private Towns m_Town;

        public ControlZoneTimer(Towns town)
            : base(TimeSpan.Zero, TimeSpan.FromSeconds(1))
        {
            m_Town = town;
        }

        protected override void OnTick()
        {
            switch (m_Town)
            {
                case Towns.Britain:
                    {
                        bool inZone = false;
                        foreach (BaseBoat b in TownDockControl.m_BritainCombatantShips)
                        {
                            if (TownDockControl.InControlZone(Towns.Britain, b.Location) && b.HitPoints > 0 && TownDockControl.BoatHasLivePlayers(b))
                            {
                                inZone = true;
                                TownDockControl.SendMessageToAllBoatMembers(b, "You are looting the town's treasury!");
                            }
                        }
                        if (inZone == false)
                        {
                            this.Stop();
                            TownDockControl.SpawnCannons(Towns.Britain);
                        }


                    } break;

            }
        }
    }
}

