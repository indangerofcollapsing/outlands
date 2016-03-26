using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x1401, 0x1400 )]
	public class Kryss : BaseSword
	{
        public override int BaseMinDamage { get { return 10; } }
        public override int BaseMaxDamage { get { return 16; } }
        public override int BaseSpeed { get { return 58; } }

        public override int InitMinHits { get { return 40; } }
        public override int InitMaxHits { get { return 60; } }

        public override int BaseHitSound { get { return 0x23C; } }
        public override int BaseMissSound { get { return 0x238; } }

		public override SkillName BaseSkill{ get{ return SkillName.Fencing; } }
		public override WeaponType BaseType{ get{ return WeaponType.Piercing; } }
		public override WeaponAnimation BaseAnimation { get{ return WeaponAnimation.Pierce1H; } }

        public override int IconItemId { get { return 5120; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -7; } }
        public override int IconOffsetY { get { return -4; } }

		[Constructable]
		public Kryss() : base( 0x1401 )
		{
            Name = "kryss";
			Weight = 2.0;
		}

		public Kryss( Serial serial ) : base( serial )
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

			if ( Weight == 1.0 )
				Weight = 2.0;

            Name = "kryss";
		}
	}
}