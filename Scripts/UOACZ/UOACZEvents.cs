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

using Server.Accounting;
using System.Linq;
using Server.Custom;

namespace Server
{
    public static class UOACZEvents
    {
        public static void MinuteTick()
        {
            CalculateBalance();

            foreach (UOACZBaseHuman civilian in UOACZBaseHuman.m_Creatures)
            {
                if (civilian.Deleted) continue;
                if (!civilian.Alive) continue;
                if (civilian.InWilderness) continue;
                if (civilian.StockpileContributionType == UOACZSystem.StockpileContributionType.None) continue;
                if (civilian.StockpileContributionScalar == 0) continue;

                if (UOACZPersistance.m_StockpileProgress.ContainsKey(civilian.StockpileContributionType))
                    UOACZPersistance.m_StockpileProgress[civilian.StockpileContributionType] += civilian.StockpileContributionScalar;

                else                
                    UOACZPersistance.m_StockpileProgress.Add(civilian.StockpileContributionType, civilian.StockpileContributionScalar);
            }
            
            if ((UOACZPersistance.m_MinutesActive % UOACZSystem.StockpileDisbursementMinutes) == 0)
                StockpileEvent();

            if ((UOACZPersistance.m_MinutesActive % UOACZSystem.TunnelOpeningMinutes) == 0)
                TunnelOpeningEvent();

            if ((UOACZPersistance.m_MinutesActive % UOACZSystem.SpawnAtPlayerMinutes) == 0)
                SpawnAtPlayerEvent();

            if ((UOACZPersistance.m_MinutesActive % UOACZSystem.HungerAndThirstLossEventMinutes) == 0)
            {
                foreach (NetState state in NetState.Instances)
                {
                    Mobile mobile = state.Mobile;
                    PlayerMobile player = mobile as PlayerMobile;

                    if (player == null)
                        continue;

                    if (UOACZRegion.ContainsMobile(player))
                    {
                        if (player.AccessLevel > AccessLevel.Player)
                            continue;

                        switch (player.m_UOACZAccountEntry.ActiveProfile)
                        {
                            case UOACZAccountEntry.ActiveProfileType.Human:
                                player.SendMessage(UOACZSystem.purpleTextHue, "Time passes...");
                                player.SendSound(0x663);

                                UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.Hunger, UOACZSystem.HungerLossEventAmount, true);
                                UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.Thirst, UOACZSystem.ThirstLossEventAmount, true);
                            break;
                        }
                    }
                }                
            }

            if ((UOACZPersistance.m_MinutesActive % UOACZSystem.UpgradeDistributionMinutes) == 0)
            {
                foreach (UOACZAccountEntry accountEntry in UOACZPersistance.m_UOACZAccountEntries)
                {
                    if (accountEntry == null) continue;
                    if (accountEntry.Deleted) continue;

                    accountEntry.HumanProfile.UpgradePoints++;
                    accountEntry.HumanProfile.SurvivalPoints++;

                    accountEntry.UndeadProfile.UpgradePoints++;
                    accountEntry.UndeadProfile.CorruptionPoints++;
                }

                foreach (NetState state in NetState.Instances)
                {
                    Mobile mobile = state.Mobile;
                    PlayerMobile player = mobile as PlayerMobile;

                    if (player == null)
                        continue;

                    if (UOACZRegion.ContainsMobile(player))
                    {
                        if (player.AccessLevel > AccessLevel.Player)
                            continue;

                        switch(player.m_UOACZAccountEntry.CurrentTeam)
                        {
                            case UOACZAccountEntry.ActiveProfileType.Human:
                                UOACZSystem.BroadcastStatChange(player, UOACZSystem.UOACZStatType.HumanUpgradePoints, 1);
                                UOACZSystem.BroadcastStatChange(player, UOACZSystem.UOACZStatType.SurvivalPoints, 1);                                
                            break;

                            case UOACZAccountEntry.ActiveProfileType.Undead:
                                UOACZSystem.BroadcastStatChange(player, UOACZSystem.UOACZStatType.UndeadUpgradePoints, 1);
                                UOACZSystem.BroadcastStatChange(player, UOACZSystem.UOACZStatType.CorruptionPoints, 1);                                
                            break;
                        }
                    }
                }
            }

            if (UOACZPersistance.m_SpawnWaveProgress + UOACZSystem.RespawnFirstWaveWarningMinutes == UOACZSystem.RespawnWaveMinutes)            
                RespawnWaveWarning(true);            

            if (UOACZPersistance.m_SpawnWaveProgress + UOACZSystem.RespawnSecondWaveWarningMinutes == UOACZSystem.RespawnWaveMinutes)            
                RespawnWaveWarning(false); 
           
            if (UOACZPersistance.m_SpawnWaveProgress >= UOACZSystem.RespawnWaveMinutes)
            {
                UOACZPersistance.m_SpawnWaveProgress = 0;
                RespawnWave();
            }

            if (UOACZPersistance.m_MinutesActive == UOACZSystem.HumanChampionSpawnMinutes)
            {   
                SpawnHumanChampion();
            }

            if (UOACZPersistance.m_MinutesActive == UOACZSystem.HumanBossSpawnMinutes)
            {
                SpawnHumanBoss();
            }

            if (UOACZPersistance.m_MinutesActive == UOACZSystem.UndeadChampionSpawnMinutes)
            {
                SpawnUndeadChampion();
            }

