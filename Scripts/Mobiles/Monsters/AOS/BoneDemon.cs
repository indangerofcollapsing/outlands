using System;
using Server;
using Server.Items;


namespace Server.Mobiles
{
	[CorpseName( "a bone demon corpse" )]
	public class BoneDemon : BaseCreature
	{
		[Constructable]
		public BoneDemon() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a bone demon";
			Body = 308;
			BaseSoundID = 0x48D;

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(2500);

            SetDamage(20, 35);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 75);

            VirtualArmor = 25;

			Fame = 20000;
			Karma = -20000;
		}

        public override Poison PoisonImmune { get { return Poison.Lethal; } }	     

		public BoneDemon( Serial serial ) : base( serial )
		{
		}

        public override void OnDeath( Container c )
        {     
          base.OnDeath( c );
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