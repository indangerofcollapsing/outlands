using System;
using Server.Items;
using Server.Targeting;
using System.Collections;

namespace Server.Mobiles
{
	[CorpseName( "a drider warrior corpse" )]
	public class DriderWarrior : BaseCreature
	{
		[Constructable]
		public DriderWarrior() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a drider warrior";
            Body = 70;
            BaseSoundID = 589;
            Hue = 1102;

			SetStr( 75 );
			SetDex( 75 );
			SetInt( 50 );

			SetHits( 450 );
            SetMana( 1000 );

			SetDamage(15, 25);

            SetSkill(SkillName.Wrestling, 90);
			SetSkill(SkillName.Tactics, 100 );			

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Magery, 50);
            SetSkill(SkillName.EvalInt, 50);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.Poisoning, 15);

			Fame = 2000;
			Karma = -2000;

			VirtualArmor = 25;
		}

        public override Poison HitPoison { get { return Poison.Greater; } }
        public override Poison PoisonImmune { get { return Poison.Greater; } }

        public DriderWarrior(Serial serial) : base(serial)
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
