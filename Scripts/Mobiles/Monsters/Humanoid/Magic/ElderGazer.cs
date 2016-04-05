using System;
using Server;
using Server.Items;


namespace Server.Mobiles
{
	[CorpseName( "an elder gazer corpse" )]
	public class ElderGazer : BaseCreature
	{
		[Constructable]
		public ElderGazer () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an elder gazer";
			Body = 22;
			BaseSoundID = 377;            

            SetStr(50);
            SetDex(50);
            SetInt(100);

            SetHits(700);
            SetMana(2000);

            SetDamage(9, 18);

            SetSkill(SkillName.Wrestling, 85);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 125);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

			Fame = 12500;
			Karma = -12500;
		}

		public override void OnDeath( Container c )
		{			
    		base.OnDeath( c );
		}

		public ElderGazer( Serial serial ) : base( serial )
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
