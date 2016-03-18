using System;
using System.Collections;
using System.Collections.Generic;
using Server.Custom;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Commands
{
	public class ZombieQuestTopTenCommand
	{
        public static bool zListEnabled = false;

		public static void Initialize()
		{
			CommandSystem.Register("zombie", AccessLevel.GameMaster, new CommandEventHandler(ZQTT_OnCommand));
            CommandSystem.Register("enableZombieList", AccessLevel.Administrator, new CommandEventHandler(enableZombieList_OnCommand));
            CommandSystem.Register("zcleartopten", AccessLevel.Administrator, new CommandEventHandler(zClearTopTen_OnCommand));
            CommandSystem.Register("endzombiequest", AccessLevel.Administrator, new CommandEventHandler(EndZombieQuest_OnCommand));
        }

		[Usage("zombie")]
		[Description("Shows the current Zombie Quest Top Ten.")]
		private static void ZQTT_OnCommand(CommandEventArgs e)
		{
			// Get the current Mobile
			Mobile from = e.Mobile;

            if (zListEnabled || from.AccessLevel > AccessLevel.Player)
            {
                // Send the Zombie Top Ten List Gump
                from.SendGump(new ZombieTopTen());
            }
            else
            {
                from.SendMessage("That list is disabled until the event is over!");
            }
		}

        [Usage("zcleartopten")]
        [Description("Clears the Zombie Top Ten.")]
        private static void zClearTopTen_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("The zombie rankings have been cleared.");
            ZombieQuestRegion.TopTen.Clear();
        }

        [Usage("endzombiequest")]
        [Description("Clears the Zombie Arena.")]
        private static void EndZombieQuest_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("The zombie arena has been cleared.");
            ZombieQuestRegion.End();
        }

        [Usage("enableZombieList")]
        [Description("Enables players to see the zombie list.")]
        private static void enableZombieList_OnCommand(CommandEventArgs e)
        {
            zListEnabled = !zListEnabled;

            e.Mobile.SendMessage("The zombie list is now {0}.", zListEnabled ? "enabled" : "disabled");
        }
    }
}