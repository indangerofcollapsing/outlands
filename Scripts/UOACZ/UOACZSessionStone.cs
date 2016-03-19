using System;
using Server;
using Server.Mobiles;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Gumps;
using System.Linq;

namespace Server.Custom
{
    public class UOACZSessionStone : Item
    {
        [Constructable]
        public UOACZSessionStone(): base(3804)
        {
            Name = "a UOACZ session stone";

            Hue = 2500;
            Movable = false;
        }

        public UOACZSessionStone(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            if (from.AccessLevel == AccessLevel.Player)
            {
                LabelTo(from, "UOACZ Stone");
                LabelTo(from, "(double click to access)");
            }

            else
                LabelTo(from, "UOACZ Map: " + UOACZPersistance.CustomRegion.ToString());

            if (UOACZPersistance.Enabled)
            {
                if (UOACZPersistance.Active)
                    LabelTo(from, "Active UOACZ Session Ends in " + Utility.CreateTimeRemainingString(DateTime.UtcNow, UOACZPersistance.m_CurrentSessionExpiration, true, true, true, true, true));

                else
                    LabelTo(from, "Next UOACZ Session Starts in " + Utility.CreateTimeRemainingString(DateTime.UtcNow, UOACZPersistance.m_NextScheduledSessionStartTime, true, true, true, true, true));
            }

            else            
                LabelTo(from, " UOACZ Disabled: " + UOACZPersistance.Enabled.ToString()); 
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            if (Utility.GetDistance(Location, from.Location) >= 10)
                return;

            player.SendSound(UOACZSystem.openGumpSound);

            player.CloseGump(typeof(UOACZScoreGump));
            player.SendGump(new UOACZScoreGump(player));
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

    public class UOACZScoreGump : Gump
    {  
        public PlayerMobile player;

        public bool allowScoreRecordsPrevious = false;
        public bool allowScoreRecordsNext = false;

        public bool allowRewardsPrevious = false;
        public bool allowRewardsNext = false;

        public bool allowUnlockablesPrevious = false;
        public bool allowUnlockablesNext = false;

        public int RecordsPerPage = 10;
        public int RewardsPerPage = 5;

        public UOACZScoreGump(PlayerMobile pm_Mobile): base(10, 10)
        {
            if (pm_Mobile == null) return;
            if (pm_Mobile.Deleted) return;

            player = pm_Mobile;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            if (player.m_UOACZAccountEntry == null) 
                return;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

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
            AddImage(600, 46, 10441);

            //--------
            
            int boldTextHue = 149;
            int normalTextHue = 2036;

            int startY = 75;

            int HumanRankX = 47;
            int HumanCharacterX = 115;
            int HumanScoreX = 187;

            int UndeadRankX = 247;
            int UndeadCharacterX = 315;
            int UndeadScoreX = 387;

            int TotalRankX = 447;
            int TotalCharacterX = 515;
            int TotalScoreX = 587;

            string nextSessionText = "";

            if (UOACZPersistance.Enabled)
            {
                if (UOACZPersistance.Active)
                    nextSessionText = "Active UOACZ Session Ends in " + Utility.CreateTimeRemainingString(DateTime.UtcNow, UOACZPersistance.m_CurrentSessionExpiration, true, true, true, true, true);
                else
                    nextSessionText = "Next UOACZ Session Starts in " + Utility.CreateTimeRemainingString(DateTime.UtcNow, UOACZPersistance.m_NextScheduledSessionStartTime, true, true, true, true, true);
            }

            string headerText = "";

            string rankText = "";
            string nameText = "";
            string scoreText = "";

            int textHue = UOACZSystem.greenTextHue;
            int characterHue = normalTextHue;

            int startingIndex = 0;

            bool useScoringTemplate = false;

            if (player.m_UOACZAccountEntry.ScorePage == UOACZAccountEntry.ScorePageType.Previous)
                useScoringTemplate = true;

            if (player.m_UOACZAccountEntry.ScorePage == UOACZAccountEntry.ScorePageType.Best)
                useScoringTemplate = true;

            if (player.m_UOACZAccountEntry.ScorePage == UOACZAccountEntry.ScorePageType.Lifetime)
                useScoringTemplate = true;

            if (useScoringTemplate)
            {
                AddLabel(70, startY, UOACZSystem.blueTextHue, "Human Scores");
                AddLabel(275, startY, UOACZSystem.redTextHue, "Undead Scores");
                AddLabel(480, startY, UOACZSystem.purpleTextHue, "Total Scores");

                startY += 35;

                AddLabel(30, startY, boldTextHue, "Rank");
                AddLabel(85, startY, boldTextHue, "Character");
                AddLabel(170, startY, boldTextHue, "Score");

                AddLabel(230, startY, boldTextHue, "Rank");
                AddLabel(285, startY, boldTextHue, "Character");
                AddLabel(370, startY, boldTextHue, "Score");

                AddLabel(430, startY, boldTextHue, "Rank");
                AddLabel(485, startY, boldTextHue, "Character");
                AddLabel(570, startY, boldTextHue, "Score");

                startY += 40;

                AddImage(30, 100, 3001);
                AddImage(270, 100, 3001);
                AddImage(350, 100, 3001);

                AddImage(30, 135, 3001);
                AddImage(270, 135, 3001);
                AddImage(350, 135, 3001);

                AddImage(30, 400, 3001);
                AddImage(270, 400, 3001);
                AddImage(350, 400, 3001);

                AddImage(215, 105, 3003);
                AddImage(215, 158, 3003);

                AddImage(415, 105, 3003);
                AddImage(415, 158, 3003);
            }

            switch (player.m_UOACZAccountEntry.ScorePage)
            {
                case UOACZAccountEntry.ScorePageType.Previous:
                    headerText = "UOACZ Most Recent Session Scores";
                   
                    AddLabel(Utility.CenteredTextOffset(320, headerText), 16, boldTextHue, headerText);
                    AddLabel(Utility.CenteredTextOffset(320, nextSessionText), 34, normalTextHue, nextSessionText);
                    
                    startingIndex = player.m_UOACZAccountEntry.ScorePageNumber * RecordsPerPage;
                    
                    if (startingIndex + RecordsPerPage <= UOACZPersistance.m_PreviousHumanScores.Count)
                        allowScoreRecordsNext = true;

                    if (startingIndex + RecordsPerPage <= UOACZPersistance.m_PreviousUndeadScores.Count)
                        allowScoreRecordsNext = true;

                    if (startingIndex + RecordsPerPage <= UOACZPersistance.m_PreviousTotalScores.Count)
                        allowScoreRecordsNext = true;
                    
                    allowScoreRecordsPrevious = true;

                    if (player.m_UOACZAccountEntry.ScorePageNumber == 0)
                        allowScoreRecordsPrevious = false;

                    //Populate Public Records
                    for (int a = 0; a < RecordsPerPage; a++)
                    {
                        int adjustedIndex = startingIndex + a;

                        //Human
                        if (adjustedIndex <= UOACZPersistance.m_PreviousHumanScores.Count - 1)
                        {
                            UOACZAccountEntry HumanEntry = UOACZPersistance.m_PreviousHumanScores[adjustedIndex];

                            if (HumanEntry != null)
                            {
                                textHue = normalTextHue;
                                characterHue = UOACZSystem.blueTextHue;

                                if (player.m_UOACZAccountEntry == HumanEntry)
                                {
                                    textHue = UOACZSystem.greenTextHue;
                                    characterHue = UOACZSystem.greenTextHue;
                                }                               
                        
                                rankText = (adjustedIndex + 1).ToString();
                                nameText = HumanEntry.MostRecentPlayerString;
                                scoreText = HumanEntry.PreviousSessionHumanScore.ToString();

                                AddLabel(Utility.CenteredTextOffset(HumanRankX, rankText), startY, textHue, rankText);
                                AddLabel(Utility.CenteredTextOffset(HumanCharacterX, nameText), startY, characterHue, nameText);
                                AddLabel(Utility.CenteredTextOffset(HumanScoreX, scoreText), startY, textHue, scoreText);
                            }
                        }

                        //Undead
                        if (adjustedIndex <= UOACZPersistance.m_PreviousUndeadScores.Count - 1)
                        {
                            UOACZAccountEntry UndeadEntry = UOACZPersistance.m_PreviousUndeadScores[adjustedIndex];

                            if (UndeadEntry != null)
                            {
                                textHue = normalTextHue;
                                characterHue = UOACZSystem.redTextHue;

                                if (player.m_UOACZAccountEntry == UndeadEntry)
                                {
                                    textHue = UOACZSystem.greenTextHue;
                                    characterHue = UOACZSystem.greenTextHue;
                                }  

                                rankText = (adjustedIndex + 1).ToString();
                                nameText = UndeadEntry.MostRecentPlayerString;
                                scoreText = UndeadEntry.PreviousSessionUndeadScore.ToString();

                                AddLabel(Utility.CenteredTextOffset(UndeadRankX, rankText), startY, textHue, rankText);
                                AddLabel(Utility.CenteredTextOffset(UndeadCharacterX, nameText), startY, characterHue, nameText);
                                AddLabel(Utility.CenteredTextOffset(UndeadScoreX, scoreText), startY, textHue, scoreText);
                            }
                        }

                        //Total
                        if (adjustedIndex <= UOACZPersistance.m_PreviousTotalScores.Count - 1)
                        {
                            UOACZAccountEntry TotalEntry = UOACZPersistance.m_PreviousTotalScores[adjustedIndex];

                            if (TotalEntry != null)
                            {
                                textHue = normalTextHue;
                                characterHue = UOACZSystem.purpleTextHue;

                                if (player.m_UOACZAccountEntry == TotalEntry)
                                {
                                    textHue = UOACZSystem.greenTextHue;
                                    characterHue = UOACZSystem.greenTextHue;
                                } 

                                rankText = (adjustedIndex + 1).ToString();
                                nameText = TotalEntry.MostRecentPlayerString;
                                scoreText = TotalEntry.PreviousSessionTotalScore.ToString();

                                AddLabel(Utility.CenteredTextOffset(TotalRankX, rankText), startY, textHue, rankText);
                                AddLabel(Utility.CenteredTextOffset(TotalCharacterX, nameText), startY, characterHue, nameText);
                                AddLabel(Utility.CenteredTextOffset(TotalScoreX, scoreText), startY, textHue, scoreText);
                            }
                        }

                        startY += 25;
                    }
                break;

                case UOACZAccountEntry.ScorePageType.Best:
                    headerText = "UOACZ Top Session Scores";

                    AddLabel(Utility.CenteredTextOffset(320, headerText), 16, boldTextHue, headerText);
                    AddLabel(Utility.CenteredTextOffset(320, nextSessionText), 34, normalTextHue, nextSessionText);
                    
                    startingIndex = player.m_UOACZAccountEntry.ScorePageNumber * RecordsPerPage;
                    
                    if (startingIndex + RecordsPerPage <= UOACZPersistance.m_BestHumanScores.Count)
                        allowScoreRecordsNext = true;

                    if (startingIndex + RecordsPerPage <= UOACZPersistance.m_BestUndeadScores.Count)
                        allowScoreRecordsNext = true;

                    if (startingIndex + RecordsPerPage <= UOACZPersistance.m_BestTotalScores.Count)
                        allowScoreRecordsNext = true;

                    allowScoreRecordsPrevious = true;

                    if (player.m_UOACZAccountEntry.ScorePageNumber == 0)
                        allowScoreRecordsPrevious = false;

                    //Populate Public Records
                    for (int a = 0; a < RecordsPerPage; a++)
                    {
                        int adjustedIndex = startingIndex + a;

                        //Human
                        if (adjustedIndex <= UOACZPersistance.m_BestHumanScores.Count - 1)
                        {
                            UOACZAccountEntry HumanEntry = UOACZPersistance.m_BestHumanScores[adjustedIndex];

                            if (HumanEntry != null)
                            {
                                textHue = normalTextHue;
                                characterHue = UOACZSystem.blueTextHue;

                                if (player.m_UOACZAccountEntry == HumanEntry)
                                {
                                    textHue = UOACZSystem.greenTextHue;
                                    characterHue = UOACZSystem.greenTextHue;
                                }                               
                        
                                rankText = (adjustedIndex + 1).ToString();
                                nameText = HumanEntry.MostRecentPlayerString;
                                scoreText = HumanEntry.BestHumanScore.ToString();

                                AddLabel(Utility.CenteredTextOffset(HumanRankX, rankText), startY, textHue, rankText);
                                AddLabel(Utility.CenteredTextOffset(HumanCharacterX, nameText), startY, characterHue, nameText);
                                AddLabel(Utility.CenteredTextOffset(HumanScoreX, scoreText), startY, textHue, scoreText);
                            }
                        }

                        //Undead
                        if (adjustedIndex <= UOACZPersistance.m_BestUndeadScores.Count - 1)
                        {
                            UOACZAccountEntry UndeadEntry = UOACZPersistance.m_BestUndeadScores[adjustedIndex];

                            if (UndeadEntry != null)
                            {
                                textHue = normalTextHue;
                                characterHue = UOACZSystem.redTextHue;

                                if (player.m_UOACZAccountEntry == UndeadEntry)
                                {
                                    textHue = UOACZSystem.greenTextHue;
                                    characterHue = UOACZSystem.greenTextHue;
                                }  

                                rankText = (adjustedIndex + 1).ToString();
                                nameText = UndeadEntry.MostRecentPlayerString;
                                scoreText = UndeadEntry.BestUndeadScore.ToString();

                                AddLabel(Utility.CenteredTextOffset(UndeadRankX, rankText), startY, textHue, rankText);
                                AddLabel(Utility.CenteredTextOffset(UndeadCharacterX, nameText), startY, characterHue, nameText);
                                AddLabel(Utility.CenteredTextOffset(UndeadScoreX, scoreText), startY, textHue, scoreText);
                            }
                        }

                        //Total
                        if (adjustedIndex <= UOACZPersistance.m_BestTotalScores.Count - 1)
                        {
                            UOACZAccountEntry TotalEntry = UOACZPersistance.m_BestTotalScores[adjustedIndex];

                            if (TotalEntry != null)
                            {
                                textHue = normalTextHue;
                                characterHue = UOACZSystem.purpleTextHue;

                                if (player.m_UOACZAccountEntry == TotalEntry)
                                {
                                    textHue = UOACZSystem.greenTextHue;
                                    characterHue = UOACZSystem.greenTextHue;
                                } 

                                rankText = (adjustedIndex + 1).ToString();
                                nameText = TotalEntry.MostRecentPlayerString;
                                scoreText = TotalEntry.BestTotalScore.ToString();

                                AddLabel(Utility.CenteredTextOffset(TotalRankX, rankText), startY, textHue, rankText);
                                AddLabel(Utility.CenteredTextOffset(TotalCharacterX, nameText), startY, characterHue, nameText);
                                AddLabel(Utility.CenteredTextOffset(TotalScoreX, scoreText), startY, textHue, scoreText);
                            }
                        }

                        startY += 25;
                    }
                break;

                case UOACZAccountEntry.ScorePageType.Lifetime:
                    headerText = "UOACZ Lifetime Total Scores";

                    AddLabel(Utility.CenteredTextOffset(320, headerText), 16, boldTextHue, headerText);
                    AddLabel(Utility.CenteredTextOffset(320, nextSessionText), 34, normalTextHue, nextSessionText);
                    
                    startingIndex = player.m_UOACZAccountEntry.ScorePageNumber * RecordsPerPage;
                    
                    if (startingIndex + RecordsPerPage <= UOACZPersistance.m_LifetimeHumanScores.Count)
                        allowScoreRecordsNext = true;

                    if (startingIndex + RecordsPerPage <= UOACZPersistance.m_LifetimeUndeadScores.Count)
                        allowScoreRecordsNext = true;

                    if (startingIndex + RecordsPerPage <= UOACZPersistance.m_LifetimeTotalScores.Count)
                        allowScoreRecordsNext = true;

                    allowScoreRecordsPrevious = true;

                    if (player.m_UOACZAccountEntry.ScorePageNumber == 0)
                        allowScoreRecordsPrevious = false;

                    //Populate Public Records
                    for (int a = 0; a < RecordsPerPage; a++)
                    {
                        int adjustedIndex = startingIndex + a;

                        //Human
                        if (adjustedIndex <= UOACZPersistance.m_LifetimeHumanScores.Count - 1)
                        {
                            UOACZAccountEntry HumanEntry = UOACZPersistance.m_LifetimeHumanScores[adjustedIndex];

                            if (HumanEntry != null)
                            {
                                textHue = normalTextHue;
                                characterHue = UOACZSystem.blueTextHue;

                                if (player.m_UOACZAccountEntry == HumanEntry)
                                {
                                    textHue = UOACZSystem.greenTextHue;
                                    characterHue = UOACZSystem.greenTextHue;
                                }                               
                        
                                rankText = (adjustedIndex + 1).ToString();
                                nameText = HumanEntry.MostRecentPlayerString;
                                scoreText = HumanEntry.LifetimeHumanScore.ToString();

                                AddLabel(Utility.CenteredTextOffset(HumanRankX, rankText), startY, textHue, rankText);
                                AddLabel(Utility.CenteredTextOffset(HumanCharacterX, nameText), startY, characterHue, nameText);
                                AddLabel(Utility.CenteredTextOffset(HumanScoreX, scoreText), startY, textHue, scoreText);
                            }
                        }

                        //Undead
                        if (adjustedIndex <= UOACZPersistance.m_LifetimeUndeadScores.Count - 1)
                        {
                            UOACZAccountEntry UndeadEntry = UOACZPersistance.m_LifetimeUndeadScores[adjustedIndex];

                            if (UndeadEntry != null)
                            {
                                textHue = normalTextHue;
                                characterHue = UOACZSystem.redTextHue;

                                if (player.m_UOACZAccountEntry == UndeadEntry)
                                {
                                    textHue = UOACZSystem.greenTextHue;
                                    characterHue = UOACZSystem.greenTextHue;
                                }  

                                rankText = (adjustedIndex + 1).ToString();
                                nameText = UndeadEntry.MostRecentPlayerString;
                                scoreText = UndeadEntry.LifetimeUndeadScore.ToString();

                                AddLabel(Utility.CenteredTextOffset(UndeadRankX, rankText), startY, textHue, rankText);
                                AddLabel(Utility.CenteredTextOffset(UndeadCharacterX, nameText), startY, characterHue, nameText);
                                AddLabel(Utility.CenteredTextOffset(UndeadScoreX, scoreText), startY, textHue, scoreText);
                            }
                        }

                        //Total
                        if (adjustedIndex <= UOACZPersistance.m_LifetimeTotalScores.Count - 1)
                        {
                            UOACZAccountEntry TotalEntry = UOACZPersistance.m_LifetimeTotalScores[adjustedIndex];

                            if (TotalEntry != null)
                            {
                                textHue = normalTextHue;
                                characterHue = UOACZSystem.purpleTextHue;

                                if (player.m_UOACZAccountEntry == TotalEntry)
                                {
                                    textHue = UOACZSystem.greenTextHue;
                                    characterHue = UOACZSystem.greenTextHue;
                                } 

                                rankText = (adjustedIndex + 1).ToString();
                                nameText = TotalEntry.MostRecentPlayerString;
                                scoreText = TotalEntry.LifetimeTotalScore.ToString();

                                AddLabel(Utility.CenteredTextOffset(TotalRankX, rankText), startY, textHue, rankText);
                                AddLabel(Utility.CenteredTextOffset(TotalCharacterX, nameText), startY, characterHue, nameText);
                                AddLabel(Utility.CenteredTextOffset(TotalScoreX, scoreText), startY, textHue, scoreText);
                            }
                        }

                        startY += 25;
                    }
                break;

                case UOACZAccountEntry.ScorePageType.RewardsTomesUnlocks:
                    headerText = "UOACZ Rewards and Unlockables";

                    AddLabel(Utility.CenteredTextOffset(320, headerText), 16, boldTextHue, headerText);

                    AddLabel(130, 40, UOACZSystem.honorTextHue, "Tomes / Rewards");
                    AddLabel(25, 60, UOACZSystem.yellowTextHue, UOACZSystem.ParticipationRewardPoints.ToString() + " Points Earned For Session Total Score Above " + UOACZSystem.MinScoreToQualifyAsParticipant.ToString());
                    AddLabel(25, 80, UOACZSystem.yellowTextHue, UOACZSystem.HighestTotalScoreRewardPoints.ToString() + " Point Earned For Highest Session Total Score");

                    AddLabel(437, 40, UOACZSystem.lightPurpleTextHue, "UOACZ Unlockables");
                    AddLabel(410, 60, UOACZSystem.yellowTextHue, "Acquired in Dungeons as Loot");
                    AddLabel(410, 80, UOACZSystem.yellowTextHue, "Bonus UOACZ Starting Gear");

                    textHue = UOACZSystem.whiteTextHue; //normalTextHue

                    //Tomes - Rewards
                    int rewardIndex = player.m_UOACZAccountEntry.RewardPage * RewardsPerPage;
                    int rewardsAvailable = Enum.GetNames(typeof(UOACZRewardType)).Length;

                    int leftX = 45;
                    int rightX = 345;

                    startY = 115;
                    
                    if (player.m_UOACZAccountEntry.RewardPage > 0)
                        allowRewardsPrevious = true;

                    if (rewardIndex + RewardsPerPage < rewardsAvailable)
                        allowRewardsNext = true;
                                        
                    for (int a = 0; a < RewardsPerPage; a++)
                    {
                        rewardIndex = (player.m_UOACZAccountEntry.RewardPage * RewardsPerPage) + a;

                        if (rewardIndex < rewardsAvailable)
                        {
                            UOACZRewardType rewardType = (UOACZRewardType)rewardIndex;
                            UOACZRewardDetail rewardDetail = UOACZRewards.GetRewardDetail(rewardType);

                            AddItem(leftX + rewardDetail.OffsetX, startY + rewardDetail.OffsetY, rewardDetail.ItemId, rewardDetail.ItemHue);
                            AddButton(leftX + 45, startY + 5, 2151, 2154, 20 + a, GumpButtonType.Reply, 0);
                            AddLabel(leftX + 85, startY, UOACZSystem.whiteTextHue, rewardDetail.Name);
                            AddLabel(leftX + 85, startY + 20, normalTextHue, "Cost:");

                            if (rewardDetail.RewardCost >= 2)
                                AddLabel(leftX + 135, startY + 20, UOACZSystem.blueTextHue, rewardDetail.RewardCost.ToString() + " points");

                            else if (rewardDetail.RewardCost == 1)
                                AddLabel(leftX + 135, startY + 20, UOACZSystem.blueTextHue, rewardDetail.RewardCost.ToString() + " point");
                            else
                                AddLabel(leftX + 135, startY + 20, UOACZSystem.blueTextHue, "No Cost");

                            AddButton(leftX + 205, startY + 23, 2118, 2118, 30 + a, GumpButtonType.Reply, 0);
                            AddLabel(leftX + 225, startY + 20, UOACZSystem.lightGreenTextHue, "Info");
                        }

                        startY += 50;
                    }

                    int pointsAvailable = player.m_UOACZAccountEntry.RewardPoints;
                    AddLabel(leftX + 60, 370, UOACZSystem.blueTextHue, pointsAvailable.ToString() + " Points Available to Spend");

                    if (allowRewardsPrevious)
                        AddButton(leftX + 110, 395, 4014, 4016, 6, GumpButtonType.Reply, 0); //Previous

                    if (allowRewardsNext)
                        AddButton(leftX + 160, 395, 4005, 4007, 7, GumpButtonType.Reply, 0); //Next

                    //Unlockables
                    int unlockablesIndex = player.m_UOACZAccountEntry.UnlockablesPage * RewardsPerPage;
                    int unlockablesAvailable = Enum.GetNames(typeof(UOACZUnlockableType)).Length;

                    startY = 115;

                     if (player.m_UOACZAccountEntry.UnlockablesPage > 0)
                        allowUnlockablesPrevious = true;

                     if (unlockablesIndex + RewardsPerPage < unlockablesAvailable)
                         allowUnlockablesNext = true;

                    for (int a = 0; a < RewardsPerPage; a++)
                    {
                        unlockablesIndex = (player.m_UOACZAccountEntry.UnlockablesPage * RewardsPerPage) + a;

                        if (unlockablesIndex < unlockablesAvailable)
                        {
                            UOACZUnlockableType unlockableType = (UOACZUnlockableType)unlockablesIndex;
                            UOACZUnlockableDetail unlockableDetail = UOACZUnlockables.GetUnlockableDetail(unlockableType);
                            UOACZUnlockableDetailEntry unlockableDetailEntry = UOACZUnlockables.GetUnlockableDetailEntry(player, unlockableType);
                            
                            bool unlocked = false;
                            bool active = false;
                            string textStatus = "Not Acquired";
                            
                            if (unlockableDetailEntry != null)
                            {
                                unlocked = true;
                                active = unlockableDetailEntry.m_Active;
                            }

                            textHue = UOACZSystem.whiteTextHue;

                            if (unlocked)
                            {
                                if (active)
                                {
                                    textStatus = "Active";
                                    AddButton(rightX + 50, startY, 2154, 2151, 40 + a, GumpButtonType.Reply, 0);

                                    textHue = UOACZSystem.greenTextHue;
                                }

                                else
                                {
                                    textStatus = "Disabled";
                                    AddButton(rightX + 50, startY, 2151, 2154, 40 + a, GumpButtonType.Reply, 0);
                                }
                            }

                            else
                                AddButton(rightX + 50, startY, 9721, 9721, 40 + a, GumpButtonType.Reply, 0);

                            AddItem(rightX + unlockableDetail.OffsetX, startY + unlockableDetail.OffsetY, unlockableDetail.ItemId, unlockableDetail.ItemHue);
                            AddLabel(rightX + 90, startY, textHue, unlockableDetail.Name);
                            AddLabel(rightX + 90, startY + 20, normalTextHue, textStatus);
                            AddButton(rightX + 180, startY + 23, 2118, 2118, 50 + a, GumpButtonType.Reply, 0);
                            AddLabel(rightX + 205, startY + 20, UOACZSystem.lightGreenTextHue, "Info");
                        }

                        startY += 50;
                    }

                    if (allowUnlockablesPrevious)
                        AddButton(rightX + 60, 395, 4014, 4016, 8, GumpButtonType.Reply, 0); //Previous

                    if (allowUnlockablesNext)
                        AddButton(rightX + 110, 395, 4005, 4007, 9, GumpButtonType.Reply, 0); //Next	 		

                break;

                case UOACZAccountEntry.ScorePageType.Admin:
                    headerText = "UOACZ Admin";

                    AddLabel(Utility.CenteredTextOffset(320, headerText), 16, boldTextHue, headerText);
                    AddLabel(Utility.CenteredTextOffset(320, nextSessionText), 34, normalTextHue, nextSessionText);
                break;
            }            

            //Page Buttons
            if (player.m_UOACZAccountEntry.ScorePage == UOACZAccountEntry.ScorePageType.Lifetime)
            {
                AddLabel(28, 440, boldTextHue, "Lifetime Totals");
                AddButton(65, 460, 9724, 9721, 1, GumpButtonType.Reply, 0);
            }

            else
            {
                AddLabel(28, 440, normalTextHue, "Lifetime Totals");
                AddButton(65, 460, 9721, 9724, 1, GumpButtonType.Reply, 0);
            }

            if (player.m_UOACZAccountEntry.ScorePage == UOACZAccountEntry.ScorePageType.Best)
            {
                AddLabel(185, 440, boldTextHue, "Top Sessions");
                AddButton(210, 460, 9724, 9721, 2, GumpButtonType.Reply, 0);
            }

            else
            {
                AddLabel(185, 440, normalTextHue, "Top Sessions");
                AddButton(210, 460, 9721, 9724, 2, GumpButtonType.Reply, 0);
            }

            if (player.m_UOACZAccountEntry.ScorePage == UOACZAccountEntry.ScorePageType.Previous)
            {
                AddLabel(320, 440, boldTextHue, "Most Recent Session");
                AddButton(370, 460, 9724, 9721, 3, GumpButtonType.Reply, 0);
            }

            else
            {
                AddLabel(320, 440, normalTextHue, "Most Recent Session");
                AddButton(370, 460, 9721, 9724, 3, GumpButtonType.Reply, 0);
            }            

            if (player.m_UOACZAccountEntry.ScorePage == UOACZAccountEntry.ScorePageType.RewardsTomesUnlocks)
            {
                AddLabel(462, 440, boldTextHue, "Rewards / Unlockables");
                AddButton(515, 460, 9724, 9721, 4, GumpButtonType.Reply, 0);
            }

            else
            {
                AddLabel(462, 440, normalTextHue, "Rewards / Unlockables");
                AddButton(515, 460, 9721, 9724, 4, GumpButtonType.Reply, 0);
            }

            if (player.AccessLevel > AccessLevel.Player)
            {
                if (player.m_UOACZAccountEntry.ScorePage == UOACZAccountEntry.ScorePageType.Admin)
                {
                    AddLabel(535, 415, boldTextHue, "Admin");
                    AddButton(578, 413, 9724, 9721, 5, GumpButtonType.Reply, 0);
                }

                else
                {
                    AddLabel(535, 415, normalTextHue, "Admin");
                    AddButton(578, 413, 9721, 9724, 5, GumpButtonType.Reply, 0);
                }
            }

            //Previous
            if (allowScoreRecordsPrevious)
            {
                AddButton(185, 415, 4014, 4016, 6, GumpButtonType.Reply, 0);
                AddLabel(220, 415, normalTextHue, "Previous");
            }

            //Next
            if (allowScoreRecordsNext)
            {
                AddButton(385, 415, 4005, 4007, 7, GumpButtonType.Reply, 0);
                AddLabel(425, 415, normalTextHue, "Next");
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (from == null) return;
            if (from.Deleted) return;
            if (player == null) return;
            if (player.Deleted) return;

            bool resendGump = false;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            //Score Pages
            if (player.m_UOACZAccountEntry.ScorePage == UOACZAccountEntry.ScorePageType.Previous || player.m_UOACZAccountEntry.ScorePage == UOACZAccountEntry.ScorePageType.Lifetime || player.m_UOACZAccountEntry.ScorePage == UOACZAccountEntry.ScorePageType.Best)
            {
                if (info.ButtonID == 6)
                {
                    if (allowScoreRecordsPrevious)
                    {
                        player.m_UOACZAccountEntry.ScorePageNumber--;
                        resendGump = true;
                    }
                }

                if (info.ButtonID == 7)
                {
                    if (allowScoreRecordsNext)
                    {
                        player.m_UOACZAccountEntry.ScorePageNumber++;
                        resendGump = true;
                    }
                }
            }

            //Rewards and Unlockables
            if (player.m_UOACZAccountEntry.ScorePage == UOACZAccountEntry.ScorePageType.RewardsTomesUnlocks)
            {
                if (info.ButtonID == 6)
                {
                    if (allowRewardsPrevious)
                    {
                        player.SendSound(UOACZSystem.changeGumpSound);

                        player.m_UOACZAccountEntry.RewardPage--;
                        resendGump = true;
                    }
                }

                if (info.ButtonID == 7)
                {
                    if (allowRewardsNext)
                    {
                        player.SendSound(UOACZSystem.changeGumpSound);

                        player.m_UOACZAccountEntry.RewardPage++;
                        resendGump = true;
                    }
                }

                if (info.ButtonID == 8)
                {
                    if (allowUnlockablesPrevious)
                    {
                        player.SendSound(UOACZSystem.changeGumpSound);

                        player.m_UOACZAccountEntry.UnlockablesPage--;
                        resendGump = true;
                    }
                }

                if (info.ButtonID == 9)
                {
                    if (allowUnlockablesNext)
                    {
                        player.SendSound(UOACZSystem.changeGumpSound);

                        player.m_UOACZAccountEntry.UnlockablesPage++;
                        resendGump = true;
                    }
                }

                //Reward: Purchase
                if (info.ButtonID >= 20 && info.ButtonID < 30)
                {
                    int rewardIndex = (info.ButtonID - 20) + (player.m_UOACZAccountEntry.RewardPage * RewardsPerPage);
                    int rewardsAvailable = Enum.GetNames(typeof(UOACZRewardType)).Length;

                    if (rewardIndex >= rewardsAvailable)                    
                        return;

                    UOACZRewardType rewardType = (UOACZRewardType)rewardIndex;

                    UOACZRewards.AttemptPurchase(player, rewardType);

                    resendGump = true;
                }

                //Reward: Info
                if (info.ButtonID >= 30 && info.ButtonID < 40)
                {
                    int rewardIndex = (info.ButtonID - 30) + (player.m_UOACZAccountEntry.RewardPage * RewardsPerPage);
                    int rewardsAvailable = Enum.GetNames(typeof(UOACZRewardType)).Length;

                    if (rewardIndex >= rewardsAvailable)
                        return;

                    UOACZRewardType rewardType = (UOACZRewardType)rewardIndex;
                    UOACZRewardDetail rewardDetail = UOACZRewards.GetRewardDetail(rewardType);

                    string description = "";

                    if (rewardDetail.Description != null)
                    {
                        for (int a = 0; a < rewardDetail.Description.Length; a++)
                            description += rewardDetail.Description[a];
                    }

                    player.SendMessage(UOACZSystem.yellowTextHue, description);

                    resendGump = true;
                }

                //Unlockable: Activate
                if (info.ButtonID >= 40 && info.ButtonID < 50)
                {
                    int unlockableIndex = (info.ButtonID - 40) + (player.m_UOACZAccountEntry.UnlockablesPage * RewardsPerPage);
                    int unlockablesAvailable = Enum.GetNames(typeof(UOACZUnlockableType)).Length;

                    if (unlockableIndex >= unlockablesAvailable)
                        return;

                    UOACZUnlockableType unlockableType = (UOACZUnlockableType)unlockableIndex;
                    UOACZUnlockableDetail unlockableDetail = UOACZUnlockables.GetUnlockableDetail(unlockableType);
                    UOACZUnlockableDetailEntry unlockableDetailEntry = UOACZUnlockables.GetUnlockableDetailEntry(player, unlockableType);

                    bool deactivatedOthers = false;

                    if (unlockableDetailEntry != null)
                    {
                        if (unlockableDetailEntry.m_Unlocked)
                        {
                            if (!unlockableDetailEntry.m_Active)
                            {
                                unlockableDetailEntry.m_Active = true;
                                player.SendSound(UOACZSystem.selectionSound);

                                if (unlockableDetail.UnlockableCategory == UOACZUnlockableCategory.UndeadDye)
                                {
                                    player.SendMessage("You activate the unlockable.");
                                }

                                else
                                {
                                    foreach (UOACZUnlockableDetailEntry otherUnlockableEntry in player.m_UOACZAccountEntry.m_Unlockables)
                                    {
                                        if (otherUnlockableEntry.m_UnlockableType == unlockableType)
                                            continue;

                                        UOACZUnlockableDetail otherUnlockableDetail = UOACZUnlockables.GetUnlockableDetail(otherUnlockableEntry.m_UnlockableType);

                                        if (otherUnlockableDetail.UnlockableCategory == unlockableDetail.UnlockableCategory)
                                        {
                                            if (otherUnlockableEntry.m_Unlocked)
                                            {
                                                otherUnlockableEntry.m_Active = false;
                                                deactivatedOthers = true;
                                            }
                                        }
                                    }

                                    string categoryName = UOACZUnlockables.GetCategoryName(unlockableDetail.UnlockableCategory);

                                    player.SendMessage("You set the item as your unlockable for the category: " + categoryName);
                                }
                            }

                            else
                            {
                                unlockableDetailEntry.m_Active = false;
                                player.SendSound(UOACZSystem.selectionSound);
                            }
                        }
                    }
                    
                    resendGump = true;
                }

                //Unlockable: Info
                if (info.ButtonID >= 50 && info.ButtonID < 60)
                {
                    int unlockableIndex = (info.ButtonID - 50) + (player.m_UOACZAccountEntry.UnlockablesPage * RewardsPerPage);
                    int unlockablesAvailable = Enum.GetNames(typeof(UOACZUnlockableType)).Length;

                    if (unlockableIndex >= unlockablesAvailable)
                        return;

                    UOACZUnlockableType unlockableType = (UOACZUnlockableType)unlockableIndex;
                    UOACZUnlockableDetail unlockableDetail = UOACZUnlockables.GetUnlockableDetail(unlockableType);

                    string description = "";

                    if (unlockableDetail.Description != null)
                    {
                        for (int a = 0; a < unlockableDetail.Description.Length; a++)
                            description += unlockableDetail.Description[a];
                    }

                    string categoryName = UOACZUnlockables.GetCategoryName(unlockableDetail.UnlockableCategory);

                    description += " [Category: " + categoryName + "]";

                    player.SendMessage(UOACZSystem.yellowTextHue, description);

                    resendGump = true;
                }
            }
            
            //Navigation
            if (info.ButtonID == 1)
            {
                player.m_UOACZAccountEntry.ScorePage = UOACZAccountEntry.ScorePageType.Lifetime;
                player.m_UOACZAccountEntry.ScorePageNumber = 0;

                player.SendSound(UOACZSystem.changeGumpSound);

                resendGump = true;
            }

            else if (info.ButtonID == 2)
            {
                player.m_UOACZAccountEntry.ScorePage = UOACZAccountEntry.ScorePageType.Best;
                player.m_UOACZAccountEntry.ScorePageNumber = 0;

                player.SendSound(UOACZSystem.changeGumpSound);

                resendGump = true;
            }

            else if (info.ButtonID == 3)
            {
                player.m_UOACZAccountEntry.ScorePage = UOACZAccountEntry.ScorePageType.Previous;
                player.m_UOACZAccountEntry.ScorePageNumber = 0;

                player.SendSound(UOACZSystem.changeGumpSound);

                resendGump = true;
            }

            else if (info.ButtonID == 4)
            {
                player.m_UOACZAccountEntry.ScorePage = UOACZAccountEntry.ScorePageType.RewardsTomesUnlocks;
                player.m_UOACZAccountEntry.ScorePageNumber = 0;

                player.SendSound(UOACZSystem.changeGumpSound);

                resendGump = true;
            }

            else if (info.ButtonID == 5)
            {
                if (player.AccessLevel > AccessLevel.Player)
                {
                    player.m_UOACZAccountEntry.ScorePage = UOACZAccountEntry.ScorePageType.Admin;
                    player.m_UOACZAccountEntry.ScorePageNumber = 0;

                    player.SendSound(UOACZSystem.changeGumpSound);

                    resendGump = true;
                }
            }

            if (resendGump)
            {
                player.CloseGump(typeof(UOACZScoreGump));
                player.SendGump(new UOACZScoreGump(player));
            }

            else            
                player.SendSound(UOACZSystem.closeGumpSound);            
        }
    }
}