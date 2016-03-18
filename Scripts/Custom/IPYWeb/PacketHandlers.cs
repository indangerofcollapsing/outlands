using System;
using System.Collections;
using Server;
using Server.Network;
using Server.Accounting;
using Server.Items;
using Server.Mobiles;
using System.Text;

namespace Server.IPYWeb
{
	public static class WebPacketHandlers
	{
        public static void Initialize()
		{
            RemoteAdmin.RemoteAdminHandlers.Register(0xA0, new OnPacketReceive(UpdatePassword));
            RemoteAdmin.RemoteAdminHandlers.Register(0xA1, new OnPacketReceive(RegisterEmail));
            RemoteAdmin.RemoteAdminHandlers.Register(0xA2, new OnPacketReceive(ServerStatisticsRequest));
		}

        private static string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * Utility.RandomDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        public static WebAckResponse PasswordChangeRequest(NetState state, string un, string pw, string newPW)
        {
            Console.WriteLine("password change request");
            Account acct = Accounts.GetAccount(un) as Account;

            if (acct == null)
            {
                Console.WriteLine("Login: {0}: Invalid web password change request for username '{1}'", state, un);
                return WebAckResponse.NoUser;
            }
            else if (!acct.HasAccess(state))
            {
                Console.WriteLine("Login: {0}: Access denied to web change password for '{1}'", state, un);
                return WebAckResponse.NoAccess;
            }
            else if (!acct.CheckPassword(pw))
            {
                Console.WriteLine("Login: {0}: Invalid login password attempting to change password for '{1}'", state, un);
                return WebAckResponse.BadPass;
            }
            else if (acct.Banned)
            {
                Console.WriteLine("Login: {0}: Attempting to change password for Banned account '{1}'", state, un);
                return WebAckResponse.Banned;
            }
            else
            {
                Console.WriteLine("Login: {0}: Valid credentials to change password for '{1}'", state, un);
                acct.SetPassword(newPW);
                return WebAckResponse.OK;
            }
        }
        private static void UpdatePassword(NetState state, PacketReader pvSrc)
		{
			string username = pvSrc.ReadString(30);
			string pass = pvSrc.ReadString(30);
            string newpass = pvSrc.ReadString(30);
            state.Send(new PasswordChangeAckPacket(PasswordChangeRequest(state, username, pass, newpass)));
		}

        public static WebAckResponse EmailRegistrationRequest(NetState state, string un, string pw, string email, string generated)
        {
            Account acct = Accounts.GetAccount(un) as Account;

            if (acct == null)
            {
                Console.WriteLine("IpyWeb: {0}: Invalid web email registration request for username '{1}'", state, un);
                return WebAckResponse.NoUser;
            }
            else if (!acct.HasAccess(state))
            {
                Console.WriteLine("IpyWeb: {0}: Access denied to web email registration for '{1}'", state, un);
                return WebAckResponse.NoAccess;
            }
            else if (!acct.CheckPassword(pw))
            {
                Console.WriteLine("IpyWeb: {0}: Invalid login password attempting to register email for '{1}'", state, un);
                return WebAckResponse.BadPass;
            }
            else if (acct.Banned)
            {
                Console.WriteLine("IpyWeb: {0}: Attempting to register email for Banned account '{1}'", state, un);
                return WebAckResponse.Banned;
            }
            else if (acct.EmailAddress != String.Empty && acct.GeneratedCode == "emailregistrationcomplete")
            {
                Console.WriteLine("IpyWeb: {0}: Attempting to register email for account already registered '{1}'", state, un);
                return WebAckResponse.AlreadyRegistered;
            }
            else
            {
                acct.GeneratedCode = generated;
                acct.EmailAddress = email;
                Console.WriteLine("IpyWeb: {0}: Set generated code {1} for email address {2}", state, generated, email);
                return WebAckResponse.OK;
            }
        }
        private static void RegisterEmail(NetState state, PacketReader pvSrc)
        {
            string username = pvSrc.ReadString(30);
            string pass = pvSrc.ReadString(30);
            string email = pvSrc.ReadString(30);
            string generated = pvSrc.ReadString(15);
            state.Send(new RegisterEmailAckPacket(EmailRegistrationRequest(state, username, pass, email, generated)));
        }

        private static void ServerStatisticsRequest(NetState state, PacketReader pvSrc)
        {
            Console.WriteLine("sending server stats...");
            Packet p = new IPYServerStatisticsPacket();
            state.Send(p);
            Console.WriteLine();
            Console.WriteLine(p);
            Console.WriteLine();
            Packet.Release(p);
        }
	}
}
