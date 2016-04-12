using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a slimey corpse" )]
	public class AncientSlime : BaseCreature
	{
		[Constructable]
		public AncientSlime () : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a slime";
			Body = 51;
			BaseSoundID = 0;
            Hue = 0;

            SetStr(50);
            SetDex(25);
            SetInt(25);

            SetHits(400);

            SetDamage(10, 20);

            SetSkill(SkillName.Wrestling, 75);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 150);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.Poisoning, 20);

            VirtualArmor = 25;
		}

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 1.25;
        }

        public override int PoisonResistance { get { return 4; } }
        public override Poison HitPoison { get { return Poison.Deadly; } }

        public override bool OnBeforeDeath()
        {            
            return base.OnBeforeDeath();
        }

		public AncientSlime( Serial serial ) : base( serial )
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