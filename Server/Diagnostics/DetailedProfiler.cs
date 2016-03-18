using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using Server.Diagnostics;

namespace Server.Custom
{
	
	public static class DetailedProfiler
	{
		public static int num_frames;

		// BaseAI
		public static int ai_wander;
		public static int ai_combat;
		public static int ai_guard;
		public static int ai_flee;
		public static int ai_interact;
		public static int ai_backoff;

		public static long ticks_ai_wander;
		public static long ticks_ai_combat;
		public static long ticks_ai_guard;
		public static long ticks_ai_flee;
		public static long ticks_ai_interact;
		public static long ticks_ai_backoff;

		private static DateTime time_start;
		public static void OnStart()
		{
			// reset vars
			time_start = DateTime.Now;

			num_frames = 0;

			ai_wander = 0;
			ai_combat = 0;
			ai_guard = 0;
			ai_flee = 0;
			ai_interact = 0;
			ai_backoff = 0;

			ticks_ai_wander = 0;
			ticks_ai_combat = 0;
			ticks_ai_guard = 0;
			ticks_ai_flee = 0;
			ticks_ai_interact = 0;
			ticks_ai_backoff = 0;
		}

		public static void OnEnd()
		{
			if (num_frames <= 0)
				return;

			try
			{
				using (StreamWriter sw = new StreamWriter("detail_profiling.log", true))
				{
					sw.WriteLine("\n\n\n# Detail Dump on {0:f}", DateTime.Now);
					sw.WriteLine("# Total time elapsed: " + (DateTime.Now - time_start).ToString());
					sw.WriteLine("# Total num frames: {0}", num_frames);

					sw.WriteLine("#### BaseAI::Think");
					float ticks_per_ms = (float)Stopwatch.Frequency / 1000.0f;
					sw.WriteLine("##  Total AI Wander time: {0}ms ({1}ms avg per frame)", (ticks_ai_wander / ticks_per_ms), (ticks_ai_wander / ticks_per_ms / num_frames));
					sw.WriteLine("##  Total AI Combat time: {0}ms ({1}ms avg per frame)", (ticks_ai_combat / ticks_per_ms), (ticks_ai_combat / ticks_per_ms / num_frames));
					sw.WriteLine("##  Total AI Guard time: {0}ms ({1}ms avg per frame)", (ticks_ai_guard / ticks_per_ms), (ticks_ai_guard / ticks_per_ms / num_frames));
					sw.WriteLine("##  Total AI Flee time: {0}ms ({1}ms avg per frame)", (ticks_ai_flee / ticks_per_ms), (ticks_ai_flee / ticks_per_ms / num_frames));
					sw.WriteLine("##  Total AI Interact time: {0}ms ({1}ms avg per frame)", (ticks_ai_interact / ticks_per_ms), (ticks_ai_interact / ticks_per_ms / num_frames));
					sw.WriteLine("##  Total AI Backoff time: {0}ms ({1}ms avg per frame)", (ticks_ai_backoff / ticks_per_ms), (ticks_ai_backoff / ticks_per_ms / num_frames));

					sw.WriteLine("##  Number of AI in Wander total: {0} ({1} avg per frame)", ai_wander, (ai_wander / num_frames));
					sw.WriteLine("##  Number of AI in Combat total: {0} ({1} avg per frame)", ai_combat, (ai_wander / num_frames));
					sw.WriteLine("##  Number of AI in Guard total: {0} ({1} avg per frame)", ai_guard, (ai_guard / num_frames));
					sw.WriteLine("##  Number of AI in Flee total: {0} ({1} avg per frame)", ai_flee, (ai_flee / num_frames));
					sw.WriteLine("##  Number of AI in Interact total: {0} ({1} avg per frame)", ai_interact, (ai_interact / num_frames));
					sw.WriteLine("##  Number of AI in Backoff total: {0} ({1} avg per frame)", ai_backoff, (ai_backoff / num_frames));
				}
			}
			catch
			{
			}

			num_frames = 0;
		}
	}
}
