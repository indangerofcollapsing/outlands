using System;
using Server;
using Server.Network;
using Server.Mobiles;
using Server.Commands;
using System.IO;
using System.Text;
using System.Collections;
using System.Net;
using Server.Accounting;
using Server.Items;
using Server.Menus;
using Server.Menus.Questions;
using Server.Menus.ItemLists;
using Server.Spells;
using Server.Targeting;
using Server.Targets;
using Server.Gumps;

namespace Server.Commands
{
    public class CheckDungeon
	{
		public static void Initialize()
		{
            CommandSystem.Register("CheckDungeon", AccessLevel.Counselor, new CommandEventHandler(CheckDungeon_OnCommand));
		}

        public static void CheckDungeon_OnCommand(CommandEventArgs e)
		{
            Mobile from = e.Mobile;
            bool gains = false;
            if ((from.Region.IsPartOf(typeof(Regions.DungeonRegion)) && !SpellHelper.IsBritainSewers(from.Map, from.Location)))
            {
                gains = true;
            }

            from.SendMessage("Your current location {0} provide dungeon gain bonuses.", gains ? "DOES" : "DOES NOT");
		}
	}
}
