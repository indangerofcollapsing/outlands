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
    public class OceanHotspot : Hotspot
    {
        public static List<OceanHotspot> m_Instances = new List<OceanHotspot>();

        public enum HotspotLocationType
        {
            BuccaneersDen
        }

        public enum HotspotEventType
        {
            KingOfTheHill
        }

        public static OceanHotspotLocationDetail GetHotspotLocationDetail(HotspotLocationType hotspotLocation)
        {
            OceanHotspotLocationDetail locationDetail = new OceanHotspotLocationDetail();

            switch (hotspotLocation)
            {
                case HotspotLocationType.BuccaneersDen:
                    locationDetail.m_Name = "Buccaneer's Den";
                    locationDetail.m_Description = new string[] { "" };
                    locationDetail.m_ControlObjectLocation = new Point3D(2000, 2000, 0);

                    locationDetail.m_TopLeftAreaPoint = new Point3D(1500, 1500, 0);
                    locationDetail.m_TopLeftAreaPoint = new Point3D(2500, 2500, 0);
                break;
            }

            return locationDetail;
        }

        public static OceanHotspotEventTypeDetail GetHotspotEventTypeDetail(HotspotEventType eventType)
        {
            OceanHotspotEventTypeDetail eventTypeDetail = new OceanHotspotEventTypeDetail();

            switch (eventType)
            {
                case HotspotEventType.KingOfTheHill:
                    eventTypeDetail.m_Name = "King of the Hill";
                    eventTypeDetail.m_Description = new string[] { "" };                    
                break;
            }

            return eventTypeDetail;
        }

        #region Properties

        private double m_ContestedPointsPerMinute = 1;
        [CommandProperty(AccessLevel.GameMaster)]
        public double ContestedPointsPerMinute
        {
            get { return m_ContestedPointsPerMinute; }
            set { m_ContestedPointsPerMinute = value; }
        }

        private double m_ControlledPointsPerMinute = 2;
        [CommandProperty(AccessLevel.GameMaster)]
        public double ControlledPointsPerMinute
        {
            get { return m_ControlledPointsPerMinute; }
            set { m_ControlledPointsPerMinute = value; }
        }        

        private HotspotEventType m_HotspotType = HotspotEventType.KingOfTheHill;
        [CommandProperty(AccessLevel.GameMaster)]
        public HotspotEventType HotspotType
        {
            get { return m_HotspotType; }
            set { m_HotspotType = value; }
        }

        public List<OceanHotspotParticipantEntry> m_Participants = new List<OceanHotspotParticipantEntry>();

        #endregion

        [Constructable]
        public OceanHotspot(): base()
        {
            Name = "ocean hotspot";

            ItemID = 4014;

            Movable = false;
            Visible = false;           

            //-----

            m_Instances.Add(this);
        }

        public void SetEventRules()
        {
            switch (m_HotspotType)
            {
                case HotspotEventType.KingOfTheHill:
                    EventTickInterval = 15;

                    ContestedPointsPerMinute = 1;
                    ControlledPointsPerMinute = 2;
                break;
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);
        }

        public static HotspotLocationType GetRandomLocation()
        {
            HotspotLocationType eventLocation = (HotspotLocationType)Utility.RandomMinMax(0, Enum.GetNames(typeof(HotspotLocationType)).Length - 1);

            return eventLocation;
        }

        public static HotspotEventType GetRandomEventType()
        {
            HotspotEventType eventType = (HotspotEventType)Utility.RandomMinMax(0, Enum.GetNames(typeof(HotspotEventType)).Length - 1);

            return eventType;
        }

        public override void StartEvent()
        {
            base.StartEvent();                     
        }

        public override void StopEvent(bool completed)
        {
            if (m_Instances.Contains(this))
                m_Instances.Remove(this);

            if (completed)
            {
                RoundValues();
                //Store Scores
                //Announcements
                DistributeRewards();

                Delete();
            }

            base.StopEvent(completed);         
        }

        public void RoundValues()
        {
            foreach (OceanHotspotParticipantEntry entry in m_Participants)
            {
                if (entry == null) continue;
                if (entry.m_Boat == null) continue;
                if (entry.m_Boat.Deleted) continue;
                if (entry.m_Boat.m_SinkTimer != null) continue;

                entry.m_Points = Math.Ceiling(entry.m_Points);                    
            }
        }

        public void DistributeRewards()
        {
            Dictionary<BaseBoat, int> m_ValidParticipants = new Dictionary<BaseBoat, int>();

            foreach (OceanHotspotParticipantEntry entry in m_Participants)
            {
                if (entry.m_Boat == null) continue;
                if (entry.m_Boat.Deleted) continue;
                if (entry.m_Boat.m_SinkTimer != null) continue;

                m_ValidParticipants.Add(entry.m_Boat, (int)entry.m_Points);
            }

            int TotalValues = 0;
            foreach (KeyValuePair<BaseBoat, int> pair in m_ValidParticipants)
            {
                TotalValues += pair.Value;
            }

            double ItemCheck = Utility.RandomDouble();

            double CumulativeAmount = 0.0;
            double AdditionalAmount = 0.0;

            //Determine Reward                      
            foreach (KeyValuePair<BaseBoat, int> pair in m_ValidParticipants)
            {
                AdditionalAmount = (double)pair.Value / (double)TotalValues;

                if (ItemCheck >= CumulativeAmount && ItemCheck < (CumulativeAmount + AdditionalAmount))
                {
                    BaseBoat winner = pair.Key;

                    //Determine Reward

                    break;
                }

                CumulativeAmount += AdditionalAmount;
            }
        }

        public override void EventTick()
        {
            base.EventTick();            

            double pointsTickScalar = EventTickInterval / 60;

            List<BaseBoat> m_Boats = new List<BaseBoat>();

            foreach (BaseBoat boat in BaseBoat.m_Instances)
            {
                if (boat == null) continue;
                if (boat.Deleted) continue;
                if (boat.m_SinkTimer != null) continue;
                if (boat.MobileControlType != MobileControlType.Player) continue;                
                if (!(Utility.IsInArea(boat.Location, TopLeftAreaPoint, BottomRightAreaPoint))) continue;               

                m_Boats.Add(boat);
            }

            foreach (BaseBoat boat in m_Boats)
            {
                OceanHotspotParticipantEntry entry = GetParticipationEntry(boat);

                if (m_Boats.Count == 1)
                    entry.m_Points += (pointsTickScalar * ControlledPointsPerMinute);

                else
                    entry.m_Points += (pointsTickScalar * ContestedPointsPerMinute);
            }
        }

        public OceanHotspotParticipantEntry GetParticipationEntry(BaseBoat boat)
        {
            foreach (OceanHotspotParticipantEntry entry in m_Participants)
            {
                if (entry == null) continue;

                if (entry.m_Boat == boat)
                    return entry;
            }

            OceanHotspotParticipantEntry newEntry = new OceanHotspotParticipantEntry(boat);

            m_Participants.Add(newEntry);

            return newEntry;
        }

        public override void OnDelete()
        {
 	        base.OnDelete();
        }

        public OceanHotspot(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            //Version 0
            writer.Write(m_ContestedPointsPerMinute);
            writer.Write(m_ControlledPointsPerMinute);
            writer.Write((int)m_HotspotType);

            writer.Write(m_Participants.Count);
            for (int a = 0; a < m_Participants.Count; a++)
            {
                writer.Write(m_Participants[a].m_Boat);
                writer.Write(m_Participants[a].m_Points);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_ContestedPointsPerMinute = reader.ReadDouble();
                m_ControlledPointsPerMinute = reader.ReadDouble();
                m_HotspotType = (HotspotEventType)reader.ReadInt();

                int participantsCount = reader.ReadInt();
                for (int a = 0; a < participantsCount; a++)
                {
                    BaseBoat boat = (BaseBoat)reader.ReadItem();
                    int points = reader.ReadInt();

                    OceanHotspotParticipantEntry entry = new OceanHotspotParticipantEntry(boat);
                    entry.m_Points = points;

                    m_Participants.Add(entry);
                }
            }

            //-----

            if (!m_Instances.Contains(this))
                m_Instances.Add(this);
        }
    }

    public class OceanHotspotParticipantEntry
    {
        public BaseBoat m_Boat;
        public double m_Points = 0;

        public OceanHotspotParticipantEntry(BaseBoat boat)
        {
            m_Boat = boat;
        }
    }

    public class OceanHotspotLocationDetail
    {
        public string m_Name = "Buccaneer's Den";
        public string[] m_Description = new string[] { "" };

        public Point3D m_ControlObjectLocation = new Point3D(2000, 2000, 0);
        public Point3D m_TopLeftAreaPoint = new Point3D(1500, 1500, 0);
        public Point3D m_BottomRightAreaPoint = new Point3D(2500, 2500, 0);

        public Map map = Map.Felucca;
    }

    public class OceanHotspotEventTypeDetail
    {
        public string m_Name = "King of the Hill";
        public string[] m_Description = new string[] { "" };
    }
}