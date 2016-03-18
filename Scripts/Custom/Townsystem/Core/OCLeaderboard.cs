using System;
using System.IO;

using Server;
using Server.Gumps;
using System.Collections;
using System.Collections.Generic;
using Server.Commands;
using Server.Mobiles;
using System.Text;


namespace Server.Custom.Townsystem
{
    public static class OCBRankingSystem
    {

        public static void Initialize()
        {
            EventSink.ServerStarted += new ServerStartedEventHandler(() => { CalculateLeaderboards(); });
            EventSink.WorldSave += new WorldSaveEventHandler((x) => { CalculateLeaderboards(); });
            Server.Commands.CommandSystem.Register("militiaboard", AccessLevel.Player, new CommandEventHandler(OCLeaderboardNew_OnCommand));
        }

        [Usage("OCLeaderboardNew")]
        [Description("Views the OC Leaderboard (New)")]
        public static void OCLeaderboardNew_OnCommand(CommandEventArgs e)
        {
            if (e.Mobile is PlayerMobile)
                e.Mobile.SendGump(new OCBLeaderboards_New(e.Mobile as PlayerMobile, 0));
        }

        public enum OCBRank
        {
            Champion = 0,
            Hero,
            Archblade,
            Avenger,
            Crusader,
            Sentinel,
            Knight,
            Defender,
            Squire,
            NumRanks,
        }

        public class Entry : IComparable<Entry>
        {
            public string name;		// player name
            public string guild;	// player guild
            public OCBRank rank;	// player rank
            public int alliancescore { get { return iscore; } set { iscore = value; sscore = value.ToString(); } }
            public int killpoints;

            private int iscore;
            public string sscore; // to avoid converting int to str every time a user opens the gump.


            public PlayerMobile temp_pm; // temp
            public int CompareTo(Entry other)
            {
                if (iscore != other.iscore)
                    return other.iscore - iscore;
                else
                    return other.killpoints - killpoints;
            }

        }

        public static string[] s_RankNameToString = new string[(int)OCBRank.NumRanks + 1]
		{
			"Champion",
			"Hero",
			"Archblade",
			"Avenger",
			"Crusader",
			"Sentinel",
			"Knight",
			"Defender",
			"Squire",
			"N/A",
		};

        public static int[] s_RankHues = new int[(int)OCBRank.NumRanks + 1]
		{
			2412,
			2424,
			1331,
			1902,
			1902,
			1902,
			1902,
			1902,
			1902,
			1902
		};

        public static Dictionary<Town, List<Entry>> TownRankings;

        public static void CalculateLeaderboards()
        {
            TownRankings = new Dictionary<Town, List<Entry>>();

            foreach (Town t in Town.Towns)
            {
                List<Entry> bucket = new List<Entry>();
                foreach (PlayerMobile player in t.MilitiaMembers)
                {
                    if (player.TownsystemPlayerState != null)
                    {
                        string g = player.Guild == null ? string.Empty : "[" + player.Guild.Abbreviation + "]";
                        bucket.Add(new Entry() { name = player.Name, guild = g, killpoints = player.TownsystemPlayerState.KillPoints, alliancescore = player.TownsystemPlayerState.AllianceScore, temp_pm = player });
                    }
                }
                TownRankings.Add(t, bucket);
            }

            // sort
            foreach (KeyValuePair<Town, List<Entry>> entry in TownRankings)
            {
                entry.Value.Sort();
            }

            SetRanks(TownRankings);
        }

        private static int[] min_score_for_rank = new int[(int)OCBRank.NumRanks]
		{
			50,
			30,
			25,
			20,
			15,
			10,
			5,
			1,
			-999999,
		};

        private static int[] players_per_rank = new int[(int)OCBRank.NumRanks] 
        {
            1,
            2,
            3,
            5,
            8,
            10,
            15,
            20,
            Int32.MaxValue,
        };


