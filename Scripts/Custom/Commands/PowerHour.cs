using Server.Commands.Generic;
using Server.Gumps;
using Server.Mobiles;
using Server.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Commands
{
    public class PowerHour
    {
        public static void Initialize()
        {
            CommandSystem.Register("PowerHour", AccessLevel.Player, new CommandEventHandler(PowerHour_OnCommand));
            CommandSystem.Register("ph", AccessLevel.Player, new CommandEventHandler(PowerHour_OnCommand));
        }

        private static void PowerHour_OnCommand(CommandEventArgs e)
        {
            if (e.Mobile is PlayerMobile)
            {
                var player = e.Mobile as PlayerMobile;

                if (player.HasGump(typeof(RestedStateGump)))
                    player.CloseGump(typeof(RestedStateGump));

                player.SendGump(new RestedStateGump(player));

            }
        }
    }

    public class PowerHourCommand : BaseCommand
    {
        private bool m_Value;

        public PowerHourCommand(bool value)
        {
            m_Value = value;

            AccessLevel = AccessLevel.Seer;
            Supports = CommandSupport.AllMobiles;
            Commands = new string[] { value ? "ResetPowerHour" : "EndPowerHour" };
            ObjectTypes = ObjectTypes.Mobiles;

            if (value)
            {
                Usage = "ResetPowerHour";
                Description = "Reset the powerhour timer of the target.";
            }
            else
            {
                Usage = "EndPowerHour";
                Description = "End the powerhour of the target";
            }
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            var targeted = (Mobile)obj;
            var from = e.Mobile;
            if (!(targeted is PlayerMobile))
            {
                //from.SendMessage("This command only works on a player.");
                return;
            }
            if (m_Value)
            {
                var player = targeted as PlayerMobile;
                player.PowerHourReset = DateTime.UtcNow.AddDays(-1);
                //from.SendMessage("Target is now able to activate powerhour.");
            }
            else
            {
                var player = targeted as PlayerMobile;
                player.PowerHourReset = DateTime.UtcNow.AddHours(-1);
                //from.SendMessage("Target's power hour is over.");
            }
        }
    }
}
