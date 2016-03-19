using System;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Xml;

using Server;
using Server.Gumps;
using Server.Network;
using Server.Commands;
using Server.Items;
using Server.Mobiles;
using Server.Achievements;
using Server.Accounting;
using Server.Targeting;

namespace Server.Achievements
{

    //////////////////////////////////
    // AchievementNotificationGump: Small popup that notifies the user of a completed achievement
    //////////////////////////////////
    public class AchievementNotificationGump : Gump
    {
        private int m_AchievementID;
        private Mobile m_Mobile;

        public AchievementNotificationGump(Mobile mobile, int achievement_id)
            : base(30, 30)
        {
            m_AchievementID = achievement_id;
            m_Mobile = mobile;

            AddPage(0);
            AddBackground(0, 0, 244, 85, 0x23f0);
            AddAlphaRegion(5, 7, 234, 71);
            AddItem(5, 18, 0x0e40); // closed chest
            AddButton(10, 12, 0x52, 0x52, 1, GumpButtonType.Reply, 0); // INVISIBLE BUTTON
            AddButton(120, 12, 0x52, 0x52, 2, GumpButtonType.Reply, 0); // INVISIBLE BUTTON
            AddHtml(64, 24, 170, 24, "<BASEFONT COLOR=white><CENTER>ACHIEVEMENT COMPLETED!</CENTER></BASEFONT>", false, false);
            AddHtml(64, 42, 170, 24, String.Format("<BASEFONT COLOR=white><CENTER>\"{0}\"</CENTER></BASEFONT>", AchievementSystemImpl.Instance.m_AllAchievements[achievement_id].m_Name), false, false);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            // if not canceled show info gump on the accomplished achievement 
            if (info.ButtonID > 0)
                state.Mobile.SendGump(new AchievementInfoGump(state.Mobile, state.Mobile, -1, m_AchievementID, -1));
        }
    }


    //////////////////////////////////
    // AchievementTrackerGump: Detailed info on a single achievement
    //////////////////////////////////
    public class AchievementInfoGump : Gump
    {
        private const int WhiteFontColor = 0xFFFFFF;
        private const int RedFontColor = 0xFF0000;
        private const int YellowFontColor = 0xFFFF00;
        private const int GreenFontColor = 0x00FA00;
        private const int AreaBackgroundImage = 0xa40;
        private const int BorderBackgroundImage = 0x23f0;

        private Mobile m_AchievementOwner;
        private Mobile m_Beholder;
        private int m_FromCategory;
        private int m_FromPageId;
        private int m_AchievementID;

        enum Button
        {
            Clear = 155,
            Complete,
            Claim
        }

