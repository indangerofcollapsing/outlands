using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Gumps
{
	public class DuelGump : Gump
	{
		private Mobile m_Owner;
		public Mobile Owner{ get{ return m_Owner; } set{ m_Owner = value; } }


		public DuelGump(Mobile owner) : base( 100, 10 )
		{
			owner.CloseGump( typeof( SpectateGump ) );
			owner.CloseGump( typeof( DuelGump ) );

			int gumpX = 0; int gumpY = 0; //bool initialState = false;

			m_Owner = owner;

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = true;

			AddPage( 0 );

			gumpX = 0; gumpY = 0;

			AddBackground( gumpX, gumpY, 300, 200, 0xE10 );

			gumpX = 15; gumpY = 15;
			AddAlphaRegion( gumpX, gumpY, 270, 170 );

			gumpX = 15; gumpY = 15;
			AddHtml( gumpX, gumpY, 270, 20, "<body text=white><center>Participation Confirmation</center></body>", false, false );

			gumpX = 40; gumpY = 70;
			AddLabel( gumpX, gumpY, 1071, "Are you sure you wish to participate?" );

			gumpX = 60; gumpY = 90;
			AddLabel( gumpX, gumpY, 1071, "10,000 gold will be withdrawn" );

			gumpX = 50; gumpY = 140;
			AddButton( gumpX, gumpY, 0xF8, 0xF7, 2, GumpButtonType.Reply, 0 );

			gumpX = 180; gumpY = 140;
			AddButton( gumpX, gumpY, 0xF2, 0xF1, 3, GumpButtonType.Reply, 0 );

			gumpX = 90; gumpY = 110;
			AddLabel( gumpX, gumpY, 1071, "from your bankbox." );
			

			AddPage( 1 );
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;

			switch( info.ButtonID )
			{
				case 2:	
					if ( m_Owner.ShortTermMurders > 5 || m_Owner.BankBox.ConsumeTotal( typeof( Gold ), 10000 ) )
					{
						m_Owner.Location = new Point3D( 5732, 1129, 0 );
						m_Owner.SendMessage( "Thanks for participating, and good luck!" );
						TournamentNPC.TourneyPot += 10000;
						++TournamentNPC.CurrentParticipants;
					}
					else
					{
						m_Owner.SendMessage( "You need 10,000 gold in your bankbox to participate in this tournament!" );
					}
					break;
				case 3:
					break;
			}
		}
	}
}
