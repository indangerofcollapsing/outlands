using Server.Achievements;
using Server.Custom.Battlegrounds.Games;
using Server.Custom.Battlegrounds.Gumps;
using Server.Custom.Battlegrounds.Items;
using Server.Custom.Battlegrounds.Mobiles;
using Server.Custom.Battlegrounds.Regions;
using Server.Engines.XmlSpawner2;
using Server.Mobiles;
using Server.Spells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.ExtensionMethods;

namespace Server.Custom.Battlegrounds
{
    public enum BattlegroundState
    {
        Active,
        Inactive
    }

    public class Battleground
    {
        #region Constants
        public virtual int MinimumPlayers { get { return 2; } }
        public virtual int MaximumPlayers { get { return 40; } }
        public virtual TimeSpan RespawnDelay { get { return TimeSpan.FromSeconds(25); } }
        public virtual TimeSpan TimeLimit { get { return TimeSpan.FromMinutes(20); } }
        public virtual TimeSpan GameStartGracePeriod { get { return TimeSpan.FromSeconds(30); } }
        public virtual TimeSpan GameEndGracePeriod { get { return TimeSpan.FromSeconds(15); } }

#region Team Colors
        public static int GoldColor = 2125;
        public static int BlueColor = 2124;
        public static int RedColor = 2118;
        public static int GreenColor = 1367;

        public static int GoldFlagCarrierColor = 2107;
        public static int BlueFlagCarrierColor = 2119;
        public static int RedFlagCarrierColor = 2116;
        public static int GreenFlagCarrierColor = 2129;
#endregion

        protected bool m_Enabled;
        public bool Enabled 
        { 
            get 
            { 
                if (BattlegroundSettingsPersistence.Instance.Settings.ContainsKey(Name))
                {
                    return BattlegroundSettingsPersistence.Instance.Settings[Name].Enabled;
                }
                else
                {
                    BattlegroundSettingsPersistence.Instance.Settings.Add(Name, new BattlegroundSettings(m_Enabled, m_FreeConsume));
                    return m_Enabled;
                }
            } 
            set 
            {
                if (BattlegroundSettingsPersistence.Instance.Settings.ContainsKey(Name))
                {
                    BattlegroundSettingsPersistence.Instance.Settings[Name].Enabled = value;
                }
                else
                {
                    BattlegroundSettingsPersistence.Instance.Settings.Add(Name, new BattlegroundSettings(value, m_FreeConsume));
                }
            } 
        }

        protected bool m_FreeConsume;
        public bool FreeConsume 
        {
            get
            {
                if (BattlegroundSettingsPersistence.Instance.Settings.ContainsKey(Name))
                {
                    return BattlegroundSettingsPersistence.Instance.Settings[Name].FreeConsume;
                }
                else
                {
                    BattlegroundSettingsPersistence.Instance.Settings.Add(Name, new BattlegroundSettings(m_Enabled, m_FreeConsume));
                    return m_FreeConsume;
                }
            }
            set
            {
                if (BattlegroundSettingsPersistence.Instance.Settings.ContainsKey(Name))
                {
                    BattlegroundSettingsPersistence.Instance.Settings[Name].FreeConsume = value;
                }
                else
                {
                    BattlegroundSettingsPersistence.Instance.Settings.Add(Name, new BattlegroundSettings(m_Enabled, value));
                }
            } 
        }

        public static TimeSpan EjectTimeout { get { return TimeSpan.FromMinutes(15); } }

        protected bool m_Ended = true;
        public virtual bool Ended { get { return m_Ended; } set { m_Ended = value; } }
        #endregion

        #region Instances
        private static List<Battleground> m_Instances = new List<Battleground>();

        public static List<Battleground> Instances
        {
            get { return m_Instances; }
        }

        #endregion

        public BattlegroundState State { get; set; }

        public string Name { get; protected set; }

        public DateTime StartTime { get; set; }

        public Scoreboard CurrentScoreboard { get; set; }

        public List<Team> Teams { get; protected set; }

        public List<PlayerMobile> Spectators { get; protected set; }
        public Point3D SpectatePoint { get; protected set; }

        public int PlayerCount { get { return Teams.Aggregate(0, (acc, team) => acc + team.Count); } }

        public Battleground()
        {
            m_Instances.Add(this);
            Queue = new Queue(this);
            Name = "Default Battleground";
            Map = Map.Felucca;
            State = BattlegroundState.Inactive;
            StartTime = DateTime.MinValue;
            m_EjectVotes = new Dictionary<PlayerMobile, EjectVotes>();
            Spectators = new List<PlayerMobile>();
        }

