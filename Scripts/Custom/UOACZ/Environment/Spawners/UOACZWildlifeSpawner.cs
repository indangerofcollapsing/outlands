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
    public class UOACZWildlifeSpawner : UOACZSpawner
    {
        public static List<UOACZWildlifeSpawner> m_Spawners = new List<UOACZWildlifeSpawner>();

        public List<Mobile> m_Mobiles = new List<Mobile>();

        [Constructable]
        public UOACZWildlifeSpawner(): base()
        {
            Name = "Wildlife Spawner";

            Hue = 2207;

            MaxSpawnCount = 10;

            HomeRange = 25;
            SpawnRange = 75;

            MinSpawnTime = 3;
            MaxSpawnTime = 5;

            m_Spawners.Add(this);
        }

        public UOACZWildlifeSpawner(Serial serial): base(serial)
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
   
            switch (Utility.RandomMinMax(1, 20))
            {                
                case 1: creatureType = typeof(UOACZBison); break;
                case 2: creatureType = typeof(UOACZBoar); break;
                case 3: creatureType = typeof(UOACZBull); break;
                case 4: creatureType = typeof(UOACZCaribou); break;                
                case 5: creatureType = typeof(UOACZCow); break;
                case 6: creatureType = typeof(UOACZDeer); break;                
                case 7: creatureType = typeof(UOACZGiantRat); break;
                case 8: creatureType = typeof(UOACZGoat); break;
                case 9: creatureType = typeof(UOACZLlama); break;
                case 10: creatureType = typeof(UOACZPig); break;
                case 11: creatureType = typeof(UOACZRabbit); break;
                case 12: creatureType = typeof(UOACZRam); break;
                case 13: creatureType = typeof(UOACZRat); break;
                case 14: creatureType = typeof(UOACZSheep); break;
                case 15: creatureType = typeof(UOACZBird); break;
                case 16: creatureType = typeof(UOACZBird); break;
                case 17: creatureType = typeof(UOACZChicken); break;
                case 18: creatureType = typeof(UOACZChicken); break;
                case 19: creatureType = typeof(UOACZEagle); break;
                case 20: creatureType = typeof(UOACZEagle); break;
            }               

            if (creatureType != null)
            {
                UOACZBaseWildlife creature = (UOACZBaseWildlife)Activator.CreateInstance(creatureType);

                if (creature != null)
                {
                    creature.RangeHome = HomeRange;
                    creature.Home = point;
                    creature.m_Spawner = this;

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
            }

            //----------

            m_Spawners.Add(this);
        }
    }
}