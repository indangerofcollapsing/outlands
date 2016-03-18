using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using Server.Mobiles;
using Server.Gumps;
using Server.Commands;
using Server.ArenaSystem;
using Server.Accounting;
using Server.Spells;
using Server.Spells.Fifth;
using Server.Spells.Seventh;
using Server.Items;

namespace Server.ArenaSystem
{
    public enum EArenaMatchRestrictions
    {
		/* 
		 * Dependency warning - ArenaTeam and ArenaSeason are serializing scores based on these enum numbers.
		 * Recommendation is to not remove eras from this list but rather just add new ones if necessary.
		 */ 
        eAMC_Chaos = 0,
        eAMC_Order = 1,

		eAMC_NumRestrictions,
    }
    public enum EArenaMatchEra
    {
		/* 
		 * Dependency warning - ArenaTeam and ArenaSeason are serializing scores based on these enum numbers.
		 * Recommendation is to not remove eras from this list but rather just add new ones if necessary.
		 */
		eAMR_IPY = 0,
        eAMR_T2A = 1,
        eAMR_Pub16 = 2,

		eAMR_NumEras,
    }

    public static class ArenaSystem
    {
        public static string[] s_restrictionNames = { "Chaos", "Order" };
        public static string[] s_eraNames = { "UOAC", "T2A", "UO:R" };

        private static string s_saveDirectory = "Saves/ArenaSystem";
        private static string s_arenaTeamsFilename = "arenaTeams.dat";
        private static string s_arenaTeamsFilepath = s_saveDirectory + "/" + s_arenaTeamsFilename;
        private static string s_arenasFilename = "arenas.dat";
        private static string s_arenasFilepath = s_saveDirectory + "/" + s_arenasFilename;
        private static string s_templatesFilename = "templates.dat";
        private static string s_templatesFilepath = s_saveDirectory + "/" + s_templatesFilename;

		private static string s_settingsFilename = "settings.dat";
		private static string s_settingsFilepath = s_saveDirectory + "/" + s_settingsFilename;

        // Because players have references to the same ArenaTeam instance, an indirect map places the
        // data replication on an int, rather than an ArenaTeam reference. This is convenient for
        // serial/deserialization.
        private static int s_nextArenaTeamIndex = 0;
        private static Dictionary<Serial, List<int>> s_playerSerialToIndicesMap;
        private static Dictionary<int, ArenaTeam> s_indicesToArenaTeamMap;
        private static ArenaMatchmaker s_matchmaker;
        private static List<Arena> s_arenaList;
        private static List<List<Arena>> s_availableArenasBySizeList;

        private static List<ArenaMatch> s_pendingMatchesList;
        private static List<ArenaMatch> s_completedMatchesList;

		public static Dictionary<int, ArenaTeam>.ValueCollection GetAllTeams()
		{
			return s_indicesToArenaTeamMap.Values;
		}

		public static ArenaTeam GetTeam_Slow(string team_name)
		{
			foreach (ArenaTeam at in ArenaSystem.s_indicesToArenaTeamMap.Values)
			{
				if (String.Compare(at.TeamName, team_name, true) == 0)
				{
					return at;
				}
			}
			return null;
		}

		public static void ResetSeasonalStats()
		{
			foreach (ArenaTeam at in ArenaSystem.s_indicesToArenaTeamMap.Values)
			{
				at.ResetSeasonalStats();
			}
		}

		public static List<Arena> GetAllArenas()
		{
			return s_arenaList;
		}

		public static List<ArenaMatch> GetAllPendingMatches()
		{
			return s_pendingMatchesList;
		}

		public static int GetAvailableArenas(int arenaCapacity)
        {
            return s_availableArenasBySizeList[arenaCapacity - 1].Count;
        }

        private static Dictionary<Serial, ArenaTemplate> s_playerSerialToTemplatesMap;

