using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Server.Mobiles;
using Server.Spells;

namespace Server.ArenaSystem
{
    public class ArenaQueue
    {
        public EArenaMatchRestrictions m_restrictions;
        public EArenaMatchEra m_era;
        public bool m_templated;
        public List<ArenaTeam> m_queue;
        public int MatchType { get { return ArenaUtilities.GetIndexForCriterion(m_restrictions, m_era, m_templated); } }

        public ArenaQueue(EArenaMatchRestrictions restrictions, EArenaMatchEra era, bool templated)
        {
            m_restrictions = restrictions;
            m_era = era;
            m_templated = templated;
            m_queue = new List<ArenaTeam>();
        }
    }
    /// <summary>
    /// The ArenaMatchmaker is responsible for arbitrating match creation.
    /// </summary>
    public class ArenaMatchmaker
    {
        private Dictionary<int, ArenaQueue> m_queues;
        private ProcessQueueTimer m_processTimer;

        public ArenaMatchmaker()
        {
            m_queues = new Dictionary<int, ArenaQueue>();
			for (int res = 0; res < (int)EArenaMatchRestrictions.eAMC_NumRestrictions; ++res )
			{
				EArenaMatchRestrictions restrictions = (EArenaMatchRestrictions)res;
				for (int e = 0; e < (int)EArenaMatchEra.eAMR_NumEras; ++e)
				{
					EArenaMatchEra era = (EArenaMatchEra)e;
					// Reserve a queue for templated, and non-templated versions.
					int index = ArenaUtilities.GetIndexForCriterion(restrictions, era, false);
					m_queues.Add(index, new ArenaQueue(restrictions, era, false));

					int indexTemplated = ArenaUtilities.GetIndexForCriterion(restrictions, era, true);
					m_queues.Add(indexTemplated, new ArenaQueue(restrictions, era, true));

				}
			}

            m_processTimer = new ProcessQueueTimer(this);
            m_processTimer.Start();
        }

        public void Pause()
        {
            if (m_processTimer.Running)
            {
                m_processTimer.Stop();
            }
        }
        public void Resume()
        {
            if (!m_processTimer.Running)
            {
                m_processTimer.Start();
            }
        }

