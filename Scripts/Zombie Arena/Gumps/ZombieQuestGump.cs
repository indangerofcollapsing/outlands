using System;
using System.Collections;
using System.Collections.Generic;
using Server.Custom;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using System.Text;

namespace Server.Gumps
{
	public class ZombieTopTen : Gump
	{
		public ZombieTopTen(): base(120, 90)
		{
			string header = "Zombie Top Ten List";
			string content = buildList();
			int height = 300;
			int width = 300;

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			// Only one page.
			AddPage(0);

			// Add a transparent background.
			AddBackground(0, 0, width, height, 5054);

			// Add the header section.
			AddImageTiled(10, 10, width - 20, 20, 2624);
			AddAlphaRegion(10, 10, width - 20, 20);
			AddHtml(10, 10, width - 20, 20, String.Format("<BASEFONT COLOR=#FF0000><CENTER>{0}</CENTER></BASEFONT>", header), false, false);

			// Add the body section.
			AddImageTiled(10, 40, width - 20, height - 80, 2624);
			AddAlphaRegion(10, 40, width - 20, height - 80);
			AddHtml(10, 40, width - 20, height - 80, String.Format("<BASEFONT COLOR=#FFFFFF>{0}</BASEFONT>", content), false, false);

			// Add the button section.
			AddImageTiled(10, height - 30, width - 20, 20, 2624);
			AddAlphaRegion(10, height - 30, width - 20, 20);
			AddHtmlLocalized(40, height - 30, 120, 20, 1011036, 32767, false, false);
			AddButton(10, height - 30, 4005, 4007, 1, GumpButtonType.Reply, 0);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
		}

		public static string buildList()
		{
            StringBuilder sb = new StringBuilder();

            int count = ZombieQuestRegion.TopTen.Count;

            TimeSpan length;

            for (int i = 0; i < count; i++)
            {
                length =  ZombieQuestRegion.TopTen[i].Duration;
                sb.AppendFormat("{0:00}h {1:00}m {2:00}s : {3}<br>", (int)length.TotalHours, length.Minutes, length.Seconds, ZombieQuestRegion.TopTen[i].Player == null ? "Unknown" : ZombieQuestRegion.TopTen[i].Player.RawName);
            }

            return sb.ToString();
		}
	}
}