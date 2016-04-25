using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Items;

namespace Server
{
    public class BoatGump : Gump
    {
        public BoatGump(Mobile from): base(10, 10)
        {    
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
        }
    }
}