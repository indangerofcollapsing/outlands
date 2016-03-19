using System;
using Server.Items;
using Server.Targeting;
using System.Collections;
using Server.Achievements;

namespace Server.Mobiles
{
	public class rGiantSpider : GiantSpider
	{
		[Constructable]
		public rGiantSpider() : base()
		{
			Name = "a giant spider (rare)";
			Hue = 2101;
		}

		public rGiantSpider( Serial serial ) : base( serial )
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