        public AchievementInfoGump(Mobile achievement_owner, Mobile beholder, int from_category, int achievement_id, int from_page)
            : base(100, 100)
        {
            Debug.Assert(achievement_owner.Player);

            m_FromCategory = from_category;
            m_FromPageId = from_page;
            m_AchievementID = achievement_id;
            m_Beholder = beholder;
            m_AchievementOwner = achievement_owner;

            m_Beholder.CloseGump(typeof(AchievementTrackerGump));
            m_Beholder.CloseGump(typeof(AchievementInfoGump));

            bool inspecting_self = achievement_owner == beholder;

            AddPage(0);
            AddBackground(0, 0, 530, 245, BorderBackgroundImage);

            //				X	Y	W	H
            AddImageTiled(10, 10, 510, 22, AreaBackgroundImage);		// top bar - title bar
            AddImageTiled(10, 37, 150, 198, AreaBackgroundImage);		// left - rewards box
            AddImageTiled(165, 37, 355, 77, AreaBackgroundImage);		// high right - description box
            AddImageTiled(165, 119, 355, 116, AreaBackgroundImage);		// low right - info/progress box

            AddAlphaRegion(10, 10, 510, 225);

            if (m_AchievementID >= AchievementSystemImpl.Instance.m_AllAchievements.Count)
                return; // meh

            Achievement achievement = AchievementSystemImpl.Instance.m_AllAchievements[achievement_id];
            PlayerAccomplishments player_achievements = (((PlayerMobile)m_AchievementOwner).Account as Account).AccountAchievements;
            int progress = player_achievements.GetTriggerProgress(achievement.m_TriggerID);
            int goal = achievement.m_TriggerCountRequired;
            bool discovered = achievement.m_MustBeDiscovered ? progress > 0 : true;
            bool completed = progress >= achievement.m_TriggerCountRequired;


            // Window title (achievement name)
            AddHtml(10, 12, 510, 20, String.Format("<BASEFONT COLOR=#{0:X6}><CENTER>{1}</CENTER></BASEFONT>", WhiteFontColor, achievement.m_Name), false, false);

            // Achievement description
            AddHtml(170, 42, 350, 90, String.Format("<BASEFONT COLOR=#{0:X6}><CENTER>{1}</CENTER></BASEFONT>", discovered ? WhiteFontColor : RedFontColor, discovered ? AchievementSystemImpl.Instance.m_AllAchievementTriggers[achievement.m_TriggerID].m_Description : "???"), false, false);

            // Reward
            AddHtml(10, 42, 150, 22, String.Format("<BASEFONT COLOR=#{0:X6}><CENTER>REWARD</CENTER></BASEFONT>", WhiteFontColor), false, false);
            RenderRewardInfo(achievement);


            // progress info
            object[] args = {	WhiteFontColor, 
								completed ? GreenFontColor : progress > 0 ? YellowFontColor : WhiteFontColor, 
								Math.Min(progress, achievement.m_TriggerCountRequired),
								WhiteFontColor,
								goal.ToString() 
							};
            AddHtml(170, 124, 355, 22, String.Format("<BASEFONT COLOR=#{0:X6}><CENTER>PROGRESS: <BASEFONT COLOR=#{1:X6}>{2}<BASEFONT COLOR=#{3:X6}>/<BASEFONT COLOR=#00fa00>{4}</CENTER></BASEFONT>", args), false, false);
            AddHtml(170, 142, 355, 22, String.Format("<BASEFONT COLOR=#{0:X6}><CENTER>This achievement has {1}</CENTER></BASEFONT>", WhiteFontColor, completed ? "been completed." : "not been completed."), false, false);
            if (player_achievements.GetIsCompleted(achievement))
            {
                if (player_achievements.GetHasBeenRewarded(achievement))
                {
                    AddHtml(165, 170, 355, 22, "<BASEFONT COLOR=#00FA00><CENTER>You have already claimed the award for this achievement</CENTER></BASEFONT>", false, false);
                }
                else if (inspecting_self || beholder.AccessLevel >= AccessLevel.GameMaster)
                {
                    // 0xfb9
                    int x = 415;
                    int y = 168;
                    AddButton(x, y, 0x81c, 0x81b, (int)Button.Claim, GumpButtonType.Reply, 0);
                    AddButton(x, y + 30, 0x81c, 0x81b, 0, GumpButtonType.Reply, 0);
                    AddHtml(170, 170, 355, 22, "<BASEFONT COLOR=#ffffff><CENTER>CLAIM REWARD NOW </CENTER></BASEFONT>", false, false);
                    AddHtml(170, 200, 355, 22, "<BASEFONT COLOR=#ffffff><CENTER>CLAIM REWARD LATER </CENTER></BASEFONT>", false, false);
                }
            }

            // admin control
            if (beholder.AccessLevel >= AccessLevel.GameMaster)
            {
                AddHtml(340, 70, 355, 22, "<BASEFONT COLOR=#ffff00>(Admin)Set completed</BASEFONT>", false, false);
                AddHtml(340, 90, 355, 22, "<BASEFONT COLOR=#ffff00>(Admin)Clear progress</BASEFONT>", false, false);
                AddButton(480, 70, 0xfa5, 0xfa7, (int)Button.Complete, GumpButtonType.Reply, 0); // complete now
                AddButton(480, 90, 0xfb1, 0xfb3, (int)Button.Clear, GumpButtonType.Reply, 0); // clear
            }
        }

