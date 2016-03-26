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
    public class AICombatHealOther
    {
        public static bool CanDoCombatHealOther(BaseCreature creature)
        {
            if (creature.IsBarded())
                return false;

            if (creature.DictCombatAction[CombatAction.CombatHealOther] > 0)
            {
                if (DateTime.UtcNow > creature.NextCombatHealActionAllowed && !creature.BardProvoked && !creature.BardPacified)
                {   
                    int TotalValues = 0;

                    Dictionary<CombatHealOther, int> DictTemp = new Dictionary<CombatHealOther, int>();

                    if (AICombatHealOther.CanDoCombatHealOtherSpellHealOther(creature, 100)) { DictTemp.Add(CombatHealOther.SpellHealOther100, creature.DictCombatHealOther[CombatHealOther.SpellHealOther100]); }
                    if (AICombatHealOther.CanDoCombatHealOtherSpellHealOther(creature, 75)) { DictTemp.Add(CombatHealOther.SpellHealOther75, creature.DictCombatHealOther[CombatHealOther.SpellHealOther75]); }
                    if (AICombatHealOther.CanDoCombatHealOtherSpellHealOther(creature, 50)) { DictTemp.Add(CombatHealOther.SpellHealOther50, creature.DictCombatHealOther[CombatHealOther.SpellHealOther50]); }
                    if (AICombatHealOther.CanDoCombatHealOtherSpellHealOther(creature, 25)) { DictTemp.Add(CombatHealOther.SpellHealOther25, creature.DictCombatHealOther[CombatHealOther.SpellHealOther25]); }
                    if (AICombatHealOther.CanDoCombatHealOtherSpellCureOther(creature)) { DictTemp.Add(CombatHealOther.SpellCureOther, creature.DictCombatHealOther[CombatHealOther.SpellCureOther]); }
                    if (AICombatHealOther.CanDoCombatHealOtherBandageHealOther(creature, 100)) { DictTemp.Add(CombatHealOther.BandageHealOther100, creature.DictCombatHealOther[CombatHealOther.BandageHealOther100]); }
                    if (AICombatHealOther.CanDoCombatHealOtherBandageHealOther(creature, 75)) { DictTemp.Add(CombatHealOther.BandageHealOther75, creature.DictCombatHealOther[CombatHealOther.BandageHealOther75]); }
                    if (AICombatHealOther.CanDoCombatHealOtherBandageHealOther(creature, 50)) { DictTemp.Add(CombatHealOther.BandageHealOther50, creature.DictCombatHealOther[CombatHealOther.BandageHealOther50]); }
                    if (AICombatHealOther.CanDoCombatHealOtherBandageHealOther(creature, 25)) { DictTemp.Add(CombatHealOther.BandageHealOther25, creature.DictCombatHealOther[CombatHealOther.BandageHealOther25]); }
                    if (AICombatHealOther.CanDoCombatHealOtherBandageCureOther(creature)) { DictTemp.Add(CombatHealOther.BandageCureOther, creature.DictCombatHealOther[CombatHealOther.BandageCureOther]); }

                    //Calculate Total Values
                    foreach (KeyValuePair<CombatHealOther, int> pair in DictTemp)
                    {
                        TotalValues += pair.Value;
                    }

                    if (TotalValues > 0)
                        return true;
                }
            }

            return false;
        }

        public static bool DoCombatHealOther(BaseCreature creature)
        {   
            CombatHealOther healAction = CombatHealOther.None;

            int TotalValues = 0;

            Dictionary<CombatHealOther, int> DictTemp = new Dictionary<CombatHealOther, int>();

            if (AICombatHealOther.CanDoCombatHealOtherSpellHealOther(creature, 100)) { DictTemp.Add(CombatHealOther.SpellHealOther100, creature.DictCombatHealOther[CombatHealOther.SpellHealOther100]); }
            if (AICombatHealOther.CanDoCombatHealOtherSpellHealOther(creature, 75)) { DictTemp.Add(CombatHealOther.SpellHealOther75, creature.DictCombatHealOther[CombatHealOther.SpellHealOther75]); }
            if (AICombatHealOther.CanDoCombatHealOtherSpellHealOther(creature, 50)) { DictTemp.Add(CombatHealOther.SpellHealOther50, creature.DictCombatHealOther[CombatHealOther.SpellHealOther50]); }
            if (AICombatHealOther.CanDoCombatHealOtherSpellHealOther(creature, 25)) { DictTemp.Add(CombatHealOther.SpellHealOther25, creature.DictCombatHealOther[CombatHealOther.SpellHealOther25]); }
            if (AICombatHealOther.CanDoCombatHealOtherSpellCureOther(creature)) { DictTemp.Add(CombatHealOther.SpellCureOther, creature.DictCombatHealOther[CombatHealOther.SpellCureOther]); }
            if (AICombatHealOther.CanDoCombatHealOtherBandageHealOther(creature, 100)) { DictTemp.Add(CombatHealOther.BandageHealOther100, creature.DictCombatHealOther[CombatHealOther.BandageHealOther100]); }
            if (AICombatHealOther.CanDoCombatHealOtherBandageHealOther(creature, 75)) { DictTemp.Add(CombatHealOther.BandageHealOther75, creature.DictCombatHealOther[CombatHealOther.BandageHealOther75]); }
            if (AICombatHealOther.CanDoCombatHealOtherBandageHealOther(creature, 50)) { DictTemp.Add(CombatHealOther.BandageHealOther50, creature.DictCombatHealOther[CombatHealOther.BandageHealOther50]); }
            if (AICombatHealOther.CanDoCombatHealOtherBandageHealOther(creature, 25)) { DictTemp.Add(CombatHealOther.BandageHealOther25, creature.DictCombatHealOther[CombatHealOther.BandageHealOther25]); }
            if (AICombatHealOther.CanDoCombatHealOtherBandageCureOther(creature)) { DictTemp.Add(CombatHealOther.BandageCureOther, creature.DictCombatHealOther[CombatHealOther.BandageCureOther]); }

            //Calculate Total Values
            foreach (KeyValuePair<CombatHealOther, int> pair in DictTemp)
            {
                TotalValues += pair.Value;
            }

            double ActionCheck = Utility.RandomDouble();
            double CumulativeAmount = 0.0;
            double AdditionalAmount = 0.0;

            //Determine CombatAction                      
            foreach (KeyValuePair<CombatHealOther, int> pair in DictTemp)
            {
                AdditionalAmount = (double)pair.Value / (double)TotalValues;

                if (ActionCheck >= CumulativeAmount && ActionCheck < (CumulativeAmount + AdditionalAmount))
                {
                    healAction = pair.Key;

                    //Spell Heal Other
                    if (healAction == CombatHealOther.SpellHealOther100)
                        AIHeal.DoSpellHealOther(creature, 100);

                    if (healAction == CombatHealOther.SpellHealOther75)
                        AIHeal.DoSpellHealOther(creature, 75);

                    if (healAction == CombatHealOther.SpellHealOther50)
                        AIHeal.DoSpellHealOther(creature, 50);

                    if (healAction == CombatHealOther.SpellHealOther25)
                        AIHeal.DoSpellHealOther(creature, 25);

                    if (healAction == CombatHealOther.SpellCureOther)
                        AIHeal.DoSpellCureOther(creature);

                    //Bandage Heal Other                   
                    if (healAction == CombatHealOther.BandageHealOther100)
                        AIHeal.DoBandageHealOther(creature, 100);

                    if (healAction == CombatHealOther.BandageHealOther75)
                        AIHeal.DoBandageHealOther(creature, 75);

                    if (healAction == CombatHealOther.BandageHealOther50)
                        AIHeal.DoBandageHealOther(creature, 50);

                    if (healAction == CombatHealOther.BandageHealOther25)
                        AIHeal.DoBandageHealOther(creature, 25);

                    if (healAction == CombatHealOther.BandageCureOther)
                        AIHeal.DoBandageCureOther(creature);

                    creature.NextCombatHealActionAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(creature.CombatHealActionMinDelay, creature.CombatHealActionMaxDelay));

                    if (creature.AcquireNewTargetEveryCombatAction)
                        creature.m_NextAcquireTargetAllowed = DateTime.UtcNow;

                    return true;
                }

                CumulativeAmount += AdditionalAmount;
            }

            return false;
        }
        
        //-------------
        
        public static bool CanDoCombatHealOtherSpellHealOther(BaseCreature creature, int percent)
        {
            if (creature.NextCombatHealActionAllowed > DateTime.UtcNow)
                return false;

            if (DateTime.UtcNow < creature.NextSpellTime)
                return false;

            if (creature.Mana < 11)
                return false;

            switch (percent)
            {
                case 100: if (creature.DictCombatHealOther[CombatHealOther.SpellHealOther100] <= 0) return false; break;
                case 75: if (creature.DictCombatHealOther[CombatHealOther.SpellHealOther75] <= 0) return false; break;
                case 50: if (creature.DictCombatHealOther[CombatHealOther.SpellHealOther50] <= 0) return false; break;
                case 25: if (creature.DictCombatHealOther[CombatHealOther.SpellHealOther25] <= 0)  return false; break;
            }

            IPooledEnumerable eable = creature.Map.GetMobilesInRange(creature.Location, creature.RangePerception);

            foreach (Mobile target in eable)
            {
                if (!AIHeal.IsTargetValidHealTarget(creature, target, true))
                    continue;

                if (AIHeal.DoesTargetNeedHealing(target, percent))
                {
                    eable.Free();
                    return true;
                }
            }

            eable.Free();

            return false;
        }

        public static bool CanDoCombatHealOtherSpellCureOther(BaseCreature creature)
        {
            if (creature.NextCombatHealActionAllowed > DateTime.UtcNow)
                return false;

            if (DateTime.UtcNow < creature.NextSpellTime)
                return false;

            if (creature.Mana < 6)
                return false;

            if (creature.DictCombatHealOther[CombatHealOther.SpellCureOther] <= 0)
                return false;

            IPooledEnumerable eable = creature.Map.GetMobilesInRange(creature.Location, creature.RangePerception);

            foreach (Mobile target in eable)
            {
                if (!AIHeal.IsTargetValidHealTarget(creature, target, true))
                    continue;

                if (target.Poison != null)
                {
                    eable.Free();
                    return true;
                }
            }

            eable.Free();

            return false;
        }

        public static bool CanDoCombatHealOtherBandageHealOther(BaseCreature creature, int percent)
        {
            if (creature.NextCombatHealActionAllowed > DateTime.UtcNow)
                return false;

            if (creature.DoingBandage)
                return false;

            switch (percent)
            {
                case 100: if (creature.DictCombatHealOther[CombatHealOther.BandageHealOther100] <= 0) return false; break;
                case 75: if (creature.DictCombatHealOther[CombatHealOther.BandageHealOther75] <= 0) return false; break;
                case 50: if (creature.DictCombatHealOther[CombatHealOther.BandageHealOther50] <= 0) return false; break;
                case 25: if (creature.DictCombatHealOther[CombatHealOther.BandageHealOther25] <= 0) return false; break;
            }

            IPooledEnumerable eable = creature.Map.GetMobilesInRange(creature.Location, creature.RangePerception);

            foreach (Mobile target in eable)
            {
                if (!AIHeal.IsTargetValidHealTarget(creature, target, true))
                    continue;

                if (AIHeal.DoesTargetNeedHealing(target, percent))
                {
                    eable.Free();
                    return true;
                }
            }

            eable.Free();

            return false;
        }

        public static bool CanDoCombatHealOtherBandageCureOther(BaseCreature creature)
        {
            if (creature.NextCombatHealActionAllowed > DateTime.UtcNow)
                return false;

            if (creature.DoingBandage)
                return false;

            if (creature.DictCombatHealOther[CombatHealOther.BandageCureOther] <= 0)
                return false;

            IPooledEnumerable eable = creature.Map.GetMobilesInRange(creature.Location, creature.RangePerception);

            foreach (Mobile target in eable)
            {
                if (!AIHeal.IsTargetValidHealTarget(creature, target, true))
                    continue;

                if (target.Poison != null)
                {
                    eable.Free();
                    return true;
                }
            }

            eable.Free();

            return false;
        }
    }
}
