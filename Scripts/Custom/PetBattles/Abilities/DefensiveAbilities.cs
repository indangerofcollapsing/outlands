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
    public class CleanseAbility : PetBattleAbility
    {
        public CleanseAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Offensive;
            m_Name = "Cleanse";
            m_Description = "Cures Poison, Disease, Bleeding and restores 15 Hit Points";
            m_Action = delegate { CleanseAbilityAction(creature, m_Name, m_Description); };
        }

        public void CleanseAbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Defensive, false, false);
                       
            creature.FixedParticles(0x376A, 10, 40, 5022, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x28E);
            creature.PlaySound(creature.GetIdleSound());            

            if (creature.Poison != null)
                creature.Poison = null;

            creature.Hits += 15;
            creature.PublicOverheadMessage(0, creature.m_PetBattleTeamHue, false, "+15");

            List<PetBattleAbilityEffectEntry> m_EntriesToRemove = new List<PetBattleAbilityEffectEntry>();           

            creature.PetBattleAbilityEntryLookupInProgress = true;

            foreach (PetBattleAbilityEffectEntry entry in creature.m_PetBattleAbilityEffectEntries)
            {
                if (entry.m_PetBattleAbilityEffect == PetBattleAbilityEffect.Burning || entry.m_PetBattleAbilityEffect == PetBattleAbilityEffect.Disease)
                    creature.RemovePetBattleAbilityEffectEntry(entry);
            }

            creature.PetBattleAbilityEntryLookupInProgress = false;
        }
    }

    public class DodgeAbility : PetBattleAbility
    {
        public DodgeAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Defensive;
            m_Name = "Dodge";
            m_Description = "Increase Defense by 12 for 12 seconds and has a 25% chance to gain 1 Defensive Power";
            m_Action = delegate { DodgeAbilityAction(creature, m_Name, m_Description); };
        }

        public void DodgeAbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Defensive, false, false);
                        
            creature.FixedParticles(0x376A, 10, 40, 5022, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x28E);
            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseDefense, creature, 12, 0, DateTime.UtcNow + TimeSpan.FromSeconds(12)));

            double randomValue = Utility.RandomDouble();

            if (randomValue <= .25)
            {
                creature.m_PetBattleTotem.DefensivePower += 1;
                creature.PublicOverheadMessage(0, creature.m_PetBattleTeamHue, false, "+1 Defensive Power");
            }
        }
    }

    public class ResistanceAbility : PetBattleAbility
    {
        public ResistanceAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Defensive;
            m_Name = "Resistance";
            m_Description = "Permanently increase Armor by 10 and Resist by 25";
            m_Action = delegate { ResistanceAbilityAction(creature, m_Name, m_Description); };
        }

        public void ResistanceAbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Defensive, false, false);
                       
            creature.FixedParticles(0x376A, 10, 40, 5022, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x28E);
            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseArmor, creature, 10, 1, DateTime.UtcNow + TimeSpan.FromDays(7)));

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseResist, creature, 25, 1, DateTime.UtcNow + TimeSpan.FromDays(7)));            
        }
    }

    public class EvasionAbility : PetBattleAbility
    {
        public EvasionAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Defensive;
            m_Name = "Evasion";
            m_Description = "Permanently increase Defense by 6";
            m_Action = delegate { EvasionAbilityAction(creature, m_Name, m_Description); };
        }

        public void EvasionAbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Defensive, false, false);
                        
            creature.FixedParticles(0x376A, 10, 40, 5022, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x28E);
            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseDefense, creature, 6, 1, DateTime.UtcNow + TimeSpan.FromDays(7)));
        }
    }

    public class ToxicityAbility : PetBattleAbility
    {
        public ToxicityAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Defensive;
            m_Name = "Toxicity";
            m_Description = "For the next 18 seconds, anytime this creature is hit it inflicts 2% of its maximum health as Magic damage and an additional 1% if opponent is Poisoned";
            m_Action = delegate { ToxicityAbilityAction(creature, m_Name, m_Description); };
        }

        public void ToxicityAbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Defensive, false, false);
                       
            creature.FixedParticles(0x376A, 10, 40, 5022, 0x110, 0, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x28E);
            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.OnHitInflictMagicDamagePercentOfMaxHits, creature, 0.02, 0.1, DateTime.UtcNow + TimeSpan.FromSeconds(18)));
        }
    }

    public class CautionAbility : PetBattleAbility
    {
        public CautionAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Defensive;
            m_Name = "Caution";
            m_Description = "Reduce Speed by 12 and increase Attack by 30 and Defense by 30 for the next 12 seconds";
            m_Action = delegate { CautionAbilityAction(creature, m_Name, m_Description); };
        }

        public void CautionAbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Defensive, false, false);
                       
            creature.FixedParticles(0x376A, 10, 40, 5022, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x28E);
            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.ReduceSpeed, creature, 12, 0, DateTime.UtcNow + TimeSpan.FromSeconds(12)));

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseAttack, creature, 30, 0, DateTime.UtcNow + TimeSpan.FromSeconds(12)));
                        
            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseDefense, creature, 30, 0, DateTime.UtcNow + TimeSpan.FromSeconds(12)));
        }
    }

    public class IronskinAbility : PetBattleAbility
    {
        public IronskinAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Defensive;
            m_Name = "Ironskin";
            m_Description = "Permanently increase Armor by 25";
            m_Action = delegate { IronskinAbilityAction(creature, m_Name, m_Description); };
        }

        public void IronskinAbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Defensive, false, false);
                        
            creature.FixedParticles(0x376A, 10, 40, 5022, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x28E);
            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseArmor, creature, 25, 1, DateTime.UtcNow + TimeSpan.FromDays(7)));
        }
    }

    public class CounterstrikesAbility : PetBattleAbility
    {
        public CounterstrikesAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Defensive;
            m_Name = "Counterstrikes";
            m_Description = "For the next 18 seconds anytime this creature dodges an attack it inflicts 33% of its Damage against its opponent";
            m_Action = delegate { AvoidanceAbilityAction(creature, m_Name, m_Description); };
        }

        public void AvoidanceAbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Defensive, false, false);
                        
            creature.FixedParticles(0x376A, 10, 40, 5022, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x28E);
            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.OnDodgeInflictMagicDamagePercentOfDamage, creature, .33, 0, DateTime.UtcNow + TimeSpan.FromSeconds(18)));
        }
    }

    public class HowlAbility : PetBattleAbility
    {
        public HowlAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Defensive;
            m_Name = "Howl";
            m_Description = "Opponent’s Attack is reduced by 12 for next 12 seconds and has a 25% chance to reduce opponent's Offensive Power by 1";
            m_Action = delegate { HowlAbilityAction(creature, m_Name, m_Description); };
        }

        public void HowlAbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Defensive, false, false);
                        
            creature.FixedParticles(0x376A, 10, 40, 5022, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x28E);
            creature.PlaySound(creature.GetIdleSound());

            BaseCreature bc_Combatant = creature.Combatant as BaseCreature;
            if (bc_Combatant != null)
            {
                bc_Combatant.FixedParticles(0x3779, 10, 15, 5004, EffectLayer.Head);
                bc_Combatant.PlaySound(0x1E4);                
                
                bc_Combatant.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                (PetBattleAbilityEffect.ReduceAttack, creature, 12, 0, DateTime.UtcNow + TimeSpan.FromSeconds(12)));

                double randomValue = Utility.RandomDouble();

                if (randomValue <= .25)
                {
                    bc_Combatant.m_PetBattleTotem.OffensivePower -= 1;
                    bc_Combatant.PublicOverheadMessage(0, creature.m_PetBattleTeamHue, false, "-1 Offensive Power");
                }
            }
        }
    }

    public class ToughnessAbility : PetBattleAbility
    {
        public ToughnessAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Defensive;
            m_Name = "Toughness";
            m_Description = "Spend all Defensive Power to increase Armor by 50 for 12 seconds. Increase Armor by 75 for each additional Defensive Power spent";
            m_Action = delegate { ToughnessAbilityAction(creature, m_Name, m_Description); };
        }

        public void ToughnessAbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Defensive, false, false);
                       
            creature.FixedParticles(0x376A, 10, 40, 5022, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x28E);
            creature.PlaySound(creature.GetIdleSound());

            int remainingDefensivePower = creature.PetBattleTotem.DefensivePower;

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseArmor, creature, 50 + (75 * remainingDefensivePower), 0, DateTime.UtcNow + TimeSpan.FromSeconds(12)));

            if (remainingDefensivePower > 0)
            {
                creature.PetBattleTotem.DefensivePower = 0;
                creature.PublicOverheadMessage(0, creature.m_PetBattleTeamHue, false, "-" + remainingDefensivePower.ToString() + " Defensive Power");
            }
        }
    }

    public class SurvivalAbility : PetBattleAbility
    {
        public SurvivalAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Defensive;
            m_Name = "Survival";
            m_Description = "For the next 18 seconds, anytime this creature is hit it permanently increases Defense by 3 and Armor by 10";
            m_Action = delegate { SurvivalAbilityAction(creature, m_Name, m_Description); };
        }

        public void SurvivalAbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Defensive, false, false);
                        
            creature.FixedParticles(0x376A, 10, 40, 5022, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x28E);
            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.OnHitIncreaseDefense, creature, 3, 1, DateTime.UtcNow + TimeSpan.FromSeconds(18)));

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.OnHitIncreaseArmor, creature, 10, 1, DateTime.UtcNow + TimeSpan.FromSeconds(18)));
        }
    }

    public class GritAbility : PetBattleAbility
    {
        public GritAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Defensive;
            m_Name = "Grit";
            m_Description = "For the next 12 seconds, anytime this creature is hit it increases Damage by 2 and Armor by 15";
            m_Action = delegate { GritAbilityAction(creature, m_Name, m_Description); };
        }

        public void GritAbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Defensive, false, false);
                       
            creature.FixedParticles(0x376A, 10, 40, 5022, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x28E);
            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.OnHitIncreaseDamage, creature, 2, 0, DateTime.UtcNow + TimeSpan.FromSeconds(12)));

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.OnHitIncreaseArmor, creature, 15, 0, DateTime.UtcNow + TimeSpan.FromSeconds(12)));
        }
    }

    public class StaredownAbility : PetBattleAbility
    {
        public StaredownAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Defensive;
            m_Name = "Staredown";
            m_Description = "Reduce opponent’s Attack by 12 for 12 seconds and has a 50% chance of permanently reducing opponent's Attack by 6";
            m_Action = delegate { StaredownAbilityAction(creature, m_Name, m_Description); };
        }

        public void StaredownAbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Defensive, false, false);
                       
            creature.FixedParticles(0x376A, 10, 40, 5022, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x28E);
            creature.PlaySound(creature.GetIdleSound());            

            BaseCreature bc_Combatant = creature.Combatant as BaseCreature;
            if (bc_Combatant != null)
            {
                bc_Combatant.FixedParticles(0x3779, 10, 15, 5004, EffectLayer.Head);
                bc_Combatant.PlaySound(0x1E4);
                
                bc_Combatant.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                (PetBattleAbilityEffect.ReduceAttack, creature, 12, 0, DateTime.UtcNow + TimeSpan.FromSeconds(12)));

                double randomValue = Utility.RandomDouble();

                if (randomValue <= .5)
                {
                    bc_Combatant.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
                    (PetBattleAbilityEffect.ReduceAttack, creature, 6, 1, DateTime.UtcNow + TimeSpan.FromDays(7)));
                }
            }
        }
    }

    public class IgnorePainAbility : PetBattleAbility
    {
        public IgnorePainAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Defensive;
            m_Name = "Ignore Pain";
            m_Description = "Cures Bleeding, Burning, and restores up to 6 Attack and 6 Defense that has been permanently lost";
            m_Action = delegate { IgnorePainAbilityAction(creature, m_Name, m_Description); };
        }

        public void IgnorePainAbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Defensive, false, false);
                       
            creature.FixedParticles(0x376A, 10, 40, 5022, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x28E);
            creature.PlaySound(creature.GetIdleSound());

            if (creature.Poison != null)
                creature.Poison = null;
                        
            double attackCanBeRestored = 6;

            creature.PetBattleAbilityEntryLookupInProgress = true;
                        
            foreach (PetBattleAbilityEffectEntry entry in creature.m_PetBattleAbilityEffectEntries)
            {
                if (entry.m_PetBattleAbilityEffect == PetBattleAbilityEffect.Burning)
                    creature.RemovePetBattleAbilityEffectEntry(entry);

                if (entry.m_PetBattleAbilityEffect == PetBattleAbilityEffect.TakeBleedDamage)
                    creature.RemovePetBattleAbilityEffectEntry(entry);

                if (entry.m_PetBattleAbilityEffect == PetBattleAbilityEffect.ReduceAttack)
                {
                    if (entry.m_Value2 == 1)
                    {
                        if (entry.m_Value2 <= attackCanBeRestored && attackCanBeRestored > 0)
                        {
                            entry.m_Value2 = 0;
                            attackCanBeRestored -= entry.m_Value2;
                            creature.RemovePetBattleAbilityEffectEntry(entry);
                        }

                        else if (entry.m_Value2 > attackCanBeRestored && attackCanBeRestored > 0)
                        {
                            entry.m_Value2 -= attackCanBeRestored;
                            attackCanBeRestored = 0;
                        }
                    }
                }
            }

            double defenseCanBeRestored = 6;

            foreach (PetBattleAbilityEffectEntry entry in creature.m_PetBattleAbilityEffectEntries)
            {
                if (entry.m_PetBattleAbilityEffect == PetBattleAbilityEffect.ReduceDefense)
                {
                    if (entry.m_Value2 == 1)
                    {
                        if (entry.m_Value2 <= defenseCanBeRestored && defenseCanBeRestored > 0)
                        {
                            entry.m_Value2 = 0;
                            defenseCanBeRestored -= entry.m_Value2;
                            creature.RemovePetBattleAbilityEffectEntry(entry);
                        }

                        else if (entry.m_Value2 > defenseCanBeRestored && defenseCanBeRestored > 0)
                        {
                            entry.m_Value2 -= defenseCanBeRestored;
                            defenseCanBeRestored = 0;
                        }
                    }
                }
            }

            creature.PetBattleAbilityEntryLookupInProgress = false;
        }
    }

    public class RageAbility : PetBattleAbility
    {
        public RageAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Defensive;
            m_Name = "Rage";
            m_Description = "Gain 4 Offensive Power";
            m_Action = delegate { RageAbilityAction(creature, m_Name, m_Description); };
        }

        public void RageAbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Defensive, false, false);
                        
            creature.FixedParticles(0x376A, 10, 40, 5022, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x28E);
            creature.PlaySound(creature.GetIdleSound());

            creature.m_PetBattleTotem.OffensivePower += 4;
            creature.PublicOverheadMessage(0, creature.m_PetBattleTeamHue, false, "+4 Offensive Power");
        }
    }

    public class IntimidateAbility : PetBattleAbility
    {
        public IntimidateAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Defensive;
            m_Name = "Intimidate";
            m_Description = "For the next 18 seconds, anytime this creature is hit opponent permanently loses 3 Attack and 1 Damage";
            m_Action = delegate { IntimidateAbilityAction(creature, m_Name, m_Description); };
        }

        public void IntimidateAbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Defensive, false, false);
                      
            creature.FixedParticles(0x376A, 10, 40, 5022, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x28E);
            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.OnHitOpponentReduceAttack, creature, 3, 1, DateTime.UtcNow + TimeSpan.FromSeconds(18)));

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.OnHitOpponentReduceDamage, creature, 1, 1, DateTime.UtcNow + TimeSpan.FromSeconds(18)));
        }
    }

    public class AuraAbility : PetBattleAbility
    {
        public AuraAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Defensive;
            m_Name = "Aura";
            m_Description = "For the next 12 seconds, anytime this creature is hit it has a 50% chance to gain 1 Offensive Power";
            m_Action = delegate { AuraAbilityAction(creature, m_Name, m_Description); };
        }

        public void AuraAbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Defensive, false, false);
                      
            creature.FixedParticles(0x375A, 10, 15, 5037, EffectLayer.Waist);            
            Effects.PlaySound(creature.Location, creature.Map, 0x5aa);

            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.OnHitChanceToIncreaseOffensivePower, creature, .5, 1, DateTime.UtcNow + TimeSpan.FromSeconds(12)));
        }
    }

    public class FireskinAbility : PetBattleAbility
    {
        public FireskinAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Defensive;
            m_Name = "Fireskin";
            m_Description = "For the next 18 seconds, anytime this creature is hit it inflicts 33% of its Damage as Magic damage";
            m_Action = delegate { FireskinAbilityAction(creature, m_Name, m_Description); };
        }

        public void FireskinAbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Defensive, false, false);

            creature.FixedParticles(0x376A, 9, 32, 5008, EffectLayer.Waist);
            Effects.SendLocationParticles(EffectItem.Create(creature.Location, creature.Map, TimeSpan.FromSeconds(1.0)), 0x3996, 10, 20, 5029);

            Effects.PlaySound(creature.Location, creature.Map, 0x5CA);
            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.OnHitInflictMagicDamagePercentOfDamage, creature, 0.33, 0, DateTime.UtcNow + TimeSpan.FromSeconds(18)));
        }
    }

    public class StonewallAbility : PetBattleAbility
    {
        public StonewallAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Defensive;
            m_Name = "Stonewall";
            m_Description = "For the next 18 seconds, anytime this creature is hit it permanently increases Armor by 25";
            m_Action = delegate { StonewallAbilityAction(creature, m_Name, m_Description); };
        }

        public void StonewallAbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Defensive, false, false);
                        
            creature.FixedParticles(0x376A, 10, 40, 5022, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x28E);
            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.OnHitIncreaseArmor, creature, 25, 1, DateTime.UtcNow + TimeSpan.FromSeconds(18)));
        }
    }
}
