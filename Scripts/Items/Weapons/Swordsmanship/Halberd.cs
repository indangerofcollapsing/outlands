using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x143E, 0x143F )]
	public class Halberd : BasePoleArm
	{
        public override int BaseMinDamage { get { return 22; } }
        public override int BaseMaxDamage { get { return 44; } }
        public override int BaseSpeed { get { return 25; } }

        public override int InitMinHits { get { return 40; } }
        public override int InitMaxHits { get { return 60; } }

        public override int IconItemId { get { return 5182; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -31; } }
        public override int IconOffsetY { get { return -11; } }

		[Constructable]
		public Halberd() : base( 0x143E )
		{
            Name = "halberd";
			Weight = 10.0;
		}

		public Halberd( Serial serial ) : base( serial )
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