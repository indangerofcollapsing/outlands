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
using Server.Commands;
using Server.ArenaSystem;
using Server.Spells;
using Server.Items;
using Server.Regions;
using Server.Multis;
namespace Server.ArenaSystem
{
	/// <summary>
	/// TEAM INFO GUMP
	/// </summary>
	public class TeamInfoGump : Server.Gumps.Gump
	{
		public TeamInfoGump(Mobile from, ArenaTeam team)
			: base(360, 160)
		{
			this.Closable = true;
			this.Disposable = true;
			this.Dragable = true;
			this.Resizable = false;
			this.AddPage(0);
			this.AddBackground(161, 47, 475, 417, 83);
			this.AddBackground(181, 86, 435, 361, 2620);

			int label_hue = 53;
			int text_hue = 2036;
			
			if (team == null)
			{
				this.AddLabel(182, 62, label_hue, "TEAM DELETED!");

			}
			else
			{
				this.AddLabel(182, 62, label_hue, team.TeamName);
				this.AddLabel(208, 187, label_hue, String.Format("Ladder rankings - {0}v{0}", team.Capacity));
				this.AddLabel(529, 187, label_hue, "W/L");
				this.AddLabel(465, 187, label_hue, "Rating");
				this.AddLabel(210, 107, label_hue, "Team Members");

				int ymember = 123;
				int ydeltamem = 16;
				for (int i = 0; i < team.Players.Count; ++i)
					this.AddLabel(210, ymember+(i*16), text_hue, team.Players[i].Name);


				int xWL = 529;
				int xscore = 465;
				int xcats = 208;

				this.AddLabel(xcats, 221, text_hue, "UOAC / Order / Not Templated");
				this.AddLabel(xWL, 221, text_hue, String.Format("{0}/{1}", team.GetWinsFor(EArenaMatchEra.eAMR_IPY, EArenaMatchRestrictions.eAMC_Order), team.GetLossesFor(EArenaMatchEra.eAMR_IPY, EArenaMatchRestrictions.eAMC_Order)));
				this.AddLabel(xscore, 221, text_hue, team.GetScore(EArenaMatchRestrictions.eAMC_Order, EArenaMatchEra.eAMR_IPY, false).ToString());

				this.AddLabel(xcats, 236, text_hue, "UOAC / Chaos / Not Templated");
				this.AddLabel(xWL, 236, text_hue, String.Format("{0}/{1}", team.GetWinsFor(EArenaMatchEra.eAMR_IPY, EArenaMatchRestrictions.eAMC_Chaos), team.GetLossesFor(EArenaMatchEra.eAMR_IPY, EArenaMatchRestrictions.eAMC_Chaos)));
				this.AddLabel(xscore, 236, text_hue, team.GetScore(EArenaMatchRestrictions.eAMC_Chaos, EArenaMatchEra.eAMR_IPY, false).ToString());

				this.AddLabel(xcats, 266, text_hue, "T2A / Order / Not Templated");
				this.AddLabel(xWL, 266, text_hue, String.Format("{0}/{1}", team.GetWinsFor(EArenaMatchEra.eAMR_T2A, EArenaMatchRestrictions.eAMC_Order), team.GetLossesFor(EArenaMatchEra.eAMR_T2A, EArenaMatchRestrictions.eAMC_Order)));
				this.AddLabel(xscore, 266, text_hue, team.GetScore(EArenaMatchRestrictions.eAMC_Order, EArenaMatchEra.eAMR_T2A, false).ToString());

				this.AddLabel(xcats, 281, text_hue, "T2A / Chaos / Not Templated");
				this.AddLabel(xWL, 281, text_hue, String.Format("{0}/{1}", team.GetWinsFor(EArenaMatchEra.eAMR_T2A, EArenaMatchRestrictions.eAMC_Chaos), team.GetLossesFor(EArenaMatchEra.eAMR_T2A, EArenaMatchRestrictions.eAMC_Chaos)));
				this.AddLabel(xscore, 281, text_hue, team.GetScore(EArenaMatchRestrictions.eAMC_Chaos, EArenaMatchEra.eAMR_T2A, false).ToString());
				
				this.AddLabel(xcats, 311, text_hue, "UO:R / Order / Not Templated");
				this.AddLabel(xWL, 311, text_hue, String.Format("{0}/{1}", team.GetWinsFor(EArenaMatchEra.eAMR_Pub16, EArenaMatchRestrictions.eAMC_Order), team.GetLossesFor(EArenaMatchEra.eAMR_Pub16, EArenaMatchRestrictions.eAMC_Order)));
				this.AddLabel(xscore, 311, text_hue, team.GetScore(EArenaMatchRestrictions.eAMC_Order, EArenaMatchEra.eAMR_Pub16, false).ToString());

				this.AddLabel(xcats, 326, text_hue, "UO:R / Chaos / Not Templated");
				this.AddLabel(xWL, 326, text_hue, String.Format("{0}/{1}", team.GetWinsFor(EArenaMatchEra.eAMR_Pub16, EArenaMatchRestrictions.eAMC_Chaos), team.GetLossesFor(EArenaMatchEra.eAMR_Pub16, EArenaMatchRestrictions.eAMC_Chaos)));
				this.AddLabel(xscore, 326, text_hue, team.GetScore(EArenaMatchRestrictions.eAMC_Chaos, EArenaMatchEra.eAMR_Pub16, false).ToString());
			}
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			sender.Mobile.SendGump(new ArenaAdminGump(sender.Mobile, ArenaAdminGump.ListType.None));
		}
	}