        private static void SetRanks(Dictionary<Town, List<Entry>> townPlayerList)
        {
            foreach (KeyValuePair<Town, List<Entry>> playerlist in townPlayerList)
            {
                int rank_index = 0;
                int next_rank_player_index = players_per_rank[0];
                for (int i = 0; i < playerlist.Value.Count; ++i)
                {
                    // new rank - find first one that the player meets in terms of minimum required score.
                    int qualified_rank_idx = rank_index;
                    while (min_score_for_rank[qualified_rank_idx] > playerlist.Value[i].alliancescore)
                        ++qualified_rank_idx;

                    playerlist.Value[i].rank = (OCBRank)qualified_rank_idx;
                    playerlist.Value[i].temp_pm.TownsystemPlayerState.AllianceRank = (OCBRank)playerlist.Value[i].rank;

                    // next range
                    if (next_rank_player_index == (i + 1))
                    {
                        ++rank_index;
                        next_rank_player_index += players_per_rank[rank_index];
                    }
                }
            }
        }
    }

    //////////////////////////////////////////////////////////////////////////
    // OCB Ranking/Leaderboard Gump
    //////////////////////////////////////////////////////////////////////////
    public class OCBLeaderboards_New : Server.Gumps.Gump
    {
        private static int s_EntriesPerPage = 7;
        private int m_CurrentPage;
        private int m_PlayerPage;
        public OCBLeaderboards_New(PlayerMobile from, int page, int playerPage = 0)
            : base(0, 0)
        {
            if (0 > page)
                page = 0;

            m_PlayerPage = playerPage;

            m_CurrentPage = page;

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
            this.AddPage(0);
            this.AddBackground(24, 80, 715, 451, 9200);
            this.AddImage(4, 501, 204);
            this.AddImage(724, 501, 205);
            this.AddImage(24, 46, 201);
            this.AddImage(310, 46, 201);
            this.AddImage(724, 70, 203);
            this.AddImage(724, 185, 203);
            this.AddImage(4, 46, 206);
            this.AddImage(724, 46, 207);
            this.AddImage(47, 501, 233);
            this.AddImage(297, 501, 233);
            this.AddImage(4, 185, 202);
            this.AddImage(4, 90, 202);
            this.AddBackground(39, 130, 693, 377, 9200);

            // dividers
            this.AddImageTiled(49, 181, 669, 3, 9102);
            this.AddImageTiled(271, 196, 1, 293, 9102);
            this.AddImageTiled(510, 195, 1, 293, 9102);


            // Hues
            int rank_dy = 43;
            int name_hue = 2036;
            int score_hue = 2036;

            // personal status
            string town_name = "N/A";
            int town_hue = 2036;
            if (from.TownsystemPlayerState != null && from.TownsystemPlayerState.Town != null && from.TownsystemPlayerState.Town.HomeFaction != null) // GEEZ!
            {
                town_name = from.TownsystemPlayerState.Town.HomeFaction.Definition.FriendlyName;
                town_hue = from.TownsystemPlayerState.Town.HomeFaction.Definition.HuePrimary - 1;
            }

            this.AddLabel(81, 81, 2036, @"Town Score");
            this.AddLabel(83, 104, town_hue, (from.TownsystemPlayerState != null ? from.TownsystemPlayerState.AllianceScore.ToString() : "---"));

            this.AddLabel(338, 81, 2036, @"Town Rank");
            OCBRankingSystem.OCBRank from_rank = (from.TownsystemPlayerState != null ? from.TownsystemPlayerState.AllianceRank : OCBRankingSystem.OCBRank.NumRanks);
            this.AddLabel(338, 104, town_hue, OCBRankingSystem.s_RankNameToString[(int)from_rank]);


            this.AddLabel(577, 81, 2036, @"Town");
            this.AddLabel(577, 104, town_hue, town_name); // -1 because text and item hues differ by 1...

            // leaderboard list
            this.AddLabel(341, 140, 2036, @"Leaderboards");

            if (page > 4)
            {
                m_CurrentPage = page = 0;
                m_PlayerPage++;
            }
            int index  = page * 3;
            int playerStart = m_PlayerPage * s_EntriesPerPage;
            var firstTown  = Town.Towns[index];
            var firstList  = OCBRankingSystem.TownRankings[firstTown];
            Town secondTown = null;
            Town thirdTown = null;
            List<OCBRankingSystem.Entry> secondList = null;
            List<OCBRankingSystem.Entry> thirdList = null;
            if (page % 4 != 0 || page == 0)
            {
                secondTown = Town.Towns[index + 1];
                secondList = OCBRankingSystem.TownRankings[secondTown];
                thirdTown = Town.Towns[index + 2];
                thirdList = OCBRankingSystem.TownRankings[thirdTown];
            }
                
            this.AddLabel(135, 161, firstTown.HomeFaction.Definition.HuePrimary-1, firstTown.HomeFaction.Definition.FriendlyName);
            if (secondTown != null)
            {
                this.AddLabel(363, 161, secondTown.HomeFaction.Definition.HuePrimary-1, secondTown.HomeFaction.Definition.FriendlyName);
                this.AddLabel(604, 161, thirdTown.HomeFaction.Definition.HuePrimary-1, thirdTown.HomeFaction.Definition.FriendlyName);
            }

            //////////////////////////////////////////////////////////////////////////
            // Ranks
            //////////////////////////////////////////////////////////////////////////
            // first town ranks
            int rank_yoffset = 0;
            for (int i = playerStart; i < Math.Min(firstList.Count, (playerStart + s_EntriesPerPage)); i++)
            {
                OCBRankingSystem.Entry e = firstList[i];
                this.AddLabel(49, 193 + rank_yoffset, OCBRankingSystem.s_RankHues[(int)e.rank], OCBRankingSystem.s_RankNameToString[(int)e.rank]);
                this.AddLabel(57, 211 + rank_yoffset, name_hue, e.name);
                this.AddLabel(186, 211 + rank_yoffset, score_hue, e.guild);
                rank_yoffset += rank_dy;
            }
            if (secondTown != null)
            {
                // second town ranks
                rank_yoffset = 0;
                for (int i = playerStart; i < Math.Min(secondList.Count, (playerStart + s_EntriesPerPage)); i++)
                {
                    OCBRankingSystem.Entry e = secondList[i];
                    this.AddLabel(290, 193 + rank_yoffset, OCBRankingSystem.s_RankHues[(int)e.rank], OCBRankingSystem.s_RankNameToString[(int)e.rank]);
                    this.AddLabel(298, 211 + rank_yoffset, name_hue, e.name);
                    this.AddLabel(427, 211 + rank_yoffset, score_hue, e.guild);
                    rank_yoffset += rank_dy;
                }

                // third town ranks
                rank_yoffset = 0;
                for (int i = playerStart; i < Math.Min(thirdList.Count, (playerStart + s_EntriesPerPage)); i++)
                {
                    OCBRankingSystem.Entry e = thirdList[i];
                    this.AddLabel(522, 193 + rank_yoffset, OCBRankingSystem.s_RankHues[(int)e.rank], OCBRankingSystem.s_RankNameToString[(int)e.rank]);
                    this.AddLabel(530, 211 + rank_yoffset, name_hue, e.name);
                    this.AddLabel(659, 211 + rank_yoffset, score_hue, e.guild);
                    rank_yoffset += rank_dy;
                }
            }
            

            // "page" buttons
            this.AddButton(89, 512, 2224, 2224, (int)Buttons.BtnNextPage, GumpButtonType.Reply, 0);
            if (page > 0 || m_PlayerPage > 0)
                this.AddButton(59, 512, 2223, 2223, (int)Buttons.BtnPrevPage, GumpButtonType.Reply, 0);

            // treasury key shortcut
            AddLabel(619, 507, 2036, "Rewards");
            AddButton(684, 507, 0xfa5, 0xfa7, (int)Buttons.BtnShowAwardsGump, GumpButtonType.Reply, 0);

            // next update info
            this.AddLabel(230, 507, 2036, "Leaderboards are updated after the militia war ends");
        }

