using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	public class rAlligator : Alligator
	{
		[Constructable]
		public rAlligator() : base()
		{
			Name = "an alligator (rare)";
			Hue = 2101;
		}

		public rAlligator(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}
