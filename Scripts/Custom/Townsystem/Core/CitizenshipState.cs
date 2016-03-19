using System;
using Server;
using Server.Mobiles;
using System.Collections.Generic;
using System.Collections;

namespace Server.Custom.Townsystem
{
	public class CitizenshipState : IComparable
	{
        public class TimePlayed
        {
            public class TimeSlot
            {
                public DateTime Added { get; set; }
                public TimeSpan Amount { get; set; }

                public TimeSlot()
                {
                }
            }

            public List<TimeSlot> Times { get; set; }

            public void RegisterTimePlayed(TimeSpan time)
            {
                var t = new TimeSlot();
				t.Added = DateTime.Now;
                t.Amount = time;

                Times.Add(t);
            }

            public void Defrag()
            {
                Queue q = new Queue();

                foreach (TimeSlot time in Times)
					if (time.Added + TimeSpan.FromDays(14) < DateTime.Now)
                        q.Enqueue(time);

                while (q.Count > 0)
                    Times.Remove(((TimeSlot)q.Dequeue()));
            }

            public TimeSpan Total()
            {
                Queue q = new Queue();
                TimeSpan played = TimeSpan.Zero;

                foreach (TimeSlot time in Times)
					if (time.Added + TimeSpan.FromDays(14) < DateTime.Now)
                        q.Enqueue(time);
                    else
                        played += time.Amount;

                while (q.Count > 0)
                    Times.Remove(((TimeSlot)q.Dequeue()));

                return played;
            }

            public void Serialize(GenericWriter writer)
            {
                Defrag();

                writer.WriteEncodedInt(0);

                //version 0
                if (Times == null)
                    writer.Write((int)0);
                else
                {
                    writer.Write(Times.Count);

                    foreach (TimeSlot t in Times)
                    {
                        writer.Write(t.Added);
                        writer.Write(t.Amount);
                    }
                }
            }

            public static TimePlayed Deserialize(GenericReader reader)
            {
                TimePlayed played = new TimePlayed();

                int version = reader.ReadEncodedInt();

                int count = reader.ReadInt();

                played.Times = new List<TimeSlot> { };

                TimeSpan twoWeeks = TimeSpan.FromDays(14);
				DateTime now = DateTime.Now;

                for (int i = 0; i < count; i++)
                {
                    DateTime added = reader.ReadDateTime();
                    TimeSpan amount = reader.ReadTimeSpan();

                    if (added + twoWeeks > now)
                    {
                        var slot = new TimeSlot();
                        slot.Added = added;
                        slot.Amount = amount;
                        played.Times.Add(slot);
                    }
                }

                return played;
            }

            public TimePlayed()
            {
                Times = new List<TimeSlot>();
            }
        }

		private Mobile m_Mobile;
        private Town m_Town;
		private DateTime m_Leaving;
        private DateTime _startDate;

        public Town Town { get { return m_Town; } }
		public Mobile Mobile{ get{ return m_Mobile; } }
		public DateTime Leaving{ get{ return m_Leaving; } set{ m_Leaving = value; } }
		public bool IsLeaving{ get{ return ( m_Leaving > DateTime.MinValue ); } }
        public DateTime StartDate { get { return _startDate; } set { _startDate = value; } }

        public DateTime LastMonsterKill { get; set; }
        public DateTime LastVendorInteraction { get; set; }

        public TimePlayed GameTime { get; set; }

        public static void RegisterGameTime(Mobile m, TimeSpan time)
        {
            CitizenshipState state = Find(m);

            if (state == null)
                return;

            if (state.GameTime == null)
                state.GameTime = new TimePlayed();

            state.GameTime.RegisterTimePlayed(time);
        }

        public static TimeSpan GetTimePlayed(Mobile m)
        {
            CitizenshipState state = Find(m);

            if (state == null || state.GameTime == null)
                return TimeSpan.Zero;

            TimeSpan playtime = state.GameTime.Total();

            if (m.NetState != null)
				playtime += (DateTime.Now - ((PlayerMobile)m).SessionStart);

            return playtime;
        }

        public static void UpdateMonsterKill(Mobile m)
        {
            CitizenshipState state = Find(m);

            if (state != null)
				state.LastMonsterKill = DateTime.Now;
        }

        public static void UpdateVendorInteraction(Mobile m)
        {
            CitizenshipState state = Find(m);

            if (state != null)
				state.LastVendorInteraction = DateTime.Now;
        }

        public static bool MeetsDispursementRequirements(Mobile m)
        {
            CitizenshipState state = Find(m);

            if (state == null)
                return false;

            PlayerMobile pm = m as PlayerMobile;
            if (pm == null)
                return false;

			DateTime now = DateTime.Now;
            TimeSpan oneWeek = TimeSpan.FromDays(7);

			// 12/04/13 - Changed from 60 days to 14 days.
            bool startReq = m.CreationTime + oneWeek < now;
            bool lastLogin = pm.LastOnline + TimeSpan.FromDays(14) > now;

            return (startReq && lastLogin);
        }