        public Map Map { get; protected set; }

        public Queue Queue { get; private set; }

        public GameOverTimer GameOverTimer { get; protected set; }

        private Dictionary<PlayerMobile, EjectVotes> m_EjectVotes;

        public virtual void TryStart()
        {
            if (Queue.Count >= MinimumPlayers)
            {
                Start();
            }
        }

        public bool Active { get { return State == BattlegroundState.Active; } }

        protected virtual void Start()
        {
            m_Ended = false;
            State = BattlegroundState.Active;
            CurrentScoreboard = new Scoreboard(this);
            AskPlayersToJoin();
            StartTime = DateTime.UtcNow;
            StartTimers();
            Queue.Players.Clear();
        }

        protected virtual void StartTimers()
        {
            if (GameOverTimer != null && GameOverTimer.Running)
                GameOverTimer.Stop();

            GameOverTimer = new GameOverTimer(this);
            GameOverTimer.Start();
        }

        public virtual void Resume()
        {
            StartTimers();
        }

        protected virtual void Finish()
        {
            AwardCompleteAchievement();
            m_Ended = true;
            Timer.DelayCall(GameEndGracePeriod, () =>
            {
                if (!Active)
                    return;

                CleanUp();
            });
        }

        protected virtual void CleanUp()
        {
            StopTimers();

            ShowScoreboard();

            EjectPlayers();

            Spectators.Clear();
            foreach (var team in Teams)
            {
                foreach (var player in team.Players)
                {
                    team.RemoveRobe(player);
                }

                team.Players.Clear();
            }
            m_EjectVotes.Clear();
            Queue.Players.Clear();
            StartTime = DateTime.MinValue;
            State = BattlegroundState.Inactive;
        }

        public virtual void OnTimeout()
        {
            Finish();
        }

        protected virtual void StopTimers()
        {
            if (GameOverTimer != null && GameOverTimer.Running)
                GameOverTimer.Stop();
        }

        protected virtual void AwardCompleteAchievement()
        {
            foreach (var team in Teams)
                foreach (var player in team.Players)
                    DailyAchievement.TickProgress(Category.PvP, player, PvPCategory.CompleteBattleground);
        }

        protected virtual void ShowScoreboard()
        {
            foreach (var spectator in Spectators)
                if (spectator.NetState != null)
                    spectator.SendGump(new ScoreboardGump(spectator, CurrentScoreboard));

            foreach (var team in Teams)
                foreach (var player in team.Players)
                    if (player.NetState != null)
                        player.SendGump(new ScoreboardGump(player, CurrentScoreboard));
        }

        protected virtual void EjectPlayers()
        {
            foreach (var player in Spectators)
            {
                if (player.Map == Map.Internal)
                {
                    player.LogoutMap = Map.Felucca;
                    player.LogoutLocation = player.LastLocation;
                }
                else
                    player.MoveToWorld(player.LastLocation, Map.Felucca);
                
                ClearPlayerProperties(player);
            }

            foreach (var team in Teams)
                foreach (var player in team.Players)
                {
                    ClearPlayerProperties(player);
                    if (player.Map == Map.Internal)
                    {
                        player.LogoutMap = Map.Felucca;
                        player.LogoutLocation = player.LastLocation;
                    }
                    else
                        player.MoveToWorld(player.LastLocation, Map.Felucca);
                }
        }

        protected virtual void AskPlayersToJoin()
        {
            foreach (var player in Queue.Players)
            {
                player.SendGump(new JoinBattlegroundGump(player, this));
                player.SendMessage(string.Format("The {0} Battleground will begin shortly.", Name));
            }
        }

        public virtual void Spectate(PlayerMobile player)
        {
            if (!SpellHelper.CheckTravel(player, TravelCheckType.RecallFrom))
            {
                return;
            }
            if (SpellHelper.CheckCombat(player, true))
            {
                player.SendMessage("You cannot spectate a battleground while in combat.");
                return;
            }

            if (player.Spell is Spells.Spell)
                ((Spells.Spell)player.Spell).Disturb(Spells.DisturbType.Kill);

            player.DropHolding();

            Targeting.Target.Cancel(player);
            Spectators.Add(player);
            player.LastLocation = player.Location;
            player.MoveToWorld(SpectatePoint, Map);
            player.Spectating = true;
            player.SpectatingTimer = new PlayerMobile.SpectatorTimer(player);
            player.SpectatingTimer.Start();
        }

