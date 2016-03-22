using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x27A4, 0x27EF )]
	public class Wakizashi : BaseSword
	{
		public override int BaseHitSound{ get{ return 0x23B; } }
		public override int BaseMissSound{ get{ return 0x23A; } }

		[Constructable]
		public Wakizashi() : base( 0x27A4 )
		{
			Weight = 5.0;
			Layer = Layer.OneHanded;
		}

		public Wakizashi( Serial serial ) : base( serial )
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