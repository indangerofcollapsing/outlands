using Server.Custom.Battlegrounds.Regions;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Battlegrounds.Gumps
{
    class SpectateGump : Gump
    {
        private PlayerMobile m_From;
        private enum Buttons
        {
            OK=99,
            Disabled=100,
            Spectate=101
        }

        public SpectateGump(Mobile from)
            : base(50, 50)
        {
            m_From = from as PlayerMobile;

            if (m_From.Region is BattlegroundRegion)
            {
                m_From.SendMessage("You cannot do that while in a battleground.");
                return;
            }

            AddPage(0);
            AddBackground(3, 0, 635, 473, 9270);
            AddLabel(235, 66, 53, @"Spectate Battlegrounds");
            AddLabel(115, 125, 53, @"Name");
            AddLabel(250, 125, 53, @"Active");
            AddLabel(330, 125, 53, @"Spectate");
            AddLabel(400, 125, 53, @"Players");
            AddLabel(470, 125, 53, @"Mode");

            int y = 165;
            for(int i = 0; i<Battleground.Instances.Count; i++)  
            {
                var bg = Battleground.Instances[i];
                if (bg.State != BattlegroundState.Active) continue;
                int buttonId = i + 1;

                AddLabel(115, y, 1153, bg.Name);
                AddImage(260, y + 5, 2361); // active image
                AddButton(335, y, 4014, 4015, buttonId, GumpButtonType.Reply, 0); // arrow button

                string gameMode = "Siege";

                if (bg is PickedTeamCTFBattleground)
                    gameMode = "cCTF";
                else if (bg is RandomTeamCTFBattleground)
                    gameMode = "rCTF";

                AddLabel(470, y, 1153, gameMode);

                AddLabel(400, y, 1153, string.Format("{0}/{1}", bg.PlayerCount, bg.MaximumPlayers));

                y += 35;
            }


            AddImage(0, 0, 9003);
            AddButton(547, 422, 247, 248, (int)Buttons.OK, GumpButtonType.Reply, 0); // OK button

        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_From.Region is BattlegroundRegion)
                return;

            switch (info.ButtonID)
            {
                case (int)Buttons.OK:
                case 0:
                    break;
                default:
                    Spectate(info.ButtonID - 1);
                    break;
            }
        }

        private void Spectate(int bgId)
        {
            if (bgId > Battleground.Instances.Count - 1)
            {
                m_From.SendMessage("Please try again.");
                m_From.SendGump(new SpectateGump(m_From));
                return;
            }

            var bg = Battleground.Instances[bgId];
            if (bg == null)
                return;

            bg.Spectate(m_From);
        }
    }
}
