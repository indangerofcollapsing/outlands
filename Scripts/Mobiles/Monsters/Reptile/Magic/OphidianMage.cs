using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an ophidian mage corpse" )]
	public class OphidianMage : BaseCreature
	{
		[Constructable]
		public OphidianMage() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
            Name = "an ophidian mage";
			Body = 85;
			BaseSoundID = 639;

            SetStr(50);
            SetDex(50);
            SetInt(100);

            SetHits(200);
            SetMana(1000);

            SetDamage(5, 10);

            SetSkill(SkillName.Wrestling, 60);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 75);

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 75);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.Poisoning, 5);

            VirtualArmor = 25;
			Fame = 4000;
			Karma = -4000;
		}

        public override Poison HitPoison { get { return Poison.Greater; } }
        public override Poison PoisonImmune { get { return Poison.Greater; } }
        
		public OphidianMage( Serial serial ) : base( serial )
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
