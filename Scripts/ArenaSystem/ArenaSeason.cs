using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

using Server.Mobiles;
using Server.Gumps;
using Server.Commands;
using Server.ArenaSystem;
using Server.Commands.Generic;
using Server.Achievements;

namespace Server.ArenaSystem
{
	//////////////////////////////////////////////////////////////////
	// An arena season, manages rewards and static data
	//////////////////////////////////////////////////////////////////
	public class ArenaSeason
	{
		public DateTime Begins { get; set; }
		public int NumTopTeamsRewarded { get; set; }
		public string SeasonName;
		public ArenaSeasonRewardPackage m_RewardsPackage;

		public void GivePrizeTo(PlayerMobile player, EArenaMatchEra era, EArenaMatchRestrictions restrictions, int teamcount, int season_rank)
		{
			// Put the price in the bank, this should never fail with the donationItem flag set to true
			Item[] prices = m_RewardsPackage.CreatePrizes(era, restrictions, teamcount, season_rank);
			foreach (Item price in prices)
			{
				if (!(price is Server.Items.Gold))
				{
					price.BlessedFor = player;
					price.DonationItem = true; // bank box wont reject donation items even if it's full
				}

				if (player.BankBox.TryDropItem(player, price, false))
				{
					// could possibly set the donation-flag here to false again.
					player.SendMessage(44, "An arena season reward has been placed in your bankbox");
					price.DonationItem = false;
				}
			}

			// achievement for winners
			if (season_rank == 1)
			{
				AchievementTriggers trigger = teamcount == 1 ? AchievementTriggers.Trigger_ArenaChampion1v1 : teamcount == 2 ? AchievementTriggers.Trigger_ArenaChampion2v2 : AchievementTriggers.Trigger_ArenaChampion3v3;
				AchievementSystem.Instance.TickProgress(player, trigger);

			}
		}

		public void GivePrizeTo(ArenaTeam team, EArenaMatchEra era, EArenaMatchRestrictions restrictions, int teamcount, int season_rank)
		{
			foreach (PlayerMobile player in team.Players)
			{
				GivePrizeTo(player, era, restrictions, teamcount, season_rank);
			}
		}
	}


	//////////////////////////////////////////////////////////////////
	// Serialization of previous arena season winners
	//////////////////////////////////////////////////////////////////	
	public class PreviousSeasonWinners
	{
		public int m_SeasonIndex;
		public string[,] m_Last1v1Winners = new string[(int)EArenaMatchEra.eAMR_NumEras, (int)EArenaMatchRestrictions.eAMC_NumRestrictions];
		public string[,] m_Last2v2Winners = new string[(int)EArenaMatchEra.eAMR_NumEras, (int)EArenaMatchRestrictions.eAMC_NumRestrictions];
		public string[,] m_Last3v3Winners = new string[(int)EArenaMatchEra.eAMR_NumEras, (int)EArenaMatchRestrictions.eAMC_NumRestrictions];

		public void Serialize(GenericWriter writer)
		{
			writer.Write(m_SeasonIndex);
			writer.Write((int)3); // num team combinations, i.e 1v1, 2v2 and 3v3 == 3
			writer.Write((int)EArenaMatchEra.eAMR_NumEras);
			writer.Write((int)EArenaMatchRestrictions.eAMC_NumRestrictions);
			for (int era = 0; era < (int)EArenaMatchEra.eAMR_NumEras; ++era)
			{
				for (int restriction = 0; restriction < (int)EArenaMatchRestrictions.eAMC_NumRestrictions; ++restriction)
				{
					writer.Write(m_Last1v1Winners[era, restriction]);
					writer.Write(m_Last2v2Winners[era, restriction]);
					writer.Write(m_Last3v3Winners[era, restriction]);
				}
			}
		}
		public void Deserialize(GenericReader reader)
		{
			m_SeasonIndex = reader.ReadInt();
			int num_team_combos_written = reader.ReadInt(); // num team combinations, i.e 1v1, 2v2 and 3v3 == 3
			int num_eras_written = reader.ReadInt();
			int num_restr_written = reader.ReadInt();

			// num written can be less BUT NOT MORE than EArenaMatchEra.numEras for example.
			Debug.Assert(num_eras_written <= (int)EArenaMatchEra.eAMR_NumEras);
			Debug.Assert(num_restr_written <= (int)EArenaMatchRestrictions.eAMC_NumRestrictions);

			for (int era = 0; era < num_eras_written; ++era)
			{
				for (int restriction = 0; restriction < num_restr_written; ++restriction)
				{
					if (num_team_combos_written > 0)
						m_Last1v1Winners[era, restriction] = reader.ReadString();
					if (num_team_combos_written > 1)
						m_Last2v2Winners[era, restriction] = reader.ReadString();
					if (num_team_combos_written > 2)
						m_Last3v3Winners[era, restriction] = reader.ReadString();
				}
			}
		}
	}

