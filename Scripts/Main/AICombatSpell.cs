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
using MoveImpl = Server.Movement.MovementImpl;
using Server.Custom;

namespace Server.Mobiles
{
    public class AICombatSpell
    {
        public static bool CanDoCombatSpell(BaseCreature creature)
        {
            if (creature.DictCombatAction[CombatAction.CombatSpell] > 0)
            {
                if (DateTime.UtcNow > creature.NextSpellTime)
                {
                    Dictionary<CombatSpell, int> DictTemp = new Dictionary<CombatSpell, int>();

                    if (AICombatSpell.CanDoCombatSpellSpellDamage1(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellDamage1, creature.DictCombatSpell[CombatSpell.SpellDamage1]); }
                    if (AICombatSpell.CanDoCombatSpellSpellDamage2(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellDamage2, creature.DictCombatSpell[CombatSpell.SpellDamage2]); }
                    if (AICombatSpell.CanDoCombatSpellSpellDamage3(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellDamage3, creature.DictCombatSpell[CombatSpell.SpellDamage3]); }
                    if (AICombatSpell.CanDoCombatSpellSpellDamage4(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellDamage4, creature.DictCombatSpell[CombatSpell.SpellDamage4]); }
                    if (AICombatSpell.CanDoCombatSpellSpellDamage5(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellDamage5, creature.DictCombatSpell[CombatSpell.SpellDamage5]); }
                    if (AICombatSpell.CanDoCombatSpellSpellDamage6(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellDamage6, creature.DictCombatSpell[CombatSpell.SpellDamage6]); }
                    if (AICombatSpell.CanDoCombatSpellSpellDamage7(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellDamage7, creature.DictCombatSpell[CombatSpell.SpellDamage7]); }
                    if (AICombatSpell.CanDoCombatSpellSpellDamageAOE7(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellDamageAOE7, creature.DictCombatSpell[CombatSpell.SpellDamageAOE7]); }
                    if (AICombatSpell.CanDoCombatSpellSpellPoison(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellPoison, creature.DictCombatSpell[CombatSpell.SpellPoison]); }
                    if (AICombatSpell.CanDoCombatSpellSpellNegative1to3(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellNegative1to3, creature.DictCombatSpell[CombatSpell.SpellNegative1to3]); }
                    if (AICombatSpell.CanDoCombatSpellSpellNegative4to7(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellNegative4to7, creature.DictCombatSpell[CombatSpell.SpellNegative4to7]); }
                    if (AICombatSpell.CanDoCombatSpellSpellSummon5(creature)) { DictTemp.Add(CombatSpell.SpellSummon5, creature.DictCombatSpell[CombatSpell.SpellSummon5]); }
                    if (AICombatSpell.CanDoCombatSpellSpellSummon8(creature)) { DictTemp.Add(CombatSpell.SpellSummon8, creature.DictCombatSpell[CombatSpell.SpellSummon8]); }
                    if (AICombatSpell.CanDoCombatSpellSpellDispelSummon(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellDispelSummon, creature.DictCombatSpell[CombatSpell.SpellDispelSummon]); }
                    if (AICombatSpell.CanDoCombatSpellSpellHarmfulField(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellHarmfulField, creature.DictCombatSpell[CombatSpell.SpellHarmfulField]); }
                    if (AICombatSpell.CanDoCombatSpellSpellNegativeField(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellNegativeField, creature.DictCombatSpell[CombatSpell.SpellNegativeField]); }
                    if (AICombatSpell.CanDoCombatSpellSpellBeneficial1to2(creature, creature)) { DictTemp.Add(CombatSpell.SpellBeneficial1to2, creature.DictCombatSpell[CombatSpell.SpellBeneficial1to2]); }
                    if (AICombatSpell.CanDoCombatSpellSpellBeneficial3to5(creature, creature)) { DictTemp.Add(CombatSpell.SpellBeneficial3to5, creature.DictCombatSpell[CombatSpell.SpellBeneficial3to5]); }

                    int TotalValues = 0;

                    //Calculate Total Values
                    foreach (KeyValuePair<CombatSpell, int> pair in DictTemp)
                    {
                        TotalValues += pair.Value;
                    }

                    if (TotalValues > 0)
                        return true;
                }
            }

            return false;
        }

