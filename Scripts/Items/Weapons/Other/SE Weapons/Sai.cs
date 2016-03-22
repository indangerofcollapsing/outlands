using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x27AF, 0x27FA )]
	public class Sai : BaseKnife
	{
		public override int BaseHitSound{ get{ return 0x23C; } }
		public override int BaseMissSound{ get{ return 0x232; } }

		public override SkillName BaseSkill{ get{ return SkillName.Fencing; } }
		public override WeaponType BaseType{ get{ return WeaponType.Piercing; } }
		public override WeaponAnimation BaseAnimation{ get{ return WeaponAnimation.Pierce1H; } }

		[Constructable]
		public Sai() : base( 0x27AF )
		{
			Weight = 7.0;
			Layer = Layer.TwoHanded;
		}

		public Sai( Serial serial ) : base( serial )
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