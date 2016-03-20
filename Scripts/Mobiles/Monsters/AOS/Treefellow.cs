using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a treefellow corpse" )]
	public class Treefellow : BaseCreature
	{
		[Constructable]
		public Treefellow() : base( AIType.AI_Melee, FightMode.Evil, 10, 1, 0.2, 0.4 )
		{
			Name = "a treefellow";
			Body = 301;

            SetStr(100);
            SetDex(25);
            SetInt(25);

            SetHits(350);

            SetDamage(15, 25);

            SetSkill(SkillName.Wrestling, 60);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 75;

			Fame = 500;
			Karma = 1500;
		}		

        public override int GetIdleSound() { return 443; }
        public override int GetDeathSound() { return 31; }
        public override int GetAttackSound() { return 672; }

		public Treefellow( Serial serial ) : base( serial )
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

			if ( BaseSoundID == 442 )
				BaseSoundID = -1;
		}
	}
}