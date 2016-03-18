using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Items;

namespace Server.Multis
{
	public class ConfirmDryDockGump : Gump
	{
		private Mobile m_From;
		private BaseBoat m_Boat;

		public ConfirmDryDockGump( Mobile from, BaseBoat boat ) : base( 150, 200 )
		{
			m_From = from;
			m_Boat = boat;

			m_From.CloseGump( typeof( ConfirmDryDockGump ) );

			AddPage( 0 );

			AddBackground( 0, 0, 220, 170, 5054 );
			AddBackground( 10, 10, 200, 150, 3000 );

			AddHtmlLocalized( 20, 20, 180, 80, 1018319, true, false ); // Do you wish to dry dock this boat?

			AddHtmlLocalized( 55, 100, 140, 25, 1011011, false, false ); // CONTINUE
			AddButton( 20, 100, 4005, 4007, 2, GumpButtonType.Reply, 0 );

			AddHtmlLocalized( 55, 125, 140, 25, 1011012, false, false ); // CANCEL
			AddButton( 20, 125, 4005, 4007, 1, GumpButtonType.Reply, 0 );
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
            if (info.ButtonID == 2)
            {
                int doubloonsInHold = m_Boat.GetHoldDoubloonTotal(m_Boat);
                
                if (!Banker.CanDepositUniqueCurrency(m_From, typeof(Doubloon), doubloonsInHold))                
                    m_From.SendMessage("Your bankbox would not be able to hold all of the doubloons from your ship's hold. You must clear out some items from your bank before you may dock this ship.");
                    
                else
                {                
                    m_Boat.MoveHoldDoubloonsToBank(m_From, false);
                    m_Boat.EndDryDock(m_From, doubloonsInHold);
                }
            }
		}
	}
}