        public static void Initialize()
        {
            s_playerSerialToIndicesMap = new Dictionary<Serial, List<int>>();
            s_indicesToArenaTeamMap = new Dictionary<int, ArenaTeam>();
            s_matchmaker = new ArenaMatchmaker();
            s_arenaList = new List<Arena>();
            
            // A list for 1v1, 2v2, and 3v3
            s_availableArenasBySizeList = new List<List<Arena>>();
            for (int i = 0; i < 3; ++i)
            {
                s_availableArenasBySizeList.Add(new List<Arena>());
            }

            s_pendingMatchesList = new List<ArenaMatch>();
            s_completedMatchesList = new List<ArenaMatch>();

            s_playerSerialToTemplatesMap = new Dictionary<Serial, ArenaTemplate>();

            EventSink.ServerStarted += new ServerStartedEventHandler(OnWorldLoad);
            EventSink.WorldSave += new WorldSaveEventHandler(OnWorldSave);
            EventSink.Disconnected += new DisconnectedEventHandler(OnDisconnected);
            EventSink.Logout += new LogoutEventHandler(OnLogout);
			EventSink.Login += new LoginEventHandler(OnLogin);
			
            RegisterCommands();
        }
        public static void OnCharacterDeleted(Mobile m)
        {
            PlayerMobile pm = m as PlayerMobile;
            foreach (ArenaTeam team in GetTeamsForPlayer(pm))
            {
                DisbandAndDelete(team);
            }
        }
        private static void RegisterCommands()
        {
            CommandSystem.Register("as_1v1", AccessLevel.GameMaster, new CommandEventHandler(Create1v1Arena));
            CommandSystem.Register("as_2v2", AccessLevel.GameMaster, new CommandEventHandler(Create2v2Arena));
            CommandSystem.Register("as_3v3", AccessLevel.GameMaster, new CommandEventHandler(Create3v3Arena));
			CommandSystem.Register("as_admin", AccessLevel.GameMaster, new CommandEventHandler(ShowAdminGump));
        }
        private static void Create1v1Arena(CommandEventArgs e)
        {
            e.Mobile.Target = new Create1v1ArenaTarget();
        }
        private static void Create2v2Arena(CommandEventArgs e)
        {
            e.Mobile.Target = new Create2v2ArenaTarget();
        }
        private static void Create3v3Arena(CommandEventArgs e)
        {
            e.Mobile.Target = new Create3v3ArenaTarget();
        }
		private static void ShowAdminGump(CommandEventArgs e)
        {
			e.Mobile.SendGump(new ArenaAdminGump(e.Mobile, ArenaAdminGump.ListType.Arenas));
        }
		private static void ShowMyArenaGump(CommandEventArgs e)
        {
			e.Mobile.SendGump(new ArenaAdminGump(e.Mobile, ArenaAdminGump.ListType.MyTeams));
        }
        private static void OnWorldSave(WorldSaveEventArgs e)
        {
            ArenaUtilities.WriteLine("Starting save routine.");
            if (!Directory.Exists(s_saveDirectory))
            {
                Directory.CreateDirectory(s_saveDirectory);
            }

            // ArenaTeams
            try
            {
                FileStream fs = new FileStream(s_arenaTeamsFilepath, FileMode.Create, FileAccess.Write, FileShare.None);
                GenericWriter writer = new BinaryFileWriter(fs, true);
                writer.Write(0); // Version
                writer.Write(s_indicesToArenaTeamMap.Count);
                foreach (KeyValuePair<int, ArenaTeam> kvp in s_indicesToArenaTeamMap)
                {
                    writer.Write(kvp.Key);
                    kvp.Value.Serialize(writer);
                }

                writer.Close();
            }
            catch (Exception exc)
            {
                ArenaUtilities.WriteError(exc, "Failed to correctly read {0}. See Xiani.", s_arenaTeamsFilename);
            }

            // Obsolete. Arenas are now found at load time and added to the system.
            //// Arenas
            //try
            //{
            //    FileStream fs = new FileStream(s_arenasFilepath, FileMode.Create, FileAccess.Write, FileShare.None);
            //    GenericWriter writer = new BinaryFileWriter(fs, true);
            //    writer.Write(0); // Version
            //    writer.Write(s_arenaList.Count);
            //    foreach (Arena arena in s_arenaList)
            //    {
            //        writer.Write(arena.Serial);
            //    }

            //    writer.Close();
            //}
            //catch (Exception exc)
            //{
            //    ArenaUtilities.WriteError(exc, "Failed to correctly read {0}.", s_arenasFilename);
            //}

            // Templates
            try
            {
                FileStream fs = new FileStream(s_templatesFilepath, FileMode.Create, FileAccess.Write, FileShare.None);
                GenericWriter writer = new BinaryFileWriter(fs, true);
                writer.Write(0); // Version
                writer.Write(s_playerSerialToTemplatesMap.Count);
                foreach (ArenaTemplate template in s_playerSerialToTemplatesMap.Values)
                {
                    template.Serialize(writer);
                }

                writer.Close();
            }
            catch (Exception exc)
            {
                ArenaUtilities.WriteError(exc, "Failed to correctly read {0}.", s_templatesFilename);
            }


			// Settings
			try
			{
				FileStream fs = new FileStream(s_settingsFilepath, FileMode.Create, FileAccess.Write, FileShare.None);
				GenericWriter writer = new BinaryFileWriter(fs, true);
				writer.Write(0); // Version
				writer.Write((int)ArenaMatch.s_matchTimeMax.TotalSeconds);
				writer.Close();
			}
			catch (Exception exc)
			{
				ArenaUtilities.WriteError(exc, "Failed to correctly read {0}.", s_settingsFilepath);
			}

            try
            {
			    // Leaderboards
			    ArenaLeaderboards.GenerateLeaderboards();
            } catch (Exception ex)
            {
                ArenaUtilities.WriteError(ex, "Failed to generate leaderboards {0}", ex.Message);
            }

            ArenaUtilities.WriteLine("Finished save routine.");
        }
        private static void OnDisconnected(DisconnectedEventArgs e)
        {
            RemovePlayerFromSystem((PlayerMobile)e.Mobile);
        }
        private static void OnLogout(LogoutEventArgs e)
        {
            RemovePlayerFromSystem((PlayerMobile)e.Mobile);
        }

