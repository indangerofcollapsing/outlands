using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x2D2F, 0x2D23 )]
	public class WarCleaver : BaseKnife
	{		
		public override int BaseHitSound{ get{ return 0x23B; } }
		public override int BaseMissSound{ get{ return 0x239; } }

		public override SkillName BaseSkill{ get{ return SkillName.Fencing; } }
		public override WeaponType BaseType{ get{ return WeaponType.Piercing; } }
		public override WeaponAnimation BaseAnimation{ get{ return WeaponAnimation.Pierce1H; } }

		[Constructable]
		public WarCleaver() : base( 0x2D2F )
		{
			Weight = 10.0;
		}

		public WarCleaver( Serial serial ) : base( serial )
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