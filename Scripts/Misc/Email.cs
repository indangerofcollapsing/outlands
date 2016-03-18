using System;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net;

using Server;

namespace Server.Misc
{
	public class Email
	{
		/* In order to support emailing, fill in EmailServer:
		 * Example:
		 *  public static readonly string EmailServer = "mail.domain.com";
		 * 
		 * If you want to add crash reporting emailing, fill in CrashAddresses:
		 * Example:
		 *  public static readonly string CrashAddresses = "first@email.here;second@email.here;third@email.here";
		 * 
		 * If you want to add speech log page emailing, fill in SpeechLogPageAddresses:
		 * Example:
		 *  public static readonly string SpeechLogPageAddresses = "first@email.here;second@email.here;third@email.here";
		 */

		public static readonly string EmailServer = "mail.google.com";
		public static readonly MailAddress FromAddress = new MailAddress("ipycrashes@gmail.com", "IPY Dev Team");
		public static readonly string FromPassword = "WeakPassword2014";
		public static readonly string CrashAddresses = "ipycrashes@gmail.com, flackman.johan@gmail.com, michaelkemski@gmail.com";
		public static readonly string SpeechLogPageAddresses = null;

		private static Regex _pattern = new Regex( @"^[a-z0-9.+_-]+@([a-z0-9-]+.)+[a-z]+$", RegexOptions.IgnoreCase );

		public static bool IsValid( string address )
		{
			if ( address == null || address.Length > 320 )
				return false;

			return _pattern.IsMatch( address );
		}

		public static SmtpClient Client;

		public static void Configure()
		{
			if ( EmailServer != null )
				Client = new SmtpClient
				{
					Host = "smtp.gmail.com",
					Port = 587,
					EnableSsl = true,
					DeliveryMethod = SmtpDeliveryMethod.Network,
					UseDefaultCredentials = false,
					Credentials = new NetworkCredential(FromAddress.Address, FromPassword)
				};
		}

		public static bool Send( MailMessage message )
		{
			try
			{
				lock( Client ){
					Client.Send( message );
				}
			}
			catch( Exception exc)
			{
				Console.Write(exc.ToString());
				return false;
			}

			return true;
		}

		public static void AsyncSend( MailMessage message )
		{
			ThreadPool.QueueUserWorkItem( new WaitCallback( SendCallback ), message );
		}

		private static void SendCallback( object state )
		{
			MailMessage message = (MailMessage) state;

			if ( Send( message ) )
				Console.WriteLine( "Sent e-mail '{0}' to '{1}'.", message.Subject, message.To );
			else
				Console.WriteLine( "Failure sending e-mail '{0}' to '{1}'.", message.Subject, message.To );
		}
	}
}