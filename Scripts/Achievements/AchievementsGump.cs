using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using Server.Network;

namespace Server
{
    public class AchievementsGump : Gump
    {
        public enum PageType
        {
            Main,
            Category,
            Settings
        }

        public PlayerMobile m_Player;        
        public PageType m_SelectedPageType = PageType.Main;

        //Achievement Main Page
        public int m_MainPage = 0;

        public static int categoryRows = 3;
        public static int categoryColumns = 4;       

        //Achievement Category Page
        public AchievementCategory m_AchievementCategory = AchievementCategory.Battle;
        public int m_AchievementListPage = 0;
        public int m_AchievementSelectedIndex = 0;         

        public AchievementsGump(Mobile from, PageType selectedPageType, int mainPage, AchievementCategory achievementCategory, int achievementListPage, int achievementSelectedIndex): base(10, 10)
        {
            m_Player = from as PlayerMobile;

            if (m_Player == null)
                return;

            AchievementsPersistance.CheckAndCreateAchievementAccountEntry(m_Player);

            m_SelectedPageType = selectedPageType;
            m_MainPage = mainPage;
            m_AchievementCategory = achievementCategory;
            m_AchievementListPage = achievementListPage;
            m_AchievementSelectedIndex = achievementSelectedIndex;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int WhiteTextHue = 2655;
            int GreenTextHue = 63;
            int YellowTextHue = 2550;
            int GreyTextHue = 2401;

            int startX = 0;
            int startY = 0;

            AddPage(0);

            int totalPages;

            switch (m_SelectedPageType)
            {
                #region Main

                case PageType.Main:
                    AddImage(142, 105, 103);
                    AddImage(143, 148, 103);
                    AddImage(143, 234, 103);
                    AddImage(143, 324, 103);
                    AddImage(145, 409, 103, 2401);
                    AddImage(8, 410, 103, 2401);
                    AddImage(8, 233, 103, 2401);
                    AddImage(142, 8, 103, 2401);
                    AddImage(7, 147, 103, 2401);
                    AddImage(8, 7, 103, 2401);
                    AddImage(8, 104, 103, 2401);
                    AddImage(17, 107, 3604, 2052);
                    AddImage(143, 107, 3604, 2052);
                    AddImage(17, 18, 3604, 2052);
                    AddImage(143, 18, 3604, 2052);            
                    AddImage(8, 323, 103, 2401);
                    AddImage(17, 194, 3604, 2052);
                    AddImage(143, 194, 3604, 2052);
                    AddImage(17, 286, 3604, 2052);
                    AddImage(143, 286, 3604, 2052);
                    AddImage(17, 370, 3604, 2052);
                    AddImage(143, 370, 3604, 2052);
                    AddImage(274, 105, 103);
                    AddImage(275, 148, 103);
                    AddImage(275, 234, 103);
                    AddImage(275, 324, 103);
                    AddImage(276, 410, 103, 2401);
                    AddImage(274, 8, 103, 2401);
                    AddImage(406, 105, 103, 2401);
                    AddImage(407, 148, 103, 2401);
                    AddImage(407, 234, 103, 2401);
                    AddImage(407, 324, 103, 2401);
                    AddImage(408, 410, 103, 2401);
                    AddImage(406, 8, 103, 2401);
                    AddImage(267, 107, 3604, 2052);
                    AddImage(267, 18, 3604, 2052);
                    AddImage(267, 194, 3604, 2052);
                    AddImage(267, 286, 3604, 2052);
                    AddImage(267, 370, 3604, 2052);
                    AddImage(375, 107, 3604, 2052);
                    AddImage(375, 18, 3604, 2052);
                    AddImage(375, 194, 3604, 2052);
                    AddImage(375, 286, 3604, 2052);
                    AddImage(375, 370, 3604, 2052);
                    AddImage(409, 107, 3604, 2052);
                    AddImage(409, 18, 3604, 2052);
                    AddImage(409, 194, 3604, 2052);
                    AddImage(409, 286, 3604, 2052);
                    AddImage(409, 370, 3604, 2052);
                    AddImage(144, -2, 1143, 2499);

                    AddLabel(235, 0, Achievements.AchievementTextHue, "Achievements");    
                   
                    int categoriesPerPage = categoryRows * categoryColumns;
                    int totalCategories = Enum.GetNames(typeof(AchievementCategory)).Length;
                    int totalCategoryPages = (int)(Math.Ceiling((double)totalCategories / (double)categoriesPerPage));

                    if (m_MainPage >= totalCategoryPages)
                        m_MainPage = 0;

                    if (m_MainPage < 0)
                        m_MainPage = 0;

                    int categoryStartIndex = m_MainPage * categoriesPerPage;
                    int categoryEndIndex = (m_MainPage * categoriesPerPage) + (categoriesPerPage - 1);

                    if (categoryEndIndex >= totalCategories)
                        categoryEndIndex = totalCategories - 1;

                    int totalCompletedAchievements = Achievements.GetCompletedAchievementCount(m_Player);
                    int totalAchievementsAvailable = Achievements.GetTotalAchievementsCount();

                    //Guild
                    AddButton(7, 4, 2094, 2095, 1, GumpButtonType.Reply, 0);
                    AddLabel(30, 4, 149, "Guide");

                    //Previous
                    if (m_MainPage > 0)
                    {
                        AddLabel(57, 472, WhiteTextHue, "Previous Page");
                        AddButton(23, 471, 4014, 4019, 2, GumpButtonType.Reply, 0);
                    }

                    //Next
                    if (m_MainPage < totalCategoryPages - 1)
                    {
                        AddButton(502, 471, 4005, 4019, 3, GumpButtonType.Reply, 0);
                        AddLabel(432, 472, WhiteTextHue, "Next Page");
                    }

                    //Settings
                    AddButton(243, 471, 4029, 4019, 4, GumpButtonType.Reply, 0);
                    AddLabel(277, 472, 63, "Settings");

                    int textHue = GreyTextHue;

                    if (totalCompletedAchievements > 0)
                        textHue = YellowTextHue;

                    if (totalCompletedAchievements == totalAchievementsAvailable)
                        textHue = GreenTextHue;

                    AddLabel(175, 447, 149, "Total");
                    AddLabel(332, 447, textHue, totalCompletedAchievements.ToString() + "/" + totalAchievementsAvailable.ToString());
                                        
                    double totalAchievementProgress = (double)totalCompletedAchievements / (double)totalAchievementsAvailable;
                    
                    AddImage(216, 452, Achievements.GetProgressBarBackground(totalAchievementProgress));
                    AddImageTiled(216 + Utility.ProgressBarX(totalAchievementProgress), 455, Utility.ProgressBarWidth(totalAchievementProgress), 7, 2488);
                    
                    int iBaseX = 25;
                    int iBaseY = 30;

                    startX = iBaseX;
                    startY = iBaseY;

                    int columnIndex = 0;
                    int rowIndex = 0;

                    int rowSpacing = 140;
                    int columnSpacing = 125;
            
                    int categoryCount = categoryEndIndex - categoryStartIndex;

                    for (int a = 0; a < categoryCount + 1; a++)
                    {
                        int categoryIndex = categoryStartIndex + a;
                        int buttonIndex = 10 + categoryIndex;
                        
                        if (categoryStartIndex >= totalCategories)
                            continue;

                        AchievementCategory category = (AchievementCategory)categoryIndex;

                        #region Achievement Category Images

                        switch (category)
                        {
                            case AchievementCategory.Battle:
                                AddItem(startX + 19, startY + 34, 18210, 2500);
                                AddItem(startX + 21, startY + 33, 5049, 2500);
                                AddItem(startX + 14, startY + 22, 5138, 2500);
                                AddItem(startX + 4, startY + 40, 7028, 2500);
                                AddLabel(startX + 45, startY + 5, 2220, "Battle");
                                AddImage(startX + 60, startY + 32, 10550, 2220);
                                AddImage(startX + 88, startY + 32, 10552, 2220);
                                AddButton(startX + 74, startY + 46, 9721, 9721, buttonIndex, GumpButtonType.Reply, 0);
                            break;

                            case AchievementCategory.Adventuring:
                                AddItem(startX + 4, startY + 19, 3226);
                                AddItem(startX + 19, startY + 44, 4967);
                                AddItem(startX + 11, startY + 55, 4970);
                                AddImage(startX + 60, startY + 32, 10550, 149);
                                AddImage(startX + 88, startY + 32, 10552, 149);
                                AddItem(startX + 26, startY + 69, 2648);
                                AddItem(startX + -4, startY + 58, 5356);
                                AddItem(startX + 2, startY + 65, 3922);
                                AddItem(startX + -37, startY + 50, 3898);
                                AddLabel(startX + 27, startY + 5, 148, "Adventuring");
                                AddButton(startX + 74, startY + 46, 9721, 9721, buttonIndex, GumpButtonType.Reply, 0);
                            break;

                            case AchievementCategory.Slaying:
                                AddItem(startX + 3, startY + 31, 7433);
                                AddItem(startX + 8, startY + 56, 4655);
                                AddItem(startX + 21, startY + 20, 7438);
                                AddItem(startX + 45, startY + 34, 7419);
                                AddItem(startX + 68, startY + 33, 7418);
                                AddItem(startX + 7, startY + 54, 7782);
                                AddItem(startX + 43, startY + 68, 7430);
                                AddImage(startX + 60, startY + 32, 10550, 2116);
                                AddImage(startX + 88, startY + 32, 10552, 2116);
                                AddItem(startX + 23, startY + 52, 3910);
                                AddLabel(startX + 44, startY + 5, 2116, "Slaying");
                                AddButton(startX + 74, startY + 46, 9721, 9721, buttonIndex, GumpButtonType.Reply, 0);
                            break;

                            case AchievementCategory.Seafaring:
                                AddItem(startX + 5, startY + 50, 5363);
                                AddItem(startX + 23, startY + 60, 5365);
                                AddItem(startX + 24, startY + 37, 5370);
                                AddImage(startX + 60, startY + 32, 10550, 2602);
                                AddImage(startX + 88, startY + 32, 10552, 2602);
                                AddLabel(startX + 36, startY + 5, 2603, "Seafaring");
                                AddButton(startX + 74, startY + 46, 9721, 9721, buttonIndex, GumpButtonType.Reply, 0);
                            break;

                            case AchievementCategory.Luxury:
                                AddItem(startX + -3, startY + 68, 2448, 2425);
                                AddImage(startX + 60, startY + 32, 10550, 2618);
                                AddImage(startX + 88, startY + 32, 10552, 2618);
                                AddItem(startX + 19, startY + 30, 16508);
                                AddItem(startX + 1, startY + 63, 2459, 2562);
                                AddItem(startX + -8, startY + 63, 2459, 2600);
                                AddItem(startX + -4, startY + 67, 2459, 2606);
                                AddLabel(startX + 43, startY + 5, 2618, "Luxury");
                                AddButton(startX + 74, startY + 46, 9721, 9721, buttonIndex, GumpButtonType.Reply, 0);
                            break;

                            case AchievementCategory.Competition:
                                AddItem(startX + 7, startY + 40, 16434);
                                AddItem(startX + 18, startY + 26, 16433);
                                AddImage(startX + 60, startY + 32, 10550, 2963);
                                AddImage(startX + 88, startY + 32, 10552, 2963);
                                AddItem(startX + 8, startY + 35, 4006);
                                AddItem(startX + 0, startY + 48, 4008, 2500);
                                AddItem(startX + 10, startY + 34, 4008, 1107);
                                AddLabel(startX + 26, startY + 5, 2962, "Competition");
                                AddButton(startX + 74, startY + 46, 9721, 9721, buttonIndex, GumpButtonType.Reply, 0);
                            break;

                            case AchievementCategory.AnimalTaming:
                                AddImage(startX + 60, startY + 32, 10550, 2208);
                                AddImage(startX + 88, startY + 32, 10552, 2208);
                                AddItem(startX + 13, startY + 49, 2476);
                                AddItem(startX + 10, startY + 40, 3191);
                                AddItem(startX + 14, startY + 38, 3191);
                                AddItem(startX + 13, startY + 39, 3713);
                                AddLabel(startX + 23, startY + 5, 2599, "Animal Taming");
                                AddButton(startX + 74, startY + 46, 9721, 9721, buttonIndex, GumpButtonType.Reply, 0);
                            break;

                            case AchievementCategory.Harvesting:
                                AddImage(startX + 88, startY + 32, 10552, 2417);
                                AddItem(startX + 27, startY + 44, 3346, 2208);
                                AddImage(startX + 60, startY + 32, 10550, 2417);
                                AddItem(startX + 6, startY + 33, 3670);
                                AddItem(startX + 36, startY + 29, 3351, 2208);
                                AddItem(startX + -15, startY + 79, 3352, 2208);
                                AddItem(startX + 2, startY + 65, 3344, 2208);
                                AddItem(startX + 18, startY + 62, 7137);
                                AddItem(startX + 27, startY + 57, 3908);
                                AddItem(startX + 28, startY + 71, 2482, 2500);
                                AddLabel(startX + 28, startY + 5, 2417, "Harvesting");
                                AddButton(startX + 74, startY + 46, 9721, 9721, buttonIndex, GumpButtonType.Reply, 0);
                            break;

                            case AchievementCategory.Virtue:
                                AddItem(startX + -5, startY + 16, 2);
                                AddImage(startX + 60, startY + 32, 10550, 2589);
                                AddImage(startX + 88, startY + 32, 10552, 2589);
                                AddItem(startX + 15, startY + 18, 3);
                                AddItem(startX + 23, startY + 79, 3618);
                                AddItem(startX + 28, startY + 60, 3619);
                                AddLabel(startX + 48, startY + 5, 2590, "Virtue");
                                AddButton(startX + 74, startY + 46, 9721, 9721, buttonIndex, GumpButtonType.Reply, 0);
                            break;

                            case AchievementCategory.Crafting:
                                AddItem(startX + -10, startY + 41, 4142);
                                AddItem(startX + 1, startY + 41, 4150);
                                AddImage(startX + 60, startY + 32, 10550, 2401);
                                AddImage(startX + 88, startY + 32, 10552, 2401);
                                AddItem(startX + 22, startY + 49, 2920);
                                AddItem(startX + 10, startY + 39, 2921);
                                AddItem(startX + 40, startY + 77, 4148);
                                AddItem(startX + 9, startY + 45, 4189);
                                AddItem(startX + 22, startY + 61, 4179);
                                AddItem(startX + 3, startY + 53, 4139);
                                AddItem(startX + 21, startY + 43, 2581);
                                AddItem(startX + 3, startY + 29, 2503);
                                AddItem(startX + 11, startY + 22, 4172);
                                AddLabel(startX + 36, startY + 5, 2036, "Crafting");
                                AddButton(startX + 74, startY + 46, 9721, 9721, buttonIndex, GumpButtonType.Reply, 0);
                            break;

                            case AchievementCategory.SkillMastery:
                                AddImage(startX + 60, startY + 32, 10550, 2652);
                                AddImage(startX + 88, startY + 32, 10552, 2652);
                                AddItem(startX + 21, startY + 42, 2942);
                                AddItem(startX + -4, startY + 28, 2943);
                                AddItem(startX + 16, startY + 33, 2507);
                                AddItem(startX + 15, startY + 49, 4030);
                                AddItem(startX + 28, startY + 28, 7716);
                                AddItem(startX + -4, startY + 20, 7717, 2652);
                                AddItem(startX + 18, startY + 39, 4031);
                                AddLabel(startX + 21, startY + 5, 2652, "Skill Mastery");
                                AddButton(startX + 74, startY + 46, 9721, 9721, buttonIndex, GumpButtonType.Reply, 0);
                            break;

                            case AchievementCategory.Vice:
                                AddItem(startX + 27, startY + 71, 6872);
                                AddItem(startX + 6, startY + 51, 6873);
                                AddItem(startX + -10, startY + 42, 6874);
                                AddItem(startX + 25, startY + 32, 6875);
                                AddItem(startX + 51, startY + 52, 6876);
                                AddItem(startX + 69, startY + 46, 6877);
                                AddItem(startX + 14, startY + 84, 6880);
                                AddImage(startX + 60, startY + 32, 10550, 1106);
                                AddImage(startX + 88, startY + 32, 10552, 1106);
                                AddItem(startX + -17, startY + 83, 6883);
                                AddLabel(startX + 48, startY + 5, 1106, "Vice");
                                AddButton(startX + 74, startY + 46, 9721, 9721, buttonIndex, GumpButtonType.Reply, 0);
                            break;

                            #region Guild

                            /*
                            case AchievementCategory.Messages:
                                AddLabel(startX + 36, startY + 5, 2526, "Messages");
                                AddImage(startX + 60, startY + 32, 10550, 55);
                                AddImage(startX + 88, startY + 32, 10552, 55);
                                AddButton(startX + 74, startY + 46, 9721, 9721, buttonIndex, GumpButtonType.Reply, 0);
                                AddItem(startX + -1, startY + 18, 7774);
                            break;

                            case AchievementCategory.Overview:
                                AddLabel(startX + 34, startY + 5, 2401, "Overview");
                                AddImage(startX + 60, startY + 32, 10550, 2401);
                                AddImage(startX + 88, startY + 32, 10552, 2401);
                                AddButton(startX + 74, startY + 46, 9721, 9721, buttonIndex, GumpButtonType.Reply, 0);
                                AddImage(startX + 13, startY + 35, 11016);
                            break;

                            case AchievementCategory.Invitations:
                                AddItem(startX + 21, startY + 41, 2942);
                                AddItem(startX + -3, startY + 25, 2943);
                                AddLabel(startX + 12, startY + 5, 2564, "Guild Invitations");
                                AddImage(startX + 60, startY + 32, 10550, 2566);
                                AddImage(startX + 88, startY + 32, 10552, 2566);
                                AddButton(startX + 74, startY + 46, 9721, 9721, buttonIndex, GumpButtonType.Reply, 0);
                                AddItem(startX + 9, startY + 31, 5359);
                                AddItem(startX + 23, startY + 24, 4031);
                                AddItem(startX + 23, startY + 50, 5357);
                                AddItem(startX + 27, startY + 52, 5357);
                            break;

                            case AchievementCategory.Members:
                                AddLabel(startX + 35, startY + 5, 64, "Members");
                                AddItem(startX + 69, startY + 46, 6877);
                                AddImage(startX + 60, startY + 32, 10550, 2551);
                                AddImage(startX + 88, startY + 32, 10552, 2551);
                                AddButton(startX + 74, startY + 46, 9721, 9721, buttonIndex, GumpButtonType.Reply, 0);
                                AddItem(startX + 20, startY + 29, 10905);
                                AddItem(startX + 19, startY + 33, 3746);
                            break;

                            case AchievementCategory.Candidates:
                                AddLabel(startX + 27, startY + 4, 2553, "Candidates");
                                AddImage(startX + 60, startY + 32, 10550, 2566);
                                AddImage(startX + 88, startY + 32, 10552, 2566);
                                AddButton(startX + 74, startY + 45, 9721, 9721, buttonIndex, GumpButtonType.Reply, 0);
                                AddItem(startX + 28, startY + 45, 4647, 2563);
                                AddItem(startX + 11, startY + 36, 4645, 2563);
                                AddItem(startX + -5, startY + 42, 5018, 2563);
                            break;

                            case AchievementCategory.CreateGuild:
                                AddImage(startX + 88, startY + 32, 10552, 2401);
                                AddLabel(startX + 25, startY + 5, 2401, "Create Guild");
                                AddImage(startX + 60, startY + 32, 10550, 2401);
                                AddButton(startX + 74, startY + 46, 9721, 9721, buttonIndex, GumpButtonType.Reply, 0);
                                AddItem(startX + 15, startY + 29, 3796);
                            break;

                            case AchievementCategory.Faction:
                                AddLabel(startX + 44, startY + 5, 1256, "Faction");
                                AddItem(startX + -15, startY + 20, 3936);
                                AddItem(startX + 18, startY + 21, 18194);
                                AddItem(startX + 10, startY + 13, 5129);
                                AddItem(startX + 32, startY + 31, 18196);
                                AddItem(startX + 9, startY + 30, 5050);
                                AddItem(startX + 26, startY + 22, 5135, 2500);
                                AddImage(startX + 60, startY + 32, 10550, 1256);
                                AddImage(startX + 88, startY + 32, 10552, 1256);
                                AddItem(startX + 26, startY + 47, 7034);
                                AddButton(startX + 74, startY + 46, 9721, 2151, buttonIndex, GumpButtonType.Reply, 0);
                                AddItem(startX + -4, startY + 33, 543);
                                AddItem(startX + 14, startY + 45, 543);
                            break;

                            case AchievementCategory.Diplomacy:
                                AddItem(startX + 21, startY + 42, 2942);
                                AddItem(startX + -4, startY + 28, 2943);
                                AddItem(startX + 11, startY + 20, 4031);
                                AddItem(startX + 13, startY + 38, 4030);
                                AddItem(startX + 20, startY + 59, 2507);
                                AddItem(startX + 22, startY + 44, 2459);
                                AddLabel(startX + 35, startY + 5, 2606, "Diplomacy");
                                AddImage(startX + 60, startY + 32, 10550, 2606);
                                AddImage(startX + 88, startY + 32, 10552, 2606);
                                AddButton(startX + 74, startY + 46, 9721, 9721, buttonIndex, GumpButtonType.Reply, 0);
                            break;
                            */

                            #endregion
                        }

                        #endregion

                        //Category Progress
                        int completedAchievementsInCategory = Achievements.GetCompletedCategoryAchievements(m_Player, category);
                        int achievementsInCategory = Achievements.GetCategoryAchievements(category).Count;

                        textHue = GreyTextHue;

                        if (completedAchievementsInCategory > 0)
                            textHue = YellowTextHue;

                        if (completedAchievementsInCategory == achievementsInCategory)
                            textHue = GreenTextHue;

                        double categoryAchievementProgress = (double)completedAchievementsInCategory / (double)achievementsInCategory;

                        string progressText = completedAchievementsInCategory.ToString() + "/" + achievementsInCategory.ToString();

                        AddImage(startX + 10, startY + 99, Achievements.GetProgressBarBackground(totalAchievementProgress));
                        AddImageTiled(startX + 10 + Utility.ProgressBarX(totalAchievementProgress), startY + 102, Utility.ProgressBarWidth(totalAchievementProgress), 7, 2488);
                        AddLabel(Utility.CenteredTextOffset(startX + 60, progressText), startY + 113, textHue, progressText);

                        startX += columnSpacing;
                        columnIndex++;

                        if (columnIndex >= categoryColumns)
                        {
                            columnIndex = 0;
                            rowIndex++;

                            startX = iBaseX;
                            startY += rowSpacing;
                        }
                    }
                break;

                #endregion

                case PageType.Category:
                    AddImage(142, 105, 103);
			        AddImage(143, 148, 103);
			        AddImage(143, 234, 103);
			        AddImage(143, 324, 103);
			        AddImage(145, 409, 103, 2401);
			        AddImage(8, 410, 103, 2401);
			        AddImage(8, 233, 103, 2401);
			        AddImage(142, 8, 103, 2401);
			        AddImage(7, 147, 103, 2401);
			        AddImage(8, 7, 103, 2401);
			        AddImage(8, 104, 103, 2401);
			        AddImage(17, 107, 3604, 2052);
			        AddImage(143, 107, 3604, 2052);
			        AddImage(17, 18, 3604, 2052);
			        AddImage(143, 18, 3604, 2052);
			        
			        AddImage(8, 323, 103, 2401);
			        AddImage(17, 194, 3604, 2052);
			        AddImage(143, 194, 3604, 2052);
			        AddImage(17, 286, 3604, 2052);
			        AddImage(143, 286, 3604, 2052);
			        AddImage(17, 370, 3604, 2052);
			        AddImage(143, 370, 3604, 2052);
			        AddImage(274, 105, 103);
			        AddImage(275, 148, 103);
			        AddImage(275, 234, 103);
			        AddImage(275, 324, 103);
			        AddImage(276, 410, 103, 2401);
			        AddImage(274, 8, 103, 2401);
			        AddImage(406, 105, 103, 2401);
			        AddImage(407, 148, 103, 2401);
			        AddImage(407, 234, 103, 2401);
			        AddImage(407, 324, 103, 2401);
			        AddImage(408, 410, 103, 2401);
			        AddImage(406, 8, 103, 2401);
			        AddImage(267, 107, 3604, 2052);
			        AddImage(267, 18, 3604, 2052);
			        AddImage(267, 194, 3604, 2052);
			        AddImage(267, 286, 3604, 2052);
			        AddImage(267, 370, 3604, 2052);
			        AddImage(375, 107, 3604, 2052);
			        AddImage(375, 18, 3604, 2052);
			        AddImage(375, 194, 3604, 2052);
			        AddImage(375, 286, 3604, 2052);
			        AddImage(375, 370, 3604, 2052);
			        AddImage(409, 107, 3604, 2052);
			        AddImage(409, 18, 3604, 2052);
			        AddImage(409, 194, 3604, 2052);
			        AddImage(409, 286, 3604, 2052);
			        AddImage(409, 370, 3604, 2052);
			        AddImage(144, -2, 1143, 2499);

                    AddLabel(235, 0, Achievements.AchievementTextHue, "Achievements");

                    List<Achievement> m_AchievementsInCategory = Achievements.GetCategoryAchievements(m_AchievementCategory);

                    if (m_AchievementsInCategory.Count == 0)
                        return;

                    if (m_AchievementSelectedIndex >= m_AchievementsInCategory.Count)
                        m_AchievementSelectedIndex = 0;

                    Achievement achievementSelected = m_AchievementsInCategory[m_AchievementSelectedIndex];                    

                    int achievementsPerPage = 12;
                    int totalAchievements = m_AchievementsInCategory.Count;
                    int totalAchievementPages = (int)(Math.Ceiling((double)totalAchievements / (double)achievementsPerPage));

                    if (m_AchievementListPage >= totalAchievementPages)
                        m_AchievementListPage = 0;

                    if (m_AchievementListPage < 0)
                        m_AchievementListPage = 0;

                    int achievementStartIndex = m_AchievementListPage * achievementsPerPage;
                    int achievementEndIndex = (m_AchievementListPage * achievementsPerPage) + (achievementsPerPage - 1);

                    if (achievementEndIndex >= totalAchievements)
                        achievementEndIndex = totalAchievements - 1;
                                
                    int achievementCount = achievementEndIndex - achievementStartIndex;

                    //Guide
                    AddButton(7, 4, 2094, 2095, 1, GumpButtonType.Reply, 0);
			        AddLabel(30, 4, 149, "Guide");

                    //Previous List Page
                    if (m_AchievementListPage > 0)
                        AddButton(69, 470, 9909, 9909, 3, GumpButtonType.Reply, 0); 

                    //Next List Page
                    if (m_AchievementListPage < totalAchievementPages - 1)
                        AddButton(115, 470, 9903, 9903, 4, GumpButtonType.Reply, 0);                    

                    //Return
                    AddButton(243, 471, 4014, 4016, 5, GumpButtonType.Reply, 0);
                    AddLabel(277, 472, WhiteTextHue, "Return");

                    //Category
                    startX = 25;
                    startY = 30;

                    #region Achievement Category Images

                    switch (m_AchievementCategory)
                    {
                        case AchievementCategory.Battle:
                            AddItem(startX + 19, startY + 34, 18210, 2500);
                            AddItem(startX + 21, startY + 33, 5049, 2500);
                            AddItem(startX + 14, startY + 22, 5138, 2500);
                            AddItem(startX + 4, startY + 40, 7028, 2500);
                            AddLabel(startX + 45, startY + 5, 2220, "Battle");
                            AddImage(startX + 60, startY + 32, 10550, 2220);
                            AddImage(startX + 88, startY + 32, 10552, 2220);
                            AddButton(startX + 74, startY + 46, 9721, 9721, 2, GumpButtonType.Reply, 0);
                        break;

                        case AchievementCategory.Adventuring:
                            AddItem(startX + 4, startY + 19, 3226);
                            AddItem(startX + 19, startY + 44, 4967);
                            AddItem(startX + 11, startY + 55, 4970);
                            AddImage(startX + 60, startY + 32, 10550, 149);
                            AddImage(startX + 88, startY + 32, 10552, 149);
                            AddItem(startX + 26, startY + 69, 2648);
                            AddItem(startX + -4, startY + 58, 5356);
                            AddItem(startX + 2, startY + 65, 3922);
                            AddItem(startX + -37, startY + 50, 3898);
                            AddLabel(startX + 27, startY + 5, 148, "Adventuring");
                            AddButton(startX + 74, startY + 46, 9721, 9721, 2, GumpButtonType.Reply, 0);
                        break;

                        case AchievementCategory.Slaying:
                            AddItem(startX + 3, startY + 31, 7433);
                            AddItem(startX + 8, startY + 56, 4655);
                            AddItem(startX + 21, startY + 20, 7438);
                            AddItem(startX + 45, startY + 34, 7419);
                            AddItem(startX + 68, startY + 33, 7418);
                            AddItem(startX + 7, startY + 54, 7782);
                            AddItem(startX + 43, startY + 68, 7430);
                            AddImage(startX + 60, startY + 32, 10550, 2116);
                            AddImage(startX + 88, startY + 32, 10552, 2116);
                            AddItem(startX + 23, startY + 52, 3910);
                            AddLabel(startX + 44, startY + 5, 2116, "Slaying");
                            AddButton(startX + 74, startY + 46, 9721, 9721, 2, GumpButtonType.Reply, 0);
                            break;

                        case AchievementCategory.Seafaring:
                            AddItem(startX + 5, startY + 50, 5363);
                            AddItem(startX + 23, startY + 60, 5365);
                            AddItem(startX + 24, startY + 37, 5370);
                            AddImage(startX + 60, startY + 32, 10550, 2602);
                            AddImage(startX + 88, startY + 32, 10552, 2602);
                            AddLabel(startX + 36, startY + 5, 2603, "Seafaring");
                            AddButton(startX + 74, startY + 46, 9721, 9721, 2, GumpButtonType.Reply, 0);
                            break;

                        case AchievementCategory.Luxury:
                            AddItem(startX + -3, startY + 68, 2448, 2425);
                            AddImage(startX + 60, startY + 32, 10550, 2618);
                            AddImage(startX + 88, startY + 32, 10552, 2618);
                            AddItem(startX + 19, startY + 30, 16508);
                            AddItem(startX + 1, startY + 63, 2459, 2562);
                            AddItem(startX + -8, startY + 63, 2459, 2600);
                            AddItem(startX + -4, startY + 67, 2459, 2606);
                            AddLabel(startX + 43, startY + 5, 2618, "Luxury");
                            AddButton(startX + 74, startY + 46, 9721, 9721, 2, GumpButtonType.Reply, 0);
                            break;

                        case AchievementCategory.Competition:
                            AddItem(startX + 7, startY + 40, 16434);
                            AddItem(startX + 18, startY + 26, 16433);
                            AddImage(startX + 60, startY + 32, 10550, 2963);
                            AddImage(startX + 88, startY + 32, 10552, 2963);
                            AddItem(startX + 8, startY + 35, 4006);
                            AddItem(startX + 0, startY + 48, 4008, 2500);
                            AddItem(startX + 10, startY + 34, 4008, 1107);
                            AddLabel(startX + 26, startY + 5, 2962, "Competition");
                            AddButton(startX + 74, startY + 46, 9721, 9721, 2, GumpButtonType.Reply, 0);
                            break;

                        case AchievementCategory.AnimalTaming:
                            AddImage(startX + 60, startY + 32, 10550, 2208);
                            AddImage(startX + 88, startY + 32, 10552, 2208);
                            AddItem(startX + 13, startY + 49, 2476);
                            AddItem(startX + 10, startY + 40, 3191);
                            AddItem(startX + 14, startY + 38, 3191);
                            AddItem(startX + 13, startY + 39, 3713);
                            AddLabel(startX + 23, startY + 5, 2599, "Animal Taming");
                            AddButton(startX + 74, startY + 46, 9721, 9721, 2, GumpButtonType.Reply, 0);
                            break;

                        case AchievementCategory.Harvesting:
                            AddImage(startX + 88, startY + 32, 10552, 2417);
                            AddItem(startX + 27, startY + 44, 3346, 2208);
                            AddImage(startX + 60, startY + 32, 10550, 2417);
                            AddItem(startX + 6, startY + 33, 3670);
                            AddItem(startX + 36, startY + 29, 3351, 2208);
                            AddItem(startX + -15, startY + 79, 3352, 2208);
                            AddItem(startX + 2, startY + 65, 3344, 2208);
                            AddItem(startX + 18, startY + 62, 7137);
                            AddItem(startX + 27, startY + 57, 3908);
                            AddItem(startX + 28, startY + 71, 2482, 2500);
                            AddLabel(startX + 28, startY + 5, 2417, "Harvesting");
                            AddButton(startX + 74, startY + 46, 9721, 9721, 2, GumpButtonType.Reply, 0);
                            break;

                        case AchievementCategory.Virtue:
                            AddItem(startX + -5, startY + 16, 2);
                            AddImage(startX + 60, startY + 32, 10550, 2589);
                            AddImage(startX + 88, startY + 32, 10552, 2589);
                            AddItem(startX + 15, startY + 18, 3);
                            AddItem(startX + 23, startY + 79, 3618);
                            AddItem(startX + 28, startY + 60, 3619);
                            AddLabel(startX + 48, startY + 5, 2590, "Virtue");
                            AddButton(startX + 74, startY + 46, 9721, 9721, 2, GumpButtonType.Reply, 0);
                            break;

                        case AchievementCategory.Crafting:
                            AddItem(startX + -10, startY + 41, 4142);
                            AddItem(startX + 1, startY + 41, 4150);
                            AddImage(startX + 60, startY + 32, 10550, 2401);
                            AddImage(startX + 88, startY + 32, 10552, 2401);
                            AddItem(startX + 22, startY + 49, 2920);
                            AddItem(startX + 10, startY + 39, 2921);
                            AddItem(startX + 40, startY + 77, 4148);
                            AddItem(startX + 9, startY + 45, 4189);
                            AddItem(startX + 22, startY + 61, 4179);
                            AddItem(startX + 3, startY + 53, 4139);
                            AddItem(startX + 21, startY + 43, 2581);
                            AddItem(startX + 3, startY + 29, 2503);
                            AddItem(startX + 11, startY + 22, 4172);
                            AddLabel(startX + 36, startY + 5, 2036, "Crafting");
                            AddButton(startX + 74, startY + 46, 9721, 9721, 2, GumpButtonType.Reply, 0);
                            break;

                        case AchievementCategory.SkillMastery:
                            AddImage(startX + 60, startY + 32, 10550, 2652);
                            AddImage(startX + 88, startY + 32, 10552, 2652);
                            AddItem(startX + 21, startY + 42, 2942);
                            AddItem(startX + -4, startY + 28, 2943);
                            AddItem(startX + 16, startY + 33, 2507);
                            AddItem(startX + 15, startY + 49, 4030);
                            AddItem(startX + 28, startY + 28, 7716);
                            AddItem(startX + -4, startY + 20, 7717, 2652);
                            AddItem(startX + 18, startY + 39, 4031);
                            AddLabel(startX + 21, startY + 5, 2652, "Skill Mastery");
                            AddButton(startX + 74, startY + 46, 9721, 9721, 2, GumpButtonType.Reply, 0);
                            break;

                        case AchievementCategory.Vice:
                            AddItem(startX + 27, startY + 71, 6872);
                            AddItem(startX + 6, startY + 51, 6873);
                            AddItem(startX + -10, startY + 42, 6874);
                            AddItem(startX + 25, startY + 32, 6875);
                            AddItem(startX + 51, startY + 52, 6876);
                            AddItem(startX + 69, startY + 46, 6877);
                            AddItem(startX + 14, startY + 84, 6880);
                            AddImage(startX + 60, startY + 32, 10550, 1106);
                            AddImage(startX + 88, startY + 32, 10552, 1106);
                            AddItem(startX + -17, startY + 83, 6883);
                            AddLabel(startX + 48, startY + 5, 1106, "Vice");
                            AddButton(startX + 74, startY + 46, 9721, 9721, 2, GumpButtonType.Reply, 0);
                        break;

                        #region Guild

                        /*
                        case AchievementCategory.Messages:
                            AddLabel(startX + 36, startY + 5, 2526, "Messages");
                            AddImage(startX + 60, startY + 32, 10550, 55);
                            AddImage(startX + 88, startY + 32, 10552, 55);
                            AddButton(startX + 74, startY + 46, 9721, 9721, 2, GumpButtonType.Reply, 0);
                            AddItem(startX + -1, startY + 18, 7774);
                        break;

                        case AchievementCategory.Overview:
                            AddLabel(startX + 34, startY + 5, 2401, "Overview");
                            AddImage(startX + 60, startY + 32, 10550, 2401);
                            AddImage(startX + 88, startY + 32, 10552, 2401);
                            AddButton(startX + 74, startY + 46, 9721, 9721, 2, GumpButtonType.Reply, 0);
                            AddImage(startX + 13, startY + 35, 11016);
                        break;

                        case AchievementCategory.Invitations:
                            AddItem(startX + 21, startY + 41, 2942);
                            AddItem(startX + -3, startY + 25, 2943);
                            AddLabel(startX + 12, startY + 5, 2564, "Guild Invitations");
                            AddImage(startX + 60, startY + 32, 10550, 2566);
                            AddImage(startX + 88, startY + 32, 10552, 2566);
                            AddButton(startX + 74, startY + 46, 9721, 9721, 2, GumpButtonType.Reply, 0);
                            AddItem(startX + 9, startY + 31, 5359);
                            AddItem(startX + 23, startY + 24, 4031);
                            AddItem(startX + 23, startY + 50, 5357);
                            AddItem(startX + 27, startY + 52, 5357);
                        break;

                        case AchievementCategory.Members:
                            AddLabel(startX + 35, startY + 5, 64, "Members");
                            AddItem(startX + 69, startY + 46, 6877);
                            AddImage(startX + 60, startY + 32, 10550, 2551);
                            AddImage(startX + 88, startY + 32, 10552, 2551);
                            AddButton(startX + 74, startY + 46, 9721, 9721, 2, GumpButtonType.Reply, 0);
                            AddItem(startX + 20, startY + 29, 10905);
                            AddItem(startX + 19, startY + 33, 3746);
                        break;

                        case AchievementCategory.Candidates:
                            AddLabel(startX + 27, startY + 4, 2553, "Candidates");
                            AddImage(startX + 60, startY + 32, 10550, 2566);
                            AddImage(startX + 88, startY + 32, 10552, 2566);
                            AddButton(startX + 74, startY + 45, 9721, 9721, 2, GumpButtonType.Reply, 0);
                            AddItem(startX + 28, startY + 45, 4647, 2563);
                            AddItem(startX + 11, startY + 36, 4645, 2563);
                            AddItem(startX + -5, startY + 42, 5018, 2563);
                        break;

                        case AchievementCategory.CreateGuild:
                            AddImage(startX + 88, startY + 32, 10552, 2401);
                            AddLabel(startX + 25, startY + 5, 2401, "Create Guild");
                            AddImage(startX + 60, startY + 32, 10550, 2401);
                            AddButton(startX + 74, startY + 46, 9721, 9721, 2, GumpButtonType.Reply, 0);
                            AddItem(startX + 15, startY + 29, 3796);
                        break;

                        case AchievementCategory.Faction:
                            AddLabel(startX + 44, startY + 5, 1256, "Faction");
                            AddItem(startX + -15, startY + 20, 3936);
                            AddItem(startX + 18, startY + 21, 18194);
                            AddItem(startX + 10, startY + 13, 5129);
                            AddItem(startX + 32, startY + 31, 18196);
                            AddItem(startX + 9, startY + 30, 5050);
                            AddItem(startX + 26, startY + 22, 5135, 2500);
                            AddImage(startX + 60, startY + 32, 10550, 1256);
                            AddImage(startX + 88, startY + 32, 10552, 1256);
                            AddItem(startX + 26, startY + 47, 7034);
                            AddButton(startX + 74, startY + 46, 9721, 2151, 2, GumpButtonType.Reply, 0);
                            AddItem(startX + -4, startY + 33, 543);
                            AddItem(startX + 14, startY + 45, 543);
                        break;

                        case AchievementCategory.Diplomacy:
                            AddItem(startX + 21, startY + 42, 2942);
                            AddItem(startX + -4, startY + 28, 2943);
                            AddItem(startX + 11, startY + 20, 4031);
                            AddItem(startX + 13, startY + 38, 4030);
                            AddItem(startX + 20, startY + 59, 2507);
                            AddItem(startX + 22, startY + 44, 2459);
                            AddLabel(startX + 35, startY + 5, 2606, "Diplomacy");
                            AddImage(startX + 60, startY + 32, 10550, 2606);
                            AddImage(startX + 88, startY + 32, 10552, 2606);
                            AddButton(startX + 74, startY + 46, 9721, 9721, 2, GumpButtonType.Reply, 0);
                        break;  
                        */

                        #endregion
                    }

                    #endregion                    

                    int playerCompletedAchievementsInCategory = Achievements.GetCompletedCategoryAchievements(m_Player, m_AchievementCategory);
                    int totalAchievementsInCategory = m_AchievementsInCategory.Count;

                    textHue = GreyTextHue;

                    if (playerCompletedAchievementsInCategory > 0)
                        textHue = YellowTextHue;

                    if (playerCompletedAchievementsInCategory == m_AchievementsInCategory.Count)
                        textHue = GreenTextHue;

                    double playerCategoryAchievementProgress = (double)playerCompletedAchievementsInCategory / (double)totalAchievementsInCategory;

                    string categoryProgressText = playerCompletedAchievementsInCategory.ToString() + "/" + totalAchievementsInCategory.ToString();

                    AddImage(startX + 10, startY + 99, Achievements.GetProgressBarBackground(playerCategoryAchievementProgress));
                    AddImageTiled(startX + 10 + Utility.ProgressBarX(playerCategoryAchievementProgress), startY + 102, Utility.ProgressBarWidth(playerCategoryAchievementProgress), 7, 2488);
                    AddLabel(Utility.CenteredTextOffset(startX + 60, categoryProgressText), startY + 113, textHue, categoryProgressText);

                    startX = 3;
                    startY = 165;

                    int achievementSpacing = 25;

                    for (int a = 0; a < achievementCount + 1; a++)
                    {
                        int achievementIndex = achievementStartIndex + a;
                        int buttonIndex = 10 + achievementIndex;

                        if (achievementStartIndex >= totalAchievements)
                            continue;

                        Achievement achievement = m_AchievementsInCategory[achievementIndex];

                        AchievementDetail achievementDetail = Achievements.GetAchievementDetail(achievement);
                        AchievementEntry achievementEntry = Achievements.GetAchievementEntry(m_Player, achievement);

                        if (achievementDetail != null && achievementEntry != null)
                        {
                            textHue = WhiteTextHue;
                            string completionPercentageText = "0%";
                            
                            if (achievementEntry.m_Completed)
                            {
                                textHue = GreenTextHue;

                                if (!achievementEntry.m_Claimed)
                                    AddItem(startX, startY + 5, 572); //Green Orb

                                completionPercentageText = "100%";                                
                            }

                            else
                            {
                                double completionPercentage = (double)achievementEntry.m_Progress / (double)achievementDetail.m_ProgressNeeded;

                                completionPercentageText = Utility.CreateDecimalPercentageString(completionPercentage, 0);

                                if (completionPercentage > 0 && completionPercentage < .01)
                                    completionPercentageText = "1%";

                                if (completionPercentage >= .99 && completionPercentage < 1.0)
                                    completionPercentageText = "99%";

                                if (completionPercentage > 0)
                                    textHue = YellowTextHue;
                            }

                            if (!achievementEntry.m_Unlocked)
                            {
                                textHue = GreyTextHue;
                                AddItem(startX, startY + 5, 573); //Orange Orb
                            }

                            if (achievementSelected == achievement)
                                AddButton(startX + 33, startY, 4030, 4030, buttonIndex, GumpButtonType.Reply, 0);
                            else
                                AddButton(startX + 33, startY, 4029, 4031, buttonIndex, GumpButtonType.Reply, 0);

                            AddLabel(startX + 68, startY + 1, textHue, completionPercentageText);
                            AddLabel(startX + 110, startY + 1, textHue, achievementDetail.m_DisplayName);
                        }

                        startY += achievementSpacing;
                    }

                    //Achivement Window
                    AddImage(383, 233, 103, 2401);
			        AddImage(383, 150, 103, 2401);
			        AddImage(383, 60, 103, 2401);
			        AddImage(281, 150, 103);
			        AddImage(278, 234, 103, 2401);
			        AddImage(278, 150, 103, 2401);
			        AddImage(278, 60, 103, 2401);
			        AddImage(291, 72, 3604, 2052);
			        AddImage(291, 195, 3604, 2052);
			        AddImage(385, 72, 3604, 2052);
			        AddImage(386, 195, 3604, 2052);
			        AddImage(383, 322, 103, 2401);
			        AddImage(278, 323, 103, 2401);
			        AddImage(291, 284, 3604, 2052);
			        AddImage(386, 284, 3604, 2052);
			        AddImage(383, 362, 103, 2401);
			        AddImage(278, 363, 103, 2401);
			        AddImage(291, 324, 3604, 2052);
			        AddImage(386, 324, 3604, 2052);
                    
                    AchievementDetail selectedAchievementDetail = Achievements.GetAchievementDetail(achievementSelected);
                    AchievementEntry selectedAchievementEntry = Achievements.GetAchievementEntry(m_Player, achievementSelected);

                    int centerTextX = 400;

                    if (selectedAchievementDetail != null && selectedAchievementEntry != null)
                    {
                        AddLabel(Utility.CenteredTextOffset(centerTextX + 5, selectedAchievementDetail.m_DisplayName), 73, 2603, selectedAchievementDetail.m_DisplayName);

                        if (!selectedAchievementEntry.m_Unlocked)
                        {
                            string unlockedText = "Stage " + selectedAchievementDetail.m_Stage.ToString() + " (Not Yet Unlocked)";

                            AddLabel(Utility.CenteredTextOffset(centerTextX + 10, unlockedText), 93, 149, unlockedText);
                        }

                        int selectedAchievementDescriptionY = 113;

                        if (selectedAchievementDetail.m_Description != null)
                        {
                            for (int a = 0; a < selectedAchievementDetail.m_Description.Length; a++)
                            {
                                AddLabel(Utility.CenteredTextOffset(centerTextX + 15, selectedAchievementDetail.m_Description[a]), selectedAchievementDescriptionY, WhiteTextHue, selectedAchievementDetail.m_Description[a]);

                                selectedAchievementDescriptionY += 20;
                            }
                        }

                        AddLabel(373, 184, 2599, "Progress");                        

                        double selectedAchievementProgress = (double)selectedAchievementEntry.m_Progress / (double)selectedAchievementDetail.m_ProgressNeeded;
                        
                        textHue = GreyTextHue;

                        if (selectedAchievementEntry.m_Progress > 0)
                            textHue = YellowTextHue;

                        if (selectedAchievementEntry.m_Progress == selectedAchievementDetail.m_ProgressNeeded)
                            textHue = GreenTextHue;

                        AddImage(345, 205, Achievements.GetProgressBarBackground(selectedAchievementProgress));
                        AddImageTiled(345 + Utility.ProgressBarX(selectedAchievementProgress), 208, Utility.ProgressBarWidth(selectedAchievementProgress), 7, 2488);

                        string progressText = selectedAchievementEntry.m_Progress.ToString() + "/" + selectedAchievementDetail.m_ProgressNeeded.ToString();

                        AddLabel(Utility.CenteredTextOffset(centerTextX, progressText), 219, textHue, progressText);

                        AddLabel(375, 253, 63, "Reward");
                        AddLabel(Utility.CenteredTextOffset(centerTextX, selectedAchievementDetail.m_RewardName), 273, WhiteTextHue, selectedAchievementDetail.m_RewardName);

                        AddItem(355 + selectedAchievementDetail.m_RewardItemOffsetX, 316 + selectedAchievementDetail.m_RewardItemOffsetY, selectedAchievementDetail.m_RewardItemID, selectedAchievementDetail.m_RewardItemHue);

                        if (selectedAchievementEntry.m_Completed)     
                        {
                            if (selectedAchievementEntry.m_Claimed)
                            {
                                AddLabel(418, 420, 63, "Claim");
                                AddButton(384, 416, 2151, 2151, 6, GumpButtonType.Reply, 0);
                            }

                            else
                                AddLabel(413, 420, 149, "Claimed");
                        }
                    }
                break;

                case PageType.Settings:
                break;
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null)
                return;

            bool closeGump = true;
            
            switch (m_SelectedPageType)
            {
                case PageType.Main:
                    int categoriesPerPage = categoryRows * categoryColumns;
                    int totalCategories = Enum.GetNames(typeof(AchievementCategory)).Length;
                    int totalPages = (int)(Math.Ceiling((double)totalCategories / (double)categoriesPerPage));

                    if (m_MainPage >= totalPages)
                        m_MainPage = 0;

                    if (m_MainPage < 0)
                        m_MainPage = 0;

                    int categoryStartIndex = m_MainPage * categoriesPerPage;
                    int categoryEndIndex = (m_MainPage * categoriesPerPage) + (categoriesPerPage - 1);

                    if (categoryEndIndex >= totalCategories)
                        categoryEndIndex = totalCategories - 1;

                    switch (info.ButtonID)
                    {
                        //Guide
                        case 1:
                            closeGump = false;
                        break;

                        //Previous Page
                        case 2:
                            if (m_MainPage > 0)
                                m_MainPage--;

                            closeGump = false;
                        break;

                        //Next Page
                        case 3:
                            if (m_MainPage < totalPages - 1)
                                m_MainPage++;

                            closeGump = false;
                        break;

                        //Settings
                        case 4:
                            m_SelectedPageType = PageType.Settings;

                            closeGump = false;
                        break;
                    }

                    //Categories
                    if (info.ButtonID >= 10)
                    {
                        int categorySelectionIndex = info.ButtonID - 10;

                        if (categorySelectionIndex >= totalCategories)
                            categorySelectionIndex = 0;

                        m_SelectedPageType = PageType.Category;

                        m_AchievementCategory = (AchievementCategory)categorySelectionIndex;
                        m_AchievementListPage = 0;
                        m_AchievementSelectedIndex = 0;

                        closeGump = false;
                    }
                break;

                case PageType.Category:
                    List<Achievement> m_AchievementsInCategory = Achievements.GetCategoryAchievements(m_AchievementCategory);

                    if (m_AchievementsInCategory.Count == 0)
                        return;

                    if (m_AchievementSelectedIndex >= m_AchievementsInCategory.Count)
                        m_AchievementSelectedIndex = 0;

                    Achievement achievementSelected = m_AchievementsInCategory[m_AchievementSelectedIndex];                    

                    int achievementsPerPage = 12;
                    int totalAchievements = m_AchievementsInCategory.Count;
                    int totalAchievementPages = (int)(Math.Ceiling((double)totalAchievements / (double)achievementsPerPage));

                    if (m_AchievementListPage >= totalAchievementPages)
                        m_AchievementListPage = 0;

                    if (m_AchievementListPage < 0)
                        m_AchievementListPage = 0;

                    int achievementStartIndex = m_AchievementListPage * achievementsPerPage;
                    int achievementEndIndex = (m_AchievementListPage * achievementsPerPage) + (achievementsPerPage - 1);

                    if (achievementEndIndex >= totalAchievements)
                        achievementEndIndex = totalAchievements - 1;
                                
                    int achievementCount = achievementEndIndex - achievementStartIndex;

                    AchievementDetail selectedAchievementDetail = Achievements.GetAchievementDetail(achievementSelected);
                    AchievementEntry selectedAchievementEntry = Achievements.GetAchievementEntry(m_Player, achievementSelected);                    

                    switch (info.ButtonID)
                    {
                        //Guide
                        case 1:
                            closeGump = false;
                        break;

                        //Category Button
                        case 2:
                            closeGump = false;
                        break;

                        //Previous List Page
                        case 3:
                            if (m_AchievementListPage > 0)
                                m_AchievementListPage--;

                            closeGump = false;
                        break;

                        //Next List Page
                        case 4:
                            if (m_AchievementListPage < totalAchievementPages - 1)
                                m_AchievementListPage++;

                            closeGump = false;
                        break;

                        //Return
                        case 5:
                            m_SelectedPageType = PageType.Main;

                            closeGump = false;
                        break;

                        //Claim Reward
                        case 6:
                            if (selectedAchievementDetail != null && selectedAchievementEntry != null)
                            {
                                if (selectedAchievementEntry.m_Completed && !selectedAchievementEntry.m_Claimed)                                
                                    Achievements.ClaimAchievement(m_Player, achievementSelected);
                            }

                            closeGump = false;
                        break;
                    }

                    //Achievement Selection
                    if (info.ButtonID >= 10)
                    {
                        int achievementSelectionIndex = info.ButtonID - 10;

                        if (achievementSelectionIndex >= totalAchievements)
                            achievementSelectionIndex = 0;

                        m_AchievementSelectedIndex = achievementSelectionIndex;

                        Achievement testAchievement = m_AchievementsInCategory[achievementSelectionIndex];    

                        closeGump = false;
                    }
                break;

                case PageType.Settings:
                    switch (info.ButtonID)
                    {
                        //Guide
                        case 1:
                            closeGump = false;
                        break;

                       //Return
                        case 2:
                            m_SelectedPageType = PageType.Main;

                            closeGump = false;
                        break;

                        //On Login Show Unclaimed Achievements
                        case 3:
                            m_Player.m_AchievementAccountEntry.OnLoginShowUnclaimedRewards = !m_Player.m_AchievementAccountEntry.OnLoginShowUnclaimedRewards;

                            if (m_Player.m_AchievementAccountEntry.OnLoginShowUnclaimedRewards)
                                m_Player.SendMessage("You will now be notified upon login of unclaimed achievement rewards.");

                            else
                                m_Player.SendMessage("You will no longer be notified of unclaimed achievement rewards.");

                            closeGump = false;
                        break;

                        //Announce Completed Achievements to Guild Members
                        case 4:
                        m_Player.m_AchievementAccountEntry.AnnounceAchievementsToGuildMembers = !m_Player.m_AchievementAccountEntry.AnnounceAchievementsToGuildMembers;

                            if (m_Player.m_AchievementAccountEntry.AnnounceAchievementsToGuildMembers)
                                m_Player.SendMessage("Your completion of achievements will now be announced to fellow guildmembers.");

                            else
                                m_Player.SendMessage("Your completion of achievements will now be kept private from other guildmembers.");

                            closeGump = false;
                        break;

                        //Announce Completed Achievements to Non-Guild Members
                        case 5:
                            m_Player.m_AchievementAccountEntry.AnnounceAchievementsToNonGuildMembers = !m_Player.m_AchievementAccountEntry.AnnounceAchievementsToNonGuildMembers;

                            if (m_Player.m_AchievementAccountEntry.AnnounceAchievementsToNonGuildMembers)
                                m_Player.SendMessage("Your completion of achievements will now be announced to non-guild members.");

                            else
                                m_Player.SendMessage("Your completion of achievements will now be kept private from non-guild members.");

                            closeGump = false;
                        break;

                        //Show Achievements Completed by Other Guild Members
                        case 6:
                            m_Player.m_AchievementAccountEntry.ShowGuildMemberAchievementAnnoucements = !m_Player.m_AchievementAccountEntry.ShowGuildMemberAchievementAnnoucements;

                            if (m_Player.m_AchievementAccountEntry.ShowGuildMemberAchievementAnnoucements)
                                m_Player.SendMessage("You will now be notified when guildmembers complete achievements.");

                            else
                                m_Player.SendMessage("You will no longer be notified when guildmembers complete achievements.");

                            closeGump = false;
                        break;

                        //Show Achievements Completed by Non-Guild Members
                        case 7:
                        m_Player.m_AchievementAccountEntry.ShowNonGuildMemberAchievementAnnoucements = !m_Player.m_AchievementAccountEntry.ShowNonGuildMemberAchievementAnnoucements;

                            if (m_Player.m_AchievementAccountEntry.ShowNonGuildMemberAchievementAnnoucements)
                                m_Player.SendMessage("You will now be notified when non-guild members complete achievements.");

                            else
                                m_Player.SendMessage("You will no longer be notified when non-guild members complete achievements.");

                            closeGump = false;
                        break;

                        //On Completed Achievement Perform Audio
                        case 8:
                            m_Player.m_AchievementAccountEntry.AudioEnabled = !m_Player.m_AchievementAccountEntry.AudioEnabled;

                            if (m_Player.m_AchievementAccountEntry.AudioEnabled)
                                m_Player.SendMessage("Audio is now enabled for your completion of achievements.");

                            else
                                m_Player.SendMessage("Audio is now disabled for your completion of achievements.");

                            closeGump = false;
                        break;

                        //On Completed Achievement Display Pop-Up
                        case 9:
                            m_Player.m_AchievementAccountEntry.PopupEnabled = !m_Player.m_AchievementAccountEntry.PopupEnabled;

                            if (m_Player.m_AchievementAccountEntry.PopupEnabled)
                                m_Player.SendMessage("Pop-Ups will display for your completion of achievements.");

                            else
                                m_Player.SendMessage("Pop-Ups will no longer display for your completion of achievements.");

                            closeGump = false;
                        break;
                    }
                break;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(AchievementsGump));
                m_Player.SendGump(new AchievementsGump(m_Player, m_SelectedPageType, m_MainPage, m_AchievementCategory, m_AchievementListPage, m_AchievementSelectedIndex));
            }
        }
    }
}