	//////////////////////////////////////////////////////////////////
	// ArenaSeasonTracker - Manages and cycles seasons
	//////////////////////////////////////////////////////////////////
	public static class ArenaSeasonTracker
	{
        private static string s_SeasonsDataPath = "Saves/ArenaSystem/arenaSeasons.dat";
		private static bool m_IsRunningTestSeason;
		private static Timer m_SeasonCheckEndTimer;
		private static List<ArenaSeason> m_Seasons;
		
		private static int m_CurrentActiveSeason;

		private static List<PreviousSeasonWinners> m_PreviousWinners;

		public static int LastSeasonIndex { get { return m_CurrentActiveSeason - 1; } }

		public static DateTime CurrentSeasonEnd { get { return m_Seasons.Count > m_CurrentActiveSeason + 1 ? m_Seasons[m_CurrentActiveSeason + 1].Begins : new DateTime(2020,1,1); } }
		public static List<ArenaSeason> Seasons { get { return m_Seasons; } }

		public static void CreateTestSeasons()
		{
			m_Seasons = new List<ArenaSeason>();
			for (int i = 0; i < 5000; ++i)
			{
				m_Seasons[i] = new ArenaSeason();
				m_Seasons[i].Begins = DateTime.Now + TimeSpan.FromDays(i);
				m_Seasons[i].SeasonName = String.Format("Test Season {0} (1 day long)", i + 1);
				m_Seasons[i].NumTopTeamsRewarded = Utility.Random(1, 5);
				m_Seasons[i].m_RewardsPackage = new DefaultRewardPackage();
			}
		}

