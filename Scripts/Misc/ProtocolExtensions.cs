using System;
using System.Collections.Generic;
using Server;
using Server.Network;
using Server.Items;
using Server.Mobiles;
using Server.Engines.PartySystem;
using Server.Regions;


namespace Server.Misc
{
	public class ProtocolExtensions
	{
		private static PacketHandler[] m_Handlers = new PacketHandler[0x100];

		public static void Initialize()
		{
			PacketHandlers.Register( 0xF0, 0, false, new OnPacketReceive( DecodeBundledPacket ) );

			Register( 0x04, true, new OnPacketReceive( QueryGuardlineData ) );
			Register( 0x05, true, new OnPacketReceive( QueryPartyLocations ) );
			Register( 0x06, true, new OnPacketReceive( QueryPartyLocationsEx ) );
		}

		public static void QueryGuardlineData( NetState state, PacketReader pvSrc ) {
			try {
				Mobile from = state.Mobile;
				Map map = from.Map;

				if ( map != null ) {
					state.Send( GuardlineData.Acquire( map ) );
				}
			} catch ( Exception ex ) {
				Console.WriteLine(ex);
			}
		}

		public static void QueryPartyLocations( NetState state, PacketReader pvSrc )
		{
			Mobile from = state.Mobile;
			Party party = Party.Get( from );

			if ( party != null )
			{
				AckPartyLocations ack = new AckPartyLocations( from, party );

				if ( ack.UnderlyingStream.Length > 8 )
					state.Send( ack );
			}
		}

		public static void QueryPartyLocationsEx( NetState state, PacketReader pvSrc ) {
			try {
			Mobile from = state.Mobile;
			Party party = Party.Get( from );

			if ( party != null ) {
				AckPartyLocationsEx ack = new AckPartyLocationsEx( from, party );

				if ( ack.Count > 0 ) {
					state.Send( ack );
				}
			}
			} catch ( Exception ex ) {
				Console.WriteLine(ex);
			}
		}

		public static void Register( int packetID, bool ingame, OnPacketReceive onReceive )
		{
			m_Handlers[packetID] = new PacketHandler( packetID, 0, ingame, onReceive );
		}

		public static PacketHandler GetHandler( int packetID )
		{
			if ( packetID >= 0 && packetID < m_Handlers.Length )
				return m_Handlers[packetID];

			return null;
		}

		public static void DecodeBundledPacket( NetState state, PacketReader pvSrc )
		{
			int packetID = pvSrc.ReadByte();

			PacketHandler ph = GetHandler( packetID );

			if ( ph != null )
			{
				if ( ph.Ingame && state.Mobile == null )
				{
					Console.WriteLine( "Client: {0}: Sent ingame packet (0xF0x{1:X2}) before having been attached to a mobile", state, packetID );
					state.Dispose();
				}
				else if ( ph.Ingame && state.Mobile.Deleted )
				{
					state.Dispose();
				}
				else
				{
					ph.OnReceive( state, pvSrc );
				}
			}
		}
	}

	public abstract class ProtocolExtension : Packet
	{
		public ProtocolExtension( int packetID, int capacity ) : base( 0xF0 )
		{
			EnsureCapacity( 4 + capacity );

			m_Stream.Write( (byte) packetID );
		}
	}

	public class AckPartyLocations : ProtocolExtension
	{
		public AckPartyLocations( Mobile from, Party party ) : base( 0x01, ((party.Members.Count - 1) * 9) + 4 )
		{
			for ( int i = 0; i < party.Members.Count; ++i )
			{
				PartyMemberInfo pmi = (PartyMemberInfo)party.Members[i];

				if ( pmi == null || pmi.Mobile == from )
					continue;

				Mobile mob = pmi.Mobile;

				if ( Utility.InUpdateRange( from, mob ) && from.CanSee( mob ) )
					continue;

				m_Stream.Write( (int) mob.Serial );
				m_Stream.Write( (short) mob.X );
				m_Stream.Write( (short) mob.Y );
				m_Stream.Write( (byte) (mob.Map == null ? 0 : mob.Map.MapID) );
			}

			m_Stream.Write( (int) 0 );
		}
	}



	public class AckPartyLocationsEx : ProtocolExtension
	{
		private Mobile _from;
		private Party _party;

		private Dictionary<Mobile, bool> _written = new Dictionary<Mobile, bool>();

		public int Count {
			get {
				return _written.Count;
			}
		}

