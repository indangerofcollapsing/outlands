using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Commands;
using Server.Mobiles;

namespace Server.Custom.Townsystem
{
    public class CitizenshipRevokeCancelGump : Gump
    {
        public CitizenshipRevokeCancelGump(Mobile from)
            : base(50, 100)
        {
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            from.CloseGump(typeof(CitizenshipRevokeCancelGump));

            var pm = (PlayerMobile)from;

            if (pm == null || pm.CitizenshipPlayerState == null)
                return;

            var state = pm.CitizenshipPlayerState;

            var leaving = state.Leaving + TimeSpan.FromHours(1);

            TimeSpan left;

            DateTime now = DateTime.Now;

            if (leaving > now)
                left = leaving - now;
            else
                left = TimeSpan.Zero;

            AddPage(0);
            AddBackground(0, 0, 484, 113, 9200);
            AddHtml(12, 14, 459, 61, String.Format("There are still {0} minutes left until your citizenship may be revoked (upon log-in). Would you like to cancel this revoke request?", left.Minutes), (bool)true, (bool)false);
            AddButton(68, 83, 4017, 4019, (int)Buttons.CancelRevoke, GumpButtonType.Reply, 0);
            AddButton(262, 83, 4023, 4025, 0, GumpButtonType.Reply, 0);
            AddLabel(99, 84, 0, @"Cancel Revoke");
            AddLabel(293, 84, 0, @"Continue Revoking");
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

                        if (pm == null || pm.CitizenshipPlayerState == null)
                            return;

                        pm.CitizenshipPlayerState.Leaving = DateTime.MinValue;
                        from.SendMessage("You have cancelled the citizenship revoke request.");
                        break;
                    }
            }
        }
    }
}