            if (UOACZPersistance.m_MinutesActive == UOACZSystem.UndeadBossSpawnMinutes)
            {
                SpawnUndeadBoss();
            }                
        }

        public static void CalculateBalance()
        {
            double humanPopulationFactor = 0;
            double undeadPopulationFactor = 0;

            double humanScoreFactor = 0;
            double undeadScoreFactor = 0;

            double humanObjectivesFactor = 0;
            double undeadObjectivesFactor = 0;

            int totalPlayers = 0;
            int totalHumanPlayers = 0;
            int totalUndeadPlayers = 0;
            int activeHumanPlayers = 0;
            int activeUndeadPlayers = 0;
            int totalScore = 0;
            int totalHumanScore = 0;
            int totalUndeadScore = 0;
            int avgHumanScore = 0;
            int avgUndeadScore = 0;

            //Population and Scores
            foreach (NetState state in NetState.Instances)
            {
                Mobile mobile = state.Mobile;
                PlayerMobile player = mobile as PlayerMobile;

                if (player == null) continue;
                if (player.AccessLevel > AccessLevel.Player) continue;                

                if (player.IsUOACZHuman)
                {
                    totalPlayers++;
                    totalHumanPlayers++;

                    if (player.m_UOACZAccountEntry.CurrentSessionHumanScore >= UOACZSystem.MinScoreToQualifyAsParticipant)
                        activeHumanPlayers++;

                    totalScore += player.m_UOACZAccountEntry.CurrentSessionHumanScore;
                    totalHumanScore += player.m_UOACZAccountEntry.CurrentSessionHumanScore;                   
                }

                if (player.IsUOACZUndead)
                {
                    totalPlayers++;
                    totalUndeadPlayers++;

                    if (player.m_UOACZAccountEntry.CurrentSessionUndeadScore >= UOACZSystem.MinScoreToQualifyAsParticipant)
                        activeUndeadPlayers++;

                    totalScore += player.m_UOACZAccountEntry.CurrentSessionUndeadScore;
                    totalUndeadScore += player.m_UOACZAccountEntry.CurrentSessionUndeadScore;
                }
            }

            //Objectives
            double HumanObjective1Progress = UOACZPersistance.m_HumanObjective1 / UOACZPersistance.m_HumanObjective1Target;
            double HumanObjective2Progress = UOACZPersistance.m_HumanObjective2 / UOACZPersistance.m_HumanObjective2Target;
            double HumanObjective3Progress = UOACZPersistance.m_HumanObjective3 / UOACZPersistance.m_HumanObjective3Target;
            double HumanObjective4Progress = UOACZPersistance.m_HumanObjective4 / UOACZPersistance.m_HumanObjective4Target;
            double HumanObjective5Progress = UOACZPersistance.m_HumanObjective5 / UOACZPersistance.m_HumanObjective5Target;

            double HumanTotalProgress = HumanObjective1Progress + HumanObjective2Progress + HumanObjective3Progress + HumanObjective4Progress + HumanObjective5Progress;
                        
            //Console.Write("HumanTotalProgress: " + HumanTotalProgress.ToString() + "\n");

            double UndeadObjective1Progress = UOACZPersistance.m_UndeadObjective1 / UOACZPersistance.m_UndeadObjective1Target;
            double UndeadObjective2Progress = UOACZPersistance.m_UndeadObjective2 / UOACZPersistance.m_UndeadObjective2Target;
            double UndeadObjective3Progress = UOACZPersistance.m_UndeadObjective3 / UOACZPersistance.m_UndeadObjective3Target;
            double UndeadObjective4Progress = UOACZPersistance.m_UndeadObjective4 / UOACZPersistance.m_UndeadObjective4Target;
            double UndeadObjective5Progress = UOACZPersistance.m_UndeadObjective5 / UOACZPersistance.m_UndeadObjective5Target;

            double UndeadTotalProgress = UndeadObjective1Progress + UndeadObjective2Progress + UndeadObjective3Progress + UndeadObjective4Progress + UndeadObjective5Progress;

            //Console.Write("UndeadTotalProgress: " + UndeadTotalProgress.ToString() + "\n");

            double humanCompletionPercent = UOACZSystem.HumanBalanceHandicapBonus + (HumanTotalProgress / 5);
            double undeadCompletionPercent = UOACZSystem.UndeadBalanceHandicapBonus + (UndeadTotalProgress / 5);

            double factorCap = .5;

            #region Population

            int humanPlayerExcess = 0;
            int undeadPlayerExcess = 0;
            
            double totalPopulationScalar = 1;

            if (totalPlayers <= 4)
                totalPopulationScalar = 3;

            else if (totalPlayers <= 8)
                totalPopulationScalar = 1.6;

            else if (totalPlayers <= 12)
                totalPopulationScalar = 1.3;

            else if (totalPlayers <= 16)
                totalPopulationScalar = 1.2;

            else if (totalPlayers <= 20)
                totalPopulationScalar = 1.1;

            else
                totalPopulationScalar = 1.0;
            
            if (totalHumanPlayers > totalUndeadPlayers)
            {
                humanPlayerExcess = totalHumanPlayers - totalUndeadPlayers;
                undeadPopulationFactor = .03 * totalPopulationScalar * (double)humanPlayerExcess;
            }
            
            if (totalUndeadPlayers > totalHumanPlayers)
            {
                undeadPlayerExcess = totalUndeadPlayers - totalHumanPlayers;
                humanPopulationFactor = .03 * totalPopulationScalar * (double)undeadPlayerExcess;
            }

            if (undeadPopulationFactor > factorCap)
                undeadPopulationFactor = factorCap;

            if (humanPopulationFactor > factorCap)
                humanPopulationFactor = factorCap;

            #endregion           

            #region Objectives

            double humanCompletionExcess = 0;
            double undeadCompletionExcess = 0;

            if (humanCompletionPercent > undeadCompletionPercent)
            {
                humanCompletionExcess = humanCompletionPercent - undeadCompletionPercent;
                undeadObjectivesFactor = humanCompletionExcess * .5;
            }

            if (undeadCompletionPercent > humanCompletionPercent)
            {
                undeadCompletionExcess = undeadCompletionPercent - humanCompletionPercent;
                humanObjectivesFactor = undeadCompletionExcess * .5;
            }

            if (undeadObjectivesFactor > factorCap)
                undeadObjectivesFactor = factorCap;

            if (humanObjectivesFactor > factorCap)
                humanObjectivesFactor = factorCap;

            #endregion

            double humanFactorsTotal = humanPopulationFactor + humanScoreFactor + humanObjectivesFactor;
            double undeadFactorsTotal = undeadPopulationFactor + undeadScoreFactor + undeadObjectivesFactor;
                        
            //Console.Write("humanFactorsTotal: " + humanFactorsTotal.ToString() + " humanPopulationFactor: " + humanPopulationFactor.ToString() + " humanScoreFactor: " + humanScoreFactor.ToString() + " humanObjectivesFactor: " + humanObjectivesFactor.ToString() + "\n");
            //Console.Write("undeadFactorsTotal: " + undeadFactorsTotal.ToString() + " undeadPopulationFactor: " + undeadPopulationFactor.ToString() + " undeadScoreFactor: " + undeadScoreFactor.ToString() + " undeadObjectivesFactor: " + undeadObjectivesFactor.ToString() + "\n");

            UOACZPersistance.HumanBalanceScalar = 1.0;
            UOACZPersistance.UndeadBalanceScalar = 1.0;

            if (humanFactorsTotal > undeadFactorsTotal)
                UOACZPersistance.HumanBalanceScalar = 1 + (humanFactorsTotal - undeadFactorsTotal);   

            if (undeadFactorsTotal > humanFactorsTotal)            
                UOACZPersistance.UndeadBalanceScalar = 1 + (undeadFactorsTotal - humanFactorsTotal);                   

            if (UOACZPersistance.HumanBalanceScalar < 1)
                UOACZPersistance.HumanBalanceScalar = 1;

            if (UOACZPersistance.HumanBalanceScalar > 1.5)
                UOACZPersistance.HumanBalanceScalar = 1.5;

            if (UOACZPersistance.UndeadBalanceScalar < 1)
                UOACZPersistance.UndeadBalanceScalar = 1;

            if (UOACZPersistance.UndeadBalanceScalar > 1.5)
                UOACZPersistance.UndeadBalanceScalar = 1.5;
            
            //Console.Write("HumanBalanceScalar: " + UOACZPersistance.HumanBalanceScalar.ToString() + "\n");
            //Console.Write("UndeadBalanceScalar: " + UOACZPersistance.UndeadBalanceScalar.ToString() + "\n");
        }

        public static void StockpileEvent()
        {
            if (UOACZStockpile.m_Instances.Count == 0)
                return;

            foreach (UOACZAccountEntry accountEntry in UOACZPersistance.m_UOACZAccountEntries)
            {
                if (accountEntry == null) continue;
                if (accountEntry.Deleted) continue;
                if (accountEntry.ActiveProfile != UOACZAccountEntry.ActiveProfileType.Human) continue;
                if (accountEntry.HumanProfile.Stockpile == null) continue;
                if (accountEntry.HumanProfile.Stockpile.Deleted) continue;

                bool itemsAdded = false;
                bool itemDeleted = false;

                foreach (KeyValuePair<UOACZSystem.StockpileContributionType, double> pair in UOACZPersistance.m_StockpileProgress)
                {
                    List<Item> m_Items = GetStockpileContribution(pair.Key, pair.Value);

                    if (m_Items.Count == 0)
                        continue;

                    foreach (Item item in m_Items)
                    {
                        itemsAdded = true;

                        if (accountEntry.HumanProfile.Stockpile.TotalItems == accountEntry.HumanProfile.Stockpile.MaxItems)
                        {
                            item.Delete();
                            itemDeleted = true;

                            continue;
                        }

                        if (item is UOACZHumanUpgradeToken)
                        {
                            UOACZHumanUpgradeToken upgradeToken = item as UOACZHumanUpgradeToken;
                            upgradeToken.Player = accountEntry.MostRecentPlayer;
                        }

                        if (item is UOACZSurvivalStone)
                        {
                            UOACZSurvivalStone survivalStone = item as UOACZSurvivalStone;
                            survivalStone.Player = accountEntry.MostRecentPlayer;
                        }
                       
                        accountEntry.HumanProfile.Stockpile.DropItem(item);                        
                    }                    
                }

                if (accountEntry.MostRecentPlayer != null && itemsAdded)
                {
                    if (accountEntry.MostRecentPlayer.NetState != null && accountEntry.MostRecentPlayer.IsUOACZHuman)
                    {
                        if (itemDeleted)
                            accountEntry.MostRecentPlayer.SendMessage(UOACZSystem.greenTextHue, "Civilians in town added items to your stockpile, however there were too many items present in the stockpile to store them all.");
                        
                        else
                            accountEntry.MostRecentPlayer.SendMessage(UOACZSystem.greenTextHue, "Civilians in town have added items to your stockpile.");
                    }
                }
            }

            foreach (NetState state in NetState.Instances)
            {
                Mobile mobile = state.Mobile;
                PlayerMobile player = mobile as PlayerMobile;

                if (player == null)
                    continue;
                
                if (UOACZRegion.ContainsMobile(player) && player.AccessLevel > AccessLevel.Player)
                    player.SendMessage(UOACZSystem.greenTextHue, "Civilians have added items to player's stockpiles.");                
            }

            UOACZPersistance.m_StockpileProgress.Clear();
        }

        public static void TunnelOpeningEvent()
        {
            List<UOACZTunnel> m_TownTunnels = new List<UOACZTunnel>();
            List<UOACZTunnel> m_WildernessTunnels = new List<UOACZTunnel>();           

            foreach (UOACZTunnel tunnel in UOACZTunnel.m_Instances)
            {
                if (!UOACZRegion.ContainsItem(tunnel)) continue;
                if (tunnel.Visible) continue;

                if (tunnel.TunnelType == UOACZTunnel.TunnelLocation.Town)
                    m_TownTunnels.Add(tunnel);

                if (tunnel.TunnelType == UOACZTunnel.TunnelLocation.Wilderness)
                    m_WildernessTunnels.Add(tunnel);
            }

            UOACZTunnel townTunnel;

            if (m_TownTunnels.Count > 0)
            {
                townTunnel = m_TownTunnels[Utility.RandomMinMax(0, m_TownTunnels.Count - 1)];
                townTunnel.Visible = true;
            }

            UOACZTunnel wildernessTunnel;

            if (m_WildernessTunnels.Count > 0)
            {
                wildernessTunnel = m_WildernessTunnels[Utility.RandomMinMax(0, m_WildernessTunnels.Count - 1)];
                wildernessTunnel.Visible = true;
            }
        }

        public static void SpawnAtPlayerEvent()
        {
            Queue m_Queue = new Queue();

            foreach (NetState state in NetState.Instances)
            {
                Mobile mobile = state.Mobile;
                PlayerMobile player = mobile as PlayerMobile;

                if (player == null)
                    continue;

                if (!UOACZSystem.IsUOACZValidMobile(player)) continue;                
                if (!player.IsUOACZHuman) continue;
                if (player.Hidden) continue;

                if (Utility.RandomDouble() <= UOACZSystem.SpawnAtPlayerWeakCreatureChance)
                {
                    int delay = Utility.RandomMinMax(UOACZSystem.SpawnAtPlayerMinDelaySeconds, UOACZSystem.SpawnAtPlayerMaxDelaySeconds);

                    Timer.DelayCall(TimeSpan.FromSeconds(delay), delegate
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                        if (!player.IsUOACZHuman) return;

                        UOACZSystem.SpawnRandomCreature(player, player.Location, player.Map, UOACZPersistance.m_ThreatLevel - 70, 0, 5, false);
                    });
                }

                if (Utility.RandomDouble() <= UOACZSystem.SpawnAtPlayerChance)
                {
                    int delay = Utility.RandomMinMax(UOACZSystem.SpawnAtPlayerMinDelaySeconds, UOACZSystem.SpawnAtPlayerMaxDelaySeconds);

                    Timer.DelayCall(TimeSpan.FromSeconds(delay), delegate
                    {
                        if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                        if (!player.IsUOACZHuman) return;

                        UOACZSystem.SpawnRandomCreature(player, player.Location, player.Map, UOACZPersistance.m_ThreatLevel - 30, 0, 5, false);
                    });
                }
            }
        }

        public static void RespawnWaveWarning(bool firstWarning)
        {
            foreach (NetState state in NetState.Instances)
            {
                Mobile mobile = state.Mobile;
                PlayerMobile player = mobile as PlayerMobile;

                if (player == null)
                    continue;
                
                if (UOACZRegion.ContainsMobile(player))
                {
                    if (firstWarning)
                        player.SendMessage(UOACZSystem.orangeTextHue, "A horde of Undead begin to gather strength...");

                    else
                        player.SendMessage(UOACZSystem.orangeTextHue, "An Undead horde's arrival is eminent...");
                }                
            }
        }

        public static void RespawnWave()
        {
            foreach (NetState state in NetState.Instances)
            {
                Mobile mobile = state.Mobile;
                PlayerMobile player = mobile as PlayerMobile;

                if (player == null)
                    continue;
                
                if (UOACZRegion.ContainsMobile(player))
                    player.SendMessage(UOACZSystem.orangeTextHue, "An Undead horde has arrived!"); 
            }

            foreach (UOACZUndeadSpawner undeadSpawner in UOACZUndeadSpawner.m_Spawners)
            {
                if (!UOACZRegion.ContainsItem(undeadSpawner)) continue;
                if (!undeadSpawner.Wilderness) continue;

                int spawnNeeded = undeadSpawner.GetAvailableSpawnAmount();

                int spawnCap = 3;
                int adjustedSpawnCap = (int)(Math.Floor((spawnCap) * UOACZPersistance.UndeadBalanceScalar));

                if (spawnNeeded > adjustedSpawnCap)
                    spawnNeeded = adjustedSpawnCap;

                if (spawnNeeded > 0)
                {
                    undeadSpawner.PerformSpawns(spawnNeeded);

                    undeadSpawner.m_LastActivity = DateTime.UtcNow;
                    double spawnDelay = undeadSpawner.MinSpawnTime + (Utility.RandomDouble() * (undeadSpawner.MaxSpawnTime - undeadSpawner.MinSpawnTime));
                    undeadSpawner.m_NextActivity = TimeSpan.FromMinutes(spawnDelay);
                }
            }
        }

        public static void SpawnHumanChampion()
        {
            UOACZFirstRanger creature = new UOACZFirstRanger();

            UOACZPersistance.HumanChampion = creature;
            UOACZPersistance.m_UndeadObjective4Target = (double)creature.HitsMax;

            creature.MoveToWorld(UOACZPersistance.HumanChampionLocation, UOACZRegion.Facet);

            foreach (NetState state in NetState.Instances)
            {
                Mobile mobile = state.Mobile;
                PlayerMobile player = mobile as PlayerMobile;

                if (player == null)
                    continue;
                
                if (UOACZRegion.ContainsMobile(player))
                    player.SendMessage(UOACZSystem.orangeTextHue, "The First Ranger has been spotted. (Undead Objective)");                
            }
        }

        public static void SpawnHumanBoss()
        {
            UOACZFortCommander creature = new UOACZFortCommander();

            UOACZPersistance.HumanBoss = creature;
            UOACZPersistance.m_UndeadObjective5Target = (double)creature.HitsMax;

            creature.MoveToWorld(UOACZPersistance.HumanBossLocation, UOACZRegion.Facet);

            foreach (NetState state in NetState.Instances)
            {
                Mobile mobile = state.Mobile;
                PlayerMobile player = mobile as PlayerMobile;

                if (player == null)
                    continue;
                
                if (UOACZRegion.ContainsMobile(player))
                    player.SendMessage(UOACZSystem.orangeTextHue, "The fort commander has arrived. (Undead Objective)");                
            }
        }

        public static void SpawnUndeadChampion()
        {
            UOACZTreeOfDeath creature = new UOACZTreeOfDeath();

            UOACZPersistance.UndeadChampion = creature;
            UOACZPersistance.m_HumanObjective4Target = (double)creature.HitsMax;

            creature.MoveToWorld(UOACZPersistance.UndeadChampionLocation, UOACZRegion.Facet);

            foreach (NetState state in NetState.Instances)
            {
                Mobile mobile = state.Mobile;
                PlayerMobile player = mobile as PlayerMobile;

                if (player == null)
                    continue;

                if (UOACZRegion.ContainsMobile(player))
                    player.SendMessage(UOACZSystem.orangeTextHue, "The Tree of Death stirs. (Human Objective)");                
            }
        }

        public static void SpawnUndeadBoss()
        {
            UOACZTheGatekeeper creature = new UOACZTheGatekeeper();

            UOACZPersistance.UndeadBoss = creature;
            UOACZPersistance.m_HumanObjective5Target = (double)creature.HitsMax;

            creature.MoveToWorld(UOACZPersistance.UndeadBossLocation, UOACZRegion.Facet);

            foreach (NetState state in NetState.Instances)
            {
                Mobile mobile = state.Mobile;
                PlayerMobile player = mobile as PlayerMobile;

                if (player == null)
                    continue;
                
                if (UOACZRegion.ContainsMobile(player))
                    player.SendMessage(UOACZSystem.orangeTextHue, "The Gatekeeper has risen. (Human Objective)");                
            }
        }        

        //Human Objectives
        public static void SourceOfCorruptionDamaged(bool destroyed)
        {
            if (!UOACZPersistance.Active)
                return;           

            if (destroyed)
            {
                UOACZPersistance.m_HumanObjective1 = UOACZPersistance.m_HumanObjective1Target;

                foreach (NetState state in NetState.Instances)
                {
                    Mobile mobile = state.Mobile;
                    PlayerMobile player = mobile as PlayerMobile;

                    if (player == null)
                        continue;

                    if (UOACZRegion.ContainsMobile(player))
                    {
                        player.SendMessage(UOACZSystem.orangeTextHue, "Human Objective Complete: Destroy Corruption Sourcestone.");

                        if (player.IsUOACZHuman)
                        {
                            UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.HumanUpgradePoints, UOACZSystem.HumanObjective1UpgradePointsGranted, true);
                            UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.SurvivalPoints, 5, true);

                            player.SendSound(UOACZSystem.earnHumanUpgradeSound);
                            player.SendMessage(UOACZSystem.greenTextHue, "You have received upgrade points and survival points for completion of an objective.");
                        }
                    }
                }
                
                UOACZSystem.ObjectiveCompleted(true);
            }

            else
            {
                List<UOACZCorruptionSourcestone> m_Sourcestones = UOACZCorruptionSourcestone.GetActiveInstances();

                if (m_Sourcestones.Count == 0)
                    return;

                UOACZCorruptionSourcestone sourcestone = m_Sourcestones[0];
                UOACZPersistance.m_HumanObjective1 = (double)(sourcestone.MaxHitPoints - sourcestone.HitPoints);
            }
        }

        public static void RepairOutpostComponent()
        {
            if (!UOACZPersistance.Active) return;
            if (UOACZPersistance.m_HumanObjective2 == UOACZPersistance.m_HumanObjective2Target) return;

            int repairedComponents = 0;

            foreach (UOACZBreakableStatic breakableStatic in UOACZPersistance.m_OutpostComponents)
            {
                if (breakableStatic.DamageState == BreakableStatic.DamageStateType.Normal)
                    repairedComponents++;
            }

            UOACZPersistance.m_HumanObjective2 = (double)repairedComponents;

            if (UOACZPersistance.m_HumanObjective2 == UOACZPersistance.m_HumanObjective2Target)
            {
                foreach (NetState state in NetState.Instances)
                {
                    Mobile mobile = state.Mobile;
                    PlayerMobile player = mobile as PlayerMobile;

                    if (player == null)
                        continue;

                    if (UOACZRegion.ContainsMobile(player))
                    {
                        player.SendMessage(UOACZSystem.orangeTextHue, "Human Objective Complete: Build the Outpost.");

                        if (player.IsUOACZHuman)
                        {
                            UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.HumanUpgradePoints, UOACZSystem.HumanObjective2UpgradePointsGranted, true);

                            Backpack backpack = new Backpack();
                            backpack.Hue = 2500;

                            Item item;

                            int reagentAmount = 4;

                            BlackPearl blackPearl = new BlackPearl();
                            blackPearl.Amount = reagentAmount;
                            backpack.DropItem(blackPearl);

                            Bloodmoss bloodmoss = new Bloodmoss();
                            bloodmoss.Amount = reagentAmount;
                            backpack.DropItem(bloodmoss);

                            MandrakeRoot mandrakeroot = new MandrakeRoot();
                            mandrakeroot.Amount = reagentAmount;
                            backpack.DropItem(mandrakeroot);

                            Garlic garlic = new Garlic();
                            garlic.Amount = reagentAmount;
                            backpack.DropItem(garlic);

                            Ginseng ginseng = new Ginseng();
                            ginseng.Amount = reagentAmount;
                            backpack.DropItem(ginseng);

                            SpidersSilk spiderssilk = new SpidersSilk();
                            spiderssilk.Amount = reagentAmount;
                            backpack.DropItem(spiderssilk);

                            SulfurousAsh sulfurousash = new SulfurousAsh();
                            sulfurousash.Amount = reagentAmount;
                            backpack.DropItem(sulfurousash);

                            Nightshade nightshade = new Nightshade();
                            nightshade.Amount = reagentAmount;
                            backpack.DropItem(nightshade);

                            item = new Arrow(25);
                            backpack.DropItem(item);

                            item = new Bolt(25);
                            backpack.DropItem(item);

                            item = new Cloth(25);
                            backpack.DropItem(item);

                            item = new IronIngot(25);
                            backpack.DropItem(item);

                            item = new Leather(25);
                            backpack.DropItem(item);

                            item = new Log(25);
                            backpack.DropItem(item);

                            item = new UOACZTorch();
                            backpack.DropItem(item);

                            item = new UOACZOilFlask();
                            backpack.DropItem(item);

                            item = new UOACZThrowingNet();
                            backpack.DropItem(item);                            

                            item = new UOACZVegetableStew();
                            backpack.DropItem(item);

                            UOACZBottleOfAle bottleOfAle = new UOACZBottleOfAle();
                            bottleOfAle.Charges = 4;
                            backpack.DropItem(bottleOfAle);

                            backpack.Name = "outpost supplies backpack";

                            if (player.Backpack != null)
                                player.Backpack.DropItem(backpack);

                            player.SendSound(UOACZSystem.earnHumanUpgradeSound);
                            player.SendMessage(UOACZSystem.greenTextHue, "You have received upgrade points and a backpack of resources for completion of an objective.");
                        }
                    }
                }
                
                UOACZSystem.ObjectiveCompleted(true);
            }
        }

        public static void CollectBones()
        {
            if (!UOACZPersistance.Active) return;
            if (UOACZPersistance.m_HumanObjective3 == UOACZPersistance.m_HumanObjective3Target) return;

            List<UOACZBoneBox> m_BoneBoxes = UOACZBoneBox.GetActiveInstances();

            if (m_BoneBoxes.Count == 0)
                return;

            UOACZBoneBox m_BoneBox = m_BoneBoxes[0];

            UOACZPersistance.m_HumanObjective3 = (double)m_BoneBox.BoneCount;

            if (UOACZPersistance.m_HumanObjective3 == UOACZPersistance.m_HumanObjective3Target)
            {
                foreach (NetState state in NetState.Instances)
                {
                    Mobile mobile = state.Mobile;
                    PlayerMobile player = mobile as PlayerMobile;

                    if (player == null)
                        continue;

                    if (UOACZRegion.ContainsMobile(player))
                    {
                        player.SendMessage(UOACZSystem.orangeTextHue, "Human Objective Complete: Gather Bones for Warding Ritual.");

                        if (player.IsUOACZHuman)
                        {
                            player.SendSound(UOACZSystem.earnHumanUpgradeSound);

                            UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.HumanUpgradePoints, UOACZSystem.HumanObjective3UpgradePointsGranted, true);
                            
                            //TEST: Create New Reward
                            //UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.Humanity, 20, true);    
                            //player.SendMessage(UOACZSystem.greenTextHue, "You have received upgrade points and regained humanity for completion of an objective.");
                        }
                    }
                }
                
                UOACZSystem.ObjectiveCompleted(true);
            }
        }

        public static void UndeadChampionDamaged(bool killed)
        {
            if (!UOACZPersistance.Active)
                return;

            if (killed)
            {
                UOACZPersistance.m_HumanObjective4 = UOACZPersistance.m_HumanObjective4Target;

                foreach (NetState state in NetState.Instances)
                {
                    Mobile mobile = state.Mobile;
                    PlayerMobile player = mobile as PlayerMobile;

                    if (player == null)
                        continue;

                    if (UOACZRegion.ContainsMobile(player))
                    {
                        player.SendMessage(UOACZSystem.orangeTextHue, "Human Objective Complete: Slay Undead Champion.");

                        if (player.IsUOACZHuman)
                        {
                            UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.HumanUpgradePoints, UOACZSystem.HumanObjective4UpgradePointsGranted, true);                           

                            player.SendSound(UOACZSystem.earnHumanUpgradeSound);
                            player.SendMessage(UOACZSystem.greenTextHue, "You have received upgrade points for completion of an objective.");
                        }
                    }
                }
                
                UOACZSystem.ObjectiveCompleted(true);
            }

            else
            {
                BaseCreature creature = UOACZPersistance.UndeadChampion;

                if (creature == null)
                    return;

                UOACZPersistance.m_HumanObjective4 = (double)(creature.HitsMax - creature.Hits);
            }
        }

        public static void UndeadBossDamaged(bool killed)
        {
            if (!UOACZPersistance.Active)
                return;

            if (killed)
            {
                UOACZPersistance.m_HumanObjective5 = UOACZPersistance.m_HumanObjective5Target;

                foreach (NetState state in NetState.Instances)
                {
                    Mobile mobile = state.Mobile;
                    PlayerMobile player = mobile as PlayerMobile;

                    if (player == null)
                        continue;

                    if (UOACZRegion.ContainsMobile(player))
                    {
                        player.SendMessage(UOACZSystem.orangeTextHue, "Human Objective Complete: Slay Undead Boss.");

                        if (player.IsUOACZHuman)
                        {
                            UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.HumanUpgradePoints, UOACZSystem.HumanObjective5UpgradePointsGranted, true);

                            player.SendSound(UOACZSystem.earnHumanUpgradeSound);
                            player.SendMessage(UOACZSystem.greenTextHue, "You have received upgrade points for completion of an objective.");
                        }
                    }
                }
                
                UOACZSystem.ObjectiveCompleted(true);
            }

            else
            {
                BaseCreature creature = UOACZPersistance.UndeadBoss;

                if (creature == null)
                    return;

                UOACZPersistance.m_HumanObjective5 = (double)(creature.HitsMax - creature.Hits);
            }
        }

        //Undead Objectives
        public static void StockpileDamaged(bool destroyed)
        {
            if (!UOACZPersistance.Active) return;
            if (UOACZPersistance.m_UndeadObjective1 == UOACZPersistance.m_UndeadObjective1Target) return;
            
            int startingTotalDurability = UOACZSystem.UndeadObjectiveRequiredStockpilesDestroyed * UOACZStockpile.StartingMaxHits;
            int durabilityRemaining = 0;

            foreach (UOACZStockpile stockpile in UOACZStockpile.m_Instances)
            {
                if (stockpile == null) continue;
                if (stockpile.Deleted) continue;

                if (stockpile.DisplayName == "Outpost")
                    continue;

                durabilityRemaining += stockpile.HitPoints;
            }

            UOACZPersistance.m_UndeadObjective1 = startingTotalDurability - durabilityRemaining;
            
            if (destroyed)
            {
                foreach (NetState state in NetState.Instances)
                {
                    Mobile mobile = state.Mobile;
                    PlayerMobile player = mobile as PlayerMobile;

                    if (player == null)
                        continue;

                    if (UOACZRegion.ContainsMobile(player))
                    {
                        player.SendMessage(UOACZSystem.orangeTextHue, "A stockpile has been destroyed.");

                        if (UOACZPersistance.m_UndeadObjective1 == UOACZPersistance.m_UndeadObjective1Target)
                        {
                            player.SendMessage(UOACZSystem.orangeTextHue, "Undead Objective Complete: Destroy Stockpiles.");

                            if (player.IsUOACZUndead)
                            {
                                UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.UndeadUpgradePoints, UOACZSystem.UndeadObjective1UpgradePointsGranted, true);
                                UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.CorruptionPoints, 2, true);

                                player.SendSound(UOACZSystem.earnUndeadUpgradeSound);
                                player.SendMessage(UOACZSystem.greenTextHue, "You have received upgrade points and corruption points for completion of an objective.");
                            }
                        }
                    }
                }

                if (UOACZPersistance.m_UndeadObjective1 == UOACZPersistance.m_UndeadObjective1Target)                        
                    UOACZSystem.ObjectiveCompleted(false);
            }
        }

        public static void CivilianKilled()
        {
            if (!UOACZPersistance.Active) return;
            if (UOACZPersistance.m_UndeadObjective2 == UOACZPersistance.m_UndeadObjective2Target) return;

            UOACZPersistance.m_UndeadObjective2++;

            if (UOACZPersistance.m_UndeadObjective2 == UOACZPersistance.m_UndeadObjective2Target)
            {
                foreach (NetState state in NetState.Instances)
                {
                    Mobile mobile = state.Mobile;
                    PlayerMobile player = mobile as PlayerMobile;

                    if (player == null)
                        continue;

                    if (UOACZRegion.ContainsMobile(player))
                        player.SendMessage(UOACZSystem.orangeTextHue, "Undead Objective Complete: Kill Civilians");

                    if (player.IsUOACZUndead)
                    {
                        UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.UndeadUpgradePoints, UOACZSystem.UndeadObjective2UpgradePointsGranted, true);
                        UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.CorruptionPoints, 2, true);

                        player.SendSound(UOACZSystem.earnUndeadUpgradeSound);
                        player.SendMessage(UOACZSystem.greenTextHue, "You have received upgrade points and corruption points for completion of an objective.");
                    }
                }
                
                UOACZSystem.ObjectiveCompleted(false);
            }
        }

        public static void SpreadCorruption()
        {
            if (!UOACZPersistance.Active) return;
            if (UOACZPersistance.m_UndeadObjective3 == UOACZPersistance.m_UndeadObjective3Target) return;

            UOACZPersistance.m_UndeadObjective3++;

            if (UOACZPersistance.m_UndeadObjective3 == UOACZPersistance.m_UndeadObjective3Target)
            {
                foreach (NetState state in NetState.Instances)
                {
                    Mobile mobile = state.Mobile;
                    PlayerMobile player = mobile as PlayerMobile;

                    if (player == null)
                        continue;

                    if (UOACZRegion.ContainsMobile(player))
                        player.SendMessage(UOACZSystem.orangeTextHue, "Undead Objective Complete: Spread Corruption.");

                    if (player.IsUOACZUndead)
                    {
                        UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.UndeadUpgradePoints, UOACZSystem.UndeadObjective3UpgradePointsGranted, true);
                        UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.CorruptionPoints, 2, true);

                        player.SendSound(UOACZSystem.earnUndeadUpgradeSound);
                        player.SendMessage(UOACZSystem.greenTextHue, "You have received upgrade points and corruption points for completion of an objective.");
                    }
                }
                
                UOACZSystem.ObjectiveCompleted(false);
            }            
        }

        public static void HumanChampionDamaged(bool killed)
        {
            if (!UOACZPersistance.Active)
                return;

            if (killed)
            {
                UOACZPersistance.m_UndeadObjective4 = UOACZPersistance.m_UndeadObjective4Target;

                foreach (NetState state in NetState.Instances)
                {
                    Mobile mobile = state.Mobile;
                    PlayerMobile player = mobile as PlayerMobile;

                    if (player == null)
                        continue;

                    if (UOACZRegion.ContainsMobile(player))
                        player.SendMessage(UOACZSystem.orangeTextHue, "Undead Objective Complete: Slay Human Champion.");

                    if (player.IsUOACZUndead)
                    {
                        UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.UndeadUpgradePoints, UOACZSystem.UndeadObjective4UpgradePointsGranted, true);
                        UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.CorruptionPoints, 3, true);

                        player.SendSound(UOACZSystem.earnUndeadUpgradeSound);
                        player.SendMessage(UOACZSystem.greenTextHue, "You have received upgrade points and corruption points for completion of an objective.");
                    }
                }
                
                UOACZSystem.ObjectiveCompleted(false);
            }

            else
            {
                BaseCreature creature = UOACZPersistance.HumanChampion;

                if (creature == null)
                    return;

                UOACZPersistance.m_UndeadObjective4 = (double)(creature.HitsMax - creature.Hits);
            }
        }

        public static void HumanBossDamaged(bool killed)
        {
            if (!UOACZPersistance.Active)
                return;

            if (killed)
            {
                UOACZPersistance.m_UndeadObjective5 = UOACZPersistance.m_UndeadObjective5Target;

                foreach (NetState state in NetState.Instances)
                {
                    Mobile mobile = state.Mobile;
                    PlayerMobile player = mobile as PlayerMobile;

                    if (player == null)
                        continue;

                    if (UOACZRegion.ContainsMobile(player))
                        player.SendMessage(UOACZSystem.orangeTextHue, "Undead Objective Complete: Slay Human Boss.");

                    if (player.IsUOACZUndead)
                    {
                        UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.UndeadUpgradePoints, UOACZSystem.UndeadObjective5UpgradePointsGranted, true);
                        UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.CorruptionPoints, 4, true);

                        player.SendSound(UOACZSystem.earnUndeadUpgradeSound);
                        player.SendMessage(UOACZSystem.greenTextHue, "You have received upgrade points and corruption points for completion of an objective.");
                    }
                }
                
                UOACZSystem.ObjectiveCompleted(false);
            }

            else
            {
                BaseCreature creature = UOACZPersistance.HumanBoss;

                if (creature == null)
                    return;

                UOACZPersistance.m_UndeadObjective5 = (double)(creature.HitsMax - creature.Hits);
            }
        }

        public static List<Item> GetStockpileContribution(UOACZSystem.StockpileContributionType type, double value)
        {
            List<Item> m_Items = new List<Item>();

            double chance = 0;

            value *= UOACZSystem.StockpileChanceModifier;

            #region Determine Contributions

            switch (type)
            {
                case UOACZSystem.StockpileContributionType.Alchemist:
                    chance = value / 100;

                    if (Utility.RandomDouble() <= chance)
                    {
                        switch (Utility.RandomMinMax(1, 2))
                        {
                            case 1: 
                                switch (Utility.RandomMinMax(1, 8))
                                {
                                    case 1: m_Items.Add(new BlackPearl(Utility.RandomMinMax(2, 3))); break;
                                    case 2: m_Items.Add(new Bloodmoss(Utility.RandomMinMax(2, 3))); break;
                                    case 3: m_Items.Add(new MandrakeRoot(Utility.RandomMinMax(2, 3))); break;
                                    case 4: m_Items.Add(new Garlic(Utility.RandomMinMax(2, 3))); break;
                                    case 5: m_Items.Add(new Ginseng(Utility.RandomMinMax(2, 3))); break;
                                    case 6: m_Items.Add(new Nightshade(Utility.RandomMinMax(2, 3))); break;
                                    case 7: m_Items.Add(new SpidersSilk(Utility.RandomMinMax(2, 3))); break;
                                    case 8: m_Items.Add(new SulfurousAsh(Utility.RandomMinMax(2, 3))); break;
                                }
                            break;

                            case 2:
                                switch (Utility.RandomMinMax(1, 8))
                                {
                                    case 1: m_Items.Add(new HealPotion()); break;
                                    case 2: m_Items.Add(new GreaterHealPotion()); break;
                                    case 3: m_Items.Add(new RefreshPotion()); break;
                                    case 4: m_Items.Add(new TotalRefreshPotion()); break;
                                    case 5: m_Items.Add(new CurePotion()); break;
                                    case 6: m_Items.Add(new GreaterCurePotion()); break;
                                    case 7: m_Items.Add(new UOACZOilFlask()); break;
                                    case 8: m_Items.Add(new UOACZOilFlask()); break;
                                }
                            break;
                        }                        
                    }
                break;

                case UOACZSystem.StockpileContributionType.Weaver:
                    chance = value / 100;

                    if (Utility.RandomDouble() <= chance)
                    {
                        switch (Utility.RandomMinMax(1, 3))
                        {
                            case 1: m_Items.Add(new UOACZCotton(Utility.RandomMinMax(1, 2))); break;
                            case 2: m_Items.Add(new UOACZWool(Utility.RandomMinMax(1, 2))); break;
                            case 3: m_Items.Add(new Cloth(Utility.RandomMinMax(3, 5))); break;
                        }
                    }
                break;

                case UOACZSystem.StockpileContributionType.Farmer:
                    chance = value / 100;

                    if (Utility.RandomDouble() <= chance)
                    {
                        switch (Utility.RandomMinMax(1, 2))
                        {
                            case 1: m_Items.Add(new UOACZVegetable()); break;
                            case 2: m_Items.Add(new UOACZFruit()); break;
                        }
                    }
                break;

                case UOACZSystem.StockpileContributionType.Fisherman:
                    chance = value / 100;

                    if (Utility.RandomDouble() <= chance)
                    {
                        switch (Utility.RandomMinMax(1, 3))
                        {
                            case 1: m_Items.Add(new UOACZRawFishsteak()); break;
                            case 2: m_Items.Add(new UOACZCookedFishsteak()); break;
                            case 3: m_Items.Add(new UOACZCuredFish()); break;                            
                        }
                    }
                break;

                case UOACZSystem.StockpileContributionType.Skinner:
                    chance = value / 100;

                    if (Utility.RandomDouble() <= chance)
                    {
                        switch (Utility.RandomMinMax(1, 2))
                        {
                            case 1: m_Items.Add(new Leather(Utility.RandomMinMax(4, 6))); break;
                            case 2: m_Items.Add(new Feather(25)); break;                           
                        }
                    }
                break;

                case UOACZSystem.StockpileContributionType.Baker:
                    chance = value / 100;

                    if (Utility.RandomDouble() <= chance)
                    {
                        switch (Utility.RandomMinMax(1, 3))
                        {
                            case 1: m_Items.Add(new UOACZBowlOfDough()); break;
                            case 2: m_Items.Add(new UOACZBreadLoaves()); break;
                            case 3: m_Items.Add(new UOACZBreadRolls()); break;
                        }
                    }
                break;

                case UOACZSystem.StockpileContributionType.Butcher:
                    chance = value / 100;

                    if (Utility.RandomDouble() <= chance)
                    {
                        switch (Utility.RandomMinMax(1, 9))
                        {
                            case 1: m_Items.Add(new UOACZRawBacon()); break;
                            case 2: m_Items.Add(new UOACZRawBird()); break;
                            case 3: m_Items.Add(new UOACZRawCutsOfMeat()); break;
                            case 4: m_Items.Add(new UOACZRawDrumstick()); break;
                            case 5: m_Items.Add(new UOACZRawHam()); break;
                            case 6: m_Items.Add(new UOACZRawMeatScraps()); break;
                            case 7: m_Items.Add(new UOACZRawMeatShank()); break;
                            case 8: m_Items.Add(new UOACZRawSausage()); break;
                            case 9: m_Items.Add(new UOACZRawSteak()); break;
                        }
                    }
                break;

                case UOACZSystem.StockpileContributionType.Bowyer:
                    chance = value / 100;

                    if (Utility.RandomDouble() <= chance)
                    {
                        switch (Utility.RandomMinMax(1, 7))
                        {
                            case 1: m_Items.Add(new Arrow(Utility.RandomMinMax(10, 20))); break;
                            case 2: m_Items.Add(new Arrow(Utility.RandomMinMax(15, 30))); break;
                            case 3: m_Items.Add(new Bolt(Utility.RandomMinMax(10, 20))); break;
                            case 4: m_Items.Add(new Bolt(Utility.RandomMinMax(15, 30))); break;
                            case 5: m_Items.Add(new Feather(25)); break;
                            case 6: m_Items.Add(new Feather(25)); break;
                            case 7: m_Items.Add(new Shaft(25)); break;
                        }
                    }
                break;

                case UOACZSystem.StockpileContributionType.Provisioner:
                    chance = value / 100;

                    if (Utility.RandomDouble() <= chance)
                    {
                        switch (Utility.RandomMinMax(1, 4))
                        {                       
                            case 1:
                                switch (Utility.RandomMinMax(1, 10))
                                {
                                    case 1: m_Items.Add(new UOACZGlassOfCider()); break;
                                    case 2: m_Items.Add(new UOACZGlassOfLiquor()); break;
                                    case 3: m_Items.Add(new UOACZGlassOfMilk()); break;
                                    case 4: m_Items.Add(new UOACZGlassOfWater()); break;
                                    case 5: m_Items.Add(new UOACZGlassOfWine()); break;
                                    case 6: m_Items.Add(new UOACZBottleOfAle() { Charges = Utility.RandomMinMax(2, 3) }); break;
                                    case 7: m_Items.Add(new UOACZBottleOfFruitJuice() { Charges = Utility.RandomMinMax(2, 3) }); break;
                                    case 8: m_Items.Add(new UOACZBottleOfLiquor() { Charges = Utility.RandomMinMax(2, 3) }); break;                                    
                                    case 9: m_Items.Add(new UOACZBottleOfWine() { Charges = Utility.RandomMinMax(2, 3) }); break;
                                    case 10: m_Items.Add(new UOACZJugOfCider() { Charges = Utility.RandomMinMax(2, 3) }); break;
                                }
                            break;
                            case 2: m_Items.Add(new Bandage(Utility.RandomMinMax(4, 6))); break;
                            case 3: m_Items.Add(new UOACZTorch()); break;
                            case 4: m_Items.Add(new Feather(25)); break;
                        }
                    }
                break;

                case UOACZSystem.StockpileContributionType.Brewmaster:
                    chance = value / 100;

                    if (Utility.RandomDouble() <= chance)
                    {
                        switch (Utility.RandomMinMax(1, 2))
                        {
                            case 1:
                                switch (Utility.RandomMinMax(1, 3))
                                {
                                    case 1: m_Items.Add(new UOACZGlassOfCider()); break;
                                    case 2: m_Items.Add(new UOACZGlassOfLiquor()); break;
                                    case 3: m_Items.Add(new UOACZGlassOfWine()); break;                                   
                                }
                            break;

                            case 2:
                                switch (Utility.RandomMinMax(1, 4))
                                {
                                    case 1: m_Items.Add(new UOACZBottleOfAle() { Charges = Utility.RandomMinMax(2, 3) }); break;
                                    case 2: m_Items.Add(new UOACZBottleOfLiquor() { Charges = Utility.RandomMinMax(2, 3) }); break;
                                    case 3: m_Items.Add(new UOACZBottleOfWine() { Charges = Utility.RandomMinMax(2, 3) }); break;
                                    case 4: m_Items.Add(new UOACZJugOfCider() { Charges = Utility.RandomMinMax(2, 3) }); break;
                                }
                            break; 
                        }
                    }
                break;

                case UOACZSystem.StockpileContributionType.Carpenter:
                    chance = value / 100;

                    if (Utility.RandomDouble() <= chance)
                    {
                        switch (Utility.RandomMinMax(1, 5))
                        {
                            case 1: m_Items.Add(new Log(Utility.RandomMinMax(15, 30))); break;
                            case 2: m_Items.Add(new Log(Utility.RandomMinMax(20, 40))); break;
                            case 3: m_Items.Add(new Shaft(25)); break;
                            case 4: m_Items.Add(new UOACZBowl()); break;
                            case 5: m_Items.Add(new UOACZTorch()); break;
                        }
                    }
                break;

                case UOACZSystem.StockpileContributionType.Miner:
                    chance = value / 100;

                    if (Utility.RandomDouble() <= chance)
                    {
                        switch (Utility.RandomMinMax(1, 4))
                        {
                            case 1: m_Items.Add(new IronIngot(Utility.RandomMinMax(10, 20))); break;
                            case 2: m_Items.Add(new IronIngot(Utility.RandomMinMax(15, 30))); break;
                            case 3: m_Items.Add(new IronIngot(Utility.RandomMinMax(20, 40))); break;
                            case 4: m_Items.Add(new UOACZIronWire()); break;
                        }
                    }
                break;

                case UOACZSystem.StockpileContributionType.Healer:
                    chance = value / 100;

                    if (Utility.RandomDouble() <= chance)
                    {
                        switch (Utility.RandomMinMax(1, 4))
                        {
                            case 1: m_Items.Add(new Bandage(Utility.RandomMinMax(4, 6))); break;
                            case 2: m_Items.Add(new Bandage(Utility.RandomMinMax(5, 10))); break;
                            case 3: m_Items.Add(new HealPotion()); break;
                            case 4: m_Items.Add(new GreaterHealPotion()); break;
                        }
                    }
                break;

                case UOACZSystem.StockpileContributionType.Tinker:
                    chance = value / 100;

                    if (Utility.RandomDouble() <= chance)
                    {
                        switch (Utility.RandomMinMax(1, 4))
                        {
                            case 1: m_Items.Add(new UOACZIronWire()); break;
                            case 2: m_Items.Add(new UOACZIronWire()); break;
                            case 3: m_Items.Add(new UOACZRope()); break;
                            case 4: m_Items.Add(new UOACZOilFlask()); break;
                        }
                    }
                break;

                case UOACZSystem.StockpileContributionType.Blacksmith:
                    chance = value / 100;

                    if (Utility.RandomDouble() <= chance)
                    {
                        switch (Utility.RandomMinMax(1, 4))
                        {
                            case 1: m_Items.Add(Loot.RandomWeapon()); break;
                            case 2: m_Items.Add(Loot.RandomWeapon()); break;
                            case 3:
                                switch (Utility.RandomMinMax(1, 6))
                                {
                                    case 1: m_Items.Add(new RingmailArms()); break;
                                    case 2: m_Items.Add(new RingmailChest()); break;
                                    case 3: m_Items.Add(new RingmailLegs()); break;
                                    case 4: m_Items.Add(new RingmailGloves()); break;
                                    case 5: m_Items.Add(new ChainmailCoif()); break;
                                    case 6: m_Items.Add(new NorseHelm()); break;
                                }
                            break;
                            case 4:
                                switch (Utility.RandomMinMax(1, 4))
                                {
                                    case 1: m_Items.Add(new Buckler()); break;
                                    case 2: m_Items.Add(new MetalShield()); break;
                                    case 3: m_Items.Add(new BronzeShield()); break;
                                    case 4: m_Items.Add(new MetalKiteShield()); break;
                                }
                            break;
                        }
                    }
                break;

                case UOACZSystem.StockpileContributionType.Merchant:
                if (Utility.RandomDouble() <= chance)
                {
                    switch (Utility.RandomMinMax(1, 4))
                    {
                        case 1: m_Items.Add(new UOACZSurvivalStone()); break;

                        case 2:
                            switch (Utility.RandomMinMax(1, 10))
                            {
                                case 1: m_Items.Add(new UOACZGlassOfCider()); break;
                                case 2: m_Items.Add(new UOACZGlassOfLiquor()); break;
                                case 3: m_Items.Add(new UOACZGlassOfMilk()); break;
                                case 4: m_Items.Add(new UOACZGlassOfWater()); break;
                                case 5: m_Items.Add(new UOACZGlassOfWine()); break;
                                case 6: m_Items.Add(new UOACZBottleOfAle() { Charges = Utility.RandomMinMax(2, 3) }); break;
                                case 7: m_Items.Add(new UOACZBottleOfFruitJuice() { Charges = Utility.RandomMinMax(2, 3) }); break;
                                case 8: m_Items.Add(new UOACZBottleOfLiquor() { Charges = Utility.RandomMinMax(2, 3) }); break;                                
                                case 9: m_Items.Add(new UOACZBottleOfWine() { Charges = Utility.RandomMinMax(2, 3) }); break;
                                case 10: m_Items.Add(new UOACZJugOfCider() { Charges = Utility.RandomMinMax(2, 3) }); break;
                            }
                        break;
                       
                        case 3:
                            switch (Utility.RandomMinMax(1, 8))
                            {
                                case 1: m_Items.Add(new BlackPearl(Utility.RandomMinMax(2, 3))); break;
                                case 2: m_Items.Add(new Bloodmoss(Utility.RandomMinMax(2, 3))); break;
                                case 3: m_Items.Add(new MandrakeRoot(Utility.RandomMinMax(2, 3))); break;
                                case 4: m_Items.Add(new Garlic(Utility.RandomMinMax(2, 3))); break;
                                case 5: m_Items.Add(new Ginseng(Utility.RandomMinMax(2, 3))); break;
                                case 6: m_Items.Add(new Nightshade(Utility.RandomMinMax(2, 3))); break;
                                case 7: m_Items.Add(new SpidersSilk(Utility.RandomMinMax(2, 3))); break;
                                case 8: m_Items.Add(new SulfurousAsh(Utility.RandomMinMax(2, 3))); break;
                            }
                        break;

                        case 4:
                            switch (Utility.RandomMinMax(1, 3))
                            {
                                case 1: m_Items.Add(new UOACZCotton(Utility.RandomMinMax(1, 2))); break;
                                case 2: m_Items.Add(new UOACZWool(Utility.RandomMinMax(1, 2))); break;
                                case 3: m_Items.Add(new Cloth(Utility.RandomMinMax(4, 6))); break;
                            }
                        break;

                        case 5:
                            switch (Utility.RandomMinMax(1, 4))
                            {
                                case 1: m_Items.Add(new UOACZIronWire()); break;
                                case 2: m_Items.Add(new UOACZRope()); break;
                                case 3: m_Items.Add(new UOACZTorch()); break;
                                case 4: m_Items.Add(new UOACZOilFlask()); break;
                            }
                        break;

                    }
                }
                break;

                case UOACZSystem.StockpileContributionType.Noble:
                    chance = value / 100;

                    if (Utility.RandomDouble() <= chance)
                    {
                        switch (Utility.RandomMinMax(1, 1))
                        {
                            case 1: m_Items.Add(new UOACZHumanUpgradeToken()); break;
                        }
                    }
                break;
            }

            #endregion

            return m_Items;
        }
    }
}