using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if Framework_4_0
using System.Linq;
using System.Threading.Tasks;
#endif
using Server;
using Server.Items;
using Server.Targeting;
using Server.Targets;
using Server.Network;
using Server.Regions;
using Server.ContextMenus;
using MoveImpl = Server.Movement.MovementImpl;
using Server.Custom;

namespace Server.Mobiles
{       
    public enum PetAbilityType
    {
        Offensive,
        Defensive,
        Opportunity
    }

    public enum PetBattleAbilityEffect
    {
        ImmediateAttack,
        PredeterminedHit,
        PredeterminedMiss,
        ReduceDefense,
        ReduceDamagePercent,
        Leech,
        Frozen,
        ReduceArmor,
        IncreaseSpeed,
        IncreaseAttack,
        IncreaseDamage,
        IncreaseDamagePercentByPercentMissingHits,
        CauseBleedPercent,
        TakeBleedDamage,
        CauseBurning,
        ReduceAttack,
        ReduceSpeedPercent,
        SuccessfulAttackIncreaseSpeed,
        SuccessfulAttackIncreaseAttack,
        SuccessfulAttackReduceAttack,
        SuccessfulAttackIncreaseDamage,
        NextHitDamageBoostPercent,
        SuccessfulAttackReduceOpponentSpeed,
        SuccessfulAttackReduceOpponentAttack,
        SuccessfulAttackReduceOpponentDefense,
        NextHitSpendDefensePowerForDamageBoost,
        ReduceResist,
        NextHitMagicDamage,
        NextHitMagicDamageIfBurning,
        NextHitLoseDefensivePowerIfBurning,
        NextHitLoseOffensivePowerIfBurning,
        Burning,
        Disease,
        NextHitDamageBoostPercentPerOpponentMissingArmor,
        IncreaseDefense,
        IncreaseResist,
        OnHitInflictMagicDamagePercentOfMaxHits,        
        ReduceDamage,
        IncreaseArmor,
        OnHitIncreaseDefense,
        OnHitIncreaseArmor,
        OnHitIncreaseDamage,
        OnHitChanceToIncreaseOffensivePower,
        OnHitOpponentReduceAttack,
        OnHitOpponentReduceDamage,
        OnHitInflictMagicDamagePercentOfDamage,
        OnDodgeInflictMagicDamagePercentOfDamage,
        ReduceSpeed,
        ReduceAttackPercent,
        GainExperience
    }

    public class PetBattleAbilities
    {
        public static void AddBloodEffect(BaseCreature bc_Creature, int min, int max)
        {
            new Blood().MoveToWorld(bc_Creature.Location, bc_Creature.Map);
            int extraBlood = Utility.RandomMinMax(min, max);
            for (int i = 0; i < extraBlood; i++)
            {
                new Blood().MoveToWorld(new Point3D(bc_Creature.X + Utility.RandomMinMax(-1, 1), bc_Creature.Y + Utility.RandomMinMax(-1, 1), bc_Creature.Z), bc_Creature.Map);
            }
        }

        public static int ScaleMeleeDamage(BaseCreature from, BaseCreature target, int damage)
        {
            int finalDamage = damage;

            if (from == null || target == null)
                return finalDamage;

            BaseCreature bc_Attacker = from as BaseCreature;
            BaseCreature bc_Defender = target as BaseCreature;

            int virtualArmor = target.VirtualArmor + target.VirtualArmorMod;

            bool petBattleAttack = false;

            if (bc_Attacker != null && bc_Defender != null)
            {
                if (bc_Attacker.PetBattleCreature && bc_Defender.PetBattleCreature)
                    petBattleAttack = true;
            }

            if (petBattleAttack)
            {
                double totalValue1; double totalValue2;

                //Defender Ability Armor Properties
                bc_Defender.PetBattleAbilityEntryLookupInProgress = true;

                foreach (PetBattleAbilityEffectEntry entry in bc_Defender.PetBattleAbilityEffectEntries)
                {
                    //IncreaseArmor
                    if (entry.m_PetBattleAbilityEffect == PetBattleAbilityEffect.IncreaseArmor)
                        virtualArmor += (int)entry.m_Value1;

                    //ReduceArmor
                    if (entry.m_PetBattleAbilityEffect == PetBattleAbilityEffect.ReduceArmor)
                        virtualArmor -= (int)entry.m_Value1;
                }

                bc_Defender.PetBattleAbilityEntryLookupInProgress = false;

                int baseDamageReductionPercent = (int)((double)virtualArmor / 5);
                int variation = (int)((double)baseDamageReductionPercent * .25);

                if (variation < 0)
                    variation = 0;

                int minDamageReduction = baseDamageReductionPercent - variation;
                int maxDamageReduction = baseDamageReductionPercent + variation;

                double percentReduction = ((double)(Utility.RandomMinMax(minDamageReduction, maxDamageReduction))) / 100;

                damage = (int)((double)damage * (1 - percentReduction));

                if (damage < 1)
                    damage = 1;

                return damage;
            }

            if (finalDamage < 1)
                finalDamage = 1;

            return finalDamage;
        }
        
        public static int ScaleMagicDamage(BaseCreature from, BaseCreature target, int damage)
        {
            int finalDamage = damage;

            int resistValue = (int)target.Skills[SkillName.MagicResist].Value;

            if (from != null && target != null)
            {
                if (from.PetBattleCreature && target.PetBattleCreature)
                {
                    double totalValue1; double totalValue2;

                    target.GetPetBattleEntryValue(PetBattleAbilityEffect.IncreaseResist, out totalValue1, out totalValue2);
                    resistValue += (int)totalValue1;

                    target.GetPetBattleEntryValue(PetBattleAbilityEffect.ReduceResist, out totalValue1, out totalValue2);
                    resistValue -= (int)totalValue1;

                    if (resistValue < 0)
                        resistValue = 0;

                    int resistScalar = 1 - (resistValue / 400);

                    if (resistScalar > 1)
                        resistScalar = 1;

                    if (resistScalar < 0)
                        resistScalar = 0;

                    finalDamage *= resistScalar;
                }
            }

            if (finalDamage < 1)
                finalDamage = 1;

            return finalDamage;
        }
    }

    public class PetBattleAbilityEffectEntry
    {
        public PetBattleAbilityEffect m_PetBattleAbilityEffect;
        public Mobile m_From;
        public double m_Value1;
        public double m_Value2;
        public DateTime m_Expiration;

        public PetBattleAbilityEffectEntry(PetBattleAbilityEffect petBattleAbilityEffect, Mobile from, double value1, double value2, DateTime expiration)
        {
            m_PetBattleAbilityEffect = petBattleAbilityEffect;
            m_From = from;
            m_Value1 = value1;
            m_Value2 = value2;
            m_Expiration = expiration;
        }
    }

    public abstract class PetBattleAbility
    {
        public BaseCreature m_Creature;
        
        public PetAbilityType m_Type = PetAbilityType.Offensive;
        public string m_Name = "";
        public string m_Description = "";
        public Action m_Action;

        public PetBattleAbility()
        {
        }

        public PetBattleAbility(BaseCreature m_Creature)
        {
            m_Creature = m_Creature;
        }

        public PetBattleAbility(BaseCreature creature, PetAbilityType type, string name, string description, Action action)
        {
            m_Creature = creature;
            
            m_Type = type;
            m_Name = name;
            m_Description = description;
            m_Action = action;
        }        
    }
}
