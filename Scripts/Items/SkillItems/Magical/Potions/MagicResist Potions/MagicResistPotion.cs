using System;
using Server;

namespace Server.Items
{
	public class MagicResistPotion : BaseMagicResistPotion
	{
        public override double MagicResist { get { return 50.0; } }
		public override TimeSpan Duration{ get{ return TimeSpan.FromMinutes( 2.0 ); } }

		[Constructable]
		public MagicResistPotion() : base( PotionEffect.MagicResist )
		{
            Name = "Magic Resist potion";
		}

		public MagicResistPotion( Serial serial ) : base( serial )
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