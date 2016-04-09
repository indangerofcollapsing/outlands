using System;
using Server;
using System.Collections;
using System.Collections.Generic;
using Server.Commands;
using Server.Network;
using Server.Prompts;
using Server.Multis;
using Server.Mobiles;
using Server.Targeting;
using Server.Gumps;
using Server.Custom;

namespace Server.Items
{
    public class FishingSpotPersistance
    {
        public static List<FishingSpotSpawner> m_FishingSpotSpawners = new List<FishingSpotSpawner>();

        public static void Initialize()
        {
            CommandSystem.Register("SpawnAllFishingSpotSpawners", AccessLevel.GameMaster, new CommandEventHandler(SpawnAllFishingSpotSpawners));
            CommandSystem.Register("DeleteAllFishingSpotSpawners", AccessLevel.GameMaster, new CommandEventHandler(DeleteAllFishingSpotSpawners));

            CommandSystem.Register("DeleteAllFishingSpots", AccessLevel.GameMaster, new CommandEventHandler(DeleteAllFishingSpots));
        }

        [Usage("SpawnAllFishingSpots")]
        [Description("Spawns All Pre-Determined Fishing Spot Spawners")]
        public static void SpawnAllFishingSpotSpawners(CommandEventArgs arg)
        {
            PlayerMobile pm_Mobile = arg.Mobile as PlayerMobile;

            if (pm_Mobile == null)
                return;

            int fishingSpotSpawners = m_FishingSpotSpawners.Count;

            for (int a = 0; a < fishingSpotSpawners; a++)
            {
                if (m_FishingSpotSpawners[0] != null)
                    m_FishingSpotSpawners[0].Delete();
            }

            #region Pre-Determined Fishing Spot Spawner Locations

            int minSpawnTime = 120;
            int maxSpawnTime = 240;

            FishingSpotSpawner fishingSpotSpawner;

            fishingSpotSpawner = new FishingSpotSpawner();
            fishingSpotSpawner.MinSpawnTime = minSpawnTime;
            fishingSpotSpawner.MaxSpawnTime = maxSpawnTime;
            fishingSpotSpawner.InstanceCount = 2;
            fishingSpotSpawner.SpawnRange = 400;
            fishingSpotSpawner.MoveToWorld(new Point3D(1875, 2027, 0), pm_Mobile.Map);

            fishingSpotSpawner = new FishingSpotSpawner();
            fishingSpotSpawner.MinSpawnTime = minSpawnTime;
            fishingSpotSpawner.MaxSpawnTime = maxSpawnTime;
            fishingSpotSpawner.InstanceCount = 2;
            fishingSpotSpawner.SpawnRange = 200;
            fishingSpotSpawner.MoveToWorld(new Point3D(2120, 2418, 0), pm_Mobile.Map);

            fishingSpotSpawner = new FishingSpotSpawner();
            fishingSpotSpawner.MinSpawnTime = minSpawnTime;
            fishingSpotSpawner.MaxSpawnTime = maxSpawnTime;
            fishingSpotSpawner.InstanceCount = 2;
            fishingSpotSpawner.SpawnRange = 200;
            fishingSpotSpawner.MoveToWorld(new Point3D(2199, 3245, 0), pm_Mobile.Map);

            fishingSpotSpawner = new FishingSpotSpawner();
            fishingSpotSpawner.MinSpawnTime = minSpawnTime;
            fishingSpotSpawner.MaxSpawnTime = maxSpawnTime;
            fishingSpotSpawner.InstanceCount = 2;
            fishingSpotSpawner.SpawnRange = 200;
            fishingSpotSpawner.MoveToWorld(new Point3D(2152, 3698, 0), pm_Mobile.Map);

            fishingSpotSpawner = new FishingSpotSpawner();
            fishingSpotSpawner.MinSpawnTime = minSpawnTime;
            fishingSpotSpawner.MaxSpawnTime = maxSpawnTime;
            fishingSpotSpawner.InstanceCount = 2;
            fishingSpotSpawner.SpawnRange = 200;
            fishingSpotSpawner.MoveToWorld(new Point3D(1364, 3364, 0), pm_Mobile.Map);

            fishingSpotSpawner = new FishingSpotSpawner();
            fishingSpotSpawner.MinSpawnTime = minSpawnTime;
            fishingSpotSpawner.MaxSpawnTime = maxSpawnTime;
            fishingSpotSpawner.InstanceCount = 2;
            fishingSpotSpawner.SpawnRange = 200;
            fishingSpotSpawner.MoveToWorld(new Point3D(855, 2681, 0), pm_Mobile.Map);

            fishingSpotSpawner = new FishingSpotSpawner();
            fishingSpotSpawner.MinSpawnTime = minSpawnTime;
            fishingSpotSpawner.MaxSpawnTime = maxSpawnTime;
            fishingSpotSpawner.InstanceCount = 2;
            fishingSpotSpawner.SpawnRange = 200;
            fishingSpotSpawner.MoveToWorld(new Point3D(562, 1812, 0), pm_Mobile.Map);

            fishingSpotSpawner = new FishingSpotSpawner();
            fishingSpotSpawner.MinSpawnTime = minSpawnTime;
            fishingSpotSpawner.MaxSpawnTime = maxSpawnTime;
            fishingSpotSpawner.InstanceCount = 1;
            fishingSpotSpawner.SpawnRange = 150;
            fishingSpotSpawner.MoveToWorld(new Point3D(95, 1430, 0), pm_Mobile.Map);

            fishingSpotSpawner = new FishingSpotSpawner();
            fishingSpotSpawner.MinSpawnTime = minSpawnTime;
            fishingSpotSpawner.MaxSpawnTime = maxSpawnTime;
            fishingSpotSpawner.InstanceCount = 2;
            fishingSpotSpawner.SpawnRange = 200;
            fishingSpotSpawner.MoveToWorld(new Point3D(750, 470, 0), pm_Mobile.Map);

            fishingSpotSpawner = new FishingSpotSpawner();
            fishingSpotSpawner.MinSpawnTime = minSpawnTime;
            fishingSpotSpawner.MaxSpawnTime = maxSpawnTime;
            fishingSpotSpawner.InstanceCount = 3;
            fishingSpotSpawner.SpawnRange = 400;
            fishingSpotSpawner.MoveToWorld(new Point3D(1395, 479, 0), pm_Mobile.Map);

            fishingSpotSpawner = new FishingSpotSpawner();
            fishingSpotSpawner.MinSpawnTime = minSpawnTime;
            fishingSpotSpawner.MaxSpawnTime = maxSpawnTime;
            fishingSpotSpawner.InstanceCount = 1;
            fishingSpotSpawner.SpawnRange = 150;
            fishingSpotSpawner.MoveToWorld(new Point3D(2225, 165, 0), pm_Mobile.Map);

            fishingSpotSpawner = new FishingSpotSpawner();
            fishingSpotSpawner.MinSpawnTime = minSpawnTime;
            fishingSpotSpawner.MaxSpawnTime = maxSpawnTime;
            fishingSpotSpawner.InstanceCount = 2;
            fishingSpotSpawner.SpawnRange = 400;
            fishingSpotSpawner.MoveToWorld(new Point3D(2953, 235, 0), pm_Mobile.Map);

            fishingSpotSpawner = new FishingSpotSpawner();
            fishingSpotSpawner.MinSpawnTime = minSpawnTime;
            fishingSpotSpawner.MaxSpawnTime = maxSpawnTime;
            fishingSpotSpawner.InstanceCount = 2;
            fishingSpotSpawner.SpawnRange = 200;
            fishingSpotSpawner.MoveToWorld(new Point3D(3415, 576, 0), pm_Mobile.Map);

            fishingSpotSpawner = new FishingSpotSpawner();
            fishingSpotSpawner.MinSpawnTime = minSpawnTime;
            fishingSpotSpawner.MaxSpawnTime = maxSpawnTime;
            fishingSpotSpawner.InstanceCount = 2;
            fishingSpotSpawner.SpawnRange = 200;
            fishingSpotSpawner.MoveToWorld(new Point3D(2639, 1289, 0), pm_Mobile.Map);

            fishingSpotSpawner = new FishingSpotSpawner();
            fishingSpotSpawner.MinSpawnTime = minSpawnTime;
            fishingSpotSpawner.MaxSpawnTime = maxSpawnTime;
            fishingSpotSpawner.InstanceCount = 3;
            fishingSpotSpawner.SpawnRange = 600;
            fishingSpotSpawner.MoveToWorld(new Point3D(2765, 1532, 0), pm_Mobile.Map);

            fishingSpotSpawner = new FishingSpotSpawner();
            fishingSpotSpawner.MinSpawnTime = minSpawnTime;
            fishingSpotSpawner.MaxSpawnTime = maxSpawnTime;
            fishingSpotSpawner.InstanceCount = 3;
            fishingSpotSpawner.SpawnRange = 500;
            fishingSpotSpawner.MoveToWorld(new Point3D(2953, 2533, 0), pm_Mobile.Map);

            fishingSpotSpawner = new FishingSpotSpawner();
            fishingSpotSpawner.MinSpawnTime = minSpawnTime;
            fishingSpotSpawner.MaxSpawnTime = maxSpawnTime;
            fishingSpotSpawner.InstanceCount = 1;
            fishingSpotSpawner.SpawnRange = 400;
            fishingSpotSpawner.MoveToWorld(new Point3D(4061, 655, 0), pm_Mobile.Map);

            fishingSpotSpawner = new FishingSpotSpawner();
            fishingSpotSpawner.MinSpawnTime = minSpawnTime;
            fishingSpotSpawner.MaxSpawnTime = maxSpawnTime;
            fishingSpotSpawner.InstanceCount = 1;
            fishingSpotSpawner.SpawnRange = 400;
            fishingSpotSpawner.MoveToWorld(new Point3D(4146, 275, 0), pm_Mobile.Map);

            fishingSpotSpawner = new FishingSpotSpawner();
            fishingSpotSpawner.MinSpawnTime = minSpawnTime;
            fishingSpotSpawner.MaxSpawnTime = maxSpawnTime;
            fishingSpotSpawner.InstanceCount = 1;
            fishingSpotSpawner.SpawnRange = 300;
            fishingSpotSpawner.MoveToWorld(new Point3D(2197, 1360, 0), pm_Mobile.Map);

            fishingSpotSpawner = new FishingSpotSpawner();
            fishingSpotSpawner.MinSpawnTime = minSpawnTime;
            fishingSpotSpawner.MaxSpawnTime = maxSpawnTime;
            fishingSpotSpawner.InstanceCount = 3;
            fishingSpotSpawner.SpawnRange = 600;
            fishingSpotSpawner.MoveToWorld(new Point3D(4727, 380, 0), pm_Mobile.Map);

            fishingSpotSpawner = new FishingSpotSpawner();
            fishingSpotSpawner.MinSpawnTime = minSpawnTime;
            fishingSpotSpawner.MaxSpawnTime = maxSpawnTime;
            fishingSpotSpawner.InstanceCount = 4;
            fishingSpotSpawner.SpawnRange = 800;
            fishingSpotSpawner.MoveToWorld(new Point3D(4563, 2351, 0), pm_Mobile.Map);

            fishingSpotSpawner = new FishingSpotSpawner();
            fishingSpotSpawner.MinSpawnTime = minSpawnTime;
            fishingSpotSpawner.MaxSpawnTime = maxSpawnTime;
            fishingSpotSpawner.InstanceCount = 3;
            fishingSpotSpawner.SpawnRange = 600;
            fishingSpotSpawner.MoveToWorld(new Point3D(4302, 3500, 0), pm_Mobile.Map);

            fishingSpotSpawner = new FishingSpotSpawner();
            fishingSpotSpawner.MinSpawnTime = minSpawnTime;
            fishingSpotSpawner.MaxSpawnTime = maxSpawnTime;
            fishingSpotSpawner.InstanceCount = 4;
            fishingSpotSpawner.SpawnRange = 800;
            fishingSpotSpawner.MoveToWorld(new Point3D(502, 3100, 0), pm_Mobile.Map);

            fishingSpotSpawner = new FishingSpotSpawner();
            fishingSpotSpawner.MinSpawnTime = minSpawnTime;
            fishingSpotSpawner.MaxSpawnTime = maxSpawnTime;
            fishingSpotSpawner.InstanceCount = 3;
            fishingSpotSpawner.SpawnRange = 500;
            fishingSpotSpawner.MoveToWorld(new Point3D(52, 1600, 0), pm_Mobile.Map);

            fishingSpotSpawner = new FishingSpotSpawner();
            fishingSpotSpawner.MinSpawnTime = minSpawnTime;
            fishingSpotSpawner.MaxSpawnTime = maxSpawnTime;
            fishingSpotSpawner.InstanceCount = 3;
            fishingSpotSpawner.SpawnRange = 500;
            fishingSpotSpawner.MoveToWorld(new Point3D(2352, 4000, 0), pm_Mobile.Map);

            fishingSpotSpawner = new FishingSpotSpawner();
            fishingSpotSpawner.MinSpawnTime = minSpawnTime;
            fishingSpotSpawner.MaxSpawnTime = maxSpawnTime;
            fishingSpotSpawner.InstanceCount = 3;
            fishingSpotSpawner.SpawnRange = 600;
            fishingSpotSpawner.MoveToWorld(new Point3D(3902, 1600, 0), pm_Mobile.Map);

            #endregion

            pm_Mobile.SendMessage("Spawning All Fishing Spot Spawners");
        }

