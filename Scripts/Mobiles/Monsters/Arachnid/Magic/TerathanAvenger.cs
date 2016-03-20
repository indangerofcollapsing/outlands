using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a terathan avenger corpse" )]
	public class TerathanAvenger : BaseCreature
	{
		[Constructable]
		public TerathanAvenger() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a terathan avenger";
			Body = 152;
			BaseSoundID = 0x24D;

            SetStr(75);
            SetDex(75);
            SetInt(50);

            SetHits(350);
            SetMana(1000);

            SetDamage(20, 30);

            SetSkill(SkillName.Wrestling, 95);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            SetSkill(SkillName.Magery, 50);
            SetSkill(SkillName.EvalInt, 50);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.Poisoning, 15);

            VirtualArmor = 25;

			Fame = 15000;
			Karma = -15000;
		}

        public override Poison HitPoison { get { return Poison.Greater; } }
        public override Poison PoisonImmune { get { return Poison.Greater; } }

		public TerathanAvenger( Serial serial ) : base( serial )
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