        public virtual void Join(PlayerMobile player)
        {
            if (PlayerCount >= MaximumPlayers)
            {
                player.SendMessage("This battleground is full!");
                return;
            }
            if (SpellHelper.CheckCombat(player, true))
            {
                player.SendMessage("You cannot enter a battleground while in combat.");
                return;
            }
            if (!SpellHelper.CheckTravel(player, TravelCheckType.RecallFrom))
            {
                return;
            }
            if (!player.Alive)
            {
                player.SendMessage("You cannot enter a battleground as a ghost.");
                return;
            }
            if (player.Region is BattlegroundRegion)
                return;

            EnterBattleground(player);
        }

        public virtual void EnterBattleground(PlayerMobile player)
        {
            Battleground.Instances.ForEach(bg => bg.Queue.Leave(player));

            if (player.Spell is Spells.Spell)
                ((Spells.Spell)player.Spell).Disturb(Spells.DisturbType.Kill);

            Targeting.Target.Cancel(player);
            player.DropHolding();
            ClearPlayerProperties(player);
        }

        public virtual void MoveToSpawn(PlayerMobile from)
        {
            var playerTeam = Teams.Find(team => team.Contains(from));
            if (playerTeam != null)
            {
                from.Location = playerTeam.SpawnPoint;
            }
        }

        public static bool AllowBeneficial(PlayerMobile from, PlayerMobile target)
        {
            Battleground battleground = ((BattlegroundRegion)from.Region).Battleground;

            return battleground.OnSameTeam(from, target);
        }

        public bool OnSameTeam(PlayerMobile from, PlayerMobile target)
        {
            var team1 = Teams.Find(team => team.Contains(from));
            var team2 = Teams.Find(team => team.Contains(target));

            return team1 == team2;
        }

        public void Broadcast(int hue, string message)
        {
            foreach (var team in Teams)
                foreach (var player in team.Players)
                    player.PrivateOverheadMessage(Network.MessageType.Regular, hue, true, message, player.NetState);
            foreach(var player in Spectators)
                player.PrivateOverheadMessage(Network.MessageType.Regular, hue, true, message, player.NetState);
        }

        public virtual void Leave(PlayerMobile player)
        {
            if (player.Spectating)
            {
                Spectators.Remove(player);
                if (player.SpectatingTimer != null && player.SpectatingTimer.Running)
                    player.SpectatingTimer.Stop();
            }
            else
            {
                var playerTeam = Teams.Find(team => team.Contains(player));

                if (playerTeam != null)
                {
                    playerTeam.RemoveRobe(player);
                    playerTeam.Remove(player);

                    if (CurrentScoreboard != null)
                        CurrentScoreboard.RemovePlayer(player);
                }
            }

            ClearPlayerProperties(player);

            if (player.LastLocation != Point3D.Zero)
                player.MoveToWorld(player.LastLocation, Map.Felucca);
            else
                player.MoveToWorld(new Point3D(1427, 1696, 0), Map.Felucca); // WBB
        }

        public virtual void ClearPlayerProperties(PlayerMobile player)
        {
            player.Spectating = false;
            player.IsDragging = false;
            player.DamageVulnerable = false;

            if (!player.Alive)
                player.Resurrect();

            if (player.SpectatingTimer != null && player.SpectatingTimer.Running)
                player.SpectatingTimer.Stop();

            player.Aggressed.Clear();
            player.Aggressors.Clear();

            player.MagicDamageAbsorb = 0; //Clear magic reflect
            player.MeleeDamageAbsorb = 0; //Clear reactive armor
            player.VirtualArmorMod = 0; //Clear protection
            player.Poison = null;

            player.Hits = player.HitsMax;
            player.Stam = player.StamMax;
            player.Mana = player.ManaMax;

            BuffInfo.RemoveBuff(player, BuffIcon.Agility);
            BuffInfo.RemoveBuff(player, BuffIcon.ArchProtection);
            BuffInfo.RemoveBuff(player, BuffIcon.Bless);
            BuffInfo.RemoveBuff(player, BuffIcon.Clumsy);
            BuffInfo.RemoveBuff(player, BuffIcon.Incognito);
            BuffInfo.RemoveBuff(player, BuffIcon.MagicReflection);
            BuffInfo.RemoveBuff(player, BuffIcon.MassCurse);
            BuffInfo.RemoveBuff(player, BuffIcon.Invisibility);
            BuffInfo.RemoveBuff(player, BuffIcon.HidingAndOrStealth);
            BuffInfo.RemoveBuff(player, BuffIcon.Paralyze);
            BuffInfo.RemoveBuff(player, BuffIcon.Poison);
            BuffInfo.RemoveBuff(player, BuffIcon.Polymorph);
            BuffInfo.RemoveBuff(player, BuffIcon.Protection);
            BuffInfo.RemoveBuff(player, BuffIcon.ReactiveArmor);
            BuffInfo.RemoveBuff(player, BuffIcon.Strength);
            BuffInfo.RemoveBuff(player, BuffIcon.Weaken);
            BuffInfo.RemoveBuff(player, BuffIcon.FeebleMind);

            player.RemoveStatModsBeginningWith("[Magic]");
        }