		private static void OnLogin(LoginEventArgs e)
		{
			if (e.Mobile is PlayerMobile) // duh
			{
                VerifyPlayerLocation((PlayerMobile)e.Mobile);
			}
		}
        public static void VerifyPlayerLocation(PlayerMobile pm)
        {
            // Kick out players that accidentally got stuck in the arena.
            // Can happen after a server crash where players were in an arena fight or
            // by a player who has disconnected and returned to an ArenaCombatRegion once
            // their match has ended.
            if (pm.Region is ArenaCombatRegion)
            {
                // A player is only permitted in in the region if they are in a match.
                ArenaMatch match = GetMatchForPlayer(pm);
                if (match == null)
                {
                    pm.Location = (pm.Region as ArenaCombatRegion).Arena.Team1EjectionLocations[0];
                }
            }
        }
        private static void RemovePlayerFromSystem(PlayerMobile pm)
        {
            ArenaMatch match = GetMatchForPlayer(pm);
            if (match == null)
            {
                List<ArenaTeam> arenaTeams = GetTeamsForPlayer(pm);
                foreach (ArenaTeam team in arenaTeams)
                {
                    s_matchmaker.LeaveAllQueues(team);
                }
            }
            else
            {
                // A 1v1 match is instantly forfeited on a disconnect/logout.
                if (match.TeamSize == 1)
                {
                    match.OnPlayerForfeited(pm);
                }
            }
        }

