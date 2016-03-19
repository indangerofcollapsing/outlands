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
using Server.Spells;

namespace Server.Items
{
    public class UOACZSpawner : Item
    {
        public static List<UOACZSpawner> m_UOACZSpawners = new List<UOACZSpawner>();

        #region Properties

        private bool m_Activated = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Activated
        {
            get { return m_Activated; }
            set
            {
                m_Activated = value;

                UpdateSpawnCriteria();
            }
        }

        private int m_TotalSpawnsOccured = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int TotalSpawnsOccured
        {
            get { return m_TotalSpawnsOccured; }
            set { m_TotalSpawnsOccured = value; }
        }

        private int m_MaxTotalSpawns = 10000;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxTotalSpawns
        {
            get { return m_MaxTotalSpawns; }
            set { m_MaxTotalSpawns = value; }
        }

        private int m_MaxSpawnCount = 1;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxSpawnCount
        {
            get { return m_MaxSpawnCount; }
            set { m_MaxSpawnCount = value; }
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

        private double m_MinSpawnTime = 5;
        [CommandProperty(AccessLevel.GameMaster)]
        public double MinSpawnTime
        {
            get { return m_MinSpawnTime; }
            set
            { 
                m_MinSpawnTime = value;

                UpdateSpawnCriteria();
            }
        }

        private double m_MaxSpawnTime = 10;
        [CommandProperty(AccessLevel.GameMaster)]
        public double MaxSpawnTime
        {
            get { return m_MaxSpawnTime; }
            set
            {
                m_MaxSpawnTime = value;

                UpdateSpawnCriteria();
            }
        }

        private bool m_IgnoreRedirector = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IgnoreRedirector
        {
            get { return m_IgnoreRedirector; }
            set { m_IgnoreRedirector = value; }
        }

        public virtual bool CheckSpawnForAdjacentBlocking { get { return false; } }

        public DateTime m_LastActivity = DateTime.UtcNow;
        public TimeSpan m_NextActivity;

        #endregion

        public Timer m_SpawnTimer;

        [Constructable]
        public UOACZSpawner(): base(6153)
        {
            Name = "UOACZ Spawner";
            
            Movable = false;

            m_UOACZSpawners.Add(this);

            double spawnDelay = m_MinSpawnTime + (Utility.RandomDouble() * (m_MaxSpawnTime - m_MinSpawnTime));
            m_NextActivity = TimeSpan.FromMinutes(spawnDelay);

            m_SpawnTimer = new SpawnTimer(this);
            m_SpawnTimer.Start();
        }

        public override bool CanBeSeenBy(Mobile from)
        {
            if (from.AccessLevel > AccessLevel.Player)
                return true;

            return false;
        }

        public void UpdateSpawnCriteria()
        {
            m_LastActivity = DateTime.UtcNow;

            double spawnDelay = m_MinSpawnTime + (Utility.RandomDouble() * (m_MaxSpawnTime - m_MinSpawnTime));
            m_NextActivity = TimeSpan.FromMinutes(spawnDelay);         
        }

        public UOACZSpawner(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            if (from.AccessLevel == AccessLevel.Player)
                return;

            LabelTo(from, Name);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel == AccessLevel.Player)
                return;
        }

        public virtual int GetAvailableSpawnAmount()
        {
            int availableSpawnAmount = 0;

            return availableSpawnAmount;
        }        

