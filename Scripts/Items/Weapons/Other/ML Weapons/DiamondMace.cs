using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x2D24, 0x2D30 )]
	public class DiamondMace : BaseBashing
	{
		[Constructable]
		public DiamondMace() : base( 0x2D24 )
		{
			Weight = 10.0;
		}

		public DiamondMace( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}