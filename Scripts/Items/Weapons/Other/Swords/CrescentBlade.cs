using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x26C1, 0x26CB )]
	public class CrescentBlade : BaseSword
	{
		public override int BaseHitSound{ get{ return 0x23B; } }
		public override int BaseMissSound{ get{ return 0x23A; } }

		[Constructable]
		public CrescentBlade() : base( 0x26C1 )
		{
			Weight = 1.0;
		}

		public CrescentBlade( Serial serial ) : base( serial )
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