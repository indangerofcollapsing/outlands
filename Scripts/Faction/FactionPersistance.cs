﻿using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Misc;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Items;

namespace Server
{
    public static class FactionPersistance
    {    
        public static FactionPersistanceItem PersistanceItem;

        public static bool AnnouncementMade = false;
        public static DateTime NextScheduledCaptureEventStartTime = DateTime.MaxValue;        
        
        public static int CurrentSchedulePosition = 1;
        public static int CurrentCyclePosition = 1;
        public static int CurrentEventPosition = 1;
        
        public static int CyclesPerSchedule = 7;
        public static List<TimeSpan> EventsInCycle = new List<TimeSpan> { TimeSpan.FromHours(8), TimeSpan.FromHours(8), TimeSpan.FromHours(6)};
        public static TimeSpan CycleDowntime = TimeSpan.FromHours(2);

        public static TimeSpan CaptureEventDuration = TimeSpan.FromHours(1);            
        
        public static TimeSpan ScheduledCaptureEventAnnouncementLeadtime = TimeSpan.FromMinutes(5);        

        public static TimeSpan TickDuration = TimeSpan.FromSeconds(5);
        public static TimeSpan TimeElapsed = TimeSpan.FromSeconds(0);

        public static List<FactionCaptureEvent> CaptureEvents = new List<FactionCaptureEvent>();
        public static FactionCaptureEvent CurrentCaptureEvent;        

        public static bool DeletePreviousCycleRecords = false;

        private static bool m_Enabled = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public static bool Enabled
        {
            get { return m_Enabled; }
            set
            {
                m_Enabled = value;

                if (!m_Enabled && Active)                
                    EndCaptureEvent();                
            }
        }

        private static bool m_Active = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public static bool Active
        {
            get { return m_Active; }
            set { m_Active = value; }
        }

        public static Timer m_Timer;

        public static void Initialize()
        {
            CommandSystem.Register("FactionEnable", AccessLevel.GameMaster, new CommandEventHandler(FactionEnable));
            CommandSystem.Register("FactionDisable", AccessLevel.GameMaster, new CommandEventHandler(FactionDisable));

            CommandSystem.Register("FactionDeleteAllCaptureEvents", AccessLevel.GameMaster, new CommandEventHandler(FactionDeleteAllCaptureEvents));

            CommandSystem.Register("FactionStartScheduleNow", AccessLevel.GameMaster, new CommandEventHandler(FactionStartScheduleNow));
            CommandSystem.Register("FactionStartScheduleInXMinutes", AccessLevel.GameMaster, new CommandEventHandler(FactionStartScheduleInXMinutes));
            CommandSystem.Register("FactionResetSchedule", AccessLevel.GameMaster, new CommandEventHandler(FactionResetSchedule));

            CommandSystem.Register("FactionStartSession", AccessLevel.GameMaster, new CommandEventHandler(FactionStartCaptureEvent));
            CommandSystem.Register("FactionEndSession", AccessLevel.GameMaster, new CommandEventHandler(FactionEndCaptureEvent));            

            Timer.DelayCall(TimeSpan.FromTicks(1), delegate
            {
                if (PersistanceItem == null)
                    PersistanceItem = new FactionPersistanceItem();

                else if (PersistanceItem.Deleted)
                    PersistanceItem = new FactionPersistanceItem();

                //Create Faction Instances
                foreach (Type type in Faction.FactionList)
                {
                    bool foundFaction = false;

                    for (int a = 0; a < Faction.Factions.Count; a++)
                    {
                        if (Faction.Factions[a].GetType() == type)
                        {
                            foundFaction = true;
                            break;
                        }
                    }

                    if (!foundFaction)
                    {
                        Faction newFaction = (Faction)Activator.CreateInstance(type);
                    }
                }

                //Audit Faction
                foreach (Faction faction in Faction.Factions)
                {
                    faction.Audit();
                }

                Timer.DelayCall(TimeSpan.FromSeconds(1), delegate
                {
                    if (Enabled)
                    {
                        if (m_Timer != null)
                        {
                            m_Timer.Stop();
                            m_Timer = null;
                        }

                        m_Timer = new FactionTimer();
                        m_Timer.Start();
                    }
                });
            });
        }