        public enum Buttons
        {
            BtnPrevPage = 30,
            BtnNextPage = 31,
            BtnShowAwardsGump = 32,
        }

        public override void OnResponse(Network.NetState sender, RelayInfo info)
        {
            PlayerMobile from = sender.Mobile as PlayerMobile;
            if (from == null || info.ButtonID == 0)
            {
                sender.Mobile.SendGump(new IPYGump(sender.Mobile));
                return;
            }

            if (info.ButtonID == (int)Buttons.BtnPrevPage)
            {
                int prevPage = m_CurrentPage == 0 ? 3 : m_CurrentPage - 1;
                int playerPage = prevPage == 3 && m_PlayerPage > 0 ? m_PlayerPage - 1 : m_PlayerPage; // only decrement when we go back to the last page of cities
                sender.Mobile.SendGump(new OCBLeaderboards_New(from, prevPage, playerPage));
            }
            else if (info.ButtonID == (int)Buttons.BtnNextPage)
            {
                sender.Mobile.SendGump(new OCBLeaderboards_New(from, m_CurrentPage + 1, m_PlayerPage));
            }
            else if (info.ButtonID == (int)Buttons.BtnShowAwardsGump)
            {
                sender.Mobile.SendGump(new TreasuryKeyGump(sender.Mobile, 0));
            }
        }
    }




