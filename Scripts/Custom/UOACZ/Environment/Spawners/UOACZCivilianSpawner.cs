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
    public class UOACZCivilianSpawner : UOACZSpawner
    {
        public static List<UOACZCivilianSpawner> m_Spawners = new List<UOACZCivilianSpawner>();

        public List<Mobile> m_Mobiles = new List<Mobile>();

        private bool m_Wilderness = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Wilderness
        {
            get { return m_Wilderness; }
            set { m_Wilderness = value; }
        }

        private UOACZBaseCivilian.CivilianType m_CivilianType = UOACZBaseCivilian.CivilianType.Any;
        [CommandProperty(AccessLevel.GameMaster)]
        public UOACZBaseCivilian.CivilianType CivilianType
        {
            get { return m_CivilianType; }
            set { m_CivilianType = value; }
        }
        
        [Constructable]
        public UOACZCivilianSpawner(): base()
        {
            Name = "Civilian Spawner";

            Hue = 2595;

            MaxSpawnCount = 1;

            HomeRange = 8;
            SpawnRange = 3;

            MinSpawnTime = 10000;
            MaxSpawnTime = 10000;

            m_Spawners.Add(this);
        }

        public UOACZCivilianSpawner(Serial serial): base(serial)
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

            switch (m_CivilianType)
            {
                case UOACZBaseCivilian.CivilianType.None: break;

                case UOACZBaseCivilian.CivilianType.Alchemist: creatureType = typeof(UOACZAlchemist); break;
                case UOACZBaseCivilian.CivilianType.Weaver: creatureType = typeof(UOACZWeaver); break;
                case UOACZBaseCivilian.CivilianType.Farmer: creatureType = typeof(UOACZFarmer); break;
                case UOACZBaseCivilian.CivilianType.Fisherman: creatureType = typeof(UOACZFisherman); break;
                case UOACZBaseCivilian.CivilianType.Skinner: creatureType = typeof(UOACZSkinner); break;
                case UOACZBaseCivilian.CivilianType.Baker: creatureType = typeof(UOACZBaker); break;
                case UOACZBaseCivilian.CivilianType.Butcher: creatureType = typeof(UOACZButcher); break;
                case UOACZBaseCivilian.CivilianType.Bowyer: creatureType = typeof(UOACZBowyer); break;
                case UOACZBaseCivilian.CivilianType.Provisioner: creatureType = typeof(UOACZProvisioner); break;
                case UOACZBaseCivilian.CivilianType.Brewmaster: creatureType = typeof(UOACZBrewmaster); break;
                case UOACZBaseCivilian.CivilianType.Carpenter: creatureType = typeof(UOACZCarpenter); break;
                case UOACZBaseCivilian.CivilianType.Miner: creatureType = typeof(UOACZMiner); break;
                case UOACZBaseCivilian.CivilianType.Healer: creatureType = typeof(UOACZHealer); break;
                case UOACZBaseCivilian.CivilianType.Tinker: creatureType = typeof(UOACZTinker); break;
                case UOACZBaseCivilian.CivilianType.Blacksmith: creatureType = typeof(UOACZBlacksmith); break;
                case UOACZBaseCivilian.CivilianType.Merchant: creatureType = typeof(UOACZMerchant); break;
                case UOACZBaseCivilian.CivilianType.Noble: creatureType = typeof(UOACZNoble); break;
                    
                case UOACZBaseCivilian.CivilianType.Any:
                    switch (Utility.RandomMinMax(1, 16))
                    {
                        case 1: creatureType = typeof(UOACZAlchemist); break;
                        case 2: creatureType = typeof(UOACZWeaver); break;
                        case 3: creatureType = typeof(UOACZFarmer); break;
                        case 4: creatureType = typeof(UOACZFisherman); break;
                        case 5: creatureType = typeof(UOACZSkinner); break;
                        case 6: creatureType = typeof(UOACZBaker); break;
                        case 7: creatureType = typeof(UOACZButcher); break;
                        case 8: creatureType = typeof(UOACZBowyer); break;
                        case 9: creatureType = typeof(UOACZProvisioner); break;
                        case 10: creatureType = typeof(UOACZBrewmaster); break;
                        case 11: creatureType = typeof(UOACZCarpenter); break;
                        case 12: creatureType = typeof(UOACZMiner); break;
                        case 13: creatureType = typeof(UOACZHealer); break;
                        case 14: creatureType = typeof(UOACZTinker); break;
                        case 15: creatureType = typeof(UOACZBlacksmith); break;
                        case 16: creatureType = typeof(UOACZMerchant); break;
                        case 17: creatureType = typeof(UOACZNoble); break;
                    }                
                break;
            }

            if (creatureType != null)
            {
                UOACZBaseCivilian creature = (UOACZBaseCivilian)Activator.CreateInstance(creatureType);

                if (creature != null)
                {
                    creature.RangeHome = HomeRange;
                    creature.Home = point;
                    creature.m_Spawner = this;

                    creature.InWilderness = m_Wilderness;

                    creature.MoveToWorld(point, map);
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
            writer.Write((int)m_CivilianType);
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
                m_CivilianType = (UOACZBaseCivilian.CivilianType)reader.ReadInt();               
            }

            //----------

            m_Spawners.Add(this);
        }
    }
}