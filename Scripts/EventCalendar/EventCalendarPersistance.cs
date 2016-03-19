﻿using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Misc;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Achievements;
using Server.Accounting;
using System.Linq;
using Server.Custom;
using System.Text;
using Server.Guilds;

namespace Server
{
    public static class EventCalendarPersistance
    {
        public static EventCalendarPersistanceItem PersistanceItem;

        public static List<EventCalendarEvent> m_EventCalendarEvents = new List<EventCalendarEvent>();
        public static List<EventCalendarAccount> m_EventCalendarAccounts = new List<EventCalendarAccount>();

        public static int AlertSound = 0x5B5; //0x5B6

        public static int OfficialEventHue = 1164;
        public static int GuildEventHue = 2599;
        public static int PlayerEventHue = 2625;

        public static int MajorEventHue = 2115;
        public static int MinorEventHue = 2548;
        public static int RecurringEventHue = 2562;
        
        private static bool m_Enabled = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public static bool Enabled
        {
            get { return m_Enabled; }
            set { m_Enabled = value; }
        }

        public static Timer m_Timer;

        public static void Initialize()
        {
            CommandSystem.Register("EventCalendar", AccessLevel.Player, new CommandEventHandler(EventCalendar));

            Timer.DelayCall(TimeSpan.FromTicks(1), delegate
            {
                if (PersistanceItem == null)
                    PersistanceItem = new EventCalendarPersistanceItem();

                else if (PersistanceItem.Deleted)
                    PersistanceItem = new EventCalendarPersistanceItem();

                m_Timer = new EventCalendarTimer();
                m_Timer.Start();
            });
        }

        public class EventCalendarTimer : Timer
        {
            public EventCalendarTimer(): base(TimeSpan.Zero, TimeSpan.FromMinutes(5))
            {
                Priority = TimerPriority.OneMinute;
            }

