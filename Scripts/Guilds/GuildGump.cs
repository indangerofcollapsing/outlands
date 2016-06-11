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
    public enum GuildGumpPage
    {
        None,
        CreateGuild,
        Invitations,
        Overview,
        Members,
        Candidates,
        Diplomacy
    }
        
    #region Create Guild

    public class CreateGuildGump : Gump
    {
        public PlayerMobile m_Player;

        public string GuildName = "Guild Name";
        public string GuildAbbreviation = "ABC";
        public int GuildSymbolIcon = 2500;
        public int GuildSymbolHue = 0;

        public CreateGuildGump(Mobile from): base(10, 10)
        {
            m_Player = from as PlayerMobile;

            if (m_Player == null) 
                return;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int WhiteTextHue = 2655;
            int GreenTextHue = 63;
            int YellowTextHue = 2550;
            int GreyTextHue = 2401;

            #region Background

            AddImage(132, 193, 103, 2499);
            AddImage(131, 98, 103, 2499);
            AddImage(132, 288, 103, 2499);
            AddImage(528, 369, 103, 2499);
            AddImage(395, 369, 103, 2499);
            AddImage(265, 369, 103, 2499);
            AddImage(132, 369, 103, 2499);
            AddImage(528, 3, 103, 2499);
            AddImage(528, 98, 103, 2499);
            AddImage(528, 193, 103, 2499);
            AddImage(527, 288, 103, 2499);
            AddImage(132, 3, 103, 2499);
            AddImage(265, 3, 103, 2499);
            AddImage(395, 3, 103, 2499);
            AddImage(395, 98, 103, 2499);
            AddImage(395, 193, 103, 2499);
            AddImage(394, 288, 103, 2499);
            AddImage(265, 98, 103, 2499);
            AddImage(265, 193, 103, 2499);
            AddImage(264, 288, 103, 2499);
            AddImage(3, 194, 103, 2499);
            AddImage(3, 272, 103, 2499);
            AddImage(3, 369, 103, 2499);
            AddImage(2, 3, 103, 2499);
            AddImage(2, 97, 103, 2499);
            AddImage(10, 15, 3604, 2052);
            AddImage(10, 108, 3604, 2052);
            AddImage(10, 213, 3604, 2052);
            AddImage(10, 330, 3604, 2052);           
            AddImage(147, 15, 3604, 2052);
            AddImage(147, 108, 3604, 2052);
            AddImage(147, 213, 3604, 2052);
            AddImage(147, 330, 3604, 2052);
            AddImage(286, 15, 3604, 2052);
            AddImage(286, 108, 3604, 2052);
            AddImage(286, 213, 3604, 2052);
            AddImage(286, 330, 3604, 2052);
            AddImage(411, 15, 3604, 2052);
            AddImage(411, 108, 3604, 2052);
            AddImage(411, 213, 3604, 2052);
            AddImage(411, 330, 3604, 2052);
            AddImage(531, 15, 3604, 2052);
            AddImage(531, 108, 3604, 2052);
            AddImage(531, 213, 3604, 2052);
            AddImage(531, 330, 3604, 2052);
            AddImage(184, 15, 3604, 2052);
            AddImage(184, 108, 3604, 2052);
            AddImage(184, 213, 3604, 2052);
            AddImage(184, 330, 3604, 2052);

            #endregion
            
            AddButton(3, 3, 2094, 2095, 1, GumpButtonType.Reply, 0);
            AddLabel(24, -2, 149, "Guide");

            AddButton(65, 22, 9900, 9900, 2, GumpButtonType.Reply, 0);
            AddButton(65, 432, 9906, 9906, 3, GumpButtonType.Reply, 0);

            //-----

            #region Images

            AddImage(326, 116, 2328, 0);
            AddImage(159, 165, 9809, 0);
            AddItem(485, 115, 1928);
            AddItem(507, 136, 1928);
            AddItem(529, 158, 1928);
            AddItem(551, 180, 1928);
            AddItem(573, 202, 1928);
            AddItem(589, 219, 1928);
            AddItem(466, 134, 1928);
            AddItem(488, 155, 1928);
            AddItem(510, 177, 1928);
            AddItem(532, 199, 1928);
            AddItem(555, 220, 1928);
            AddItem(571, 237, 1928);
            AddItem(445, 154, 1928);
            AddItem(467, 175, 1928);
            AddItem(489, 197, 1928);
            AddItem(511, 219, 1928);
            AddItem(534, 240, 1928);
            AddItem(550, 257, 1928);
            AddItem(424, 176, 1928);
            AddItem(446, 197, 1928);
            AddItem(468, 219, 1928);
            AddItem(490, 241, 1928);
            AddItem(513, 262, 1928);
            AddItem(529, 279, 1928);
            AddItem(606, 235, 1930);
            AddItem(586, 255, 1930);
            AddItem(564, 276, 1930);
            AddItem(544, 295, 1930);
            AddItem(403, 198, 1929);
            AddItem(422, 217, 1929);
            AddItem(443, 238, 1929);
            AddItem(462, 256, 1929);
            AddItem(482, 275, 1929);
            AddItem(504, 295, 1929);
            AddItem(524, 315, 1934);           
            AddLabel(212, 86, 149, "Guild Abbreviation");
            AddLabel(212, 46, 149, "Guild Name");
            AddImage(286, 44, 1141, 2400);
            AddImage(335, 85, 2444, 2401);
            AddItem(163, 43, 3796);            
            AddLabel(212, 134, 149, "Guild Symbol");
            AddItem(156, 123, 7776);           
            AddLabel(369, 16, 149, "Create Guild");
            AddLabel(319, 406, 63, "Confirm and Create Guild");           
            AddItem(160, 187, 3823);
            AddLabel(212, 185, 149, "Registration Fee");            
            AddItem(470, 146, 6918);
            AddItem(497, 267, 6914);
            AddItem(550, 202, 6916);
            AddItem(516, 291, 6920);
            AddItem(577, 289, 6915);
            AddItem(555, 214, 6917);
            AddItem(523, 344, 6918);
            AddItem(464, 279, 6913);
            AddItem(399, 229, 6916);
            AddItem(486, 158, 6916);
            AddItem(499, 161, 16645);
            AddItem(497, 193, 3823);
            AddItem(592, 232, 6913);
            AddItem(527, 207, 6918);
            AddItem(504, 113, 17074);
            AddItem(412, 170, 633, 2419);
            AddItem(585, 198, 635, 2419);
            AddItem(429, 142, 636, 2419);
            AddItem(604, 153, 16119);
            AddItem(563, 264, 634, 2419);
            AddItem(547, 274, 633, 2419);
            AddItem(592, 252, 6915);
            AddItem(471, 341, 3791);
            AddItem(463, 154, 3786);
            AddItem(378, 214, 3792);
            AddItem(609, 197, 3814);
            AddItem(562, 236, 3790);
            AddItem(442, 144, 3812);
            AddItem(243, 303, 3816);
            AddItem(265, 320, 3808);
            AddItem(276, 275, 3816);
            AddItem(298, 292, 3808);
            AddItem(243, 303, 3816);
            AddItem(265, 320, 3808);
            AddItem(308, 243, 3816);
            AddItem(331, 259, 3808);
            AddItem(297, 230, 3799);
            AddItem(206, 331, 3816);
            AddItem(228, 346, 3808);
            AddItem(199, 313, 4455);
            AddItem(287, 343, 2322);
            AddItem(312, 293, 2322);
            AddItem(208, 355, 2322);
            AddItem(351, 263, 7685, 2401);
            AddItem(330, 325, 7685, 2401);
            AddItem(252, 360, 7685, 2401);
            AddItem(379, 272, 7684, 2401);
            AddItem(475, 310, 7684, 2401);
            AddItem(231, 388, 7684, 2401);
            AddItem(385, 348, 7684, 2401);
            AddItem(447, 321, 7684, 2401);
            AddItem(421, 321, 7682, 2401);
            AddItem(243, 338, 7684, 1107);
            AddItem(292, 326, 7682, 2401);
            AddItem(469, 254, 7684, 2401);
            AddItem(331, 244, 7681, 2415);
            AddItem(232, 324, 7684, 2401);
            AddItem(269, 291, 7684, 2401);
            AddItem(224, 305, 7684, 2401);
            AddItem(331, 287, 7682, 2401);
            AddItem(365, 300, 7684, 2401);
            AddItem(418, 271, 7684, 2401);
            AddItem(601, 178, 7574);
            AddItem(496, 309, 7570);
            AddItem(240, 271, 4465);
            AddItem(272, 253, 4479);
            AddItem(505, 335, 4651);
            AddItem(441, 258, 6937);
            AddItem(397, 288, 6927);
            AddItem(311, 289, 6925);
            AddItem(348, 341, 6922);
            AddItem(214, 346, 6930);
            AddItem(226, 353, 6938);
            AddItem(276, 353, 3786);
            AddItem(391, 254, 6938);
            AddItem(472, 228, 3793);
            AddItem(295, 371, 7684, 2401);
            AddItem(196, 367, 7685, 2401);
            AddItem(368, 241, 7684, 2401);
            AddItem(368, 324, 7685, 2401);
            AddItem(492, 199, 2507, 2415);
            AddItem(507, 199, 3880);
            AddItem(493, 208, 4234, 2499);
            AddItem(436, 92, 3938);
            AddItem(476, 102, 18236);
            AddItem(470, 96, 5128, 2401);
            AddItem(457, 124, 7032);
            AddItem(549, 150, 18194);

            AddItem(541, 142, 5135, 2500);

            AddItem(530, 152, 5114);
            AddItem(532, 248, 18196);
            AddItem(531, 268, 7034);

            AddItem(522, 240, 5133, 2413);

            AddItem(513, 253, 5178);

            AddItem(442, 211, 18210, 2500);
            AddItem(445, 208, 5049, 2500);
            AddItem(437, 199, 5138, 2500);
            AddItem(426, 221, 7028, 2500);

            AddItem(550, 163, 7030);
            AddItem(345, 312, 6930);
            AddItem(261, 330, 6934);
            AddItem(400, 332, 6924);
            AddItem(424, 312, 6883);
            AddItem(310, 352, 6881);
            AddItem(405, 217, 7684, 2401);
            AddItem(341, 273, 3788);
            AddItem(441, 350, 7685, 2401);
            AddItem(467, 336, 7684, 2401);
            AddItem(518, 298, 17075);
            AddItem(538, 205, 3788);
            AddItem(334, 304, 3788);
            AddItem(369, 259, 3790);
            AddItem(263, 331, 6930);
            AddItem(298, 295, 6938);
            AddItem(276, 294, 6883);
            AddItem(480, 293, 7684, 2401);
            AddItem(405, 235, 7684, 2401);
            AddItem(453, 185, 7684, 2401);
            AddItem(304, 325, 3793);

            #endregion

            AddTextEntry(295, 46, 248, 20, WhiteTextHue, 7, "Guild Name", Guilds.GuildNameCharacterLimit);
            AddTextEntry(355, 86, 47, 20, WhiteTextHue, 8, "ABC", Guilds.GuildNameCharacterLimit);           

            AddButton(300, 137, 2223, 2223, 4, GumpButtonType.Reply, 0);
            AddItem(341, 123, 4014); // Symbol
            AddButton(413, 137, 2224, 2224, 5, GumpButtonType.Reply, 0);
           
            AddLabel(341, 185, GreenTextHue, Utility.CreateCurrencyString(Guilds.GuildRegistrationFee));

            AddButton(366, 429, 247, 249, 6, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null)
                return;

            bool closeGump = true;

            switch (info.ButtonID)
            {
                //Guide
                case 1:
                break;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(CreateGuildGump));
                m_Player.SendGump(new CreateGuildGump(m_Player));
            }
        }
    }

    #endregion

    #region Guild Invitation

    public class GuildInvitationsGump : Gump
    {
        public enum SortColumn
        {
            GuildName,
            Expiration
        }

        public PlayerMobile m_Player;

        public string m_SearchText = "Guild Name";
        public int m_SearchIndex = 0;

        public SortColumn m_SortColumn = SortColumn.GuildName;
        public bool m_SortDescending = true;

        public int m_Page = 0;

        public GuildInvitationsGump(Mobile from): base(10, 10)
        {
            m_Player = from as PlayerMobile;

            if (m_Player == null)
                return;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int WhiteTextHue = 2655;
            int GreenTextHue = 63;
            int YellowTextHue = 2550;
            int GreyTextHue = 2401;

            #region Background

            AddImage(132, 193, 103, 2499);
            AddImage(131, 98, 103, 2499);
            AddImage(132, 288, 103, 2499);
            AddImage(528, 369, 103, 2499);
            AddImage(395, 369, 103, 2499);
            AddImage(265, 369, 103, 2499);
            AddImage(132, 369, 103, 2499);
            AddImage(528, 3, 103, 2499);
            AddImage(528, 98, 103, 2499);
            AddImage(528, 193, 103, 2499);
            AddImage(527, 288, 103, 2499);
            AddImage(132, 3, 103, 2499);
            AddImage(265, 3, 103, 2499);
            AddImage(395, 3, 103, 2499);
            AddImage(395, 98, 103, 2499);
            AddImage(395, 193, 103, 2499);
            AddImage(394, 288, 103, 2499);
            AddImage(265, 98, 103, 2499);
            AddImage(265, 193, 103, 2499);
            AddImage(264, 288, 103, 2499);
            AddImage(3, 194, 103, 2499);
            AddImage(3, 272, 103, 2499);
            AddImage(3, 369, 103, 2499);
            AddImage(2, 3, 103, 2499);
            AddImage(2, 97, 103, 2499);
            AddImage(10, 15, 3604, 2052);
            AddImage(10, 108, 3604, 2052);
            AddImage(10, 213, 3604, 2052);
            AddImage(10, 330, 3604, 2052);
            AddImage(147, 15, 3604, 2052);
            AddImage(147, 108, 3604, 2052);
            AddImage(147, 213, 3604, 2052);
            AddImage(147, 330, 3604, 2052);
            AddImage(286, 15, 3604, 2052);
            AddImage(286, 108, 3604, 2052);
            AddImage(286, 213, 3604, 2052);
            AddImage(286, 330, 3604, 2052);
            AddImage(411, 15, 3604, 2052);
            AddImage(411, 108, 3604, 2052);
            AddImage(411, 213, 3604, 2052);
            AddImage(411, 330, 3604, 2052);
            AddImage(531, 15, 3604, 2052);
            AddImage(531, 108, 3604, 2052);
            AddImage(531, 213, 3604, 2052);
            AddImage(531, 330, 3604, 2052);
            AddImage(184, 15, 3604, 2052);
            AddImage(184, 108, 3604, 2052);
            AddImage(184, 213, 3604, 2052);
            AddImage(184, 330, 3604, 2052);

            #endregion

            AddButton(3, 3, 2094, 2095, 1, GumpButtonType.Reply, 0);
            AddLabel(24, -2, 149, "Guide");

            AddButton(65, 22, 9900, 9900, 2, GumpButtonType.Reply, 0);
            AddButton(65, 432, 9906, 9906, 3, GumpButtonType.Reply, 0);

            //-----

            AddLabel(345, 15, 2563, "Guild Invitations");            
            AddLabel(188, 429, WhiteTextHue, "Previous Page");
            AddLabel(552, 429, WhiteTextHue, "Next Page");
            AddImage(299, 39, 2446, 2401);
            AddLabel(508, 39, 149, "Search for Guild Name");            
            AddLabel(360, 400, 149, "Page");
            AddLabel(356, 429, 63, "Ignore Guild Invitations");
            AddLabel(157, 400, 2599, "Total Invitations");            
            AddLabel(289, 70, 149, "Guild Name");            
            AddLabel(605, 70, 149, "Decline");
            AddLabel(534, 70, 149, "Guild Info");            
            AddLabel(158, 70, 149, "Accept");
            AddLabel(458, 70, 149, "Expires In");

            AddButton(271, 40, 9909, 9909, 3, GumpButtonType.Reply, 0); //Search Left
            AddTextEntry(308, 40, 158, 10, WhiteTextHue, 0, "Guild Name", Guilds.GuildNameCharacterLimit);
            AddButton(483, 39, 9903, 9903, 4, GumpButtonType.Reply, 0); //Search Right

            AddButton(268, 73, 2117, 2118, 5, GumpButtonType.Reply, 0); //Guild Name Sort

            AddButton(438, 73, 5602, 5606, 6, GumpButtonType.Reply, 0); //Expiration Sort

            //List
            AddButton(165, 90, 9721, 9724, 0, GumpButtonType.Reply, 0);
            AddLabel(218, 97, WhiteTextHue, "Outlands Shipping Company (OSC)");
            AddLabel(448, 95, WhiteTextHue, "24d 18h 20m");
            AddButton(550, 95, 4011, 4013, 0, GumpButtonType.Reply, 0);
            AddButton(614, 90, 2472, 2473, 0, GumpButtonType.Reply, 0);
            //--

            AddLabel(270, 400, WhiteTextHue, "1"); //Total Invitations
            AddLabel(397, 400, WhiteTextHue, "1/1"); //Page

            AddButton(154, 429, 4014, 4016, 7, GumpButtonType.Reply, 0); //Previous Page
            AddButton(620, 429, 4005, 4007, 8, GumpButtonType.Reply, 0); //Next Page
            AddButton(322, 426, 9721, 9724, 9, GumpButtonType.Reply, 0); //Ignore Guild Invites
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null)
                return;

            bool closeGump = true;

            switch (info.ButtonID)
            {
                //Guide
                case 1:
                    break;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(GuildInvitationsGump));
                m_Player.SendGump(new GuildInvitationsGump(m_Player));
            }
        }
    }

    #endregion

    #region Guild Overview

    public class GuildOverviewGump : Gump
    {
        public PlayerMobile m_Player;

        public GuildOverviewGump(Mobile from): base(10, 10)
        {
            m_Player = from as PlayerMobile;

            if (m_Player == null)
                return;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int WhiteTextHue = 2655;
            int GreenTextHue = 63;
            int YellowTextHue = 2550;
            int GreyTextHue = 2401;

            #region Background

            AddImage(132, 193, 103, 2499);
            AddImage(131, 98, 103, 2499);
            AddImage(132, 288, 103, 2499);
            AddImage(528, 369, 103, 2499);
            AddImage(395, 369, 103, 2499);
            AddImage(265, 369, 103, 2499);
            AddImage(132, 369, 103, 2499);
            AddImage(528, 3, 103, 2499);
            AddImage(528, 98, 103, 2499);
            AddImage(528, 193, 103, 2499);
            AddImage(527, 288, 103, 2499);
            AddImage(132, 3, 103, 2499);
            AddImage(265, 3, 103, 2499);
            AddImage(395, 3, 103, 2499);
            AddImage(395, 98, 103, 2499);
            AddImage(395, 193, 103, 2499);
            AddImage(394, 288, 103, 2499);
            AddImage(265, 98, 103, 2499);
            AddImage(265, 193, 103, 2499);
            AddImage(264, 288, 103, 2499);
            AddImage(3, 194, 103, 2499);
            AddImage(3, 272, 103, 2499);
            AddImage(3, 369, 103, 2499);
            AddImage(2, 3, 103, 2499);
            AddImage(2, 97, 103, 2499);
            AddImage(10, 15, 3604, 2052);
            AddImage(10, 108, 3604, 2052);
            AddImage(10, 213, 3604, 2052);
            AddImage(10, 330, 3604, 2052);
            AddImage(147, 15, 3604, 2052);
            AddImage(147, 108, 3604, 2052);
            AddImage(147, 213, 3604, 2052);
            AddImage(147, 330, 3604, 2052);
            AddImage(286, 15, 3604, 2052);
            AddImage(286, 108, 3604, 2052);
            AddImage(286, 213, 3604, 2052);
            AddImage(286, 330, 3604, 2052);
            AddImage(411, 15, 3604, 2052);
            AddImage(411, 108, 3604, 2052);
            AddImage(411, 213, 3604, 2052);
            AddImage(411, 330, 3604, 2052);
            AddImage(531, 15, 3604, 2052);
            AddImage(531, 108, 3604, 2052);
            AddImage(531, 213, 3604, 2052);
            AddImage(531, 330, 3604, 2052);
            AddImage(184, 15, 3604, 2052);
            AddImage(184, 108, 3604, 2052);
            AddImage(184, 213, 3604, 2052);
            AddImage(184, 330, 3604, 2052);

            #endregion
                        
            AddButton(3, 3, 2094, 2095, 1, GumpButtonType.Reply, 0);
            AddLabel(24, -2, 149, "Guide");

            AddButton(65, 22, 9900, 9900, 2, GumpButtonType.Reply, 0);
            AddButton(65, 432, 9906, 9906, 3, GumpButtonType.Reply, 0);

            //-----

            #region Images

            AddImage(285, 90, 2328, 0);
            AddItem(155, 384, 4647);
            AddItem(156, 191, 8900);
            AddItem(153, 150, 4810, 2563);
            AddLabel(197, 159, 149, "Guildmaster");
            AddLabel(197, 61, 149, "Guild Abbreviation");
            AddLabel(197, 25, 149, "Guild Name");
            AddImage(271, 23, 1141, 2400);            
            AddImage(320, 60, 2444, 2401);            
            AddItem(151, 22, 3796);
            AddImage(278, 160, 2446, 2401);
            AddLabel(197, 109, 149, "Guild Symbol");
            AddItem(144, 98, 7776);
            AddLabel(197, 203, 149, "Guildhouse");
            AddImage(269, 202, 2446, 2401);
            AddLabel(473, 203, 2550, "Show Location");            
            AddLabel(515, 109, 149, "Faction");
            AddBackground(158, 347, 493, 23, 5100);
            
            AddLabel(197, 324, 149, "Website");            
            AddLabel(197, 392, 149, "My Rank");
            AddImage(256, 391, 2446, 2401);           
            
            AddLabel(503, 392, 149, "Show Guild Title");
            AddItem(454, 397, 3034, 2562);
            AddItem(458, 380, 2978);      
            AddLabel(310, 429, 2115, "Resign from Guild");            
            AddItem(482, 80, 5402);
                        
            AddLabel(299, 257, 2502, "Players");
            AddLabel(382, 257, 2506, "Characters");
            AddLabel(493, 257, 1256, "Wars");
            AddLabel(553, 257, 2599, "Alliances");
          
            AddLabel(197, 257, 149, "Guild Age");
           
            AddItem(164, 250, 3103);
            AddItem(296, 281, 8454, 2500);
            AddItem(383, 281, 8454, 2515);
           
            AddItem(404, 281, 8455, 2515);
            AddItem(493, 292, 3914);
            AddItem(479, 293, 5049);
            AddItem(558, 295, 4030);
            AddItem(558, 287, 4031);

            #endregion

            AddLabel(323, 25, WhiteTextHue, "Outlands Shipping Company");
            AddLabel(337, 61, WhiteTextHue, "OSC");
            AddItem(300, 97, 4014); //Symbol

            AddItem(555, 58, 17099, 2603); //Flag
            AddItem(600, 91, 11009, 2603); //Shield
            AddLabel(586, 144, 2603, "Order");

            AddLabel(322, 161, WhiteTextHue, "Merrill Calder");

            AddLabel(285, 203, WhiteTextHue, "Owned by Merrill Calder");
            AddLabel(282, 226, 2599, "(located at 2500, 2500)");
            AddButton(453, 206, 2117, 2118, 4, GumpButtonType.Reply, 0); //Show Guildhouse Location

            AddLabel(196, 282, WhiteTextHue, "795 Days"); //Guild Age
            AddLabel(332, 296, WhiteTextHue, "25"); //Players
            AddLabel(443, 297, WhiteTextHue, "125"); //Characters
            AddLabel(529, 297, WhiteTextHue, "1"); //Wars
            AddLabel(599, 297, WhiteTextHue, "0"); //Alliances

            AddButton(165, 327, 30008, 30009, 5, GumpButtonType.Reply, 0); //Launch Website
            AddLabel(167, 348, WhiteTextHue, "http://www.outlandsuo.com/TheRebellionGuild/index.htm"); //Website

            AddLabel(317, 392, WhiteTextHue, "Veteran"); //Guild Rank

            AddButton(611, 389, 9724, 9721, 6, GumpButtonType.Reply, 0); //Show Guild Title

            AddButton(425, 425, 2472, 2473, 7, GumpButtonType.Reply, 0); //Resign from Guild
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null)
                return;

            bool closeGump = true;

            switch (info.ButtonID)
            {
                //Guide
                case 1:
                    break;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(GuildOverviewGump));
                m_Player.SendGump(new GuildOverviewGump(m_Player));
            }
        }
    }

    #endregion

    #region Guild Candidates

    public class GuildCandidatesGump : Gump
    {
        public enum SortColumn
        {
            PlayerName,
            Expiration
        }

        public PlayerMobile m_Player;

        public string m_SearchText = "Player Name";
        public int m_SearchIndex = 0;

        public SortColumn m_SortColumn = SortColumn.PlayerName;
        public bool m_SortDescending = true;

        public int m_Page = 0;

        public GuildCandidatesGump(Mobile from): base(10, 10)
        {
            m_Player = from as PlayerMobile;

            if (m_Player == null)
                return;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int WhiteTextHue = 2655;
            int GreenTextHue = 63;
            int YellowTextHue = 2550;
            int GreyTextHue = 2401;

            #region Background

            AddImage(132, 193, 103, 2499);
            AddImage(131, 98, 103, 2499);
            AddImage(132, 288, 103, 2499);
            AddImage(528, 369, 103, 2499);
            AddImage(395, 369, 103, 2499);
            AddImage(265, 369, 103, 2499);
            AddImage(132, 369, 103, 2499);
            AddImage(528, 3, 103, 2499);
            AddImage(528, 98, 103, 2499);
            AddImage(528, 193, 103, 2499);
            AddImage(527, 288, 103, 2499);
            AddImage(132, 3, 103, 2499);
            AddImage(265, 3, 103, 2499);
            AddImage(395, 3, 103, 2499);
            AddImage(395, 98, 103, 2499);
            AddImage(395, 193, 103, 2499);
            AddImage(394, 288, 103, 2499);
            AddImage(265, 98, 103, 2499);
            AddImage(265, 193, 103, 2499);
            AddImage(264, 288, 103, 2499);
            AddImage(3, 194, 103, 2499);
            AddImage(3, 272, 103, 2499);
            AddImage(3, 369, 103, 2499);
            AddImage(2, 3, 103, 2499);
            AddImage(2, 97, 103, 2499);
            AddImage(10, 15, 3604, 2052);
            AddImage(10, 108, 3604, 2052);
            AddImage(10, 213, 3604, 2052);
            AddImage(10, 330, 3604, 2052);
            AddImage(147, 15, 3604, 2052);
            AddImage(147, 108, 3604, 2052);
            AddImage(147, 213, 3604, 2052);
            AddImage(147, 330, 3604, 2052);
            AddImage(286, 15, 3604, 2052);
            AddImage(286, 108, 3604, 2052);
            AddImage(286, 213, 3604, 2052);
            AddImage(286, 330, 3604, 2052);
            AddImage(411, 15, 3604, 2052);
            AddImage(411, 108, 3604, 2052);
            AddImage(411, 213, 3604, 2052);
            AddImage(411, 330, 3604, 2052);
            AddImage(531, 15, 3604, 2052);
            AddImage(531, 108, 3604, 2052);
            AddImage(531, 213, 3604, 2052);
            AddImage(531, 330, 3604, 2052);
            AddImage(184, 15, 3604, 2052);
            AddImage(184, 108, 3604, 2052);
            AddImage(184, 213, 3604, 2052);
            AddImage(184, 330, 3604, 2052);

            #endregion
                        
            AddButton(3, 3, 2094, 2095, 1, GumpButtonType.Reply, 0);
            AddLabel(24, -2, 149, "Guide");

            AddButton(65, 22, 9900, 9900, 2, GumpButtonType.Reply, 0);
            AddButton(65, 432, 9906, 9906, 3, GumpButtonType.Reply, 0);

            //-----
            
            AddLabel(365, 15, 2563, "Candidates");
            AddLabel(267, 70, 149, "Player Name");
            AddLabel(188, 429, WhiteTextHue, "Previous Page");
            AddLabel(552, 429, WhiteTextHue, "Next Page");
            AddLabel(373, 429, 63, "Invite Player to Guild");            
            AddLabel(548, 70, 149, "Approve");
            AddLabel(605, 70, 149, "Decline");
            AddLabel(466, 70, 149, "Player Info");            
            AddLabel(174, 70, 149, "Accepted");
            AddLabel(387, 70, 149, "Expires In");
            AddImage(299, 39, 2446, 2401);       
            AddLabel(508, 39, 149, "Search for Player");            
            AddLabel(360, 400, 149, "Page");           
            AddLabel(513, 400, 2550, "Total Candidates");            
            AddLabel(157, 400, 2599, "Total Accepted");

            AddButton(271, 40, 9909, 9909, 4, GumpButtonType.Reply, 0); //Search Left
            AddTextEntry(308, 40, 158, 12, WhiteTextHue, 12, "Player Name", Guilds.GuildNameCharacterLimit);
            AddButton(483, 39, 9903, 9903, 5, GumpButtonType.Reply, 0); //Search Right

            AddButton(154, 73, 2117, 2118, 6, GumpButtonType.Reply, 0); //Sort Accepted
            AddButton(246, 73, 2117, 2118, 7, GumpButtonType.Reply, 0); //Sort Player Name
            AddButton(367, 73, 5602, 5606, 8, GumpButtonType.Reply, 0); //Sort Expiration

            //List
            AddButton(185, 90, 9723, 2151, 0, GumpButtonType.Reply, 0); //Accepted
            AddLabel(260, 95, WhiteTextHue, "Merrill Calder");
            AddLabel(377, 95, WhiteTextHue, "24d 18h 20m");
            AddButton(485, 95, 4011, 4013, 0, GumpButtonType.Reply, 0); //Player Info
            AddButton(559, 90, 2151, 2154, 0, GumpButtonType.Reply, 0); //Approve
            AddButton(614, 90, 2472, 2473, 0, GumpButtonType.Reply, 0); //Decline
            //

            AddLabel(262, 400, WhiteTextHue, "1"); //Total Accepted
            AddLabel(397, 400, WhiteTextHue, "1/1"); //Page
            AddLabel(625, 400, WhiteTextHue, "1"); //Total Candidates

            AddButton(154, 429, 4014, 4016, 9, GumpButtonType.Reply, 0);
            AddButton(620, 429, 4005, 4007, 10, GumpButtonType.Reply, 0);
            AddButton(337, 429, 4002, 4004, 11, GumpButtonType.Reply, 0); //Invite Player            
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null)
                return;

            bool closeGump = true;

            switch (info.ButtonID)
            {
                //Guide
                case 1:
                    break;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(GuildCandidatesGump));
                m_Player.SendGump(new GuildCandidatesGump(m_Player));
            }
        }
    }

    #endregion

    #region Guild Members

    public class GuildMembersGump : Gump
    {
        public enum SortColumn
        {
            LastOnline,
            PlayerName,
            Rank
        }

        public PlayerMobile m_Player;

        public string m_SearchText = "Player Name";
        public int m_SearchIndex = 0;

        public SortColumn m_SortColumn = SortColumn.PlayerName;
        public bool m_SortDescending = true;

        public int m_Page = 0;

        public GuildMembersGump(Mobile from): base(10, 10)
        {
            m_Player = from as PlayerMobile;

            if (m_Player == null)
                return;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int WhiteTextHue = 2655;
            int GreenTextHue = 63;
            int YellowTextHue = 2550;
            int GreyTextHue = 2401;

            #region Background

            AddImage(132, 193, 103, 2499);
            AddImage(131, 98, 103, 2499);
            AddImage(132, 288, 103, 2499);
            AddImage(528, 369, 103, 2499);
            AddImage(395, 369, 103, 2499);
            AddImage(265, 369, 103, 2499);
            AddImage(132, 369, 103, 2499);
            AddImage(528, 3, 103, 2499);
            AddImage(528, 98, 103, 2499);
            AddImage(528, 193, 103, 2499);
            AddImage(527, 288, 103, 2499);
            AddImage(132, 3, 103, 2499);
            AddImage(265, 3, 103, 2499);
            AddImage(395, 3, 103, 2499);
            AddImage(395, 98, 103, 2499);
            AddImage(395, 193, 103, 2499);
            AddImage(394, 288, 103, 2499);
            AddImage(265, 98, 103, 2499);
            AddImage(265, 193, 103, 2499);
            AddImage(264, 288, 103, 2499);
            AddImage(3, 194, 103, 2499);
            AddImage(3, 272, 103, 2499);
            AddImage(3, 369, 103, 2499);
            AddImage(2, 3, 103, 2499);
            AddImage(2, 97, 103, 2499);
            AddImage(10, 15, 3604, 2052);
            AddImage(10, 108, 3604, 2052);
            AddImage(10, 213, 3604, 2052);
            AddImage(10, 330, 3604, 2052);
            AddImage(147, 15, 3604, 2052);
            AddImage(147, 108, 3604, 2052);
            AddImage(147, 213, 3604, 2052);
            AddImage(147, 330, 3604, 2052);
            AddImage(286, 15, 3604, 2052);
            AddImage(286, 108, 3604, 2052);
            AddImage(286, 213, 3604, 2052);
            AddImage(286, 330, 3604, 2052);
            AddImage(411, 15, 3604, 2052);
            AddImage(411, 108, 3604, 2052);
            AddImage(411, 213, 3604, 2052);
            AddImage(411, 330, 3604, 2052);
            AddImage(531, 15, 3604, 2052);
            AddImage(531, 108, 3604, 2052);
            AddImage(531, 213, 3604, 2052);
            AddImage(531, 330, 3604, 2052);
            AddImage(184, 15, 3604, 2052);
            AddImage(184, 108, 3604, 2052);
            AddImage(184, 213, 3604, 2052);
            AddImage(184, 330, 3604, 2052);

            #endregion
            
            AddButton(3, 3, 2094, 2095, 1, GumpButtonType.Reply, 0);
            AddLabel(24, -2, 149, "Guide");

            AddButton(65, 22, 9900, 9900, 2, GumpButtonType.Reply, 0);
            AddButton(65, 432, 9906, 9906, 3, GumpButtonType.Reply, 0);

            //-----           
            
            AddLabel(365, 15, 2563, "Members");
            AddLabel(280, 70, 149, "Player Name");          
            AddLabel(407, 70, 149, "Guild Rank");
            AddLabel(188, 429, WhiteTextHue, "Previous Page");
            AddLabel(552, 429, WhiteTextHue, "Next Page");
            AddLabel(373, 429, 63, "Invite Player to Guild");
            AddLabel(170, 70, 149, "Last Online");
            AddLabel(556, 70, 149, "Fealty");
            AddLabel(604, 70, 149, "Dismiss");
            AddLabel(503, 70, 149, "Manage");           
            AddImage(299, 39, 2446, 2401);
            AddLabel(508, 39, 149, "Search for Player");
            AddLabel(360, 400, 149, "Page");            
            AddLabel(526, 400, 2550, "Total Members");            
            AddLabel(157, 400, 2599, "Online Members");

            AddButton(271, 40, 9909, 9909, 4, GumpButtonType.Reply, 0); //Search Left
            AddTextEntry(308, 40, 158, 12, WhiteTextHue, 0, "Player Name", Guilds.GuildNameCharacterLimit);
            AddButton(483, 39, 9903, 9903, 5, GumpButtonType.Reply, 0); //Search Right

            AddButton(151, 73, 5602, 5606, 6, GumpButtonType.Reply, 0); //Sort Last Online
            AddButton(259, 73, 2117, 2118, 7, GumpButtonType.Reply, 0); //Sort Player Name
            AddButton(387, 73, 2117, 2118, 8, GumpButtonType.Reply, 0); //Sort Rank

            //List
            AddButton(163, 101, 2361, 2361, 0, GumpButtonType.Reply, 0); //Online Icon
            AddLabel(185, 97, 63, "Online");
            AddLabel(272, 97, WhiteTextHue, "Merrill Calder");
            AddLabel(401, 97, 2603, "Guildmaster");
            AddButton(512, 95, 4011, 4013, 0, GumpButtonType.Reply, 0); //Manage Player
            AddButton(564, 92, 9721, 9724, 0, GumpButtonType.Reply, 0); //Declare Fealty
            AddButton(614, 92, 2472, 2473, 0, GumpButtonType.Reply, 0); //Dismiss
            //

            AddLabel(263, 400, WhiteTextHue, "8"); //Online Members
            AddLabel(397, 400, WhiteTextHue, "10/10"); //Page
            AddLabel(623, 400, WhiteTextHue, "50"); //Total Members

            AddButton(154, 429, 4014, 4016, 9, GumpButtonType.Reply, 0); //Previous Page
            AddButton(620, 429, 4005, 4007, 10, GumpButtonType.Reply, 0); //Next Page
            AddButton(337, 429, 4002, 4004, 11, GumpButtonType.Reply, 0); //Invite Player
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null)
                return;

            bool closeGump = true;

            switch (info.ButtonID)
            {
                //Guide
                case 1:
                    break;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(GuildMembersGump));
                m_Player.SendGump(new GuildMembersGump(m_Player));
            }
        }
    }

    #endregion

    #region Guild Diplomacy

    public class GuildDiplomacyGump : Gump
    {
        public enum SortColumn
        {
            GuildName,
            Relationship,
            PlayerCount
        }

        public enum FilterMode
        {
            ShowAll,
            ShowActive,
            ShowSent,
            ShowReceived
        }

        public PlayerMobile m_Player;

        public string m_SearchText = "Guild Name";
        public int m_SearchIndex = 0;

        public SortColumn m_SortColumn = SortColumn.Relationship;
        public bool m_SortDescending = true;

        public FilterMode m_FilterMode = FilterMode.ShowAll;

        public int m_Page = 0;

        public GuildDiplomacyGump(Mobile from): base(10, 10)
        {
            m_Player = from as PlayerMobile;

            if (m_Player == null)
                return;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int WhiteTextHue = 2655;
            int GreenTextHue = 63;
            int YellowTextHue = 2550;
            int GreyTextHue = 2401;

            #region Background

            AddImage(132, 193, 103, 2499);
            AddImage(131, 98, 103, 2499);
            AddImage(132, 288, 103, 2499);
            AddImage(528, 369, 103, 2499);
            AddImage(395, 369, 103, 2499);
            AddImage(265, 369, 103, 2499);
            AddImage(132, 369, 103, 2499);
            AddImage(528, 3, 103, 2499);
            AddImage(528, 98, 103, 2499);
            AddImage(528, 193, 103, 2499);
            AddImage(527, 288, 103, 2499);
            AddImage(132, 3, 103, 2499);
            AddImage(265, 3, 103, 2499);
            AddImage(395, 3, 103, 2499);
            AddImage(395, 98, 103, 2499);
            AddImage(395, 193, 103, 2499);
            AddImage(394, 288, 103, 2499);
            AddImage(265, 98, 103, 2499);
            AddImage(265, 193, 103, 2499);
            AddImage(264, 288, 103, 2499);
            AddImage(3, 194, 103, 2499);
            AddImage(3, 272, 103, 2499);
            AddImage(3, 369, 103, 2499);
            AddImage(2, 3, 103, 2499);
            AddImage(2, 97, 103, 2499);
            AddImage(10, 15, 3604, 2052);
            AddImage(10, 108, 3604, 2052);
            AddImage(10, 213, 3604, 2052);
            AddImage(10, 330, 3604, 2052);
            AddImage(147, 15, 3604, 2052);
            AddImage(147, 108, 3604, 2052);
            AddImage(147, 213, 3604, 2052);
            AddImage(147, 330, 3604, 2052);
            AddImage(286, 15, 3604, 2052);
            AddImage(286, 108, 3604, 2052);
            AddImage(286, 213, 3604, 2052);
            AddImage(286, 330, 3604, 2052);
            AddImage(411, 15, 3604, 2052);
            AddImage(411, 108, 3604, 2052);
            AddImage(411, 213, 3604, 2052);
            AddImage(411, 330, 3604, 2052);
            AddImage(531, 15, 3604, 2052);
            AddImage(531, 108, 3604, 2052);
            AddImage(531, 213, 3604, 2052);
            AddImage(531, 330, 3604, 2052);
            AddImage(184, 15, 3604, 2052);
            AddImage(184, 108, 3604, 2052);
            AddImage(184, 213, 3604, 2052);
            AddImage(184, 330, 3604, 2052);

            #endregion
                        
            AddButton(3, 3, 2094, 2095, 1, GumpButtonType.Reply, 0);
            AddLabel(24, -2, 149, "Guide");

            AddButton(65, 22, 9900, 9900, 2, GumpButtonType.Reply, 0);
            AddButton(65, 432, 9906, 9906, 3, GumpButtonType.Reply, 0);

            //-----
            
            AddLabel(365, 15, 2606, "Diplomacy");
            AddLabel(244, 70, 149, "Guild Name");           
            AddLabel(422, 70, 149, "Relationship");           
            AddLabel(188, 429, WhiteTextHue, "Previous Page");
            AddLabel(552, 429, WhiteTextHue, "Next Page");
            AddLabel(373, 429, 63, "Add New Guild");
            AddLabel(609, 70, 149, "Manage");
            AddImage(299, 39, 2446, 2401);           
            AddLabel(508, 39, 149, "Search for Guild Name");
            AddLabel(360, 400, 149, "Page");          
            AddLabel(546, 70, 149, "Players");

            AddButton(271, 40, 9909, 9909, 4, GumpButtonType.Reply, 0); //Search Left
            AddTextEntry(308, 40, 158, 14, WhiteTextHue, 0, "Guild Name", Guilds.GuildNameCharacterLimit);
            AddButton(483, 39, 9903, 9903, 5, GumpButtonType.Reply, 0); //Search Right

            AddButton(223, 73, 2117, 2118, 6, GumpButtonType.Reply, 0); //Sort Guild Name
            AddButton(400, 73, 5602, 5606, 7, GumpButtonType.Reply, 0); //Sort Relationship
            AddButton(526, 73, 2117, 2118, 8, GumpButtonType.Reply, 0); //Sort Players

            //List
            AddLabel(192, 95, WhiteTextHue, "God's Wrath Clan (GoD)");
            AddLabel(398, 95, 2599, "Alliance Requested");
            AddLabel(561, 95, WhiteTextHue, "5"); //Players
            AddButton(617, 95, 4011, 4013, 0, GumpButtonType.Reply, 0); //Manage Relationship
            //--

            AddButton(295, 379, 2223, 2223, 9, GumpButtonType.Reply, 0); //Previous Relationship Filter
            AddLabel(326, 375, 2603, "Show All Relationships");
            AddButton(473, 379, 2224, 2224, 10, GumpButtonType.Reply, 0); //Next Relationship Filter

            AddLabel(158, 400, 1256, "War Requests");
            AddLabel(251, 400, WhiteTextHue, "10");

            AddLabel(397, 400, WhiteTextHue, "1/1"); //Page

            AddLabel(533, 400, 2599, "Ally Requests");
            AddLabel(625, 400, WhiteTextHue, "10");

            AddButton(154, 429, 4014, 4016, 11, GumpButtonType.Reply, 0); //Previous Page
            AddButton(620, 429, 4005, 4007, 12, GumpButtonType.Reply, 0); //Next Page
            AddButton(337, 429, 4002, 4004, 13, GumpButtonType.Reply, 0); //Add New Guild
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null)
                return;

            bool closeGump = true;

            switch (info.ButtonID)
            {
                //Guide
                case 1:
                break;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(GuildDiplomacyGump));
                m_Player.SendGump(new GuildDiplomacyGump(m_Player));
            }
        }
    }

    #endregion
}