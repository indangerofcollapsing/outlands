using System;
using Server;
using Server.Commands;
using Server.Items;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;
using Server.Accounting;
using Server.Network;
using Server.Misc;
using Server.Multis;
using Server.Targeting;
using Server.Gumps;
using System.Net.Mail;
using System.Threading;
using System.Net;

namespace Server.Gumps
{
	public class ChangePasswordGump : Gump
	{
		public ChangePasswordGump() : base(0, 0)
		{
			Closable = true;
			Dragable = true;
			Resizable = false;

			AddBackground( 203, 176, 415, 171, 9300 );
            AddBackground(209, 233, 402, 6, 9350);
			AddLabel(350, 182, 0, "Change Password");

			AddLabel(211, 208, 0, "Old Password:");
            AddImage(332, 204, 1143);
            AddTextEntry(340, 204, 257, 25, 0, 2, "");

			AddLabel(215, 249, 0, "New Password:");
            AddImage(333, 245, 1143);
            AddTextEntry(341, 246, 257, 25, 0, 3, "");

			AddLabel(218, 285, 0, "Confirm:");
            AddImage(333, 279, 1143);
            AddTextEntry(341, 280, 256, 25, 0, 4, "");

            AddButton(277, 316, 4005, 4006, 1, GumpButtonType.Reply, 0);
            AddLabel(309, 317, 0, "Submit");
		}

        public bool ValidatePassword(PlayerMobile m, string old, string new1, string new2)
        {
            Account acct = (Account)m.Account;

			if (!acct.CheckPassword(old))
			{
				m.SendMessage("Wrong old password");
				return false;
			}
			if (new1 == "")
			{
				m.SendMessage("You must enter a new password");
				return false;
			}
			if (new1 != new2)
			{
				m.SendMessage("New passwords does not match");
				return false;
			}
            return true;
        }

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			PlayerMobile from = (PlayerMobile)sender.Mobile;

            Account acct = (Account)from.Account;

            string key = (string)acct.Username;

            switch (info.ButtonID)
            {
                case 0:
                    {
                        from.SendMessage("You decide not to change your password.");
                        break;
                    }
                case 1:
                    {
                        TextRelay txt = (TextRelay)info.GetTextEntry(2);
                        string s = (string)txt.Text.Trim();

                        TextRelay txt2 = (TextRelay)info.GetTextEntry(3);
                        string s2 = (string)txt2.Text.Trim();

                        TextRelay txt3 = (TextRelay)info.GetTextEntry(4);
                        string s3 = (string)txt3.Text.Trim();

                        if (ValidatePassword(from, s, s2, s3))
                        {
                            acct.SetPassword(s2);
                            from.SendMessage("Your password has been changed.");

                            string msg = "A notification from UO An Corp.\n\nThe password for account \"" + acct.Username + "\" was changed from within the game." + "\n\nThe password change occurred on " + DateTime.UtcNow.ToString() + " server time.\n\n\nThank you,\nThe UOAC Dev Team";
                            string email = (string)EmailHolder.Emails[acct.Username];
                            EmailEventArgs eea = new EmailEventArgs(true, null, email, "Password Changed", msg);
                            RegisterEmailClient.SendMail(eea);
                        }

                        break;
                    }
            }
		}
	}
}
