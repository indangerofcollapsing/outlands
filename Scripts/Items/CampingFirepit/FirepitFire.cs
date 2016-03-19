using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Network;
using Server.Mobiles;
using Server.Items;

namespace Server.Custom
{
	public class FirepitFire : Item
	{
		private static readonly Hashtable m_Table = new Hashtable();

		public static FirepitFireEntry GetEntry( Mobile player )
		{
			return (FirepitFireEntry) m_Table[player];
		}

		public static void RemoveEntry( FirepitFireEntry entry )
		{
			m_Table.Remove( entry.Player );
			entry.Fire.m_Entries.Remove( entry );
		}

		private Timer m_Timer;
		private DateTime m_Created;

        private ArrayList m_Entries = new ArrayList();

        public CampingFirepitPlaced m_Firepit;

		public FirepitFire(CampingFirepitPlaced firepit) : base( 0xDE3 )
		{
            m_Firepit = firepit;

			Movable = false;
            Visible = false;

			m_Entries = new ArrayList();

			m_Created = DateTime.UtcNow;

            if (!m_Firepit.InDungeon)
			    m_Timer = Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ), new TimerCallback( OnTick ) );
		}

        public FirepitFire(Serial serial): base(serial)
		{
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime Created
		{
			get{ return m_Created; }
		}
        
		private void OnTick()
		{
			DateTime now = DateTime.UtcNow;
            
			foreach ( FirepitFireEntry entry in new ArrayList( m_Entries ) )
			{
				if ( !entry.Valid || entry.Player.NetState == null )
				{
					RemoveEntry( entry );
				}

				else if ( !entry.Safe && now - entry.Start >= TimeSpan.FromSeconds( 30.0 ) )
				{
					entry.Safe = true;
					entry.Player.SendLocalizedMessage( 500621 ); // The camp is now secure.
				}
			}

			IPooledEnumerable eable = this.GetClientsInRange( m_Firepit.EffectRadius );

			foreach ( NetState state in eable )
			{
				PlayerMobile pm = state.Mobile as PlayerMobile;

				if ( pm != null && GetEntry( pm ) == null )
				{
					FirepitFireEntry entry = new FirepitFireEntry( pm, this );

					m_Table[pm] = entry;
					m_Entries.Add( entry );

					pm.SendLocalizedMessage( 500620 ); // You feel it would take a few moments to secure your camp.
				}
			}

			eable.Free();
		}

		private void ClearEntries()
		{
			if ( m_Entries == null )
				return;

			foreach ( FirepitFireEntry entry in new ArrayList( m_Entries ) )
			{
				RemoveEntry( entry );
			}
		}

		public override void OnAfterDelete()
		{
			if ( m_Timer != null )
				m_Timer.Stop();

			ClearEntries();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); //version

            writer.Write(m_Firepit);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();	
		
            if (version >= 0)
            {
                m_Firepit = (CampingFirepitPlaced)reader.ReadItem();
            }

            //-----

            if (!m_Firepit.InDungeon)
                m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0), new TimerCallback(OnTick));

            m_Entries = new ArrayList();
		}
	}

	public class FirepitFireEntry
	{
		private PlayerMobile m_Player;
		private FirepitFire m_Fire;
		private DateTime m_Start;
		private bool m_Safe;

		public PlayerMobile Player{ get{ return m_Player; } }
		public FirepitFire Fire{ get{ return m_Fire; } }
		public DateTime Start{ get{ return m_Start; } }

		public bool Valid
		{
			get
            {
                int effectRadius = 0;

                if (m_Fire != null)
                {
                    if (m_Fire.m_Firepit != null)
                        effectRadius = m_Fire.m_Firepit.EffectRadius;
                }

                return !Fire.Deleted && Player.Map == Fire.Map && Player.InRange(Fire, effectRadius);
            }
		}

		public bool Safe
		{
			get{ return Valid && m_Safe; }
			set{ m_Safe = value; }
		}

		public FirepitFireEntry( PlayerMobile player, FirepitFire fire )
		{
			m_Player = player;
			m_Fire = fire;
			m_Start = DateTime.UtcNow;
			m_Safe = false;
		}
	}
}