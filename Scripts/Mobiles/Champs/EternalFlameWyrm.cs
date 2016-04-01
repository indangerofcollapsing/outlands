using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Spells.Third;
using Server.Achievements;

namespace Server.Mobiles
{
    [CorpseName("eternal flame rythin's corpse")]
    public class EternalFlameWyrm : BaseCreature
    {
        public DateTime m_NextAIChangeAllowed;
        public TimeSpan NextAIChangeDelay = TimeSpan.FromSeconds(30);

        [Constructable]
        public EternalFlameWyrm(): base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Eternal Flame Rythin";
            Body = 46;
            Hue = 1357;
            BaseSoundID = 362;

            SetStr(100);
            SetDex(50);
            SetInt(100);

            SetHits(5000);
            SetStam(3000);
            SetMana(5000);

            SetDamage(30, 50);

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 115);

            SetSkill(SkillName.Magery, 125);
            SetSkill(SkillName.EvalInt, 125);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

            Fame = 25500;
            Karma = -22500;
        }        
        
        public override void SetUniqueAI()
        {
            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.FireBreathAttack] = 1;

            ResolveAcquireTargetDelay = 0.5;
            RangePerception = 24;
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
        }

        public override bool AlwaysMiniBoss { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }
        
        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

        public override int GetIdleSound(){return 0x2D3;}
        public override int GetHurtSound() {return 0x2D1;}

        public EternalFlameWyrm(Serial serial): base(serial)
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
