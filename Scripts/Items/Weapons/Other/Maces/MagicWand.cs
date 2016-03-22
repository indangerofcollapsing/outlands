using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	public class MagicWand : BaseBashing
	{
        public override int IconItemId { get { return 3570; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -5; } }
        public override int IconOffsetY { get { return -2; } }

		[Constructable]
		public MagicWand() : base( 0xDF2 )
		{
			Weight = 1.0;
		}

		public MagicWand( Serial serial ) : base( serial )
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