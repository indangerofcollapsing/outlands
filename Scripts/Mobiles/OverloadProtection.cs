using System;
using Server;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;
using Server.Network;

namespace Server
{
    public static class OverloadProtection
    {
        public static bool OverloadProtectionEnabled = true;

        private static OverloadProtectionTimer m_OverloadProtectionTimer;

        public static void Initialize()
        {
            Timer.DelayCall(TimeSpan.FromTicks(1), delegate
            {
                if (OverloadProtectionEnabled)
                {
                    m_OverloadProtectionTimer = new OverloadProtectionTimer();
                    m_OverloadProtectionTimer.Start();
                }
            });           
        }

        public class OverloadProtectionTimer : Timer
        {
            public OverloadProtectionTimer(): base(TimeSpan.Zero, PlayerMobile.SystemOverloadInterval)
            {
                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {           
                List<PlayerMobile> m_PlayerViolations = new List<PlayerMobile>();

                foreach (NetState state in NetState.Instances)
                {
                    PlayerMobile player = state.Mobile as PlayerMobile;

                    if (player == null)
                        continue;

                    if (player.SystemOverloadActions >= PlayerMobile.SystemOverloadActionThreshold && player.AccessLevel == AccessLevel.Player)                
                        m_PlayerViolations.Add(player);                

                    player.SystemOverloadActions = 0;
                }

                foreach (NetState state in NetState.Instances)
                {
                    PlayerMobile player = state.Mobile as PlayerMobile;

                    if (player == null) continue;
                    if (player.AccessLevel == AccessLevel.Player) continue;

                    foreach (PlayerMobile playerViolator in m_PlayerViolations)
                    {
                        player.SendMessage(2115, "Action spam detected for " + playerViolator.RawName + " at location " + playerViolator.Location.ToString() + ".");
                    }
                }
            }
        }
    }
}