        public static bool DoCombatSpell(BaseCreature creature)
        {
            if (creature.Combatant == null)
                return false;           

            Dictionary<CombatSpell, int> DictTemp = new Dictionary<CombatSpell, int>();

            if (AICombatSpell.CanDoCombatSpellSpellDamage1(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellDamage1, creature.DictCombatSpell[CombatSpell.SpellDamage1]); }
            if (AICombatSpell.CanDoCombatSpellSpellDamage2(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellDamage2, creature.DictCombatSpell[CombatSpell.SpellDamage2]); }
            if (AICombatSpell.CanDoCombatSpellSpellDamage3(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellDamage3, creature.DictCombatSpell[CombatSpell.SpellDamage3]); }
            if (AICombatSpell.CanDoCombatSpellSpellDamage4(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellDamage4, creature.DictCombatSpell[CombatSpell.SpellDamage4]); }
            if (AICombatSpell.CanDoCombatSpellSpellDamage5(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellDamage5, creature.DictCombatSpell[CombatSpell.SpellDamage5]); }
            if (AICombatSpell.CanDoCombatSpellSpellDamage6(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellDamage6, creature.DictCombatSpell[CombatSpell.SpellDamage6]); }
            if (AICombatSpell.CanDoCombatSpellSpellDamage7(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellDamage7, creature.DictCombatSpell[CombatSpell.SpellDamage7]); }
            if (AICombatSpell.CanDoCombatSpellSpellDamageAOE7(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellDamageAOE7, creature.DictCombatSpell[CombatSpell.SpellDamageAOE7]); }
            if (AICombatSpell.CanDoCombatSpellSpellPoison(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellPoison, creature.DictCombatSpell[CombatSpell.SpellPoison]); }
            if (AICombatSpell.CanDoCombatSpellSpellNegative1to3(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellNegative1to3, creature.DictCombatSpell[CombatSpell.SpellNegative1to3]); }
            if (AICombatSpell.CanDoCombatSpellSpellNegative4to7(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellNegative4to7, creature.DictCombatSpell[CombatSpell.SpellNegative4to7]); }
            if (AICombatSpell.CanDoCombatSpellSpellSummon5(creature)) { DictTemp.Add(CombatSpell.SpellSummon5, creature.DictCombatSpell[CombatSpell.SpellSummon5]); }
            if (AICombatSpell.CanDoCombatSpellSpellSummon8(creature)) { DictTemp.Add(CombatSpell.SpellSummon8, creature.DictCombatSpell[CombatSpell.SpellSummon8]); }
            if (AICombatSpell.CanDoCombatSpellSpellDispelSummon(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellDispelSummon, creature.DictCombatSpell[CombatSpell.SpellDispelSummon]); }
            if (AICombatSpell.CanDoCombatSpellSpellHarmfulField(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellHarmfulField, creature.DictCombatSpell[CombatSpell.SpellHarmfulField]); }
            if (AICombatSpell.CanDoCombatSpellSpellNegativeField(creature, creature.Combatant)) { DictTemp.Add(CombatSpell.SpellNegativeField, creature.DictCombatSpell[CombatSpell.SpellNegativeField]); }
            if (AICombatSpell.CanDoCombatSpellSpellBeneficial1to2(creature, creature)) { DictTemp.Add(CombatSpell.SpellBeneficial1to2, creature.DictCombatSpell[CombatSpell.SpellBeneficial1to2]); }
            if (AICombatSpell.CanDoCombatSpellSpellBeneficial3to5(creature, creature)) { DictTemp.Add(CombatSpell.SpellBeneficial3to5, creature.DictCombatSpell[CombatSpell.SpellBeneficial3to5]); }

            int TotalValues = 0;

            //Calculate Total Values
            foreach (KeyValuePair<CombatSpell, int> pair in DictTemp)
            {
                TotalValues += pair.Value;
            }

            double ActionCheck = Utility.RandomDouble();

            double CumulativeAmount = 0.0;
            double AdditionalAmount = 0.0;

            Spell selectedSpell = null;

            //Determine CombatSpell                      
            foreach (KeyValuePair<CombatSpell, int> pair in DictTemp)
            {
                AdditionalAmount = (double)pair.Value / (double)TotalValues;

                if (ActionCheck >= CumulativeAmount && ActionCheck < (CumulativeAmount + AdditionalAmount))
                {
                    CombatSpell combatSpell = pair.Key;
                    Spell spell = null;

                    switch (combatSpell)
                    {
                        case CombatSpell.SpellDamage1: spell = AICombatSpell.GetSpellDamage1(creature, creature.Combatant); break;
                        case CombatSpell.SpellDamage2: spell = AICombatSpell.GetSpellDamage2(creature, creature.Combatant); break;
                        case CombatSpell.SpellDamage3: spell = AICombatSpell.GetSpellDamage3(creature, creature.Combatant); break;
                        case CombatSpell.SpellDamage4: spell = AICombatSpell.GetSpellDamage4(creature, creature.Combatant); break;
                        case CombatSpell.SpellDamage5: spell = AICombatSpell.GetSpellDamage5(creature, creature.Combatant); break;
                        case CombatSpell.SpellDamage6: spell = AICombatSpell.GetSpellDamage6(creature, creature.Combatant); break;
                        case CombatSpell.SpellDamage7: spell = AICombatSpell.GetSpellDamage7(creature, creature.Combatant); break;
                        case CombatSpell.SpellDamageAOE7: spell = AICombatSpell.GetSpellDamageAOE7(creature, creature.Combatant); break;
                        case CombatSpell.SpellPoison: spell = AICombatSpell.GetSpellPoison(creature, creature.Combatant); break;
                        case CombatSpell.SpellNegative1to3: spell = AICombatSpell.GetSpellNegative1to3(creature, creature.Combatant); break;
                        case CombatSpell.SpellNegative4to7: spell = AICombatSpell.GetSpellNegative4to7(creature, creature.Combatant); break;
                        case CombatSpell.SpellSummon5: spell = AICombatSpell.GetSpellSummon5(creature); break;
                        case CombatSpell.SpellSummon8: spell = AICombatSpell.GetSpellSummon8(creature); break;
                        case CombatSpell.SpellDispelSummon: spell = AICombatSpell.GetSpellDispelSummon(creature, creature.Combatant); break;
                        case CombatSpell.SpellHarmfulField: spell = AICombatSpell.GetSpellHarmfulField(creature, creature.Combatant); break;
                        case CombatSpell.SpellNegativeField: spell = AICombatSpell.GetSpellNegativeField(creature, creature.Combatant); break;
                        case CombatSpell.SpellBeneficial1to2: spell = AICombatSpell.GetSpellBeneficial1to2(creature, creature); break;
                        case CombatSpell.SpellBeneficial3to5: spell = AICombatSpell.GetSpellBeneficial3to5(creature, creature); break;
                    }

                    if (spell != null)
                    {
                        if (creature.AcquireNewTargetEveryCombatAction)
                            creature.m_NextAcquireTargetAllowed = DateTime.UtcNow;

                        if (creature is UOACZBaseUndead)
                        {
                            UOACZBaseUndead uoaczBaseUndead = creature as UOACZBaseUndead;
                            uoaczBaseUndead.m_LastActivity = DateTime.UtcNow;
                        }

                        spell.Cast();                                               

                        return true;
                    }

                    break;
                }

                CumulativeAmount += AdditionalAmount;
            }

            return false;
        }

