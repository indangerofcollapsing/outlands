using System;
using Server;
using System.Collections.Generic;
using Server.Network;
using Server.Prompts;
using Server.Multis;
using Server.Mobiles;   
using Server.Targeting;
using Server.Gumps;
using Server.Spells;
using Server.Items;

namespace Server.Custom
{
    public class Hotspot : Item
    {       
        public static List<Hotspot> m_Instances = new List<Hotspot>();               

        #region Properties        

        private bool m_Active = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active
        {
            get { return m_Active; }
            set { m_Active = value; }
        }

        private bool m_GreyZone = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool GreyZone
        {
            get { return m_GreyZone; }
            set { m_GreyZone = value; }
        }

        private DateTime m_Expiration = DateTime.UtcNow + TimeSpan.FromMinutes(BaseDuration);
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime Expiration
        {
            get { return m_Expiration; }
            set { m_Expiration = value; }
        }

        public static int BaseDuration = 60; //in Minutes

        private double m_Duration = BaseDuration; 
        [CommandProperty(AccessLevel.GameMaster)]
        public double Duration
        {
            get { return m_Duration; }
            set { m_Duration = value; }
        }

        public DateTime NextTickEvent = DateTime.UtcNow + TimeSpan.FromSeconds(BaseTickInterval);
        public static int BaseTickInterval = 60; //in Seconds

        private double m_EventTickInterval = BaseTickInterval;
        [CommandProperty(AccessLevel.GameMaster)]
        public double EventTickInterval
        {
            get { return m_EventTickInterval; }
            set { m_EventTickInterval = value; }
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

        private Timer m_Timer;

        #endregion Properties

        [Constructable]
        public Hotspot(): base()
        {
            Name = "hotspot";

            ItemID = 4014;

            Movable = false;
            Visible = false;

            //-----

            m_Instances.Add(this);

            m_Timer = new InternalTimer(this);
            m_Timer.Start();
        }        

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);
        }

        public static bool InHotspotArea(Point3D location, Map map, bool greyZoneOnly)
        {
            bool hotspotFound = false;

            foreach (Hotspot hotspotInstance in Hotspot.m_Instances)
            {
                if (hotspotInstance == null) continue;
                if (hotspotInstance.Deleted) continue;

                if (!hotspotInstance.Active) continue;
                if (map != hotspotInstance.Map) continue;
                if (greyZoneOnly && !hotspotInstance.m_GreyZone) continue;

                if (Utility.IsInArea(location, hotspotInstance.m_TopLeftAreaPoint, hotspotInstance.m_BottomRightAreaPoint))
                    return true;
            }

            return hotspotFound;
        }

        public virtual void StartEvent()
        {
            m_Expiration = DateTime.UtcNow + TimeSpan.FromMinutes(m_Duration);  
            m_Active = true;           
        }

        public virtual void StopEvent(bool completed)
        {
            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }

            if (m_Instances.Contains(this))
                m_Instances.Remove(this);

            if (completed)
            {
                EventCompleted();

                Delete();
            }
        }

        public virtual void EventTick()
        {
            if (Deleted)
                return;
        }

        public virtual void CreatureKilled(PlayerMobile player, BaseCreature creature, double damagePercent)
        {          
        }

        public virtual bool EventCheckCompletion()
        {
            return false;
        }    

        public virtual void EventCompleted()
        {            
        }    

        private class InternalTimer : Timer
        {
            private Hotspot m_Hotspot;

            public InternalTimer(Hotspot Hotspot): base(TimeSpan.Zero, TimeSpan.FromMinutes(1))
            {
                m_Hotspot = Hotspot;

                Priority = TimerPriority.FiveSeconds;         
            }

            protected override void OnTick()
            {
                if (m_Hotspot == null)
                {
                    Stop();
                    return;
                }

                if (m_Hotspot.Deleted)
                {
                    Stop();
                    return;
                }

                if (!m_Hotspot.m_Active)
                    return;

                if (m_Hotspot.EventCheckCompletion())
                {
                    Stop();

                    m_Hotspot.StopEvent(true);

                    return;
                }

                if (m_Hotspot.m_Expiration <= DateTime.UtcNow)
                {
                    Stop();

                    m_Hotspot.StopEvent(true);                   

                    return;
                }

                if (m_Hotspot.NextTickEvent <= DateTime.UtcNow)
                {
                    m_Hotspot.NextTickEvent = m_Hotspot.NextTickEvent + TimeSpan.FromSeconds(m_Hotspot.EventTickInterval);
                    m_Hotspot.EventTick();
                }
            }
        }

        public override void OnDelete()
        {
            StopEvent(false);

            foreach (Item item in World.Items.Values)
            {
                if (item is XmlSpawner)
                {
                    XmlSpawner spawner = item as XmlSpawner;

                    if (spawner.Hotspot == this)
                        spawner.Hotspot = null;
                }
            }

            if (m_Instances.Contains(this))
                m_Instances.Remove(this);

 	        base.OnDelete();
        }

        public Hotspot(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            //Version 0 
            writer.Write(m_Active);
            writer.Write(m_GreyZone);
            writer.Write(m_Expiration);
            writer.Write(m_Duration);
            writer.Write(m_EventTickInterval);
            writer.Write(m_TopLeftAreaPoint);
            writer.Write(m_BottomRightAreaPoint);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Active = reader.ReadBool();
                m_GreyZone = reader.ReadBool();
                m_Expiration = reader.ReadDateTime();
                m_Duration = reader.ReadDouble();
                m_EventTickInterval = reader.ReadDouble();
                m_TopLeftAreaPoint = reader.ReadPoint3D();
                m_BottomRightAreaPoint = reader.ReadPoint3D();                                          
            }

            //-----

            if (!m_Instances.Contains(this))
                m_Instances.Add(this);

            m_Timer = new InternalTimer(this);
            m_Timer.Start();
        }
    }    
}