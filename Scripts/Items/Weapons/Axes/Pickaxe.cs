using System;
using Server.Items;
using Server.Network;
using Server.Engines.Harvest;

namespace Server.Items
{
	[FlipableAttribute( 0xE86, 0xE85 )]
	public class Pickaxe : BaseAxe, IUsesRemaining
	{
		public override HarvestSystem HarvestSystem{ get{ return Mining.System; } }

		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.DoubleStrike; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.Disarm; } }

		public override int AosStrengthReq{ get{ return 50; } }
		public override int AosMinDamage{ get{ return 13; } }
		public override int AosMaxDamage{ get{ return 15; } }
		public override int AosSpeed{ get{ return 35; } }
		public override float MlSpeed{ get{ return 3.00f; } }

        public override int OldStrengthReq { get { return 1; } }
        public override int OldMinDamage { get { return 1; } }
        public override int OldMaxDamage { get { return 2; } }
        public override int OldSpeed { get { return 50; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 60; } }

        public override int IconItemId { get { return 3717; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -3; } }
        public override int IconOffsetY { get { return -5; } }

		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.Slash1H; } }

		public override void OnSingleClick(Mobile from)
		{
			LabelTo(from, "Durability : {0}", this.UsesRemaining);
			LabelTo(from, "pick axe");
		}

		public override WeaponAnimation GetAnimation()
		{
			WeaponAnimation animation = WeaponAnimation.Slash1H;

			Mobile attacker = this.Parent as Mobile;

			if (attacker != null)
			{
				switch (Utility.Random(4))
				{
					case 0:
						animation = WeaponAnimation.Bash1H;
						break;

					case 1:
						animation = WeaponAnimation.Bash2H;
						break;

					case 2:
						animation = WeaponAnimation.Slash1H;
						break;

					case 3:
						animation = WeaponAnimation.Slash2H;
						break;
				}

			}

			return animation;
		}        

		[Constructable]
		public Pickaxe() : base( 0xE86 )
		{
            Name = "pickaxe";

			Weight = 11.0;
			UsesRemaining = 30;
			ShowUsesRemaining = true;
		}

		public Pickaxe( Serial serial ) : base( serial )
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
			ShowUsesRemaining = true;

            Name = "pickaxe";
		}
	}
}