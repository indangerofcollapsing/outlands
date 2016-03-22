using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute( 0x13FB, 0x13FA )]
	public class LargeBattleAxe : BaseAxe
	{
        public override int BaseMinDamage { get { return 17; } }
        public override int BaseMaxDamage { get { return 34; } }
        public override int BaseSpeed { get { return 42; } }

        public override int InitMinHits { get { return 40; } }
        public override int InitMaxHits { get { return 60; } }

        public override int IconItemId { get { return 5114; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -4; } }
        public override int IconOffsetY { get { return -7; } }

		[Constructable]
		public LargeBattleAxe() : base( 0x13FB )
		{
            Name = "large battle axe";
			Weight = 7.0;
		}

		public LargeBattleAxe( Serial serial ) : base( serial )
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