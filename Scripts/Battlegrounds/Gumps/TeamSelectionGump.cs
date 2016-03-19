using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Battlegrounds.Gumps
{

    class TeamSelectionGump : Gump
    {
        private enum Buttons
        {
            Cancel = 99,
            Ready  = 100,
        }

        private PlayerMobile m_From;
        private PickedTeamCTFBattleground m_Battleground;

        public TeamSelectionGump(Mobile from, PickedTeamCTFBattleground bg)
            : base(75, 75)
        {
            m_From = from as PlayerMobile;
            m_Battleground = bg;

            AddPage(0);
            AddBackground(80, 67, 610, 346, 9260);
            AddLabel(335, 86, 1153, bg.Name);

            int x = 110;
            for (int i = 0; i < bg.Teams.Count; i++ )
            {
                var team = bg.Teams[i];
                var responseID = i + 1;
                AddLabel(x + 30, 110, team.Color - 1, string.Format("{0} Team", team.Name));
                
                var html = new StringBuilder();
                foreach (var player in team.Players) 
                {
                    string ready = "";
                    if (m_Battleground.ReadyPlayers.Contains(player))
                        ready = "Ready: ";

                    html.Append(string.Format("{1}{0}<br/>", player.Name, ready)); 
                }
                AddHtml(x, 150, 130, 165, html.ToString(), true, true);

                AddButton(x + 70, 325, 4014, 4015, responseID, GumpButtonType.Reply, 0);
                AddLabel(x + 30, 325, team.Color -1, @"Join");

                x += 140;
            }

            AddButton(95, 371, 4014, 4016, (int)Buttons.Ready, GumpButtonType.Reply, 0);
            if (m_Battleground.ReadyPlayers.Contains(m_From))
                AddLabel(125, 371, 1153, "Cancel");
            else
                AddLabel(125, 371, 1153, "Ready");

            AddLabel(182, 357, 1153, @"The match will begin as soon as enough ready players are on each team.");
            AddLabel(200, 379, 1153, string.Format("Each team requires {0} players, teams will be autobalanced.", m_Battleground.MinimumPlayersPerTeam));
            AddButton(594, 371, 242, 241, 0, GumpButtonType.Reply, 0);

        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            switch (info.ButtonID)
            {
                case 0:
                case (int)Buttons.Cancel:
                    LeaveTeam();
                    UpdateGumps();
                    break;
                case (int)Buttons.Ready:
                    UpdateReady();
                    UpdateGumps();
                    break;
                default:
                    SwitchTeams(info.ButtonID -1);
                    UpdateGumps();
                    break;
            }
        }

        private void UpdateReady()
        {
            m_Battleground.ToggleReady(m_From);
        }

        private void UpdateGumps()
        {
            m_Battleground.UpdateTeamSelectionGumps();
        }

        private void LeaveTeam()
        {
            var teams = m_Battleground.Teams.Where(t => t.Contains(m_From));

            foreach(var team in teams)
                team.Players.Remove(m_From);

            if (m_Battleground.ReadyPlayers.Contains(m_From))
                m_Battleground.ReadyPlayers.Remove(m_From);
        }

        private void SwitchTeams(int teamIdx)
        {
            if (teamIdx >= 0 && teamIdx < m_Battleground.Teams.Count)
            {
                LeaveTeam();
                m_Battleground.Teams[teamIdx].Add(m_From);
                m_Battleground.TryStart();
            }
        }
    }
}
