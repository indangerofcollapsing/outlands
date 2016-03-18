using System;
using Server;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Custom.Townsystem
{
	public class PlayerState : IComparable
	{
		private Mobile m_Mobile;
		private Faction m_Faction;
		private List<PlayerState> m_Owner;
		private int m_KillPoints;
		private DateTime m_Leaving;
		private RankDefinition m_Rank;
        private RankDefinition m_CurrentRank;
		private List<KillPointsGivenEntry> m_PointsGiven;
		private bool m_IsActive;
        private bool m_HideRank;
        private int m_VendorCredit;
		private OCBRankingSystem.OCBRank m_AllianceRank; // rank within alliance, based on killpoints

		private Town m_King;

		private DateTime m_LastHonorTime;

		// NOT SERIALIZED! 
		// This is created and set when the player joins a faction. 
		// Only used to prevent join immediately followed by a treasury steal.
		private DateTime m_Joined;


		public Mobile Mobile{ get{ return m_Mobile; } }
        public Faction Faction { get { return m_Faction; } set { m_Faction = value; } }
        public List<PlayerState> Owner { get { return m_Owner; } set { m_Owner = value; } }
        public Town King{ get{ return m_King; } set{ m_King = value; Invalidate(); } }

        public Town Town { get { return ((PlayerMobile)Mobile).Citizenship; } }
		public List<KillPointsGivenEntry> PointsGiven { get { return m_PointsGiven; } }
		public int VendorCredit { get { return m_VendorCredit; } set { m_VendorCredit = value; } }
		public OCBRankingSystem.OCBRank AllianceRank { get { return m_AllianceRank; } set { m_AllianceRank = value; } }
		
		// OCB Alliance score (base for alliance ranks)
		private int m_AllianceScore;
		public static int AllianceScoreDailyMax = 55;
		public int AllianceScore { get { return m_AllianceScore; } set { m_AllianceScore = value; } }

		public int KillPoints { 
			get { return m_KillPoints; } 
			set { 
				if ( m_KillPoints != value ) {
					if ( value > m_KillPoints ) {
						if ( m_KillPoints <= 0 ) {
							if ( value <= 0 ) {
								m_KillPoints = value;
								Invalidate();
								return;
							}
							
							m_Owner.Remove( this );
							m_Owner.Insert( m_Faction.ZeroRankOffset, this );

							m_RankIndex = m_Faction.ZeroRankOffset;
							m_Faction.ZeroRankOffset++;
						}
						while ( ( m_RankIndex - 1 ) >= 0 ) {
							PlayerState p = m_Owner[m_RankIndex-1] as PlayerState;
							if ( value > p.KillPoints ) {
								m_Owner[m_RankIndex] = p;
								m_Owner[m_RankIndex-1] = this;
								RankIndex--;
								p.RankIndex++;
							}
							else
								break;
						}
					}
					else {
						if ( value <= 0 ) {
							if ( m_KillPoints <= 0 ) {
								m_KillPoints = value;
								Invalidate();
								return;
							}

							while ( ( m_RankIndex + 1 ) < m_Faction.ZeroRankOffset ) {
								PlayerState p = m_Owner[m_RankIndex+1] as PlayerState;
								m_Owner[m_RankIndex+1] = this;
								m_Owner[m_RankIndex] = p;
								RankIndex++;
								p.RankIndex--;
							}

							m_RankIndex = -1;
							m_Faction.ZeroRankOffset--;
						}
						else {
							while ( ( m_RankIndex + 1 ) < m_Faction.ZeroRankOffset ) {
								PlayerState p = m_Owner[m_RankIndex+1] as PlayerState;
								if ( value < p.KillPoints ) {
									m_Owner[m_RankIndex+1] = this;
									m_Owner[m_RankIndex] = p;
									RankIndex++;
									p.RankIndex--;
								}
								else
									break;
							}
						}
					}

					m_KillPoints = value;
                    //UpdateRank();
					Invalidate();
				}
			}
		}

		private bool m_InvalidateRank = true;
		private int  m_RankIndex = -1;

		public int RankIndex { get { return m_RankIndex; } set { if ( m_RankIndex != value ) { m_RankIndex = value; m_InvalidateRank = true; } } }
		
		public RankDefinition Rank { 
			get { 
				if ( m_InvalidateRank ) {
					RankDefinition[] ranks = m_Faction.Definition.Ranks;
					int percent;

					if ( m_Owner.Count == 1 )
						percent = 1000;
					else if ( m_RankIndex == -1 )
						percent = 0;
					else
						percent = ( ( m_Faction.ZeroRankOffset - m_RankIndex ) * 1000 ) / m_Faction.ZeroRankOffset;

					for ( int i = 0; i < ranks.Length; i++ ) {
						RankDefinition check = ranks[i];

						if ( percent >= check.Required ) {
							m_Rank = check;
							m_InvalidateRank = false;
							break;
						}
					}

					Invalidate();
				}

				return m_Rank;
			}
		}

		public DateTime LastHonorTime{ get{ return m_LastHonorTime; } set{ m_LastHonorTime = value; } }
		public DateTime Leaving{ get{ return m_Leaving; } set{ m_Leaving = value; } }
		public bool IsLeaving{ get{ return ( m_Leaving > DateTime.MinValue ); } }
        public bool HideRank { get { return m_HideRank; } set { m_HideRank = value; } }

		public bool IsActive{ get{ return m_IsActive; } set{ m_IsActive = value; } }

        public void UpdateRank()
        {
            PlayerMobile pm = m_Mobile as PlayerMobile;
            RankDefinition newRank = Rank;

            if (m_CurrentRank == newRank)
                return;
            else
            {
                if (m_CurrentRank != null)
                    pm.RemovePrefixTitle = m_CurrentRank.Title;

                m_CurrentRank = newRank;

                if (newRank != null)
                    pm.AddPrefixTitle = newRank.Title;

                pm.SendMessage("Your factions rank has changed and you have been awarded a new factions title.");
            }
        }

        public bool CanGiveKillPointsTo(Mobile mob)
		{
			if ( m_PointsGiven == null )
				return true;

			for ( int i = 0; i < m_PointsGiven.Count; ++i )
			{
				KillPointsGivenEntry sge = m_PointsGiven[i];

				if ( sge.IsExpired )
					m_PointsGiven.RemoveAt( i-- );
				else if ( sge.GivenTo == mob )
					return false;
			}

			return true;
		}

        public string GetTitle()
        {
            if (m_HideRank)
                return m_Faction.Definition.FriendlyName;
            else
                return Rank.Title;
        }

        public string GetAllianceTitle()
        {
            if (m_Faction != null && m_Faction.Alliance != null)
            {
                return m_Faction.Alliance.Name;
            }

            return "";
        }

        public void RemoveTitle()
        {
            PlayerMobile pm = m_Mobile as PlayerMobile;

            if (m_CurrentRank != null)
                pm.RemovePrefixTitle = m_CurrentRank.Title;
        }

		public void OnGivenKillPointsTo( Mobile mob )
		{
			if ( m_PointsGiven == null )
				m_PointsGiven = new List<KillPointsGivenEntry>();

			m_PointsGiven.Add( new KillPointsGivenEntry( mob ) );
		}

		public void Invalidate()
		{
			if ( m_Mobile is PlayerMobile )
			{
				PlayerMobile pm = (PlayerMobile)m_Mobile;
				pm.InvalidateProperties();
				pm.InvalidateMyRunUO();
			}
		}

		public void Attach()
		{
			if ( m_Mobile is PlayerMobile )
				((PlayerMobile)m_Mobile).TownsystemPlayerState = this;
		}

		public PlayerState( Mobile mob, Faction faction, List<PlayerState> owner )
		{
			m_Mobile = mob;
			m_Faction = faction;
			m_Owner = owner;
			m_Joined = new DateTime();
			m_Joined = DateTime.Now;

			Attach();
			Invalidate();
		}

		public PlayerState( GenericReader reader, Faction faction, List<PlayerState> owner )
		{
			m_Faction = faction;
			m_Owner = owner;

			int version = reader.ReadEncodedInt();

			switch ( version )
			{
				case 4:
					{
						m_AllianceScore = reader.ReadInt();
						goto case 3;
					}
                case 3:
                    {
                        m_VendorCredit = reader.ReadInt();
                        goto case 2;
                    }
                case 2:
                    {
                        m_HideRank = reader.ReadBool();
                        goto case 1;
                    }
                case 1:
                    {
                        int c = reader.ReadInt();

                        if (c != -1)
                            m_CurrentRank = m_Faction.Definition.Ranks[c];

                        goto case 0;
                    }
				case 0:
					m_IsActive = reader.ReadBool();
					m_LastHonorTime = reader.ReadDateTime();

					m_Mobile = reader.ReadMobile();
                    
					m_KillPoints = reader.ReadEncodedInt();

					m_Leaving = reader.ReadDateTime();
					break;
			}

			Attach();
		}

		public void Serialize( GenericWriter writer )
		{
			writer.WriteEncodedInt( (int) 4 ); // version

			// version 4
			writer.Write(m_AllianceScore);

            //version 3
            writer.Write(m_VendorCredit);

            //version 2
            writer.Write(m_HideRank);

            //version 1

            if (m_CurrentRank == null)
                writer.Write((int)-1);
            else
            {
                bool found = false;
                for (int i = 0; i < m_Faction.Definition.Ranks.Length; i++)
                {
                    if (m_CurrentRank == m_Faction.Definition.Ranks[i])
                    {
                        writer.Write((int)i);
                        found = true;
                        break;
                    }
                }

                if (!found)
                    writer.Write((int)-1);
            }

			writer.Write( m_IsActive );
			writer.Write( m_LastHonorTime );

			writer.Write( (Mobile) m_Mobile );

			writer.WriteEncodedInt( (int) m_KillPoints );

			writer.Write( (DateTime) m_Leaving );
		}

		public static PlayerState Find( Mobile mob )
		{
			if ( mob is PlayerMobile )
				return ((PlayerMobile)mob).TownsystemPlayerState;

			return null;
		}

		public int CompareTo( object obj )
		{
			return ((PlayerState)obj).m_KillPoints - m_KillPoints;
		}

		public bool JoinedWithinMinutes(int minutes)
		{
			return m_Joined != null && minutes > (DateTime.Now - m_Joined).TotalMinutes; 
		}
	}
}