        //----------------
        
        public static bool CanDoCombatSpellSpellDamage1(BaseCreature creature, Mobile target)
        {
            if (target == null)
                return false;

            if (DateTime.UtcNow < creature.NextSpellTime)
                return false;

            if (creature.DictCombatSpell[CombatSpell.SpellDamage1] > 0)
            {
                if (creature.InLOS(target) && SpellInDefaultRange(creature, target) && creature.Mana >= 4)
                    return true;
            }

            return false;
        }

        public static bool CanDoCombatSpellSpellDamage2(BaseCreature creature, Mobile target)
        {
            if (target == null)
                return false;

            if (DateTime.UtcNow < creature.NextSpellTime)
                return false;

            if (creature.DictCombatSpell[CombatSpell.SpellDamage2] > 0)
            {
                if (creature.InLOS(target) && SpellInDefaultRange(creature, target) && creature.Mana >= 6)
                    return true;
            }

            return false;
        }

        public static bool CanDoCombatSpellSpellDamage3(BaseCreature creature, Mobile target)
        {
            if (target == null)
                return false;

            if (DateTime.UtcNow < creature.NextSpellTime)
                return false;

            if (creature.DictCombatSpell[CombatSpell.SpellDamage3] > 0)
            {
                if (creature.InLOS(target) && SpellInDefaultRange(creature, target) && creature.Mana >= 9)
                    return true;
            }

            return false;
        }

