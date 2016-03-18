using System;
using System.Collections.Generic;
using Server;
using Server.Network;
using Server.Mobiles;

namespace Server.Misc
{
	public static class SpeechThrottle
	{
		public static void Initialize()
		{
			PacketHandlers.RegisterThrottler( 0x03, new ThrottlePacketCallback( SpeechThrottle_Callback ) );
			PacketHandlers.RegisterThrottler( 0xAD, new ThrottlePacketCallback( SpeechThrottle_Callback ) );
		}

		private static long spam_threshold = 5; // Higher value == more sensitive

		// This quick and dirty code will only kick the heaviest spammers
		private static Dictionary<PlayerMobile,long> _speech = new Dictionary<PlayerMobile,long>();
		private static Dictionary<PlayerMobile,long> _floodcount = new Dictionary<PlayerMobile,long>();
		public static bool SpeechThrottle_Callback( NetState ns )
		{
			PlayerMobile pm = ns.Mobile as PlayerMobile;

			if (pm == null || pm.AccessLevel >= AccessLevel.Counselor)
				return true;

			if ( !_speech.ContainsKey(pm) )
			{
				_speech[pm] = Core.TickCount;
				return true;
			}

			long last = 0;

			if (!_speech.TryGetValue(pm, out last))
				return false;

			long ts = Core.TickCount - last;

			if (ts >= spam_threshold)
			{
				_floodcount.Remove(pm);
				_speech[pm] = Core.TickCount;
				return true;
			}

			long c = 0;
			_floodcount.TryGetValue(pm, out c);
			if (c == 0)
				_floodcount[pm] = 1;
			else
				_floodcount[pm]++;

			if (c == 100) 
			{
				_floodcount.Remove(pm);
				Console.WriteLine("SPEECH FLOODING: {0} ({1})", pm.Name, ns);
				ns.Dispose();
			}
		
			return false;
		}
	}
}