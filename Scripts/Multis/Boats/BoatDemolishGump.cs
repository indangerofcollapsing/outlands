using System;
using Server;
using Server.Items;
using Server.Multis;
using Server.Multis.Deeds;
using Server.Network;
using Server.Mobiles;

namespace Server.Gumps
{
	public class BoatDemolishGump : Gump
	{
		private Mobile m_Mobile;
        private BaseBoat m_Boat;

        public BoatDemolishGump(Mobile mobile, BaseBoat boat)
            : base(110, 100)
		{
			m_Mobile = mobile;
            m_Boat = boat;

            mobile.CloseGump(typeof(BoatDemolishGump));

			Closable = false;

			AddPage( 0 );

			AddBackground( 0, 0, 420, 280, 5054 );

			AddImageTiled( 10, 10, 400, 20, 2624 );
			AddAlphaRegion( 10, 10, 400, 20 );

			AddHtmlLocalized( 10, 10, 400, 20, 1060635, 30720, false, false ); // <CENTER>WARNING</CENTER>

			AddImageTiled( 10, 40, 400, 200, 2624 );
			AddAlphaRegion( 10, 40, 400, 200 );

            AddHtml(10, 40, 400, 200, "You are about to scuttle this ship, sinking it and all creatures and items onboard. Once initiated, this action cannot be reversed. Are you sure you wish to proceed?", true, false);
            //AddHtmlLocalized( 10, 40, 400, 200, 1061795, 32512, false, true );

			AddImageTiled( 10, 250, 400, 20, 2624 );
			AddAlphaRegion( 10, 250, 400, 20 );

			AddButton( 10, 250, 4005, 4007, 1, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 40, 250, 170, 20, 1011036, 32767, false, false ); // OKAY

			AddButton( 210, 250, 4005, 4007, 0, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 240, 250, 170, 20, 1011012, 32767, false, false ); // CANCEL
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
            if (info.ButtonID == 1 && !m_Boat.Deleted)
			{
                if (m_Boat.IsOwner(m_Mobile))
				{
                    m_Boat.BeginScuttle();
				}

				else
				{
                    m_Mobile.SendMessage("Only the owner this ship may scuttle it");
				}
			}
		}
	}
}