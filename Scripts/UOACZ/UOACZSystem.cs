﻿using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Misc;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Achievements;
using Server.Accounting;
using System.Linq;
using Server.Custom;
using Server.Spells;

namespace Server
{
    public static class UOACZSystem
    {
        public enum UOACZStatType
        {
            Hunger,
            Thirst,
            Humanity,
            Honor,
            HumanScore,
            SurvivalPoints,
            HumanUpgradePoints,
            UndeadScore,
            CorruptionPoints,
            UndeadUpgradePoints,
            MonsterTier
        }

        public enum StockpileContributionType
        {
            None,
            Alchemist,
            Weaver,
            Farmer,
            Fisherman,
            Skinner,
            Baker,
            Butcher,
            Bowyer,
            Provisioner,
            Brewmaster,
            Carpenter,
            Miner,
            Healer,
            Tinker,
            Blacksmith,
            Merchant,
            Noble
        }
        
        public static TimeSpan TeamSwitchCooldown = TimeSpan.FromMinutes(30);
        public static TimeSpan TeamSwitchPvPThreshold = TimeSpan.FromSeconds(60);

        public static TimeSpan TunnelDigPvPThreshold = TimeSpan.FromSeconds(60);

        public static Double FatigueBaselineScalar = 1.0;
        public static Double FatigueActiveScalar = .25;
        public static TimeSpan FatigueDuration = TimeSpan.FromMinutes(10);

        public static double HumanPlayerVsPlayerDamageScalar = 1.0;
        public static double UndeadPlayerVsPlayerDamageScalar = .8;

        public static double UndeadPlayerVsCreatureDamageScalar = 1;
        public static double HumanPlayerVsCreatureDamageScalar = 1.5;

        public static double TamedCreatureVsCreatureDamageScalar = 2;
        public static double CreatureVsTamedCreatureDamageScalar = .75;
        public static double SpellDamageScalar = .5;

        public static double HitsRegen = 2;
        public static double StamRegen = 1;
        public static double ManaRegenScalar = .5;

        public static int RawMeatDiseaseAmount = 5;
        public static int CorruptedMeatDiseaseAmount = 15;
        public static int FoodDiseaseSeconds = 60;

        public static TimeSpan HideCooldown = TimeSpan.FromSeconds(60);

        public static double TinkerTrapDuration = 3600;

        public static TimeSpan MinimumDelayBetweenHumanAbilities = TimeSpan.FromSeconds(1);
        public static TimeSpan MinimumDelayBetweenUndeadAbilities = TimeSpan.FromSeconds(1);

        public static int AbilityManaCost = 10;

        public static double AbilityInitialDelaySeconds = .75;

        public static double UndeadItemCooldownSeconds = 60;
        
        public static TimeSpan ScavengeExpiration { get { return TimeSpan.FromMinutes(1000); } }
        public static TimeSpan AfterScavengeDeletion = TimeSpan.FromSeconds(5);

        public static double scavengeSkillScalar = 2.0;

        public static int StartingThreatLevel = 100;
        public static int MinutesPerThreatLevelIncrease = 3;        
       
        public static TimeSpan DelayBetweenMoongateActivation = TimeSpan.FromMinutes(5);

        public static TimeSpan CombatDelayBeforeEnteringMoongate = TimeSpan.FromSeconds(60);
        public static TimeSpan CombatDelayBeforeExitingMoongate = TimeSpan.FromSeconds(0);

        public static TimeSpan CombatDelayBeforeStockpileAccess = TimeSpan.FromSeconds(0);

        public static int TunnelOpeningMinutes = 4;

        public static double SpawnAtPlayerWeakCreatureChance = .33;
        public static double SpawnAtPlayerChance = .33;
        public static int SpawnAtPlayerMinutes = 1;
        public static int SpawnAtPlayerMinDelaySeconds = 0;
        public static int SpawnAtPlayerMaxDelaySeconds = 50;

        public static int RespawnFirstWaveWarningMinutes = 5;
        public static int RespawnSecondWaveWarningMinutes = 1;
        public static int RespawnWaveMinutes = 30;

        public static double SwarmControllerDeathDamageScalar = .5;

        public static int HumanChampionSpawnMinutes = 90;
        public static int HumanBossSpawnMinutes = 180;
        public static int UndeadChampionSpawnMinutes = 90;
        public static int UndeadBossSpawnMinutes = 180;

        public static double HumanBalanceHandicapBonus = .05;
        public static double UndeadBalanceHandicapBonus = 0;

        public static int ObjectivesNeededForVictory = 4;

        public static int HumanObjectiveBonesNeeded = 1000;
        public static int UndeadObjectiveRequiredStockpilesDestroyed = 4;
        public static int UndeadObjectiveSpreadCorruption = 200;
        public static double UndeadObjectivePercentOfCivilians = .80;

        public static int HumanObjective1UpgradePointsGranted = 2;
        public static int HumanObjective2UpgradePointsGranted = 2;
        public static int HumanObjective3UpgradePointsGranted = 2;
        public static int HumanObjective4UpgradePointsGranted = 3;
        public static int HumanObjective5UpgradePointsGranted = 4;

        public static int UndeadObjective1UpgradePointsGranted = 2;
        public static int UndeadObjective2UpgradePointsGranted = 2;
        public static int UndeadObjective3UpgradePointsGranted = 2;
        public static int UndeadObjective4UpgradePointsGranted = 3;
        public static int UndeadObjective5UpgradePointsGranted = 4;

        public static double HumanObjective1ScoreChance = .1;
        public static double HumanObjective2ScoreChance = 0;
        public static double HumanObjective3ScoreChance = .1;
        public static double HumanObjective4ScoreChance = .01;
        public static double HumanObjective5ScoreChance = .01;

        public static double UndeadObjective1ScoreChance = .1;
        public static double UndeadObjective2ScoreChance = 0;
        public static double UndeadObjective3ScoreChance = 0;
        public static double UndeadObjective4ScoreChance = .01;
        public static double UndeadObjective5ScoreChance = .01;

        public static int StockpileDisbursementMinutes = 15;
        public static double StockpileChanceModifier = .20;

        public static int UndeadMaxDynamicWaypointRange = 300;
        public static int HumanMaxDynamicWaypointRange = 100;

        public static int HonorAggressionThreshold = 0;
        public static int CommitMurderHonorLoss = -100;
        public static int HumanKillCivilianHonorLoss = -40;
        public static int HumanAttackPlayerHonorLoss = -10;
        public static int HumanAttackCivilianHonorLoss = -10;

        public static double HumanRessStatsPercent = 1.0;
        public static double UndeadRessStatsPercent = 0.5;

        public static int HumanStartingUpgradePoints = 7;
        public static int HumanStartingSurvivalPoints = 7;

        public static int UndeadStartingUpgradePoints = 0;
        public static int UndeadStartingCorruptionPoints = 5;

        public static int HumanUpgradesAvailablePerShuffle = 3;

        public static int UndeadMonsterShuffleCost = 1;
        public static int[] UndeadMonsterTierUpgradeCosts = { 4, 8, 12, 16, -1};
        public static int UndeadPostTier5UpgradeCost = 10;
        public static double UndeadPostTier5StatSkillIncrease = .05;

        public static double BaseUndeadAbilityScalar = 1.0;
        public static double UndeadAbilityIncreasePerTier = .03;
        
        public static int UpgradeDistributionMinutes = 15;

        public static int HungerAndThirstLossEventMinutes = 5;

        public static double HumanDeathFoodWaterLossChance = .5;
        public static double UndeadDeathBrainLossChance = .5;

        public static int HungerLossEventAmount = -10;
        public static int ThirstLossEventAmount = -10;

        public static int MissingHungerStatReductionThreshold = 80;
        public static double MissingHungerStatReduction = .01;

        public static int MissingThirstSkillReductionThreshold = 80;
        public static double MissingThirstSkillReduction = .01;

        public static int humanAbilitiesPerPage = 5;
        public static int undeadAbilitiesPerPage = 5;

        public static int HumanPlayerKillUndeadPlayerScore = 5;
        public static int HumanPlayerKillFatiguedUndeadPlayerScore = 2;

        public static double HumanRepairScoreChance = .04;
        public static double HumanOutpostRepairScoreChance = .02;
        public static double HumanRepairSurvivalStoneChance = .03;
        public static double HumanRepairUpgradeTokenChance = .015;

        public static double HumanDamageSourceStoneScoreChance = .06;
        public static double HumanDamageSourceStoneSurvivalStoneChance = .04;
        public static double HumanDamageSourceStoneUpgradeTokenChance = .02;

        public static int HumanBonesPerScore = 5;

        public static int HumanConstructableScorePoints = 15;
        public static double HumanConstructableSurvivalStoneChance = 1;
        public static double HumanConstructableUpgradeTokenChance = 1;

        public static int UndeadPlayerKillHumanPlayerScore = 5;
        public static int UndeadPlayerKillFatiguedHumanPlayerScore = 2;

        public static int UndeadPlayerCorruptWildlifeScore = 2;

        public static double UndeadPlayerToNonCorruptedWildlifeDamageScalar = .01;
        public static double WildlifeCreatureCorruptChance = .25;
        public static double WildlifePlayerCorruptChance = .50;
        public static double WildlifeRewardChance = .3;
        public static double WildlifeCorruptionStoneChance = .3;
        public static double WildlifeUpgradeTokenChance = .15;        

        public static double UndeadDamageObjectScoreChance = .075;
        public static double UndeadDamageObjectRewardChance = .05;
        public static double UndeadDamageObjectCorruptionTokenChance = .03;
        public static double UndeadDamageObjectUpgradeTokenChance = .015;        

        public static double UndeadDamageOutpostRewardReduction = .666;
        public static double UndeadDamageOutpostAlreadyBuiltRewardReduction = .5;

        public static double UndeadDamageStockpileScoreChance = .08;
        public static double UndeadDamageStockpileRewardChance = .06;
        public static double UndeadDamageStockpileCorruptionTokenChance = .04;
        public static double UndeadDamageStockpileUpgradeTokenChance = .02;        

        public static int GiftOfCorruptionAbilityScore = 2;
        public static int CreepAbilityScore = 2;

        public static double GiftOfCorruptionAbilityCorruptionTokenChance = .3;
        public static double GiftOfCorruptionAbilityUpgradeTokenChance = .15;

        public static double CreepAbilityCorruptionTokenChance = .3;
        public static double CreepAbilityUpgradeTokenChance = .15;

        public static int PostSessionTextDelayPerMessage = 8;

        public static int MinScoreToQualifyAsParticipant = 25;
        public static int ParticipationRewardPoints = 2;
        public static int HighestTotalScoreRewardPoints = 1; //Additional Points

        public static int whiteTextHue = 2655;
        public static int greenTextHue = 0x3F;
        public static int lightGreenTextHue = 2599;
        public static int blueTextHue = 2603;
        public static int purpleTextHue = 2606;
        public static int lightPurpleTextHue = 2628;
        public static int yellowTextHue = 2550;
        public static int redTextHue = 2115; //0x22
        public static int greyTextHue = 2401;
        public static int orangeTextHue = 149;

        public static int hungerTextHue = 2208;
        public static int thirstTextHue = 2603;
        public static int humanityTextHue = 2605;
        public static int honorTextHue = 2115;

        public static int humanScoreTextHue = whiteTextHue;
        public static int survivalPointsTextHue = lightGreenTextHue;
        public static int humanUpgradePointsTextHue = yellowTextHue;

        public static int undeadScoreTextHue = whiteTextHue;
        public static int corruptionPointsTextHue = greyTextHue;
        public static int undeadUpgradePointsTextHue = yellowTextHue;

        public static int monsterTierTextHue = lightPurpleTextHue;

        public static int openGumpSound = 0x055;
        public static int changeGumpSound = 0x057;
        public static int closeGumpSound = 0x058;

        public static int earnSurvivalSound = 0x64B;
        public static int earnHumanUpgradeSound = 0x64E;

        public static int earnCorruptionSound = 0x655;
        public static int earnUndeadUpgradeSound = 0x659;

        public static int purchaseUpgradeSound = 0x5B9;
        public static int selectionSound = 0x4D2;
        public static int shuffleSound = 0x5BA;

        public static void CommandActivateAbility(PlayerMobile player, int slot)
        {
            if (player == null)
                return;

            if (!UOACZPersistance.Active)
            {
                player.SendMessage("That may only be activated while within UOACZ.");
                return;
            }

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            switch (player.m_UOACZAccountEntry.ActiveProfile)
            {
                case UOACZAccountEntry.ActiveProfileType.None:
                    player.SendMessage("That may only be activated while within UOACZ.");
                    break;

                case UOACZAccountEntry.ActiveProfileType.Human:
                    if (player.HasGump(typeof(HumanProfileAbilitiesHotbarGump)))
                    {
                        int abilityIndex = ((player.m_UOACZAccountEntry.HumanProfile.AbilitiesHotbarPageNumber - 1) * UOACZSystem.humanAbilitiesPerPage) + slot;

                        if (player.m_UOACZAccountEntry.HumanProfile.m_Abilities.Count <= abilityIndex)
                            return;

                        UOACZHumanAbilityType abilityType = player.m_UOACZAccountEntry.HumanProfile.m_Abilities[abilityIndex].m_AbilityType;
                        UOACZHumanAbilities.ActivateAbility(player, abilityType);
                    }
                    break;

                case UOACZAccountEntry.ActiveProfileType.Undead:
                    if (player.HasGump(typeof(UndeadProfileAbilitiesHotbarGump)))
                    {
                        int abilityIndex = ((player.m_UOACZAccountEntry.UndeadProfile.AbilitiesHotbarPageNumber - 1) * UOACZSystem.undeadAbilitiesPerPage) + slot;

                        if (player.m_UOACZAccountEntry.UndeadProfile.m_Abilities.Count <= abilityIndex)
                            return;

                        UOACZUndeadAbilityType abilityType = player.m_UOACZAccountEntry.UndeadProfile.m_Abilities[abilityIndex].m_AbilityType;
                        UOACZUndeadAbilities.ActivateAbility(player, abilityType);
                    }
                    break;
            }
        }

        public static double GetUndeadAbilityScalar(PlayerMobile player)
        {
            double scalar = BaseUndeadAbilityScalar;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            scalar += ((double)player.m_UOACZAccountEntry.UndeadProfile.MonsterTier * UndeadAbilityIncreasePerTier);
            scalar += ((double)player.m_UOACZAccountEntry.UndeadProfile.PostTier5Upgrades * UndeadAbilityIncreasePerTier);
            
            return scalar;
        }

        public static void NavigateHotbar(PlayerMobile player, int changeAmount)
        {
            if (player == null)
                return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            //Human Hotbar
            if (player.HasGump(typeof(HumanProfileAbilitiesHotbarGump)))
            {
                int abilitiesPerPage = UOACZSystem.humanAbilitiesPerPage;
                int totalAbilities = player.m_UOACZAccountEntry.HumanProfile.m_Abilities.Count;
                int totalAbilityPages = (int)(Math.Ceiling((double)totalAbilities / (double)abilitiesPerPage));

                if (totalAbilities == 0)
                    return;

                if (totalAbilityPages <= 0)
                    totalAbilityPages = 1;

                int abilitiesOnPage;

                if (player.m_UOACZAccountEntry.HumanProfile.AbilitiesHotbarPageNumber < totalAbilityPages)
                    abilitiesOnPage = abilitiesPerPage;

                else
                    abilitiesOnPage = totalAbilities - ((totalAbilityPages - 1) * abilitiesPerPage);

                //Previous Hotbar Page
                if (changeAmount == -1)
                {
                    if (player.m_UOACZAccountEntry.HumanProfile.AbilitiesHotbarPageNumber > 1)
                        player.m_UOACZAccountEntry.HumanProfile.AbilitiesHotbarPageNumber--;
                }

                //Next Hotbar Page
                if (changeAmount == 1)
                {
                    if (player.m_UOACZAccountEntry.HumanProfile.AbilitiesHotbarPageNumber < totalAbilityPages)
                        player.m_UOACZAccountEntry.HumanProfile.AbilitiesHotbarPageNumber++;
                }

                player.CloseGump(typeof(HumanProfileAbilitiesHotbarGump));
                player.SendGump(new HumanProfileAbilitiesHotbarGump(player));
            }

            //Undead Hotbar
            if (player.HasGump(typeof(UndeadProfileAbilitiesHotbarGump)))
            {
                int abilitiesPerPage = UOACZSystem.undeadAbilitiesPerPage;
                int totalAbilities = player.m_UOACZAccountEntry.UndeadProfile.m_Abilities.Count;
                int totalAbilityPages = (int)(Math.Ceiling((double)totalAbilities / (double)abilitiesPerPage));

                if (totalAbilities == 0)
                    return;

                if (totalAbilityPages <= 0)
                    totalAbilityPages = 1;

                int abilitiesOnPage;

                if (player.m_UOACZAccountEntry.UndeadProfile.AbilitiesHotbarPageNumber < totalAbilityPages)
                    abilitiesOnPage = abilitiesPerPage;

                else
                    abilitiesOnPage = totalAbilities - ((totalAbilityPages - 1) * abilitiesPerPage);

                //Previous Hotbar Page
                if (changeAmount == -1)
                {
                    if (player.m_UOACZAccountEntry.UndeadProfile.AbilitiesHotbarPageNumber > 1)
                        player.m_UOACZAccountEntry.UndeadProfile.AbilitiesHotbarPageNumber--;
                }

                //Next Hotbar Page
                if (changeAmount == 1)
                {
                    if (player.m_UOACZAccountEntry.UndeadProfile.AbilitiesHotbarPageNumber < totalAbilityPages)
                        player.m_UOACZAccountEntry.UndeadProfile.AbilitiesHotbarPageNumber++;
                }

                player.CloseGump(typeof(UndeadProfileAbilitiesHotbarGump));
                player.SendGump(new UndeadProfileAbilitiesHotbarGump(player));
            }
        }

        public static void ChangeStat(PlayerMobile player, UOACZStatType statType, int amount, bool broadcast)
        {
            if (player == null || amount == 0)
                return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            string textChange = "+";
            string changeDirection = "increased";

            if (amount < 0)
            {
                textChange = "-";
                changeDirection = " decreased ";
            }

            string changeAmount = textChange + Math.Abs(amount).ToString();

            switch (statType)
            {
                case UOACZStatType.Hunger:
                    if (player.m_UOACZAccountEntry.HumanProfile.HungerPoints == 0 && amount < 0)
                        return;

                    player.m_UOACZAccountEntry.HumanProfile.HungerPoints += amount;

                    UOACZSystem.UpdateStatsSkills(player);

                    player.ResetRegenTimers();

                    player.SendMessage(hungerTextHue, GetHungerStatusText(player));

                    if (broadcast)
                        BroadcastStatChange(player, UOACZStatType.Hunger, amount);
                    break;

                case UOACZStatType.Thirst:
                    if (player.m_UOACZAccountEntry.HumanProfile.ThirstPoints == 0 && amount < 0)
                        return;

                    player.m_UOACZAccountEntry.HumanProfile.ThirstPoints += amount;

                    UOACZSystem.UpdateStatsSkills(player);

                    player.SendMessage(thirstTextHue, GetThirstStatusText(player));

                    if (broadcast)
                        BroadcastStatChange(player, UOACZStatType.Thirst, amount);
                break;
                    
                case UOACZStatType.Honor:
                    if (player.m_UOACZAccountEntry.HumanProfile.HonorPoints == 0 && amount < 0)
                        return;

                    player.m_UOACZAccountEntry.HumanProfile.HonorPoints += amount;

                    if (amount < 0)
                        player.m_UOACZAccountEntry.HonorLost += Math.Abs(amount);

                    if (player.m_UOACZAccountEntry.HumanProfile.HonorPoints <= UOACZSystem.HonorAggressionThreshold)
                        player.SendMessage("Due to your dishonorable actions you are now an outcast and freely attackable by other humans.");

                    if (broadcast)
                        BroadcastStatChange(player, UOACZStatType.Honor, amount);
                break;

                case UOACZStatType.HumanScore:
                    player.m_UOACZAccountEntry.CurrentSessionHumanScore += amount;
                    player.m_UOACZAccountEntry.CurrentSessionTotalScore += amount;

                    if (broadcast)
                        BroadcastStatChange(player, UOACZStatType.HumanScore, amount);
                break;

                case UOACZStatType.SurvivalPoints:
                    player.m_UOACZAccountEntry.HumanProfile.SurvivalPoints += amount;

                    if (amount < 0)
                        player.m_UOACZAccountEntry.HumanSurvivalPointsSpentTotal += Math.Abs(amount);

                    if (broadcast)
                        BroadcastStatChange(player, UOACZStatType.SurvivalPoints, amount);
                break;

                case UOACZStatType.HumanUpgradePoints:
                    player.m_UOACZAccountEntry.HumanProfile.UpgradePoints += amount;

                    if (amount < 0)
                        player.m_UOACZAccountEntry.HumanUpgradePointsSpentTotal += Math.Abs(amount);

                    if (broadcast)
                        BroadcastStatChange(player, UOACZStatType.HumanUpgradePoints, amount);
                    break;

                case UOACZStatType.UndeadScore:
                    player.m_UOACZAccountEntry.CurrentSessionUndeadScore += amount;
                    player.m_UOACZAccountEntry.CurrentSessionTotalScore += amount;

                    if (broadcast)
                        BroadcastStatChange(player, UOACZStatType.UndeadScore, amount);
                    break;

                case UOACZStatType.CorruptionPoints:
                    player.m_UOACZAccountEntry.UndeadProfile.CorruptionPoints += amount;

                    if (amount < 0)
                        player.m_UOACZAccountEntry.UndeadCorruptionPointsSpentTotal += Math.Abs(amount);

                    if (broadcast)
                        BroadcastStatChange(player, UOACZStatType.CorruptionPoints, amount);
                break;

                case UOACZStatType.UndeadUpgradePoints:
                    player.m_UOACZAccountEntry.UndeadProfile.UpgradePoints += amount;

                    if (amount < 0)
                        player.m_UOACZAccountEntry.UndeadUpgradePointsSpentTotal += Math.Abs(amount);

                    if (broadcast)
                        BroadcastStatChange(player, UOACZStatType.UndeadUpgradePoints, amount);
                    break;

                case UOACZStatType.MonsterTier:
                    player.m_UOACZAccountEntry.UndeadProfile.MonsterTier += amount;

                    if (broadcast)
                        BroadcastStatChange(player, UOACZStatType.MonsterTier, amount);
                    break;
            }

            UOACZSystem.RefreshAllGumps(player);
        }

        public static void BroadcastStatChange(PlayerMobile player, UOACZStatType statType, int amount)
        {
            if (player == null) return;
            if (amount == 0) return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            string textChange = "+";
            string changeDirection = "increased";

            if (amount < 0)
            {
                textChange = "-";
                changeDirection = " decreased ";
            }

            string changeAmount = textChange + Math.Abs(amount).ToString();

            switch (statType)
            {
                case UOACZStatType.Hunger:
                    int hunger = player.m_UOACZAccountEntry.HumanProfile.HungerPoints;
                    int maxHunger = player.m_UOACZAccountEntry.HumanProfile.MaxHungerPoints;

                    if (player.NetState != null)                    
                        player.PrivateOverheadMessage(MessageType.Regular, UOACZSystem.hungerTextHue, false, "Hunger " + changeAmount + " (" + hunger.ToString() + "/" + maxHunger.ToString() + ")", player.NetState);
                break;

                case UOACZStatType.Thirst:
                    int thirst = player.m_UOACZAccountEntry.HumanProfile.ThirstPoints;
                    int maxThirst = player.m_UOACZAccountEntry.HumanProfile.MaxThirstPoints;

                    if (player.NetState != null)                    
                        player.PrivateOverheadMessage(MessageType.Regular, UOACZSystem.thirstTextHue, false, "Thirst " + changeAmount + " (" + thirst.ToString() + "/" + maxThirst.ToString() + ")", player.NetState);
                break;

                case UOACZStatType.Humanity:
                    int humanity = player.m_UOACZAccountEntry.HumanProfile.HumanityPoints;
                    int maxHumanity = player.m_UOACZAccountEntry.HumanProfile.MaxHumanityPoints;

                    if (player.NetState != null)                    
                        player.PrivateOverheadMessage(MessageType.Regular, UOACZSystem.humanityTextHue, false, "Humanity " + changeAmount + " (" + humanity.ToString() + "/" + maxHumanity.ToString() + ")", player.NetState);
                break;

                case UOACZStatType.Honor:
                    int honor = player.m_UOACZAccountEntry.HumanProfile.HonorPoints;
                    int maxHonor = player.m_UOACZAccountEntry.HumanProfile.MaxHonorPoints;

                    if (player.NetState != null)
                        player.PrivateOverheadMessage(MessageType.Regular, UOACZSystem.honorTextHue, false, "Honor " + changeAmount + " (" + honor.ToString() + "/" + maxHonor.ToString() + ")", player.NetState);
                break;

                case UOACZStatType.HumanScore:
                    if (player.NetState != null)
                        player.PrivateOverheadMessage(MessageType.Regular, UOACZSystem.humanScoreTextHue, false, "Score " + changeAmount, player.NetState);
                break;

                case UOACZStatType.SurvivalPoints:
                    if (player.NetState != null)
                        player.PrivateOverheadMessage(MessageType.Regular, UOACZSystem.survivalPointsTextHue, false, "Survival Points " + changeAmount, player.NetState);
                break;

                case UOACZStatType.HumanUpgradePoints:
                    if (player.NetState != null)
                        player.PrivateOverheadMessage(MessageType.Regular, UOACZSystem.humanUpgradePointsTextHue, false, "Upgrade Points " + changeAmount, player.NetState);
                break;

                case UOACZStatType.UndeadScore:
                    if (player.NetState != null)
                        player.PrivateOverheadMessage(MessageType.Regular, UOACZSystem.undeadScoreTextHue, false, "Score " + changeAmount, player.NetState);
                break;

                case UOACZStatType.CorruptionPoints:
                    if (player.NetState != null)
                        player.PrivateOverheadMessage(MessageType.Regular, UOACZSystem.corruptionPointsTextHue, false, "Corruption Points " + changeAmount, player.NetState);
                break;

                case UOACZStatType.UndeadUpgradePoints:
                    if (player.NetState != null)
                        player.PrivateOverheadMessage(MessageType.Regular, UOACZSystem.undeadUpgradePointsTextHue, false, "Upgrade Points " + changeAmount, player.NetState);
                break;

                case UOACZStatType.MonsterTier:
                    if (player.NetState != null)
                        player.PrivateOverheadMessage(MessageType.Regular, UOACZSystem.monsterTierTextHue, false, "Monster Tier " + changeAmount, player.NetState);
                break;
            }
        }
        
        public static void SwitchTeams(PlayerMobile player, bool undead)
        {
            if (player == null)
                return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            if (!UOACZPersistance.Active)
            {
                if (undead)
                {
                    player.m_UOACZAccountEntry.CurrentTeam = UOACZAccountEntry.ActiveProfileType.Undead;
                    player.SendMessage("You will begin as Undead when the next UOACZ session begins.");

                    player.SendSound(UOACZSystem.earnUndeadUpgradeSound);

                }
                   
                else
                {
                    player.m_UOACZAccountEntry.CurrentTeam = UOACZAccountEntry.ActiveProfileType.Human;
                    player.SendMessage("You will begin as Human when the next UOACZ session begins.");

                    player.SendSound(UOACZSystem.earnHumanUpgradeSound);
                }               

                return;
            }

            if (UOACZRegion.ContainsMobile(player))
            {
                if (!player.Alive)
                    player.Resurrect();

                ClearPlayerStatus(player);

                Point3D location = player.Location;
                Map map = player.Map;
                
                foreach (Mobile mobile in World.Mobiles.Values)
                {
                    if (mobile == null) continue;
                    if (!UOACZRegion.ContainsMobile(mobile)) continue;

                    mobile.RemoveAggressor(player);
                    mobile.RemoveAggressed(player);

                    if (mobile.Combatant == player)
                        mobile.Combatant = null;
                }
            }
                        
            //Human to Undead
            if (undead)
            {
                //Outside of UOACZ
                if (player.m_UOACZAccountEntry.ActiveProfile == UOACZAccountEntry.ActiveProfileType.None)
                {
                    player.m_UOACZAccountEntry.CurrentTeam = UOACZAccountEntry.ActiveProfileType.Undead;                    
                    
                    player.SendMessage("You will play as Undead when you enter the UOACZ world.");
                    player.SendSound(UOACZSystem.earnHumanUpgradeSound);

                    RefreshAllGumps(player);
                }

                //Inside UOACZ
                else
                {
                    player.m_UOACZAccountEntry.CurrentTeam = UOACZAccountEntry.ActiveProfileType.Undead;
                    player.m_UOACZAccountEntry.ActiveProfile = UOACZAccountEntry.ActiveProfileType.Undead;

                    DepositUOACZItems(player, true);         

                    ApplyActiveProfile(player);
                    RefreshAllGumps(player);

                    if (!player.m_UOACZAccountEntry.InitialUndeadItemsGenerated)
                        InitializeItems(player);

                    else if (player.m_UOACZAccountEntry.InitialUndeadItemsGenerated)
                        EquipUOACZItems(player, false);

                    UOACZDestination destination = UOACZDestination.GetRandomEntrance(false);

                    if (destination != null)
                        player.MoveToWorld(destination.Location, destination.Map);

                    else
                        player.MoveToWorld(UOACZPersistance.DefaultUndeadLocation, UOACZPersistance.DefaultMap);
                    
                    player.SendMessage("You become Undead.");
                }                
            }

            //Undead to Human
            else
            {
                //Outside of UOACZ
                if (player.m_UOACZAccountEntry.ActiveProfile == UOACZAccountEntry.ActiveProfileType.None)
                {
                    player.m_UOACZAccountEntry.CurrentTeam = UOACZAccountEntry.ActiveProfileType.Human;
                    
                    player.SendMessage("You will play as a Human when you enter the UOACZ world.");
                    player.SendSound(UOACZSystem.earnUndeadUpgradeSound);

                    RefreshAllGumps(player);
                }

                //In UOACZ
                else
                {
                    player.m_UOACZAccountEntry.CurrentTeam = UOACZAccountEntry.ActiveProfileType.Human;
                    player.m_UOACZAccountEntry.ActiveProfile = UOACZAccountEntry.ActiveProfileType.Human;

                    DepositUOACZItems(player, false);

                    ApplyActiveProfile(player);
                    RefreshAllGumps(player);                    

                    if (!player.m_UOACZAccountEntry.InitialHumanItemsGenerated)
                        InitializeItems(player);

                    else if (player.m_UOACZAccountEntry.InitialHumanItemsGenerated)
                        EquipUOACZItems(player, true);

                    UOACZDestination destination = UOACZDestination.GetRandomEntrance(true);

                    if (destination != null)
                        player.MoveToWorld(destination.Location, destination.Map);

                    else
                        player.MoveToWorld(UOACZPersistance.DefaultHumanLocation, UOACZPersistance.DefaultMap);

                    player.SendMessage("You become Human.");
                }                
            }

            if (UOACZRegion.ContainsMobile(player))
            {
                player.Hits = player.HitsMax;
                player.Stam = player.StamMax;
                player.Mana = player.ManaMax;

                player.ResetRegenTimers();
            }

            player.m_UOACZAccountEntry.NextTeamSwitchAllowed = DateTime.UtcNow + UOACZSystem.TeamSwitchCooldown;

            UOACZSystem.RefreshAllGumps(player);
        }