        private void RenderRewardInfo(Achievement achievement)
        {
            int x = 15 + achievement.m_RewardItemGraphicsOffset.X;
            int y = 60 + achievement.m_RewardItemGraphicsOffset.Y;

            if (achievement.m_RewardItemGraphics > 0)
            {
                AddItem(x, y, achievement.m_RewardItemGraphics, achievement.m_RewardHue);
                string desc = achievement.m_RewardWindowShortDesc.Length > 0 ? achievement.m_RewardWindowShortDesc : achievement.m_RewardItemType != null ? achievement.m_RewardItemType.Name : achievement.m_RewardDescription;
                AddHtml(15, 170, 140, 40, String.Format("<BASEFONT COLOR=#ffffff><CENTER>{0}</CENTER>", desc), false, false);                
                //AddHtml(15, 170, 140, 22, String.Format("<BASEFONT COLOR=#ffffff><CENTER>{0}</CENTER>", desc), false, false);
                
            }
            else if (achievement.m_RewardTitle.Length > 0 || achievement.m_RewardTitle2.Length > 0)
            {
                AddHtml(15, 170, 140, 22, "<BASEFONT COLOR=#ffffff><CENTER>A new title</CENTER>", false, false);
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            PlayerMobile player = m_Beholder as PlayerMobile;

            if (player == null)
                return;

            // Back Button or window closed
            if (info.ButtonID == 0)
            {
                AchievementTrackerGump trackerGump = new AchievementTrackerGump(m_AchievementOwner, m_Beholder, m_FromCategory, m_FromPageId);
                m_Beholder.SendGump(trackerGump);
            }

            else if (info.ButtonID == (int)Button.Clear)
            {
                if (m_Beholder.AccessLevel >= AccessLevel.GameMaster)
                {
                    Achievement achievement = AchievementSystemImpl.Instance.m_AllAchievements[m_AchievementID];
                    PlayerAccomplishments player_state = (((PlayerMobile)m_AchievementOwner).Account as Account).AccountAchievements;
                    AchievementSystemImpl.Instance.SetProgress(m_AchievementOwner, (ushort)achievement.m_TriggerID, 0);
                    m_Beholder.SendGump(new AchievementInfoGump(m_AchievementOwner, m_Beholder, m_FromCategory, m_AchievementID, m_FromPageId));
                }
            }

            else if (info.ButtonID == (int)Button.Complete)
            {
                if (m_Beholder.AccessLevel >= AccessLevel.GameMaster)
                {
                    Achievement achievement = AchievementSystemImpl.Instance.m_AllAchievements[m_AchievementID];
                    PlayerAccomplishments player_state = (((PlayerMobile)m_AchievementOwner).Account as Account).AccountAchievements;
                    AchievementSystemImpl.Instance.SetProgress(m_AchievementOwner, (ushort)achievement.m_TriggerID, 0);
                    AchievementSystemImpl.Instance.TickProgressMulti(m_AchievementOwner, (AchievementTriggers)achievement.m_TriggerID, achievement.m_TriggerCountRequired);
                    m_Beholder.SendGump(new AchievementInfoGump(m_AchievementOwner, m_Beholder, m_FromCategory, m_AchievementID, m_FromPageId));
                }
            }

            // Claim Reward
            else if (info.ButtonID == (int)Button.Claim)
            {
                if (player.IsInUOACZ)
                {
                    player.SendMessage("Achievements cannot be redeemed while in the UOACZ world.");
                    return;
                }

                if (AchievementSystemImpl.Instance.m_AllAchievements.Count > m_AchievementID)
                    AchievementSystemImpl.Instance.TryClaimReward(m_AchievementOwner, m_AchievementID);

                if (m_FromCategory != -1 && m_FromPageId != -1)
                {
                    AchievementTrackerGump trackerGump = new AchievementTrackerGump(m_AchievementOwner, m_Beholder, m_FromCategory, m_FromPageId);
                    m_Beholder.SendGump(trackerGump);
                }
            }
        }
    }


    //////////////////////////////////
    // AchievementTrackerGump: Overiew of a players achievements
    //////////////////////////////////
    public class AchievementTrackerGump : Gump
    {
        //private Mobile m_From;
        private Mobile m_AchievementOwner;
        private Mobile m_Beholder;
        private int m_CurrentCategory;
        private int m_CurrentPageNumber;

        private const int DeepBlueColor = 0x7FFF;
        private const int WhiteFontColor = 0xFFFFFF;
        private const int YellowFontColor = 0xFFFF00;
        private const int GreenFontColor = 0x00FA00;

        private const int ENTRIES_PER_PAGE = 12;
        private const int NEXT_PAGE_ID = 256;
        private const int PREV_PAGE_ID = 257;
        private const int ACHIEVEMENT_INFO_RANGE_START = 512;

        enum Button
        {
            BtnPrevious = 4000,
            BtnNext = 4001,
            BtnInspectOthers = 4002,
            ChkShowNotifications = 4003,
            BtnLeaderboards = 4004,
        }

