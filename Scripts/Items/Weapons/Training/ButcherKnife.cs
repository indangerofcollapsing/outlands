using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x13F6, 0x13F7 )]
	public class ButcherKnife : BaseKnife
	{
        public override int BaseMinDamage { get { return 1; } }
        public override int BaseMaxDamage { get { return 2; } }
        public override int BaseSpeed { get { return 60; } }

        public override int InitMinHits { get { return 40; } }
        public override int InitMaxHits { get { return 60; } }

        public override bool TrainingWeapon { get { return true; } }

        public override int IconItemId { get { return 5111; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -10; } }
        public override int IconOffsetY { get { return 2; } }

		[Constructable]
		public ButcherKnife() : base( 0x13F6 )
		{
			Weight = 1.0;
		}

		public ButcherKnife( Serial serial ) : base( serial )
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