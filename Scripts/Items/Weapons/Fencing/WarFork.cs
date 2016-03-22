using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x1405, 0x1404 )]
	public class WarFork : BaseSpear
	{
        public override int BaseMinDamage { get { return 13; } }
        public override int BaseMaxDamage { get { return 22; } }
        public override int BaseSpeed { get { return 52; } }

        public override int InitMinHits { get { return 40; } }
        public override int InitMaxHits { get { return 60; } }

		public override int BaseHitSound{ get{ return 0x236; } }
		public override int BaseMissSound{ get{ return 0x238; } }

        public override int IconItemId { get { return 5124; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -2; } }
        public override int IconOffsetY { get { return -7; } }

		public override WeaponAnimation BaseAnimation { get{ return WeaponAnimation.Pierce1H; } }

		[Constructable]
		public WarFork() : base( 0x1405 )
		{
            Name = "war fork";
			Weight = 3.0;
		}

		public WarFork( Serial serial ) : base( serial )
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

            Name = "war fork";
		}
	}
}