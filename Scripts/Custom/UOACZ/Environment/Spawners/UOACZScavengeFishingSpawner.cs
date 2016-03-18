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
    public class UOACZScavengeFishingSpawner : UOACZSpawner
    {
        public static List<UOACZScavengeFishingSpawner> m_Spawners = new List<UOACZScavengeFishingSpawner>();

        public List<Item> m_Items = new List<Item>();

        public override bool CheckSpawnForAdjacentBlocking { get { return false; } }

        [Constructable]
        public UOACZScavengeFishingSpawner(): base()
        {
            Name = "Scavenge Fishing Spawner";

            Hue = 2220;

            MaxSpawnCount = 1;

            HomeRange = 0;
            SpawnRange = 0;
            MaxTotalSpawns = 1;

            MinSpawnTime = 1;
            MaxSpawnTime = 240;

            m_Spawners.Add(this);
        }

        public UOACZScavengeFishingSpawner(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            if (from.AccessLevel == AccessLevel.Player)
                return;

            LabelTo(from, Name);

            if (!Activated)
            {
                LabelTo(from, "Inactive: " + m_Items.Count + " / " + MaxSpawnCount.ToString());
                return;
            }

            LabelTo(from, "Spawned: " + m_Items.Count + " / " + MaxSpawnCount.ToString());
            
            if (GetAvailableSpawnAmount() > 0)
                LabelTo(from, "(Next Spawn in " + Utility.CreateTimeRemainingString(DateTime.UtcNow, m_LastActivity + m_NextActivity, true, true, true, true, true) + ")");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel == AccessLevel.Player)
                return;

            if (m_Items.Count < MaxSpawnCount)
            {
                if (SpawnAllAvailable)
                {
                    PerformSpawns(MaxSpawnCount - m_Items.Count);
                    from.SendMessage("Spawning All Available.");
                }

                else
                {
                    PerformSpawns(1);
                    from.SendMessage("Spawned: " + m_Items.Count + " / " + MaxSpawnCount.ToString());
                }
            }

            else
            {
                from.SendMessage("Spawn Count at Max. Deleting All Spawns.");
                DeleteAllInstances();
            }
        }

        public override int GetAvailableSpawnAmount()
        {
            int availableSpawnAmount = MaxSpawnCount - m_Items.Count;

            return availableSpawnAmount;
        }  
        
        public override void Spawn(Point3D point, Map map)
        {
            base.Spawn(point, map);

            Type itemType = null;

            switch (Utility.RandomMinMax(1, 1))
            {
                case 1: itemType = typeof(UOACZScavengeFishing); break;
            }

            if (itemType != null)
            {
                UOACZScavengeFishing item = (UOACZScavengeFishing)Activator.CreateInstance(itemType);

                if (item != null)
                {
                    item.m_Spawner = this;

                    item.MoveToWorld(point, map);
                    m_Items.Add(item);
                }
            }
        }      

        public override void DeleteAllInstances()
        {
            Queue m_Queue = new Queue();

            foreach (Item item in m_Items)
            {
                m_Queue.Enqueue(item);
            }

            while (m_Queue.Count > 0)
            {
                Item item = (Item)m_Queue.Dequeue();

                m_Items.Remove(item);
                item.Delete();
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
            writer.Write(m_Items.Count);
            for (int a = 0; a < m_Items.Count; a++)
            {
                writer.Write(m_Items[a]);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Items = new List<Item>();

            //Version 0
            if (version >= 0)
            {
                int mobileCount = reader.ReadInt();

                for (int a = 0; a < mobileCount; a++)
                {
                    Item item = reader.ReadItem();

                    if (item == null) continue;
                    if (item.Deleted) continue;

                    m_Items.Add(item);
                }
            }

            //----------

            m_Spawners.Add(this);
        }
    }
}