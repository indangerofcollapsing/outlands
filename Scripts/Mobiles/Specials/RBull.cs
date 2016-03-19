using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	public class rBull : Bull
	{
		[Constructable]
		public rBull() : base()
		{
			Name = "a bull (rare)";
			Hue = 2101;
		}

		public rBull(Serial serial) : base(serial)
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
