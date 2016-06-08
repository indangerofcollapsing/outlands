using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;
using Server.Gumps;


namespace Server.Items
{    
    public class EventCalendarGump : Gump
    {
        PlayerMobile m_Player;

        public int EntriesPerPage = 7;      
        
        public EventCalendarGump(PlayerMobile m_Player): base(10, 10)
        {
            m_Player = m_Player;

            if (m_Player == null) return;
            if (m_Player.Deleted) return;

            EventCalendarPersistance.CheckAndCreateEventCalendarAccount(m_Player);

            if (m_Player.m_EventCalendarAccount == null) 
                return;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int BackgroundHue = 0; //2412
          
            #region Scroll Background

            AddImage(2, 3, 9390, BackgroundHue);
            AddImage(654, 3, 9392, BackgroundHue);
            AddImage(114, 3, 9391, BackgroundHue);            
            AddImage(2, 141, 9393, BackgroundHue);
            AddImage(654, 142, 9395, BackgroundHue);
            AddImage(115, 452, 9397, BackgroundHue);
            AddImage(116, 140, 9394, BackgroundHue);
            AddImage(164, 3, 9391, BackgroundHue);
            AddImage(202, 3, 9391, BackgroundHue);
            AddImage(250, 3, 9391, BackgroundHue);
            AddImage(306, 3, 9391, BackgroundHue);
            AddImage(355, 3, 9391, BackgroundHue);
            AddImage(405, 3, 9391, BackgroundHue);
            AddImage(443, 3, 9391, BackgroundHue);
            AddImage(491, 3, 9391, BackgroundHue);
            AddImage(547, 3, 9391, BackgroundHue);
            AddImage(598, 3, 9391, BackgroundHue);
            AddImage(2, 175, 9393, BackgroundHue);
            AddImage(2, 211, 9393, BackgroundHue);
            AddImage(2, 246, 9393, BackgroundHue);
            AddImage(2, 280, 9393, BackgroundHue);
            AddImage(2, 316, 9393, BackgroundHue);
            AddImage(2, 350, 9393, BackgroundHue);
            AddImage(2, 391, 9393, BackgroundHue);
            AddImage(2, 420, 9393, BackgroundHue);

            AddImage(654, 180, 9395, BackgroundHue);
            AddImage(654, 221, 9395, BackgroundHue);
            AddImage(654, 258, 9395, BackgroundHue);
            AddImage(654, 291, 9395, BackgroundHue);
            AddImage(654, 329, 9395, BackgroundHue);
            AddImage(654, 370, 9395, BackgroundHue);
            AddImage(654, 407, 9395, BackgroundHue);
            AddImage(654, 430, 9395, BackgroundHue);
            AddImage(654, 452, 9398, BackgroundHue);

            AddImage(2, 452, 9396, BackgroundHue);

            AddImage(157, 140, 9394, BackgroundHue);

            AddImage(162, 452, 9397, BackgroundHue);
            AddImage(212, 452, 9397, BackgroundHue);
            AddImage(269, 452, 9397, BackgroundHue);
            AddImage(316, 452, 9397, BackgroundHue);
            AddImage(366, 452, 9397, BackgroundHue);
            AddImage(395, 452, 9397, BackgroundHue);
            AddImage(442, 452, 9397, BackgroundHue);
            AddImage(492, 452, 9397, BackgroundHue);
            AddImage(500, 452, 9397, BackgroundHue);
            AddImage(547, 452, 9397, BackgroundHue);
            AddImage(597, 452, 9397, BackgroundHue);
            AddImage(207, 140, 9394, BackgroundHue);
            AddImage(248, 140, 9394, BackgroundHue);
            AddImage(296, 140, 9394, BackgroundHue);
            AddImage(337, 140, 9394, BackgroundHue);
            AddImage(376, 140, 9394, BackgroundHue);
            AddImage(417, 140, 9394, BackgroundHue);
            AddImage(467, 140, 9394, BackgroundHue);
            AddImage(508, 140, 9394, BackgroundHue);
            AddImage(556, 140, 9394, BackgroundHue);
            AddImage(597, 140, 9394, BackgroundHue);
            AddImage(116, 182, 9394, BackgroundHue);
            AddImage(157, 182, 9394, BackgroundHue);
            AddImage(207, 182, 9394, BackgroundHue);
            AddImage(248, 182, 9394, BackgroundHue);
            AddImage(296, 182, 9394, BackgroundHue);
            AddImage(337, 182, 9394, BackgroundHue);
            AddImage(376, 182, 9394, BackgroundHue);
            AddImage(417, 182, 9394, BackgroundHue);
            AddImage(467, 182, 9394, BackgroundHue);
            AddImage(508, 182, 9394, BackgroundHue);
            AddImage(556, 182, 9394, BackgroundHue);
            AddImage(597, 182, 9394, BackgroundHue);
            AddImage(116, 220, 9394, BackgroundHue);
            AddImage(157, 220, 9394, BackgroundHue);
            AddImage(207, 220, 9394, BackgroundHue);
            AddImage(248, 220, 9394, BackgroundHue);
            AddImage(296, 220, 9394, BackgroundHue);
            AddImage(337, 220, 9394, BackgroundHue);
            AddImage(376, 220, 9394, BackgroundHue);
            AddImage(417, 220, 9394, BackgroundHue);
            AddImage(467, 220, 9394, BackgroundHue);
            AddImage(508, 220, 9394, BackgroundHue);
            AddImage(556, 220, 9394, BackgroundHue);
            AddImage(597, 220, 9394, BackgroundHue);
            AddImage(116, 254, 9394, BackgroundHue);
            AddImage(157, 254, 9394, BackgroundHue);
            AddImage(207, 254, 9394, BackgroundHue);
            AddImage(248, 254, 9394, BackgroundHue);
            AddImage(296, 254, 9394, BackgroundHue);
            AddImage(337, 254, 9394, BackgroundHue);
            AddImage(376, 254, 9394, BackgroundHue);
            AddImage(417, 254, 9394, BackgroundHue);
            AddImage(467, 254, 9394, BackgroundHue);
            AddImage(508, 254, 9394, BackgroundHue);
            AddImage(556, 254, 9394, BackgroundHue);
            AddImage(597, 254, 9394, BackgroundHue);
            AddImage(116, 294, 9394, BackgroundHue);
            AddImage(157, 294, 9394, BackgroundHue);
            AddImage(207, 294, 9394, BackgroundHue);
            AddImage(248, 294, 9394, BackgroundHue);
            AddImage(296, 294, 9394, BackgroundHue);
            AddImage(337, 294, 9394, BackgroundHue);
            AddImage(376, 294, 9394, BackgroundHue);
            AddImage(417, 294, 9394, BackgroundHue);
            AddImage(467, 294, 9394, BackgroundHue);
            AddImage(508, 294, 9394, BackgroundHue);
            AddImage(556, 294, 9394, BackgroundHue);
            AddImage(597, 294, 9394, BackgroundHue);
            AddImage(116, 335, 9394, BackgroundHue);
            AddImage(157, 335, 9394, BackgroundHue);
            AddImage(207, 335, 9394, BackgroundHue);
            AddImage(248, 335, 9394, BackgroundHue);
            AddImage(296, 335, 9394, BackgroundHue);
            AddImage(337, 335, 9394, BackgroundHue);
            AddImage(376, 335, 9394, BackgroundHue);
            AddImage(417, 335, 9394, BackgroundHue);
            AddImage(467, 335, 9394, BackgroundHue);
            AddImage(508, 335, 9394, BackgroundHue);
            AddImage(556, 335, 9394, BackgroundHue);
            AddImage(597, 335, 9394, BackgroundHue);
            AddImage(116, 377, 9394, BackgroundHue);
            AddImage(157, 377, 9394, BackgroundHue);
            AddImage(207, 377, 9394, BackgroundHue);
            AddImage(248, 377, 9394, BackgroundHue);
            AddImage(296, 377, 9394, BackgroundHue);
            AddImage(337, 377, 9394, BackgroundHue);
            AddImage(376, 377, 9394, BackgroundHue);
            AddImage(417, 377, 9394, BackgroundHue);
            AddImage(467, 377, 9394, BackgroundHue);
            AddImage(508, 377, 9394, BackgroundHue);
            AddImage(556, 377, 9394, BackgroundHue);
            AddImage(597, 377, 9394, BackgroundHue);

            AddImage(116, 420, 9394, BackgroundHue);
            AddImage(157, 420, 9394, BackgroundHue);
            AddImage(207, 420, 9394, BackgroundHue);
            AddImage(248, 420, 9394, BackgroundHue);
            AddImage(296, 420, 9394, BackgroundHue);
            AddImage(337, 420, 9394, BackgroundHue);
            AddImage(376, 420, 9394, BackgroundHue);
            AddImage(417, 420, 9394, BackgroundHue);
            AddImage(467, 420, 9394, BackgroundHue);
            AddImage(508, 420, 9394, BackgroundHue);
            AddImage(556, 420, 9394, BackgroundHue);
            AddImage(597, 420, 9394, BackgroundHue);

            #endregion
            			
			AddLabel(327, 9, 2606, "An Corp Event Calendar");
			
			AddImage(29, 104, 3001, 2415);
			AddImage(272, 104, 3001, 2415);
			AddImage(496, 104, 3001, 2415);
			
			AddImage(29, 418, 3001, 2415);
			AddImage(272, 418, 3001, 2415);
			AddImage(497, 418, 3001, 2415);

            int WhiteTextHue = 2499;
            int GreenTextHue = 0x3F;
        
            //-------------            
            List<EventCalendarEvent> Events = EventCalendarPersistance.GetCalendarEventsForPlayer(m_Player, m_Player.m_EventCalendarAccount.PastDate);

            int CurrentPage = m_Player.m_EventCalendarAccount.DisplayPage;
            int TotalEntries = Events.Count;
            int TotalPages = (int)(Math.Ceiling((double)Events.Count / (double)EntriesPerPage));           

            if (TotalPages == 0)
                TotalPages = 1;

            if (CurrentPage < 1)
            {
                CurrentPage = 1;
                m_Player.m_EventCalendarAccount.DisplayPage = 1;
            }

            if (CurrentPage > TotalPages)
            {
                CurrentPage = TotalPages;
                m_Player.m_EventCalendarAccount.DisplayPage = TotalPages;
            }

            EventCalendarPersistance.ValidateSelectedEvent(m_Player, m_Player.m_EventCalendarAccount.PastDate);
            EventCalendarEvent selectedEvent = m_Player.m_EventCalendarAccount.SelectedEvent;

            //Wiki
            AddLabel(48, 5, 149, "Wiki Page");
            AddButton(21, 3, 2095, 2095, 18, GumpButtonType.Reply, 0);

            //Filters
            AddLabel(29, 54, WhiteTextHue, "Filters");

            if (m_Player.m_EventCalendarAccount.FilterOfficialEvents)
                AddButton(79, 36, 2154, 2151, 6, GumpButtonType.Reply, 0);
            else
                AddButton(79, 36, 2151, 2154, 6, GumpButtonType.Reply, 0);
            AddLabel(112, 40, 1164, "Official");

            if (m_Player.m_EventCalendarAccount.FilterGuildEvents)
                AddButton(174, 36, 2154, 2151, 7, GumpButtonType.Reply, 0);
            else
                AddButton(174, 36, 2151, 2154, 7, GumpButtonType.Reply, 0);
            AddLabel(207, 39, 2599, "Guild");

            if (m_Player.m_EventCalendarAccount.FilterPlayerEvents)
                AddButton(259, 36, 2154, 2151, 8, GumpButtonType.Reply, 0);
            else
                AddButton(259, 36, 2151, 2154, 8, GumpButtonType.Reply, 0);
            AddLabel(294, 40, 2625, "Player");

            if (m_Player.m_EventCalendarAccount.FilterMyEvents)
                AddButton(367, 36, 2154, 2151, 9, GumpButtonType.Reply, 0);
            else
                AddButton(367, 36, 2151, 2154, 9, GumpButtonType.Reply, 0);
            AddLabel(401, 40, 2507, "My Events");

            if (m_Player.m_EventCalendarAccount.FilterMajorEvents)
                AddButton(79, 68, 2154, 2151, 10, GumpButtonType.Reply, 0);
            else
                AddButton(79, 68, 2151, 2154, 10, GumpButtonType.Reply, 0);
            AddLabel(112, 72, 2115, "Major");

            if (m_Player.m_EventCalendarAccount.FilterMinorEvents)
                AddButton(174, 68, 2154, 2151, 11, GumpButtonType.Reply, 0);
            else
                AddButton(174, 68, 2151, 2154, 11, GumpButtonType.Reply, 0);
            AddLabel(207, 72, 2548, "Minor");

            if (m_Player.m_EventCalendarAccount.FilterRecurringEvents)
                AddButton(258, 68, 2154, 2151, 12, GumpButtonType.Reply, 0);
            else
                AddButton(258, 68, 2151, 2154, 12, GumpButtonType.Reply, 0);
            AddLabel(293, 72, 2562, "Recurring");

            if (m_Player.m_EventCalendarAccount.FilterEventsInProgress)
                AddButton(367, 68, 2154, 2151, 13, GumpButtonType.Reply, 0);
            else
                AddButton(367, 68, 2151, 2154, 13, GumpButtonType.Reply, 0);
            AddLabel(400, 72, 2509, "Events in Progress");

            //Past Event Filter
            AddLabel(558, 34, 50, "Show Events Starting After");

            string pastDateText = m_Player.m_EventCalendarAccount.PastDate.ToShortDateString();

            AddLabel(Utility.CenteredTextOffset(645, pastDateText), 56, WhiteTextHue, pastDateText);
            AddButton(575, 81, 2223, 2223, 14, GumpButtonType.Reply, 0);
            AddLabel(605, 77, 2534, "Starting Date");
            AddButton(697, 81, 2224, 2224, 15, GumpButtonType.Reply, 0);

            //Headings
            AddLabel(28, 110, 149, "Select");
            AddLabel(197, 110, 149, "Event Name");
            AddLabel(444, 110, 149, "Event Time");
            AddLabel(546, 110, 149, "Begins In");
            AddLabel(628, 110, 149, "Level");
            AddLabel(699, 110, 149, "Type");

            int startY = 140;
            int rowSpacing = 55;

            //Event Rows Loops
            if (Events.Count > 0)
            {
                int startIndex = (CurrentPage - 1) * EntriesPerPage;
                int endIndex = startIndex + EntriesPerPage;

                if (endIndex > TotalEntries)
                    endIndex = TotalEntries;

                for (int a = startIndex; a < endIndex; a++)
                {
                    if (a < Events.Count)
                    {
                        EventCalendarEvent currentEvent = Events[a];

                        if (currentEvent != null)
                        {
                            bool eventSelected = false;

                            int textHue = WhiteTextHue;

                            if (m_Player.m_EventCalendarAccount.SelectedEvent == currentEvent)
                            {
                                eventSelected = true;
                                textHue = GreenTextHue;
                            }

                            if (eventSelected)
                                AddButton(33, startY, 9724, 9721, 20 + a, GumpButtonType.Reply, 0);
                            else
                                AddButton(33, startY, 9721, 9724, 20 + a, GumpButtonType.Reply, 0);

                            EventIconDetail iconDetail = EventCalendarPersistance.GetIconDetailDetail(currentEvent.EventIcon);

                            AddItem(69 + iconDetail.OffsetX, startY - 1 + iconDetail.OffsetY, iconDetail.ItemId, currentEvent.Hue);

                            string eventName = currentEvent.DisplayName;

                            AddLabel(Utility.CenteredTextOffset(260, eventName), startY + 5, textHue, eventName);

                            string startTimeText = EventCalendarPersistance.PlayerFormattedDateTime(m_Player, currentEvent.StartTime);

                            AddLabel(Utility.CenteredTextOffset(470, startTimeText), startY + 5, textHue, startTimeText);

                            string beginsInText = "";

                            if (currentEvent.StartTime > DateTime.UtcNow)
                                beginsInText = Utility.CreateTimeRemainingString(DateTime.UtcNow, currentEvent.StartTime, true, true, true, true, false);
                            else
                            {
                                if (currentEvent.StartTime + currentEvent.Duration > DateTime.UtcNow)
                                    beginsInText = "In Progress";
                                else
                                    beginsInText = "Ended";
                            }

                            AddLabel(Utility.CenteredTextOffset(575, beginsInText), startY + 5, textHue, beginsInText);

                            int levelHue = EventCalendarPersistance.GetEventLevelHue(currentEvent.EventLevel);
                            int typeHue = EventCalendarPersistance.GetEventTypeHue(currentEvent.EventType);

                            string levelText = currentEvent.EventLevel.ToString();
                            string typeText = currentEvent.EventType.ToString();

                            AddLabel(Utility.CenteredTextOffset(650, levelText), startY + 5, levelHue, levelText);
                            AddLabel(Utility.CenteredTextOffset(715, typeText), startY + 5, typeHue, typeText);
                        }

                        startY += rowSpacing;
                    }
                }
            }

            //Previous
            if (CurrentPage > 1)
            {
                AddButton(117, 393, 4014, 4016, 4, GumpButtonType.Reply, 0);
                AddLabel(152, 393, WhiteTextHue, "Previous Page");
            }

            //Next
            if (CurrentPage < TotalPages)
            {
                AddButton(557, 393, 4005, 4007, 5, GumpButtonType.Reply, 0);
                AddLabel(598, 393, WhiteTextHue, "Next Page");
            }

            //Selected
            if (selectedEvent != null)
            {
                AddImage(304, 404, 2440);
                AddLabel(317, 405, 2525, "Selected Event Details");

                AddLabel(30, 425, 149, "Event Name");
                AddLabel(116, 425, GreenTextHue, selectedEvent.DisplayName);

                AddLabel(518, 425, 149, "Event Criteria");
                AddLabel(618, 425, EventCalendarPersistance.GetEventLevelHue(selectedEvent.EventLevel), selectedEvent.EventLevel.ToString());
                AddLabel(679, 425, EventCalendarPersistance.GetEventTypeHue(selectedEvent.EventType), selectedEvent.EventType.ToString());

                AddLabel(30, 447, 149, "Event Start");
                AddLabel(111, 447, WhiteTextHue, EventCalendarPersistance.PlayerFormattedDateTime(m_Player, selectedEvent.StartTime));

                AddLabel(278, 447, 149, "Begins In");

                string beginsInText = "";

                if (selectedEvent.StartTime > DateTime.UtcNow)
                    beginsInText = Utility.CreateTimeRemainingString(DateTime.UtcNow, selectedEvent.StartTime, true, true, true, true, false);
                else
                {
                    if (selectedEvent.StartTime + selectedEvent.Duration > DateTime.UtcNow)
                        beginsInText = "In Progress";
                    else
                        beginsInText = "Ended";
                }

                AddLabel(339, 447, WhiteTextHue, beginsInText);

                AddLabel(431, 447, 149, "Duration");
                AddLabel(493, 447, WhiteTextHue, Utility.CreateTimeRemainingString(DateTime.UtcNow, DateTime.UtcNow + selectedEvent.Duration, true, true, true, true, false));
                
                AddLabel(558, 447, 149, "Creator");

                string creator = selectedEvent.CreatorName;

                if (selectedEvent.EventLevel == EventCalendarEvent.EventLevelType.Guild)
                {
                    string guildAbbreviation = "";

                    if (selectedEvent.Guild != null)
                        creator += "(" + selectedEvent.Guild.m_Abbreviation + ")";                   
                }

                if (selectedEvent.Creator == m_Player)
                    AddLabel(616, 447, GreenTextHue, creator);

                else
                    AddLabel(616, 447, WhiteTextHue, creator);

                AddLabel(30, 469, 149, "Location");
                AddLabel(93, 469, WhiteTextHue, selectedEvent.LocationText);

                AddLabel(431, 469, 149, "Coordinates");
                AddLabel(515, 469, WhiteTextHue, selectedEvent.LocationPoint.X.ToString() + ", " + selectedEvent.LocationPoint.Y.ToString());

                AddButton(597, 472, 2118, 248, 16, GumpButtonType.Reply, 0);
                AddLabel(615, 469, 2610, "View Map");

                if (selectedEvent.WebLink != "")
                {
                    AddButton(684, 472, 2118, 248, 17, GumpButtonType.Reply, 0);
                    AddLabel(701, 469, 2606, "Weblink");
                }

                AddTextEntry(25, 500, 620, 60, WhiteTextHue, 1000, selectedEvent.DescriptionText);
                
                AddLabel(660, 495, 2578, "Ignore Alerts");
                AddLabel(658, 511, 2578, "For This Event");

                bool eventIgnored = false;

                if (selectedEvent.m_AccountsIgnoringAlert.Contains(m_Player.m_EventCalendarAccount))
                    eventIgnored = true;

                if (eventIgnored)                
                    AddButton(685, 530, 9724, 9721, 19, GumpButtonType.Reply, 0);
                else
                    AddButton(685, 530, 9721, 9724, 19, GumpButtonType.Reply, 0);
            }

            //Controls
            AddButton(29, 570, 4029, 4007, 1, GumpButtonType.Reply, 0);
            AddLabel(63, 570, 2603, "Create New Event");

            bool canEditSelectedEvent = false;

            if (selectedEvent != null && canEditSelectedEvent)
            {
                AddButton(299, 570, 4026, 4007, 2, GumpButtonType.Reply, 0);
                AddLabel(333, 570, 169, "Edit Selected Event");
            }          

            AddButton(529, 570, 4002, 4007, 3, GumpButtonType.Reply, 0);
            AddLabel(563, 570, 2550, "Change Your Calendar Settings");
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (sender == null)
                return;

            PlayerMobile player = sender.Mobile as PlayerMobile;

            if (player == null) return;
            if (player.Deleted) return;

            EventCalendarPersistance.CheckAndCreateEventCalendarAccount(player);

            DateTime startingDateTimeOfSearch = DateTime.UtcNow;

            List<EventCalendarEvent> Events = EventCalendarPersistance.GetCalendarEventsForPlayer(player, startingDateTimeOfSearch);

            int CurrentPage = player.m_EventCalendarAccount.DisplayPage;
            int TotalEntries = Events.Count;
            int TotalPages = (int)(Math.Ceiling((double)Events.Count / (double)EntriesPerPage));

            if (TotalPages == 0)
                TotalPages = 1;

            if (CurrentPage < 1)
            {
                CurrentPage = 1;
                player.m_EventCalendarAccount.DisplayPage = 1;
            }

            if (CurrentPage > TotalPages)
            {
                CurrentPage = TotalPages;
                player.m_EventCalendarAccount.DisplayPage = TotalPages;
            }

            EventCalendarPersistance.ValidateSelectedEvent(player, startingDateTimeOfSearch);
            EventCalendarEvent selectedEvent = player.m_EventCalendarAccount.SelectedEvent;

            bool closeGump = true;

            switch (info.ButtonID)
            {
                case 1:
                    //Create New Event
                    player.CloseGump(typeof(EventCalendarGump));
                    player.SendGump(new EventCalendarGump(player));
                    
                    player.m_EventCalendarAccount.EditingEvent = null;

                    if (player.m_EventCalendarAccount.StoredValuesEvent != null)
                    {
                        if (!player.m_EventCalendarAccount.StoredValuesEvent.Deleted)
                            player.m_EventCalendarAccount.StoredValuesEvent.Delete();
                    }

                    player.m_EventCalendarAccount.StoredValuesEvent = new EventCalendarEvent();

                    player.m_EventCalendarAccount.StoredValuesEvent.Creator = player;
                    player.m_EventCalendarAccount.StoredValuesEvent.CreatorName = player.Name;
                    player.m_EventCalendarAccount.StoredValuesEvent.CreationDate = DateTime.UtcNow;
                    player.m_EventCalendarAccount.StoredValuesEvent.LastUpdated = DateTime.UtcNow;

                    player.CloseGump(typeof(EventCalendarGump));
                    player.SendGump(new EventCalendarGump(player));

                    player.CloseGump(typeof(EventCalendarEditorGump));
                    player.SendGump(new EventCalendarEditorGump(player));

                    return;
                break;

                case 2:
                    //Edit Existing Event
                    if (selectedEvent != null)
                    {
                        if (player.m_EventCalendarAccount.StoredValuesEvent != null)
                            player.m_EventCalendarAccount.StoredValuesEvent.Delete();                        

                        player.m_EventCalendarAccount.StoredValuesEvent = new EventCalendarEvent();
                        player.m_EventCalendarAccount.SelectedEvent.PushEventValues(player.m_EventCalendarAccount.StoredValuesEvent);

                        player.CloseGump(typeof(EventCalendarGump));
                        player.SendGump(new EventCalendarGump(player));

                        player.CloseGump(typeof(EventCalendarEditorGump));
                        player.SendGump(new EventCalendarEditorGump(player));

                        return;
                    }

                    else
                    {
                        player.SendMessage("The selected event is no longer accessible.");
                        closeGump = false;
                    }
                break;

                case 3:
                    //Change Calendar Settings
                    player.CloseGump(typeof(EventCalendarGump));
                    player.SendGump(new EventCalendarGump(player));

                    player.CloseGump(typeof(EventCalendarSettingsGump));
                    player.SendGump(new EventCalendarSettingsGump(player));

                    return;
                break;

                case 4:
                    //Previous Page
                    if (player.m_EventCalendarAccount.DisplayPage > 1)
                    {
                        player.m_EventCalendarAccount.DisplayPage--;
                        closeGump = false;
                    }
                break;

                case 5:
                    //Next Page
                    player.m_EventCalendarAccount.DisplayPage++;
                    closeGump = false;
                break;

                case 6:
                    //Filter Official Events
                    player.m_EventCalendarAccount.FilterOfficialEvents = !player.m_EventCalendarAccount.FilterOfficialEvents;
                    player.m_EventCalendarAccount.DisplayPage = 1;

                    closeGump = false;
                break;

                case 7:
                    //Filter Guild Events
                    player.m_EventCalendarAccount.FilterGuildEvents = !player.m_EventCalendarAccount.FilterGuildEvents;
                    player.m_EventCalendarAccount.DisplayPage = 1;

                    closeGump = false;
                break;

                case 8:
                    //Filter Player Events
                    player.m_EventCalendarAccount.FilterPlayerEvents = !player.m_EventCalendarAccount.FilterPlayerEvents;
                    player.m_EventCalendarAccount.DisplayPage = 1;

                    closeGump = false;
                break;

                case 9:
                //Filter My Events
                    player.m_EventCalendarAccount.FilterMyEvents = !player.m_EventCalendarAccount.FilterMyEvents;
                    player.m_EventCalendarAccount.DisplayPage = 1;

                closeGump = false;
                break;

                case 10:
                    //Filter Major Events
                    player.m_EventCalendarAccount.FilterMajorEvents = !player.m_EventCalendarAccount.FilterMajorEvents;
                    player.m_EventCalendarAccount.DisplayPage = 1;

                    closeGump = false;
                break;

                case 11:
                    //Filter Minor Events
                    player.m_EventCalendarAccount.FilterMinorEvents = !player.m_EventCalendarAccount.FilterMinorEvents;
                    player.m_EventCalendarAccount.DisplayPage = 1;

                    closeGump = false;
                break;

                case 12:
                    //Filter Recurring Events
                    player.m_EventCalendarAccount.FilterRecurringEvents = !player.m_EventCalendarAccount.FilterRecurringEvents;
                    player.m_EventCalendarAccount.DisplayPage = 1;

                    closeGump = false;
                break;

                case 13:
                    //Filter Events in Progress
                    player.m_EventCalendarAccount.FilterEventsInProgress = !player.m_EventCalendarAccount.FilterEventsInProgress;
                    player.m_EventCalendarAccount.DisplayPage = 1;

                    closeGump = false;
                break;

                case 14:
                    //Past Date Backwards
                    player.m_EventCalendarAccount.PastDate = player.m_EventCalendarAccount.PastDate - TimeSpan.FromDays(1);

                    closeGump = false;
                break;

                case 15:
                    //Past Date Forwards
                    player.m_EventCalendarAccount.PastDate = player.m_EventCalendarAccount.PastDate + TimeSpan.FromDays(1);

                    closeGump = false;
                break;

                case 16:
                    //View Map
                    closeGump = false;
                break;

                case 17:
                    //View Website
                    closeGump = false;
                break;

                case 18:
                    //View Wiki
                    closeGump = false;
                break;

                case 19:
                    //Add Player to Alert Ignore List for Event
                    if (selectedEvent != null)
                    {
                        if (selectedEvent.m_AccountsIgnoringAlert.Contains(player.m_EventCalendarAccount))
                            selectedEvent.m_AccountsIgnoringAlert.Remove(player.m_EventCalendarAccount);
                        else
                            selectedEvent.m_AccountsIgnoringAlert.Add(player.m_EventCalendarAccount);
                    }

                    closeGump = false;
                break;
            }

            //Select Event
            if (info.ButtonID >= 20 && info.ButtonID <= 20 + EntriesPerPage)
            {
                int index = info.ButtonID - 20;

                if (Events.Count > 0)
                {
                    int startIndex = (CurrentPage - 1) * EntriesPerPage;
                    int eventIndex = startIndex + index;

                    if (eventIndex < Events.Count)
                    {
                        if (player.m_EventCalendarAccount.SelectedEvent != Events[eventIndex])
                            player.m_EventCalendarAccount.SelectedEvent = Events[eventIndex];
                        else
                            player.m_EventCalendarAccount.SelectedEvent = null;
                    }
                }

                closeGump = false;
            }

            if (!closeGump)
            {
                player.CloseGump(typeof(EventCalendarGump));
                player.SendGump(new EventCalendarGump(player));
            }
        }        
    }

