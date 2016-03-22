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

        public override int BaseMinDamage { get { return 1; } }
        public override int BaseMaxDamage { get { return 2; } }
        public override int BaseSpeed { get { return 40; } }

        public override int InitMinHits { get { return 40; } }
        public override int InitMaxHits { get { return 60; } }

        public override bool TrainingWeapon { get { return true; } }

        public override int IconItemId { get { return 3717; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -3; } }
        public override int IconOffsetY { get { return -5; } }

		public override WeaponAnimation BaseAnimation { get{ return WeaponAnimation.Slash1H; } }

        [Constructable]
        public Pickaxe(): base(0xE86)
        {
            Name = "pickaxe";

            Weight = 5.0;

            UsesRemaining = 30;
            ShowUsesRemaining = true;
        }

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
				switch (Utility.RandomMinMax(1, 4))
				{
					case 1: animation = WeaponAnimation.Bash1H; break;
					case 2: animation = WeaponAnimation.Bash2H; break;
					case 3: animation = WeaponAnimation.Slash1H; break;
					case 4: animation = WeaponAnimation.Slash2H; break;
				}
			}

			return animation;
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
		}
	}
}