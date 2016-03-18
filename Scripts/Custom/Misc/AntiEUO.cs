using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;
using Server.Accounting;

namespace Server.Network
{
	public class AntiEUO
	{
		public static void Initialize()
		{
			PacketHandlers.Register( 108, 19, true, new OnPacketReceive( AntiEUOTargeting ) );
		}

		private static void Catch( Mobile m )
		{
			Account a = (Account)m.Account;
			string tag = a.GetTag( "EasyUO Targeted" );

			if ( tag == null )
				a.AddTag( "EasyUO Targeted", "True" );
						
			foreach ( NetState ns in NetState.Instances )
			{
				if ( ns.Mobile is PlayerMobile )
				{
					PlayerMobile from = (PlayerMobile)ns.Mobile;
	
					if ( from.AccessLevel >= AccessLevel.Counselor )
						from.SendMessage( String.Format( "EasyUO Targeting Detected: [Serial: {0} Name: {1}]", m.Serial, m.Name ) );
				}
			}
		}

		public static void AntiEUOTargeting( NetState state, PacketReader pvSrc )
		{
			int type = pvSrc.ReadByte(); // type
			int cid = pvSrc.ReadInt32(); // cursor id
			int cur = pvSrc.ReadByte(); // cursor
			Serial s = (Serial)pvSrc.ReadInt32(); // serial
			int x = pvSrc.ReadInt16(); // x
			int y = pvSrc.ReadInt16(); // y
			int z = pvSrc.ReadInt16(); // z
			int model = pvSrc.ReadInt16(); // model number
			
			Mobile m = state.Mobile;
			Target t = m.Target;
			
			if ( t == null )
				return;

			if ( x == -1 && y == -1 && !s.IsValid )
			{
				t.Cancel( m, TargetCancelType.Canceled );
				return; 
			}

			object o;

			if ( type == 1 )
			{
				if ( model == 0 )
				{
					o = new LandTarget( new Point3D( x, y, z ), m.Map);
				}
				else
				{
					if ( m.Map == null || m.Map == Map.Internal )
					{
						t.Cancel( m, TargetCancelType.Canceled );
						return; 
					}

					StaticTile[] TileArray = m.Map.Tiles.GetStaticTiles( x, y, !t.DisallowMultis );
					bool flag = false;

					for ( int i = 0; ( !flag && ( i < TileArray.Length ) ); i++ )
						if ( ( TileArray[i].Z == z ) && ( ( TileArray[i].ID & 16383 ) == ( model & 16383) ) )
							flag = true;
			 
					if ( !flag )
					{
						t.Cancel( m, TargetCancelType.Canceled );
						return; 
					}

					o = new StaticTarget( new Point3D( x, y, z ), model );
				}
			}
			else
			{
				PlayerMobile pm = m as PlayerMobile;
				if ( pm == null ) // should never happen
				{
					t.Cancel( m, TargetCancelType.Canceled );
					return;
				}

				if ( pm.LastTarget == Serial.MinusOne ) // last target not set yet
				{
					if ( s == Serial.Zero ) // client is a fucking retard
					{
						t.Cancel( m, TargetCancelType.Canceled );
						return;
					}

					if ( s != m.Serial) 
						pm.LastTarget = s;
				}
				else if ( x == 0 && y == 0 && z == 0 ) // is last target or euo
					if ( s != pm.LastTarget ) // they are sending last target while server monitored last target is different				{
					{
						Catch( pm );
					/*	m.SendMessage( "BAD LAST TARGET!!! CURSOR ID:{0} CURSOR:{1} X:{2} Y:{3} Z:{4} MODEL:{5} TYPE:{6} LAST:{7} NEW:{8}", cid, cur, x, y, z, model, type, ((PlayerMobile)m).LastTarget, s );
						m.SendMessage( "CURSOR ID:{0} CURSOR:{1} X:{2} Y:{3} Z:{4} MODEL:{5} TYPE:{6} LAST:{7} NEW:{8}", cid, cur, x, y, z, model, type, ((PlayerMobile)m).LastTarget, s );	
					*/			
					//	t.Cancel( m, TargetCancelType.Canceled );
					//	return;
					}

				if ( s != m.Serial )
					pm.LastTarget = s;

				if ( s.IsMobile )
					o = World.FindMobile( s ); 
				else if ( s.IsItem )
					o = World.FindItem( s );
				else
				{
					t.Cancel( m, TargetCancelType.Canceled );
					return;
				}
			}

			t.Invoke( m, o );
		}
	}
}