        public void KillFeedBroadcast(PlayerMobile killer, PlayerMobile victim)
        {
            FeedBroadcast(string.Format("{0} has slain {1}.", killer.Name, victim.Name), hue: 33);
        }

        public void FeedBroadcast(string message, Team team = null, int hue = 0)
        {
            if (team != null)
                foreach (var player in team.Players)
                    player.SendMessage(hue, message);
            else
            {
                foreach (var t in Teams)
                    foreach (var player in t.Players)
                        player.SendMessage(hue, message);

                foreach (var player in Spectators)
                    player.SendMessage(hue, message);
            }
        }

        protected virtual void AwardWinners(Team winners)
        {
            //var top = CurrentScoreboard.TopOverallScore(winners);
            //if (top != null && top.NetState != null)
            //{
            //    var trophy = new BattlegroundTrophy(winners.Color, string.Format("An Overall Top Score Trophy - {0}", top.Name));
            //    top.AddToBackpack(trophy);
            //}

            //var categories = new Categories[] { Categories.Damage, Categories.Deaths, Categories.Healing, Categories.Kills };
            //foreach (var category in categories)
            //{
            //    var player = CurrentScoreboard.TopScoreForTeamAndCategory(winners, category);
            //    if (player == null || player.NetState == null)
            //        continue;

            //    var description = category.GetAttribute<DescriptionAttribute>().Description;
            //    var trophy = new BattlegroundTrophy(winners.Color, string.Format("A Top {0} Trophy - {1}", description, player.Name));

            //    player.AddToBackpack(trophy);
            //}

            //if (this is CTFBattleground)
            //{
            //    var ctfCategories = new Categories[] { Categories.Captures, Categories.Returns };
            //    foreach (var category in ctfCategories)
            //    {
            //        var player = CurrentScoreboard.TopScoreForTeamAndCategory(winners, category);
            //        if (player == null || player.NetState == null)
            //            continue;

            //        var description = category.GetAttribute<DescriptionAttribute>().Description;
            //        var trophy = new BattlegroundTrophy(winners.Color, string.Format("A Top {0} Trophy - {1}", description, player.Name));

            //        player.AddToBackpack(trophy);
            //    }
            //}
            //else
            //{
            //    var topSiegeer = CurrentScoreboard.TopScoreForTeamAndCategory(winners, Categories.SiegeDamage);
            //    if (topSiegeer != null && topSiegeer.NetState != null)
            //    {
            //        var trophy = new BattlegroundTrophy(winners.Color, string.Format("A Top Siege Damage Trophy - {0}", topSiegeer.Name));

            //        topSiegeer.AddToBackpack(trophy);
            //    }
            //}

        }

        public void VoteEject(PlayerMobile from, PlayerMobile target)
        {
            if (from.Spectating)
                return;

            if (OnSameTeam(from, target))
            {
                if (!m_EjectVotes.ContainsKey(target))
                    m_EjectVotes[target] = new EjectVotes();

                m_EjectVotes[target].AddVote(from);

                var team = Teams.Find(t => t.Contains(from));

                int threshold = (int)(team.Count * 0.66);

                if (m_EjectVotes[target].Count >= threshold)
                {
                    FeedBroadcast(string.Format("{0} has been ejected from the game!", target.Name));
                    target.NextBattlegroundTime = DateTime.UtcNow + EjectTimeout;
                    Leave(target);
                }
                else
                {
                    FeedBroadcast(string.Format("{0} has voted to eject {1} from the game. {2} votes needed.", from.Name, target.Name, threshold - m_EjectVotes[target].Count), team);
                }
            }
            else
            {
                from.SendMessage("You cannot vote to eject a player on the enemy team");
            }
        }

        private class EjectVotes
        {
            private List<PlayerMobile> Voters;
            public int Count { get { return Voters.Count; } }

            public EjectVotes()
            {
                Voters = new List<PlayerMobile>();
            }

            public void AddVote(PlayerMobile from)
            {
                if (!Voters.Contains(from))
                    Voters.Add(from);
            }
        }
    }
}
