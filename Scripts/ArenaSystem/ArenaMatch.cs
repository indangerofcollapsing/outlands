using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server.Mobiles;
using Server.Items;

namespace Server.ArenaSystem
{
    public class ArenaMatch
    {
        public static TimeSpan s_matchTimeMax = new TimeSpan(0, 15, 0);
        //public static TimeSpan s_matchTimeMax = new TimeSpan(0, 0, 10);
        private static int s_matchId = 1;

        private List<Pair<PlayerMobile, List<ArenaStatOverride>>> m_statModifiers;
        private List<Pair<PlayerMobile, List<ArenaSkillOverride>>> m_skillModifiers;
        public ArenaTeam Team1 { get; set; }
        public ArenaTeam Team2 { get; set; }
        public List<PlayerMobile> Team1Players { get { return Team1.Players; } }
        public List<PlayerMobile> Team2Players { get { return Team2.Players; } }
        private List<PlayerMobile> AllPlayers { get { return Team1Players.Concat(Team2Players).ToList<PlayerMobile>(); } }
        public int TeamSize { get { return Team1.Players.Count; } }
        private ArenaAnnouncer m_announcer;
        public ArenaAnnouncer Announcer { get { return m_announcer; } }

        public Arena m_arena;

        // Match criteria
        public EArenaMatchRestrictions ERestrictions { get; set; }
        public EArenaMatchEra ERulesets { get; set; }
        public bool Templated { get; set; }
        public ArenaRuleset Ruleset { get; set; }
        public ArenaRestriction Restriction { get; set; }

        // Timers. Maintain a reference in case a match needs to be terminated for any technical reasons.
        private PreMatchTimer m_preMatchTimer;
        private ArenaMatchTimer m_matchTimer;
        private ArenaPostMatchTimer m_postMatchTimer;

        // Results
        public enum EMatchEndType
        {
            eMET_Win,       // Awarded on a genuine defeat
            eMET_Forfeit,   // A player disconnects
            eMET_Draw,      // Match time expired
        }

        public bool MatchComplete { get; set; }
        public EMatchEndType MatchEndResult { get; set; }
        public ArenaTeam Winner { get; set; }
        public ArenaTeam Loser { get; set; }
        private PlayerMobile m_forfeitingPlayer;

        public class ArenaSkillOverride : TimedSkillMod
        {
            public ArenaSkillOverride(SkillName skill, double value)
                : base(skill, false, value, s_matchTimeMax)
            {
            }
            public override bool CheckCondition()
            {
                // 11/16/13 - Verify that the skill mods are validated on character login. If so, we can kill this mod
                // by checking with the system for an active match (which it won't find).
                bool expired = base.CheckCondition();
                return expired || ArenaSystem.IsPlayerInMatch((PlayerMobile)Owner);
            }
        }

        public class ArenaStatOverride : StatMod
        {
            // The 30 seconds added is for the pre-match and post-match announcements.
            public ArenaStatOverride(StatType type, int newValue, int prevValue)
                : base(type, ArenaStatToString(type), GetStat(newValue,prevValue), s_matchTimeMax + TimeSpan.FromSeconds(30))
            {
            }
            private static string ArenaStatToString(StatType type)
            {
                return "ArenaStatMod_" + type.ToString();
            }
            /// <summary>
            /// Returns the offset necessary to bring the stat to the new value.
            /// </summary>
            /// <param name="newValue"></param>
            /// <param name="prevValue"></param>
            /// <returns></returns>
            private static int GetStat(int newValue, int prevValue)
            {
                return newValue - prevValue;
            }
        }

        public ArenaMatch(ArenaTeam team1, ArenaTeam team2, EArenaMatchRestrictions restrictions, EArenaMatchEra era, bool templated)
        {
            ERestrictions = restrictions;
            ERulesets = era;
            Templated = templated;
            Team1 = team1;
            Team2 = team2;
            m_statModifiers = new List<Pair<PlayerMobile, List<ArenaStatOverride>>>();
            m_skillModifiers = new List<Pair<PlayerMobile, List<ArenaSkillOverride>>>();

            if (restrictions == EArenaMatchRestrictions.eAMC_Order)
                Restriction = new OrderRestrictions();
            else if (restrictions == EArenaMatchRestrictions.eAMC_Chaos)
                Restriction = new ChaosRestrictions();
            if (era == EArenaMatchEra.eAMR_IPY)
                Ruleset = new IPY3ArenaRuleset();
            else if (era == EArenaMatchEra.eAMR_Pub16)
                Ruleset = new Pub16ArenaRuleset();
            else if (era == EArenaMatchEra.eAMR_T2A)
                Ruleset = new T2AArenaRuleset();
            s_matchId = s_matchId++;

            MatchComplete = false;
        }

