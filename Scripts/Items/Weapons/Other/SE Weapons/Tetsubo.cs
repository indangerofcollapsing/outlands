using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x27A6, 0x27F1 )]
	public class Tetsubo : BaseBashing
	{
		public override int BaseHitSound{ get{ return 0x233; } }
		public override int BaseMissSound{ get{ return 0x238; } }

		public override WeaponAnimation BaseAnimation{ get{ return WeaponAnimation.Bash2H; } }

		[Constructable]
		public Tetsubo() : base( 0x27A6 )
		{
			Weight = 8.0;
			Layer = Layer.TwoHanded;
		}

		public Tetsubo( Serial serial ) : base( serial )
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