		public static void CreateSeasons()
		{
			// initialize seasons. Completely coded AND RELIANT ON INDICES!
			// Don't remove seasons! Just add more new ones at the end of the list.
            m_Seasons = new List<ArenaSeason>
			{
			    new ArenaSeason(){ Begins = new DateTime(2014, 1, 1, 23,0,0), SeasonName="Beta Arena Season 1", NumTopTeamsRewarded = 3, m_RewardsPackage = new BetaRewardPackage() },
			    new ArenaSeason(){ Begins = new DateTime(2014, 1, 14, 23,0,0), SeasonName="Beta Arena Season 2", NumTopTeamsRewarded = 3, m_RewardsPackage = new BetaRewardPackage() },
			    new ArenaSeason(){ Begins = new DateTime(2014, 1, 21, 23,0,0), SeasonName="Beta Arena Season 3", NumTopTeamsRewarded = 3, m_RewardsPackage = new BetaRewardPackage() },
			    new ArenaSeason(){ Begins = new DateTime(2014, 1, 31, 23,0,0), SeasonName="February Arena Season", NumTopTeamsRewarded = 3, m_RewardsPackage = new FebruaryRewardPackage() },
			    new ArenaSeason(){ Begins = new DateTime(2014, 3, 1, 23,0,0), SeasonName="March Arena Season", NumTopTeamsRewarded = 3, m_RewardsPackage = new MarchRewardPackage() },
			    new ArenaSeason(){ Begins = new DateTime(2014, 4, 1, 23,0,0), SeasonName="April Arena Season", NumTopTeamsRewarded = 3, m_RewardsPackage = new AprilRewardPackage() },
			    new ArenaSeason(){ Begins = new DateTime(2014, 5, 1, 23,0,0), SeasonName="May Arena Season", NumTopTeamsRewarded = 3, m_RewardsPackage = new MayRewardPackage() },
			    new ArenaSeason(){ Begins = new DateTime(2014, 6, 1, 23,0,0), SeasonName="June Arena Season", NumTopTeamsRewarded = 3, m_RewardsPackage = new JuneRewardPackage() },
			    new ArenaSeason(){ Begins = new DateTime(2014, 7, 1, 23,0,0), SeasonName="July Arena Season", NumTopTeamsRewarded = 3, m_RewardsPackage = new JulyRewardPackage() },
			    new ArenaSeason(){ Begins = new DateTime(2014, 8, 1, 23,0,0), SeasonName="August Arena Season", NumTopTeamsRewarded = 3, m_RewardsPackage = new AugustRewardPackage() },
			    new ArenaSeason(){ Begins = new DateTime(2014, 9, 1, 23,0,0), SeasonName="September Arena Season", NumTopTeamsRewarded = 3, m_RewardsPackage = new SeptemberRewardPackage() },
			    new ArenaSeason(){ Begins = new DateTime(2014, 10, 1, 23,0,0), SeasonName="October Arena Season", NumTopTeamsRewarded = 3, m_RewardsPackage = new OctoberRewardPackage() },
			    new ArenaSeason(){ Begins = new DateTime(2014, 11, 1, 23,0,0), SeasonName="November Arena Season", NumTopTeamsRewarded = 3, m_RewardsPackage = new DefaultRewardPackage() },
			    new ArenaSeason(){ Begins = new DateTime(2014, 12, 1, 23,0,0), SeasonName="December Arena Season", NumTopTeamsRewarded = 3, m_RewardsPackage = new DefaultRewardPackage() },
			};

            // next 15 years of seasons lol
            for (int i = 2015; i < 2030; i++ )
            {
                m_Seasons.AddRange(new List<ArenaSeason>() {
                    new ArenaSeason() { Begins = new DateTime(i, 1, 1, 23, 0, 0), SeasonName = "January Arena Season", NumTopTeamsRewarded = 3, m_RewardsPackage = new JanuaryRewardPackage() },
                    new ArenaSeason() { Begins = new DateTime(i, 2, 1, 23, 0, 0), SeasonName = "February Arena Season", NumTopTeamsRewarded = 3, m_RewardsPackage = new FebruaryRewardPackage() },
                    new ArenaSeason() { Begins = new DateTime(i, 3, 1, 23, 0, 0), SeasonName = "March Arena Season", NumTopTeamsRewarded = 3, m_RewardsPackage = new MarchRewardPackage() },
                    new ArenaSeason() { Begins = new DateTime(i, 4, 1, 23, 0, 0), SeasonName = "April Arena Season", NumTopTeamsRewarded = 3, m_RewardsPackage = new AprilRewardPackage() },
                    new ArenaSeason() { Begins = new DateTime(i, 5, 1, 23, 0, 0), SeasonName = "May Arena Season", NumTopTeamsRewarded = 3, m_RewardsPackage = new MayRewardPackage() },
                    new ArenaSeason() { Begins = new DateTime(i, 6, 1, 23, 0, 0), SeasonName = "June Arena Season", NumTopTeamsRewarded = 3, m_RewardsPackage = new JuneRewardPackage() },
                    new ArenaSeason() { Begins = new DateTime(i, 7, 1, 23, 0, 0), SeasonName = "July Arena Season", NumTopTeamsRewarded = 3, m_RewardsPackage = new JulyRewardPackage() },
                    new ArenaSeason() { Begins = new DateTime(i, 8, 1, 23, 0, 0), SeasonName = "August Arena Season", NumTopTeamsRewarded = 3, m_RewardsPackage = new AugustRewardPackage() },
                    new ArenaSeason() { Begins = new DateTime(i, 9, 1, 23, 0, 0), SeasonName = "September Arena Season", NumTopTeamsRewarded = 3, m_RewardsPackage = new SeptemberRewardPackage() },
                    new ArenaSeason() { Begins = new DateTime(i, 10, 1, 23, 0, 0), SeasonName = "October Arena Season", NumTopTeamsRewarded = 3, m_RewardsPackage = new OctoberRewardPackage() },
                    new ArenaSeason() { Begins = new DateTime(i, 11, 1, 23, 0, 0), SeasonName = "November Arena Season", NumTopTeamsRewarded = 3, m_RewardsPackage = new NovemberRewardPackage() },
                    new ArenaSeason() { Begins = new DateTime(i, 12, 1, 23, 0, 0), SeasonName = "December Arena Season", NumTopTeamsRewarded = 3, m_RewardsPackage = new DecemberRewardPackage() }
                });
            }
		}

		[Usage("[ArenaStartRegularSeason ")]
		[Description("Start test seasons, 1 hour long each")]
		public static void StartArenaSeasonTest(CommandEventArgs e)
		{
			m_CurrentActiveSeason = 0;
			m_IsRunningTestSeason = true;
			CreateTestSeasons();
			if( e != null && e.Mobile != null )
				e.Mobile.SendMessage("Debug arena seasons started");
		}

		[Usage("[ArenaResumeRegularSeason seasonIdx")]
		[Description("Start regular arena season")]
		public static void StartRegularSeason(CommandEventArgs e)
		{
			if (e.Arguments.Length <= 0)
			{
				e.Mobile.SendMessage("Usage ArenaResumeRegularSeason <int seasonIndex>");
				return;
			}
			int season;
			if (!Int32.TryParse(e.Arguments[0], out season))
				e.Mobile.SendMessage("Invalid season index");

			m_IsRunningTestSeason = false;
			m_CurrentActiveSeason = season;
			CreateSeasons();
			e.Mobile.SendMessage("Regular arena season set and started");
		}

