using System;
using Server;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
    public class UOACZWayPoint : WayPoint
    {
        public static List<UOACZWayPoint> m_UOACZWaypoints = new List<UOACZWayPoint>();

        public enum UOACZWaypointType
        {
            UndeadWilderness,
            UndeadTown,
            HumanWilderness,
            HumanTown
        }

        public UOACZWaypointType m_WaypointType = UOACZWaypointType.UndeadWilderness;
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public UOACZWaypointType WaypointType
        {
            get { return m_WaypointType; }
            set 
            {
                m_WaypointType = value;

                UpdateConfiguration();
            }
        }

        public bool m_IsBaseNode = false;
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public bool IsBaseNode
        {
            get { return m_IsBaseNode; }
            set
            {
                m_IsBaseNode = value;

                UpdateConfiguration();
            }
        }

        public bool m_AllowJumpTo = true;
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public bool AllowJumpTo
        {
            get { return m_AllowJumpTo; }
            set
            { 
                m_AllowJumpTo = value;

                UpdateConfiguration();
            }
        }

        public int m_GotoSplitGroup = -1;
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public int GotoSplitGroup
        {
            get { return m_GotoSplitGroup; }
            set
            {
                m_GotoSplitGroup = value;
            }
        }

        public int m_SplitGroup = -1;
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public int SplitGroup
        {
            get { return m_SplitGroup; }
            set
            {
                m_SplitGroup = value;

                UpdateConfiguration();
            }
        }

        [Constructable]
        public UOACZWayPoint(): base()		
		{
            Name = "UOACZ Waypoint";
            Visible = true;

            UpdateConfiguration();

            m_UOACZWaypoints.Add(this);
		}
        
        public override bool CanBeSeenBy(Mobile from)
        {
            if (from.AccessLevel > AccessLevel.Player)
                return true;

            return false;
        }

        public override void OnSingleClick(Mobile from)
        {
            if (from.AccessLevel == AccessLevel.Player)
                return;

            switch (Behavior)
            {
                case WaypointBehavior.NextWaypoint:
                    LabelTo(from, "Waypoint: Next");
                break;

                case WaypointBehavior.ReverseOrder:
                    LabelTo(from, "Waypoint: Reverse Order");
                break;

                case WaypointBehavior.ReturnToStart:
                    LabelTo(from, "Waypoint: Return to Start");
                break;

                case WaypointBehavior.SetHomeHere:
                    LabelTo(from, "Waypoint: Set Home Here");
                break;

                case WaypointBehavior.Idle10secThenNextWaypoint:
                    LabelTo(from, "Waypoint: 10s Next");
                break;

                case WaypointBehavior.Idle30secThenNextWaypoint:
                    LabelTo(from, "Waypoint: 30s Next");
                break;

                case WaypointBehavior.Idle10secThenReverseOrder:
                    LabelTo(from, "Waypoint: 10s Reverse");
                break;

                case WaypointBehavior.Idle30secThenReverseOrder:
                    LabelTo(from, "Waypoint: 30s Reverse");
                    break;
            }

            string splitGroupText = "";

            if (m_GotoSplitGroup > -1)
                splitGroupText += "Goto Split: " + m_GotoSplitGroup.ToString() + " ";

            if (m_SplitGroup > -1)
                splitGroupText += "Split Group is: " + m_SplitGroup.ToString();

            if (splitGroupText != "")
                LabelTo(from, splitGroupText);           

            LabelTo(from, "Previous Waypoints: " + PreviousWaypoints.Count);

            if (m_GotoSplitGroup > -1)
            {
                int splitWaypointTargets = 0;

                from.SendMessage(2550, "Waypoint split destinations at:");

                foreach (UOACZWayPoint waypoint in m_UOACZWaypoints)
                {
                    if (!UOACZRegion.ContainsItem(waypoint)) continue;

                    if (waypoint.m_WaypointType == m_WaypointType && waypoint.m_SplitGroup == m_GotoSplitGroup)
                    {
                        splitWaypointTargets++;
                        from.SendMessage(waypoint.Location.X.ToString() + "," + waypoint.Location.Y.ToString() + "," + waypoint.Location.Z.ToString());
                    }
                }

                from.SendMessage(2550, splitWaypointTargets.ToString() + " potential split destinations.");
            }

            else
            {
                if (NextWaypoint != null)
                    from.SendMessage(2550, "Next Waypoint at: " + NextWaypoint.Location.X.ToString() + ", " + NextWaypoint.Location.Y.ToString() + ", " + NextWaypoint.Location.Z.ToString());
            }
        }

        public void UpdateConfiguration()
        {
            switch(m_WaypointType)
            {
                case UOACZWaypointType.UndeadWilderness: Hue = 2117; break;
                case UOACZWaypointType.UndeadTown: Hue = 2116; break;
                case UOACZWaypointType.HumanWilderness: Hue = 2615; break;
                case UOACZWaypointType.HumanTown: Hue = 2220; break;
            }

            ItemID = 6178;

            if (m_IsBaseNode)
                ItemID = 6180;

            else if (m_SplitGroup > -1)
                ItemID = 6182;

            else if (!m_AllowJumpTo)
                ItemID = 6179;            
        }


        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_UOACZWaypoints.Contains(this))
                m_UOACZWaypoints.Remove(this);
        }

        public UOACZWayPoint(Serial serial): base(serial)
        {   
        }        
        
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version

            //Version 0
            writer.Write(m_AllowJumpTo);
            writer.Write(m_IsBaseNode);
            writer.Write((int)m_WaypointType);
            writer.Write(m_GotoSplitGroup);
            writer.Write(m_SplitGroup);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_AllowJumpTo = reader.ReadBool();
                m_IsBaseNode = reader.ReadBool();
                m_WaypointType = (UOACZWaypointType)reader.ReadInt();
                m_GotoSplitGroup = reader.ReadInt();
                m_SplitGroup = reader.ReadInt();
            }

            //-----

            m_UOACZWaypoints.Add(this);
        }
    }
}