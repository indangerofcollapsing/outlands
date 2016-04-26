using System;
using Server.Items;
using System.Collections;
using System.Collections.Generic;

namespace Server
{
    public class FactionCaptureEvent : Item
    {
        public static List<Type> FactionCaptureLocationList = new List<Type>()
        { 
            typeof(PrevaliaBankAndShipwrightCaptureLocation),
        };

        public int SchedulePosition = 1;
        public int CyclePosition = 1;
        public int EventPosition = 1;

        public DateTime EventStart;
        public DateTime EventCompletion;

        public bool Completed = false;

        public FactionCaptureLocation CaptureLocation;
       
        public FactionCaptureEvent()
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
            }
        }
    }
}