        public static bool MeetsRequirements(Mobile m)
        {
            CitizenshipState state = Find(m);

            if (state == null)
                return false;

			DateTime now = DateTime.Now;
            TimeSpan oneWeek = TimeSpan.FromDays(7);
            TimeSpan twoWeeks = TimeSpan.FromDays(14);

            bool startReq = m.CreationTime + oneWeek < now;
            bool monsterReq = state.LastMonsterKill + twoWeeks > now;
            bool vendorReq = state.LastVendorInteraction + twoWeeks > now;
            bool playTime = GetTimePlayed(m) > TimeSpan.FromHours(10.5);
            var playerM = m as PlayerMobile;
            bool militiaPlayer = false;
            if (playerM != null)
                militiaPlayer = playerM.OCBAllianceScore > 0 || playerM.TreasuryKeys > 0;

            bool mageMeetsMin = m.Skills.Magery.Value >= 90.0;
            bool tacticsMeetsMin = m.Skills.Tactics.Value >= 90.0;
            bool thief = m.Skills.Stealing.Value >= 90.0;
            bool crafter = (m.Skills.Tailoring.Value >= 90.0 || m.Skills.Blacksmith.Value >= 90.0 || m.Skills.Fletching.Value >= 90.0 || m.Skills.Alchemy.Value >= 90.0 || m.Skills.Carpentry.Value >= 90.0 || m.Skills.Cooking.Value >= 90.0 || m.Skills.Tinkering.Value >= 90.0);
            bool skillReqs = mageMeetsMin || tacticsMeetsMin || thief || crafter;



            return (startReq && (monsterReq || vendorReq || militiaPlayer) && playTime && skillReqs);
        }

        public static CitizenshipState Find(Mobile m)
        {
            if (m == null || !(m is PlayerMobile))
                return null;

            return ((PlayerMobile)m).CitizenshipPlayerState;
        }

		public void Invalidate()
		{
			if ( m_Mobile is PlayerMobile )
			{
				PlayerMobile pm = (PlayerMobile)m_Mobile;

				pm.InvalidateProperties();
			}
		}

		public void Attach()
		{
            if (m_Mobile is PlayerMobile && ((PlayerMobile)m_Mobile).CitizenshipPlayerState == null)
				((PlayerMobile)m_Mobile).CitizenshipPlayerState = this;
		}

		public  CitizenshipState( Mobile mob, Town town)
		{
            m_Town = town;
			m_Mobile = mob;
			_startDate = DateTime.Now;
			Attach();
			Invalidate();
            GameTime = new TimePlayed();
		}

        public CitizenshipState(GenericReader reader, Town town)
		{
            m_Town = town;
            TimeSpan previousSession = TimeSpan.Zero;

			int version = reader.ReadEncodedInt();
			switch ( version )
			{
                case 3:
                    {
                        goto case 2;
                    }
                case 2:
                    {
                        previousSession = reader.ReadTimeSpan();
                        goto case 1;
                    }
                case 1:
                    {
                        LastMonsterKill = reader.ReadDateTime();
                        LastVendorInteraction = reader.ReadDateTime();
                        GameTime = TimePlayed.Deserialize(reader);

                        if (previousSession > TimeSpan.Zero)
                            GameTime.RegisterTimePlayed(previousSession);

                        goto case 0;
                    }
				case 0:
				{
                    m_Town = Town.ReadReference(reader);
					m_Mobile = reader.ReadMobile();
					m_Leaving = reader.ReadDateTime();
                    _startDate = reader.ReadDateTime();
					break;
				}
			}

            if (version == 0)
                GameTime = new TimePlayed();

            if (version == 2 && m_Mobile != null && m_Leaving > DateTime.MinValue)
            {
                if (Town.CanLeave(m_Mobile))
					m_Leaving = DateTime.Now - TimeSpan.FromHours(1);
                else
                    m_Leaving = DateTime.MinValue;

            }

			Attach();
		}

		public void Serialize( GenericWriter writer )
		{
			writer.WriteEncodedInt( (int) 3 ); // version

            //version 3: citizenship leaving change

            //version 2
            if (m_Mobile != null && m_Mobile is PlayerMobile && ((PlayerMobile)m_Mobile).NetState != null)
				writer.Write(DateTime.Now - ((PlayerMobile)m_Mobile).SessionStart);
            else
                writer.Write(TimeSpan.Zero);

            //version 1
            writer.Write(LastMonsterKill);
            writer.Write(LastVendorInteraction);

            if (GameTime == null)
                GameTime = new TimePlayed();

            GameTime.Serialize(writer);


            //version 0
            Town.WriteReference(writer, m_Town);
			writer.Write( (Mobile) m_Mobile );
			writer.Write( (DateTime) m_Leaving );
            writer.Write((DateTime)_startDate);
		}

		public int CompareTo( object obj )
		{
            return ((CitizenshipState)obj)._startDate.CompareTo(_startDate);
		}
	}
}