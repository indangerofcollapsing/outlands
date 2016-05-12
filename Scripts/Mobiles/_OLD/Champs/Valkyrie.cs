using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("hildr the valkyrie's corpse")]
    public class Valkyrie : BaseCreature
    {
        public DateTime m_NextArrowAllowed;
        public TimeSpan NextArrowDelay = TimeSpan.FromSeconds(2);

        public DateTime m_NextAIChangeAllowed;
        public TimeSpan NextAIChangeDelay = TimeSpan.FromSeconds(30);

        [Constructable]
        public Valkyrie(): base(AIType.AI_Generic, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            SpeechHue = 1326;
            Name = "Hildr the Valkyrie";

            Body = 728;

            SetStr(100);
            SetDex(75);
            SetInt(25);

            SetHits(5000);
            SetStam(5000);

            SetDamage(20, 30);

            AttackSpeed = 50;

            VirtualArmor = 25;

            SetSkill(SkillName.Archery, 100);
            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);           

            SetSkill(SkillName.MagicResist, 115);

            Fame = 1500;
            Karma = -1500;
        }        

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 2.48;

            DictCombatTargeting[CombatTargeting.Predator] = 1;
            
            DictCombatRange[CombatRange.WeaponAttackRange] = 0;
            DictCombatRange[CombatRange.SpellRange] = 8;
            DictCombatRange[CombatRange.Withdraw] = 1;

            ResolveAcquireTargetDelay = 0.5;
            RangePerception = 24;
        }

        public override bool AlwaysChamp { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override bool IsHighSeasBodyType { get { return true; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            m_NextArrowAllowed = DateTime.UtcNow + NextArrowDelay;
        }

        public override void OnThink()
        {
            base.OnThink();
           
            if (Utility.RandomDouble() < .01 && DateTime.UtcNow > m_NextAIChangeAllowed)
            {
                Effects.PlaySound(Location, Map, GetAngerSound());

                switch (Utility.RandomMinMax(1, 5))
                {
                    case 1:
                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                        break;

                    case 2:
                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                        break;

                    case 3:
                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                        break;

                    case 4:
                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                        break;

                    case 5:
                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 10;
                        break;
                }

                m_NextAIChangeAllowed = DateTime.UtcNow + NextAIChangeDelay;                
            }

            if (Utility.RandomDouble() < 1 && DateTime.UtcNow > m_NextArrowAllowed && AIObject.currentCombatRange != CombatRange.Withdraw && AIObject.Action != ActionType.Flee)
            {
                Mobile combatant = this.Combatant;

                if (combatant != null)
                {
                    if (combatant.Alive && this.InLOS(combatant) && this.GetDistanceToSqrt(combatant) <= 14)
                    {
                        int minDamage = DamageMin;
                        int maxDamage = DamageMax;
                        
                        //Backstab
                        if (Hidden)
                        {
                            minDamage = (int)((double)DamageMin * 1.5);
                            maxDamage = (int)((double)DamageMax * 1.5);

                            Timer.DelayCall(TimeSpan.FromSeconds(0.4), delegate
                            {
                                if (this == null) return;
                                if (!this.Alive || this.Deleted) return;

                                if (this.Hidden)
                                {
                                    Effects.PlaySound(Location, Map, 0x51D);
                                    RevealingAction();
                                }
                            });
                        }                                               

                        AIObject.NextMove = DateTime.UtcNow + TimeSpan.FromSeconds(1.5);
                        LastSwingTime = LastSwingTime + TimeSpan.FromSeconds(3);

                        m_NextArrowAllowed = DateTime.UtcNow + NextArrowDelay;

                        Animate(Utility.RandomList(4), 6, 1, true, false, 0);
                        Effects.PlaySound(Location, Map, this.GetAttackSound());
                        
                        Timer.DelayCall(TimeSpan.FromSeconds(.475), delegate
                        {
                            if (this == null) return;
                            if (!this.Alive || this.Deleted) return;
                            if (this.Combatant == null) return;
                            if (!this.Combatant.Alive || this.Combatant.Deleted) return;

                            this.MovingEffect(combatant, 0xF42, 18, 1, false, false);   
                        
                            double distance = this.GetDistanceToSqrt(combatant.Location);
                            double destinationDelay = (double)distance * .08;

                            Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                            {
                                if (this == null) return;
                                if (!this.Alive || this.Deleted) return;
                                if (this.Combatant == null) return;
                                if (!this.Combatant.Alive || this.Combatant.Deleted) return;

                                if (Utility.RandomDouble() < .80)
                                {
                                    Effects.PlaySound(Location, Map, 0x234);

                                    int damage = Utility.RandomMinMax(minDamage, maxDamage);

                                    if (damage < 1)
                                        damage = 1;

                                    this.DoHarmful(combatant);

                                    SpecialAbilities.EntangleSpecialAbility(0.25, this, combatant, 1.0, 5, -1, true, "", "Their arrow pins you in place!", "-1");

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

        public override int GetAngerSound() { return 0x370; }
        public override int GetIdleSound() { return 0x373; }
        public override int GetAttackSound() { return 0x612; }
        public override int GetHurtSound() { return 0x375; }
        public override int GetDeathSound() { return 0x376; }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

        public Valkyrie(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}