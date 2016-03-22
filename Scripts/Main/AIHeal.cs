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
using Server.Spells;
using Server.Spells.First;
using Server.Spells.Second;
using Server.Spells.Third;
using Server.Spells.Fourth;
using Server.Spells.Fifth;
using Server.Spells.Sixth;
using Server.Spells.Seventh;
using Server.Spells.Eighth;
using Server.Spells.Spellweaving;
using MoveImpl = Server.Movement.MovementImpl;
using Server.Custom;

namespace Server.Mobiles
{
    public static class AIHeal
    {
        public static bool DoSpellHeal(BaseCreature creature, Mobile target)
        {
            Spell spell = new GreaterHealSpell(creature, null);

            if (target != null)
            {
                creature.SpellTarget = target;
                spell.Cast();

                return true;
            }

            else
                return false;
        }

        public static bool DoSpellCure(BaseCreature creature, Mobile target)
        {
            Spell spell = new CureSpell(creature, null);

            if (target != null)
            {
                creature.SpellTarget = target;
                spell.Cast();
                return true;
            }

            else
                return false;
        }

        public static bool DoSpellHealOther(BaseCreature creature, int percent)
        {
            if (creature == null)
                return false;

            IPooledEnumerable eable = creature.Map.GetMobilesInRange(creature.Location, creature.RangePerception);

            Dictionary<Mobile, int> DictCreaturesToHeal = new Dictionary<Mobile, int>();

            foreach (Mobile target in eable)
            {
                if (!IsTargetValidHealTarget(creature, target, true))
                    continue;

                if (DoesTargetNeedHealing(target, percent))
                {
                    int creaturePercent = (int)(100 * (1 - ((double)target.Hits / (double)target.HitsMax)));
                    DictCreaturesToHeal.Add(target, creaturePercent);
                }
            }

            eable.Free();

            //Calculate Total Values
            int TotalValues = 0;

            foreach (KeyValuePair<Mobile, int> pair in DictCreaturesToHeal)
            {
                TotalValues += pair.Value;
            }

            double ActionCheck = Utility.RandomDouble();
            double CumulativeAmount = 0.0;
            double AdditionalAmount = 0.0;

            bool foundDirection = true;

            //Determine Healing Target                      
            foreach (KeyValuePair<Mobile, int> pair in DictCreaturesToHeal)
            {
                AdditionalAmount = (double)pair.Value / (double)TotalValues;

                if (ActionCheck >= CumulativeAmount && ActionCheck < (CumulativeAmount + AdditionalAmount))
                {
                    DoSpellHeal(creature, (Mobile)pair.Key);
                    return true;
                }

                CumulativeAmount += AdditionalAmount;
            }

            return false;
        }

        public static bool DoSpellCureOther(BaseCreature creature)
        {
            if (creature == null)
                return false;

            IPooledEnumerable eable = creature.Map.GetMobilesInRange(creature.Location, creature.RangePerception);

            Dictionary<Mobile, int> DictCreaturesToCure = new Dictionary<Mobile, int>();

            foreach (Mobile target in eable)
            {
                if (!IsTargetValidHealTarget(creature, target, true))
                    continue;

                if (target.Poison != null)
                    DictCreaturesToCure.Add(target, target.Poison.Level * 2);
            }

            eable.Free();

            //Calculate Total Values
            int TotalValues = 0;

            foreach (KeyValuePair<Mobile, int> pair in DictCreaturesToCure)
            {
                TotalValues += pair.Value;
            }

            double ActionCheck = Utility.RandomDouble();
            double CumulativeAmount = 0.0;
            double AdditionalAmount = 0.0;

            bool foundDirection = true;

            //Determine Healing Target                      
            foreach (KeyValuePair<Mobile, int> pair in DictCreaturesToCure)
            {
                AdditionalAmount = (double)pair.Value / (double)TotalValues;

                if (ActionCheck >= CumulativeAmount && ActionCheck < (CumulativeAmount + AdditionalAmount))
                {
                    DoSpellCure(creature, (Mobile)pair.Key);
                    return true;
                }

                CumulativeAmount += AdditionalAmount;
            }

            return false;
        }

        public static bool DoBandageHealOther(BaseCreature creature, int percent)
        {
            if (creature == null)
                return false;

            IPooledEnumerable eable = creature.Map.GetMobilesInRange(creature.Location, creature.RangePerception);

            Dictionary<Mobile, int> DictCreaturesToHeal = new Dictionary<Mobile, int>();

            foreach (Mobile target in eable)
            {
                if (!IsTargetValidHealTarget(creature, target, true))
                    continue;

                if (DoesTargetNeedHealing(target, percent))
                {
                    int creaturePercent = (int)(100 * (1 - ((double)target.Hits / (double)target.HitsMax)));
                    DictCreaturesToHeal.Add(target, creaturePercent);
                }
            }

            eable.Free();

            //Calculate Total Values
            int TotalValues = 0;

            foreach (KeyValuePair<Mobile, int> pair in DictCreaturesToHeal)
            {
                TotalValues += pair.Value;
            }

            double ActionCheck = Utility.RandomDouble();
            double CumulativeAmount = 0.0;
            double AdditionalAmount = 0.0;

            bool foundDirection = true;

            //Determine Healing Target                      
            foreach (KeyValuePair<Mobile, int> pair in DictCreaturesToHeal)
            {
                AdditionalAmount = (double)pair.Value / (double)TotalValues;

                if (ActionCheck >= CumulativeAmount && ActionCheck < (CumulativeAmount + AdditionalAmount))
                {
                    DoBandageHeal(creature, (Mobile)pair.Key);
                    return true;
                }

                CumulativeAmount += AdditionalAmount;
            }

            return false;
        }