    /// <summary>
    /// OLD IPY2 OC LEADERBOARD
    /// </summary>
    public static class OCLeaderboard
    {
        public static void Initialize()
        {
            Server.Commands.CommandSystem.Register("OCLeaderboard", AccessLevel.Player, new CommandEventHandler(OCLeaderboard_OnCommand));
            Server.Commands.CommandSystem.Register("OCBUpdateAllianceScores", AccessLevel.GameMaster, new CommandEventHandler(OCBLeaderboard_UpdateAllianceScores));

            EventSink.ServerStarted += new ServerStartedEventHandler(() => { OnWorldLoad(); });
            EventSink.WorldSave += new WorldSaveEventHandler((x) => { OnWorldSave(x); });
        }

        private static string s_saveDirectory = "Saves/OCBScores";
        private static string s_saveFilename = "dailyOcbScores.dat";
        private static string s_saveFilepath = s_saveDirectory + "/" + s_saveFilename;
        private static void OnWorldSave(WorldSaveEventArgs e)
        {
            if (!Directory.Exists(s_saveDirectory))
            {
                Directory.CreateDirectory(s_saveDirectory);
            }
            FileStream fs = new FileStream(s_saveFilepath, FileMode.Create, FileAccess.Write, FileShare.None);
            GenericWriter writer = new BinaryFileWriter(fs, true);

            writer.Write(0); // Version
            writer.Write((int)_entries.Count);
            foreach (KeyValuePair<Mobile, OCLeaderboard.LeaderboardEntry> entry in _entries)
            {
                writer.Write(entry.Key); // mobile
                writer.Write(entry.Value.m_AllianceScore_Braziers);
                writer.Write(entry.Value.m_AllianceScore_Damage);
                writer.Write(entry.Value.m_AllianceScore_Healing);
                writer.Write(entry.Value.m_AllianceScore_KillPointDelta);
                writer.Write(entry.Value.m_AllianceScore_Kills);

                writer.Write(entry.Value.BraziersLit);
                writer.Write(entry.Value.TotalDamage);
                writer.Write(entry.Value.TotalHealing);
                writer.Write(entry.Value.KillPoints);
                writer.Write(entry.Value.Kills);
            }
            writer.Close();
        }
        private static void OnWorldLoad()
        {
            _entries = new Dictionary<Mobile, LeaderboardEntry>();
            if (File.Exists(s_saveFilepath))
            {
                FileStream fs = new FileStream(s_saveFilepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                BinaryFileReader reader = new BinaryFileReader(new BinaryReader(fs));
                int version = reader.ReadInt();

                // version 0
                int num_entries = reader.ReadInt();
                for (int i = 0; i < num_entries; ++i)
                {
                    Mobile m = reader.ReadMobile(); // m can be null here if the mobile was deleted
                    LeaderboardEntry new_entry = new LeaderboardEntry(m);
                    new_entry.m_AllianceScore_Braziers = reader.ReadInt();
                    new_entry.m_AllianceScore_Damage = reader.ReadInt();
                    new_entry.m_AllianceScore_Healing = reader.ReadInt();
                    new_entry.m_AllianceScore_KillPointDelta = reader.ReadInt();
                    new_entry.m_AllianceScore_Kills = reader.ReadInt();

                    new_entry.BraziersLit = reader.ReadInt();
                    new_entry.TotalDamage = reader.ReadInt();
                    new_entry.TotalHealing = reader.ReadInt();
                    new_entry.KillPoints = reader.ReadInt();
                    new_entry.Kills = reader.ReadInt();
                    if (m != null)
                        _entries.Add(m, new_entry);
                }
                reader.Close();
            }

        }

        public static void OCBLeaderboard_UpdateAllianceScores(CommandEventArgs e)
        {
            OCLeaderboard.Clear();
        }


        [Usage("OCLeaderboard")]
        [Description("Views the OC Leaderboard")]
        public static void OCLeaderboard_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            if (e.Length == 0)
            {
                from.SendGump(new LeaderboardGump());
            }
            else
                e.Mobile.SendMessage(0x25, "Bad Format: OCLeaderboard");
        }