        public virtual void PerformSpawns(int spawnsNeeded)
        {
            for (int a = 0; a < spawnsNeeded; a++)
            {
                bool spawnCreated = false;

                for (int b = 0; b < 25; b++)
                {
                    if (spawnCreated)
                        break;

                    Point3D newLocation = new Point3D();

                    int x = X;

                    int xOffset = Utility.RandomMinMax(0, SpawnRange);
                    if (Utility.RandomDouble() >= .5)
                        xOffset *= -1;

                    x += xOffset;

                    int y = Y;

                    int yOffset = Utility.RandomMinMax(0, SpawnRange);
                    if (Utility.RandomDouble() >= .5)
                        yOffset *= -1;

                    y += yOffset;

                    newLocation.X = x;
                    newLocation.Y = y;
                    newLocation.Z = Z;

                    SpellHelper.AdjustField(ref newLocation, Map, 12, false);
                    
                    UOACZSpawnRedirector spawnRedirector = null;

                    if (!IgnoreRedirector)
                    {
                        foreach (UOACZSpawnRedirector redirector in UOACZSpawnRedirector.m_Instances)
                        {
                            if (!UOACZRegion.ContainsItem(redirector))
                                continue;

                            if (Utility.GetDistance(redirector.Location, newLocation) <= redirector.TriggerRadius)
                            {
                                spawnRedirector = redirector;
                                break;
                            }
                        }
                    }

                    if (spawnRedirector != null)
                    {
                        newLocation.X = spawnRedirector.Location.X + Utility.RandomMinMax(-1 * spawnRedirector.TriggerRadius, spawnRedirector.TriggerRadius);
                        newLocation.Y = spawnRedirector.Location.Y + Utility.RandomMinMax(-1 * spawnRedirector.TriggerRadius, spawnRedirector.TriggerRadius);
                        newLocation.Z = spawnRedirector.Location.Z;

                        int redirectionX = 0;
                        int redirectionY = 0;

                        switch (Utility.RandomMinMax(1, 3))
                        {
                            case 1:
                                redirectionX = Utility.RandomMinMax(spawnRedirector.MinRedirection, spawnRedirector.MaxRedirection);

                                if (Utility.RandomDouble() <= .5)
                                    redirectionX *= -1;

                                newLocation.X = spawnRedirector.Location.X + redirectionX;
                            break;

                            case 2:
                                redirectionY = Utility.RandomMinMax(spawnRedirector.MinRedirection, spawnRedirector.MaxRedirection);

                                if (Utility.RandomDouble() <= .5)
                                    redirectionY *= -1;

                                newLocation.Y = spawnRedirector.Location.Y + redirectionY;
                            break;

                            case 3:
                                redirectionX = Utility.RandomMinMax(spawnRedirector.MinRedirection, spawnRedirector.MaxRedirection);

                                if (Utility.RandomDouble() <= .5)
                                    redirectionX *= -1;

                                newLocation.X = spawnRedirector.Location.X + redirectionX;

                                redirectionY = Utility.RandomMinMax(spawnRedirector.MinRedirection, spawnRedirector.MaxRedirection);

                                if (Utility.RandomDouble() <= .5)
                                    redirectionY *= -1;

                                newLocation.Y = spawnRedirector.Location.Y + redirectionY;
                            break;
                        }

                        SpellHelper.AdjustField(ref newLocation, Map, 12, false);
                    }

                    bool areaIsBlocked = false;

                    foreach (UOACZSpawnAreaBlocker spawnAreaBlocker in UOACZSpawnAreaBlocker.m_Instances)
                    {
                        if (!UOACZRegion.ContainsItem(spawnAreaBlocker))
                            continue;

                        if (spawnAreaBlocker.BlockedArea.Contains(newLocation))
                        {
                            areaIsBlocked = true;
                            break;
                        }
                    }

                    if (areaIsBlocked)
                        continue;
                    
                    if (UOACZSystem.IsValidSpawnLocation(newLocation, Map, CheckSpawnForAdjacentBlocking))
                    {
                        Spawn(newLocation, Map);

                        TotalSpawnsOccured++;

                        m_LastActivity = DateTime.UtcNow;
                        spawnCreated = true;
                    }
                }
            }
        }

        public virtual void Spawn(Point3D point, Map map)
        {
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            DeleteAllInstances();

            if (m_UOACZSpawners.Contains(this))
                m_UOACZSpawners.Remove(this);
        }

