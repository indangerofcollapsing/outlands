using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a poison lich corpse" )]
	public class MagusLich : BaseCreature
	{
		[Constructable]
		public MagusLich() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a poison lich";
			Body = 79;
			BaseSoundID = 412;
			Hue = 2207;

            SetStr(100);
            SetDex(50);
            SetInt(100);

            SetHits(2500);
            SetMana(5000);

            SetDamage(20, 35);

            SetSkill(SkillName.Wrestling, 85);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 150);

            SetSkill(SkillName.Magery, 150);
            SetSkill(SkillName.EvalInt, 150);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.Poisoning, 25);

            VirtualArmor = 25;

			Fame = 125000;
			Karma = -185000;
		}

        public override Poison HitPoison { get { return Poison.Deadly; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override bool CanRummageCorpses { get { return true; } }				

		public MagusLich( Serial serial ) : base( serial )
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