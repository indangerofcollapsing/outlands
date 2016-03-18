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
    public class PetBattleOpportunityAbilities
    {        
        public static void UseOpportunityAbility(BaseCreature creature, int level)
        {
            //Level 1 Opportunity Abilities
            List<PetBattleAbility> m_Level1OpportunityAbilities = new List<PetBattleAbility>();

            m_Level1OpportunityAbilities.Add(new HealingAAbility(creature));
            m_Level1OpportunityAbilities.Add(new HealingBAbility(creature));

            m_Level1OpportunityAbilities.Add(new RestorePowerA_Ability(creature));
            m_Level1OpportunityAbilities.Add(new RestorePowerB_Ability(creature));

            m_Level1OpportunityAbilities.Add(new IncreaseDamageA_Ability(creature));
            m_Level1OpportunityAbilities.Add(new IncreaseAttackA_Ability(creature));
            m_Level1OpportunityAbilities.Add(new IncreaseDefenseA_Ability(creature));
            m_Level1OpportunityAbilities.Add(new IncreaseSpeedA_Ability(creature));
            m_Level1OpportunityAbilities.Add(new IncreaseArmorA_Ability(creature));
            m_Level1OpportunityAbilities.Add(new IncreaseResistA_Ability(creature));

            m_Level1OpportunityAbilities.Add(new IncreaseDamageB_Ability(creature));
            m_Level1OpportunityAbilities.Add(new IncreaseAttackB_Ability(creature));
            m_Level1OpportunityAbilities.Add(new IncreaseDefenseB_Ability(creature));
            m_Level1OpportunityAbilities.Add(new IncreaseSpeedB_Ability(creature));
            m_Level1OpportunityAbilities.Add(new IncreaseArmorB_Ability(creature));
            m_Level1OpportunityAbilities.Add(new IncreaseResistB_Ability(creature));

            m_Level1OpportunityAbilities.Add(new IncreaseDamageD_Ability(creature));
            m_Level1OpportunityAbilities.Add(new IncreaseAttackD_Ability(creature));
            m_Level1OpportunityAbilities.Add(new IncreaseDefenseD_Ability(creature));
            m_Level1OpportunityAbilities.Add(new IncreaseSpeedD_Ability(creature));
            m_Level1OpportunityAbilities.Add(new IncreaseArmorD_Ability(creature));
            m_Level1OpportunityAbilities.Add(new IncreaseResistD_Ability(creature));
            
            //Determine Ability
            switch (level)
            {
                case 1:
                    m_Level1OpportunityAbilities[Utility.RandomMinMax(0, m_Level1OpportunityAbilities.Count - 1)].m_Action.Invoke();
                break;
            }
        }
    }
    
    public class HealingAAbility : PetBattleAbility
    {
        public HealingAAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Offensive;
            m_Name = "Minor Healing";
            m_Description = "Restores 5% of creature’s maximum Hit Points";
            m_Action = delegate { HealingAAbilityAction(creature, m_Name, m_Description); };
        }

        public void HealingAAbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Opportunity, false, false);
            
            creature.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);
            creature.FixedParticles(0x376A, 10, 40, 5022, EffectLayer.LeftFoot);
            Effects.PlaySound(creature.Location, creature.Map, 0x28E);
            creature.PlaySound(creature.GetIdleSound());

            int healingAmount = (int)((double)creature.HitsMax * .05);

            creature.Hits += healingAmount;
            creature.PublicOverheadMessage(0, creature.m_PetBattleTeamHue, false, "+" + healingAmount.ToString());
        }
    }

    public class HealingBAbility : PetBattleAbility
    {
        public HealingBAbility(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Offensive;
            m_Name = "Moderate Healing";
            m_Description = "Restores 10% of creature’s maximum Hit Points";
            m_Action = delegate { HealingBAbilityAction(creature, m_Name, m_Description); };
        }

        public void HealingBAbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Opportunity, false, false);

            creature.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);
            creature.FixedParticles(0x376A, 10, 40, 5022, EffectLayer.LeftFoot);
            Effects.PlaySound(creature.Location, creature.Map, 0x28E);
            creature.PlaySound(creature.GetIdleSound());

            int healingAmount = (int)((double)creature.HitsMax * .10);

            creature.Hits += healingAmount;
            creature.PublicOverheadMessage(0, creature.m_PetBattleTeamHue, false, "+" + healingAmount.ToString());
        }
    }

    public class RestorePowerA_Ability : PetBattleAbility
    {
        public RestorePowerA_Ability(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Offensive;
            m_Name = "Moderate Power Restoration";
            m_Description = "2 Offensive and 2 Defensive Restored";
            m_Action = delegate { RestorePowerA_AbilityAction(creature, m_Name, m_Description); };
        }

        public void RestorePowerA_AbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Opportunity, false, false);
            
            creature.FixedEffect(0x5683, 10, 20);
            Effects.PlaySound(creature.Location, creature.Map, 0x525);
            creature.PlaySound(creature.GetAngerSound());

            creature.m_PetBattleTotem.OffensivePower += 2;
            creature.m_PetBattleTotem.DefensivePower += 2;

            creature.PublicOverheadMessage(0, creature.m_PetBattleTeamHue, false, "+2 Offensive and +2 Defensive Power");
        }
    }

    public class RestorePowerB_Ability : PetBattleAbility
    {
        public RestorePowerB_Ability(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Offensive;
            m_Name = "Major Power Restoration";
            m_Description = "4 Offensive and 4 Defensive Restored";
            m_Action = delegate { RestorePowerB_AbilityAction(creature, m_Name, m_Description); };
        }

        public void RestorePowerB_AbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Opportunity, false, false);

            creature.FixedEffect(0x5683, 10, 20);
            Effects.PlaySound(creature.Location, creature.Map, 0x525);
            creature.PlaySound(creature.GetAngerSound());

            creature.m_PetBattleTotem.OffensivePower += 4;
            creature.m_PetBattleTotem.DefensivePower += 4;

            creature.PublicOverheadMessage(0, creature.m_PetBattleTeamHue, false, "+4 Offensive and +4 Defensive Power");
        }
    }

    public class IncreaseDamageA_Ability : PetBattleAbility
    {
        public IncreaseDamageA_Ability(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Opportunity;
            m_Name = "Extreme Damage Increase";
            m_Description = "Increase Damage by 18 for 6 seconds";
            m_Action = delegate { IncreaseDamageA_AbilityAction(creature, m_Name, m_Description); };
        }

        public void IncreaseDamageA_AbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Opportunity, false, false);

            //Effects & Sounds
            creature.FixedParticles(0x375A, 9, 40, 5027, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x5BC);
            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseDamage, creature, 18, 0, DateTime.UtcNow + TimeSpan.FromSeconds(6)));
        }
    }

    public class IncreaseAttackA_Ability : PetBattleAbility
    {
        public IncreaseAttackA_Ability(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Opportunity;
            m_Name = "Extreme Attack Increase";
            m_Description = "Increase Attack by 36 for 6 seconds";
            m_Action = delegate { IncreaseAttackA_AbilityAction(creature, m_Name, m_Description); };
        }

        public void IncreaseAttackA_AbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Opportunity, false, false);

            //Effects & Sounds
            creature.FixedParticles(0x375A, 9, 40, 5027, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x5BC);
            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseAttack, creature, 36, 0, DateTime.UtcNow + TimeSpan.FromSeconds(6)));
        }
    }

    public class IncreaseDefenseA_Ability : PetBattleAbility
    {
        public IncreaseDefenseA_Ability(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Opportunity;
            m_Name = "Extreme Defense Increase";
            m_Description = "Increase Defense by 36 for 6 seconds";
            m_Action = delegate { IncreaseDefenseA_AbilityAction(creature, m_Name, m_Description); };
        }

        public void IncreaseDefenseA_AbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Opportunity, false, false);

            //Effects & Sounds
            creature.FixedParticles(0x375A, 9, 40, 5027, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x5BC);
            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseDefense, creature, 36, 0, DateTime.UtcNow + TimeSpan.FromSeconds(6)));
        }
    }

    public class IncreaseSpeedA_Ability : PetBattleAbility
    {
        public IncreaseSpeedA_Ability(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Opportunity;
            m_Name = "Extreme Speed Increase";
            m_Description = "Increase Speed by 24 for 6 seconds";
            m_Action = delegate { IncreaseSpeedA_AbilityAction(creature, m_Name, m_Description); };
        }

        public void IncreaseSpeedA_AbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Opportunity, false, false);

            //Effects & Sounds
            creature.FixedParticles(0x375A, 9, 40, 5027, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x5BC);
            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseSpeed, creature, 24, 0, DateTime.UtcNow + TimeSpan.FromSeconds(6)));
        }
    }

    public class IncreaseArmorA_Ability : PetBattleAbility
    {
        public IncreaseArmorA_Ability(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Opportunity;
            m_Name = "Extreme Armor Increase";
            m_Description = "Increase Armor by 150 for 6 seconds";
            m_Action = delegate { IncreaseArmorA_AbilityAction(creature, m_Name, m_Description); };
        }

        public void IncreaseArmorA_AbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Opportunity, false, false);

            //Effects & Sounds
            creature.FixedParticles(0x375A, 9, 40, 5027, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x5BC);
            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseArmor, creature, 150, 0, DateTime.UtcNow + TimeSpan.FromSeconds(6)));
        }
    }

    public class IncreaseResistA_Ability : PetBattleAbility
    {
        public IncreaseResistA_Ability(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Opportunity;
            m_Name = "Extreme Resist Increase";
            m_Description = "Increase Resist by 150 for 6 seconds";
            m_Action = delegate { IncreaseResistA_AbilityAction(creature, m_Name, m_Description); };
        }

        public void IncreaseResistA_AbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Opportunity, false, false);

            //Effects & Sounds
            creature.FixedParticles(0x375A, 9, 40, 5027, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x5BC);
            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseResist, creature, 150, 0, DateTime.UtcNow + TimeSpan.FromSeconds(6)));
        }
    }
    
    //Version B
    public class IncreaseDamageB_Ability : PetBattleAbility
    {
        public IncreaseDamageB_Ability(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Opportunity;
            m_Name = "Greater Damage Increase";
            m_Description = "Increase Damage by 9 for 18 seconds";
            m_Action = delegate { IncreaseDamageB_AbilityAction(creature, m_Name, m_Description); };
        }

        public void IncreaseDamageB_AbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Opportunity, false, false);

            //Effects & Sounds
            creature.FixedParticles(0x375A, 9, 40, 5027, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x5BC);
            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseDamage, creature, 9, 0, DateTime.UtcNow + TimeSpan.FromSeconds(18)));
        }
    }

    public class IncreaseAttackB_Ability : PetBattleAbility
    {
        public IncreaseAttackB_Ability(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Opportunity;
            m_Name = "Greater Attack Increase";
            m_Description = "Increase Attack by 18 for 18 seconds";
            m_Action = delegate { IncreaseAttackB_AbilityAction(creature, m_Name, m_Description); };
        }

        public void IncreaseAttackB_AbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Opportunity, false, false);

            //Effects & Sounds
            creature.FixedParticles(0x375A, 9, 40, 5027, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x5BC);
            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseAttack, creature, 18, 0, DateTime.UtcNow + TimeSpan.FromSeconds(18)));
        }
    }

    public class IncreaseDefenseB_Ability : PetBattleAbility
    {
        public IncreaseDefenseB_Ability(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Opportunity;
            m_Name = "Greater Defense Increase";
            m_Description = "Increase Defense by 18 for 18 seconds";
            m_Action = delegate { IncreaseDefenseB_AbilityAction(creature, m_Name, m_Description); };
        }

        public void IncreaseDefenseB_AbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Opportunity, false, false);

            //Effects & Sounds
            creature.FixedParticles(0x375A, 9, 40, 5027, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x5BC);
            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseDefense, creature, 18, 0, DateTime.UtcNow + TimeSpan.FromSeconds(18)));
        }
    }

    public class IncreaseSpeedB_Ability : PetBattleAbility
    {
        public IncreaseSpeedB_Ability(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Opportunity;
            m_Name = "Greater Speed Increase";
            m_Description = "Increase Speed by 12 for 18 seconds";
            m_Action = delegate { IncreaseSpeedB_AbilityAction(creature, m_Name, m_Description); };
        }

        public void IncreaseSpeedB_AbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Opportunity, false, false);

            //Effects & Sounds
            creature.FixedParticles(0x375A, 9, 40, 5027, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x5BC);
            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseSpeed, creature, 12, 0, DateTime.UtcNow + TimeSpan.FromSeconds(18)));
        }
    }

    public class IncreaseArmorB_Ability : PetBattleAbility
    {
        public IncreaseArmorB_Ability(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Opportunity;
            m_Name = "Greater Armor Increase";
            m_Description = "Increase Armor by 75 for 18 seconds";
            m_Action = delegate { IncreaseArmorB_AbilityAction(creature, m_Name, m_Description); };
        }

        public void IncreaseArmorB_AbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Opportunity, false, false);

            //Effects & Sounds
            creature.FixedParticles(0x375A, 9, 40, 5027, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x5BC);
            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseArmor, creature, 75, 0, DateTime.UtcNow + TimeSpan.FromSeconds(18)));
        }
    }

    public class IncreaseResistB_Ability : PetBattleAbility
    {
        public IncreaseResistB_Ability(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Opportunity;
            m_Name = "Greater Resist Increase";
            m_Description = "Increase Resist by 75 for 18 seconds";
            m_Action = delegate { IncreaseResistB_AbilityAction(creature, m_Name, m_Description); };
        }

        public void IncreaseResistB_AbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Opportunity, false, false);

            //Effects & Sounds
            creature.FixedParticles(0x375A, 9, 40, 5027, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x5BC);
            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseResist, creature, 150, 0, DateTime.UtcNow + TimeSpan.FromSeconds(18)));
        }
    }

    //Version C
    public class IncreaseDamageC_Ability : PetBattleAbility
    {
        public IncreaseDamageC_Ability(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Opportunity;
            m_Name = "Moderate Damage Increase";
            m_Description = "Increase Damage by 6 for 30 seconds";
            m_Action = delegate { IncreaseDamageC_AbilityAction(creature, m_Name, m_Description); };
        }

        public void IncreaseDamageC_AbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Opportunity, false, false);

            //Effects & Sounds
            creature.FixedParticles(0x375A, 9, 40, 5027, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x5BC);
            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseDamage, creature, 6, 0, DateTime.UtcNow + TimeSpan.FromSeconds(30)));
        }
    }

    public class IncreaseAttackC_Ability : PetBattleAbility
    {
        public IncreaseAttackC_Ability(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Opportunity;
            m_Name = "Moderate Attack Increase";
            m_Description = "Increase Attack by 12 for 30 seconds";
            m_Action = delegate { IncreaseAttackC_AbilityAction(creature, m_Name, m_Description); };
        }

        public void IncreaseAttackC_AbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Opportunity, false, false);

            //Effects & Sounds
            creature.FixedParticles(0x375A, 9, 40, 5027, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x5BC);
            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseAttack, creature, 12, 0, DateTime.UtcNow + TimeSpan.FromSeconds(30)));
        }
    }

    public class IncreaseDefenseC_Ability : PetBattleAbility
    {
        public IncreaseDefenseC_Ability(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Opportunity;
            m_Name = "Moderate Defense Increase";
            m_Description = "Increase Defense by 12 for 30 seconds";
            m_Action = delegate { IncreaseDefenseC_AbilityAction(creature, m_Name, m_Description); };
        }

        public void IncreaseDefenseC_AbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Opportunity, false, false);

            //Effects & Sounds
            creature.FixedParticles(0x375A, 9, 40, 5027, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x5BC);
            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseDefense, creature, 12, 0, DateTime.UtcNow + TimeSpan.FromSeconds(30)));
        }
    }

    public class IncreaseSpeedC_Ability : PetBattleAbility
    {
        public IncreaseSpeedC_Ability(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Opportunity;
            m_Name = "Moderate Speed Increase";
            m_Description = "Increase Speed by 8 for 30 seconds";
            m_Action = delegate { IncreaseSpeedC_AbilityAction(creature, m_Name, m_Description); };
        }

        public void IncreaseSpeedC_AbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Opportunity, false, false);

            //Effects & Sounds
            creature.FixedParticles(0x375A, 9, 40, 5027, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x5BC);
            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseSpeed, creature, 8, 0, DateTime.UtcNow + TimeSpan.FromSeconds(30)));
        }
    }

    public class IncreaseArmorC_Ability : PetBattleAbility
    {
        public IncreaseArmorC_Ability(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Opportunity;
            m_Name = "Moderate Armor Increase";
            m_Description = "Increase Armor by 50 for 30 seconds";
            m_Action = delegate { IncreaseArmorC_AbilityAction(creature, m_Name, m_Description); };
        }

        public void IncreaseArmorC_AbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Opportunity, false, false);

            //Effects & Sounds
            creature.FixedParticles(0x375A, 9, 40, 5027, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x5BC);
            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseArmor, creature, 50, 0, DateTime.UtcNow + TimeSpan.FromSeconds(30)));
        }
    }

    public class IncreaseResistC_Ability : PetBattleAbility
    {
        public IncreaseResistC_Ability(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Opportunity;
            m_Name = "Moderate Resist Increase";
            m_Description = "Increase Resist by 50 for 30 seconds";
            m_Action = delegate { IncreaseResistC_AbilityAction(creature, m_Name, m_Description); };
        }

        public void IncreaseResistC_AbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Opportunity, false, false);

            //Effects & Sounds
            creature.FixedParticles(0x375A, 9, 40, 5027, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x5BC);
            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseResist, creature, 50, 0, DateTime.UtcNow + TimeSpan.FromSeconds(30)));
        }
    }

    //Version D
    public class IncreaseDamageD_Ability : PetBattleAbility
    {
        public IncreaseDamageD_Ability(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Opportunity;
            m_Name = "Minor Damage Increase";
            m_Description = "Permanently increase Damage by 3";
            m_Action = delegate { IncreaseDamageD_AbilityAction(creature, m_Name, m_Description); };
        }

        public void IncreaseDamageD_AbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Opportunity, false, false);

            //Effects & Sounds
            creature.FixedParticles(0x375A, 9, 40, 5027, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x5BC);
            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseDamage, creature, 3, 1, DateTime.UtcNow + TimeSpan.FromDays(7)));
        }
    }

    public class IncreaseAttackD_Ability : PetBattleAbility
    {
        public IncreaseAttackD_Ability(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Opportunity;
            m_Name = "Minor Attack Increase";
            m_Description = "Permanently increase Attack by 6";
            m_Action = delegate { IncreaseAttackD_AbilityAction(creature, m_Name, m_Description); };
        }

        public void IncreaseAttackD_AbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Opportunity, false, false);

            //Effects & Sounds
            creature.FixedParticles(0x375A, 9, 40, 5027, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x5BC);
            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseAttack, creature, 6, 1, DateTime.UtcNow + TimeSpan.FromDays(7)));
        }
    }

    public class IncreaseDefenseD_Ability : PetBattleAbility
    {
        public IncreaseDefenseD_Ability(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Opportunity;
            m_Name = "Minor Defense Increase";
            m_Description = "Permanently increase Defense by 6";
            m_Action = delegate { IncreaseDefenseD_AbilityAction(creature, m_Name, m_Description); };
        }

        public void IncreaseDefenseD_AbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Opportunity, false, false);

            //Effects & Sounds
            creature.FixedParticles(0x375A, 9, 40, 5027, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x5BC);
            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseDefense, creature, 6, 1, DateTime.UtcNow + TimeSpan.FromDays(7)));
        }
    }

    public class IncreaseSpeedD_Ability : PetBattleAbility
    {
        public IncreaseSpeedD_Ability(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Opportunity;
            m_Name = "Minor Speed Increase";
            m_Description = "Permanently increase Speed by 4";
            m_Action = delegate { IncreaseSpeedD_AbilityAction(creature, m_Name, m_Description); };
        }

        public void IncreaseSpeedD_AbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Opportunity, false, false);

            //Effects & Sounds
            creature.FixedParticles(0x375A, 9, 40, 5027, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x5BC);
            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseSpeed, creature, 4, 1, DateTime.UtcNow + TimeSpan.FromDays(7)));
        }
    }

    public class IncreaseArmorD_Ability : PetBattleAbility
    {
        public IncreaseArmorD_Ability(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Opportunity;
            m_Name = "Minor Armor Increase";
            m_Description = "Permanently increase Armor by 25";
            m_Action = delegate { IncreaseArmorD_AbilityAction(creature, m_Name, m_Description); };
        }

        public void IncreaseArmorD_AbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Opportunity, false, false);

            //Effects & Sounds
            creature.FixedParticles(0x375A, 9, 40, 5027, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x5BC);
            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseArmor, creature, 25, 1, DateTime.UtcNow + TimeSpan.FromDays(7)));
        }
    }

    public class IncreaseResistD_Ability : PetBattleAbility
    {
        public IncreaseResistD_Ability(BaseCreature creature)
        {
            m_Creature = creature;

            m_Type = PetAbilityType.Opportunity;
            m_Name = "Minor Resist Increase";
            m_Description = "Permanently increase Resist by 25";
            m_Action = delegate { IncreaseResistD_AbilityAction(creature, m_Name, m_Description); };
        }

        public void IncreaseResistD_AbilityAction(BaseCreature creature, string name, string description)
        {
            creature.m_PetBattle.AnnounceAbility(creature, name, description, PetAbilityType.Opportunity, false, false);

            //Effects & Sounds
            creature.FixedParticles(0x375A, 9, 40, 5027, EffectLayer.Waist);
            Effects.PlaySound(creature.Location, creature.Map, 0x5BC);
            creature.PlaySound(creature.GetIdleSound());

            creature.AddPetBattleAbilityEffectEntry(new PetBattleAbilityEffectEntry
            (PetBattleAbilityEffect.IncreaseResist, creature, 25, 1, DateTime.UtcNow + TimeSpan.FromDays(7)));
        }
    }
}
