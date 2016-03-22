using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0xF4D, 0xF4E )]
	public class Bardiche : BasePoleArm
	{
        public override int BaseMinDamage { get { return 22; } }
        public override int BaseMaxDamage { get { return 44; } }
        public override int BaseSpeed { get { return 25; } }

        public override int InitMinHits { get { return 40; } }
        public override int InitMaxHits { get { return 60; } }

        public override int IconItemId { get { return 3918; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -16; } }
        public override int IconOffsetY { get { return -9; } }

        public override int BaseHitSound { get { return 0x236; } }

		[Constructable]
		public Bardiche() : base( 0xF4D )
		{
            Name = "bardiche";
			Weight = 9.0;
		}

		public Bardiche( Serial serial ) : base( serial )
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