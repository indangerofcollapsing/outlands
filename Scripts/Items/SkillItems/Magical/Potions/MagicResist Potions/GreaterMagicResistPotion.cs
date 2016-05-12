using System;
using Server;

namespace Server.Items
{
	public class GreaterMagicResistPotion : BaseMagicResistPotion
	{
        public override double MagicResist { get { return 100.0; } }
		public override TimeSpan Duration{ get{ return TimeSpan.FromMinutes( 2.0 ); } }

		[Constructable]
		public GreaterMagicResistPotion() : base( PotionEffect.MagicResistance )
		{
            Name = "Greater Magic Resist potion";
		}

		public GreaterMagicResistPotion( Serial serial ) : base( serial )
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