        public static bool CanDoCombatSpellSpellDamage4(BaseCreature creature, Mobile target)
        {
            if (target == null)
                return false;

            if (DateTime.UtcNow < creature.NextSpellTime)
                return false;

            if (creature.DictCombatSpell[CombatSpell.SpellDamage4] > 0)
            {
                if (creature.InLOS(target) && SpellInDefaultRange(creature, target) && creature.Mana >= 11)
                    return true;
            }

            return false;
        }

        public static bool CanDoCombatSpellSpellDamage5(BaseCreature creature, Mobile target)
        {
            if (target == null)
                return false;

            if (DateTime.UtcNow < creature.NextSpellTime)
                return false;

            if (creature.DictCombatSpell[CombatSpell.SpellDamage5] > 0)
            {
                if (creature.InLOS(target) && SpellInDefaultRange(creature, target) && creature.Mana >= 14)
                    return true;
            }

            return false;
        }

        public static bool CanDoCombatSpellSpellDamage6(BaseCreature creature, Mobile target)
        {
            if (target == null)
                return false;

            if (DateTime.UtcNow < creature.NextSpellTime)
                return false;

            if (creature.DictCombatSpell[CombatSpell.SpellDamage6] > 0)
            {
                if (creature.InLOS(target) && SpellInDefaultRange(creature, target) && creature.Mana >= 20)
                    return true;
            }

            return false;
        }

        public static bool CanDoCombatSpellSpellDamage7(BaseCreature creature, Mobile target)
        {
            if (target == null)
                return false;

            if (DateTime.UtcNow < creature.NextSpellTime)
                return false;

            if (creature.DictCombatSpell[CombatSpell.SpellDamage7] > 0)
            {
                if (creature.InLOS(target) && SpellInDefaultRange(creature, target) && creature.Mana >= 40)
                    return true;
            }

            return false;
        }

        public static bool CanDoCombatSpellSpellDamageAOE7(BaseCreature creature, Mobile target)
        {
            if (target == null)
                return false;

            if (DateTime.UtcNow < creature.NextSpellTime)
                return false;

            if (creature.DictCombatSpell[CombatSpell.SpellDamageAOE7] > 0)
            {
                if (creature.InLOS(target) && SpellInDefaultRange(creature, target) && creature.Mana >= 40)
                    return true;
            }

            return false;
        }

