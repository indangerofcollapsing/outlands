using System;
using Server.Items;
using Server.Network;
using Server.Spells;

namespace Server.Mobiles
{
    [CorpseName("a bot corpse")]
    public class DeDOSBot : BaseCreature
    {
        public bool isExploding = false;
        public int ExplosionHue = 2587;

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
                    "*beep*",
                };
            }
        }

        [Constructable]
        public DeDOSBot() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a bot";

            Body = 756;
            BaseSoundID = 456;

            Hue = 2525;
            SpeechHue = 0x3F;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(300);

            SetDamage(8, 16);

            SetSkill(SkillName.Wrestling, 60);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 150);

            VirtualArmor = 25;

            Fame = 500;
            Karma = -500;                     
        }

        public override void SetUniqueAI()
        {
            ResolveAcquireTargetDelay = 1;
            RangePerception = 18;
                                    
            DictCombatTargetingWeight[CombatTargetingWeight.LeastCombatants] = 100;

            DictCombatFlee[CombatFlee.Flee50] = 0;
            DictCombatFlee[CombatFlee.Flee25] = 0;
            DictCombatFlee[CombatFlee.Flee10] = 0;
            DictCombatFlee[CombatFlee.Flee5] = 0;            
        }

        public void CreateFlashText(Point3D location, Map map, string text, int textHue)
        {
            TimedStatic flash = new TimedStatic(0x37Be, 1.5);
            flash.Hue = 2950;
            flash.MoveToWorld(location, map);

            Timer.DelayCall(TimeSpan.FromSeconds(.25), delegate
            {
                if (flash != null && SpecialAbilities.Exists(this))
                    flash.PublicOverheadMessage(MessageType.Regular, textHue, false, text);
            });
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() <= .1 && !isExploding)
            {
                isExploding = true;

                CreateFlashText(Location, Map, "*terminates*", SpeechHue);

                Timer.DelayCall(TimeSpan.FromSeconds(Utility.RandomMinMax(1, 3)), delegate
                {
                    if (Deleted || !Alive) return;

                    SpecialAbilities.AnimalExplosion(null, Location, Map, null, Utility.RandomMinMax(1, 2), 10, 20, 5, ExplosionHue, true, true);

                    Kill();
                });
            }            
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (!willKill && amount >= 10)
            {
                if (Utility.RandomDouble() <= .1 && !isExploding)
                {
                    isExploding = true;

                    CreateFlashText(Location, Map, "*terminates*", SpeechHue);

                    Timer.DelayCall(TimeSpan.FromSeconds(Utility.RandomMinMax(1, 3)), delegate
                    {
                        if (Deleted || !Alive) return;

                        SpecialAbilities.AnimalExplosion(null, Location, Map, null, Utility.RandomMinMax(1, 2), 10, 20, 5, ExplosionHue, false, true);

                        Kill();
                    });
                }
            }
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

        public override bool AlwaysEventMinion { get { return true; } }
        public override bool AllowParagon { get { return false; } }

        public override int GetAngerSound() { return 0x219; }
        public override int GetIdleSound() { return 0x219; }
        public override int GetAttackSound() { return 0x21A; }
        public override int GetHurtSound() { return 0x387; }
        public override int GetDeathSound() { return 0x476; }

        public override bool OnBeforeDeath()
        {
            return base.OnBeforeDeath();
        }

        public DeDOSBot(Serial serial) : base(serial) { }        

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