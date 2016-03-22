using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x13B6, 0x13B5 )]
	public class Scimitar : BaseSword
	{
        public override int BaseMinDamage { get { return 11; } }
        public override int BaseMaxDamage { get { return 17; } }
        public override int BaseSpeed { get { return 56; } }

        public override int InitMinHits { get { return 40; } }
        public override int InitMaxHits { get { return 60; } }

		public override int BaseHitSound{ get{ return 0x23B; } }
		public override int BaseMissSound{ get{ return 0x23A; } }

        public override int IconItemId { get { return 5045; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -9; } }
        public override int IconOffsetY { get { return -5; } }

		[Constructable]
		public Scimitar() : base( 0x13B6 )
		{
            Name = "scimitar";
			Weight = 4.0;
		}

		public Scimitar( Serial serial ) : base( serial )
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