        public static bool CanDoCombatSpellSpellPoison(BaseCreature creature, Mobile target)
        {
            if (target == null)
                return false;

            if (DateTime.UtcNow < creature.NextSpellTime)
                return false;

            if (creature.Region is UOACZRegion)
                return false;

            if (creature.DictCombatSpell[CombatSpell.SpellPoison] > 0)
            {
                if (creature.InLOS(target) && SpellInDefaultRange(creature, target) && creature.Mana >= 9)
                {
                    BaseCreature bc_Target = target as BaseCreature;

                    if (bc_Target != null)
                    {
                        if (bc_Target.PoisonImmune != null)
                            return false;
                    }

                    if (target.Poison == null)
                        return true;
                }
            }

            return false;
        }

        public static bool CanDoCombatSpellSpellNegative1to3(BaseCreature creature, Mobile target)
        {
            if (target == null)
                return false;

            if (DateTime.UtcNow < creature.NextSpellTime)
                return false;

            if (creature.DictCombatSpell[CombatSpell.SpellNegative1to3] > 0)
            {
                if (creature.InLOS(target) && SpellInDefaultRange(creature, target) && creature.Mana >= 9)
                    return true;
            }

            return false;
        }

        public static bool CanDoCombatSpellSpellNegative4to7(BaseCreature creature, Mobile target)
        {
            if (target == null)
                return false;

            if (DateTime.UtcNow < creature.NextSpellTime)
                return false;

            if (creature.DictCombatSpell[CombatSpell.SpellNegative4to7] > 0)
            {
                if (creature.InLOS(target) && SpellInDefaultRange(creature, target) && creature.Mana >= 40)
                    return true;
            }

            return false;
        }

        public static bool CanDoCombatSpellSpellSummon5(BaseCreature creature)
        {
            if (DateTime.UtcNow < creature.NextSpellTime)
                return false;

            if (creature.DictCombatSpell[CombatSpell.SpellSummon5] > 0)
            {
                if (creature.Mana >= 15 && creature.ControlSlots <= 4)
                    return true;
            }

            return false;
        }

        public static bool CanDoCombatSpellSpellSummon8(BaseCreature creature)
        {
            if (DateTime.UtcNow < creature.NextSpellTime)
                return false;

            if (creature.DictCombatSpell[CombatSpell.SpellSummon8] > 0)
            {
                if (creature.Mana >= 50 && creature.ControlSlots <= 3)
                    return true;
            }

            return false;
        }

        public static bool CanDoCombatSpellSpellDispelSummon(BaseCreature creature, Mobile target)
        {
            if (target == null)
                return false;

            if (DateTime.UtcNow < creature.NextSpellTime)
                return false;

            bool summoned = false;

            BaseCreature bc_Target = target as BaseCreature;

            if (bc_Target != null)
            {
                if (bc_Target.Summoned)
                    summoned = true;
            }

            if (!summoned)
                return false;

            if (creature.DictCombatSpell[CombatSpell.SpellDispelSummon] > 0)
            {
                if (creature.InLOS(target) && SpellInDefaultRange(creature, target) && creature.Mana >= 20)
                    return true;
            }

            return false;
        }

        public static bool CanDoCombatSpellSpellHarmfulField(BaseCreature creature, Mobile target)
        {
            if (target == null)
                return false;

            if (DateTime.UtcNow < creature.NextSpellTime)
                return false;

            if (creature.DictCombatSpell[CombatSpell.SpellHarmfulField] > 0)
            {
                if (creature.InLOS(target) && SpellInDefaultRange(creature, target) && creature.Mana >= 14)
                    return true;
            }

            return false;
        }

        public static bool CanDoCombatSpellSpellNegativeField(BaseCreature creature, Mobile target)
        {
            if (target == null)
                return false;

            if (DateTime.UtcNow < creature.NextSpellTime)
                return false;

            if (creature.DictCombatSpell[CombatSpell.SpellNegativeField] > 0)
            {
                if (creature.InLOS(target) && SpellInDefaultRange(creature, target) && creature.Mana >= 40)
                    return true;
            }

            return false;
        }

