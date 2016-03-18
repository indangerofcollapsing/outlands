using System;
using Server;
using Server.Network;
using Server.Mobiles;

namespace Server.Gumps
{
	public class QuestConversationGump : Gump
	{
		public QuestConversationGump( string text ) : base( 30, 50 )
		{
			AddPage( 0 );
			Closable = false;

			AddImage( 349, 10, 0x24B0 );
			AddImageTiled( 349, 130, 100, 120, 0x24B3 );
			AddImageTiled( 149, 10, 200, 140, 0x24AF );
			AddImageTiled( 149, 250, 200, 140, 0x24B5 );
			AddImage( 349, 250, 0x24B6 );
			AddImage( 35, 10, 0x24AE );
			AddImageTiled( 35, 150, 120, 100, 0x24B1 );
			AddImage( 35, 250, 0x24B4 );
			AddHtml( 110, 60, 200, 20, "<basefont color=#FFFFFF>Quest Conversation</basefont>" , false, false );
			AddImage( 65, 14, 0x2776 );
			AddImageTiled( 81, 14, 349, 17, 0x2775 );
			AddImage( 426, 14, 0x2778 );
			AddImageTiled( 55, 40, 388, 323, 0xA40 );
			AddAlphaRegion( 55, 40, 388, 323 );
			AddImageTiled( 75, 90, 200, 1, 0x238D );
			AddImage( 75, 58, 0x2635 );
			AddImage( 380, 45, 0xDF );
			AddButton( 220, 335, 0x909, 0x908, 0x1, GumpButtonType.Reply, 0 );
			AddImage( 0, 0, 0x28C8 );
			AddHtml( 70, 110, 365, 220, text, false, true );
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
		}
	}
}