        [Usage("DeleteAllFishingSpotSpawners")]
        [Description("Deletes All Fishing Spot Spawners")]
        public static void DeleteAllFishingSpotSpawners(CommandEventArgs arg)
        {
            PlayerMobile pm_Mobile = arg.Mobile as PlayerMobile;

            if (pm_Mobile == null)
                return;

            int fishingSpotSpawners = m_FishingSpotSpawners.Count;

            for (int a = 0; a < fishingSpotSpawners; a++)
            {
                if (m_FishingSpotSpawners[0] != null)
                    m_FishingSpotSpawners[0].Delete();
            }

            pm_Mobile.SendMessage("Deleted All Fishing Spot Spawners");
        }

        [Usage("DeleteAllFishingSpots")]
        [Description("Deletes All Fishing Spots")]
        public static void DeleteAllFishingSpots(CommandEventArgs arg)
        {
            PlayerMobile pm_Mobile = arg.Mobile as PlayerMobile;

            if (pm_Mobile == null)
                return;

            int fishingSpotCount = FishingSpot.m_Instances.Count;
            int deleteCount = 0;
            {
                for (int a = 0; a < fishingSpotCount; a++)
                {
                    int index = a - deleteCount;

                    FishingSpot fishingSpot = FishingSpot.m_Instances[index];

                    FishingSpot.m_Instances.RemoveAt(index);
                    fishingSpot.Delete();
                    deleteCount++;                    
                }
            }
            
            pm_Mobile.SendMessage("Deleted All Fishing Spot Spawners");
        }
    }