        public static bool DoBandageCureOther(BaseCreature creature)
        {
            if (creature == null)
                return false;

            IPooledEnumerable eable = creature.Map.GetMobilesInRange(creature.Location, creature.RangePerception);

            Dictionary<Mobile, int> DictCreaturesToCure = new Dictionary<Mobile, int>();

            foreach (Mobile target in eable)
            {
                if (!IsTargetValidHealTarget(creature, target, true))
                    continue;

                if (target.Poison != null)
                    DictCreaturesToCure.Add(target, target.Poison.Level * 2);
            }

            eable.Free();

            //Calculate Total Values
            int TotalValues = 0;

            foreach (KeyValuePair<Mobile, int> pair in DictCreaturesToCure)
            {
                TotalValues += pair.Value;
            }

            double ActionCheck = Utility.RandomDouble();
            double CumulativeAmount = 0.0;
            double AdditionalAmount = 0.0;

            bool foundDirection = true;

            //Determine Healing Target                      
            foreach (KeyValuePair<Mobile, int> pair in DictCreaturesToCure)
            {
                AdditionalAmount = (double)pair.Value / (double)TotalValues;

                if (ActionCheck >= CumulativeAmount && ActionCheck < (CumulativeAmount + AdditionalAmount))
                {
                    DoBandageHeal(creature, (Mobile)pair.Key);
                    return true;
                }

                CumulativeAmount += AdditionalAmount;
            }

            return false;
        }

        public static bool DoBandageHeal(BaseCreature creature, Mobile target)
        {
            if (target == null || !target.Alive || target.Map != creature.Map || target.Deleted || target.IsDeadBondedPet || !creature.CanSee(target))
                return false;

            creature.DoingBandage = true;
            creature.HealTarget = target;

            //If Healing Self
            if (creature.HealTarget == creature)
                StartBandageHeal(creature);

            //Bandaging Someone Else: 
            else
            {
                creature.BandageOtherReady = true;

                //m_Mobile Will Run Towards Target, and When In Proximity Begin Heal and Timer
                creature.BandageTimeout = DateTime.UtcNow + TimeSpan.FromSeconds(creature.BandageTimeoutLength);
            }

            return true;
        }

        public static bool StartBandageHeal(BaseCreature creature)
        {
            if (creature == null)
                return false;

            if (creature.HealTarget == null)
                BandageFail(creature);

            creature.HealTarget.BeingBandaged = true;
            creature.BandageOtherReady = false;

            creature.Emote("*begins bandaging*");

            creature.AIObject.NextMove = DateTime.UtcNow + TimeSpan.FromSeconds(1.5);
            creature.NextCombatTime = DateTime.UtcNow + TimeSpan.FromSeconds(1.5);

            //Targeting Self
            if (creature == creature.HealTarget)
            {
                if (creature.Body.IsHuman)
                    creature.Animate(33, 5, 1, true, false, 0);

                else
                    creature.Animate(11, 5, 1, true, false, 0);

                Timer.DelayCall(TimeSpan.FromSeconds(creature.CreatureBandageSelfDuration), new TimerStateCallback(EndBandageHeal), new object[] { creature, creature });
            }

            //Targeting Other
            else
            {
                if (creature.Body.IsHuman)
                    creature.Animate(16, 7, 1, true, false, 0);

                else
                    creature.Animate(11, 5, 1, true, false, 0);

                Timer.DelayCall(TimeSpan.FromSeconds(creature.CreatureBandageOtherDuration), new TimerStateCallback(EndBandageHeal), new object[] { creature });
            }

            return true;
        }

        public static bool BandageFail(BaseCreature creature)
        {
            if (creature == null)
                return false;

            creature.DoingBandage = false;
            creature.BandageOtherReady = false;

            Mobile target = creature.HealTarget as Mobile;

            if (target != null)
                target.BeingBandaged = false;

            return true;
        }

        public static void EndBandageHeal(object state)
        {
            object[] states = (object[])state;

            BaseCreature creature = (BaseCreature)states[0];
            Mobile target = creature.HealTarget as Mobile;

            if (creature.Deleted)
                return;

            //If Heal Target Isn't Valid
            if (target == null || target.Deleted || !target.Alive || target.Map != creature.Map || target.Deleted || target.IsDeadBondedPet || !creature.CanSee(target))
            {
                creature.DoingBandage = false;
                target.BeingBandaged = false;

                creature.NextCombatHealActionAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(creature.CombatHealActionMinDelay, creature.CombatHealActionMaxDelay));

                return;
            }

