using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x27Ab, 0x27F6 )]
	public class Tekagi : BaseKnife
	{
        public override int BaseHitSound{ get{ return 0x238; } }
		public override int BaseMissSound{ get{ return 0x232; } }

		public override SkillName BaseSkill{ get{ return SkillName.Fencing; } }
		public override WeaponType BaseType{ get{ return WeaponType.Piercing; } }
		public override WeaponAnimation BaseAnimation{ get{ return WeaponAnimation.Pierce1H; } }

		[Constructable]
		public Tekagi() : base( 0x27AB )
		{
			Weight = 5.0;
			Layer = Layer.TwoHanded;
		}

		public Tekagi( Serial serial ) : base( serial )
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