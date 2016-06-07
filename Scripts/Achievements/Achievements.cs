using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server
{
    public enum AchievementCategory
    {
        Warfare,
        Seafaring,
        AnimalTaming,
        Crafting,

        Adventuring,
        Luxury,
        Harvesting,
        SkillMastery,

        Slaying,
        Competition,
        Virtue,
        Vice
    }
    
    public enum Achievement
    {
        SinkOrSwim,
        WalkThePlank,
        HighTide,
        AFewGoodMen,
        Circumnavigate,
        Stormchaser,
        BoardToDeath,
        ForSail,
        ToTheDepths,
        GunsAndRum,
        BackFromTheBrink,
        GhostPirate,
        TESTA,
        TESTB,
        TESTC
    }

    public class Achievements
    {
        public static int GetTotalAchievementsCount()
        {
            int achievementsTotal = Enum.GetNames(typeof(Achievement)).Length;

            return achievementsTotal;
        }

        public static int GetCompletedAchievementCount(PlayerMobile player)
        {
            if (player == null)
                return 0;

            AchievementsPersistance.CheckAndCreateAchievementAccountEntry(player);

            int completedAchievements = 0;

            foreach (AchievementEntry entry in player.m_AchievementAccountEntry.m_Achievements)
            {
                if (entry == null) 
                    continue;

                if (entry.m_Completed)
                    completedAchievements++;
            }

            return completedAchievements;
        }

        public static int GetProgressBarBackground(double completionPercentage)
        {
            int progressItemId = 2053;

            if (completionPercentage >= .33)
                progressItemId = 2057;

            if (completionPercentage >= .66)
                progressItemId = 2054;

            if (completionPercentage >= 1.0)
                progressItemId = 2056;

            return progressItemId;
        }

        public static int GetCompletedCategoryAchievements(PlayerMobile player, AchievementCategory category)
        {
            AchievementsPersistance.CheckAndCreateAchievementAccountEntry(player);

            int completionCount = 0;

            List<Achievement> categoryAchievements = GetCategoryAchievements(category);

            foreach (Achievement achievement in categoryAchievements)
            {
                foreach (AchievementEntry entry in player.m_AchievementAccountEntry.m_Achievements)
                {
                    if (entry == null) continue;
                    if (entry.m_Achievement == achievement && entry.m_Completed)
                    {
                        completionCount++;
                        break;
                    }
                }
            }

            return completionCount;
        }

        public static List<Achievement> GetCategoryAchievements(AchievementCategory category)
        {
            List<Achievement> achievementsList = new List<Achievement>();

            switch (category)
            {
                case AchievementCategory.Seafaring:
                    achievementsList.Add(Achievement.SinkOrSwim);
                    achievementsList.Add(Achievement.WalkThePlank);
                    achievementsList.Add(Achievement.HighTide);
                    achievementsList.Add(Achievement.AFewGoodMen);
                    achievementsList.Add(Achievement.Circumnavigate);
                    achievementsList.Add(Achievement.Stormchaser);
                    achievementsList.Add(Achievement.BoardToDeath);
                    achievementsList.Add(Achievement.ForSail);
                    achievementsList.Add(Achievement.ToTheDepths);
                    achievementsList.Add(Achievement.GunsAndRum);
                    achievementsList.Add(Achievement.BackFromTheBrink);
                    achievementsList.Add(Achievement.GhostPirate);

                    achievementsList.Add(Achievement.TESTA);
                    achievementsList.Add(Achievement.TESTB);
                    achievementsList.Add(Achievement.TESTC);  
                break;
            }

            return achievementsList;
        }

        public static AchievementEntry GetAchievementEntry(PlayerMobile player, Achievement achievement)
        {
            if (player == null)
                return null;

            AchievementsPersistance.CheckAndCreateAchievementAccountEntry(player);

            foreach (AchievementEntry entry in player.m_AchievementAccountEntry.m_Achievements)
            {
                if (entry.m_Achievement == achievement)
                    return entry;
            }

            AchievementEntry newEntry = new AchievementEntry(achievement, 0);
            AchievementDetail entryDetail = GetAchievementDetail(achievement);
            
            if (entryDetail.m_Stage > 1)
            {
                bool allCompleted = true;

                for (int a = 0; a < entryDetail.m_Stages.Length; a++)
                {
                    if (entryDetail.m_Stages[a] == achievement)
                        break;

                    Achievement priorAchievement = entryDetail.m_Stages[a];
                    AchievementEntry priorEntry = Achievements.GetAchievementEntry(player, priorAchievement);

                    if (priorEntry == null)
                        continue;

                    if (!priorEntry.m_Completed)
                    {
                        allCompleted = false;
                        break;
                    }
                }

                if (allCompleted)
                    newEntry.m_Unlocked = true;
            }

            else
                newEntry.m_Unlocked = true;

            player.m_AchievementAccountEntry.m_Achievements.Add(newEntry);

            return newEntry;
        }

        public static void UpdateAchievement(PlayerMobile player, Achievement achievement, int progressAmount)
        {
            if (player == null)
                return;

            AchievementsPersistance.CheckAndCreateAchievementAccountEntry(player);

            AchievementEntry entry = GetAchievementEntry(player, achievement);
            AchievementDetail detail = GetAchievementDetail(achievement);

            if (entry == null) return;
            if (detail == null) return;           

            //Multi-Staged Achievement: Get Current Stage
            if (detail.m_Stages.Length > 0)
            {
                for (int a = 0; a < detail.m_Stages.Length; a++)
                {
                    Achievement achievementStage = detail.m_Stages[a];
                    
                    AchievementEntry stageEntry = GetAchievementEntry(player, achievementStage);
                    AchievementDetail stageDetail = GetAchievementDetail(achievementStage);

                    if (stageEntry == null) continue;
                    if (stageDetail == null) continue;

                    if (stageEntry.m_Completed || !stageDetail.Enabled)
                        continue;

                    entry = stageEntry;
                    detail = stageDetail;

                    break;
                }
            }

            if (entry.m_Completed || !detail.Enabled)
                return;

            entry.m_Progress += progressAmount;           

            if (entry.m_Progress >= detail.m_ProgressNeeded)
            {
                entry.m_Progress = detail.m_ProgressNeeded;
                CompleteAchievement(player, achievement);
            }            
        }

        public static void CompleteAchievement(PlayerMobile player, Achievement achievement)
        {
            AchievementsPersistance.CheckAndCreateAchievementAccountEntry(player);

            AchievementEntry entry = GetAchievementEntry(player, achievement);
            AchievementDetail detail = GetAchievementDetail(achievement);

            if (entry == null) return;
            if (detail == null) return; 

            entry.m_Progress = detail.m_ProgressNeeded;
            entry.m_Completed = true;

            //Unlock Next Achievement if Multiple Stages Exist
            int achievementIndex = detail.m_Stage - 1;
            int stageCount = 1;

            if (detail.m_Stages != null)            
                stageCount = detail.m_Stages.Length;

            if (detail.m_Stage < stageCount)
            {
                if (achievementIndex + 1 < detail.m_Stages.Length)
                {
                    Achievement nextAchievement = detail.m_Stages[achievementIndex + 1];
                    AchievementEntry nextAchievementEntry = GetAchievementEntry(player, achievement);

                    if (nextAchievementEntry != null)
                        nextAchievementEntry.m_Unlocked = true;
                }
            }
        }

        public static void ClaimAchievement(PlayerMobile player, Achievement achievement)
        {
            AchievementsPersistance.CheckAndCreateAchievementAccountEntry(player);

            AchievementEntry entry = GetAchievementEntry(player, achievement);
            AchievementDetail detail = GetAchievementDetail(achievement);
            
            //TEST
            player.Say("Claiming achievement for: " + detail.m_DisplayName);
            
            entry.m_Claimed = true;            
        }

        public static AchievementDetail GetAchievementDetail(Achievement achievement)
        {
            AchievementDetail detail = new AchievementDetail();

            detail.m_Achievement = achievement;

            switch (achievement)
            {
                #region Seafaring

                case Achievement.SinkOrSwim:                    
                    detail.m_Category = AchievementCategory.Seafaring;

                    detail.m_Stages = new Achievement[] { Achievement.SinkOrSwim, Achievement.WalkThePlank, Achievement.HighTide };

                    detail.m_ProgressNeeded = 1;

                    detail.m_DisplayName = "Sink or Swim";
                    detail.m_Description = new string[] {"Kill 1,000 enemy ship combatants"};                     

                    detail.m_RewardName = "Driftwood";
                    detail.m_RewardDescription = "Receive a decorative piece of driftwood";

                    detail.m_RewardItemID = 2500;
                    detail.m_RewardItemHue = 0;
                    detail.m_RewardItemOffsetX = 0;
                    detail.m_RewardItemOffsetY = 0;
                break;

                case Achievement.WalkThePlank:
                    detail.m_Category = AchievementCategory.Seafaring;

                    detail.m_Stages = new Achievement[] { Achievement.SinkOrSwim, Achievement.WalkThePlank, Achievement.HighTide };

                    detail.m_ProgressNeeded = 1;

                    detail.m_DisplayName = "Walk the Plank";
                    detail.m_Description = new string[] { "Kill 5,000 enemy ship combatants" };

                    detail.m_RewardName = "Driftwood 2";
                    detail.m_RewardDescription = "Receive a 2nd decorative piece of driftwood";

                    detail.m_RewardItemID = 2500;
                    detail.m_RewardItemHue = 0;
                    detail.m_RewardItemOffsetX = 0;
                    detail.m_RewardItemOffsetY = 0;
                break;

                case Achievement.HighTide:
                    detail.m_Category = AchievementCategory.Seafaring;

                    detail.m_Stages = new Achievement[] { Achievement.SinkOrSwim, Achievement.WalkThePlank, Achievement.HighTide };

                    detail.m_ProgressNeeded = 1;

                    detail.m_DisplayName = "High Tide";
                    detail.m_Description = new string[] { "Kill 25,000 enemy ship combatants" };

                    detail.m_RewardName = "Driftwood 3";
                    detail.m_RewardDescription = "Receive a 3rd decorative piece of driftwood";

                    detail.m_RewardItemID = 2500;
                    detail.m_RewardItemHue = 0;
                    detail.m_RewardItemOffsetX = 0;
                    detail.m_RewardItemOffsetY = 0;
                break;

                case Achievement.TESTA:
                detail.m_Category = AchievementCategory.Seafaring;

                detail.m_Stages = new Achievement[] {};

                detail.m_ProgressNeeded = 1;

                detail.m_DisplayName = "TEST A";
                detail.m_Description = new string[] { "Kill 25,000 enemy ship combatants" };

                detail.m_RewardName = "Driftwood A";
                detail.m_RewardDescription = "Receive a 3rd decorative piece of driftwood";

                detail.m_RewardItemID = 2500;
                detail.m_RewardItemHue = 0;
                detail.m_RewardItemOffsetX = 0;
                detail.m_RewardItemOffsetY = 0;
                break;

                #endregion
            }

            if (detail.m_Stages == null)
                detail.m_Stage = 1;

            else if (detail.m_Stages.Length == 0)
                detail.m_Stage = 1;

            else
            {
                for (int a = 0; a < detail.m_Stages.Length; a++)
                {
                    if (detail.m_Stages[a] == detail.m_Achievement)
                    {
                        detail.m_Stage = a + 1;
                        break;
                    }
                }
            }

            return detail;
        }
    }    

    public class AchievementDetail
    {
        public bool Enabled = true;

        public Achievement m_Achievement;
        public AchievementCategory m_Category = AchievementCategory.Adventuring;

        public int m_Stage = 1;
        public Achievement[] m_Stages = new Achievement[] { };

        public int m_ProgressNeeded = 1;

        public string m_DisplayName = "Achievement Name";
        public string[] m_Description = new string[] {"Achievement Description" };

        public string m_RewardName = "Reward Name";
        public string m_RewardDescription = "Reward Description";

        public int m_RewardItemID = 2500;
        public int m_RewardItemHue = 0;
        public int m_RewardItemOffsetX = 0;
        public int m_RewardItemOffsetY = 0;

        public AchievementDetail()
        {            
        }
    }

    public class AchievementEntry
    {
        public Achievement m_Achievement;
        public int m_Progress = 0;

        public bool m_Unlocked = false;
        public bool m_Completed = false;
        public bool m_Claimed = false;

        public AchievementEntry(Achievement achievement, int progress)
        {
            m_Achievement = achievement;
            m_Progress = progress;
        }
    }
}