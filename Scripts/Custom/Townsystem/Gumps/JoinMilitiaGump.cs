using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Custom.Townsystem;

namespace Server.Gumps
{
    public class ConfirmMilitiaGump : Gump
    {
        private Mobile m_From;
        Town m_Town;

        public ConfirmMilitiaGump(Mobile from, Town town)
            : base(150, 200)
        {
            m_From = from;
            m_Town = town;

            m_From.CloseGump(typeof(ConfirmMilitiaGump));

            AddPage(0);

            AddBackground(0, 0, 220, 190, 5054);
            AddBackground(10, 10, 200, 170, 3000);

            AddHtml(20, 20, 180, 100, "Do you wish to join the militia? Leaving will take three days upon speaking 'I wish to resign from the militia.'", true, false); // Do you wish to dry dock this boat?

            AddHtmlLocalized(55, 120, 140, 25, 1011011, false, false); // CONTINUE
            AddButton(20, 120, 4005, 4007, 2, GumpButtonType.Reply, 0);

            AddHtmlLocalized(55, 145, 140, 25, 1011012, false, false); // CANCEL
            AddButton(20, 145, 4005, 4007, 1, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info.ButtonID == 2)
            {
                if (m_From != null && m_Town != null)
					m_Town.HomeFaction.OnJoinAccepted(m_From);
            }
        }
    }
}