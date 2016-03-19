using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	public class rGrizzlyBear : GrizzlyBear
	{
		[Constructable]
		public rGrizzlyBear() : base()
		{
			Name = "a grizzly bear (rare)";
			Hue = 2101;
		}

		public rGrizzlyBear( Serial serial ) : base( serial )
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}