	/// <summary>
	/// ARENA ADMIN GUMP
	/// </summary>
	public class ArenaAdminGump : Server.Gumps.Gump
	{
		public enum Buttons
		{
			listarenasbutton= 1,
			listgamesbutton,
			listmyteamsbutton,
			new1v1,
			new2v2,
			new3v3,
			matchtimefield,
			setmatchtime,

			// dont add new ones after these
			teaminfo_start = 7000,
			leave_team_start = 6000,
			goto_match_start = 5000,
			delete_arena_start = 4000,
			goto_arena_start = 3000,
		}

		public enum ListType
		{
			None,
			Arenas,
			Games,
			MyTeams,
		}


		private int m_TeamSizeAtStart;

		public ArenaAdminGump(Mobile from, ListType list_type)
			: base(40, 40)
		{
			int label_hue = 2036;
			int textentry_hue = 53;

			// STATICS
			this.Closable = true;
			this.Disposable = true;
			this.Dragable = true;
			this.Resizable = false;
			this.AddPage(0);
			this.AddBackground(94, 58, 584, 477, 83);
			this.AddBackground(108, 158, 556, 363, 2620);

			this.AddButton(192, 84, 4005, 4007, (int)Buttons.listmyteamsbutton, GumpButtonType.Reply, 0);
			this.AddLabel(113, 85, label_hue, @"My Teams");

			this.AddButton(192, 108, 4005, 4007, (int)Buttons.listarenasbutton, GumpButtonType.Reply, 0);
			this.AddLabel(113, 109, label_hue, @"List Arenas");
	
			this.AddButton(192, 132, 4005, 4007, (int)Buttons.listgamesbutton, GumpButtonType.Reply, 0);
			this.AddLabel(113, 133, label_hue, @"List Games");
			


			// Add / Delete buttons for gamemasters
			if (from.AccessLevel >= AccessLevel.GameMaster)
			{
				this.AddLabel(534, 75, label_hue, @"New 1v1 arena");
				this.AddLabel(534, 102, label_hue, @"New 2v2 arena");
				this.AddLabel(534, 129, label_hue, @"New 3v3 arena");
				this.AddButton(634, 75, 4023, 4025, (int)Buttons.new1v1, GumpButtonType.Reply, 0);
				this.AddButton(634, 102, 4023, 4025, (int)Buttons.new2v2, GumpButtonType.Reply, 0);
				this.AddButton(634, 129, 4023, 4025, (int)Buttons.new3v3, GumpButtonType.Reply, 0);
				this.AddTextEntry(413, 129, 51, 20, textentry_hue, (int)Buttons.matchtimefield, ((int)ArenaMatch.s_matchTimeMax.TotalSeconds).ToString());
				this.AddButton(471, 129, 4023, 4025, (int)Buttons.setmatchtime, GumpButtonType.Reply, 0);
				this.AddLabel(313, 129, label_hue, @"Set match time");
			}
		
			if (list_type == ListType.Arenas)
			{
				int xname = 146;
				int xtype = 418;
				int xgoto = 520;
				int xdelete = 590;
				int y = 213;
				int ydelta = 25;

				this.AddLabel(146, 176, label_hue, @"Arena Name");
				this.AddLabel(416, 176, label_hue, @"Type");
				this.AddLabel(506, 176, label_hue, @"Go there");
				if (from.AccessLevel >= AccessLevel.GameMaster)
					this.AddLabel(579, 176, label_hue, @"Delete it");
				List<Arena> all_arenas = ArenaSystem.GetAllArenas();
				for (int i = 0; i < all_arenas.Count; ++i )
				{
					Arena a = all_arenas[i];
					AddLabel(xname, y, textentry_hue, a.Name);
					AddLabel(xtype, y, textentry_hue, ArenaTypeName(a));
					AddButton(xgoto, y, 4005, 4007, (int)Buttons.goto_arena_start + i, GumpButtonType.Reply, 0);
					if( from.AccessLevel >= AccessLevel.GameMaster )
						AddButton(xdelete, y, 4017, 4019, (int)Buttons.delete_arena_start + i, GumpButtonType.Reply, 0);
					y += ydelta;
				}
			}
			else if (list_type == ListType.Games)
			{
				int y = 213;
				int ydelta = 25;
				int xt1 = 146;
				int xt2 = 298;
				int xtype = 438;
				int xgoto = 520;

				this.AddLabel(146, 176, label_hue, @"Team 1");
				this.AddLabel(300, 176, label_hue, @"Team 2");
				this.AddLabel(438, 176, label_hue, @"Type");
				this.AddLabel(506, 176, label_hue, @"Go there");

				List<ArenaMatch> all_matches = ArenaSystem.GetAllPendingMatches();
				for (int i = 0; i < all_matches.Count; ++i)
				{
					this.AddLabel(146, y, textentry_hue, all_matches[i].Team1.TeamName);
					this.AddLabel(298, y, textentry_hue, all_matches[i].Team2.TeamName);
					this.AddLabel(438, y, textentry_hue, ArenaTypeName(all_matches[i].m_arena));
					AddButton(xgoto, y, 4005, 4007, (int)Buttons.goto_match_start + i, GumpButtonType.Reply, 0);
					y += ydelta;
				}
			}
			else if (list_type == ListType.MyTeams)
			{
				int xname = 146;
				int xtype = 430;
				int xfullinfo = 500;
				int xleave = 580;
				int xmember = xname + 10;
				int y = 213;
				int ydelta = 25;
				int membername_hue = 2028;

				this.AddLabel(xname, 176, label_hue, "Team");
				this.AddLabel(xtype, 176, label_hue, "Type");
				this.AddLabel(xfullinfo + 2, 176, label_hue, "Info");
				this.AddLabel(xleave - 15, 176, label_hue, "Disband team");

				//[4:23:16 PM] (IPY) Michael Kemski - Xiani: Make sure to add checks for them being in a match before they can augment any team details

				List<ArenaTeam> my_teams = ArenaSystem.GetTeamsForPlayer(from as PlayerMobile);
				m_TeamSizeAtStart = my_teams.Count;
				for (int i = 0; i < my_teams.Count; ++i)
				{
					this.AddLabel(xname, y, textentry_hue, my_teams[i].TeamName);
					this.AddLabel(xtype, y, textentry_hue, String.Format("{0}v{0}", my_teams[i].Capacity));
					if( my_teams[i].Capacity != 1 ) // can't leave 1v1 teams
						AddButton(xleave, y, 4023, 4025, (int)Buttons.leave_team_start + i, GumpButtonType.Reply, 0);
					AddButton(xfullinfo, y, 4005, 4007, (int)Buttons.teaminfo_start + i, GumpButtonType.Reply, 0);
					for (int j = 0; j < my_teams[i].Players.Count; ++j)
					{
						y += 18;
						this.AddLabel(xmember, y, membername_hue, String.Format("Member : {1}",j, my_teams[i].Players[j].Name));
					}
					y += ydelta;
				}
			}
		}

