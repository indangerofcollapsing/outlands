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
    public class InvasionHotspot : Hotspot
    {
        public static List<InvasionHotspot> m_Instances = new List<InvasionHotspot>();

        public enum HotspotLocationType
        {
            YewOrcFort
        }

        public enum HotspotEventType
        {
            Invasion
        }

        public static InvasionHotspotLocationDetail GetHotspotLocationDetail(HotspotLocationType hotspotLocation)
        {
            InvasionHotspotLocationDetail locationDetail = new InvasionHotspotLocationDetail();

            switch (hotspotLocation)
            {
                case HotspotLocationType.YewOrcFort:
                    locationDetail.m_Name = "Yew Orc Fort";
                    locationDetail.m_Description = new string[] { "" };
                    locationDetail.m_ControlObjectLocation = new Point3D(2000, 2000, 0);

                    locationDetail.m_TopLeftAreaPoint = new Point3D(1500, 1500, 0);
                    locationDetail.m_TopLeftAreaPoint = new Point3D(2500, 2500, 0);
                break;
            }

            return locationDetail;
        }

        public static InvasionHotspotEventTypeDetail GetHotspotEventTypeDetail(HotspotEventType eventType)
        {
            InvasionHotspotEventTypeDetail eventTypeDetail = new InvasionHotspotEventTypeDetail();

            switch (eventType)
            {
                case HotspotEventType.Invasion:
                    eventTypeDetail.m_Name = "Invasion";
                    eventTypeDetail.m_Description = new string[] { "" };                    
                break;
            }

            return eventTypeDetail;
        }

        #region Properties

        private double m_DifficultyValueToPointsRatio = .33;
        [CommandProperty(AccessLevel.GameMaster)]
        public double DifficultyValueToPointsRatio
        {
            get { return m_DifficultyValueToPointsRatio; }
            set { m_DifficultyValueToPointsRatio = value; }
        }        

        private HotspotEventType m_HotspotType = HotspotEventType.Invasion;
        [CommandProperty(AccessLevel.GameMaster)]
        public HotspotEventType HotspotType
        {
            get { return m_HotspotType; }
            set { m_HotspotType = value; }
        }

        public List<InvasionHotspotParticipantEntry> m_Participants = new List<InvasionHotspotParticipantEntry>();

        #endregion

        [Constructable]
        public InvasionHotspot(): base()
        {
            Name = "invasion hotspot";

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
                case HotspotEventType.Invasion:
                    DifficultyValueToPointsRatio = .33;
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
            foreach (InvasionHotspotParticipantEntry entry in m_Participants)
            {
                if (entry == null) continue;
                if (entry.m_Player == null) continue;

                entry.m_Points = Math.Ceiling(entry.m_Points);                    
            }
        }

        public void DistributeRewards()
        {
            Dictionary<PlayerMobile, int> m_ValidParticipants = new Dictionary<PlayerMobile, int>();

            foreach (InvasionHotspotParticipantEntry entry in m_Participants)
            {
                if (entry == null) continue;
                if (entry.m_Player == null) continue;

                m_ValidParticipants.Add(entry.m_Player, (int)entry.m_Points);
            }

            int TotalValues = 0;
            foreach (KeyValuePair<PlayerMobile, int> pair in m_ValidParticipants)
            {
                TotalValues += pair.Value;
            }

            double ItemCheck = Utility.RandomDouble();

            double CumulativeAmount = 0.0;
            double AdditionalAmount = 0.0;

            //Determine Reward                      
            foreach (KeyValuePair<PlayerMobile, int> pair in m_ValidParticipants)
            {
                AdditionalAmount = (double)pair.Value / (double)TotalValues;

                if (ItemCheck >= CumulativeAmount && ItemCheck < (CumulativeAmount + AdditionalAmount))
                {
                    PlayerMobile winner = pair.Key;

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
        }

        public InvasionHotspotParticipantEntry GetParticipationEntry(PlayerMobile player)
        {
            foreach (InvasionHotspotParticipantEntry entry in m_Participants)
            {
                if (entry == null) continue;

                if (entry.m_Player == player)
                    return entry;
            }

            InvasionHotspotParticipantEntry newEntry = new InvasionHotspotParticipantEntry(player);

            m_Participants.Add(newEntry);

            return newEntry;
        }

        public override void OnDelete()
        {
            if (m_Instances.Contains(this))
                m_Instances.Remove(this);

 	        base.OnDelete();
        }

        public InvasionHotspot(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            //Version 0
            writer.Write(m_DifficultyValueToPointsRatio);
            writer.Write((int)m_HotspotType);

            writer.Write(m_Participants.Count);
            for (int a = 0; a < m_Participants.Count; a++)
            {
                writer.Write(m_Participants[a].m_Player);
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
                m_DifficultyValueToPointsRatio = reader.ReadDouble();
                m_HotspotType = (HotspotEventType)reader.ReadInt();

                int participantsCount = reader.ReadInt();
                for (int a = 0; a < participantsCount; a++)
                {
                    PlayerMobile player = (PlayerMobile)reader.ReadMobile();
                    int points = reader.ReadInt();

                    InvasionHotspotParticipantEntry entry = new InvasionHotspotParticipantEntry(player);
                    entry.m_Points = points;

                    m_Participants.Add(entry);
                }
            }

            //-----

            if (!m_Instances.Contains(this))
                m_Instances.Add(this);
        }
    }

    public class InvasionHotspotParticipantEntry
    {
        public PlayerMobile m_Player;
        public double m_Points = 0;

        public InvasionHotspotParticipantEntry(PlayerMobile m_Player)
        {
            m_Player = m_Player;
        }
    }

    public class InvasionHotspotLocationDetail
    {
        public string m_Name = "Yew Orc Fort";
        public string[] m_Description = new string[] { "" };

        public Point3D m_ControlObjectLocation = new Point3D(2000, 2000, 0);
        public Point3D m_TopLeftAreaPoint = new Point3D(1500, 1500, 0);
        public Point3D m_BottomRightAreaPoint = new Point3D(2500, 2500, 0);

        public Map map = Map.Felucca;
    }

    public class InvasionHotspotEventTypeDetail
    {
        public string m_Name = "Invasion";
        public string[] m_Description = new string[] { "" };
    }
}