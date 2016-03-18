using System;
using System.Net;
using System.Text;
using System.IO;
using System.Collections;
using System.Diagnostics;
using Server;
using Server.Items;
using Server.Prompts;
using Server.Network;
using Server.Accounting;

namespace Server.Misc
{
	public class MetricFetch : Timer
	{
		private static TimeSpan m_Delay = TimeSpan.FromMinutes( 60.0 );

		public static void Configure()
		{
			EventSink.WorldLoad += new WorldLoadEventHandler( OnLoad ); 
		}

		private static void OnLoad() 
		{
			new MetricFetch().Start();
		}

		public MetricFetch() : base( m_Delay, m_Delay )
		{
			Priority = TimerPriority.OneMinute;
		}

		public static string FormatTimeSpan( TimeSpan ts )
		{
			return String.Format( "{0:D2}:{1:D2}:{2:D2}:{3:D2}", ts.Days, ts.Hours % 24, ts.Minutes % 60, ts.Seconds % 60 );
		}

		public static string FormatByteAmount( long totalBytes )
		{
			if ( totalBytes > 1000000000 )
				return String.Format( "{0:F1} GB", (double)totalBytes / 1073741824 );

			if ( totalBytes > 1000000 )
				return String.Format( "{0:F1} MB", (double)totalBytes / 1048576 );

			if ( totalBytes > 1000 )
				return String.Format( "{0:F1} KB", (double)totalBytes / 1024 );

			return String.Format( "{0} Bytes", totalBytes );
		}

		protected override void OnTick()
		{
			string file_path = Path.Combine( Core.BaseDirectory, "Data/metrics.txt" );

			try
			{
				if ( File.Exists( file_path ) )
				{

					using ( StreamWriter sw = File.AppendText( file_path ) )
					{
						int banned = 0;
						int active = 0;

						foreach ( Account acct in Accounts.GetAccounts() )
						{
							if ( acct.Banned )
								++banned;
							else
								++active;
						}

						sw.WriteLine("Save Information: {0}", DateTime.UtcNow);
						sw.WriteLine( active.ToString() );
						sw.WriteLine( banned.ToString() );
						sw.WriteLine( Firewall.List.Count.ToString() );
						sw.WriteLine( Server.RemoteAdmin.ServerInfo.NetStateCount().ToString() );
						sw.WriteLine( World.Mobiles.Count.ToString() );
						sw.WriteLine( Core.ScriptMobiles.ToString() );
						sw.WriteLine( World.Items.Count.ToString());
						sw.WriteLine( Core.ScriptItems.ToString() );
						sw.WriteLine(FormatTimeSpan(DateTime.UtcNow - Clock.ServerStart));
						sw.WriteLine( FormatByteAmount( GC.GetTotalMemory( false ) ) );
						sw.WriteLine( "" );
						sw.WriteLine( "" );
						sw.Close();
					}
				}
			}
			catch
			{
			}
		}
	}
}