            //If Target is More Than 2 Spaces Away: We Are Being Generous With Proximity
            else if (creature.GetDistanceToSqrt(target) > 2)
            {
                creature.DoingBandage = false;
                target.BeingBandaged = false;

                creature.NextCombatHealActionAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(creature.CombatHealActionMinDelay, creature.CombatHealActionMaxDelay));

                return;
            }

            int healingSkill = creature.Skills[SkillName.Healing].BaseFixedPoint;
            int maxHealingAmount = healingSkill / 10; //100 At Healing Skill 100

            double healingPercent = (double)healingSkill / 1000 / 4;

            double lowPercent = healingPercent * .90;
            double highPercent = healingPercent * 1.10;

            double healingAmount = (double)target.HitsMax * (lowPercent + ((highPercent - lowPercent) * Utility.RandomDouble()));

            //Bandage Cure
            double cureChance = .5 + ((creature.Skills[SkillName.Healing].Value / 2) / 100);

            if (target.Poisoned)
            {
                cureChance -= (5 * target.Poison.Level);

                if (Utility.RandomDouble() < cureChance)
                    target.CurePoison(creature);

                healingAmount /= 2;
            }

            int amountToHeal = (int)healingAmount;

            if (amountToHeal > maxHealingAmount)
                amountToHeal = maxHealingAmount;

            target.Heal(amountToHeal, creature);

            creature.PlaySound(0x57);

            creature.DoingBandage = false;
            target.BeingBandaged = false;

            creature.NextCombatHealActionAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(creature.CombatHealActionMinDelay, creature.CombatHealActionMaxDelay));
        }

        public static bool DoPotionHeal(BaseCreature creature)
        {
            creature.AIObject.NextMove = DateTime.UtcNow + TimeSpan.FromSeconds(1.5);
            creature.NextCombatTime = DateTime.UtcNow + TimeSpan.FromSeconds(1.5);

            if (creature.Body.IsHuman)
                creature.Animate(34, 5, 1, true, false, 0);

            else
                creature.Animate(11, 5, 1, true, false, 0);

            //Percent Healing Amounts
            double MinHealingAmount = .15;
            double MaxHealingAmount = .25;

            double amountHealed = (double)creature.HitsMax * (MinHealingAmount + ((MaxHealingAmount - MinHealingAmount) * Utility.RandomDouble()));

            if (amountHealed > 50)
                amountHealed = 50;

            creature.Heal((int)amountHealed);
            creature.PlaySound(0x031);

            return true;
        }

        public static bool DoPotionCure(BaseCreature creature)
        {
            if (creature == null)
                return false;

            if (creature.AIObject == null)
                return false;

            creature.AIObject.NextMove = DateTime.UtcNow + TimeSpan.FromSeconds(1.5);
            creature.NextCombatTime = DateTime.UtcNow + TimeSpan.FromSeconds(1.5);

            if (creature.Poisoned)
            {
                if (creature.Body.IsHuman)
                    creature.Animate(34, 5, 1, true, false, 0);

                else
                    creature.Animate(11, 5, 1, true, false, 0);

                double cureChance = Utility.RandomDouble();

                if (cureChance >= .10)
                    creature.CurePoison(creature);
            }

            creature.PlaySound(0x031);

            return true;
        }

        //--------------------------------------

        public static bool DoesTargetNeedHealing(Mobile target, int threshold)
        {
            double HealthPercentage = (double)target.Hits / (double)target.HitsMax;

            if (HealthPercentage < ((double)threshold / 100))
                return true;

            return false;
        }

        public static bool IsTargetValidHealTarget(BaseCreature creature, Mobile target, bool healOther)
        {  
            //Ignore If Deleted or Blessed
            if (target.Deleted || target.Blessed)
                return false;

            //Ignore If Dead
            if (!target.Alive || target.IsDeadBondedPet)
                return false;

            //Ignore ServerStaff
            if (target.AccessLevel > AccessLevel.Player)
                return false;

            //Target Self (Always Allowed)
            if (healOther && creature == target)
                return false;            

            //Ignore If Target out of Immediate Area
            if (!creature.CanSee(target))
                return false;

            //Ignore If Target out of LOS
            if (!creature.InLOS(target))
                return false;            

            //Controlled Creatures
            BaseCreature bc_Target = target as BaseCreature;

            if (bc_Target != null)
            {
                if (bc_Target.Controlled && bc_Target.ControlMaster != null)
                {
                    //Controller of Target is This Creature
                    if (bc_Target.ControlMaster == creature)
                        return true;

                    //Controller of Target is Teammate
                    //if (AITeamList.CheckTeam(creature, bc_Target.ControlMaster))
                        //return true;
                }
            }

            //if (!AITeamList.CheckTeam(creature, target))
                //return false;

            return true;
        }  
    }
}
