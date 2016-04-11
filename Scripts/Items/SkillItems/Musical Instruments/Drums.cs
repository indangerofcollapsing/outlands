using System;

using Server.Mobiles;

namespace Server.Items
{
	public class Drums : BaseInstrument
	{
        public override int IconItemId { get { return 3740; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 6; } }
        public override int IconOffsetY { get { return -4; } }

		[Constructable]
		public Drums() : base( 0xE9C, 0x38, 0x39 )
		{
            Name = "drums";
			Weight = 3.0;
		}

		public Drums( Serial serial ) : base( serial )
		{
		}

		public override void PlayInstrumentWell(Mobile from)
		{
			base.PlayInstrumentWell(from);
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