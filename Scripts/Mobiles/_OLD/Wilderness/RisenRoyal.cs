using System;
using System.Collections;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Sixth;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a risen royal's corpse" )]
	public class RisenRoyal : BaseCreature
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
		public RisenRoyal() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.4, 0.8 )
		{
			Name = "a risen royal";

            Hue = 2635;
			Body = 154;
			BaseSoundID = 471;

            SetStr(50);
            SetDex(50);
            SetInt(100);

            SetHits(1800);
            SetMana(2000);

            SetDamage(20, 30);

            SetSkill(SkillName.Wrestling, 110);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 125);

            SetSkill(SkillName.Magery, 125);
            SetSkill(SkillName.EvalInt, 125);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

			Fame = 4000;
			Karma = -4000;
		}

        public override int PoisonResistance { get { return 5; } }

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

        public RisenRoyal(Serial serial): base(serial)
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