        public bool JoinQueue(ArenaTeam team, EArenaMatchRestrictions restrictions, EArenaMatchEra eras, bool templated)
        {
            int index = ArenaUtilities.GetIndexForCriterion(restrictions, eras, templated);
            if (!m_queues[index].m_queue.Contains(team))
            {
                // You can not have a player in x different teams in y different queues. Any time
                // a join to any queue occurs, all of the teams the player represents must be removed
                // from every queue.
                foreach (PlayerMobile pm in team.Players)
                {
                    List<ArenaTeam> arenaTeams = ArenaSystem.GetTeamsForPlayer(pm);
                    foreach (ArenaTeam t in arenaTeams)
                    {
                        LeaveAllQueues(t);
                    }
                }

                m_queues[index].m_queue.Add(team);
                team.SetQueued(true);
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool LeaveAllQueues(ArenaTeam team)
        {
            bool found = false;
            foreach (KeyValuePair<int, ArenaQueue> kvp in m_queues)
            {
                ArenaQueue queue = kvp.Value;
                if (queue.m_queue.Contains(team))
                {
                    found = true;
                    queue.m_queue.Remove(team);

                    foreach (PlayerMobile player in team.Players)
                    {
						player.SendMessage("Your team {0} has been removed from the arena queue.", team.TeamName);
                        if (player.HasGump(typeof(ArenaBoard.ArenaQueueStatusGump)))
                        {
                            player.CloseGump(typeof(ArenaBoard.ArenaQueueStatusGump));
                        }
                    }
                    
                }
            }

            return found;
        }
        public void LeaveQueue(ArenaTeam team, EArenaMatchRestrictions restrictions, EArenaMatchEra eras, bool templated)
        {
            int index = ArenaUtilities.GetIndexForCriterion(restrictions, eras, templated);
            team.SetQueued(false);
            m_queues[index].m_queue.Remove(team);
        }
        public void ProcessQueues()
        {
            // Can't modify the dictionary during enumeration. Iterate over a list.
            List<ArenaQueue> queuesList = m_queues.Values.ToList<ArenaQueue>();
            foreach (ArenaQueue queue in queuesList)
            {
                FindCompatableMatches(queue);
            }
        }

        /// <summary>
        /// With the variety of queues and a small set expected, a n^2 comparison algorithm is acceptable for now.
        /// </summary>
        /// <param name="queue"></param>
        public void FindCompatableMatches(ArenaQueue queue)
        {
            //IOrderedEnumerable<ArenaTeam> orderedScores = queue.m_queue.OrderByDescending(x => 
            //    x.GetScoreByCriteria(queue.m_restrictions, queue.m_era, queue.m_templated));
            //List<ArenaTeam> orderedList = orderedScores.ToList<ArenaTeam>();

            List<Pair<ArenaTeam, ArenaTeam>> pendingMatches = new List<Pair<ArenaTeam, ArenaTeam>>();
            List<ArenaTeam>[] eligiblesList = 
            {
                new List<ArenaTeam>(),
                new List<ArenaTeam>(),
                new List<ArenaTeam>()
            };

            // Divide the teams into 3 lists, one for each arena type (1v1,2v2,3v3).
            foreach (ArenaTeam team in queue.m_queue)
            {
                eligiblesList[team.Capacity - 1].Add(team);
            }

            for (int i = 0; i < 3; ++i)
            {
                List<ArenaTeam> teamList = eligiblesList[i];
                foreach (ArenaTeam team1 in teamList)
                {
                    if (ArenaSystem.GetAvailableArenas(team1.Capacity) <= 0)
                        break;

                    if (team1.ReservedByMatchmaker == true)
                        continue;

                    // List of arena teams and their difference in score.
                    List<Pair<ArenaTeam, int>> compatableMatches = new List<Pair<ArenaTeam,int>>();
                    foreach (ArenaTeam team2 in teamList)
                    {
                        if (team2.ReservedByMatchmaker == true || team2 == team1)
                            continue;

                        int difference = Int16.MaxValue;
                        if (CheckCompatability(queue, team1, team2, ref difference))
                        {
                            compatableMatches.Add(new Pair<ArenaTeam,int>(team2,difference));
                            break;
                        }
                    }
                    
                    //// if only one other team in the queue, classify as compatible
                    //if (teamList.Count == 2 && compatableMatches.Count == 0) 
                    //{
                    //    ArenaTeam team2 = teamList.Find(t => t != team1);
                    //    if (team2 != null)
                    //        compatableMatches.Add(new Pair<ArenaTeam, int>(team2, 0));
                    //}

                    int count = compatableMatches.Count;
                    if (count == 0)
                        continue;
                    
                    // The arena team with the closest score should be selected for a matchup.
                    int lowestDifferenceFound = int.MaxValue;
                    ArenaTeam closestTeam = null;
                    for (int j = 0; j < count; ++j)
                    {
                        Pair<ArenaTeam,int> comparisonPair = compatableMatches[j];
                        if (comparisonPair.Second < lowestDifferenceFound)
                        {
                            closestTeam = comparisonPair.First;
                        }
                    }

                    if (closestTeam == null)
                        continue;

                    // A match against the closest scoring teams in the queue has been found.
                    team1.ReservedByMatchmaker = true;
                    closestTeam.ReservedByMatchmaker = true;

                    pendingMatches.Add(new Pair<ArenaTeam, ArenaTeam>(team1, closestTeam));
                    break;
                }
            }
            

            foreach (Pair<ArenaTeam, ArenaTeam> pairing in pendingMatches)
            {
                ArenaTeam team1 = pairing.First;
                ArenaTeam team2 = pairing.Second;
                queue.m_queue.Remove(team1);
                queue.m_queue.Remove(team2);
                team1.ReservedByMatchmaker = false;
                team2.ReservedByMatchmaker = false;

                foreach (PlayerMobile player in team1.Players)
                {
                    if (player.HasGump(typeof(ArenaBoard.ArenaQueueStatusGump)))
                    {
                        player.CloseGump(typeof(ArenaBoard.ArenaQueueStatusGump));
                    }
                }
                foreach (PlayerMobile player in team2.Players)
                {
                    if (player.HasGump(typeof(ArenaBoard.ArenaQueueStatusGump)))
                    {
                        player.CloseGump(typeof(ArenaBoard.ArenaQueueStatusGump));
                    }
                }
                ArenaSystem.CreateArenaMatch(team1, team2, queue);
            }
        }

        public bool CheckCompatability(ArenaQueue queue, ArenaTeam team1, ArenaTeam team2, ref int difference)
        {
            int team1Score = team1.GetScore(queue.m_restrictions, queue.m_era, queue.m_templated);
            int team2Score = team2.GetScore(queue.m_restrictions, queue.m_era, queue.m_templated);

            if (team1.Players.Any(p => SpellHelper.CheckCombat(p, true)))
            {
                team1.Players.ForEach(p => p.SendMessage(33, "You can not enter an Arena Match while in combat!"));
                return false;
            }

            if (team2.Players.Any(p => SpellHelper.CheckCombat(p, true)))
            {
                team2.Players.ForEach(p => p.SendMessage(33, "You can not enter an Arena Match while in combat!"));
                return false;
            }

            TimeSpan timeInQueue = team1.GetTimeInQueue();
            // Every 15s in the queue extends the score difference by 100.
            int nSeconds = timeInQueue.Seconds + (timeInQueue.Minutes * 60);
            float reach = ((float)nSeconds / 10.0f) * 100;
            int delta = Math.Abs(team1Score - team2Score);
            if (delta <= reach)
            {
                // The scores between these teams are compatible, considering time in queue, and base scores.
                difference = delta;
                return true;
            }
            return false;
        }
        private class ProcessQueueTimer : Timer
        {
            ArenaMatchmaker m_matchmaker;

            public ProcessQueueTimer(ArenaMatchmaker matchmaker)
                : base(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5))
            {
                m_matchmaker = matchmaker;
                Priority = TimerPriority.FiveSeconds;
            }
            protected override void OnTick()
            {
                m_matchmaker.ProcessQueues();
            }
        }
    }
}
