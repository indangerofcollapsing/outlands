using System;
using Server;
using System.Net;
using System.Text;
using Server.Gumps;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Misc;
using Server.Accounting;
using Server.Commands;

namespace Server.Gumps
{
	public class PlayerSearchGump : Gump
	{
		private Mobile m_From;
		private int m_ListPage;
		private int m_TotalPages;
		private List<NetState> m_List;

		public void AddBlackAlpha( int x, int y, int width, int height )
		{
			AddImageTiled( x, y, width, height, 2624 );
			AddAlphaRegion( x, y, width, height );
		}

		public int GetButtonID( int type, int index )
		{
			return 1 + (index * 10) + type;
		}

		public static int GetHueFor( Mobile m )
		{
			if ( m == null )
				return 0x47D;

			switch ( m.AccessLevel )
			{
				case AccessLevel.Administrator: return 0x516;
				case AccessLevel.Seer: return 0x144;
				case AccessLevel.GameMaster: return 0x21;
				case AccessLevel.Counselor: return 0x2;
				case AccessLevel.Player: default:
				{
					if ( m.ShortTermMurders >= 5 )
						return 0x21;
					else if ( m.Criminal )
						return 0x3B1;

					return 0x58;
				}
			}
		}

		public static void Initialize()
		{
			CommandSystem.Register( "PlayerSearch", AccessLevel.Administrator, new CommandEventHandler( PlayerSearch_OnCommand ) );
		}

		[Usage( "PlayerSearch" )]
		[Aliases( "PS" )]
		[Description( "Opens an interface providing searchable player lists." )]
		public static void PlayerSearch_OnCommand( CommandEventArgs e )
		{
			e.Mobile.SendGump( new PlayerSearchGump( e.Mobile, 0, null ) );
		}

		public PlayerSearchGump( Mobile from, int listPage, List<NetState> list ) : base( 20, 20 )
		{
			from.CloseGump( typeof( PlayerSearchGump ) );
			m_From = from;
			m_ListPage = listPage;
			m_List = list;
			
			Resizable = false;
			AddPage( 0 );
			AddBackground( 0, 0, 280, 370, 5054 );
			AddBlackAlpha( 10, 10, 260, 350 );

			if ( list != null )
			{
				m_TotalPages = (int)( list.Count / 12 );

				if ( m_TotalPages < 1 )
					m_TotalPages = 1;

				AddHtml( 10, 11, 260, 25, "<body text=\"#FFFFFF\"><Center>" + String.Format( "Page {0} of {1} ( {2} Users Online )", m_ListPage + 1, m_TotalPages, Server.RemoteAdmin.ServerInfo.NetStateCount() ) + "</center></body>", false, false );
			}
			else
				AddHtml( 10, 11, 260, 25, "<body text=\"#FFFFFF\"><Center>Please enter a search string below.</center></body>", false, false );

			AddImageTiled( 10, 100, 260, 10, 5058 );
			AddTextEntry( 30, 40, 225, 20, 0x47D, 0, null );
			AddImageTiled( 25, 40, 230, 1, 3504 );
			AddImageTiled( 25, 60, 230, 1, 3504 );
			AddImageTiled( 25, 40, 1, 20, 3504 );
			AddImageTiled( 255, 40, 1, 21, 3504 );
			AddButton( 145, 70, 4008, 4010, 2, GumpButtonType.Reply, 0 );
			AddButton( 25, 70, 4011, 4013, 1, GumpButtonType.Reply, 0 );
			AddLabel( 60, 70, 0x47D, @"Find Player" );
			AddLabel( 180, 70, 0x47D, @"List Players" );
			AddLabel( 12, 120, 0x47D, @"Player Name" );
			AddLabel( 130, 120, 0x47D, @"IP Address" );

			if ( list != null )
			{
				if ( listPage > 0 )
					AddButton( 230, 120, 0x15E3, 0x15E7, 3, GumpButtonType.Reply, 0 );
				else
					AddImage( 230, 120, 0x25EA );

				if ( (listPage + 1) * 12 < list.Count )
					AddButton( 250, 120, 0x15E1, 0x15E5, 4, GumpButtonType.Reply, 0 );
				else
					AddImage( 250, 120, 0x25E6 );

				for ( int i = 0, index = (listPage * 12); i < 12 && index >= 0 && index < list.Count; ++i, ++index )
				{
					NetState ns = list[index] as NetState;

					if ( ns == null )
						continue;
					else
					{
						Mobile m = ns.Mobile;
						Account a = ns.Account as Account;
						int offset = 140 + (i * 20);

						if ( m != null )
						{
							AddLabelCropped(  12, offset, 81, 20, GetHueFor( m ), m.Name );
						}
						AddLabelCropped( 130, offset, 81, 20, 0x47D, ns.ToString() );

						if ( a != null || m != null )
							AddButton( 240, offset - 1, 0xFA5, 0xFA7, 100 + index, GumpButtonType.Reply, 0 );
					}
				}
			}
		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
			Mobile from = sender.Mobile;

			if ( info.ButtonID >= 100 )
			{
				try
				{
					int newIndex = info.ButtonID - 100;

					NetState ns = m_List[newIndex] as NetState;

					if ( ns == null )
						return;

					Mobile m = ns.Mobile as Mobile;
					Account a = ns.Account as Account;
				
					if ( m != null && a != null )
						from.SendGump( new ClientGump( from, m.NetState ) );
				}
				catch( Exception caught )
				{
					from.SendMessage( String.Format( "{0}", caught ) );
				}
			}
			else
			{
				switch( info.ButtonID )
				{
					case 1:
					{
						try
						{
							List<NetState> results = new List<NetState>();

							TextRelay matchEntry = info.GetTextEntry( 0 );
							string match = ( matchEntry == null ? null : matchEntry.Text.Trim().ToLower() );

							if ( match != null && ( match != "" || match.Length > 0 ) )
							{
								List<NetState> instances = NetState.Instances;

								for ( int i = 0; i < instances.Count; ++i )
								{
									NetState ns = (NetState)instances[i];

									bool isMatch;

									isMatch = ( ns.Mobile.Name.ToLower().IndexOf( match ) >= 0 );

									if ( isMatch )
										results.Add( ns );
								}

								//	results.Sort( NetStateComparer.Instance );
							}
					

							if ( results.Count >= 1 )
								from.SendGump( new PlayerSearchGump( from, 0, results ) );
							else
								from.SendGump( new PlayerSearchGump( from, 0, null ) );
						}
						catch( Exception caught )
						{
							from.SendMessage( String.Format( "{0}", caught ) );
						}

							break;
					} 
					case 2:
					{
						List<NetState> instances = NetState.Instances;
						from.SendGump( new PlayerSearchGump( from, 0, instances ) );

						break;
					}
					case 3:
					{
						if ( m_List != null && m_ListPage > 0 )
							from.SendGump( new PlayerSearchGump( from, m_ListPage - 1, m_List ) );

						break;
					}
					case 4:
					{
					
						if ( m_List != null )
							from.SendGump( new PlayerSearchGump( from, m_ListPage + 1, m_List ) );

						break;
					}
				}
			}
		}
	}
}