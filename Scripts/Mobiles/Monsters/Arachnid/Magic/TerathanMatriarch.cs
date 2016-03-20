using System;
using Server;
using Server.Items;
namespace Server.Mobiles
{
	[CorpseName( "a terathan matriarch corpse" )]
	public class TerathanMatriarch : BaseCreature
	{
		[Constructable]
		public TerathanMatriarch() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a terathan matriarch";
			Body = 72;
			BaseSoundID = 599;

            SetStr(50);
            SetDex(75);
            SetInt(100);

            SetHits(600);
            SetMana(2000);

            SetDamage(20, 30);

            SetSkill(SkillName.Wrestling, 85);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.Poisoning, 15);

            VirtualArmor = 25;

			Fame = 10000;
			Karma = -10000;
		}

        public override Poison HitPoison { get { return Poison.Deadly; } }
        public override Poison PoisonImmune { get { return Poison.Deadly; } }    
        
		public TerathanMatriarch( Serial serial ) : base( serial )
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