		/*
		[Usage("[TestArenaWinner seasonIdx teamcount restriction")]
		[Description("temp")]
		public static void TestGenPrice(CommandEventArgs e)
		{
			if (e.Arguments.Length != 3)
			{
				e.Mobile.SendMessage("<seasonIdx teamcount restriction>");
				return;
			}
			int season, restriction, teamcount;
			if (!Int32.TryParse(e.Arguments[0], out season))
				e.Mobile.SendMessage("Invalid season index");
			if (!Int32.TryParse(e.Arguments[1], out teamcount))
				e.Mobile.SendMessage("Invalid restriction index");
			if (!Int32.TryParse(e.Arguments[2], out restriction))
				e.Mobile.SendMessage("Invalid place");

			ArenaSeasonTracker.Seasons[season].GivePrizeTo(e.Mobile as PlayerMobile, EArenaMatchEra.eAMR_IPY, (EArenaMatchRestrictions)restriction, teamcount, 0);
		}
		*/

		public static void Initialize()
		{
			// Events
			EventSink.ServerStarted += new ServerStartedEventHandler(OnWorldPostLoad);
			EventSink.WorldSave += new WorldSaveEventHandler(OnWorldSave);

			// default values
			m_CurrentActiveSeason = 0;
			m_PreviousWinners = new List<PreviousSeasonWinners>();

			CommandSystem.Register("ArenaStartSeasonQuickTest", AccessLevel.GameMaster, new CommandEventHandler(StartArenaSeasonTest));
			CommandSystem.Register("ArenaStartRegularSeason", AccessLevel.GameMaster, new CommandEventHandler(StartRegularSeason));
		}



		public static void CheckSeasonEnd()
		{
			// since all seasons are back to back we're actually checking if the next season has started.
			// if so finish the current one and kickoff the next
			DateTime now = DateTime.Now; // LOCAL TIME
			if(m_Seasons.Count == m_CurrentActiveSeason+1) //last defined season! BOOO!
			{
				//World.Broadcast(0x20, true, "Current arena season can not end as there are no more seasons defined. Notify a GM...");
			}
			else if (now > m_Seasons[m_CurrentActiveSeason+1].Begins )
			{
				// next season should be started. Finish current and kick off the next
				OnSeasonFinished();
			}
		}

		public static ArenaSeason GetCurrentSeason()
		{
			return m_CurrentActiveSeason != -1 ? m_Seasons[m_CurrentActiveSeason] : null;
		}

		private static void OnSeasonFinished()
		{
			// generate final leaderboards
			World.Broadcast(0x35, true, "Calculating final season leaderboards, please wait.");
			ArenaLeaderboards.GenerateLeaderboards();
			
			// award
			AwardWinners();

			// clear all seasonal stats (scores and categorized W/L) for all teams
			ArenaSystem.ResetSeasonalStats();
			
			// generate new "clean" leaderboards
			ArenaLeaderboards.GenerateLeaderboards();

			// kick new
			World.Broadcast(0x20, false, "Arena Season {0} has ENDED!", m_Seasons[m_CurrentActiveSeason].SeasonName);
			StartNewSeason();
		}

		private static void AwardWinners()
		{
			PreviousSeasonWinners season_winner_data = new PreviousSeasonWinners();
			season_winner_data.m_SeasonIndex = m_CurrentActiveSeason;

			// Here we simply set a value on the PlayerMobil that he has unclaimed arena rewards.
			// This will enable a gump somewhere else that is used to claim the price(s).
			for (int era = 0; era < (int)EArenaMatchEra.eAMR_NumEras; ++era)
			{
				for (int restriction = 0; restriction < (int)EArenaMatchRestrictions.eAMC_NumRestrictions; ++restriction)
				{
					//1v1 winners
					season_winner_data.m_Last1v1Winners[era, restriction] = AwardTopRankedTeams((EArenaMatchEra)era, (EArenaMatchRestrictions)restriction, 1);
					//2v2 winners
					season_winner_data.m_Last2v2Winners[era, restriction] = AwardTopRankedTeams((EArenaMatchEra)era, (EArenaMatchRestrictions)restriction, 2);
					//3v3 winners
					season_winner_data.m_Last3v3Winners[era, restriction] = AwardTopRankedTeams((EArenaMatchEra)era, (EArenaMatchRestrictions)restriction, 3);
				}
			}
			m_PreviousWinners.Add(season_winner_data);
		}

