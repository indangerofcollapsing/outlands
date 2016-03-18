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
	public class RegisterEmailGump : Gump
	{

		public RegisterEmailGump() : base(0, 0)
		{
			Closable = true;
			Dragable = true;
			Resizable = false;
			int text_hue = 53;

            AddBackground(47, 312, 520, 200, 83);
			AddLabel(67, 320, text_hue, "Protect your account and receive updates, register now.");
			AddLabel(67, 348, text_hue, "- Your email address will not be shared with anyone.");
			AddLabel(67, 366, text_hue, "- We recommend that you register the same address used on the forums.");
			AddLabel(67, 384, text_hue, "- You can register the same address for multiple accounts.");

			AddLabel(67, 410, text_hue, "Email:");
            AddImage(118, 406, 1143);
			AddTextEntry(124, 408, 257, 20, 0, 2, "");

			AddLabel(67, 449, text_hue, "Again:");
            AddImage(118, 443, 1143);
			AddTextEntry(124, 445, 257, 20, 0, 3, "" );

			AddButton(67, 480, 4007, 4006, 1, GumpButtonType.Reply, 0);
			AddLabel(118, 480, text_hue, "Submit");
		}

        public bool ValidateEmail(string s, string s2)
        {
            if (s == "" || s2 == "")
                return false;

            if (!s.Contains("@") || !s2.Contains("@"))
                return false;

            if (s != s2)
                return false;

            return true;
        }

        public string RandomChar()
        {
            int rand = (int)Utility.Random(1, 16);

            switch (rand)
            {
                case 1:
                    {
                        return "a";
                    }
                case 2:
                    {
                        return "G";
                    }
                case 3:
                    {
                        return "e";
                    }
                case 4:
                    {
                        return "4";
                    }
                case 5:
                    {
                        return "8";
                    }
                case 6:
                    {
                        return "k";
                    }
                case 7:
                    {
                        return "M";
                    }
                case 8:
                    {
                        return "6";
                    }
                case 9:
                    {
                        return "8";
                    }
                case 10:
                    {
                        return "v";
                    }
                case 11:
                    {
                        return "J";
                    }
                case 12:
                    {
                        return "f";
                    }
                case 13:
                    {
                        return "X";
                    }
                case 14:
                    {
                        return "2";
                    }
                case 15:
                    {
                        return "3";
                    }
                case 16:
                    {
                        return "1";
                    }
            }
            return "";
        }

        public string ReturnCode()
        {
            string toreturn = "";

            for (string s = ""; s.Length < 11; s += RandomChar())
            {
                toreturn = s;
            }

            return toreturn;
        }

        public string CreateConFirmation()
        {
            if (EmailHolder.Emails == null)
                EmailHolder.Emails = new Dictionary<string, string>();
            if (EmailHolder.Confirm == null)
                EmailHolder.Confirm = new Dictionary<string, string>();
            if (EmailHolder.Codes == null)
                EmailHolder.Codes = new Dictionary<string, string>();

            string toreturn = ReturnCode();

            do
            {
                toreturn = ReturnCode();
            }
            while(EmailHolder.Codes.ContainsValue(toreturn));

            return toreturn;
        }

        public override void OnResponse(NetState sender, RelayInfo info)
		{
            if (EmailHolder.Emails == null)
                EmailHolder.Emails = new Dictionary<string, string>();
            if (EmailHolder.Confirm == null)
                EmailHolder.Confirm = new Dictionary<string, string>();
            if (EmailHolder.Codes == null)
                EmailHolder.Codes = new Dictionary<string, string>();

			Mobile from = sender.Mobile;

            switch (info.ButtonID)
            {
                case 0:
                    {
                        from.SendMessage("You will recieve this gump again next login.");
                        break;
                    }
                case 1:
                    {
                        TextRelay relay = (TextRelay)info.GetTextEntry(2);
                        string txt1 = (string)relay.Text.Trim();

                        TextRelay relay2 = (TextRelay)info.GetTextEntry(3);
                        string txt2 = (string)relay2.Text.Trim();

                        if (ValidateEmail(txt1, txt2))
                        {
                            string c = CreateConFirmation();

                            Account acct = (Account)from.Account;

                            string test = (string)acct.Username;

                            string email = txt1;

                            if (!EmailHolder.Confirm.ContainsKey(test))
                            {
                                EmailHolder.Confirm.Add(test, txt1);
                                EmailHolder.Codes.Add(test, c);
                            }
                            else
                            {
                                EmailHolder.Confirm.Remove(test);
                                EmailHolder.Codes.Remove(test);

                                EmailHolder.Confirm.Add(test, txt1);
                                EmailHolder.Codes.Add(test, c);
                            }

                            string msg = "Welcome to UO An Corp, the best Ultima Online experience available.\nUse the confirmation code below to confirm your email address.\n\n" + c + "\n\nTo confirm your email address type \"[auth\" in game and enter the code exactly as it appears below into the text box provided.\n\n\nThank you,\nThe UOAC Dev Team";

                            EmailEventArgs eea = new EmailEventArgs(true, null, email, "Email Registration", msg);

                            RegisterEmailClient.SendMail(eea);
                            from.SendMessage("You have been sent a confirmation code, it will remain valid until you log out.");
                        }
                        else
                        {
                            from.SendMessage("That does not seem to be a valid email address.");
                            from.SendGump(new RegisterEmailGump());
                        }
                        break;
                    }
            }
		}
	}
}
