using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.Townsystem
{
    public class MilitiaRevokeCancelGump : Gump
    {
        public MilitiaRevokeCancelGump(Mobile from)
            : base(50, 100)
        {
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            from.CloseGump(typeof(MilitiaRevokeCancelGump));

            var pm = (PlayerMobile)from;

            if (pm == null || pm.TownsystemPlayerState == null)
                return;

            var state = pm.TownsystemPlayerState;

            var leaving = state.Leaving;

            TimeSpan left;

            DateTime now = DateTime.Now;

            if (leaving > now)
                left = leaving - now;
            else
                left = TimeSpan.Zero;

            AddPage(0);
            AddBackground(0, 0, 484, 113, 9200);
            AddHtml(12, 14, 459, 61, String.Format("There are still {0} days, {1} hours, and {2} minutes left until you will have resigned from the militia. Would you like to cancel this resign request?", left.Days, left.Hours, left.Minutes), (bool)true, (bool)false);
            AddButton(68, 83, 4017, 4019, (int)Buttons.CancelRevoke, GumpButtonType.Reply, 0);
            AddButton(262, 83, 4023, 4025, 0, GumpButtonType.Reply, 0);
            AddLabel(99, 84, 0, @"Cancel Resign");
            AddLabel(293, 84, 0, @"Continue Resigning");
        }

        public enum Buttons
        {
            CancelGump,
            CancelRevoke,
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            switch (info.ButtonID)
            {
                case (int)Buttons.CancelRevoke:
                    {
                        var pm = (PlayerMobile)from;

                        if (pm == null || pm.TownsystemPlayerState == null)
                            return;

                        pm.TownsystemPlayerState.Leaving = DateTime.MinValue;
                        from.SendMessage("You have cancelled the militia resign request.");
                        break;
                    }
            }
        }
    }
}