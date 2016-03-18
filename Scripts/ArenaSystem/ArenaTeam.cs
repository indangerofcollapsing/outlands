using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server.Mobiles;
using Server.Achievements;
using Server.Items;

namespace Server.ArenaSystem
{
    public class ArenaTeam
    {
		public static int INITIAL_ARENA_TEAM_SCORE = 1500;

        static int s_nextTeamId = 1;
        private int m_teamId;
        private string m_name;
		public string TeamName { get { return m_name; } set { m_name = value; } }
        private bool m_queued;
        private DateTime m_queuedAtTime;
        public int Capacity { get { return m_members.Count; } }
        private List<PlayerMobile> m_members;
        public List<PlayerMobile> Players { get { return m_members; } }
        private int[] m_scores;

		private int m_totalWins, m_totalLosses, m_totalDraws;
		public int TotalDraws { get { return m_totalDraws; } }
		public int TotalWins { get { return m_totalWins; } }
		public int TotalLosses { get { return m_totalLosses; } }
		private int[,] m_categorizedWins = new int[(int)EArenaMatchEra.eAMR_NumEras, (int)EArenaMatchRestrictions.eAMC_NumRestrictions];
		private int[,] m_categorizedLosses = new int[(int)EArenaMatchEra.eAMR_NumEras, (int)EArenaMatchRestrictions.eAMC_NumRestrictions];

		// This is a back reference to ArenaSystem.s_indicesToArenaTeamMap for the cases where we need to go from a ArenaTeam to s_indicesToArenaTeamMap
		public int IndicesToArenaTeamMapIndex { set; get; }
		private List<LimitedArenaMatchResult>[] m_results;

        public bool ReservedByMatchmaker { get; set; }

		public void StoreWinFor(EArenaMatchEra era, EArenaMatchRestrictions restrictions)
		{
			++m_categorizedWins[(int)era, (int)restrictions];
			++m_totalWins;

			// achievements
			AchievementTriggers trigger = Capacity == 1 ? AchievementTriggers.Trigger_ArenaWin1v1 : Capacity == 2 ? AchievementTriggers.Trigger_ArenaWin2v2 : AchievementTriggers.Trigger_ArenaWin3v3;
			foreach (PlayerMobile pm in Players)
				AchievementSystem.Instance.TickProgress(pm, trigger);
		}
		public void StoreLossFor(EArenaMatchEra era, EArenaMatchRestrictions restrictions)
		{
			++m_categorizedLosses[(int)era, (int)restrictions];
			++m_totalLosses;

			// achievements
			AchievementTriggers trigger = Capacity == 1 ? AchievementTriggers.Trigger_ArenaLoose1v1 : Capacity == 2 ? AchievementTriggers.Trigger_ArenaLoose2v2 : AchievementTriggers.Trigger_ArenaLoose3v3;
			foreach (PlayerMobile pm in Players)
				AchievementSystem.Instance.TickProgress(pm, trigger);
		}
		public void StoreDraw()
		{
			++m_totalDraws;
		}
		public int GetWinsFor(EArenaMatchEra era, EArenaMatchRestrictions restrictions)
		{
			return m_categorizedWins[(int)era, (int)restrictions];
		}
		public int GetLossesFor(EArenaMatchEra era, EArenaMatchRestrictions restrictions)
		{
			return m_categorizedLosses[(int)era, (int)restrictions];
		}