        private static Dictionary<Mobile, LeaderboardEntry> _entries = new Dictionary<Mobile, LeaderboardEntry>();

        private static List<LeaderboardEntry> _recentKills, _recentKillPoints, _recentDamage, _recentBraziers, _recentHealing;

        private static List<LeaderboardEntry> _top10Kills = new List<LeaderboardEntry>(10);
        private static List<LeaderboardEntry> _top10KillPoints = new List<LeaderboardEntry>(10);
        private static List<LeaderboardEntry> _top10Damage = new List<LeaderboardEntry>(10);
        private static List<LeaderboardEntry> _top10Braziers = new List<LeaderboardEntry>(10);
        private static List<LeaderboardEntry> _top10Healing = new List<LeaderboardEntry>(10);

        public static Dictionary<Mobile, LeaderboardEntry> GetAllEntries()
        {
            return _entries;
        }

        public static void RegisterKill(Mobile m)
        {
            LeaderboardEntry e = Find(m);
            e.m_AllianceScore_Kills++;
            e.Kills++;
        }
        public static void RegisterKillPoints(Mobile m, int amt)
        {
            LeaderboardEntry e = Find(m);
            e.m_AllianceScore_KillPointDelta += amt;
            e.KillPoints += amt;
        }
        public static void RegisterDamage(Mobile m, int amt)
        {
            LeaderboardEntry e = Find(m);
            e.m_AllianceScore_Damage += amt;
            e.TotalDamage += amt;
        }
        public static void RegisterHealing(Mobile m, int amt)
        {
            LeaderboardEntry e = Find(m);
            e.m_AllianceScore_Healing += amt;
            e.TotalHealing += amt;
        }
        public static void RegisterBrazier(Mobile m)
        {
            LeaderboardEntry e = Find(m);
            e.m_AllianceScore_Braziers++;
            e.BraziersLit++;
        }

        private static LeaderboardEntry Find(Mobile m)
        {
            LeaderboardEntry entry;

            _entries.TryGetValue(m, out entry);

            if (entry == null)
            {
                entry = new LeaderboardEntry(m);
                _entries.Add(m, entry);
            }

            return entry;
        }

        public static bool WasActiveInActualTownFight(LeaderboardEntry entry)
        {
            // these 3 metrics are only tracked during OCB hour when the player is inside the vulnerable towns region
            return entry.BraziersLit > 0 || entry.m_AllianceScore_Damage > 0 || entry.m_AllianceScore_Healing > 0;
        }

