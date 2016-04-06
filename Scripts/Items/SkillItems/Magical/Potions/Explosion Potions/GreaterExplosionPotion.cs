using System;
using Server;

namespace Server.Items
{
	public class GreaterExplosionPotion : BaseExplosionPotion
	{
		public override int MinDamage { get { return 15; } }
		public override int MaxDamage { get { return 23; } }

		[Constructable]
		public GreaterExplosionPotion() : base( PotionEffect.ExplosionGreater )
		{
            Name = "Greater Explosion potion";
		}

		public GreaterExplosionPotion( Serial serial ) : base( serial )
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