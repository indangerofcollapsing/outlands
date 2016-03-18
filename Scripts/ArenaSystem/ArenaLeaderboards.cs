using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Server.Accounting;
using Server.Achievements;
using Server.Mobiles;
using Server.Gumps;
using Server.Commands;
using Server.ArenaSystem;
using Server.Network;

namespace Server.ArenaSystem
{
	/// <summary>
	/// LEADERBOARDS MANAGER-THINGIE
	/// </summary>
	public static class ArenaLeaderboards
	{
		public class Entry : IComparable<Entry>
		{
			public string name;				// team name
			public string display_name;		// the name displayed on the leaderboards. For 1v1 this is the characters name
			public int score { get { return iscore; } set { iscore = value; sscore = value.ToString(); } }

			private int iscore;
			public int wins;
			public string sscore; // to avoid converting int to str every time a user opens the gump.

			public int CompareTo(Entry other)
			{
				if (iscore != other.iscore)
					return other.iscore - iscore;
				else
					return other.wins - wins;
			}
		}

		public static DateTime m_NextGenTime; // Simply set to 30 minutes ahead each time leaderboards are generated. Based on the assumption that we save ever 30mins

		public static List<Entry>[,] m_1v1_Ranks = new List<Entry>[(int)EArenaMatchEra.eAMR_NumEras, (int)EArenaMatchRestrictions.eAMC_NumRestrictions];
		public static List<Entry>[,] m_2v2_Ranks = new List<Entry>[(int)EArenaMatchEra.eAMR_NumEras, (int)EArenaMatchRestrictions.eAMC_NumRestrictions];
		public static List<Entry>[,] m_3v3_Ranks = new List<Entry>[(int)EArenaMatchEra.eAMR_NumEras, (int)EArenaMatchRestrictions.eAMC_NumRestrictions];

		public static void InitializeToCapacity(int capacity)
		{
			for (int i = 0; i < (int)EArenaMatchEra.eAMR_NumEras; ++i)
			{
				for (int j = 0; j < (int)EArenaMatchRestrictions.eAMC_NumRestrictions; ++j)
				{
					m_1v1_Ranks[i, j] = new List<Entry>(capacity);
					m_2v2_Ranks[i, j] = new List<Entry>(capacity);
					m_3v3_Ranks[i, j] = new List<Entry>(capacity);
				}
			}

			m_NextGenTime = new DateTime();
		}

		public static List<Entry> GetLeaderboard(EArenaMatchEra era, EArenaMatchRestrictions restriction, int versus_count)
		{
			if (versus_count == 1)
				return m_1v1_Ranks[(int)era, (int)restriction];
			else if (versus_count == 2 )
				return m_2v2_Ranks[(int)era, (int)restriction];
			else if (versus_count == 3)
				return m_3v3_Ranks[(int)era, (int)restriction];
			return null;
		}

		static List<Entry>[,] GetLeaderboards(int versus_count)
		{
			if (versus_count == 1)
				return m_1v1_Ranks;
			else if (versus_count == 2)
				return m_2v2_Ranks;
			else if (versus_count == 3)
				return m_3v3_Ranks;
			else
				return null;
		}

