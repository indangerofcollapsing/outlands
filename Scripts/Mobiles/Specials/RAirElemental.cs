using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Achievements;

namespace Server.Mobiles
{
	public class RAirElemental : AirElemental
	{
		[Constructable]
		public RAirElemental () : base()
		{
			Name = "an air elemental (rare)";
			Hue = 2101;
		}

		public RAirElemental( Serial serial ) : base( serial )
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
