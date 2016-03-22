using System;
using Server.Network;
using Server.Targeting;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0xF52, 0xF51 )]
	public class UOACZDagger : BaseKnife
	{
        public override int BaseMinDamage { get { return 4; } }
        public override int BaseMaxDamage { get { return 8; } }
        public override int BaseSpeed { get { return 60; } }

        public override int InitMinHits { get { return 40; } }
        public override int InitMaxHits { get { return 60; } }

        public override int BaseHitSound { get { return 0x23C; } }
        public override int BaseMissSound { get { return 0x238; } }

        public override SkillName BaseSkill { get { return SkillName.Fencing; } }
        public override WeaponType BaseType { get { return WeaponType.Piercing; } }
        public override WeaponAnimation BaseAnimation { get { return WeaponAnimation.Pierce1H; } }

		[Constructable]
		public UOACZDagger() : base( 0xF52 )
		{
			Weight = 1.0;
		}

		public UOACZDagger( Serial serial ) : base( serial )
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