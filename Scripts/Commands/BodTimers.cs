using Server.Commands;
using Server.Gumps;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Commands
{
    class BodTimers
    {
        public static void Initialize()
        {
            CommandSystem.Register("bodtimers", AccessLevel.Player, new CommandEventHandler(OnBodTimers));
        }

        private static void OnBodTimers(CommandEventArgs e)
        {
            var pm = e.Mobile as PlayerMobile;
            TimeSpan tailor = pm.NextTailorBulkOrder;
			TimeSpan smith = pm.NextSmithBulkOrder;
            
			pm.Say( "Tailor: {0} Smith: {1}", tailor, smith );
			
        }
    }

    
}
