using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x27A8, 0x27F3 )]
	public class Bokuto : BaseSword
	{
		public override int BaseHitSound{ get{ return 0x536; } }
		public override int BaseMissSound{ get{ return 0x23A; } }

		[Constructable]
		public Bokuto() : base( 0x27A8 )
		{
			Weight = 7.0;
		}

		public Bokuto( Serial serial ) : base( serial )
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