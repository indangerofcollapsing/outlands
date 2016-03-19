using System;
using Server.Items;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using Server.Achievements;

namespace Server
{
    public class UOACZAchievements
    {
        public static List<AchievementTriggers> m_AchievementsList = new List<AchievementTriggers>()
        {
            AchievementTriggers.Trigger_UOACZKillZombies,
            AchievementTriggers.Trigger_UOACZCorruptWildlife,
            AchievementTriggers.Trigger_UOACZScoreFromKillingUndead,
            AchievementTriggers.Trigger_UOACZScoreFromKillingHumans,
            AchievementTriggers.Trigger_UOACZCorruptionAbilities,
            AchievementTriggers.Trigger_UOACZKillUndeadChampion,
            AchievementTriggers.Trigger_UOACZKillUndeadBoss,
            AchievementTriggers.Trigger_UOACZKillHumanChampion,
            AchievementTriggers.Trigger_UOACZKillHumanBoss,
            AchievementTriggers.Trigger_UOACZUseTunnels,
            AchievementTriggers.Trigger_UOACZMurderAnotherHuman,
            AchievementTriggers.Trigger_UOACZKillUndeadPlayer,
            AchievementTriggers.Trigger_UOACZKillHumanPlayer,
            AchievementTriggers.Trigger_UOACZRepairObjects,
            AchievementTriggers.Trigger_UOACZHumanVictory,
            AchievementTriggers.Trigger_UOACZUndeadVictory,
            AchievementTriggers.Trigger_UOACZNightwalker,
            AchievementTriggers.Trigger_UOACZMonsterTierLevel5,
            AchievementTriggers.Trigger_UOACZConsumeBrains,
            AchievementTriggers.Trigger_UOACZPurchaseUpgradesWithoutShuffles,
            AchievementTriggers.Trigger_UOACZPurchaseDifferentHumanUpgrades,
            AchievementTriggers.Trigger_UOACZHighestSessionTotalScore,
            AchievementTriggers.Trigger_UOACZDamageObjects,
            AchievementTriggers.Trigger_UOACZCraftingSkill,
            AchievementTriggers.Trigger_UOACZHarvesting,
            AchievementTriggers.Trigger_UOACZScavenging,
            AchievementTriggers.Trigger_UOACZStarve,
            AchievementTriggers.Trigger_UOACZDieOfThirst,
            AchievementTriggers.Trigger_UOACZKillCreaturesWithFirefields,
            AchievementTriggers.Trigger_UOACZCauseUndeadTransformation,
            AchievementTriggers.Trigger_UOACZBuildAndPlaceFortifications,
            AchievementTriggers.Trigger_UOACZHumanSurvival1,
            AchievementTriggers.Trigger_UOACZHumanSurvival2,
            AchievementTriggers.Trigger_UOACZHumanSurvival3,
            AchievementTriggers.Trigger_UOACZUndeadSwarmSize15,
            AchievementTriggers.Trigger_UOACZUndeadAddToSwarm,
            AchievementTriggers.Trigger_UOACZSnaresTrapsNets,
            AchievementTriggers.Trigger_UOACZKillWithDifferentForms1, 
            AchievementTriggers.Trigger_UOACZKillWithDifferentForms2,  
            AchievementTriggers.Trigger_UOACZKillWithDifferentForms3,  
        };
    }
}