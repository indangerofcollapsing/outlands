using System;
using System.Collections;
using Server.Misc;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
    [CorpseName("a ratman rogue corpse")]
    public class RatmanArcher : BaseCreature
    {
        public DateTime m_NextArrowAllowed;
        public TimeSpan NextArrowDelay = TimeSpan.FromSeconds(3);

        public override InhumanSpeech SpeechType { get { return InhumanSpeech.Ratman; } }

        [Constructable]
        public RatmanArcher(): base(AIType.AI_Archer, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = NameList.RandomName("ratman");
            Body = 0x8E;
            BaseSoundID = 437;

            SetStr(75);
            SetDex(50);
            SetInt(25);

            SetHits(125);

            SetDamage(5, 10);

            SetSkill(SkillName.Wrestling, 45);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            Fame = 6500;
            Karma = -6500;
            PackItem(new Arrow(10));
        }

        public override void SetUniqueAI()
        {
            if (Global_AllowAbilities)
                UniqueCreatureDifficultyScalar = 2;
            
            DictCombatRange[CombatRange.WeaponAttackRange] = 0;
            DictCombatRange[CombatRange.SpellRange] = 8;
            DictCombatRange[CombatRange.Withdraw] = 1;
        }

        public override int AttackRange { get { return 10; } }

        public override void OnThink()
        {
            base.OnThink();
            
            if (Utility.RandomDouble() < 1 && DateTime.UtcNow > m_NextArrowAllowed && AIObject.currentCombatRange != CombatRange.Withdraw && AIObject.Action != ActionType.Flee)
            {
                Mobile combatant = this.Combatant;

                if (combatant != null)
                {
                    if (combatant.Alive && this.InLOS(combatant) && this.GetDistanceToSqrt(combatant) <= 12)
                    {
                        int minDamage = DamageMin;
                        int maxDamage = DamageMax;

                        AIObject.NextMove = DateTime.UtcNow + TimeSpan.FromSeconds(1.5);
                        NextCombatTime = NextCombatTime + TimeSpan.FromSeconds(3);

                        m_NextArrowAllowed = DateTime.UtcNow + NextArrowDelay;

                        Animate(Utility.RandomList(4), 6, 1, true, false, 0);
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

                                if (Utility.RandomDouble() < .33)
                                {
                                    Effects.PlaySound(Location, Map, 0x224);

                                    int damage = Utility.RandomMinMax(minDamage, maxDamage);

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

        public override bool CanRummageCorpses { get { return true; } }
        public override int Hides { get { return 8; } }
        public override HideType HideType { get { return HideType.Spined; } }
        public override int Meat { get { return 1; } }

        public RatmanArcher(Serial serial): base(serial)
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

            if (Body == 42)
            {
                Body = 0x8E;
                Hue = 0;
            }
        }
    }
}
