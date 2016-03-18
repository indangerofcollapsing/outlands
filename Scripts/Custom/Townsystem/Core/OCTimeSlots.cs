using System;
using System.Collections.Generic;

using Server.Mobiles;
using Server.Guilds;

namespace Server.Custom.Townsystem
{
    public static class OCTimeSlots
    {
        private static Timer m_Timer;
        private static Town m_ActiveTown = ActiveTown;

        public static void StartTimer()
        {
            m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30), Slice);
        }

        public static void Initialize()
        {
            StartTimer();
        }

        public static void Slice()
        {
            DateTime now = DateTime.Now;
            if (m_ActiveTown != ActiveTown)
            {
                // reset states, reward keys, change town ownership
                DetermineWinner();
                Town.DouseAllBraziers();
                RewardTreasuryKeys();
                OCLeaderboard.StopCounting();
                TreasuryChest.ClearThiefList();
                OCLeaderboard.Clear();
                m_ActiveTown = ActiveTown;
                WindBattleground.ResetOwner();
            }
        }

        private static Town ActiveTown
        {
            get
            {
                DateTime now = DateTime.Now;
                return Town.Towns[OCVulnerability.Vulnerabilities[now.Month - 1][now.Day - 1] - 1];
            }
        }

        private static void DetermineWinner()
        {
            var town = m_ActiveTown;

            if (town.KingOfTheHillTimer != null)
            {
                var winner = town;
                int points = 0;
                foreach(var entry in town.KingOfTheHillTimer.TownCapturePoints)
                {
                    if (entry.Value > points)
                    {
                        winner = entry.Key;
                        points = entry.Value;
                    }
                }

                town.Capture(winner);
            }
        }

        private static void RewardTreasuryKeys()
        {
            // vulnerable town
            DateTime now = DateTime.Now;
            Town active_town = m_ActiveTown;

            bool hometown_recaptured = active_town == active_town.ControllingTown;

            // distribute treasury gold unless the town was recaptured by the hometown
            int num_attackers = 0;
            int toMilitia = 0;
            Town capturing_town = active_town.ControllingTown;
            Dictionary<Mobile, OCLeaderboard.LeaderboardEntry> all_entries = OCLeaderboard.GetAllEntries();
            if (!hometown_recaptured)
            {
                int totalToSteal = (int)(active_town.Treasury * 0.25);
                active_town.Treasury -= totalToSteal; // removed from captured town

                toMilitia = Math.Min(totalToSteal / 2, 200000); // 50% of the 25% to militia, 250k max.
                capturing_town.Treasury += (totalToSteal - toMilitia) * 0.85;	// 15% additional taxation

                // reward active capturers with 5 keys and all other active participants with 3 keys
                foreach (var entry in all_entries)
                {	// yuck
                    PlayerMobile pm = entry.Key as PlayerMobile;
                    // only reward capturers
                    if (pm.IsInMilitia && pm.Citizenship == capturing_town && OCLeaderboard.WasActiveInActualTownFight(entry.Value))
                        ++num_attackers;
                }
            }

            // give keys and gold to players who were active in the actual fight
            int gold_per_attacker = num_attackers == 0 ? 0 : toMilitia / num_attackers;
            var guilds = new HashSet<Guild>();
            foreach (KeyValuePair<Mobile, OCLeaderboard.LeaderboardEntry> entry in all_entries)
            {
                PlayerMobile mob = entry.Key as PlayerMobile;
                if (!mob.Deleted && mob.Citizenship != null && mob.IsInMilitia)
                {
                    if (!OCLeaderboard.WasActiveInActualTownFight(entry.Value))
                        continue;

                    bool capturer = mob.Citizenship == capturing_town;
                    int keyreward = capturer ? 1000 : 500;

                    if (mob.Citizenship.HasActiveBuff(Custom.Townsystem.CitizenshipBuffs.Greed))
                    {
                        keyreward = (int)(keyreward * 1.1);
                    }

                    mob.TreasuryKeys += keyreward;

                    if (capturer)
                    {
                        // capturing a town grants guild xp
                        if (mob.Guild != null && mob.Guild is Guild)
                        {
                            var playerGuild = mob.Guild as Guild;
                            if (!guilds.Contains(playerGuild))
                                guilds.Add(playerGuild);
                        }
                        Item reward = new TreasuryKeyGoodiebag(mob, gold_per_attacker);
                        if (!mob.BankBox.TryDropItem(mob, reward, false))
                        {
                            mob.PlaceInBackpack(reward); // this can also fail. highly unlikely
                        }
                    }
                    mob.SendMessage(String.Format("You have been rewarded a total of {0} treasury keys for your participation in the town militia wars", keyreward));
                }
            }

            foreach (var guild in guilds)
                guild.TickExperience();
        }

        public static bool IsActiveTown(Town town)
        {
            return town == m_ActiveTown;
        }
    }
}
