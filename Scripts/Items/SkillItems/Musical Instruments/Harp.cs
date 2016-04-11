using System;

using Server.Mobiles;

namespace Server.Items
{
	public class Harp : BaseInstrument
	{
        public override int IconItemId { get { return 3762; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -5; } }
        public override int IconOffsetY { get { return -8; } }

		[Constructable]
        public Harp(): base(3762, 0x43, 0x44)
		{
            Name = "harp";
			Weight = 3.0;
		}

		public Harp( Serial serial ) : base( serial )
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