		public string ArenaTypeName(Arena a)
		{
			return a.Size == 3 ? "3v3" : a.Size == 2 ? "2v2" : "1v1";
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			// do nothing if the player is in a match.
			if (ArenaSystem.GetMatchForPlayer(sender.Mobile as PlayerMobile) != null)
			{
				sender.Mobile.SendMessage("You decide to focus on your current fight instead.");
				return;
			}
			if( info.ButtonID == (int)Buttons.listarenasbutton )
			{
				sender.Mobile.SendGump( new ArenaAdminGump(sender.Mobile, ListType.Arenas));
			}
			else if (info.ButtonID == (int)Buttons.listgamesbutton)
			{
				sender.Mobile.SendGump( new ArenaAdminGump(sender.Mobile, ListType.Games));
			}
			else if (info.ButtonID == (int)Buttons.listmyteamsbutton)
			{
				sender.Mobile.SendGump( new ArenaAdminGump(sender.Mobile, ListType.MyTeams));
			}
			else if (info.ButtonID == (int)Buttons.new1v1)
			{
				if( sender.Mobile.AccessLevel >= AccessLevel.GameMaster )
					CommandSystem.Handle(sender.Mobile, "[as_1v1");
			}
			else if (info.ButtonID == (int)Buttons.new2v2)
			{
				if (sender.Mobile.AccessLevel >= AccessLevel.GameMaster)
					CommandSystem.Handle(sender.Mobile, "[as_2v2");
			}
			else if (info.ButtonID == (int)Buttons.new3v3)
			{
				if (sender.Mobile.AccessLevel >= AccessLevel.GameMaster)
					CommandSystem.Handle(sender.Mobile, "[as_3v3");
			}
			else if (info.ButtonID == (int)Buttons.matchtimefield)
			{
				sender.Mobile.SendGump(new ArenaAdminGump(sender.Mobile, ListType.Arenas));
			}
			else if (info.ButtonID == (int)Buttons.setmatchtime)
			{
				if (sender.Mobile.AccessLevel > AccessLevel.GameMaster)
				{
					int seconds = 0;
					if (info.TextEntries[0].Text.Length == 0 || !Int32.TryParse(info.TextEntries[0].Text, out seconds) || seconds <= 0)
					{
						sender.Mobile.SendMessage(0x20, "ERROR: must be a number > 0");
					}
					else
					{
						ArenaMatch.s_matchTimeMax = new TimeSpan(0, 0, seconds);
						sender.Mobile.SendMessage(0x52, "New match length set to {0} seconds.", seconds);
					}
					sender.Mobile.SendGump(new ArenaAdminGump(sender.Mobile, ListType.Arenas));
				}
			}
			///// OUT HERE : If frozen return 
			else if (sender.Mobile.Frozen)
			{
				sender.Mobile.SendMessage("You can not do that while frozen.");
				return;
			}

			// RANGES
			else if (info.ButtonID >= (int)Buttons.teaminfo_start)
			{
				int teamidx = info.ButtonID - (int)Buttons.teaminfo_start;
				List<ArenaTeam> my_teams = ArenaSystem.GetTeamsForPlayer(sender.Mobile as PlayerMobile);
				if( my_teams.Count > teamidx )
					sender.Mobile.SendGump(new TeamInfoGump(sender.Mobile, my_teams[teamidx]));
			}
			else if (info.ButtonID >= (int)Buttons.leave_team_start)
			{
				int teamidx = info.ButtonID - (int)Buttons.leave_team_start;
				List<ArenaTeam> my_teams = ArenaSystem.GetTeamsForPlayer(sender.Mobile as PlayerMobile);
				
				// Why this check? -> Team can be deleted between gump spawned and this was pressed 
				// which means we could accidentally disband another team
				if (my_teams.Count > teamidx && my_teams.Count == m_TeamSizeAtStart) 
				{
					ArenaTeam the_team = my_teams[teamidx];
					if(the_team.Capacity != 1 ) // can't disband 1v1 teams
						ArenaSystem.DisbandAndDelete(the_team);
				}
			}
			else if (info.ButtonID >= (int)Buttons.goto_match_start)
			{
				int which_one = info.ButtonID - (int)Buttons.goto_match_start;
				List<ArenaMatch> all_matches = ArenaSystem.GetAllPendingMatches();
				if (which_one >= 0 && all_matches.Count > which_one && all_matches[which_one].m_arena != null)
				{
					MagerySpell spell = new ArenaTransportSpell(sender.Mobile, all_matches[which_one].m_arena.Location);
					spell.Cast();
				}
				sender.Mobile.SendGump(new ArenaAdminGump(sender.Mobile, ListType.Games));
			}
			else if(info.ButtonID >= (int)Buttons.delete_arena_start)
			{
				if (sender.Mobile.AccessLevel >= AccessLevel.GameMaster)
				{
					int which_one = info.ButtonID - (int)Buttons.delete_arena_start;
					List<Arena> all_arenas = ArenaSystem.GetAllArenas();
					if (all_arenas.Count > which_one)
					{
						all_arenas[which_one].Delete();
					}
					sender.Mobile.SendGump(new ArenaAdminGump(sender.Mobile, ListType.Arenas));
				}
			}
			else if(info.ButtonID >= (int)Buttons.goto_arena_start)
			{
				int which_one = info.ButtonID - (int)Buttons.goto_arena_start;
				List<Arena> all_arenas = ArenaSystem.GetAllArenas();
				if (which_one >= 0 && all_arenas.Count > which_one && all_arenas[which_one] != null)
				{
					if (sender.Mobile.AccessLevel > AccessLevel.Player)
						sender.Mobile.MoveToWorld(all_arenas[which_one].Location, Map.Felucca);
					else
					{
						MagerySpell spell = new ArenaTransportSpell(sender.Mobile, all_arenas[which_one].Location);
						spell.Cast();
					}
				}
				sender.Mobile.SendGump(new ArenaAdminGump(sender.Mobile, ListType.Arenas));
			}
		}
	}



