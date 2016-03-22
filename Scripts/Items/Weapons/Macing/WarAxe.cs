using System;
using Server.Items;
using Server.Network;
using Server.Engines.Harvest;
using Server.Mobiles;

namespace Server.Items
{
	[FlipableAttribute( 0x13B0, 0x13AF )]
	public class WarAxe : BaseAxe
	{
        public override int BaseMinDamage { get { return 12; } }
        public override int BaseMaxDamage { get { return 18; } }
        public override int BaseSpeed { get { return 54; } }

        public override int InitMinHits { get { return 40; } }
        public override int InitMaxHits { get { return 60; } }

        public override int IconItemId { get { return 5039; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -3; } }
        public override int IconOffsetY { get { return -3; } }

		public override SkillName BaseSkill{ get{ return SkillName.Macing; } }
		public override WeaponType BaseType{ get{ return WeaponType.Bashing; } }
		public override WeaponAnimation BaseAnimation { get{ return WeaponAnimation.Bash1H; } }

		public override HarvestSystem HarvestSystem{ get{ return null; } }

		[Constructable]
		public WarAxe() : base( 0x13B0 )
		{
            Name = "war axe";
			Weight = 4.0;
            Layer = Layer.OneHanded;
		}

		public WarAxe( Serial serial ) : base( serial )
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