using System;
using Server.Network;
using Server.Mobiles;
using Server;
using Server.Items;

namespace Server.Misc
{
    public class HungerThirstDecayTimer : Timer
    {
        public static void Initialize()
        {
            new HungerThirstDecayTimer().Start();
        }

        public HungerThirstDecayTimer(): base(TimeSpan.FromMinutes(Food.HungerThirstTickDuration), TimeSpan.FromMinutes(Food.HungerThirstTickDuration))
        {
            Priority = TimerPriority.OneMinute;
        }

        protected override void OnTick()
        {
            ApplyHungerThirst();
        }

        public static void ApplyHungerThirst()
        {
            foreach (NetState state in NetState.Instances)
            {
                BaseCreature bc_Creature = state.Mobile as BaseCreature;

                if (bc_Creature != null)
                {
                    if (Utility.RandomDouble() < .10)
                    {
                        HungerDecay(state.Mobile);
                        ThirstDecay(state.Mobile);
                    }
                }

                else
                {
                    HungerDecay(state.Mobile);
                    ThirstDecay(state.Mobile);
                }
            }
        }

        public static void HungerDecay(Mobile from)
        {
            if (from != null && from.Hunger >= Food.HungerThirstLostPerTick)
                from.Hunger -= Food.HungerThirstLostPerTick;

            PlayerMobile player = from as PlayerMobile;

            if (player != null)
            {
                if (player.SatisfactionLevel > Food.SatisfactionLevelType.None && player.SatisfactionExpiration <= DateTime.UtcNow)
                {
                    player.SendMessage("You no longer feel satisified from food.");
                    player.SatisfactionLevel = Food.SatisfactionLevelType.None;                    
                }
            }
        }

        public static void ThirstDecay(Mobile m)
        {
            if (m != null && m.Thirst >= Food.HungerThirstLostPerTick)
                m.Thirst -= Food.HungerThirstLostPerTick;
        }
    }
}