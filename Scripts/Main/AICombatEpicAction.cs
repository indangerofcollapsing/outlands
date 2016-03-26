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
using Server.Multis;
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
using MoveImpl = Server.Movement.MovementImpl;
using Server.Custom;

namespace Server.Mobiles
{
    public class AICombatEpicAction
    {
        public static bool CanDoCombatEpicAction(BaseCreature creature)
        {
            if (!SpecialAbilities.Global_AllowAbilities)
                return false;

            if (creature.DictCombatAction[CombatAction.CombatEpicAction] > 0)
            {
                if (DateTime.UtcNow > creature.NextCombatEpicActionAllowed)
                    return true;
            }

            return false;
        }

        public static bool DoCombatEpicAction(BaseCreature creature)
        {
            CombatEpicAction combatEpicAction = CombatEpicAction.None;

            int TotalValues = 0;

            Dictionary<CombatEpicAction, int> DictTemp = new Dictionary<CombatEpicAction, int>();

            DictTemp.Add(CombatEpicAction.None, creature.DictCombatEpicAction[CombatEpicAction.None]);

            if (AICombatEpicAction.CanDoMeleeBleedAoe(creature)) { DictTemp.Add(CombatEpicAction.MeleeBleedAoE, creature.DictCombatEpicAction[CombatEpicAction.MeleeBleedAoE]); }
            if (AICombatEpicAction.CanDoMassiveFireBreathAttack(creature)) { DictTemp.Add(CombatEpicAction.MassiveFireBreathAttack, creature.DictCombatEpicAction[CombatEpicAction.MassiveFireBreathAttack]); }
            if (AICombatEpicAction.CanDoMassiveIceBreathAttack(creature)) { DictTemp.Add(CombatEpicAction.MassiveIceBreathAttack, creature.DictCombatEpicAction[CombatEpicAction.MassiveIceBreathAttack]); }
            if (AICombatEpicAction.CanDoMassivePoisonBreathAttack(creature)) { DictTemp.Add(CombatEpicAction.MassivePoisonBreathAttack, creature.DictCombatEpicAction[CombatEpicAction.MassivePoisonBreathAttack]); }
            if (AICombatEpicAction.CanDoMassiveBoneBreathAttack(creature)) { DictTemp.Add(CombatEpicAction.MassiveBoneBreathAttack, creature.DictCombatEpicAction[CombatEpicAction.MassiveBoneBreathAttack]); }
            if (AICombatEpicAction.CanDoMassivePlantBreathAttack(creature)) { DictTemp.Add(CombatEpicAction.MassivePlantBreathAttack, creature.DictCombatEpicAction[CombatEpicAction.MassivePlantBreathAttack]); }

            //Calculate Total Values
            foreach (KeyValuePair<CombatEpicAction, int> pair in DictTemp)
            {
                TotalValues += pair.Value;
            }

            double ActionCheck = Utility.RandomDouble();
            double CumulativeAmount = 0.0;
            double AdditionalAmount = 0.0;

            //Determine CombatEpicAction                      
            foreach (KeyValuePair<CombatEpicAction, int> pair in creature.DictCombatEpicAction)
            {
                AdditionalAmount = (double)pair.Value / (double)TotalValues;

                if (ActionCheck >= CumulativeAmount && ActionCheck < (CumulativeAmount + AdditionalAmount))
                {
                    combatEpicAction = pair.Key;

                    switch (combatEpicAction)
                    {
                        case CombatEpicAction.MeleeBleedAoE: AICombatEpicAction.DoMeleeBleedAoE(creature); break;
                        case CombatEpicAction.MassiveFireBreathAttack: AICombatEpicAction.DoMassiveFireBreathAttack(creature); break;
                        case CombatEpicAction.MassiveIceBreathAttack: AICombatEpicAction.DoMassiveIceBreathAttack(creature); break;
                        case CombatEpicAction.MassivePoisonBreathAttack: AICombatEpicAction.DoMassivePoisonBreathAttack(creature); break;
                        case CombatEpicAction.MassiveBoneBreathAttack: AICombatEpicAction.DoMassiveBoneBreathAttack(creature); break;
                        case CombatEpicAction.MassivePlantBreathAttack: AICombatEpicAction.DoMassivePlantBreathAttack(creature); break;
                    }

                    creature.NextCombatSpecialActionAllowed = creature.NextCombatSpecialActionAllowed.AddSeconds(5);
                    creature.NextCombatEpicActionAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(creature.CombatEpicActionMinDelay, creature.CombatEpicActionMaxDelay));

                    if (creature.AcquireNewTargetEveryCombatAction)
                        creature.m_NextAcquireTargetAllowed = DateTime.UtcNow;

                    return true;
                }

                CumulativeAmount += AdditionalAmount;
            }

            return false;
        }

        //-------------
        
        public static bool CanDoMeleeBleedAoe(BaseCreature creature)
        {
            if (creature.Combatant == null)
                return false;

            if (creature.GetDistanceToSqrt(creature.Combatant) >= 2)
                return false;

            if (creature.Paralyzed || creature.Frozen || !creature.Alive)
                return false;

            return true;
        }
        
