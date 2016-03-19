using Server.Custom.Battlegrounds.Regions;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Battlegrounds.Gumps
{
    class BattlegroundHelpGump : Gump
    {
        private enum Buttons
        {
            OK = 0,
            Wiki = 1
        }

        public BattlegroundHelpGump(Mobile from)
            : base(0, 0)
        {
            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;
            if (!(from.Region is BattlegroundRegion))
                return;
            var bg = ((BattlegroundRegion)from.Region).Battleground;

            AddPage(0);
            AddBackground(94, 79, 284, 284, 9200);
            AddLabel(190, 89, 0, @"Battleground Help");
            AddButton(330, 330, 4014, 4016, (int)Buttons.Wiki, GumpButtonType.Reply, 0);
            AddLabel(290, 330, 0, @"Wiki");
            AddButton(113, 330, 247, 248, (int)Buttons.OK, GumpButtonType.Reply, 0);

            AddLabel(105, 113, 0, @"Objective");
            if (bg is SiegeBattleground)
            {
                AddLabel(120, 135, 0, @"Offense:");
                AddLabel(182, 135, 0, @"Kill the enemy's NPC King,");
                AddLabel(130, 155, 0, @"located behind many penetrable walls.");
                AddLabel(120, 175, 0, @"Defense:");
                AddLabel(181, 175, 0, @"Prevent the offense from");
                AddLabel(130, 195, 0, @"getting to your king.");

                AddLabel(120, 300, 0, @"[teamswitch - Change teams");
            }
            else
            {
                AddLabel(120, 135, 0, "Capture the enemy team's flag.");
                AddLabel(120, 155, 0, "Protect your own flag from capture.");
                AddLabel(120, 175, 0, "Enemy flags must be turned in to your");
                AddLabel(130, 195, 0, "flag to capture them.");
            }

            AddLabel(105, 220, 0, @"Commands");
            AddLabel(120, 240, 0, @"[leave - Leave the battleground");
            AddLabel(120, 260, 0, @"[scoreboard - Show the score");
            AddLabel(120, 280, 0, @"[votekick - Vote to kick a player");
        }



        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            switch (info.ButtonID)
            {
                case (int)Buttons.OK:
                        break;
                case (int)Buttons.Wiki:
                        from.LaunchBrowser("http://uoancorp.com/wiki/index.php/Battlegrounds");
                        break;
            }
        }
    }

    class BattlegroundGump : Gump
    {
        private PlayerMobile m_From;
        private enum Buttons
        {
            OK=99,
            Disabled=100,
            Spectate=101
        }

        private bool CanEnterBattleground()
        {
            if (m_From.Region is BattlegroundRegion)
            {
                m_From.SendMessage("You cannot do that while in a battleground.");
                return false;
            }

            return true;
        }

        public BattlegroundGump(Mobile from)
            : base(50, 50)
        {
            m_From = from as PlayerMobile;

            if (!CanEnterBattleground())
                return;

            AddPage(0);
            AddBackground(3, 0, 635, 473, 9270);
            AddLabel(251, 66, 53, @"Battlegrounds");
            AddLabel(115, 125, 53, @"Name");
            AddLabel(225, 125, 53, @"Active");
            AddLabel(275, 125, 53, @"Free");
            AddLabel(330, 125, 53, @"Join");
            AddLabel(400, 125, 53, @"Queue");
            AddLabel(470, 125, 53, @"Mode");
            AddLabel(540, 125, 53, @"Players");

            int y = 165;
            for(int i = 0; i<Battleground.Instances.Count; i++)  
            {
                var bg = Battleground.Instances[i];
                if (!bg.Enabled)
                    continue;

                int buttonId = i + 1;

                AddLabel(115, y, 1153, bg.Name);
                if (bg.FreeConsume)
                    AddImage(280, y + 5, 2361); // active image
                else
                    AddImage(280, y + 5, 2360); // inactive image

                if (bg.Active)
                {
                    AddImage(235, y + 5, 2361); // active image
                    AddButton(330, y, 4014, 4016, buttonId, GumpButtonType.Reply, 0); // arrow button
                    AddButton(400, y, 4020, 4021, (int)Buttons.Disabled, GumpButtonType.Reply, 0); // disabled button
                }
                else
                {
                    AddImage(235, y + 5, 2360); // inactive image
                    AddButton(330, y, 4020, 4021, (int)Buttons.Disabled, GumpButtonType.Reply, 0); // disabled button
                    int showId = 4008;
                    int pressedId = 4009;
                    int responseId = buttonId;
                    if (bg.Queue.AlreadyQueued(m_From))
                    {
                        showId = 4020;
                        pressedId = 4021;
                        responseId = (int)Buttons.Disabled;
                    }

                    AddButton(400, y, showId, pressedId, responseId, GumpButtonType.Reply, 0); // queue button
                }
                string gameMode = "Siege";

                if (bg is PickedTeamCTFBattleground)
                    gameMode = "cCTF";
                else if (bg is RandomTeamCTFBattleground)
                    gameMode = "rCTF";

                AddLabel(470, y, 1153, gameMode);

                int hue = 1153;
                if (bg.PlayerCount > 0)
                    hue = 33;


                if (!(bg is PickedTeamCTFBattleground))
                {
                    if (bg.Active)
                        AddLabel(540, y, hue, string.Format("{0}/{1}", bg.PlayerCount, bg.MaximumPlayers));
                    else
                        AddLabel(540, y, hue, string.Format("{0}/{1}", bg.Queue.Count, bg.MinimumPlayers));
                }

                y += 35;
            }


            AddImage(0, 0, 9003);
            if (Battleground.Instances.Any(i => i.Active))
            {
                AddButton(400, 422, 4014, 4016, (int)Buttons.Spectate, GumpButtonType.Reply, 0);
                AddLabel(435, 422, 1153, "Spectate");
            }
            AddButton(547, 422, 247, 248, (int)Buttons.OK, GumpButtonType.Reply, 0); // OK button

        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (!CanEnterBattleground())
                return;

            switch (info.ButtonID)
            {
                case (int)Buttons.OK:
                case 0:
                    break;
                case (int)Buttons.Spectate:
                    m_From.SendGump(new SpectateGump(m_From));
                    break;
                case (int)Buttons.Disabled:
                    m_From.SendGump(new BattlegroundGump(m_From));
                    break;
                default:
                    HandleJoinQueue(info.ButtonID - 1);
                    break;
            }
        }

        private void HandleJoinQueue(int i)
        {
            if (i > Battleground.Instances.Count - 1)
            {
                m_From.SendMessage("Please try again.");
                m_From.SendGump(new BattlegroundGump(m_From));
                return;
            }

            var bg = Battleground.Instances[i];
            if (bg == null || !bg.Enabled)
                return;
            
            if (!(bg is PickedTeamCTFBattleground))
            {
                if (bg.Active)
                {
                    bg.Join(m_From);
                    m_From.SendGump(new BattlegroundHelpGump(m_From));
                }
                else
                {
                    bg.Queue.Join(m_From);
                    m_From.SendGump(new BattlegroundGump(m_From));
                }
            }
            else
            {
                if (bg.Active && bg.Teams.Any(team => team.Players.Contains(m_From)))
                {
                    bg.Join(m_From);
                    m_From.SendGump(new BattlegroundHelpGump(m_From));
                }
                else
                {
                    var pickedctf = bg as PickedTeamCTFBattleground;
                    pickedctf.JoinSmallestTeam(m_From);
                    pickedctf.UpdateTeamSelectionGumps();
                }
            }
        }
    }
}
