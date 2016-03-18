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
    public class UOACZReagentSpawner : UOACZSpawner
    {
        public static List<UOACZReagentSpawner> m_Spawners = new List<UOACZReagentSpawner>();

        public List<Item> m_Items = new List<Item>();

        [Constructable]
        public UOACZReagentSpawner(): base()
        {
            Name = "Reagent Spawner";

            Hue = 1257;

            MaxSpawnCount = 50;
            MaxTotalSpawns = 50;

            HomeRange = 0;
            SpawnRange = 75;

            MinSpawnTime = 5;
            MaxSpawnTime = 10;

            m_Spawners.Add(this);
        }

        public UOACZReagentSpawner(Serial serial): base(serial)
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

            LabelTo(from, "Spawned: " + TotalSpawnsOccured.ToString()+ " / " + MaxSpawnCount.ToString());
            
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
                    PerformSpawns(MaxSpawnCount - TotalSpawnsOccured);
                    from.SendMessage("Spawning All Available.");
                }

                else
                {
                    PerformSpawns(1);
                    from.SendMessage("Spawned: " + TotalSpawnsOccured + " / " + MaxSpawnCount.ToString());
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
            int availableSpawnAmount = MaxSpawnCount - TotalSpawnsOccured;

            return availableSpawnAmount;
        }  

        public override void Spawn(Point3D point, Map map)
        {
            base.Spawn(point, map);

            Item item = null;

            int minAmount = 2;
            int maxAmount = 4;

            int amount = Utility.RandomMinMax(minAmount, maxAmount);

            switch (Utility.RandomMinMax(1, 8))
            {
                case 1: item = new BlackPearl(amount); break;
                case 2: item = new Bloodmoss(amount); break;
                case 3: item = new MandrakeRoot(amount); break;
                case 4: item = new Ginseng(amount); break;
                case 5: item = new Garlic(amount); break;
                case 6: item = new SpidersSilk(amount); break;
                case 7: item = new SulfurousAsh(amount); break;
                case 8: item = new Nightshade(amount); break;
            }

            item.MoveToWorld(point, map);
        }      

        public override void DeleteAllInstances()
        {
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
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //----------

            m_Spawners.Add(this);
        }
    }
}