        #region Commmands
        
        [Usage("FactionEnable")]
        [Description("Enables Faction System")]
        public static void FactionEnable(CommandEventArgs arg)
        {
            PlayerMobile pm_Mobile = arg.Mobile as PlayerMobile;

            if (pm_Mobile == null)
                return;

            Enabled = true;

            pm_Mobile.SendMessage("Enabling Faction system.");
        }

        [Usage("FactionDisable")]
        [Description("Disables Faction System")]
        public static void FactionDisable(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            if (Active)
            {
                foreach (NetState state in NetState.Instances)
                {
                    Mobile mobile = state.Mobile;
                    PlayerMobile pm_Mobile = mobile as PlayerMobile;

                    if (pm_Mobile == null)
                        continue;

                    pm_Mobile.SendMessage(Faction.purpleTextHue, "Faction Capture Event has ended.");
                }
            }

            Enabled = false;

            player.SendMessage("Disabling Faction system.");
        }

        [Usage("FactionDeleteAllCaptureEvents")]
        [Description("Deletes All Previous Capture Events Recorded")]
        public static void FactionDeleteAllCaptureEvents(CommandEventArgs arg)
        {
            PlayerMobile pm_Mobile = arg.Mobile as PlayerMobile;

            if (pm_Mobile == null)
                return;

            CurrentSchedulePosition = 1;
            CurrentEventPosition = 1;
            CurrentCyclePosition = 1;

            if (Active)
                EndCaptureEvent();

            Queue m_Queue = new Queue();

            foreach(FactionCaptureEvent FactionCaptureEvent in CaptureEvents)
            {
                m_Queue.Enqueue(FactionCaptureEvent);
            }

            while (m_Queue.Count > 0)
            {
                FactionCaptureEvent factionCaptureEvent = (FactionCaptureEvent)m_Queue.Dequeue();
                factionCaptureEvent.Delete();
            }

            CurrentCaptureEvent = null;

            //Prevent New Event Immediately Occuring: Will Require Schedule Realignment
            if (DateTime.UtcNow >= FactionPersistance.NextScheduledCaptureEventStartTime)            
                FactionPersistance.NextScheduledCaptureEventStartTime = DateTime.UtcNow + TimeSpan.FromDays(365);            
        }

        [Usage("FactionStartScheduleNow")]
        [Description("Resets Scheduled Timer To Now As Starting Point")]
        public static void FactionStartScheduleNow(CommandEventArgs arg)
        {
            PlayerMobile pm_Mobile = arg.Mobile as PlayerMobile;

            if (pm_Mobile == null)
                return;

            CurrentEventPosition = 1;
            CurrentCyclePosition = 1;

            NextScheduledCaptureEventStartTime = DateTime.UtcNow;

            pm_Mobile.SendMessage("Faction Schedule Synced To Current Time.");
        }

        [Usage("FactionStartScheduleInXMinutes")]
        [Description("Resets Scheduled Timer To Now + X Minutes As Midnight of Schedule Start")]
        public static void FactionStartScheduleInXMinutes(CommandEventArgs arg)
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

                    CurrentEventPosition = 1;
                    CurrentCyclePosition = 1;

                    NextScheduledCaptureEventStartTime = DateTime.UtcNow + TimeSpan.FromMinutes(minutes) + EventsInCycle[0];
                }

