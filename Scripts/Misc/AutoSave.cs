using System;
using System.IO;
using Server;
using Server.Commands;

namespace Server.Misc
{
	public class AutoSave : Timer
    {       
		public static TimeSpan m_Delay = TimeSpan.FromMinutes( 10 );

		private static TimeSpan m_Warning = TimeSpan.FromSeconds(15);
		//private static TimeSpan m_Warning = TimeSpan.FromSeconds( 15.0 );

		public static void Initialize()
		{
			new AutoSave().Start();
			CommandSystem.Register( "SetSaves", AccessLevel.Administrator, new CommandEventHandler( SetSaves_OnCommand ) );
		}

		private static bool m_SavesEnabled = true;

		public static bool SavesEnabled
		{
			get{ return m_SavesEnabled; }
			set{ m_SavesEnabled = value; }
		}

		[Usage( "SetSaves <true | false>" )]
		[Description( "Enables or disables automatic shard saving." )]
		public static void SetSaves_OnCommand( CommandEventArgs e )
		{
			if ( e.Length == 1 )
			{
				m_SavesEnabled = e.GetBoolean( 0 );
				e.Mobile.SendMessage( "Saves have been {0}.", m_SavesEnabled ? "enabled" : "disabled" );
			}
			else
			{
				e.Mobile.SendMessage( "Format: SetSaves <true | false>" );
			}
		}

		public AutoSave() : base( m_Delay - m_Warning, m_Delay )
		{
			Priority = TimerPriority.OneMinute;
		}

		protected override void OnTick()
		{
			if ( !m_SavesEnabled || AutoRestart.Restarting )
				return;

			if ( m_Warning == TimeSpan.Zero )
			{
				Save( true );
			}
			else
			{
				int s = (int)m_Warning.TotalSeconds;
				int m = s / 60;
				s %= 60;

				if ( m > 0 && s > 0 )
					World.Broadcast( 0x35, true, "The world will save in {0} minute{1} and {2} second{3}.", m, m != 1 ? "s" : "", s, s != 1 ? "s" : "" );
				else if ( m > 0 )
					World.Broadcast( 0x35, true, "The world will save in {0} minute{1}.", m, m != 1 ? "s" : "" );
				else
					World.Broadcast( 0x35, true, "The world will save in {0} second{1}.", s, s != 1 ? "s" : "" );

				Timer.DelayCall( m_Warning, new TimerCallback( Save ) );
			}
		}

		public static void Save()
		{
			AutoSave.Save( false );
		}

		public static void Save( bool permitBackgroundWrite )
		{
			if ( AutoRestart.Restarting )
				return;

			World.WaitForWriteCompletion();

			try{ Backup(); }
            catch (Exception e) { Console.WriteLine("WARNING: Automatic backup FAILED: {0}", e); }

			World.Save( true, permitBackgroundWrite );

            try { BackupBatScript(); }
            catch (Exception e) { Console.WriteLine("WARNING: Backup Bat Script FAILED: {0}", e); }
		}

        private static string BackupScript = "backup.bat";
        private static string FullPath = AppDomain.CurrentDomain.BaseDirectory;
        private static DateTime LastBackup = DateTime.MinValue;
        private static TimeSpan BetweenBackups = TimeSpan.FromMinutes(30);

        private static void BackupBatScript()
        {
            string path = string.Join("", FullPath, BackupScript);
            if (File.Exists(path) && DateTime.UtcNow > LastBackup + BetweenBackups)
            {
                System.Diagnostics.Process.Start(path);
                LastBackup = DateTime.UtcNow;            	
            }
        }

		private static string[] m_Backups = new string[]
			{


				"70th Backup",
				"69th Backup",
				"68th Backup",
				"67th Backup",
				"66th Backup",
				"65th Backup",
				"64th Backup",
				"63rd Backup",
				"62nd Backup",
				"61st Backup",
				"60th Backup",
				"59th Backup",
				"58th Backup",
				"57th Backup",
				"56th Backup",
				"55th Backup",
				"54th Backup",
				"53rd Backup",
				"52nd Backup",
				"51th Backup",
				"50th Backup",
				"49th Backup",
				"48th Backup",
				"47th Backup",
				"46th Backup",
				"45th Backup",
				"44th Backup",
				"43rd Backup",
				"42nd Backup",
				"41st Backup",
				"40th Backup",
				"39th Backup",
				"38th Backup",
				"37th Backup",
				"36th Backup",
				"35th Backup",
				"34th Backup",
				"33rd Backup",
				"32nd Backup",
				"31st Backup",
				"30th Backup",
				"29th Backup",
				"28th Backup",
				"27th Backup",
				"26th Backup",
				"25th Backup",
				"24th Backup",
				"23rd Backup",
				"22nd Backup",
				"21st Backup",
				"20th Backup",
				"19th Backup",
				"18th Backup",
				"17th Backup",
				"16th Backup",
				"15th Backup",
				"14th Backup",
				"13th Backup",
				"12th Backup",
				"11th Backup",
				"10th Backup",
				"9th Backup",
				"8th Backup",
				"7th Backup",
				"6th Backup",
				"5th Backup",
				"Fourth Backup",
				"Third Backup",
				"Second Backup",
				"Most Recent"
			};

		private static void Backup()
		{
			if ( m_Backups.Length == 0 )
				return;

			string root = Path.Combine( Core.BaseDirectory, "Backups/Automatic" );

			if ( !Directory.Exists( root ) )
				Directory.CreateDirectory( root );

			string[] existing = Directory.GetDirectories( root );

			for ( int i = 0; i < m_Backups.Length; ++i )
			{
				DirectoryInfo dir = Match( existing, m_Backups[i] );

				if ( dir == null )
					continue;

				if ( i > 0 )
				{
					string timeStamp = FindTimeStamp( dir.Name );

					if ( timeStamp != null )
					{
						try{ dir.MoveTo( FormatDirectory( root, m_Backups[i - 1], timeStamp ) ); }
						catch{}
					}
				}
				else
				{
					try{ dir.Delete( true ); }
					catch{}
				}
			}

			string saves = Path.Combine( Core.BaseDirectory, "Saves" );

			if ( Directory.Exists( saves ) )
				Directory.Move( saves, FormatDirectory( root, m_Backups[m_Backups.Length - 1], GetTimeStamp() ) );
		}

		private static DirectoryInfo Match( string[] paths, string match )
		{
			for ( int i = 0; i < paths.Length; ++i )
			{
				DirectoryInfo info = new DirectoryInfo( paths[i] );

				if ( info.Name.StartsWith( match ) )
					return info;
			}

			return null;
		}

		private static string FormatDirectory( string root, string name, string timeStamp )
		{
			return Path.Combine( root, String.Format( "{0} ({1})", name, timeStamp ) );
		}

		private static string FindTimeStamp( string input )
		{
			int start = input.IndexOf( '(' );

			if ( start >= 0 )
			{
				int end = input.IndexOf( ')', ++start );

				if ( end >= start )
					return input.Substring( start, end-start );
			}

			return null;
		}

		private static string GetTimeStamp()
		{
			DateTime now = DateTime.UtcNow;

			return String.Format( "{0}-{1}-{2} {3}-{4:D2}-{5:D2}",
					now.Day,
					now.Month,
					now.Year,
					now.Hour,
					now.Minute,
					now.Second
				);
		}
	}
}