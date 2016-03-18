using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Custom.Townsystem;
using Server.Mobiles;

namespace Server.Gumps
{
    public class ConfirmExileGump : Gump
    {
        private Mobile m_ToExile;
        Town m_Town;

        public ConfirmExileGump(Mobile from, Town town, Mobile toExile)
            : base(150, 200)
        {
            m_ToExile = toExile;
            m_Town = town;

            from.CloseGump(typeof(ConfirmMilitiaGump));

            AddPage(0);

            AddBackground(0, 0, 220, 190, 5054);
            AddBackground(10, 10, 200, 170, 3000);

            AddHtml(20, 20, 180, 100, String.Format("Are you sure you wish to exile {0}? Kings may exile one player per day.", toExile.RawName), true, false); // Do you wish to dry dock this boat?

            AddHtmlLocalized(55, 120, 140, 25, 1011011, false, false); // CONTINUE
            AddButton(20, 120, 4005, 4007, 2, GumpButtonType.Reply, 0);

            AddHtmlLocalized(55, 145, 140, 25, 1011012, false, false); // CANCEL
            AddButton(20, 145, 4005, 4007, 1, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info.ButtonID == 2)
            {
                Mobile from = state.Mobile;
                PlayerMobile pltoExile = m_ToExile as PlayerMobile;
                if (pltoExile != null && from != null && m_Town != null)
                {
                    //The king can now exile anyone after the vote count is met.
                    //if (pltoExile.IsInMilitia && pltoExile.Citizenship != m_Town)
                    //    from.SendMessage("You may not exile other cities militia");
                    //else
                    m_Town.Exile(state.Mobile, m_ToExile);
                }
            }
        }
    }
}