using System;
using Server.Network;
using Server.Mobiles;

namespace Server
{
	public class MemoryHelperThinggie : Timer
	{
		private static TimeSpan Frequency = TimeSpan.FromMinutes( 15.0 );
		public static void Initialize()
		{
			//new MemoryHelperThinggie().Start();
		}
		private MemoryHelperThinggie() : base( TimeSpan.Zero, Frequency )
		{
			Priority = TimerPriority.OneMinute;
		}
		[System.Runtime.InteropServices.DllImport( "Kernel32" )]
		private static extern uint SetProcessWorkingSetSize( IntPtr hProc, int minSize, int maxSize );
		protected override void OnTick()
		{
			foreach ( NetState ns in NetState.Instances )
			{
				Mobile m = ns.Mobile;

				if ( m != null && m.AccessLevel >= AccessLevel.Counselor && m.AutoPageNotify )
					m.SendMessage( "Memory optimization initiated..." );
			}

			SetProcessWorkingSetSize( System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1 );
		}
	}
}