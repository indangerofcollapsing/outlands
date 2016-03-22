using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x1439, 0x1438 )]
	public class WarHammer : BaseBashing
	{
        public override int BaseMinDamage { get { return 19; } }
        public override int BaseMaxDamage { get { return 38; } }
        public override int BaseSpeed { get { return 30; } }

        public override int InitMinHits { get { return 40; } }
        public override int InitMaxHits { get { return 60; } }

        public override int IconItemId { get { return 5176; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -3; } }
        public override int IconOffsetY { get { return 0; } }

		public override WeaponAnimation BaseAnimation { get{ return WeaponAnimation.Bash2H; } }

		[Constructable]
		public WarHammer() : base( 0x1439 )
		{
            Name = "war hammer";
			Weight = 7.0;
			Layer = Layer.TwoHanded;
		}

		public WarHammer( Serial serial ) : base( serial )
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