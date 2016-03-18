using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom.Townsystem
{
	public class ElectionGump : Gump
	{
		private PlayerMobile m_From;
		private Election m_Election;

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			switch ( info.ButtonID )
			{
				case 0: // back
				{
					//m_From.SendGump( new FactionStoneGump( m_From, m_Election.Faction ) );
					break;
				}
				case 1: // vote
				{
					if ( m_Election.State == ElectionState.Election )
						m_From.SendGump( new VoteGump( m_From, m_Election ) );

					break;
				}
				case 2: // campaign
				{
					if ( m_Election.CanBeCandidate( m_From ) )
						m_Election.AddCandidate( m_From );

					break;
				}
			}
		}

		public ElectionGump( PlayerMobile from, Election election ) : base( 50, 50 )
		{
			m_From = from;
			m_Election = election;

			AddPage( 0 );

			AddBackground( 0, 0, 420, 180, 5054 );
			AddBackground( 10, 10, 400, 160, 3000 );

			AddHtml( 20, 20, 380, 20, String.Format("ELECTION FOR THE KING OF {0}", m_Election.Town.Definition.TownName), false, false );

			// NOTE: Gump not entirely OSI-accurate, intentionally so

			switch ( election.State )
			{
				case (ElectionState.Starting):
				{
					TimeSpan toGo = ( election.LastStateTime + Election.StartingPeriod ) - DateTime.Now;
					int minutes = (int) (toGo.TotalMinutes);

					AddHtmlLocalized( 20, 40, 380, 20, 1038034, false, false ); // A new election campaign is pending

					if ( minutes > 0 )
					{
						AddHtml( 20, 60, 280, 20, "Minutes until the next election :", false, false ); // Days until next election :
						AddLabel( 300, 60, 0, minutes.ToString() );
					}
					else
					{
						AddHtml( 20, 60, 280, 20, "Election campaigning will begin shortly", false, false ); // Election campaigning begins tonight.
					}

					break;
				}
                case (ElectionState.Announced):
                {
                    TimeSpan toGo = (election.LastStateTime + Election.AnnouncedPeriod) - DateTime.Now;
                    int hours = (int)(toGo.TotalHours+1);

                    AddHtmlLocalized(20, 40, 380, 20, 1038034, false, false); // A new election campaign is pending

                    if (hours > 0)
                    {
                        AddHtml(20, 60, 280, 20, "Hours until the next election :", false, false); // Days until next election :
                        AddLabel(300, 60, 0, hours.ToString());
                    }
                    else
                    {
                        AddHtmlLocalized(20, 60, 280, 20, 1018059, false, false); // Election campaigning begins tonight.
                    }

                    break;
                }
                case (ElectionState.Pending):
				{
					TimeSpan toGo = ( election.LastStateTime + Election.PendingPeriod + Election.AnnouncedPeriod) - DateTime.Now;
					int days = (int) (toGo.TotalDays + 0.5);

					AddHtmlLocalized( 20, 40, 380, 20, 1038034, false, false ); // A new election campaign is pending

					if ( days > 0 )
					{
						AddHtmlLocalized( 20, 60, 280, 20, 1018062, false, false ); // Days until next election :
						AddLabel( 300, 60, 0, days.ToString() );
					}
					else
					{
						AddHtmlLocalized( 20, 60, 280, 20, 1018059, false, false ); // Election campaigning begins tonight.
					}

					break;
				}
				case ElectionState.Campaign:
				{
					TimeSpan toGo = ( election.LastStateTime + Election.CampaignPeriod ) - DateTime.Now;
					int days = (int) (toGo.TotalDays + 0.5);

					AddHtmlLocalized( 20, 40, 380, 20, 1018058, false, false ); // There is an election campaign in progress.

					if ( days > 0 )
					{
						AddHtmlLocalized( 20, 60, 280, 20, 1038033, false, false ); // Days to go:
						AddLabel( 300, 60, 0, days.ToString() );
					}
					else
					{
                        string voting = m_Election.NextStateTime.Hours < 1 ? "less than one hour" : String.Format("less than {0} hours", m_Election.NextStateTime.Hours+1);
                        AddHtml( 20, 60, 380, 20, String.Format("Campaigning in progress. Voting begins in {0}.",voting), false, false ); // Campaign in progress. Voting begins tonight.
					}

                    if (!Election.MeetsMinCandidacyReqs(from))
                        AddHtml(20, 100, 380, 20, "Campaigning requires a rating of 80 in at least one skill.", false, false); // You must have a higher rank to run for office
                    else if (m_Election.Candidates.Count > Election.MaxCandidates)
                    {
                        AddHtml(20, 100, 380, 20, "The maximum number of candidates are currently campaigning.", false, false);
                    }
                    else if (m_Election.IsCandidate( from ))
                    {
                        AddHtml(20, 100, 380, 20, "You are currently campaigning for King.", false, false);
                    }
                    else if ( m_Election.CanBeCandidate( m_From ) )
					{
						AddButton( 20, 110, 4005, 4007, 2, GumpButtonType.Reply, 0 );
						AddHtmlLocalized( 55, 110, 350, 20, 1011427, false, false ); // CAMPAIGN FOR LEADERSHIP
					}
					break;
				}
				case ElectionState.Election:
				{
					TimeSpan toGo = ( election.LastStateTime + Election.VotingPeriod ) - DateTime.Now;
					int days = (int) Math.Ceiling( toGo.TotalDays );

					AddHtmlLocalized( 20, 40, 380, 20, 1018060, false, false ); // There is an election vote in progress.

					AddHtmlLocalized( 20, 60, 280, 20, 1038033, false, false );
					AddLabel( 300, 60, 0, days.ToString() );

                    AddHtmlLocalized(55, 100, 380, 20, 1011428, false, false); // VOTE FOR LEADERSHIP
                    AddButton(20, 100, 4005, 4007, 1, GumpButtonType.Reply, 0);
					break;
				}
			}

			AddButton( 20, 140, 4005, 4007, 0, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 55, 140, 350, 20, 1011012, false, false ); // CANCEL
		}
	}
}