        public bool Alive
        {
            get
            {
                foreach (PlayerMobile pm in m_members)
                {
                    if (pm.Alive)
                        return true;
                }
                return false;
            }
        }
        public int AliveCount
        {
            get
            {
                int aliveCount = 0;
                foreach (PlayerMobile pm in m_members)
                {
                    if (pm.Alive)
                    {
                        ++aliveCount;
                    }
                }
                return aliveCount;
            }
        }
        public void SetQueued(bool queued)
        {
            m_queued = queued;
            if (queued)
            {
				m_queuedAtTime = DateTime.UtcNow;
            }
            
        }
        public void Disable(bool disabled)
        {
            foreach (PlayerMobile pm in Players)
            {
                pm.Frozen = disabled;
                pm.CombatProhibited = disabled;
                if (pm.Target != null)
                {
                    pm.Target.Cancel(pm, Targeting.TargetCancelType.Canceled);
                }
            }
        }
        public void Resurrect()
        {
            foreach (PlayerMobile pm in Players)
            {
                if (!pm.Alive)
                {
                    pm.Resurrect();
                    DeathRobe robe = pm.FindItemOnLayer(Layer.OuterTorso) as DeathRobe;

                    if (robe != null)
                        robe.Delete();

                    Corpse corpse = pm.Corpse as Corpse;

                    if (corpse != null)
                    {
                        for (int i = 0; i < corpse.EquipItems.Count; ++i)
                        {
                            Item item = corpse.EquipItems[i];

                            if (item.Movable && item.Layer != Layer.Hair && item.Layer != Layer.FacialHair && item.IsChildOf(pm.Backpack))
                                pm.EquipItem(item);
                        }
                    }                    
                }
            }
        }
        public void Invalidate()
        {
            foreach (PlayerMobile pm in Players)
            {
                pm.InvalidateProperties();
                pm.SendIncomingPacket();
                pm.SendEverything();
                pm.Delta(MobileDelta.Noto);
            }
            
        }
        public void ClearAggression()
        {
            foreach (PlayerMobile pm in Players)
            {
                pm.Poison = null;
                pm.Aggressed.Clear();
                pm.Aggressors.Clear();
                pm.DamageEntries.Clear();
                pm.Combatant = null;
                pm.Criminal = false;
                pm.Warmode = false;
            }
        }
        public ArenaTeam(ArenaTeamCharter charter)
        {
            m_teamId = ++s_nextTeamId;
            m_name = charter.TeamName;
            m_members = charter.m_members;

            // Scores per match type.
            int count = ArenaUtilities.GetQueueCriterionCount();
            m_scores = new int[count];
            m_results = new List<LimitedArenaMatchResult>[count];

            for (int i = 0; i < count; ++i)
            {
                m_scores[i] = INITIAL_ARENA_TEAM_SCORE;
                m_queued = false;
                m_results[i] = new List<LimitedArenaMatchResult>();
            }

			ReservedByMatchmaker = false;

            m_totalDraws = m_totalWins = m_totalLosses = 0;
        }
        /// <summary>
        /// Called on construction during server startup.
        /// </summary>
        /// <param name="reader"></param>
        public ArenaTeam(GenericReader reader)
        {
            m_teamId = ++s_nextTeamId;
            m_members = new List<PlayerMobile>();
            m_queued = false;
            ReservedByMatchmaker = false;

            Deserialize(reader);
        }
        public TimeSpan GetTimeInQueue()
        {
			return DateTime.UtcNow.Subtract(m_queuedAtTime);
        }

