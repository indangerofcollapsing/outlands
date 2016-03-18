using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a gaman corpse" )]
	public class Gaman : BaseCreature
	{
		[Constructable]
		public Gaman() : base( AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Name = "a gaman";
			Body = 248;

            SetStr(50);
            SetDex(25);
            SetInt(25);

            SetHits(300);

            SetDamage(10, 20);

            SetSkill(SkillName.Wrestling, 60);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;  

			Fame = 2000;
			Karma = -2000;
		}

        public override int Meat { get { return 10; } }
        public override int Hides { get { return 15; } }

		public override int GetAngerSound()
		{
			return 0x4F8;
		}

		public override int GetIdleSound()
		{
			return 0x4F7;
		}

		public override int GetAttackSound()
		{
			return 0x4F6;
		}

		public override int GetHurtSound()
		{
			return 0x4F9;
		}

		public override int GetDeathSound()
		{
			return 0x4F5;
		}

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );			
		}

		public Gaman( Serial serial ) : base( serial )
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
