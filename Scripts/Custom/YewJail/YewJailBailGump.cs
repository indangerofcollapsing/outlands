/***************************************************************************
 *                              YewJailBailGump.cs
 *                            -------------------
 *   begin                : July 2010
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/
using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Gumps;

namespace Server.YewJail
{
	public class YewJailBailGump : Gump
	{
		private Mobile m_Owner;
        private YewJailItem m_JailItem;
        public YewJailItem JailItem { get { return m_JailItem; } set { m_JailItem = value; } }
		public Mobile Owner{ get{ return m_Owner; } set{ m_Owner = value; } }

        private TimeSpan GetTimeLeft(YewJail.YewJailItem item) { return item.m_End- ((PlayerMobile)item.m_Jailed).GameTime; }

		public YewJailBailGump(YewJail.YewJailItem item) : base( 100, 10 )
		{
            Mobile owner = item.m_Jailed;
            JailItem = item;
            owner.CloseGump( typeof( YewJailBailGump ) );
			owner.CloseGump( typeof( DuelGump ) );
            TimeSpan timeLeft = GetTimeLeft(item);

			int gumpX = 0; int gumpY = 0; //bool initialState = false;

			m_Owner = owner;

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = true;

			AddPage( 0 );

			gumpX = 0; gumpY = 0;

			AddBackground( gumpX, gumpY, 450, 200, 0xE10 );

			gumpX = 15; gumpY = 15;
			AddAlphaRegion( gumpX, gumpY, 420, 170 );

			gumpX = 15; gumpY = 15;
			AddHtml( gumpX, gumpY, 420, 20, "<body text=white><center>Bail Confirmation</center></body>", false, false );

            gumpX = 15; gumpY = 50;
            AddHtml(gumpX, gumpY, 420, 20, string.Format("<body text=white><center>You currently have {0} hours and {1} minutes remaining.</center></body>", timeLeft.Hours, timeLeft.Minutes), false, false);
            
            gumpX = 15; gumpY = 70;
            AddHtml(gumpX, gumpY, 420, 20, "<body text=white><center>250,000 gold will be deducted from your bank account</center></body>", false, false);

			gumpX = 15; gumpY = 90;
            AddHtml(gumpX, gumpY, 420, 20, "<body text=white><center>in exchange for 12 hours of jailtime credit.</center></body>", false, false);

			gumpX = 125; gumpY = 140;
			AddButton( gumpX, gumpY, 0xF8, 0xF7, 2, GumpButtonType.Reply, 0 );

			gumpX = 255; gumpY = 140;
			AddButton( gumpX, gumpY, 0xF2, 0xF1, 3, GumpButtonType.Reply, 0 );

			gumpX = 15; gumpY = 110;


            AddHtml(gumpX, gumpY, 420, 20, "<body text=white><center>Do you wish to post bail?</center></body>", false, false);
			

			AddPage( 1 );
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;

			switch( info.ButtonID )
			{
				case 2:
					if ( m_Owner.BankBox.ConsumeTotal( typeof( Gold ), 250000 ) ) //OKAY
					{
                        JailItem.PostBail(TimeSpan.FromHours(12));
					}
					else
					{
						m_Owner.SendMessage( "You need 250,000 gold in your bankbox to post bail!" );
					}
					break;
				case 3:
					break;
			}
		}
	}
}
