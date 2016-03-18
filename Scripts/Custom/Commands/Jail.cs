using System; 
using Server; 
using Server.Misc; 
using Server.Targeting;
using Server.Mobiles;

namespace Server.Commands 
{ 
	public class JailCommands 
	{ 
		public static void Initialize() 
		{ 
			CommandSystem.Register( "Jail", AccessLevel.Counselor, new CommandEventHandler( Jail_OnCommand ) ); 
			CommandSystem.Register( "UnJail", AccessLevel.Counselor, new CommandEventHandler( UnJail_OnCommand ) ); 
		} 

		[Usage( "Jail <jailcell>" )] 
		[Description( "Moves a player into jail." )] 
		[Aliases( "Jail" )] 
		public static void Jail_OnCommand( CommandEventArgs e ) 
		{ 
			Mobile from = e.Mobile;
			if ( e.Length == 1 ) 
			{ 
				string jailCell = e.GetString( 0 ).Trim() ; 

				from.SendMessage( String.Format( "Who would you like to send to Jail Cell {0}?", jailCell ));
				from.Target = new JailTarget( jailCell );
				
			} 
			else 
			{ 
				e.Mobile.SendMessage ( 0x25, "Bad Format: Jail <jailcell>" );      
			}
		} 

		[Usage( "UnJail" )] 
		[Description( "Moves a player from jail to his original location." )] 
		[Aliases( "UnJail" )] 
		public static void UnJail_OnCommand( CommandEventArgs e ) 
		{ 
			Mobile from = e.Mobile;

			from.SendMessage( "Who would you like to unjail?" );
			from.Target = new UnJailTarget();
		} 

		private class JailTarget : Target
		{
			private string m_Cell;
			public JailTarget( string cell ) : base( 10, false, TargetFlags.None )
			{
				m_Cell = cell;
			}

			protected override void OnTarget( Mobile from, object targ )
			{
				if ( targ != null && targ is PlayerMobile )
				{
					PlayerMobile m = (PlayerMobile)targ;
                    m.LastLocation = new Point3D(m.Location);
					
					switch( m_Cell )
					{
						case "1":{ m.Location = new Point3D( 5274, 1164, 0 ); } break;
						case "2":{ m.Location = new Point3D( 5284, 1164, 0 ); } break;
						case "3":{ m.Location = new Point3D( 5294, 1164, 0 ); } break;
						case "4":{ m.Location = new Point3D( 5304, 1164, 0 ); } break;
						case "5":{ m.Location = new Point3D( 5274, 1174, 0 ); } break;
						case "6":{ m.Location = new Point3D( 5284, 1174, 0 ); } break;
						case "7":{ m.Location = new Point3D( 5294, 1174, 0 ); } break;
						case "8":{ m.Location = new Point3D( 5304, 1174, 0 ); } break;
						case "9":{ m.Location = new Point3D( 5278, 1184, 0 ); } break;
						case "10":{ m.Location = new Point3D( 5300, 1184, 0 ); } break;
						default:{ m.Location = new Point3D( 5274, 1164, 0 ); } break;
					}
				}
			}
		}

		private class UnJailTarget : Target
		{
			public UnJailTarget() : base( 10, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object targ )
			{
				if ( from is PlayerMobile )
				{
					if ( targ != null && targ is PlayerMobile )
					{
						PlayerMobile m = (PlayerMobile)targ;
						if ( m.LastLocation != new Point3D( 0, 0, 0 ) )
						{
							try
							{
								m.Location = m.LastLocation;
							}
							catch( Exception e )
							{
								Console.WriteLine( e );
							}
						}
						else
						{
							m.Location = new Point3D( 1495, 1629, 10 );
						}
					}
				}
			}
		}
	} 
}