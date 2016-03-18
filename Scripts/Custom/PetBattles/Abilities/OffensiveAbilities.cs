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
    public class BiteAbility : PetBattleAbility
    {
        public BiteAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Offensive;
            m_Name = "Bite";
            m_Description = "Attempts an immediate attack that if it hits permanently reduces opponent’s Attack by 2";
            m_Action = delegate { BiteAbilityAction(creature, m_Name, m_Description); };
        }

        public void BiteAbilityAction(BaseCreature creature, string name, string description)
        {          
            Effects.PlaySound(creature.Location, creature.Map, 0x525);
            
            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.ImmediateAttack, creature, 20, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));
                        
            BaseCreature bc_Combatant = creature.Combatant as BaseCreature;
            if (bc_Combatant != null)
            {
                if ((creature.GetDefaultWeapon() as BaseWeapon).PetBattlePredetermineCheckHit(creature, bc_Combatant))
                {                    
                    creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, true);

                    PetBattleAbilities.AddBloodEffect(bc_Combatant, 1, 2);
                    bc_Combatant.FixedEffect(0x5683, 10, 20);
                    
                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.PredeterminedHit, creature, 1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

                    bc_Combatant.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.ReduceAttack, creature, 2, 1, DateTime.UtcNow + TimeSpan.FromDays(7)));
                }

                else
                {
                    creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, false);
                    
                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.PredeterminedMiss, creature, -1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));
                }
            }

            creature.PetBattlePostAbilitySwingDelay = creature.NextCombatTime - DateTime.UtcNow;
            creature.NextCombatTime = DateTime.UtcNow;
        }
    }

    public class PoisonAbility : PetBattleAbility
    {
        public PoisonAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Offensive;
            m_Name = "Poison";
            m_Description = "Attempts an immediate attack that if it hits causes Poison";
            m_Action = delegate { PoisonAbilityAction(creature, m_Name, m_Description); };
        }

        public void PoisonAbilityAction(BaseCreature creature, string name, string description)
        {          
            Effects.PlaySound(creature.Location, creature.Map, 0x525);

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.ImmediateAttack, creature, 40, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

            BaseCreature bc_Combatant = creature.Combatant as BaseCreature;
            if (bc_Combatant != null)
            {
                if ((creature.GetDefaultWeapon() as BaseWeapon).PetBattlePredetermineCheckHit(creature, bc_Combatant))
                {
                    creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, true);

                    Effects.PlaySound(bc_Combatant.Location, bc_Combatant.Map, 0x474);
                    bc_Combatant.FixedParticles(0x374A, 10, 20, 5021, 0x110, 0, EffectLayer.Head);
                    
                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.PredeterminedHit, creature, 1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

                    bc_Combatant.ApplyPoison(creature, Poison.Greater);                    
                }

                else
                {
                    creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, false);
                    
                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.PredeterminedMiss, creature, -1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));
                }
            }

            creature.PetBattlePostAbilitySwingDelay = creature.NextCombatTime - DateTime.UtcNow;
            creature.NextCombatTime = DateTime.UtcNow;
        }
    }

    public class WeakenAbility : PetBattleAbility
    {
        public WeakenAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Offensive;
            m_Name = "Weaken";
            m_Description = "Attempts an immediate attack that if it hits permanently reduces opponent's Damage by 3 and additionally inflicts 30 Magic damage if opponent is Poisoned";
            m_Action = delegate { WeakenAbilityAction(creature, m_Name, m_Description); };
        }

        public void WeakenAbilityAction(BaseCreature creature, string name, string description)
        {            
            Effects.PlaySound(creature.Location, creature.Map, 0x525);

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.ImmediateAttack, creature, 60, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

            BaseCreature bc_Combatant = creature.Combatant as BaseCreature;
            if (bc_Combatant != null)
            {
                if ((creature.GetDefaultWeapon() as BaseWeapon).PetBattlePredetermineCheckHit(creature, bc_Combatant))
                {
                    creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, true);

                    bc_Combatant.FixedParticles(0x3779, 10, 15, 5009, EffectLayer.Waist);
                    Effects.PlaySound(bc_Combatant.Location, bc_Combatant.Map, 0x1E6);                    

                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.PredeterminedHit, creature, 1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

                    if (bc_Combatant.Poison != null)
                    {
                        creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                        (PetBattleAbilityEffect.NextHitMagicDamage, creature, 30, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));
                    }

                    bc_Combatant.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.ReduceDamage, creature, 3, 1, DateTime.UtcNow + TimeSpan.FromDays(7)));                    

                    Effects.PlaySound(bc_Combatant.Location, bc_Combatant.Map, 0x474);
                    bc_Combatant.FixedParticles(0x374A, 10, 20, 5021, 0x110, 0, EffectLayer.Head);
                }

                else
                {
                    creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, false);
                    
                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.PredeterminedMiss, creature, -1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));
                }
            }

            creature.PetBattlePostAbilitySwingDelay = creature.NextCombatTime - DateTime.UtcNow;
            creature.NextCombatTime = DateTime.UtcNow;
        }
    }

    public class LeechAbility : PetBattleAbility
    {
        public LeechAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Offensive;
            m_Name = "Leech";
            m_Description = "Attacks over the next 12 seconds heal the creature for 100% of damage inflicted";
            m_Action = delegate { LeechAbilityAction(creature, m_Name, m_Description); };
        }

        public void LeechAbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, false, false);
            
            creature.FixedParticles(0x377A, 10, 40, 5022, EffectLayer.Waist);

            Effects.PlaySound(creature.Location, creature.Map, 0x525);
            creature.PlaySound(creature.GetAngerSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.Leech, creature, 1.0, 0, DateTime.UtcNow + TimeSpan.FromSeconds(12)));
        }
    }

    public class WebAbility : PetBattleAbility
    {
        public WebAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Offensive;
            m_Name = "Web";
            m_Description = "Attempts an immediate attack that if it hits reduces opponent's Defense by 100 for 6 seconds and makes opponent to be unable to attack, use abilities, or gain Power for 6 seconds";
            m_Action = delegate { WebAbilityAction(creature, m_Name, m_Description); };
        }

        public void WebAbilityAction(BaseCreature creature, string name, string description)
        {            
            Effects.PlaySound(creature.Location, creature.Map, 0x525);

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.ImmediateAttack, creature, 100, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

            BaseCreature bc_Combatant = creature.Combatant as BaseCreature;
            if (bc_Combatant != null)
            {
                if ((creature.GetDefaultWeapon() as BaseWeapon).PetBattlePredetermineCheckHit(creature, bc_Combatant))
                {
                    creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, true);

                    bc_Combatant.FixedEffect(0x376A, 6, 2);
                    bc_Combatant.FixedParticles(0x0EE6, 0, 200, 5021, 0, 0, EffectLayer.CenterFeet);
                    Effects.PlaySound(bc_Combatant.Location, bc_Combatant.Map, 0x204);                    
                    
                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.PredeterminedHit, creature, 1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

                    bc_Combatant.Frozen = true;

                    bc_Combatant.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.Frozen, creature, 6, 0, DateTime.UtcNow + TimeSpan.FromSeconds(6)));

                    bc_Combatant.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                     (PetBattleAbilityEffect.ReduceDefense, creature, 100, 0, DateTime.UtcNow + TimeSpan.FromSeconds(6)));
                }

                else
                {
                    creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, false);
                    
                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.PredeterminedMiss, creature, -1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));
                }
            }

            creature.PetBattlePostAbilitySwingDelay = creature.NextCombatTime - DateTime.UtcNow;
            creature.NextCombatTime = DateTime.UtcNow;
        }
    }

    public class RendAbility : PetBattleAbility
    {
        public RendAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Offensive;
            m_Name = "Rend";
            m_Description = "Attempts an immediate attack that if it hits reduces opponent's Armor by 25 for 12 seconds";
            m_Action = delegate { RendAbilityAction(creature, m_Name, m_Description); };
        }

        public void RendAbilityAction(BaseCreature creature, string name, string description)
        {            
            Effects.PlaySound(creature.Location, creature.Map, 0x525);

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.ImmediateAttack, creature, 20, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

            BaseCreature bc_Combatant = creature.Combatant as BaseCreature;
            if (bc_Combatant != null)
            {
                if ((creature.GetDefaultWeapon() as BaseWeapon).PetBattlePredetermineCheckHit(creature, bc_Combatant))
                {
                    creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, true);
                    
                    PetBattleAbilities.AddBloodEffect(bc_Combatant, 1, 2);
                    bc_Combatant.FixedEffect(0x5683, 10, 20);                    
                    
                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.PredeterminedHit, creature, 1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

                    bc_Combatant.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.ReduceArmor, creature, 25, 0, DateTime.UtcNow + TimeSpan.FromSeconds(12)));
                }

                else
                {
                    creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, false);
                    
                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.PredeterminedMiss, creature, -1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));
                }
            }

            creature.PetBattlePostAbilitySwingDelay = creature.NextCombatTime - DateTime.UtcNow;
            creature.NextCombatTime = DateTime.UtcNow;
        }
    }

    public class QuickenAbility : PetBattleAbility
    {
        public QuickenAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Offensive;
            m_Name = "Quicken";
            m_Description = "Permanently reduce Attack by 3 and increase Speed by 4";
            m_Action = delegate { QuickenAbilityAction(creature, m_Name, m_Description); };
        }

        public void QuickenAbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, false, false);

            creature.FixedParticles(0x375A, 10, 30, 5010, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x28E);

            creature.PlaySound(creature.GetAngerSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.ReduceAttack, creature, 3, 1, DateTime.UtcNow + TimeSpan.FromDays(7)));

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseSpeed, creature, 4, 1, DateTime.UtcNow + TimeSpan.FromDays(7)));
        }
    }

    public class FrenzyAbility : PetBattleAbility
    {
        public FrenzyAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Offensive;
            m_Name = "Frenzy";
            m_Description = "Each successful attack over the next 12 seconds reduces Attack by 3, increases Speed by 4 and increases Damage by 3";
            m_Action = delegate { FrenzyAbilityAction(creature, m_Name, m_Description); };
        }

        public void FrenzyAbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, false, false);

            creature.FixedParticles(0x373A, 10, 30, 5018, EffectLayer.Waist);
            creature.FixedEffect(0x5683, 10, 20);
            Effects.PlaySound(creature.Location, creature.Map, 0x1EA);

            creature.PlaySound(creature.GetAngerSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.SuccessfulAttackIncreaseAttack, creature, 6, 0, DateTime.UtcNow + TimeSpan.FromSeconds(12)));
            
            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.SuccessfulAttackIncreaseSpeed, creature, 4, 0, DateTime.UtcNow + TimeSpan.FromSeconds(12)));            

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.SuccessfulAttackIncreaseDamage, creature, 3, 0, DateTime.UtcNow + TimeSpan.FromSeconds(12)));            
        }
    }

    public class PrecisionAbility : PetBattleAbility
    {
        public PrecisionAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Offensive;
            m_Name = "Precision";
            m_Description = "Makes an immediate attack that automatically hits and permanently increases Attack by 12";
            m_Action = delegate { PrecisionAbilityAction(creature, m_Name, m_Description); };
        }

        public void PrecisionAbilityAction(BaseCreature creature, string name, string description)
        {
            BaseCreature bc_Combatant = creature.Combatant as BaseCreature;
            if (bc_Combatant != null)
            {
                creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, true);

                PetBattleAbilities.AddBloodEffect(bc_Combatant, 1, 2);
                creature.FixedParticles(0x375A, 10, 15, 5011, EffectLayer.Head);
                Effects.PlaySound(creature.Location, creature.Map, 0x1EB);
                //bc_Combatant.FixedEffect(0x5683, 10, 20);

                PetBattleAbilities.AddBloodEffect(bc_Combatant, 1, 2);
            }            
            
            Effects.PlaySound(creature.Location, creature.Map, 0x525);

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.ImmediateAttack, creature, 1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseAttack, creature, 10, 1, DateTime.UtcNow + TimeSpan.FromDays(7)));

            creature.PetBattlePostAbilitySwingDelay = creature.NextCombatTime - DateTime.UtcNow;
            creature.NextCombatTime = DateTime.UtcNow;            
        }
    }

    public class BerserkAbility : PetBattleAbility
    {
        public BerserkAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Offensive;
            m_Name = "Berserk";
            m_Description = "Increase Damage by a percentage equal to percent of missing Hit Points for 18 seconds";
            m_Action = delegate { BerserkAbilityAction(creature, m_Name, m_Description); };
        }

        public void BerserkAbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, false, false);
                        
            creature.FixedEffect(0x5683, 10, 20);

            Effects.PlaySound(creature.Location, creature.Map, 0x525);
            creature.PlaySound(creature.GetAngerSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseDamagePercentByPercentMissingHits, creature, 1.0, 0, DateTime.UtcNow + TimeSpan.FromSeconds(18)));
        }
    }

    public class ShredAbility : PetBattleAbility
    {
        public ShredAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Offensive;
            m_Name = "Shred";
            m_Description = "Attempts an immediate attack that if it hits additionally inflicts 33% of damage caused as Bleeding damage over the next 12 seconds";
            m_Action = delegate { ShredAbilityAction(creature, m_Name, m_Description); };
        }

        public void ShredAbilityAction(BaseCreature creature, string name, string description)
        {            
            Effects.PlaySound(creature.Location, creature.Map, 0x525);

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.ImmediateAttack, creature, 20, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

            BaseCreature bc_Combatant = creature.Combatant as BaseCreature;
            if (bc_Combatant != null)
            {
                if ((creature.GetDefaultWeapon() as BaseWeapon).PetBattlePredetermineCheckHit(creature, bc_Combatant))
                {
                    creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, true);

                    PetBattleAbilities.AddBloodEffect(bc_Combatant, 3, 4);
                    bc_Combatant.FixedEffect(0x5683, 10, 20);
                    
                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.PredeterminedHit, creature, 1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.CauseBleedPercent, creature, 0.33, 3, DateTime.UtcNow + TimeSpan.FromSeconds(12)));
                }

                else
                {
                    creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, false);
                    
                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.PredeterminedMiss, creature, -1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));
                }
            }            

            creature.PetBattlePostAbilitySwingDelay = creature.NextCombatTime - DateTime.UtcNow;
            creature.NextCombatTime = DateTime.UtcNow;
        }
    }

    public class GrowlAbility : PetBattleAbility
    {
        public GrowlAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Offensive;
            m_Name = "Growl";
            m_Description = "Permanently increase Attack by 3, opponent permanently reduces Attack by 3, and opponent has a 33% chance to lose 1 Offensive Power";
            m_Action = delegate { GrowlAbilityAction(creature, m_Name, m_Description); };
        }

        public void GrowlAbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, false, false);
                          
            creature.FixedParticles(0x375A, 10, 30, 5011, EffectLayer.Head);
            Effects.PlaySound(creature.Location, creature.Map, 0x1EB);

            creature.PlaySound(creature.GetAngerSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseAttack, creature, 3, 1, DateTime.UtcNow + TimeSpan.FromDays(7)));

            BaseCreature bc_Combatant = creature.Combatant as BaseCreature;
            if (bc_Combatant != null)
            {
                bc_Combatant.FixedParticles(0x3779, 10, 15, 5004, EffectLayer.Head);
                Effects.PlaySound(creature.Location, creature.Map, 0x1E4);
                
                bc_Combatant.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                (PetBattleAbilityEffect.ReduceAttack, creature, 3, 1, DateTime.UtcNow + TimeSpan.FromDays(7)));

                double randomValue = Utility.RandomDouble();

                if (randomValue <= .33)
                {
                    bc_Combatant.m_PetBattleTotem.OffensivePower -= 1;
                    bc_Combatant.PublicOverheadMessage(0, creature.m_PetBattleTeamHue, false, "-1 Offensive Power");
                }
            }
        }
    }

    public class HamstringAbility : PetBattleAbility
    {
        public HamstringAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Offensive;
            m_Name = "Hamstring";
            m_Description = "Attempts an immediate attack that if it hits permanently reduces opponent’s Speed by 4";
            m_Action = delegate { HamstringAbilityAction(creature, m_Name, m_Description); };
        }

        public void HamstringAbilityAction(BaseCreature creature, string name, string description)
        {     
            Effects.PlaySound(creature.Location, creature.Map, 0x525);

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.ImmediateAttack, creature, 60, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

            BaseCreature bc_Combatant = creature.Combatant as BaseCreature;
            if (bc_Combatant != null)
            {
                if ((creature.GetDefaultWeapon() as BaseWeapon).PetBattlePredetermineCheckHit(creature, bc_Combatant))
                {   
                    creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, true);

                    PetBattleAbilities.AddBloodEffect(bc_Combatant, 1, 2);

                    bc_Combatant.FixedParticles(0x3779, 10, 15, 5002, EffectLayer.Head);
                    Effects.PlaySound(creature.Location, creature.Map, 0x1DF);
                    //bc_Combatant.FixedEffect(0x5683, 10, 20);
                    
                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.PredeterminedHit, creature, 1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

                    bc_Combatant.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.ReduceSpeed, creature, 2, 1, DateTime.UtcNow + TimeSpan.FromDays(7)));
                }

                else
                {
                    creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, false);
                    
                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.PredeterminedMiss, creature, -1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));
                }
            } 

            creature.PetBattlePostAbilitySwingDelay = creature.NextCombatTime - DateTime.UtcNow;
            creature.NextCombatTime = DateTime.UtcNow;
        }
    }

    public class RavageAbility : PetBattleAbility
    {
        public RavageAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Offensive;
            m_Name = "Ravage";
            m_Description = "Attempts an immediate attack that if it hits additionally inflicts 200% of damage caused as Bleeding damage over the next 12 seconds";
            m_Action = delegate { RavageAbilityAction(creature, m_Name, m_Description); };
        }

        public void RavageAbilityAction(BaseCreature creature, string name, string description)
        {   
            Effects.PlaySound(creature.Location, creature.Map, 0x525);

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.ImmediateAttack, creature, 80, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

            BaseCreature bc_Combatant = creature.Combatant as BaseCreature;
            if (bc_Combatant != null)
            {
                if ((creature.GetDefaultWeapon() as BaseWeapon).PetBattlePredetermineCheckHit(creature, bc_Combatant))
                {
                    creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, true);

                    PetBattleAbilities.AddBloodEffect(bc_Combatant, 4, 8);
                    bc_Combatant.FixedEffect(0x5683, 10, 20);
                    
                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.PredeterminedHit, creature, 1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.CauseBleedPercent, creature, 2.0, 3, DateTime.UtcNow + TimeSpan.FromSeconds(12)));
                }

                else
                {
                    creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, false);

                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.PredeterminedMiss, creature, -1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));
                }
            }

            creature.PetBattlePostAbilitySwingDelay = creature.NextCombatTime - DateTime.UtcNow;
            creature.NextCombatTime = DateTime.UtcNow;
        }
    }

    public class FerocityAbility : PetBattleAbility
    {
        public FerocityAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Offensive;
            m_Name = "Ferocity";
            m_Description = "Each successful attack over the next 18 seconds permanently increases Attack by 3 and Damage by 2";
            m_Action = delegate { FerocityAbilityAction(creature, m_Name, m_Description); };
        }

        public void FerocityAbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, false, false);
                        
            creature.FixedEffect(0x5683, 10, 20);
            Effects.PlaySound(creature.Location, creature.Map, 0x525);
            creature.PlaySound(creature.GetAngerSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.SuccessfulAttackIncreaseAttack, creature, 3, 1, DateTime.UtcNow + TimeSpan.FromSeconds(18)));

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.SuccessfulAttackIncreaseDamage, creature, 2, 1, DateTime.UtcNow + TimeSpan.FromSeconds(18)));
        }
    }

    public class SlamAbility : PetBattleAbility
    {
        public SlamAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Offensive;
            m_Name = "Slam";
            m_Description = "Attempts an immediate attack that if it hits causes an additional 25% damage and permanently reduces opponent's Defense by 2";
            m_Action = delegate { SlamAbilityAction(creature, m_Name, m_Description); };
        }

        public void SlamAbilityAction(BaseCreature creature, string name, string description)
        {
            Effects.PlaySound(creature.Location, creature.Map, 0x525);

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.ImmediateAttack, creature, 20, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

            BaseCreature bc_Combatant = creature.Combatant as BaseCreature;
            if (bc_Combatant != null)
            {
                if ((creature.GetDefaultWeapon() as BaseWeapon).PetBattlePredetermineCheckHit(creature, bc_Combatant))
                {
                    creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, true);

                    PetBattleAbilities.AddBloodEffect(bc_Combatant, 1, 2);
                    bc_Combatant.FixedEffect(0x5683, 10, 20);

                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.PredeterminedHit, creature, 1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.NextHitDamageBoostPercent, creature, 0.25, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.ReduceDefense, creature, 1, 1, DateTime.UtcNow + TimeSpan.FromDays(7)));
                }

                else
                {
                    creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, false);
                    
                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.PredeterminedMiss, creature, -1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));
                }
            }            

            creature.PetBattlePostAbilitySwingDelay = creature.NextCombatTime - DateTime.UtcNow;
            creature.NextCombatTime = DateTime.UtcNow;
        }
    }

    public class GrappleAbility : PetBattleAbility
    {
        public GrappleAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Offensive;
            m_Name = "Grapple";
            m_Description = "Each successful attack over the next 12 seconds reduces opponent’s Speed by 4, Attack by 6, and Defense by 6";
            m_Action = delegate { GrappleAbilityAction(creature, m_Name, m_Description); };
        }

        public void GrappleAbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, false, false);
            
            Effects.PlaySound(creature.Location, creature.Map, 0x525);
            creature.PlaySound(creature.GetAngerSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.SuccessfulAttackReduceOpponentSpeed, creature, 4, 0, DateTime.UtcNow + TimeSpan.FromSeconds(12)));

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.SuccessfulAttackReduceOpponentAttack, creature, 6, 0, DateTime.UtcNow + TimeSpan.FromSeconds(12)));

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.SuccessfulAttackReduceOpponentDefense, creature, 6, 0, DateTime.UtcNow + TimeSpan.FromSeconds(12)));
        }
    }

    public class KnockdownAbility : PetBattleAbility
    {
        public KnockdownAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Offensive;
            m_Name = "Knockdown";
            m_Description = "Attempts an immediate attack that causes an additional 100% damage and if it hits makes opponent unable to attack, use abilities, or gain Power for 3 seconds";
            m_Action = delegate { KnockdownAbilityAction(creature, m_Name, m_Description); };
        }

        public void KnockdownAbilityAction(BaseCreature creature, string name, string description)
        {
            Effects.PlaySound(creature.Location, creature.Map, 0x525);

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.ImmediateAttack, creature, 60, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

            BaseCreature bc_Combatant = creature.Combatant as BaseCreature;
            if (bc_Combatant != null)
            {
                if ((creature.GetDefaultWeapon() as BaseWeapon).PetBattlePredetermineCheckHit(creature, bc_Combatant))
                {
                    creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, true);

                    PetBattleAbilities.AddBloodEffect(bc_Combatant, 3, 4);
                    Effects.PlaySound(creature.Location, creature.Map, 0x204);                    
                    bc_Combatant.FixedEffect(0x376A, 6, 2);
                    //bc_Combatant.FixedEffect(0x5683, 10, 20);
                    
                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.PredeterminedHit, creature, 1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.NextHitDamageBoostPercent, creature, 1.0, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

                    bc_Combatant.Frozen = true;

                    bc_Combatant.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.Frozen, creature, 3, 0, DateTime.UtcNow + TimeSpan.FromSeconds(3)));
                }

                else
                {
                    creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, false);
                    
                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.PredeterminedMiss, creature, -1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));
                }
            } 

            creature.PetBattlePostAbilitySwingDelay = creature.NextCombatTime - DateTime.UtcNow;
            creature.NextCombatTime = DateTime.UtcNow;
        }
    }

    public class FuryAbility : PetBattleAbility
    {
        public FuryAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Offensive;
            m_Name = "Fury";
            m_Description = "Permanently reduce Defense by 12 and permanently increase Damage by 9";
            m_Action = delegate { FuryAbilityAction(creature, m_Name, m_Description); };
        }

        public void FuryAbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, false, false);
            
            Effects.PlaySound(creature.Location, creature.Map, 0x525);
            creature.PlaySound(creature.GetAngerSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.ReduceDefense, creature, 12, 1, DateTime.UtcNow + TimeSpan.FromDays(7)));

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseDamage, creature, 9, 1, DateTime.UtcNow + TimeSpan.FromDays(7)));
        }
    }

    public class CrushAbility : PetBattleAbility
    {
        public CrushAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Offensive;
            m_Name = "Crush";
            m_Description = "Attempts an immediate attack that if it hits spends all available Defensive Power and increases the hit's damage by 60% per Power spent";
            m_Action = delegate { CrushAbilityAction(creature, m_Name, m_Description); };
        }

        public void CrushAbilityAction(BaseCreature creature, string name, string description)
        {
            Effects.PlaySound(creature.Location, creature.Map, 0x525);

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.ImmediateAttack, creature, 100, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

            BaseCreature bc_Combatant = creature.Combatant as BaseCreature;
            if (bc_Combatant != null)
            {
                if ((creature.GetDefaultWeapon() as BaseWeapon).PetBattlePredetermineCheckHit(creature, bc_Combatant))
                {
                    creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, true);

                    PetBattleAbilities.AddBloodEffect(bc_Combatant, 5, 6);
                    bc_Combatant.FixedEffect(0x5683, 10, 20);
                    
                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.PredeterminedHit, creature, 1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.NextHitSpendDefensePowerForDamageBoost, creature, 0.6, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));
                }

                else
                {
                    creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, false);
                    
                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.PredeterminedMiss, creature, -1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));
                }
            }
            
            creature.PetBattlePostAbilitySwingDelay = creature.NextCombatTime - DateTime.UtcNow;
            creature.NextCombatTime = DateTime.UtcNow;
        }
    }

    public class MaulAbility : PetBattleAbility
    {
        public MaulAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Offensive;
            m_Name = "Maul";
            m_Description = "Attempts an immediate attack that if it hits permanently increases Damage by 3 and opponent loses 1 Defensive Power";
            m_Action = delegate { MaulAbilityAction(creature, m_Name, m_Description); };
        }

        public void MaulAbilityAction(BaseCreature creature, string name, string description)
        {
            Effects.PlaySound(creature.Location, creature.Map, 0x525);

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.ImmediateAttack, creature, 60, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

            BaseCreature bc_Combatant = creature.Combatant as BaseCreature;
            if (bc_Combatant != null)
            {
                if ((creature.GetDefaultWeapon() as BaseWeapon).PetBattlePredetermineCheckHit(creature, bc_Combatant))
                {
                    creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, true);

                    PetBattleAbilities.AddBloodEffect(bc_Combatant, 3, 4);
                    creature.FixedParticles(0x375A, 10, 30, 5017, EffectLayer.Waist);
                    Effects.PlaySound(creature.Location, creature.Map, 0x1EE);
                    //bc_Combatant.FixedEffect(0x5683, 10, 20);
                    
                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.PredeterminedHit, creature, 1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.IncreaseDamage, creature, 3, 1, DateTime.UtcNow + TimeSpan.FromDays(7)));

                    bc_Combatant.m_PetBattleTotem.DefensivePower -= 1;
                    bc_Combatant.PublicOverheadMessage(0, creature.m_PetBattleTeamHue, false, "-1 Defensive Power");
                }

                else
                {
                    creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, false);
                    
                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.PredeterminedMiss, creature, -1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));
                }
            }

            creature.PetBattlePostAbilitySwingDelay = creature.NextCombatTime - DateTime.UtcNow;
            creature.NextCombatTime = DateTime.UtcNow;
        }
    }

    public class RoarAbility : PetBattleAbility
    {
        public RoarAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Offensive;
            m_Name = "Roar";
            m_Description = "Each successful attack over the next 18 seconds reduces opponent’s Attack by 12 and Defense by 12";
            m_Action = delegate { RoarAbilityAction(creature, m_Name, m_Description); };
        }

        public void RoarAbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, false, false);
            
            Effects.PlaySound(creature.Location, creature.Map, 0x525);
            creature.PlaySound(creature.GetAngerSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.SuccessfulAttackReduceOpponentAttack, creature, 12, 0, DateTime.UtcNow + TimeSpan.FromSeconds(18)));

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.SuccessfulAttackReduceOpponentDefense, creature, 12, 0, DateTime.UtcNow + TimeSpan.FromSeconds(18)));
        }
    }

    public class GnawAbility : PetBattleAbility
    {
        public GnawAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Offensive;
            m_Name = "Gnaw";
            m_Description = "Attempts an immediate attack that if it hits permanently reduces opponent’s Armor by 5 and Resist by 10";
            m_Action = delegate { GnawAbilityAction(creature, m_Name, m_Description); };
        }

        public void GnawAbilityAction(BaseCreature creature, string name, string description)
        {
            Effects.PlaySound(creature.Location, creature.Map, 0x525);

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.ImmediateAttack, creature, 20, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

            BaseCreature bc_Combatant = creature.Combatant as BaseCreature;
            if (bc_Combatant != null)
            {
                if ((creature.GetDefaultWeapon() as BaseWeapon).PetBattlePredetermineCheckHit(creature, bc_Combatant))
                {
                    creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, true);

                    PetBattleAbilities.AddBloodEffect(bc_Combatant, 1, 2);
                    bc_Combatant.FixedEffect(0x5683, 20, 20);
                    
                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.PredeterminedHit, creature, 1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

                    bc_Combatant.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.ReduceArmor, creature, 5, 1, DateTime.UtcNow + TimeSpan.FromDays(7)));

                    bc_Combatant.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.ReduceResist, creature, 10, 1, DateTime.UtcNow + TimeSpan.FromDays(7)));
                }

                else
                {
                    creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, false);
                    
                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.PredeterminedMiss, creature, -1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));
                }
            }

            creature.PetBattlePostAbilitySwingDelay = creature.NextCombatTime - DateTime.UtcNow;
            creature.NextCombatTime = DateTime.UtcNow;
        }
    }

    public class ScorchAbility : PetBattleAbility
    {
        public ScorchAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Offensive;
            m_Name = "Scorch";
            m_Description = "Attempts an immediate attack that if it hits also inflicts 30 Magic damage and has a 33% chance to inflict 30 Magic damage as Burning over 30 seconds";
            m_Action = delegate { ScorchAbilityAction(creature, m_Name, m_Description); };
        }

        public void ScorchAbilityAction(BaseCreature creature, string name, string description)
        {
            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.ImmediateAttack, creature, 40, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

            BaseCreature bc_Combatant = creature.Combatant as BaseCreature;
            if (bc_Combatant != null)
            {
                if ((creature.GetDefaultWeapon() as BaseWeapon).PetBattlePredetermineCheckHit(creature, bc_Combatant))
                {
                    creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, true);

                    PetBattleAbilities.AddBloodEffect(bc_Combatant, 3, 4);

                    creature.MovingParticles(bc_Combatant, 0x36D4, 2, 10, false, true, 9502, 4019, 0x160);
                    creature.MovingParticles(bc_Combatant, 0x36D4, 4, 20, false, true, 9502, 4019, 0x160);
                    creature.MovingParticles(bc_Combatant, 0x36D4, 6, 30, false, true, 9502, 4019, 0x160);
                    creature.MovingParticles(bc_Combatant, 0x36D4, 8, 40, false, true, 9502, 4019, 0x160);

                    Effects.PlaySound(creature.Location, creature.Map, 0x227);
                    
                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.PredeterminedHit, creature, 1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.NextHitMagicDamage, creature, 30, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

                    double randomValue = Utility.RandomDouble();

                    if (randomValue <= .33)
                    {
                        Effects.PlaySound(bc_Combatant.Location, bc_Combatant.Map, 0x208);
                        Effects.SendLocationParticles(EffectItem.Create(bc_Combatant.Location, bc_Combatant.Map, TimeSpan.FromSeconds(1.0)), 0x3996, 10, 80, 5029);
                        
                        for (int a = 0; a < 10; a++)
                        {
                            bc_Combatant.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                            (PetBattleAbilityEffect.Burning, creature, 3, 1, DateTime.UtcNow + TimeSpan.FromSeconds((1 + a) * 3)));
                        }
                    }
                }

                else
                {
                    Effects.PlaySound(creature.Location, creature.Map, 0x226);
                    
                    creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, false);
                    
                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.PredeterminedMiss, creature, -1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));
                }
            }

            creature.PetBattlePostAbilitySwingDelay = creature.NextCombatTime - DateTime.UtcNow;
            creature.NextCombatTime = DateTime.UtcNow;
        }
    }

    public class FireblastAbility : PetBattleAbility
    {
        public FireblastAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Offensive;
            m_Name = "Fireblast";
            m_Description = "Attempts an immediate attack that if it hits strikes for an additional 60 Magic damage. If opponent is Burning, they take an additional 30 Magic damage and lose 1 Defensive Power";
            m_Action = delegate { FireblastAbilityAction(creature, m_Name, m_Description); };
        }

        public void FireblastAbilityAction(BaseCreature creature, string name, string description)
        {
            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.ImmediateAttack, creature, 80, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

            BaseCreature bc_Combatant = creature.Combatant as BaseCreature;
            if (bc_Combatant != null)
            {
                if ((creature.GetDefaultWeapon() as BaseWeapon).PetBattlePredetermineCheckHit(creature, bc_Combatant))
                {
                    creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, true);

                    PetBattleAbilities.AddBloodEffect(bc_Combatant, 3, 4);
                    bc_Combatant.FixedParticles(0x36BD, 20, 20, 5044, EffectLayer.Head);
                    Effects.PlaySound(creature.Location, creature.Map, 0x357);
                    
                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.PredeterminedHit, creature, 1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.NextHitMagicDamage, creature, 60, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.NextHitMagicDamageIfBurning, creature, 30, 1, DateTime.UtcNow + TimeSpan.FromDays(7)));

                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.NextHitLoseDefensivePowerIfBurning, creature, 1, 1, DateTime.UtcNow + TimeSpan.FromDays(7)));
                }

                else
                {
                    Effects.PlaySound(creature.Location, creature.Map, 0x226);
                    
                    creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, false);
                    
                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.PredeterminedMiss, creature, -1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));
                }
            }

            creature.PetBattlePostAbilitySwingDelay = creature.NextCombatTime - DateTime.UtcNow;
            creature.NextCombatTime = DateTime.UtcNow;
        }
    }

    public class IgniteAbility : PetBattleAbility
    {
        public IgniteAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Offensive;
            m_Name = "Ignite";
            m_Description = "Attempts an immediate attack that if it hits inflicts 60 Magic damage as Burning over 30 seconds and opponent loses 2 Defensive Power";
            m_Action = delegate { IgniteAbilityAction(creature, m_Name, m_Description); };
        }

        public void IgniteAbilityAction(BaseCreature creature, string name, string description)
        {
            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.ImmediateAttack, creature, 100, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

            BaseCreature bc_Combatant = creature.Combatant as BaseCreature;
            if (bc_Combatant != null)
            {
                if ((creature.GetDefaultWeapon() as BaseWeapon).PetBattlePredetermineCheckHit(creature, bc_Combatant))
                {
                    creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, true);

                    PetBattleAbilities.AddBloodEffect(bc_Combatant, 5, 6);
                    bc_Combatant.FixedParticles(0x3709, 5, 60, 5052, EffectLayer.LeftFoot);
                    Effects.PlaySound(bc_Combatant.Location, creature.Map, 0x4D0);                    
                    
                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.PredeterminedHit, creature, 1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

                    bc_Combatant.m_PetBattleTotem.DefensivePower -= 2;
                    bc_Combatant.PublicOverheadMessage(0, creature.m_PetBattleTeamHue, false, "-2 Defensive Power");

                    Effects.PlaySound(bc_Combatant.Location, bc_Combatant.Map, 0x208);
                    Effects.SendLocationParticles(EffectItem.Create(bc_Combatant.Location, bc_Combatant.Map, TimeSpan.FromSeconds(1.0)), 0x3996, 10, 80, 5029);

                    for (int a = 0; a < 10; a++)
                    {
                        bc_Combatant.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                        (PetBattleAbilityEffect.Burning, creature, 6, 1, DateTime.UtcNow + TimeSpan.FromSeconds((1 + a) * 3)));
                    }
                }

                else
                {
                    Effects.PlaySound(creature.Location, creature.Map, 0x226);
                    
                    creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, false);
                    
                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.PredeterminedMiss, creature, -1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));
                }
            }

            creature.PetBattlePostAbilitySwingDelay = creature.NextCombatTime - DateTime.UtcNow;
            creature.NextCombatTime = DateTime.UtcNow;
        }
    }

    public class CarveAbility : PetBattleAbility
    {
        public CarveAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Offensive;
            m_Name = "Carve";
            m_Description = "Attempts an immediate attack that if it hits inflicts additionally inflicts 50% of damage caused as Bleeding damage over the next 12 seconds and permanently reduces opponent’s Armor by 25";
            m_Action = delegate { CarveAbilityAction(creature, m_Name, m_Description); };
        }

        public void CarveAbilityAction(BaseCreature creature, string name, string description)
        {
            Effects.PlaySound(creature.Location, creature.Map, 0x525);

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.ImmediateAttack, creature, 60, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

            BaseCreature bc_Combatant = creature.Combatant as BaseCreature;
            if (bc_Combatant != null)
            {
                if ((creature.GetDefaultWeapon() as BaseWeapon).PetBattlePredetermineCheckHit(creature, bc_Combatant))
                {
                    creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, true);

                    PetBattleAbilities.AddBloodEffect(bc_Combatant, 5, 6);
                    bc_Combatant.FixedEffect(0x5683, 10, 20);
                    
                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.PredeterminedHit, creature, 1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.CauseBleedPercent, creature, 0.5, 3, DateTime.UtcNow + TimeSpan.FromSeconds(12)));

                    bc_Combatant.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.ReduceArmor, creature, 25, 1, DateTime.UtcNow + TimeSpan.FromDays(7)));
                }

                else
                {
                    creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, false);
                    
                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.PredeterminedMiss, creature, -1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));
                }
            }

            creature.PetBattlePostAbilitySwingDelay = creature.NextCombatTime - DateTime.UtcNow;
            creature.NextCombatTime = DateTime.UtcNow;
        }
    }

    public class BurrowAbility : PetBattleAbility
    {
        public BurrowAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Offensive;
            m_Name = "Burrow";
            m_Description = "Gain 3 Defensive Power, restore 25 Hit Points, and both this creature and opponent are unable to unable to attack, use abilities, or gain Power for 3 seconds";
            m_Action = delegate { BurrowAbilityAction(creature, m_Name, m_Description); };
        }

        public void BurrowAbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, false, false);

            Effects.PlaySound(creature.Location, creature.Map, 0x220);
            creature.PlaySound(creature.GetAngerSound());

            creature.FixedParticles(0x1772, 0, 75, 5021, 0, 0, EffectLayer.CenterFeet);
                        
            creature.Frozen = true;
            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.Frozen, creature, 3, 0, DateTime.UtcNow + TimeSpan.FromSeconds(3)));            

            BaseCreature bc_Combatant = creature.Combatant as BaseCreature;
            if (bc_Combatant != null)
            {
                bc_Combatant.Frozen = true;

                bc_Combatant.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                (PetBattleAbilityEffect.Frozen, creature, 3, 0, DateTime.UtcNow + TimeSpan.FromSeconds(3)));
            }

            creature.m_PetBattleTotem.DefensivePower += 3;
            creature.PublicOverheadMessage(0, creature.m_PetBattleTeamHue, false, "+3 Defensive Power");

            creature.Hits += 25;
            creature.PublicOverheadMessage(0, creature.m_PetBattleTeamHue, false, "+25");
        }
    }

    public class TeardownAbility : PetBattleAbility
    {
        public TeardownAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Offensive;
            m_Name = "Teardown";
            m_Description = "Attempts an immediate attack that if it hits causes an additional 3% damage for each point of Armor opponent is currently reduced by";
            m_Action = delegate { TeardownAbilityAction(creature, m_Name, m_Description); };
        }

        public void TeardownAbilityAction(BaseCreature creature, string name, string description)
        {
            Effects.PlaySound(creature.Location, creature.Map, 0x525);

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.ImmediateAttack, creature, 100, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

            BaseCreature bc_Combatant = creature.Combatant as BaseCreature;
            if (bc_Combatant != null)
            {
                if ((creature.GetDefaultWeapon() as BaseWeapon).PetBattlePredetermineCheckHit(creature, bc_Combatant))
                {
                    creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, true);

                    PetBattleAbilities.AddBloodEffect(bc_Combatant, 4, 8);
                    bc_Combatant.FixedEffect(0x5683, 10, 20);
                    
                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.PredeterminedHit, creature, 1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));

                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.NextHitDamageBoostPercentPerOpponentMissingArmor, creature, 0.03, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));
                }

                else
                {
                    creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Offensive, true, false);
                    
                    creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.PredeterminedMiss, creature, -1000, 0, DateTime.UtcNow + TimeSpan.FromDays(7)));
                }
            }

            creature.PetBattlePostAbilitySwingDelay = creature.NextCombatTime - DateTime.UtcNow;
            creature.NextCombatTime = DateTime.UtcNow;
        }
    }
}
