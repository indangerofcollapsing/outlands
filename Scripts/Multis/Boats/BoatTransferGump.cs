using System;
using Server;
using Server.Items;
using Server.Multis;
using Server.Multis.Deeds;
using Server.Network;

namespace Server.Gumps
{
	public class BoatTransferGump : Gump
	{
		private Mobile m_From, m_To;
		private BaseBoat m_Boat;

        public BoatTransferGump(Mobile from, Mobile to, BaseBoat boat): base(110, 100)
		{   
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{           
		}
	}
}