using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	public class rDireWolf : DireWolf
	{
        [Constructable]
        public rDireWolf()
            : base()
        {
            Name = "a dire wolf (rare)";
            Hue = 2101;
        }

		public rDireWolf(Serial serial) : base(serial)
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
