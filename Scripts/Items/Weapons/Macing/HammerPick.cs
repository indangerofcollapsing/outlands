using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x143D, 0x143C )]
	public class HammerPick : BaseBashing
	{
        public override int BaseMinDamage { get { return 12; } }
        public override int BaseMaxDamage { get { return 18; } }
        public override int BaseSpeed { get { return 54; } }

        public override int InitMinHits { get { return 40; } }
        public override int InitMaxHits { get { return 60; } }

        public override int IconItemId { get { return 5180; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -4; } }
        public override int IconOffsetY { get { return -3; } }

		[Constructable]
		public HammerPick() : base( 0x143D )
		{
            Name = "hammer pick";
			Weight = 4.0;
			Layer = Layer.OneHanded;
		}

		public HammerPick( Serial serial ) : base( serial )
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