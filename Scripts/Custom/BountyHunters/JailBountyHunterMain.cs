using System;
using System.Collections.Generic;
using System.Collections;
using Server.Mobiles;
using Server.YewJail;

namespace Server.Custom
{
    static class JailBountyHunterControl
    {
        private static int m_DistanceFromTarget = 12;
        private static Server.PlayerDeathEventHandler DeathEventHandler = new PlayerDeathEventHandler(EventSink_PlayerDeath);
        private static double m_ChanceToSpawn = 0.0;
        private static List<JailBountyHunter> _hunters = new List<JailBountyHunter>();

        public static void SpawnBountyHunters(Mobile target)
        {
            SpawnBountyHunters(target, 1, 3, m_ChanceToSpawn);
        }

        public static void SpawnBountyHunters(Mobile target, double chance)
        {
            SpawnBountyHunters(target, 1, 3, chance);
        }

        public static void SpawnBountyHunters(Mobile target, int Min, int Max, double Chance)
        {
            if (Utility.RandomDouble() < Chance)
            {
                if (SpaceAvailableInJail() && target is PlayerMobile && !((PlayerMobile)target).YewJailed)
                {
                    int noSpawn = Utility.RandomMinMax(Min, Max);
                    List<Point3D> spawnLocation = GenerateSpawnLocations(noSpawn, target);
                    for (int i = 0; i < noSpawn; i++)
                    {
                        JailBountyHunter newBH = new JailBountyHunter(target);
                        newBH.MoveToWorld(spawnLocation[i], target.Map);
                        newBH.Combatant = target;
                        newBH.Warmode = true;
                        newBH.Attack(target);
                        AddHunter(newBH);
                    }
                }
            }
        }

        public static void AddHunter(JailBountyHunter m)
        {
            _hunters.Add(m);

            if (_hunters.Count == 1)
                EventSink.PlayerDeath += DeathEventHandler;
        }

        public static void RemoveHunter(JailBountyHunter m)
        {
            if (_hunters.Contains(m))
            {
                _hunters.Remove(m);

                if (_hunters.Count == 0)
                    EventSink.PlayerDeath -= DeathEventHandler;
            }
        }

        public static void EventSink_PlayerDeath(PlayerDeathEventArgs e)
        {
            Mobile m = e.Mobile;
            if (m.Player && m.ShortTermMurders > 0 && m.LastKiller is JailBountyHunter) {
                RegisterHunterKill(m);
            }
        }

        public static void RegisterHunterKill(Mobile m) 
        {
            if (m.Player && m.ShortTermMurders > 0 ) {
                m.LastKiller = null;
                Custom.Detective.OnBountyHunterKill(m);
                YewJailControl.NewJailedTarget(m);
            }
        }

        private static bool SpaceAvailableInJail()
        {
            bool m_found = false;
            Int32 i = 1;
            while (m_found == false && i <= 20)
            {
                if (!YewJailControl.m_OccupiedCell.Contains(i++))
                    m_found = true;
            }
            return m_found;
        }

        private static List<Point3D> GenerateSpawnLocations(int noSpawns ,Mobile target)
        {
            List <Point3D> spawnlocs = new List<Point3D>();
	    if (target == null)
                return spawnlocs;

            Point3D targetLoc = target.Location;

            for (int i = 1; i <= noSpawns; i++)
            {
                double angle = 6.28/noSpawns;
                double x = m_DistanceFromTarget*Math.Cos(angle*i);
                double y = m_DistanceFromTarget*Math.Sin(angle*i);
                int X = (target.X + (int)x);
                int Y = (target.Y + (int)y);
                int Z = target.Map.GetAverageZ(X, Y);

                if (target.Map.CanFit(X, Y, Z, 16, false, false))
                {
                    spawnlocs.Add(new Point3D(X, Y, Z));
                }
                else
                {
                    bool done = false;
                    int j = Math.Abs((int)x); 
                    while (done == false && j >= 0)
                    {
                        X = target.X + (int)(Math.Sign(x) * j);

                        for (int k = Math.Abs((int)y); k >= 0; k--)
                        {
                            Y = target.Y + (int)(Math.Sign(y) * k);
                            if (target.Map.CanFit(X, Y, Z, 16, false, false))
                            {
                                spawnlocs.Add(new Point3D(X, Y, Z));
                                done = true;
                                break;
                            }
                        }
                        j--;

                        if (done == false)
                            spawnlocs.Add(new Point3D(target.X, target.Y, target.Z));
                    }

                }
            }
            return spawnlocs;
        }
    }

   
}