    public class FishingSpotSpawner : Item
    {
        private bool m_Activated = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Activated
        {
            get { return m_Activated; }
            set { m_Activated = value; }
        }

        private int m_InstanceCount = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int InstanceCount
        {
            get { return m_InstanceCount; }
            set { m_InstanceCount = value; }
        }

        private bool m_SpawnAllAvailable = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool SpawnAllAvailable
        {
            get { return m_SpawnAllAvailable; }
            set { m_SpawnAllAvailable = value; }
        }

        private int m_HomeRange = 10;
        [CommandProperty(AccessLevel.GameMaster)]
        public int HomeRange
        {
            get { return m_HomeRange; }
            set { m_HomeRange = value; }
        }

        private int m_SpawnRange = 10;
        [CommandProperty(AccessLevel.GameMaster)]
        public int SpawnRange
        {
            get { return m_SpawnRange; }
            set { m_SpawnRange = value; }
        }

        private int m_MinSpawnTime = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MinSpawnTime
        {
            get { return m_MinSpawnTime; }
            set { m_MinSpawnTime = value; }
        }

        private int m_MaxSpawnTime = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxSpawnTime
        {
            get { return m_MaxSpawnTime; }
            set { m_MaxSpawnTime = value; }
        }

        public List<FishingSpot> m_FishingSpots = new List<FishingSpot>();
        public DateTime m_LastActivity = DateTime.UtcNow;
        public TimeSpan m_NextActivity;

