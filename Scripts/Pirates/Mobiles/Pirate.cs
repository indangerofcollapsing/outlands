using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Multis;
using Server.Misc;
using Server.Network;
using Server.Mobiles;

namespace Server.Custom.Pirates
{
    public class Pirate : BaseCreature
    {
        public DateTime m_NextSpeechAllowed;
        public TimeSpan NextSpeechDelay = TimeSpan.FromSeconds(20);

        public DateTime m_NextDaggerAllowed;
        public TimeSpan NextDaggerDelay = TimeSpan.FromSeconds(Utility.RandomMinMax(10, 15));

        public string[] idleSpeech
        {
            get
            {
                return new string[] {       "Pillagin' and plunderin's the life...",
                                            "There's got to be some rum around here somewhere...",
                                            "*spins dagger*",
                                            "*spits*" 
                                            };
            }
        }

        public string[] combatSpeech
        {
            get
            {
                return new string[] {   "Filthy coward!",
                                        "I'll cut out yer heart!",
                                        "Scurvy dog!",
                                        "Another one for Davy' Jones!" 
                                        };
            }
        }

        [Constructable]
        public Pirate(): base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            SpeechHue = 0x22;
            Hue = Utility.RandomSkinHue();

            if (this.Female = Utility.RandomBool())
            {
                Title = "the pirate";
                Body = 0x191;
                Name = NameList.RandomName("female");
            }

            else
            {
                Title = "the pirate";
                Body = 0x190;
                Name = NameList.RandomName("male");
            }

            SetStr(75);
            SetDex(75);
            SetInt(25);

            SetHits(450);

            SetDamage(15, 25);
            
            SetSkill(SkillName.Fencing, 90);
            SetSkill(SkillName.Swords, 90);
            SetSkill(SkillName.Macing, 90);

            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Parry, 20);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 1000;
            Karma = -2000;

            Utility.AssignRandomHair(this, Utility.RandomHairHue());

            switch (Utility.RandomMinMax(0, 1))
            {
                case 0: AddItem(new LongPants(Utility.RandomDyedHue())); break;
                case 1: AddItem(new ShortPants(Utility.RandomDyedHue())); break;
            }

            switch (Utility.RandomMinMax(0, 3))
            {
                case 0: AddItem(new FancyShirt(Utility.RandomDyedHue())); break;
                case 1: AddItem(new Shirt(Utility.RandomDyedHue())); break;
                case 2: AddItem(new Doublet(Utility.RandomDyedHue())); break;
            }

            switch (Utility.RandomMinMax(0, 2))
            {
                case 0: AddItem(new Bandana(Utility.RandomDyedHue())); break;
                case 1: AddItem(new SkullCap(Utility.RandomDyedHue())); break;
            }

            switch (Utility.RandomMinMax(1, 6))
            {
                case 1: { AddItem(new Cutlass()); AddItem(new Buckler()); break; }
                case 2: { AddItem(new Scimitar()); AddItem(new Buckler()); break; }
                case 3: { AddItem(new Club()); AddItem(new Buckler()); break; }
                case 4: { AddItem(new Kryss()); AddItem(new Buckler()); break; }
                case 5: { AddItem(new WarFork()); AddItem(new Buckler()); break; }
                case 6: { AddItem(new Pitchfork()); break; }
            }
        }

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 1.025;
        }

        public Pirate(Serial serial): base(serial)
        {
        }

        public override bool AlwaysMurderer { get { return true; } }

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

            if (DateTime.UtcNow > m_NextDaggerAllowed && AIObject.currentCombatRange != CombatRange.Withdraw && AIObject.Action != ActionType.Flee)
            {
                Mobile combatant = this.Combatant;

                if (combatant != null)
                {
                    if (combatant.Alive && this.InLOS(combatant) && this.GetDistanceToSqrt(combatant) <= 8)
                    {
                        int minDamage = DamageMin;
                        int maxDamage = DamageMax;

                        AIObject.NextMove = DateTime.UtcNow + TimeSpan.FromSeconds(1.5);
                        NextCombatTime = NextCombatTime + TimeSpan.FromSeconds(3);

                        m_NextDaggerAllowed = DateTime.UtcNow + NextDaggerDelay;

                        Animate(31, 7, 1, true, false, 0);
                        Effects.PlaySound(Location, Map, this.GetAttackSound());

                        Timer.DelayCall(TimeSpan.FromSeconds(.475), delegate
                        {
                            if (this == null) return;
                            if (!this.Alive || this.Deleted) return;
                            if (this.Combatant == null) return;
                            if (!this.Combatant.Alive || this.Combatant.Deleted) return;

                            this.MovingEffect(combatant, 3921, 18, 1, false, false);

                            double distance = this.GetDistanceToSqrt(combatant.Location);
                            double destinationDelay = (double)distance * .08;

                            Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                            {
                                if (this == null) return;
                                if (!this.Alive || this.Deleted) return;
                                if (this.Combatant == null) return;
                                if (!this.Combatant.Alive || this.Combatant.Deleted) return;

                                if (Utility.RandomDouble() < .5)
                                {
                                    Effects.PlaySound(Location, Map, 0x224);

                                    int damage = maxDamage;

                                    if (damage < 1)
                                        damage = 1;

                                    this.DoHarmful(combatant);

                                    AOS.Damage(combatant, this, damage, 100, 0, 0, 0, 0);
                                    new Blood().MoveToWorld(combatant.Location, combatant.Map);
                                }

                                else
                                    Effects.PlaySound(Location, Map, 0x238);
                            });
                        });
                    }
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
