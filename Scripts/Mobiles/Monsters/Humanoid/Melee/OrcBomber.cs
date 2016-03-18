using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Misc;

namespace Server.Mobiles
{
    [CorpseName("an orcish corpse")]
    public class OrcBomber : BaseCreature
    {
        public override InhumanSpeech SpeechType { get { return InhumanSpeech.Orc; } }

        public DateTime m_NextPotionThrowAllowed;
        public TimeSpan NextPotionThrowDelay = TimeSpan.FromSeconds(Utility.RandomMinMax(6, 8));

        [Constructable]
        public OrcBomber(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 182;

            Name = "an orc bomber";
            BaseSoundID = 0x45A;

            SetStr(75);
            SetDex(50);
            SetInt(25);

            SetHits(150);

            SetDamage(5, 10);

            SetSkill(SkillName.Wrestling, 55);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 2500;
            Karma = -2500;

            PackItem(new SulfurousAsh(Utility.RandomMinMax(6, 10)));
            PackItem(new MandrakeRoot(Utility.RandomMinMax(6, 10)));
            PackItem(new BlackPearl(Utility.RandomMinMax(6, 10)));
            PackItem(new MortarPestle());
            PackItem(new LesserExplosionPotion());
        }

        public override int Meat { get { return 1; } }

        public override void SetUniqueAI()
        {
            if (Global_AllowAbilities)
                UniqueCreatureDifficultyScalar = 2;
        }

        public override bool CanRummageCorpses { get { return true; } }

        public override void OnThink()
        {
            base.OnThink();

            if (Global_AllowAbilities)
            {
                if (Utility.RandomDouble() < 1 && DateTime.UtcNow > m_NextPotionThrowAllowed && AIObject.currentCombatRange != CombatRange.Withdraw && AIObject.Action != ActionType.Flee)
                {
                    Mobile combatant = this.Combatant;

                    if (combatant != null && !BardPacified)
                    {
                        if (combatant.Alive && this.InLOS(combatant) && this.GetDistanceToSqrt(combatant) <= 8)
                        {
                            int potionType = Utility.RandomMinMax(1, 5);

                            switch (potionType)
                            {
                                case 1: SpecialAbilities.ThrowPotionAbility(this, combatant, 1.5, 1.5, PotionAbilityEffectType.Explosion, 1, 10, 20, 1, 0, false, true); break;
                                case 2: SpecialAbilities.ThrowPotionAbility(this, combatant, 1.5, 1.5, PotionAbilityEffectType.Paralyze, 1, 5, 10, 1, 5, false, true); break;
                                case 3: SpecialAbilities.ThrowPotionAbility(this, combatant, 1.5, 1.5, PotionAbilityEffectType.Poison, 1, 5, 10, Utility.RandomMinMax(1, 2), 1, false, true); break;
                                case 4: SpecialAbilities.ThrowPotionAbility(this, combatant, 1.5, 1.5, PotionAbilityEffectType.Frost, 1, 5, 10, .2, 10, false, true); break;
                                case 5: SpecialAbilities.ThrowPotionAbility(this, combatant, 1.5, 1.5, PotionAbilityEffectType.Shrapnel, 2, 7, 15, 1, 0, true, true); break;
                            }

                            m_NextPotionThrowAllowed = DateTime.UtcNow + NextPotionThrowDelay;
                        }
                    }
                }
            }
        }

        public OrcBomber(Serial serial): base(serial)
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
