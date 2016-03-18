using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Custom.Townsystem;
using Server.Mobiles;

namespace Server.Gumps
{
    public class CitizenshipGump : Gump
    {
        private Town m_Town;

        public CitizenshipGump(Town town) : base(0, 0)
        {
            m_Town = town;
            this.Closable=true;
            this.Disposable=true;
            this.Dragable=true;
            this.Resizable=false;
            this.AddPage(0);
            this.AddImageTiled(31, 30, 243, 483, 3504);
            this.AddImage(31, 4, 3501);
            this.AddImage(5, 4, 3500);
            this.AddImage(273, 4, 3502);
            this.AddImage(5, 30, 3503);
            this.AddImage(273, 29, 3505);
            this.AddImage(273, 270, 3505);
            this.AddImage(5, 270, 3503);
            this.AddImage(31, 512, 3507);
            this.AddImage(5, 512, 3506);
            this.AddImage(273, 512, 3508);
            this.AddImage(24, 22, 22220);
            this.AddImage(61, 59, 22225);
            this.AddImage(62, 36, 22228);
            this.AddHtml(107, 25, 161, 79, String.Format("<BASEFONT COLOR=BLACK><center>Welcome to <br>{0}!</center>",town.Definition.FriendlyName), (bool)false, (bool)false);
            this.AddHtml(30, 118, 262, 294, @"<BASEFONT COLOR=BLACK>Towns in Britannia provide numerous benefits to their citizens.  Among these benefits are:<br>*Lowered prices from vendors<br>*The ability to own land within the town<br>*The ability to place vendors at the town market<br>*The right to vote in elections<br>*Protection by town guards<br><br>You can change your citizenship no more than one time in a two week period.<br><br>You can request citizenship at a later date by speaking the words 'I wish to become a citizen.'", (bool)false, (bool)false);
            this.AddButton(26, 425, 4023, 4025, (int)Buttons.btnJoin, GumpButtonType.Reply, 0);
            this.AddLabel(61, 426, 0, String.Format("Become a citizen of {0}.", town.Definition.FriendlyName));
            this.AddButton(26, 452, 4017, 4019, (int)Buttons.btnCancel, GumpButtonType.Reply, 0);
            this.AddLabel(61, 453, 0, @"Not at this time.");
            this.AddCheck(31, 487, 210, 211, false, (int)Buttons.chkSquelch);
            this.AddLabel(52, 488, 0, @"Do not show this message again.");
        }

        public enum Buttons
        {
            btnCancel,
            btnJoin,
            chkSquelch,
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            base.OnResponse(sender, info);

            ((PlayerMobile)sender.Mobile).SquelchCitizenship = info.IsSwitched((int)Buttons.chkSquelch);

            if (info.ButtonID == (int)Buttons.btnJoin)
            {
                Town.AddCitizen(sender.Mobile, m_Town);
            }
        }
    }
}