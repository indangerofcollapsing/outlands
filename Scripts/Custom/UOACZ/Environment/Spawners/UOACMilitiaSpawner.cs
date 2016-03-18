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
    public class UOACZMilitiaSpawner : UOACZSpawner
    {
        public enum SpawnType
        {  
            RandomAny,
            RandomWilderness,
            RandomMilitia,
            RandomElite,             
            MilitiaArcherSentry,
            MilitiaArcher,
            MilitiaGuardsman,
            Soldier,
            Ranger            
        }

        public static List<UOACZMilitiaSpawner> m_Spawners = new List<UOACZMilitiaSpawner>();

        public List<Mobile> m_Mobiles = new List<Mobile>();

        private bool m_Wilderness = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Wilderness
        {
            get { return m_Wilderness; }
            set { m_Wilderness = value; }
        }

        private SpawnType m_MilitiaType = SpawnType.RandomAny;
        [CommandProperty(AccessLevel.GameMaster)]
        public SpawnType MilitiaType
        {
            get { return m_MilitiaType; }
            set { m_MilitiaType = value; }
        }

        [Constructable]
        public UOACZMilitiaSpawner(): base()
        {
            Name = "Militia Spawner";

            Hue = 2615;

            MaxSpawnCount = 1;

            HomeRange = 30;
            SpawnRange = 5;

            MinSpawnTime = 10000;
            MaxSpawnTime = 10000;

            m_Spawners.Add(this);
        }

        public UOACZMilitiaSpawner(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            if (from.AccessLevel == AccessLevel.Player)
                return;

            LabelTo(from, Name);

            if (!Activated)
            {
                LabelTo(from, "Inactive: " + m_Mobiles.Count + " / " + MaxSpawnCount.ToString());
                return;
            }

            LabelTo(from, "Spawned: " + m_Mobiles.Count + " / " + MaxSpawnCount.ToString());

            if (GetAvailableSpawnAmount() > 0)
                LabelTo(from, "(Next Spawn in " + Utility.CreateTimeRemainingString(DateTime.UtcNow, m_LastActivity + m_NextActivity, true, true, true, true, true) + ")");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel == AccessLevel.Player)
                return;

            if (m_Mobiles.Count < MaxSpawnCount)
            {
                if (SpawnAllAvailable)
                {
                    PerformSpawns(MaxSpawnCount - m_Mobiles.Count);
                    from.SendMessage("Spawning All Available.");
                }

                else
                {
                    PerformSpawns(1);
                    from.SendMessage("Spawned: " + m_Mobiles.Count + " / " + MaxSpawnCount.ToString());
                }
            }

            else
            {
                from.SendMessage("Spawn Count at Maximum. Deleting All Spawns.");
                DeleteAllInstances();
            }
        }

        public override int GetAvailableSpawnAmount()
        {
            int availableSpawnAmount = MaxSpawnCount - m_Mobiles.Count;

            return availableSpawnAmount;
        }
        
        public override void Spawn(Point3D point, Map map)
        {
            Type creatureType = null;

            switch (m_MilitiaType)
            {
                case SpawnType.MilitiaArcherSentry: creatureType = typeof(UOACZSentry); break;
                case SpawnType.MilitiaArcher: creatureType = typeof(UOACZMilitiaArcher); break;
                case SpawnType.MilitiaGuardsman: creatureType = typeof(UOACZMilitiaGuardsman); break;
                case SpawnType.Soldier: creatureType = typeof(UOACZSoldier); break;
                case SpawnType.Ranger: creatureType = typeof(UOACZRanger); break;
                case SpawnType.RandomAny:
                    switch(Utility.RandomMinMax(1, 4))
                    {
                        case 1: creatureType = typeof(UOACZMilitiaArcher); break;
                        case 2: creatureType = typeof(UOACZMilitiaGuardsman); break;
                        case 3: creatureType = typeof(UOACZRanger); break;
                        case 4: creatureType = typeof(UOACZSoldier); break;
                    }
                break;
                case SpawnType.RandomWilderness:
                    switch (Utility.RandomMinMax(1, 2))
                    {
                        case 1: creatureType = typeof(UOACZHunter); break;
                        case 2: creatureType = typeof(UOACZScout); break;
                    }
                break;
                case SpawnType.RandomMilitia:
                    switch(Utility.RandomMinMax(1, 2))
                    {
                        case 1: creatureType = typeof(UOACZMilitiaArcher); break;
                        case 2: creatureType = typeof(UOACZMilitiaGuardsman); break;
                    }
                break;
                case SpawnType.RandomElite:
                    switch (Utility.RandomMinMax(1, 2))
                    {
                        case 1: creatureType = typeof(UOACZRanger); break;
                        case 2: creatureType = typeof(UOACZSoldier); break;
                    }
                break;
            }

            if (creatureType != null)
            {
                UOACZBaseMilitia creature = (UOACZBaseMilitia)Activator.CreateInstance(creatureType);

                if (creature != null)
                {
                    creature.RangeHome = HomeRange;
                    creature.Home = point;
                    creature.m_Spawner = this;

                    creature.InWilderness = Wilderness;

                    creature.MoveToWorld(point, map);
                    creature.FaceRandomDirection();

                    m_Mobiles.Add(creature);
                }
            }
        }   

        public override void DeleteAllInstances()
        {
            Queue m_Queue = new Queue();

            foreach (Mobile mobile in m_Mobiles)
            {
                m_Queue.Enqueue(mobile);
            }

            while (m_Queue.Count > 0)
            {
                Mobile mobile = (Mobile)m_Queue.Dequeue();

                m_Mobiles.Remove(mobile);
                mobile.Delete();
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_Spawners.Contains(this))
                m_Spawners.Remove(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version            

            //Version 0
            writer.Write(m_Mobiles.Count);
            for (int a = 0; a < m_Mobiles.Count; a++)
            {
                writer.Write(m_Mobiles[a]);
            }

            writer.Write(m_Wilderness);
            writer.Write((int)m_MilitiaType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Mobiles = new List<Mobile>();

            //Version 0
            if (version >= 0)
            {
                int mobileCount = reader.ReadInt();

                for (int a = 0; a < mobileCount; a++)
                {
                    Mobile mobile = reader.ReadMobile();

                    if (mobile == null) continue;
                    if (mobile.Deleted || !mobile.Alive) continue;

                    m_Mobiles.Add(mobile);
                }

                m_Wilderness = reader.ReadBool();
                m_MilitiaType = (SpawnType)reader.ReadInt();   
            }

            //----------

            m_Spawners.Add(this);
        }
    }
}