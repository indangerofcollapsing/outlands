using System;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Commands;

namespace Server.Custom.Townsystem
{
	public class FactionState
	{
		private Faction m_Faction;
		private List<PlayerState> m_Members;
		private DateTime m_LastAtrophy;
        private AllianceInfo m_Alliance;

        public DateTime LastAtrophy{ get{ return m_LastAtrophy; } set{ m_LastAtrophy = value; } }
		public bool IsAtrophyReady { get { return DateTime.UtcNow >= (m_LastAtrophy + TimeSpan.FromHours(24)); } }

        public int CheckAtrophy()
        {
            int distrib = 0;
			m_LastAtrophy = DateTime.UtcNow;

            List<PlayerState> members = m_Members.Distinct().ToList();
            for ( int i = 0; i < members.Count; ++i )
            {
                PlayerState ps = members[i];

				// Alliance score
				// This score is much more important than the showscore, so a bit smoother decay.
				if (!ps.IsActive)
				{
					int max_decay = (int)(PlayerState.AllianceScoreDailyMax * 0.6); // 30something points
					int decay_val = Math.Min(max_decay, (int)(ps.AllianceScore * 0.1)); // 10% or 30 points is max decay
					ps.AllianceScore -= decay_val;
				}
				if (ps.AllianceScore < 0)
					ps.AllianceScore = 0;


				// Killpoints / Showscore
                if ( ps.IsActive )
                {
                    ps.IsActive = false;
                    continue;
                }
                else
                {
					if (ps.KillPoints > 0)
					{
						int atrophy = (ps.KillPoints + 9) / 10;
						ps.KillPoints -= atrophy;
						distrib += atrophy;
					}
                }
            }

            return distrib;
        }

		public List<PlayerState> Members
		{
			get{ return m_Members; }
			set{ m_Members = value; }
		}

        public AllianceInfo Alliance
        {
            get { return m_Alliance; }
            set { m_Alliance = value; }
        }

		private DateTime m_NextAllowedAllianceChange;
		public DateTime NextAllowedAllianceChange
		{
			get { return m_NextAllowedAllianceChange; }
			set { m_NextAllowedAllianceChange = value; }
		}


		public FactionState( Faction faction )
		{
			m_Faction = faction;
			m_Members = new List<PlayerState>();

			m_NextAllowedAllianceChange = DateTime.UtcNow;
		}

		public FactionState( GenericReader reader )
		{
			int version = reader.ReadEncodedInt();
            bool toReadAllianceInfo = false;

            switch (version)
            {
				case 2:
					{
						m_NextAllowedAllianceChange = reader.ReadDateTime();
						goto case 1;
					}
                case 1:
                    {
                        
                        goto case 0;
                    }
                case 0:
                    {
                        m_LastAtrophy = reader.ReadDateTime();
                        m_Faction = Faction.ReadReference(reader);

                        int memberCount = reader.ReadEncodedInt();

                        m_Members = new List<PlayerState>();

                        for (int i = 0; i < memberCount; ++i)
                        {
                            PlayerState pl = new PlayerState(reader, m_Faction, m_Members);

                            if (pl.Mobile != null)
                                m_Members.Add(pl);
                        }

                        m_Faction.State = this;

                        m_Faction.ZeroRankOffset = m_Members.Count;
                        m_Members.Sort();

                        for (int i = m_Members.Count - 1; i >= 0; i--)
                        {
                            PlayerState player = m_Members[i];

                            if (player.KillPoints <= 0)
                                m_Faction.ZeroRankOffset = i;
                            else
                                player.RankIndex = i;
                        }

                        if (version > 0)
                        {
                            toReadAllianceInfo = reader.ReadBool();
                            if (toReadAllianceInfo)
                            {
                                m_Alliance = AllianceInfo.Deserialize(reader, m_Faction);
                                Timer.DelayCall(TimeSpan.FromTicks(1), SetupAlliance);
                            }
                        }
                        else
                        {
                            Timer.DelayCall(TimeSpan.FromTicks(2), delegate { AllianceInfo.ResetAlliances(); } );
                            Timer.DelayCall(TimeSpan.FromTicks(2), delegate { Keywords.RemoveFactionTitles(); });
                        }

                    }break;
            }

		}

        public void SetupAlliance()
        {
            if (m_Alliance == null)
                return;

            foreach (Faction f in m_Alliance.Members)
                f.Alliance = m_Alliance;

			Server.Custom.Townsystem.OCBRankingSystem.CalculateLeaderboards();
        }

		public void Serialize( GenericWriter writer )
		{
			writer.WriteEncodedInt( (int) 2 ); // version

			// 2
			writer.Write(m_NextAllowedAllianceChange);

			// 1

			// 0
			writer.Write( m_LastAtrophy );
			Faction.WriteReference( writer, m_Faction );

			writer.WriteEncodedInt( (int) m_Members.Count );

			for ( int i = 0; i < m_Members.Count; ++i )
			{
				PlayerState pl = (PlayerState) m_Members[i];
				pl.Serialize( writer );
			}

            bool toWrite = m_Alliance != null && m_Alliance.Leader == m_Faction;
            writer.Write(toWrite);
            if (toWrite)
                m_Alliance.Serialize(writer);

		}

		public static void Initialize()
        {
            Server.Commands.CommandSystem.Register("OCBDecayNow", AccessLevel.GameMaster, new CommandEventHandler(OCBScoring_DecayNow));
        }

        public static void OCBScoring_DecayNow(CommandEventArgs e)
        {
			foreach (Faction fs in Faction.Factions)
			{
				fs.State.CheckAtrophy();
			}
        }
	}
}