            protected override void OnTick()
            {     
                foreach(EventCalendarEvent eventCalendarEvent in m_EventCalendarEvents)
                {
                    DateTime eventStart = eventCalendarEvent.StartTime;

                    bool recurringEvent = false;

                    //Recurring Events: Find Next Closest Instance
                    if (eventCalendarEvent.EventType == EventCalendarEvent.EventTypeType.Recurring)
                    {
                        recurringEvent = true;

                        if (eventStart < DateTime.UtcNow)
                        {
                            DateTime recurringStart = eventStart;
                            TimeSpan recurringInterval = eventCalendarEvent.FrequencyInterval;
                            TimeSpan startUntilNow = DateTime.UtcNow - recurringStart;

                            double intervalsFromStartToCurrent = Math.Ceiling(startUntilNow.TotalMinutes / recurringInterval.TotalMinutes);

                            eventStart = recurringStart + TimeSpan.FromMinutes(intervalsFromStartToCurrent * recurringInterval.TotalMinutes);
                        }
                    }

                    if (eventStart < DateTime.UtcNow)
                        continue;

                    if (eventStart > DateTime.UtcNow + TimeSpan.FromDays(1))
                        continue;

                    if (eventCalendarEvent.EventLevel == EventCalendarEvent.EventLevelType.Player && eventCalendarEvent.ApprovalStatus != EventCalendarEvent.ApprovalStatusType.Approved)
                        continue;

                    if (eventCalendarEvent.EventLevel != EventCalendarEvent.EventLevelType.Player && !eventCalendarEvent.EventSubmitted)
                        continue;

                    TimeSpan timeUntilEvent = eventStart - DateTime.UtcNow;

                    bool TwentyFourHourNotification = false;
                    bool TwoHoursNotification = false;
                    bool OneHourNotification = false;
                    bool FifteenMinuteNotification = false;
                    bool FiveMinuteNotification = false;

                    //One Day Notification
                    if (timeUntilEvent <= TimeSpan.FromDays(1) && !eventCalendarEvent.AlertMadeTwentyFourHoursPrior)
                    {
                        if (eventCalendarEvent.EventType == EventCalendarEvent.EventTypeType.Recurring && eventCalendarEvent.FrequencyInterval < TimeSpan.FromDays(2))
                            continue;

                        TwentyFourHourNotification = true;
                        eventCalendarEvent.AlertMadeTwentyFourHoursPrior = true;
                    }

                    //Two Hours Notification
                    if (timeUntilEvent <= TimeSpan.FromHours(2) && !eventCalendarEvent.AlertMadeTwoHoursPrior)
                    {
                        if (eventCalendarEvent.EventType == EventCalendarEvent.EventTypeType.Recurring && eventCalendarEvent.FrequencyInterval < TimeSpan.FromHours(4))
                            continue;

                        TwoHoursNotification = true;
                        eventCalendarEvent.AlertMadeTwoHoursPrior = true;
                    }

                    //One Hour Notification
                    if (timeUntilEvent <= TimeSpan.FromHours(1) && !eventCalendarEvent.AlertMadeOneHourPrior)
                    {
                        if (eventCalendarEvent.EventType == EventCalendarEvent.EventTypeType.Recurring && eventCalendarEvent.FrequencyInterval < TimeSpan.FromHours(2))
                            continue;

                        OneHourNotification = true;
                        eventCalendarEvent.AlertMadeOneHourPrior = true;
                    }

                    //15 Minute Notification
                    if (timeUntilEvent <= TimeSpan.FromMinutes(15) && !eventCalendarEvent.AlertMadeFifteenMinutesPrior)
                    {
                        if (eventCalendarEvent.EventType == EventCalendarEvent.EventTypeType.Recurring && eventCalendarEvent.FrequencyInterval < TimeSpan.FromHours(1))
                            continue;

                        FifteenMinuteNotification = true;
                        eventCalendarEvent.AlertMadeFifteenMinutesPrior = true;
                    }

                    //5 Minute Notification
                    if (eventStart <= DateTime.UtcNow && !eventCalendarEvent.AlertMadeFiveMinutesPrior)
                    {
                        FiveMinuteNotification = true;
                        eventCalendarEvent.AlertMadeFiveMinutesPrior = true;

                        Timer.DelayCall(TimeSpan.FromMinutes(5), delegate
                        {
                            if (eventCalendarEvent == null)
                                return;

                            //Reset Alerts for Next Occurance of Recurring Event
                            eventCalendarEvent.AlertMadeTwentyFourHoursPrior = false;
                            eventCalendarEvent.AlertMadeTwoHoursPrior = false;
                            eventCalendarEvent.AlertMadeOneHourPrior = false;
                            eventCalendarEvent.AlertMadeFifteenMinutesPrior = false;
                            eventCalendarEvent.AlertMadeFiveMinutesPrior = false;
                        });
                    }

                    //No Notifications Neccessary
                    if (!TwentyFourHourNotification && !TwoHoursNotification && !OneHourNotification && !FifteenMinuteNotification && !FiveMinuteNotification)
                        return;

                    //In Case of Overlap, Only Do Alert Closest to Start Time
                    if (TwoHoursNotification)                    
                        TwentyFourHourNotification = false;                    

                    if (OneHourNotification)
                    {
                        TwentyFourHourNotification = false;
                        TwoHoursNotification = false;
                    }

                    if (FifteenMinuteNotification)
                    {
                        TwentyFourHourNotification = false;
                        TwoHoursNotification = false;
                        OneHourNotification = false;
                    }

                    if (FiveMinuteNotification)
                    {
                        TwentyFourHourNotification = false;
                        TwoHoursNotification = false;
                        OneHourNotification = false;
                        FifteenMinuteNotification = false;                        
                    }

                    string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, eventStart, false, true, true, true, false);
                    
                    foreach (NetState state in NetState.Instances)
                    {
                        PlayerMobile player = state.Mobile as PlayerMobile;

                        if (player == null)
                            continue;                       

                        CheckAndCreateEventCalendarAccount(player);

                        if (eventCalendarEvent.m_AccountsIgnoringAlert.Contains(player.m_EventCalendarAccount))
                            continue;

                        bool doPlayerPopup = false;
                        bool doPlayerAudio = false;

                        EventCalendarAlertSetting alertSetting = player.m_EventCalendarAccount.GetAlertSetting(eventCalendarEvent.EventLevel, eventCalendarEvent.EventType);

                        if (alertSetting == null)
                            continue;

                        if (TwentyFourHourNotification && !alertSetting.AlertTwentyFourHoursPrior) continue;
                        if (TwoHoursNotification && !alertSetting.AlertTwoHoursPrior) continue;
                        if (OneHourNotification && !alertSetting.AlertOneHourPrior) continue;
                        if (FifteenMinuteNotification && !alertSetting.AlertFifteenMinutesPrior) continue;
                        if (FiveMinuteNotification && !alertSetting.AlertFiveMinutesPrior) continue;

                        //Pop-Up
                        if (alertSetting.AlertPopUp)
                        {
                            if (player.HasGump(typeof(EventCalendarAlert)))
                            {
                                //Wait 1 Minute For New Alert if One Currently On Screen
                                Timer.DelayCall(TimeSpan.FromSeconds(60), delegate
                                {
                                    if (player == null) return;
                                    if (eventCalendarEvent == null) return;
                                    if (eventCalendarEvent.Deleted) return;

                                    CheckAndCreateEventCalendarAccount(player);

                                    //Override Existing Alert
                                    if (player.HasGump(typeof(EventCalendarAlert)))                                    
                                        player.CloseGump(typeof(EventCalendarAlert));

                                    player.SendGump(new EventCalendarAlert(player, eventCalendarEvent));                                    
                                });
                            }

                            else
                                player.SendGump(new EventCalendarAlert(player, eventCalendarEvent));                                                          
                        }

                        //Audio Alert
                        if (alertSetting.AlertAudio)
                            player.SendSound(AlertSound);

                        player.SendMessage("[Event] " + eventCalendarEvent.DisplayName + " will begin in " + timeRemaining + ".");
                    }                    
                }               
            }
        }

        [Usage("EventCalendar")]
        [Description("Opens the Event Calendar")]
        public static void EventCalendar(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;            

            OpenEventCalender(player);
        }

