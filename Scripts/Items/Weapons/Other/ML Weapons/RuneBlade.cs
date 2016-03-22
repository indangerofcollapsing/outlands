using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x2D32, 0x2D26 )]
	public class RuneBlade : BaseSword
	{		
		public override int BaseHitSound{ get{ return 0x23B; } }
		public override int BaseMissSound{ get{ return 0x239; } }

		[Constructable]
		public RuneBlade() : base( 0x2D32 )
		{
			Weight = 7.0;
			Layer = Layer.TwoHanded;
		}

		public RuneBlade( Serial serial ) : base( serial )
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