using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x27A2, 0x27ED )]
	public class NoDachi : BaseSword
	{
		public override int BaseHitSound{ get{ return 0x23B; } }
		public override int BaseMissSound{ get{ return 0x23A; } }

		[Constructable]
		public NoDachi() : base( 0x27A2 )
		{
			Weight = 10.0;
			Layer = Layer.TwoHanded;
		}

		public NoDachi( Serial serial ) : base( serial )
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