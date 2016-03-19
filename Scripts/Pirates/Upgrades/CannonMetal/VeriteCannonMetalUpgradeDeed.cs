using System;
using Server;
using Server.Regions;
using Server.Targeting;
using Server.Engines.CannedEvil;
using Server.Network;
using Server.Mobiles;
using Server.Multis;
using Server.Gumps;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
    public class VeriteCannonMetalUpgradeDeed : BaseBoatCannonMetalUpgradeDeed
    {
        public override string DisplayName { get { return "Verite"; } }
        public override int CannonHue { get { return 0x89F; } }  

        [Constructable]
        public VeriteCannonMetalUpgradeDeed(): base()
        {
            Name = "a ship cannon metal upgrade: Verite";
        }

        public VeriteCannonMetalUpgradeDeed(Serial serial): base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
    }
}