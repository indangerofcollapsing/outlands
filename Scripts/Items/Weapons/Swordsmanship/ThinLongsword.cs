using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x13B8, 0x13B7 )]
	public class ThinLongsword : BaseSword
	{
        public override int BaseMinDamage { get { return 14; } }
        public override int BaseMaxDamage { get { return 24; } }
        public override int BaseSpeed { get { return 50; } }

        public override int InitMinHits { get { return 40; } }
        public override int InitMaxHits { get { return 60; } }

		public override int BaseHitSound{ get{ return 0x237; } }
		public override int BaseMissSound{ get{ return 0x23A; } }

        public override int IconItemId { get { return 5047; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -2; } }
        public override int IconOffsetY { get { return -4; } }

		[Constructable]
		public ThinLongsword() : base( 0x13B8 )
		{
            Name = "longsword";
			Weight = 4.0;
		}

		public ThinLongsword( Serial serial ) : base( serial )
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