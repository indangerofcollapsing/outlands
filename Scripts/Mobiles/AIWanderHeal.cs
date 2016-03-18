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
    public static class AIWanderHeal
    {
        public static bool CanDoWanderHealSelfSpellHealSelf(BaseCreature creature, int percent)
        {
            if (DateTime.UtcNow < creature.NextSpellTime)
                return false;

            switch (percent)
            {
                case 100:
                    if (creature.DictWanderAction[WanderAction.SpellHealSelf100] > 0)
                    {
                        if (AIHeal.DoesTargetNeedHealing(creature, 100) && creature.Mana >= 11)
                            return true;
                    }
                break;

                case 50:
                    if (creature.DictWanderAction[WanderAction.SpellHealSelf50] > 0)
                    {
                        if (AIHeal.DoesTargetNeedHealing(creature, 50) && creature.Mana >= 11)
                            return true;
                    }
                break;
            }

            return false;
        }

        public static bool CanDoWanderHealSelfSpellCureSelf(BaseCreature creature)
        {
            if (DateTime.UtcNow < creature.NextSpellTime)
                return false;

            if (creature.DictWanderAction[WanderAction.SpellCureSelf] > 0)
            {
                if (creature.Poison != null && creature.Mana >= 6)
                    return true;
            }

            return false;
        }

        public static bool CanDoWanderHealSelfPotionHealSelf(BaseCreature creature, int percent)
        {
            switch (percent)
            {
                case 100:
                    if (creature.DictWanderAction[WanderAction.PotionHealSelf100] > 0)
                    {
                        if (AIHeal.DoesTargetNeedHealing(creature, 100))
                            return true;
                    }
                break;

                case 50:
                    if (creature.DictWanderAction[WanderAction.PotionHealSelf50] > 0)
                    {
                        if (AIHeal.DoesTargetNeedHealing(creature, 50))
                            return true;
                    }
                break;
            }

            return false;
        }

        public static bool CanDoWanderHealSelfPotionCureSelf(BaseCreature creature)
        {
            if (creature.DictWanderAction[WanderAction.PotionCureSelf] > 0)
            {
                if (creature.Poison != null)
                    return true;
            }

            return false;
        }

        public static bool CanDoWanderHealSelfBandageHealSelf(BaseCreature creature, int percent)
        {
            if (creature.DoingBandage)
                return false;

            switch (percent)
            {
                case 100:
                    if (creature.DictWanderAction[WanderAction.BandageHealSelf100] > 0)
                    {
                        if (AIHeal.DoesTargetNeedHealing(creature, 100))
                            return true;
                    }
                break;

                case 50:
                    if (creature.DictWanderAction[WanderAction.BandageHealSelf50] > 0)
                    {
                        if (AIHeal.DoesTargetNeedHealing(creature, 50))                        
                            return true;                        
                    }
                break;
            }

            return false;
        }

        public static bool CanDoWanderHealSelfBandageCureSelf(BaseCreature creature)
        {
            if (creature.DoingBandage)
                return false;

            if (creature.DictWanderAction[WanderAction.BandageCureSelf] > 0)
            {
                if (creature.Poison != null)
                    return true;
            }

            return false;
        }

        public static bool CanDoWanderHealOtherSpellHealOther(BaseCreature creature, int percent)
        {
            if (creature.Mana < 11)
                return false;

            if (DateTime.UtcNow < creature.NextSpellTime)
                return false;

            switch (percent)
            {
                case 100:
                    if (creature.DictWanderAction[WanderAction.SpellHealOther100] <= 0)
                        return false;
                break;

                case 50:
                    if (creature.DictWanderAction[WanderAction.SpellHealOther50] <= 0)
                        return false;
                break;
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

        public static bool CanDoWanderHealOtherSpellCureOther(BaseCreature creature)
        {
            if (creature.Mana < 6)
                return false;

            if (DateTime.UtcNow < creature.NextSpellTime)
                return false;

            if (creature.DictWanderAction[WanderAction.SpellCureOther] <= 0)
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

        public static bool CanDoWanderHealOtherBandageHealOther(BaseCreature creature, int percent)
        {
            if (creature.DoingBandage)
                return false;

            switch (percent)
            {
                case 100:
                    if (creature.DictWanderAction[WanderAction.BandageHealOther100] <= 0)
                        return false;
                break;

                case 50:
                    if (creature.DictWanderAction[WanderAction.BandageHealOther50] <= 0)
                        return false;
                break;
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

        public static bool CanDoWanderHealOtherBandageCureOther(BaseCreature creature)
        {
            if (creature.DoingBandage)
                return false;

            if (creature.DictWanderAction[WanderAction.BandageCureOther] <= 0)
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
