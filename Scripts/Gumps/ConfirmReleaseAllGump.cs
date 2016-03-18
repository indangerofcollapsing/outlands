using System;
using Server;
using Server.Mobiles;
using Server.Regions;

namespace Server.Gumps
{
    public class ConfirmReleaseAllGump : Gump
    {
        private Mobile m_From;
        private BaseCreature m_Pet;

        public ConfirmReleaseAllGump(Mobile from)
            : base(50, 50)
        {
            m_From = from;

            m_From.CloseGump(typeof(ConfirmReleaseGump));

            if (m_From.Region is NewbieDungeonRegion)
            {
                m_From.SendMessage("You cannot release your pets here.");
                return;
            }

            AddPage(0);

            AddBackground(0, 0, 270, 120, 5054);
            AddBackground(10, 10, 250, 100, 3000);

            AddHtml(20, 15, 230, 60, "Are you sure you wish to release all your followers?", true, true);

            AddButton(20, 80, 4005, 4007, 2, GumpButtonType.Reply, 0);
            AddHtmlLocalized(55, 80, 75, 20, 1011011, false, false); // CONTINUE

            AddButton(135, 80, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(170, 80, 75, 20, 1011012, false, false); // CANCEL
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            if (m_From.Region is NewbieDungeonRegion)
            {
                m_From.SendMessage("You cannot release your pets here.");
                return;
            }
            
            if (info.ButtonID == 2)
            {
                PlayerMobile pm = m_From as PlayerMobile;

                if (pm != null)
                {
                    pm.ReleaseAllFollowers();
                }
            }
        }
    }
}