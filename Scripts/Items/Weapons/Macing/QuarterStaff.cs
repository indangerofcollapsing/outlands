using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0xE89, 0xE8a )]
	public class QuarterStaff : BaseStaff
	{
        public override int BaseMinDamage { get { return 18; } }
        public override int BaseMaxDamage { get { return 36; } }
        public override int BaseSpeed { get { return 40; } }

        public override int InitMinHits { get { return 40; } }
        public override int InitMaxHits { get { return 60; } }

        public override int IconItemId { get { return 3722; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -13; } }
        public override int IconOffsetY { get { return -13; } }

		[Constructable]
		public QuarterStaff() : base( 0xE89 )
		{
            Name = "quarter staff";
			Weight = 6.0;
		}

		public QuarterStaff( Serial serial ) : base( serial )
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