        public static int[] IconItemIDs = new int[]
        {
            8397, 8398, 8409, 8415, 8425, 8438, 8440, 8484, 8486, 8532, 8700, 8900, 8950,
            8960, 9002, 9006, 10840, 11650, 11669, 11762, 12584, 13704, 14036,
            733, 939, 2422, 2451, 2462, 2472, 2743, 2474, 2475, 2477, 3645, 3647,
            3649, 2491, 3651, 3655, 508, 2537, 2541, 2572, 2581, 2599, 2842, 2648, 2886,
            3103, 3718, 3530, 3542, 3672, 3700, 3703, 3717, 3740, 3743, 3764,
            3786, 3800, 3823, 3830, 3834, 3894, 3905, 3910, 3935, 4011, 4013, 4015,
            4017, 4029, 4020, 4167, 4202, 4458, 4517, 4552, 4650, 4653, 4643, 4648,
            5041, 5066, 5095, 5356, 5360, 5362, 5367, 5368, 5445, 5447, 5437, 5921,
            5995, 6160, 6227, 6235, 6225, 6263, 6585, 7028, 7107, 7108, 2230,
            7132, 7159, 7390, 7716, 7754, 7776, 7777, 7778, 7779, 7780, 7781, 7782,
            99, 543, 1115, 1289, 1294, 1328, 2105, 2281, 2325, 3219, 3347,
            3367, 3555
        };

        public static int[] IconHues = new int[] { 0 };

        public static EventIconDetail GetIconDetailDetail(int itemId)
        {
            EventIconDetail iconDetail = new EventIconDetail();

            iconDetail.ItemId = itemId;

            switch (itemId)
            {   
                case 543: iconDetail.OffsetX = 0; iconDetail.OffsetY = 0; break;
            }

            return iconDetail;
        }

        public static void OpenEventCalender(PlayerMobile player)
        {
            CheckAndCreateEventCalendarAccount(player);
            
            //Adjust For Local Time
            DateTime adjustedDateTime = DateTime.UtcNow;

            if (player.m_EventCalendarAccount.TimeZoneOffset > 0)
                adjustedDateTime = adjustedDateTime + TimeSpan.FromHours(Math.Abs(player.m_EventCalendarAccount.TimeZoneOffset));

            else if (player.m_EventCalendarAccount.TimeZoneOffset < 0)
                adjustedDateTime = adjustedDateTime - TimeSpan.FromHours(Math.Abs(player.m_EventCalendarAccount.TimeZoneOffset));

            player.m_EventCalendarAccount.PastDate = adjustedDateTime;            

            player.m_EventCalendarAccount.DisplayPage = 1;

            player.m_EventCalendarAccount.SelectedEvent = null;

            if (player.m_EventCalendarAccount.StoredValuesEvent != null)
            {
                player.m_EventCalendarAccount.StoredValuesEvent.Delete();
                player.m_EventCalendarAccount.StoredValuesEvent = null;
            }

            player.CloseGump(typeof(EventCalendarGump));
            player.CloseGump(typeof(EventCalendarEditorGump));

            player.SendGump(new EventCalendarGump(player));
        }

        public class EventCalendarPersistanceItem : Item
        {
            public override string DefaultName { get { return "EventCalendarPersistance"; } }

            public EventCalendarPersistanceItem(): base(0x0)
            {
                Movable = false;
            }

            public EventCalendarPersistanceItem(Serial serial): base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
                writer.Write((int)0); //Version

                EventCalendarPersistance.Serialize(writer);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                int version = reader.ReadInt();

                Movable = false;

