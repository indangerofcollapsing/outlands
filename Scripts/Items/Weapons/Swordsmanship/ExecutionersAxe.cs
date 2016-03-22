using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute( 0xf45, 0xf46 )]
	public class ExecutionersAxe : BaseAxe
	{
        public override int BaseMinDamage { get { return 17; } }
        public override int BaseMaxDamage { get { return 34; } }
        public override int BaseSpeed { get { return 42; } }

        public override int InitMinHits { get { return 40; } }
        public override int InitMaxHits { get { return 60; } }

        public override int IconItemId { get { return 3910; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -10; } }
        public override int IconOffsetY { get { return -5; } }

		[Constructable]
		public ExecutionersAxe() : base( 0xF45 )
		{
            Name = "executioner's axe";
			Weight = 7.0;
		}

		public ExecutionersAxe( Serial serial ) : base( serial )
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