                catch
                {
                    pm_Mobile.SendMessage("Error in arguments. Usage: [FactionStartScheduleInXMinutes minutes");
                    return;
                }
            }

            pm_Mobile.SendMessage("Current DateTime: " + DateTime.UtcNow.ToString());
            pm_Mobile.SendMessage("Faction Schedule set to start at: " + NextScheduledCaptureEventStartTime.ToString());
        }

        [Usage("FactionResetSchedule")]
        [Description("Resets Scheduled Timer To Now As Schedule Starting Point")]
        public static void FactionResetSchedule(CommandEventArgs arg)
        {
            PlayerMobile pm_Mobile = arg.Mobile as PlayerMobile;

            if (pm_Mobile == null)
                return;

            CurrentEventPosition = 1;
            CurrentCyclePosition = 1;

            NextScheduledCaptureEventStartTime = DateTime.UtcNow + EventsInCycle[0];

            pm_Mobile.SendMessage("Faction Schedule Restarted.");
        }
        
        [Usage("FactionStartCaptureEvent")]
        [Description("Starts the Current Faction Capture Event")]
        public static void FactionStartCaptureEvent(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            if (!FactionPersistance.Enabled)
            {
                player.SendMessage("Faction system is currently disabled.");
                return;
            }

            if (FactionPersistance.Active)
            {
                player.SendMessage("Faction session is already in progress.");
                return;
            }

            else
            {
                FactionPersistance.StartCaptureEvent();
                return;
            }
        }

        [Usage("FactionEndCaptureEvent")]
        [Description("Ends the current Faction Capture Event")]
        public static void FactionEndCaptureEvent(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            if (!FactionPersistance.Active)
            {
                player.SendMessage("There is no Faction Capture Event in progress.");
                return;
            }

            else
            {
                foreach (NetState state in NetState.Instances)
                {
                    Mobile mobile = state.Mobile;
                    PlayerMobile pm_Mobile = mobile as PlayerMobile;

                    if (pm_Mobile == null)
                        continue;

                    pm_Mobile.SendMessage(Faction.purpleTextHue, "Faction Capture Event has ended.");
                }

                FactionPersistance.EndCaptureEvent();

                return;
            }
        }

        #endregion        
                
        public class FactionTimer : Timer
        {
            public FactionTimer(): base(TimeSpan.Zero, TickDuration)
            {
                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                if (!FactionPersistance.Enabled)                
                    return;

                if (FactionPersistance.Active && CurrentCaptureEvent != null)
                {
                    TimeElapsed = TimeElapsed + TickDuration;
                    
                    if (DateTime.UtcNow >= CurrentCaptureEvent.EventStart + CaptureEventDuration)
                    {
                        EndCaptureEvent();

                        return;
                    }                                    

                    TimerTick();
                }

                else
                {
                    if (DateTime.UtcNow >= FactionPersistance.NextScheduledCaptureEventStartTime)
                    {
                        //Somehow a Previous Event Was Currently Active
                        if (CurrentCaptureEvent != null)                        
                            EndCaptureEvent();

                        FactionPersistance.AnnouncementMade = false;

                        StartCaptureEvent();

                        return;
                    }

                    if (!FactionPersistance.AnnouncementMade && DateTime.UtcNow >= FactionPersistance.NextScheduledCaptureEventStartTime - FactionPersistance.ScheduledCaptureEventAnnouncementLeadtime)
                    {
                        FactionPersistance.AnnouncementMade = true;

                        foreach (NetState state in NetState.Instances)
                        {
                            PlayerMobile player = state.Mobile as PlayerMobile;

                            if (player == null)
                                continue;

                            string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, DateTime.UtcNow + FactionPersistance.ScheduledCaptureEventAnnouncementLeadtime, false, false, true, true, true);

                            player.SendMessage(Faction.purpleTextHue, "Faction Capture Event will begin in " + timeRemaining + ".");
                        }
                    }
                }
            }
        }

        public static void TimerTick()
        {
            if (CurrentCaptureEvent == null)
                return;

            CurrentCaptureEvent.OnEventTick();
        }

        public static void StartCaptureEvent()
        {
            if (Active)
                return;

            FactionCaptureEvent.CreateCaptureEvent();

            if (CurrentCaptureEvent == null)
                return;

            CurrentCaptureEvent.OnEventStart();

            Active = true;            

            CurrentCaptureEvent.EventStart = DateTime.UtcNow;          

            TimeElapsed = TimeSpan.FromSeconds(0);

            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }

            m_Timer = new FactionTimer();
            m_Timer.Start();            
        }

        public static void EndCaptureEvent()
        {
            if (!Active)
                return;

            Active = false;
            TimeElapsed = TimeSpan.FromSeconds(0);

            if (CurrentCaptureEvent != null)
            {
                CurrentCaptureEvent.OnEventCompletion();

                CurrentCaptureEvent.EventCompletion = DateTime.UtcNow;
                CurrentCaptureEvent.Completed = true;

                CurrentCaptureEvent = null;
            }

            CurrentEventPosition++;

            //Cycle Completed
            if (CurrentEventPosition > EventsInCycle.Count)
            {
                CurrentCyclePosition++;
                CurrentEventPosition = 1;                

                //Schedule Completed
                if (CurrentCyclePosition > CyclesPerSchedule)
                {
                    CurrentSchedulePosition++;
                    CurrentCyclePosition = 1;

                    ResolveScheduleCompletion();
                }
            }

            if (CurrentCyclePosition > EventsInCycle.Count)
                CurrentCyclePosition = EventsInCycle.Count;

            TimeSpan timeUntilNextEvent = EventsInCycle[CurrentCyclePosition - 1];

            if (CurrentCyclePosition == 1)
                timeUntilNextEvent = timeUntilNextEvent + CycleDowntime;

            FactionPersistance.NextScheduledCaptureEventStartTime = FactionPersistance.NextScheduledCaptureEventStartTime + timeUntilNextEvent;                        
        }   
        
        public static void ResolveScheduleCompletion()
        {
            if (DeletePreviousCycleRecords)
            {              
                Queue m_Queue = new Queue();

                while (m_Queue.Count > 0)
                {
                    FactionCaptureEvent captureEvent = (FactionCaptureEvent)m_Queue.Dequeue();

                    captureEvent.Delete();
                }
            }
        }
        
        public static void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); //Version

            //Version 0   
            writer.Write(m_Enabled);
            writer.Write(m_Active);
            writer.Write(NextScheduledCaptureEventStartTime);
            writer.Write(AnnouncementMade);
            writer.Write(TimeElapsed);
            writer.Write(CurrentSchedulePosition);
            writer.Write(CurrentCyclePosition);
            writer.Write(CurrentEventPosition);
            
            writer.Write(CaptureEvents.Count);
            for (int a = 0; a < CaptureEvents.Count; a++)
            {
                writer.Write(CaptureEvents[a]);
            }

            writer.Write(CurrentCaptureEvent);
        }

        public static void Deserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            //Version 0
            if (version >= 0)
            {
                m_Enabled = reader.ReadBool();
                m_Active = reader.ReadBool();
                NextScheduledCaptureEventStartTime = reader.ReadDateTime();
                AnnouncementMade = reader.ReadBool();
                TimeElapsed = reader.ReadTimeSpan();
                CurrentSchedulePosition = reader.ReadInt();
                CurrentCyclePosition = reader.ReadInt();
                CurrentEventPosition = reader.ReadInt();

                int captureEventCount = reader.ReadInt();
                for (int a = 0; a < captureEventCount; a++)
                {
                    FactionCaptureEvent captureEvent = (FactionCaptureEvent)reader.ReadItem();
                    CaptureEvents.Add(captureEvent);
                }

                CurrentCaptureEvent = (FactionCaptureEvent)reader.ReadItem();
            }       
        }
    }    

    public class FactionPersistanceItem : Item
    {
        public override string DefaultName { get { return "FactionPersistance"; } }

        public FactionPersistanceItem(): base(0x0)
        {
            Movable = false;
        }

        public FactionPersistanceItem(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            FactionPersistance.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Movable = false;

            FactionPersistance.PersistanceItem = this;
            FactionPersistance.Deserialize(reader);
        }
    }
}
