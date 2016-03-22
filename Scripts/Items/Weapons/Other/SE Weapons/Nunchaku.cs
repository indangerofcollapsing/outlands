using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x27AE, 0x27F9 )]
	public class Nunchaku : BaseBashing
	{
        public override int BaseHitSound{ get{ return 0x535; } }
		public override int BaseMissSound{ get{ return 0x239; } }

		[Constructable]
		public Nunchaku() : base( 0x27AE )
		{
			Weight = 5.0;
		}

		public Nunchaku( Serial serial ) : base( serial )
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