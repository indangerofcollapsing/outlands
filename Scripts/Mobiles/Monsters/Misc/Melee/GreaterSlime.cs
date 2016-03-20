using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a slimey corpse" )]
	public class GreaterSlime : BaseCreature
	{
		[Constructable]
		public GreaterSlime () : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an angry slime";
			Body = 51;
			BaseSoundID = 0;
            Hue = 472;

            SetStr(50);
            SetDex(25);
            SetInt(25);

            SetHits(200);

            SetDamage(7, 14);

            SetSkill(SkillName.Wrestling, 65);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 150);

            SetSkill(SkillName.Poisoning, 15);

            VirtualArmor = 25;

            Fame = 1000;
            Karma = -1000;
		}

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 1.25;
        }

        public override Poison PoisonImmune{ get{ return Poison.Regular; } }
        public override Poison HitPoison { get { return Poison.Regular; } }

        public override bool OnBeforeDeath()
        {
            return base.OnBeforeDeath();
        }

		public GreaterSlime( Serial serial ) : base( serial )
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