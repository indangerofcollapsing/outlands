using System;
using Server;

namespace Server.Items
{
	public class LesserMagicResistPotion : BaseMagicResistPotion
	{
        public override double MagicResist { get { return 25.0; } }
		public override TimeSpan Duration{ get{ return TimeSpan.FromMinutes( 2.0 ); } }

		[Constructable]
		public LesserMagicResistPotion() : base( PotionEffect.MagicResist )
		{
            Name = "Lesser Magic Resist potion";
		}

		public LesserMagicResistPotion( Serial serial ) : base( serial )
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