		// Slow'ish. Do during world load/save
		public static void GenerateLeaderboards()
		{
			// clear all containers
			for(int i = 0; i < (int)EArenaMatchEra.eAMR_NumEras; ++i)
			{
				for(int j = 0; j < (int)EArenaMatchRestrictions.eAMC_NumRestrictions; ++j)
				{
					m_1v1_Ranks[i,j].Clear();
					m_2v2_Ranks[i,j].Clear();
					m_3v3_Ranks[i,j].Clear();
				}
			}

			// add teamdata to appropriate sorted container
			foreach (ArenaTeam team in ArenaSystem.GetAllTeams())
			{
                if (team == null || team.TeamName == null) continue;

				List<Entry>[,] lb = GetLeaderboards(team.Capacity);
				string dn = team.TeamName;

				lb[(int)EArenaMatchEra.eAMR_IPY, (int)EArenaMatchRestrictions.eAMC_Order].Add(new Entry() { score = team.GetScore(EArenaMatchRestrictions.eAMC_Order, EArenaMatchEra.eAMR_IPY, false), wins = team.GetWinsFor(EArenaMatchEra.eAMR_IPY, EArenaMatchRestrictions.eAMC_Order), name = team.TeamName, display_name = dn });
				lb[(int)EArenaMatchEra.eAMR_T2A, (int)EArenaMatchRestrictions.eAMC_Order].Add(new Entry() { score = team.GetScore(EArenaMatchRestrictions.eAMC_Order, EArenaMatchEra.eAMR_T2A, false), wins = team.GetWinsFor(EArenaMatchEra.eAMR_T2A, EArenaMatchRestrictions.eAMC_Order), name = team.TeamName, display_name = dn });
				lb[(int)EArenaMatchEra.eAMR_Pub16, (int)EArenaMatchRestrictions.eAMC_Order].Add(new Entry() { score = team.GetScore(EArenaMatchRestrictions.eAMC_Order, EArenaMatchEra.eAMR_Pub16, false), wins = team.GetWinsFor(EArenaMatchEra.eAMR_Pub16, EArenaMatchRestrictions.eAMC_Order), name = team.TeamName, display_name = dn });

				lb[(int)EArenaMatchEra.eAMR_IPY, (int)EArenaMatchRestrictions.eAMC_Chaos].Add(new Entry() { score = team.GetScore(EArenaMatchRestrictions.eAMC_Chaos, EArenaMatchEra.eAMR_IPY, false), wins = team.GetWinsFor(EArenaMatchEra.eAMR_IPY, EArenaMatchRestrictions.eAMC_Chaos), name = team.TeamName, display_name = dn });
				lb[(int)EArenaMatchEra.eAMR_T2A, (int)EArenaMatchRestrictions.eAMC_Chaos].Add(new Entry() { score = team.GetScore(EArenaMatchRestrictions.eAMC_Chaos, EArenaMatchEra.eAMR_T2A, false), wins = team.GetWinsFor(EArenaMatchEra.eAMR_T2A, EArenaMatchRestrictions.eAMC_Chaos), name = team.TeamName, display_name = dn });
				lb[(int)EArenaMatchEra.eAMR_Pub16, (int)EArenaMatchRestrictions.eAMC_Chaos].Add(new Entry() { score = team.GetScore(EArenaMatchRestrictions.eAMC_Chaos, EArenaMatchEra.eAMR_Pub16, false), wins = team.GetWinsFor(EArenaMatchEra.eAMR_Pub16, EArenaMatchRestrictions.eAMC_Chaos), name = team.TeamName, display_name = dn });
			}

			// and sort..
			// clear all containers
			for (int i = 0; i < (int)EArenaMatchEra.eAMR_NumEras; ++i)
			{
				for (int j = 0; j < (int)EArenaMatchRestrictions.eAMC_NumRestrictions; ++j)
				{
					m_1v1_Ranks[i, j].Sort();
					m_2v2_Ranks[i, j].Sort();
					m_3v3_Ranks[i, j].Sort();
				}
			}

			m_NextGenTime = DateTime.UtcNow + TimeSpan.FromMinutes(30);
		}
	}

	/// <summary>
	/// LEADERBOARDS GUMP
	/// </summary>
	public class ArenaLeaderboardsGump : Server.Gumps.Gump
	{
		int m_StartIdx;
		EArenaMatchEra m_Era;
		EArenaMatchRestrictions m_Restrictions;
		private static int ENTRIES_PER_PAGE = 10;

		private static string[] m_Months = new string[12] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
		
