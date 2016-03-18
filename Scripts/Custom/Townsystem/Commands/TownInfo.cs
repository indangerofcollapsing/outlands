using Server.Commands;
using Server.Custom.Townsystem.Gumps;
using Server.Gumps;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Townsystem.Commands
{
    class TownInfo
    {
        public static void Initialize()
        {
            CommandSystem.Register("towninfo", AccessLevel.Player, new CommandEventHandler(OnTownInfo));
        }

        private static void OnTownInfo(CommandEventArgs e)
        {
            var pm = e.Mobile as PlayerMobile;
            var loc = new Point3D(pm);
            Town town = Town.FromLocation(loc, Map.Felucca);
            if (town == null)
            {
                pm.SendMessage("You are not currently in a town.");
            }
            else
            {
                pm.SendGump(new TownInfoGump(pm, town));
            }
        }
    }

    class TownInfoGump : Gump
    {
        private PlayerMobile m_From;
        private Town m_Town;

        enum Buttons
        {
            SecondaryBuffs = 1,
            PreviousTown = 2,
            NextTown = 3,
        }

        public TownInfoGump(PlayerMobile from, Town town)
            : base(200, 200)
        {
            m_From = from;
            m_Town = town;
            #region static content
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
            this.AddPage(0);
            this.AddImage(0, 4, 206);
            this.AddImageTiled(44, 45, 427, 338, 200);
            this.AddImage(44, 4, 201);
            this.AddImage(471, 4, 207);
            this.AddImage(0, 48, 202);
            this.AddImage(471, 48, 203);
            this.AddImage(0, 68, 202);
            this.AddImage(471, 68, 203);
            this.AddImage(0, 383, 204);
            this.AddImage(471, 383, 205);
            this.AddImage(44, 383, 233);
            #endregion

            this.AddHtml(43, 22, 426, 14, String.Format("<BASEFONT COLOR=BLACK><center>{0} TOWN INFO</center>", town.Definition.TownName), (bool)false, (bool)false);
            this.AddButton(140, 25, 2223, 2223, (int)Buttons.PreviousTown, GumpButtonType.Reply, 0);
            this.AddButton(350, 25, 2224, 2224, (int)Buttons.NextTown, GumpButtonType.Reply, 0);

            this.AddLabel(305, 46, 0x0, @"Controlling Town:");
            this.AddLabel(414, 46, town.ControllingTown == town ? 0x5 : 0x0, town.ControllingTown == null ? "None" : town.ControllingTown.Definition.FriendlyName);

            this.AddImageTiled(36, 95, 171, 19, 214);
            this.AddLabel(35, 80, 0x0, @"Treasury:");
            this.AddLabel(120, 80, 0x44, ((int)town.Treasury).ToString());

            this.AddImageTiled(35, 125, 171, 19, 214);
            this.AddLabel(35, 110, 0x0, @"Sales Tax:");
            this.AddLabel(152, 110, 0x5, (town.SalesTax * 100).ToString());
            this.AddLabel(196, 110, 0x0, @"%");

            this.AddImageTiled(35, 155, 171, 19, 214);
            this.AddLabel(35, 140, 0x0, string.Format("Secondary Buffs: {0}", m_Town.SecondaryCitizenshipBuffs.Count));
            this.AddButton(210, 140, 4005, 4007, (int)Buttons.SecondaryBuffs, GumpButtonType.Reply, 0);


            string kingName = "";
            if (town.King != null)
                kingName = town.King.Name;
            this.AddImageTiled(35, 185, 171, 19, 214);
            this.AddLabel(35, 170, 0x0, @"King:");
            this.AddLabel(75, 170, 0x5, kingName);
            

            this.AddImageTiled(306, 95, 171, 19, 214);
            this.AddLabel(305, 80, 0x0, @"Active Citizens:");
            this.AddLabel(441, 80, 0x0, town.ActiveCitizens.ToString());

            this.AddImageTiled(306, 125, 171, 19, 214);
            this.AddLabel(305, 110, 0x0, @"Active Militia:");
            this.AddLabel(441, 110, 0x0, town.ActiveMilitia.ToString());

            this.AddImageTiled(306, 155, 171, 19, 214);
            this.AddLabel(305, 140, 0x0, @"Exiles:");
            this.AddLabel(441, 140, 0x0, town.Exiles.Count.ToString());

            this.AddImageTiled(306, 185, 171, 19, 214);
            this.AddLabel(305, 170, 0x0, @"Guards:");
            this.AddLabel(441, 170, 0x0, town.NumberOfGuards.ToString());

            this.AddGroup(1); //GUARD GROUP
            this.AddImageTiled(35, 218, 171, 19, 214);
            this.AddLabel(35, 203, 0x0, @"Passive Guard Protection:");
            this.AddRadio(35, 230, 208, 209, town.GuardState == GuardStates.None ? true : false, 20);
            this.AddLabel(57, 230, 0x0, @"None");
            this.AddRadio(35, 255, 208, 209, town.GuardState == GuardStates.Lax ? true : false, 21);
            this.AddLabel(57, 255, 0x0, @"Lax");
            this.AddRadio(35, 280, 208, 209, town.GuardState == GuardStates.Standard ? true : false, 22);
            this.AddLabel(57, 280, 0x0, @"Standard");
            this.AddRadio(35, 305, 208, 209, town.GuardState == GuardStates.Strong ? true : false, 23);
            this.AddLabel(56, 305, 0x0, @"Strong");
            this.AddRadio(35, 330, 208, 209, town.GuardState == GuardStates.Exceptional ? true : false, 24);
            this.AddLabel(56, 330, 0x0, @"Exceptional");


            this.AddImageTiled(35, 370, 171, 19, 214);
            this.AddLabel(35, 355, 0x0, @"Town Buff:");
            this.AddLabel(35, 380, 0x5, TownBuff.GetBuffName(town.PrimaryCitizenshipBuff));
        }

        public override void OnResponse(Network.NetState sender, RelayInfo info)
        {
            base.OnResponse(sender, info);
            if (info.ButtonID == (int)Buttons.SecondaryBuffs)
            {
                sender.Mobile.SendGump(new SecondaryBuffViewGump(m_Town));
            }
            else if (info.ButtonID == (int)Buttons.PreviousTown)
            {
                var currentIndex = Town.Towns.IndexOf(m_Town);
                var previousIndex = currentIndex - 1;
                if (previousIndex < 0)
                    previousIndex = Town.Towns.Count - 1;
                sender.Mobile.SendGump(new TownInfoGump(sender.Mobile as PlayerMobile, Town.Towns[previousIndex]));
            }
            else if (info.ButtonID == (int)Buttons.NextTown)
            {
                var currentIndex = Town.Towns.IndexOf(m_Town);
                var nextIndex = currentIndex + 1;
                if (nextIndex >= Town.Towns.Count)
                    nextIndex = 0;
                sender.Mobile.SendGump(new TownInfoGump(sender.Mobile as PlayerMobile, Town.Towns[nextIndex]));
            }
        }
    }
}