        public static bool CanDoCombatSpellSpellBeneficial1to2(BaseCreature creature, Mobile target)
        {
            if (target == null)
                return false;

            if (DateTime.UtcNow < creature.NextSpellTime)
                return false;

            if (creature.DictCombatSpell[CombatSpell.SpellBeneficial1to2] > 0)
            {
                if (creature.InLOS(target) && SpellInDefaultRange(creature, target) && creature.Mana >= 6)
                {
                    if (target.GetStatMod("[Magic] Str Offset") == null)
                        return true;

                    if (target.GetStatMod("[Magic] Dex Offset") == null)
                        return true;

                    if (target.GetStatMod("[Magic] Int Offset") == null)
                        return true;

                    if (Spells.Second.ProtectionSpell.Registry.Contains(target) == false)
                        return true;

                    if (target.MeleeDamageAbsorb <= 0)
                        return true;
                }
            }

            return false;
        }

        public static bool CanDoCombatSpellSpellBeneficial3to5(BaseCreature creature, Mobile target)
        {
            if (target == null)
                return false;

            if (DateTime.UtcNow < creature.NextSpellTime)
                return false;

            if (creature.DictCombatSpell[CombatSpell.SpellBeneficial3to5] > 0)
            {
                if (creature.InLOS(target) && SpellInDefaultRange(creature, target) && creature.Mana >= 14)
                {
                    if (target.GetStatMod("[Magic] Str Offset") == null || target.GetStatMod("[Magic] Dex Offset") == null || target.GetStatMod("[Magic] Int Offset") == null)
                        return true;

                    if (target.MagicDamageAbsorb <= 0)
                        return true;
                }
            }

            return false;
        }
               
        public static Spell GetSpellDamage1(BaseCreature creature, Mobile target)
        {
            Spell spell = null;

            if (creature == null || target == null)
                return spell;

            creature.SpellTarget = target;

            List<Spell> spells = new List<Spell>();

            spells.Add(new MagicArrowSpell(creature, null));

            return spells[Utility.RandomMinMax(0, spells.Count - 1)];
        }

        public static Spell GetSpellDamage2(BaseCreature creature, Mobile target)
        {
            Spell spell = null;

            if (creature == null || target == null)
                return spell;

            creature.SpellTarget = target;

            List<Spell> spells = new List<Spell>();

            spells.Add(new HarmSpell(creature, null));

            return spells[Utility.RandomMinMax(0, spells.Count - 1)];
        }

        public static Spell GetSpellDamage3(BaseCreature creature, Mobile target)
        {
            Spell spell = null;

            if (creature == null || target == null)
                return spell;

            creature.SpellTarget = target;

            List<Spell> spells = new List<Spell>();

            spells.Add(new FireballSpell(creature, null));

            return spells[Utility.RandomMinMax(0, spells.Count - 1)];
        }

        public static Spell GetSpellDamage4(BaseCreature creature, Mobile target)
        {
            Spell spell = null;

            if (creature == null || target == null)
                return spell;

            creature.SpellTarget = target;

            List<Spell> spells = new List<Spell>();

            spells.Add(new LightningSpell(creature, null));

            return spells[Utility.RandomMinMax(0, spells.Count - 1)];
        }

        public static Spell GetSpellDamage5(BaseCreature creature, Mobile target)
        {
            Spell spell = null;

            if (creature == null || target == null)
                return spell;

            creature.SpellTarget = target;

            List<Spell> spells = new List<Spell>();

            spells.Add(new MindBlastSpell(creature, null));

            return spells[Utility.RandomMinMax(0, spells.Count - 1)];
        }

        public static Spell GetSpellDamage6(BaseCreature creature, Mobile target)
        {
            Spell spell = null;

            if (creature == null || target == null)
                return spell;

            creature.SpellTarget = target;

            List<Spell> spells = new List<Spell>();

            if (!creature.CastOnlyFireSpells)
                spells.Add(new EnergyBoltSpell(creature, null));

            if (!creature.CastOnlyEnergySpells)
                spells.Add(new ExplosionSpell(creature, null));

            return spells[Utility.RandomMinMax(0, spells.Count - 1)];
        }

