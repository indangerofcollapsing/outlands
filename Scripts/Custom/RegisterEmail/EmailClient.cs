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

namespace Server.Misc
{
    public class RegisterEmailClient
    {
        public static bool Enabled = false; // Is this system enabled?

        public static string ServerName = "UO An Corp";

		public static string EmailServer = "server.inporylem.com";	// outgoing server
		public static int OutgoingPort = 25;						// server port
		public static string User = "account@inporylem.com";		// email username
		public static string Pass = "6Eosf~T@lN1{";					// email password
		public static string YourAddress = "account@inporylem.com"; // Sender address email address here
		public static bool UseSSL = false;

        public static SmtpClient m_Client;
        public static MailMessage m_MailMessage;

        public static void Initialize()
        {
            if (Enabled)
            {
				m_Client = new SmtpClient
				{
					Host = EmailServer,
					Port = OutgoingPort,
					EnableSsl = UseSSL,
					DeliveryMethod = SmtpDeliveryMethod.Network,
					UseDefaultCredentials = false,
					Credentials = new NetworkCredential(User, Pass)
				};

                m_Client.Credentials = new NetworkCredential(User, Pass);

                m_MailMessage = new MailMessage();
                m_MailMessage.Subject = ServerName;
				m_MailMessage.From = new MailAddress(YourAddress, ServerName);
            }
        }

        public static void SendMail(EmailEventArgs e)
        {
            bool single = e.Single;

            if (single)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(SendSingle), e);
            }
            else
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(SendMultiple), e);
            }

            return;
        }

        private static void SendMultiple(object e)
        {
            EmailEventArgs eea = (EmailEventArgs)e;

            List<MailAddress> emails = (List<MailAddress>)eea.Emails;
            string sub = (string)eea.Subject;
            string msg = (string)eea.Message;

            for (int i = 0; i < emails.Count; ++i)
            {
                MailAddress ma = (MailAddress)emails[i];

                m_MailMessage.To.Add(ma);
            }

            m_MailMessage.Subject += " - " + sub;
            m_MailMessage.Body = msg;

            try
            {
                m_Client.Send(m_MailMessage);
            }
            catch { }
            m_MailMessage.To.Clear();
            m_MailMessage.Body = "";
            m_MailMessage.Subject = ServerName;

            return;
        }

        private static void SendSingle(object e)
        {
            EmailEventArgs eea = (EmailEventArgs)e;

            string to = (string)eea.To;
            string sub = (string)eea.Subject;
            string msg = (string)eea.Message;

            m_MailMessage.To.Add(to);
            m_MailMessage.Subject += " - " + sub;
            m_MailMessage.Body = msg;

            try
            {
                m_Client.Send(m_MailMessage);
            }
            catch { }
            m_MailMessage.To.Clear();
            m_MailMessage.Body = "";
            m_MailMessage.Subject = ServerName;

            return;
        }
    }

    public class EmailEventArgs
    {
        public bool Single;
        public List<MailAddress> Emails;
        public string To;
        public string Subject;
        public string Message;

        public EmailEventArgs(bool single, List<MailAddress> list, string to, string sub, string msg)
        {
            Single = single;
            Emails = list;
            To = to;
            Subject = sub;
            Message = msg;
        }
    }
}