        public AchievementTrackerGump(Mobile achievement_owner, Mobile beholder, int category_index, int page_number)
            : base(50, 50)
        {
            //m_From = from;
            m_AchievementOwner = achievement_owner;
            m_Beholder = beholder;
            m_Beholder.CloseGump(typeof(AchievementTrackerGump));
            m_Beholder.CloseGump(typeof(AchievementInfoGump));

            PlayerAccomplishments player_achievements = (achievement_owner.Account as Account).AccountAchievements;

            bool inspecting_self = m_Beholder == m_AchievementOwner;
            m_CurrentCategory = Math.Min(category_index, AchievementSystemImpl.Instance.m_CategoriesLookup.Count - 1);
            m_CurrentPageNumber = page_number;

            AddPage(0);

            AddBackground(0, 0, 530, 450, 0x23f0);

            //				x		y		w		h
            AddImageTiled(10, 10, 510, 22, 2624); // top bar
            AddImageTiled(10, 37, 200, 403, 2624); // left side
            AddImageTiled(215, 37, 305, 322, 2624); // upper right
            AddImageTiled(215, 364, 305, 76, 2624); // lower right
            AddAlphaRegion(10, 10, 510, 430);

            // Headers
            AddHtml(10, 12, 510, 20, String.Format("<BASEFONT COLOR=#{0:X6}><CENTER>{1} - {2}</CENTER></BASEFONT>", WhiteFontColor, AchievementSystemImpl.Instance.m_SysTweaks.m_TrackerTitle, m_AchievementOwner.Name), false, false);
            AddHtml(10, 37, 200, 22, String.Format("<BASEFONT COLOR=#{0:X6}><CENTER>CATEGORIES</CENTER></BASEFONT>", WhiteFontColor), false, false);

            // lower right buttons (inspect others etc)
            AddButton(220, 369, 0xd0, 0xd1, (int)Button.BtnLeaderboards, GumpButtonType.Reply, 0); // 0xd0 / d1
            AddHtml(242, 369, 295, 20, String.Format("<BASEFONT COLOR=#{0:X6}>Server Top Achievers</BASEFONT>", WhiteFontColor), false, false);
            AddButton(220, 390, 0xd0, 0xd1, (int)Button.BtnInspectOthers, GumpButtonType.Reply, 0); // 0xd0 / d1
            AddHtml(242, 390, 295, 20, String.Format("<BASEFONT COLOR=#{0:X6}>Inspect Other</BASEFONT>", WhiteFontColor), false, false);
            if (inspecting_self)
            {
                this.AddCheck(220, 411, 0xD2, 0xD3, player_achievements.m_ShowNotifications, (int)Button.ChkShowNotifications);
                this.AddHtml(242, 411, 295, 20, String.Format("<BASEFONT COLOR=#{0:X6}>Show Achievement Notifications</BASEFONT>", WhiteFontColor), false, false);
            }

            // left-hand achievement categories
            int total_completed = 0;
            int i = 0;
            foreach (AchievementCategory category in AchievementSystemImpl.Instance.m_CategoriesLookup)
            {
                int category_completed = 0;
                for (int j = 0; j < category.m_AchievementIndices.Count; ++j)
                {
                    Achievement ach = AchievementSystemImpl.Instance.m_AllAchievements[category.m_AchievementIndices[j]];
                    if (player_achievements.GetIsCompleted(ach))
                        ++category_completed;
                }

                int y_button = 60 + (i * 20);
                int y_text = 63 + (i * 20);
                AddButton(15, y_button, 4005, 4007, i + 1, GumpButtonType.Reply, 0);
                AddHtml(50, y_text, 150, 18, String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", WhiteFontColor, category.m_Category), false, false);
                int first_number_color = category_completed == category.m_AchievementIndices.Count ? GreenFontColor : category_completed > 0 ? YellowFontColor : WhiteFontColor;
                AddHtml(160, y_text, 200, 25, String.Format("<BASEFONT COLOR=#{0:X6}>{1}<BASEFONT COLOR=#ffffff>/<BASEFONT COLOR=#00FA00>{2}</BASEFONT>", first_number_color, category_completed, category.m_AchievementIndices.Count), false, false);
                ++i;
                total_completed += category_completed;
            }
            // daily achievement
            AddButton(15, 60 + (i * 20), 4005, 4007, i + 1, GumpButtonType.Reply, 0);
            AddHtml(50, 63 + (i * 20), 150, 18, String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", WhiteFontColor, "Daily"), false, false);

            // left hand below categories; total count
            AddHtml(14, 411, 200, 25, String.Format("<BASEFONT COLOR=#{0:X6}>Total Achievements:</BASEFONT>", WhiteFontColor), false, false);
            int first_num_color = total_completed == AchievementSystemImpl.Instance.m_AllAchievements.Count ? GreenFontColor : total_completed > 0 ? YellowFontColor : WhiteFontColor;
            AddHtml(160, 411, 200, 25, String.Format("<BASEFONT COLOR=#{0:X6}>{1}<BASEFONT COLOR=#ffffff>/<BASEFONT COLOR=#00FA00>{2}</BASEFONT>", first_num_color, total_completed, AchievementSystemImpl.Instance.m_AllAchievements.Count), false, false);

            // populate category if one was selected
            if (m_CurrentCategory >= 0)
            {
                AchievementCategory category = AchievementSystemImpl.Instance.m_CategoriesLookup[m_CurrentCategory];
                AddHtml(215, 37, 305, 22, String.Format("<BASEFONT COLOR=#{0:X6}><CENTER>{1} ACHIEVEMENTS</CENTER></BASEFONT>", WhiteFontColor, category.m_Category), false, false);

                int start = Math.Max(0, m_CurrentPageNumber * ENTRIES_PER_PAGE);

                if (start > 0)
                    AddButton(445, 335, 4014, 4016, (int)Button.BtnPrevious, GumpButtonType.Reply, 0); // previous button
                if (category.m_AchievementIndices.Count > (m_CurrentPageNumber + 1) * ENTRIES_PER_PAGE)
                    AddButton(480, 335, 4005, 4007, (int)Button.BtnNext, GumpButtonType.Reply, 0); // next button


                int end = Math.Min(category.m_AchievementIndices.Count, start + ENTRIES_PER_PAGE);
                int green_dot = 0x939;
                for (i = start; i < end; ++i)
                {
                    Achievement achievement = AchievementSystemImpl.Instance.m_AllAchievements[category.m_AchievementIndices[i]];
                    string achievement_name = achievement.m_Name;
                    int y_text = 63 + ((i - start) * 20);
                    bool has_been_rewarded = player_achievements.GetHasBeenRewarded(achievement);
                    bool completed = player_achievements.GetIsCompleted(achievement);
                    if (completed)
                        AddImage(222, y_text + 4, green_dot);
                    AddHtml(242, y_text, 255, 18, String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", completed && !has_been_rewarded ? YellowFontColor : WhiteFontColor, achievement_name), false, false);
                    AddButton(480, y_text - 3, 4011, 4012, (ACHIEVEMENT_INFO_RANGE_START + category.m_AchievementIndices[i]), GumpButtonType.Reply, 0);
                }
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            // Always handle the check box states
            PlayerAccomplishments player_achievements = (((PlayerMobile)m_AchievementOwner).Account as Account).AccountAchievements;
            player_achievements.m_ShowNotifications = info.IsSwitched((int)Button.ChkShowNotifications);

            if (info.ButtonID <= 0)
                return; // Canceled

            if (info.ButtonID == (int)Button.BtnPrevious)
            {
                // previous
                m_Beholder.SendGump(new AchievementTrackerGump(m_AchievementOwner, m_Beholder, m_CurrentCategory, m_CurrentPageNumber - 1));
            }
            else if (info.ButtonID == (int)Button.BtnNext)
            {
                // next
                m_Beholder.SendGump(new AchievementTrackerGump(m_AchievementOwner, m_Beholder, m_CurrentCategory, m_CurrentPageNumber + 1));
            }
            else if (info.ButtonID == (int)Button.BtnInspectOthers)
            {
                m_Beholder.Target = new TargetOtherForAchInspectTarget(m_Beholder);
            }
            else if (info.ButtonID == (int)Button.BtnLeaderboards)
            {
                AchievementsReport.ShowGump(m_Beholder);
            }
            else if (info.ButtonID >= ACHIEVEMENT_INFO_RANGE_START)
            {
                // info button clicked
                int achievement_index = info.ButtonID - ACHIEVEMENT_INFO_RANGE_START;
                if (AchievementSystemImpl.Instance.m_AllAchievements.Count > achievement_index)
                {
                    AchievementInfoGump info_gump = new AchievementInfoGump(m_AchievementOwner, m_Beholder, m_CurrentCategory, achievement_index, m_CurrentPageNumber);
                    m_Beholder.SendGump(info_gump);
                }
            }
            else
            {
                // category change
                int new_category = info.ButtonID - 1;
                if (new_category > (AchievementSystemImpl.Instance.m_CategoriesLookup.Count - 1))
                    sender.Mobile.SendGump(new DailyAchievementTrackerGump(m_AchievementOwner, m_Beholder));
                else
                    sender.Mobile.SendGump(new AchievementTrackerGump(m_AchievementOwner, m_Beholder, new_category, 0));
            }
        }

        private class TargetOtherForAchInspectTarget : Target
        {
            public TargetOtherForAchInspectTarget(Mobile owner)
                : base(12, false, TargetFlags.None)
            {
                owner.SendMessage("Target the player you wish to inspect.");
            }
            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is PlayerMobile)
                {
                    from.SendGump(new AchievementTrackerGump(targeted as PlayerMobile, from, 0, 0));
                }
                else
                {
                    from.SendMessage("That has not achieved anything.");
                }
            }
        }
    }
}