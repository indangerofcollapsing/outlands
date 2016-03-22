using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x2D35, 0x2D29 )]
	public class ElvenMachete : BaseSword
	{
		public override int BaseHitSound{ get{ return 0x23B; } }
		public override int BaseMissSound{ get{ return 0x239; } }

		[Constructable]
		public ElvenMachete() : base( 0x2D35 )
		{
			Weight = 6.0;
		}

		public ElvenMachete( Serial serial ) : base( serial )
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