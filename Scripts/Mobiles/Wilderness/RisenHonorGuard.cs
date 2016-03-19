using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a risen honor guard's corpse" )]
	public class RisenHonorGuard : BaseCreature
	{
        public DateTime m_NextSpeechAllowed;
        public TimeSpan NextSpeechDelay = TimeSpan.FromSeconds(20);

        public string[] idleSpeech
        {
            get
            {
                return new string[]
                {
                    "Who disturbs our slumber!",
                    "You are not welcome here!",
                    "This trespass is sacrilege!"
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
		public RisenHonorGuard() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a risen honor guard";
			Body = 57;
			Hue = 2635;
			BaseSoundID = 451;

            SetStr(75);
            SetDex(50);
            SetInt(25);

            SetHits(1250);

            SetDamage(30, 40);

            SetSkill(SkillName.Wrestling, 95);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 75;            

			Fame = 7000;
			Karma = -7000;
		}

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

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

        public RisenHonorGuard(Serial serial): base(serial)
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