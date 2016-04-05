using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;


namespace Server.Mobiles
{
	[CorpseName( "an elder poison elemental corpse" )]
	public class ElderPoisonElemental : BaseCreature
	{
        public DateTime m_NextAIChangeAllowed;
        public TimeSpan NextAIChangeDelay = TimeSpan.FromSeconds(30);

		[Constructable]
		public ElderPoisonElemental () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an elder poison elemental";
			Body = 162;
			BaseSoundID = 263;
            Hue = 1268;

			SetStr(75);
            SetDex(75);
            SetInt(100);

            SetHits(1250);
            SetMana(2000);

            SetDamage(20, 35);

            SetSkill(SkillName.Wrestling, 95);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.Poisoning, 75);

            VirtualArmor = 25;

			Fame = 12500;
			Karma = -12500;
		}

        public override void SetUniqueAI()
        {            
            UniqueCreatureDifficultyScalar = 0.9;
        }

        public override Poison HitPoison { get { return Poison.Lethal; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override void OnThink()
        {
            base.OnThink();           
        }

		public override void OnDeath( Container c )
		{			
			base.OnDeath( c );
		}

		public ElderPoisonElemental( Serial serial ) : base( serial )
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
