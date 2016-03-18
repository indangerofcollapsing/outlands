using System;
using Server;
using Server.Gumps;

namespace Server.Gumps
{
	public class WorldSaveGump : Gump
	{
		public WorldSaveGump() : base( 200, 200 )
		{
			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage(0);
			AddBackground( 0, 0, 400, 150, 5054 );
			AddImageTiled(10, 10, 380, 130, 2624 );
			AddAlphaRegion( 10, 10, 380, 130 );
			AddHtml( 80, 30, 290, 60, "<basefont color=#FFFFFF>The world is currently saving. This may take a few moments. This menu will disappear once the worldsave has completed.</basefont>", false, false );
			AddItem( 30, 35, 4171 );
			AddButton( 140, 110, 9905, 9903, 1, GumpButtonType.Reply, 0 );
			AddButton( 330, 108, 9905, 9903, 2, GumpButtonType.Reply, 0 );
			AddLabel( 40, 110, 1071, @"UOAC Homepage" );
            AddLabel(229, 110, 1071, @"Vote for UOAC");

		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
			switch( info.ButtonID )
			{
				case 1:
				{
					sender.LaunchBrowser( "http://www.uoancorp.com" );
					break;
				}
				case 2:
				{
					sender.LaunchBrowser( "http://www.uoancorp.com" );
					break;
				}
			}
		}
	}
}