using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Guilds;

namespace Server.Gumps
{
	public class ReprieveGump : Gump
	{
		private Mobile m_From;

		public ReprieveGump( Mobile from ) : base( 20, 20 )
		{
			m_From = from;

			AddPage(0);
			AddBackground(0, 0, 300, 200, 9270);
			AddAlphaRegion(15, 15, 270, 170);
			AddHtml( 15, 15, 270, 20, @"<body text=white><center>Murderer Reprieve</center></body>", (bool)false, (bool)false);
			AddHtml( 15, 40, 270, 100, @"<body text=white><center>You have a one-time opportunity to reset your kill counts to 4, would you like to do so at this time? Please note that this menu will only appear 1 time, so choose wisely.</center></body>", (bool)false, (bool)false);
			AddButton(55, 154, 4005, 4007, 1, GumpButtonType.Reply, 0);
			AddButton(160, 154, 4017, 4019, 2, GumpButtonType.Reply, 0);
			AddLabel(90, 155, 2100, @"Accept");
			AddLabel(195, 154, 2100, @"Deny");
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			switch( info.ButtonID )
			{	
				case 1:
				{
					m_From.ShortTermMurders = 4;
					m_From.ShortTermMurders = 4;
					m_From.SendMessage( "You have accepted a reprieve." );
					((PlayerMobile)m_From).CanReprieve = 1;
					break;
				}
				case 2:
				{
					((PlayerMobile)m_From).CanReprieve = 1;
					m_From.SendMessage( "You have denied a reprieve." );

					if ( m_From.Guild != null && m_From.ShortTermMurders >= 5 && m_From.Guild.Type != GuildType.Regular )
					{
						m_From.SendMessage( "You have been removed from your Order/Chaos Guild" );
						((Guild)m_From.Guild).RemoveMember( m_From );
					}
					break;
				} 
			}
		}
	}
}