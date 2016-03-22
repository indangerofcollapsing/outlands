using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x27A5, 0x27F0 )]
	public class Yumi : BaseRanged
	{
		public override int EffectID{ get{ return 0xF42; } }
		public override Type AmmoType{ get{ return typeof( Arrow ); } }
		public override Item Ammo{ get{ return new Arrow(); } }
        
		public override WeaponAnimation BaseAnimation{ get{ return WeaponAnimation.ShootBow; } }

		[Constructable]
		public Yumi() : base( 0x27A5 )
		{
			Weight = 9.0;
			Layer = Layer.TwoHanded;
		}

		public Yumi( Serial serial ) : base( serial )
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

			if ( Weight == 7.0 )
				Weight = 6.0;
		}
	}
}