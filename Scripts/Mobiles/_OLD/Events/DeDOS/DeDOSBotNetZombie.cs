
using Server.Items;
using Server.Spells.Fourth;
using System;
using System.Collections.Generic;
using System.Text;
using Server.Spells;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a botnet zombie corpse")]
    public class DeDOSBotNetZombie : BaseCreature
    {
        public DateTime m_NextSpeechAllowed;
        public TimeSpan NextSpeechDelay = TimeSpan.FromSeconds(20);        

        public string[] idleSpeech
        {
            get
            {
                return new string[]
                {
                    ""
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
                };
            }
        }

        [Constructable]
        public DeDOSBotNetZombie(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 1.0)
        {
            Name = "a botnet zombie";

            Body = 3;
            Hue = 2587;
            BaseSoundID = 471;

            SetStr(75);
            SetDex(25);
            SetInt(25);

            SetHits(300);

            SetDamage(8, 16);

            SetSkill(SkillName.Wrestling, 20);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 25;

            Fame = 3500;
            Karma = -3500;
        }

        public override bool AlwaysEventMinion { get { return true; } }
        public override bool AllowParagon { get { return false; } }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override void SetUniqueAI()
        {
            ResolveAcquireTargetDelay = 1;
            RangePerception = 18;

            UniqueCreatureDifficultyScalar = 1.5;

            DictCombatTargetingWeight[CombatTargetingWeight.CurrentCombatant] = 100;

            DictCombatFlee[CombatFlee.Flee50] = 0;
            DictCombatFlee[CombatFlee.Flee25] = 0;
            DictCombatFlee[CombatFlee.Flee10] = 0;
            DictCombatFlee[CombatFlee.Flee5] = 0;
        }        

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

        public override bool OnBeforeDeath()
        {
            Effects.PlaySound(Location, Map, 0x5C3);                    

            return base.OnBeforeDeath();
        }

        public override void OnDeath(Container c)
        {   
            base.OnDeath(c);            
        }    

        public DeDOSBotNetZombie(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }        
    }
}
