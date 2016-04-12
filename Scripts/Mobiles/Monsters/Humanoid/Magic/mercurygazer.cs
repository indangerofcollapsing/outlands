using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a mercury corpse" )]
	public class MercuryGazer : BaseCreature
	{
		[Constructable]
		public MercuryGazer () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a mercury gazer";
			Body = 22;
			BaseSoundID = 377;
			Hue = 0x835;

            SetStr(75);
            SetDex(50);
            SetInt(100);

            SetHits(5000);
            SetMana(5000);

            SetDamage(20, 30);

            SetSkill(SkillName.Wrestling, 85);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 125);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.Poisoning, 20);

            VirtualArmor = 75;

			Fame = 200500;
			Karma = -200500;
		}

        public override Poison HitPoison { get { return Poison.Deadly; } }
        public override int PoisonResistance { get { return 4; } }

        public override bool CanRummageCorpses { get { return true; } }		
		
		public MercuryGazer( Serial serial ) : base( serial )
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