using Server.Custom.Battlegrounds.Gumps;
using Server.Custom.Battlegrounds.Items;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Battlegrounds
{
    public class CTFBattleground : Battleground
    {
        public virtual int MinimumPlayersPerTeam { get { return 4; } }
        public virtual int ScoreRequiredToWin { get { return 5; } }
        public override TimeSpan TimeLimit { get { return TimeSpan.FromMinutes(30); } }

        public CTFBattlegroundPersistence Persistence { get; set; }
        protected CTFAutobalancer Autobalancer { get; set; }
        public override TimeSpan GameEndGracePeriod { get { return TimeSpan.FromSeconds(5); } }

        public CTFBattleground()
            : base()
        {
            Map = Map.Tokuno;
        }

        public override void OnTimeout()
        {
            base.OnTimeout();

            // team with the highest score wins
            var leader = Teams.First();
            int leadCaptures = 0;
            int leadOverall = 0;
            foreach (var team in Teams)
            {
                int captures = 0;
                int overall = 0;
                foreach(var player in team.Players)
                {
                    if (CurrentScoreboard.PlayerScores.ContainsKey(player))
                    {
                        captures += CurrentScoreboard.PlayerScores[player].Scores[Categories.Captures];
                        overall += CurrentScoreboard.PlayerScores[player].TotalScore();
                    }
                }
                // winner determined by captures or overall score if tied by captures
                if (captures > leadCaptures || (captures == leadCaptures && overall > leadOverall))
                {
                    leader = team;
                    leadCaptures = captures;
                    leadOverall = overall;
                }
            }

            Broadcast(leader.Color, string.Format("The {0} team has won!", leader.Name));
            AwardWinners(leader);
        }

        public override void Leave(PlayerMobile player)
        {
            RemoveFlagFrom(player);
            base.Leave(player);
        }

        protected override void StartTimers()
        {
            base.StartTimers();
            if (Autobalancer != null && Autobalancer.Running)
                Autobalancer.Stop();

            Autobalancer = new CTFAutobalancer(this);
            Autobalancer.Start();
        }

        protected override void StopTimers()
        {
            base.StopTimers();

            if (Autobalancer != null && Autobalancer.Running)
                Autobalancer.Stop();
        }

        protected override void CleanUp()
        {
            base.CleanUp();

            if (Persistence != null && !Persistence.Deleted)
                Persistence.Delete();

            foreach (var team in Teams)
                team.RemoveFlag();
        }

        public override void Resume()
        {
            base.Resume();
            foreach (var team in Teams)
                team.SetupFlag();
        }

        protected override void Start()
        {
            base.Start();
            if (Persistence != null && !Persistence.Deleted)
                Persistence.Delete();

            Persistence = new CTFBattlegroundPersistence(this);

            foreach (var team in Teams)
                team.SetupFlag();
        }

        public virtual void OnFlagPickedUp(PlayerMobile player, CTFFlag flag)
        {
            var team = Teams.Find(t => t.Contains(player));
            FeedBroadcast(string.Format("{0} has picked up the {1} team's flag!", player.Name, flag.Team.Name), hue: team.Color);
        }

        public virtual void OnFlagReturned(PlayerMobile player)
        {
            var team = Teams.Find(t => t.Players.Contains(player));
            FeedBroadcast(string.Format("{0} has returned the {1} team's flag!", player.Name, team.Name), hue: team.Color);
            CurrentScoreboard.UpdateScore(player, Categories.Returns, 1);
        }

        public virtual void OnFlagCaptured(PlayerMobile player, CTFFlag flag)
        {
            var playersTeam = Teams.Find(t => t.Contains(player));
            FeedBroadcast(string.Format("{0} has captured the {1} team's flag", player.Name, flag.Team.Name), hue: playersTeam.Color);
            CurrentScoreboard.UpdateScore(player, Categories.Captures, 1);
            CheckForWin(playersTeam);
        }

        protected virtual void CheckForWin(Team team)
        {
            int captures = 0;
            foreach(var player in team.Players)
            {
                if (CurrentScoreboard.PlayerScores.ContainsKey(player))
                    captures += CurrentScoreboard.PlayerScores[player].Scores[Categories.Captures];
            }

            if (captures >= ScoreRequiredToWin)
            {
                Broadcast(team.Color, string.Format("The {0} team has won!", team.Name));
                AwardWinners(team);
                CurrentScoreboard.Winner = team;
                Finish();
            }
        }

        protected virtual void RemoveFlagFrom(PlayerMobile player)
        {
            if (CTFFlag.ExistsOn(player))
            {
                var flag = player.Backpack.FindItemByType<CTFFlag>();
                if (flag != null)
                {
                    FeedBroadcast(string.Format("{0} has dropped the {1} team's flag!", player.Name, flag.Team.Name), hue: flag.Team.Color);
                    flag.SendHome();
                }
            }
        }

        public virtual void BalanceTeams()
        {
            var largestTeam = Teams.OrderByDescending(t => t.Count).First();
            var smallestTeam = Teams.OrderBy(t => t.Count).First();
            int largestCount = largestTeam.Count;
            int smallestCount = smallestTeam.Count;

            int difference = largestCount - smallestCount;

            if (difference >= 2)
            {
                int toMove = difference / 2;

                for (int i = 0; i < toMove; i++)
                {
                    var player = largestTeam.Players.Last();

                    RemoveFlagFrom(player);

                    largestTeam.Remove(player);
                    largestTeam.RemoveRobe(player);

                    CurrentScoreboard.ChangeTeams(player, smallestTeam.Color);

                    smallestTeam.Add(player);
                    smallestTeam.AddRobe(player);

                    MoveToSpawn(player);
                    player.SendMessage("You have switched teams");
                }
            }
        }
    }

    // hotjoin ctf
    public class RandomTeamCTFBattleground : CTFBattleground
    {

        public RandomTeamCTFBattleground()
            : base()
        {
        }

        public override void EnterBattleground(PlayerMobile player)
        {
            base.EnterBattleground(player);

            var smallestTeam = Teams.OrderBy(team => team.Count).First();
            smallestTeam.Add(player);
            CurrentScoreboard.AddPlayer(player, smallestTeam.Color);
            player.LastLocation = player.Location;
            player.MoveToWorld(smallestTeam.SpawnPoint, Map);
            smallestTeam.AddRobe(player);
        }

        public override void TryStart()
        {
            if (Queue.Count >= MinimumPlayers)
                Start();
        }

    }

    // players will be prompted to pick a team and late joins will not be allowed
    // Queue is not involved with this game mode
    public class PickedTeamCTFBattleground : CTFBattleground
    {

        public List<PlayerMobile> ReadyPlayers { get; private set; }

        public PickedTeamCTFBattleground()
            : base()
        {
            ReadyPlayers = new List<PlayerMobile>();
        }

        public void ToggleReady(PlayerMobile player)
        {
            if (ReadyPlayers.Contains(player))
                ReadyPlayers.Remove(player);
            else if (!ReadyPlayers.Contains(player))
                ReadyPlayers.Add(player);

            TryStart();
        }

        public override void EnterBattleground(PlayerMobile player)
        {
            base.EnterBattleground(player);
            var playersTeam = Teams.Find(team => team.Contains(player));
            CurrentScoreboard.AddPlayer(player, playersTeam.Color);
            player.LastLocation = player.Location;
            player.MoveToWorld(playersTeam.SpawnPoint, Map);
            playersTeam.AddRobe(player);
        }

        public override void TryStart()
        {
            bool ready = PlayerCount == ReadyPlayers.Count;
            if (Teams.All(t => t.Players.Count >= MinimumPlayersPerTeam) && ready)
            {
                BalanceTeams();
                Start();
            }
        }

        protected override void Start()
        {
            base.Start();
            foreach (var team in Teams)
                foreach (var player in team.Players)
                {
                    EnterBattleground(player);
                }

            ReadyPlayers.Clear();
        }

        public void UpdateTeamSelectionGumps() 
        {
            foreach (var team in Teams)
            {
                foreach (var p in team.Players)
                {
                    p.CloseGump(typeof(TeamSelectionGump));
                    if (!Active)
                        p.SendGump(new TeamSelectionGump(p, this));
                }
            }
        }

        public void JoinSmallestTeam(PlayerMobile player)
        {
            var smallestTeam = Teams.OrderBy(team => team.Count).First();
            smallestTeam.Add(player);
            TryStart();
        }
    }
}
