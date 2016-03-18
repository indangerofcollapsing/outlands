using Server.Guilds;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Server
{
    public class WarpBlockerTotem : Item
    {
        public static List<WarpBlockerTotem> m_Instances = new List<WarpBlockerTotem>();

        public static string DefaultRecallInResponse = "A mysterious force prevents your spell from working here.";
        public static string DefaultRecallOutResponse = "A mysterious force prevents your spell from working here.";
        public static string DefaultGateInResponse = "A mysterious force prevents your spell from working here.";
        public static string DefaultGateOutResponse = "A mysterious force prevents your spell from working here.";
        public static string DefaultTeleportInResponse = "A mysterious force prevents your spell from working here.";
        public static string DefaultTeleportOutResponse = "A mysterious force prevents your spell from working here.";
        public static string DefaultGateResponse = "A mysterious force prevents your spell from working here.";
        public static string DefaultMarkResponse = "A mysterious force prevents your spell from working here.";

        public static string DefaultHelpStuckResponse = "A mysterious force prevents this ability from working here.";

        public enum MovementMode
        {
            RecallOut,
            RecallIn,
            GateOut,
            GateIn,
            TeleportOut,
            TeleportIn,            
            HelpStuck,
            Mark
        }

        #region Properties

        private bool m_Activated = true;
        [CommandProperty(AccessLevel.Seer)]
        public bool Activated
        {
            get { return m_Activated; }
            set { m_Activated = value; }
        }

        private Point3D m_TopLeftAreaPoint;
        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D TopLeftAreaPoint
        {
            get { return m_TopLeftAreaPoint; }
            set { m_TopLeftAreaPoint = value; }
        }

        private Point3D m_BottomRightAreaPoint;
        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D BottomRightAreaPoint
        {
            get { return m_BottomRightAreaPoint; }
            set { m_BottomRightAreaPoint = value; }
        }

        private bool m_PreventRecallIn = true;
        [CommandProperty(AccessLevel.Seer)]
        public bool PreventRecallIn
        {
            get { return m_PreventRecallIn; }
            set { m_PreventRecallIn = value; }
        }

        private bool m_PreventRecallOut = true;
        [CommandProperty(AccessLevel.Seer)]
        public bool PreventRecallOut
        {
            get { return m_PreventRecallOut; }
            set { m_PreventRecallOut = value; }
        }

        private bool m_PreventGateIn = true;
        [CommandProperty(AccessLevel.Seer)]
        public bool PreventGateIn
        {
            get { return m_PreventGateIn; }
            set { m_PreventGateIn = value; }
        }

        private bool m_PreventGateOut = true;
        [CommandProperty(AccessLevel.Seer)]
        public bool PreventGateOut
        {
            get { return m_PreventGateOut; }
            set { m_PreventGateOut = value; }
        }
        
        private bool m_PreventTeleportIn = true;
        [CommandProperty(AccessLevel.Seer)]
        public bool PreventTeleportIn
        {
            get { return m_PreventTeleportIn; }
            set { m_PreventTeleportIn = value; }
        }

        private bool m_PreventTeleportOut = true;
        [CommandProperty(AccessLevel.Seer)]
        public bool PreventTeleportOut
        {
            get { return m_PreventTeleportOut; }
            set { m_PreventTeleportOut = value; }
        }

        private bool m_PreventHelpStuck = true;
        [CommandProperty(AccessLevel.Seer)]
        public bool PreventHelpStuck
        {
            get { return m_PreventHelpStuck; }
            set { m_PreventHelpStuck = value; }
        }

        private bool m_PreventMark = true;
        [CommandProperty(AccessLevel.Seer)]
        public bool PreventMark
        {
            get { return m_PreventMark; }
            set { m_PreventMark = value; }
        }

        //Response Message Overrides
        private string m_PreventRecallInResponse = "";
        [CommandProperty(AccessLevel.Seer)]
        public string PreventRecallInResponse
        {
            get { return m_PreventRecallInResponse; }
            set { m_PreventRecallInResponse = value; }
        }        

        private string m_PreventRecallOutResponse = "";
        [CommandProperty(AccessLevel.Seer)]
        public string PreventRecallOutResponse
        {
            get { return m_PreventRecallOutResponse; }
            set { m_PreventRecallOutResponse = value; }
        }        

        private string m_PreventGateInResponse = "";
        [CommandProperty(AccessLevel.Seer)]
        public string PreventGateInResponse
        {
            get { return m_PreventGateInResponse; }
            set { m_PreventGateInResponse = value; }
        }        

        private string m_PreventGateOutResponse = "";
        [CommandProperty(AccessLevel.Seer)]
        public string PreventGateOutResponse
        {
            get { return m_PreventGateOutResponse; }
            set { m_PreventGateOutResponse = value; }
        }
        
        private string m_PreventTeleportInResponse = "";
        [CommandProperty(AccessLevel.Seer)]
        public string PreventTeleportInResponse
        {
            get { return m_PreventTeleportInResponse; }
            set { m_PreventTeleportInResponse = value; }
        }

        private string m_PreventTeleportOutResponse = "";
        [CommandProperty(AccessLevel.Seer)]
        public string PreventTeleportOutResponse
        {
            get { return m_PreventTeleportOutResponse; }
            set { m_PreventTeleportOutResponse = value; }
        }
        
        private string m_PreventHelpStuckResponse = "";
        [CommandProperty(AccessLevel.Seer)]
        public string PreventHelpStuckResponse
        {
            get { return m_PreventHelpStuckResponse; }
            set { m_PreventHelpStuckResponse = value; }
        }

        private string m_PreventMarkResponse = "";
        [CommandProperty(AccessLevel.Seer)]
        public string PreventMarkResponse
        {
            get { return m_PreventMarkResponse; }
            set { m_PreventMarkResponse = value; }
        }

        #endregion

        [Constructable]
        public WarpBlockerTotem() : base(16954)
        {
            Name = "Warp Blocker Totem";

            Hue = 2635;

            Visible = false;
            Movable = false;
        }

        public override void OnSingleClick(Mobile from)
        {
 	        base.OnSingleClick(from);

            LabelTo(from, "Active: " + m_Activated.ToString());
            LabelTo(from, "Area: " + m_TopLeftAreaPoint.ToString() + " to " + m_BottomRightAreaPoint.ToString());
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnSingleClick(from);
        }

        public static WarpBlockerTotem RecallBlockerTriggered(Mobile from, MovementMode movementMode, Point3D endLocation, Map endMap)
        {
            WarpBlockerTotem recallBlocker = null;

            foreach (WarpBlockerTotem targetRecallBlocker in WarpBlockerTotem.m_Instances)
            {
                if (targetRecallBlocker == null) continue;
                if (targetRecallBlocker.Deleted) continue;

                if (!targetRecallBlocker.Activated) continue;
                if (from.Map != targetRecallBlocker.Map) continue;
                
                switch (movementMode)
                {                      
                    case MovementMode.RecallOut: 
                        if (Utility.IsInArea(from.Location, targetRecallBlocker.m_TopLeftAreaPoint, targetRecallBlocker.m_BottomRightAreaPoint))
                            return targetRecallBlocker;
                    break;

                    case MovementMode.RecallIn:
                        if (Utility.IsInArea(endLocation, targetRecallBlocker.m_TopLeftAreaPoint, targetRecallBlocker.m_BottomRightAreaPoint))
                            return targetRecallBlocker;
                    break;

                    case MovementMode.GateOut:
                        if (Utility.IsInArea(from.Location, targetRecallBlocker.m_TopLeftAreaPoint, targetRecallBlocker.m_BottomRightAreaPoint))
                            return targetRecallBlocker;
                    break;

                    case MovementMode.GateIn:
                        if (Utility.IsInArea(endLocation, targetRecallBlocker.m_TopLeftAreaPoint, targetRecallBlocker.m_BottomRightAreaPoint))
                            return targetRecallBlocker;
                    break;

                    case MovementMode.TeleportOut:
                        if (Utility.IsInArea(from.Location, targetRecallBlocker.m_TopLeftAreaPoint, targetRecallBlocker.m_BottomRightAreaPoint))
                            return targetRecallBlocker;
                    break;

                    case MovementMode.TeleportIn:
                        if (Utility.IsInArea(endLocation, targetRecallBlocker.m_TopLeftAreaPoint, targetRecallBlocker.m_BottomRightAreaPoint))
                            return targetRecallBlocker;
                    break;

                    case MovementMode.HelpStuck:
                        if (Utility.IsInArea(from.Location, targetRecallBlocker.m_TopLeftAreaPoint, targetRecallBlocker.m_BottomRightAreaPoint))
                            return targetRecallBlocker;
                    break;

                    case MovementMode.Mark:
                        if (Utility.IsInArea(from.Location, targetRecallBlocker.m_TopLeftAreaPoint, targetRecallBlocker.m_BottomRightAreaPoint))
                            return targetRecallBlocker;
                    break;
                }
            }

            return recallBlocker;
        }

        public WarpBlockerTotem(Serial serial) : base(serial) 
        {
        }

        public override void OnDelete()
        {
            if (m_Instances.Contains(this))
                m_Instances.Remove(this);

            base.OnDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(m_Activated);
            writer.Write(m_TopLeftAreaPoint);
            writer.Write(m_BottomRightAreaPoint);
            writer.Write(m_PreventRecallIn);
            writer.Write(m_PreventRecallInResponse);
            writer.Write(m_PreventRecallOut);
            writer.Write(m_PreventRecallOutResponse);
            writer.Write(m_PreventGateIn);
            writer.Write(m_PreventGateInResponse);
            writer.Write(m_PreventGateOut);
            writer.Write(m_PreventGateOutResponse);
            writer.Write(m_PreventTeleportIn);
            writer.Write(m_PreventTeleportInResponse);
            writer.Write(m_PreventTeleportOut);
            writer.Write(m_PreventTeleportOutResponse);
            writer.Write(m_PreventHelpStuck);
            writer.Write(m_PreventHelpStuckResponse);
            writer.Write(m_PreventMark);
            writer.Write(m_PreventMarkResponse);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Activated = reader.ReadBool();
            m_TopLeftAreaPoint = reader.ReadPoint3D();
            m_BottomRightAreaPoint = reader.ReadPoint3D();
            m_PreventRecallIn = reader.ReadBool();
            m_PreventRecallInResponse = reader.ReadString();
            m_PreventRecallOut = reader.ReadBool();
            m_PreventRecallOutResponse = reader.ReadString();
            m_PreventGateIn = reader.ReadBool();
            m_PreventGateInResponse = reader.ReadString();
            m_PreventGateOut = reader.ReadBool();
            m_PreventGateOutResponse = reader.ReadString();
            m_PreventTeleportIn = reader.ReadBool();
            m_PreventTeleportInResponse = reader.ReadString();
            m_PreventTeleportOut = reader.ReadBool();
            m_PreventTeleportOutResponse = reader.ReadString();
            m_PreventHelpStuck = reader.ReadBool();
            m_PreventHelpStuckResponse = reader.ReadString();
            m_PreventMark = reader.ReadBool();
            m_PreventMarkResponse = reader.ReadString();

            //----------------

            m_Instances.Add(this);
        }
    }    
}
