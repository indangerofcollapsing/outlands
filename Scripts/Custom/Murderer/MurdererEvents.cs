using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Accounting;
using Server.Misc;
using Server.Spells;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Achievements;

namespace Server
{
    public static class MurdererEvents
    {
        public static void MurdererKillPaladinResult(PlayerMobile murderer, PlayerMobile paladin, bool isHighestDamager)
        {
            if (isHighestDamager)
                murderer.SendMessage("You have slain a cowardly paladin!");

            else
                murderer.SendMessage("You assist in the slaying of a cowardly paladin!");
        }
    }
}