        private static void OnWorldLoad()
        {
            ArenaUtilities.WriteLine("Starting load routine.");
            // ArenaTeams
            if (File.Exists(s_arenaTeamsFilepath))
            {
                try
                {
                    FileStream fs = new FileStream(s_arenaTeamsFilepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    BinaryFileReader reader = new BinaryFileReader(new BinaryReader(fs));

                    int version = reader.ReadInt();
                    int arenaTeamsCount = reader.ReadInt();
                    for (var i = 0; i < arenaTeamsCount; ++i)
                    {
                        int index = reader.ReadInt();
                        ArenaTeam team = new ArenaTeam(reader);
						
						if (team.Players.Count == 0)
							continue; // Discard saved "empty" teams.  

                        // Guarantee that the next arena team index is unused.
                        s_nextArenaTeamIndex = Math.Max(index + 1, s_nextArenaTeamIndex);

                        s_indicesToArenaTeamMap.Add(index, team);
						team.IndicesToArenaTeamMapIndex = index; // Setting a reference back for the cases where we need to go from ArenaTeam to s_indicesToArenaTeamMap

                        // Re-mapping of serial to ArenaTeam unique index.
                        foreach (PlayerMobile pm in team.Players)
                        {
                            if (!s_playerSerialToIndicesMap.ContainsKey(pm.Serial))
                            {
                                List<int> indicesList = new List<int>();
                                indicesList.Add(index);
                                s_playerSerialToIndicesMap.Add(pm.Serial, indicesList);
                            }
                            else
                            {
                                s_playerSerialToIndicesMap[pm.Serial].Add(index);
                            }
                        }
                    }

                    reader.Close();
                }
                catch (Exception exc)
                {
                    ArenaUtilities.WriteError(exc, "Failed to correctly read {0}.", s_arenaTeamsFilename);
                }
            }

            // Arenas
            Dictionary<Serial,Item>.Enumerator itemsEnum = World.Items.GetEnumerator();
            while (itemsEnum.MoveNext())
            {
                if (itemsEnum.Current.Value is Arena)
                {
                    Arena arena = itemsEnum.Current.Value as Arena;
                    RegisterArena(arena);
                }
            }

            // Obsolete. Arenas are now found at load time and added to the system.
            //// Arenas
            //if (File.Exists(s_arenasFilepath))
            //{
            //    try
            //    {
            //        FileStream fs = new FileStream(s_arenasFilepath, FileMode.Open, FileAccess.Read, FileShare.Read);
            //        BinaryFileReader reader = new BinaryFileReader(new BinaryReader(fs));

            //        int version = reader.ReadInt();
            //        int arenaCount = reader.ReadInt();
            //        for (int i = 0; i < arenaCount; ++i)
            //        {
            //            Serial ser = reader.ReadInt();
            //            Arena arena = (Arena)World.FindItem(ser);
            //            if (arena != null)
            //            {
            //                RegisterArena(arena);
            //            }
            //        }

            //        reader.Close();
            //    }
            //    catch (Exception exc)
            //    {
            //        ArenaUtilities.WriteError(exc, "Failed to correctly read {0}.", s_arenasFilename);
            //    }
            //}
            // Templates
            if (File.Exists(s_templatesFilepath))
            {
                try
                {
                    FileStream fs = new FileStream(s_templatesFilepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    BinaryFileReader reader = new BinaryFileReader(new BinaryReader(fs));

                    int version = reader.ReadInt();
                    int templatesCount = reader.ReadInt();
                    for (int i = 0; i < templatesCount; ++i)
                    {
                        ArenaTemplate template = new ArenaTemplate(reader);
                        s_playerSerialToTemplatesMap.Add(template.m_serialId, template);
                    }

                    reader.Close();
                }
                catch (Exception exc)
                {
                    ArenaUtilities.WriteError(exc, "Failed to correctly read {0}.", s_templatesFilename);
                }
            }

			// Settings
			if (File.Exists(s_settingsFilepath))
			{
				try
				{
					FileStream fs = new FileStream(s_settingsFilepath, FileMode.Open, FileAccess.Read, FileShare.Read);
					BinaryFileReader reader = new BinaryFileReader(new BinaryReader(fs));

					int version = reader.ReadInt();
					ArenaMatch.s_matchTimeMax = new TimeSpan(0, 0, reader.ReadInt());
					reader.Close();
				}
				catch (Exception exc)
				{
					ArenaUtilities.WriteError(exc, "Failed to correctly read {0}.", s_templatesFilename);
				}
			}

			// Leaderboards
			ArenaLeaderboards.InitializeToCapacity(s_indicesToArenaTeamMap.Count);
			ArenaLeaderboards.GenerateLeaderboards();

            ArenaUtilities.WriteLine("Finished load routine.");
        }
        public static void RegisterArena(Arena arena)
        {
            s_arenaList.Add(arena);
            s_availableArenasBySizeList[arena.Size-1].Add(arena);
        }

		public static bool IsRankedArenaWorthy(PlayerMobile mob)
		{
			if (mob.Skills.Total > 600)
			{
				// gm in at least 5 combat skills
				int num_gm = 0;
				if (mob.Skills.Anatomy.BaseFixedPoint == 1000) ++num_gm;
				if (mob.Skills.Parry.BaseFixedPoint == 1000) ++num_gm;
				if (mob.Skills.EvalInt.BaseFixedPoint == 1000) ++num_gm;
				if (mob.Skills.Healing.BaseFixedPoint == 1000) ++num_gm;
				if (mob.Skills.Magery.BaseFixedPoint == 1000) ++num_gm;
				if (mob.Skills.MagicResist.BaseFixedPoint == 1000) ++num_gm;
				if (mob.Skills.Tactics.BaseFixedPoint == 1000) ++num_gm;
				if (mob.Skills.Poisoning.BaseFixedPoint == 1000) ++num_gm;
				if (mob.Skills.Archery.BaseFixedPoint == 1000) ++num_gm;
				if (mob.Skills.Swords.BaseFixedPoint == 1000) ++num_gm;
				if (mob.Skills.Macing.BaseFixedPoint == 1000) ++num_gm;
				if (mob.Skills.Fencing.BaseFixedPoint == 1000) ++num_gm;
				if (mob.Skills.Wrestling.BaseFixedPoint == 1000) ++num_gm;
				if (mob.Skills.Meditation.BaseFixedPoint == 1000) ++num_gm;

				return num_gm >= 5;
			}
			return false;
		}

        public static void UnregisterArena(Arena arena)
        {
            bool removed = s_arenaList.Remove(arena);
			Debug.Assert(removed);
			removed = s_availableArenasBySizeList[arena.Size - 1].Remove(arena);
			Debug.Assert(removed);
		}

        public static void JoinQueue(ArenaTeam team, EArenaMatchRestrictions restrictions, EArenaMatchEra era, bool templated, Point3D location)
        {
            // Every member of the team must be located in some arena region, somewhere.
            foreach (PlayerMobile player in team.Players)
            {
                if (!(player.Region is ArenaSpectatorRegion))
                {
                    player.SendMessage("Your entire team is required in the vicinity of an arena before it can be queued for a match.");
                    return;
                }

                if (player.RawStr < 75)
                {
                    player.SendMessage("One or more of your team members does not meet the 75 Str prerequisite required to participate in a match.");
                    return;
                }

                bool restrictedTransformation = false;

                if (!player.CanBeginAction(typeof(PolymorphSpell)))
                    restrictedTransformation = true;

                if (!player.CanBeginAction(typeof(PolymorphPotion)))
                    restrictedTransformation = true;

                if (!player.CanBeginAction(typeof(IncognitoSpell)) || player.IsBodyMod)
                    restrictedTransformation = true;

                if (DisguiseTimers.IsDisguised(player))
                    restrictedTransformation = true;

                if (restrictedTransformation)
                {
                    player.SendMessage("One or more of your team members is currently under the effects of polymorph, incognito, or a diguise and may not participate in a match.");
                    return;
                }
            }

			// Team name check!
			// If for some reason the characters name have changed (namechange deed, GM change or whatever)
			//	then rename the 1v1 team as well
			if (team.Capacity == 1 && team.Players[0].Name != team.TeamName)
				team.TeamName = team.Players[0].Name;

            if (s_matchmaker.JoinQueue(team, restrictions, era, templated))
            {
                foreach (PlayerMobile player in team.Players)
                {
                    player.SendMessage("Your team {0} is in the queue for a {1} {2} {3} era match.",
                        team.TeamName, templated ? "PRACTICE " : "RANKED ", s_restrictionNames[(int)restrictions], s_eraNames[(int)era]);

                    if (player.HasGump(typeof(ArenaBoard.ArenaQueueStatusGump)))
                    {
                        player.CloseGump(typeof(ArenaBoard.ArenaQueueStatusGump));
                    }

					player.SendGump(new ArenaBoard.ArenaQueueStatusGump(team.TeamName, templated, restrictions, era));
                }
            }

            else
            {
                foreach (PlayerMobile player in team.Players)
                {
                    player.SendMessage("You're already in the queue for that ladder.");
                }
            }
        }
        public static bool LeaveQueue(PlayerMobile pm)
        {
            bool found = false;
            List<ArenaTeam> arenaTeams = GetTeamsForPlayer(pm);
            foreach (ArenaTeam team in arenaTeams)
            {
                found = s_matchmaker.LeaveAllQueues(team);
                // A team only be in one queue, so there is no point searching additional queues.
                if (found)
                     break;
            }
            return found;
        }
        /// <summary>
        /// Locates and initializes an available arena, and starts a match.
        /// </summary>
        /// <param name="team1"></param>
        /// <param name="team2"></param>
        /// <param name="templated"></param>
        public static void CreateArenaMatch(ArenaTeam team1, ArenaTeam team2, ArenaQueue queue)
        {
            int arenaSize = team1.Capacity;
            List<Arena> availableArenas = s_availableArenasBySizeList[arenaSize - 1];
            int nAvailable = availableArenas.Count;
            if (nAvailable <= 0)
                return;

            // Get a random arena from the pool.
			Random r = new Random(DateTime.UtcNow.Millisecond);
            int index = r.Next(nAvailable);
            Arena arena = availableArenas[index];
            availableArenas.RemoveAt(index);

            ArenaMatch match = new ArenaMatch(team1, team2, queue.m_restrictions, queue.m_era, queue.m_templated);
            s_pendingMatchesList.Add(match); 
            
            // Initialize the match.
            match.StartPreMatch(arena);
        }

		private static bool MatchingIP2(PlayerMobile pm1, PlayerMobile pm2)
		{
			return pm1.NetState.Address.Equals(pm2.NetState.Address);
		}
		private static bool MatchingIP3(PlayerMobile pm1, PlayerMobile pm2, PlayerMobile pm3)
		{
			return pm1.NetState.Address.Equals(pm2.NetState.Address) || pm1.NetState.Address.Equals(pm3.NetState.Address);
		}
        /// <summary>
        /// Points are issued at the system level.
        /// </summary>
        /// <param name="match"></param>
        public static void ConcludeArenaMatch(ArenaMatch match)
        {
            // Match is complete.
            s_pendingMatchesList.Remove(match);
            s_completedMatchesList.Add(match);

            // Because a request for match results has a measurable expense on the server, 
            // the quantity and granularity of the results is limited to 10 per queue type.
            // Verbose details are stored on the server until a server save, where it is
            // off-loaded to disk. A parser will be written later to extract this match
            // data and send it to the leaderboard on the website.


			// Johan - Added a very basic IP check here. Basically preventing any score adjustments if cross-team multi boxing is detected
			bool two_online = match.Team1Players[0].NetState != null && match.Team2Players[0].NetState != null;
			bool three_online = two_online && match.Team1Players.Count == 2 && match.Team1Players[1].NetState != null;
			if (three_online && MatchingIP3(match.Team2Players[0], match.Team1Players[0], match.Team1Players[1]) ||
				two_online && MatchingIP2(match.Team2Players[0], match.Team1Players[0]) )
			{
				; // dual-boxing
			}
			else
			{
				// Awards
				int team1Adj = 0;
				int team2Adj = 0;
				AdjustTeamRatings(match, ref team1Adj, ref team2Adj);

                ArenaSeason season = ArenaSeasonTracker.GetCurrentSeason();
                string seasonId = "invalid";
                if (season != null)
                {
                    seasonId = season.SeasonName;
                }

				// Store result
				string winnerName = match.MatchEndResult == ArenaMatch.EMatchEndType.eMET_Win ? match.Winner.TeamName : "None";
				LimitedArenaMatchResult limitedResult = new LimitedArenaMatchResult(match.Team1.TeamName, match.Team2.TeamName,
                    match.MatchEndResult, winnerName, team1Adj, team2Adj, seasonId);
				match.Team1.StoreMatchResult(match.ERestrictions, match.ERulesets, match.Templated, limitedResult);
				match.Team2.StoreMatchResult(match.ERestrictions, match.ERulesets, match.Templated, limitedResult);
			}

            // Return the arena back to the pool.
            int arenaSize = match.Team1.Capacity;
            s_availableArenasBySizeList[arenaSize - 1].Add(match.m_arena);
            match.m_arena = null;
        }

        private static void AdjustTeamRatings(ArenaMatch match, ref int team1Adj, ref int team2Adj)
        {
            float team1ResultMultiplier = 0.0f;
            float team2ResultMultipler = 0.0f;
            if (match.MatchEndResult == ArenaMatch.EMatchEndType.eMET_Draw)
            {
                team1ResultMultiplier = team2ResultMultipler = 0.5f;
            }
            else
            {
                if (match.Winner == match.Team1)
                {
                    team1ResultMultiplier = 1.0f;
                }
                else if (match.Winner == match.Team2)
                {
                    team2ResultMultipler = 1.0f;
                }
            }

            ArenaTeam team1 = match.Team1;
            ArenaTeam team2 = match.Team2;
            int team1Rating = team1.GetScore(match.ERestrictions,match.ERulesets,match.Templated);
            int team2Rating = team2.GetScore(match.ERestrictions, match.ERulesets, match.Templated);

            float team1ExpectedChance = GetExpectedChance(team1, team2, match.ERestrictions, match.ERulesets, match.Templated);
            float team2ExpectedChance = 1.0f - team1ExpectedChance;
            int newTeam1Rating = (int)(team1Rating + 32 * (team1ResultMultiplier - team1ExpectedChance));
            int newTeam2Rating = (int)(team2Rating + 32 * (team2ResultMultipler - team2ExpectedChance));
            team1.SetScore(match.ERestrictions, match.ERulesets, match.Templated, newTeam1Rating);
            team2.SetScore(match.ERestrictions, match.ERulesets, match.Templated, newTeam2Rating);
            team1Adj = newTeam1Rating - team1Rating;
            team2Adj = newTeam2Rating - team2Rating;
        }
        // [0,1]. eA + eB = 1
        private static float GetExpectedChance(ArenaTeam team1, ArenaTeam team2,
            EArenaMatchRestrictions restrictions, EArenaMatchEra era, bool templated)
        {
            int team1Rating = team1.GetScore(restrictions, era, templated);
            int team2Rating = team2.GetScore(restrictions, era, templated);
            float expectedA = (1.0f / (1.0f + (float)(Math.Pow(10, (double)(team2Rating - team1Rating) / 400))));
            if (expectedA < 0) expectedA *= -1;
            return expectedA;
        }
        /// <summary>
        /// Checks if a player isn't in an existing team with the specified size.
        /// A player can be a part of a 1v1, 2v2, and 3v3 team, but not 2x 2v2 teams.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="teamSize"></param>
        /// <returns></returns>
        public static bool CanPlayerJoinTeam(PlayerMobile player, int teamSize)
        {
            // 1. A player can not be part of another team whose size is the same as the size of their existing team.
            // e.g. a player can be a part of a 1v1, 2v2, and 3v3 team, but not 2x 2v2 teams.
            List<ArenaTeam> arenaTeams = GetTeamsForPlayer(player);
            foreach (ArenaTeam team in arenaTeams)
            {
                if (team.Capacity == teamSize)
                {
                    return false;
                }
            }
            return true;
        }

        public static ArenaTeam GetTeamForPlayer(PlayerMobile player, int teamSize)
        {
            List<ArenaTeam> arenaTeams = GetTeamsForPlayer(player);
            return arenaTeams.Find(x => x.Capacity == teamSize);
        }
        public static List<ArenaTeam> GetTeamsForPlayer(PlayerMobile player)
        {
            Serial ser = player.Serial;
            List<ArenaTeam> teamList = new List<ArenaTeam>();
            if (s_playerSerialToIndicesMap.ContainsKey(ser))
            {
                List<int> teamIndicesList = s_playerSerialToIndicesMap[ser];
                foreach (int index in teamIndicesList)
                {
                    if (s_indicesToArenaTeamMap.ContainsKey(index))
                    {
                        teamList.Add(s_indicesToArenaTeamMap[index]);
                    }
                }
            }
            return teamList;
        }

		public static void DisbandAndDelete(ArenaTeam team)
		{
			if (s_indicesToArenaTeamMap.ContainsKey(team.IndicesToArenaTeamMapIndex))
			{
				Debug.Assert(s_indicesToArenaTeamMap[team.IndicesToArenaTeamMapIndex] == team);

				// remove the team
				s_indicesToArenaTeamMap.Remove(team.IndicesToArenaTeamMapIndex);

				// online team members
				foreach (PlayerMobile p in team.Players)
					p.SendMessage(String.Format("Your Arena Team \"{0}\" has been disbanded.", team.TeamName));
			}
		}
        /// <summary>
        /// A complete verification is done to ensure that the members of the charter are not in
        /// an existing team of the charter's current size.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="charter"></param>
        /// <returns></returns>
        public static bool SubmitTeam(Mobile sender, ArenaTeamCharter charter)
        {
            // Verify charter.
            if (charter.TeamName.Length == 0)
            {
                sender.SendMessage("The charter was rejected because the team was empty!");
                return false;
            }
            if (charter.TeamName == ArenaTeamCharter.s_defaultName)
            {
                sender.SendMessage("The charter was rejected because of an invalid team name.");
                return false;
            }
            if (!(charter.TeamName.Length >= 2))
            {
                sender.SendMessage("The charter was rejected because the team name was less than 3 characters.");
                return false;
            }
			if (charter.TeamName.Length > 16)
			{
				sender.SendMessage("The charter was rejected because the team name was longer than 16 characters.");
				return false;
			}

            foreach (PlayerMobile m in charter.m_members)
            {
                if (!CanPlayerJoinTeam(m, charter.Capacity))
                {
                    sender.SendMessage("The charter was rejected because {0} already represents a {1}v{1} team.", m.Name, charter.Capacity);
                    return false;
                }
            }

			// linear lookup
			if (GetTeam_Slow(charter.TeamName) != null )
			{
				sender.SendMessage("The charter was rejected because the team name is already used.");
				return false;
			}

            // The ArenaTeam needs to be stored uniquely, and every player in the team needs
            // to have a reference to it saved.
            ArenaTeam team = new ArenaTeam(charter);
            int index = s_nextArenaTeamIndex++;
            if (!s_indicesToArenaTeamMap.ContainsKey(index))
            {
				team.IndicesToArenaTeamMapIndex = index; // Setting a reference back for the cases where we need to go from ArenaTeam to s_indicesToArenaTeamMap
				s_indicesToArenaTeamMap.Add(index, team);
			}
            else
            {
                throw new Exception("An arena team was allocated to a non-unique index.");
            }

            // Map the player to the unique index.
            foreach (PlayerMobile p in charter.m_members)
            {
                MapPlayerToArenaTeam(p.Serial, team, index);
                p.SendMessage( String.Format("Your {0}v{0} team {1} is now registered with the arena!", charter.m_members.Count, charter.TeamName));
            }

			// All team interactions are done from the Arena Menu from this point and on.
			charter.Delete();
            return true;
        }
        /// <summary>
        /// Adds the arena team to internal storage.
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="team"></param>
        private static void MapPlayerToArenaTeam(Serial serial, ArenaTeam team, int index)
        {
            if (!s_playerSerialToIndicesMap.ContainsKey(serial))
            {
                List<int> teamList = new List<int>();
                teamList.Add(index);
                s_playerSerialToIndicesMap.Add(serial, teamList);
            }
            else
            {
                s_playerSerialToIndicesMap[serial].Add(index);
            }
        }
        public static ArenaTemplate GetTemplate(PlayerMobile player)
        {
            return s_playerSerialToTemplatesMap.ContainsKey(player.Serial) ? s_playerSerialToTemplatesMap[player.Serial] : null;
        }
        public static void CacheTemplate(PlayerMobile player, ArenaTemplate template)
        {
            if (!s_playerSerialToTemplatesMap.ContainsKey(player.Serial))
            {
                s_playerSerialToTemplatesMap.Add(player.Serial, template);
            }
        }
        #region Queries
        public static ArenaMatch GetMatchForPlayer(PlayerMobile pm)
        {
            foreach (ArenaMatch match in s_pendingMatchesList)
            {
                if (match.ContainsPlayer(pm))
                    return match;
            }
            return null;
        }
        public static bool IsPlayerInMatch(PlayerMobile pm)
        {
            foreach(ArenaMatch match in s_pendingMatchesList)
            {
                if (match.ContainsPlayer(pm))
                    return true;
            }
            return false;
        }

        public static bool AllowBeneficial(Mobile source, Mobile target)
        {
            return GetNotoriety(source, target) == Notoriety.Ally;
        }

        public static int GetNotoriety(Mobile source, Mobile target)
        {
            PlayerMobile player1 = source as PlayerMobile;
            PlayerMobile player2 = target as PlayerMobile;
            if (player1 == null || player2 == null)
                return -1;

            foreach (ArenaMatch match in s_pendingMatchesList)
            {
                if (match.Team1Players.Contains(source))
                {
                    if (match.Team1Players.Contains(target))
                        return Notoriety.Ally;
                    else if (match.Team2Players.Contains(target))
                        return Notoriety.Enemy;
                }
                else if (match.Team2Players.Contains(source))
                {
                    if (match.Team2Players.Contains(target))
                        return Notoriety.Ally;
                    else if (match.Team1Players.Contains(target))
                        return Notoriety.Enemy;
                }
            }

            return -1;
        }
        public static bool IsPlayerInTemplatedMatch(PlayerMobile pm)
        {
            foreach (ArenaMatch match in s_pendingMatchesList)
            {
                if (match.ContainsPlayer(pm) && match.Templated)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion // Queries
    }

    public class ArenaTemplate
    {
        public static string[] s_statNames = new string[]
                {
                    "Str", "Dex", "Int",
                };
        public static string[] s_armorGroups = new string[]
                {
                    "Don't modify",
                    "None",
                    "Leather",
                    "Chainmail",
                    "Platemail",
                };

        public Serial m_serialId;
        public Pair<StatType, int>[] m_statList;
        public Pair<SkillName, int>[] m_skillList;
        public int m_armorGroup;

        public ArenaTemplate(Mobile from)
        {
            m_serialId = from.Serial;

            m_statList = new Pair<StatType, int>[]
            {
                new Pair<StatType,int>(StatType.Str,40),
                new Pair<StatType,int>(StatType.Dex,40),
                new Pair<StatType,int>(StatType.Int,40),
            };
            m_skillList = new Pair<SkillName, int>[]
            {
                new Pair<SkillName,int>(SkillName.Archery,0),
                new Pair<SkillName,int>(SkillName.Fencing,0),
                new Pair<SkillName,int>(SkillName.Macing,0),
                new Pair<SkillName,int>(SkillName.Swords,0),
                new Pair<SkillName,int>(SkillName.Wrestling,0),
                new Pair<SkillName,int>(SkillName.Tactics,0),
                new Pair<SkillName,int>(SkillName.Parry,0),
                new Pair<SkillName,int>(SkillName.Lumberjacking,0),
                new Pair<SkillName,int>(SkillName.Magery,0),
                new Pair<SkillName,int>(SkillName.MagicResist,0),
                new Pair<SkillName,int>(SkillName.EvalInt,0),
                new Pair<SkillName,int>(SkillName.Anatomy,0),
                new Pair<SkillName,int>(SkillName.Healing,0),
                new Pair<SkillName,int>(SkillName.Hiding,0),
                new Pair<SkillName,int>(SkillName.DetectHidden,0),
                new Pair<SkillName,int>(SkillName.Meditation,0),
                new Pair<SkillName,int>(SkillName.Poisoning,0),
                new Pair<SkillName,int>(SkillName.Snooping,0),
                new Pair<SkillName,int>(SkillName.Stealing,0),
            };

            m_armorGroup = 0;
        }
        public ArenaTemplate(BinaryFileReader reader)
        {
            m_serialId = reader.ReadInt();

            m_statList = new Pair<StatType, int>[3];
            for (int i = 0; i < 3; ++i)
            {
                StatType type = (StatType)reader.ReadInt();
                int val = reader.ReadInt();
                m_statList[i] = new Pair<StatType, int>((StatType)type, val);
            }

            int count = reader.ReadInt();
            m_skillList = new Pair<SkillName, int>[count];
            for (int i = 0; i < count; ++i)
            {
                SkillName skill = (SkillName)reader.ReadInt();
                int val = reader.ReadInt();
                m_skillList[i] = new Pair<SkillName, int>((SkillName)skill, val);
            }
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(m_serialId);
            foreach (Pair<StatType, int> pair in m_statList)
            {
                writer.Write((int)pair.First);
                writer.Write(pair.Second);
            }
            writer.Write(m_skillList.Length);
            foreach (Pair<SkillName, int> pair in m_skillList)
            {
                writer.Write((int)pair.First);
                writer.Write(pair.Second);
            }
        }
        
    }
}
