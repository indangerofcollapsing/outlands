using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x13F8, 0x13F9 )]
	public class GnarledStaff : BaseStaff
	{
        public override int BaseMinDamage { get { return 18; } }
        public override int BaseMaxDamage { get { return 36; } }
        public override int BaseSpeed { get { return 40; } }

        public override int InitMinHits { get { return 40; } }
        public override int InitMaxHits { get { return 60; } }

        public override int IconItemId { get { return 5113; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -13; } }
        public override int IconOffsetY { get { return -13; } }

		[Constructable]
		public GnarledStaff() : base( 0x13F8 )
		{
            Name = "gnarled staff";
			Weight = 6.0;
		}

		public GnarledStaff( Serial serial ) : base( serial )
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