using System;
using Server.Network;
using Server.Targeting;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0xF52, 0xF51 )]
	public class Dagger : BaseKnife
	{
        public override int BaseMinDamage { get { return 1; } }
        public override int BaseMaxDamage { get { return 2; } }
        public override int BaseSpeed { get { return 60; } }

        public override int InitMinHits { get { return 40; } }
        public override int InitMaxHits { get { return 60; } }

        public override bool TrainingWeapon { get { return true; } }

        public override int IconItemId { get { return 3922; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return 3; } }
        public override int IconOffsetY { get { return 1; } }

		public override SkillName BaseSkill{ get{ return SkillName.Fencing; } }
		public override WeaponType BaseType{ get{ return WeaponType.Piercing; } }
		public override WeaponAnimation BaseAnimation { get{ return WeaponAnimation.Pierce1H; } }

		[Constructable]
		public Dagger() : base( 0xF52 )
		{
			Weight = 1.0;
		}

		public Dagger( Serial serial ) : base( serial )
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