        public virtual void DeleteAllInstances()
        {
        }        

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version

            //Version 0            
            writer.Write(m_Activated);
            writer.Write(m_TotalSpawnsOccured);
            writer.Write(m_MaxTotalSpawns);
            writer.Write(m_MaxSpawnCount);
            writer.Write(m_SpawnAllAvailable);
            writer.Write(m_HomeRange);
            writer.Write(m_SpawnRange);
            writer.Write(m_MinSpawnTime);
            writer.Write(m_MaxSpawnTime);

            writer.Write(m_LastActivity);
            writer.Write(m_NextActivity);
            writer.Write(m_IgnoreRedirector);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0            
            m_Activated = reader.ReadBool();
            m_TotalSpawnsOccured = reader.ReadInt();
            m_MaxTotalSpawns = reader.ReadInt();
            m_MaxSpawnCount = reader.ReadInt();
            m_SpawnAllAvailable = reader.ReadBool();
            m_HomeRange = reader.ReadInt();
            m_SpawnRange = reader.ReadInt();
            m_MinSpawnTime = reader.ReadDouble();
            m_MaxSpawnTime = reader.ReadDouble();             

            m_LastActivity = reader.ReadDateTime();
            m_NextActivity = reader.ReadTimeSpan();
            m_IgnoreRedirector = reader.ReadBool();
            
            //-----------

            m_UOACZSpawners.Add(this);

            m_SpawnTimer = new SpawnTimer(this);
            m_SpawnTimer.Start();           
        }          

        private class SpawnTimer : Timer
        {
            public UOACZSpawner m_UOACZSpawner;

            public SpawnTimer(UOACZSpawner UOACZSpawner): base(TimeSpan.Zero, TimeSpan.FromSeconds(10))
            {
                m_UOACZSpawner = UOACZSpawner;
                Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                if (m_UOACZSpawner == null)
                {
                    Stop();
                    return;
                }

                if (m_UOACZSpawner.Deleted)
                {
                    Stop();
                    return;
                }

                if (!UOACZRegion.ContainsItem(m_UOACZSpawner)) return;
                if (!UOACZPersistance.Active) return;                
                if (!m_UOACZSpawner.Activated || m_UOACZSpawner.RootParentEntity != null) return;

                if (m_UOACZSpawner.TotalSpawnsOccured >= m_UOACZSpawner.MaxTotalSpawns)
                    return;

                int spawnsNeeded = m_UOACZSpawner.GetAvailableSpawnAmount();

                if (spawnsNeeded == 0)
                {
                    m_UOACZSpawner.m_LastActivity = DateTime.UtcNow;

                    double spawnDelay = m_UOACZSpawner.m_MinSpawnTime + (Utility.RandomDouble() * (m_UOACZSpawner.m_MaxSpawnTime - m_UOACZSpawner.m_MinSpawnTime));
                    m_UOACZSpawner.m_NextActivity = TimeSpan.FromMinutes(spawnDelay);

                    return;
                }  
              
                DateTime nextSpawnTime = m_UOACZSpawner.m_LastActivity + m_UOACZSpawner.m_NextActivity;
                
                if (nextSpawnTime <= DateTime.UtcNow)
                {  
                    int spotsSpawned = 0;

                    if (spawnsNeeded > 0)
                    {
                        if (!m_UOACZSpawner.SpawnAllAvailable)
                            spawnsNeeded = 1;

                        m_UOACZSpawner.PerformSpawns(spawnsNeeded);

                        m_UOACZSpawner.m_LastActivity = DateTime.UtcNow;

                        double spawnDelay = m_UOACZSpawner.m_MinSpawnTime + (Utility.RandomDouble() * (m_UOACZSpawner.m_MaxSpawnTime - m_UOACZSpawner.m_MinSpawnTime));
                        m_UOACZSpawner.m_NextActivity = TimeSpan.FromMinutes(spawnDelay);
                    }
                }
            }
        }
    }
}