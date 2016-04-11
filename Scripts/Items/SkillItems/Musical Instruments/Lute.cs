using System;

using Server.Mobiles;

namespace Server.Items
{
	public class Lute : BaseInstrument
	{
        public override int IconItemId { get { return 3763; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 0; } }
        public override int IconOffsetY { get { return 0; } }

		[Constructable]
		public Lute() : base( 0xEB3, 0x4C, 0x4D )
		{
            Name = "lute";
			Weight = 3.0;
		}

        public override void PlayInstrumentWell(Mobile from)
        {
            base.PlayInstrumentWell(from);
        }

		public Lute( Serial serial ) : base( serial )
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