		private bool HasWritten( Mobile mob ) {
			return _written.ContainsKey( mob );
		}

		private void Write( Mobile mob ) {
			m_Stream.Write( (int) mob.Serial );
			m_Stream.Write( (short) mob.X );
			m_Stream.Write( (short) mob.Y );
			m_Stream.Write( (byte) (mob.Map == null ? 0 : mob.Map.MapID) );

			m_Stream.Write( (bool) ( mob.Party == _party ) );

			_written[mob] = true;
		}

		private bool IsOnScreen( Mobile player, Mobile check ) {
			return ( player != null && check != null && Utility.InUpdateRange( player, check ) && player.CanSee( check ) );
		}

		private bool IsOnAnyScreen( Mobile mob ) {
			for ( int i = 0; i < _party.Members.Count; ++i ) {
				PartyMemberInfo pmi = (PartyMemberInfo) _party.Members[i];

				if ( pmi != null ) {
					Mobile friend = pmi.Mobile;

					if ( IsOnScreen( friend, mob ) ) {
						return true;
					}
				}
			}

			return false;
		}
			
		public AckPartyLocationsEx( Mobile from, Party party ) : base( 0x02, ((party.Members.Count - 1) * 9) + 4 )
		{
			_from = from;
			_party = party;

			for ( int i = 0; i < party.Members.Count; ++i )
			{
				PartyMemberInfo pmi = (PartyMemberInfo)party.Members[i];

				if ( pmi == null )
					continue;

				Mobile mob = pmi.Mobile;

				if ( mob != from && mob.Player && !HasWritten( mob ) && !IsOnScreen( from, mob ) ) {
					Write( mob );
				}

				foreach ( AggressorInfo agi in mob.Aggressors ) {
					Mobile c = agi.Attacker;

					if ( c != null && c != from && c.Player && !HasWritten( c ) && IsOnAnyScreen( c ) ) {
						Write( c );
					}
				}

				foreach ( AggressorInfo agi in mob.Aggressed ) {
					Mobile c = agi.Defender;

					if ( c != null && c != from && c.Player && !HasWritten( c ) && IsOnAnyScreen( c ) ) {
						Write( c );
					}
				}
			}

			m_Stream.Write( (int) 0 );
		}
	}

	public sealed class CriticalRegion : Packet {
		public static readonly Packet Enter = Packet.SetStatic( new CriticalRegion( 0xF2 ) );
		public static readonly Packet Leave = Packet.SetStatic( new CriticalRegion( 0xF3 ) );

		private CriticalRegion( byte packetId ) : base( packetId, 1 ) {
		}
	}

	public sealed class GuardlineData : ProtocolExtension {
		private static Dictionary<Map, GuardlineData> _cache;

		public static void Invalidate() {
			if ( _cache != null ) {
				foreach ( GuardlineData gd in _cache.Values ) {
					Packet.Release( gd );
				}

				_cache.Clear();
			}
		}

		public static GuardlineData Acquire( Map map ) {
			if ( map == null ) {
				throw new ArgumentNullException( "map" );
			}

			if ( _cache == null ) {
				_cache = new Dictionary<Map, GuardlineData>();
			}

			GuardlineData gd;

			if ( !_cache.TryGetValue( map, out gd ) ) {
				gd = new GuardlineData( map );
				gd.SetStatic();
				_cache.Add( map, gd );
			}

			return gd;
		}

		public GuardlineData( Map map ) : base( 0x04, 2048 ) {
			m_Stream.Write( (byte) map.MapID );

			foreach ( Region region in Region.Regions ) {
				if ( region.Map == map ) {
					GuardedRegion gr = region as GuardedRegion;

					if ( gr != null && ( !gr.IsDisabled() || gr is Server.Engines.ConPVP.SafeZone ) ) {
						Serialize( gr );
					}
				}
			}
		}

		private void Serialize( GuardedRegion gr ) {
			foreach ( Rectangle3D rect in gr.Area ) {
				m_Stream.Write( (short) rect.Start.X );
				m_Stream.Write( (short) rect.Start.Y );
				m_Stream.Write( (short) rect.Width );
				m_Stream.Write( (short) rect.Height );
				m_Stream.Write( (sbyte) Math.Max( sbyte.MinValue, Math.Min( sbyte.MaxValue, rect.Start.Z ) ) );
				m_Stream.Write( (sbyte) Math.Max( sbyte.MinValue, Math.Min( sbyte.MaxValue, rect.End.Z ) ) );
			}
		}
	}
}