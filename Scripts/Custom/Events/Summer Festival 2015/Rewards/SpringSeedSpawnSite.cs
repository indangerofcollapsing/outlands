using System;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
using Server.Targeting;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
    public class SpringSeedSpawnSite : Item
    {
        public List<Point3D> m_WildGrowthPoints = new List<Point3D>();
        public List<Point3D> m_SpawnablePoints = new List<Point3D>();

        private DateTime m_Expiration;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime Expiration
        {
            get { return m_Expiration; }
            set { m_Expiration = value; }
        }

        public List<Item> m_Items = new List<Item>();
        public List<Mobile> m_Mobiles = new List<Mobile>();

        public static TimeSpan Duration = TimeSpan.FromHours(2);

        private Timer m_Timer;    

        [Constructable]
        public SpringSeedSpawnSite(): base(3387)
        {
            Name = "spring seed spawn site";

            Visible = false;
        }

        public void BeginSpawn(Mobile from)
        {
            Expiration = DateTime.UtcNow + Duration;

            m_Timer = new InternalTimer(this);
            m_Timer.Start();

            Direction direction = Utility.GetDirection(from.Location, Location);

            from.Direction = direction;

            int range = SpringSeed.Radius;

            int minRange = -1 * range;
            int maxRange = range + 1;

            bool validSpawnArea = true;

            for (int a = minRange; a < maxRange; a++)
            {
                for (int b = minRange; b < maxRange; b++)
                {
                    Point3D newLocation = new Point3D(Location.X + a, Location.Y + b, Location.Z);
                    SpellHelper.AdjustField(ref newLocation, Map, 12, false);

                    if (Map.CanSpawnMobile(newLocation))
                        m_SpawnablePoints.Add(newLocation);

                    if (Utility.RandomDouble() <= .75)
                        m_WildGrowthPoints.Add(newLocation);
                }
            }

            Timer.DelayCall(TimeSpan.FromSeconds(.25), delegate
            {
                if (from != null)
                    from.Animate(17, 5, 1, true, false, 0);

                from.SendMessage("You plant the seed into the ground.");

                Effects.SendLocationEffect(Location, Map, 0xCC6, 100, 0);
                Effects.PlaySound(Location, Map, 0x4);
            });

            int totalPoints = m_WildGrowthPoints.Count;

            Point3D location = Location;
            Map map = Map;

            for (int a = 0; a < totalPoints; a++)
            {
                double delay = 1 + (.05 * a);               

                Timer.DelayCall(TimeSpan.FromSeconds(delay), delegate
                {
                    if (this == null) return;
                    if (Deleted) return;

                    int index = Utility.RandomMinMax(0, m_WildGrowthPoints.Count - 1);

                    if (index > m_WildGrowthPoints.Count - 1)
                        return;

                    Point3D point = m_WildGrowthPoints[index];
                    m_WildGrowthPoints.RemoveAt(index);

                    int itemId = Utility.RandomList(3219, 3220, 3255, 3256, 3152, 3153, 3223, 6809, 6811, 3204, 3247, 3248, 3254, 3258, 3259, 3378,
                                3267, 3237, 3267, 9036, 3239, 3208, 3307, 3310, 3311, 3313, 3314, 3332, 3271, 3212, 3213);

                    TimedStatic meadowItem = new TimedStatic(itemId, Duration.TotalSeconds);
                    meadowItem.Name = "wild growth";
                    meadowItem.MoveToWorld(point, map);

                    m_Items.Add(meadowItem);

                    Effects.PlaySound(point, map, 0x1BB);
                });
            }

            double spawnDelay = 1 + (totalPoints * .05);

            Timer.DelayCall(TimeSpan.FromSeconds(spawnDelay), delegate
            {
                if (this == null) return;
                if (Deleted) return;

                SpawnWave(location, map, range);
            });
        }

        public void SpawnWave(Point3D location, Map map, int range)
        {
            for (int a = 0; a < 4; a++)
            {
                switch (Utility.RandomMinMax(1, 5))
                {
                    case 1:
                        Spawn(typeof(EarthlyTendril));
                        Spawn(typeof(EarthlyTendril));
                        Spawn(typeof(EarthlyTendril));
                    break;

                    case 2:
                        Spawn(typeof(ArborealMyconid));
                        Spawn(typeof(ArborealMyconid));
                    break;

                    case 3:
                        Spawn(typeof(WildOne));
                    break;

                    case 4:
                        Spawn(typeof(TreeStalker));
                    break;

                    case 5:
                        Spawn(typeof(Ent));
                    break;
                }
            }

            Spawn(typeof(TreeOfLife));

            Effects.PlaySound(Location, Map, 0x64D);
        }

        public void Spawn(Type type)
        {
            Point3D spawnLocation;

            if (m_SpawnablePoints.Count > 0)            
                spawnLocation = m_SpawnablePoints[Utility.RandomMinMax(0, m_SpawnablePoints.Count - 1)];
            else
                spawnLocation = Location;

            BaseCreature bc_Creature = (BaseCreature)Activator.CreateInstance(type);

            if (bc_Creature != null)
            {
                bc_Creature.MoveToWorld(spawnLocation, Map);
                bc_Creature.PlaySound(bc_Creature.AngerSound);
            }            
        }

        public SpringSeedSpawnSite(Serial serial): base(serial)
        {
        }

        private class InternalTimer : Timer
        {
            private SpringSeedSpawnSite m_SpringSeedSpawnSite;

            public InternalTimer(SpringSeedSpawnSite springSeedSpawnSite): base(TimeSpan.Zero, TimeSpan.FromMinutes(1))
            {
                Priority = TimerPriority.OneMinute;

                m_SpringSeedSpawnSite = springSeedSpawnSite;
            }

            protected override void OnTick()
            {
                if (m_SpringSeedSpawnSite == null)
                {
                    Stop();

                    return;
                }

                if (m_SpringSeedSpawnSite.Deleted)
                {
                    Stop();

                    return;
                }

                if (m_SpringSeedSpawnSite.m_Expiration < DateTime.UtcNow)
                {
                    Stop();
                    m_SpringSeedSpawnSite.Delete();

                    return;
                }
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            for (int a = 0; a < m_Items.Count; ++a)
            {
                if (m_Items[a] != null)
                {
                    if (!m_Items[a].Deleted)
                        m_Items[a].Delete();
                }
            }

            for (int a = 0; a < m_Mobiles.Count; ++a)
            {
                if (m_Mobiles[a] != null)
                {
                    if (!m_Mobiles[a].Deleted)
                        m_Mobiles[a].Delete();
                }
            }

            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }

            Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write(m_Expiration);

            writer.Write((int)m_Items.Count);
            foreach (Item item in m_Items)
            {
                writer.Write(item);
            }

            writer.Write((int)m_Mobiles.Count);
            foreach (Mobile mobile in m_Mobiles)
            {
                writer.Write(mobile);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Expiration = reader.ReadDateTime();

            int itemsCount = reader.ReadInt();
            for (int i = 0; i < itemsCount; ++i)
            {
                m_Items.Add(reader.ReadItem());
            }

            int mobileCount = reader.ReadInt();
            for (int i = 0; i < mobileCount; ++i)
            {
                m_Mobiles.Add(reader.ReadMobile());
            }

            //------

            m_Timer = new InternalTimer(this);
            m_Timer.Start();
        }
    }
}