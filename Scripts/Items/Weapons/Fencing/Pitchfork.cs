using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0xE87, 0xE88 )]
	public class Pitchfork : BaseSpear
	{
        public override int BaseMinDamage { get { return 16; } }
        public override int BaseMaxDamage { get { return 32; } }
        public override int BaseSpeed { get { return 44; } }

        public override int InitMinHits { get { return 40; } }
        public override int InitMaxHits { get { return 60; } }

        public override int IconItemId { get { return 3720; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -28; } }
        public override int IconOffsetY { get { return -14; } }

		[Constructable]
		public Pitchfork() : base( 0xE87 )
		{
            Name = "pitchfork";
			Weight = 6.0;
		}

		public Pitchfork( Serial serial ) : base( serial )
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

			if ( Weight >= 10.0 )
				Weight = 10;

            Name = "pitchfork";
		}
	}
}