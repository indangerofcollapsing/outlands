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
    public class UOACZUndeadSpawner : UOACZSpawner
    {
        public static List<UOACZUndeadSpawner> m_Spawners = new List<UOACZUndeadSpawner>();

        public List<Mobile> m_Mobiles = new List<Mobile>();

        private bool m_Wilderness = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Wilderness
        {
            get { return m_Wilderness; }
            set { m_Wilderness = value; }
        }

        private int m_GotoSplitGroup = -1; 
        [CommandProperty(AccessLevel.GameMaster)]
        public int GotoSplitGroup
        {
            get { return m_GotoSplitGroup; }
            set { m_GotoSplitGroup = value; }
        }

        private bool m_AllowTeleportToBaseNode = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool AllowTeleportToBaseNode
        {
            get { return m_AllowTeleportToBaseNode; }
            set { m_AllowTeleportToBaseNode = value; }
        }

        private int m_MinThreatLevel = -1;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MinThreatLevel
        {
            get { return m_MinThreatLevel; }
            set { m_MinThreatLevel = value; }
        }

        private int m_MaxThreatLevel = -1;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxThreatLevel
        {
            get { return m_MaxThreatLevel; }
            set { m_MaxThreatLevel = value; }
        }

        private bool m_GuardHome = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool GuardHome
        {
            get { return m_GuardHome; }
            set { m_GuardHome = value; }
        }

        private int m_GuardHomeRange = 25;
        [CommandProperty(AccessLevel.GameMaster)]
        public int GuardHomeRange
        {
            get { return m_GuardHomeRange; }
            set { m_GuardHomeRange = value; }
        }

        [Constructable]
        public UOACZUndeadSpawner(): base()
        {
            Name = "Undead Spawner";

            Hue = 2117;

            MaxSpawnCount = 10;

            HomeRange = 25;
            SpawnRange = 75;

            MinSpawnTime = 1;
            MaxSpawnTime = 2;

            m_Spawners.Add(this);
        }

        public UOACZUndeadSpawner(Serial serial): base(serial)
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
                from.SendMessage("Spawn Count at Max. Deleting All Spawns.");
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
            if (!UOACZPersistance.Active)
                return;

            int creatureThreatLevel = UOACZPersistance.m_ThreatLevel;

            if (!m_Wilderness)
                creatureThreatLevel -= 30;

            int minThreatLevel = 0;
            int maxThreatLevel = creatureThreatLevel;

            if (m_MinThreatLevel > -1)
                minThreatLevel = m_MinThreatLevel;

            if (m_MaxThreatLevel > -1)
                maxThreatLevel = m_MaxThreatLevel;

            Type creatureType = UOACZBaseUndead.GetRandomUndeadType(minThreatLevel, maxThreatLevel);

            if (creatureType != null)
            {
                UOACZBaseUndead creature = (UOACZBaseUndead)Activator.CreateInstance(creatureType);

                if (creature != null)
                {
                    creature.RangeHome = HomeRange;
                    creature.Home = point;
                    creature.m_Spawner = this;
                    creature.InWilderness = m_Wilderness;
                    creature.CanTeleportToBaseNode = m_AllowTeleportToBaseNode;

                    creature.MoveToWorld(point, map);
                    m_Mobiles.Add(creature);

                    if (m_AllowTeleportToBaseNode)
                    {
                        if (m_GotoSplitGroup == -1)
                            creature.m_NeedWaypoint = true;

                        else
                        {
                            UOACZWayPoint closestWaypoint = null;
                            int closestWaypointDistance = 100000;

                            foreach (UOACZWayPoint waypoint in UOACZWayPoint.m_UOACZWaypoints)
                            {
                                if (waypoint == null) continue;
                                if (waypoint.Deleted) continue;
                                if (waypoint.m_GotoSplitGroup == -1) continue;
                                if (!UOACZRegion.ContainsItem(waypoint)) continue;
                                if (waypoint.WaypointType == UOACZWayPoint.UOACZWaypointType.UndeadTown && m_Wilderness) continue;
                                if (waypoint.WaypointType == UOACZWayPoint.UOACZWaypointType.UndeadWilderness && !m_Wilderness) continue;

                                if (waypoint.SplitGroup == m_GotoSplitGroup)
                                {
                                    int distanceToWaypoint = Utility.GetDistance(creature.Location, waypoint.Location);

                                    if (distanceToWaypoint <= closestWaypointDistance)
                                    {
                                        closestWaypoint = waypoint;
                                        closestWaypointDistance = distanceToWaypoint;
                                    }
                                }
                            }

                            if (closestWaypoint != null)
                                creature.CurrentWaypoint = closestWaypoint;
                        }
                    }
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
            writer.Write((int)1); //version
            
            //Version 0
            writer.Write(m_Mobiles.Count);
            for (int a = 0; a < m_Mobiles.Count; a++)
            {
                writer.Write(m_Mobiles[a]);
            }

            writer.Write(m_Wilderness);

            //Version 1
            writer.Write(m_GotoSplitGroup);
            writer.Write(m_AllowTeleportToBaseNode);
            writer.Write(m_MinThreatLevel);
            writer.Write(m_MaxThreatLevel);
            writer.Write(m_GuardHome);
            writer.Write(m_GuardHomeRange);
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
            }   
           
            //Version 1
            if (version >= 1)
            {
                m_GotoSplitGroup = reader.ReadInt();
                m_AllowTeleportToBaseNode = reader.ReadBool();
                m_MinThreatLevel = reader.ReadInt();
                m_MaxThreatLevel = reader.ReadInt();
                m_GuardHome = reader.ReadBool();
                m_GuardHomeRange = reader.ReadInt();
            }

            //----------

            m_Spawners.Add(this);
        }
    }
}