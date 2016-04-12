using System;
using Server.Items;
using Server.Targeting;
using System.Collections;

namespace Server.Mobiles
{
	[CorpseName( "a terathan warrior corpse" )]
	public class TerathanWarrior : BaseCreature
	{
		[Constructable]
		public TerathanWarrior() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a terathan warrior";
			Body = 70;
			BaseSoundID = 589;

            SetStr(75);
            SetDex(50);
            SetInt(25);

            SetHits(200);

            SetDamage(8, 16);

            SetSkill(SkillName.Wrestling, 75);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            SetSkill(SkillName.Poisoning, 10);

            VirtualArmor = 25;

			Fame = 4000;
			Karma = -4000;
		}

        public override Poison HitPoison { get { return Poison.Regular; } }
        public override int PoisonResistance { get { return 2; } }   

		public TerathanWarrior( Serial serial ) : base( serial )
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
