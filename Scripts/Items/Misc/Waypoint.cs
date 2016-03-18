using System;
using Server;
using Server.Targeting;
using Server.Commands;
using Server.Mobiles;
using Server.Network;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
    public enum WayPointOrder
    {
        Forward,
        Reverse,
        IdleForward,
        IdleReverse
    }
    
    public enum WaypointBehavior
    {
        NextWaypoint,
        ReverseOrder,
        ReturnToStart,
        SetHomeHere,        
        Idle10secThenNextWaypoint,
        Idle30secThenNextWaypoint,        
        Idle10secThenReverseOrder,
        Idle30secThenReverseOrder
    }
    
    public class WayPoint : Item
    {
        private WaypointBehavior m_Behavior;
        private bool m_DoWaypointAction = false;
        private WayPoint m_NextWaypoint;
        private List<WayPoint> m_PreviousWaypoints;
        private List<XmlSpawner> m_XMLSpawner;
        private string m_Description;

        public static void Initialize()
        {
            CommandSystem.Register("GotoNextWaypoint", AccessLevel.Seer, new CommandEventHandler(GotoNextWaypoint));
            CommandSystem.Register("ShowAllWaypoints", AccessLevel.Seer, new CommandEventHandler(ShowAllWaypoints));
            CommandSystem.Register("HideAllWaypoints", AccessLevel.Seer, new CommandEventHandler(HideAllWaypoints));
        }

        [Usage("GotoNextWaypoint")]
        [Description("Teleports player to the next waypoint of a waypoint or XMLSpawner")]
        public static void GotoNextWaypoint(CommandEventArgs arg)
        {
            arg.Mobile.SendMessage("Target a XMLSpawner or WayPoint");
            arg.Mobile.Target = new GotoNextWaypointTarget();
        }

        [Usage("ShowAllWaypoints")]
        [Description("Sets all waypoints to visible: can see their hue")]
        public static void ShowAllWaypoints(CommandEventArgs e)
        {
            List<Item> ToShow = new List<Item>();
            foreach (Item item in World.Items.Values)
            {
                if (item is WayPoint)
                {
                    item.Visible = true;
                }
            }
        }

        [Usage("HideAllWaypoints")]
        [Description("Sets all waypoints to invisible: turns them grey")]
        public static void HideAllWaypoints(CommandEventArgs e)
        {
            List<Item> ToHide = new List<Item>();
            foreach (Item item in World.Items.Values)
            {
                if (item is WayPoint)
                {
                    item.Visible = false;
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public WaypointBehavior Behavior
        {
            get
            {
                return m_Behavior;
            }

            set
            {

                m_Behavior = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool DoWaypointAction
        {
            get
            {
                return m_DoWaypointAction;
            }

            set
            {

                m_DoWaypointAction = value;
            }
        } 

        [CommandProperty(AccessLevel.GameMaster)]
        public WayPoint NextWaypoint
        {
            get
            {
                return m_NextWaypoint;
            }

            set
            {                
                m_NextWaypoint = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public List<WayPoint> PreviousWaypoints
        {
            get
            {
                return m_PreviousWaypoints;
            }

            set
            {
                m_PreviousWaypoints = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public List <XmlSpawner> XMLSpawners
        {
            get
            {
                return m_XMLSpawner;
            }

            set
            {
                m_XMLSpawner = value;
            }
        }      

        [CommandProperty(AccessLevel.GameMaster)]
        public string Description
        {
            get
            {
                return m_Description;
            }

            set
            {
                m_Description = value;
            }
        }

        [Constructable]
        public WayPoint()
            : base(0x1822)
        {
            this.Visible = false;
            this.Movable = false;

            m_PreviousWaypoints = new List<WayPoint>();
            m_XMLSpawner = new List<XmlSpawner>();
        }

        public override void OnSingleClick(Mobile from)
        {
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

            if (m_NextWaypoint != null)
            {
                LabelTo(from, "Next=True Action=" + m_DoWaypointAction.ToString());
                
                from.SendMessage("Next Waypoint at: " + m_NextWaypoint.Location.X.ToString() + "," + m_NextWaypoint.Location.Y.ToString() + "," + m_NextWaypoint.Location.Z.ToString());
            }

            else
            {
                LabelTo(from, "Next=False Action=" + m_DoWaypointAction.ToString());
            }

            int iPreviousWaypoints = m_PreviousWaypoints.Count;
            int iXMLSpawners = m_XMLSpawner.Count;

            LabelTo(from, "Previous: " + iPreviousWaypoints.ToString() + " Spawners: " + iXMLSpawners.ToString());

            if (iPreviousWaypoints == 0 && iXMLSpawners == 0)
            {
                from.SendMessage(0x8A, "*Warning: Waypoint has no previous waypoint or XMLSpawner*");                
            }         
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
            {
                from.SendMessage("Target a waypoint or XMLSpawner");
                from.Target = new NextPointTarget(this);
            }
        }

        public WayPoint(Serial serial)
            : base(serial)
        {
        }

        public override void OnDelete()
        {
            base.OnDelete();

            //Remove Reference to this Waypoint from All Previous Waypoints
            if (m_PreviousWaypoints != null)
            {
                foreach (WayPoint waypoint in m_PreviousWaypoints)
                {
                    if (waypoint != null)
                        waypoint.m_NextWaypoint = null;
                }
            }
            
            //Remove Reference to this Waypoint from Connected Next Waypoint
            if (m_NextWaypoint != null)
            {
                if (m_NextWaypoint.PreviousWaypoints != null)
                {
                    if (m_NextWaypoint.PreviousWaypoints.IndexOf(this) >= 0)
                    {
                        m_NextWaypoint.PreviousWaypoints.Remove(this);
                    }
                }
            }

            //Remove Reference to this WayPoint from Any Connected XMLSpawners
            if (m_XMLSpawner != null)
            {
                foreach (XmlSpawner xmlSpawner in m_XMLSpawner)
                {
                    if (xmlSpawner != null)
                        xmlSpawner.WayPoint = null;
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); //Version

            writer.Write((int)m_Behavior);           
            writer.Write(m_NextWaypoint);

            int previousWaypoints = m_PreviousWaypoints.Count;

            writer.Write(previousWaypoints);
            for (int a = 0; a < previousWaypoints; a++)
            {
                writer.Write(m_PreviousWaypoints[a]);
            }

            int xmlSpawners = m_XMLSpawner.Count;

            writer.Write(xmlSpawners);
            for (int a = 0; a < xmlSpawners; a++)
            {
                writer.Write(m_XMLSpawner[a]);
            }

            writer.Write(m_Description); 
           
            //Version 1
            writer.Write((bool)m_DoWaypointAction);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
          
            //Version 0
            m_Behavior = (WaypointBehavior)reader.ReadInt();
            m_NextWaypoint = reader.ReadItem() as WayPoint;

            m_PreviousWaypoints = new List<WayPoint>();
            int previousWaypoints = reader.ReadInt();
                   
            for (int a = 0; a < previousWaypoints; a++)
            {
                WayPoint previousWaypoint = reader.ReadItem() as WayPoint;

                if (m_PreviousWaypoints.IndexOf(previousWaypoint) < 0)
                {
                    m_PreviousWaypoints.Add(previousWaypoint);
                }
            }

            m_XMLSpawner = new List<XmlSpawner>();
            int xmlSpawners = reader.ReadInt();

            for (int a = 0; a < xmlSpawners; a++)
            {
                XmlSpawner xmlspawner = reader.ReadItem() as XmlSpawner;

                if (m_XMLSpawner.IndexOf(xmlspawner) < 0)
                {
                    m_XMLSpawner.Add(xmlspawner);
                }
            }

            m_Description = reader.ReadString();                    
                        
            if (version >= 1)
            {
                m_DoWaypointAction = reader.ReadBool();
            }              
        }  
    }

    public class NextPointTarget : Target
    {
        private WayPoint m_ThisWaypoint;

        public NextPointTarget(WayPoint waypoint)
            : base(-1, false, TargetFlags.None)
        {
            m_ThisWaypoint = waypoint;
        }

        protected override void OnTarget(Mobile from, object target)
        {
            if (target is WayPoint && m_ThisWaypoint != null)
            {
                WayPoint nextWaypoint = target as WayPoint;

                if (m_ThisWaypoint == nextWaypoint)
                {
                    from.SendMessage("You cannot make a waypoint it's own waypoint");
                    return;
                }

                m_ThisWaypoint.NextWaypoint = nextWaypoint;
                nextWaypoint.Hue = m_ThisWaypoint.Hue;

                if (m_ThisWaypoint.Visible)
                    nextWaypoint.Visible = true;

                if (nextWaypoint.PreviousWaypoints.IndexOf(m_ThisWaypoint) < 0)
                {
                    nextWaypoint.PreviousWaypoints.Add(m_ThisWaypoint);
                }
            }

            else if (target is XmlSpawner && m_ThisWaypoint != null)
            {
                XmlSpawner xmlSpawner = target as XmlSpawner;

                xmlSpawner.WayPoint = m_ThisWaypoint;
                m_ThisWaypoint.Hue = xmlSpawner.Hue;

                if (xmlSpawner.Visible)
                    m_ThisWaypoint.Visible = true;

                if (m_ThisWaypoint.XMLSpawners.IndexOf(xmlSpawner) < 0)
                {
                    m_ThisWaypoint.XMLSpawners.Add(xmlSpawner);
                }
            }

            else
            {
                from.SendMessage("That is not a waypoint or XMLSpawner.");
            }
        }
    }    

    public class GotoNextWaypointTarget : Target
    {
        public GotoNextWaypointTarget()
            : base(12, false, TargetFlags.None)
        {
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (targeted is WayPoint)
            {
                WayPoint waypoint = targeted as WayPoint;

                if (waypoint.NextWaypoint != null)
                {
                    from.Location = waypoint.NextWaypoint.Location;
                }   
            }

            else if (targeted is XmlSpawner)
            {
                XmlSpawner xmlspawner = targeted as XmlSpawner;

                if (xmlspawner.WayPoint != null)
                {
                    from.Location = xmlspawner.WayPoint.Location;
                }   
            }
        }
    }
}
