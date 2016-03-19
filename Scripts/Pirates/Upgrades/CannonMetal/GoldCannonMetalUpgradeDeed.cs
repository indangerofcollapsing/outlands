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
    public class GoldCannonMetalUpgradeDeed : BaseBoatCannonMetalUpgradeDeed
    {
        public override string DisplayName { get { return "Gold"; } }
        public override int CannonHue { get { return 0x8A5; } }  

        [Constructable]
        public GoldCannonMetalUpgradeDeed(): base()
        {
            Name = "a ship cannon metal upgrade: Gold";
        }

        public GoldCannonMetalUpgradeDeed(Serial serial): base(serial)
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