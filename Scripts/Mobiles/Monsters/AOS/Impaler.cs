using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an impaler corpse" )]
	public class Impaler : BaseCreature
	{
        [Constructable]
		public Impaler() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "impaler" );
			Body = 306;
			BaseSoundID = 0x2A7;
            Hue = 1196;

            SetStr(100);
            SetDex(75);
            SetInt(25);

            SetHits(1500);

            SetDamage(25, 35);

            SetSkill(SkillName.Wrestling, 95);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 25;

			Fame = 24000;
			Karma = -24000;
		}

        public override void SetUniqueAI()
        {
            if (Global_AllowAbilities)
                UniqueCreatureDifficultyScalar = 1.15;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < .20)
            {
                SpecialAbilities.BleedSpecialAbility(1.0, this, defender, DamageMax, 8.0, -1, true, "", "Their claws rip through your armor and causes you to bleed!");
                SpecialAbilities.PierceSpecialAbility(1.0, this, defender, .5, 10, -1, true, "", "");
            }
        }

		public Impaler( Serial serial ) : base( serial )
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

			if ( BaseSoundID == 1200 )
				BaseSoundID = 0x2A7;
		}
	}
}