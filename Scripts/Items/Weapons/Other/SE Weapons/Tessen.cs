using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x27A3, 0x27EE )]
	public class Tessen : BaseBashing
	{
		public override int BaseHitSound{ get{ return 0x232; } }
		public override int BaseMissSound{ get{ return 0x238; } }

		public override WeaponAnimation BaseAnimation{ get{ return WeaponAnimation.Bash2H; } }

		[Constructable]
		public Tessen() : base( 0x27A3 )
		{
			Weight = 6.0;
			Layer = Layer.TwoHanded;
		}

		public Tessen( Serial serial ) : base( serial )
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