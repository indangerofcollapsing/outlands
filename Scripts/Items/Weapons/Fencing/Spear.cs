using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0xF62, 0xF63 )]
	public class Spear : BaseSpear
	{
        public override int BaseMinDamage { get { return 18; } }
        public override int BaseMaxDamage { get { return 36; } }
        public override int BaseSpeed { get { return 35; } }

        public override int InitMinHits { get { return 40; } }
        public override int InitMaxHits { get { return 60; } }

        public override int IconItemId { get { return 3939; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -24; } }
        public override int IconOffsetY { get { return -12; } }

		[Constructable]
		public Spear() : base( 0xF62 )
		{
            Name = "spear";
			Weight = 7.0;
		}

		public Spear( Serial serial ) : base( serial )
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

            Name = "spear";
		}
	}
}