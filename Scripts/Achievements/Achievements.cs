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
        Misc,
        Crafting,
    }
    
    public enum Achievement
    {
        ReachGMCrafting,
        ReachGMCrafting2,
        ReachGMCrafting3
    }

    public class Achievements
    {
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

            entry.m_Progress = detail.m_ProgressNeeded;
            entry.m_Completed = true;            
        }

        public static void ClaimAchievement(PlayerMobile player, Achievement achievement)
        {
            AchievementsPersistance.CheckAndCreateAchievementAccountEntry(player);

            AchievementEntry entry = GetAchievementEntry(player, achievement);
            AchievementDetail detail = GetAchievementDetail(achievement);

            //Distribute Reward
            
            entry.m_Claimed = true;            
        }

        public static AchievementDetail GetAchievementDetail(Achievement achievement)
        {
            AchievementDetail detail = new AchievementDetail();

            detail.m_Achievement = achievement;

            switch (achievement)
            {
                #region Group: Crafting

                case Achievement.ReachGMCrafting:                    
                    detail.m_AchievementGroup = AchievementCategory.Crafting;

                    detail.m_Stages = new Achievement[] { Achievement.ReachGMCrafting, Achievement.ReachGMCrafting2, Achievement.ReachGMCrafting3 };

                    detail.m_ProgressNeeded = 1;

                    detail.m_DisplayName = "Reach GM Crafting";
                    detail.m_Description = "Reach 100.0 crafting in any crafting skill";                     

                    detail.m_RewardName = "Title: Master Crafter";
                    detail.m_RewardDescription = "Earn the title of Master Crafter (permanent)";

                    detail.m_RewardItemID = 2500;
                    detail.m_RewardItemHue = 0;
                    detail.m_RewardItemOffsetX = 0;
                    detail.m_RewardItemOffsetY = 0;
                break;

                #endregion
            }

            return detail;
        }
    }    

    public class AchievementDetail
    {
        public bool Enabled = true;

        public Achievement m_Achievement;
        public AchievementCategory m_AchievementGroup = AchievementCategory.Misc;

        public Achievement[] m_Stages = new Achievement[] { };

        public int m_ProgressNeeded = 1;

        public string m_DisplayName = "Achievement Name";
        public string m_Description = "Achievement Description";

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

        public bool m_Completed = false;
        public bool m_Claimed = false;

        public AchievementEntry(Achievement achievement, int progress)
        {
            m_Achievement = achievement;
            m_Progress = progress;
        }
    }
}