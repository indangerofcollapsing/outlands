using System;
using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Custom
{
    public class FireDungeonConfirmExitGump : Gump
    {
        private Mobile m_From;

        public FireDungeonConfirmExitGump(Mobile from)
            : base(150, 200)
        {
            m_From = from;

            m_From.CloseGump(typeof(FireDungeonConfirmExitGump));

            AddPage(0);

            AddBackground(0, 0, 220, 190, 5054);
            AddBackground(10, 10, 200, 170, 3000);

            AddHtml(20, 20, 180, 100, "Do you wish to exit Fire Dungeon?", true, false);

            AddHtmlLocalized(55, 120, 140, 25, 1011011, false, false); // CONTINUE
            AddButton(20, 120, 4005, 4007, 2, GumpButtonType.Reply, 0);

            AddHtmlLocalized(55, 145, 140, 25, 1011012, false, false); // CANCEL
            AddButton(20, 145, 4005, 4007, 1, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info.ButtonID == 2)
            {
                if (m_From != null)
                    FireDungeon.OnExitRequest(m_From);
            }
        }
    }
}