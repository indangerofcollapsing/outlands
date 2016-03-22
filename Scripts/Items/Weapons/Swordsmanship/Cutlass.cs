using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x1441, 0x1440 )]
	public class Cutlass : BaseSword
	{
        public override int BaseMinDamage { get { return 11; } }
        public override int BaseMaxDamage { get { return 17; } }
        public override int BaseSpeed { get { return 56; } }

        public override int InitMinHits { get { return 40; } }
        public override int InitMaxHits { get { return 60; } }

		public override int BaseHitSound{ get{ return 0x23B; } }
		public override int BaseMissSound{ get{ return 0x23A; } }

        public override int IconItemId { get { return 5184; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -5; } }
        public override int IconOffsetY { get { return -0; } }

		[Constructable]
		public Cutlass() : base( 0x1441 )
		{
            Name = "cutlass";
			Weight = 4.0;
		}

		public Cutlass( Serial serial ) : base( serial )
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