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
    public class AICombatHealSelf
    {
        public static bool CanDoCombatHealSelf(BaseCreature creature)
        {
            if (creature.IsBarded())
                return false;

            if (creature.DictCombatAction[CombatAction.CombatHealSelf] > 0)
            {
                if (DateTime.UtcNow > creature.NextCombatHealActionAllowed)
                {
                    Dictionary<CombatHealSelf, int> DictTemp = new Dictionary<CombatHealSelf, int>();

                    if (AICombatHealSelf.CanDoCombatHealSelfSpellHealSelf(creature, 100)) { DictTemp.Add(CombatHealSelf.SpellHealSelf100, creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf100]); }
                    if (AICombatHealSelf.CanDoCombatHealSelfSpellHealSelf(creature, 75)) { DictTemp.Add(CombatHealSelf.SpellHealSelf75, creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75]); }
                    if (AICombatHealSelf.CanDoCombatHealSelfSpellHealSelf(creature, 50)) { DictTemp.Add(CombatHealSelf.SpellHealSelf50, creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50]); }
                    if (AICombatHealSelf.CanDoCombatHealSelfSpellHealSelf(creature, 25)) { DictTemp.Add(CombatHealSelf.SpellHealSelf25, creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25]); }
                    if (AICombatHealSelf.CanDoCombatHealSelfSpellCureSelf(creature)) { DictTemp.Add(CombatHealSelf.SpellCureSelf, creature.DictCombatHealSelf[CombatHealSelf.SpellCureSelf]); }
                    if (AICombatHealSelf.CanDoCombatHealSelfPotionHealSelf(creature, 100)) { DictTemp.Add(CombatHealSelf.PotionHealSelf100, creature.DictCombatHealSelf[CombatHealSelf.PotionHealSelf100]); }
                    if (AICombatHealSelf.CanDoCombatHealSelfPotionHealSelf(creature, 75)) { DictTemp.Add(CombatHealSelf.PotionHealSelf75, creature.DictCombatHealSelf[CombatHealSelf.PotionHealSelf75]); }
                    if (AICombatHealSelf.CanDoCombatHealSelfPotionHealSelf(creature, 50)) { DictTemp.Add(CombatHealSelf.PotionHealSelf50, creature.DictCombatHealSelf[CombatHealSelf.PotionHealSelf50]); }
                    if (AICombatHealSelf.CanDoCombatHealSelfPotionHealSelf(creature, 25)) { DictTemp.Add(CombatHealSelf.PotionHealSelf25, creature.DictCombatHealSelf[CombatHealSelf.PotionHealSelf25]); }
                    if (AICombatHealSelf.CanDoCombatHealSelfPotionCureSelf(creature)) { DictTemp.Add(CombatHealSelf.PotionCureSelf, creature.DictCombatHealSelf[CombatHealSelf.PotionCureSelf]); }
                    if (AICombatHealSelf.CanDoCombatHealSelfBandageHealSelf(creature, 100)) { DictTemp.Add(CombatHealSelf.BandageHealSelf100, creature.DictCombatHealSelf[CombatHealSelf.BandageHealSelf100]); }
                    if (AICombatHealSelf.CanDoCombatHealSelfBandageHealSelf(creature, 75)) { DictTemp.Add(CombatHealSelf.BandageHealSelf75, creature.DictCombatHealSelf[CombatHealSelf.BandageHealSelf75]); }
                    if (AICombatHealSelf.CanDoCombatHealSelfBandageHealSelf(creature, 50)) { DictTemp.Add(CombatHealSelf.BandageHealSelf50, creature.DictCombatHealSelf[CombatHealSelf.BandageHealSelf50]); }
                    if (AICombatHealSelf.CanDoCombatHealSelfBandageHealSelf(creature, 25)) { DictTemp.Add(CombatHealSelf.BandageHealSelf25, creature.DictCombatHealSelf[CombatHealSelf.BandageHealSelf25]); }
                    if (AICombatHealSelf.CanDoCombatHealSelfBandageCureSelf(creature)) { DictTemp.Add(CombatHealSelf.BandageCureSelf, creature.DictCombatHealSelf[CombatHealSelf.BandageCureSelf]); }

                    int TotalValues = 0;

                    //Calculate Total Values
                    foreach (KeyValuePair<CombatHealSelf, int> pair in DictTemp)
                    {
                        TotalValues += pair.Value;
                    }

                    if (TotalValues > 0)
                        return true;
                }
            }

            return false;
        }

        public static bool DoCombatHealSelf(BaseCreature creature)
        {
            CombatHealSelf healAction = CombatHealSelf.None;

            int TotalValues = 0;

            Dictionary<CombatHealSelf, int> DictTemp = new Dictionary<CombatHealSelf, int>();

            if (AICombatHealSelf.CanDoCombatHealSelfSpellHealSelf(creature, 100)) { DictTemp.Add(CombatHealSelf.SpellHealSelf100, creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf100]); }
            if (AICombatHealSelf.CanDoCombatHealSelfSpellHealSelf(creature, 75)) { DictTemp.Add(CombatHealSelf.SpellHealSelf75, creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75]); }
            if (AICombatHealSelf.CanDoCombatHealSelfSpellHealSelf(creature, 50)) { DictTemp.Add(CombatHealSelf.SpellHealSelf50, creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50]); }
            if (AICombatHealSelf.CanDoCombatHealSelfSpellHealSelf(creature, 25)) { DictTemp.Add(CombatHealSelf.SpellHealSelf25, creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25]); }
            if (AICombatHealSelf.CanDoCombatHealSelfSpellCureSelf(creature)) { DictTemp.Add(CombatHealSelf.SpellCureSelf, creature.DictCombatHealSelf[CombatHealSelf.SpellCureSelf]); }
            if (AICombatHealSelf.CanDoCombatHealSelfPotionHealSelf(creature, 100)) { DictTemp.Add(CombatHealSelf.PotionHealSelf100, creature.DictCombatHealSelf[CombatHealSelf.PotionHealSelf100]); }
            if (AICombatHealSelf.CanDoCombatHealSelfPotionHealSelf(creature, 75)) { DictTemp.Add(CombatHealSelf.PotionHealSelf75, creature.DictCombatHealSelf[CombatHealSelf.PotionHealSelf75]); }
            if (AICombatHealSelf.CanDoCombatHealSelfPotionHealSelf(creature, 50)) { DictTemp.Add(CombatHealSelf.PotionHealSelf50, creature.DictCombatHealSelf[CombatHealSelf.PotionHealSelf50]); }
            if (AICombatHealSelf.CanDoCombatHealSelfPotionHealSelf(creature, 25)) { DictTemp.Add(CombatHealSelf.PotionHealSelf25, creature.DictCombatHealSelf[CombatHealSelf.PotionHealSelf25]); }
            if (AICombatHealSelf.CanDoCombatHealSelfPotionCureSelf(creature)) { DictTemp.Add(CombatHealSelf.PotionCureSelf, creature.DictCombatHealSelf[CombatHealSelf.PotionCureSelf]); }
            if (AICombatHealSelf.CanDoCombatHealSelfBandageHealSelf(creature, 100)) { DictTemp.Add(CombatHealSelf.BandageHealSelf100, creature.DictCombatHealSelf[CombatHealSelf.BandageHealSelf100]); }
            if (AICombatHealSelf.CanDoCombatHealSelfBandageHealSelf(creature, 75)) { DictTemp.Add(CombatHealSelf.BandageHealSelf75, creature.DictCombatHealSelf[CombatHealSelf.BandageHealSelf75]); }
            if (AICombatHealSelf.CanDoCombatHealSelfBandageHealSelf(creature, 50)) { DictTemp.Add(CombatHealSelf.BandageHealSelf50, creature.DictCombatHealSelf[CombatHealSelf.BandageHealSelf50]); }
            if (AICombatHealSelf.CanDoCombatHealSelfBandageHealSelf(creature, 25)) { DictTemp.Add(CombatHealSelf.BandageHealSelf25, creature.DictCombatHealSelf[CombatHealSelf.BandageHealSelf25]); }
            if (AICombatHealSelf.CanDoCombatHealSelfBandageCureSelf(creature)) { DictTemp.Add(CombatHealSelf.BandageCureSelf, creature.DictCombatHealSelf[CombatHealSelf.BandageCureSelf]); }

            //Calculate Total Values
            foreach (KeyValuePair<CombatHealSelf, int> pair in DictTemp)
            {
                TotalValues += pair.Value;
            }

            double ActionCheck = Utility.RandomDouble();
            double CumulativeAmount = 0.0;
            double AdditionalAmount = 0.0;

            //Determine CombatAction                      
            foreach (KeyValuePair<CombatHealSelf, int> pair in DictTemp)
            {
                AdditionalAmount = (double)pair.Value / (double)TotalValues;

                if (ActionCheck >= CumulativeAmount && ActionCheck < (CumulativeAmount + AdditionalAmount))
                {
                    healAction = pair.Key;

                    //Spell
                    if (healAction == CombatHealSelf.SpellHealSelf100 || healAction == CombatHealSelf.SpellHealSelf75 || healAction == CombatHealSelf.SpellHealSelf50 || healAction == CombatHealSelf.SpellHealSelf25)
                        AIHeal.DoSpellHeal(creature, creature);

                    if (healAction == CombatHealSelf.SpellCureSelf)
                        AIHeal.DoSpellCure(creature, creature);

                    //Potion
                    if (healAction == CombatHealSelf.PotionHealSelf100 || healAction == CombatHealSelf.PotionHealSelf75 || healAction == CombatHealSelf.PotionHealSelf50 || healAction == CombatHealSelf.PotionHealSelf25)
                        AIHeal.DoPotionHeal(creature);

                    if (healAction == CombatHealSelf.PotionCureSelf)
                        AIHeal.DoPotionCure(creature);

                    //Bandage
                    if (healAction == CombatHealSelf.BandageHealSelf100 || healAction == CombatHealSelf.BandageHealSelf75 || healAction == CombatHealSelf.BandageHealSelf50 || healAction == CombatHealSelf.BandageHealSelf25)
                        AIHeal.DoBandageHeal(creature, creature);

                    if (healAction == CombatHealSelf.BandageCureSelf)
                        AIHeal.DoBandageHeal(creature, creature);

                    creature.NextCombatHealActionAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(creature.CombatHealActionMinDelay, creature.CombatHealActionMaxDelay));

                    if (creature.AcquireNewTargetEveryCombatAction)
                        creature.m_NextAcquireTargetAllowed = DateTime.UtcNow;

                    return true;
                }

                CumulativeAmount += AdditionalAmount;
            }

            return false;
        }

        //-------------------------
        
        public static bool CanDoCombatHealSelfSpellHealSelf(BaseCreature creature, int percent)
        {
            if (creature.NextCombatHealActionAllowed > DateTime.UtcNow)
                return false;

            if (DateTime.UtcNow < creature.NextSpellTime)
                return false;

            switch (percent)
            {
                case 100:
                    if (creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] > 0)
                    {
                        if (AIHeal.DoesTargetNeedHealing(creature, 100) && creature.Mana >= 11)
                            return true;
                    }
                break;

                case 75:
                    if (creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] > 0)
                    {
                        if (AIHeal.DoesTargetNeedHealing(creature, 75) && creature.Mana >= 11)
                            return true;
                    }
                break;

                case 50:
                    if (creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] > 0)
                    {
                        if (AIHeal.DoesTargetNeedHealing(creature, 50) && creature.Mana >= 11)
                            return true;
                    }
                break;

                case 25:
                    if (creature.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] > 0)
                    {
                        if (AIHeal.DoesTargetNeedHealing(creature, 25) && creature.Mana >= 11)
                            return true;
                    }
                break;
            }

            return false;
        }

        public static bool CanDoCombatHealSelfSpellCureSelf(BaseCreature creature)
        {
            if (creature.NextCombatHealActionAllowed > DateTime.UtcNow)
                return false;

            if (DateTime.UtcNow < creature.NextSpellTime)
                return false;

            if (creature.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] > 0)
            {
                if (creature.Poison != null && creature.Mana >= 6)
                    return true;
            }

            return false;
        }

        public static bool CanDoCombatHealSelfPotionHealSelf(BaseCreature creature, int percent)
        {            
            if (creature.NextCombatHealActionAllowed > DateTime.UtcNow)
                return false;           

            switch (percent)
            {
                case 100:
                    if (creature.DictCombatHealSelf[CombatHealSelf.PotionHealSelf100] > 0)
                    {
                        if (AIHeal.DoesTargetNeedHealing(creature, 100))
                            return true;
                    }
                break;

                case 75:
                    if (creature.DictCombatHealSelf[CombatHealSelf.PotionHealSelf75] > 0)
                    {
                        if (AIHeal.DoesTargetNeedHealing(creature, 75))
                            return true;
                    }
                break;

                case 50:
                    if (creature.DictCombatHealSelf[CombatHealSelf.PotionHealSelf50] > 0)
                    {
                        if (AIHeal.DoesTargetNeedHealing(creature, 50))
                            return true;
                    }
                break;

                case 25:
                    if (creature.DictCombatHealSelf[CombatHealSelf.PotionHealSelf25] > 0)
                    {
                        if (AIHeal.DoesTargetNeedHealing(creature, 25))
                            return true;
                    }
                break;
            }

            return false;
        }

        public static bool CanDoCombatHealSelfPotionCureSelf(BaseCreature creature)
        {
            if (creature.NextCombatHealActionAllowed > DateTime.UtcNow)
                return false;            

            if (creature.DictCombatHealSelf[CombatHealSelf.PotionCureSelf] > 0)
            {
                if (creature.Poison != null)
                    return true;
            }

            return false;
        }

        public static bool CanDoCombatHealSelfBandageHealSelf(BaseCreature creature, int percent)
        {
            if (creature.NextCombatHealActionAllowed > DateTime.UtcNow)
                return false;

            if (creature.DoingBandage)
                return false;

            switch (percent)
            {
                case 100:
                    if (creature.DictCombatHealSelf[CombatHealSelf.BandageHealSelf100] > 0)
                    {
                        if (AIHeal.DoesTargetNeedHealing(creature, 100))
                            return true;
                    }
                break;

                case 75:
                    if (creature.DictCombatHealSelf[CombatHealSelf.BandageHealSelf75] > 0)
                    {
                        if (AIHeal.DoesTargetNeedHealing(creature, 75))
                            return true;
                    }
                break;

                case 50:
                    if (creature.DictCombatHealSelf[CombatHealSelf.BandageHealSelf50] > 0)
                    {
                        if (AIHeal.DoesTargetNeedHealing(creature, 50))
                            return true;
                    }
                break;

                case 25:
                    if (creature.DictCombatHealSelf[CombatHealSelf.BandageHealSelf25] > 0)
                    {
                        if (AIHeal.DoesTargetNeedHealing(creature, 25))
                            return true;
                    }
                break;
            }

            return false;
        }

        public static bool CanDoCombatHealSelfBandageCureSelf(BaseCreature creature)
        {
            if (creature.NextCombatHealActionAllowed > DateTime.UtcNow)
                return false;

            if (creature.DoingBandage)
                return false;

            if (creature.DictCombatHealSelf[CombatHealSelf.BandageCureSelf] > 0)
            {
                if (creature.Poison != null)
                    return true;
            }

            return false;
        }        
    }
}
