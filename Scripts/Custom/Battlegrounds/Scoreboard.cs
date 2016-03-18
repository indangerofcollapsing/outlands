using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Battlegrounds
{
    public enum Categories
    {
        [Description("Damage Done")]
        Damage,
        [Description("Healing")]
        Healing,
        [Description("Siege Damage")]
        SiegeDamage,
        [Description("Kills")]
        Kills,
        [Description("Deaths")]
        Deaths,
        [Description("Captures")]
        Captures,
        [Description("Returns")]
        Returns
    }

    public class Scoreboard
    {
        private Battleground m_Battleground;
        public Battleground Battleground { get { return m_Battleground; } }

        public Dictionary<PlayerMobile, Score> PlayerScores { get; protected set; }

        public Team Winner {get; set;}

        public Dictionary<Team, Score> TeamScores 
        {
            get 
            {
                var scores = new Dictionary<Team, Score>();
                foreach(var team in Battleground.Teams) 
                {
                    scores[team] = new Score(team);
                }

                foreach(var kvp in PlayerScores) 
                {
                    var team = Battleground.Teams.Find(t => t.Color == kvp.Value.TeamHue);
                    scores[team].Combine(kvp.Value);
                }

                return scores;
            }
        }

        public Scoreboard(Battleground battleground)
        {
            PlayerScores = new Dictionary<PlayerMobile, Score>();
            m_Battleground = battleground;
        }

        public List<Score> SortedPlayerScores()
        {
            return PlayerScores.Values.OrderByDescending(s => s.TotalScore() ).ToList();
        }

        public List<Score> SortedTeamScores()
        {
            return TeamScores.Values.OrderByDescending(s => s.TotalScore()).ToList();
        }

        public void AddPlayer(PlayerMobile player, int teamHue)
        {
            if (!PlayerScores.ContainsKey(player))
                PlayerScores[player] = new Score(player, teamHue);
        }

        public void RemovePlayer(PlayerMobile player)
        {
            if (PlayerScores.ContainsKey(player))
                PlayerScores.Remove(player);
        }

        public PlayerMobile TopOverallScore(Team team)
        {
            var score = SortedPlayerScores().Find(s => team.Players.Contains(s.Player));
            if (score != null)
                return score.Player;
            return null;
        }

        public PlayerMobile TopScoreForTeamAndCategory(Team team, Categories category)
        {
            PlayerMobile player = null;
            int score = 0;
            foreach (var kvp in PlayerScores)
            {
                if (kvp.Value.Scores[category] > score)
                    player = kvp.Key;

            }
            return player;
        }

        public void UpdateScore(PlayerMobile player, Categories category, int amount)
        {
            if (PlayerScores.ContainsKey(player))
                PlayerScores[player].Scores[category] += amount;
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(PlayerScores.Count);
            foreach (var kvp in PlayerScores)
            {
                writer.Write(kvp.Key);
                kvp.Value.Serialize(writer);
            }
        }

        public void Deserialize(GenericReader reader)
        {
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                var player = reader.ReadMobile() as PlayerMobile;
                var team = reader.ReadInt();
                PlayerScores[player] = new Score(player, team);
                PlayerScores[player].Deserialize(reader);
            }
        }

        internal void ChangeTeams(PlayerMobile player, int teamHue)
        {
            if (PlayerScores.ContainsKey(player))
            {
                var score = PlayerScores[player];
                score.TeamHue = teamHue;
            }
        }
    }

    public class Score 
    {
        public Dictionary<Categories, int> Scores;
        public PlayerMobile Player;
        public Team Team;
        public int TeamHue;

        public int TotalScore()
        {
            int adjusted = 0;

            adjusted += Scores[Categories.Damage];
            adjusted += Scores[Categories.Healing] * 3;
            adjusted += Scores[Categories.Kills] * 2;
            adjusted -= Scores[Categories.Deaths] * 5;
            // ctf
            adjusted += Scores[Categories.Captures] * 15;
            adjusted += Scores[Categories.Returns] * 5;
            // siege
            adjusted += Scores[Categories.SiegeDamage] * 2;

            return adjusted;
        }

        public void Combine(Score score)
        {
            Scores[Categories.Damage] += score.Scores[Categories.Damage];
            Scores[Categories.Healing] += score.Scores[Categories.Healing];
            Scores[Categories.SiegeDamage] += score.Scores[Categories.SiegeDamage];
            Scores[Categories.Kills] += score.Scores[Categories.Kills];
            Scores[Categories.Deaths] += score.Scores[Categories.Deaths];
            Scores[Categories.Captures] += score.Scores[Categories.Captures];
            Scores[Categories.Returns] += score.Scores[Categories.Returns];
        }

        public Score(Team team)
        {
            Team = team;
            TeamHue = team.Color;
            Scores = new Dictionary<Categories, int>();
            Scores[Categories.Damage] = 0;
            Scores[Categories.Healing] = 0;
            Scores[Categories.SiegeDamage] = 0;
            Scores[Categories.Kills] = 0;
            Scores[Categories.Deaths] = 0;
            Scores[Categories.Captures] = 0;
            Scores[Categories.Returns] = 0;
        }

        public Score(PlayerMobile player, int team)
        {
            Player = player;
            TeamHue = team;
            Scores = new Dictionary<Categories, int>();
            Scores[Categories.Damage] = 0;
            Scores[Categories.Healing] = 0;
            Scores[Categories.SiegeDamage] = 0;
            Scores[Categories.Kills] = 0;
            Scores[Categories.Deaths] = 0;
            Scores[Categories.Captures] = 0;
            Scores[Categories.Returns] = 0;
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write((int)TeamHue);

            writer.Write(Scores[Categories.Damage]);
            writer.Write(Scores[Categories.Healing]);
            writer.Write(Scores[Categories.SiegeDamage]);
            writer.Write(Scores[Categories.Kills]);
            writer.Write(Scores[Categories.Deaths]);
            writer.Write(Scores[Categories.Captures]);
            writer.Write(Scores[Categories.Returns]);
        }

        public void Deserialize(GenericReader reader)
        {
            Scores[Categories.Damage] = reader.ReadInt();
            Scores[Categories.Healing] = reader.ReadInt();
            Scores[Categories.SiegeDamage] = reader.ReadInt();
            Scores[Categories.Kills] = reader.ReadInt();
            Scores[Categories.Deaths] = reader.ReadInt();
            Scores[Categories.Captures] = reader.ReadInt();
            Scores[Categories.Returns] = reader.ReadInt();
        }
    }
}
