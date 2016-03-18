using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x13B2, 0x13B1 )]
	public class TrainingBow : BaseRanged
	{
        public override int EffectID { get { return 0xF42; } }
        public override Type AmmoType { get { return typeof(Arrow); } }
        public override Item Ammo { get { return new Arrow(); } }

        public override WeaponAbility PrimaryAbility { get { return WeaponAbility.ParalyzingBlow; } }
        public override WeaponAbility SecondaryAbility { get { return WeaponAbility.MortalStrike; } }

        public override int AosStrengthReq { get { return 30; } }
        public override int AosMinDamage { get { return Core.ML ? 15 : 16; } }
        public override int AosMaxDamage { get { return Core.ML ? 19 : 18; } }
        public override int AosSpeed { get { return 25; } }
        public override float MlSpeed { get { return 4.25f; } }

        public override int OldStrengthReq { get { return 20; } }
        public override int OldMinDamage { get { return 1; } }
        public override int OldMaxDamage { get { return 2; } }
        public override int OldSpeed { get { return 40; } }

        public override int DefMaxRange { get { return 10; } }

        public override int InitMinHits { get { return 31; } }
        public override int InitMaxHits { get { return 60; } }

        public override int IconItemId { get { return 5041; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -2; } }
        public override int IconOffsetY { get { return -3; } }

		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.ShootBow; } }
        
		[Constructable]
		public TrainingBow() : base( 0x13B2 )
		{
            Name = "a training bow";
			Weight = 6.0;
			Layer = Layer.TwoHanded;

            Hue = 2307;
		}

        public TrainingBow(Serial serial): base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}