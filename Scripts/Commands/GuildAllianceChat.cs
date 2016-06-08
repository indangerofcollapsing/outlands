using Server.Commands;

using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Commands
{
    class GuildAllianceChat
    {
        public static void Initialize()
        {
            CommandSystem.Register("ga", AccessLevel.Player, new CommandEventHandler(OnGuildAllianceChat));
        }

        private static void OnGuildAllianceChat(CommandEventArgs e)
        {
            var pm = e.Mobile as PlayerMobile;
            string message = String.Join(" ", e.Arguments);

            //TEST: GUILD
            /*
            Guild g = pm.Guild as Guild;

            if (g != null && g.Alliance != null) 
            {
                g.Alliance.AllianceChat(e.Mobile, Guild.GuildAllianceMessageHue, message);
                PlayerMobile.SendToStaffMessage(pm, String.Format("[Alliance]: {0}", message));
            }

            else
            {
                pm.SendLocalizedMessage(1071020); // You are not in an alliance!
            }
            */
        }
    }
}