        public static void RemoveAllStatSkillMods(PlayerMobile player)
        {
            Queue m_Queue = new Queue();

            foreach (SkillMod skillMod in player.SkillMods)
            {
                m_Queue.Enqueue(skillMod);
            }

            while (m_Queue.Count > 0)
            {
                SkillMod skillMod = (SkillMod)m_Queue.Dequeue();
                player.RemoveSkillMod(skillMod);
            }

            m_Queue = new Queue();

            foreach (StatMod statMod in player.StatMods)
            {
                m_Queue.Enqueue(statMod);
            }

            while (m_Queue.Count > 0)
            {
                StatMod statMod = (StatMod)m_Queue.Dequeue();
                player.RemoveStatMod(statMod.Name);
            }

            player.RemoveStatMod("[Magic] Str Offset");
            player.RemoveStatMod("[Magic] Dex Offset");
            player.RemoveStatMod("[Magic] Int Offset");
            player.RemoveStatMod("Concussion");

            player.MagicDamageAbsorb = 0;
            player.MeleeDamageAbsorb = 0;

            Spells.Second.ProtectionSpell.Registry.Remove(player);

            player.EndAction(typeof(DefensiveSpell));

            player.ValidateSkillMods();
            player.InvalidateProperties();
        }

        public static void PlayerEnterUOACZRegion(PlayerMobile player)
        {
            if (player == null) return;
            if (player.Deleted) return;
            if (player.AccessLevel > AccessLevel.Player) return;

            ClearPlayerStatus(player);

            player.RevealingAction();

            UOACZPersistance.StoreCharacterSnapshot(player);           

            DepositRealWorldItems(player);

            player.m_UOACZAccountEntry.MostRecentPlayer = player;
            player.m_UOACZAccountEntry.MostRecentPlayerString = player.RawName;

            DetermineAndApplyPlayerProfile(player);

            switch (player.m_UOACZAccountEntry.ActiveProfile)
            {
                case UOACZAccountEntry.ActiveProfileType.Human:
                    if (!player.m_UOACZAccountEntry.InitialHumanItemsGenerated)
                        InitializeItems(player);

                    else
                        EquipUOACZItems(player, true);                    
                break;

                case UOACZAccountEntry.ActiveProfileType.Undead:
                    if (!player.m_UOACZAccountEntry.InitialUndeadItemsGenerated)
                        InitializeItems(player);

                    else
                        EquipUOACZItems(player, false);                    
                break;
            }           

            bool isHuman = true;

            if (player.m_UOACZAccountEntry.ActiveProfile == UOACZAccountEntry.ActiveProfileType.Undead)
                isHuman = false;

            UOACZDestination destination = UOACZDestination.GetRandomEntrance(isHuman);

            if (destination != null)
                player.MoveToWorld(destination.Location, destination.Map);

            else
            {
                if (isHuman)
                    player.MoveToWorld(UOACZPersistance.DefaultHumanLocation, UOACZPersistance.DefaultMap);

                else
                    player.MoveToWorld(UOACZPersistance.DefaultUndeadLocation, UOACZPersistance.DefaultMap);
            }

            player.Hits = player.HitsMax;
            player.Stam = player.StamMax;
            player.Mana = player.ManaMax;

            player.ResetRegenTimers();
        }

        public static void CheckForOtherAccountPlayerStillInUOACZ(PlayerMobile player)
        {
            if (player == null)
                return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            if (player.m_UOACZAccountEntry.MostRecentPlayer != null)
            {
                UOACZCharacterSnapshot otherPlayerSnapshot = UOACZPersistance.FindCharacterSnapshot(player.m_UOACZAccountEntry.MostRecentPlayer);
                
                if (otherPlayerSnapshot != null)
                {
                    if (!otherPlayerSnapshot.Deleted)
                    {
                        if (otherPlayerSnapshot.m_Player != null)
                        {
                            if (!otherPlayerSnapshot.m_Player.Deleted)
                            {                               
                                switch (player.m_UOACZAccountEntry.CurrentTeam)
                                {
                                    case UOACZAccountEntry.ActiveProfileType.Human:
                                        DepositUOACZItems(otherPlayerSnapshot.m_Player, true);
                                    break;

                                    case UOACZAccountEntry.ActiveProfileType.Undead:
                                        DepositUOACZItems(otherPlayerSnapshot.m_Player, false);
                                    break;
                                }   

                                Queue m_Queue = new Queue();

                                foreach(Mobile mobile in otherPlayerSnapshot.m_Player.AllFollowers)
                                {
                                    BaseCreature bc_Creature = mobile as BaseCreature;

                                    if (bc_Creature == null) continue;
                                    if (bc_Creature.Deleted || !bc_Creature.Alive || bc_Creature.IsDeadPet || bc_Creature.IsDeadBondedPet) continue;
                                    if (!bc_Creature.Controlled) continue;

                                    if (UOACZRegion.ContainsMobile(mobile))
                                        m_Queue.Enqueue(mobile);
                                }

                                while (m_Queue.Count > 0)
                                {
                                    BaseCreature bc_Creature = (BaseCreature)m_Queue.Dequeue();

                                    if (bc_Creature == null) 
                                        continue;

                                    if (bc_Creature.AIObject != null)
                                        bc_Creature.AIObject.DoOrderRelease();
                                }
                            }   
                        }
                    }
                }
            }
        }

        public static void PlayerExitUOACZRegion(PlayerMobile player)
        {
            if (player == null) return;
            if (player.Deleted) return;
            if (player.AccessLevel > AccessLevel.Player) return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            bool sessionComplete = !UOACZPersistance.Active;

            ClearPlayerStatus(player);

            player.Hidden = true;
            player.IsStealthing = false;
            player.AllowedStealthSteps = 0;

            UOACZCharacterSnapshot snapshot = UOACZPersistance.FindCharacterSnapshot(player);

            if (snapshot != null)
            {
                if (!snapshot.Deleted)
                {
                    if (player.m_UOACZAccountEntry.ActiveProfile == UOACZAccountEntry.ActiveProfileType.Human)
                        DepositUOACZItems(player, true);

                    if (player.m_UOACZAccountEntry.ActiveProfile == UOACZAccountEntry.ActiveProfileType.Undead)
                        DepositUOACZItems(player, false);

                    player.m_UOACZAccountEntry.ActiveProfile = UOACZAccountEntry.ActiveProfileType.None;

                    ApplyActiveProfile(player);
                    
                    EquipRealWorldItems(player);

                    UOACZPersistance.RestoreCharacterSnapshot(player);

                    player.Hits = player.HitsMax;
                    player.Stam = player.StamMax;
                    player.Mana = player.ManaMax;

                    player.ResetRegenTimers();
                }
            }
        }

        public static void ClearPlayerStatus(PlayerMobile player)
        {
            player.CloseAllGumps();

            player.ReleaseAllFollowers();

            RemoveAllStatSkillMods(player);

            if (!player.Alive)
                player.Resurrect();

            player.DropHolding();

            player.ClearTarget();

            BandageContext bandageContext = BandageContext.GetContext(player);

            if (bandageContext != null)
                bandageContext.StopHeal();

            player.DamageEntries.Clear();
            player.Combatant = null;
            player.Aggressed.Clear();
            player.Aggressors.Clear();

            player.Criminal = false;

            player.AggroBonus = 0;

            player.Poison = null;
            player.Paralyzed = false;
            player.CantWalk = false;

            SpecialAbilities.ClearSpecialEffects(player);
        }

        public static void DetermineAndApplyPlayerProfile(PlayerMobile player)
        {
            if (player == null)
                return;

            player.m_UOACZAccountEntry.ActiveProfile = UOACZAccountEntry.ActiveProfileType.None; 
           
            switch (player.m_UOACZAccountEntry.CurrentTeam)
            {
                case UOACZAccountEntry.ActiveProfileType.Human:
                    player.m_UOACZAccountEntry.ActiveProfile = UOACZAccountEntry.ActiveProfileType.Human;
                break;

                case UOACZAccountEntry.ActiveProfileType.Undead:
                    player.m_UOACZAccountEntry.ActiveProfile = UOACZAccountEntry.ActiveProfileType.Undead;
                break;
            }            
            
            ApplyActiveProfile(player);
        }

        public static void InitializeItems(PlayerMobile player)
        {
            if (player.Backpack == null)
               player.AddItem(new Backpack());

            else if (player.Backpack.Deleted)
                player.AddItem(new Backpack());

            switch (player.m_UOACZAccountEntry.CurrentTeam)
            {
                case UOACZAccountEntry.ActiveProfileType.Human:
                    player.m_UOACZAccountEntry.InitialHumanItemsGenerated = true;

                    #region Clothing

                    //Shirt            
                    switch (Utility.RandomMinMax(1, 3))
                    {
                        case 1: player.AddItem(new Shirt(Utility.RandomNeutralHue()) { LootType = LootType.Blessed }); break;
                        case 2: player.AddItem(new FancyShirt(Utility.RandomNeutralHue()) { LootType = LootType.Blessed }); break;
                        case 3: player.AddItem(new Doublet(Utility.RandomNeutralHue()) { LootType = LootType.Blessed }); break;
                    }

                    //Pants
                    if (player.Female)
                    {
                        switch (Utility.RandomMinMax(1, 3))
                        {
                            case 1: player.AddItem(new Skirt(Utility.RandomNeutralHue()) { LootType = LootType.Blessed }); break;
                            case 2: player.AddItem(new Kilt(Utility.RandomNeutralHue()) { LootType = LootType.Blessed }); break;
                            case 3: player.AddItem(new ShortPants(Utility.RandomNeutralHue()) { LootType = LootType.Blessed }); break;
                        }
                    }

                    else
                    {
                        switch (Utility.RandomMinMax(1, 2))
                        {
                            case 1: player.AddItem(new ShortPants(Utility.RandomNeutralHue()) { LootType = LootType.Blessed }); break;
                            case 2: player.AddItem(new LongPants(Utility.RandomNeutralHue()) { LootType = LootType.Blessed }); break;
                        }
                    }

                    //Shoes            
                    switch (Utility.RandomMinMax(1, 4))
                    {
                        case 1: player.AddItem(new Shoes(Utility.RandomNeutralHue()) { LootType = LootType.Blessed }); break;
                        case 2: player.AddItem(new Boots(Utility.RandomNeutralHue()) { LootType = LootType.Blessed }); break;
                        case 3: player.AddItem(new ThighBoots(Utility.RandomNeutralHue()) { LootType = LootType.Blessed }); break;
                        case 4: player.AddItem(new Sandals(Utility.RandomNeutralHue()) { LootType = LootType.Blessed }); break;
                    }

                    #endregion

                    #region Starting Gear

                    player.AddItem(new UOACZSurvivalMachete() { LootType = Server.LootType.Blessed });

                    Backpack survivalBackpack = new Backpack();
                    survivalBackpack.LootType = LootType.Blessed;
                    survivalBackpack.Hue = 2575;
                    survivalBackpack.Name = "survival backpack";
                    player.Backpack.DropItem(survivalBackpack);
                    survivalBackpack.X = 44;
                    survivalBackpack.Y = 65;

                    Pouch survivalPouch = new Pouch();
                    survivalPouch.LootType = LootType.Blessed;
                    survivalPouch.Hue = 2575;
                    survivalPouch.Name = "survival pouch";
                    player.Backpack.DropItem(survivalPouch);
                    survivalPouch.X = 60;
                    survivalPouch.Y = 70;

                    Bag survivalBag = new Bag();
                    survivalBag.LootType = LootType.Blessed;
                    survivalBag.Hue = 2575;
                    survivalBag.Name = "survival bag";
                    player.Backpack.DropItem(survivalBag);
                    survivalBag.X = 84;
                    survivalBag.Y = 68;

                    UOACZSurvivalTome survivalTome = new UOACZSurvivalTome();
                    player.Backpack.DropItem(survivalTome);
                    survivalTome.X = 142;
                    survivalTome.Y = 65;

                    UOACZCorruptionTome corruptionTome = new UOACZCorruptionTome();
                    player.Backpack.DropItem(corruptionTome);
                    corruptionTome.X = 44;
                    corruptionTome.Y = 123;

                    //Survival Backpack Items
                    UOACZSewingKit sewingKit = new UOACZSewingKit();
                    sewingKit.LootType = Server.LootType.Blessed;
                    survivalBackpack.DropItem(sewingKit);
                    sewingKit.X = 44;
                    sewingKit.Y = 65;

                    UOACZTinkersTools tinkerTools = new UOACZTinkersTools();
                    tinkerTools.LootType = Server.LootType.Blessed;
                    survivalBackpack.DropItem(tinkerTools);
                    tinkerTools.X = 93;
                    tinkerTools.Y = 65;

                    UOACZTongs tongs = new UOACZTongs();
                    tongs.LootType = Server.LootType.Blessed;
                    survivalBackpack.DropItem(tongs);
                    tongs.X = 105;
                    tongs.Y = 75;

                    UOACZSaw saw = new UOACZSaw();
                    saw.LootType = Server.LootType.Blessed;
                    survivalBackpack.DropItem(saw);
                    saw.X = 49;
                    saw.Y = 89;

                    UOACZSkillet skillet = new UOACZSkillet();
                    skillet.LootType = Server.LootType.Blessed;
                    survivalBackpack.DropItem(skillet);
                    skillet.X = 64;
                    skillet.Y = 89;

                    UOACZFishingPole fishingPole = new UOACZFishingPole();
                    fishingPole.LootType = Server.LootType.Blessed;
                    survivalBackpack.DropItem(fishingPole);
                    fishingPole.X = 72;
                    fishingPole.Y = 101;

                    UOACZLockpickKit lockpickKit = new UOACZLockpickKit();
                    lockpickKit.LootType = Server.LootType.Blessed;
                    survivalBackpack.DropItem(lockpickKit);
                    lockpickKit.X = 108;
                    lockpickKit.Y = 88;

                    UOACZPickaxe pickaxe = new UOACZPickaxe();
                    pickaxe.LootType = Server.LootType.Blessed;
                    survivalBackpack.DropItem(pickaxe);
                    pickaxe.X = 124;
                    pickaxe.Y = 96;

                    Scissors scissors = new Scissors();
                    scissors.LootType = Server.LootType.Blessed;
                    survivalBackpack.DropItem(scissors);
                    scissors.X = 55;
                    scissors.Y = 104;

                    UOACZMortarPestle mortarPestle = new UOACZMortarPestle();
                    mortarPestle.LootType = Server.LootType.Blessed;
                    survivalBackpack.DropItem(mortarPestle);
                    mortarPestle.X = 44;
                    mortarPestle.Y = 119;

                    UOACZHatchet hatchet = new UOACZHatchet();
                    hatchet.LootType = Server.LootType.Blessed;
                    survivalBackpack.DropItem(hatchet);
                    hatchet.X = 61;
                    hatchet.Y = 113;

                    UOACZRepairHammer repairHammer = new UOACZRepairHammer();
                    repairHammer.LootType = Server.LootType.Blessed;
                    survivalBackpack.DropItem(repairHammer);
                    repairHammer.X = 89;
                    repairHammer.Y = 122;

                    //Survival Pouch Items
                    UOACZRope rope = new UOACZRope();
                    rope.Amount = 3;
                    survivalPouch.DropItem(rope);
                    rope.X = 44;
                    rope.Y = 65;

                    UOACZIronWire ironwire = new UOACZIronWire();
                    ironwire.Amount = 2;
                    survivalPouch.DropItem(ironwire);
                    ironwire.X = 82;
                    ironwire.Y = 73;

                    UOACZTorch torch = new UOACZTorch();
                    survivalPouch.DropItem(torch);
                    torch.X = 113;
                    torch.Y = 65;

                    torch = new UOACZTorch();
                    survivalPouch.DropItem(torch);
                    torch.X = 119;
                    torch.Y = 73;

                    UOACZOilFlask oilflask = new UOACZOilFlask();
                    survivalPouch.DropItem(oilflask);
                    oilflask.X = 142;
                    oilflask.Y = 75;

                    Feather feather = new Feather();
                    feather.Amount = 25;
                    survivalPouch.DropItem(feather);
                    feather.X = 47;
                    feather.Y = 102;

                    Bottle bottle = new Bottle();
                    bottle.Amount = 10;
                    survivalPouch.DropItem(bottle);
                    bottle.X = 84;
                    bottle.Y = 98;

                    UOACZBowl bowl = new UOACZBowl();
                    survivalPouch.DropItem(bowl);
                    bowl.X = 115;
                    bowl.Y = 100;

                    //Survival Bag Items
                    BlackPearl blackPearl = new BlackPearl();
                    blackPearl.Amount = 2;
                    survivalBag.DropItem(blackPearl);
                    blackPearl.X = 29;
                    blackPearl.Y = 48;

                    Bloodmoss bloodmoss = new Bloodmoss();
                    bloodmoss.Amount = 2;
                    survivalBag.DropItem(bloodmoss);
                    bloodmoss.X = 55;
                    bloodmoss.Y = 45;

                    MandrakeRoot mandrakeroot = new MandrakeRoot();
                    mandrakeroot.Amount = 2;
                    survivalBag.DropItem(mandrakeroot);
                    mandrakeroot.X = 88;
                    mandrakeroot.Y = 44;

                    Ginseng ginseng = new Ginseng();
                    ginseng.Amount = 2;
                    survivalBag.DropItem(ginseng);
                    ginseng.X = 29;
                    ginseng.Y = 64;

                    Garlic garlic = new Garlic();
                    garlic.Amount = 2;
                    survivalBag.DropItem(garlic);
                    garlic.X = 63;
                    garlic.Y = 64;

                    SpidersSilk spiderssilk = new SpidersSilk();
                    spiderssilk.Amount = 2;
                    survivalBag.DropItem(spiderssilk);
                    spiderssilk.X = 88;
                    spiderssilk.Y = 65;

                    Nightshade nightshade = new Nightshade();
                    nightshade.Amount = 2;
                    survivalBag.DropItem(nightshade);
                    nightshade.X = 29;
                    nightshade.Y = 89;

                    SulfurousAsh sulfurousash = new SulfurousAsh();
                    sulfurousash.Amount = 2;
                    survivalBag.DropItem(sulfurousash);
                    sulfurousash.X = 60;
                    sulfurousash.Y = 90;

                    Bandage bandage = new Bandage();
                    bandage.Amount = 10;
                    survivalBag.DropItem(bandage);
                    bandage.X = 88;
                    bandage.Y = 89;

                    #endregion

                    #region Unlockables

                    WoodenBox survivalBox = new WoodenBox();
                    survivalBox.LootType = LootType.Blessed;
                    survivalBox.Hue = 2575;
                    survivalBox.Name = "survival box";
                    player.Backpack.DropItem(survivalBox);
                    survivalBox.X = 114;
                    survivalBox.Y = 65;

                    UOACZSurvivalLantern survivalLantern = new UOACZSurvivalLantern();
                    survivalLantern.LootType = LootType.Blessed;
                    survivalBox.DropItem(survivalLantern);
                    survivalLantern.X = 96;
                    survivalLantern.Y = 92;

                    foreach (UOACZUnlockableDetailEntry unlockableDetailEntry in player.m_UOACZAccountEntry.m_Unlockables)
                    {
                        if (!unlockableDetailEntry.m_Active)
                            continue;

                        UOACZUnlockableDetail unlockableDetail = UOACZUnlockables.GetUnlockableDetail(unlockableDetailEntry.m_UnlockableType);

                        foreach (Type type in unlockableDetail.m_Items)
                        {
                            if (unlockableDetail.UnlockableCategory == UOACZUnlockableCategory.UndeadDye)
                                continue;

                            Item item = (Item)Activator.CreateInstance(type);
                            item.LootType = LootType.Blessed;
                            survivalBox.DropItem(item);
                        }
                    }

                    #endregion                    
                break;

                case UOACZAccountEntry.ActiveProfileType.Undead:                
                    player.m_UOACZAccountEntry.InitialUndeadItemsGenerated = true;

                    UOACZCorruptionBox corruptionBox = new UOACZCorruptionBox();
                    player.Backpack.DropItem(corruptionBox);
                    corruptionBox.X = 44;
                    corruptionBox.Y = 65;

                    corruptionBox.DropItem(new UOACZSurvivalTome());

                    corruptionTome = new UOACZCorruptionTome();
                    player.Backpack.DropItem(corruptionTome);
                    corruptionTome.X = 142;
                    corruptionTome.Y = 65;

                    foreach (UOACZUnlockableDetailEntry unlockableDetailEntry in player.m_UOACZAccountEntry.m_Unlockables)
                    {
                        if (!unlockableDetailEntry.m_Active)
                            continue;

                        UOACZUnlockableDetail unlockableDetail = UOACZUnlockables.GetUnlockableDetail(unlockableDetailEntry.m_UnlockableType);

                        foreach (Type type in unlockableDetail.m_Items)
                        {
                            if (unlockableDetail.UnlockableCategory != UOACZUnlockableCategory.UndeadDye)
                                continue;

                            Item item = (Item)Activator.CreateInstance(type);
                            item.LootType = LootType.Blessed;

                            if (item is UOACZUndeadDye)
                            {
                                UOACZUndeadDye undeadDye = item as UOACZUndeadDye;
                                undeadDye.Owner = player;
                            }

                            corruptionBox.DropItem(item);
                        }
                    }                
                break;
            }            
        }

        public static void SessionBegin()
        {
            ClearWorld();
            BuildWorld();
            SetupRegionEntities();

            foreach (UOACZMoongate moongate in UOACZMoongate.m_Instances)
            {
                moongate.Visible = true;
            }

            foreach (NetState state in NetState.Instances)
            {
                Mobile mobile = state.Mobile;
                PlayerMobile player = mobile as PlayerMobile;

                if (player == null)
                    continue;

                player.SendMessage(UOACZSystem.purpleTextHue, "UOACZ has begun!");
            }
        }

        public static void SessionComplete()
        {
            UOACZPersistance.Active = false;
            UOACZPersistance.m_MinutesActive = 0;
            UOACZPersistance.m_StockpileProgress.Clear();

            foreach (NetState state in NetState.Instances)
            {
                Mobile mobile = state.Mobile;

                PlayerMobile player = mobile as PlayerMobile;

                if (player == null)
                    continue;

                if (player.HasGump(typeof(UOACZScoreGump)))
                    player.CloseGump(typeof(UOACZScoreGump));

                UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

                player.m_UOACZAccountEntry.ScorePage = UOACZAccountEntry.ScorePageType.Previous;
                player.m_UOACZAccountEntry.ScorePageNumber = 0;

                player.CloseAllGumps();
            }

            CalculateScores();

            foreach (UOACZMoongate moongate in UOACZMoongate.m_Instances)
            {
                moongate.Visible = false;
            }

            Queue m_Queue = new Queue();

            //Process Remaining Snapshots (Players Who Haven't Already Left UOACZ)
            foreach (UOACZCharacterSnapshot snapshot in UOACZPersistance.m_UOACZCharacterSnapshots)
            {
                if (snapshot == null) continue;
                if (snapshot.Deleted) continue;

                m_Queue.Enqueue(snapshot);
            }

            while (m_Queue.Count > 0)
            {
                UOACZCharacterSnapshot snapshot = (UOACZCharacterSnapshot)m_Queue.Dequeue();

                PlayerMobile player = snapshot.m_Player;

                if (player == null) continue;
                if (player.Deleted) continue;

                UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

                PlayerExitUOACZRegion(player);

                UOACZDestination exit = UOACZDestination.GetRandomExit(player.Murderer);

                if (exit != null)
                {
                    if (player.NetState == null)
                    {
                        player.MoveToWorld(exit.Location, exit.Map);

                        if (player.m_LogoutTimer != null)
                        {
                            player.m_LogoutTimer.Stop();
                            player.m_LogoutTimer = null;
                        }

                        EventSink.InvokeLogout(new LogoutEventArgs(player));

                        player.LogoutLocation = player.Location;
                        player.LogoutMap = player.Map;

                        player.Internalize();
                    }

                    else
                        player.MoveToWorld(exit.Location, exit.Map);
                }

                else
                {
                    if (player.NetState == null)
                    {
                        player.MoveToWorld(UOACZPersistance.DefaultBritainLocation, UOACZPersistance.DefaultBritainMap);

                        if (player.m_LogoutTimer != null)
                        {
                            player.m_LogoutTimer.Stop();
                            player.m_LogoutTimer = null;
                        }

                        EventSink.InvokeLogout(new LogoutEventArgs(player));

                        player.LogoutLocation = player.Location;
                        player.LogoutMap = player.Map;

                        player.Internalize();
                    }

                    else
                        player.MoveToWorld(UOACZPersistance.DefaultBritainLocation, UOACZPersistance.DefaultBritainMap);
                }
            }

            UOACZPersistance.m_UOACZCharacterSnapshots.Clear();

            foreach (UOACZAccountEntry entry in UOACZPersistance.m_UOACZAccountEntries)
            {
                if (entry == null) continue;
                if (entry.Deleted) continue;

                UOACZPersistance.ResetProfile(entry);
            }

            ClearWorld();

            UOACZPersistance.HumanBalanceScalar = 1.0;
            UOACZPersistance.UndeadBalanceScalar = 1.0;

            UOACZPersistance.m_HumanObjective1 = 0;
            UOACZPersistance.m_HumanObjective2 = 0;
            UOACZPersistance.m_HumanObjective3 = 0;
            UOACZPersistance.m_HumanObjective4 = 0;
            UOACZPersistance.m_HumanObjective5 = 0;

            UOACZPersistance.m_UndeadObjective1 = 0;
            UOACZPersistance.m_UndeadObjective2 = 0;
            UOACZPersistance.m_UndeadObjective3 = 0;
            UOACZPersistance.m_UndeadObjective4 = 0;
            UOACZPersistance.m_UndeadObjective5 = 0;

            UOACZPersistance.m_HumanObjective1Target = 0;
            UOACZPersistance.m_HumanObjective2Target = 0;
            UOACZPersistance.m_HumanObjective3Target = 0;
            UOACZPersistance.m_HumanObjective4Target = 0;
            UOACZPersistance.m_HumanObjective5Target = 0;

            UOACZPersistance.m_UndeadObjective1Target = 0;
            UOACZPersistance.m_UndeadObjective2Target = 0;
            UOACZPersistance.m_UndeadObjective3Target = 0;
            UOACZPersistance.m_UndeadObjective4Target = 0;
            UOACZPersistance.m_UndeadObjective5Target = 0;
        }