        public static Spell GetSpellDamage7(BaseCreature creature, Mobile target)
        {
            Spell spell = null;

            if (creature == null || target == null)
                return spell;

            creature.SpellTarget = target;

            List<Spell> spells = new List<Spell>();

            spells.Add(new FlameStrikeSpell(creature, null));

            return spells[Utility.RandomMinMax(0, spells.Count - 1)];
        }

        public static Spell GetSpellDamageAOE7(BaseCreature creature, Mobile target)
        {
            Spell spell = null;

            if (creature == null || target == null)
                return spell;

            creature.SpellTarget = target;

            List<Spell> spells = new List<Spell>();

            if (!creature.CastOnlyFireSpells)
                spells.Add(new ChainLightningSpell(creature, null));

            if (!creature.CastOnlyEnergySpells)
                spells.Add(new MeteorSwarmSpell(creature, null));

            return spells[Utility.RandomMinMax(0, spells.Count - 1)];
        }

        public static Spell GetSpellPoison(BaseCreature creature, Mobile target)
        {
            Spell spell = null;

            if (creature == null || target == null)
                return spell;

            creature.SpellTarget = target;

            List<Spell> spells = new List<Spell>();

            spells.Add(new PoisonSpell(creature, null));

            return spells[Utility.RandomMinMax(0, spells.Count - 1)];
        }

        public static Spell GetSpellNegative1to3(BaseCreature creature, Mobile target)
        {
            Spell spell = null;

            if (creature == null || target == null)
                return spell;

            creature.SpellTarget = target;

            List<Spell> spells = new List<Spell>();

            spells.Add(new ClumsySpell(creature, null));
            spells.Add(new FeeblemindSpell(creature, null));
            spells.Add(new WeakenSpell(creature, null));

            return spells[Utility.RandomMinMax(0, spells.Count - 1)];
        }

        public static Spell GetSpellNegative4to7(BaseCreature creature, Mobile target)
        {
            Spell spell = null;

            if (creature == null || target == null)
                return spell;

            creature.SpellTarget = target;

            List<Spell> spells = new List<Spell>();

            spells.Add(new CurseSpell(creature, null));
            spells.Add(new ManaDrainSpell(creature, null));
            spells.Add(new ParalyzeSpell(creature, null));

            return spells[Utility.RandomMinMax(0, spells.Count - 1)];
        }

        public static Spell GetSpellSummon5(BaseCreature creature)
        {
            Spell spell = null;

            if (creature == null)
                return spell;

            creature.SpellTarget = creature;

            List<Spell> spells = new List<Spell>();

            spells.Add(new SummonCreatureSpell(creature, null));

            return spells[Utility.RandomMinMax(0, spells.Count - 1)];
        }

        public static Spell GetSpellSummon8(BaseCreature creature)
        {
            Spell spell = null;

            if (creature == null)
                return spell;

            creature.SpellTarget = creature;

            List<Spell> spells = new List<Spell>();

            spells.Add(new AirElementalSpell(creature, null));
            spells.Add(new EarthElementalSpell(creature, null));
            spells.Add(new FireElementalSpell(creature, null));
            spells.Add(new WaterElementalSpell(creature, null));
            spells.Add(new SummonDaemonSpell(creature, null));

            return spells[Utility.RandomMinMax(0, spells.Count - 1)];
        }

        public static Spell GetSpellDispelSummon(BaseCreature creature, Mobile target)
        {
            Spell spell = null;

            if (creature == null || target == null)
                return spell;

            creature.SpellTarget = target;

            List<Spell> spells = new List<Spell>();

            spells.Add(new DispelSpell(creature, null));

            return spells[Utility.RandomMinMax(0, spells.Count - 1)];
        }

        public static Spell GetSpellHarmfulField(BaseCreature creature, Mobile target)
        {
            Spell spell = null;

            if (creature == null || target == null)
                return spell;

            creature.SpellTarget = target;

            List<Spell> spells = new List<Spell>();

            spells.Add(new FireFieldSpell(creature, null));
            spells.Add(new PoisonFieldSpell(creature, null));

            return spells[Utility.RandomMinMax(0, spells.Count - 1)];
        }

