using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a minotaur corpse" )]
	public class MinotaurCaptain : BaseCreature
	{
        public DateTime m_NextSpeechAllowed;
        public TimeSpan NextSpeechDelay = TimeSpan.FromSeconds(20);

        public string[] idleSpeech
        {
            get
            {
                return new string[]
                {
                    "*snorts* Once the chief is freed, we strike!",
                    "*snorts* Those humans don't understand what they've done!"
                };
            }
        }

        public string[] combatSpeech
        {
            get
            {
                return new string[]
                {                                           
                    "",
                    "",
                    "",
                    "",
                    "",
                    "", 
                };
            }
        }

		[Constructable]
		public MinotaurCaptain() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 ) // NEED TO CHECK
		{
			Name = "a minotaur captain";
			Body = 280;

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(650);

            SetDamage(22, 32);

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 75);

            VirtualArmor = 25;

			Fame = 7000;
			Karma = -7000;
		}

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 1.1;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() <= .2)
            {
                PlaySound(GetAngerSound());

                SpecialAbilities.BleedSpecialAbility(1.0, this, defender, DamageMax, 8.0, -1, true, "", "The beast gores you with their horns, causing you to bleed!");
            }
        }

		public override int Meat { get { return 4; } }

        public override void OnThink()
        {
            base.OnThink();

            if (Utility.RandomDouble() < 0.01 && !Hidden && DateTime.UtcNow > m_NextSpeechAllowed)
            {
                if (Combatant == null)
                    Say(idleSpeech[Utility.Random(idleSpeech.Length - 1)]);
                else
                    Say(combatSpeech[Utility.Random(combatSpeech.Length - 1)]);

                m_NextSpeechAllowed = DateTime.UtcNow + NextSpeechDelay;
            }
        }

        public override int GetAngerSound() { return 0x597; }
        public override int GetIdleSound() { return 0x596; }
        public override int GetAttackSound() { return 0x599; }
        public override int GetHurtSound() { return 0x59a; }
        public override int GetDeathSound() { return 0x59c; }

		public MinotaurCaptain( Serial serial ) : base( serial )
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
