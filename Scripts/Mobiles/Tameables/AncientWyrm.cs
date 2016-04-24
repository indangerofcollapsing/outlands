using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an ancient wyrm corpse")]
    public class AncientWyrm : BaseCreature
    {
        [Constructable]
        public AncientWyrm(): base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an ancient wyrm";
            Body = 46;
            BaseSoundID = 362;

            SetStr(100);
            SetDex(50);
            SetInt(100);

            SetHits(2000);
            SetMana(2000);

            SetDamage(25, 45);

            SetSkill(SkillName.Wrestling, 95);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 75);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 75;

            Fame = 25500;
            Karma = -22500;

            Tameable = true;
            ControlSlots = 5;
            MinTameSkill = 115.1;
        }

        public override int TamedItemId { get { return 17062; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 10; } }
        public override int TamedItemYOffset { get { return 0; } }

        public override int TamedBaseMaxHits { get { return 600; } }
        public override int TamedBaseMinDamage { get { return 38; } }
        public override int TamedBaseMaxDamage { get { return 40; } }
        public override double TamedBaseWrestling { get { return 100; } }
        public override double TamedBaseEvalInt { get { return 50; } }

        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 50; } }
        public override int TamedBaseInt { get { return 75; } }
        public override int TamedBaseMaxMana { get { return 1000; } }
        public override double TamedBaseMagicResist { get { return 100; } }
        public override double TamedBaseMagery { get { return 50; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 125; } }
        public override int TamedBaseVirtualArmor { get { return 100; } }

        public override void SetUniqueAI()
        {
            MassiveBreathRange = 6;
        }

        public override void SetTamedAI()
        {
            AISubGroup = AISubGroupType.MeleeMage2;
            UpdateAI(false);

            MassiveBreathRange = 6;
        }

        public override SlayerGroupType SlayerGroup { get { return SlayerGroupType.Beastial; } }
        public override SpeedGroupType BaseSpeedGroup { get { return SpeedGroupType.Medium; } }
        public override AIGroupType AIBaseGroup { get { return AIGroupType.EvilMonster; } }
        public override AISubGroupType AIBaseSubGroup { get { return AISubGroupType.MeleeMage3; } }
        public override double BaseUniqueDifficultyScalar { get { return 1.0; } }

        public DateTime m_NextMassiveBreathAllowed;
        public TimeSpan NextMassiveBreathDelay = TimeSpan.FromSeconds(20);

        public DateTime m_NextBreathAllowed;
        public TimeSpan NextBreathDelay = TimeSpan.FromSeconds(20);

        public DateTime m_NextAbilityAllowed;
        public TimeSpan NextAbilityDelay = TimeSpan.FromSeconds(10);

        public override void OnThink()
        {
            base.OnThink();

            if (Combatant != null && DateTime.UtcNow >= m_NextAbilityAllowed && !Frozen)
            {   
                if (DateTime.UtcNow >= m_NextMassiveBreathAllowed && AICombatEpicAction.CanDoMassiveFireBreathAttack(this))
                {
                    double totalDelay = 3;

                    SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1.0, totalDelay, true, 0, false, "", "", "-1");

                    Effects.PlaySound(Location, Map, GetAngerSound());

                    Direction direction = Utility.GetDirection(Location, Combatant.Location);

                    SpecialAbilities.DoMassiveBreathAttack(this, Location, direction, MassiveBreathRange, true, BreathType.Fire, true);

                    m_NextMassiveBreathAllowed = DateTime.UtcNow + NextMassiveBreathDelay + TimeSpan.FromSeconds(totalDelay);
                    m_NextAbilityAllowed = DateTime.UtcNow + NextAbilityDelay + TimeSpan.FromSeconds(totalDelay);

                    NextCombatTime = DateTime.UtcNow + TimeSpan.FromSeconds(totalDelay + 2);
                    NextSpellTime = DateTime.UtcNow + TimeSpan.FromSeconds(totalDelay + 2);

                    return;
                }
                
                if (DateTime.UtcNow >= m_NextBreathAllowed && AICombatSpecialAction.CanDoFireBreathAttack(this))
                {
                    AICombatSpecialAction.DoFireBreathAttack(this, Combatant);

                    m_NextBreathAllowed = DateTime.UtcNow + NextBreathDelay;
                    m_NextAbilityAllowed = DateTime.UtcNow + NextAbilityDelay;

                    NextCombatTime = DateTime.UtcNow + TimeSpan.FromSeconds(4);
                    NextSpellTime = DateTime.UtcNow + TimeSpan.FromSeconds(4);

                    return;
                }                
            }
        }
                
        public override int GetIdleSound(){ return 0x2D3; }
        public override int GetHurtSound(){ return 0x2D1; }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

        public AncientWyrm(Serial serial): base(serial)
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
