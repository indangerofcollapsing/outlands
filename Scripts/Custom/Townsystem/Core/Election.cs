using System;
using System.Net;
using System.Collections;
using Server;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Prompts;
using System.IO;
using System.Text;
using Server.Commands;

namespace Server.Custom.Townsystem
{
    public class Election
    {
        public static readonly TimeSpan AnnouncedPeriod = TimeSpan.FromHours(23); //Occurs at noon on the last day of the month
        public static readonly TimeSpan StartingPeriod = TimeSpan.FromHours(1); //One hour notification at noon of the 1st.
        public static readonly TimeSpan CampaignPeriod = TimeSpan.FromDays(1); //Campaigning starts at 1:00pm on the first.
        public static readonly TimeSpan VotingPeriod = TimeSpan.FromDays(1); //Voting ends 1:00pm on the 5th.
        public static readonly TimeSpan PendingPeriod = TimeSpan.FromDays(30); //not currently used. Recalculates based on the first of the month.

        public static TimeSpan NextKing { get { return TimeSpan.FromDays(28); } }

        public const int MaxCandidates = 100;
        //public const double MinCombatSkill = 59.9;
		public const double MinSkillForCandidacy = 79.9;

        private Town m_Town;
        private List<Candidate> m_Candidates;
        private ElectionStone m_ElectionStone;

        private ElectionState m_State;
        private DateTime m_LastStateTime;

        public Town Town { get { return m_Town; } }
        public ElectionStone ElectionStone { get { return m_ElectionStone; } set { m_ElectionStone = value; } }

        public List<Candidate> Candidates { get { return m_Candidates; } }

