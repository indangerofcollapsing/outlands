using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Seventh;
using Server.Spells.Sixth;

namespace Server.Mobiles
{
	public class rGargoyle : Gargoyle
	{
		[Constructable]
		public rGargoyle() : base()
		{
			Name = "a gargoyle (rare)";
			Hue = 2101;
		}

		public rGargoyle( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