                EventCalendarPersistance.PersistanceItem = this;
                EventCalendarPersistance.Deserialize(reader);
            }
        }       

        public static void CheckAndCreateEventCalendarAccount(PlayerMobile player)
        {
            if (player == null) return;
            if (player.Deleted) return;
            
            if (player.m_EventCalendarAccount == null)
                CreateEventCalendarAccountEntry(player);

            if (player.m_EventCalendarAccount.Deleted)
                CreateEventCalendarAccountEntry(player);         
        }

        public static void CreateEventCalendarAccountEntry(PlayerMobile player)
        {
            if (player == null) return;
            if (player.Account == null) return;

            string accountName = player.Account.Username;

            EventCalendarAccount eventCalendarAccount = null;

            foreach (EventCalendarAccount entry in m_EventCalendarAccounts)
            {
                if (entry.AccountUsername == accountName && !entry.Deleted)
                {
                    player.m_EventCalendarAccount = entry;
                    return;
                }
            }

            EventCalendarAccount newEntry = new EventCalendarAccount(accountName);

            Account account = player.Account as Account;

            if (account.accountMobiles == null)
                return;

            for (int a = 0; a < account.accountMobiles.Length; a++)
            {
                Mobile mobile = account.accountMobiles[a] as Mobile;

                if (mobile != null)
                {
                    PlayerMobile pm_Mobile = mobile as PlayerMobile;

                    if (pm_Mobile != null)                    
                        pm_Mobile.m_EventCalendarAccount = newEntry;
                }
            }
        }

        public static List<EventCalendarEvent> GetCalendarEventsForPlayer(PlayerMobile player, DateTime startTime)
        {
            List<EventCalendarEvent> m_EventsList = new List<EventCalendarEvent>();

            if (player == null) 
                return m_EventsList;

            EventCalendarPersistance.CheckAndCreateEventCalendarAccount(player);            
            
            foreach (EventCalendarEvent calendarEvent in EventCalendarPersistance.m_EventCalendarEvents)
            {
                if (calendarEvent == null) continue;
                if (calendarEvent.Deleted) continue;

                //Event is Currently in Progress
                if (calendarEvent.StartTime < DateTime.UtcNow && DateTime.UtcNow < calendarEvent.StartTime + calendarEvent.Duration)
                {
                    if (!player.m_EventCalendarAccount.FilterEventsInProgress)
                        continue;
                }

                //Event is in The Past
                else if (calendarEvent.StartTime < startTime)
                    continue;

                //My Events Filter: Always Show Upcoming Events From Player Regardless of Other Filters
                if (player.m_EventCalendarAccount.FilterMyEvents && calendarEvent.Creator == player)
                {
                    m_EventsList.Add(calendarEvent);
                    continue;
                }

                if (!player.m_EventCalendarAccount.FilterOfficialEvents && calendarEvent.EventLevel == EventCalendarEvent.EventLevelType.Official) continue;
                if (!player.m_EventCalendarAccount.FilterGuildEvents && calendarEvent.EventLevel == EventCalendarEvent.EventLevelType.Guild) continue;                    
                if (!player.m_EventCalendarAccount.FilterPlayerEvents && calendarEvent.EventLevel == EventCalendarEvent.EventLevelType.Player) continue;                    
                if (!player.m_EventCalendarAccount.FilterMajorEvents && calendarEvent.EventType == EventCalendarEvent.EventTypeType.Major) continue;
                if (!player.m_EventCalendarAccount.FilterMinorEvents && calendarEvent.EventType == EventCalendarEvent.EventTypeType.Minor) continue;
                if (!player.m_EventCalendarAccount.FilterRecurringEvents && calendarEvent.EventType == EventCalendarEvent.EventTypeType.Recurring) continue;
                                
                if (calendarEvent.EventLevel == EventCalendarEvent.EventLevelType.Guild)
                {
                    if (calendarEvent.Guild == null)
                        continue;

                    if (player.AccessLevel == AccessLevel.Player && player.Guild != calendarEvent.Guild)
                        continue;
                }                

                m_EventsList.Add(calendarEvent);
            }             

            return m_EventsList;
        }

        public static bool EventValuesMatch(EventCalendarEvent firstEvent, EventCalendarEvent secondEvent)
        {
            bool eventsMatch = true;

            if (firstEvent.EventLevel != secondEvent.EventLevel)
                eventsMatch = false;

            if (firstEvent.EventType != secondEvent.EventType)
                eventsMatch = false;

            //if (firstEvent.EventSubmitted != secondEvent.EventSubmitted)
                //eventsMatch = false;

            //if (firstEvent.ApprovalStatus != secondEvent.ApprovalStatus)
            //eventsMatch = false;

            //if (firstEvent.ApprovalText != secondEvent.ApprovalText)
            //eventsMatch = false;

            if (firstEvent.StartTime != secondEvent.StartTime)
                eventsMatch = false;

            if (firstEvent.Duration != secondEvent.Duration)
                eventsMatch = false;

            //if (firstEvent.FrequencyInterval != secondEvent.FrequencyInterval)
                //eventsMatch = false;

            if (firstEvent.DisplayName != secondEvent.DisplayName)
                eventsMatch = false;

            if (firstEvent.DescriptionText != secondEvent.DescriptionText)
                eventsMatch = false;

            if (firstEvent.WebLink != secondEvent.WebLink)
                eventsMatch = false;

            if (firstEvent.EventIcon != secondEvent.EventIcon)
                eventsMatch = false;

            if (firstEvent.EventIconHue != secondEvent.EventIconHue)
                eventsMatch = false;

            if (firstEvent.LocationText != secondEvent.LocationText)
                eventsMatch = false;

            if (firstEvent.LocationPoint != secondEvent.LocationPoint)
                eventsMatch = false;

            if (firstEvent.LocationMap != secondEvent.LocationMap)
                eventsMatch = false;

            //if (firstEvent.Creator != secondEvent.Creator)
                //eventsMatch = false;

            //if (firstEvent.CreatorName != secondEvent.CreatorName)
            //eventsMatch = false;

            //if (firstEvent.Guild != secondEvent.Guild)
            //eventsMatch = false;

            //if (firstEvent.CreationDate != secondEvent.CreationDate)
            //eventsMatch = false;

            //if (firstEvent.LastUpdated != secondEvent.LastUpdated)
            //eventsMatch = false;

            return eventsMatch;
        }

        public static void ValidateSelectedEvent(PlayerMobile player, DateTime startingDateTimeOfSearch)
        {
            EventCalendarEvent selectedEvent = player.m_EventCalendarAccount.SelectedEvent;

            if (selectedEvent != null)
            {
                if (!selectedEvent.Deleted)
                {
                    if (selectedEvent.EventLevel == EventCalendarEvent.EventLevelType.Guild)
                    {
                        //Guild Event No Longer Has Guild
                        if (selectedEvent.Guild == null)
                        {
                            if (player.AccessLevel == AccessLevel.Player)
                                player.m_EventCalendarAccount.SelectedEvent = null;
                        }

                        else
                        {
                            //Player Doesn't Belong to Guild Anymore
                            if (player.Guild != selectedEvent.Guild)
                            {
                                if (player.AccessLevel == AccessLevel.Player)
                                    player.m_EventCalendarAccount.SelectedEvent = null;
                            }
                        }
                    }

                    //Event Expired Before Starting Search Date/Time 
                    if (selectedEvent.StartTime + selectedEvent.Duration < startingDateTimeOfSearch)                    
                        player.m_EventCalendarAccount.SelectedEvent = null;
                }

                else                
                    player.m_EventCalendarAccount.SelectedEvent = null;
            }                     
        }

        public static string PlayerFormattedDateTime(PlayerMobile player, DateTime dateTime)
        {
            string formatedDateTime = "";

            if (player == null)
                return formatedDateTime;

            CheckAndCreateEventCalendarAccount(player);

            DateTime adjustedDateTime = dateTime;

            if (player.m_EventCalendarAccount.TimeZoneOffset > 0)
                adjustedDateTime = dateTime + TimeSpan.FromHours(Math.Abs(player.m_EventCalendarAccount.TimeZoneOffset));

            else if (player.m_EventCalendarAccount.TimeZoneOffset < 0)
                adjustedDateTime = dateTime - TimeSpan.FromHours(Math.Abs(player.m_EventCalendarAccount.TimeZoneOffset));
            
            string month = "Jan";

            switch (adjustedDateTime.Month)
            {
                case 1: month = "Jan"; break;
                case 2: month = "Feb"; break;
                case 3: month = "Mar"; break;
                case 4: month = "Apr"; break;
                case 5: month = "May"; break;
                case 6: month = "Jun"; break;
                case 7: month = "Jul"; break;
                case 8: month = "Aug"; break;
                case 9: month = "Sep"; break;
                case 10: month = "Oct"; break;
                case 11: month = "Nov"; break;
                case 12: month = "Dec"; break;
            }

            string day = adjustedDateTime.Day.ToString();

            int hour = adjustedDateTime.Hour;
            int minute = adjustedDateTime.Minute;

            string minuteText = "";

            if (minute.ToString().Length == 1)
                minuteText += "0";

            minuteText += minute.ToString();

            string AMPM = "am";
            
            if (hour >= 12)
            {
                AMPM = "pm";

                if (hour >= 13)
                    hour -= 12;
            }            

            string time = hour.ToString() + ":" + minuteText + " " + AMPM;            

            formatedDateTime = month + " " + day + " @ " + time;
            
            return formatedDateTime;
        }

        public static string FormattedDuration(TimeSpan duration)
        {
            string formatedDateTime = "";

            formatedDateTime = Utility.CreateTimeRemainingString(DateTime.UtcNow, DateTime.UtcNow + duration, false, true, true, true, false); 

            return formatedDateTime;
        }

        public static int GetEventLevelHue(EventCalendarEvent.EventLevelType levelType)
        {
            switch (levelType)
            {
                case EventCalendarEvent.EventLevelType.Official: return 1164;
                case EventCalendarEvent.EventLevelType.Guild: return 2599;
                case EventCalendarEvent.EventLevelType.Player: return 2625;
            }

            return 0;
        }

        public static int GetEventTypeHue(EventCalendarEvent.EventTypeType eventType)
        {
            switch (eventType)
            {
                case EventCalendarEvent.EventTypeType.Major: return 2115;
                case EventCalendarEvent.EventTypeType.Minor: return 2548;
                case EventCalendarEvent.EventTypeType.Recurring: return 2562;
            }

            return 0;
        }

        public static void EventChanged(PlayerMobile playerEditor, EventCalendarEvent eventChanged)
        {
            if (playerEditor == null || eventChanged == null)
                return;

            foreach (NetState state in NetState.Instances)
            {
                Mobile mobile = state.Mobile;
                PlayerMobile player = mobile as PlayerMobile;

                if (player == null) continue;
                if (player == playerEditor) continue;

                CheckAndCreateEventCalendarAccount(player);

                if (player.m_EventCalendarAccount.SelectedEvent == eventChanged)
                {
                    player.CloseGump(typeof(EventCalendarGump));
                    player.SendGump(new EventCalendarGump(player));

                    player.SendMessage(149, "Details of the event you were viewing have changed.");
                }

                if (player.m_EventCalendarAccount.EditingEvent == eventChanged)
                {
                    if (player.m_EventCalendarAccount.StoredValuesEvent != null)
                        player.m_EventCalendarAccount.StoredValuesEvent.Delete();

                    if (player.HasGump(typeof(EventCalendarEditorGump)))
                        player.CloseGump(typeof(EventCalendarEditorGump));

                    player.SendMessage(149, "Details of the event you were editing have been changed by another player.");
                }                
            }
        }

        public static void EventDeleted(PlayerMobile playerEditor, EventCalendarEvent eventChanged)
        {
            if (playerEditor == null || eventChanged == null)
                return;

            foreach (NetState state in NetState.Instances)
            {
                Mobile mobile = state.Mobile;
                PlayerMobile player = mobile as PlayerMobile;

                if (player == null) continue;
                if (player == playerEditor) continue;

                CheckAndCreateEventCalendarAccount(player);

                bool selectedEvent = false;

                if (player.m_EventCalendarAccount.SelectedEvent == eventChanged)
                {
                    player.m_EventCalendarAccount.SelectedEvent = null;

                    player.CloseGump(typeof(EventCalendarGump));
                    player.SendGump(new EventCalendarGump(player));
                    
                    player.SendMessage(149, "The event you were viewing has been deleted.");
                }

                if (player.m_EventCalendarAccount.EditingEvent == eventChanged)
                {
                    player.m_EventCalendarAccount.EditingEvent = null;

                    if (player.m_EventCalendarAccount.StoredValuesEvent != null)
                        player.m_EventCalendarAccount.StoredValuesEvent.Delete();

                    if (player.HasGump(typeof(EventCalendarEditorGump)))                    
                        player.CloseGump(typeof(EventCalendarEditorGump));   
                    
                    player.SendMessage(149, "The event you were editing has been deleted by another player.");                    
                }
            }            
        }

        public static void CheckForChangeClearingSubmittedStatus(PlayerMobile playerEditor, EventCalendarEvent eventChanged)
        {
            if (playerEditor == null || eventChanged == null) return;

            CheckAndCreateEventCalendarAccount(playerEditor);

            if (playerEditor.m_EventCalendarAccount.StoredValuesEvent == null) 
                return;

            //Only Applies to Player Events
            if (eventChanged.EventLevel != EventCalendarEvent.EventLevelType.Player)
                return;

            //Staff Editing Event Shouldn't Force Need for Reapproval
            if (playerEditor.AccessLevel > AccessLevel.Player)
                return;

            eventChanged.EventSubmitted = false;
            eventChanged.ApprovalStatus = EventCalendarEvent.ApprovalStatusType.Pending;
            eventChanged.ApprovalText = "Awaiting staff approval";
        }
        
        public static void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); //Version

            //Version 0
            writer.Write(m_Enabled);

            writer.Write(m_EventCalendarEvents.Count);
            for (int a = 0; a < m_EventCalendarEvents.Count; a++)
            {
                writer.Write(m_EventCalendarEvents[a]);
            }
        }

        public static void Deserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            //Version 0
            if (version >= 0)
            {
                m_Enabled = reader.ReadBool();

                int eventsCount = reader.ReadInt();

                for (int a = 0; a < eventsCount; a++)
                {
                    EventCalendarEvent eventCalendarEvent = (EventCalendarEvent)reader.ReadItem();

                    m_EventCalendarEvents.Add(eventCalendarEvent);
                }
            }     
        }        
    }

    public class EventCalendarAccount : Item
    {
        public string AccountUsername = "";
        public double TimeZoneOffset = 0;

        public int DisplayPage = 1;

        public EventCalendarEvent SelectedEvent = null;
        public EventCalendarEvent EditingEvent = null;
        public EventCalendarEvent StoredValuesEvent = null;

        public bool FilterOfficialEvents = true;
        public bool FilterGuildEvents = true;
        public bool FilterPlayerEvents = true;

        public bool FilterMajorEvents = true;
        public bool FilterMinorEvents = true;
        public bool FilterRecurringEvents = true;

        public bool FilterMyEvents = true;
        public bool FilterEventsInProgress = true;

        public DateTime PastDate = DateTime.UtcNow;

        public List<EventCalendarAlertSetting> AlertSettings = new List<EventCalendarAlertSetting>();

        [Constructable]
        public EventCalendarAccount(string accountName): base(0x0)
        {
            Visible = false;
            Movable = false;

            AccountUsername = accountName;

            PopulateAlertSettings();

            EventCalendarPersistance.m_EventCalendarAccounts.Add(this);
        }

        public void PopulateAlertSettings()
        {
            AlertSettings.Add(new EventCalendarAlertSetting(EventCalendarEvent.EventLevelType.Official, EventCalendarEvent.EventTypeType.Major));
            AlertSettings.Add(new EventCalendarAlertSetting(EventCalendarEvent.EventLevelType.Official, EventCalendarEvent.EventTypeType.Minor));
            AlertSettings.Add(new EventCalendarAlertSetting(EventCalendarEvent.EventLevelType.Official, EventCalendarEvent.EventTypeType.Recurring));
            AlertSettings.Add(new EventCalendarAlertSetting(EventCalendarEvent.EventLevelType.Guild, EventCalendarEvent.EventTypeType.Major));
            AlertSettings.Add(new EventCalendarAlertSetting(EventCalendarEvent.EventLevelType.Guild, EventCalendarEvent.EventTypeType.Minor));
            AlertSettings.Add(new EventCalendarAlertSetting(EventCalendarEvent.EventLevelType.Guild, EventCalendarEvent.EventTypeType.Recurring));
            AlertSettings.Add(new EventCalendarAlertSetting(EventCalendarEvent.EventLevelType.Player, EventCalendarEvent.EventTypeType.Major));
            AlertSettings.Add(new EventCalendarAlertSetting(EventCalendarEvent.EventLevelType.Player, EventCalendarEvent.EventTypeType.Minor));
        }

        public EventCalendarAlertSetting GetAlertSetting(EventCalendarEvent.EventLevelType eventLevel, EventCalendarEvent.EventTypeType eventType)
        {
            EventCalendarAlertSetting alertSetting = null;

            foreach (EventCalendarAlertSetting alert in AlertSettings)
            {
                if (alert.EventLevel == eventLevel && alert.EventType == eventType)
                    return alert;
            }

            return alertSetting;
        }

        public EventCalendarAccount(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            //Version 0
            writer.Write(AccountUsername);
            writer.Write(TimeZoneOffset);

            writer.Write(DisplayPage);

            writer.Write(SelectedEvent);
            writer.Write(EditingEvent);
            writer.Write(StoredValuesEvent);

            writer.Write(FilterOfficialEvents);
            writer.Write(FilterGuildEvents);
            writer.Write(FilterPlayerEvents);

            writer.Write(FilterMajorEvents);
            writer.Write(FilterMinorEvents);
            writer.Write(FilterRecurringEvents);

            writer.Write(FilterMyEvents);
            writer.Write(FilterEventsInProgress);

            writer.Write(PastDate);

            writer.Write(AlertSettings.Count);
            for (int a = 0; a < AlertSettings.Count; a++)
            {
                writer.Write((int)AlertSettings[a].EventLevel);
                writer.Write((int)AlertSettings[a].EventType);

                writer.Write(AlertSettings[a].AlertPopUp);
                writer.Write(AlertSettings[a].AlertAudio);

                writer.Write(AlertSettings[a].AlertFiveMinutesPrior);
                writer.Write(AlertSettings[a].AlertFifteenMinutesPrior);
                writer.Write(AlertSettings[a].AlertOneHourPrior);
                writer.Write(AlertSettings[a].AlertTwoHoursPrior);
                writer.Write(AlertSettings[a].AlertTwentyFourHoursPrior);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                AccountUsername = reader.ReadString();
                TimeZoneOffset = reader.ReadDouble();

                DisplayPage = reader.ReadInt();
                SelectedEvent = (EventCalendarEvent)reader.ReadItem();
                EditingEvent = (EventCalendarEvent)reader.ReadItem();
                StoredValuesEvent = (EventCalendarEvent)reader.ReadItem();

                FilterOfficialEvents = reader.ReadBool();
                FilterGuildEvents = reader.ReadBool();
                FilterPlayerEvents = reader.ReadBool();

                FilterMajorEvents = reader.ReadBool();
                FilterMinorEvents = reader.ReadBool();
                FilterRecurringEvents = reader.ReadBool();

                FilterMyEvents = reader.ReadBool();
                FilterEventsInProgress = reader.ReadBool();

                PastDate = reader.ReadDateTime();

                int alertSettingsCount = reader.ReadInt();

                for (int a = 0; a < alertSettingsCount; a++)
                {
                    EventCalendarEvent.EventLevelType eventLevel = (EventCalendarEvent.EventLevelType)reader.ReadInt();
                    EventCalendarEvent.EventTypeType eventType = (EventCalendarEvent.EventTypeType)reader.ReadInt();

                    EventCalendarAlertSetting alertSetting = new EventCalendarAlertSetting(eventLevel, eventType);

                    alertSetting.AlertPopUp = reader.ReadBool();
                    alertSetting.AlertAudio = reader.ReadBool();

                    alertSetting.AlertFiveMinutesPrior = reader.ReadBool();
                    alertSetting.AlertFifteenMinutesPrior = reader.ReadBool();
                    alertSetting.AlertOneHourPrior = reader.ReadBool();
                    alertSetting.AlertTwoHoursPrior = reader.ReadBool();
                    alertSetting.AlertTwentyFourHoursPrior = reader.ReadBool();

                    AlertSettings.Add(alertSetting);
                }
            }

            EventCalendarPersistance.m_EventCalendarAccounts.Add(this);
        }        
    }

    public class EventCalendarAlertSetting
    {
        public EventCalendarEvent.EventLevelType EventLevel = EventCalendarEvent.EventLevelType.Official;
        public EventCalendarEvent.EventTypeType EventType = EventCalendarEvent.EventTypeType.Major;

        public bool AlertPopUp = true;
        public bool AlertAudio = true;

        public bool AlertFiveMinutesPrior = false;
        public bool AlertFifteenMinutesPrior = true;
        public bool AlertOneHourPrior = false;
        public bool AlertTwoHoursPrior = false;
        public bool AlertTwentyFourHoursPrior = false;

        public EventCalendarAlertSetting(EventCalendarEvent.EventLevelType eventLevel, EventCalendarEvent.EventTypeType eventType)
        {
            EventLevel = eventLevel;
            EventType = eventType;
        }
    }

    public class EventCalendarEvent : Item
    {
        public enum EventLevelType
        {
            Official,
            Guild,
            Player
        }

        public enum EventTypeType
        {            
            Minor,
            Major,
            Recurring
        }        

        public enum ApprovalStatusType
        {
            Approved,
            Pending,
            Denied
        }
        
        public EventLevelType EventLevel = EventLevelType.Official;
        public EventTypeType EventType = EventTypeType.Minor;
        
        public bool EventSubmitted = false;  
        public ApprovalStatusType ApprovalStatus = ApprovalStatusType.Pending;
        public string ApprovalText = "Awaiting staff approval";      
        
        public DateTime StartTime = DateTime.UtcNow + TimeSpan.FromHours(24);
        public TimeSpan Duration = TimeSpan.FromMinutes(60);
        public TimeSpan FrequencyInterval = TimeSpan.FromDays(7);
        
        public string DisplayName = "Enter event name here";
        public string DescriptionText = "Enter event description here";
        public string WebLink = "";

        public int EventIcon = 99;
        public int EventIconHue = 0;

        public string LocationText = "Enter event location here";
        public Point3D LocationPoint = new Point3D(1496, 1630, 10);
        public Map LocationMap = Map.Felucca;        

        public PlayerMobile Creator = null;
        public string CreatorName = "";
        public BaseGuild Guild = null;
        public DateTime CreationDate = DateTime.UtcNow;
        public DateTime LastUpdated = DateTime.UtcNow;

        public bool AlertMadeFiveMinutesPrior = false;
        public bool AlertMadeFifteenMinutesPrior = false;
        public bool AlertMadeOneHourPrior = false;
        public bool AlertMadeTwoHoursPrior = false;
        public bool AlertMadeTwentyFourHoursPrior = false;

        public List<EventCalendarAccount> m_AccountsIgnoringAlert = new List<EventCalendarAccount>();

        public void PushEventValues(EventCalendarEvent eventTo)
        {           
            if (eventTo == null) return;
            if (eventTo.Deleted) return;

            eventTo.EventLevel = EventLevel;
            eventTo.EventType = EventType;

            eventTo.EventSubmitted = EventSubmitted;
            eventTo.ApprovalStatus = ApprovalStatus;
            eventTo.ApprovalText = ApprovalText;

            eventTo.StartTime = StartTime;
            eventTo.Duration = Duration;
            eventTo.FrequencyInterval = FrequencyInterval;

            eventTo.DisplayName = DisplayName;
            eventTo.DescriptionText = DescriptionText;
            eventTo.WebLink = WebLink;

            eventTo.EventIcon = EventIcon;
            eventTo.EventIconHue = EventIconHue;

            eventTo.LocationText = LocationText;
            eventTo.LocationPoint = LocationPoint;
            eventTo.LocationMap = LocationMap;

            eventTo.Creator = Creator;
            eventTo.CreatorName = CreatorName;
            eventTo.Guild = Guild;
            eventTo.CreationDate = CreationDate;
            eventTo.LastUpdated = LastUpdated;

            eventTo.AlertMadeFiveMinutesPrior = AlertMadeFiveMinutesPrior;
            eventTo.AlertMadeFifteenMinutesPrior = AlertMadeFifteenMinutesPrior;
            eventTo.AlertMadeOneHourPrior = AlertMadeOneHourPrior;
            eventTo.AlertMadeTwoHoursPrior = AlertMadeTwoHoursPrior;
            eventTo.AlertMadeTwentyFourHoursPrior = AlertMadeTwentyFourHoursPrior;

            eventTo.m_AccountsIgnoringAlert = new List<EventCalendarAccount>();
            foreach (EventCalendarAccount account in m_AccountsIgnoringAlert)
            {
                eventTo.m_AccountsIgnoringAlert.Add(account);
            }
        }
        
        [Constructable]
        public EventCalendarEvent(): base(0x0)
        {
            Visible = false;
            Movable = false;
        }

        public EventCalendarEvent(Serial serial): base(serial)
        {
        }

        public override void OnDelete()
        {
            if (EventCalendarPersistance.m_EventCalendarEvents.Contains(this))
                EventCalendarPersistance.m_EventCalendarEvents.Remove(this);

            base.OnDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            //Version 0
            writer.Write((int)EventLevel);
            writer.Write((int)EventType);

            writer.Write(EventSubmitted);
            writer.Write((int)ApprovalStatus);
            writer.Write(ApprovalText);

            writer.Write(StartTime);
            writer.Write(Duration);
            writer.Write(FrequencyInterval);

            writer.Write(DisplayName);
            writer.Write(DescriptionText);
            writer.Write(WebLink);

            writer.Write(EventIcon);
            writer.Write(EventIconHue);

            writer.Write(LocationText);
            writer.Write(LocationPoint);
            writer.Write(LocationMap);

            writer.Write(Creator);
            writer.Write(CreatorName);
            writer.Write(Guild);
            writer.Write(CreationDate);
            writer.Write(LastUpdated);

            writer.Write(AlertMadeFiveMinutesPrior);
            writer.Write(AlertMadeFifteenMinutesPrior);
            writer.Write(AlertMadeOneHourPrior);
            writer.Write(AlertMadeTwoHoursPrior);
            writer.Write(AlertMadeTwentyFourHoursPrior);

            writer.Write(m_AccountsIgnoringAlert.Count);
            for (int a = 0; a < m_AccountsIgnoringAlert.Count; a++)
            {
                writer.Write(m_AccountsIgnoringAlert[a]);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                EventLevel = (EventLevelType)reader.ReadInt();
                EventType = (EventTypeType)reader.ReadInt();

                EventSubmitted = reader.ReadBool();
                ApprovalStatus = (ApprovalStatusType)reader.ReadInt();
                ApprovalText = reader.ReadString();

                StartTime = reader.ReadDateTime();
                Duration = reader.ReadTimeSpan();
                FrequencyInterval = reader.ReadTimeSpan();

                DisplayName = reader.ReadString();
                DescriptionText = reader.ReadString();
                WebLink = reader.ReadString();

                EventIcon = reader.ReadInt();
                EventIconHue = reader.ReadInt();

                LocationText = reader.ReadString();
                LocationPoint = reader.ReadPoint3D();
                LocationMap = reader.ReadMap();

                Creator = (PlayerMobile)reader.ReadMobile();
                CreatorName = reader.ReadString();
                Guild = reader.ReadGuild();
                CreationDate = reader.ReadDateTime();
                LastUpdated = reader.ReadDateTime();

                AlertMadeFiveMinutesPrior = reader.ReadBool();
                AlertMadeFifteenMinutesPrior = reader.ReadBool();
                AlertMadeOneHourPrior = reader.ReadBool();
                AlertMadeTwoHoursPrior = reader.ReadBool();
                AlertMadeTwentyFourHoursPrior = reader.ReadBool();

                int accountsIgnoringAlertCount = reader.ReadInt();
                for (int a = 0; a < accountsIgnoringAlertCount; a++)
                {
                    EventCalendarAccount account = (EventCalendarAccount)reader.ReadItem();
                    m_AccountsIgnoringAlert.Add(account);
                }
            }
        }
    }

    public class EventIconDetail
    {
        public int ItemId = 8397;
        public int OffsetX = 0;
        public int OffsetY = 0;
    }
}