		public ElectionState State { get { return m_State; } set { m_State = value; m_LastStateTime = DateTime.Now; } }
        public DateTime LastStateTime { get { return m_LastStateTime; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public ElectionState CurrentState { get { return m_State; } }

        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Administrator)]
        public TimeSpan NextElection
        {
            get
            {
                TimeSpan period;

                switch (m_State)
                {
                    default:
                    case ElectionState.Pending: period = PendingPeriod + AnnouncedPeriod + StartingPeriod + CampaignPeriod + VotingPeriod; break;
                    case ElectionState.Announced: period = AnnouncedPeriod + StartingPeriod + CampaignPeriod + VotingPeriod; break;
                    case ElectionState.Starting: period = StartingPeriod + CampaignPeriod + VotingPeriod; break;
                    case ElectionState.Campaign: period = CampaignPeriod + VotingPeriod; break;
                    case ElectionState.Election: period = VotingPeriod; break;
                }

				TimeSpan until = (m_LastStateTime + period) - DateTime.Now;

                if (until < TimeSpan.Zero)
                    until = TimeSpan.Zero;

                return until;
            }
        }

        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Administrator)]
        public TimeSpan NextStateTime
        {
            get
            {
                TimeSpan period;

                switch (m_State)
                {
                    default:
                    case ElectionState.Pending: period = PendingPeriod; break;
                    case ElectionState.Announced: period = AnnouncedPeriod; break;
                    case ElectionState.Starting: period = StartingPeriod; break;
                    case ElectionState.Election: period = VotingPeriod; break;
                    case ElectionState.Campaign: period = CampaignPeriod; break;
                }

				TimeSpan until = (m_LastStateTime + period) - DateTime.Now;

                if (until < TimeSpan.Zero)
                    until = TimeSpan.Zero;

                return until;
            }
            set
            {
                TimeSpan period;

                switch (m_State)
                {
                    default:
                    case ElectionState.Pending: period = PendingPeriod; break;
                    case ElectionState.Election: period = VotingPeriod; break;
                    case ElectionState.Campaign: period = CampaignPeriod; break;
                }

				m_LastStateTime = DateTime.Now - period + value;
            }
        }

        private Timer m_Timer;

        public void StartTimer()
        {
            m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(1.0), TimeSpan.FromMinutes(1.0), new TimerCallback(Slice));
        }

        public Election(Town town)
        {
            m_Town = town;
            m_Candidates = new List<Candidate>();

            StartTimer();
        }

        public void BeginElection()
        {
            if (CurrentState == ElectionState.Pending)
                State = ElectionState.Announced;
        }

        public Election(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            switch (version)
            {
                case 0:
                    {
                        m_Town = Town.ReadReference(reader);

                        m_LastStateTime = reader.ReadDateTime();
                        m_State = (ElectionState)reader.ReadEncodedInt();

                        m_Candidates = new List<Candidate>();

                        int count = reader.ReadEncodedInt();

                        for (int i = 0; i < count; ++i)
                        {
                            Candidate cd = new Candidate(reader);

                            if (cd.Mobile != null)
                                m_Candidates.Add(cd);
                        }

                        m_ElectionStone = (ElectionStone)reader.ReadItem();

                        break;
                    }
            }

            StartTimer();
        }

        public void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)0); // version

            Town.WriteReference(writer, m_Town);

            writer.Write((DateTime)m_LastStateTime);
            writer.WriteEncodedInt((int)m_State);

            writer.WriteEncodedInt(m_Candidates.Count);

            for (int i = 0; i < m_Candidates.Count; ++i)
                m_Candidates[i].Serialize(writer);

            writer.WriteItem((ElectionStone)m_ElectionStone);
        }

        public void AddCandidate(Mobile mob)
        {
            if (IsCandidate(mob))
                return;

            mob.SendMessage("Enter a slogan for your campaign.");
            mob.Prompt = new SloganPrompt(this);
        }

        public void SloganResponse(Mobile mob, string slogan)
        {
            if (!CanBeCandidate(mob))
                return;

            m_Candidates.Add(new Candidate(mob, slogan));
            mob.SendMessage("You are now running for King.");
        }

        public void RemoveVoter(Mobile mob)
        {
            if (m_State == ElectionState.Election)
            {
                for (int i = 0; i < m_Candidates.Count; ++i)
                {
                    List<Voter> voters = m_Candidates[i].Voters;

                    for (int j = 0; j < voters.Count; ++j)
                    {
                        Voter voter = voters[j];

                        if (voter.From == mob)
                            voters.RemoveAt(j--);
                    }
                }
            }
        }

        public void RemoveCandidate(Mobile mob)
        {
            Candidate cd = FindCandidate(mob);

            if (cd == null)
                return;

            m_Candidates.Remove(cd);
            mob.SendMessage("There are no longer any valid candidates in the election.");

            if (m_State == ElectionState.Election)
            {
                if (m_Candidates.Count == 1)
                {
                    m_Town.Broadcast(0, "There are no longer any valid candidates in the election.");
                    Candidate winner = m_Candidates[0];

                    Mobile winMob = winner.Mobile;

                    if (winMob == null || Town.CheckCitizenship(winMob) != m_Town || winMob == m_Town.King)
                    {
                        //m_Town.Broadcast(0, "Town leadership has not changed.");
                        m_Town.AddTownCrierEntry(new string[] { String.Format("Town leadership remains under the control of {0}!", winMob.RawName) }, TimeSpan.FromMinutes(30));
                    }
                    else
                    {
                        m_Town.AddTownCrierEntry(new string[] { String.Format("Town leadership has changed!", winMob.RawName), String.Format("All hail new King {0}", winMob.RawName) }, TimeSpan.FromMinutes(30));
                        //m_Town.Broadcast(0, "The town has a new King!"); // The faction has a new commander.
                        m_Town.King = winMob;
                        m_Town.TotalVoted = winner.Votes;
                    }

                    m_Candidates.Clear();
                    State = ElectionState.Pending;
                }
                else if (m_Candidates.Count == 0) // well, I guess this'll never happen
                {
                    m_Town.Broadcast(0, "There are no longer any valid candiates in the election."); // There are no longer any valid candidates in the Faction Commander election.

                    m_Candidates.Clear();
                    State = ElectionState.Pending;
                }
            }
        }

        public bool IsCandidate(Mobile mob)
        {
            return (FindCandidate(mob) != null);
        }

        public bool CanVote(Mobile mob)
        {
            return (m_State == ElectionState.Election && !HasVoted(mob));
        }

        public bool HasVoted(Mobile mob)
        {
            return (FindVoter(mob) != null);
        }

        public Candidate FindCandidate(Mobile mob)
        {
            for (int i = 0; i < m_Candidates.Count; ++i)
            {
                if (m_Candidates[i].Mobile == mob)
                    return m_Candidates[i];
            }

            return null;
        }

        public Candidate FindVoter(Mobile mob)
        {
            for (int i = 0; i < m_Candidates.Count; ++i)
            {
                List<Voter> voters = m_Candidates[i].Voters;
                for (int j = 0; j < voters.Count; ++j)
                {
                    Voter voter = voters[j];
                    if (voter.Account == mob.Account)
                        return m_Candidates[i];
                }
            }

            return null;
        }

        public static bool MeetsMinVotingReqs(Mobile from)
        {
			return true;
        }

        public static bool MeetsMinCandidacyReqs(Mobile from)
        {
            return (from.Skills.Highest.Base > MinSkillForCandidacy);
        }

        public bool CanBeCandidate(Mobile mob)
        {
            if (IsCandidate(mob))
                return false;

            if (m_Candidates.Count >= MaxCandidates)
                return false;

            if (m_State != ElectionState.Campaign)
                return false; // sanity..

            return (Town.CheckCitizenship(mob) == m_Town && MeetsMinCandidacyReqs(mob));
        }

        public void Slice()
        {
            if (m_Town.Election != this)
            {
                if (m_Timer != null)
                    m_Timer.Stop();

                m_Timer = null;

                return;
            }

            switch (m_State)
            {

                case ElectionState.Pending:
                    {
						if ((m_LastStateTime + PendingPeriod) > DateTime.Now)
                            break;
                        m_Town.AddTownCrierEntry(new string[] { "Potential Kings and Queens may begin campaigning within 24 hours!" }, TimeSpan.FromHours(4));
                        //m_Town.Broadcast(0, "Campaigning for King will begin in one day.");
                        State = ElectionState.Announced;
                        break;
                    }
                case ElectionState.Announced:
                    {
						if ((m_LastStateTime + AnnouncedPeriod) > DateTime.Now)
                            break;
                        m_Town.AddTownCrierEntry(new string[] { "Prospective Kings and Queens may begin campaigning within the hour!" }, TimeSpan.FromHours(1));
                        //m_Town.Broadcast(0, "Campaigning for King will begin in one hour.");
                        State = ElectionState.Starting;
                        break;
                    }
                case ElectionState.Starting:
                    {
						if ((m_LastStateTime + StartingPeriod) > DateTime.Now)
                            break;

                        m_Town.AddTownCrierEntry(new string[] { "Campaigning has begun!" }, TimeSpan.FromHours(4));
                        //m_Town.Broadcast(0, "Campaigning for King has begun.");

                        m_Candidates.Clear();
                        State = ElectionState.Campaign;

                        break;
                    }
                case ElectionState.Campaign:
                    {
						if ((m_LastStateTime + CampaignPeriod) > DateTime.Now)
                            break;

                        if (m_Candidates.Count == 0)
                        {
                            m_Town.Broadcast("Nobody ran for King");
                        }
                        else if (m_Candidates.Count == 1)
                        {
                            m_Town.Broadcast("Only one member ran for King");

                            Candidate winner = m_Candidates[0];

                            Mobile mob = winner.Mobile;

                            if (mob == null || Town.CheckCitizenship(mob) != m_Town || mob == m_Town.King)
                            {
                                m_Town.AddTownCrierEntry(new string[] { String.Format("Town leadership remains under the control of {0}!", mob.RawName) }, TimeSpan.FromMinutes(30));
                                m_Town.AllowNewCitizenshipBuffs = true;
                            }
                            else
                            {
                                m_Town.AddTownCrierEntry(new string[] { String.Format("Town leadership has changed!", mob.RawName), String.Format("All hail new King {0}", mob.RawName) }, TimeSpan.FromHours(3));
                                m_Town.King = mob;
                                m_Town.TotalVoted = m_Candidates[0].Votes;
                            }

                            m_Candidates.Clear();
                        }
                        else
                        {
                            m_Town.AddTownCrierEntry(new string[] { "Voting for King and Queen is underway!", "Cast your vote at the Election Stone!" }, VotingPeriod);
                        }
                        State = ElectionState.Election;
                        break;
                    }
                case ElectionState.Election:
                    {
						if ((m_LastStateTime + VotingPeriod) > DateTime.Now)
                            break;

                        m_Town.Broadcast(0, "The results are in!");

                        ElectionWriter.InitializeString(this);
                        ElectionWriter.Append("<Election>");

                        Candidate winner = null;

                        for (int i = 0; i < m_Candidates.Count; ++i)
                        {
                            Candidate cd = m_Candidates[i];

                            if (cd.Mobile == null || Town.CheckCitizenship(cd.Mobile) != m_Town)
                                continue;

                            ElectionWriter.Append("<Candidate>");
                            ElectionWriter.Append("<CandidateName>" + cd.Mobile.RawName + "</CandidateName>");

                            cd.CleanMuleVotes();

                            ElectionWriter.Append("</Candidate>");

                            if (winner == null || cd.Votes > winner.Votes)
                                winner = cd;
                        }

                        ElectionWriter.Append("</Election>");
                        ElectionWriter.Write(m_Town.Definition.FriendlyName + "_votes.xml");

                        if (winner == null)
                        {
                            m_Town.AddTownCrierEntry(new string[] { "Town leadership has not changed!" }, TimeSpan.FromMinutes(30));
                            m_Town.AllowNewCitizenshipBuffs = true;
                        }
                        else if (winner.Mobile == m_Town.King)
                        {
                            m_Town.AddTownCrierEntry(new string[] { String.Format("Town leadership remains under the control of {0}!", winner.Mobile.RawName) }, TimeSpan.FromHours(3));
                            m_Town.AllowNewCitizenshipBuffs = true;
                            m_Town.TotalVoted = winner.Votes;
                        }
                        else
                        {
                            m_Town.AddTownCrierEntry(new string[] { String.Format("Town leadership has changed!", winner.Mobile.RawName), String.Format("All hail new King {0}", winner.Mobile.RawName) }, TimeSpan.FromHours(3));
                            m_Town.King = winner.Mobile;
                            m_Town.TotalVoted = winner.Votes;
                        }

                        m_Candidates.Clear();
                        State = ElectionState.Pending;
                        NextStateTime = NextKing;

                        break;
                    }
            }
        }
    }    

    public class Voter
    {
        private Mobile m_From;
        private Accounting.Account m_Account;
        private Mobile m_Candidate;

        private IPAddress m_Address;
        private DateTime m_Time;

        public Mobile From
        {
            get { return m_From; }
        }

        public Accounting.Account Account
        {
            get { return m_Account; }
        }

        public Mobile Candidate
        {
            get { return m_Candidate; }
        }

        public IPAddress Address
        {
            get { return m_Address; }
        }

        public DateTime Time
        {
            get { return m_Time; }
        }

        public object[] AcquireFields()
        {
            TimeSpan gameTime = TimeSpan.Zero;

            if (m_From is PlayerMobile)
                gameTime = ((PlayerMobile)m_From).GameTime;

            int kp = 0;

            PlayerState pl = PlayerState.Find(m_From);

            if (pl != null)
                kp = pl.KillPoints;

            int sk = m_From.Skills.Total;

            int factorSkills = 50 + ((sk * 100) / 10000);
            int factorKillPts = 100 + (kp * 2);
            int factorGameTime = 50 + (int)((gameTime.Ticks * 100) / TimeSpan.TicksPerDay);

            int totalFactor = (factorSkills * factorKillPts * Math.Max(factorGameTime, 100)) / 10000;

            if (totalFactor > 100)
                totalFactor = 100;
            else if (totalFactor < 0)
                totalFactor = 0;

            return new object[] { m_From, m_Address, m_Time, totalFactor };
        }

        public Voter(Mobile from, Mobile candidate)
        {
            m_From = from;
            m_Candidate = candidate;
            m_Account = from.Account as Accounting.Account;

            if (m_From.NetState != null)
                m_Address = m_From.NetState.Address;
            else
                m_Address = IPAddress.None;

			m_Time = DateTime.Now;
        }

        public Voter(GenericReader reader, Mobile candidate)
        {
            m_Candidate = candidate;

            int version = reader.ReadEncodedInt();

            switch (version)
            {
                case 0:
                    {
                        m_From = reader.ReadMobile();
                        m_Address = Utility.Intern(reader.ReadIPAddress());
                        m_Time = reader.ReadDateTime();

                        if (m_From != null)
                            Timer.DelayCall(TimeSpan.FromTicks(1), delegate { m_Account = m_From.Account as Accounting.Account; });

                        break;
                    }
            }
        }

        public void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)0);

            writer.Write((Mobile)m_From);
            writer.Write((IPAddress)m_Address);
            writer.Write((DateTime)m_Time);
        }
    }

    public static class ElectionWriter
    {
        private static StreamWriter m_Writer;
        private static StringBuilder m_Local;

        public static void InitializeString(Election e)
        {
            Clear();
            int stringLength = 21;

            foreach (Candidate candidate in e.Candidates)
                stringLength += 70 + 162 * candidate.Voters.Count;

            m_Local = new StringBuilder(stringLength);
        }

        public static void Clear()
        {
            if (m_Writer != null)
            {
                m_Writer.Close();
                m_Writer.Dispose();
                m_Writer = null;
            }

            m_Local = null;
        }

        public static void Append(string s)
        {
            if (m_Local == null)
                return;

            m_Local.Append(s);
        }

        public static void Write(string file)
        {
            m_Writer = new StreamWriter(file);
            m_Writer.Write(m_Local);
            Clear();
        }
    }

    public class Candidate
    {
        private Mobile m_Mobile;
        private string m_Slogan;
        private List<Voter> m_Voters;

        public Mobile Mobile { get { return m_Mobile; } }
        public string Slogan { get { return m_Slogan; } }
        public List<Voter> Voters { get { return m_Voters; } }

        public int Votes { get { return m_Voters.Count; } }

        public void CleanMuleVotes()
        {
            int goodVotes = 0, badVotes = 0, nullVotes = 0;
            List<IPAddress> addresses = new List<IPAddress>(m_Voters.Count);

            for (int i = 0; i < m_Voters.Count; ++i)
            {
                Voter voter = m_Voters[i];
                Mobile from = voter.From;
                if (from == null)
                {
                    m_Voters.RemoveAt(i--);
                    nullVotes++;
                    continue;
                }

                ElectionWriter.Append("<Voter>");

                ElectionWriter.Append("<Account>" + from.Account.Username + "</Account>");
                ElectionWriter.Append("<IPAddress>" + voter.Address + "</IPAddress>");
                ElectionWriter.Append("<RawName>" + from.RawName + "</RawName>");
                ElectionWriter.Append("<GoodVote>");

                if (addresses.Contains(voter.Address) || !Election.MeetsMinVotingReqs(from) || !CitizenshipState.MeetsRequirements(from))
                {
                    m_Voters.RemoveAt(i--);
                    ElectionWriter.Append("false");
                    badVotes++;
                }
                else
                {
                    addresses.Add(voter.Address);
                    ElectionWriter.Append("true");
                    goodVotes++;
                }

                ElectionWriter.Append("</GoodVote></Voter>");
            }

            ElectionWriter.Append("<Statistics>");
            ElectionWriter.Append("<NullVotes>" + nullVotes + "</NullVotes>");
            ElectionWriter.Append("<BadVotes>" + badVotes + "</BadVotes>");
            ElectionWriter.Append("<GoodVotes>" + goodVotes + "</GoodVotes>");
            ElectionWriter.Append("</Statistics>");
        }

        public Candidate(Mobile mob, string slogan)
        {
            m_Mobile = mob;
            m_Voters = new List<Voter>();
            m_Slogan = slogan;
        }

        public Candidate(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            switch (version)
            {
                case 2:
                    {
                        m_Slogan = reader.ReadString();

                        goto case 1;
                    }
                case 1:
                    {
                        m_Mobile = reader.ReadMobile();

                        int count = reader.ReadEncodedInt();
                        m_Voters = new List<Voter>(count);

                        for (int i = 0; i < count; ++i)
                        {
                            Voter voter = new Voter(reader, m_Mobile);

                            if (voter.From != null)
                                m_Voters.Add(voter);
                        }

                        break;
                    }
                case 0:
                    {
                        m_Mobile = reader.ReadMobile();

                        List<Mobile> mobs = reader.ReadStrongMobileList();
                        m_Voters = new List<Voter>(mobs.Count);

                        for (int i = 0; i < mobs.Count; ++i)
                            m_Voters.Add(new Voter(mobs[i], m_Mobile));

                        break;
                    }
            }
        }

        public void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)2); // version

            writer.Write(m_Slogan);

            writer.Write((Mobile)m_Mobile);

            writer.WriteEncodedInt((int)m_Voters.Count);

            for (int i = 0; i < m_Voters.Count; ++i)
                ((Voter)m_Voters[i]).Serialize(writer);
        }
    }

    public enum ElectionState
    {
        Announced,
        Starting,
        Campaign,
        Election,
        Pending
    }

    public class SloganPrompt : Prompt
    {
        private Election m_Election;

        public SloganPrompt(Election e)
        {
            m_Election = e;
        }

        public override void OnResponse(Mobile from, string text)
        {
            m_Election.SloganResponse(from, text);
        }
    }
}