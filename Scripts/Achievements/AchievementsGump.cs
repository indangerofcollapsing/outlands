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
        public AchievementCategory m_AchievementCategory = AchievementCategory.Warfare;
        public int m_AchievementListPage = 0;
        public int m_AchievementSelected = 0;         

        public AchievementsGump(Mobile from, PageType selectedPageType, int mainPage, AchievementCategory achievementCategory, int achievementListPage, int achievementSelected): base(10, 10)
        {
            m_Player = from as PlayerMobile;

            if (m_Player == null)
                return;

            m_SelectedPageType = selectedPageType;
            m_MainPage = mainPage;
            m_AchievementCategory = achievementCategory;
            m_AchievementListPage = achievementListPage;
            m_AchievementSelected = achievementSelected;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int WhiteTextHue = 2036;

            int startX = 0;
            int startY = 0;

            AddPage(0);

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

                    AddLabel(235, 0, 2606, "Achievements");    
                   
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
                    if (m_MainPage < totalPages - 1)
                    {
                        AddButton(502, 471, 4005, 4019, 3, GumpButtonType.Reply, 0);
                        AddLabel(432, 472, WhiteTextHue, "Next Page");
                    }

                    //Settings
                    AddButton(243, 471, 4029, 4019, 4, GumpButtonType.Reply, 0);
                    AddLabel(277, 472, 63, "Settings");

                    //Progress
                    AddImage(216, 446, 2057);
                    AddLabel(332, 441, WhiteTextHue, "500/1000");
                    AddImageTiled(283, 449, 39, 7, 2488);
                    AddLabel(175, 440, 149, "Total");                    

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
                            case AchievementCategory.Warfare:                               
                                AddItem(startX + 19, startY + 34, 18210, 2500);
                                AddItem(startX + 21, startY + 33, 5049, 2500);
                                AddItem(startX + 14, startY + 22, 5138, 2500);
                                AddItem(startX + 4, startY + 40, 7028, 2500);
                                AddImage(startX + 60, startY + 32, 10550, 1256);
                                AddImage(startX + 88, startY + 32, 10552, 1256);
                                AddLabel(startX + 41, startY + 5, 1256, "Warfare");
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
                        }

                        #endregion

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

                    AddLabel(235, 0, 2606, "Achievements");

                    //Guide
                    AddButton(7, 4, 2094, 2095, 1, GumpButtonType.Reply, 0);
			        AddLabel(30, 4, 149, "Guide");

                    //Previous List Page
                    AddButton(69, 470, 9909, 2151, 3, GumpButtonType.Reply, 0); 

                    //Next List Page
                    AddButton(115, 470, 9903, 2151, 4, GumpButtonType.Reply, 0);

                    //Return
                    AddButton(243, 471, 4014, 4019, 5, GumpButtonType.Reply, 0);
                    AddLabel(277, 472, WhiteTextHue, "Return");

                    //Category
                    startX = 25;
                    startY = 30;

                    #region Achievement Category Images

                    switch (m_AchievementCategory)
                    {
                        case AchievementCategory.Warfare:
                            AddItem(startX + 19, startY + 34, 18210, 2500);
                            AddItem(startX + 21, startY + 33, 5049, 2500);
                            AddItem(startX + 14, startY + 22, 5138, 2500);
                            AddItem(startX + 4, startY + 40, 7028, 2500);
                            AddImage(startX + 60, startY + 32, 10550, 1256);
                            AddImage(startX + 88, startY + 32, 10552, 1256);
                            AddLabel(startX + 41, startY + 5, 1256, "Warfare");
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
                    }

                    #endregion

                    //Achievement List
                    AddItem(3, 170, 572); //Green Orb
                    AddButton(36, 165, 4030, 4029, 0, GumpButtonType.Reply, 0);
                    AddLabel(71, 166, 63, "100%");
			        AddLabel(113, 166, 63, "Sink or Swim");

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

                    AddLabel(356, 73, 2603, "Walk the Plank");
                    AddLabel(320, 93, 149, "Stage 2 (Not Yet Unlocked)");
			        AddLabel(305, 113, WhiteTextHue, "Kill 1000 enemy ship combatants");
                    AddLabel(400, 133, WhiteTextHue, "-");
                    AddLabel(400, 153, WhiteTextHue, "-");
			        AddLabel(373, 184, 2599, "Progress");
                    AddLabel(375, 219, WhiteTextHue, "0/1000");
			        AddLabel(375, 253, 63, "Reward");
                    AddLabel(367, 273, WhiteTextHue, "Driftwood");
			        AddItem(355, 316, 3387);			        
			        AddImage(346, 205, 2053);
			        AddImageTiled(360, 208, 92, 7, 2488);

                    AddLabel(418, 420, 63, "Claim");
			        AddButton(384, 416, 2151, 2151, 6, GumpButtonType.Reply, 0);
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
                        int categorySelected = (m_MainPage * categoriesPerPage) + categorySelectionIndex;

                        if (categorySelected >= totalCategories)
                            categorySelected = 0;

                        m_SelectedPageType = PageType.Category;

                        m_AchievementCategory = (AchievementCategory)categorySelected;
                        m_AchievementListPage = 0;
                        m_AchievementSelected = 0;
                    }

                    closeGump = false;
                break;

                case PageType.Category:
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
                            closeGump = false;
                        break;

                        //Next List Page
                        case 4:
                            closeGump = false;
                        break;

                        //Return
                        case 5:
                            m_SelectedPageType = PageType.Main;

                            closeGump = false;
                        break;

                        //Claim Reward
                        case 6:
                            closeGump = false;
                        break;
                    }

                    //Achievement Selection
                    if (info.ButtonID >= 10)
                    {
                    }
                break;

                case PageType.Settings:
                break;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(AchievementsGump));
                m_Player.SendGump(new AchievementsGump(m_Player, m_SelectedPageType, m_MainPage, m_AchievementCategory, m_AchievementListPage, m_AchievementSelected));
            }
        }
    }
}