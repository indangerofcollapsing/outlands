using Server.Gumps;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Battlegrounds.Gumps
{
    class JoinBattlegroundGump : Gump
    {
        private PlayerMobile m_From;
        private Battleground m_Battleground;

        private enum Buttons
        {
            Accept,
            Cancel
        }

        public JoinBattlegroundGump(Mobile from, Battleground bg)
            : base(50, 50)
        {
            m_From = from as PlayerMobile;
            m_Battleground = bg;
            AddPage(0);
            AddBackground(33, 14, 237, 98, 9200);
            AddButton(161, 73, 12018, 12019, (int)Buttons.Cancel, GumpButtonType.Reply, 0);
            AddLabel(56, 32, 32, string.Format("The {0} Awaits.", m_Battleground.Name));
            AddButton(69, 73, 12000, 12001, (int)Buttons.Accept, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(Network.NetState sender, RelayInfo info)
        {
            switch ((Buttons)info.ButtonID)
            {
                case Buttons.Accept:
                    m_Battleground.Join(m_From);
                    break;
                case Buttons.Cancel:
                    m_Battleground.Queue.Leave(m_From);
                    break;
            }
        }

    }
}