        public static void SetupRegionEntities()
        {
            int maxSpawnCount = 0;
            int amountToSpawn = 0;

            double spawnDelay = 0;

            //Creature Spawners
            foreach (UOACZUndeadSpawner undeadSpawner in UOACZUndeadSpawner.m_Spawners)
            {
                if (!UOACZRegion.ContainsItem(undeadSpawner))
                    continue;

                maxSpawnCount = undeadSpawner.MaxSpawnCount;
                amountToSpawn = 0;

                if (undeadSpawner.GuardHome)
                    amountToSpawn = maxSpawnCount;

                else
                {
                    if (undeadSpawner.Wilderness)
                        amountToSpawn = (int)(Math.Ceiling((double)maxSpawnCount * .2));

                    else
                        amountToSpawn = 0;
                }

                if (amountToSpawn > 0)
                    undeadSpawner.PerformSpawns(amountToSpawn);

                undeadSpawner.m_LastActivity = DateTime.UtcNow;
                spawnDelay = undeadSpawner.MinSpawnTime + (Utility.RandomDouble() * (undeadSpawner.MaxSpawnTime - undeadSpawner.MinSpawnTime));
                undeadSpawner.m_NextActivity = TimeSpan.FromMinutes(spawnDelay);
            }

            foreach (UOACZWildlifeSpawner wildlifeSpawner in UOACZWildlifeSpawner.m_Spawners)
            {
                if (!UOACZRegion.ContainsItem(wildlifeSpawner))
                    continue;

                maxSpawnCount = wildlifeSpawner.MaxSpawnCount;
                amountToSpawn = (int)(Math.Ceiling((double)maxSpawnCount * .1));

                wildlifeSpawner.PerformSpawns(amountToSpawn);

                wildlifeSpawner.m_LastActivity = DateTime.UtcNow;
                spawnDelay = wildlifeSpawner.MinSpawnTime + (Utility.RandomDouble() * (wildlifeSpawner.MaxSpawnTime - wildlifeSpawner.MinSpawnTime));
                wildlifeSpawner.m_NextActivity = TimeSpan.FromMinutes(spawnDelay);
            }

            foreach (UOACZMilitiaSpawner militiaSpawner in UOACZMilitiaSpawner.m_Spawners)
            {
                if (!UOACZRegion.ContainsItem(militiaSpawner))
                    continue;

                maxSpawnCount = militiaSpawner.MaxSpawnCount;
                amountToSpawn = 0;

                if (militiaSpawner.Wilderness)
                {
                    if (Utility.RandomDouble() <= .5)
                        amountToSpawn = 1;
                }

                else
                    amountToSpawn = (int)(Math.Ceiling((double)maxSpawnCount * 1.0));

                militiaSpawner.PerformSpawns(amountToSpawn);

                militiaSpawner.m_LastActivity = DateTime.UtcNow;
                spawnDelay = militiaSpawner.MinSpawnTime + (Utility.RandomDouble() * (militiaSpawner.MaxSpawnTime - militiaSpawner.MinSpawnTime));
                militiaSpawner.m_NextActivity = TimeSpan.FromMinutes(spawnDelay);
            }

            foreach (UOACZCivilianSpawner civilianSpawner in UOACZCivilianSpawner.m_Spawners)
            {
                if (!UOACZRegion.ContainsItem(civilianSpawner))
                    continue;

                maxSpawnCount = civilianSpawner.MaxSpawnCount;
                amountToSpawn = 0;

                if (civilianSpawner.Wilderness)
                {
                    if (Utility.RandomDouble() <= .5)
                        amountToSpawn = 1;
                }

                else
                    amountToSpawn = (int)(Math.Ceiling((double)maxSpawnCount * 1.0));

                civilianSpawner.PerformSpawns(amountToSpawn);

                civilianSpawner.m_LastActivity = DateTime.UtcNow;
                spawnDelay = civilianSpawner.MinSpawnTime + (Utility.RandomDouble() * (civilianSpawner.MaxSpawnTime - civilianSpawner.MinSpawnTime));
                civilianSpawner.m_NextActivity = TimeSpan.FromMinutes(spawnDelay);
            }

            //Item Spawners
            foreach (UOACZCityScavengeSpawner cityScavengeSpawner in UOACZCityScavengeSpawner.m_Spawners)
            {
                if (!UOACZRegion.ContainsItem(cityScavengeSpawner))
                    continue;

                maxSpawnCount = cityScavengeSpawner.MaxSpawnCount;
                amountToSpawn = (int)(Math.Ceiling((double)maxSpawnCount * .33));

                cityScavengeSpawner.PerformSpawns(amountToSpawn);

                cityScavengeSpawner.m_LastActivity = DateTime.UtcNow;
                spawnDelay = cityScavengeSpawner.MinSpawnTime + (Utility.RandomDouble() * (cityScavengeSpawner.MaxSpawnTime - cityScavengeSpawner.MinSpawnTime));
                cityScavengeSpawner.m_NextActivity = TimeSpan.FromMinutes(spawnDelay);
            }

            foreach (UOACZScavengeCottonSpawner scavengeCottonSpawner in UOACZScavengeCottonSpawner.m_Spawners)
            {
                if (!UOACZRegion.ContainsItem(scavengeCottonSpawner))
                    continue;

                maxSpawnCount = scavengeCottonSpawner.MaxSpawnCount;
                amountToSpawn = (int)(Math.Ceiling((double)maxSpawnCount * .1));

                scavengeCottonSpawner.PerformSpawns(amountToSpawn);

                scavengeCottonSpawner.m_LastActivity = DateTime.UtcNow;
                spawnDelay = scavengeCottonSpawner.MinSpawnTime + (Utility.RandomDouble() * (scavengeCottonSpawner.MaxSpawnTime - scavengeCottonSpawner.MinSpawnTime));
                scavengeCottonSpawner.m_NextActivity = TimeSpan.FromMinutes(spawnDelay);
            }

            foreach (UOACZScavengeFishingSpawner scavengeFishingSpawner in UOACZScavengeFishingSpawner.m_Spawners)
            {
                if (!UOACZRegion.ContainsItem(scavengeFishingSpawner))
                    continue;

                if (Utility.RandomDouble() <= .125)
                    scavengeFishingSpawner.PerformSpawns(1);
            }

            foreach (UOACZScavengeOreSpawner scavengeOreSpawner in UOACZScavengeOreSpawner.m_Spawners)
            {
                if (!UOACZRegion.ContainsItem(scavengeOreSpawner))
                    continue;

                if (Utility.RandomDouble() <= .2)
                    scavengeOreSpawner.PerformSpawns(1);
            }

            foreach (UOACZScavengeVegetationSpawner vegetationSpawner in UOACZScavengeVegetationSpawner.m_Spawners)
            {
                if (!UOACZRegion.ContainsItem(vegetationSpawner))
                    continue;

                maxSpawnCount = vegetationSpawner.MaxSpawnCount;
                amountToSpawn = (int)(Math.Ceiling((double)maxSpawnCount * .2));

                vegetationSpawner.PerformSpawns(amountToSpawn);

                vegetationSpawner.m_LastActivity = DateTime.UtcNow;
                spawnDelay = vegetationSpawner.MinSpawnTime + (Utility.RandomDouble() * (vegetationSpawner.MaxSpawnTime - vegetationSpawner.MinSpawnTime));
                vegetationSpawner.m_NextActivity = TimeSpan.FromMinutes(spawnDelay);
            }

            foreach (UOACZWaterSpawner waterSpawner in UOACZWaterSpawner.m_Spawners)
            {
                if (!UOACZRegion.ContainsItem(waterSpawner))
                    continue;

                amountToSpawn = 0;
                
                maxSpawnCount = waterSpawner.MaxSpawnCount;
                amountToSpawn = (int)(Math.Ceiling((double)maxSpawnCount * .1));                

                waterSpawner.PerformSpawns(amountToSpawn);

                waterSpawner.m_LastActivity = DateTime.UtcNow;
                spawnDelay = waterSpawner.MinSpawnTime + (Utility.RandomDouble() * (waterSpawner.MaxSpawnTime - waterSpawner.MinSpawnTime));
                waterSpawner.m_NextActivity = TimeSpan.FromMinutes(spawnDelay);
            }

            foreach (UOACZReagentSpawner reagentSpawner in UOACZReagentSpawner.m_Spawners)
            {
                if (!UOACZRegion.ContainsItem(reagentSpawner))
                    continue;

                maxSpawnCount = reagentSpawner.MaxSpawnCount;
                amountToSpawn = (int)(Math.Ceiling((double)maxSpawnCount * .04));

                reagentSpawner.PerformSpawns(amountToSpawn);

                reagentSpawner.m_LastActivity = DateTime.UtcNow;
                spawnDelay = reagentSpawner.MinSpawnTime + (Utility.RandomDouble() * (reagentSpawner.MaxSpawnTime - reagentSpawner.MinSpawnTime));
                reagentSpawner.m_NextActivity = TimeSpan.FromMinutes(spawnDelay);
            }

            foreach (UOACZSpawner spawner in UOACZSpawner.m_UOACZSpawners)
            {
                if (!UOACZRegion.ContainsItem(spawner))
                    continue;

                if (spawner.m_SpawnTimer != null)
                    spawner.m_SpawnTimer.Start();
            }

            //Tunnels
            foreach (UOACZTunnel tunnel in UOACZTunnel.m_Instances)
            {
                if (!UOACZRegion.ContainsItem(tunnel))
                    continue;               

                tunnel.Visible = false;
            }

            //Outpost            
            foreach (UOACZBreakableStatic breakableStatic in UOACZPersistance.m_OutpostComponents)
            {
                if (breakableStatic == null)
                    continue;

                double condition = Utility.RandomDouble();

                if (condition <= .25)
                    breakableStatic.HitPoints = 0;
                else
                {
                    condition *= .666;
                    breakableStatic.HitPoints = (int)(Math.Round(condition * (double)breakableStatic.MaxHitPoints));
                }                
            }

            //Stockpiles
            UOACZStockpile stockpile = new UOACZStockpile();
            stockpile.DisplayName = "Outpost";
            stockpile.MoveToWorld(new Point3D(1653, 1244, -90), UOACZRegion.Facet);

            stockpile = new UOACZStockpile();
            stockpile.DisplayName = "Southwest";
            stockpile.MoveToWorld(new Point3D(1990, 1367, -80), UOACZRegion.Facet);

            stockpile = new UOACZStockpile();
            stockpile.DisplayName = "Southeast";
            stockpile.MoveToWorld(new Point3D(2093, 1375, -85), UOACZRegion.Facet);

            stockpile = new UOACZStockpile();
            stockpile.DisplayName = "Northeast";
            stockpile.MoveToWorld(new Point3D(2058, 1304, -90), UOACZRegion.Facet);

            stockpile = new UOACZStockpile();
            stockpile.DisplayName = "Northwest";
            stockpile.MoveToWorld(new Point3D(2004, 1304, -90), UOACZRegion.Facet);            

            //Bonebox
            UOACZBoneBox bonebox = new UOACZBoneBox();
            bonebox.MoveToWorld(new Point3D(2037, 1360, -85), UOACZRegion.Facet);

            //Corruption SourceStone
            UOACZCorruptionSourcestone corruptionSourceStone = new UOACZCorruptionSourcestone();
            corruptionSourceStone.MoveToWorld(new Point3D(2367, 1127, -90), UOACZRegion.Facet);

            SetObjectives();
        }

        public static void SetObjectives()
        {
            UOACZPersistance.m_HumanObjective1Target = UOACZCorruptionSourcestone.DefaultMaxHitPoints;
            UOACZPersistance.m_HumanObjective2Target = UOACZPersistance.m_OutpostComponents.Count;
            UOACZPersistance.m_HumanObjective3Target = UOACZSystem.HumanObjectiveBonesNeeded;
            UOACZPersistance.m_HumanObjective4Target = 1; //Kill Undead Champion
            UOACZPersistance.m_HumanObjective5Target = 1; //Kill Undead Boss

            UOACZPersistance.m_UndeadObjective1Target = UOACZSystem.UndeadObjectiveRequiredStockpilesDestroyed * UOACZStockpile.StartingMaxHits;
            UOACZPersistance.m_UndeadObjective2Target = (int)(Math.Floor((double)UOACZBaseCivilian.GetActiveInstances().Count * UOACZSystem.UndeadObjectivePercentOfCivilians));
            UOACZPersistance.m_UndeadObjective3Target = UOACZSystem.UndeadObjectiveSpreadCorruption;
            UOACZPersistance.m_UndeadObjective4Target = 1; //Kill Human Champion
            UOACZPersistance.m_UndeadObjective5Target = 1; //Kill Human Boss 
        }

        public static void ObjectiveCompleted(bool human)
        {
            if (!UOACZPersistance.Active)
                return;

            if (UOACZPersistance.HumanObjectivesComplete >= UOACZSystem.ObjectivesNeededForVictory || UOACZPersistance.UndeadObjectivesComplete >= UOACZSystem.ObjectivesNeededForVictory)
                return;

            if (human)
                UOACZPersistance.HumanObjectivesComplete++;

            else
                UOACZPersistance.UndeadObjectivesComplete++;

            bool victoryOccurred = false;

            bool humanVictory = false;
            bool undeadVictory = false;

            string message = "";

            if (UOACZPersistance.HumanObjectivesComplete >= UOACZSystem.ObjectivesNeededForVictory)
            {
                victoryOccurred = true;
                humanVictory = true;

                message = "UOACZ has ended. The Humans are victorious!";
            }

            if (UOACZPersistance.UndeadObjectivesComplete >= UOACZSystem.ObjectivesNeededForVictory)
            {
                victoryOccurred = true;
                undeadVictory = true;

                message = "UOACZ has ended. The Undead are victorious!";
            }       

            foreach (NetState state in NetState.Instances)
            {
                Mobile mobile = state.Mobile;
                PlayerMobile player = mobile as PlayerMobile;

                if (player == null)
                    continue;

                RefreshAllGumps(player);

                if (victoryOccurred)
                {
                    if (UOACZRegion.ContainsMobile(player) || player.AccessLevel > AccessLevel.Player)
                        player.SendMessage(UOACZSystem.purpleTextHue, message);
                }
            }

            if (!victoryOccurred)
                return;    

            foreach (UOACZAccountEntry playerEntry in UOACZPersistance.m_UOACZAccountEntries)
            {
                if (playerEntry.MostRecentPlayer == null)
                    continue;

                if (playerEntry.CurrentSessionHumanScore >= UOACZSystem.MinScoreToQualifyAsParticipant && humanVictory)
                {
                    AchievementSystemImpl.Instance.TickProgressMulti(playerEntry.MostRecentPlayer, AchievementTriggers.Trigger_UOACZHumanVictory, 1);
                    playerEntry.HumanVictoryCount++;
                }

                if (playerEntry.CurrentSessionUndeadScore >= UOACZSystem.MinScoreToQualifyAsParticipant && undeadVictory)
                {
                    AchievementSystemImpl.Instance.TickProgressMulti(playerEntry.MostRecentPlayer, AchievementTriggers.Trigger_UOACZUndeadVictory, 1);
                    playerEntry.UndeadVictoryCount++;
                }
            }

            Timer.DelayCall(TimeSpan.FromSeconds(5), delegate
            {
                if (UOACZPersistance.Active)
                    UOACZPersistance.EndSession();
            });
        }

        public static void OnLogin(PlayerMobile player)
        {
            if (player == null)
                return;

            player.m_UOACZAccountEntry = UOACZPersistance.FindUOACZAccountEntry(player);

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);            

            CheckForOtherAccountPlayerStillInUOACZ(player);

            player.m_UOACZAccountEntry.ActiveProfile = UOACZAccountEntry.ActiveProfileType.None;

            if (UOACZPersistance.Active && player.Region is UOACZRegion && player.AccessLevel == AccessLevel.Player)
            {
                DetermineAndApplyPlayerProfile(player);
                
                player.Hits = player.HitsMax;
                player.Stam = player.StamMax;
                player.Mana = player.ManaMax;

                player.ResetRegenTimers();                

                player.Hidden = false;
                
                if (player.m_UOACZAccountEntry.ActiveProfile == UOACZAccountEntry.ActiveProfileType.Human)
                    UOACZSystem.EquipUOACZItems(player, true);

                if (player.m_UOACZAccountEntry.ActiveProfile == UOACZAccountEntry.ActiveProfileType.Undead)
                    UOACZSystem.EquipUOACZItems(player, false);                

                player.m_UOACZAccountEntry.MostRecentPlayer = player;
                player.m_UOACZAccountEntry.MostRecentPlayerString = player.RawName;
            }            
        }

        public static void DeleteAllHeldItems(PlayerMobile player)
        {
            if (player == null)
                return;

            Queue m_Queue = new Queue();

            List<Item> m_BackpackItems = new List<Item>();

            if (player.Backpack != null)
            {
                if (!player.Backpack.Deleted)
                {
                    List<Layer> layers = new List<Layer>();

                    layers.Add(Layer.Arms);
                    layers.Add(Layer.Bracelet);
                    layers.Add(Layer.Cloak);
                    layers.Add(Layer.Earrings);
                    layers.Add(Layer.Gloves);
                    layers.Add(Layer.Helm);
                    layers.Add(Layer.InnerLegs);
                    layers.Add(Layer.InnerTorso);
                    layers.Add(Layer.MiddleTorso);
                    layers.Add(Layer.Neck);
                    layers.Add(Layer.OneHanded);
                    layers.Add(Layer.OuterLegs);
                    layers.Add(Layer.OuterTorso);
                    layers.Add(Layer.Pants);
                    layers.Add(Layer.Ring);
                    layers.Add(Layer.Shirt);
                    layers.Add(Layer.Shoes);
                    layers.Add(Layer.Talisman);
                    layers.Add(Layer.TwoHanded);
                    layers.Add(Layer.Waist);

                    foreach (Layer layer in layers)
                    {
                        Item item = player.FindItemOnLayer(layer);

                        if (item == null) continue;
                        if (item.Deleted) continue;
                        if (item == player.Backpack) continue;

                        m_Queue.Enqueue(item);
                    }

                    m_BackpackItems = player.Backpack.Items;

                    foreach (Item item in m_BackpackItems)
                    {
                        //Item Overrides
                        if (item == player.Backpack) continue;
                        if (item is StaffBracelet) continue;

                        m_Queue.Enqueue(item);
                    }

                    while (m_Queue.Count > 0)
                    {
                        Item item = (Item)m_Queue.Dequeue();
                        item.Delete();
                    }
                }
            }
        }

        public static void DeleteAllAccountItems(PlayerMobile player)
        {
            if (player == null)
                return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            Queue m_Queue = new Queue();

            foreach (Item item in player.m_UOACZAccountEntry.m_UOACZHumanBackpackItems.Items)
            {
                m_Queue.Enqueue(item);
            }

            while (m_Queue.Count > 0)
            {
                Item item = (Item)m_Queue.Dequeue();
                item.Delete();
            }

            foreach (Item item in player.m_UOACZAccountEntry.m_UOACZHumanWornItems.Items)
            {
                m_Queue.Enqueue(item);
            }

            while (m_Queue.Count > 0)
            {
                Item item = (Item)m_Queue.Dequeue();
                item.Delete();
            }

            foreach (Item item in player.m_UOACZAccountEntry.m_UOACZUndeadBackpackItems.Items)
            {
                m_Queue.Enqueue(item);
            }

            while (m_Queue.Count > 0)
            {
                Item item = (Item)m_Queue.Dequeue();
                item.Delete();
            }

            foreach (Item item in player.m_UOACZAccountEntry.m_UOACZUndeadWornItems.Items)
            {
                m_Queue.Enqueue(item);
            }

            while (m_Queue.Count > 0)
            {
                Item item = (Item)m_Queue.Dequeue();
                item.Delete();
            }

            player.m_UOACZAccountEntry.m_UOACZHumanBackpackItems.Items.Clear();
            player.m_UOACZAccountEntry.m_UOACZHumanWornItems.Items.Clear();

            player.m_UOACZAccountEntry.m_UOACZUndeadBackpackItems.Items.Clear();
            player.m_UOACZAccountEntry.m_UOACZUndeadWornItems.Items.Clear();
        }

        public static void DepositRealWorldItems(PlayerMobile player)
        {
            if (player == null)
                return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);
            UOACZCharacterSnapshot snapshot = UOACZPersistance.FindCharacterSnapshot(player);

            if (snapshot == null)
                return;

            Queue m_Queue = new Queue();

