using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server.Accounting;
using Server.Achievements;
using Server.Mobiles;
using Server.Network;
using Server.Misc;
using Server.Gumps;

namespace Server.Achievements
{
	public static class AchievementsReport
	{
		public class Entry : IComparable<Entry>
		{
			public string name;
			public int count;
			public string guild;

			public int CompareTo(Entry other)
			{
				if (other == null)
					return 1;
				else
					return other.count - count;
			}
		}

		public class Report
		{
			public int m_NumAccounts;
			public int[] m_AchNumCompleted;
			public List<Entry> m_Top10;
		}

		static Report m_LastReport;

		public static Report LastReport { get { return m_LastReport; } }

		public static void Initialize()
		{
			m_LastReport = new Report();
			m_LastReport.m_Top10 = new List<Entry>();
		}

		public static void GenerateHTML()
		{
		}

		public static void ShowGump(Mobile m)
		{
			m.SendGump(new AchievementReportGump(m, m_LastReport));
		}

		public static void GenerateReport()
		{
			List<Achievement> all_ach = AchievementSystemImpl.Instance.m_AllAchievements;
			m_LastReport.m_AchNumCompleted = new int[AchievementSystemImpl.Instance.m_AllAchievements.Count];
			m_LastReport.m_NumAccounts = Accounts.GetAccounts().Count;
			List<Entry> allentries = new List<Entry>();

			// count number of achievements accomplished for each account
			foreach (Account a in Accounts.GetAccounts())
			{
				if( a.AccessLevel > AccessLevel.Player )
					continue;

				PlayerAccomplishments player_state = a.AccountAchievements;
				int total_comp = 0;
				for (int i = 0; i < all_ach.Count; ++i)
				{
					if (player_state.GetIsCompleted(all_ach[i]))
					{
						++m_LastReport.m_AchNumCompleted[i];
						++total_comp;
					}
				}

				if (a.accountMobiles.Length > 0)
				{
					string first_char_name = null;
					string first_char_guild = "N/A";
					for (int i = 0; i < a.accountMobiles.Length; ++i)
					{
						if (a.accountMobiles[i] != null)
						{
							first_char_name = a.accountMobiles[i].Name;
							if (a.accountMobiles[i].Guild != null)
								first_char_guild = a.accountMobiles[i].Guild.Abbreviation;
							break;
						}
					}
					if(first_char_name != null )
						allentries.Add(new Entry() { name = first_char_name, count = total_comp, guild = first_char_guild });
				}
			}

			// get top 10 scorers
			allentries.Sort();
			m_LastReport.m_Top10 = new List<Entry>();
			for (int i = 0; i < Math.Min(10, allentries.Count); ++i)
			{
				m_LastReport.m_Top10.Add(allentries[i]);
			}
		}
	}

	public class AchievementReportGump : Server.Gumps.Gump
	{
		public AchievementReportGump(Mobile from, AchievementsReport.Report report)
			: base(0, 0)
		{
			from.CloseGump(typeof(AchievementReportGump));

			this.Closable = true;
			this.Disposable = true;
			this.Dragable = true;
			this.Resizable = false;

			this.AddPage(0);
			this.AddImage(0, 44, 206);
			this.AddImageTiled(44, 85, 427, 318, 200);
			this.AddImage(44, 44, 201);
			this.AddImage(471, 44, 207);
			this.AddImage(471, 87, 203);

			this.AddImage(0, 88, 202);
			this.AddImage(0, 403, 204);
			this.AddImage(471, 403, 205);
			this.AddImage(44, 403, 233);
			this.AddImage(142, 22, 1419);
			this.AddButton(232, 403, 0x81c, 0x81b, 0, GumpButtonType.Reply, 0);
			this.AddImage(218, 4, 1417);
			this.AddImage(229, 13, 5608);


			this.AddImageTiled(51, 143, 400, 3, 0x238e);//horizontal bar

			int label_hue = 2036;
			this.AddLabel(175, 93, 53, @"ACHIEVEMENTS LEADERBOARD");
			this.AddLabel(90, 123, label_hue, @"PLAYER");
			this.AddLabel(240, 123, label_hue, @"GUILD");
			this.AddLabel(378, 123, label_hue, @"POINTS");

			int entry_hue = 53;
			int y = 150;
			int delta = 22;
			for (int i = 0; i < report.m_Top10.Count; ++i)
			{
				string l1 = "#" + (i + 1).ToString();
				AddLabel(60, y + (delta * i), entry_hue, l1);
				AddLabel(90, y + (delta * i), entry_hue, report.m_Top10[i].name);
				AddLabel(240, y + (delta * i), entry_hue, report.m_Top10[i].guild);
				AddLabel(378, y + (delta * i), entry_hue, report.m_Top10[i].count.ToString() + " points");
			}
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
		}
	}
}
