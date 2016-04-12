using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an ophidian archmage corpse" )]
	public class OphidianArchmage : BaseCreature
	{
		[Constructable]
		public OphidianArchmage() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
            Name = "an ophidian archmage";
			Body = 85;
			BaseSoundID = 639;
            Hue = 2008;

            SetStr(50);
            SetDex(50);
            SetInt(100);

            SetHits(350);
            SetMana(2000);

            SetDamage(7, 14);

            SetSkill(SkillName.Wrestling, 70);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 75);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.Poisoning, 10);

			Fame = 11500;
			Karma = -11500;
		}

        public override Poison HitPoison { get { return Poison.Deadly; } }
        public override int PoisonResistance { get { return 4; } }
        
		public OphidianArchmage( Serial serial ) : base( serial )
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
