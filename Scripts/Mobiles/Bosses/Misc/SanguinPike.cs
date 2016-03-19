using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{	
	public class SanguinPike : Pike
	{
        public override int DefHitSound { get { return 0x236; } } //0x237, 0x235, //0x3BB
        public override int DefMissSound { get { return 0x238; } }

		[Constructable]
		public SanguinPike() : base()
		{
			Weight = 8.0;
		}

		public SanguinPike( Serial serial ) : base( serial )
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