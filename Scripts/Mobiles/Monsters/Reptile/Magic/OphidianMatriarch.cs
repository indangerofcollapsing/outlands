using System;
using Server;
using Server.Items;
using Server.Custom.Ubercrafting;
namespace Server.Mobiles
{
	[CorpseName( "an ophidian corpse" )]
	public class OphidianMatriarch : BaseCreature
	{
		[Constructable]
		public OphidianMatriarch() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an ophidian matriarch";
			Body = 87;
			BaseSoundID = 644;

            SetStr(50);
            SetDex(50);
            SetInt(100);

            SetHits(750);
            SetMana(3000);

            SetDamage(20, 30);

            SetSkill(SkillName.Wrestling, 80);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 75);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.Poisoning, 20);            

			Fame = 16000;
			Karma = -16000;
		}

      public override int Meat { get { return 2; } }

        public override Poison HitPoison { get { return Poison.Deadly; } }
        public override Poison PoisonImmune { get { return Poison.Deadly; } }

		public OphidianMatriarch( Serial serial ) : base( serial )
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
