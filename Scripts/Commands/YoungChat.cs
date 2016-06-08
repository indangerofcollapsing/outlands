using Server.Accounting;
using Server.Commands;

using Server.Mobiles;
using Server.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Commands
{
    public class YoungChat
    {


        public static void Initialize()
        {
            CommandSystem.Register("yc", AccessLevel.Player, new CommandEventHandler(OnYoungChat));
            
            
        
        }

        private static void OnYoungChat(CommandEventArgs e)
        {
            var pm = e.Mobile as PlayerMobile;
            string message = String.Join(" ", e.Arguments);

            if (pm.Young || pm.Companion)
            {
                pm.YoungPlayerChat(message);
                PlayerMobile.SendToStaffMessage(pm, String.Format("[Young]: {0}", message));
            }
        }
    }
}