	public class ArenaTransportSpell : MagerySpell
	{
		private Mobile m_Caster;
		private Point3D m_Destination;
		private static float s_castTime = 2.0f;

		private static SpellInfo m_Info = new SpellInfo(
				"Recall To Arena", "Kal Ex Por",
				263,
				9032
			);

		public ArenaTransportSpell(Mobile caster, Point3D destination)
			: base(caster, null, m_Info)
		{
			m_Destination = destination;
		}

		public override SpellCircle Circle { get { return SpellCircle.First; } }

		public override TimeSpan GetCastDelay()
		{
			return TimeSpan.FromSeconds(s_castTime);
		}
		public override void GetCastSkills(out double min, out double max)
		{
			min = 0;
			max = 0;
		}
		public override int GetMana()
		{
			return 0;
		}
		public override bool CheckCast()
		{
			// Base requirements, can only use while in guarded zone, arena zone or in friendly house.
			GuardedRegion gr = Caster.Region as GuardedRegion;
			BaseHouse house = BaseHouse.FindHouseAt(Caster.Location, Caster.Map, 64);

			bool in_guarded_region = gr != null && !gr.Disabled;
			bool in_friendly_house = house != null && house.IsFriend(Caster);
			if ( !in_guarded_region && !in_friendly_house )
			{
				Caster.SendMessage("That can only be used in guarded regions or from a friendly house");
				return false;
			}
            if (Caster.Criminal)
			{
				Caster.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.
				return false;
			}
			else if (SpellHelper.CheckCombat(Caster, true))
			{
				Caster.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
				return false;
			}
			return base.CheckCast();
		}
		public override void OnCast()
		{
			if (!SpellHelper.CheckTravel(Caster, TravelCheckType.RecallFrom))
			{
				Caster.SendLocalizedMessage(501802); // Thy spell doth not appear to work...
			}
			else
			{
				Caster.MoveToWorld(m_Destination, Map.Felucca);
			}
			FinishSequence();
		}
	}
}