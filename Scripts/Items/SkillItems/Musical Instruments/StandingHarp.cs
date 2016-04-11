using System;

namespace Server.Items
{
	public class StandingHarp : BaseInstrument
	{
        public override int IconItemId { get { return 3761; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -21; } }
        public override int IconOffsetY { get { return -18; } }

		[Constructable]
		public StandingHarp() : base( 3761, 0x45, 0x46 )
		{
            Name = "standing harp";
			Weight = 10.0;
		}

		public StandingHarp( Serial serial ) : base( serial )
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