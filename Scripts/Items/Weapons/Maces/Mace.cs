using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0xF5C, 0xF5D )]
	public class Mace : BaseBashing
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ConcussionBlow; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.Disarm; } }

		public override int AosStrengthReq{ get{ return 45; } }
		public override int AosMinDamage{ get{ return 12; } }
		public override int AosMaxDamage{ get{ return 14; } }
		public override int AosSpeed{ get{ return 40; } }
		public override float MlSpeed{ get{ return 2.75f; } }

        public override int OldStrengthReq { get { return 30; } }
        public override int OldMinDamage { get { return 15; } }
        public override int OldMaxDamage { get { return 25; } }
        public override int OldSpeed { get { return 42; } }

		public override int InitMinHits{ get{ return 30; } }
		public override int InitMaxHits{ get{ return 75; } }

        public override int IconItemId { get { return 3933; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -2; } }
        public override int IconOffsetY { get { return -1; } }

		[Constructable]
		public Mace() : base( 0xF5C )
		{
            Name = "mace";
			Weight = 10.0;
		}

		public Mace( Serial serial ) : base( serial )
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

            Name = "mace";
		}
	}
}