        private Timer m_SpawnTimer;

        [Constructable]
        public FishingSpotSpawner(): base(7778)
        {
            Name = "Fishing Spot Spawner";

            Visible = false;
            Movable = false;

            if (!FishingSpotPersistance.m_FishingSpotSpawners.Contains(this))
                FishingSpotPersistance.m_FishingSpotSpawners.Add(this);

            m_NextActivity = TimeSpan.FromMinutes(Utility.RandomMinMax(m_MinSpawnTime, m_MaxSpawnTime));

            m_SpawnTimer = new SpawnTimer(this);
            m_SpawnTimer.Start();
        }

        public FishingSpotSpawner(Serial serial)
            : base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            if (from.AccessLevel == AccessLevel.Player)
                return;

            LabelTo(from, "Spots Active: " + m_FishingSpots.Count.ToString() + " / " + m_InstanceCount.ToString());
            LabelTo(from, "[Double Click to Delete All Fishing Spots]");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel == AccessLevel.Player)
                return;

            int spotCount = m_FishingSpots.Count;

            for (int a = 0; a < spotCount; a++)
            {
                if (m_FishingSpots[0] != null)
                    m_FishingSpots[0].Delete();
            }
        }

        public void StartSpawn(Mobile from)
        {
        }
        
        public override void OnAfterDelete()
        {
            int fishingSpotCount = m_FishingSpots.Count;            

            for (int a = 0; a < fishingSpotCount; a++)
            {
                m_FishingSpots[0].Delete();
            }

            if (FishingSpotPersistance.m_FishingSpotSpawners.Contains(this))
                FishingSpotPersistance.m_FishingSpotSpawners.Remove(this);

            base.OnAfterDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); //version

            //Version 0
            writer.Write(m_Activated);
            writer.Write(m_InstanceCount);
            writer.Write(m_SpawnAllAvailable);
            writer.Write(m_HomeRange);
            writer.Write(m_SpawnRange);
            writer.Write(m_MinSpawnTime);
            writer.Write(m_MaxSpawnTime);

            writer.Write(m_FishingSpots.Count);

            foreach (FishingSpot fishingSpot in m_FishingSpots)
            {
                writer.Write(fishingSpot);
            }

            writer.Write(m_LastActivity);
            writer.Write(m_NextActivity);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            m_FishingSpots = new List<FishingSpot>();

            m_Activated = reader.ReadBool();
            m_InstanceCount = reader.ReadInt();
            m_SpawnAllAvailable = reader.ReadBool();
            m_HomeRange = reader.ReadInt();
            m_SpawnRange = reader.ReadInt();
            m_MinSpawnTime = reader.ReadInt();
            m_MaxSpawnTime = reader.ReadInt();

            int spotCount = reader.ReadInt();
            for (int a = 0; a < spotCount; a++)
            {
                FishingSpot fishingSpot = (FishingSpot)reader.ReadItem();
                m_FishingSpots.Add(fishingSpot);
            }

            m_LastActivity = reader.ReadDateTime();
            m_NextActivity = reader.ReadTimeSpan();

            FishingSpotPersistance.m_FishingSpotSpawners.Add(this);

            if (m_SpawnTimer == null)
            {
                m_SpawnTimer = new SpawnTimer(this);
                m_SpawnTimer.Start();
            }

            else if (!m_SpawnTimer.Running)
                m_SpawnTimer.Start();
        }

        private class SpawnTimer : Timer
        {
            public FishingSpotSpawner m_FishingSpotSpawner;

            public SpawnTimer(FishingSpotSpawner FishingSpotSpawner)
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(10))
            {
                m_FishingSpotSpawner = FishingSpotSpawner;
                Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                if (!m_FishingSpotSpawner.Activated)
                    return;

                DateTime nextSpawnTime = m_FishingSpotSpawner.m_LastActivity + m_FishingSpotSpawner.m_NextActivity;

                if (nextSpawnTime < DateTime.UtcNow)
                {
                    if (m_FishingSpotSpawner.m_FishingSpots.Count < m_FishingSpotSpawner.InstanceCount)
                    {
                        int spotsNeeded = m_FishingSpotSpawner.InstanceCount - m_FishingSpotSpawner.m_FishingSpots.Count;
                        int spotsSpawned = 0;

                        if (!m_FishingSpotSpawner.SpawnAllAvailable)
                            spotsNeeded = 1;

                        m_FishingSpotSpawner.m_LastActivity = DateTime.UtcNow;
                        m_FishingSpotSpawner.m_NextActivity = TimeSpan.FromMinutes(Utility.RandomMinMax(m_FishingSpotSpawner.m_MinSpawnTime, m_FishingSpotSpawner.m_MaxSpawnTime));

                        //Ships Needed
                        for (int a = 0; a < spotsNeeded; a++)
                        {
                            bool spotSpawned = false;

                            for (int b = 0; b < 25; b++)
                            {
                                if (spotSpawned)
                                    break;

                                Point3D newLocation = new Point3D();

                                int x = m_FishingSpotSpawner.X;

                                int xOffset = Utility.RandomMinMax(0, m_FishingSpotSpawner.SpawnRange);
                                if (Utility.RandomDouble() >= .5)
                                    xOffset *= -1;

                                x += xOffset;

                                int y = m_FishingSpotSpawner.Y;

                                int yOffset = Utility.RandomMinMax(0, m_FishingSpotSpawner.SpawnRange);
                                if (Utility.RandomDouble() >= .5)
                                    yOffset *= -1;

                                y += yOffset;

                                newLocation.X = x;
                                newLocation.Y = y;
                                newLocation.Z = m_FishingSpotSpawner.Z;

                                bool validWaterSpot = true;

                                for (int c = -2; c < 3; c++)
                                {
                                    if (!validWaterSpot)
                                        break;

                                    for (int d = -2; d < 3; d++)
                                    {
                                        Point3D componentPoint = new Point3D(newLocation.X - c, newLocation.Y - d, newLocation.Z);

                                        if (!BaseBoat.IsWaterTile(componentPoint, m_FishingSpotSpawner.Map))
                                        {
                                            validWaterSpot = false;
                                            break;
                                        }
                                    }
                                }

                                if (validWaterSpot)
                                {
                                    FishingSpot spot = null;

                                    switch (Utility.RandomMinMax(1, 2))
                                    {
                                        case 1: spot = new SchoolOfFish(); break;
                                        case 2: spot = new Shipwreck(); break;
                                    }

                                    if (spot != null)
                                    {
                                        spot.MoveToWorld(newLocation, m_FishingSpotSpawner.Map);
                                        spot.FishingSpotSpawner = m_FishingSpotSpawner;

                                        m_FishingSpotSpawner.m_LastActivity = DateTime.UtcNow;
                                        m_FishingSpotSpawner.m_FishingSpots.Add(spot);

                                        spotSpawned = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}