            if (player.AccessLevel == AccessLevel.Player)
            {
                if (player.Backpack != null)
                {
                    List<Item> m_PlayerItems = player.Backpack.Items;

                    foreach (Item item in m_PlayerItems)
                    {
                        if (item == player.Backpack)
                            continue;

                        m_Queue.Enqueue(item);
                    }

                    while (m_Queue.Count > 0)
                    {
                        Item item = (Item)m_Queue.Dequeue();

                        int locationX = item.X;
                        int locationY = item.Y;

                        snapshot.m_RealWorldBackpackItems.DropItem(item);

                        item.X = locationX;
                        item.Y = locationY;
                    }
                }

                List<Layer> layers = new List<Layer>();

                layers.Add(Layer.Arms);
                layers.Add(Layer.Bracelet);
                layers.Add(Layer.Cloak);
                layers.Add(Layer.Earrings);
                layers.Add(Layer.Gloves);
                layers.Add(Layer.Helm);
                layers.Add(Layer.InnerLegs);
                layers.Add(Layer.InnerTorso);
                layers.Add(Layer.MiddleTorso);
                layers.Add(Layer.Neck);
                layers.Add(Layer.OneHanded);
                layers.Add(Layer.OuterLegs);
                layers.Add(Layer.OuterTorso);
                layers.Add(Layer.Pants);
                layers.Add(Layer.Ring);
                layers.Add(Layer.Shirt);
                layers.Add(Layer.Shoes);
                layers.Add(Layer.Talisman);
                layers.Add(Layer.TwoHanded);
                layers.Add(Layer.Waist);

                m_Queue = new Queue();

                foreach (Layer layer in layers)
                {
                    Item item = player.FindItemOnLayer(layer);

                    if (item == null) continue;
                    if (item.Deleted) continue;
                    if (item == player.Backpack) continue;
                    if (item is StaffBracelet) continue;

                    m_Queue.Enqueue(item);
                }

                while (m_Queue.Count > 0)
                {
                    Item item = (Item)m_Queue.Dequeue();

                    snapshot.m_RealWorldWornItems.DropItem(item);
                }
            }
        }

        public static void EquipUOACZItems(PlayerMobile player, bool human)
        {
            if (player == null)
                return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);
            
            Queue m_Queue = new Queue();

            if (human)
            {
                List<Item> m_BackpackItems = player.m_UOACZAccountEntry.m_UOACZHumanBackpackItems.Items;

                foreach (Item item in m_BackpackItems)
                {
                    m_Queue.Enqueue(item);
                }

                while (m_Queue.Count > 0)
                {
                    Item item = (Item)m_Queue.Dequeue();

                    int locationX = item.X;
                    int locationY = item.Y;

                    player.Backpack.DropItem(item);

                    item.X = locationX;
                    item.Y = locationY;
                }

                List<Item> m_WormItems = player.m_UOACZAccountEntry.m_UOACZHumanWornItems.Items;

                m_Queue = new Queue();

                foreach (Item item in m_WormItems)
                {
                    m_Queue.Enqueue(item);
                }

                while (m_Queue.Count > 0)
                {
                    Item item = (Item)m_Queue.Dequeue();
                    player.AddItem(item);
                }
            }

            else
            {
                List<Item> m_BackpackItems = player.m_UOACZAccountEntry.m_UOACZUndeadBackpackItems.Items;

                foreach (Item item in m_BackpackItems)
                {
                    m_Queue.Enqueue(item);
                }

                while (m_Queue.Count > 0)
                {
                    Item item = (Item)m_Queue.Dequeue();

                    int locationX = item.X;
                    int locationY = item.Y;

                    player.Backpack.DropItem(item);

                    item.X = locationX;
                    item.Y = locationY;
                }

                List<Item> m_WormItems = player.m_UOACZAccountEntry.m_UOACZUndeadWornItems.Items;

                m_Queue = new Queue();

                foreach (Item item in m_WormItems)
                {
                    m_Queue.Enqueue(item);
                }

                while (m_Queue.Count > 0)
                {
                    Item item = (Item)m_Queue.Dequeue();
                    player.AddItem(item);
                }
            }
        }

        public static void DepositUOACZItems(PlayerMobile player, bool human)
        {
            if (player == null)
                return;
            
            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            Queue m_Queue = new Queue();

            if (human)
            {
                if (player.Backpack != null)
                {
                    List<Item> m_PlayerItems = player.Backpack.Items;

                    foreach (Item item in m_PlayerItems)
                    {
                        if (item == player.Backpack)
                            continue;

                        m_Queue.Enqueue(item);
                    }

                    while (m_Queue.Count > 0)
                    {
                        Item item = (Item)m_Queue.Dequeue();

                        int locationX = item.X;
                        int locationY = item.Y;

                        player.m_UOACZAccountEntry.m_UOACZHumanBackpackItems.DropItem(item);

                        item.X = locationX;
                        item.Y = locationY;
                    }
                }

                List<Layer> layers = new List<Layer>();

                layers.Add(Layer.Arms);
                layers.Add(Layer.Bracelet);
                layers.Add(Layer.Cloak);
                layers.Add(Layer.Earrings);
                layers.Add(Layer.Gloves);
                layers.Add(Layer.Helm);
                layers.Add(Layer.InnerLegs);
                layers.Add(Layer.InnerTorso);
                layers.Add(Layer.MiddleTorso);
                layers.Add(Layer.Neck);
                layers.Add(Layer.OneHanded);
                layers.Add(Layer.OuterLegs);
                layers.Add(Layer.OuterTorso);
                layers.Add(Layer.Pants);
                layers.Add(Layer.Ring);
                layers.Add(Layer.Shirt);
                layers.Add(Layer.Shoes);
                layers.Add(Layer.Talisman);
                layers.Add(Layer.TwoHanded);
                layers.Add(Layer.Waist);

                m_Queue = new Queue();

                foreach (Layer layer in layers)
                {
                    Item item = player.FindItemOnLayer(layer);

                    if (item == null) continue;
                    if (item.Deleted) continue;
                    if (item == player.Backpack) continue;
                    if (item is StaffBracelet) continue;

                    m_Queue.Enqueue(item);
                }

                while (m_Queue.Count > 0)
                {
                    Item item = (Item)m_Queue.Dequeue();
                    player.m_UOACZAccountEntry.m_UOACZHumanWornItems.DropItem(item);
                }
            }

            else
            {
                if (player.Backpack != null)
                {
                    List<Item> m_PlayerItems = player.Backpack.Items;

                    foreach (Item item in m_PlayerItems)
                    {
                        if (item == player.Backpack)
                            continue;

                        m_Queue.Enqueue(item);
                    }

                    while (m_Queue.Count > 0)
                    {
                        Item item = (Item)m_Queue.Dequeue();

                        int locationX = item.X;
                        int locationY = item.Y;

                        player.m_UOACZAccountEntry.m_UOACZUndeadBackpackItems.DropItem(item);

                        item.X = locationX;
                        item.Y = locationY;
                    }
                }

                List<Layer> layers = new List<Layer>();

                layers.Add(Layer.Arms);
                layers.Add(Layer.Bracelet);
                layers.Add(Layer.Cloak);
                layers.Add(Layer.Earrings);
                layers.Add(Layer.Gloves);
                layers.Add(Layer.Helm);
                layers.Add(Layer.InnerLegs);
                layers.Add(Layer.InnerTorso);
                layers.Add(Layer.MiddleTorso);
                layers.Add(Layer.Neck);
                layers.Add(Layer.OneHanded);
                layers.Add(Layer.OuterLegs);
                layers.Add(Layer.OuterTorso);
                layers.Add(Layer.Pants);
                layers.Add(Layer.Ring);
                layers.Add(Layer.Shirt);
                layers.Add(Layer.Shoes);
                layers.Add(Layer.Talisman);
                layers.Add(Layer.TwoHanded);
                layers.Add(Layer.Waist);

                m_Queue = new Queue();

                foreach (Layer layer in layers)
                {
                    Item item = player.FindItemOnLayer(layer);

                    if (item == null) continue;
                    if (item.Deleted) continue;
                    if (item == player.Backpack) continue;
                    if (item is StaffBracelet) continue;

                    m_Queue.Enqueue(item);
                }

                while (m_Queue.Count > 0)
                {
                    Item item = (Item)m_Queue.Dequeue();
                    player.m_UOACZAccountEntry.m_UOACZUndeadWornItems.DropItem(item);
                }
            }
        }

        public static void EquipRealWorldItems(PlayerMobile player)
        {
            if (player == null)
                return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);
            UOACZCharacterSnapshot snapshot = UOACZPersistance.FindCharacterSnapshot(player);

            if (snapshot == null)
                return;

            bool needBackpack = false;

            if (player.Backpack == null)
                needBackpack = true;

            else if (player.Backpack.Deleted)
                needBackpack = true;

            if (needBackpack)
            {
                Backpack backpack = new Backpack();
                backpack.Movable = false;

                player.AddItem(backpack);
            }

            Queue m_Queue = new Queue();

            //Restore Real World Items
            if (player.AccessLevel == AccessLevel.Player)
            {
                if (snapshot.m_RealWorldBackpackItems != null)
                {
                    List<Item> m_BackpackItems = snapshot.m_RealWorldBackpackItems.Items;

                    foreach (Item item in m_BackpackItems)
                    {
                        m_Queue.Enqueue(item);
                    }

                    while (m_Queue.Count > 0)
                    {
                        Item item = (Item)m_Queue.Dequeue();

                        int locationX = item.X;
                        int locationY = item.Y;

                        player.Backpack.DropItem(item);

                        item.X = locationX;
                        item.Y = locationY;
                    }
                }

                if (snapshot.m_RealWorldWornItems != null)
                {
                    List<Item> m_WormItems = snapshot.m_RealWorldWornItems.Items;

                    m_Queue = new Queue();

                    foreach (Item item in m_WormItems)
                    {
                        m_Queue.Enqueue(item);
                    }

                    while (m_Queue.Count > 0)
                    {
                        Item item = (Item)m_Queue.Dequeue();
                        player.AddItem(item);
                    }
                }
            }
        }

        public static void ApplyActiveProfile(PlayerMobile player)
        {
            if (player == null)
                return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);
            UOACZCharacterSnapshot snapshot = UOACZPersistance.FindCharacterSnapshot(player);            

            switch (player.m_UOACZAccountEntry.ActiveProfile)
            {
                case UOACZAccountEntry.ActiveProfileType.None:
                    RemoveAllStatSkillMods(player);

                    player.BodyMod = 0;

                    if (snapshot != null)
                    {
                        player.HueMod = snapshot.m_HueMod;
                        player.FollowersMax = snapshot.m_FollowersMax;
                    }

                    player.AngerSound = -1;
                    player.IdleSound = -1;
                    player.AttackSound = -1;
                    player.HurtSound = -1;
                    player.DeathSound = -1;
                break;

                case UOACZAccountEntry.ActiveProfileType.Human:
                    UpdateStatsSkills(player);

                    player.BodyMod = 0;
                    player.FollowersMax = player.m_UOACZAccountEntry.HumanProfile.FollowersMax;

                    player.AngerSound = -1;
                    player.IdleSound = -1;
                    player.AttackSound = -1;
                    player.HurtSound = -1;
                    player.DeathSound = -1;

                    if (snapshot != null)                    
                        player.HueMod = snapshot.m_HueMod;                    
                break;

                case UOACZAccountEntry.ActiveProfileType.Undead:
                    UpdateStatsSkills(player);

                    UOACZUndeadUpgradeDetail upgradeDetail = UOACZUndeadUpgrades.GetUpgradeDetail(player.m_UOACZAccountEntry.UndeadProfile.ActiveForm);

                    player.m_UOACZAccountEntry.UndeadProfile.BodyValue = upgradeDetail.m_BodyValue;
                    player.m_UOACZAccountEntry.UndeadProfile.HueMod = upgradeDetail.m_HueMod;

                    if (player.m_UOACZAccountEntry.UndeadProfile.DyedHueMod != -1)
                        player.m_UOACZAccountEntry.UndeadProfile.HueMod = player.m_UOACZAccountEntry.UndeadProfile.DyedHueMod;

                    player.m_UOACZAccountEntry.UndeadProfile.IconItemID = upgradeDetail.m_IconItemID;
                    player.m_UOACZAccountEntry.UndeadProfile.IconHue = upgradeDetail.m_IconHue;
                    player.m_UOACZAccountEntry.UndeadProfile.IconOffsetX = upgradeDetail.m_IconOffsetX;
                    player.m_UOACZAccountEntry.UndeadProfile.IconOffsetY = upgradeDetail.m_IconOffsetY;

                    player.m_UOACZAccountEntry.UndeadProfile.AngerSound = upgradeDetail.m_AngerSound;
                    player.m_UOACZAccountEntry.UndeadProfile.IdleSound = upgradeDetail.m_IdleSound;
                    player.m_UOACZAccountEntry.UndeadProfile.AttackSound = upgradeDetail.m_AttackSound;
                    player.m_UOACZAccountEntry.UndeadProfile.HurtSound = upgradeDetail.m_HurtSound;
                    player.m_UOACZAccountEntry.UndeadProfile.DeathSound = upgradeDetail.m_DeathSound;

                    player.m_UOACZAccountEntry.UndeadProfile.AttackAnimation = upgradeDetail.m_AttackAnimation;
                    player.m_UOACZAccountEntry.UndeadProfile.AttackAnimationFrames = upgradeDetail.m_AttackAnimationFrames;

                    player.m_UOACZAccountEntry.UndeadProfile.IdleAnimation = upgradeDetail.m_IdleAnimation;
                    player.m_UOACZAccountEntry.UndeadProfile.IdleAnimationFrames = upgradeDetail.m_IdleAnimationFrames;

                    player.m_UOACZAccountEntry.UndeadProfile.HurtAnimation = upgradeDetail.m_HurtAnimation;
                    player.m_UOACZAccountEntry.UndeadProfile.HurtAnimationFrames = upgradeDetail.m_HurtAnimationFrames;

                    player.m_UOACZAccountEntry.UndeadProfile.SpecialAnimation = upgradeDetail.m_SpecialAnimation;
                    player.m_UOACZAccountEntry.UndeadProfile.SpecialAnimationFrames = upgradeDetail.m_SpecialAnimationFrames;

                    player.m_UOACZAccountEntry.UndeadProfile.CastingAnimation = upgradeDetail.m_CastingAnimation;
                    player.m_UOACZAccountEntry.UndeadProfile.CastingAnimationFrames = upgradeDetail.m_CastingAnimationFrames;

                    player.BodyMod = player.m_UOACZAccountEntry.UndeadProfile.BodyValue;
                    player.HueMod = player.m_UOACZAccountEntry.UndeadProfile.HueMod;
                    player.FollowersMax = player.m_UOACZAccountEntry.UndeadProfile.FollowersMax;

                    player.AngerSound = player.m_UOACZAccountEntry.UndeadProfile.AngerSound;
                    player.IdleSound = player.m_UOACZAccountEntry.UndeadProfile.IdleSound;
                    player.AttackSound = player.m_UOACZAccountEntry.UndeadProfile.AttackSound;
                    player.HurtSound = player.m_UOACZAccountEntry.UndeadProfile.HurtSound;
                    player.DeathSound = player.m_UOACZAccountEntry.UndeadProfile.DeathSound;
                break;
            }
        }

        public static void UpdateStatsSkills(PlayerMobile player)
        {
            if (player == null)
                return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            int currentHits = player.Hits;
            int currentStam = player.Stam;
            int currentMana = player.Mana;

            RemoveAllStatSkillMods(player);

            UOACZStatMod strMod;
            UOACZStatMod dexMod;
            UOACZStatMod intMod;

            int strBase;
            int dexBase;
            int intBase;

            switch (player.m_UOACZAccountEntry.ActiveProfile)
            {
                case UOACZAccountEntry.ActiveProfileType.None:
                break;

                case UOACZAccountEntry.ActiveProfileType.Human:
                    strBase = player.m_UOACZAccountEntry.HumanProfile.m_Stats[StatType.Str];
                    dexBase = player.m_UOACZAccountEntry.HumanProfile.m_Stats[StatType.Dex];
                    intBase = player.m_UOACZAccountEntry.HumanProfile.m_Stats[StatType.Int];

                    double hungerScalar = 1;
                    int missingHunger = UOACZSystem.MissingHungerStatReductionThreshold - player.m_UOACZAccountEntry.HumanProfile.HungerPoints;

                    if (missingHunger > 0)
                        hungerScalar = 1 - (UOACZSystem.MissingThirstSkillReduction * (double)missingHunger);

                    int strAdjusted = (int)(Math.Round((double)strBase * hungerScalar));
                    int dexAdjusted = (int)(Math.Round((double)dexBase * hungerScalar));
                    int intAdjusted = (int)(Math.Round((double)intBase * hungerScalar));

                    strMod = new UOACZStatMod(StatType.Str, strAdjusted, player.RawStr);
                    player.AddStatMod(strMod);

                    dexMod = new UOACZStatMod(StatType.Dex, dexAdjusted, player.RawDex);
                    player.AddStatMod(dexMod);

                    intMod = new UOACZStatMod(StatType.Int, intAdjusted, player.RawInt);
                    player.AddStatMod(intMod);

                    double thirstScalar = 1;
                    int missingThirst = UOACZSystem.MissingThirstSkillReductionThreshold - player.m_UOACZAccountEntry.HumanProfile.ThirstPoints;

                    if (missingThirst > 0)
                        thirstScalar = 1 - (UOACZSystem.MissingThirstSkillReduction * (double)missingThirst);

                    for (int a = 0; a < player.Skills.Length; a++)
                    {
                        SkillName skillName = player.Skills[a].SkillName;
                        double skillValue = player.m_UOACZAccountEntry.HumanProfile.m_Skills[skillName];

                        skillValue *= thirstScalar;

                        UOACZSkillMod skillMod = new UOACZSkillMod(skillName, false, skillValue);
                        player.AddSkillMod(skillMod);
                    }

                    player.Hits = currentHits;
                    player.Stam = currentStam;
                    player.Mana = currentMana;
                break;

                case UOACZAccountEntry.ActiveProfileType.Undead:
                    strBase = player.m_UOACZAccountEntry.UndeadProfile.m_Stats[StatType.Str];
                    dexBase = player.m_UOACZAccountEntry.UndeadProfile.m_Stats[StatType.Dex];
                    intBase = player.m_UOACZAccountEntry.UndeadProfile.m_Stats[StatType.Int];

                    strMod = new UOACZStatMod(StatType.Str, strBase, player.RawStr);
                    player.AddStatMod(strMod);

                    dexMod = new UOACZStatMod(StatType.Dex, dexBase, player.RawDex);
                    player.AddStatMod(dexMod);

                    intMod = new UOACZStatMod(StatType.Int, intBase, player.RawInt);
                    player.AddStatMod(intMod);

                    for (int a = 0; a < player.Skills.Length; a++)
                    {
                        SkillName skillName = player.Skills[a].SkillName;
                        double skillValue = player.m_UOACZAccountEntry.UndeadProfile.m_Skills[skillName];

                        UOACZSkillMod skillMod = new UOACZSkillMod(skillName, false, skillValue);
                        player.AddSkillMod(skillMod);
                    }

                    player.Hits = currentHits;
                    player.Stam = currentStam;
                    player.Mana = currentMana;
                break;
            }
                        
            player.ValidateSkillMods();
            player.InvalidateProperties();
        }

        public static bool CheckAllowSkillUse(Mobile from, int Skill)
        {
            PlayerMobile player = from as PlayerMobile;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            bool allowUsage = true;

            if (from.AccessLevel == AccessLevel.Player)
            {
                switch (player.m_UOACZAccountEntry.ActiveProfile)
                {
                    case UOACZAccountEntry.ActiveProfileType.Human:
                        switch (Skill)
                        {
                            case (int)SkillName.Anatomy: allowUsage = false; break;
                            case (int)SkillName.AnimalLore: allowUsage = false; break;
                            case (int)SkillName.AnimalTaming: allowUsage = false; break;
                            case (int)SkillName.Begging: allowUsage = false; break;
                            case (int)SkillName.DetectHidden: allowUsage = false; break;
                            case (int)SkillName.Discordance: allowUsage = false; break;
                            case (int)SkillName.EvalInt: allowUsage = false; break;
                            case (int)SkillName.Forensics: allowUsage = false; break;
                            case (int)SkillName.Herding: allowUsage = false; break;
                            case (int)SkillName.Hiding: allowUsage = false; break;
                            case (int)SkillName.ItemID: allowUsage = false; break;
                            case (int)SkillName.Meditation: allowUsage = false; break;
                            case (int)SkillName.Musicianship: allowUsage = false; break;
                            case (int)SkillName.Peacemaking: allowUsage = false; break;
                            case (int)SkillName.Poisoning: allowUsage = false; break;
                            case (int)SkillName.Provocation: allowUsage = false; break;
                            case (int)SkillName.Snooping: allowUsage = false; break;
                            case (int)SkillName.SpiritSpeak: allowUsage = false; break;
                            case (int)SkillName.Stealing: allowUsage = false; break;
                            case (int)SkillName.Stealth: allowUsage = false; break;
                            case (int)SkillName.TasteID: allowUsage = false; break;
                        }
                        break;

                    case UOACZAccountEntry.ActiveProfileType.Undead:
                        switch (Skill)
                        {
                            case (int)SkillName.Anatomy: allowUsage = false; break;
                            case (int)SkillName.AnimalLore: allowUsage = false; break;
                            case (int)SkillName.AnimalTaming: allowUsage = false; break;
                            case (int)SkillName.ArmsLore: allowUsage = false; break;
                            case (int)SkillName.Begging: allowUsage = false; break;
                            case (int)SkillName.DetectHidden: allowUsage = false; break;
                            case (int)SkillName.Discordance: allowUsage = false; break;
                            case (int)SkillName.EvalInt: allowUsage = false; break;
                            case (int)SkillName.Forensics: allowUsage = false; break;
                            case (int)SkillName.Herding: allowUsage = false; break;
                            case (int)SkillName.Hiding: allowUsage = false; break;
                            case (int)SkillName.ItemID: allowUsage = false; break;
                            case (int)SkillName.Meditation: allowUsage = false; break;
                            case (int)SkillName.Musicianship: allowUsage = false; break;
                            case (int)SkillName.Peacemaking: allowUsage = false; break;
                            case (int)SkillName.Poisoning: allowUsage = false; break;
                            case (int)SkillName.Provocation: allowUsage = false; break;
                            case (int)SkillName.RemoveTrap: allowUsage = false; break;
                            case (int)SkillName.Snooping: allowUsage = false; break;
                            case (int)SkillName.SpiritSpeak: allowUsage = false; break;
                            case (int)SkillName.Stealing: allowUsage = false; break;
                            case (int)SkillName.Stealth: allowUsage = false; break;
                            case (int)SkillName.TasteID: allowUsage = false; break;
                        }
                        break;
                }

                if (!allowUsage)
                {
                    from.SendMessage("That skill does not appear to work here.");
                    return false;
                }
            }

            return true;
        }

        public static bool IsUndeadUsableItem(PlayerMobile player, Item item)
        {
            if (item is Backpack && item.RootParentEntity == player && item.Layer == Layer.Backpack)
                return true;

            if (item is UOACZMoongate || item is UOACZTunnel || item is BreakableStatic)
                return true;

            if (item is UOACZSurvivalTome || item is UOACZCorruptionTome || item is UOACZCorruptionStone ||
                item is UOACZUndeadUpgradeToken || item is UOACZBrains || item is UOACZCorruptionBox || item is UOACZUndeadDye)
                return true;

            return false;
        }

        public static int GetModdedStatValue(PlayerMobile player, int value)
        {
            if (player == null)
                return value;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            int statAdjusted = 0;

            if (player.m_UOACZAccountEntry.ActiveProfile == UOACZAccountEntry.ActiveProfileType.Human)
            {
                int missingHunger = MissingHungerStatReductionThreshold - player.m_UOACZAccountEntry.HumanProfile.HungerPoints;

                if (missingHunger <= 0)
                    return value;

                statAdjusted = (int)(Math.Round((1 - (MissingHungerStatReduction * (double)missingHunger)) * (double)value));
            }

            return statAdjusted;
        }

        public static int GetModdedSkillValue(PlayerMobile player, int value)
        {
            if (player == null)
                return value;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            int skillAdjusted = 0;

            if (player.m_UOACZAccountEntry.ActiveProfile == UOACZAccountEntry.ActiveProfileType.Human)
            {
                int missingThirst = MissingThirstSkillReductionThreshold - player.m_UOACZAccountEntry.HumanProfile.ThirstPoints;

                if (missingThirst <= 0)
                    return value;

                skillAdjusted = (int)(Math.Round((1 - (MissingThirstSkillReduction * (double)missingThirst)) * (double)value));
            }

            return skillAdjusted;
        }

        public static string GetHungerStatusText(PlayerMobile player)
        {
            string hungerStatus = "";

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            int hunger = player.m_UOACZAccountEntry.HumanProfile.HungerPoints;
            int missingHunger = 0;
            double hungerStatloss = 0;
            string hungerStatlossPercent = "";

            if (hunger < UOACZSystem.MissingHungerStatReductionThreshold)
                missingHunger = UOACZSystem.MissingHungerStatReductionThreshold - hunger;

            hungerStatloss = UOACZSystem.MissingHungerStatReduction * (double)missingHunger;
            hungerStatlossPercent = Utility.CreateDecimalPercentageString(hungerStatloss, 1);

            if (hunger == 0)
                hungerStatus = "";

            else if (hunger < 10)
                hungerStatus = "You are starving to death.";

            else if (hunger < 20)
                hungerStatus = "You feel wracked with hunger pain.";

            else if (hunger < 30)
                hungerStatus = "You feel crippled by hunger.";

            else if (hunger < 40)
                hungerStatus = "You feel very weak from hunger.";

            else if (hunger < 50)
                hungerStatus = "You feel weak from hunger.";

            else if (hunger < 60)
                hungerStatus = "You feel very hungry.";

            else if (hunger < 70)
                hungerStatus = "You feel moderately hungry.";

            else if (hunger < 80)
                hungerStatus = "You feel slightly hungry.";

            else if (hunger < 90)
                hungerStatus = "You feel decently full.";

            else if (hunger < 100)
                hungerStatus = "You feel nearly full.";

            else
                hungerStatus = "You feel full.";

            if (missingHunger > 0)
                hungerStatus += " (-" + hungerStatlossPercent + " to stats)";

            return hungerStatus;
        }

        public static string GetThirstStatusText(PlayerMobile player)
        {
            string thirstStatus = "";

            int thirst = player.m_UOACZAccountEntry.HumanProfile.ThirstPoints;

            int missingThirst = 0;
            double thirstSkillLoss = 0;
            string thirstSkillLossPercent = "";

            if (thirst < UOACZSystem.MissingThirstSkillReductionThreshold)
                missingThirst = UOACZSystem.MissingThirstSkillReductionThreshold - thirst;

            thirstSkillLoss = UOACZSystem.MissingThirstSkillReduction * (double)missingThirst;
            thirstSkillLossPercent = Utility.CreateDecimalPercentageString(thirstSkillLoss, 1);

            if (thirst == 0)
                thirstStatus = "";

            else if (thirst < 10)
                thirstStatus = "You are dying from thirst.";

            else if (thirst < 20)
                thirstStatus = "You feel delirious with pain from thirst.";

            else if (thirst < 30)
                thirstStatus = "You feel crippled from thirst.";

            else if (thirst < 40)
                thirstStatus = "You feel very weak from thirst.";

            else if (thirst < 50)
                thirstStatus = "You feel weak from thirst.";

            else if (thirst < 60)
                thirstStatus = "You feel very thirsty.";

            else if (thirst < 70)
                thirstStatus = "You feel moderately thirsty.";

            else if (thirst < 80)
                thirstStatus = "You feel slightly thirsty.";

            else if (thirst < 90)
                thirstStatus = "You feel decently hydrated.";

            else if (thirst < 100)
                thirstStatus = "You feel almost completely hydrated.";

            else
                thirstStatus = "You feel completely hydrated.";

            if (missingThirst > 0)
                thirstStatus += " (-" + thirstSkillLossPercent + " to skills)";

            return thirstStatus;
        }

        public static void RefreshHotbars(PlayerMobile player)
        {
            if (player == null) return;
            if (!player.Alive) return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            //Human
            if (player.HasGump(typeof(HumanProfileAbilitiesHotbarGump)))
            {
                player.m_UOACZAccountEntry.HumanAbilitiesHotbarDisplayed = true;

                player.CloseGump(typeof(HumanProfileAbilitiesHotbarGump));
                player.SendGump(new HumanProfileAbilitiesHotbarGump(player));
            }

            if (player.HasGump(typeof(HumanProfileStatsHotbarGump)))
            {
                player.m_UOACZAccountEntry.HumanStatsHotbarDisplayed = true;

                player.CloseGump(typeof(HumanProfileStatsHotbarGump));
                player.SendGump(new HumanProfileStatsHotbarGump(player));
            }

            //Undead
            if (player.HasGump(typeof(UndeadProfileAbilitiesHotbarGump)))
            {
                player.m_UOACZAccountEntry.UndeadAbilitiesHotbarDisplayed = true;

                player.CloseGump(typeof(UndeadProfileAbilitiesHotbarGump));
                player.SendGump(new UndeadProfileAbilitiesHotbarGump(player));
            }

            if (player.HasGump(typeof(UndeadProfileStatsHotbarGump)))
            {
                player.m_UOACZAccountEntry.UndeadStatsHotbarDisplayed = true;

                player.CloseGump(typeof(UndeadProfileStatsHotbarGump));
                player.SendGump(new UndeadProfileStatsHotbarGump(player));
            }

            //Objectives
            if (player.HasGump(typeof(ObjectivesHotbarGump)))
            {
                player.m_UOACZAccountEntry.ObjectivesDisplayed = true;

                player.CloseGump(typeof(ObjectivesHotbarGump));
                player.SendGump(new ObjectivesHotbarGump(player));
            }

            //Overview
            if (player.HasGump(typeof(UOACZOverviewGump)))
            {
                player.CloseGump(typeof(UOACZOverviewGump));
                player.SendGump(new UOACZOverviewGump(player));
            }
        }

        public static void RefreshAllGumps(PlayerMobile player)
        {
            if (player == null)
                return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            //Human
            if (player.HasGump(typeof(HumanProfileGump)))
            {
                player.CloseGump(typeof(HumanProfileGump));
                player.SendGump(new HumanProfileGump(player));
            }

            if (player.HasGump(typeof(HumanProfileUpgradeGump)))
            {
                player.CloseGump(typeof(HumanProfileUpgradeGump));
                player.SendGump(new HumanProfileUpgradeGump(player));
            }

            if (player.HasGump(typeof(HumanProfileAbilitiesHotbarGump)))
            {
                player.m_UOACZAccountEntry.HumanAbilitiesHotbarDisplayed = true;

                player.CloseGump(typeof(HumanProfileAbilitiesHotbarGump));
                player.SendGump(new HumanProfileAbilitiesHotbarGump(player));
            }

            if (player.HasGump(typeof(HumanProfileStatsHotbarGump)))
            {
                player.m_UOACZAccountEntry.HumanStatsHotbarDisplayed = true;

                player.CloseGump(typeof(HumanProfileStatsHotbarGump));
                player.SendGump(new HumanProfileStatsHotbarGump(player));
            }

            //Undead
            if (player.HasGump(typeof(UndeadProfileGump)))
            {
                player.CloseGump(typeof(UndeadProfileGump));
                player.SendGump(new UndeadProfileGump(player));
            }

            if (player.HasGump(typeof(UndeadProfileUpgradeGump)))
            {
                player.CloseGump(typeof(UndeadProfileUpgradeGump));
                player.SendGump(new UndeadProfileUpgradeGump(player));
            }

            if (player.HasGump(typeof(UndeadProfileAbilitiesHotbarGump)))
            {
                player.m_UOACZAccountEntry.UndeadAbilitiesHotbarDisplayed = true;

                player.CloseGump(typeof(UndeadProfileAbilitiesHotbarGump));
                player.SendGump(new UndeadProfileAbilitiesHotbarGump(player));
            }

            if (player.HasGump(typeof(UndeadProfileStatsHotbarGump)))
            {
                player.m_UOACZAccountEntry.UndeadStatsHotbarDisplayed = true;

                player.CloseGump(typeof(UndeadProfileStatsHotbarGump));
                player.SendGump(new UndeadProfileStatsHotbarGump(player));
            }

            //Objectives
            if (player.HasGump(typeof(ObjectivesHotbarGump)))
            {
                player.m_UOACZAccountEntry.ObjectivesDisplayed = true;

                player.CloseGump(typeof(ObjectivesHotbarGump));
                player.SendGump(new ObjectivesHotbarGump(player));
            }

            //Overview
            if (player.HasGump(typeof(UOACZOverviewGump)))
            {
                player.CloseGump(typeof(UOACZOverviewGump));
                player.SendGump(new UOACZOverviewGump(player));
            }
        }

        public static double GetFatigueScalar(PlayerMobile player)
        {  
            double fatigueScalar = UOACZSystem.FatigueBaselineScalar;

            if (player == null)
                return fatigueScalar;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            if (player.m_UOACZAccountEntry.FatigueExpiration > DateTime.UtcNow)
                fatigueScalar = UOACZSystem.FatigueActiveScalar;

            return fatigueScalar;
        }

        public static bool IsUOACZValidMobile(Mobile target)
        {
            if (target == null) return false;
            if (!UOACZPersistance.Active) return false;
            if (!(target.Region is UOACZRegion)) return false;
            if (target.Deleted || !target.Alive) return false;
            if (!target.CanBeDamaged() || target.AccessLevel > AccessLevel.Player) return false;

            return true;
        }

        public static void ClearWorld()
        {
            Queue m_Queue = new Queue();

            foreach (Item item in World.Items.Values)
            {
                if (UOACZRegion.ContainsItem(item))
                {
                    bool deleteItem = true;

                    if (item.RootParentEntity is PlayerMobile)
                    {
                        PlayerMobile player = item.RootParentEntity as PlayerMobile;

                        if (player.AccessLevel > AccessLevel.Player)
                            deleteItem = false;
                    }

                    if (deleteItem)
                        m_Queue.Enqueue(item);
                }
            }

            while (m_Queue.Count > 0)
            {
                Item item = (Item)m_Queue.Dequeue();
                item.Delete();
            }

            foreach (Mobile mobile in World.Mobiles.Values)
            {
                if (UOACZRegion.ContainsMobile(mobile))
                {
                    if (mobile is BaseCreature)
                        m_Queue.Enqueue(mobile);
                }
            }

            while (m_Queue.Count > 0)
            {
                BaseCreature creature = (BaseCreature)m_Queue.Dequeue();
                creature.Delete();
            }
        }

        public static void BuildWorld()
        {
            Map map = Map.Felucca;

            UOACZWayPoint waypoint;
            List<Point3D> m_MilitiaSpawners = new List<Point3D>();
            List<Point3D> m_EliteSpawners = new List<Point3D>();
            List<Point3D> m_SentrySpawners = new List<Point3D>();
            UOACZMilitiaSpawner militiaSpawner;
            UOACZConstructionTile constructionTile;
            UOACZConstructionObjectEffectTargeter constructionTileTargeter;

            #region Felucca Session Stones / Moongates

            //Moongates
            List<Point3D> m_Moongates = new List<Point3D>();

            //Britain
            m_Moongates.Add(new Point3D(1422, 1703, 7));
            m_Moongates.Add(new Point3D(1442, 1701, 0));
            m_Moongates.Add(new Point3D(1427, 1670, 10));
            m_Moongates.Add(new Point3D(1426, 1571, 30));
            m_Moongates.Add(new Point3D(1487, 1627, 20));
            m_Moongates.Add(new Point3D(1648, 1619, 20));
            m_Moongates.Add(new Point3D(1601, 1596, 20));            

            foreach (Point3D point in m_Moongates)
            {
                UOACZMoongate moongate = new UOACZMoongate();
                moongate.GateDirection = UOACZMoongate.GateDirectionType.Entrance;
                moongate.MoveToWorld(point, map);
            }

            foreach (Point3D point in m_Moongates)
            {
                UOACZDestination destination = new UOACZDestination();
                destination.GateDirection = UOACZDestination.DirectionType.Exit;
                destination.MoveToWorld(point, map);
            }

            m_Moongates.Clear();

            //Buccaneers Den
            m_Moongates.Add(new Point3D(2722, 2190, 0));
            m_Moongates.Add(new Point3D(2741, 2163, -2));

            foreach (Point3D point in m_Moongates)
            {
                UOACZMoongate moongate = new UOACZMoongate();
                moongate.GateDirection = UOACZMoongate.GateDirectionType.Entrance;
                moongate.MoveToWorld(point, map);
            }

            foreach (Point3D point in m_Moongates)
            {
                UOACZDestination destination = new UOACZDestination();
                destination.MurdererFriendly = true;
                destination.GateDirection = UOACZDestination.DirectionType.Exit;
                destination.MoveToWorld(point, map);
            }

            #endregion

            map = UOACZRegion.Facet;

            switch (UOACZPersistance.CustomRegion)
            {
                case UOACZCustomRegion.Malas1:

                    #region Session Stones / Moongates

                    List<Point3D> m_ExitMoongates = new List<Point3D>();

                    //Town
                    m_ExitMoongates.Add(new Point3D(2033, 1333, -88));
                    m_ExitMoongates.Add(new Point3D(1957, 1368, -90));
                    m_ExitMoongates.Add(new Point3D(2027, 1409, -90));
                    m_ExitMoongates.Add(new Point3D(1978, 1285, -90));
                    m_ExitMoongates.Add(new Point3D(2107, 1339, -90));
                    m_ExitMoongates.Add(new Point3D(2060, 1266, -90));

                    //Wilderness
                    m_ExitMoongates.Add(new Point3D(1820, 1583, -110));
                    m_ExitMoongates.Add(new Point3D(2372, 1425, -90));
                    m_ExitMoongates.Add(new Point3D(2435, 1320, -95));
                    m_ExitMoongates.Add(new Point3D(2283, 1047, -95));
                    m_ExitMoongates.Add(new Point3D(1804, 1093, -90));
                    m_ExitMoongates.Add(new Point3D(1627, 1217, -89));
                    m_ExitMoongates.Add(new Point3D(1627, 1546, -110));

                    foreach (Point3D point in m_ExitMoongates)
                    {
                        UOACZMoongate moongate = new UOACZMoongate();
                        moongate = new UOACZMoongate();
                        moongate.GateDirection = UOACZMoongate.GateDirectionType.Exit;
                        moongate.MoveToWorld(point, map);
                    }

                    #endregion

                    #region Forts

                    //West Town Fort Location
                    Point3D WestFortLocation = new Point3D(1935, 1317, -90);

                    UOACZMultis.BuildTower(new Point3D(1947, 1310, -90), map, Direction.West);
                    UOACZMultis.BuildTower(new Point3D(1926, 1309, -90), map, Direction.South);
                    UOACZMultis.BuildTower(new Point3D(1926, 1328, -90), map, Direction.East);
                    UOACZMultis.BuildTower(new Point3D(1943, 1328, -90), map, Direction.West);

                    UOACZMultis.BuildGate(new Point3D(1935, 1308, -90), map, Direction.East);
                    UOACZMultis.BuildGate(new Point3D(1925, 1320, -90), map, Direction.North);
                    UOACZMultis.BuildGate(new Point3D(1933, 1329, -90), map, Direction.East);

                    //South
                    UOACZMultis.BuildConstructionPlatform(new Point3D(1937, 1328, -90), map, Direction.South);
                    constructionTile = new UOACZConstructionTile();
                    constructionTile.ObjectFacing = Direction.South;
                    constructionTile.MoveToWorld(new Point3D(1937, 1328, -70), map);

                    constructionTileTargeter = new UOACZConstructionObjectEffectTargeter();
                    constructionTileTargeter.ConstructionTile = constructionTile;
                    constructionTileTargeter.MoveToWorld(new Point3D(1937, 1330, -90), map);

                    UOACZMultis.BuildConstructionPlatform(new Point3D(1931, 1328, -90), map, Direction.South);
                    constructionTile = new UOACZConstructionTile();
                    constructionTile.ObjectFacing = Direction.South;
                    constructionTile.MoveToWorld(new Point3D(1931, 1328, -70), map);

                    constructionTileTargeter = new UOACZConstructionObjectEffectTargeter();
                    constructionTileTargeter.ConstructionTile = constructionTile;
                    constructionTileTargeter.MoveToWorld(new Point3D(1931, 1330, -90), map);

                    //West
                    UOACZMultis.BuildConstructionPlatform(new Point3D(1926, 1322, -90), map, Direction.West);
                    constructionTile = new UOACZConstructionTile();
                    constructionTile.ObjectFacing = Direction.West;
                    constructionTile.MoveToWorld(new Point3D(1926, 1322, -70), map);

                    constructionTileTargeter = new UOACZConstructionObjectEffectTargeter();
                    constructionTileTargeter.ConstructionTile = constructionTile;
                    constructionTileTargeter.MoveToWorld(new Point3D(1923, 1321, -90), map);

                    UOACZMultis.BuildConstructionPlatform(new Point3D(1926, 1315, -90), map, Direction.West);
                    constructionTile = new UOACZConstructionTile();
                    constructionTile.ObjectFacing = Direction.West;
                    constructionTile.MoveToWorld(new Point3D(1926, 1315, -70), map);

                    constructionTileTargeter = new UOACZConstructionObjectEffectTargeter();
                    constructionTileTargeter.ConstructionTile = constructionTile;
                    constructionTileTargeter.MoveToWorld(new Point3D(1923, 1314, -90), map);

                    //North
                    UOACZMultis.BuildConstructionPlatform(new Point3D(1932, 1309, -90), map, Direction.North);
                    constructionTile = new UOACZConstructionTile();
                    constructionTile.ObjectFacing = Direction.North;
                    constructionTile.MoveToWorld(new Point3D(1932, 1309, -70), map);

                    constructionTileTargeter = new UOACZConstructionObjectEffectTargeter();
                    constructionTileTargeter.ConstructionTile = constructionTile;
                    constructionTileTargeter.MoveToWorld(new Point3D(1931, 1306, -90), map);

                    UOACZMultis.BuildConstructionPlatform(new Point3D(1940, 1309, -90), map, Direction.North);
                    constructionTile = new UOACZConstructionTile();
                    constructionTile.ObjectFacing = Direction.North;
                    constructionTile.MoveToWorld(new Point3D(1940, 1309, -70), map);

                    constructionTileTargeter = new UOACZConstructionObjectEffectTargeter();
                    constructionTileTargeter.ConstructionTile = constructionTile;
                    constructionTileTargeter.MoveToWorld(new Point3D(1939, 1306, -90), map);

                    //BreakableStatics
                    new UOACZNorthSouthWallUnbreakable().MoveToWorld(new Point3D(1925, 1322, -90), map);
                    new UOACZEastWestWallUnbreakable().MoveToWorld(new Point3D(1932, 1308, -90), map);
                    new UOACZEastWestWallUnbreakable().MoveToWorld(new Point3D(1940, 1308, -90), map);
                    new UOACZEastWestWallUnbreakable().MoveToWorld(new Point3D(1944, 1308, -90), map);
                    new UOACZEastWestWallUnbreakable().MoveToWorld(new Point3D(1945, 1308, -90), map);
                    new UOACZEastWestWallUnbreakable().MoveToWorld(new Point3D(1946, 1308, -90), map);
                    new UOACZEastWestWallUnbreakable().MoveToWorld(new Point3D(1947, 1308, -90), map);

                    new UOACZNorthSouthWall().MoveToWorld(new Point3D(1944, 1325, -88), map);
                    new UOACZNorthSouthWall().MoveToWorld(new Point3D(1944, 1326, -90), map);
                    new UOACZNorthSouthWall().MoveToWorld(new Point3D(1944, 1327, -90), map);
                    new UOACZNorthSouthWall().MoveToWorld(new Point3D(1944, 1328, -90), map);
                    new UOACZNorthSouthWall().MoveToWorld(new Point3D(1944, 1329, -90), map);

                    new UOACZEastWestWall().MoveToWorld(new Point3D(1944, 1329, -90), map);
                    new UOACZEastWestWall().MoveToWorld(new Point3D(1943, 1329, -90), map);
                    new UOACZEastWestWall().MoveToWorld(new Point3D(1942, 1329, -90), map);
                    new UOACZEastWestWallTurret().MoveToWorld(new Point3D(1941, 1329, -90), map);
                    new UOACZEastWestWall().MoveToWorld(new Point3D(1940, 1329, -90), map);
                    new UOACZEastWestWall().MoveToWorld(new Point3D(1939, 1329, -90), map);
                    new UOACZEastWestWall().MoveToWorld(new Point3D(1938, 1329, -90), map);
                    new UOACZEastWestWall().MoveToWorld(new Point3D(1937, 1329, -90), map);

                    new UOACZEastWestWall().MoveToWorld(new Point3D(1932, 1329, -90), map);
                    new UOACZEastWestWall().MoveToWorld(new Point3D(1931, 1329, -90), map);
                    new UOACZEastWestWall().MoveToWorld(new Point3D(1930, 1329, -90), map);
                    new UOACZEastWestWallTurret().MoveToWorld(new Point3D(1929, 1329, -90), map);
                    new UOACZEastWestWall().MoveToWorld(new Point3D(1928, 1329, -90), map);
                    new UOACZEastWestWall().MoveToWorld(new Point3D(1927, 1329, -90), map);
                    new UOACZEastWestWall().MoveToWorld(new Point3D(1926, 1329, -90), map);

                    new UOACZNorthSouthWall().MoveToWorld(new Point3D(1925, 1329, -90), map);

                    new UOACZNorthSouthWall().MoveToWorld(new Point3D(1925, 1326, -90), map);
                    new UOACZNorthSouthWall().MoveToWorld(new Point3D(1925, 1325, -90), map);
                    new UOACZNorthSouthWallTurret().MoveToWorld(new Point3D(1925, 1324, -90), map);
                    new UOACZNorthSouthWall().MoveToWorld(new Point3D(1925, 1323, -90), map);

                    new UOACZNorthSouthWall().MoveToWorld(new Point3D(1925, 1321, -90), map);

                    new UOACZNorthSouthWall().MoveToWorld(new Point3D(1925, 1316, -90), map);

                    new UOACZNorthSouthWall().MoveToWorld(new Point3D(1925, 1314, -90), map);
                    new UOACZNorthSouthWall().MoveToWorld(new Point3D(1925, 1313, -90), map);
                    new UOACZNorthSouthWallTurret().MoveToWorld(new Point3D(1925, 1312, -90), map);
                    new UOACZNorthSouthWall().MoveToWorld(new Point3D(1925, 1311, -90), map);
                    new UOACZNorthSouthWall().MoveToWorld(new Point3D(1925, 1310, -90), map);

                    new UOACZEastWestWall().MoveToWorld(new Point3D(1927, 1308, -90), map);
                    new UOACZEastWestWall().MoveToWorld(new Point3D(1928, 1308, -90), map);
                    new UOACZEastWestWallTurret().MoveToWorld(new Point3D(1929, 1308, -90), map);
                    new UOACZEastWestWall().MoveToWorld(new Point3D(1930, 1308, -90), map);
                    new UOACZEastWestWall().MoveToWorld(new Point3D(1931, 1308, -90), map);

                    new UOACZEastWestWall().MoveToWorld(new Point3D(1933, 1308, -90), map);
                    new UOACZEastWestWall().MoveToWorld(new Point3D(1934, 1308, -90), map);

                    new UOACZEastWestWall().MoveToWorld(new Point3D(1939, 1308, -90), map);

                    new UOACZEastWestWall().MoveToWorld(new Point3D(1941, 1308, -90), map);
                    new UOACZEastWestWallTurret().MoveToWorld(new Point3D(1942, 1308, -90), map);
                    new UOACZEastWestWall().MoveToWorld(new Point3D(1943, 1308, -90), map);

                    new UOACZEastWestWall().MoveToWorld(new Point3D(1948, 1308, -90), map);

                    new UOACZNorthSouthWall().MoveToWorld(new Point3D(1948, 1309, -90), map);
                    new UOACZNorthSouthWall().MoveToWorld(new Point3D(1948, 1310, -90), map);
                    new UOACZNorthSouthWall().MoveToWorld(new Point3D(1948, 1311, -90), map);
                    new UOACZNorthSouthWall().MoveToWorld(new Point3D(1948, 1312, -90), map);
                    new UOACZNorthSouthWall().MoveToWorld(new Point3D(1948, 1313, -90), map);
                    new UOACZNorthSouthWall().MoveToWorld(new Point3D(1948, 1314, -90), map);
                    new UOACZNorthSouthWall().MoveToWorld(new Point3D(1948, 1315, -90), map);

                    m_EliteSpawners.Add(new Point3D(1934, 1337, -90));
                    m_EliteSpawners.Add(new Point3D(1916, 1318, -90));
                    m_EliteSpawners.Add(new Point3D(1934, 1300, -90));

                    m_EliteSpawners.Add(new Point3D(1935, 1318, -90));

                    m_SentrySpawners.Add(new Point3D(1943, 1328, -45));
                    m_SentrySpawners.Add(new Point3D(1942, 1328, -45));
                    //m_SentrySpawners.Add(new Point3D(1942, 1327, -45));

                    m_SentrySpawners.Add(new Point3D(1926, 1328, -45));
                    m_SentrySpawners.Add(new Point3D(1925, 1327, -45));
                    //m_SentrySpawners.Add(new Point3D(1925, 1328, -45));

                    m_SentrySpawners.Add(new Point3D(1925, 1309, -45));
                    m_SentrySpawners.Add(new Point3D(1926, 1308, -48));
                    //m_SentrySpawners.Add(new Point3D(1925, 1308, -45));

                    m_SentrySpawners.Add(new Point3D(1946, 1309, -45));
                    m_SentrySpawners.Add(new Point3D(1947, 1309, -48));
                    //m_SentrySpawners.Add(new Point3D(1946, 1310, -45));

                    //West Bridge Waypoint
                    UOACZWayPoint westBridgeWaypoint = new UOACZWayPoint();
                    westBridgeWaypoint.GotoSplitGroup = 10;
                    westBridgeWaypoint.WaypointType = UOACZWayPoint.UOACZWaypointType.UndeadTown;
                    westBridgeWaypoint.MoveToWorld(new Point3D(1949, 1320, -80), map);

                    //West Fort Approach Splitters
                    List<Point3D> m_WestFortApproachSplitters = new List<Point3D>();

                    m_WestFortApproachSplitters.Add(new Point3D(1922, 1354, -90));
                    m_WestFortApproachSplitters.Add(new Point3D(1896, 1349, -90));
                    m_WestFortApproachSplitters.Add(new Point3D(1891, 1316, -90));
                    m_WestFortApproachSplitters.Add(new Point3D(1906, 1268, -90));

                    foreach (Point3D point in m_WestFortApproachSplitters)
                    {
                        waypoint = new UOACZWayPoint();
                        waypoint.SplitGroup = 1;
                        waypoint.GotoSplitGroup = 2;
                        waypoint.MoveToWorld(point, map);
                    }

                    //West Fort Inner Splitters
                    List<Point3D> m_WestFortInnerSplitters = new List<Point3D>();

                    m_WestFortInnerSplitters.Add(new Point3D(WestFortLocation.X, WestFortLocation.Y, WestFortLocation.Z));

                    foreach (Point3D point in m_WestFortInnerSplitters)
                    {
                        waypoint = new UOACZWayPoint();
                        waypoint.SplitGroup = 2;
                        waypoint.AllowJumpTo = false;
                        waypoint.NextWaypoint = westBridgeWaypoint;
                        waypoint.MoveToWorld(point, map);
                    }

                    //East Town Fort Location
                    Point3D EastFortLocation = new Point3D(2125, 1391, -90);

                    UOACZMultis.BuildTower(new Point3D(2118, 1398, -90), map, Direction.East);
                    UOACZMultis.BuildTower(new Point3D(2130, 1398, -90), map, Direction.West);
                    UOACZMultis.BuildTower(new Point3D(2130, 1385, -90), map, Direction.West);

                    UOACZMultis.BuildGate(new Point3D(2123, 1399, -90), map, Direction.East);
                    UOACZMultis.BuildGate(new Point3D(2131, 1393, -90), map, Direction.North);
                    UOACZMultis.BuildGate(new Point3D(2124, 1383, -90), map, Direction.East);

                    //West
                    constructionTile = new UOACZConstructionTile();
                    constructionTile.ObjectFacing = Direction.South;
                    constructionTile.MoveToWorld(new Point3D(2117, 1397, -45), map);

                    constructionTileTargeter = new UOACZConstructionObjectEffectTargeter();
                    constructionTileTargeter.ConstructionTile = constructionTile;
                    constructionTileTargeter.MoveToWorld(new Point3D(2116, 1399, -90), map);

                    constructionTileTargeter = new UOACZConstructionObjectEffectTargeter();
                    constructionTileTargeter.ConstructionTile = constructionTile;
                    constructionTileTargeter.MoveToWorld(new Point3D(2118, 1400, -90), map);

                    //South
                    constructionTile = new UOACZConstructionTile();
                    constructionTile.ObjectFacing = Direction.East;
                    constructionTile.MoveToWorld(new Point3D(2129, 1397, -45), map);

                    constructionTileTargeter = new UOACZConstructionObjectEffectTargeter();
                    constructionTileTargeter.ConstructionTile = constructionTile;
                    constructionTileTargeter.MoveToWorld(new Point3D(2129, 1400, -90), map);

                    constructionTileTargeter = new UOACZConstructionObjectEffectTargeter();
                    constructionTileTargeter.ConstructionTile = constructionTile;
                    constructionTileTargeter.MoveToWorld(new Point3D(2132, 1397, -90), map);

                    //North
                    constructionTile = new UOACZConstructionTile();
                    constructionTile.ObjectFacing = Direction.North;
                    constructionTile.MoveToWorld(new Point3D(2129, 1385, -45), map);

                    constructionTileTargeter = new UOACZConstructionObjectEffectTargeter();
                    constructionTileTargeter.ConstructionTile = constructionTile;
                    constructionTileTargeter.MoveToWorld(new Point3D(2132, 1384, -90), map);

                    constructionTileTargeter = new UOACZConstructionObjectEffectTargeter();
                    constructionTileTargeter.ConstructionTile = constructionTile;
                    constructionTileTargeter.MoveToWorld(new Point3D(2129, 1381, -90), map);

                    new UOACZEastWestWallUnbreakable().MoveToWorld(new Point3D(2116, 1398, -90), map);

                    new UOACZEastWestWallUnbreakable().MoveToWorld(new Point3D(2128, 1383, -90), map);
                    new UOACZEastWestWallUnbreakable().MoveToWorld(new Point3D(2129, 1383, -90), map);
                    new UOACZEastWestWallUnbreakable().MoveToWorld(new Point3D(2130, 1383, -90), map);

                    new UOACZEastWestWallUnbreakable().MoveToWorld(new Point3D(2123, 1383, -90), map);

                    new UOACZNorthSouthWall().MoveToWorld(new Point3D(2117, 1399, -90), map);

                    new UOACZEastWestWall().MoveToWorld(new Point3D(2118, 1399, -90), map);
                    new UOACZEastWestWall().MoveToWorld(new Point3D(2119, 1399, -90), map);
                    new UOACZEastWestWallTurret().MoveToWorld(new Point3D(2120, 1399, -90), map);
                    new UOACZEastWestWall().MoveToWorld(new Point3D(2121, 1399, -90), map);
                    new UOACZEastWestWall().MoveToWorld(new Point3D(2122, 1399, -90), map);

                    new UOACZEastWestWall().MoveToWorld(new Point3D(2127, 1399, -90), map);
                    new UOACZEastWestWallTurret().MoveToWorld(new Point3D(2128, 1399, -90), map);
                    new UOACZEastWestWall().MoveToWorld(new Point3D(2129, 1399, -90), map);
                    new UOACZEastWestWall().MoveToWorld(new Point3D(2130, 1399, -90), map);
                    new UOACZEastWestWall().MoveToWorld(new Point3D(2131, 1399, -90), map);

                    new UOACZNorthSouthWall().MoveToWorld(new Point3D(2131, 1399, -90), map);
                    new UOACZNorthSouthWall().MoveToWorld(new Point3D(2131, 1398, -90), map);
                    new UOACZNorthSouthWall().MoveToWorld(new Point3D(2131, 1397, -90), map);
                    new UOACZNorthSouthWall().MoveToWorld(new Point3D(2131, 1396, -90), map);
                    new UOACZNorthSouthWallTurret().MoveToWorld(new Point3D(2131, 1395, -90), map);
                    new UOACZNorthSouthWall().MoveToWorld(new Point3D(2131, 1394, -90), map);

                    new UOACZNorthSouthWall().MoveToWorld(new Point3D(2131, 1389, -90), map);
                    new UOACZNorthSouthWall().MoveToWorld(new Point3D(2131, 1388, -90), map);
                    new UOACZNorthSouthWall().MoveToWorld(new Point3D(2131, 1389, -90), map);
                    new UOACZNorthSouthWallTurret().MoveToWorld(new Point3D(2131, 1387, -90), map);
                    new UOACZNorthSouthWall().MoveToWorld(new Point3D(2131, 1386, -90), map);
                    new UOACZNorthSouthWall().MoveToWorld(new Point3D(2131, 1385, -90), map);
                    new UOACZNorthSouthWall().MoveToWorld(new Point3D(2131, 1384, -90), map);

                    new UOACZEastWestWall().MoveToWorld(new Point3D(2131, 1383, -90), map);

                    m_EliteSpawners.Add(new Point3D(2122, 1407, -90));
                    m_EliteSpawners.Add(new Point3D(2139, 1390, -90));
                    m_EliteSpawners.Add(new Point3D(2131, 1375, -90));

                    m_EliteSpawners.Add(new Point3D(2125, 1390, -90));

                    m_SentrySpawners.Add(new Point3D(2117, 1398, -45));
                    m_SentrySpawners.Add(new Point3D(2118, 1398, -45));
                    //m_SentrySpawners.Add(new Point3D(2118, 1397, -48));

                    m_SentrySpawners.Add(new Point3D(2129, 1398, -45));
                    m_SentrySpawners.Add(new Point3D(2130, 1397, -48));
                    //m_SentrySpawners.Add(new Point3D(2130, 1398, -45));

                    m_SentrySpawners.Add(new Point3D(2130, 1385, -45));
                    m_SentrySpawners.Add(new Point3D(2129, 1384, -45));
                    //m_SentrySpawners.Add(new Point3D(2130, 1384, -48));

                    m_MilitiaSpawners.Add(new Point3D(2004, 1303, -90));
                    m_MilitiaSpawners.Add(new Point3D(1989, 1366, -80));
                    m_MilitiaSpawners.Add(new Point3D(2091, 1375, -85));
                    m_MilitiaSpawners.Add(new Point3D(2058, 1303, -90));

                    foreach (Point3D point in m_EliteSpawners)
                    {
                        militiaSpawner = new UOACZMilitiaSpawner();
                        militiaSpawner.MilitiaType = UOACZMilitiaSpawner.SpawnType.RandomElite;
                        militiaSpawner.MaxSpawnCount = 4;
                        militiaSpawner.SpawnRange = 5;
                        militiaSpawner.HomeRange = 5;
                        militiaSpawner.MoveToWorld(point, map);
                    }

                    foreach (Point3D point in m_SentrySpawners)
                    {
                        militiaSpawner = new UOACZMilitiaSpawner();
                        militiaSpawner.MilitiaType = UOACZMilitiaSpawner.SpawnType.MilitiaArcherSentry;
                        militiaSpawner.SpawnRange = 0;
                        militiaSpawner.MoveToWorld(point, map);
                    }

                    foreach (Point3D point in m_MilitiaSpawners)
                    {
                        militiaSpawner = new UOACZMilitiaSpawner();
                        militiaSpawner.MilitiaType = UOACZMilitiaSpawner.SpawnType.RandomMilitia;
                        militiaSpawner.MaxSpawnCount = 4;
                        militiaSpawner.SpawnRange = 5;
                        militiaSpawner.HomeRange = 8;
                        militiaSpawner.MoveToWorld(point, map);
                    }

                    //East Bridge Waypoint
                    UOACZWayPoint eastBridgeWaypoint = new UOACZWayPoint();
                    eastBridgeWaypoint.GotoSplitGroup = 10;
                    eastBridgeWaypoint.WaypointType = UOACZWayPoint.UOACZWaypointType.UndeadTown;
                    eastBridgeWaypoint.MoveToWorld(new Point3D(2113, 1391, -80), map);

                    //East Fort Approach Splitters
                    List<Point3D> m_EastFortApproachSplitters = new List<Point3D>();

                    m_EastFortApproachSplitters.Add(new Point3D(2124, 1430, -90));
                    m_EastFortApproachSplitters.Add(new Point3D(2178, 1402, -90));
                    m_EastFortApproachSplitters.Add(new Point3D(2172, 1375, -90));
                    m_EastFortApproachSplitters.Add(new Point3D(2154, 1339, -90));

                    foreach (Point3D point in m_EastFortApproachSplitters)
                    {
                        waypoint = new UOACZWayPoint();
                        waypoint.SplitGroup = 3;
                        waypoint.GotoSplitGroup = 4;
                        waypoint.MoveToWorld(point, map);
                    }

                    //East Fort Inner Splitters
                    List<Point3D> m_EastFortInnerSplitters = new List<Point3D>();

                    m_EastFortInnerSplitters.Add(new Point3D(EastFortLocation.X, EastFortLocation.Y, EastFortLocation.Z));

                    foreach (Point3D point in m_EastFortInnerSplitters)
                    {
                        waypoint = new UOACZWayPoint();
                        waypoint.SplitGroup = 4;
                        waypoint.AllowJumpTo = false;
                        waypoint.NextWaypoint = eastBridgeWaypoint;
                        waypoint.MoveToWorld(point, map);
                    }

                    #endregion

                    #region Human Destinations

                    UOACZDestination destination = new UOACZDestination();
                    destination.GateDirection = UOACZDestination.DirectionType.Entrance;
                    destination.ProfileTypeAllowed = UOACZDestination.ProfileType.Human;
                    destination.MoveToWorld(new Point3D(1970, 1324, -90), map);

                    destination = new UOACZDestination();
                    destination.GateDirection = UOACZDestination.DirectionType.Entrance;
                    destination.ProfileTypeAllowed = UOACZDestination.ProfileType.Human;
                    destination.MoveToWorld(new Point3D(1981, 1290, -90), map);

                    destination = new UOACZDestination();
                    destination.GateDirection = UOACZDestination.DirectionType.Entrance;
                    destination.ProfileTypeAllowed = UOACZDestination.ProfileType.Human;
                    destination.MoveToWorld(new Point3D(2027, 1298, -56), map);

                    destination = new UOACZDestination();
                    destination.GateDirection = UOACZDestination.DirectionType.Entrance;
                    destination.ProfileTypeAllowed = UOACZDestination.ProfileType.Human;
                    destination.MoveToWorld(new Point3D(2065, 1302, -90), map);

                    destination = new UOACZDestination();
                    destination.GateDirection = UOACZDestination.DirectionType.Entrance;
                    destination.ProfileTypeAllowed = UOACZDestination.ProfileType.Human;
                    destination.MoveToWorld(new Point3D(2058, 1325, -90), map);

                    destination = new UOACZDestination();
                    destination.GateDirection = UOACZDestination.DirectionType.Entrance;
                    destination.ProfileTypeAllowed = UOACZDestination.ProfileType.Human;
                    destination.MoveToWorld(new Point3D(2091, 1341, -89), map);

                    destination = new UOACZDestination();
                    destination.GateDirection = UOACZDestination.DirectionType.Entrance;
                    destination.ProfileTypeAllowed = UOACZDestination.ProfileType.Human;
                    destination.MoveToWorld(new Point3D(2101, 1372, -90), map);

                    destination = new UOACZDestination();
                    destination.GateDirection = UOACZDestination.DirectionType.Entrance;
                    destination.ProfileTypeAllowed = UOACZDestination.ProfileType.Human;
                    destination.MoveToWorld(new Point3D(2062, 1398, -90), map);

                    destination = new UOACZDestination();
                    destination.GateDirection = UOACZDestination.DirectionType.Entrance;
                    destination.ProfileTypeAllowed = UOACZDestination.ProfileType.Human;
                    destination.MoveToWorld(new Point3D(2060, 1363, -84), map);

                    destination = new UOACZDestination();
                    destination.GateDirection = UOACZDestination.DirectionType.Entrance;
                    destination.ProfileTypeAllowed = UOACZDestination.ProfileType.Human;
                    destination.MoveToWorld(new Point3D(2020, 1366, -90), map);

                    destination = new UOACZDestination();
                    destination.GateDirection = UOACZDestination.DirectionType.Entrance;
                    destination.ProfileTypeAllowed = UOACZDestination.ProfileType.Human;
                    destination.MoveToWorld(new Point3D(1988, 1400, -90), map);

                    destination = new UOACZDestination();
                    destination.GateDirection = UOACZDestination.DirectionType.Entrance;
                    destination.ProfileTypeAllowed = UOACZDestination.ProfileType.Human;
                    destination.MoveToWorld(new Point3D(1965, 1368, -90), map);

                    destination = new UOACZDestination();
                    destination.GateDirection = UOACZDestination.DirectionType.Entrance;
                    destination.ProfileTypeAllowed = UOACZDestination.ProfileType.Human;
                    destination.MoveToWorld(new Point3D(1996, 1353, -90), map);

                    destination = new UOACZDestination();
                    destination.GateDirection = UOACZDestination.DirectionType.Entrance;
                    destination.ProfileTypeAllowed = UOACZDestination.ProfileType.Human;
                    destination.MoveToWorld(new Point3D(2026, 1325, -83), map);

                    destination = new UOACZDestination();
                    destination.GateDirection = UOACZDestination.DirectionType.Entrance;
                    destination.ProfileTypeAllowed = UOACZDestination.ProfileType.Human;
                    destination.MoveToWorld(new Point3D(2070, 1272, -90), map);

                    #endregion

                    #region Undead Destinations

                    List<Point3D> m_UndeadDestinations = new List<Point3D>();

                    m_UndeadDestinations.Add(new Point3D(1848, 1323, -86));
                    m_UndeadDestinations.Add(new Point3D(2023, 1230, -90));
                    m_UndeadDestinations.Add(new Point3D(2166, 1412, -90));
                    m_UndeadDestinations.Add(new Point3D(2351, 1281, -90));
                    m_UndeadDestinations.Add(new Point3D(2359, 1159, -89));
                    m_UndeadDestinations.Add(new Point3D(2240, 1400, -88));
                    m_UndeadDestinations.Add(new Point3D(2244, 1293, -84));
                    m_UndeadDestinations.Add(new Point3D(1929, 1459, -90));
                    m_UndeadDestinations.Add(new Point3D(1697, 1282, -90));
                    m_UndeadDestinations.Add(new Point3D(1668, 1510, -110));

                    foreach (Point3D point in m_UndeadDestinations)
                    {
                        destination = new UOACZDestination();
                        destination.GateDirection = UOACZDestination.DirectionType.Entrance;
                        destination.ProfileTypeAllowed = UOACZDestination.ProfileType.Undead;
                        destination.MoveToWorld(point, map);
                    }

                    #endregion

                    #region Tunnels

                    //Town Tunnels
                    List<Point3D> m_TownTunnels = new List<Point3D>();

                    m_TownTunnels.Add(new Point3D(2018, 1312, -90));
                    m_TownTunnels.Add(new Point3D(2053, 1364, -90));
                    m_TownTunnels.Add(new Point3D(2001, 1328, -90));
                    m_TownTunnels.Add(new Point3D(1973, 1293, -90));
                    m_TownTunnels.Add(new Point3D(2017, 1361, -90));
                    m_TownTunnels.Add(new Point3D(2096, 1336, -90));
                    m_TownTunnels.Add(new Point3D(2006, 1298, -90));
                    m_TownTunnels.Add(new Point3D(2074, 1287, -80));
                    m_TownTunnels.Add(new Point3D(2060, 1395, -90));
                    m_TownTunnels.Add(new Point3D(1998, 1391, -90));
                    m_TownTunnels.Add(new Point3D(2048, 1284, -90));
                    m_TownTunnels.Add(new Point3D(2098, 1399, -90));
                    m_TownTunnels.Add(new Point3D(2033, 1410, -90));
                    m_TownTunnels.Add(new Point3D(1959, 1326, -90));
                    m_TownTunnels.Add(new Point3D(2099, 1366, -90));
                    m_TownTunnels.Add(new Point3D(1973, 1353, -90));
                    m_TownTunnels.Add(new Point3D(1961, 1384, -90));

                    foreach (Point3D point in m_TownTunnels)
                    {
                        UOACZTunnel tunnel = new UOACZTunnel();
                        tunnel.TunnelType = UOACZTunnel.TunnelLocation.Town;
                        tunnel.MoveToWorld(point, map);
                    }

                    //Wilderness Tunnels
                    List<Point3D> m_WildernessTunnels = new List<Point3D>();

                    m_WildernessTunnels.Add(new Point3D(2388, 1256, -90));
                    m_WildernessTunnels.Add(new Point3D(1703, 1210, -90));
                    m_WildernessTunnels.Add(new Point3D(1944, 1274, -90));
                    m_WildernessTunnels.Add(new Point3D(2364, 1403, -90));
                    m_WildernessTunnels.Add(new Point3D(1835, 1148, -90));
                    m_WildernessTunnels.Add(new Point3D(1822, 1111, -87));
                    m_WildernessTunnels.Add(new Point3D(1640, 1250, -90));
                    m_WildernessTunnels.Add(new Point3D(1624, 1546, -110));
                    m_WildernessTunnels.Add(new Point3D(1742, 1551, -110));
                    m_WildernessTunnels.Add(new Point3D(1931, 1344, -90));
                    m_WildernessTunnels.Add(new Point3D(1897, 1401, -90));
                    m_WildernessTunnels.Add(new Point3D(1981, 1493, -90));
                    m_WildernessTunnels.Add(new Point3D(2014, 1553, -90));
                    m_WildernessTunnels.Add(new Point3D(1972, 1585, -90));
                    m_WildernessTunnels.Add(new Point3D(1820, 1585, -110));
                    m_WildernessTunnels.Add(new Point3D(1666, 1508, -110));
                    m_WildernessTunnels.Add(new Point3D(1778, 1293, -90));
                    m_WildernessTunnels.Add(new Point3D(1624, 1342, -90));

                    m_WildernessTunnels.Add(new Point3D(1690, 1392, -110));
                    m_WildernessTunnels.Add(new Point3D(1846, 1418, -90));
                    m_WildernessTunnels.Add(new Point3D(1748, 1337, -90));
                    m_WildernessTunnels.Add(new Point3D(1645, 1370, -95));
                    m_WildernessTunnels.Add(new Point3D(1805, 1356, -90));
                    m_WildernessTunnels.Add(new Point3D(1892, 1127, -90));
                    m_WildernessTunnels.Add(new Point3D(2087, 1106, -84));
                    m_WildernessTunnels.Add(new Point3D(2233, 1291, -84));
                    m_WildernessTunnels.Add(new Point3D(2146, 1306, -90));
                    m_WildernessTunnels.Add(new Point3D(2178, 1108, -85));
                    m_WildernessTunnels.Add(new Point3D(2251, 1388, -90));
                    m_WildernessTunnels.Add(new Point3D(2150, 1341, -90));
                    m_WildernessTunnels.Add(new Point3D(2169, 1408, -90));
                    m_WildernessTunnels.Add(new Point3D(2224, 1286, -89));
                    m_WildernessTunnels.Add(new Point3D(2298, 1402, -87));
                    m_WildernessTunnels.Add(new Point3D(2289, 1334, -90));
                    m_WildernessTunnels.Add(new Point3D(2436, 1307, -95));
                    m_WildernessTunnels.Add(new Point3D(2358, 1163, -88));

                    foreach (Point3D point in m_WildernessTunnels)
                    {
                        UOACZTunnel tunnel = new UOACZTunnel();
                        tunnel.TunnelType = UOACZTunnel.TunnelLocation.Wilderness;
                        tunnel.MoveToWorld(point, map);
                    }

                    #endregion

                    #region Fishing Spawners

                    List<Point3D> m_FishingSpawnerLocations = new List<Point3D>();

                    m_FishingSpawnerLocations.Add(new Point3D(1845, 1645, -115));
                    m_FishingSpawnerLocations.Add(new Point3D(1832, 1616, -115));
                    m_FishingSpawnerLocations.Add(new Point3D(1809, 1583, -115));
                    m_FishingSpawnerLocations.Add(new Point3D(1771, 1563, -115));
                    m_FishingSpawnerLocations.Add(new Point3D(1752, 1559, -115));
                    m_FishingSpawnerLocations.Add(new Point3D(1722, 1553, -115));
                    m_FishingSpawnerLocations.Add(new Point3D(1697, 1554, -115));
                    m_FishingSpawnerLocations.Add(new Point3D(1661, 1557, -115));
                    m_FishingSpawnerLocations.Add(new Point3D(1627, 1552, -115));
                    m_FishingSpawnerLocations.Add(new Point3D(1599, 1544, -115));
                    m_FishingSpawnerLocations.Add(new Point3D(1577, 1537, -115));
                    m_FishingSpawnerLocations.Add(new Point3D(1545, 1534, -115));
                    m_FishingSpawnerLocations.Add(new Point3D(1513, 1545, -115));
                    m_FishingSpawnerLocations.Add(new Point3D(1483, 1569, -115));
                    m_FishingSpawnerLocations.Add(new Point3D(1433, 1551, -110));
                    m_FishingSpawnerLocations.Add(new Point3D(1431, 1553, -110));
                    m_FishingSpawnerLocations.Add(new Point3D(1430, 1550, -110));
                    m_FishingSpawnerLocations.Add(new Point3D(1399, 1431, -90));
                    m_FishingSpawnerLocations.Add(new Point3D(1399, 1428, -90));
                    m_FishingSpawnerLocations.Add(new Point3D(1635, 1248, -90));
                    m_FishingSpawnerLocations.Add(new Point3D(1635, 1245, -90));
                    m_FishingSpawnerLocations.Add(new Point3D(1637, 1246, -90));
                    m_FishingSpawnerLocations.Add(new Point3D(1822, 1122, -90));
                    m_FishingSpawnerLocations.Add(new Point3D(1822, 1119, -90));
                    m_FishingSpawnerLocations.Add(new Point3D(1824, 1120, -90));

                    foreach (Point3D point in m_FishingSpawnerLocations)
                    {
                        UOACZScavengeFishingSpawner fishingSpawner = new UOACZScavengeFishingSpawner();

                        fishingSpawner.MoveToWorld(point, map);
                    }

                    #endregion

                    #region Ore Spawners

                    UOACZScavengeOreSpawner oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(1979, 1611, -85), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(1976, 1587, -85), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(1972, 1566, -87), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(1977, 1543, -90), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(1992, 1549, -90), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2006, 1559, -87), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2023, 1548, -90), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2021, 1533, -90), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2015, 1514, -89), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2010, 1489, -88), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2030, 1479, -89), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2060, 1467, -87), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2077, 1451, -86), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2105, 1445, -88), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2125, 1463, -86), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2148, 1468, -90), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2171, 1444, -90), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2193, 1424, -90), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2208, 1431, -88), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2235, 1441, -89), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2255, 1437, -90), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2288, 14314257, -89), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2301, 1433, -90), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2326, 1451, -90), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2362, 1447, -96), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2085, 1274, -80), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2082, 1303, -90), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2083, 1287, -82), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2120, 1320, -90), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2140, 1306, -87), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2163, 1313, -86), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2185, 1322, -85), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2206, 1313, -84), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2220, 1292, -86), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2212, 1264, -85), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2189, 1242, -87), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2160, 1233, -84), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2129, 1235, -82), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2103, 1255, -75), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2437, 1306, -95), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2425, 1321, -90), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2401, 1319, -90), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2377, 1310, -89), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2377, 1288, -89), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2391, 1269, -90), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2407, 1267, -90), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(2424, 1277, -95), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(1701, 1206, -90), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(1704, 1206, -89), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(1703, 1208, -90), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(1665, 1297, -87), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(1668, 1296, -88), map);

                    oreSpawner = new UOACZScavengeOreSpawner();
                    oreSpawner.MoveToWorld(new Point3D(1667, 1299, -88), map);

                    //--------            

                    #endregion

                    #region Town Spawners

                    UOACZCivilianSpawner civilianSpawner = new UOACZCivilianSpawner();
                    civilianSpawner.MaxSpawnCount = 25;
                    civilianSpawner.SpawnRange = 70;
                    civilianSpawner.MoveToWorld(new Point3D(2031, 1342, -88), map);

                    militiaSpawner = new UOACZMilitiaSpawner();
                    militiaSpawner.MaxSpawnCount = 25;
                    militiaSpawner.SpawnRange = 70;
                    militiaSpawner.MoveToWorld(new Point3D(2031, 1343, -88), map);

                    UOACZUndeadSpawner undeadSpawner;

                    /*
                    UOACZUndeadSpawner undeadSpawner = new UOACZUndeadSpawner();
                    undeadSpawner.MaxSpawnCount = 40;
                    undeadSpawner.SpawnRange = 70;
                    undeadSpawner.Wilderness = false;
                    undeadSpawner.GotoSplitGroup = 10;
                    undeadSpawner.MoveToWorld(new Point3D(2031, 1344, -88), map);
                    */

                    UOACZWaterSpawner waterSpawner = new UOACZWaterSpawner();
                    waterSpawner.MaxSpawnCount = 20;
                    waterSpawner.SpawnRange = 70;
                    waterSpawner.MoveToWorld(new Point3D(2031, 1345, -88), map);

                    UOACZCityScavengeSpawner cityScavengeSpawner = new UOACZCityScavengeSpawner();
                    cityScavengeSpawner.MaxSpawnCount = 40;
                    cityScavengeSpawner.SpawnRange = 70;
                    cityScavengeSpawner.MoveToWorld(new Point3D(2031, 1346, -88), map);

                    civilianSpawner = new UOACZCivilianSpawner();
                    civilianSpawner.CivilianType = UOACZBaseCivilian.CivilianType.Blacksmith;
                    civilianSpawner.MoveToWorld(new Point3D(1986, 1357, -80), map);

                    civilianSpawner = new UOACZCivilianSpawner();
                    civilianSpawner.CivilianType = UOACZBaseCivilian.CivilianType.Tinker;
                    civilianSpawner.MoveToWorld(new Point3D(1989, 1361, -80), map);

                    civilianSpawner = new UOACZCivilianSpawner();
                    civilianSpawner.CivilianType = UOACZBaseCivilian.CivilianType.Miner;
                    civilianSpawner.MoveToWorld(new Point3D(1983, 1364, -80), map);

                    civilianSpawner = new UOACZCivilianSpawner();
                    civilianSpawner.CivilianType = UOACZBaseCivilian.CivilianType.Healer;
                    civilianSpawner.MoveToWorld(new Point3D(2070, 1374, -75), map);

                    civilianSpawner = new UOACZCivilianSpawner();
                    civilianSpawner.CivilianType = UOACZBaseCivilian.CivilianType.Alchemist;
                    civilianSpawner.MoveToWorld(new Point3D(2070, 1373, -75), map);

                    civilianSpawner = new UOACZCivilianSpawner();
                    civilianSpawner.CivilianType = UOACZBaseCivilian.CivilianType.Weaver;
                    civilianSpawner.MoveToWorld(new Point3D(2078, 1326, -80), map);

                    civilianSpawner = new UOACZCivilianSpawner();
                    civilianSpawner.CivilianType = UOACZBaseCivilian.CivilianType.Skinner;
                    civilianSpawner.MoveToWorld(new Point3D(2084, 1322, -80), map);

                    civilianSpawner = new UOACZCivilianSpawner();
                    civilianSpawner.CivilianType = UOACZBaseCivilian.CivilianType.Carpenter;
                    civilianSpawner.MoveToWorld(new Point3D(2063, 1283, -80), map);

                    civilianSpawner = new UOACZCivilianSpawner();
                    civilianSpawner.CivilianType = UOACZBaseCivilian.CivilianType.Bowyer;
                    civilianSpawner.MoveToWorld(new Point3D(2065, 1283, -80), map);

                    civilianSpawner = new UOACZCivilianSpawner();
                    civilianSpawner.CivilianType = UOACZBaseCivilian.CivilianType.Butcher;
                    civilianSpawner.MoveToWorld(new Point3D(1994, 1315, -90), map);

                    civilianSpawner = new UOACZCivilianSpawner();
                    civilianSpawner.CivilianType = UOACZBaseCivilian.CivilianType.Farmer;
                    civilianSpawner.MoveToWorld(new Point3D(1996, 1315, -90), map);

                    civilianSpawner = new UOACZCivilianSpawner();
                    civilianSpawner.CivilianType = UOACZBaseCivilian.CivilianType.Brewmaster;
                    civilianSpawner.MoveToWorld(new Point3D(2014, 1327, -80), map);

                    civilianSpawner = new UOACZCivilianSpawner();
                    civilianSpawner.CivilianType = UOACZBaseCivilian.CivilianType.Provisioner;
                    civilianSpawner.MoveToWorld(new Point3D(2016, 1324, -80), map);

                    civilianSpawner = new UOACZCivilianSpawner();
                    civilianSpawner.CivilianType = UOACZBaseCivilian.CivilianType.Baker;
                    civilianSpawner.MoveToWorld(new Point3D(2017, 1363, -90), map);

                    civilianSpawner = new UOACZCivilianSpawner();
                    civilianSpawner.CivilianType = UOACZBaseCivilian.CivilianType.Noble;
                    civilianSpawner.MoveToWorld(new Point3D(2039, 1318, -65), map);

                    civilianSpawner = new UOACZCivilianSpawner();
                    civilianSpawner.CivilianType = UOACZBaseCivilian.CivilianType.Merchant;
                    civilianSpawner.MoveToWorld(new Point3D(2049, 1344, -85), map);

                    civilianSpawner = new UOACZCivilianSpawner();
                    civilianSpawner.CivilianType = UOACZBaseCivilian.CivilianType.Fisherman;
                    civilianSpawner.MoveToWorld(new Point3D(2016, 1354, -90), map);

                    civilianSpawner = new UOACZCivilianSpawner();
                    civilianSpawner.CivilianType = UOACZBaseCivilian.CivilianType.Any;
                    civilianSpawner.MoveToWorld(new Point3D(2023, 1377, -80), map);

                    civilianSpawner = new UOACZCivilianSpawner();
                    civilianSpawner.CivilianType = UOACZBaseCivilian.CivilianType.Any;
                    civilianSpawner.MoveToWorld(new Point3D(2024, 1383, -80), map);

                    civilianSpawner = new UOACZCivilianSpawner();
                    civilianSpawner.CivilianType = UOACZBaseCivilian.CivilianType.Any;
                    civilianSpawner.MoveToWorld(new Point3D(2025, 1389, -80), map);

                    #endregion

                    #region Wilderness Spawners and Waypoints

                    List<Point3D> m_GroupPoints = new List<Point3D>();

                    m_GroupPoints.Add(new Point3D(1963, 1494, -90));
                    m_GroupPoints.Add(new Point3D(1892, 1571, -110));
                    m_GroupPoints.Add(new Point3D(1764, 1462, -110));
                    m_GroupPoints.Add(new Point3D(1627, 1441, -110));
                    m_GroupPoints.Add(new Point3D(1554, 1316, -84));
                    m_GroupPoints.Add(new Point3D(1688, 1297, -84));
                    m_GroupPoints.Add(new Point3D(1824, 1328, -90));
                    m_GroupPoints.Add(new Point3D(1900, 1194, -84));
                    m_GroupPoints.Add(new Point3D(1808, 1152, -90));
                    m_GroupPoints.Add(new Point3D(2006, 1184, -85));
                    m_GroupPoints.Add(new Point3D(2160, 1166, -84));
                    m_GroupPoints.Add(new Point3D(2264, 1192, -86));
                    m_GroupPoints.Add(new Point3D(2403, 1187, -89));
                    m_GroupPoints.Add(new Point3D(2301, 1283, -90));
                    m_GroupPoints.Add(new Point3D(2210, 1391, -88));
                    m_GroupPoints.Add(new Point3D(2337, 1402, -87));

                    foreach (Point3D point in m_GroupPoints)
                    {
                        waterSpawner = new UOACZWaterSpawner();
                        waterSpawner.MoveToWorld(new Point3D(point.X - 1, point.Y, point.Z), map);

                        UOACZScavengeVegetationSpawner vegetationSpawner = new UOACZScavengeVegetationSpawner();
                        vegetationSpawner.MoveToWorld(new Point3D(point.X, point.Y, point.Z), map);

                        UOACZScavengeCottonSpawner cottonSpawner = new UOACZScavengeCottonSpawner();
                        cottonSpawner.MoveToWorld(new Point3D(point.X + 1, point.Y, point.Z), map);

                        UOACZWildlifeSpawner wildlifeSpawner = new UOACZWildlifeSpawner();
                        wildlifeSpawner.MoveToWorld(new Point3D(point.X, point.Y - 1, point.Z), map);

                        UOACZReagentSpawner reagentSpawner = new UOACZReagentSpawner();
                        reagentSpawner.MoveToWorld(new Point3D(point.X + 1, point.Y - 1, point.Z), map);

                        civilianSpawner = new UOACZCivilianSpawner();
                        civilianSpawner.SpawnRange = 70;
                        civilianSpawner.Wilderness = true;
                        civilianSpawner.MinSpawnTime = 20;
                        civilianSpawner.MaxSpawnTime = 60;
                        civilianSpawner.MaxSpawnCount = 1;
                        civilianSpawner.MaxTotalSpawns = 5;
                        civilianSpawner.CivilianType = UOACZBaseCivilian.CivilianType.Any;
                        civilianSpawner.MoveToWorld(new Point3D(point.X - 1, point.Y + 1, point.Z), map);

                        militiaSpawner = new UOACZMilitiaSpawner();
                        militiaSpawner.SpawnRange = 70;
                        militiaSpawner.Wilderness = true;
                        militiaSpawner.MinSpawnTime = 20;
                        militiaSpawner.MaxSpawnTime = 60;
                        militiaSpawner.MaxSpawnCount = 1;
                        militiaSpawner.MaxTotalSpawns = 5;
                        militiaSpawner.MilitiaType = UOACZMilitiaSpawner.SpawnType.RandomWilderness;
                        militiaSpawner.MoveToWorld(new Point3D(point.X + 1, point.Y + 1, point.Z), map);

                        undeadSpawner = new UOACZUndeadSpawner();

                        int gotoSplitGroup = -1;

                        int distanceToWestFort = Utility.GetDistance(point, WestFortLocation);
                        int distanceToEastFort = Utility.GetDistance(point, EastFortLocation);

                        if (distanceToWestFort < distanceToEastFort)
                            undeadSpawner.GotoSplitGroup = 1;

                        else
                            undeadSpawner.GotoSplitGroup = 3;

                        undeadSpawner.MoveToWorld(new Point3D(point.X, point.Y + 1, point.Z), map);
                    }

                    #endregion

                    #region Undead Fort Proximity Spawners

                    //West Fort
                    undeadSpawner = new UOACZUndeadSpawner();
                    undeadSpawner.GotoSplitGroup = 1;
                    undeadSpawner.SpawnRange = 30;
                    undeadSpawner.MinSpawnTime = 1;
                    undeadSpawner.MaxSpawnTime = 2;
                    undeadSpawner.MoveToWorld(new Point3D(1868, 1404, -90), map);

                    //East Fort
                    undeadSpawner = new UOACZUndeadSpawner();
                    undeadSpawner.GotoSplitGroup = 3;
                    undeadSpawner.SpawnRange = 20;
                    undeadSpawner.MinSpawnTime = 1;
                    undeadSpawner.MaxSpawnTime = 2;
                    undeadSpawner.MoveToWorld(new Point3D(2235, 1415, -83), map);

                    undeadSpawner = new UOACZUndeadSpawner();
                    undeadSpawner.GotoSplitGroup = 3;
                    undeadSpawner.SpawnRange = 20;
                    undeadSpawner.MinSpawnTime = 1;
                    undeadSpawner.MaxSpawnTime = 2;
                    undeadSpawner.MoveToWorld(new Point3D(2260, 1288, -87), map);

                    undeadSpawner = new UOACZUndeadSpawner();
                    undeadSpawner.GotoSplitGroup = 3;
                    undeadSpawner.SpawnRange = 20;
                    undeadSpawner.MinSpawnTime = 1;
                    undeadSpawner.MaxSpawnTime = 2;
                    undeadSpawner.MoveToWorld(new Point3D(1988, 1473, -90), map);

                    #endregion

                    #region Undead Town Waypoints

                    List<Point3D> m_UndeadTownWaypoints = new List<Point3D>();

                    m_UndeadTownWaypoints.Add(new Point3D(1982, 1362, -80));
                    m_UndeadTownWaypoints.Add(new Point3D(2050, 1410, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(2037, 1399, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(2115, 1303, -25));
                    m_UndeadTownWaypoints.Add(new Point3D(2113, 1305, -50));
                    m_UndeadTownWaypoints.Add(new Point3D(1955, 1344, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(1990, 1366, -80));
                    m_UndeadTownWaypoints.Add(new Point3D(2079, 1403, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(1987, 1359, -80));
                    m_UndeadTownWaypoints.Add(new Point3D(2079, 1331, -75));
                    m_UndeadTownWaypoints.Add(new Point3D(2093, 1376, -85));
                    m_UndeadTownWaypoints.Add(new Point3D(2097, 1321, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(2039, 1317, -45));
                    m_UndeadTownWaypoints.Add(new Point3D(2031, 1353, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(2106, 1354, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(2082, 1325, -50));
                    m_UndeadTownWaypoints.Add(new Point3D(1961, 1321, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(2023, 1383, -80));
                    m_UndeadTownWaypoints.Add(new Point3D(2015, 1373, -80));
                    m_UndeadTownWaypoints.Add(new Point3D(1948, 1379, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(1993, 1402, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(1995, 1382, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(2077, 1361, -89));
                    m_UndeadTownWaypoints.Add(new Point3D(2015, 1412, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(1970, 1380, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(1977, 1352, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(2101, 1391, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(1950, 1360, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(2018, 1355, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(1973, 1333, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(1963, 1339, -75));
                    m_UndeadTownWaypoints.Add(new Point3D(2038, 1390, -80));
                    m_UndeadTownWaypoints.Add(new Point3D(2036, 1379, -75));
                    m_UndeadTownWaypoints.Add(new Point3D(1969, 1407, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(1963, 1340, -50));
                    m_UndeadTownWaypoints.Add(new Point3D(2053, 1382, -90));

                    m_UndeadTownWaypoints.Add(new Point3D(2049, 1397, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(2071, 1373, -75));
                    m_UndeadTownWaypoints.Add(new Point3D(2013, 1283, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(2056, 1393, -85));
                    m_UndeadTownWaypoints.Add(new Point3D(2053, 1401, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(2034, 1317, -85));
                    m_UndeadTownWaypoints.Add(new Point3D(2047, 1364, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(1994, 1297, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(2066, 1317, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(2022, 1363, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(1992, 1315, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(1992, 1338, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(2083, 1372, -75));
                    m_UndeadTownWaypoints.Add(new Point3D(2018, 1296, -89));
                    m_UndeadTownWaypoints.Add(new Point3D(2017, 1336, -80));
                    m_UndeadTownWaypoints.Add(new Point3D(1980, 1308, -50));
                    m_UndeadTownWaypoints.Add(new Point3D(1991, 1325, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(1979, 1307, -75));

                    m_UndeadTownWaypoints.Add(new Point3D(2003, 1313, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(1969, 1293, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(2048, 1337, -80));
                    m_UndeadTownWaypoints.Add(new Point3D(2054, 1341, -80));
                    m_UndeadTownWaypoints.Add(new Point3D(2079, 1346, -88));
                    m_UndeadTownWaypoints.Add(new Point3D(2079, 1278, -85));
                    m_UndeadTownWaypoints.Add(new Point3D(2041, 1342, -80));
                    m_UndeadTownWaypoints.Add(new Point3D(2046, 1350, -80));
                    m_UndeadTownWaypoints.Add(new Point3D(2062, 1345, -86));
                    m_UndeadTownWaypoints.Add(new Point3D(2089, 1345, -88));
                    m_UndeadTownWaypoints.Add(new Point3D(2091, 1323, -75));
                    m_UndeadTownWaypoints.Add(new Point3D(2106, 1313, -48));
                    m_UndeadTownWaypoints.Add(new Point3D(2048, 1344, -55));
                    m_UndeadTownWaypoints.Add(new Point3D(2040, 1315, -65));
                    m_UndeadTownWaypoints.Add(new Point3D(2035, 1315, -65));
                    m_UndeadTownWaypoints.Add(new Point3D(2061, 1262, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(2038, 1321, -85));
                    m_UndeadTownWaypoints.Add(new Point3D(2041, 1322, -85));

                    m_UndeadTownWaypoints.Add(new Point3D(2047, 1315, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(2068, 1284, -80));
                    m_UndeadTownWaypoints.Add(new Point3D(2080, 1304, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(2061, 1282, -80));
                    m_UndeadTownWaypoints.Add(new Point3D(2066, 1295, -75));
                    m_UndeadTownWaypoints.Add(new Point3D(2036, 1323, -65));
                    m_UndeadTownWaypoints.Add(new Point3D(1967, 1363, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(1968, 1313, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(2026, 1322, -84));
                    m_UndeadTownWaypoints.Add(new Point3D(1986, 1309, -85));
                    m_UndeadTownWaypoints.Add(new Point3D(1988, 1283, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(2046, 1296, -89));
                    m_UndeadTownWaypoints.Add(new Point3D(2048, 1276, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(2033, 1285, -86));
                    m_UndeadTownWaypoints.Add(new Point3D(2023, 1285, -86));
                    m_UndeadTownWaypoints.Add(new Point3D(2023, 1338, -81));
                    m_UndeadTownWaypoints.Add(new Point3D(2029, 1305, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(2029, 1296, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(2038, 1332, -90));

                    m_UndeadTownWaypoints.Add(new Point3D(2087, 1389, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(2058, 1305, -90));
                    m_UndeadTownWaypoints.Add(new Point3D(2004, 1305, -90));

                    foreach (Point3D point in m_UndeadTownWaypoints)
                    {
                        waypoint = new UOACZWayPoint();
                        waypoint.GotoSplitGroup = 10;
                        waypoint.IsBaseNode = true;
                        waypoint.SplitGroup = 10;
                        waypoint.WaypointType = UOACZWayPoint.UOACZWaypointType.UndeadTown;
                        waypoint.MoveToWorld(point, map);
                    }

                    #endregion

                    #region Blockers

                    //Boundaries
                    BuildStaticLine(new Point3D(1974, 1589, -90), new Point3D(1943, 1589, -99), map, Direction.West, 375, 0, "wall");
                    BuildStaticLine(new Point3D(1940, 1589, -102), new Point3D(1819, 1589, -110), map, Direction.West, 375, 0, "wall");

                    BuildLOSBlockerLine(new Point3D(1975, 1589, -89), new Point3D(1980, 1589, -99), map, Direction.East);
                    BuildLOSBlockerLine(new Point3D(1818, 1589, -102), new Point3D(1818, 1579, -102), map, Direction.West);

                    BuildStatic(new Point3D(1622, 1549, -118), map, 375, 0, "wall");
                    BuildStaticLine(new Point3D(1622, 1548, -110), new Point3D(1622, 1432, -110), map, Direction.North, 375, 0, "wall");
                    BuildStaticLine(new Point3D(1622, 1431, -110), new Point3D(1622, 1399, -105), map, Direction.North, 844, 0, "wall");
                    BuildStaticLine(new Point3D(1622, 1398, -103), new Point3D(1622, 1393, -98), map, Direction.North, 844, 0, "wall");
                    BuildStaticLine(new Point3D(1622, 1392, -97), new Point3D(1622, 1385, -93), map, Direction.North, 844, 0, "wall");
                    BuildStaticLine(new Point3D(1622, 1384, -92), new Point3D(1622, 1314, -87), map, Direction.North, 844, 0, "wall");
                    BuildStaticLine(new Point3D(1622, 1313, -87), new Point3D(1622, 1216, -87), map, Direction.North, 844, 0, "wall");
                    BuildStatic(new Point3D(1622, 1215, -91), map, 844, 0, "wall");
                    BuildStatic(new Point3D(1622, 1214, -95), map, 844, 0, "wall");

                    BuildLOSBlockerLine(new Point3D(1622, 1550, -118), new Point3D(1622, 1560, -118), map, Direction.South);
                    BuildLOSBlockerLine(new Point3D(1622, 1214, -95), new Point3D(1622, 1204, -95), map, Direction.North);

                    //Caves
                    BuildStaticLine(new Point3D(2113, 1300, -50), new Point3D(2115, 1300, -50), map, Direction.East, 13883, 2405, "wall");
                    BuildStaticLine(new Point3D(2122, 1462, -89), new Point3D(2122, 1458, -90), map, Direction.North, 13882, 2405, "wall");

                    #endregion

                    #region Human Outpost

                    int palisadeWidth = 20;
                    int cabinWidth = 12;

                    Point3D palisadeLeft = new Point3D(1643, 1251, -90);
                    Point3D cabinLeft = new Point3D(1647, 1247, -90);

                    UOACZPersistance.m_OutpostComponents = new List<UOACZBreakableStatic>();

                    //Palisade Walls           
                    Point3D currentPoint = palisadeLeft;

                    UOACZBreakableStatic outpostComponent;

                    //Left
                    for (int a = 0; a < palisadeWidth; a++)
                    {
                        outpostComponent = new UOACZNorthSouthPalisade();
                        outpostComponent.MoveToWorld(currentPoint, map);
                        UOACZPersistance.m_OutpostComponents.Add(outpostComponent);

                        currentPoint.Y--;
                    }

                    //Top
                    for (int a = 0; a < palisadeWidth; a++)
                    {
                        if (a == 0)
                        {
                            currentPoint.X++;
                            continue;
                        }

                        if (a == 9)
                        {
                            outpostComponent = new UOACZEastWestPalisadeRightDoor();
                            outpostComponent.MoveToWorld(new Point3D(currentPoint.X, currentPoint.Y, currentPoint.Z + 2), map);
                            UOACZPersistance.m_OutpostComponents.Add(outpostComponent);
                        }

                        else if (a == 10)
                        {
                            outpostComponent = new UOACZEastWestPalisadeLeftDoor();
                            outpostComponent.MoveToWorld(new Point3D(currentPoint.X, currentPoint.Y, currentPoint.Z + 2), map);
                            UOACZPersistance.m_OutpostComponents.Add(outpostComponent);
                        }

                        else
                        {
                            outpostComponent = new UOACZEastWestPalisade();
                            outpostComponent.MoveToWorld(currentPoint, map);
                            UOACZPersistance.m_OutpostComponents.Add(outpostComponent);
                        }

                        currentPoint.X++;
                    }

                    //Right
                    for (int a = 0; a < palisadeWidth; a++)
                    {
                        if (a == 0)
                        {
                            outpostComponent = new UOACZEastWestPalisade();
                            outpostComponent.MoveToWorld(currentPoint, map);
                            UOACZPersistance.m_OutpostComponents.Add(outpostComponent);
                        }

                        else
                        {
                            outpostComponent = new UOACZNorthSouthPalisade();
                            outpostComponent.MoveToWorld(currentPoint, map);
                            UOACZPersistance.m_OutpostComponents.Add(outpostComponent);
                        }

                        currentPoint.Y++;
                    }

                    //Bottom
                    for (int a = 0; a < palisadeWidth; a++)
                    {
                        if (a == 0)
                        {
                            outpostComponent = new UOACZNorthSouthPalisade();
                            outpostComponent.MoveToWorld(currentPoint, map);
                            UOACZPersistance.m_OutpostComponents.Add(outpostComponent);
                        }

                        if (a == 9)
                        {
                            outpostComponent = new UOACZEastWestPalisadeRightDoor();
                            outpostComponent.MoveToWorld(new Point3D(currentPoint.X, currentPoint.Y, currentPoint.Z + 2), map);
                            UOACZPersistance.m_OutpostComponents.Add(outpostComponent);
                        }

                        else if (a == 10)
                        {
                            outpostComponent = new UOACZEastWestPalisadeLeftDoor();
                            outpostComponent.MoveToWorld(new Point3D(currentPoint.X, currentPoint.Y, currentPoint.Z + 2), map);
                            UOACZPersistance.m_OutpostComponents.Add(outpostComponent);
                        }

                        else
                        {
                            outpostComponent = new UOACZEastWestPalisade();
                            outpostComponent.MoveToWorld(currentPoint, map);
                            UOACZPersistance.m_OutpostComponents.Add(outpostComponent);
                        }

                        currentPoint.X--;
                    }

                    //Cabin Walls   
                    currentPoint = cabinLeft;

                    for (int a = 0; a < cabinWidth; a++)
                    {
                        outpostComponent = new UOACZNorthSouthLogWall();
                        outpostComponent.MoveToWorld(currentPoint, map);
                        UOACZPersistance.m_OutpostComponents.Add(outpostComponent);

                        currentPoint.Y--;
                    }

                    for (int a = 0; a < cabinWidth; a++)
                    {
                        if (a == 0)
                        {
                            currentPoint.X++;
                            continue;
                        }

                        outpostComponent = new UOACZEastWestLogWall();
                        outpostComponent.MoveToWorld(currentPoint, map);
                        UOACZPersistance.m_OutpostComponents.Add(outpostComponent);

                        currentPoint.X++;
                    }

                    for (int a = 0; a < cabinWidth; a++)
                    {
                        if (a == 0)
                        {
                            outpostComponent = new UOACZEastWestLogWall();
                            outpostComponent.MoveToWorld(currentPoint, map);
                            UOACZPersistance.m_OutpostComponents.Add(outpostComponent);
                        }

                        else
                        {
                            outpostComponent = new UOACZNorthSouthLogWall();
                            outpostComponent.MoveToWorld(currentPoint, map);
                            UOACZPersistance.m_OutpostComponents.Add(outpostComponent);
                        }

                        currentPoint.Y++;
                    }

                    for (int a = 0; a < cabinWidth; a++)
                    {
                        if (a == 0)
                        {
                            outpostComponent = new UOACZDownLogWall();
                            outpostComponent.MoveToWorld(currentPoint, map);
                            UOACZPersistance.m_OutpostComponents.Add(outpostComponent);
                        }

                        else if (a == 5)
                        {
                            outpostComponent = new UOACZEastWestLogRightDoor();
                            outpostComponent.MoveToWorld(new Point3D(currentPoint.X, currentPoint.Y, currentPoint.Z + 2), map);
                            UOACZPersistance.m_OutpostComponents.Add(outpostComponent);
                        }

                        else if (a == 6)
                        {
                            outpostComponent = new UOACZEastWestLogLeftDoor();
                            outpostComponent.MoveToWorld(new Point3D(currentPoint.X, currentPoint.Y, currentPoint.Z + 2), map);
                            UOACZPersistance.m_OutpostComponents.Add(outpostComponent);
                        }

                        else
                        {
                            outpostComponent = new UOACZEastWestLogWall();
                            outpostComponent.MoveToWorld(currentPoint, map);
                            UOACZPersistance.m_OutpostComponents.Add(outpostComponent);
                        }

                        currentPoint.X--;
                    }

                    //Cabin Interiors
                    currentPoint = cabinLeft;
                    currentPoint.Y -= cabinWidth;

                    currentPoint.X++;
                    currentPoint.Y++;

                    for (int a = 0; a < cabinWidth - 1; a++)
                    {
                        for (int b = 0; b < cabinWidth - 1; b++)
                        {
                            BuildStatic(new Point3D(currentPoint.X + a, currentPoint.Y + b, currentPoint.Z), map, 1184, 2415, "floor");
                        }
                    }

                    BuildStatic(new Point3D(1649, 1237, -90), map, 2651, 0, "bed");
                    BuildStatic(new Point3D(1651, 1237, -90), map, 2651, 0, "bed");
                    BuildStatic(new Point3D(1653, 1237, -90), map, 2651, 0, "bed");
                    BuildStatic(new Point3D(1655, 1237, -90), map, 2651, 0, "bed");
                    BuildStatic(new Point3D(1657, 1237, -90), map, 2651, 0, "bed");

                    BuildStatic(new Point3D(1649, 1238, -90), map, 2650, 0, "bed");
                    BuildStatic(new Point3D(1651, 1238, -90), map, 2650, 0, "bed");
                    BuildStatic(new Point3D(1653, 1238, -90), map, 2650, 0, "bed");
                    BuildStatic(new Point3D(1655, 1238, -90), map, 2650, 0, "bed");
                    BuildStatic(new Point3D(1657, 1238, -90), map, 2650, 0, "bed");

                    BuildStatic(new Point3D(1649, 1240, -90), map, 2854, 0, "candelabra");
                    BuildStatic(new Point3D(1655, 1240, -90), map, 2854, 0, "candelabra");

                    BuildStatic(new Point3D(1649, 1243, -90), map, 2861, 0, "bench");
                    BuildStatic(new Point3D(1656, 1243, -90), map, 2861, 0, "bench");

                    BuildStatic(new Point3D(1649, 1244, -90), map, 4012, 0, "firepit");
                    BuildStatic(new Point3D(1649, 1244, -90), map, 2420, 0, "cauldron");

                    BuildStatic(new Point3D(1655, 1244, -90), map, 4015, 0, "anvil");
                    BuildStatic(new Point3D(1656, 1244, -90), map, 4017, 0, "forge");

                    BuildStatic(new Point3D(1652, 1241, -90), map, 4573, 0, "table");
                    BuildStatic(new Point3D(1653, 1241, -90), map, 4572, 0, "table");
                    BuildStatic(new Point3D(1652, 1242, -90), map, 4573, 0, "table");
                    BuildStatic(new Point3D(1653, 1242, -90), map, 4572, 0, "table");

                    BuildStatic(new Point3D(1653, 1242, -84), map, 7824, 0, "deer carcass");

                    //Stumps
                    BuildStatic(new Point3D(1641, 1254, -90), map, 3670, 0, "stump");
                    BuildStatic(new Point3D(1648, 1255, -90), map, 3671, 0, "stump");
                    BuildStatic(new Point3D(1647, 1259, -90), map, 3671, 0, "stump");
                    BuildStatic(new Point3D(1656, 1260, -90), map, 3671, 0, "stump");
                    BuildStatic(new Point3D(1659, 1254, -90), map, 3671, 0, "stump");
                    BuildStatic(new Point3D(1664, 1256, -90), map, 3670, 0, "stump");
                    BuildStatic(new Point3D(1668, 1248, -90), map, 3673, 0, "stump");
                    BuildStatic(new Point3D(1667, 1239, -90), map, 3673, 0, "stump");
                    BuildStatic(new Point3D(1671, 1239, -90), map, 3673, 0, "stump");
                    BuildStatic(new Point3D(1674, 1232, -90), map, 3673, 0, "stump");
                    BuildStatic(new Point3D(1669, 1230, -90), map, 3672, 0, "stump");
                    BuildStatic(new Point3D(1668, 1223, -90), map, 3671, 0, "stump");
                    BuildStatic(new Point3D(1662, 1225, -90), map, 3671, 0, "stump");
                    BuildStatic(new Point3D(1655, 1223, -90), map, 3671, 0, "stump");
                    BuildStatic(new Point3D(1649, 1227, -90), map, 3671, 0, "stump");
                    BuildStatic(new Point3D(1640, 1226, -90), map, 3671, 0, "stump");
                    BuildStatic(new Point3D(1639, 1239, -90), map, 3671, 0, "stump");
                    BuildStatic(new Point3D(1636, 1233, -90), map, 3672, 0, "stump");

                    //Fort Construction Tiles

                    //Top
                    Point3D constructionPoint = new Point3D(1644, 1232, -90);

                    BuildStatic(constructionPoint, map, 1825, 0, "wood");
                    constructionPoint.Z += 5;
                    BuildStatic(constructionPoint, map, 1825, 0, "wood");
                    constructionPoint.Z += 5;
                    BuildStatic(constructionPoint, map, 1825, 0, "wood");
                    constructionPoint.Z += 5;

                    constructionTile = new UOACZConstructionTile();
                    constructionTile.ObjectFacing = Direction.North;
                    constructionTile.Hue = 2415;
                    constructionTile.MoveToWorld(constructionPoint, map);

                    constructionTileTargeter = new UOACZConstructionObjectEffectTargeter();
                    constructionTileTargeter.ConstructionTile = constructionTile;
                    constructionTileTargeter.MoveToWorld(new Point3D(constructionPoint.X - 2, constructionPoint.Y, -90), map);

                    constructionTileTargeter = new UOACZConstructionObjectEffectTargeter();
                    constructionTileTargeter.ConstructionTile = constructionTile;
                    constructionTileTargeter.MoveToWorld(new Point3D(constructionPoint.X - 2, constructionPoint.Y - 2, -90), map);

                    constructionTileTargeter = new UOACZConstructionObjectEffectTargeter();
                    constructionTileTargeter.ConstructionTile = constructionTile;
                    constructionTileTargeter.MoveToWorld(new Point3D(constructionPoint.X, constructionPoint.Y - 2, -90), map);

                    //Right
                    constructionPoint = new Point3D(1662, 1232, -90);

                    BuildStatic(constructionPoint, map, 1825, 0, "wood");
                    constructionPoint.Z += 5;
                    BuildStatic(constructionPoint, map, 1825, 0, "wood");
                    constructionPoint.Z += 5;
                    BuildStatic(constructionPoint, map, 1825, 0, "wood");
                    constructionPoint.Z += 5;

                    constructionTile = new UOACZConstructionTile();
                    constructionTile.ObjectFacing = Direction.North;
                    constructionTile.Hue = 2415;
                    constructionTile.MoveToWorld(constructionPoint, map);

                    constructionTileTargeter = new UOACZConstructionObjectEffectTargeter();
                    constructionTileTargeter.ConstructionTile = constructionTile;
                    constructionTileTargeter.MoveToWorld(new Point3D(constructionPoint.X, constructionPoint.Y - 2, -90), map);

                    constructionTileTargeter = new UOACZConstructionObjectEffectTargeter();
                    constructionTileTargeter.ConstructionTile = constructionTile;
                    constructionTileTargeter.MoveToWorld(new Point3D(constructionPoint.X + 2, constructionPoint.Y - 2, -90), map);

                    constructionTileTargeter = new UOACZConstructionObjectEffectTargeter();
                    constructionTileTargeter.ConstructionTile = constructionTile;
                    constructionTileTargeter.MoveToWorld(new Point3D(constructionPoint.X + 2, constructionPoint.Y, -90), map);

                    //Bottom
                    constructionPoint = new Point3D(1662, 1250, -90);

                    BuildStatic(constructionPoint, map, 1825, 0, "wood");
                    constructionPoint.Z += 5;
                    BuildStatic(constructionPoint, map, 1825, 0, "wood");
                    constructionPoint.Z += 5;
                    BuildStatic(constructionPoint, map, 1825, 0, "wood");
                    constructionPoint.Z += 5;

                    constructionTile = new UOACZConstructionTile();
                    constructionTile.ObjectFacing = Direction.North;
                    constructionTile.Hue = 2415;
                    constructionTile.MoveToWorld(constructionPoint, map);

                    constructionTileTargeter = new UOACZConstructionObjectEffectTargeter();
                    constructionTileTargeter.ConstructionTile = constructionTile;
                    constructionTileTargeter.MoveToWorld(new Point3D(constructionPoint.X + 2, constructionPoint.Y, -90), map);

                    constructionTileTargeter = new UOACZConstructionObjectEffectTargeter();
                    constructionTileTargeter.ConstructionTile = constructionTile;
                    constructionTileTargeter.MoveToWorld(new Point3D(constructionPoint.X + 2, constructionPoint.Y + 2, -90), map);

                    constructionTileTargeter = new UOACZConstructionObjectEffectTargeter();
                    constructionTileTargeter.ConstructionTile = constructionTile;
                    constructionTileTargeter.MoveToWorld(new Point3D(constructionPoint.X, constructionPoint.Y + 2, -90), map);

                    //Left
                    constructionPoint = new Point3D(1644, 1250, -90);

                    BuildStatic(constructionPoint, map, 1825, 0, "wood");
                    constructionPoint.Z += 5;
                    BuildStatic(constructionPoint, map, 1825, 0, "wood");
                    constructionPoint.Z += 5;
                    BuildStatic(constructionPoint, map, 1825, 0, "wood");
                    constructionPoint.Z += 5;

                    constructionTile = new UOACZConstructionTile();
                    constructionTile.ObjectFacing = Direction.North;
                    constructionTile.Hue = 2415;
                    constructionTile.MoveToWorld(constructionPoint, map);

                    constructionTileTargeter = new UOACZConstructionObjectEffectTargeter();
                    constructionTileTargeter.ConstructionTile = constructionTile;
                    constructionTileTargeter.MoveToWorld(new Point3D(constructionPoint.X, constructionPoint.Y + 2, -90), map);

                    constructionTileTargeter = new UOACZConstructionObjectEffectTargeter();
                    constructionTileTargeter.ConstructionTile = constructionTile;
                    constructionTileTargeter.MoveToWorld(new Point3D(constructionPoint.X - 2, constructionPoint.Y + 2, -90), map);

                    constructionTileTargeter = new UOACZConstructionObjectEffectTargeter();
                    constructionTileTargeter.ConstructionTile = constructionTile;
                    constructionTileTargeter.MoveToWorld(new Point3D(constructionPoint.X - 2, constructionPoint.Y, -90), map);

                    //Trash Barrels
                    TrashBarrel trashBarrel = new TrashBarrel();
                    trashBarrel.MoveToWorld(new Point3D(1654, 1242, -90), map);

                    trashBarrel = new TrashBarrel();
                    trashBarrel.MoveToWorld(new Point3D(2093, 1377, -88), map);

                    trashBarrel = new TrashBarrel();
                    trashBarrel.MoveToWorld(new Point3D(1990, 1369, -80), map);

                    trashBarrel = new TrashBarrel();
                    trashBarrel.MoveToWorld(new Point3D(2058, 1306, -90), map);

                     trashBarrel = new TrashBarrel();
                    trashBarrel.MoveToWorld(new Point3D(2004, 1306, -90), map);

                    #endregion

                    #region Forges

                    List<Point3D> m_Anvils = new List<Point3D>();
                    List<Point3D> m_Forges = new List<Point3D>();

                    m_Anvils.Add(new Point3D(1993, 1553, -90));
                    m_Forges.Add(new Point3D(1993, 1552, -90));

                    m_Anvils.Add(new Point3D(2144, 1468, -90));
                    m_Forges.Add(new Point3D(2143, 1468, -90));

                    m_Anvils.Add(new Point3D(2287, 1424, -87));
                    m_Forges.Add(new Point3D(2286, 1424, -87));

                    m_Anvils.Add(new Point3D(2221, 1297, -87));
                    m_Forges.Add(new Point3D(2221, 1296, -87));

                    m_Anvils.Add(new Point3D(2428, 1319, -90));
                    m_Forges.Add(new Point3D(2427, 1319, -90));

                    m_Anvils.Add(new Point3D(2110, 1250, -75));
                    m_Forges.Add(new Point3D(2109, 1250, -76));

                    foreach (Point3D point in m_Anvils)
                    {
                        BuildStatic(point, map, 4015, 0, "anvil");
                    }

                    foreach (Point3D point in m_Forges)
                    {
                        BuildStatic(point, map, 4017, 0, "forge");
                    }

                    #endregion

                    #region Spawn Redirectors

                    UOACZSpawnRedirector spawnRedirector = new UOACZSpawnRedirector();
                    spawnRedirector.TriggerRadius = 10;
                    spawnRedirector.MinRedirection = 12;
                    spawnRedirector.MaxRedirection = 16;
                    spawnRedirector.MoveToWorld(new Point3D(1653, 1242, -84), map);

                    spawnRedirector = new UOACZSpawnRedirector();
                    spawnRedirector.TriggerRadius = 30;
                    spawnRedirector.MinRedirection = 35;
                    spawnRedirector.MaxRedirection = 40;
                    spawnRedirector.MoveToWorld(new Point3D(2367, 1128, -90), map);

                    #endregion

                    #region Spawn Area Blockers

                    UOACZSpawnAreaBlocker spawnAreaBlocker = new UOACZSpawnAreaBlocker();
                    spawnAreaBlocker.BlockedArea = new Rectangle2D(new Point2D(0, 0), new Point2D(1622, 1550));
                    spawnAreaBlocker.MoveToWorld(new Point3D(1622, 1550, -115), map);

                    spawnAreaBlocker = new UOACZSpawnAreaBlocker();
                    spawnAreaBlocker.BlockedArea = new Rectangle2D(new Point2D(0, 1589), new Point2D(2000, 2000));
                    spawnAreaBlocker.MoveToWorld(new Point3D(1967, 1586, -90), map);

                    spawnAreaBlocker = new UOACZSpawnAreaBlocker();
                    spawnAreaBlocker.BlockedArea = new Rectangle2D(new Point2D(2240, 1465), new Point2D(2348, 1514));
                    spawnAreaBlocker.MoveToWorld(new Point3D(2273, 1467, -90), map);

                    #endregion

                    #region Corruption Sourcestone Building

                    List<Point3D> m_SourcestoneGuards = new List<Point3D>();

                    m_SourcestoneGuards.Add(new Point3D(2367, 1132, -90));
                    m_SourcestoneGuards.Add(new Point3D(2362, 1127, -90));
                    m_SourcestoneGuards.Add(new Point3D(2367, 1122, -90));
                    m_SourcestoneGuards.Add(new Point3D(2372, 1127, -90));

                    foreach (Point3D point in m_SourcestoneGuards)
                    {
                        undeadSpawner = new UOACZUndeadSpawner();
                        undeadSpawner.HomeRange = 8;
                        undeadSpawner.SpawnRange = 8;
                        undeadSpawner.MaxSpawnCount = 2;
                        undeadSpawner.MinSpawnTime = 5;
                        undeadSpawner.MaxSpawnTime = 10;

                        undeadSpawner.MinThreatLevel = 130;
                        undeadSpawner.MaxThreatLevel = 160;
                        undeadSpawner.IgnoreRedirector = true;
                        undeadSpawner.GuardHome = true;
                        undeadSpawner.GuardHomeRange = 25;

                        undeadSpawner.AllowTeleportToBaseNode = false;
                        undeadSpawner.MoveToWorld(point, map);
                    }

                    #endregion

                    #region Hero Locations

                    //Human
                    UOACZPersistance.HumanChampionLocation = new Point3D(2295, 1358, -87);
                    UOACZPersistance.HumanBossLocation = new Point3D(2016, 1343, -90);

                    //Undead
                    UOACZPersistance.UndeadChampionLocation = new Point3D(2322, 1267, -110);
                    UOACZPersistance.UndeadBossLocation = new Point3D(2029, 1297, -90);

                    #endregion
                    break;

                case UOACZCustomRegion.Terminus1:
                    break;
            }
        }

        public static void BuildStatic(Point3D location, Map map, int itemId, int itemHue, string itemName)
        {
            UOACZStatic item = new UOACZStatic();

            item.Name = itemName;
            item.ItemID = itemId;
            item.Hue = itemHue;
            item.Movable = false;

            item.MoveToWorld(location, map);
        }

        public static void BuildStaticLine(Point3D pointStart, Point3D pointEnd, Map map, Direction direction, int itemId, int hue, string name)
        {
            int distance = Utility.GetDistance(pointStart, pointEnd);

            Point3D currentLocation = pointStart;

            for (int a = 0; a < distance + 1; a++)
            {
                Point3D adjustedLocation = currentLocation;

                SpellHelper.AdjustField(ref adjustedLocation, map, 20, false);

                UOACZStatic staticItem = new UOACZStatic();
                staticItem.ItemID = itemId;
                staticItem.Hue = hue;
                staticItem.Name = name;
                staticItem.MoveToWorld(adjustedLocation, map);

                new LOSBlocker().MoveToWorld(adjustedLocation, map);

                currentLocation = SpecialAbilities.GetPointByDirection(currentLocation, direction);
            }
        }

        public static void BuildLOSBlockerLine(Point3D pointStart, Point3D pointEnd, Map map, Direction direction)
        {
            int distance = Utility.GetDistance(pointStart, pointEnd);

            Point3D currentLocation = pointStart;

            for (int a = 0; a < distance + 1; a++)
            {
                Point3D adjustedLocation = currentLocation;

                SpellHelper.AdjustField(ref adjustedLocation, map, 20, false);
                new LOSBlocker().MoveToWorld(adjustedLocation, map);

                currentLocation = SpecialAbilities.GetPointByDirection(currentLocation, direction);
            }
        }

        public static void CalculateScores()
        {
            UOACZAccountEntry m_HighestTotalScoreAccount = null;

            int highestTotalScoreValue = 0;
            List<UOACZAccountEntry> m_PotentialHighestTotalScoreAccounts = new List<UOACZAccountEntry>();

            List<UOACZAccountEntry> m_SetNewBestHumanScore = new List<UOACZAccountEntry>();
            List<UOACZAccountEntry> m_SetNewBestUndeadScore = new List<UOACZAccountEntry>();
            List<UOACZAccountEntry> m_SetNewBestTotalScore = new List<UOACZAccountEntry>();

            List<UOACZAccountEntry> m_ParticipationRewards = new List<UOACZAccountEntry>();

            foreach (UOACZAccountEntry accountEntry in UOACZPersistance.m_UOACZAccountEntries)
            {
                if (accountEntry == null)
                    continue;

                //Personal Best
                if (accountEntry.CurrentSessionHumanScore > accountEntry.BestHumanScore)
                {
                    accountEntry.BestHumanScore = accountEntry.CurrentSessionHumanScore;
                    m_SetNewBestHumanScore.Add(accountEntry);
                }

                if (accountEntry.CurrentSessionUndeadScore > accountEntry.BestUndeadScore)
                {
                    accountEntry.BestUndeadScore = accountEntry.CurrentSessionUndeadScore;
                    m_SetNewBestUndeadScore.Add(accountEntry);
                }

                if (accountEntry.CurrentSessionTotalScore > accountEntry.BestTotalScore)
                {
                    accountEntry.BestTotalScore = accountEntry.CurrentSessionTotalScore;
                    m_SetNewBestTotalScore.Add(accountEntry);
                }

                //Participation Reward Points
                if (accountEntry.CurrentSessionTotalScore >= UOACZSystem.MinScoreToQualifyAsParticipant)
                {
                    accountEntry.RewardPoints += UOACZSystem.ParticipationRewardPoints;
                    accountEntry.TotalRewardPointsEarned += UOACZSystem.ParticipationRewardPoints;

                    m_ParticipationRewards.Add(accountEntry);
                }

                //Determine Highest Session Scores
                if (accountEntry.CurrentSessionTotalScore > 0)
                {
                    if (accountEntry.CurrentSessionTotalScore == highestTotalScoreValue)
                    {
                        if (accountEntry.MostRecentPlayer != null)
                        {
                            if (!accountEntry.MostRecentPlayer.Deleted)
                                m_PotentialHighestTotalScoreAccounts.Add(accountEntry);
                        }
                    }

                    else if (accountEntry.CurrentSessionTotalScore > highestTotalScoreValue)
                    {
                        if (accountEntry.MostRecentPlayer != null)
                        {
                            if (!accountEntry.MostRecentPlayer.Deleted)
                            {
                                highestTotalScoreValue = accountEntry.CurrentSessionTotalScore;

                                m_PotentialHighestTotalScoreAccounts.Clear();
                                m_PotentialHighestTotalScoreAccounts.Add(accountEntry);
                            }
                        }
                    }
                }

                accountEntry.PreviousSessionHumanScore = accountEntry.CurrentSessionHumanScore;
                accountEntry.PreviousSessionUndeadScore = accountEntry.CurrentSessionUndeadScore;
                accountEntry.PreviousSessionTotalScore = accountEntry.CurrentSessionTotalScore;

                accountEntry.LifetimeHumanScore += accountEntry.CurrentSessionHumanScore;
                accountEntry.LifetimeUndeadScore += accountEntry.CurrentSessionUndeadScore;
                accountEntry.LifetimeTotalScore += accountEntry.CurrentSessionTotalScore;

                accountEntry.CurrentSessionHumanScore = 0;
                accountEntry.CurrentSessionUndeadScore = 0;
                accountEntry.CurrentSessionTotalScore = 0;
            }

            if (highestTotalScoreValue > 0 && m_PotentialHighestTotalScoreAccounts.Count > 0)
            {
                if (m_PotentialHighestTotalScoreAccounts.Count > 1)
                {
                    int highestTotalMinutes = 0;
                    UOACZAccountEntry bestPotentialEntry = null;

                    foreach (UOACZAccountEntry potentialEntry in m_PotentialHighestTotalScoreAccounts)
                    {
                        int potentialTotalMinutes = potentialEntry.HumanProfile.TotalMinutesAlive;

                        if (potentialTotalMinutes > highestTotalMinutes)
                        {
                            highestTotalMinutes = potentialTotalMinutes;

                            m_HighestTotalScoreAccount = potentialEntry;
                        }
                    }
                }

                else
                    m_HighestTotalScoreAccount = m_PotentialHighestTotalScoreAccounts[0];
            }

            string privateWinnerMessage = "";
            string publicWinnerMessage = "";

            //Winner
            if (m_HighestTotalScoreAccount != null && highestTotalScoreValue > 0)
            {
                m_HighestTotalScoreAccount.RewardPoints += UOACZSystem.HighestTotalScoreRewardPoints;
                m_HighestTotalScoreAccount.TotalRewardPointsEarned += UOACZSystem.HighestTotalScoreRewardPoints;

                int totalPointsEarned = ParticipationRewardPoints + UOACZSystem.HighestTotalScoreRewardPoints;

                PlayerMobile winner = m_HighestTotalScoreAccount.MostRecentPlayer;

                AchievementSystemImpl.Instance.TickProgressMulti(winner, AchievementTriggers.Trigger_UOACZHighestSessionTotalScore, 1);

                if (winner != null)
                {
                    privateWinnerMessage = "You have earned the top score with a total of " + highestTotalScoreValue.ToString() + ". You have earned " + totalPointsEarned.ToString() + " UOACZ Reward Points.";
                    publicWinnerMessage = winner.RawName + " earned the top score with a total of " + highestTotalScoreValue.ToString() + ".";

                    m_HighestTotalScoreAccount.HighestSessionScoreCount++;
                }
            }

            //Announcements
            foreach (NetState state in NetState.Instances)
            {
                Mobile mobile = state.Mobile;
                PlayerMobile player = mobile as PlayerMobile;

                if (player == null)
                    continue;

                if (player.HasGump(typeof(UOACZScoreGump)))
                    player.CloseGump(typeof(UOACZScoreGump));

                List<String> m_QueuedMessages = new List<string>();

                UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

                if (m_HighestTotalScoreAccount != null && highestTotalScoreValue > 0)
                {
                    if (m_HighestTotalScoreAccount == player.m_UOACZAccountEntry)
                        m_QueuedMessages.Add(privateWinnerMessage);

                    else
                        m_QueuedMessages.Add(publicWinnerMessage);
                }

                if (player.m_UOACZAccountEntry != m_HighestTotalScoreAccount && player.AccessLevel == AccessLevel.Player)
                {
                    if (m_ParticipationRewards.Contains(player.m_UOACZAccountEntry))
                        m_QueuedMessages.Add("Your total score for the session was " + player.m_UOACZAccountEntry.PreviousSessionTotalScore.ToString() + ". You have received " + UOACZSystem.ParticipationRewardPoints.ToString() + " UOACZ Reward Points.");

                    else
                        m_QueuedMessages.Add("Your total score for the session was " + player.m_UOACZAccountEntry.PreviousSessionTotalScore.ToString() + ". You must score a total of at least " + UOACZSystem.ParticipationRewardPoints.ToString() + " to qualify for UOACZ Reward Points.");
                }

                m_QueuedMessages.Add("You can view scores and rankings as well as claim rewards and manage unlockables at the UOACZ Stone at The West Britain Bank or Buccs Den.");

                for (int a = 0; a < m_QueuedMessages.Count; a++)
                {
                    string message = m_QueuedMessages[a];

                    Timer.DelayCall(TimeSpan.FromSeconds(a * UOACZSystem.PostSessionTextDelayPerMessage), delegate
                    {
                        if (player == null)
                            return;

                        player.SendMessage(UOACZSystem.purpleTextHue, message);
                    });
                }
            }

            //Store and Sort Scores
            UOACZPersistance.m_PreviousHumanScores = new List<UOACZAccountEntry>();
            UOACZPersistance.m_PreviousUndeadScores = new List<UOACZAccountEntry>();
            UOACZPersistance.m_PreviousTotalScores = new List<UOACZAccountEntry>();

            UOACZPersistance.m_BestHumanScores = new List<UOACZAccountEntry>();
            UOACZPersistance.m_BestUndeadScores = new List<UOACZAccountEntry>();
            UOACZPersistance.m_BestTotalScores = new List<UOACZAccountEntry>();

            UOACZPersistance.m_LifetimeHumanScores = new List<UOACZAccountEntry>();
            UOACZPersistance.m_LifetimeUndeadScores = new List<UOACZAccountEntry>();
            UOACZPersistance.m_LifetimeTotalScores = new List<UOACZAccountEntry>();

            foreach (UOACZAccountEntry accountEntry in UOACZPersistance.m_UOACZAccountEntries)
            {
                if (accountEntry.PreviousSessionHumanScore > 0)
                    UOACZPersistance.m_PreviousHumanScores.Add(accountEntry);

                if (accountEntry.PreviousSessionUndeadScore > 0)
                    UOACZPersistance.m_PreviousUndeadScores.Add(accountEntry);

                if (accountEntry.PreviousSessionTotalScore > 0)
                    UOACZPersistance.m_PreviousTotalScores.Add(accountEntry);

                if (accountEntry.BestHumanScore > 0)
                    UOACZPersistance.m_BestHumanScores.Add(accountEntry);

                if (accountEntry.BestUndeadScore > 0)
                    UOACZPersistance.m_BestUndeadScores.Add(accountEntry);

                if (accountEntry.BestTotalScore > 0)
                    UOACZPersistance.m_BestTotalScores.Add(accountEntry);

                if (accountEntry.LifetimeHumanScore > 0)
                    UOACZPersistance.m_LifetimeHumanScores.Add(accountEntry);

                if (accountEntry.LifetimeUndeadScore > 0)
                    UOACZPersistance.m_LifetimeUndeadScores.Add(accountEntry);

                if (accountEntry.LifetimeTotalScore > 0)
                    UOACZPersistance.m_LifetimeTotalScores.Add(accountEntry);
            }

            UOACZPersistance.m_PreviousHumanScores = UOACZPersistance.m_PreviousHumanScores.OrderByDescending(a => a.PreviousSessionHumanScore).ToList();
            UOACZPersistance.m_PreviousUndeadScores = UOACZPersistance.m_PreviousUndeadScores.OrderByDescending(a => a.PreviousSessionUndeadScore).ToList();
            UOACZPersistance.m_PreviousTotalScores = UOACZPersistance.m_PreviousTotalScores.OrderByDescending(a => a.PreviousSessionTotalScore).ToList();

            UOACZPersistance.m_BestHumanScores = UOACZPersistance.m_BestHumanScores.OrderByDescending(a => a.BestHumanScore).ToList();
            UOACZPersistance.m_BestUndeadScores = UOACZPersistance.m_BestUndeadScores.OrderByDescending(a => a.BestUndeadScore).ToList();
            UOACZPersistance.m_BestTotalScores = UOACZPersistance.m_BestTotalScores.OrderByDescending(a => a.BestTotalScore).ToList();

            UOACZPersistance.m_LifetimeHumanScores = UOACZPersistance.m_LifetimeHumanScores.OrderByDescending(a => a.LifetimeHumanScore).ToList();
            UOACZPersistance.m_LifetimeUndeadScores = UOACZPersistance.m_LifetimeUndeadScores.OrderByDescending(a => a.LifetimeUndeadScore).ToList();
            UOACZPersistance.m_LifetimeTotalScores = UOACZPersistance.m_LifetimeTotalScores.OrderByDescending(a => a.LifetimeTotalScore).ToList();
        }

        public class UOACZStatMod : StatMod
        {
            public UOACZStatMod(StatType type, int newValue, int prevValue)
                : base(type, GetStatModString(type), GetStat(newValue, prevValue), TimeSpan.FromDays(2))
            {
            }

            private static string GetStatModString(StatType type)
            {
                return "UOACZStatMod_" + type.ToString();
            }

            private static int GetStat(int newValue, int prevValue)
            {
                return newValue - prevValue;
            }
        }

        public static BaseArmor GetRandomCrudeBoneArmor()
        {
            BaseArmor armor = null;

            switch (Utility.RandomMinMax(1, 6))
            {
                case 1: armor = new UOACZCrudeBoneGloves(); break;
                case 2: armor = new UOACZCrudeBoneHelm(); break;
                case 3: armor = new UOACZCrudeBoneArms(); break;
                case 4: armor = new UOACZCrudeBoneLegs(); break;
                case 5: armor = new UOACZCrudeBoneChest(); break;
                case 6: armor = new UOACZCrudeBoneShield(); break;
            }

            return armor;
        }

        public static BaseWeapon GetRandomCrudeBoneWeapon()
        {
            BaseWeapon weapon = null;

            switch (Utility.RandomMinMax(1, 4))
            {
                case 1:
                    switch (Utility.RandomMinMax(1, 3))
                    {
                        case 1: weapon = new Bow() { Name = "crude bone bow" }; break;
                        case 2: weapon = new Crossbow() { Name = "crude bone crossbow" }; break;
                        case 3: weapon = new HeavyCrossbow() { Name = "crude bone heavy crossbow" }; break;
                    }
                    break;

                case 2:
                    switch (Utility.RandomMinMax(1, 6))
                    {
                        case 1: weapon = new UOACZDagger() { Name = "crude bone dagger" }; break;
                        case 2: weapon = new Kryss() { Name = "crude bone kryss" }; break;
                        case 3: weapon = new WarFork() { Name = "crude bone warfork" }; break;
                        case 4: weapon = new ShortSpear() { Name = "crude bone short spear" }; break;
                        case 5: weapon = new Spear() { Name = "crude bone spear" }; break;
                        case 6: weapon = new Pitchfork() { Name = "crude bone pitchfork" }; break;
                    }
                    break;

                case 3:
                    switch (Utility.RandomMinMax(1, 10))
                    {
                        case 1: weapon = new Club() { Name = "crude bone club" }; break;
                        case 2: weapon = new Mace() { Name = "crude bone mace" }; break;
                        case 3: weapon = new Maul() { Name = "crude bone maul" }; break;
                        case 4: weapon = new WarMace() { Name = "crude bone warmace" }; break;
                        case 5: weapon = new WarHammer() { Name = "crude bone warhammer" }; break;
                        case 6: weapon = new QuarterStaff() { Name = "crude bone quarterstaff" }; break;
                        case 7: weapon = new BlackStaff() { Name = "crude bone blackstaff" }; break;
                        case 8: weapon = new GnarledStaff() { Name = "crude bone gnarled staff" }; break;
                        case 9: weapon = new HammerPick() { Name = "crude bone hammerpick" }; break;
                        case 10: weapon = new WarAxe() { Name = "crude bone war axe" }; break;
                    }
                    break;

                case 4:
                    switch (Utility.RandomMinMax(1, 13))
                    {
                        case 1: weapon = new Cutlass() { Name = "crude bone cutlass" }; break;
                        case 2: weapon = new Scimitar() { Name = "crude bone scimitar" }; break;
                        case 3: weapon = new Katana() { Name = "crude bone katana" }; break;
                        case 4: weapon = new Longsword() { Name = "crude bone longsword" }; break;
                        case 5: weapon = new Broadsword() { Name = "crude bone broadsword" }; break;
                        case 6: weapon = new VikingSword() { Name = "crude bone viking sword" }; break;
                        case 7: weapon = new Axe() { Name = "crude bone axe" }; break;
                        case 8: weapon = new DoubleAxe() { Name = "crude bone double axe" }; break;
                        case 9: weapon = new TwoHandedAxe() { Name = "crude bone two-handed axe" }; break;
                        case 10: weapon = new ExecutionersAxe() { Name = "crude bone executioner's axe" }; break;
                        case 11: weapon = new LargeBattleAxe() { Name = "crude bone large battle axe" }; break;
                        case 12: weapon = new Bardiche() { Name = "crude bone bardiche" }; break;
                        case 13: weapon = new Halberd() { Name = "crude bone halberd" }; break;
                    }
                    break;
            }

            if (weapon != null)
            {
                weapon.Hue = 2498;
                weapon.Quality = Quality.Low;
            }

            return weapon;
        }

        public static void DamageCorpse(Point3D location, Map map, bool largeExplosion)
        {
            int projectiles = 5;
            int particleSpeed = 8;

            int minRadius = 1;
            int maxRadius = 5;

            List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(location, true, false, location, map, projectiles, 20, minRadius, maxRadius, false);

            if (m_ValidLocations.Count == 0)
                return;

            for (int b = 0; b < projectiles; b++)
            {
                Point3D newLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(location.X, location.Y, location.Z + 2), map);
                IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 20), map);

                newLocation.Z += 5;

                Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(4651, 4652, 4653, 4654, 5701), particleSpeed, 0, false, false, 0, 0);
            }

            for (int a = 0; a < 4; a++)
            {
                Point3D newPoint = new Point3D(location.X + Utility.RandomList(-2, -1, 1, 2), location.Y + Utility.RandomList(-2, -1, 1, 2), location.Z);
                SpellHelper.AdjustField(ref newPoint, map, 12, false);

                new Blood().MoveToWorld(newPoint, map);
            }

            if (largeExplosion)
            {
                List<int> m_ExtraParts = new List<int>();

                m_ExtraParts.Add(Utility.RandomList(7407)); //Entrail
                m_ExtraParts.Add(Utility.RandomList(6929)); //Bones
                m_ExtraParts.Add(Utility.RandomList(6930)); //Bones
                m_ExtraParts.Add(Utility.RandomList(6937)); //Bones
                m_ExtraParts.Add(Utility.RandomList(6938)); //Bones
                m_ExtraParts.Add(Utility.RandomList(6931)); //Bones
                m_ExtraParts.Add(Utility.RandomList(6932)); //Bones

                m_ExtraParts.Add(Utility.RandomList(4650)); //Blood
                m_ExtraParts.Add(Utility.RandomList(4651)); //Blood
                m_ExtraParts.Add(Utility.RandomList(4652)); //Blood
                m_ExtraParts.Add(Utility.RandomList(4653)); //Blood
                m_ExtraParts.Add(Utility.RandomList(4654)); //Blood
                m_ExtraParts.Add(Utility.RandomList(5701)); //Blood
                m_ExtraParts.Add(Utility.RandomList(4655)); //Blood
                m_ExtraParts.Add(Utility.RandomList(7439)); //Blood
                m_ExtraParts.Add(Utility.RandomList(7438)); //Blood
                m_ExtraParts.Add(Utility.RandomList(7436)); //Blood
                m_ExtraParts.Add(Utility.RandomList(7433)); //Blood
                m_ExtraParts.Add(Utility.RandomList(7431)); //Blood
                m_ExtraParts.Add(Utility.RandomList(7428)); //Blood
                m_ExtraParts.Add(Utility.RandomList(7425)); //Blood
                m_ExtraParts.Add(Utility.RandomList(7410)); //Blood
                m_ExtraParts.Add(Utility.RandomList(7415)); //Blood
                m_ExtraParts.Add(Utility.RandomList(7416)); //Blood
                m_ExtraParts.Add(Utility.RandomList(7418)); //Blood
                m_ExtraParts.Add(Utility.RandomList(7420)); //Blood
                m_ExtraParts.Add(Utility.RandomList(7425)); //Blood

                double extraPartChance = .5;

                int radius = 2;
                int explosionSound = Utility.RandomList(0x4F1, 0x5D8, 0x5DA, 0x580);

                Effects.PlaySound(location, map, explosionSound);

                int minRange = radius * -1;
                int maxRange = radius;

                List<Point3D> m_ExplosionPoints = new List<Point3D>();

                for (int a = minRange; a < maxRange + 1; a++)
                {
                    for (int b = minRange; b < maxRange + 1; b++)
                    {
                        Point3D newPoint = new Point3D(location.X + a, location.Y + b, location.Z);
                        SpellHelper.AdjustField(ref newPoint, map, 12, false);

                        if (map.InLOS(location, newPoint))
                            m_ExplosionPoints.Add(newPoint);
                    }
                }

                for (int a = 0; a < m_ExplosionPoints.Count; a++)
                {
                    if (Utility.RandomDouble() <= extraPartChance)
                    {
                        Point3D explosionPoint = m_ExplosionPoints[a];

                        int itemId = m_ExtraParts[Utility.RandomMinMax(0, m_ExtraParts.Count - 1)];

                        int distance = Utility.GetDistance(location, explosionPoint);

                        Timer.DelayCall(TimeSpan.FromSeconds(distance * .15), delegate
                        {
                            TimedStatic gore = new TimedStatic(itemId, 10);
                            gore.Name = "gore";
                            gore.MoveToWorld(explosionPoint, map);
                        });
                    }
                }
            }
        }

        public static void SpawnRandomCreature(Mobile from, Point3D location, Map map, int threatLevel, int minRange, int maxRange, bool ignoreRedirector)
        {
            bool spawnCreated = false;

            for (int b = 0; b < 25; b++)
            {
                if (spawnCreated)
                    break;

                Point3D newLocation = new Point3D();

                int x = location.X;

                int xOffset = Utility.RandomMinMax(minRange, maxRange);
                if (Utility.RandomDouble() >= .5)
                    xOffset *= -1;

                x += xOffset;

                int y = location.Y;

                int yOffset = Utility.RandomMinMax(minRange, maxRange);
                if (Utility.RandomDouble() >= .5)
                    yOffset *= -1;

                y += yOffset;

                newLocation.X = x;
                newLocation.Y = y;
                newLocation.Z = location.Z;

                SpellHelper.AdjustField(ref newLocation, map, 12, false);

                bool areaIsBlocked = false;

                foreach (UOACZSpawnAreaBlocker spawnAreaBlocker in UOACZSpawnAreaBlocker.m_Instances)
                {
                    if (!UOACZRegion.ContainsItem(spawnAreaBlocker))
                        continue;

                    if (spawnAreaBlocker.BlockedArea.Contains(newLocation))
                    {
                        areaIsBlocked = true;
                        break;
                    }
                }

                if (areaIsBlocked)
                    continue;

                UOACZSpawnRedirector spawnRedirector = null;

                if (!ignoreRedirector)
                {
                    foreach (UOACZSpawnRedirector redirector in UOACZSpawnRedirector.m_Instances)
                    {
                        if (!UOACZRegion.ContainsItem(redirector))
                            continue;

                        if (Utility.GetDistance(redirector.Location, newLocation) <= redirector.TriggerRadius)
                        {
                            spawnRedirector = redirector;
                            break;
                        }
                    }
                }

                if (spawnRedirector != null)
                {
                    newLocation.X = spawnRedirector.Location.X + Utility.RandomMinMax(-1 * spawnRedirector.TriggerRadius, spawnRedirector.TriggerRadius);
                    newLocation.Y = spawnRedirector.Location.Y + Utility.RandomMinMax(-1 * spawnRedirector.TriggerRadius, spawnRedirector.TriggerRadius);
                    newLocation.Z = spawnRedirector.Location.Z;

                    int redirectionX = 0;
                    int redirectionY = 0;

                    switch (Utility.RandomMinMax(1, 3))
                    {
                        case 1:
                            redirectionX = Utility.RandomMinMax(spawnRedirector.MinRedirection, spawnRedirector.MaxRedirection);

                            if (Utility.RandomDouble() <= .5)
                                redirectionX *= -1;

                            newLocation.X = spawnRedirector.Location.X + redirectionX;
                            break;

                        case 2:
                            redirectionY = Utility.RandomMinMax(spawnRedirector.MinRedirection, spawnRedirector.MaxRedirection);

                            if (Utility.RandomDouble() <= .5)
                                redirectionY *= -1;

                            newLocation.Y = spawnRedirector.Location.Y + redirectionY;
                            break;

                        case 3:
                            redirectionX = Utility.RandomMinMax(spawnRedirector.MinRedirection, spawnRedirector.MaxRedirection);

                            if (Utility.RandomDouble() <= .5)
                                redirectionX *= -1;

                            newLocation.X = spawnRedirector.Location.X + redirectionX;

                            redirectionY = Utility.RandomMinMax(spawnRedirector.MinRedirection, spawnRedirector.MaxRedirection);

                            if (Utility.RandomDouble() <= .5)
                                redirectionY *= -1;

                            newLocation.Y = spawnRedirector.Location.Y + redirectionY;
                            break;
                    }

                    SpellHelper.AdjustField(ref newLocation, map, 12, false);
                }

                if (UOACZSystem.IsValidSpawnLocation(newLocation, map, false))
                {
                    UOACZBaseUndead bc_Creature = (UOACZBaseUndead)Activator.CreateInstance(UOACZBaseUndead.GetRandomUndeadType(0, threatLevel));

                    if (bc_Creature != null)
                    {
                        bc_Creature.MoveToWorld(newLocation, map);

                        bc_Creature.RangeHome = 50;
                        bc_Creature.Home = newLocation;
                        bc_Creature.InWilderness = false;
                        bc_Creature.CanTeleportToBaseNode = true;

                        //Accelerate Potential Sending to BaseNode
                        bc_Creature.m_LastActivity = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(3));

                        Timer.DelayCall(TimeSpan.FromSeconds(1), delegate
                        {
                            PlayerMobile player = from as PlayerMobile;

                            if (player == null) return;
                            if (!IsUOACZValidMobile(from)) return;
                            if (!player.IsUOACZHuman) return;
                            if (bc_Creature == null) return;
                            if (bc_Creature.Deleted || !bc_Creature.Alive) return;

                            bc_Creature.Combatant = player;
                        });
                    }

                    spawnCreated = true;
                }
            }
        }

        public static bool IsValidSpawnLocation(Point3D point, Map map, bool requireAdjacentUnblocked)
        {
            if (map == null)
                return false;

            if (map.CanSpawnMobile(point))
            {
                if (!requireAdjacentUnblocked)
                    return true;

                List<Point3D> m_Points = new List<Point3D>();

                m_Points.Add(SpecialAbilities.GetPointByDirection(point, Direction.Up));
                m_Points.Add(SpecialAbilities.GetPointByDirection(point, Direction.North));
                m_Points.Add(SpecialAbilities.GetPointByDirection(point, Direction.Right));
                m_Points.Add(SpecialAbilities.GetPointByDirection(point, Direction.East));
                m_Points.Add(SpecialAbilities.GetPointByDirection(point, Direction.Down));
                m_Points.Add(SpecialAbilities.GetPointByDirection(point, Direction.South));
                m_Points.Add(SpecialAbilities.GetPointByDirection(point, Direction.Left));
                m_Points.Add(SpecialAbilities.GetPointByDirection(point, Direction.West));

                int blockingPoints = 0;

                foreach (Point3D checkPoint in m_Points)
                {
                    if (!map.CanSpawnMobile(checkPoint))
                        blockingPoints++;
                }

                if (blockingPoints > 1)
                    return false;

                return true;
            }

            return false;
        }

        public static int GetUOACZPlayerCount(bool human)
        {
            int onlineCount = 0;

            foreach (NetState state in NetState.Instances)
            {
                Mobile mobile = state.Mobile;
                PlayerMobile pm_Mobile = mobile as PlayerMobile;

                if (pm_Mobile == null)
                    continue;

                if (UOACZSystem.IsUOACZValidMobile(pm_Mobile))
                {
                    if (human && pm_Mobile.IsUOACZHuman)
                        onlineCount++;

                    if (!human && pm_Mobile.IsUOACZUndead)
                        onlineCount++;
                }
            }

            return onlineCount;
        }

        public static PlayerMobile GetUOACZPlayerOnline(bool human, bool allowHidden)
        {
            PlayerMobile player = null;

            List<PlayerMobile> m_Players = new List<PlayerMobile>();

            foreach (NetState state in NetState.Instances)
            {
                Mobile mobile = state.Mobile;
                PlayerMobile pm_Mobile = mobile as PlayerMobile;

                if (pm_Mobile == null)
                    continue;

                if (UOACZSystem.IsUOACZValidMobile(pm_Mobile))
                {
                    if (pm_Mobile.Hidden && !allowHidden)
                        continue;

                    if (human && pm_Mobile.IsUOACZHuman)
                        m_Players.Add(pm_Mobile);

                    if (!human && pm_Mobile.IsUOACZUndead)
                        m_Players.Add(pm_Mobile);
                }
            }

            if (m_Players.Count > 0)
                player = m_Players[Utility.RandomMinMax(0, m_Players.Count - 1)];

            return player;
        }

        public static void AddCreatureToPlayerSwarm(PlayerMobile player, BaseCreature creature)
        {
            AchievementSystemImpl.Instance.TickProgressMulti(player, AchievementTriggers.Trigger_UOACZUndeadAddToSwarm, 1);

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);
            player.m_UOACZAccountEntry.TotalFollowers++;

            if (player.Followers > player.m_UOACZAccountEntry.LargestSwarm)
                player.m_UOACZAccountEntry.LargestSwarm = player.Followers;

            if (player.Followers >= 15)
                AchievementSystemImpl.Instance.TickProgressMulti(player, AchievementTriggers.Trigger_UOACZUndeadSwarmSize15, 1);

            Timer.DelayCall(TimeSpan.FromSeconds(1), delegate
            {
                if (!IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZUndead) return;
                if (!IsUOACZValidMobile(creature)) return;

                creature.ControlOrder = OrderType.Guard;

                creature.PublicOverheadMessage(MessageType.Regular, 0, false, "*guards*");

                if (creature.AIObject != null)
                    creature.AIObject.DoOrderGuard();
            });
        }

        public static void PlayerInflictDamage(PlayerMobile player, Mobile target, bool melee, int damageAmount)
        {
            if (player == null || target == null) return;
            if (player.NetState == null) return;

            //TEST: Figure Out How Much Damage Armor is Reducing for Melee Hits

            if (melee)
            {
                if (player.m_ShowMeleeDamage == DamageDisplayMode.PrivateMessage)
                    player.SendMessage(player.PlayerMeleeDamageTextHue, "You attack " + target.Name + " for " + damageAmount.ToString() + " damage.");

                if (player.m_ShowMeleeDamage == DamageDisplayMode.PrivateOverhead)
                    target.PrivateOverheadMessage(MessageType.Regular, player.PlayerMeleeDamageTextHue, false, "-" + damageAmount.ToString(), player.NetState);
            }

            else
            {
                if (player.m_ShowSpellDamage == DamageDisplayMode.PrivateMessage)
                    player.SendMessage(player.PlayerSpellDamageTextHue, "Your attack hits " + target.Name + " for " + damageAmount.ToString() + " damage.");

                if (player.m_ShowSpellDamage == DamageDisplayMode.PrivateOverhead)
                    target.PrivateOverheadMessage(MessageType.Regular, player.PlayerSpellDamageTextHue, false, "-" + damageAmount.ToString(), player.NetState);
            }
        }        

        public static int GetWindItemId(Direction direction, bool small)
        {
            int windItemId = 0;

            if (small)
            {
                switch (direction)
                {
                    case Direction.North: windItemId = 8099; break;
                    case Direction.Right: windItemId = 8099; break;

                    case Direction.West: windItemId = 8104; break;
                    case Direction.Up: windItemId = 8104; break;

                    case Direction.East: windItemId = 8109; break;
                    case Direction.Down: windItemId = 8109; break;

                    case Direction.South: windItemId = 8114; break;
                    case Direction.Left: windItemId = 8114; break;
                }
            }

            else
            {
                switch (direction)
                {
                    case Direction.North: windItemId = 8119; break;
                    case Direction.Right: windItemId = 8119; break;

                    case Direction.West: windItemId = 8124; break;
                    case Direction.Up: windItemId = 8124; break;

                    case Direction.East: windItemId = 8129; break;
                    case Direction.Down: windItemId = 8129; break;

                    case Direction.South: windItemId = 8134; break;
                    case Direction.Left: windItemId = 8134; break;
                }
            }

            return windItemId;
        }

        public class UOACZSkillMod : DefaultSkillMod
        {
            public UOACZSkillMod(SkillName skill, bool relative, double value): base(skill, false, value)
            {
            }

            public override bool CheckCondition()
            {
                bool expired = base.CheckCondition();

                return expired;
            }
        }

        public static void PlayerRefreshTick(PlayerMobile player)
        {
            RefreshAllGumps(player);
        }

        public static void PlayerTick(PlayerMobile player)
        {
            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            switch (player.m_UOACZAccountEntry.ActiveProfile)
            {
                case UOACZAccountEntry.ActiveProfileType.Human:
                    player.m_UOACZAccountEntry.HumanProfile.TotalMinutesAlive++;
                    player.m_UOACZAccountEntry.TotalTimeSpentAsHuman++;

                    if (player.m_UOACZAccountEntry.HumanProfile.TotalMinutesAlive > player.m_UOACZAccountEntry.LongestTimeAsHuman)
                        player.m_UOACZAccountEntry.LongestTimeAsHuman = player.m_UOACZAccountEntry.HumanProfile.TotalMinutesAlive;

                    if (player.m_UOACZAccountEntry.HumanProfile.TotalMinutesAlive > player.m_UOACZAccountEntry.LongestTimeAsHumanWithoutADeath && player.m_UOACZAccountEntry.HumanProfile.Deaths == 0)
                        player.m_UOACZAccountEntry.LongestTimeAsHumanWithoutADeath = player.m_UOACZAccountEntry.HumanProfile.TotalMinutesAlive;
                    
                    if (player.m_UOACZAccountEntry.HumanProfile.Deaths == 0 && player.m_UOACZAccountEntry.HumanProfile.TotalMinutesAlive > 180)
                        AchievementSystemImpl.Instance.TickProgressMulti(player, AchievementTriggers.Trigger_UOACZHumanSurvival1, 1);

                    if (player.m_UOACZAccountEntry.HumanProfile.Deaths == 0 && player.m_UOACZAccountEntry.HumanProfile.TotalMinutesAlive > 360)
                        AchievementSystemImpl.Instance.TickProgressMulti(player, AchievementTriggers.Trigger_UOACZHumanSurvival2, 1);

                    if (player.m_UOACZAccountEntry.HumanProfile.Deaths == 0 && player.m_UOACZAccountEntry.HumanProfile.TotalMinutesAlive > 540)
                        AchievementSystemImpl.Instance.TickProgressMulti(player, AchievementTriggers.Trigger_UOACZHumanSurvival3, 1);                                       
                break;

                case UOACZAccountEntry.ActiveProfileType.Undead:
                    player.m_UOACZAccountEntry.UndeadProfile.TotalMinutesUndead++;
                    player.m_UOACZAccountEntry.TotalTimeSpentAsUndead++;                    
                break;
            }
        }

        public static void TimerTick(bool minuteOccured)
        {
            foreach (NetState state in NetState.Instances)
            {
                Mobile mobile = state.Mobile;
                PlayerMobile player = mobile as PlayerMobile;

                if (player == null)
                    continue;

                if (UOACZSystem.IsUOACZValidMobile(player))
                {
                    if (minuteOccured)
                        PlayerTick(player);

                    PlayerRefreshTick(player);
                }
            }
        }
    }

    public class ObjectivesHotbarGump : Gump
    {
        UOACZAccountEntry m_AccountEntry;
        PlayerMobile player;

        public ObjectivesHotbarGump(PlayerMobile player)
            : base(10, 10)
        {
            if (player == null) return;
            if (player.Deleted) return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int startY = 20;

            int textHue = UOACZSystem.whiteTextHue;

            int humanObjectivesCompleted = 0;
            int undeadObjectivesCompleted = 0;

            int humanTotalObjectives = UOACZSystem.ObjectivesNeededForVictory;
            int undeadTotalObjectives = UOACZSystem.ObjectivesNeededForVictory;

            bool humanObjective1Completed = false;
            bool humanObjective2Completed = false;
            bool humanObjective3Completed = false;
            bool humanObjective4Completed = false;
            bool humanObjective5Completed = false;

            bool undeadObjective1Completed = false;
            bool undeadObjective2Completed = false;
            bool undeadObjective3Completed = false;
            bool undeadObjective4Completed = false;
            bool undeadObjective5Completed = false;

            string humanObjective1Progress = "0%";
            string humanObjective2Progress = "0%";
            string humanObjective3Progress = "0%";
            string humanObjective4Progress = "0%";
            string humanObjective5Progress = "0%";

            string undeadObjective1Progress = "0%";
            string undeadObjective2Progress = "0%";
            string undeadObjective3Progress = "0%";
            string undeadObjective4Progress = "0%";
            string undeadObjective5Progress = "0%";

            double progress = 0;

            //Human Objective Progress
            if (UOACZPersistance.m_HumanObjective1Target != 0)
            {
                progress = UOACZPersistance.m_HumanObjective1 / UOACZPersistance.m_HumanObjective1Target;
                humanObjective1Progress = Utility.CreateDecimalPercentageString(progress, 1);

                if (UOACZPersistance.m_HumanObjective1 == UOACZPersistance.m_HumanObjective1Target)
                {
                    humanObjective1Completed = true;
                    humanObjectivesCompleted++;
                }
            }

            if (UOACZPersistance.m_HumanObjective2Target != 0)
            {
                progress = UOACZPersistance.m_HumanObjective2 / UOACZPersistance.m_HumanObjective2Target;
                humanObjective2Progress = Utility.CreateDecimalPercentageString(progress, 1);

                if (UOACZPersistance.m_HumanObjective2 == UOACZPersistance.m_HumanObjective2Target)
                {
                    humanObjective2Completed = true;
                    humanObjectivesCompleted++;
                }
            }

            if (UOACZPersistance.m_HumanObjective3Target != 0)
            {
                progress = UOACZPersistance.m_HumanObjective3 / UOACZPersistance.m_HumanObjective3Target;
                humanObjective3Progress = Utility.CreateDecimalPercentageString(progress, 1);

                if (UOACZPersistance.m_HumanObjective3 == UOACZPersistance.m_HumanObjective3Target)
                {
                    humanObjective3Completed = true;
                    humanObjectivesCompleted++;
                }
            }

            if (UOACZPersistance.m_HumanObjective4Target != 0)
            {
                progress = UOACZPersistance.m_HumanObjective4 / UOACZPersistance.m_HumanObjective4Target;
                humanObjective4Progress = Utility.CreateDecimalPercentageString(progress, 1);

                if (UOACZPersistance.m_HumanObjective4 == UOACZPersistance.m_HumanObjective4Target)
                {
                    humanObjective4Completed = true;
                    humanObjectivesCompleted++;
                }
            }

            if (UOACZPersistance.m_HumanObjective5Target != 0)
            {
                progress = UOACZPersistance.m_HumanObjective5 / UOACZPersistance.m_HumanObjective5Target;
                humanObjective5Progress = Utility.CreateDecimalPercentageString(progress, 1);

                if (UOACZPersistance.m_HumanObjective5 == UOACZPersistance.m_HumanObjective5Target)
                {
                    humanObjective5Completed = true;
                    humanObjectivesCompleted++;
                }
            }

            if (humanObjectivesCompleted > humanTotalObjectives)
                humanObjectivesCompleted = humanTotalObjectives;

            //Undead Objectives
            if (UOACZPersistance.m_UndeadObjective1Target != 0)
            {
                progress = UOACZPersistance.m_UndeadObjective1 / UOACZPersistance.m_UndeadObjective1Target;
                undeadObjective1Progress = Utility.CreateDecimalPercentageString(progress, 1);

                if (UOACZPersistance.m_UndeadObjective1 == UOACZPersistance.m_UndeadObjective1Target)
                {
                    undeadObjective1Completed = true;
                    undeadObjectivesCompleted++;
                }
            }

            if (UOACZPersistance.m_UndeadObjective2Target != 0)
            {
                progress = UOACZPersistance.m_UndeadObjective2 / UOACZPersistance.m_UndeadObjective2Target;
                undeadObjective2Progress = Utility.CreateDecimalPercentageString(progress, 1);

                if (UOACZPersistance.m_UndeadObjective2 == UOACZPersistance.m_UndeadObjective2Target)
                {
                    undeadObjective2Completed = true;
                    undeadObjectivesCompleted++;
                }
            }

            if (UOACZPersistance.m_UndeadObjective3Target != 0)
            {
                progress = UOACZPersistance.m_UndeadObjective3 / UOACZPersistance.m_UndeadObjective3Target;
                undeadObjective3Progress = Utility.CreateDecimalPercentageString(progress, 1);

                if (UOACZPersistance.m_UndeadObjective3 == UOACZPersistance.m_UndeadObjective3Target)
                {
                    undeadObjective3Completed = true;
                    undeadObjectivesCompleted++;
                }
            }

            if (UOACZPersistance.m_UndeadObjective4Target != 0)
            {
                progress = UOACZPersistance.m_UndeadObjective4 / UOACZPersistance.m_UndeadObjective4Target;
                undeadObjective4Progress = Utility.CreateDecimalPercentageString(progress, 1);

                if (UOACZPersistance.m_UndeadObjective4 == UOACZPersistance.m_UndeadObjective4Target)
                {
                    undeadObjective4Completed = true;
                    undeadObjectivesCompleted++;
                }
            }

            if (UOACZPersistance.m_UndeadObjective5Target != 0)
            {
                progress = UOACZPersistance.m_UndeadObjective5 / UOACZPersistance.m_UndeadObjective5Target;
                undeadObjective5Progress = Utility.CreateDecimalPercentageString(progress, 1);

                if (UOACZPersistance.m_UndeadObjective5 == UOACZPersistance.m_UndeadObjective5Target)
                {
                    undeadObjective5Completed = true;
                    undeadObjectivesCompleted++;
                }
            }

            if (undeadObjectivesCompleted > undeadTotalObjectives)
                undeadObjectivesCompleted = undeadTotalObjectives;

            int spacing = 18;
            int arrowX = 0;
            int leftX = 25;
            int rightX = 145;

            AddButton(arrowX, startY + 2, 2118, 2117, 3, GumpButtonType.Reply, 0);
            AddLabel(leftX, startY, UOACZSystem.yellowTextHue, "Overview / Teams");

            startY += spacing;
            startY += spacing;

            if (player.m_UOACZAccountEntry.HumanObjectivesExpanded)
                AddButton(arrowX, startY + 5, 5602, 5602, 1, GumpButtonType.Reply, 0);

            else
                AddButton(arrowX, startY + 5, 5601, 5601, 1, GumpButtonType.Reply, 0);

            AddLabel(leftX, startY, 1164, "Human Objectives (" + humanObjectivesCompleted.ToString() + "/" + humanTotalObjectives.ToString() + ")");

            startY += spacing;

            if (player.m_UOACZAccountEntry.HumanObjectivesExpanded)
            {
                if (humanObjective1Completed)
                    textHue = UOACZSystem.greenTextHue;
                else
                    textHue = UOACZSystem.whiteTextHue;

                AddLabel(leftX, startY, textHue, "Destroy Corruption");
                AddLabel(leftX + 10, startY + spacing, textHue, "Sourcestone");
                AddLabel(rightX, startY, textHue, humanObjective1Progress);

                startY += spacing;
                startY += spacing;

                if (humanObjective2Completed)
                    textHue = UOACZSystem.greenTextHue;
                else
                    textHue = UOACZSystem.whiteTextHue;

                AddLabel(leftX, startY, textHue, "Build the Outpost");
                AddLabel(rightX, startY, textHue, humanObjective2Progress);

                startY += spacing;

                if (humanObjective3Completed)
                    textHue = UOACZSystem.greenTextHue;
                else
                    textHue = UOACZSystem.whiteTextHue;

                AddLabel(leftX, startY, textHue, "Gather Bones");
                AddLabel(rightX, startY, textHue, humanObjective3Progress);

                startY += spacing;

                if (humanObjective4Completed)
                    textHue = UOACZSystem.greenTextHue;
                else
                    textHue = UOACZSystem.whiteTextHue;

                if (UOACZPersistance.UndeadChampion == null && !humanObjective4Completed)
                {
                    int minutesUntilSpawn = UOACZSystem.UndeadChampionSpawnMinutes - UOACZPersistance.m_MinutesActive;
                    string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, DateTime.UtcNow + TimeSpan.FromMinutes(minutesUntilSpawn), true, false, true, true, false);

                    AddLabel(leftX, startY, textHue, "Undead Champion");
                    AddLabel(rightX - 15, startY, textHue, "(" + timeRemaining + ")");
                }

                else
                {
                    AddLabel(leftX, startY, textHue, "Slay Undead Champ");
                    AddLabel(rightX, startY, textHue, humanObjective4Progress);
                }

                startY += spacing;

                if (humanObjective5Completed)
                    textHue = UOACZSystem.greenTextHue;
                else
                    textHue = UOACZSystem.whiteTextHue;

                if (UOACZPersistance.UndeadBoss == null && !humanObjective5Completed)
                {
                    int minutesUntilSpawn = UOACZSystem.UndeadBossSpawnMinutes - UOACZPersistance.m_MinutesActive;
                    string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, DateTime.UtcNow + TimeSpan.FromMinutes(minutesUntilSpawn), true, false, true, true, false);

                    AddLabel(leftX, startY, textHue, "Undead Boss");
                    AddLabel(rightX - 15, startY, textHue, "(" + timeRemaining + ")");
                }

                else
                {
                    AddLabel(leftX, startY, textHue, "Slay Undead Boss");
                    AddLabel(rightX, startY, textHue, humanObjective5Progress);
                }

                startY += spacing;
            }

            startY += spacing;

            if (player.m_UOACZAccountEntry.UndeadObjectivesExpanded)
                AddButton(arrowX, startY + 5, 5602, 5602, 2, GumpButtonType.Reply, 0);

            else
                AddButton(arrowX, startY + 5, 5601, 5601, 2, GumpButtonType.Reply, 0);

            AddLabel(leftX, startY, 2115, "Undead Objectives (" + undeadObjectivesCompleted.ToString() + "/" + undeadTotalObjectives.ToString() + ")");

            startY += spacing;

            if (player.m_UOACZAccountEntry.UndeadObjectivesExpanded)
            {
                if (undeadObjective1Completed)
                    textHue = UOACZSystem.greenTextHue;
                else
                    textHue = UOACZSystem.whiteTextHue;

                AddLabel(leftX, startY, textHue, "Destroy Stockpiles");
                AddLabel(rightX, startY, textHue, undeadObjective1Progress);

                startY += spacing;

                if (undeadObjective2Completed)
                    textHue = UOACZSystem.greenTextHue;
                else
                    textHue = UOACZSystem.whiteTextHue;

                AddLabel(leftX, startY, textHue, "Kill Town Civilians");
                AddLabel(rightX, startY, textHue, undeadObjective2Progress);

                startY += spacing;

                if (undeadObjective3Completed)
                    textHue = UOACZSystem.greenTextHue;
                else
                    textHue = UOACZSystem.whiteTextHue;

                AddLabel(leftX, startY, textHue, "Spread Corruption");
                AddLabel(rightX, startY, textHue, undeadObjective3Progress);

                startY += spacing;

                if (undeadObjective4Completed)
                    textHue = UOACZSystem.greenTextHue;
                else
                    textHue = UOACZSystem.whiteTextHue;

                if (UOACZPersistance.HumanChampion == null && !undeadObjective4Completed)
                {
                    int minutesUntilSpawn = UOACZSystem.HumanChampionSpawnMinutes - UOACZPersistance.m_MinutesActive;
                    string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, DateTime.UtcNow + TimeSpan.FromMinutes(minutesUntilSpawn), true, false, true, true, false);

                    AddLabel(leftX, startY, textHue, "Human Champion");
                    AddLabel(rightX - 15, startY, textHue, "(" + timeRemaining + ")");
                }

                else
                {
                    AddLabel(leftX, startY, textHue, "Slay Human Champ");
                    AddLabel(rightX, startY, textHue, undeadObjective4Progress);
                }

                startY += spacing;

                if (undeadObjective5Completed)
                    textHue = UOACZSystem.greenTextHue;
                else
                    textHue = UOACZSystem.whiteTextHue;

                if (UOACZPersistance.HumanBoss == null && !undeadObjective5Completed)
                {
                    int minutesUntilSpawn = UOACZSystem.HumanBossSpawnMinutes - UOACZPersistance.m_MinutesActive;
                    string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, DateTime.UtcNow + TimeSpan.FromMinutes(minutesUntilSpawn), true, false, true, true, false);

                    AddLabel(leftX, startY, textHue, "Human Boss");
                    AddLabel(rightX - 15, startY, textHue, "(" + timeRemaining + ")");
                }

                else
                {
                    AddLabel(leftX, startY, textHue, "Slay Human Boss");
                    AddLabel(rightX, startY, textHue, undeadObjective5Progress);
                }
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (sender == null)
                return;

            PlayerMobile player = sender.Mobile as PlayerMobile;

            if (player == null) return;
            if (player.Deleted) return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            bool closeGump = true;

            switch (info.ButtonID)
            {
                //Human Objectives
                case 1:
                    player.m_UOACZAccountEntry.HumanObjectivesExpanded = !player.m_UOACZAccountEntry.HumanObjectivesExpanded;
                    closeGump = false;
                break;

                //Undead Objectives
                case 2:
                    player.m_UOACZAccountEntry.UndeadObjectivesExpanded = !player.m_UOACZAccountEntry.UndeadObjectivesExpanded;
                    closeGump = false;
                break;

                //Overview / Teams
                case 3:
                    player.CloseGump(typeof(UOACZOverviewGump));
                    player.SendGump(new UOACZOverviewGump(player));

                    player.SendSound(UOACZSystem.selectionSound);

                    closeGump = false;
                break;
            }

            if (!closeGump)
            {
                player.CloseGump(typeof(ObjectivesHotbarGump));
                player.SendGump(new ObjectivesHotbarGump(player));
            }
        }
    }
}
