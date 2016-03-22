using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x2D20, 0x2D2C )]
	public class ElvenSpellblade : BaseKnife
	{		
		public override int BaseMissSound{ get{ return 0x239; } }

		[Constructable]
		public ElvenSpellblade() : base( 0x2D20 )
		{
			Weight = 5.0;
			Layer = Layer.TwoHanded;
		}

		public ElvenSpellblade( Serial serial ) : base( serial )
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