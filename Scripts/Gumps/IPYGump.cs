using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Misc;
using Server.Network;
using Server.Achievements;
using Server.Commands;
using Server.Items;
using Server.Mobiles;
using Server;
using Server.ArenaSystem;

namespace Server.Gumps
{
	public class IPYGump : Server.Gumps.Gump
	{
		public static void Initialize()
		{
			EventSink.QuestGumpRequest += new QuestGumpRequestHandler(IPYGump.QuestButton);
		}

		public static void QuestButton(QuestGumpRequestArgs e)
		{
			if( !e.Mobile.HasGump( typeof(IPYGump) ) )
				e.Mobile.SendGump(new IPYGump(e.Mobile));
		}

		private enum Buttons
		{
			Donations = 1,
			MessageOfTheDay,
			OCBControl,
			OCBTreasuryRewards,
			OCBCalendar,
			OCBLeaders,
			ArenaLeaders,
			ArenaTournaments,
			Codex,
			AchievementsYours,
			Quests,
			Titles,
			DungeonQueue,
            ArenaMyTeams,
            PveLeaderboards,
            Battlegrounds
		}
		
		public IPYGump(Mobile from)
			: base(0, 0)
		{
			from.CloseGump(typeof(IPYGump));

			int sublabel_hue = 1153;
			int label_hue = 53;

			this.Closable = true;
			this.Disposable = true;
			this.Dragable = true;
			this.Resizable = false;

			// background
			this.AddPage(0);
			this.AddBackground( 0, 31, 360, 440, 0x2436 );
			this.AddImage(140, 0, 1417);
			this.AddImage(150, 9, 5608);
			this.AddImage(323, 316, 10410);


			// HR dividers
			int div_x = 20;
			this.AddImageTiled(div_x, 200, 325, 3, 9102); 
			this.AddImageTiled(div_x, 319, 325, 3, 9102);
			this.AddImageTiled(div_x, 420, 325, 3, 9102);

			int cat_l_x = 35;
			int cat_r_x = 225;
			int but_l_x = cat_l_x + 3;
			int but_r_x = cat_r_x + 3;
			int lab_l_x = cat_l_x + 23;
			int lab_r_x = cat_r_x + 23;

			// Labels and buttons
			this.AddLabel(cat_l_x, 109, label_hue, @"Character");
				this.AddLabel(lab_l_x, 130, sublabel_hue, @"Titles");
				this.AddButton(but_l_x, 135, 2103, 2104, (int)Buttons.Titles, GumpButtonType.Reply, 0);
				this.AddLabel(lab_l_x, 150, sublabel_hue, @"Achievements");
				this.AddButton(but_l_x, 155, 2103, 2104, (int)Buttons.AchievementsYours, GumpButtonType.Reply, 0);
				this.AddLabel(lab_l_x, 170, sublabel_hue, @"Quests");
				this.AddButton(but_l_x, 175, 2103, 2104, (int)Buttons.Quests, GumpButtonType.Reply, 0);

			this.AddLabel(cat_r_x, 109, label_hue, @"Shard");
				this.AddLabel(lab_r_x, 130, sublabel_hue, @"MOTD");
				this.AddButton(but_r_x, 135, 2103, 2104, (int)Buttons.MessageOfTheDay, GumpButtonType.Reply, 0);
				this.AddLabel(lab_r_x, 150, sublabel_hue, @"Codex (web)");
				this.AddButton(but_r_x, 155, 2103, 2104, (int)Buttons.Codex, GumpButtonType.Reply, 0);
                this.AddLabel(lab_r_x, 170, sublabel_hue, @"Donations");
                this.AddButton(but_r_x, 175, 2103, 2104, (int)Buttons.Donations, GumpButtonType.Reply, 0);

			this.AddLabel(cat_l_x, 214, label_hue, @"OCB Wars");
				this.AddLabel(lab_l_x, 235, sublabel_hue, @"Rankings / Leaderboards");
				this.AddButton(but_l_x, 240, 2103, 2104, (int)Buttons.OCBLeaders, GumpButtonType.Reply, 0);
				this.AddLabel(lab_l_x, 255, sublabel_hue, @"Control Map");
				this.AddButton(but_l_x, 260, 2103, 2104, (int)Buttons.OCBControl, GumpButtonType.Reply, 0);
				this.AddLabel(lab_l_x, 275, sublabel_hue, @"Calendar");
				this.AddButton(but_l_x, 280, 2103, 2104, (int)Buttons.OCBCalendar, GumpButtonType.Reply, 0);				
				this.AddLabel(lab_l_x, 295, sublabel_hue, @"Treasury Rewards");
				this.AddButton(but_l_x, 300, 2103, 2104, (int)Buttons.OCBTreasuryRewards, GumpButtonType.Reply, 0);
			
			this.AddLabel(cat_r_x, 214, label_hue, @"Arena");
				this.AddLabel(lab_r_x, 235, sublabel_hue, @"Leaderboards");
				this.AddButton(but_r_x, 240, 2103, 2104, (int)Buttons.ArenaLeaders, GumpButtonType.Reply, 0);
				this.AddLabel(lab_r_x, 255, sublabel_hue, @"Tournaments");
				this.AddButton(but_r_x, 260, 2103, 2104, (int)Buttons.ArenaTournaments, GumpButtonType.Reply, 0);
				this.AddLabel(lab_r_x, 275, sublabel_hue, @"My Arena Teams");
				this.AddButton(but_r_x, 280, 2103, 2104, (int)Buttons.ArenaMyTeams, GumpButtonType.Reply, 0);

			this.AddLabel(cat_l_x, 326, label_hue, @"PvE");
                this.AddLabel(lab_l_x, 347, sublabel_hue, @"Leaderboards");
                this.AddButton(but_l_x, 352, 2103, 2104, (int)Buttons.PveLeaderboards, GumpButtonType.Reply, 0);
			//    this.AddLabel(lab_l_x, 367, sublabel_hue, @"Placeholder");
			//    this.AddButton(but_l_x, 372, 2103, 2104, (int)0, GumpButtonType.Reply, 0);

			this.AddHtml(20, 435, 300, 20, String.Format("<center><basefont color=#cecece>{0}", Profile.GetAccountDuration(from)), false, false);

			// Events
			this.AddLabel(cat_r_x, 326, label_hue, @"Events");
            this.AddLabel(lab_r_x, 347, sublabel_hue, @"Battlegrounds");
            this.AddButton(but_r_x, 352, 2103, 2104, (int)Buttons.Battlegrounds, GumpButtonType.Reply, 0);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			// Back Button or window closed
			switch (info.ButtonID)
			{
				case (int)Buttons.Donations:
					CommandSystem.Handle(sender.Mobile, "[DonationShop");
					break;
				case (int)Buttons.MessageOfTheDay:
					sender.Mobile.SendGump(new WelcomeGump(0));
					break;
				case (int)Buttons.OCBControl:
					CommandSystem.Handle(sender.Mobile, "[Controlmap");
					break;

				//case (int)Buttons.OCBTreasuryRewards:
				//sender.Mobile.SendGump(new Server.Custom.Townsystem.TreasuryKeyGump(sender.Mobile));
				//break;

				case (int)Buttons.OCBCalendar:
					CommandSystem.Handle(sender.Mobile, "[VulnMap");
					break;
				case (int)Buttons.OCBLeaders:
					//CommandSystem.Handle(sender.Mobile, "[OCLeaderboard");
					CommandSystem.Handle(sender.Mobile, "[militiaboard");
					break;
				case (int)Buttons.ArenaLeaders:
					sender.Mobile.SendGump(new ArenaLeaderboardsGump(sender.Mobile, EArenaMatchEra.eAMR_IPY, EArenaMatchRestrictions.eAMC_Order, 0));
					break;
				case (int)Buttons.ArenaTournaments:
					break;
				case (int)Buttons.ArenaMyTeams:
					sender.Mobile.SendGump(new ArenaAdminGump(sender.Mobile, ArenaAdminGump.ListType.MyTeams));
					break;
                case (int)Buttons.PveLeaderboards:
                    sender.Mobile.SendGump(new Server.Custom.GoldCoinReportGump(sender.Mobile, Server.Custom.GoldCoinTracker.LastReport));
                    break;
				case (int)Buttons.Codex:
					string url = "http://www.uoancorp.com";
					sender.Mobile.LaunchBrowser(url);
					break;
				case (int)Buttons.AchievementsYours:
					CommandSystem.Handle(sender.Mobile, "[ach");
					break;
				case (int)Buttons.Quests:
					XmlQuest.NormalQuestButton(sender.Mobile as PlayerMobile);
					break;
				case (int)Buttons.Titles:
					CommandSystem.Handle(sender.Mobile, "[titles");
					break;
				case (int)Buttons.DungeonQueue:
					break;
                case (int)Buttons.Battlegrounds:
                    CommandSystem.Handle(sender.Mobile, "[battlegrounds");
                    break;
				default:
					break;
			}
		}
	}
}