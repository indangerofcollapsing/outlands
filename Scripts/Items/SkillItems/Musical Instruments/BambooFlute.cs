using System;

namespace Server.Items
{
	public class BambooFlute : BaseInstrument
	{
        public override int IconItemId { get { return 10245; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 6; } }
        public override int IconOffsetY { get { return -1; } }

		[Constructable]
		public BambooFlute() : base( 10245, 0x504, 0x503 )
		{
            Name = "bamboo flute";
			Weight = 2.0;
		}

		public BambooFlute( Serial serial ) : base( serial )
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