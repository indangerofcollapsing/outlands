using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server
{
    public class FactionCaptureEvent : Item
    {
        public static List<Type> FactionCaptureLocationList = new List<Type>()
        { 
            typeof(PrevaliaBankAndShipwrightCaptureLocation),
            typeof(PrevaliaTownsquareAndSlumsCaptureLocation),
        };

        public int SchedulePosition = 1;
        public int CyclePosition = 1;
        public int EventPosition = 1;

        public DateTime EventStart;
        public DateTime EventCompletion;

        public bool Completed = false;

        public FactionCaptureLocation CaptureLocation;

        public List<CaptureEventData> FactionEventData = new List<CaptureEventData>();
        public List<PlayerMobile> ParticipatingPlayers = new List<PlayerMobile>();

        [Constructable]
        public FactionCaptureEvent(): base()
        {
        }
        
        public FactionCaptureEvent(Serial serial): base(serial)
        {
        }

        public static void CreateCaptureEvent()
        {
            FactionCaptureEvent factionCaptureEvent = null;

            //TEST: Populate

            factionCaptureEvent.SchedulePosition = FactionPersistance.CurrentSchedulePosition;
            factionCaptureEvent.CyclePosition = FactionPersistance.CurrentCyclePosition;
            factionCaptureEvent.EventPosition = FactionPersistance.CurrentEventPosition;

            FactionPersistance.CurrentCaptureEvent = factionCaptureEvent;
            FactionPersistance.CaptureEvents.Add(factionCaptureEvent);
        }

        public void OnEventStart()
        {
            if (CaptureLocation != null)
                CaptureLocation.OnEventStart();

            foreach (NetState state in NetState.Instances)
            {
                Mobile mobile = state.Mobile;
                PlayerMobile player = mobile as PlayerMobile;

                if (player == null)
                    continue;

                player.SendMessage(Faction.purpleTextHue, "Faction Capture Event has begun!");
            }           
        }

        public void OnEventTick()
        {
            if (CaptureLocation != null)
                CaptureLocation.OnEventTick();
        }

        public void OnEventCompletion()
        {
            if (CaptureLocation != null)
                CaptureLocation.OnEventCompletetion();            

            foreach (NetState state in NetState.Instances)
            {
                Mobile mobile = state.Mobile;
                PlayerMobile player = mobile as PlayerMobile;

                if (player == null)
                    continue;

                player.SendMessage(Faction.purpleTextHue, "Capture event has has ended. The result is a tie.");
            }

            ResolveCaptureEventScores();
        }

        public void ResolveCaptureEventScores()
        {
            //Resolve Scores

            //Clear Player Profile Data
            foreach (PlayerMobile player in ParticipatingPlayers)
            {
                if (player == null)
                    continue;

                Faction.CheckCreateFactionPlayerProfile(player);

                player.m_FactionPlayerProfile.ResetCaptureEventValues();
            }
        }

        public override void OnAfterDelete()
        {
            if (FactionPersistance.CaptureEvents.Contains(this))
                FactionPersistance.CaptureEvents.Remove(this);

            if (FactionPersistance.CurrentCaptureEvent == this)
                FactionPersistance.CurrentCaptureEvent = null;

            base.OnAfterDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            //Version 0
            writer.Write(SchedulePosition);
            writer.Write(CyclePosition);
            writer.Write(EventPosition);

            writer.Write(EventStart);
            writer.Write(EventCompletion);
            writer.Write(Completed);

            writer.Write(FactionEventData.Count);
            for (int a = 0; a < FactionEventData.Count; a++)
            {
                writer.Write(FactionEventData[a].Faction);
                writer.Write(FactionEventData[a].Score);               
            }

            writer.Write(ParticipatingPlayers.Count);
            for (int a = 0; a < ParticipatingPlayers.Count; a++)
            {
                writer.Write(ParticipatingPlayers[a]);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                SchedulePosition = reader.ReadInt();
                CyclePosition = reader.ReadInt();
                EventPosition = reader.ReadInt();

                EventStart = reader.ReadDateTime();
                EventCompletion = reader.ReadDateTime();
                Completed = reader.ReadBool();

                int factionCount = reader.ReadInt();
                for (int a = 0; a < factionCount; a++)
                {
                    CaptureEventData factionScore = new CaptureEventData();

                    Faction faction = (Faction)reader.ReadItem();
                    double score = reader.ReadDouble();

                    factionScore.Faction = faction;
                    factionScore.Score = score;                 

                    FactionEventData.Add(factionScore);                    
                }

                int participatingPlayersCount = reader.ReadInt();
                for (int a = 0; a < participatingPlayersCount; a++)
                {
                    PlayerMobile player = (PlayerMobile)reader.ReadMobile();
                    ParticipatingPlayers.Add(player);
                }
            }
        }

        public class CaptureEventData
        {
            public Faction Faction;
            public double Score;

            public CaptureEventData()
            {
            }
        }
    }    
}