		public ArenaLeaderboardsGump(Mobile from, EArenaMatchEra era, EArenaMatchRestrictions restrictions, int startidx)
			: base(0, 0)
		{
			if (era == EArenaMatchEra.eAMR_NumEras)
				era = EArenaMatchEra.eAMR_IPY;

			if (restrictions == EArenaMatchRestrictions.eAMC_NumRestrictions)
				restrictions = EArenaMatchRestrictions.eAMC_Order;

			startidx = Math.Max(0, startidx);
			int endidx = startidx + ENTRIES_PER_PAGE;
			m_StartIdx = startidx;
			m_Era = era;
			m_Restrictions = restrictions;

			// fetch leaderboard data
			List<ArenaLeaderboards.Entry> lb_1v1 = ArenaLeaderboards.GetLeaderboard(era, restrictions, 1);
			List<ArenaLeaderboards.Entry> lb_2v2 = ArenaLeaderboards.GetLeaderboard(era, restrictions, 2);
			List<ArenaLeaderboards.Entry> lb_3v3 = ArenaLeaderboards.GetLeaderboard(era, restrictions, 3);

			this.Closable = true;
			this.Disposable = true;
			this.Dragable = true;
			this.Resizable = false;
			this.AddPage(0);
			this.AddImage(23, 44, 206);
			this.AddImageTiled(127, 147, 21, 21, 200);
			this.AddImageTiled(67, 85, 539, 424, 200);
			this.AddImage(66, 44, 201);
			this.AddImage(178, 44, 201);
			this.AddImage(605, 82, 203);
			this.AddImage(605, 465, 205);
			this.AddImage(605, 149, 203);
			this.AddImage(22, 465, 204);
			this.AddImage(604, 44, 207);
			this.AddImage(178, 465, 233);
			this.AddImage(66, 465, 233);
			this.AddImage(251, 21, 1419);
			this.AddImage(327, 5, 1417);
			this.AddImage(338, 15, 5608);
			this.AddImage(23, 88, 202);
			this.AddImage(23, 149, 202);
			this.AddImage(615, 77, 10441);
			this.AddBackground(134, 92, 466, 367, 9200);


			// HUES
			int optcat_hue = 2036;
			int optopt_hue = 0;
			int vv_hue = 2036;
			int mainlabel_hue = 55;
			int selected_type_hue = 2036;
			int champion_line_hue = 55;
			int champion_name_hue = 2400;

			int name_hue = 2036;
			int leadername_hue = 53;

			int nextupdate_hue = 48;
			int ranklabel_hue = 2036;

			// Season end
			DateTime end = ArenaSeasonTracker.CurrentSeasonEnd;
			this.AddLabel(285, 458, nextupdate_hue, String.Format("Season ends: {0} {1}", m_Months[end.Month-1], end.Day));
			
			// Next update info
			TimeSpan timetonext = ArenaLeaderboards.m_NextGenTime - DateTime.UtcNow;
			this.AddLabel(235, 472, 2036, String.Format("Leaderboards will be updated in {0} minutes", (int)(timetonext.TotalMinutes + 1.0)));


			AddGroup(0);
			this.AddRadio(94, 116, 2151, 2153, (era == EArenaMatchEra.eAMR_IPY), (int)Buttons.CheckIPY);
			this.AddRadio(94, 147, 2151, 2153, (era == EArenaMatchEra.eAMR_Pub16), (int)Buttons.CheckUOR);
			this.AddLabel(46, 99, optcat_hue, @"Era");
			this.AddLabel(53, 121, optopt_hue, @"UOAC");
			this.AddLabel(53, 152, optopt_hue, @"UO:R");

			//this.AddRadio(94, 178, 2151, 2153, (era == EArenaMatchEra.eAMR_T2A), (int)Buttons.CheckT2A);

			AddGroup(1);
			this.AddLabel(46, 186, optcat_hue, @"Rules");
			this.AddLabel(53, 208, optopt_hue, @"Order");
			this.AddLabel(53, 238, optopt_hue, @"Chaos"); 
			this.AddRadio(94, 203, 2151, 2153, (restrictions == EArenaMatchRestrictions.eAMC_Order), (int)Buttons.CheckOrder);
			this.AddRadio(94, 233, 2151, 2153, (restrictions == EArenaMatchRestrictions.eAMC_Chaos), (int)Buttons.CheckChaos);
			this.AddButton(59, 275, 2076, 2075, (int)Buttons.ButtonSubmit, GumpButtonType.Reply, 0);


			int xtop = 280;
			this.AddLabel(310, 100, mainlabel_hue, ArenaSeasonTracker.GetCurrentSeason().SeasonName);
			this.AddLabel(335, 121, selected_type_hue, String.Format("{0} - {1}", EraToStr(era), restrictions == EArenaMatchRestrictions.eAMC_Order ? "Order" : "Chaos"));

			this.AddLabel(208, 152, vv_hue, @"1 vs 1");
			this.AddLabel(349, 152, vv_hue, @"2 vs 2");
			this.AddLabel(506, 152, vv_hue, @"3 vs 3");	

			// dividers
			this.AddImageTiled(143, 176, 448, 3, 9102);
			this.AddImageTiled(143, 218, 448, 3, 9102);
			this.AddImageTiled(290, 178, 1, 268, 9102);
			this.AddImageTiled(441, 178, 1, 268, 9102);

			// Reigning champions
			PreviousSeasonWinners champions = ArenaSeasonTracker.GetPreviousSeasonWinners(ArenaSeasonTracker.LastSeasonIndex);
			this.AddLabel(142, 180, champion_line_hue, "Reigning Champion");
			this.AddLabel(296, 180, champion_line_hue, "Reigning Champions");
			this.AddLabel(446, 180, champion_line_hue, "Reigning Champions");
			this.AddLabel(142, 199, champion_name_hue, champions != null ? champions.m_Last1v1Winners[(int)era, (int)restrictions] : "None");
			this.AddLabel(296, 199, champion_name_hue, champions != null ? champions.m_Last2v2Winners[(int)era, (int)restrictions] : "None");
			this.AddLabel(446, 199, champion_name_hue, champions != null ? champions.m_Last3v3Winners[(int)era, (int)restrictions] : "None");
			
			// pagescrollers
			if (startidx > 0)
				this.AddButton(246, 439, 2223, 2223, (int)Buttons.BtnPrevPage, GumpButtonType.Reply, 0);
			this.AddButton(267, 439, 2224, 2224, (int)Buttons.BtnNextPage, GumpButtonType.Reply, 0);
			this.AddLabel(139, 436, ranklabel_hue, String.Format("Rank {0} - {1}", m_StartIdx+1, endidx));

			// add top teams
			int ystart = 222;
			int ydelta = 20;
			int x1v1name = 142;
			int x1v1score = 253;
			int x2v2name = 296;
			int x2v2score = 402;
			int x3v3name = 446;
			int x3v3score = 554;

			int y = ystart;
			for (int i = m_StartIdx; i < Math.Min(lb_1v1.Count, endidx); ++i)
			{
				int hue = i < 1 ? leadername_hue + i + (1 * i) : name_hue; // 
				this.AddLabel(x1v1name, y, hue, lb_1v1[i].display_name);
				this.AddLabel(x1v1score, y, hue, lb_1v1[i].sscore);
				y += ydelta;
			}

			y = ystart;
			for (int i = m_StartIdx; i < Math.Min(lb_2v2.Count, endidx); ++i)
			{
				int hue = i < 1 ? leadername_hue + i + (1 * i) : name_hue;
				this.AddLabel(x2v2name, y, hue, lb_2v2[i].name);
				this.AddLabel(x2v2score, y, hue, lb_2v2[i].sscore);
				y += ydelta;
			}

			y = ystart;
			for (int i = m_StartIdx; i < Math.Min(lb_3v3.Count, endidx); ++i)
			{
				int hue = i < 1 ? leadername_hue + i + (1 * i) : name_hue;
				this.AddLabel(x3v3name, y, hue, lb_3v3[i].name);
				this.AddLabel(x3v3score, y, hue, lb_3v3[i].sscore);
				y += ydelta;
			}
		}

