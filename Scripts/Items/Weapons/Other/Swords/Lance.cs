using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x26C0, 0x26CA )]
	public class Lance : BaseSword
	{		
		public override int BaseHitSound{ get{ return 0x23C; } }
		public override int BaseMissSound{ get{ return 0x238; } }

		public override SkillName BaseSkill{ get{ return SkillName.Fencing; } }
		public override WeaponType BaseType{ get{ return WeaponType.Piercing; } }
		public override WeaponAnimation BaseAnimation{ get{ return WeaponAnimation.Pierce1H; } }

		[Constructable]
		public Lance() : base( 0x26C0 )
		{
			Weight = 12.0;
		}

		public Lance( Serial serial ) : base( serial )
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