        public static Spell GetSpellNegativeField(BaseCreature creature, Mobile target)
        {
            Spell spell = null;

            if (creature == null || target == null)
                return spell;

            creature.SpellTarget = target;

            List<Spell> spells = new List<Spell>();

            spells.Add(new WallOfStoneSpell(creature, null));
            spells.Add(new ParalyzeFieldSpell(creature, null));
            spells.Add(new EnergyFieldSpell(creature, null));

            return spells[Utility.RandomMinMax(0, spells.Count - 1)];
        }

        public static Spell GetSpellBeneficial1to2(BaseCreature creature, Mobile target)
        {
            Spell spell = null;

            if (creature == null || target == null)
                return spell;

            creature.SpellTarget = target;

            List<Spell> m_ValidSpells = new List<Spell>();

            if (target.GetStatMod("[Magic] Str Offset") == null)
                m_ValidSpells.Add(new StrengthSpell(creature, null));

            if (target.GetStatMod("[Magic] Dex Offset") == null)
                m_ValidSpells.Add(new AgilitySpell(creature, null));

            if (target.GetStatMod("[Magic] Int Offset") == null)
                m_ValidSpells.Add(new CunningSpell(creature, null));

            if (Spells.Second.ProtectionSpell.Registry.Contains(target) == false)
                m_ValidSpells.Add(new ProtectionSpell(creature, null));

            if (target.MeleeDamageAbsorb <= 0)
                m_ValidSpells.Add(new ReactiveArmorSpell(creature, null));

            if (m_ValidSpells.Count > 0)
                return m_ValidSpells[Utility.RandomMinMax(0, m_ValidSpells.Count - 1)];

            return spell;
        }

        public static Spell GetSpellBeneficial3to5(BaseCreature creature, Mobile target)
        {
            Spell spell = null;

            if (creature == null || target == null)
                return spell;

            creature.SpellTarget = target;

            List<Spell> m_ValidSpells = new List<Spell>();

            if (target.GetStatMod("[Magic] Str Offset") == null || target.GetStatMod("[Magic] Dex Offset") == null || target.GetStatMod("[Magic] Int Offset") == null)
                m_ValidSpells.Add(new BlessSpell(creature, null));

            if (target.MagicDamageAbsorb <= 0)
                m_ValidSpells.Add(new MagicReflectSpell(creature, null));

            if (m_ValidSpells.Count > 0)
                return m_ValidSpells[Utility.RandomMinMax(0, m_ValidSpells.Count - 1)];

            return spell;
        }

        //---------------------

        public static bool SpellInDefaultRange(BaseCreature creature, Mobile target)
        {
            //Return False If Null Target
            if (target == null)
                return false;

            if (creature.GetDistanceToSqrt(target) <= creature.CreatureSpellCastRange)
                return true;

            return false;
        }

        public static bool CreatureHasCastingAI(BaseCreature creature)
        {
            if (creature == null)
                return false;

            bool hasCastingAI = false;

            foreach (KeyValuePair<CombatSpell, int> pair in creature.DictCombatSpell)
            {
                if (pair.Value > 0)
                {
                    hasCastingAI = true;
                    break;
                }
            }

            if (creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] > 0 || creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] > 0 || creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] > 0 || creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] > 0 || creature.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] > 0)
                hasCastingAI = true;

            if (creature.DictCombatHealOther[CombatHealOther.SpellHealOther100] > 0 || creature.DictCombatHealOther[CombatHealOther.SpellHealOther75] > 0 || creature.DictCombatHealOther[CombatHealOther.SpellHealOther50] > 0 || creature.DictCombatHealOther[CombatHealOther.SpellHealOther25] > 0 || creature.DictCombatHealOther[CombatHealOther.SpellCureOther] > 0)
                hasCastingAI = true;

            return hasCastingAI;
        }
    }
}
