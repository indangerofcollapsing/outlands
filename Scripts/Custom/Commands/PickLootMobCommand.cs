using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Commands.Generic;
using Server.Spells;
using Server.Targeting;
using Server.Targets;

namespace Server.Commands
{
    public class PickLootMobCommand
    {

        public static void Initialize()
        {

            CommandSystem.Register("DropLoot", AccessLevel.GameMaster, new CommandEventHandler(DropLootMob_OnCommand ));

        }

        [Usage("DropLoot [integer 0-2] ")]
        [Description("Target mobile, target item. Drops targeted reward in a pack.")]
        private static void DropLootMob_OnCommand(CommandEventArgs e)
        {
            int flag = 3;
            if (e.Length >= 1)
                flag = e.GetInt32(0);

            e.Mobile.Target = new PickLootMob(flag);//Call first target, select mobile
        }

    }
}