    public class EventCalendarAlert : Gump
    {
        PlayerMobile m_Player; 
        EventCalendarEvent m_Event;             

        public EventCalendarAlert(PlayerMobile player, EventCalendarEvent eventCalendarEvent): base(10, 10)
        {
            if (player == null || eventCalendarEvent == null)
                return;

            m_Player = player;
            m_Event = eventCalendarEvent;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int WhiteTextHue = 2499;

            AddImage(10, 10, 1249);
            
            AddLabel(171, 48, 2606, "Event Alert");
            AddLabel(Utility.CenteredTextOffset(205, m_Event.DisplayName), 65, 169, m_Event.DisplayName);

            string firstLine = "";
            string secondLine = "";

            if (m_Event.StartTime > DateTime.UtcNow)
            {
                firstLine = "Begins In";
                secondLine = Utility.CreateTimeRemainingString(DateTime.UtcNow, m_Event.StartTime, true, true, true, true, false);
            }

            else
            {
                if (m_Event.StartTime + m_Event.Duration > DateTime.UtcNow)
                {
                    firstLine = "Event Is";
                    secondLine = "In Progress";
                }

                else
                {
                    firstLine = "Event Has";
                    secondLine = "Ended";
                }
            }

            AddLabel(Utility.CenteredTextOffset(110, firstLine), 97, 149, firstLine);
            AddLabel(Utility.CenteredTextOffset(110, secondLine), 117, 2603, secondLine);

            EventIconDetail iconDetail = EventCalendarPersistance.GetIconDetailDetail(m_Event.EventIcon);

            AddItem(188 + iconDetail.OffsetX, 96 + iconDetail.OffsetY, iconDetail.ItemId, m_Event.EventIconHue);

            AddLabel(275, 97, 149, "Creator");
            AddLabel(Utility.CenteredTextOffset(300, m_Event.CreatorName), 117, WhiteTextHue, m_Event.CreatorName);

            AddLabel(55, 153, 149, "Location");
            AddLabel(119, 153, WhiteTextHue, m_Event.LocationText);

            AddLabel(55, 172, 149, "Coordinates");
            AddLabel(139, 172, WhiteTextHue, m_Event.LocationPoint.X + "," + m_Event.LocationPoint.Y);

            AddButton(287, 175, 2118, 248, 1, GumpButtonType.Reply, 0);
            AddLabel(304, 172, 2610, "View Map");

            AddTextEntry(53, 193, 310, 105, WhiteTextHue, 1000, m_Event.DescriptionText);            
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null)
                return;

