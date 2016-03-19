using System;
using System.Collections.Generic;
using Server.Accounting;
using Server.Mobiles;
using Server.Commands;
using Server.Targeting;
using Server.Gumps;
using System.IO;

namespace Server.Custom
{
    public static class InactiveScrollPromotion
    {
        public static void Initialize()
        {
            CommandSystem.Register("GiveScrollPromotion", AccessLevel.Administrator, new CommandEventHandler(GiveScrollPromotion_OnCommand));
        }

        public static void GiveScrollPromotion_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            from.Target = new InternalTarget();
            from.SendMessage("Who would you like to give a skill scroll promotion to?");
        }

        public class InternalTarget : Target
        {
            public InternalTarget()
                : base(18, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                PlayerMobile pm = targeted as PlayerMobile;

                if (pm == null)
                    return;

                if (pm.LoginElapsedTime == TimeSpan.Zero)
                    pm.LoginElapsedTime = MinimumDelay;

                SendPromotionGump(pm);
            }
        }

        public static TimeSpan MinimumDelay = TimeSpan.FromDays(14);

        public static void AccountLogin(LoginEventArgs e)
        {
            Account acct = e.Mobile.Account as Account;

            if (acct == null)
                return;

            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm == null)
                return;

            if (pm.Region is UOACZRegion)
                return;

			DateTime now = DateTime.UtcNow;

            if (pm.LoginElapsedTime > TimeSpan.Zero)            
                SendPromotionGump(e.Mobile);            

            else
            {
                if (acct.LastLogin == DateTime.MinValue || acct.LastLogin + MinimumDelay > now)
                    return;

                TimeSpan elapsed = now - acct.LastLogin;

                pm.LoginElapsedTime = elapsed;

                SendPromotionGump(e.Mobile);
            }
        }

        public static void SendPromotionGump(Mobile m)
        {
            if (m == null)
                return;

            m.SendGump(new InactiveScrollPromotionGump(m));
        }

        public static void LogUse(Mobile m)
        {
            if (m == null)
                return;

            try
            {
                using (StreamWriter writer = new StreamWriter("SkillscrollPromotion.csv", true))
                {
                    writer.WriteLine(String.Format("{0},{1},{2},{3}", m.RawName, m.Account.Username, m.Serial, m.NetState == null ? "0.0.0.0" : m.NetState.Address.ToString()));
                }
            }
            catch { }
        }
    }
}
