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
    public class GreyZoneTotem : Item
    {
        public static List<GreyZoneTotem> m_Instances = new List<GreyZoneTotem>();
        
        private bool m_Active = true;
        [CommandProperty(AccessLevel.Seer)]
        public bool Active
        {
            get { return m_Active; }
            set { m_Active = value; }
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

        [Constructable]
        public GreyZoneTotem() : base(16954)
        {
            Name = "Grey Zone Totem";

            Hue = 2635;

            Visible = false;
            Movable = false;
        }

        public static bool InGreyZoneTotemArea(Point3D location, Map map)
        {
            bool greyZoneFound = false;

            foreach (GreyZoneTotem greyZoneTotem in GreyZoneTotem.m_Instances)
            {
                if (greyZoneTotem == null) continue;
                if (greyZoneTotem.Deleted) continue;

                if (!greyZoneTotem.Active) continue;
                if (map != greyZoneTotem.Map) continue;

                if (Utility.IsInArea(location, greyZoneTotem.m_TopLeftAreaPoint, greyZoneTotem.m_BottomRightAreaPoint))                
                    return true;
            }

            return greyZoneFound;
        }

        public override void OnSingleClick(Mobile from)
        {
 	        base.OnSingleClick(from);

            LabelTo(from, "Active: " + m_Active.ToString());
            LabelTo(from, "Area: " + m_TopLeftAreaPoint.ToString() + " to " + m_BottomRightAreaPoint.ToString());
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnSingleClick(from);
        }        

        public GreyZoneTotem(Serial serial) : base(serial) 
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

            writer.Write(m_Active);
            writer.Write(m_TopLeftAreaPoint);
            writer.Write(m_BottomRightAreaPoint);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Active = reader.ReadBool();
            m_TopLeftAreaPoint = reader.ReadPoint3D();
            m_BottomRightAreaPoint = reader.ReadPoint3D();

            //---------

            m_Instances.Add(this);
        }
    }    
}
