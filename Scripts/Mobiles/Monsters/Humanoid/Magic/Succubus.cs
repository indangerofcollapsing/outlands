using System;
using System.Collections;
using Server;
using Server.Items;


namespace Server.Mobiles
{
	[CorpseName( "a succubus corpse" )]
	public class Succubus : BaseCreature
	{
		[Constructable]
		public Succubus () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a succubus";
			Body = 149;
			BaseSoundID = 0x4B0;

            SetStr(75);
            SetDex(75);
            SetInt(100);

            SetHits(1000);
            SetMana(2000);

            SetDamage(20, 30);

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 125);

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 75);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

			Fame = 24000;
			Karma = -24000;
		}

        public override void SetUniqueAI()
        {           
            UniqueCreatureDifficultyScalar = 1.15;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            SpecialAbilities.HinderSpecialAbility(.1, this, defender, 1.0, 5.0, false, -1, true, "", "You are transfixed by their gaze!", "-1");           
        }

		public override bool OnBeforeDeath()
		{
			return base.OnBeforeDeath();
		}

		public Succubus( Serial serial ) : base( serial )
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
