﻿using System;
using Server;
using System.Collections;
using System.Collections.Generic;
using Server.Commands;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Accounting;
using Server.Custom;

namespace Server
{
    public static class OceanHotspotPersistance
    {
        public static OceanHotspotPersistanceItem PersistanceItem;     

        private static bool m_Enabled = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public static bool Enabled
        {
            get { return m_Enabled; }
            set { m_Enabled = value; }
        }

        public static TimeSpan ScheduledHotspotInterval = TimeSpan.FromHours(24);
        public static TimeSpan ScheduledHotspotAnnouncementLeadtime = TimeSpan.FromMinutes(10);
        
        public static DateTime m_NextScheduledSessionStartTime = DateTime.MaxValue;
        public static DateTime m_CurrentHotspotStartTime = DateTime.MaxValue;

        public static Timer m_Timer;

        public static void Initialize()
        {
            Timer.DelayCall(TimeSpan.FromTicks(1), delegate
            {
                if (PersistanceItem == null)
                    PersistanceItem = new OceanHotspotPersistanceItem();

                else if (PersistanceItem.Deleted)
                    PersistanceItem = new OceanHotspotPersistanceItem();

                CommandSystem.Register("StartScheduleInXMinutes", AccessLevel.GameMaster, new CommandEventHandler(StartScheduleInXMinutes));

                Timer.DelayCall(TimeSpan.FromSeconds(1), delegate
                {
                    if (Enabled)
                    {
                        if (m_Timer != null)
                        {
                            m_Timer.Stop();
                            m_Timer = null;
                        }

                        m_Timer = new OceanHotspotPersistanceTimer();
                        m_Timer.Start();
                    }
                });
            });
        }

        [Usage("StartScheduleInXMinutes")]
        [Description("Resets Scheduled Timer To Now + X Minutes As Starting Point")]
        public static void StartScheduleInXMinutes(CommandEventArgs arg)
        {
            PlayerMobile pm_Mobile = arg.Mobile as PlayerMobile;

            if (pm_Mobile == null)
                return;

            if (arg.Length == 1)
            {
                try
                {
                    int minutes = arg.GetInt32(0);

                    if (minutes <= 0)
                    {
                        pm_Mobile.SendMessage("Minutes must be more than 0.");
                        return;
                    }

                    m_NextScheduledSessionStartTime = DateTime.UtcNow + TimeSpan.FromMinutes(minutes);
                }

                catch
                {
                    pm_Mobile.SendMessage("Error in arguments. Usage: [StartScheduleInXMinutes minutes");
                    return;
                }
            }

            pm_Mobile.SendMessage("Current DateTime: " + DateTime.UtcNow.ToString());
            pm_Mobile.SendMessage("Schedule set to start at: " + m_NextScheduledSessionStartTime.ToString());
        }

        public class OceanHotspotPersistanceTimer : Timer
        {
            public OceanHotspotPersistanceTimer(): base(TimeSpan.Zero, TimeSpan.FromSeconds(5))
            {
                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                if (!OceanHotspotPersistance.Enabled)                
                    return;

                if (DateTime.UtcNow >= m_NextScheduledSessionStartTime)
                    StartSession();                
            }
        }

        public static void StartSession()
        {
            OceanHotspotPersistance.m_NextScheduledSessionStartTime = OceanHotspotPersistance.m_NextScheduledSessionStartTime + ScheduledHotspotInterval;
            m_CurrentHotspotStartTime = DateTime.UtcNow;

            OceanHotspot.HotspotLocationType eventLocation = OceanHotspot.GetRandomLocation();
            OceanHotspot.HotspotEventType eventType = OceanHotspot.GetRandomEventType();            

            OceanHotspotLocationDetail eventLocationDetail = OceanHotspot.GetHotspotLocationDetail(eventLocation);
            OceanHotspotEventTypeDetail eventTypeDetail = OceanHotspot.GetHotspotEventTypeDetail(eventType);

            OceanHotspot oceanHotspot = new OceanHotspot();

            oceanHotspot.HotspotType = eventType;
            oceanHotspot.GreyZone = true;
            oceanHotspot.Duration = 60;            
            oceanHotspot.TopLeftAreaPoint = eventLocationDetail.m_TopLeftAreaPoint;
            oceanHotspot.BottomRightAreaPoint = eventLocationDetail.m_BottomRightAreaPoint;

            oceanHotspot.MoveToWorld(eventLocationDetail.m_ControlObjectLocation, eventLocationDetail.map);

            oceanHotspot.StartEvent();
        }

        public static void EndSession()
        {           
        }

        public static void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); //Version

            //Version 0   
            writer.Write(m_Enabled);          
            writer.Write(m_NextScheduledSessionStartTime);         
            writer.Write(m_CurrentHotspotStartTime);
        }

        public static void Deserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            //Version 0
            if (version >= 0)
            {
                m_Enabled = reader.ReadBool();  
                m_NextScheduledSessionStartTime = reader.ReadDateTime();              
                m_CurrentHotspotStartTime = reader.ReadDateTime();
            }        
        }
    }

    public class OceanHotspotPersistanceItem : Item
    {
        public override string DefaultName { get { return "OceanHotspotPersistance"; } }

        public OceanHotspotPersistanceItem(): base(0x0)
        {
            Movable = false;
        }

        public OceanHotspotPersistanceItem(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            OceanHotspotPersistance.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Movable = false;

            OceanHotspotPersistance.PersistanceItem = this;
            OceanHotspotPersistance.Deserialize(reader);
        }
    }    
}