		public void ResetSeasonalStats()
		{
			for (int i = 0; i < m_scores.Length; ++i)
				m_scores[i] = INITIAL_ARENA_TEAM_SCORE;

			for (int i = 0; i < m_categorizedWins.GetLength(0); ++i)
			{
				for (int j = 0; j < m_categorizedWins.GetLength(1); ++j)
				{
					m_categorizedWins[i, j] = 0;
					m_categorizedLosses[i, j] = 0;
				}
			}
		}
        public int GetScore(EArenaMatchRestrictions restrictions, EArenaMatchEra era, bool templated)
        {
            int index = ArenaUtilities.GetIndexForCriterion(restrictions, era, templated);
            return m_scores[index];
        }
        public void SetScore(EArenaMatchRestrictions restrictions, EArenaMatchEra era, bool templated, int score)
        {
            int index = ArenaUtilities.GetIndexForCriterion(restrictions, era, templated);
            m_scores[index] = score;

			// achievements
			if (score >= 2000)
			{
				foreach (PlayerMobile pm in Players)
					AchievementSystem.Instance.TickProgress(pm, AchievementTriggers.Trigger_Reach2000ArenaRanking);
			}
			else if (score >= 1750)
			{
				foreach (PlayerMobile pm in Players)
					AchievementSystem.Instance.TickProgress(pm, AchievementTriggers.Trigger_Reach1750ArenaRanking);
			}
        }
        public void StoreMatchResult(EArenaMatchRestrictions restrictions, EArenaMatchEra era, bool templated, LimitedArenaMatchResult result)
        {
            int index = ArenaUtilities.GetIndexForCriterion(restrictions, era, templated);
            m_results[index].Add(result);
            if (m_results[index].Count > 10)
            {
                m_results[index].RemoveAt(0);
            }
        }
        public List<LimitedArenaMatchResult> GetMatchResults(EArenaMatchRestrictions restrictions, EArenaMatchEra era, bool templated)
        {
            int index = ArenaUtilities.GetIndexForCriterion(restrictions, era, templated);
            return m_results[index];
        }
        public void Serialize(GenericWriter writer)
        {
            writer.Write(4); // Version

            // Version 0
            // Name
            writer.Write(m_name);

            // Scores
            int queuesCount = m_scores.Length;
            writer.Write(queuesCount);
            for (int i = 0; i < queuesCount; ++i)
            {
                writer.Write(m_scores[i]);
            }

            // Capacity and members.
            int membersCount = m_members.Count;
            writer.Write(membersCount);
            for (int i = 0; i < membersCount; ++i)
            {
                writer.Write(m_members[i].Serial);
            }

            // Version 1
            writer.Write(queuesCount);
            foreach (List<LimitedArenaMatchResult> list in m_results)
            {
                writer.Write(list.Count);
                foreach (LimitedArenaMatchResult result in list)
                {
                    result.Serialize(writer);
                }
            }

            // Version 2
            writer.Write(m_teamId);

            // Version 3
            writer.Write(TotalWins);
            writer.Write(TotalLosses);
            writer.Write(TotalDraws);

			// Version 4
			writer.Write((int)EArenaMatchEra.eAMR_NumEras);
			writer.Write((int)EArenaMatchRestrictions.eAMC_NumRestrictions);
			for (int i = 0; i < (int)EArenaMatchEra.eAMR_NumEras; ++i)
			{
				for (int j = 0; j < (int)EArenaMatchRestrictions.eAMC_NumRestrictions; ++j)
				{
					writer.Write(m_categorizedWins[i, j]);
					writer.Write(m_categorizedLosses[i, j]);
				}
			}
        }
        public void Deserialize(GenericReader reader)
        {
            int version = reader.ReadInt();

            if (version >= 0)
            {
                // Name
                m_name = reader.ReadString();

                // Scores
                int queuesCount = reader.ReadInt();
                m_scores = new int[queuesCount];
                m_results = new List<LimitedArenaMatchResult>[queuesCount];

                for (int i = 0; i < queuesCount; ++i)
                {
                    m_scores[i] = reader.ReadInt();
                    m_results[i] = new List<LimitedArenaMatchResult>();
                }

                // Capacity and members.
                int membersCount = reader.ReadInt();
                for (int i = 0; i < membersCount; ++i)
                {
                    Serial ser = reader.ReadInt();
                    Mobile m = World.FindMobile(ser);

                    // 12/31/13 Xiani - Checking for weirdness. Az reported seeing a Skeleton cast to PlayerMobile
                    // crash. My asumption is that the game data was corrupted somehow.
                    if (m == null)
                    {
                        Console.WriteLine("Alert!!! ArenaTeam: No mobile exists with serial {0}", ser);
                        continue;
                    }

                    if (!(m is PlayerMobile))
                    {
                        Console.WriteLine("Alert!!! ArenaTeam: Mobile with serial {0} is not a PlayerMobile", ser);
                        continue;
                    }

                    PlayerMobile pm = (PlayerMobile)m;
                    m_members.Add(pm);
                }
            }
            if (version >= 1)
            {
                int queuesCount = reader.ReadInt();
                for (int i = 0; i < queuesCount; ++i)
                {
                    int listCount = reader.ReadInt();
                    for (int j = 0; j < listCount; ++j)
                    {
                        m_results[i].Add(new LimitedArenaMatchResult(reader));
                    }
                }
            }
            if (version >= 2)
            {
                m_teamId = reader.ReadInt();

                // No guarantee in the order these were serialized.
                if (s_nextTeamId <= m_teamId)
                {
                    s_nextTeamId = ++m_teamId;
                }
            }
            if (version >= 3)
            {
                m_totalWins = reader.ReadInt();
                m_totalLosses = reader.ReadInt();
                m_totalDraws = reader.ReadInt();
            }

			if (version >= 4)
			{
				int num_stored_eras = reader.ReadInt();
				int num_stored_restrictions = reader.ReadInt();
				for (int i = 0; i < num_stored_eras; ++i)
				{
					for (int j = 0; j < num_stored_restrictions; ++j)
					{
						m_categorizedWins[i, j] = reader.ReadInt();
						m_categorizedLosses[i, j] = reader.ReadInt();
					}
				}
			}
        }
    }
}
