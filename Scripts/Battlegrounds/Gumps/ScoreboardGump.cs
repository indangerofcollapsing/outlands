using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Battlegrounds.Gumps
{
    class ScoreboardGump : Gump
    {
        private enum Buttons
        {
            Prev,
            Next
        }

        PlayerMobile m_From;
        Scoreboard m_Scoreboard;
        int perPage = 13;
        int currentPage = 0;
        public ScoreboardGump(Mobile from, Scoreboard scoreboard, int page = 0) 
            : base(50, 50)
        {
            m_From = from as PlayerMobile;
            m_Scoreboard = scoreboard;
            currentPage = page;

            AddPage(0);
            AddBackground(9, 6, 501, 439, 9270);
            AddImageTiled(26, 40, 467, 2, 9102);
            AddLabel(30, 20, 1153, @"Name");
            AddLabel(160, 20, 1153, @"Kills");
            AddLabel(200, 20, 1153, @"Deaths");
            AddLabel(255, 20, 1153, @"Damage");
            AddLabel(440, 20, 1153, @"Healing");
            if (scoreboard.Battleground is CTFBattleground)
            {
                AddLabel(315, 20, 1153, @"Captures");
                AddLabel(380, 20, 1153, @"Returns");
            }
            else
            {
                AddLabel(325, 20, 1153, @"Siege Damage");
            }


            int y = 55;
            int spacer = 20;

            var sortedTeamScores = scoreboard.SortedTeamScores();
            foreach(var score in sortedTeamScores) 
            {
                int hue = score.TeamHue - 1;

                AddLabel(35, y, hue, score.Team.Name);
                AddLabel(165, y, hue, score.Scores[Categories.Kills].ToString());
                AddLabel(205, y, hue, score.Scores[Categories.Deaths].ToString());
                AddLabel(260, y, hue, score.Scores[Categories.Damage].ToString());
                if (scoreboard.Battleground is CTFBattleground)
                {
                    AddLabel(320, y, hue, score.Scores[Categories.Captures].ToString());
                    AddLabel(385, y, hue, score.Scores[Categories.Returns].ToString());
                }
                else
                {
                    AddLabel(330, y, hue, score.Scores[Categories.SiegeDamage].ToString());
                }

                AddLabel(445, y, hue, score.Scores[Categories.Healing].ToString());

                y += spacer;
            }

            y += spacer;

            AddImageTiled(26, y, 467, 2, 9102);

            y += spacer;

            var sortedScores = scoreboard.SortedPlayerScores().Skip(page * perPage).Take(perPage);
            foreach (var score in sortedScores)
            {
                int hue = score.TeamHue - 1;

                AddLabel(35, y, hue, score.Player.Name);
                AddLabel(165, y, hue, score.Scores[Categories.Kills].ToString());
                AddLabel(205, y, hue, score.Scores[Categories.Deaths].ToString());
                AddLabel(260, y, hue, score.Scores[Categories.Damage].ToString());
                if (scoreboard.Battleground is CTFBattleground)
                {
                    AddLabel(320, y, hue, score.Scores[Categories.Captures].ToString());
                    AddLabel(385, y, hue, score.Scores[Categories.Returns].ToString());
                }
                else
                {
                    AddLabel(330, y, hue, score.Scores[Categories.SiegeDamage].ToString());
                }

                AddLabel(445, y, hue, score.Scores[Categories.Healing].ToString());

                y += spacer;
            }

            if (scoreboard.Winner != null)
                AddLabel(35, 410, scoreboard.Winner.Color - 1, string.Format("The {0} team wins!", scoreboard.Winner.Name));

            // Page buttons
            if (scoreboard.PlayerScores.Count > ((currentPage + 1) * perPage))
                AddButton(485, 422, 2224, 2224, (int)Buttons.Next, GumpButtonType.Reply, 0);
            if (currentPage > 0)
                AddButton(461, 422, 2223, 2223, (int)Buttons.Prev, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            switch ((Buttons)info.ButtonID)
            {
                case Buttons.Prev:
                    if (currentPage > 0)
                        m_From.SendGump(new ScoreboardGump(m_From, m_Scoreboard, currentPage - 1));
                    break;
                case Buttons.Next:
                    m_From.SendGump(new ScoreboardGump(m_From, m_Scoreboard, currentPage + 1));
                    break;
            }
        }
    }
}
