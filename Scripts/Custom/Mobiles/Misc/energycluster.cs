using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an energy cluster corpse" )]
	public class EnergyCluster : BaseCreature
	{
		[Constructable]
		public EnergyCluster() : base( AIType.AI_Mage, FightMode.Aggressor, 10, 1, 0.3, 0.6 )
		{
			Name = "an energy cluster";
			Body = 165;
			BaseSoundID = 466;
			Hue = 0x8A1;

			SetStr( 100 );
			SetDex( 75 );
			SetInt( 100 );

			SetHits(3000);
            SetMana(5000);

			SetDamage( 5, 10 );

            SetSkill(SkillName.Wrestling, 80);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 150);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);            
            SetSkill(SkillName.Meditation, 100);

			Fame = 100500;
			Karma = -100500;

			VirtualArmor = 25;

			AddItem( new LightSource() );
		}

		public EnergyCluster( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}