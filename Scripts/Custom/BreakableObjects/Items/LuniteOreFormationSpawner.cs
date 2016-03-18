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
    public class LuniteOreFormationPersistance
    {
        public static List<LuniteOreFormationSpawner> m_Instances = new List<LuniteOreFormationSpawner>();

        public static void Initialize()
        {
            CommandSystem.Register("SpawnAllLuniteOreFormationSpawners", AccessLevel.GameMaster, new CommandEventHandler(SpawnAllLuniteOreFormationSpawners));
            CommandSystem.Register("DeleteAllLuniteOreFormationSpawners", AccessLevel.GameMaster, new CommandEventHandler(DeleteAllLuniteOreFormationSpawners));

            CommandSystem.Register("SpawnAllLuniteOreFormations", AccessLevel.GameMaster, new CommandEventHandler(SpawnAllLuniteOreFormations));
            CommandSystem.Register("ResetAllLuniteOreFormationTimers", AccessLevel.GameMaster, new CommandEventHandler(ResetAllLuniteOreFormationTimers));
        }

        #region Commands

        [Usage("SpawnAllLuniteOreFormations")]
        [Description("Spawns All Pre-Determined Lunite Ore Formation Spawners")]
        public static void SpawnAllLuniteOreFormationSpawners(CommandEventArgs arg)
        {
            PlayerMobile pm_Mobile = arg.Mobile as PlayerMobile;

            if (pm_Mobile == null)
                return;

            int LuniteOreFormationSpawners = m_Instances.Count;

            for (int a = 0; a < LuniteOreFormationSpawners; a++)
            {
                if (m_Instances[0] != null)
                    m_Instances[0].Delete();
            }

            #region Pre-Determined Lunite Ore Formation Spawn Locations

            LuniteOreFormationSpawner LuniteOreFormationSpawner;
            Point3D location;

            //Wrong Dungeon
            location = new Point3D(5789, 526, 10);
            LuniteOreFormationSpawner = new LuniteOreFormationSpawner();
            LuniteOreFormationSpawner.MoveToWorld(location, pm_Mobile.Map);  

            location = new Point3D(5872, 596, 15);
            LuniteOreFormationSpawner = new LuniteOreFormationSpawner();
            LuniteOreFormationSpawner.MoveToWorld(location, pm_Mobile.Map);  

            location = new Point3D(5829, 517, 0);
            LuniteOreFormationSpawner = new LuniteOreFormationSpawner();
            LuniteOreFormationSpawner.MoveToWorld(location, pm_Mobile.Map);  

            location = new Point3D(5739, 549, 20);
            LuniteOreFormationSpawner = new LuniteOreFormationSpawner();
            LuniteOreFormationSpawner.MoveToWorld(location, pm_Mobile.Map); 
 
            location = new Point3D(5646, 577, 22);
            LuniteOreFormationSpawner = new LuniteOreFormationSpawner();
            LuniteOreFormationSpawner.MoveToWorld(location, pm_Mobile.Map);

            //Covetous Dungeon
            location = new Point3D(5395, 1849, 0);
            LuniteOreFormationSpawner = new LuniteOreFormationSpawner();
            LuniteOreFormationSpawner.MoveToWorld(location, pm_Mobile.Map);
            
            location = new Point3D(5480, 1982, 0);
            LuniteOreFormationSpawner = new LuniteOreFormationSpawner();
            LuniteOreFormationSpawner.MoveToWorld(location, pm_Mobile.Map);

            location = new Point3D(5440, 2034, 0);
            LuniteOreFormationSpawner = new LuniteOreFormationSpawner();
            LuniteOreFormationSpawner.MoveToWorld(location, pm_Mobile.Map);

            location = new Point3D(5515, 1817, 0);
            LuniteOreFormationSpawner = new LuniteOreFormationSpawner();
            LuniteOreFormationSpawner.MoveToWorld(location, pm_Mobile.Map);

            //Deceit Dungeon
            location = new Point3D(5144, 552, -40);
            LuniteOreFormationSpawner = new LuniteOreFormationSpawner();
            LuniteOreFormationSpawner.MoveToWorld(location, pm_Mobile.Map);

            location = new Point3D(5388, 531, 65);
            LuniteOreFormationSpawner = new LuniteOreFormationSpawner();
            LuniteOreFormationSpawner.MoveToWorld(location, pm_Mobile.Map);

            location = new Point3D(5250, 662, 0);
            LuniteOreFormationSpawner = new LuniteOreFormationSpawner();
            LuniteOreFormationSpawner.MoveToWorld(location, pm_Mobile.Map);
            
            location = new Point3D(5139, 720, 0);
            LuniteOreFormationSpawner = new LuniteOreFormationSpawner();
            LuniteOreFormationSpawner.MoveToWorld(location, pm_Mobile.Map);

            location = new Point3D(5258, 1834, 0);
            LuniteOreFormationSpawner = new LuniteOreFormationSpawner();
            LuniteOreFormationSpawner.MoveToWorld(location, pm_Mobile.Map);

            //Despise Dungeon
            location = new Point3D(5391, 602, 44);
            LuniteOreFormationSpawner = new LuniteOreFormationSpawner();
            LuniteOreFormationSpawner.MoveToWorld(location, pm_Mobile.Map);

            location = new Point3D(5384, 684, 20);
            LuniteOreFormationSpawner = new LuniteOreFormationSpawner();
            LuniteOreFormationSpawner.MoveToWorld(location, pm_Mobile.Map);

            location = new Point3D(5391, 658, 30);
            LuniteOreFormationSpawner = new LuniteOreFormationSpawner();
            LuniteOreFormationSpawner.MoveToWorld(location, pm_Mobile.Map);

            location = new Point3D(5568, 911, 30);
            LuniteOreFormationSpawner = new LuniteOreFormationSpawner();
            LuniteOreFormationSpawner.MoveToWorld(location, pm_Mobile.Map);

            location = new Point3D(5406, 994, 5);
            LuniteOreFormationSpawner = new LuniteOreFormationSpawner();
            LuniteOreFormationSpawner.MoveToWorld(location, pm_Mobile.Map);

            //Destard Dungeon
            location = new Point3D(5227, 905, -40);
            LuniteOreFormationSpawner = new LuniteOreFormationSpawner();
            LuniteOreFormationSpawner.MoveToWorld(location, pm_Mobile.Map);

            location = new Point3D(5128, 854, 0);
            LuniteOreFormationSpawner = new LuniteOreFormationSpawner();
            LuniteOreFormationSpawner.MoveToWorld(location, pm_Mobile.Map);
            
            location = new Point3D(5356, 943, 1);
            LuniteOreFormationSpawner = new LuniteOreFormationSpawner();
            LuniteOreFormationSpawner.MoveToWorld(location, pm_Mobile.Map);
            
            location = new Point3D(5199, 1007, 0);
            LuniteOreFormationSpawner = new LuniteOreFormationSpawner();
            LuniteOreFormationSpawner.MoveToWorld(location, pm_Mobile.Map);
            
            //Hythloth
            location = new Point3D(5988, 24, 22);
            LuniteOreFormationSpawner = new LuniteOreFormationSpawner();
            LuniteOreFormationSpawner.MoveToWorld(location, pm_Mobile.Map);

            location = new Point3D(5918, 236, 44);
            LuniteOreFormationSpawner = new LuniteOreFormationSpawner();
            LuniteOreFormationSpawner.MoveToWorld(location, pm_Mobile.Map);

            location = new Point3D(6026, 197, 22);
            LuniteOreFormationSpawner = new LuniteOreFormationSpawner();
            LuniteOreFormationSpawner.MoveToWorld(location, pm_Mobile.Map);

            location = new Point3D(6048, 230, 44);
            LuniteOreFormationSpawner = new LuniteOreFormationSpawner();
            LuniteOreFormationSpawner.MoveToWorld(location, pm_Mobile.Map);

            //Shame
            location = new Point3D(5429, 112, -1);
            LuniteOreFormationSpawner = new LuniteOreFormationSpawner();
            LuniteOreFormationSpawner.MoveToWorld(location, pm_Mobile.Map);

            location = new Point3D(5522, 95, 0);
            LuniteOreFormationSpawner = new LuniteOreFormationSpawner();
            LuniteOreFormationSpawner.MoveToWorld(location, pm_Mobile.Map);

            location = new Point3D(5483, 245, 0);
            LuniteOreFormationSpawner = new LuniteOreFormationSpawner();
            LuniteOreFormationSpawner.MoveToWorld(location, pm_Mobile.Map);

            location = new Point3D(5864, 117, 10);
            LuniteOreFormationSpawner = new LuniteOreFormationSpawner();
            LuniteOreFormationSpawner.MoveToWorld(location, pm_Mobile.Map);

            //Ice Dungeon
            location = new Point3D(5875, 145, 30);
            LuniteOreFormationSpawner = new LuniteOreFormationSpawner();
            LuniteOreFormationSpawner.MoveToWorld(location, pm_Mobile.Map);

            location = new Point3D(5759, 136, 9);
            LuniteOreFormationSpawner = new LuniteOreFormationSpawner();
            LuniteOreFormationSpawner.MoveToWorld(location, pm_Mobile.Map);

            location = new Point3D(5779, 257, -2);
            LuniteOreFormationSpawner = new LuniteOreFormationSpawner();
            LuniteOreFormationSpawner.MoveToWorld(location, pm_Mobile.Map);

            location = new Point3D(5806, 181, -3);
            LuniteOreFormationSpawner = new LuniteOreFormationSpawner();
            LuniteOreFormationSpawner.MoveToWorld(location, pm_Mobile.Map);

            //Fire Dungeon

            #endregion

            pm_Mobile.SendMessage("Spawning All Lunite Ore Formation Spawners");
        }

        [Usage("DeleteAllLuniteOreFormationSpawners")]
        [Description("Deletes All Lunite Ore Formation Spawners")]
        public static void DeleteAllLuniteOreFormationSpawners(CommandEventArgs arg)
        {
            PlayerMobile pm_Mobile = arg.Mobile as PlayerMobile;

            if (pm_Mobile == null)
                return;

            int LuniteOreFormationSpawners = m_Instances.Count;

            for (int a = 0; a < LuniteOreFormationSpawners; a++)
            {
                if (m_Instances[0] != null)
                    m_Instances[0].Delete();
            }

            pm_Mobile.SendMessage("Deleted All Lunite Ore Formation Spawners");
        }

        [Usage("SpawnAllLuniteOreFormations")]
        [Description("Spawns All Lunite Ore Formations")]
        public static void SpawnAllLuniteOreFormations(CommandEventArgs arg)
        {
            PlayerMobile pm_Mobile = arg.Mobile as PlayerMobile;

            int LuniteOreFormationSpawners = m_Instances.Count;

            for (int a = 0; a < LuniteOreFormationSpawners; a++)
            {
                m_Instances[a].Spawn();
            }

            pm_Mobile.SendMessage("Spawned All Lunite Ore Formations.");
        }

        [Usage("ResetAllLuniteOreFormationTimers")]
        [Description("Reset All Lunite Ore Formation Spawner Timers")]
        public static void ResetAllLuniteOreFormationTimers(CommandEventArgs arg)
        {
            PlayerMobile pm_Mobile = arg.Mobile as PlayerMobile;

            int LuniteOreFormationSpawners = m_Instances.Count;

            for (int a = 0; a < LuniteOreFormationSpawners; a++)
            {
                m_Instances[a].ResetTimer();
            }

            pm_Mobile.SendMessage("Reset All Lunite Ore Formation Spawner Timers.");
        }

        #endregion
    }

    public class LuniteOreFormationSpawner : Item
    {
        #region Old Properties: No Longer Used        

        private int m_InstanceCount = 1;
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

        private int m_SpawnRange = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int SpawnRange
        {
            get { return m_SpawnRange; }
            set { m_SpawnRange = value; }
        }

        private int m_MinSpawnTime = 1000;  //in Minutes
        [CommandProperty(AccessLevel.GameMaster)]
        public int MinSpawnTime
        {
            get { return m_MinSpawnTime; }
            set { m_MinSpawnTime = value; }
        }

        private int m_MaxSpawnTime = 2000; //in Minutes
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxSpawnTime
        {
            get { return m_MaxSpawnTime; }
            set { m_MaxSpawnTime = value; }
        }

        public TimeSpan m_NextActivity;
        public List<LuniteOreFormation> m_LuniteOreFormations = new List<LuniteOreFormation>();

        #endregion        

        private bool m_Activated = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Activated
        {
            get { return m_Activated; }
            set { m_Activated = value; }
        }

        public static int MinSpawnMinutes = 2800;
        public static int MaxSpawnMinutes = 5600;

        public DateTime m_LastActivity = DateTime.UtcNow;       

        private Timer m_SpawnTimer;

        [Constructable]
        public LuniteOreFormationSpawner(): base(9414)
        {
            Name = "Lunite Ore Formation Spawner";

            Visible = false;
            Movable = false;

            ResetTimer();

            LuniteOreFormationPersistance.m_Instances.Add(this);

            m_SpawnTimer = new SpawnTimer(this);
            m_SpawnTimer.Start();
        }

        public LuniteOreFormationSpawner(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            if (from.AccessLevel == AccessLevel.Player)
                return;
           
            LabelTo(from, "[Double Click to Delete or Spawn This Formation]");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel == AccessLevel.Player)
                return;

            ResetTimer();

            if (SpawnExists())            
                DeleteSpawn();

            else            
                Spawn();
        }

        public void ResetTimer()
        {
            m_LastActivity = DateTime.UtcNow + TimeSpan.FromMinutes(Utility.RandomMinMax(MinSpawnMinutes, MaxSpawnMinutes));
        }

        public bool SpawnExists()
        {
            IPooledEnumerable nearbyItems = Map.GetItemsInRange(Location, 0);

            foreach (Item item in nearbyItems)
            {
                if (item is LuniteOreFormation)
                    return true;
            }

            nearbyItems.Free();

            return false;
        }

        public void DeleteSpawn()
        {
            LuniteOreFormation luniteOreFormation = null;

            IPooledEnumerable nearbyItems = Map.GetItemsInRange(Location, 0);

            foreach (Item item in nearbyItems)
            {
                if (item is LuniteOreFormation)
                    luniteOreFormation = (LuniteOreFormation)item;
            }

            nearbyItems.Free();

            if (luniteOreFormation != null)
                luniteOreFormation.Delete();
        }

        public void Spawn()
        {
            LuniteOreFormation luniteOreFormation = new LuniteOreFormation();

            luniteOreFormation.m_LuniteOreFormationSpawner = this;
            luniteOreFormation.MoveToWorld(Location, Map);
        }
        
        public override void OnAfterDelete()
        {
            if (LuniteOreFormationPersistance.m_Instances.Contains(this))
                LuniteOreFormationPersistance.m_Instances.Remove(this);

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

            writer.Write(m_LastActivity);
            writer.Write(m_NextActivity);

            writer.Write(m_LuniteOreFormations.Count);
            for (int a = 0; a < m_LuniteOreFormations.Count; a++)
            {
                writer.Write(m_LuniteOreFormations[a]);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            m_Activated = reader.ReadBool();
            m_InstanceCount = reader.ReadInt();
            m_SpawnAllAvailable = reader.ReadBool();
            m_HomeRange = reader.ReadInt();
            m_SpawnRange = reader.ReadInt();
            m_MinSpawnTime = reader.ReadInt();
            m_MaxSpawnTime = reader.ReadInt();

            m_LastActivity = reader.ReadDateTime();
            m_NextActivity = reader.ReadTimeSpan();

            int formationCount = reader.ReadInt();
            for (int a = 0; a < formationCount; a++)
            {
                LuniteOreFormation luniteFormation = (LuniteOreFormation)reader.ReadItem();                
            }

            //----------------
            
            m_SpawnTimer = new SpawnTimer(this);
            m_SpawnTimer.Start();
        }

        private class SpawnTimer : Timer
        {
            public LuniteOreFormationSpawner m_LuniteOreFormationSpawner;

            public SpawnTimer(LuniteOreFormationSpawner LuniteOreFormationSpawner): base(TimeSpan.Zero, TimeSpan.FromMinutes(10))
            {
                m_LuniteOreFormationSpawner = LuniteOreFormationSpawner;

                Priority = TimerPriority.OneMinute;
            }

            protected override void OnTick()
            {
                if (!m_LuniteOreFormationSpawner.Activated)
                    return;

                if (DateTime.UtcNow >= m_LuniteOreFormationSpawner.m_LastActivity)
                {
                    m_LuniteOreFormationSpawner.ResetTimer();

                    if (!m_LuniteOreFormationSpawner.SpawnExists())
                        m_LuniteOreFormationSpawner.Spawn();
                }
            }
        }
    }
}