        public static bool DoMeleeBleedAoE(BaseCreature creature)
        {
            if (creature == null)
                return false;

            double swingCount = 12;
            double swingInterval = .3; 
            double preSwingDelay = .1;

            double hitChance = .25;
            
            double actionsCooldown = (swingCount * swingInterval) + (swingCount * preSwingDelay) + 1;

            if (creature.AIObject != null)
            {
                creature.AIObject.NextMove = DateTime.UtcNow + TimeSpan.FromSeconds(actionsCooldown);
                creature.NextCombatTime = creature.NextCombatTime + TimeSpan.FromSeconds(actionsCooldown);
                creature.NextSpellTime = creature.NextSpellTime + TimeSpan.FromSeconds(actionsCooldown);
                creature.NextCombatHealActionAllowed = creature.NextCombatHealActionAllowed + TimeSpan.FromSeconds(actionsCooldown);
                creature.NextCombatSpecialActionAllowed = creature.NextCombatSpecialActionAllowed + TimeSpan.FromSeconds(actionsCooldown);
                creature.NextCombatEpicActionAllowed = creature.NextCombatEpicActionAllowed + TimeSpan.FromSeconds(actionsCooldown);
            }

            for (int a = 0; a < swingCount; a++)
            {
                double delay = (a * swingInterval) + (a * preSwingDelay);

                Timer.DelayCall(TimeSpan.FromSeconds(delay), delegate
                {
                    if (creature == null)
                        return;

                    if (!creature.Alive || creature.Paralyzed || creature.Frozen)
                        return;

                    Effects.PlaySound(creature.Location, creature.Map, 0x51F);

                    int newDirectionValue = (int)creature.Direction;
                    newDirectionValue += Utility.RandomList(-3, -4, -5, 3, 4, 5);

                    if (newDirectionValue > 7)
                        newDirectionValue = 0 + (newDirectionValue - 8);
                    else if (newDirectionValue < 0)
                        newDirectionValue = 8 + newDirectionValue;
                    
                    creature.Direction = (Direction)(newDirectionValue);  

                    Timer.DelayCall(TimeSpan.FromSeconds(preSwingDelay), delegate
                    {
                        if (creature == null)
                            return;

                        if (!creature.Alive || creature.Paralyzed || creature.Frozen)
                            return;

                        BaseWeapon weapon = creature.Weapon as BaseWeapon;

                        if (weapon != null)
                        {
                            weapon.PlaySwingAnimation(creature);

                            IPooledEnumerable eable = creature.Map.GetObjectsInRange(creature.Location, 1);

                            foreach (object obj in eable)
                            {
                                if (obj is Mobile)
                                {
                                    Mobile mobile = obj as Mobile;

                                    if (creature == null || mobile == null)
                                        continue;

                                    if (!creature.Alive || !mobile.Alive)
                                        continue;

                                    if (creature == mobile)
                                        continue;

                                    if (creature.CanBeHarmful(mobile))
                                    {
                                        BaseCreature bc_CreatureTarget = mobile as BaseCreature;

                                        bool validCreatureTarget = false;

                                        if (bc_CreatureTarget != null)
                                        {
                                            if (bc_CreatureTarget.Controlled && bc_CreatureTarget.ControlMaster is PlayerMobile)
                                                validCreatureTarget = true;
                                        }

                                        //Valid Targets
                                        if (mobile == creature.Combatant || mobile is PlayerMobile || validCreatureTarget)
                                        {
                                            if (a == 0 || Utility.RandomDouble() < hitChance)
                                            {
                                                int damage = Utility.RandomMinMax(creature.DamageMin, creature.DamageMax);

                                                if (damage < 1)
                                                    damage = 1;

                                                creature.DoHarmful(mobile);
                                                
                                                BaseCreature bc_Mobile = mobile as BaseCreature;
                                                PlayerMobile pm_Mobile = mobile as PlayerMobile;

                                                double bleedValue = 0;

                                                if (bc_Mobile != null)
                                                {
                                                    bc_Mobile.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Bleed, out bleedValue);
                                                    
                                                    if (bleedValue == 0)
                                                        SpecialAbilities.BleedSpecialAbility(1.0, creature, mobile, creature.DamageMax, 8.0, -1, true, "", "");
                                                }

                                                if (pm_Mobile != null)
                                                {   
                                                    pm_Mobile.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Bleed, out bleedValue);

                                                    if (bleedValue == 0)
                                                        SpecialAbilities.BleedSpecialAbility(1.0, creature, mobile, creature.DamageMax, 8.0, -1, true, "", "Their attack causes you to bleed!");
                                                }                                                
                                                
                                                AOS.Damage(mobile, creature, damage, 100, 0, 0, 0, 0);                                                
                                                new Blood().MoveToWorld(mobile.Location, mobile.Map);
                                            }
                                        }
                                    }
                                }
                            }

                            eable.Free();
                        }
                    });
                });
            }            

            return true;
        }

        public static bool CanDoMassiveFireBreathAttack(BaseCreature creature)
        {
            if (creature == null)
                return false;
            
            if (creature.Combatant == null)
                return false;

            if (creature.Frozen || !creature.Alive)
                return false;

            if (Utility.GetDistance(creature.Location, creature.Combatant.Location) > creature.MassiveBreathRange)
                return false;

            return true;
        }

        public static bool CanDoMassiveIceBreathAttack(BaseCreature creature)
        {
            if (creature == null)
                return false;

            if (creature.Combatant == null)
                return false;

            if (creature.Frozen || !creature.Alive)
                return false;

            if (Utility.GetDistance(creature.Location, creature.Combatant.Location) > creature.MassiveBreathRange)
                return false;

            return true;
        }

        public static bool CanDoMassivePoisonBreathAttack(BaseCreature creature)
        {
            if (creature == null)
                return false;

            if (creature.Combatant == null)
                return false;

            if (creature.Frozen || !creature.Alive)
                return false;

            if (Utility.GetDistance(creature.Location, creature.Combatant.Location) > creature.MassiveBreathRange)
                return false;

            return true;
        }

        public static bool CanDoMassiveBoneBreathAttack(BaseCreature creature)
        {
            if (creature == null)
                return false;

            if (creature.Combatant == null)
                return false;

            if (creature.Frozen || !creature.Alive)
                return false;

            if (Utility.GetDistance(creature.Location, creature.Combatant.Location) > creature.MassiveBreathRange)
                return false;

            return true;
        }

        public static bool CanDoMassivePlantBreathAttack(BaseCreature creature)
        {
            if (creature == null)
                return false;

            if (creature.Combatant == null)
                return false;

            if (creature.Frozen || !creature.Alive)
                return false;

            if (Utility.GetDistance(creature.Location, creature.Combatant.Location) > creature.MassiveBreathRange)
                return false;

            return true;
        }

        public static bool DoMassiveFireBreathAttack(BaseCreature creature)
        {
            if (creature == null) return false;
            if (creature.Combatant == null) return false;
            if (!creature.Combatant.Alive) return false;

            Direction direction = creature.GetDirectionTo(creature.Combatant);
            Point3D newPoint = creature.GetPointByDirection(creature.Location, direction);
            SpellHelper.AdjustField(ref newPoint, creature.Map, 12, false);
            
            creature.PublicOverheadMessage(MessageType.Regular, 0, false, "*takes a massive breath*");

            SpecialAbilities.DoMassiveBreathAttack(creature, creature.Location, direction, creature.MassiveBreathRange, true, BreathType.Fire, true);

            return true;
        }

        public static bool DoMassiveIceBreathAttack(BaseCreature creature)
        {
            if (creature == null) return false;
            if (creature.Deleted) return false;

            if (creature.Combatant == null) return false;
            if (!creature.Combatant.Alive) return false;

            Direction direction = creature.GetDirectionTo(creature.Combatant);           

            creature.PublicOverheadMessage(MessageType.Regular, 0, false, "*takes a massive breath*");

            SpecialAbilities.DoMassiveBreathAttack(creature, creature.Location, direction, creature.MassiveBreathRange, true, BreathType.Ice, true);

            return true;
        }

        public static bool DoMassivePoisonBreathAttack(BaseCreature creature)
        {
            if (creature == null) return false;
            if (creature.Combatant == null) return false;
            if (!creature.Combatant.Alive) return false;

            Direction direction = creature.GetDirectionTo(creature.Combatant);

            creature.PublicOverheadMessage(MessageType.Regular, 0, false, "*takes a massive breath*");

            SpecialAbilities.DoMassiveBreathAttack(creature, creature.Location, direction, creature.MassiveBreathRange, true, BreathType.Poison, true);

            return true;
        }

        public static bool DoMassiveBoneBreathAttack(BaseCreature creature)
        {
            if (creature == null) return false;
            if (creature.Combatant == null) return false;
            if (!creature.Combatant.Alive) return false;

            Direction direction = creature.GetDirectionTo(creature.Combatant);

            creature.PublicOverheadMessage(MessageType.Regular, 0, false, "*gathers nearby bones*");

            SpecialAbilities.DoMassiveBreathAttack(creature, creature.Location, direction, creature.MassiveBreathRange, true, BreathType.Bone, true);

            return true;
        }

        public static bool DoMassivePlantBreathAttack(BaseCreature creature)
        {
            if (creature == null) return false;
            if (creature.Combatant == null) return false;
            if (!creature.Combatant.Alive) return false;

            Direction direction = creature.GetDirectionTo(creature.Combatant);

            creature.PublicOverheadMessage(MessageType.Regular, 0, false, "*gathers nearby plantlife*");

            SpecialAbilities.DoMassiveBreathAttack(creature, creature.Location, direction, creature.MassiveBreathRange, true, BreathType.Plant, true);

            return true;
        }
    }   
}
