using System;
using Server.Mobiles;


namespace Server.Items
{
	public class Tambourine : BaseInstrument
	{
        public override int IconItemId { get { return 3742; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 8; } }
        public override int IconOffsetY { get { return -2; } }

		[Constructable]
        public Tambourine(): base(0xE9E, 0x52, 0x53)
		{
            Name = "tambourine";
			Weight = 2.0;
		}

		public Tambourine( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void PlayInstrumentWell(Mobile from)
		{
			base.PlayInstrumentWell(from);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}