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
    public static class AIMiscAction
    {
        public static bool CanDoDetectHidden(BaseCreature creature)
        {
            return true;
        }

        public static bool CanDoSpellDispelSummon(BaseCreature creature)
        {
            return true;
        }

        public static bool CanDoSpellReveal(BaseCreature creature)
        {
            return true;
        }

        public static bool CanDoStealth(BaseCreature creature)
        {
            if (creature.BardPacified || creature.BardProvoked)
                return false;

            return true;
        }

        public static bool CanDoStealing(BaseCreature creature)
        {
            return true;
        }

        public static bool CanDoTracking(BaseCreature creature)
        {
            return true;
        }

        public static bool DoStealing(BaseCreature creature)
        {
            bool success = true;

            return success;
        }

        public static bool DoTracking(BaseCreature creature)
        {
            bool success = true;

            return success;
        }

        public static bool DoStealth(BaseCreature creature)
        {
            creature.Hidden = true;
            creature.IsStealthing = true;
            creature.StealthAttackReady = true;

            return true;
        }

        public static void DoDetectHidden(BaseCreature creature)
        {
            if (creature.Deleted || creature.Map == null)
                return;

            creature.Emote("*searches*");

            double skillValue = creature.Skills.DetectHidden.Base;

            double minRange = skillValue / 20;
            double maxRange = skillValue / 10;

            int range = (int)(minRange + ((maxRange - minRange) * Utility.RandomDouble()));
            
            if (range > 0)
            {
                IPooledEnumerable inRange = creature.Map.GetMobilesInRange(creature.Location, range);

                foreach (Mobile mobile in inRange)
                {
                    if (!mobile.Hidden || !mobile.Alive) continue;
                    if (mobile.AccessLevel > AccessLevel.Player) continue;
                    if (!creature.InLOS(mobile)) continue;

                    PlayerMobile player = mobile as PlayerMobile;
                    BaseCreature bc_Creature = mobile as BaseCreature;

                    bool isPlayer = (player != null);
                    bool isTamedCreature = false;

                    if (bc_Creature != null)
                    {
                        if (bc_Creature.Controlled && bc_Creature.ControlMaster is PlayerMobile)
                            isTamedCreature = true;
                    }

                    if (!(isPlayer || isTamedCreature))
                        continue;

                    double revealChance = (skillValue / 100);

                    if (Utility.RandomDouble() < revealChance)
                    {
                        mobile.RevealingAction();
                        mobile.SendMessage("You have been revealed!");
                    }
                }

                inRange.Free();
            }
        }

        public static void DoSpellDispelSummon(BaseCreature creature)
        {
        }

        public static void DoSpellReveal(BaseCreature creature)
        {
        }
    }
}
