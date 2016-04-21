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
    public class AICombatSpecialAction
    {
        public enum BreathAttackType
        {
            Fire,
            Ice,
            Poison
        }

        public static bool CanDoCombatSpecialAction(BaseCreature creature)
        {
            if (creature.DictCombatAction[CombatAction.CombatSpecialAction] > 0)
            {
                if (DateTime.UtcNow > creature.NextCombatSpecialActionAllowed)                
                    return true;                
            }

            return false;
        }

        public static bool DoCombatSpecialAction(BaseCreature creature)
        {
            CombatSpecialAction combatSpecialAction = CombatSpecialAction.None;

            int TotalValues = 0;

            Dictionary<CombatSpecialAction, int> DictTemp = new Dictionary<CombatSpecialAction, int>();

            DictTemp.Add(CombatSpecialAction.None, creature.DictCombatSpecialAction[CombatSpecialAction.None]);

            if (AICombatSpecialAction.CanDoApplyWeaponPoison(creature)) { DictTemp.Add(CombatSpecialAction.ApplyWeaponPoison, creature.DictCombatSpecialAction[CombatSpecialAction.ApplyWeaponPoison]); }
            if (AICombatSpecialAction.CanDoThrowShipBomb(creature)) { DictTemp.Add(CombatSpecialAction.ThrowShipBomb, creature.DictCombatSpecialAction[CombatSpecialAction.ThrowShipBomb]); }
            if (AICombatSpecialAction.CanDoCauseWounds(creature)) { DictTemp.Add(CombatSpecialAction.CauseWounds, creature.DictCombatSpecialAction[CombatSpecialAction.CauseWounds]); }
            if (AICombatSpecialAction.CanDoFireBreathAttack(creature)) { DictTemp.Add(CombatSpecialAction.FireBreathAttack, creature.DictCombatSpecialAction[CombatSpecialAction.FireBreathAttack]); }
            if (AICombatSpecialAction.CanDoIceBreathAttack(creature)) { DictTemp.Add(CombatSpecialAction.IceBreathAttack, creature.DictCombatSpecialAction[CombatSpecialAction.IceBreathAttack]); }
            if (AICombatSpecialAction.CanDoPoisonBreathAttack(creature)) { DictTemp.Add(CombatSpecialAction.PoisonBreathAttack, creature.DictCombatSpecialAction[CombatSpecialAction.PoisonBreathAttack]); }

            //Calculate Total Values
            foreach (KeyValuePair<CombatSpecialAction, int> pair in DictTemp)
            {
                TotalValues += pair.Value;
            }

            double ActionCheck = Utility.RandomDouble();
            double CumulativeAmount = 0.0;
            double AdditionalAmount = 0.0;

            //Determine CombatSpecialAction                      
            foreach (KeyValuePair<CombatSpecialAction, int> pair in creature.DictCombatSpecialAction)
            {
                AdditionalAmount = (double)pair.Value / (double)TotalValues;

                if (ActionCheck >= CumulativeAmount && ActionCheck < (CumulativeAmount + AdditionalAmount))
                {
                    combatSpecialAction = pair.Key;

                    switch (combatSpecialAction)
                    {
                        case CombatSpecialAction.ApplyWeaponPoison: AICombatSpecialAction.DoApplyWeaponPoison(creature); break;
                        case CombatSpecialAction.ThrowShipBomb: AICombatSpecialAction.DoThrowShipBomb(creature); break;
                        case CombatSpecialAction.CauseWounds: AICombatSpecialAction.DoCauseWounds(creature, creature.Combatant); break;
                        case CombatSpecialAction.FireBreathAttack: AICombatSpecialAction.DoFireBreathAttack(creature, creature.Combatant); break;
                        case CombatSpecialAction.IceBreathAttack: AICombatSpecialAction.DoIceBreathAttack(creature, creature.Combatant); break;
                        case CombatSpecialAction.PoisonBreathAttack: AICombatSpecialAction.DoPoisonBreathAttack(creature, creature.Combatant); break;
                    }

                    //Specia Increasing Delay
                    double speedScalar = 1;
                    double crippleModifier = 0;
                    double discordModifier = 0;

                    //Cripple Effect on Creature
                    creature.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Cripple, out crippleModifier);

                    //Discordance Effect on Creature                   
                    discordModifier = creature.DiscordEffect;

                    speedScalar += crippleModifier + discordModifier;

                    int specialActionDelay = (int)((double)Utility.RandomMinMax(creature.CombatSpecialActionMinDelay, creature.CombatSpecialActionMaxDelay) * speedScalar); 

                    creature.NextCombatSpecialActionAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(specialActionDelay);
                    creature.NextCombatEpicActionAllowed = creature.NextCombatEpicActionAllowed.AddSeconds(5);

                    if (creature.AcquireNewTargetEveryCombatAction)
                        creature.m_NextAcquireTargetAllowed = DateTime.UtcNow;

                    return true;
                }

                CumulativeAmount += AdditionalAmount;
            }

            return false;
        }

        //--------------
        
        public static bool CanDoApplyWeaponPoison(BaseCreature creature)
        {
            BaseWeapon weapon = creature.Weapon as BaseWeapon;

            if (weapon == null)
                return false;

            if (creature.Hidden)
                return false;

            bool poisonable = (weapon.Type == WeaponType.Slashing || weapon.Type == WeaponType.Piercing || weapon.Type == WeaponType.Ranged);

            if (!poisonable)
                return false;

            if (weapon.PoisonCharges > 0)
                return false;

            return true;
        }

        public static bool CanDoPoisonHit(BaseCreature creature)
        {
            return true;
        }

        public static bool CanDoThrowKnife(BaseCreature creature)
        {
            return true;
        }

        public static bool CanDoThrowPotion(BaseCreature creature)
        {
            return true;
        }

        public static bool CanDoThrowBomb(BaseCreature creature)
        {
            return true;
        }

        public static bool CanDoThrowShipBomb(BaseCreature creature)
        {
            BaseBoat targetBoat = null;

            if (creature.Combatant != null)
            {
                BaseCreature bc_Combatant = creature.Combatant as BaseCreature;
                PlayerMobile pm_Combatant = creature.Combatant as PlayerMobile;

                if (bc_Combatant != null)
                {
                    if (bc_Combatant.BoatOccupied != null && bc_Combatant.BoatOccupied != creature.BoatOccupied)
                    {
                        if (!bc_Combatant.BoatOccupied.Deleted && bc_Combatant.BoatOccupied.m_SinkTimer == null && Utility.GetDistance(creature.Location, bc_Combatant.BoatOccupied.Location) <= 8)
                            targetBoat = bc_Combatant.BoatOccupied;
                    }
                }

                if (pm_Combatant != null)
                {
                    if (pm_Combatant.BoatOccupied != null && pm_Combatant.BoatOccupied != creature.BoatOccupied)
                    {
                        if (!pm_Combatant.BoatOccupied.Deleted && pm_Combatant.BoatOccupied.m_SinkTimer == null && Utility.GetDistance(creature.Location, pm_Combatant.BoatOccupied.Location) <= 8)
                            targetBoat = pm_Combatant.BoatOccupied;
                    }
                }
            }

            if (targetBoat == null)
            {
                if (creature.BoatOccupied != null)
                {
                    if (creature.BoatOccupied.BoatCombatant != null && creature.BoatOccupied.BoatCombatant != creature.BoatOccupied)
                    {
                        if (creature.BoatOccupied.BoatCombatant != null)
                        {
                            if (!creature.BoatOccupied.BoatCombatant.Deleted && creature.BoatOccupied.BoatCombatant.m_SinkTimer == null && Utility.GetDistance(creature.Location, creature.BoatOccupied.BoatCombatant.Location) <= 8)
                                targetBoat = creature.BoatOccupied.BoatCombatant;
                        }
                    }
                }
            }

            bool targetBoatValid = false;

            if (targetBoat != null && targetBoat != creature.BoatOccupied)
            {
                if (targetBoat.GetBoatToLocationDistance(targetBoat, creature.Location) <= 8)
                    targetBoatValid = true;
            }

            return targetBoatValid;
        }

        public static bool CanDoCauseWounds(BaseCreature creature)
        {
            if (creature.Combatant == null)
                return false;

            if (creature.GetDistanceToSqrt(creature.Combatant) > creature.RangePerception || !creature.InLOS(creature.Combatant))
                return false;

            return true;
        }        

        public static bool CanDoFireBreathAttack(BaseCreature creature)
        {
            if (creature.Combatant == null)
                return false;

            if (creature.Frozen || !creature.Alive)
                return false;

            if (creature.GetDistanceToSqrt(creature.Combatant) > creature.RangePerception || !creature.InLOS(creature.Combatant))
                return false;
            
            return true;
        }

        public static bool CanDoIceBreathAttack(BaseCreature creature)
        {
            if (creature.Combatant == null)
                return false;

            if (creature.Frozen || !creature.Alive)
                return false;

            if (creature.GetDistanceToSqrt(creature.Combatant) > creature.RangePerception || !creature.InLOS(creature.Combatant))
                return false;

            return true;
        }

        public static bool CanDoPoisonBreathAttack(BaseCreature creature)
        {
            if (creature.Combatant == null)
                return false;

            if (creature.Frozen || !creature.Alive)
                return false;

            if (creature.GetDistanceToSqrt(creature.Combatant) > creature.RangePerception || !creature.InLOS(creature.Combatant))
                return false;

            return true;
        } 

        public static bool DoApplyWeaponPoison(BaseCreature creature)
        {
            bool success = true;

            BaseWeapon weapon = creature.Weapon as BaseWeapon;

            if (weapon == null)
                return false;

            bool poisonable = (weapon.Type == WeaponType.Slashing || weapon.Type == WeaponType.Piercing || weapon.Type == WeaponType.Ranged);

            if (!poisonable)
                return false;

            if (weapon.PoisonCharges > 0)
                return false;

            if (creature.AIObject != null)
                creature.AIObject.NextMove = DateTime.UtcNow + TimeSpan.FromSeconds(1.5);
            creature.NextCombatTime = DateTime.UtcNow + TimeSpan.FromSeconds(1.5);

            if (creature.Body.IsHuman)
                creature.Animate(17, 7, 1, true, false, 0);

            else
                creature.Animate(11, 5, 1, true, false, 0);

            double poisoningSkill = creature.Skills[SkillName.Poisoning].Base;
            int poisonLevel = (int)(Math.Floor(poisoningSkill / 25));

            Poison m_Poison = Poison.GetPoison(poisonLevel);

            weapon.Poison = m_Poison;
            weapon.PoisonCharges = 12 - (m_Poison.Level * 2);

            creature.PlaySound(0x4F);

            creature.PublicOverheadMessage(MessageType.Regular, 0, false, "*coats weapon in poison*");

            return success;
        }

        public static bool DoCauseWounds(BaseCreature creature, Mobile target)
        {
            if (creature == null || target == null)
                return false;

            if (!creature.Alive || !target.Alive)
                return false;

            creature.Direction = creature.GetDirectionTo(target);
            creature.PlaySound(0x605);

            CastingAnimationInfo.GetCastAnimationForBody(creature);
            creature.Animate(creature.SpellCastAnimation, creature.SpellCastFrameCount, 1, true, false, 0);

            double actionsCooldown = 2.0;

            if (creature.AIObject != null)
                creature.AIObject.NextMove = creature.AIObject.NextMove + TimeSpan.FromSeconds(actionsCooldown);
            creature.NextCombatTime = creature.NextCombatTime + TimeSpan.FromSeconds(actionsCooldown);
            creature.NextSpellTime = creature.NextSpellTime + TimeSpan.FromSeconds(actionsCooldown);
            creature.NextCombatHealActionAllowed = creature.NextCombatHealActionAllowed + TimeSpan.FromSeconds(actionsCooldown);
            creature.NextCombatSpecialActionAllowed = creature.NextCombatSpecialActionAllowed + TimeSpan.FromSeconds(actionsCooldown);
            creature.NextCombatEpicActionAllowed = creature.NextCombatEpicActionAllowed + TimeSpan.FromSeconds(actionsCooldown);
            
            target.FixedParticles(0x374A, 10, 30, 5038, 1149, 0, EffectLayer.Head);
            target.PlaySound(0x658);

            double magerySkill = creature.Skills[SkillName.Magery].Value;

            int baseDamage = (int)(magerySkill / 5);

            if (baseDamage < 1)
                baseDamage = 1;
            
            AOS.Damage(target, creature, baseDamage, 0, 100, 0, 0, 0);            

            return true;
        }

        public static void DoFireBreathAttack(BaseCreature creature, Mobile target)
        {
            DoBreathAttack(BreathAttackType.Fire, creature, target);
        }

        public static void DoIceBreathAttack(BaseCreature creature, Mobile target)
        {
            DoBreathAttack(BreathAttackType.Ice, creature, target);
        }

        public static void DoPoisonBreathAttack(BaseCreature creature, Mobile target)
        {
            DoBreathAttack(BreathAttackType.Poison, creature, target);
        }

        public static void DoBreathAttack(BreathAttackType breathType, BaseCreature creature, Mobile target)
        {
            if (!SpecialAbilities.Exists(creature)) return;
            if (!SpecialAbilities.Exists(target)) return;
            if (!creature.CanBeHarmful(target)) return;

            creature.DoHarmful(target);
            creature.Direction = creature.GetDirectionTo(target);

            if (creature.IsHighSeasBodyType)
                creature.Animate(Utility.RandomList(27), 5, 1, true, false, 0);
            else
                creature.Animate(12, 5, 1, true, false, 0);

            SpecialAbilities.HinderSpecialAbility(1.0, null, creature, 1.0, 1.5, true, 0, false, "", "");

            Timer.DelayCall(TimeSpan.FromSeconds(1.3), delegate
            {
                if (!SpecialAbilities.Exists(creature))
                    return;

                switch (breathType)
                {
                    case BreathAttackType.Fire:
                        Effects.PlaySound(creature.Location, creature.Map, 0x227);
                        Effects.SendMovingEffect(creature, target, 0x36D4, 5, 0, false, false, 0, 0);
                    break;

                    case BreathAttackType.Ice:
                        Effects.PlaySound(creature.Location, creature.Map, 0x64F);
                        Effects.SendMovingEffect(creature, target, 0x36D4, 5, 0, false, false, 1153, 0);
                    break;

                    case BreathAttackType.Poison:
                        Effects.PlaySound(creature.Location, creature.Map, 0x22F);
                        Effects.SendMovingEffect(creature, target, 0x372A, 10, 0, false, false, 2208, 0);
                    break;
                }

                Timer.DelayCall(TimeSpan.FromSeconds(1.0), delegate
                {
                    if (creature == null)
                        return;

                    if (creature.CanBeHarmful(target))
                    {
                        double baseDamage = (double)creature.DamageMax;

                        if (creature.Controlled && creature.ControlMaster is PlayerMobile)
                        {
                            if (target is PlayerMobile)
                                baseDamage *= BaseCreature.BreathDamageToPlayerScalar * creature.PvPAbilityDamageScalar;

                            if (target is BaseCreature)
                                baseDamage *= BaseCreature.BreathDamageToCreatureScalar;
                        }

                        DungeonArmor.PlayerDungeonArmorProfile defenderDungeonArmor = new DungeonArmor.PlayerDungeonArmorProfile(target, null);

                        if (defenderDungeonArmor.MatchingSet && !defenderDungeonArmor.InPlayerCombat)
                            baseDamage *= defenderDungeonArmor.DungeonArmorDetail.BreathDamageReceivedScalar;

                        switch (breathType)
                        {
                            case BreathAttackType.Fire:     
                                Effects.PlaySound(target.Location, target.Map, 0x208);
                                Effects.SendLocationParticles(EffectItem.Create(target.Location, target.Map, TimeSpan.FromSeconds(0.5)), 0x3996, 10, 20, 5029);
                            break;

                            case BreathAttackType.Ice:
                                baseDamage = (double)creature.DamageMax * .75;

                                if (target is PlayerMobile)
                                    SpecialAbilities.CrippleSpecialAbility(1.0, creature, target, .05, 10, -1, true, "", "The blast of ice has slowed your actions!");                                    
                                else
                                    SpecialAbilities.CrippleSpecialAbility(1.0, creature, target, .10, 10, -1, true, "", "The blast of ice has slowed your actions!");  

                                Effects.PlaySound(target.Location, target.Map, 0x208);
                                Effects.SendLocationParticles(EffectItem.Create(target.Location, target.Map, TimeSpan.FromSeconds(0.25)), 0x3779, 10, 20, 1153, 0, 5029, 0);
                            break;

                            case BreathAttackType.Poison:                           
                                baseDamage = (double)creature.DamageMax * .5;

                                int poisonLevel = 0;

                                if (creature.HitPoison != null)
                                    poisonLevel = creature.HitPoison.Level;

                                double poisonChance = 1.0;

                                if (creature.IsControlledCreature())
                                {
                                    if (target is PlayerMobile)
                                    {
                                        poisonChance = .5;
                                        poisonLevel--;
                                    }

                                    if (target is BaseCreature)
                                    {
                                        BaseCreature bc_Target = target as BaseCreature;

                                        if (bc_Target.IsControlledCreature())
                                        {
                                            poisonChance = .5;
                                            poisonLevel--;
                                        }
                                    }
                                }
                                
                                if (poisonLevel < 0)
                                    poisonLevel = 0;

                                int poisonHue = 2208;

                                poisonHue += poisonLevel;

                                if (Utility.RandomDouble() <= poisonChance)
                                {
                                    Poison poison = Poison.GetPoison(poisonLevel);
                                    target.ApplyPoison(target, poison);
                                }

                                Effects.PlaySound(target.Location, target.Map, 0x22F);
                                Effects.SendLocationParticles(EffectItem.Create(target.Location, target.Map, TimeSpan.FromSeconds(0.25)), 0x372A, 10, 20, poisonHue, 0, 5029, 0);
                            break;
                        }

                        int finalDamage = (int)baseDamage;

                        if (target != null)
                        {
                            int finalAdjustedDamage = AOS.Damage(target, creature, finalDamage, 100, 0, 0, 0, 0);

                            if (creature != null)
                                creature.DisplayFollowerDamage(target, finalAdjustedDamage);
                        }
                    }
                });
            });
        }

        public static bool DoThrowShipBomb(BaseCreature creature)
        {
            if (creature == null)
                return false;

            BaseBoat targetBoat = null;

            if (creature.Combatant != null)
            {
                BaseCreature bc_Combatant = creature.Combatant as BaseCreature;
                PlayerMobile pm_Combatant = creature.Combatant as PlayerMobile;

                if (bc_Combatant != null)
                {
                    if (bc_Combatant.BoatOccupied != null && bc_Combatant.BoatOccupied != creature.BoatOccupied)
                    {
                        if (!bc_Combatant.BoatOccupied.Deleted && bc_Combatant.BoatOccupied.m_SinkTimer == null && Utility.GetDistance(creature.Location, bc_Combatant.BoatOccupied.Location) <= 8)
                            targetBoat = bc_Combatant.BoatOccupied;
                    }
                }

                if (pm_Combatant != null)
                {
                    if (pm_Combatant.BoatOccupied != null && pm_Combatant.BoatOccupied != creature.BoatOccupied)
                    {
                        if (!pm_Combatant.BoatOccupied.Deleted && pm_Combatant.BoatOccupied.m_SinkTimer == null && Utility.GetDistance(creature.Location, pm_Combatant.BoatOccupied.Location) <= 8)
                            targetBoat = pm_Combatant.BoatOccupied;
                    }
                }
            }

            if (targetBoat == null)
            {
                if (creature.BoatOccupied != null)
                {
                    if (creature.BoatOccupied.BoatCombatant != null && creature.BoatOccupied.BoatCombatant != creature.BoatOccupied)
                    {
                        if (creature.BoatOccupied.BoatCombatant != null)
                        {
                            if (!creature.BoatOccupied.BoatCombatant.Deleted && creature.BoatOccupied.BoatCombatant.m_SinkTimer == null && Utility.GetDistance(creature.Location, creature.BoatOccupied.BoatCombatant.Location) <= 8)
                                targetBoat = creature.BoatOccupied.BoatCombatant;
                        }
                    }
                }
            }

            if (targetBoat == null || targetBoat == creature.BoatOccupied)
                return false;

            if (creature.AIObject != null)
                creature.AIObject.NextMove = creature.AIObject.NextMove + TimeSpan.FromSeconds(1.5);

            creature.NextCombatTime = creature.NextCombatTime + TimeSpan.FromSeconds(1.5);

            Effects.PlaySound(creature.Location, creature.Map, 0x4B9);

            if (creature.Body.IsHuman)
                creature.Animate(31, 7, 1, true, false, 0);

            else
                creature.Animate(4, 4, 1, true, false, 0);

            Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate { StartThrowShipBomb(creature, targetBoat); });

            return true;
        }

        public static void StartThrowShipBomb(BaseCreature creature, BaseBoat targetBoat)
        {
            Timer.DelayCall(TimeSpan.FromSeconds(.3), delegate { Effects.PlaySound(creature.Location, creature.Map, 0x666); });

            Point3D randomShipLocation = targetBoat.GetRandomEmbarkLocation(true);

            IEntity startLocation = new Entity(Serial.Zero, new Point3D(creature.Location.X, creature.Location.Y, creature.Location.Z + 10), creature.Map);
            IEntity endLocation = new Entity(Serial.Zero, new Point3D(randomShipLocation.X, randomShipLocation.Y, randomShipLocation.Z + 5), targetBoat.Map);

            Effects.SendMovingEffect(startLocation, endLocation, 0x1C19, 10, 0, false, false, 0, 0);

            int distance = targetBoat.GetBoatToLocationDistance(targetBoat, creature.Location);

            double destinationDelay = (double)distance * .06;
            double explosionDelay = (double)distance * .16;

            Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate { Effects.SendLocationEffect(endLocation.Location, targetBoat.Map, 0x1C19, 20); });
            Timer.DelayCall(TimeSpan.FromSeconds(explosionDelay), delegate { DetonateShipBomb(creature, targetBoat, randomShipLocation); });
        }

        public static void DetonateShipBomb(BaseCreature creature, BaseBoat targetBoat, Point3D location)
        {
            IEntity explosionLocation = new Entity(Serial.Zero, new Point3D(location.X, location.Y, location.Z - 1), targetBoat.Map);

            Effects.SendLocationParticles(explosionLocation, Utility.RandomList(0x36BD, 0x36BF, 0x36CB, 0x36BC), 30, 7, 5044);
            Effects.PlaySound(explosionLocation, targetBoat.Map, 0x11D);

            BaseBoat boatAtLocation = BaseBoat.FindBoatAt(location, targetBoat.Map);

            if (boatAtLocation != null)
            {
                int boatDamage = Utility.RandomMinMax(10, 20);

                boatAtLocation.ReceiveDamage(null, null, boatDamage, DamageType.Hull);

                int mobileDamage = Utility.RandomMinMax(10, 20);

                IPooledEnumerable eable = targetBoat.Map.GetObjectsInRange(location, 1);

                foreach (object obj in eable)
                {
                    if (obj is Mobile)
                    {
                        Mobile mobile = obj as Mobile;

                        if (creature.CanBeHarmful(mobile))
                        {
                            int finalDamage = mobileDamage;

                            PlayerMobile pm_Target = mobile as PlayerMobile;
                            
                            creature.DoHarmful(mobile);
                            AOS.Damage(mobile, creature, finalDamage, 100, 0, 0, 0, 0);
                            new Blood().MoveToWorld(mobile.Location, mobile.Map);
                        }
                    }
                }

                eable.Free();
            }
        }
    }
}