		private static string AwardTopRankedTeams(EArenaMatchEra era, EArenaMatchRestrictions restrictions, int team_count)
		{
			List<ArenaLeaderboards.Entry> leaderboard = ArenaLeaderboards.GetLeaderboard(era, restrictions, team_count);
			ArenaSeason season = m_Seasons[m_CurrentActiveSeason];
			int season_rank = 1;
			string firstplace_teamname = "None";
			for (int i = 0; i < leaderboard.Count; ++i)
			{
				ArenaTeam team = ArenaSystem.GetTeam_Slow(leaderboard[i].name);
				if (team == null || team.GetWinsFor(era, restrictions) == 0)
					continue; // deleted/disbanded team or for some reason the team had 0 wins but still scored high. weird.

				season.GivePrizeTo(team, era, restrictions, team.Capacity, season_rank);

				// store winner history.
				if (season_rank == 1)
					firstplace_teamname = team.TeamName;

				if (++season_rank >= season.NumTopTeamsRewarded)
					break; // done
			}
			return firstplace_teamname;
		}

		private static void StartNewSeason()
		{
			Debug.Assert(m_Seasons.Count > m_CurrentActiveSeason + 1); // must be handled earlier
			m_CurrentActiveSeason += 1;
			Timer.DelayCall(TimeSpan.FromSeconds(5), () => { World.Broadcast(0x20, false, "Arena Season \"{0}\" has STARTED! ", m_Seasons[m_CurrentActiveSeason].SeasonName); } );
			Timer.DelayCall(TimeSpan.FromSeconds(8), () => { World.Broadcast(0x20, false, "All leaderboards and rankings have been reset. ", m_Seasons[m_CurrentActiveSeason].SeasonName); });
		}

		public static PreviousSeasonWinners GetPreviousSeasonWinners(int season_index)
		{
			return (m_Seasons.Count > season_index && season_index >= 0) ? m_PreviousWinners[season_index] : null;
		}

		private static void OnWorldPostLoad()
		{
			// ArenaTeams
			if (File.Exists(s_SeasonsDataPath))
			{
				FileStream fs = new FileStream(s_SeasonsDataPath, FileMode.Open, FileAccess.Read, FileShare.Read);
				BinaryFileReader reader = new BinaryFileReader(new BinaryReader(fs));

				//version
				int version = reader.ReadInt();

				switch (version)
				{
					case 1:
					{
						m_IsRunningTestSeason = reader.ReadBool();
						goto case 0;
					}
					case 0:
					{
						// active season
						m_CurrentActiveSeason = reader.ReadInt();

						// previous season winners
						int num_previous_winners = reader.ReadInt();
						for (int i = 0; i < num_previous_winners; ++i)
						{
							PreviousSeasonWinners winner = new PreviousSeasonWinners();
							winner.Deserialize(reader);
							m_PreviousWinners.Add(winner);
						}
						break;
					}
				}
				reader.Close();
			}


			// Initialize seasons
			if (m_IsRunningTestSeason)
				StartArenaSeasonTest(null);
			else
				CreateSeasons();

			foreach (ArenaSeason s in m_Seasons)
				Debug.Assert(s.m_RewardsPackage != null);

			// kick off the periodic season timer
			//if (System.Diagnostics.Debugger.IsAttached)
			//{
			//    m_SeasonCheckEndTimer = Timer.DelayCall(TimeSpan.FromMinutes(5), TimeSpan.FromSeconds(5), CheckSeasonEnd);
			//}
			//else
			{
				m_SeasonCheckEndTimer = Timer.DelayCall(TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(2), CheckSeasonEnd);
			}
		}

		private static void OnWorldSave(WorldSaveEventArgs e)
		{
			(new FileInfo(s_SeasonsDataPath)).Directory.Create();

			// ArenaTeams
			try
			{
				FileStream fs = new FileStream(s_SeasonsDataPath, FileMode.Create, FileAccess.Write, FileShare.None);
				GenericWriter writer = new BinaryFileWriter(fs, true);
				writer.Write((int)1); // Version

				///
				///	Version 1
				/// 
				writer.Write(m_IsRunningTestSeason);

				///
				///	Version 0
				/// 
				// current active season (index into m_Seasons)
				writer.Write(m_CurrentActiveSeason);

				// previous season winners
				writer.Write((int)m_PreviousWinners.Count);
				foreach(PreviousSeasonWinners winner in m_PreviousWinners)
					winner.Serialize(writer);
				
				writer.Close();
			}
			catch (Exception exc)
			{
				ArenaUtilities.WriteError(exc, "Failed to correctly write {0}. Tell Jimmy.", s_SeasonsDataPath);
			}
		}
	}
}