		public enum Buttons
		{
			CheckIPY,
			CheckT2A,
			CheckUOR,
			CheckOrder,
			CheckChaos,
			ButtonSubmit,
			BtnPrevPage,
			BtnNextPage
		}
		public override void OnResponse(NetState sender, RelayInfo info)
		{
			if (info.ButtonID == (int)Buttons.ButtonSubmit)
			{
				EArenaMatchEra new_era = EArenaMatchEra.eAMR_NumEras;
				if (info.IsSwitched((int)Buttons.CheckIPY))
					new_era = EArenaMatchEra.eAMR_IPY;
				else if (info.IsSwitched((int)Buttons.CheckT2A))
					new_era = EArenaMatchEra.eAMR_T2A;
				else if (info.IsSwitched((int)Buttons.CheckUOR))
					new_era = EArenaMatchEra.eAMR_Pub16;

				EArenaMatchRestrictions new_rest = info.IsSwitched((int)Buttons.CheckOrder) ? EArenaMatchRestrictions.eAMC_Order : EArenaMatchRestrictions.eAMC_Chaos;
				sender.Mobile.SendGump(new ArenaLeaderboardsGump(sender.Mobile, new_era, new_rest, 0));
			}
			else if (info.ButtonID == (int)Buttons.BtnNextPage)
			{
				sender.Mobile.SendGump(new ArenaLeaderboardsGump(sender.Mobile, m_Era, m_Restrictions, m_StartIdx + ENTRIES_PER_PAGE));
			}
			else if (info.ButtonID == (int)Buttons.BtnPrevPage)
			{
				sender.Mobile.SendGump(new ArenaLeaderboardsGump(sender.Mobile, m_Era, m_Restrictions, m_StartIdx - ENTRIES_PER_PAGE));
			}
			else
			{
				sender.Mobile.SendGump(new IPYGump(sender.Mobile));
				return;
			}
		}
		private string EraToStr(EArenaMatchEra era)
		{
			return era == EArenaMatchEra.eAMR_IPY ? "UOAC" : era == EArenaMatchEra.eAMR_Pub16 ? "UO:R" : "T2A";
		}

	}
}
