using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Accounting;

namespace Server.Custom.Townsystem
{
	public class VoteGump : Gump
	{
		private PlayerMobile m_From;
		private Election m_Election;
        private static readonly int m_CandidatesPerPage = 10;
        private int m_Page;

        private static bool MeetsCustomReqs(Mobile m)
        {
            return Election.MeetsMinVotingReqs(m) && CitizenshipState.MeetsRequirements(m);
        }

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			if ( info.ButtonID == 0 )
			{
				//m_From.SendGump( new FactionStoneGump( m_From, m_Election.Faction ) );
			}
            else if (info.ButtonID == 98)
            {
                m_From.SendGump(new VoteGump(m_From, m_Election, m_Page - 1));
            }
            else if (info.ButtonID == 99)
            {
                m_From.SendGump(new VoteGump(m_From, m_Election, m_Page + 1));
            }
            else if (info.ButtonID >= 50)
            {
                int index = info.ButtonID - 51;
                index += m_Page * m_CandidatesPerPage;

                if (index >= 0 && index < m_Election.Candidates.Count)
                {
                    Candidate c = m_Election.Candidates[index];

                    if (c == null)
                        return;

                    Mobile m = c.Mobile;

                    if (m == null)
                        return;

                    m_From.SendMessage(String.Format("{0}'s campaign slogan:", m_Election.Candidates[index].Mobile.Name));
                    m_From.SendMessage(m_Election.Candidates[index].Slogan);

                    m_From.SendGump(new VoteGump(m_From, m_Election));
                }
            }
            else
            {
                if (!m_Election.CanVote(m_From))
                    return;

                Account acct = (Account)m_From.Account;

                if (acct == null)
                    return;

                int index = info.ButtonID - 1;
                index += m_Page * m_CandidatesPerPage;

                if (index >= 0 && index < m_Election.Candidates.Count)
                    m_Election.Candidates[index].Voters.Add(new Voter(m_From, m_Election.Candidates[index].Mobile));

                m_From.SendGump(new VoteGump(m_From, m_Election));
            }
		}

		public VoteGump( PlayerMobile from, Election election, int page = 0 ) : base( 50, 50 )
		{
			m_From = from;
			m_Election = election;
            m_Page = page;

			bool canVote = election.CanVote( from );

			AddPage( 0 );

			AddBackground( 0, 0, 420, 350, 5054 );
			AddBackground( 10, 10, 400, 330, 3000 );

            AddHtml(20, 20, 380, 20, String.Format("ELECTION FOR THE KING OF {0}", m_Election.Town.Definition.TownName), false, false);

            if (m_Election.IsCandidate(from))
            {
                canVote = false;
                AddHtml(20, 60, 380, 20, "Candidates are not allowed to vote in their own elections.", false, false);
            }
            else if (canVote)
                AddHtml(20, 60, 380, 20, "VOTE FOR THE KING", false, false); // VOTE FOR LEADERSHIP
            else
                AddHtmlLocalized(20, 60, 380, 20, 1038032, false, false); // You have already voted in this election.


            int startIndex = Math.Max(0, m_Page * m_CandidatesPerPage);
            int count = Math.Max(0, election.Candidates.Count - startIndex);
            int index = 0;

            if (m_Page > 0)
                AddButton(272, 92, 5600, 5604, 98, GumpButtonType.Reply, 0);

            if (count > m_CandidatesPerPage)
            {
                AddButton(272, 271, 5602, 5606, 99, GumpButtonType.Reply, 0);
                count = m_CandidatesPerPage;
            }

            for (int i = 0; i < count; ++i)
			{
                index = startIndex + i;

                if (index >= election.Candidates.Count)
                    break;

				Candidate cd = election.Candidates[index];

                AddButton(20, 90 + (i * 20), 4029, 4031, i + 51, GumpButtonType.Reply, 0);

				if ( canVote )
					AddButton( 55, 90 + (i * 20), 4005, 4007, i + 1, GumpButtonType.Reply, 0 );

				AddLabel( 90, 90 + (i * 20), 0, cd.Mobile.Name );
				//AddLabel( 335, 100 + (i * 20), 0, cd.Votes.ToString() );
			}

			AddButton( 20, 310, 4005, 4007, 0, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 55, 310, 100, 20, 1011012, false, false ); // CANCEL
		}
	}
}