            bool closeGump = true;

            if (info.ButtonID == 1)
            {
                //View Map

                closeGump = false;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(EventCalendarAlert));
                m_Player.SendGump(new EventCalendarAlert(m_Player, m_Event));
            }
        }
    }

    public class EventCalendarEditorGump : Gump
    {
        PlayerMobile m_Player;

        public int BigClickSound = 0x057;
        public int SmallClickSound = 0x3E7;
        public int OpenPageSound = 0x055;
        public int ClosePageSound = 0x058;

        public TimeSpan MinimumDuration = TimeSpan.FromMinutes(15);
        public TimeSpan MinimumFrequencyInterval = TimeSpan.FromHours(4);
        
        public int MinEventNameLength = 10;
        public int MaxEventNameLength = 100;

        public int MinDescriptionLength = 5;
        public int MaxDescriptionLength = 500;

        public int MaxWebLinkLength = 200;
        public int MaxApprovalMessageLength = 200;

        public int MinLocationTextLength = 4;
        public int MaxLocationTextLength = 200;

        public EventCalendarEditorGump(PlayerMobile player): base(10, 10)
        {
            if (player == null) return;
            if (player.Deleted) return;

            EventCalendarPersistance.CheckAndCreateEventCalendarAccount(player);

            if (player.m_EventCalendarAccount == null)
                return;
            
            bool storedValuesValid = true;

            if (player.m_EventCalendarAccount.StoredValuesEvent == null)
                storedValuesValid = false;

            else if (player.m_EventCalendarAccount.StoredValuesEvent.Deleted)
                storedValuesValid = false;

            if (!storedValuesValid)
            {
                player.SendMessage(2115, "That event is no longer accessible.");

                player.CloseGump(typeof(EventCalendarGump));
                player.SendGump(new EventCalendarGump(player));

                return;
            }
            
            m_Player = player;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddImage(326, 1, 11049);
            AddImage(326, 169, 11049);
            AddImage(326, 330, 11049);
            AddImage(9, 1, 11049);
            AddImage(9, 169, 11049);
            AddImage(9, 329, 11049);
            AddImage(51, 8, 2524);
            AddImage(51, 111, 2524);
            AddImage(51, 213, 2524);
            AddImage(51, 324, 2524);
            AddImage(51, 433, 2524);
            AddImage(199, 9, 2524);
            AddImage(199, 112, 2524);
            AddImage(199, 214, 2524);
            AddImage(199, 325, 2524);
            AddImage(199, 434, 2524);
            AddImage(363, 9, 2524);
            AddImage(364, 110, 2524);
            AddImage(364, 212, 2524);
            AddImage(364, 323, 2524);
            AddImage(364, 434, 2524);
            AddImage(522, 9, 2524);
            AddImage(521, 109, 2524);
            AddImage(521, 212, 2524);
            AddImage(521, 323, 2524);
            AddImage(521, 434, 2524);
            AddImage(368, 239, 3005, 2415);
            AddImage(363, 240, 3003, 2415);
            AddImage(368, 10, 3005, 2415);
            AddImage(363, 11, 3003, 2415);
            AddImage(368, 301, 3005, 2415);
            AddImage(363, 302, 3003, 2415);
            AddImage(289, 4, 2440, 2425);
            
            int GreyTextHue = 2401;
            int WhiteTextHue = 2499; //2036; //2655; //2499
            int GreenTextHue = 0x3F;
            int YellowTextHue = 2610; // 2550;
            int RedTextHue = 2115;

            int OfficialEventHue = 1164;
            int GuildEventHue = 2599;
            int PlayerEventHue = 2625;

            int MajorEventHue = 2115;
            int MinorEventHue = 2548;
            int RecurringEventHue = 2562;            
            
            //---------------------------------

            AddButton(49, 8, 2095, 2094, 64, GumpButtonType.Reply, 0);
            AddLabel(75, 6, 149, "Wiki Page");

            AddLabel(331, 4, WhiteTextHue, "Event Editor");

            //Event Level
            AddLabel(168, 25, 2606, "Event Level");
            
            if (player.m_EventCalendarAccount.StoredValuesEvent.EventLevel == EventCalendarEvent.EventLevelType.Player)
            {
                AddLabel(78, 45, GreenTextHue, "All Players");
                AddButton(96, 65, 4009, 4008, 3, GumpButtonType.Reply, 0);
            }

            else
            {
                AddLabel(78, 45, WhiteTextHue, "All Players");
                AddButton(96, 65, 4008, 4009, 3, GumpButtonType.Reply, 0);
            }

            if (player.m_EventCalendarAccount.StoredValuesEvent.EventLevel == EventCalendarEvent.EventLevelType.Guild)
            {
                AddLabel(171, 45, GreenTextHue, "Guild-Only");
                AddButton(188, 65, 4009, 4008, 4, GumpButtonType.Reply, 0);
            }

            else
            {
                if (player.m_EventCalendarAccount.StoredValuesEvent.Creator != null)
                {
                    if (player.m_EventCalendarAccount.StoredValuesEvent.Creator.Guild != null)
                    {
                        AddLabel(171, 45, WhiteTextHue, "Guild-Only");
                        AddButton(188, 65, 4008, 4009, 4, GumpButtonType.Reply, 0);
                    }

                    else
                        AddLabel(160, 45, GreyTextHue, "Not Available");
                        AddImage(188, 65, 4017);
                }               
            }
            
            if (player.AccessLevel > AccessLevel.Player)
            {
                if (player.m_EventCalendarAccount.StoredValuesEvent.EventLevel == EventCalendarEvent.EventLevelType.Official)
                {
                    AddLabel(256, 45, GreenTextHue, "Staff-Made");
                    AddButton(274, 65, 4009, 4008, 5, GumpButtonType.Reply, 0);
                }

                else
                {
                    AddLabel(256, 45, WhiteTextHue, "Staff-Made");
                    AddButton(274, 65, 4008, 4009, 5, GumpButtonType.Reply, 0);
                }
            }

            //Event Type
            AddLabel(169, 93, 2603, "Event Type");

            if (player.m_EventCalendarAccount.StoredValuesEvent.EventType == EventCalendarEvent.EventTypeType.Minor)
            {
                AddLabel(91, 112, GreenTextHue, "Minor");
                AddButton(93, 132, 4012, 4011, 6, GumpButtonType.Reply, 0);
            }

            else
            {
                AddLabel(91, 112, WhiteTextHue, "Minor");
                AddButton(93, 132, 4011, 4012, 6, GumpButtonType.Reply, 0);
            }

            if (player.m_EventCalendarAccount.StoredValuesEvent.EventType == EventCalendarEvent.EventTypeType.Major)
            {
                AddLabel(183, 112, GreenTextHue, "Major");
                AddButton(186, 132, 4012, 4011, 7, GumpButtonType.Reply, 0);
            }

            else
            {
                AddLabel(183, 112, WhiteTextHue, "Major");
                AddButton(186, 132, 4011, 4012, 7, GumpButtonType.Reply, 0);
            }

            if (player.m_EventCalendarAccount.StoredValuesEvent.EventLevel != EventCalendarEvent.EventLevelType.Player)
            {
                if (player.m_EventCalendarAccount.StoredValuesEvent.EventType == EventCalendarEvent.EventTypeType.Recurring)
                {
                    AddLabel(259, 112, GreenTextHue, "Recurring");
                    AddButton(272, 132, 4012, 4011, 8, GumpButtonType.Reply, 0);
                }

                else
                {
                    AddLabel(259, 112, WhiteTextHue, "Recurring");
                    AddButton(272, 132, 4011, 4012, 8, GumpButtonType.Reply, 0);
                }
            }

            else
            {
                AddLabel(247, 112, GreyTextHue, "Not Available");
                AddImage(272, 132, 4017);
            }
            
            //Event Name
            AddLabel(164, 160, 2587, "Event Name");
            AddBackground(58, 180, 300, 45, 3000);            

            if (player.m_EventCalendarAccount.StoredValuesEvent.DisplayName.Length <= MaxEventNameLength)
                AddTextEntry(63, 181, 295, 40, WhiteTextHue, 9, player.m_EventCalendarAccount.StoredValuesEvent.DisplayName);
            else
                AddTextEntry(63, 181, 295, 40, RedTextHue, 9, player.m_EventCalendarAccount.StoredValuesEvent.DisplayName);
            
            //Event Description
            AddLabel(144, 230, 2596, "Event Description");
            AddBackground(58, 251, 300, 100, 3000);
            
            if (player.m_EventCalendarAccount.StoredValuesEvent.DescriptionText.Length <= MaxDescriptionLength)
                AddTextEntry(63, 256, 295, 90, WhiteTextHue, 10, player.m_EventCalendarAccount.StoredValuesEvent.DescriptionText);
            else
                AddTextEntry(63, 256, 295, 90, RedTextHue, 10, player.m_EventCalendarAccount.StoredValuesEvent.DescriptionText);
            
            //Event Website
            AddLabel(138, 357, 2425, "Event Website Link");
            AddBackground(58, 376, 300, 45, 3000);

            if (player.m_EventCalendarAccount.StoredValuesEvent.WebLink.Length <= MaxWebLinkLength)
                AddTextEntry(63, 378, 295, 40, WhiteTextHue, 11, player.m_EventCalendarAccount.StoredValuesEvent.WebLink);
            else
                AddTextEntry(63, 378, 295, 40, RedTextHue, 11, player.m_EventCalendarAccount.StoredValuesEvent.WebLink);
            
            //Approval
            if (player.m_EventCalendarAccount.StoredValuesEvent.EventLevel != EventCalendarEvent.EventLevelType.Player)
            {
                AddLabel(94, 431, 149, "Event Published");

                if (player.m_EventCalendarAccount.StoredValuesEvent.EventSubmitted)
                    AddButton(56, 427, 9724, 9721, 12, GumpButtonType.Reply, 0);
                else
                    AddButton(56, 427, 9721, 9724, 12, GumpButtonType.Reply, 0);
            }
            
            else
            {               
                AddLabel(255, 422, 149, "Event Status");

                switch (player.m_EventCalendarAccount.StoredValuesEvent.ApprovalStatus)
                {
                    case EventCalendarEvent.ApprovalStatusType.Approved:
                        AddLabel(Utility.CenteredTextOffset(302, "Approved!"), 440, GreenTextHue, "Approved!");
                    break;

                    case EventCalendarEvent.ApprovalStatusType.Pending:
                        AddLabel(94, 431, 149, "Submit for Approval");
                        AddLabel(Utility.CenteredTextOffset(302,"Pending Approval") , 440, YellowTextHue, "Pending Approval");

                        if (player.m_EventCalendarAccount.StoredValuesEvent.EventSubmitted)
                            AddButton(56, 427, 9724, 9721, 12, GumpButtonType.Reply, 0);
                        else
                            AddButton(56, 427, 9721, 9724, 12, GumpButtonType.Reply, 0);
                    break;

                    case EventCalendarEvent.ApprovalStatusType.Denied:
                        AddLabel(94, 431, 149, "Submit for Approval");
                        AddLabel(Utility.CenteredTextOffset(302, "Rejected"), 440, RedTextHue, "Rejected");

                        if (player.m_EventCalendarAccount.StoredValuesEvent.EventSubmitted)
                            AddButton(56, 427, 9724, 9721, 12, GumpButtonType.Reply, 0);
                        else
                            AddButton(56, 427, 9721, 9724, 12, GumpButtonType.Reply, 0);
                    break;
                }
            }
                                   
            if (player.m_EventCalendarAccount.StoredValuesEvent.EventLevel == EventCalendarEvent.EventLevelType.Player)
            {
                AddBackground(59, 459, 300, 45, 3000);

                if (player.AccessLevel > AccessLevel.Player)
                {
                    if (player.m_EventCalendarAccount.StoredValuesEvent.ApprovalText.Length <= MaxApprovalMessageLength)
                        AddTextEntry(63, 463, 300, 40, WhiteTextHue, 13, player.m_EventCalendarAccount.StoredValuesEvent.ApprovalText);
                    else
                        AddTextEntry(63, 463, 300, 40, RedTextHue, 13, player.m_EventCalendarAccount.StoredValuesEvent.ApprovalText);                    
                }

                else
                    AddHtml(58, 463, 300, 40, player.m_EventCalendarAccount.StoredValuesEvent.ApprovalText, false, false);                
            }
            
            if (player.AccessLevel > AccessLevel.Player)
            {
                if (player.m_EventCalendarAccount.StoredValuesEvent.EventLevel == EventCalendarEvent.EventLevelType.Player)
                {
                    if (player.m_EventCalendarAccount.StoredValuesEvent.ApprovalStatus == EventCalendarEvent.ApprovalStatusType.Approved)
                    {
                        AddLabel(85, 501, GreenTextHue, "Approved");
                        AddButton(97, 521, 4003, 4002, 14, GumpButtonType.Reply, 0);
                    }

                    else
                    {
                        AddLabel(85, 501, WhiteTextHue, "Approved");
                        AddButton(97, 521, 4002, 4003, 14, GumpButtonType.Reply, 0);
                    }

                    if (player.m_EventCalendarAccount.StoredValuesEvent.ApprovalStatus == EventCalendarEvent.ApprovalStatusType.Pending)
                    {
                        AddLabel(180, 501, GreenTextHue, "Pending");
                        AddButton(186, 521, 4003, 4002, 15, GumpButtonType.Reply, 0);
                    }

                    else
                    {
                        AddLabel(180, 501, WhiteTextHue, "Pending");
                        AddButton(186, 521, 4002, 4003, 15, GumpButtonType.Reply, 0);
                    }

                    if (player.m_EventCalendarAccount.StoredValuesEvent.ApprovalStatus == EventCalendarEvent.ApprovalStatusType.Denied)
                    {
                        AddLabel(272, 501, GreenTextHue, "Denied");
                        AddButton(276, 521, 4003, 4002, 16, GumpButtonType.Reply, 0);
                    }

                    else
                    {
                        AddLabel(272, 501, WhiteTextHue, "Denied");
                        AddButton(276, 521, 4002, 4003, 16, GumpButtonType.Reply, 0);
                    }
                }
            }
            
            //Event Start Time
            AddLabel(473, 26, 2560, "Event Date and Time");

            string dateTimeText = EventCalendarPersistance.PlayerFormattedDateTime(player, player.m_EventCalendarAccount.StoredValuesEvent.StartTime);

            if (player.m_EventCalendarAccount.StoredValuesEvent.StartTime > DateTime.UtcNow)
                AddLabel(Utility.CenteredTextOffset(530, dateTimeText), 44, WhiteTextHue, dateTimeText);
            else
                AddLabel(Utility.CenteredTextOffset(530, dateTimeText), 44, RedTextHue, dateTimeText);

            AddBackground(411, 66, 244, 34, 3000);

            AddButton(417, 73, 253, 253, 20, GumpButtonType.Reply, 0);
            AddLabel(436, 73, 2550, "Day");
            AddButton(465, 72, 251, 251, 21, GumpButtonType.Reply, 0);

            AddButton(490, 72, 253, 253, 22, GumpButtonType.Reply, 0);
            AddLabel(509, 72, 2550, "Hour");
            AddButton(543, 72, 251, 251, 23, GumpButtonType.Reply, 0);

            AddButton(567, 72, 253, 253, 24, GumpButtonType.Reply, 0);
            AddLabel(586, 72, 2550, "Minute");
            AddButton(634, 72, 251, 251, 25, GumpButtonType.Reply, 0);
            
            //Event Duration
            AddLabel(482, 106, 2573, "Event Duration");

            string durationText = EventCalendarPersistance.FormattedDuration(player.m_EventCalendarAccount.StoredValuesEvent.Duration);

            if (player.m_EventCalendarAccount.StoredValuesEvent.Duration >= MinimumDuration)
                AddLabel(Utility.CenteredTextOffset(530, durationText) , 124, WhiteTextHue, durationText);
            else
                AddLabel(Utility.CenteredTextOffset(530, durationText), 124, RedTextHue, durationText);

            AddBackground(411, 147, 244, 34, 3000);

            AddButton(417, 154, 253, 253, 30, GumpButtonType.Reply, 0);
            AddLabel(436, 154, 2550, "Day");
            AddButton(465, 153, 251, 251, 31, GumpButtonType.Reply, 0);

            AddButton(490, 153, 253, 253, 32, GumpButtonType.Reply, 0);
            AddLabel(509, 153, 2550, "Hour");
            AddButton(543, 153, 251, 251, 33, GumpButtonType.Reply, 0);

            AddButton(567, 153, 253, 253, 34, GumpButtonType.Reply, 0);
            AddLabel(586, 153, 2550, "Minute");
            AddButton(634, 153, 251, 251, 35, GumpButtonType.Reply, 0);
            
            //Recurring Interval
            if (player.m_EventCalendarAccount.StoredValuesEvent.EventType == EventCalendarEvent.EventTypeType.Recurring)
            {
                string intervalFrequencyText = EventCalendarPersistance.FormattedDuration(player.m_EventCalendarAccount.StoredValuesEvent.FrequencyInterval);

                AddLabel(458, 186, 2577, "Recurring Event Interval");
               
                if (player.m_EventCalendarAccount.StoredValuesEvent.FrequencyInterval >= MinimumFrequencyInterval)
                    AddLabel(Utility.CenteredTextOffset(530, intervalFrequencyText), 204, WhiteTextHue, intervalFrequencyText);
                else
                    AddLabel(Utility.CenteredTextOffset(530, intervalFrequencyText), 204, RedTextHue, intervalFrequencyText);
                
                AddBackground(412, 227, 244, 34, 3000);

                AddButton(418, 234, 253, 253, 40, GumpButtonType.Reply, 0);
                AddLabel(437, 234, 2550, "Day");
                AddButton(466, 233, 251, 251, 41, GumpButtonType.Reply, 0);

                AddButton(491, 233, 253, 253, 42, GumpButtonType.Reply, 0);
                AddLabel(510, 233, 2550, "Hour");
                AddButton(544, 233, 251, 251, 43, GumpButtonType.Reply, 0);

                AddButton(568, 233, 253, 253, 44, GumpButtonType.Reply, 0);
                AddLabel(587, 233, 2550, "Minute");
                AddButton(635, 233, 251, 251, 45, GumpButtonType.Reply, 0);
            }
            
            //Event Icon
            EventIconDetail iconDetail = EventCalendarPersistance.GetIconDetailDetail(player.m_EventCalendarAccount.StoredValuesEvent.EventIcon);

            AddItem(414 + iconDetail.OffsetX, 276 + iconDetail.OffsetY, iconDetail.ItemId, player.m_EventCalendarAccount.StoredValuesEvent.EventIconHue);

            AddLabel(499, 269, 2603, "Event Icon");
            AddButton(464, 273, 2223, 2223, 50, GumpButtonType.Reply, 0);
            AddButton(576, 273, 2224, 2224, 51, GumpButtonType.Reply, 0);

            AddLabel(506, 290, 2630, "Icon Hue");
            AddButton(476, 294, 2223, 2223, 52, GumpButtonType.Reply, 0);
            AddButton(564, 294, 2224, 2224, 53, GumpButtonType.Reply, 0);

            //Event Location
            AddLabel(482, 316, 2425, "Event Location");
            AddLabel(531, 330, 2515, "");
            AddBackground(382, 337, 300, 45, 3000);

            if (player.m_EventCalendarAccount.StoredValuesEvent.LocationText.Length <= MaxLocationTextLength)
                AddTextEntry(386, 339, 295, 40, WhiteTextHue, 60, player.m_EventCalendarAccount.StoredValuesEvent.LocationText);
            else
                AddTextEntry(386, 339, 295, 40, RedTextHue, 60, player.m_EventCalendarAccount.StoredValuesEvent.LocationText);

            //Event Coordinates
            AddLabel(468, 381, 149, "Event Coordinates");
            AddLabel(490, 401, WhiteTextHue, player.m_EventCalendarAccount.StoredValuesEvent.LocationPoint.X + ", " + player.m_EventCalendarAccount.StoredValuesEvent.LocationPoint.Y);

            AddButton(386, 424, 2118, 2117, 61, GumpButtonType.Reply, 0);
            AddLabel(405, 421, 2610, "Set Coords to My Location");

            AddButton(586, 423, 2118, 2117, 62, GumpButtonType.Reply, 0);
            AddLabel(606, 420, 2610, "View Map");
            
            //Event Creation Details
            AddLabel(379, 445, 149, "Event Creator");
            AddLabel(477, 445, WhiteTextHue, player.m_EventCalendarAccount.StoredValuesEvent.CreatorName);

            if (player.m_EventCalendarAccount.StoredValuesEvent.EventLevel == EventCalendarEvent.EventLevelType.Guild)
            {
                AddLabel(613, 446, 149, "Guild");

                if (player.m_EventCalendarAccount.StoredValuesEvent.Guild != null)
                    AddLabel(652, 446, WhiteTextHue, player.m_EventCalendarAccount.StoredValuesEvent.Guild.m_Abbreviation);
                else
                    AddLabel(652, 446, WhiteTextHue, "-");
            }

            AddLabel(379, 465, 149, "Creation Date");
            AddLabel(477, 465, WhiteTextHue, EventCalendarPersistance.PlayerFormattedDateTime(player, player.m_EventCalendarAccount.StoredValuesEvent.CreationDate));

            AddLabel(379, 485, 149, "Last Updated");
            AddLabel(477, 485, WhiteTextHue, EventCalendarPersistance.PlayerFormattedDateTime(player, player.m_EventCalendarAccount.StoredValuesEvent.LastUpdated));

            AddButton(377, 512, 2152, 2151, 1, GumpButtonType.Reply, 0);
            AddLabel(413, 516, GreenTextHue, "Save Changes and Exit");

            AddButton(568, 512, 2473, 2472, 2, GumpButtonType.Reply, 0);
            AddLabel(600, 516, 2115, "Delete Event");
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (sender == null)
                return;

            PlayerMobile player = sender.Mobile as PlayerMobile;

            if (player == null) return;
            if (player.Deleted) return;
                        
            EventCalendarPersistance.CheckAndCreateEventCalendarAccount(player);
                        
            bool closeGump = true;

            bool storedValuesValid = true;

            if (player.m_EventCalendarAccount.StoredValuesEvent == null)
                storedValuesValid = false;

            else if (player.m_EventCalendarAccount.StoredValuesEvent.Deleted)
                storedValuesValid = false;
            
            if (!storedValuesValid)
            {
                player.SendMessage(2115, "That event is no longer accessible.");

                player.CloseGump(typeof(EventCalendarGump));
                player.SendGump(new EventCalendarGump(player));

                return;
            }
            
            if (!HasAccessToEdit(player, player.m_EventCalendarAccount.StoredValuesEvent))
            {
                player.SendMessage(2115, "You no longer have the access privileges needed to edit this event.");

                player.CloseGump(typeof(EventCalendarGump));
                player.SendGump(new EventCalendarGump(player));
                
                return;              
            }
                        
            string EventNameText = "";
            string EventDescriptionText = "";
            string WebLinkText = "";
            string ApprovalMessageText = "";
            string EventLocationText = "";           

            //Validate Event Name
            TextRelay textRelayMessage = info.GetTextEntry(9);
            string textMessage = textRelayMessage.Text.Trim();
           
            if (textMessage == null)            
                player.m_EventCalendarAccount.StoredValuesEvent.DisplayName = "Enter event name here";            

            else
                player.m_EventCalendarAccount.StoredValuesEvent.DisplayName = textMessage;
                        
            //Validate Event Description
            textRelayMessage = info.GetTextEntry(10);
            textMessage = textRelayMessage.Text.Trim();

            if (textMessage == null)
                player.m_EventCalendarAccount.StoredValuesEvent.DescriptionText = "Enter event description here";            

            else
                player.m_EventCalendarAccount.StoredValuesEvent.DescriptionText = textMessage;
                        
            //Validate Web Link
            textRelayMessage = info.GetTextEntry(11);
            textMessage = textRelayMessage.Text.Trim();

            if (textMessage == null)
                player.m_EventCalendarAccount.StoredValuesEvent.WebLink = "www.ancorp.com";            

            else
                player.m_EventCalendarAccount.StoredValuesEvent.WebLink = textMessage;
          
            
            //Validate Approval Message
            if (player.m_EventCalendarAccount.StoredValuesEvent.EventLevel == EventCalendarEvent.EventLevelType.Player)
            {
                if (player.AccessLevel > AccessLevel.Player)
                {
                    textRelayMessage = info.GetTextEntry(13);
                    textMessage = textRelayMessage.Text.Trim();

                    if (textMessage == null)
                        player.m_EventCalendarAccount.StoredValuesEvent.ApprovalText = "Awaiting staff approval";

                    else
                        player.m_EventCalendarAccount.StoredValuesEvent.ApprovalText = textMessage;
                }
            }
                        
            //Validate Event Location
            textRelayMessage = info.GetTextEntry(60);
            textMessage = textRelayMessage.Text.Trim();

            if (textMessage == null)
                player.m_EventCalendarAccount.StoredValuesEvent.LocationText = "Enter event location here";            

            else
                player.m_EventCalendarAccount.StoredValuesEvent.LocationText = textMessage;
            
            //Buttons
            switch (info.ButtonID)
            {
                case 1:
                    //Confirm Changes
                    bool allFieldsValid = true;

                    if (player.AccessLevel == AccessLevel.Player)
                    {
                        //Text Field Validation
                        if (player.m_EventCalendarAccount.StoredValuesEvent.DisplayName.Length < MinEventNameLength)
                        {
                            player.SendMessage(2115, "Event Name must be more than " + MinEventNameLength.ToString() + " characters.");
                            allFieldsValid = false;
                        }

                        if (player.m_EventCalendarAccount.StoredValuesEvent.DisplayName.Length > MaxEventNameLength)
                        {
                            player.SendMessage(2115, "Event Name must be less than " + MaxEventNameLength.ToString() + " characters.");
                            allFieldsValid = false;
                        }

                        if (player.m_EventCalendarAccount.StoredValuesEvent.DescriptionText.Length < MinDescriptionLength)
                        {
                            player.SendMessage(2115, "Event Description must be more than " + MinDescriptionLength.ToString() + " characters.");
                            allFieldsValid = false;
                        }

                        if (player.m_EventCalendarAccount.StoredValuesEvent.DescriptionText.Length > MaxDescriptionLength)
                        {
                            player.SendMessage(2115, "Event Description must be less than " + MaxDescriptionLength.ToString() + " characters.");
                            allFieldsValid = false;
                        }

                        if (player.m_EventCalendarAccount.StoredValuesEvent.WebLink.Length > MaxWebLinkLength)
                        {
                            player.SendMessage(2115, "Event Weblink must be less than " + MaxWebLinkLength.ToString() + " characters.");
                            allFieldsValid = false;
                        }

                        if (player.m_EventCalendarAccount.StoredValuesEvent.ApprovalText.Length > MaxApprovalMessageLength)
                        {
                            player.SendMessage(2115, "Event Approval Message must be less than " + MaxApprovalMessageLength.ToString() + " characters.");
                            allFieldsValid = false;
                        }

                        if (player.m_EventCalendarAccount.StoredValuesEvent.LocationText.Length < MinLocationTextLength)
                        {
                            player.SendMessage(2115, "Event Location must be more than " + MinLocationTextLength.ToString() + " characters.");
                            allFieldsValid = false;
                        }

                        if (player.m_EventCalendarAccount.StoredValuesEvent.LocationText.Length > MaxLocationTextLength)
                        {
                            player.SendMessage(2115, "Event Location must be less than " + MaxLocationTextLength.ToString() + " characters.");
                            allFieldsValid = false;
                        }

                        //Property Validation
                        if (player.m_EventCalendarAccount.StoredValuesEvent.StartTime < DateTime.UtcNow)
                        {
                            player.SendMessage(2115, "Event Start must be in the future.");
                            allFieldsValid = false;
                        }

                        if (player.m_EventCalendarAccount.StoredValuesEvent.Duration < MinimumDuration)
                        {
                            player.SendMessage(2115, "Event Duration must be at least 15 minutes.");
                            allFieldsValid = false;
                        }

                        if (player.m_EventCalendarAccount.StoredValuesEvent.EventType == EventCalendarEvent.EventTypeType.Recurring)
                        {
                            if (player.m_EventCalendarAccount.StoredValuesEvent.FrequencyInterval < MinimumFrequencyInterval)
                            {
                                player.SendMessage(2115, "Recurring events may occur at most once every 4 hours.");
                                allFieldsValid = false;
                            }
                        }                          
                    }

                    if (allFieldsValid)
                    {
                        if (player.m_EventCalendarAccount.EditingEvent == null)
                        {
                            EventCalendarEvent newEvent = new EventCalendarEvent();

                            player.m_EventCalendarAccount.StoredValuesEvent.Creator = player;
                            player.m_EventCalendarAccount.StoredValuesEvent.CreatorName = player.Name;

                            if (player.m_EventCalendarAccount.StoredValuesEvent.EventLevel == EventCalendarEvent.EventLevelType.Guild)
                                player.m_EventCalendarAccount.StoredValuesEvent.Guild = player.Guild;

                            player.m_EventCalendarAccount.StoredValuesEvent.CreationDate = DateTime.UtcNow;
                            player.m_EventCalendarAccount.StoredValuesEvent.LastUpdated = DateTime.UtcNow;

                            player.m_EventCalendarAccount.StoredValuesEvent.PushEventValues(newEvent);

                            EventCalendarPersistance.m_EventCalendarEvents.Add(newEvent);

                            if (player.m_EventCalendarAccount.StoredValuesEvent.EventLevel == EventCalendarEvent.EventLevelType.Player)
                            {
                                if (player.m_EventCalendarAccount.StoredValuesEvent.EventSubmitted)
                                    player.SendMessage(0x3F, "You create a new Player event. Staff will now determine approval for this event.");

                                else
                                    player.SendMessage(149, "You create a new Player event, however the 'Submit for Approval' button must be checked on this event for staff to review and approve it.");
                            }

                            else
                                player.SendMessage(0x3F, "You create a new event.");

                            player.m_EventCalendarAccount.StoredValuesEvent.Delete();
                        }

                        else
                        {
                            bool EventValuesMatch = EventCalendarPersistance.EventValuesMatch(player.m_EventCalendarAccount.EditingEvent, player.m_EventCalendarAccount.StoredValuesEvent);

                            //Player Event
                            if (!EventValuesMatch && player.m_EventCalendarAccount.EditingEvent.EventLevel == EventCalendarEvent.EventLevelType.Player)
                            {
                                //If Values Have Changed and Was Already Approved
                                if (player.m_EventCalendarAccount.EditingEvent.ApprovalStatus == EventCalendarEvent.ApprovalStatusType.Approved)
                                {
                                    player.m_EventCalendarAccount.StoredValuesEvent.ApprovalStatus = EventCalendarEvent.ApprovalStatusType.Pending;
                                    player.m_EventCalendarAccount.StoredValuesEvent.ApprovalText = "Pending Approval";

                                    player.SendMessage(149, "Changes have been made to this event since it was last approved by staff, and it must now await reapproval.");
                                }

                                else
                                {
                                    if (player.m_EventCalendarAccount.StoredValuesEvent.EventSubmitted)
                                        player.SendMessage(0x3F, "You update the details of the event. Staff will now determine approval for this event.");

                                    else
                                        player.SendMessage(149, "You update the details of the event, however the 'Submit for Approval' button must be checked on this event for staff to review and approve it.");
                                }
                            }

                            else
                                player.SendMessage(0x3F, "You update the details of the event.");

                            player.m_EventCalendarAccount.StoredValuesEvent.LastUpdated = DateTime.UtcNow;

                            player.m_EventCalendarAccount.StoredValuesEvent.PushEventValues(player.m_EventCalendarAccount.EditingEvent);
                            player.m_EventCalendarAccount.StoredValuesEvent.Delete();

                            EventCalendarPersistance.EventChanged(player, player.m_EventCalendarAccount.EditingEvent);
                        }

                        player.m_EventCalendarAccount.StoredValuesEvent = null;
                        player.m_EventCalendarAccount.EditingEvent = null;

                        player.CloseGump(typeof(EventCalendarGump));
                        player.SendGump(new EventCalendarGump(player));

                        player.SendSound(ClosePageSound);

                        return;
                    }

                    else                    
                        closeGump = false;
                                                            
                break;

                case 2:
                    //Delete Event  
                    if (player.m_EventCalendarAccount.EditingEvent != null)
                    {
                        player.m_EventCalendarAccount.StoredValuesEvent.Delete();

                        if (EventCalendarPersistance.m_EventCalendarEvents.Contains(player.m_EventCalendarAccount.EditingEvent))
                            EventCalendarPersistance.m_EventCalendarEvents.Remove(player.m_EventCalendarAccount.EditingEvent);

                        EventCalendarPersistance.EventDeleted(player, player.m_EventCalendarAccount.EditingEvent);

                        player.m_EventCalendarAccount.EditingEvent.Delete();

                        player.m_EventCalendarAccount.StoredValuesEvent = null;
                        player.m_EventCalendarAccount.EditingEvent = null;

                        player.SendMessage(0x3F, "You delete the event.");

                        player.CloseGump(typeof(EventCalendarGump));
                        player.SendGump(new EventCalendarGump(player));

                        player.SendSound(ClosePageSound);

                        return;
                    }

                    else
                    {
                        player.SendMessage(0x3F, "The event you were editing is no longer accessible.");

                        player.CloseGump(typeof(EventCalendarGump));
                        player.SendGump(new EventCalendarGump(player));

                        return;
                    }
                break;

                #region Event Level

                 case 3:
                    //EventAccess: Player 
                    if (player.m_EventCalendarAccount.StoredValuesEvent.EventLevel != EventCalendarEvent.EventLevelType.Player)
                    {
                        player.m_EventCalendarAccount.StoredValuesEvent.EventLevel = EventCalendarEvent.EventLevelType.Player;
                        player.m_EventCalendarAccount.StoredValuesEvent.Guild = null;

                        player.m_EventCalendarAccount.StoredValuesEvent.EventType = EventCalendarEvent.EventTypeType.Minor;

                        EventCalendarPersistance.CheckForChangeClearingSubmittedStatus(player, player.m_EventCalendarAccount.StoredValuesEvent);

                        player.SendSound(BigClickSound);
                    }

                    closeGump = false;
                break;

                case 4:
                    //EventAccess: Guild
                    if (player.m_EventCalendarAccount.StoredValuesEvent.EventLevel != EventCalendarEvent.EventLevelType.Guild)
                    {
                        if (player.m_EventCalendarAccount.StoredValuesEvent.Creator != null)
                        {
                            if (player.m_EventCalendarAccount.StoredValuesEvent.Creator.Guild != null)
                            {
                                player.m_EventCalendarAccount.StoredValuesEvent.EventLevel = EventCalendarEvent.EventLevelType.Guild;
                                player.m_EventCalendarAccount.StoredValuesEvent.Guild = player.m_EventCalendarAccount.StoredValuesEvent.Creator.Guild;

                                player.SendSound(BigClickSound);
                            }
                        }
                    }

                    closeGump = false;
                break;
                
                case 5:
                    //EventAccess: Official
                    if (player.AccessLevel > AccessLevel.Player)
                    {
                        if (player.m_EventCalendarAccount.StoredValuesEvent.EventLevel != EventCalendarEvent.EventLevelType.Official)
                        {
                            player.m_EventCalendarAccount.StoredValuesEvent.EventLevel = EventCalendarEvent.EventLevelType.Official;
                            player.m_EventCalendarAccount.StoredValuesEvent.Guild = null;

                            player.SendSound(BigClickSound);
                        }
                    }

                    closeGump = false;
                break;

                #endregion

                #region Event Type
                
                case 6:
                    //EventLevel: Minor  
                    if (player.m_EventCalendarAccount.StoredValuesEvent.EventType != EventCalendarEvent.EventTypeType.Minor)
                    {
                        player.m_EventCalendarAccount.StoredValuesEvent.EventType = EventCalendarEvent.EventTypeType.Minor;                        

                        EventCalendarPersistance.CheckForChangeClearingSubmittedStatus(player, player.m_EventCalendarAccount.StoredValuesEvent);

                        player.SendSound(BigClickSound);
                    }

                    closeGump = false;
                break;
               
                case 7:
                    //EventLevel: Major
                    if (player.m_EventCalendarAccount.StoredValuesEvent.EventType != EventCalendarEvent.EventTypeType.Major)
                    {
                        player.m_EventCalendarAccount.StoredValuesEvent.EventType = EventCalendarEvent.EventTypeType.Major;                       

                        EventCalendarPersistance.CheckForChangeClearingSubmittedStatus(player, player.m_EventCalendarAccount.StoredValuesEvent);

                        player.SendSound(BigClickSound);                       
                    }

                    closeGump = false;
                break;
                
                case 8:
                    //EventLevel: Recurring
                    if (player.m_EventCalendarAccount.StoredValuesEvent.EventType != EventCalendarEvent.EventTypeType.Recurring)
                    {
                        if (player.m_EventCalendarAccount.StoredValuesEvent.EventLevel != EventCalendarEvent.EventLevelType.Player)
                        {
                            player.m_EventCalendarAccount.StoredValuesEvent.EventType = EventCalendarEvent.EventTypeType.Recurring;                           

                            player.SendSound(BigClickSound);                           
                        }
                    }

                    closeGump = false;
                break;

                #endregion                                

                #region ApprovalStatus

                case 12:
                    player.m_EventCalendarAccount.StoredValuesEvent.EventSubmitted = !player.m_EventCalendarAccount.StoredValuesEvent.EventSubmitted;

                    player.SendSound(BigClickSound);

                    closeGump = false;
                break;

                case 14:
                    //ApprovalStatus: Approved
                    if (player.AccessLevel > AccessLevel.Player)
                    {
                        if (player.m_EventCalendarAccount.StoredValuesEvent.ApprovalStatus != EventCalendarEvent.ApprovalStatusType.Approved)
                        {
                            if (player.m_EventCalendarAccount.StoredValuesEvent.EventLevel == EventCalendarEvent.EventLevelType.Player)
                            {
                                player.m_EventCalendarAccount.StoredValuesEvent.ApprovalStatus = EventCalendarEvent.ApprovalStatusType.Approved;

                                player.SendSound(BigClickSound);
                            }
                        }
                    }
                    
                    closeGump = false;
                break;

                case 15:
                    //ApprovalStatus: Pending
                    if (player.AccessLevel > AccessLevel.Player)
                    {
                        if (player.m_EventCalendarAccount.StoredValuesEvent.ApprovalStatus != EventCalendarEvent.ApprovalStatusType.Pending)
                        {
                            if (player.m_EventCalendarAccount.StoredValuesEvent.EventLevel == EventCalendarEvent.EventLevelType.Player)
                            {
                                player.m_EventCalendarAccount.StoredValuesEvent.ApprovalStatus = EventCalendarEvent.ApprovalStatusType.Pending;

                                player.SendSound(BigClickSound);       
                            }
                        }
                    }
                    
                    closeGump = false;
                break;

                case 16:
                    //ApprovalStatus: Denied
                    if (player.AccessLevel > AccessLevel.Player)
                    {
                        if (player.m_EventCalendarAccount.StoredValuesEvent.ApprovalStatus != EventCalendarEvent.ApprovalStatusType.Denied)
                        {
                            if (player.m_EventCalendarAccount.StoredValuesEvent.EventLevel == EventCalendarEvent.EventLevelType.Player)
                            {
                                player.m_EventCalendarAccount.StoredValuesEvent.ApprovalStatus = EventCalendarEvent.ApprovalStatusType.Denied;

                                player.SendSound(BigClickSound);       
                            }
                        }
                    }
                    
                    closeGump = false;
                break;

                #endregion

                #region StartTime

                case 20:
                    //StartTime: Days--
                    player.m_EventCalendarAccount.StoredValuesEvent.StartTime = player.m_EventCalendarAccount.StoredValuesEvent.StartTime.Subtract(TimeSpan.FromDays(1));
                    
                    EventCalendarPersistance.CheckForChangeClearingSubmittedStatus(player, player.m_EventCalendarAccount.StoredValuesEvent);

                    player.SendSound(SmallClickSound);

                    closeGump = false;
                break;

                case 21:
                    //StartTime: Days++
                    player.m_EventCalendarAccount.StoredValuesEvent.StartTime = player.m_EventCalendarAccount.StoredValuesEvent.StartTime.AddDays(1);
                    
                    EventCalendarPersistance.CheckForChangeClearingSubmittedStatus(player, player.m_EventCalendarAccount.StoredValuesEvent);

                    player.SendSound(SmallClickSound);

                    closeGump = false;
                break;

                case 22:
                    //StartTime: Hours--
                    player.m_EventCalendarAccount.StoredValuesEvent.StartTime = player.m_EventCalendarAccount.StoredValuesEvent.StartTime.Subtract(TimeSpan.FromHours(1));
                    
                    EventCalendarPersistance.CheckForChangeClearingSubmittedStatus(player, player.m_EventCalendarAccount.StoredValuesEvent);

                    player.SendSound(SmallClickSound);

                    closeGump = false;                    
                break;

                case 23:
                    //StartTime: Hours++
                    player.m_EventCalendarAccount.StoredValuesEvent.StartTime = player.m_EventCalendarAccount.StoredValuesEvent.StartTime.AddHours(1);
                    
                    EventCalendarPersistance.CheckForChangeClearingSubmittedStatus(player, player.m_EventCalendarAccount.StoredValuesEvent);

                    player.SendSound(SmallClickSound);

                    closeGump = false;
                break;

                case 24:
                    //StartTime: Minutes--
                    player.m_EventCalendarAccount.StoredValuesEvent.StartTime = player.m_EventCalendarAccount.StoredValuesEvent.StartTime.Subtract(TimeSpan.FromMinutes(5));
                    
                    EventCalendarPersistance.CheckForChangeClearingSubmittedStatus(player, player.m_EventCalendarAccount.StoredValuesEvent);

                    player.SendSound(SmallClickSound);

                    closeGump = false;                    
                break;

                case 25:
                    //StartTime: Minutes++
                    player.m_EventCalendarAccount.StoredValuesEvent.StartTime = player.m_EventCalendarAccount.StoredValuesEvent.StartTime.AddMinutes(5);
                    
                    EventCalendarPersistance.CheckForChangeClearingSubmittedStatus(player, player.m_EventCalendarAccount.StoredValuesEvent);

                    player.SendSound(SmallClickSound);

                    closeGump = false;
                break;

                #endregion

                #region Duration

                case 30:
                    //Duration: Days--
                    player.m_EventCalendarAccount.StoredValuesEvent.Duration = player.m_EventCalendarAccount.StoredValuesEvent.Duration.Subtract(TimeSpan.FromDays(1));

                    if (player.m_EventCalendarAccount.StoredValuesEvent.Duration < MinimumDuration)
                        player.m_EventCalendarAccount.StoredValuesEvent.Duration = MinimumDuration;

                    EventCalendarPersistance.CheckForChangeClearingSubmittedStatus(player, player.m_EventCalendarAccount.StoredValuesEvent);                    

                    player.SendSound(SmallClickSound);

                    closeGump = false;                    
                break;

                case 31:
                    //Duration: Days++
                    player.m_EventCalendarAccount.StoredValuesEvent.Duration = player.m_EventCalendarAccount.StoredValuesEvent.Duration.Add(TimeSpan.FromDays(1));

                    if (player.m_EventCalendarAccount.StoredValuesEvent.Duration < MinimumDuration)
                        player.m_EventCalendarAccount.StoredValuesEvent.Duration = MinimumDuration;

                    EventCalendarPersistance.CheckForChangeClearingSubmittedStatus(player, player.m_EventCalendarAccount.StoredValuesEvent);

                    player.SendSound(SmallClickSound);


                    closeGump = false;                    
                break;

                case 32:
                    //Duration: Hours--
                    player.m_EventCalendarAccount.StoredValuesEvent.Duration = player.m_EventCalendarAccount.StoredValuesEvent.Duration.Subtract(TimeSpan.FromHours(1));

                    if (player.m_EventCalendarAccount.StoredValuesEvent.Duration < MinimumDuration)
                        player.m_EventCalendarAccount.StoredValuesEvent.Duration = MinimumDuration;

                    EventCalendarPersistance.CheckForChangeClearingSubmittedStatus(player, player.m_EventCalendarAccount.StoredValuesEvent);

                    player.SendSound(SmallClickSound);

                    closeGump = false;                    
                break;

                case 33:
                    //Duration: Hours++
                    player.m_EventCalendarAccount.StoredValuesEvent.Duration = player.m_EventCalendarAccount.StoredValuesEvent.Duration.Add(TimeSpan.FromHours(1));

                    if (player.m_EventCalendarAccount.StoredValuesEvent.Duration < MinimumDuration)
                        player.m_EventCalendarAccount.StoredValuesEvent.Duration = MinimumDuration;

                    if (player.m_EventCalendarAccount.StoredValuesEvent.Duration < MinimumDuration)
                        player.m_EventCalendarAccount.StoredValuesEvent.Duration = MinimumDuration;

                    EventCalendarPersistance.CheckForChangeClearingSubmittedStatus(player, player.m_EventCalendarAccount.StoredValuesEvent);

                    player.SendSound(SmallClickSound);


                    closeGump = false;
                break;

                case 34:
                    //Duration: Minutes--
                    player.m_EventCalendarAccount.StoredValuesEvent.Duration = player.m_EventCalendarAccount.StoredValuesEvent.Duration.Subtract(TimeSpan.FromMinutes(5));

                    if (player.m_EventCalendarAccount.StoredValuesEvent.Duration < MinimumDuration)
                        player.m_EventCalendarAccount.StoredValuesEvent.Duration = MinimumDuration;

                    EventCalendarPersistance.CheckForChangeClearingSubmittedStatus(player, player.m_EventCalendarAccount.StoredValuesEvent);

                    player.SendSound(SmallClickSound);

                    closeGump = false;                    
                break;

                case 35:
                    //Duration: Minutes++
                    player.m_EventCalendarAccount.StoredValuesEvent.Duration = player.m_EventCalendarAccount.StoredValuesEvent.Duration.Add(TimeSpan.FromMinutes(5));

                    if (player.m_EventCalendarAccount.StoredValuesEvent.Duration < MinimumDuration)
                        player.m_EventCalendarAccount.StoredValuesEvent.Duration = MinimumDuration;

                    EventCalendarPersistance.CheckForChangeClearingSubmittedStatus(player, player.m_EventCalendarAccount.StoredValuesEvent);

                    player.SendSound(SmallClickSound);

                    closeGump = false;                   
                break;

                #endregion

                #region FrequencyInterval

                case 40:
                    //FrequencyInterval: Days--
                    if (player.m_EventCalendarAccount.StoredValuesEvent.EventType == EventCalendarEvent.EventTypeType.Recurring)
                    {
                        player.m_EventCalendarAccount.StoredValuesEvent.FrequencyInterval = player.m_EventCalendarAccount.StoredValuesEvent.FrequencyInterval.Subtract(TimeSpan.FromDays(1));

                        if (player.m_EventCalendarAccount.StoredValuesEvent.FrequencyInterval < MinimumFrequencyInterval)
                            player.m_EventCalendarAccount.StoredValuesEvent.FrequencyInterval = MinimumFrequencyInterval;

                        EventCalendarPersistance.CheckForChangeClearingSubmittedStatus(player, player.m_EventCalendarAccount.StoredValuesEvent);

                        player.SendSound(SmallClickSound);
                    }

                    closeGump = false;
                break;

                case 41:
                    //FrequencyInterval: Days++
                    if (player.m_EventCalendarAccount.StoredValuesEvent.EventType == EventCalendarEvent.EventTypeType.Recurring)
                    {
                        player.m_EventCalendarAccount.StoredValuesEvent.FrequencyInterval = player.m_EventCalendarAccount.StoredValuesEvent.FrequencyInterval.Add(TimeSpan.FromDays(1));

                        if (player.m_EventCalendarAccount.StoredValuesEvent.FrequencyInterval < MinimumFrequencyInterval)
                            player.m_EventCalendarAccount.StoredValuesEvent.FrequencyInterval = MinimumFrequencyInterval;

                        EventCalendarPersistance.CheckForChangeClearingSubmittedStatus(player, player.m_EventCalendarAccount.StoredValuesEvent);

                        player.SendSound(SmallClickSound);

                    }

                    closeGump = false;
                break;

                case 42:
                    //FrequencyInterval: Hours--
                    if (player.m_EventCalendarAccount.StoredValuesEvent.EventType == EventCalendarEvent.EventTypeType.Recurring)
                    {
                        player.m_EventCalendarAccount.StoredValuesEvent.FrequencyInterval = player.m_EventCalendarAccount.StoredValuesEvent.FrequencyInterval.Subtract(TimeSpan.FromHours(1));

                        if (player.m_EventCalendarAccount.StoredValuesEvent.FrequencyInterval < MinimumFrequencyInterval)
                            player.m_EventCalendarAccount.StoredValuesEvent.FrequencyInterval = MinimumFrequencyInterval;

                        EventCalendarPersistance.CheckForChangeClearingSubmittedStatus(player, player.m_EventCalendarAccount.StoredValuesEvent);

                        player.SendSound(SmallClickSound);

                    }

                    closeGump = false;
                break;

                case 43:
                    //FrequencyInterval: Hours++
                    if (player.m_EventCalendarAccount.StoredValuesEvent.EventType == EventCalendarEvent.EventTypeType.Recurring)
                    {
                        player.m_EventCalendarAccount.StoredValuesEvent.FrequencyInterval = player.m_EventCalendarAccount.StoredValuesEvent.FrequencyInterval.Add(TimeSpan.FromHours(1));

                        if (player.m_EventCalendarAccount.StoredValuesEvent.FrequencyInterval < MinimumFrequencyInterval)
                            player.m_EventCalendarAccount.StoredValuesEvent.FrequencyInterval = MinimumFrequencyInterval;

                        EventCalendarPersistance.CheckForChangeClearingSubmittedStatus(player, player.m_EventCalendarAccount.StoredValuesEvent);

                        player.SendSound(SmallClickSound);

                    }

                    closeGump = false;
                break;

                case 44:
                    //FrequencyInterval: Minutes--
                    if (player.m_EventCalendarAccount.StoredValuesEvent.EventType == EventCalendarEvent.EventTypeType.Recurring)
                    {
                        player.m_EventCalendarAccount.StoredValuesEvent.FrequencyInterval = player.m_EventCalendarAccount.StoredValuesEvent.FrequencyInterval.Subtract(TimeSpan.FromMinutes(5));

                        if (player.m_EventCalendarAccount.StoredValuesEvent.FrequencyInterval < MinimumFrequencyInterval)
                            player.m_EventCalendarAccount.StoredValuesEvent.FrequencyInterval = MinimumFrequencyInterval;

                        EventCalendarPersistance.CheckForChangeClearingSubmittedStatus(player, player.m_EventCalendarAccount.StoredValuesEvent);

                        player.SendSound(SmallClickSound);
                    }

                    closeGump = false;                    
                break;

                case 45:
                    //FrequencyInterval: Minutes++
                    if (player.m_EventCalendarAccount.StoredValuesEvent.EventType == EventCalendarEvent.EventTypeType.Recurring)
                    {
                        player.m_EventCalendarAccount.StoredValuesEvent.FrequencyInterval = player.m_EventCalendarAccount.StoredValuesEvent.FrequencyInterval.Add(TimeSpan.FromMinutes(5));

                        if (player.m_EventCalendarAccount.StoredValuesEvent.FrequencyInterval < MinimumFrequencyInterval)
                            player.m_EventCalendarAccount.StoredValuesEvent.FrequencyInterval = MinimumFrequencyInterval;

                        EventCalendarPersistance.CheckForChangeClearingSubmittedStatus(player, player.m_EventCalendarAccount.StoredValuesEvent);

                        player.SendSound(SmallClickSound);

                    }

                    closeGump = false;
                break;

                #endregion

                #region Change Icon

                case 50:
                    //Previous Icon  
                    int iconIndex = 0;

                    for (int a = 0; a < EventCalendarPersistance.IconItemIDs.Length; a++)
                    {
                        if (EventCalendarPersistance.IconItemIDs[a] == player.m_EventCalendarAccount.StoredValuesEvent.EventIcon)
                        {
                            iconIndex = a;
                            break;
                        }
                    }

                    iconIndex--;

                    if (iconIndex < 0)
                        iconIndex = EventCalendarPersistance.IconItemIDs.Length - 1;

                    else if (iconIndex >= EventCalendarPersistance.IconItemIDs.Length)
                        iconIndex = 0;

                    player.m_EventCalendarAccount.StoredValuesEvent.EventIcon = EventCalendarPersistance.IconItemIDs[iconIndex];

                    EventCalendarPersistance.CheckForChangeClearingSubmittedStatus(player, player.m_EventCalendarAccount.StoredValuesEvent);

                    player.SendSound(SmallClickSound);

                    closeGump = false;
                break;

                case 51:
                    //Next Icon   
                    iconIndex = 0;

                    for (int a = 0; a < EventCalendarPersistance.IconItemIDs.Length; a++)
                    {
                        if (EventCalendarPersistance.IconItemIDs[a] == player.m_EventCalendarAccount.StoredValuesEvent.EventIcon)
                        {
                            iconIndex = a;
                            break;
                        }
                    }

                    iconIndex++;

                    if (iconIndex < 0)
                        iconIndex = EventCalendarPersistance.IconItemIDs.Length - 1;

                    else if (iconIndex >= EventCalendarPersistance.IconItemIDs.Length)
                        iconIndex = 0;

                    player.m_EventCalendarAccount.StoredValuesEvent.EventIcon = EventCalendarPersistance.IconItemIDs[iconIndex];

                    EventCalendarPersistance.CheckForChangeClearingSubmittedStatus(player, player.m_EventCalendarAccount.StoredValuesEvent);

                    player.SendSound(SmallClickSound);

                    closeGump = false;
                break;

                case 52:
                    //Previous Icon Hue  
                    int iconHueIndex = 0;

                    for (int a = 0; a < EventCalendarPersistance.IconHues.Length; a++)
                    {
                        if (EventCalendarPersistance.IconHues[a] == player.m_EventCalendarAccount.StoredValuesEvent.EventIconHue)
                        {
                            iconIndex = a;
                            break;
                        }
                    }

                    iconHueIndex--;

                    if (iconHueIndex < 0)
                        iconHueIndex = EventCalendarPersistance.IconHues.Length - 1;

                    else if (iconHueIndex >= EventCalendarPersistance.IconHues.Length)
                        iconHueIndex = 0;

                    player.m_EventCalendarAccount.StoredValuesEvent.EventIconHue = EventCalendarPersistance.IconHues[iconHueIndex];

                    EventCalendarPersistance.CheckForChangeClearingSubmittedStatus(player, player.m_EventCalendarAccount.StoredValuesEvent);

                    player.SendSound(SmallClickSound);

                    closeGump = false;
                break;

                case 53:
                    //Next Icon Hue  
                    iconHueIndex = 0;

                    for (int a = 0; a < EventCalendarPersistance.IconHues.Length; a++)
                    {
                        if (EventCalendarPersistance.IconHues[a] == player.m_EventCalendarAccount.StoredValuesEvent.EventIconHue)
                        {
                            iconIndex = a;
                            break;
                        }
                    }

                    iconHueIndex++;

                    if (iconHueIndex < 0)
                        iconHueIndex = EventCalendarPersistance.IconHues.Length - 1;

                    else if (iconHueIndex >= EventCalendarPersistance.IconHues.Length)
                        iconHueIndex = 0;

                    player.m_EventCalendarAccount.StoredValuesEvent.EventIconHue = EventCalendarPersistance.IconHues[iconHueIndex];

                    EventCalendarPersistance.CheckForChangeClearingSubmittedStatus(player, player.m_EventCalendarAccount.StoredValuesEvent);

                    player.SendSound(SmallClickSound);

                    closeGump = false;
                break;

                #endregion

                #region Location

                case 61:
                    //Set Coordinates to Location
                    if (player.Map != Map.Felucca)
                    {
                        player.SendMessage("That location may not be saved for events.");
                    }

                    else
                    {
                        player.m_EventCalendarAccount.StoredValuesEvent.LocationPoint = player.Location;
                        player.m_EventCalendarAccount.StoredValuesEvent.LocationMap = player.Map;

                        EventCalendarPersistance.CheckForChangeClearingSubmittedStatus(player, player.m_EventCalendarAccount.StoredValuesEvent);

                        player.SendSound(SmallClickSound);

                    }

                    closeGump = false;
                break;               

                case 62:
                    //View Map
                    player.SendSound(SmallClickSound);


                    closeGump = false;
                break;

                #endregion                

                case 63:
                    //Test Weblink   
                    player.SendSound(SmallClickSound);                    

                    closeGump = false;
                break;

                case 64:
                    //Launch Wiki       
                    player.SendSound(SmallClickSound);
                    

                    closeGump = false;
                break;                
            }
            
            if (!closeGump)
            {
                player.CloseGump(typeof(EventCalendarGump));
                player.SendGump(new EventCalendarGump(player));

                player.CloseGump(typeof(EventCalendarEditorGump));
                player.SendGump(new EventCalendarEditorGump(player));
            }

            else
            {
                if (player.m_EventCalendarAccount.StoredValuesEvent != null)
                    player.m_EventCalendarAccount.StoredValuesEvent.Delete();

                player.m_EventCalendarAccount.StoredValuesEvent = null;
                player.m_EventCalendarAccount.EditingEvent = null;

                player.SendSound(ClosePageSound);
            }
        }

        public bool HasAccessToEdit(PlayerMobile player, EventCalendarEvent eventCalendarEvent)
        {
            if (player == null)
                return false;

            bool validAccess = false;            

            switch (eventCalendarEvent.EventLevel)
            {
                case EventCalendarEvent.EventLevelType.Official:
                    if (player.AccessLevel > AccessLevel.Player)
                        return true;
                break;

                case EventCalendarEvent.EventLevelType.Player:
                    if (player.AccessLevel > AccessLevel.Player)
                        return true;

                    else if (player == eventCalendarEvent.Creator && eventCalendarEvent.Creator != null)
                            return true;
                break;

                case EventCalendarEvent.EventLevelType.Guild:
                    if (player.AccessLevel > AccessLevel.Player)
                        return true;

                    else if (eventCalendarEvent.Guild != null)
                    {
                        if (player.Guild == eventCalendarEvent.Guild)
                            return true;
                    }
                break;
            }

            return validAccess;
        }
    }

    public class EventCalendarSettingsGump : Gump
    {
        PlayerMobile m_Player;

        public int EntriesPerPage = 7;

        public EventCalendarSettingsGump(PlayerMobile player): base(10, 10)
        {
            m_Player = player;

            if (m_Player == null) return;
            if (m_Player.Deleted) return;

            EventCalendarPersistance.CheckAndCreateEventCalendarAccount(m_Player);

            if (m_Player.m_EventCalendarAccount == null)
                return;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int WhiteTextHue = 2499;

            AddImage(326, 1, 11049);
            AddImage(326, 168, 11049);
            AddImage(9, 1, 11049);
            AddImage(9, 167, 11049);
            AddImage(51, 8, 2524);
            AddImage(51, 111, 2524);
            AddImage(51, 188, 2524);
            AddImage(51, 268, 2524);
            AddImage(199, 9, 2524);
            AddImage(199, 112, 2524);
            AddImage(199, 189, 2524);
            AddImage(199, 268, 2524);
            AddImage(363, 9, 2524);
            AddImage(364, 110, 2524);
            AddImage(364, 187, 2524);
            AddImage(364, 268, 2524);
            AddImage(522, 9, 2524);
            AddImage(521, 109, 2524);
            AddImage(521, 187, 2524);
            AddImage(521, 268, 2524);
            AddImage(289, 4, 2440, 2425);

            AddLabel(319, 4, WhiteTextHue, "Account Settings");
            AddLabel(65, 55, 149, "Event Level + Type");
            AddLabel(199, 55, 149, "Show Pop-Up");
            AddLabel(294, 55, 149, "Play Sound");
            AddLabel(410, 55, 149, "Create Event Alert At Time Before Event");           
            AddImage(51, 190, 3001, 2415);
            AddImage(294, 190, 3001, 2415);
            AddImage(446, 190, 3001, 2415);
            AddImage(51, 80, 3001, 2415);
            AddImage(294, 80, 3001, 2415);
            AddImage(446, 80, 3001, 2415);                    
           
            AddImage(51, 302, 3001, 2415);
            AddImage(294, 302, 3001, 2415);
            AddImage(446, 302, 3001, 2415); 

            //-----

            AddLabel(75, 6, 149, "Wiki Page");
            AddButton(49, 8, 2095, 248, 1, GumpButtonType.Reply, 0);

            AddLabel(121, 31, 2599, "Your Local Time");

            string timeText = EventCalendarPersistance.PlayerFormattedDateTime(m_Player, DateTime.UtcNow);

            AddLabel(232, 31, WhiteTextHue, timeText);

            AddLabel(400, 31, 2603, "Time Zone Offset");
            AddButton(524, 29, 4014, 4016, 2, GumpButtonType.Reply, 0);

            string timeZoneOffset = m_Player.m_EventCalendarAccount.TimeZoneOffset.ToString();

            if (m_Player.m_EventCalendarAccount.TimeZoneOffset > 0)
                timeZoneOffset = "+" + timeZoneOffset;

            if (m_Player.m_EventCalendarAccount.TimeZoneOffset != 0)
                timeZoneOffset += "h";

            AddLabel(Utility.CenteredTextOffset(577, timeZoneOffset), 30, WhiteTextHue, timeZoneOffset);            
            
            AddButton(601, 29, 4005, 4007, 3, GumpButtonType.Reply, 0);

            int iStartY = 85;

            for (int a = 0; a < 8; a++)
            {
                string level = "";
                int levelHue = 0;

                string type = "";
                int typeHue = 0;

                switch (a)
                {
                    case 0: level = "Official"; levelHue = EventCalendarPersistance.OfficialEventHue; type = "Major"; typeHue = EventCalendarPersistance.MajorEventHue; break;
                    case 1: level = "Official"; levelHue = EventCalendarPersistance.OfficialEventHue; type = "Minor"; typeHue = EventCalendarPersistance.MinorEventHue; break;
                    case 2: level = "Official"; levelHue = EventCalendarPersistance.OfficialEventHue; type = "Recurring"; typeHue = EventCalendarPersistance.RecurringEventHue; break;

                    case 3: level = "Guild"; levelHue = EventCalendarPersistance.GuildEventHue; type = "Major"; typeHue = EventCalendarPersistance.MajorEventHue; break;
                    case 4: level = "Guild"; levelHue = EventCalendarPersistance.GuildEventHue; type = "Minor"; typeHue = EventCalendarPersistance.MinorEventHue; break;
                    case 5: level = "Guild"; levelHue = EventCalendarPersistance.GuildEventHue; type = "Recurring"; typeHue = EventCalendarPersistance.RecurringEventHue; break;

                    case 6: level = "Player"; levelHue = EventCalendarPersistance.PlayerEventHue; type = "Major"; typeHue = EventCalendarPersistance.MajorEventHue; break;
                    case 7: level = "Player"; levelHue = EventCalendarPersistance.PlayerEventHue; type = "Minor"; typeHue = EventCalendarPersistance.MinorEventHue; break;                    
                }

                AddLabel(70, iStartY + 3, levelHue, level);
                AddLabel(130, iStartY + 3, typeHue, type);

                AddItem(198, iStartY + 3, 5357);
                AddItem(278, iStartY, 3762);

                AddLabel(384, iStartY + 5, 0x3F, "5m");
                AddLabel(446, iStartY + 5, 2599, "15m");
                AddLabel(511, iStartY + 5, 2655, "1h");
                AddLabel(565, iStartY + 5, 2550, "2h");
                AddLabel(623, iStartY + 5, 2525, "24h");

                if (a < m_Player.m_EventCalendarAccount.AlertSettings.Count)
                {
                    EventCalendarAlertSetting alertSetting = m_Player.m_EventCalendarAccount.AlertSettings[a];

                    if (alertSetting.AlertPopUp)
                        AddButton(230, iStartY, 2154, 2151, (10 * a) + 10, GumpButtonType.Reply, 0);
                    else
                        AddButton(230, iStartY, 2151, 2154, (10 * a) + 10, GumpButtonType.Reply, 0);

                    if (alertSetting.AlertAudio)
                        AddButton(323, iStartY, 2154, 2151, (10 * a) + 11, GumpButtonType.Reply, 0);
                    else
                        AddButton(323, iStartY, 2151, 2154, (10 * a) + 11, GumpButtonType.Reply, 0);

                    if (alertSetting.AlertFiveMinutesPrior)
                        AddButton(406, iStartY, 2154, 2151, (10 * a) + 12, GumpButtonType.Reply, 0);
                    else
                        AddButton(406, iStartY, 2151, 2154, (10 * a) + 12, GumpButtonType.Reply, 0);

                    if (alertSetting.AlertFifteenMinutesPrior)
                        AddButton(472, iStartY, 2154, 2151, (10 * a) + 13, GumpButtonType.Reply, 0);
                    else
                        AddButton(472, iStartY, 2151, 2154, (10 * a) + 13, GumpButtonType.Reply, 0);

                    if (alertSetting.AlertOneHourPrior)
                        AddButton(527, iStartY, 2154, 2151, (10 * a) + 14, GumpButtonType.Reply, 0);
                    else
                        AddButton(527, iStartY, 2151, 2154, (10 * a) + 14, GumpButtonType.Reply, 0);

                    if (alertSetting.AlertTwoHoursPrior)
                        AddButton(583, iStartY, 2154, 2151, (10 * a) + 15, GumpButtonType.Reply, 0);
                    else
                        AddButton(583, iStartY, 2151, 2154, (10 * a) + 15, GumpButtonType.Reply, 0);

                    if (alertSetting.AlertTwentyFourHoursPrior)
                        AddButton(647, iStartY, 2154, 2151, (10 * a) + 16, GumpButtonType.Reply, 0);
                    else
                        AddButton(647, iStartY, 2151, 2154, (10 * a) + 16, GumpButtonType.Reply, 0);
                }

                iStartY += 35;

                if (a == 2)
                    iStartY += 9;

                if (a == 5)
                    iStartY += 8;
            }           
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null) return;
            if (m_Player.Deleted) return;

            EventCalendarPersistance.CheckAndCreateEventCalendarAccount(m_Player);
            
            bool closeGump = true;

            switch (info.ButtonID)
            {
                case 1:
                    //Wiki Page
                    closeGump = false;
                    break;

                case 2:
                    //Time Zone Offset Backward
                    if (m_Player.m_EventCalendarAccount.TimeZoneOffset > -24)
                    {
                        m_Player.m_EventCalendarAccount.TimeZoneOffset -= .5;
                    }

                    closeGump = false;
                break;

                case 3:
                    //Time Zone Offset Forward
                    if (m_Player.m_EventCalendarAccount.TimeZoneOffset < 24)
                    {
                        m_Player.m_EventCalendarAccount.TimeZoneOffset += .5;
                    }

                    closeGump = false;
                break;
            }

            //Alert Buttons
            if (info.ButtonID >= 10)
            {
                int alertIndex = ((int)(Math.Floor((double)info.ButtonID / 10))) - 1;
                int buttonIndex = info.ButtonID % 10;

                if (alertIndex < m_Player.m_EventCalendarAccount.AlertSettings.Count)
                {
                    EventCalendarAlertSetting alertSetting = m_Player.m_EventCalendarAccount.AlertSettings[alertIndex];

                    switch (buttonIndex)
                    {
                        case 0: alertSetting.AlertPopUp = !alertSetting.AlertPopUp; break;
                        case 1: alertSetting.AlertPopUp = !alertSetting.AlertPopUp; break;

                        case 2: alertSetting.AlertFiveMinutesPrior = !alertSetting.AlertFiveMinutesPrior; break;
                        case 3: alertSetting.AlertFifteenMinutesPrior = !alertSetting.AlertFifteenMinutesPrior; break;
                        case 4: alertSetting.AlertOneHourPrior = !alertSetting.AlertOneHourPrior; break;
                        case 5: alertSetting.AlertTwoHoursPrior = !alertSetting.AlertTwoHoursPrior; break;
                        case 6: alertSetting.AlertTwentyFourHoursPrior = !alertSetting.AlertTwentyFourHoursPrior; break;
                    }
                }
                
                closeGump = false;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(EventCalendarSettingsGump));
                m_Player.SendGump(new EventCalendarSettingsGump(m_Player));
            }
        }
    }
}