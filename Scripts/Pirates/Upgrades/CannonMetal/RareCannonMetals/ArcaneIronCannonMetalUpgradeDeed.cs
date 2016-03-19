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
    public class ArcaneIronCannonMetalUpgradeDeed : BaseBoatCannonMetalUpgradeDeed
    {
        public override int DoubloonCost { get { return 0; } }
        public override string DisplayName { get { return "Arcane Iron"; } }
        public override int CannonHue { get { return 2612; } }  

        [Constructable]
        public ArcaneIronCannonMetalUpgradeDeed(): base()
        {
            Name = "a ship cannon metal upgrade: Arcane Iron";
        }

        public ArcaneIronCannonMetalUpgradeDeed(Serial serial): base(serial)
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