        private static float[] random_multip = new float[] { 1.0f, 1.05f, 1.1f };
        public static void Clear()
        {
            // Called when last OCB hour STARTS
            float multip = random_multip[Utility.Random(random_multip.Length)];
            // commit last batch of scoring, this is done daily
            // should be changed to be town based
            foreach (KeyValuePair<Mobile, LeaderboardEntry> kvp in _entries)
            {
                PlayerMobile player = kvp.Key as PlayerMobile;
                if (player != null && !player.Deleted && player.TownsystemPlayerState != null)
                {
                    // Max per hour: 35
                    int total_increase = Math.Min(5, kvp.Value.m_AllianceScore_Kills);			// tracked outside OCB hour as well
                    total_increase += Math.Min(10, kvp.Value.m_AllianceScore_KillPointDelta);	// tracked outside OCB hour as well
                    total_increase += Math.Min(10, kvp.Value.m_AllianceScore_Braziers * 5);		// tracked during OCB hour only
                    total_increase += Math.Min(5, kvp.Value.m_AllianceScore_Damage / 1000);		// tracked during OCB hour only
                    total_increase += Math.Min(5, kvp.Value.m_AllianceScore_Healing / 500);		// tracked during OCB hour only

                    total_increase = (int)(total_increase * multip);

                    player.TownsystemPlayerState.AllianceScore += total_increase;
                    if (player.TownsystemPlayerState.AllianceScore < 0)
                        player.TownsystemPlayerState.AllianceScore = 0;
                }
            }

            _entries.Clear();
        }

        public static List<Mobile> GetActivePlayers()
        {
            var list = new List<Mobile>(_recentBraziers.Count);

            for (int i = 0; i < _recentBraziers.Count; ++i)
                if (_recentBraziers[i].BraziersLit > 0)
                    list.Add(_recentBraziers[i].Mobile);

            return list;
        }

        public static void StopCounting()
        {
            _recentBraziers = new List<LeaderboardEntry>(_entries.Values);
            _recentBraziers.Sort(delegate(LeaderboardEntry e1, LeaderboardEntry e2) { return e2.BraziersLit.CompareTo(e1.BraziersLit); });

            _recentKills = new List<LeaderboardEntry>(_entries.Values);
            _recentKills.Sort(delegate(LeaderboardEntry e1, LeaderboardEntry e2) { return e2.Kills.CompareTo(e1.Kills); });

            _recentKillPoints = new List<LeaderboardEntry>(_entries.Values);
            _recentKillPoints.Sort(delegate(LeaderboardEntry e1, LeaderboardEntry e2) { return e2.KillPoints.CompareTo(e1.KillPoints); });

            _recentDamage = new List<LeaderboardEntry>(_entries.Values);
            _recentDamage.Sort(delegate(LeaderboardEntry e1, LeaderboardEntry e2) { return e2.TotalDamage.CompareTo(e1.TotalDamage); });

            _recentHealing = new List<LeaderboardEntry>(_entries.Values);
            _recentHealing.Sort(delegate(LeaderboardEntry e1, LeaderboardEntry e2) { return e2.TotalHealing.CompareTo(e1.TotalHealing); });
        }

        public class LeaderboardEntry : IComparable
        {
            private Mobile _mobile;
            private int _killPoints;
            private int _kills;
            private int _totalDamage;
            private int _totalHealing;
            private int _braziersLit;

            // alliance score factors
            public int m_AllianceScore_Kills;
            public int m_AllianceScore_KillPointDelta;
            public int m_AllianceScore_Damage;
            public int m_AllianceScore_Healing;
            public int m_AllianceScore_Braziers;

            public Mobile Mobile { get { return _mobile; } }
            public int KillPoints
            {
                get
                {
                    return _killPoints;
                }
                set
                {
                    _killPoints = value;
                }
            }
            public int TotalDamage
            {
                get
                {
                    return _totalDamage;
                }
                set
                {
                    _totalDamage = value;
                }
            }
            public int TotalHealing
            {
                get
                {
                    return _totalHealing;
                }
                set
                {
                    _totalHealing = value;
                }
            }
            public int BraziersLit
            {
                get
                {
                    return _braziersLit;
                }
                set
                {
                    _braziersLit = value;
                }
            }
            public int Kills
            {
                get
                {
                    return _kills;
                }
                set
                {
                    _kills = value;
                }
            }

