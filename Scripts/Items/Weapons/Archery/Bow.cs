using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x13B2, 0x13B1 )]
	public class Bow : BaseRanged
	{
		public override int EffectID{ get{ return 0xF42; } }
		public override Type AmmoType{ get{ return typeof( Arrow ); } }
		public override Item Ammo{ get{ return new Arrow(); } }

        public override int BaseMinDamage { get { return 13; } }
        public override int BaseMaxDamage { get { return 22; } }
        public override int BaseSpeed { get { return 46; } }
        public override int BaseMaxRange { get { return 10; } }

		public override int InitMinHits{ get{ return 40; } }
		public override int InitMaxHits{ get{ return 60; } }

        public override int IconItemId { get { return 5041; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -2; } }
        public override int IconOffsetY { get { return -3; } }

		public override WeaponAnimation BaseAnimation { get{ return WeaponAnimation.ShootBow; } }

		[Constructable]
		public Bow() : base( 0x13B2 )
		{
            Name = "bow";

			Weight = 5.0;
			Layer = Layer.TwoHanded;
		}

		public Bow( Serial serial ) : base( serial )
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