		private string EraToString(EArenaMatchEra era)
		{
			return era == EArenaMatchEra.eAMR_IPY ? "UOAC" : era == EArenaMatchEra.eAMR_Pub16 ? "UO:R" : "T2A";
		}
        /// <summary>
        /// Announces the game conditions to the players and counts down to start of match.
        /// </summary>
        public void StartPreMatch(Arena arena)
        {
            m_arena = arena;
            m_arena.Setup(this);

            // Player prep
            StripConditions();

            foreach(PlayerMobile pm in AllPlayers)
                pm.DropHolding();

            Restriction.VerifyItems(AllPlayers);

            if (Templated)
            {
                ApplyTemplates();
            }

            foreach (PlayerMobile pm in AllPlayers)
            { 
                pm.Hits = pm.HitsMax;
                pm.Stam = pm.StamMax;
                pm.Mana = pm.ManaMax;
                pm.Poison = null;

                if (pm.Spell is Spells.Spell)
                    ((Spells.Spell)pm.Spell).Disturb(Spells.DisturbType.Kill);

                Targeting.Target.Cancel(pm);
            }

			Team1.Disable(true);
            Team2.Disable(true);
            Team1.Invalidate();
            Team2.Invalidate();

			foreach (PlayerMobile pm in Team1.Players)
            {
				pm.IsInArenaFight = true;
                pm.BankBox.Close();
            }
			foreach (PlayerMobile pm in Team2.Players)
            {
				pm.IsInArenaFight = true;
                pm.BankBox.Close();
            }

            // Announcer prep
            m_announcer = new ArenaAnnouncer();
            m_announcer.MoveToWorld(m_arena.AnnouncerLocation, Map.Felucca);
            m_announcer.Direction = Direction.East;

			m_announcer.PlaySound(0x521); // drumwhirl

            // Build the announcements from the match criteria.
            List<string> announcementsList = new List<String>();
			if (!Templated)
				announcementsList.Add("Ladies and Gentlemen!");
			announcementsList.Add(String.Format("Next is a {0} {1}/{2} fight", Templated ? "practice" : "ranked", ERestrictions == EArenaMatchRestrictions.eAMC_Order ? "Order" : "Chaos", EraToString(ERulesets)));
			if (!Templated)
			{
				// Ranked match, cooler announcement. People will be more tolerant to delays
				announcementsList.Add("Between...");
				announcementsList.Add(String.Format("{0} ({1})", Team1.TeamName, Team1.GetScore(ERestrictions, ERulesets, Templated)));
				announcementsList.Add("and...");
				announcementsList.Add(String.Format("{0} ({1})", Team2.TeamName, Team2.GetScore(ERestrictions, ERulesets, Templated)));
			}
			announcementsList.Add("Starting match in 3...");
            announcementsList.Add("Starting match in 2...");
            announcementsList.Add("Starting match in 1...");

            m_preMatchTimer = new PreMatchTimer(this, announcementsList);
            m_preMatchTimer.Start();
        }
        /// <summary>
        /// Starts the combat phase.
        /// </summary>
        public void StartMatch()
        {
            // Move the announcer to boundary line.
            Point3D loc1 = m_announcer.Location;
            Point3D loc2 = new Point3D(loc1.X - 5, loc1.Y, loc1.Z);
			m_announcer.Location = loc2;
            Effects.SendLocationParticles(EffectItem.Create(loc1, m_announcer.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
            Effects.SendLocationParticles(EffectItem.Create(loc2, m_announcer.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023);
            m_matchTimer = new ArenaMatchTimer(this);
            m_matchTimer.Start();

            Team1.Disable(false);
            Team2.Disable(false);
        }
        public void EndByDraw()
        {
			Team1.StoreDraw();
			Team2.StoreDraw();
            MatchEndResult = EMatchEndType.eMET_Draw;
            EndMatch();
        }
        public void EndByWin(ArenaTeam winner)
        {
            MatchEndResult = EMatchEndType.eMET_Win;
            Winner = winner;
            Loser = winner == Team1 ? Team2 : Team1;

			Winner.StoreWinFor(ERulesets, ERestrictions);
			Loser.StoreLossFor(ERulesets, ERestrictions);

			ArenaSpectator.DoPostMatchShouts(Announcer.Location, Winner.TeamName, Loser.TeamName);

			EndMatch();
        }
        public void EndByForfeit(ArenaTeam winner, PlayerMobile offender)
        {
            m_forfeitingPlayer = offender;

            MatchEndResult = EMatchEndType.eMET_Forfeit;
            Winner = winner;
            Loser = winner == Team1 ? Team2 : Team1;

			Winner.StoreWinFor(ERulesets, ERestrictions);
			Loser.StoreLossFor(ERulesets, ERestrictions);

            EndMatch();
        }
        /// <summary>
        /// Closes the combat phase, and begins the debriefing.
        /// </summary>
        public void EndMatch()
        {
            // Move the announcer back to center.
            Point3D loc1 = m_announcer.Location;
            Point3D loc2 = new Point3D(loc1.X + 5, loc1.Y, loc1.Z);
            m_announcer.Location = loc2;
            Effects.SendLocationParticles(EffectItem.Create(loc1, m_announcer.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
            Effects.SendLocationParticles(EffectItem.Create(loc2, m_announcer.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023);

            List<string> announcementsList = new List<string>();
            switch (MatchEndResult)
            {
                case EMatchEndType.eMET_Forfeit:
                    {
						announcementsList.Add(String.Format("{0} forfeited to {1} due to a disconnect or logout by {2}!", Loser.TeamName, Winner.TeamName, m_forfeitingPlayer.Name));
                        break;
                    }
                case EMatchEndType.eMET_Draw:
                    {
                        announcementsList.Add("The match is a draw!");
                        break;
                    }
                case EMatchEndType.eMET_Win:
                    {
						announcementsList.Add(String.Format("{0} has defeated {1}", Winner.TeamName, Loser.TeamName));
                        break;
                    }
            }

            announcementsList.Add("Ending match in 3...");
            announcementsList.Add("Ending match in 2...");
            announcementsList.Add("Ending match in 1...");

            foreach (PlayerMobile pm in AllPlayers)
            {
                pm.Poison = null;
            }

            // Invalidation so combat completely stops.
            Team1.Disable(true);
            Team1.ClearAggression();
            Team2.Disable(true);
            Team2.ClearAggression();

            m_postMatchTimer = new ArenaPostMatchTimer(this, announcementsList);
            m_postMatchTimer.Start();

            MatchComplete = true;
        }
        /// <summary>
        /// Cleans up the arena, ejects players and wipes any template allocations.
        /// </summary>
        public void EndPostMatch()
        {
            // The server can crash when debugging at the end of a match due to
            // duplicate timers.
            if (m_arena == null)
                return;

            StripConditions();
            StripModifiers();

            Team1.Resurrect();
            Team1.Disable(false);
            Team2.Resurrect();
            Team2.Disable(false);

            Restriction.OnEndMatch(Templated);

            foreach (PlayerMobile pm in AllPlayers)
            {
                pm.Hits = pm.HitsMax;
                pm.Stam = pm.StamMax;
                pm.Mana = pm.ManaMax;
                pm.Poison = null;
            }

            m_announcer.Delete();

            int index = ArenaUtilities.GetIndexForCriterion(ERestrictions, ERulesets, Templated);
            Team1.SetQueued(false);
            Team2.SetQueued(false);

            m_arena.Shutdown();
           
            // Final invalidation so client sees stats/skills removal.
            Team1.Invalidate();
            Team2.Invalidate();

            ArenaSystem.ConcludeArenaMatch(this);

			foreach (PlayerMobile pm in Team1.Players)
				pm.IsInArenaFight = false;
			foreach (PlayerMobile pm in Team2.Players)
				pm.IsInArenaFight = false;

        }
        /// <summary>
        /// A forfeit can occur when a player disconnects or logs out during a match.
        /// </summary>
        /// <param name="pm"></param>
        public void OnPlayerForfeited(PlayerMobile pm)
        {
            if (m_preMatchTimer != null && m_preMatchTimer.Running)
            {
                m_preMatchTimer.Stop();

                if (Team1Players.Contains(pm))
                {
                    EndByForfeit(Team2, pm);
                }
                else
                {
                    EndByForfeit(Team1, pm);
                }
            }
            if (m_matchTimer != null && m_matchTimer.Running)
            {
                m_matchTimer.Stop();

                if (Team1Players.Contains(pm))
                {
                    EndByForfeit(Team2, pm);
                }
                else
                {
                    EndByForfeit(Team1, pm);
                }
            }
        }
        private void ApplyTemplates()
        {
            foreach (PlayerMobile pm in AllPlayers)
            {
                ArenaTemplate template = ArenaSystem.GetTemplate(pm);
                if (template != null)
                {
                    // Skills
                    List<ArenaSkillOverride> skillsList = new List<ArenaSkillOverride>();
                    foreach(Pair<SkillName,int> skillPair in template.m_skillList)
                    {
                        ArenaSkillOverride mod = new ArenaSkillOverride(skillPair.First, skillPair.Second);
                        skillsList.Add(mod);
                        pm.AddSkillMod(mod);
                    }
                    m_skillModifiers.Add(new Pair<PlayerMobile, List<ArenaSkillOverride>>(pm, skillsList));

                    // Stats
                    List<ArenaStatOverride> statsList = new List<ArenaStatOverride>();
                    Trip<StatType, int, int>[] stats = new Trip<StatType, int, int>[3];
                    int idx = 0;
                    foreach(Pair<StatType, int> statPair in template.m_statList)
                    {
                        int rawStat = 0;
                        switch(idx)
                        {
                            case 0: rawStat = pm.RawStr; break;
                            case 1: rawStat = pm.RawDex; break;
                            case 2: rawStat = pm.RawInt; break;
                        }
                        stats[idx++] = new Trip<StatType, int, int>(statPair.First, statPair.Second, rawStat);
                    }
                    foreach (Trip<StatType, int, int> stat in stats)
                    {
                        ArenaStatOverride mod = new ArenaStatOverride(stat.First, stat.Second, stat.Third);
                        statsList.Add(mod);
                        pm.AddStatMod(mod);
                    }
                    m_statModifiers.Add(new Pair<PlayerMobile, List<ArenaStatOverride>>(pm, statsList));

                }
                
                pm.ValidateSkillMods();
                pm.InvalidateProperties();
            }
        }
        private void StripModifiers()
        {
            foreach (Pair<PlayerMobile, List<ArenaSkillOverride>> pair in m_skillModifiers)
            {
                PlayerMobile pm = pair.First;
                List<ArenaSkillOverride> skillsList = pair.Second;
                foreach (ArenaSkillOverride mod in skillsList)
                {
                    pm.RemoveSkillMod(mod);
                }
            }

            foreach (Pair<PlayerMobile, List<ArenaStatOverride>> pair in m_statModifiers)
            {
                PlayerMobile pm = pair.First;
                List<ArenaStatOverride> statsList = pair.Second;
                foreach (ArenaStatOverride mod in statsList)
                {
                    pm.RemoveStatMod(mod.Name);
                }
            }

            foreach (PlayerMobile pm in AllPlayers)
            {
                pm.ValidateSkillMods();
                pm.InvalidateProperties();
            }
        }
        /// <summary>
        /// Removes augments such as spells, potion effects, and on the player.
        /// </summary>
        private void StripConditions()
        {
            foreach (PlayerMobile pm in AllPlayers)
            {
                pm.MagicDamageAbsorb = 0; //Clear magic reflect
                pm.MeleeDamageAbsorb = 0; //Clear reactive armor
                pm.VirtualArmorMod = 0; //Clear protection

                BuffInfo.RemoveBuff(pm, BuffIcon.Agility);
                BuffInfo.RemoveBuff(pm, BuffIcon.ArchProtection);
                BuffInfo.RemoveBuff(pm, BuffIcon.Bless);
                BuffInfo.RemoveBuff(pm, BuffIcon.Clumsy);
                BuffInfo.RemoveBuff(pm, BuffIcon.Incognito);
                BuffInfo.RemoveBuff(pm, BuffIcon.MagicReflection);
                BuffInfo.RemoveBuff(pm, BuffIcon.MassCurse);
                BuffInfo.RemoveBuff(pm, BuffIcon.Invisibility);
                BuffInfo.RemoveBuff(pm, BuffIcon.HidingAndOrStealth);
                BuffInfo.RemoveBuff(pm, BuffIcon.Paralyze);
                BuffInfo.RemoveBuff(pm, BuffIcon.Poison);
                BuffInfo.RemoveBuff(pm, BuffIcon.Polymorph);
                BuffInfo.RemoveBuff(pm, BuffIcon.Protection);
                BuffInfo.RemoveBuff(pm, BuffIcon.ReactiveArmor);
                BuffInfo.RemoveBuff(pm, BuffIcon.Strength);
                BuffInfo.RemoveBuff(pm, BuffIcon.Weaken);
                BuffInfo.RemoveBuff(pm, BuffIcon.FeebleMind);

                pm.RemoveStatModsBeginningWith("[Magic]");
            }
        }
        public bool ContainsPlayer(PlayerMobile pm)
        {
            return AllPlayers.Contains(pm);
        }

        #region Timers
        private class PreMatchTimer : Timer
        {
            private int m_seconds;
            private ArenaMatch m_match;
            private List<string> m_announcementsList;
            private static int s_announcementTime = 1;
            private static int s_countdownTime = 1;
            private int m_countdownSeconds;

            public PreMatchTimer(ArenaMatch match, List<String> announcementsList)
                : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
            {
                Priority = TimerPriority.FiftyMS;
                m_seconds = 0;
                m_match = match;
                m_announcementsList = announcementsList;
                m_countdownSeconds = s_countdownTime;

				ArenaSpectator.DoPreMatchShouts(m_match.Announcer.Location, m_match.Team1.TeamName, m_match.Team2.TeamName);
			}
            protected override void OnTick()
            {
                if (m_announcementsList.Count > 0)
                {
                    if (m_seconds >= s_announcementTime)
                    {
                        string msg = m_announcementsList[0];
                        m_announcementsList.RemoveAt(0);
                        m_match.Announcer.Say(msg);
                        m_seconds = 0;
                    }
                }
                else if (m_countdownSeconds > 0 && m_seconds >= 3)
                {
                    m_match.Announcer.Say(m_countdownSeconds.ToString());
                    --m_countdownSeconds;
                }
                else
                {

                    Stop();
                    m_match.Announcer.Say("Fight!");
                    m_match.StartMatch();
                }

                ++m_seconds;
            }
        }

        private class ArenaMatchTimer : Timer
        {
            private DateTime m_matchStartTime;
            private ArenaMatch m_match;

            public ArenaMatchTimer(ArenaMatch match)
                : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
            {
                Priority = TimerPriority.OneSecond;
				m_matchStartTime = DateTime.UtcNow;
                m_match = match;
            }
            protected override void OnTick()
            {
                // Accurate time recording.
				TimeSpan span = DateTime.UtcNow.Subtract(m_matchStartTime);

				int seconds = span.Seconds;
				if (seconds % 10 == 0)
					m_match.Announcer.Say("[{0}m {1}s]", span.Minutes, span.Seconds);

				if ( (seconds+3) % 25 == 0)
				{
					ArenaSpectator.DoDuringMatchShout(m_match.Announcer.Location, Utility.RandomBool() ? m_match.Team1.TeamName : m_match.Team2.TeamName);
				}

                // Win conditions
                if (!m_match.Team1.Alive)
                {
                    m_match.EndByWin(m_match.Team2);
                    Stop();
                }
                else if (!m_match.Team2.Alive)
                {
                    m_match.EndByWin(m_match.Team1);
                    Stop();
                }
                if (span > ArenaMatch.s_matchTimeMax)
                {
                    // Remaining players becomes win criteria.
                    int team1Remaining = m_match.Team1.AliveCount;
                    int team2Remaining = m_match.Team2.AliveCount;
                    if (team1Remaining > team2Remaining)
                    {
                        m_match.EndByWin(m_match.Team1);
                        Stop();
                    }
                    else if (team2Remaining > team1Remaining)
                    {
                        m_match.EndByWin(m_match.Team2);
                        Stop();
                    }
                    else
                    {
                        // Draw
                        m_match.EndByDraw();
                        Stop();
                    }
                }
            }
        }

        private class ArenaPostMatchTimer : Timer
        {
            private int m_seconds;
            private ArenaMatch m_match;
            private List<string> m_announcementsList;
            private int m_announcementSeconds;
            private static int s_announcementTime = 1;

            public ArenaPostMatchTimer(ArenaMatch match, List<String> announcementsList)
                : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
            {
                Priority = TimerPriority.FiftyMS;
                m_seconds = 0;
                m_match = match;
                m_announcementsList = announcementsList;
            }
            protected override void OnTick()
            {
                if (m_announcementsList.Count > 0)
                {
                    if (m_seconds >= s_announcementTime)
                    {
                        string msg = m_announcementsList[0];
                        m_announcementsList.RemoveAt(0);
                        m_match.Announcer.Say(msg);
                        m_seconds = 0;
                    }
                }
                else if (m_seconds >= s_announcementTime)
                {
                    m_match.EndPostMatch();
                    Stop();
                }

                ++m_seconds;
            }
        }
        #endregion // Timers
    }
}