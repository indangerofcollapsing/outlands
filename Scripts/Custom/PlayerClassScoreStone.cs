using System;
using Server;
using Server.Mobiles;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Gumps;

namespace Server.Custom
{
    public class PlayerClassScoreStone : Item
    {
        public virtual PlayerClass PlayerClassType { get { return PlayerClass.Paladin; } }

        [Constructable]
        public PlayerClassScoreStone(): base(0x0EDE)
        {
            Name = "a player class score stone";
            Movable = false;
        }

        public PlayerClassScoreStone(Serial serial): base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerClassPersistance.InitializeLifetimeScores();

            from.CloseAllGumps();
            from.SendGump(new PlayerClassScoresGump(PlayerClassType, from, 2, 1));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version          
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class PlayerClassScoresGump : Gump
    {
        private PlayerClass m_PlayerClass = PlayerClass.Paladin;

        private PlayerMobile m_Player;        

        private int m_CycleMode = 0;
        private int m_PageNumber = 1;
        private int m_TotalPages = 1;

        private int recordsPerPage = 10;

        public PlayerClassScoresGump(PlayerClass playerClass, Mobile from, int cycleMode, int pageNumber): base(10, 10)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            m_PlayerClass = playerClass;
            m_Player = player;
            m_CycleMode = cycleMode;
            m_PageNumber = pageNumber;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            //-----
            AddImage(0, 44, 202);
            AddImage(44, 0, 201);
            AddImage(0, 0, 206);
            AddImage(0, 468, 204);
            AddImage(590, 1, 207);
            AddImage(590, 468, 205);
            AddImage(44, 468, 233);
            AddImage(590, 45, 203);
            AddImageTiled(44, 44, 546, 424, 200);
            AddImage(0, 152, 202);
            AddImage(163, 0, 201);
            AddImage(166, 468, 233);
            AddImage(590, 152, 203);
            AddImage(75, 94, 3001);
            AddImage(319, 94, 3001);
            AddImage(75, 397, 3001);
            AddImage(319, 397, 3001);
            AddImage(600, 46, 10441);

            int textHue = 2036;
            int boldHue = 149;
            int greenHue = 168;

            int paladinTextHue = 149;
            int murdererTextHue = 149;
            int pirateTextHue = 149;

            int playerRanking = -1;
            int playerScore = 0;

            int currentPlayerPosition = 0;
            int totalPlayers = 0;
            int totalScores = 0;

            List<PlayerClassPersistance.PlayerClassRecord> m_Records = new List<PlayerClassPersistance.PlayerClassRecord>();

            #region Populate PlayerClassRecords

            switch (m_PlayerClass)
            {
                case PlayerClass.Paladin:
                    AddLabel(276, 16, paladinTextHue, "Paladin Scores");

                    switch (m_CycleMode)
                    {
                        case 0:
                            for (int a = 0; a < PlayerClassPersistance.m_LifetimePaladinScores.Count; a++ )
                            {
                                PlayerClassPersistance.PlayerClassRecord record = PlayerClassPersistance.m_LifetimePaladinScores[a];

                                if (record == null) 
                                    continue;

                                if (record.pm_Player == null)
                                    continue;

                                if (record.pm_Player.AccessLevel > AccessLevel.Player)
                                    continue;

                                totalPlayers++;

                                m_Records.Add(record);
                                totalScores += record.m_Score;

                                if (record.pm_Player == player)
                                {
                                    playerRanking = totalPlayers;
                                    playerScore = record.m_Score;
                                }
                            }
                        break;

                        case 1:
                            for (int a = 0; a < PlayerClassPersistance.m_PreviousPaladinScores.Count; a++)
                            {
                                PlayerClassPersistance.PlayerClassRecord record = PlayerClassPersistance.m_PreviousPaladinScores[a];

                                if (record == null)
                                    continue;

                                if (record.pm_Player == null)
                                    continue;

                                if (record.pm_Player.AccessLevel > AccessLevel.Player)
                                    continue;

                                totalPlayers++;

                                m_Records.Add(record);
                                totalScores += record.m_Score;

                                if (record.pm_Player == player)
                                {
                                    playerRanking = totalPlayers;
                                    playerScore = record.m_Score;
                                }
                            }
                        break;

                        case 2:
                            for (int a = 0; a < PlayerClassPersistance.m_CurrentPaladinScores.Count; a++)
                            {
                                PlayerClassPersistance.PlayerClassRecord record = PlayerClassPersistance.m_CurrentPaladinScores[a];

                                if (record == null)
                                    continue;

                                if (record.pm_Player == null)
                                    continue;

                                if (record.pm_Player.AccessLevel > AccessLevel.Player)
                                    continue;

                                totalPlayers++;

                                m_Records.Add(record);
                                totalScores += record.m_Score;

                                if (record.pm_Player == player)
                                {
                                    playerRanking = totalPlayers;
                                    playerScore = record.m_Score;
                                }
                            }
                        break;
                    }
                break;

                case PlayerClass.Murderer:
                    AddLabel(276, 16, murdererTextHue, "Murderer Scores");

                    switch (m_CycleMode)
                    {
                        case 0:
                            for (int a = 0; a < PlayerClassPersistance.m_LifetimeMurdererScores.Count; a++)
                            {
                                PlayerClassPersistance.PlayerClassRecord record = PlayerClassPersistance.m_LifetimeMurdererScores[a];

                                if (record == null)
                                    continue;

                                if (record.pm_Player == null)
                                    continue;

                                if (record.pm_Player.AccessLevel > AccessLevel.Player)
                                    continue;

                                totalPlayers++;

                                m_Records.Add(record);
                                totalScores += record.m_Score;

                                if (record.pm_Player == player)
                                {
                                    playerRanking = totalPlayers;
                                    playerScore = record.m_Score;
                                }
                            }
                        break;

                        case 1:
                            for (int a = 0; a < PlayerClassPersistance.m_PreviousMurdererScores.Count; a++)
                            {
                                PlayerClassPersistance.PlayerClassRecord record = PlayerClassPersistance.m_PreviousMurdererScores[a];

                                if (record == null)
                                    continue;

                                if (record.pm_Player == null)
                                    continue;

                                if (record.pm_Player.AccessLevel > AccessLevel.Player)
                                    continue;

                                totalPlayers++;

                                m_Records.Add(record);
                                totalScores += record.m_Score;

                                if (record.pm_Player == player)
                                {
                                    playerRanking = totalPlayers;
                                    playerScore = record.m_Score;
                                }
                            }
                        break;

                        case 2:
                            for (int a = 0; a < PlayerClassPersistance.m_CurrentMurdererScores.Count; a++)
                            {
                                PlayerClassPersistance.PlayerClassRecord record = PlayerClassPersistance.m_CurrentMurdererScores[a];

                                if (record == null)
                                    continue;

                                if (record.pm_Player == null)
                                    continue;

                                if (record.pm_Player.AccessLevel > AccessLevel.Player)
                                    continue;

                                totalPlayers++;

                                m_Records.Add(record);
                                totalScores += record.m_Score;

                                if (record.pm_Player == player)
                                {
                                    playerRanking = totalPlayers;
                                    playerScore = record.m_Score;
                                }
                            }
                        break;
                    }
                break;

                case PlayerClass.Pirate:
                    AddLabel(276, 16, pirateTextHue, "Pirate Scores");

                    switch (m_CycleMode)
                    {
                        case 0:
                            for (int a = 0; a < PlayerClassPersistance.m_LifetimePirateScores.Count; a++)
                            {
                                PlayerClassPersistance.PlayerClassRecord record = PlayerClassPersistance.m_LifetimePirateScores[a];

                                if (record == null)
                                    continue;

                                if (record.pm_Player == null)
                                    continue;

                                if (record.pm_Player.AccessLevel > AccessLevel.Player)
                                    continue;

                                totalPlayers++;

                                m_Records.Add(record);
                                totalScores += record.m_Score;

                                if (record.pm_Player == player)
                                {
                                    playerRanking = totalPlayers;
                                    playerScore = record.m_Score;
                                }
                            }
                        break;

                        case 1:
                            for (int a = 0; a < PlayerClassPersistance.m_PreviousPirateScores.Count; a++)
                            {
                                PlayerClassPersistance.PlayerClassRecord record = PlayerClassPersistance.m_PreviousPirateScores[a];

                                if (record == null)
                                    continue;

                                if (record.pm_Player == null)
                                    continue;

                                if (record.pm_Player.AccessLevel > AccessLevel.Player)
                                    continue;

                                totalPlayers++;

                                m_Records.Add(record);
                                totalScores += record.m_Score;

                                if (record.pm_Player == player)
                                {
                                    playerRanking = totalPlayers;
                                    playerScore = record.m_Score;
                                }
                            }
                        break;

                        case 2:
                            for (int a = 0; a < PlayerClassPersistance.m_CurrentPirateScores.Count; a++)
                            {
                                PlayerClassPersistance.PlayerClassRecord record = PlayerClassPersistance.m_CurrentPirateScores[a];

                                if (record == null)
                                    continue;

                                if (record.pm_Player == null)
                                    continue;

                                if (record.pm_Player.AccessLevel > AccessLevel.Player)
                                    continue;

                                totalPlayers++;

                                m_Records.Add(record);
                                totalScores += record.m_Score;

                                if (record.pm_Player == player)
                                {
                                    playerRanking = totalPlayers;
                                    playerScore = record.m_Score;
                                }
                            }
                        break;
                    }
                break;
            }

            #endregion

            string sRanking = Utility.CreateCurrencyString(playerRanking);
            string sScore = Utility.CreateCurrencyString(playerScore);
            string sTotalPlayers = Utility.CreateCurrencyString(totalPlayers);
            string sTotalScores = Utility.CreateCurrencyString(totalScores);

            if (player.AccessLevel > AccessLevel.Player)
            {
                sRanking = "*Staff*";
                sScore = "*Staff*";
            }

            else
            {
                if (playerRanking == -1)
                    sRanking = "-";
            }

            AddLabel(115, 40, boldHue, "Your Ranking:");
            AddLabel(250 - (sRanking.Length * 3), 40, greenHue, sRanking);  

            AddLabel(111, 64, boldHue, "Total Players:");
            AddLabel(250 - (sTotalPlayers.Length * 3), 64, textHue, sTotalPlayers);

            AddLabel(373, 40, boldHue, "Your Score:");
            AddLabel(500 - (sScore.Length * 3), 40, greenHue, sScore);

            AddLabel(365, 65, boldHue, "Total Scores:");
            AddLabel(500 - (sTotalScores.Length * 3), 65, textHue, sTotalScores);

            int iStartY = 105;

            string cycleText = "";

            //Current
            if (m_CycleMode == 2)
            {
                if (DateTime.UtcNow >= InfluencePersistance.NextCycleCompletion)
                    AddLabel(220, iStartY, textHue, "Current Cycle: Calculating Results");
                else
                    AddLabel(220, iStartY, textHue, "Current Cycle: Ends in " + Utility.CreateTimeRemainingString(DateTime.UtcNow, InfluencePersistance.NextCycleCompletion, true, true, true, true, false));
            }

            //Previous
            else if (m_CycleMode == 1)
            {
                if (InfluencePersistance.PreviousCycleCompletion != DateTime.MinValue)
                {
                    cycleText = InfluencePersistance.PreviousCycleCompletion.Month.ToString() + "-" + InfluencePersistance.PreviousCycleCompletion.Day.ToString() + "-" + InfluencePersistance.PreviousCycleCompletion.Year.ToString();
                    AddLabel(180, iStartY, textHue, "Results of Previous Cycle Ending on " + cycleText);
                }

                else
                    AddLabel(180, iStartY, textHue, "Results of Previous Cycle: No Previous Cycle Yet");
            }

            else
                AddLabel(270, iStartY, textHue, "Lifetime Scores");

            iStartY += 25;

            AddLabel(136, iStartY, boldHue, "Ranking");
            AddLabel(247, iStartY, boldHue, "Player");
            AddLabel(355, iStartY, boldHue, "Guild");
            AddLabel(457, iStartY, boldHue, "Score");

            iStartY += 40;

            m_TotalPages = (int)(Math.Ceiling((double)totalPlayers / (double)recordsPerPage));

            if (m_TotalPages == 0)
                m_TotalPages = 1;

            if (m_PageNumber < 1)
                m_PageNumber = 1;

            if (m_PageNumber > m_TotalPages)
                m_PageNumber = m_TotalPages;

            if (totalPlayers > 0)
            {
                int startIndex = (m_PageNumber - 1) * recordsPerPage;
                int endIndex = startIndex + recordsPerPage;

                if (endIndex > totalPlayers)
                    endIndex = totalPlayers;

                for (int a = startIndex; a < endIndex; a++)
                {
                    string ranking = (a + 1).ToString();
                    string playerName = "-";
                    string guildName = "";
                    string score = "0";

                    int labelHue = textHue;

                    bool foundRecord = false;

                    if (a < m_Records.Count)
                    {
                        if (m_Records[a].pm_Player != null)
                        {
                            foundRecord = true;

                            if (m_Records[a].pm_Player == player)
                                labelHue = greenHue;

                            playerName = m_Records[a].pm_Player.RawName;

                            if (m_Records[a].pm_Player.Guild != null)
                                guildName = m_Records[a].pm_Player.Guild.Abbreviation;

                            score = Utility.CreateCurrencyString(m_Records[a].m_Score);
                        }
                    }

                    if (foundRecord)
                    {
                        AddLabel(157 - (ranking.Length * 3), iStartY, labelHue, ranking);
                        AddLabel(265 - (playerName.Length * 3), iStartY, labelHue, playerName);
                        AddLabel(366 - (guildName.Length * 3), iStartY, labelHue, guildName);
                        AddLabel(471 - (score.Length * 3), iStartY, labelHue, score);
                    }

                    iStartY += 20;
                }               
            }

            if (m_CycleMode == 0)
            {
                AddButton(131, 460, 9724, 9721, 1, GumpButtonType.Reply, 0);
                AddLabel(120, 440, boldHue, "Lifetime");
            }

            else
            {
                AddButton(131, 460, 9721, 9724, 1, GumpButtonType.Reply, 0);
                AddLabel(120, 440, textHue, "Lifetime");
            }

            if (m_CycleMode == 1)
            {
                AddButton(304, 460, 9724, 9721, 2, GumpButtonType.Reply, 0);
                AddLabel(275, 440, boldHue, "Previous Cycle");
            }

            else
            {
                AddButton(304, 460, 9721, 9724, 2, GumpButtonType.Reply, 0);
                AddLabel(275, 440, textHue, "Previous Cycle");
            }

            if (m_CycleMode == 2)
            {
                AddButton(454, 460, 9724, 9721, 3, GumpButtonType.Reply, 0);
                AddLabel(423, 440, boldHue, "Current Cycle");
            }

            else
            {
                AddButton(454, 460, 9721, 9724, 3, GumpButtonType.Reply, 0);
                AddLabel(423, 440, textHue, "Current Cycle");
            }

            if (m_PageNumber > 1)
            {
                AddButton(29, 410, 4014, 4016, 4, GumpButtonType.Reply, 0);
                AddLabel(64, 411, textHue, "Previous");
            }

            if (m_PageNumber < m_TotalPages)
            {
                AddButton(537, 410, 4005, 4007, 5, GumpButtonType.Reply, 0);
                AddLabel(573, 411, textHue, "Next");
            }            
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (from == null) return;
            if (from.Deleted) return;
            if (m_Player == null) return;
            if (m_Player.Deleted) return;

            //Lifetime
            if (info.ButtonID == 1)
            {
                m_CycleMode = 0;
                m_PageNumber = 1;

                from.SendSound(0x055);

                from.CloseGump(typeof(PlayerClassScoresGump));
                from.SendGump(new PlayerClassScoresGump(m_PlayerClass, m_Player, m_CycleMode, m_PageNumber));
            }

            //Previous Month
            if (info.ButtonID == 2)
            {
                m_CycleMode = 1;
                m_PageNumber = 1;

                from.SendSound(0x055);

                from.CloseGump(typeof(PlayerClassScoresGump));
                from.SendGump(new PlayerClassScoresGump(m_PlayerClass, m_Player, m_CycleMode, m_PageNumber));
            }

            //Current Month
            if (info.ButtonID == 3)
            {
                m_CycleMode = 2;
                m_PageNumber = 1;

                from.SendSound(0x055);

                from.CloseGump(typeof(PlayerClassScoresGump));
                from.SendGump(new PlayerClassScoresGump(m_PlayerClass, m_Player, m_CycleMode, m_PageNumber));
            }
            
            //Previous Record Page
            if (info.ButtonID == 4)
            {
                if (m_PageNumber > 0)
                {
                    m_PageNumber--;

                    from.SendSound(0x055);

                    from.CloseGump(typeof(PlayerClassScoresGump));
                    from.SendGump(new PlayerClassScoresGump(m_PlayerClass, m_Player, m_CycleMode, m_PageNumber));

                    return;
                }
            }

            //Next Record Page
            if (info.ButtonID == 5)
            {
                if (m_PageNumber < m_TotalPages)
                {
                    m_PageNumber++;

                    from.SendSound(0x055);

                    from.CloseGump(typeof(PlayerClassScoresGump));
                    from.SendGump(new PlayerClassScoresGump(m_PlayerClass, m_Player, m_CycleMode, m_PageNumber));

                    return;
                }
            }
        }
    }
}