            public LeaderboardEntry(Mobile m)
            {
                _mobile = m;
                _killPoints = 0;
                _totalDamage = 0;
                _totalHealing = 0;
                _braziersLit = 0;
                _kills = 0;
            }

            public int CompareTo(object obj)
            {
                return _killPoints - ((LeaderboardEntry)obj)._killPoints;
            }
        }

        public class LeaderboardGump : Gump
        {

            public LeaderboardGump()
                : base(0, 0)
            {

                AddPage(0);
                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;
                this.AddPage(0);
                this.AddImage(0, 44, 206);
                this.AddImageTiled(44, 85, 658, 318, 200);
                this.AddImage(44, 44, 201);
                this.AddImage(699, 45, 207);
                this.AddImage(0, 88, 202);
                this.AddImage(699, 87, 203);
                this.AddImage(0, 403, 204);
                this.AddImage(699, 403, 205);
                this.AddImage(44, 403, 233);
                this.AddImage(272, 44, 201);
                this.AddImage(53, 22, 1419);
                this.AddImage(129, 4, 1417);
                this.AddImage(140, 13, 5608);
                this.AddImage(272, 403, 233);
                this.AddButton(40, 120, 4005, 4007, 1, GumpButtonType.Page, 1);
                this.AddLabel(75, 120, 0, @"KillPoints Gained");
                this.AddButton(40, 160, 4005, 4007, 2, GumpButtonType.Page, 2);
                this.AddLabel(75, 160, 0, @"Kills");
                this.AddButton(41, 200, 4005, 4007, 3, GumpButtonType.Page, 3);
                this.AddLabel(75, 200, 0, @"Braziers Lit");
                this.AddButton(41, 240, 4005, 4007, 4, GumpButtonType.Page, 4);
                this.AddLabel(75, 240, 0, @"Damage Dealt");
                this.AddButton(41, 280, 4005, 4007, 5, GumpButtonType.Page, 5);
                this.AddLabel(75, 280, 0, @"Hit Points Healed");
                this.AddLabel(375, 72, 0, @"Militia Leaderboard");

                #region KILLPOINTS
                AddPage(1);
                string _strRKP = "<center>TIMESLOT KILL POINTS DELTA</center><br><br>";
                for (int i = 0; i < 5; i++)
                {
                    if (_recentKillPoints == null || _recentKillPoints.Count <= i)
                        break;

                    Mobile m = _recentKillPoints[i].Mobile;

                    if (m == null)
                        continue;

                    _strRKP = String.Concat(_strRKP, _recentKillPoints[i].KillPoints, " - ", m.Name, "<br>");
                }
                this.AddHtml(190, 106, 249, 291, _strRKP, (bool)true, (bool)false);

                string _strOKP = "<center>OVERALL KILL POINTS GAINED</center><br><br><center>Disabled</center>";
                for (int i = 0; i < 5; i++)
                {
                    if (_top10KillPoints == null || _top10KillPoints.Count <= i)
                        break;

                    Mobile m = _top10KillPoints[i].Mobile;

                    if (m == null)
                        continue;

                    _strOKP = String.Concat(_strOKP, _top10KillPoints[i].KillPoints, " - ", m.Name, "<br>");
                }
                this.AddHtml(455, 106, 249, 291, _strOKP, (bool)true, (bool)false);
                #endregion

                #region KILLS
                AddPage(2);
                string _strRK = "<center>TIMESLOT KILLS</center><br><br>";
                for (int i = 0; i < 5; i++)
                {
                    if (_recentKills == null || _recentKills.Count <= i)
                        break;

                    Mobile m = _recentKills[i].Mobile;

                    if (m == null)
                        continue;

                    _strRK = String.Concat(_strRK, _recentKills[i].Kills, " - ", m.Name, "<br>");
                }
                this.AddHtml(190, 106, 249, 291, _strRK, (bool)true, (bool)false);

                string _strOK = "<center>OVERALL KILLS</center><br><br><center>Disabled</center>";
                for (int i = 0; i < 5; i++)
                {
                    if (_top10Kills == null || _top10Kills.Count <= i)
                        break;

                    Mobile m = _top10Kills[i].Mobile;

                    if (m == null)
                        continue;

                    _strOK = String.Concat(_strOK, _top10Kills[i].Kills, " - ", m.Name, "<br>");
                }
                this.AddHtml(455, 106, 249, 291, _strOK, (bool)true, (bool)false);
                #endregion

                #region BRAZIERS
                AddPage(3);
                string _strRBL = "<center>TIMESLOT BRAZIERS LIT</center><br><br>";
                for (int i = 0; i < 5; i++)
                {
                    if (_recentBraziers == null || _recentBraziers.Count <= i)
                        break;

                    Mobile m = _recentBraziers[i].Mobile;

                    if (m == null)
                        continue;

                    _strRBL = String.Concat(_strRBL, _recentBraziers[i].BraziersLit, " - ", m.Name, "<br>");
                }
                this.AddHtml(190, 106, 249, 291, _strRBL, (bool)true, (bool)false);

                string _strOBL = "<center>OVERALL BRAZIERS LIT</center><br><br><center>Disabled</center>";
                for (int i = 0; i < 5; i++)
                {
                    if (_top10Braziers == null || _top10Braziers.Count <= i)
                        break;

                    Mobile m = _top10Braziers[i].Mobile;

                    if (m == null)
                        continue;

                    _strOBL = String.Concat(_strOBL, _top10Braziers[i].BraziersLit, " - ", m.Name, "<br>");
                }
                this.AddHtml(455, 106, 249, 291, _strOBL, (bool)true, (bool)false);
                #endregion

                #region DAMAGE DEALT
                AddPage(4);
                string _strRDD = "<center>TIMESLOT DAMAGE DEALT</center><br><br>";
                for (int i = 0; i < 5; i++)
                {
                    if (_recentDamage == null || _recentDamage.Count <= i)
                        break;

                    Mobile m = _recentDamage[i].Mobile;

                    if (m == null)
                        continue;

                    _strRDD = String.Concat(_strRDD, _recentDamage[i].TotalDamage, " - ", m.Name, "<br>");
                }
                this.AddHtml(190, 106, 249, 291, _strRDD, (bool)true, (bool)false);

                string _strODD = "<center>OVERALL DAMAGE DEALT</center><br><br><center>Disabled</center>";
                for (int i = 0; i < 5; i++)
                {
                    if (_top10Damage == null || _top10Damage.Count <= i)
                        break;

                    Mobile m = _top10Damage[i].Mobile;

                    if (m == null)
                        continue;

                    _strODD = String.Concat(_strODD, _top10Damage[i].TotalDamage, " - ", m.Name, "<br>");
                }
                this.AddHtml(455, 106, 249, 291, _strODD, (bool)true, (bool)false);
                #endregion

                #region HEALTH HEALED
                AddPage(5);
                string _strRHPH = "<center>TIMESLOT HIT POINTS HEALED</center><br><br>";
                for (int i = 0; i < 5; i++)
                {
                    if (_recentHealing == null || _recentHealing.Count <= i)
                        break;

                    Mobile m = _recentHealing[i].Mobile;

                    if (m == null)
                        continue;

                    _strRHPH = String.Concat(_strRHPH, _recentHealing[i].TotalHealing, " - ", m.Name, "<br>");
                }
                this.AddHtml(190, 106, 249, 291, _strRHPH, (bool)true, (bool)false);

                string _strOHPH = "<center>OVERALL HIT POINTS HEALED</center><br><br><center>Disabled</center>";
                for (int i = 0; i < 5; i++)
                {
                    if (_top10Healing == null || _top10Healing.Count <= i)
                        break;

                    Mobile m = _top10Healing[i].Mobile;

                    if (m == null)
                        continue;

                    _strOHPH = String.Concat(_strOHPH, _top10Healing[i].TotalHealing, " - ", m.Name, "<br>");
                }
                this.AddHtml(455, 106, 249, 291, _strOHPH, (bool)true, (bool)false);
                #endregion
            }
        }
    }



}
