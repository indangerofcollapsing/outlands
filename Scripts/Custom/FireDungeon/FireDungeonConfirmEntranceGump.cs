using System;
using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Custom
{
    public class FireDungeonConfirmEntranceGump : Gump
    {
        private Mobile m_From;

        public FireDungeonConfirmEntranceGump(Mobile from)
            : base(150, 200)
        {
            m_From = from;

            m_From.CloseGump(typeof(FireDungeonConfirmEntranceGump));

            AddPage(0);

            AddBackground(0, 0, 220, 190, 5054);
            AddBackground(10, 10, 200, 170, 3000);

            AddHtml(20, 20, 180, 100, "Do you wish to enter Fire Dungeon? You must be in a group of at least five, with all party members present.'", true, false);

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
                    FireDungeon.CheckEntry(m_From);
            }
        }
    }
}