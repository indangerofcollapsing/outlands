using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;

namespace Server.Custom.Townsystem
{
    public class Withdraws
    {
        public enum WithdrawType
        {
            Personal,
            MilitiaDispursement,
            CitizenDispursement
        }

        public class WithdrawEntry
        {
            public DateTime TimeStamp { get; set; }
            public int Amount { get; set; }
            public bool Given { get; set; }
            public WithdrawType WithdrawType { get; set; }

            public WithdrawEntry(int amount, WithdrawType type)
            {
                TimeStamp = DateTime.Now;
                Amount = amount;
                Given = false;
                WithdrawType = type;
            }

            public WithdrawEntry(int amount, DateTime timeStamp, bool given, WithdrawType type)
            {
                TimeStamp = timeStamp;
                Amount = amount;
                Given = given;
                WithdrawType = type;
            }
        }

        public static readonly TimeSpan WithdrawFrequency = TimeSpan.FromDays(7);
        public static readonly TimeSpan PendingPeriod = TimeSpan.FromDays(1);
        public static readonly double WithdrawPercent = 0.1;
        public static readonly int WithdrawCap = 200000;
        private InternalTimer m_Timer;

        public List<WithdrawEntry> Entries;
        public Town Town;

        public int AvailableToWithdraw
        {
            get
            {
                int withdrawnAmount = 0;
                DateTime now = DateTime.Now;

                for (int i = 0; i < Entries.Count; i++)
                {
                    var entry = Entries[i];
                    if (entry.TimeStamp + WithdrawFrequency < now)
                    {
                        Entries.RemoveAt(i--);
                        continue;
                    }

                    withdrawnAmount += entry.Amount;
                }

                int withdrawCapacity = (int)(Town.Treasury * WithdrawPercent);
                // hard limit king withdrawals to 200k
                if (withdrawCapacity > WithdrawCap)
                    withdrawCapacity = WithdrawCap;

                return Math.Max(0, withdrawCapacity - withdrawnAmount);
            }
        }

        public Withdraws(Town town)
        {
            Entries = new List<WithdrawEntry>();
            Town = town;
        }

        public bool CanWithdraw(int toWithdraw)
        {
            return toWithdraw <= AvailableToWithdraw;
        }

        public void Withdraw(Mobile m, int toWithdraw, WithdrawType type)
        {
            if (toWithdraw < 1)
                return;

            var entry = new WithdrawEntry(toWithdraw, type);
            Entries.Add(entry);

            if (m_Timer == null || !m_Timer.Running)
            {
                m_Timer = new InternalTimer(this);
                m_Timer.Start();
            }
        }

        public void StopTimer()
        {
            if (m_Timer != null && m_Timer.Running)
            {
                m_Timer.Start();
            }
        }

        public void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); //version

            writer.Write(Entries.Count);
            foreach (WithdrawEntry entry in Entries)
            {
                writer.Write(entry.TimeStamp);
                writer.Write(entry.Amount);
                writer.Write((byte)entry.WithdrawType);
                writer.Write((bool)entry.Given);
            }
        }

        public static Withdraws Deserialize(Town town, GenericReader reader)
        {
            var withdraws = new Withdraws(town);
            int version = reader.ReadEncodedInt();

            int count = reader.ReadInt();

            for (int i = 0; i < count; i++)
            {
                DateTime dateTime = reader.ReadDateTime();
                int amount = reader.ReadInt();
                WithdrawType type = (WithdrawType)reader.ReadByte();
                bool given = reader.ReadBool();
                withdraws.Entries.Add(new WithdrawEntry(amount, dateTime, given, type));
            }


            if (count > 0)
            {
                int index = count - 1;
                var entry = withdraws.Entries[index];
                if (!entry.Given)
                {
                    withdraws.m_Timer = new InternalTimer(withdraws);
                    withdraws.m_Timer.Start();
                }
            }
            return withdraws;
        }

        public static TimeSpan GetDelay(WithdrawType type)
        {
            if (type == WithdrawType.Personal)
                return TimeSpan.FromHours(36);
            else
                return TimeSpan.FromHours(3);
        }

        public bool IsPendingTransaction
        {
            get { return (Entries != null && Entries.Count > 0 && Entries[Entries.Count - 1].Given == false); }
        }

        public void CompleteWithdraw(WithdrawEntry entry)
        {
            if (Town.King == null)
                return;

            entry.Given = true;

            Mobile from = Town.King;

            int amount = entry.Amount;
            Town.Treasury -= amount;

            amount = (int)Math.Ceiling(0.85 * amount);


            switch ((int)entry.WithdrawType)
            {
                case (int)WithdrawType.Personal:
                    {
                        while (amount > 100000)
                        {
                            Town.King.AddToBackpack(new BankCheck(100000));
                            amount -= 100000;
                        }

                        from.AddToBackpack(new BankCheck((int)amount));
                        from.SendMessage(0, "You have withdrawn {0} gold from the {1} treasury.", amount, Town.Definition.FriendlyName);

						Town.LastTreasuryWithdraw = DateTime.Now;
                        Town.WithdrawLog.AddLogEntry(from, amount, TreasuryLogType.Withdraw);

                        break;
                    }
                case (int)WithdrawType.CitizenDispursement:
                    {
                        List<Mobile> Citizens = new List<Mobile>(Town.Members.Count);
                        foreach (CitizenshipState state in Town.Members)
                        {
                            Mobile mob = state.Mobile;

                            if (CitizenshipState.MeetsDispursementRequirements(mob))
                                Citizens.Add(mob);
                        }

                        int toGive = Math.Max(1, amount / Math.Max(1, Citizens.Count));

                        foreach (Mobile m in Citizens)
                            if (m.BankBox != null)
                            {
                                var bag = new Bag();
                                bag.DropItem(new Gold(toGive));
                                bag.DropItem(new DispursementNote(Town));

                                m.BankBox.AddItem(bag);
                            }

                        Town.WithdrawLog.AddLogEntry(from, amount, TreasuryLogType.CitizenDispursement);

                        break;
                    }
                case (int)WithdrawType.MilitiaDispursement:
                    {
                        List<Mobile> Members = new List<Mobile>(Town.MilitiaMembers.Count);
                        foreach (Mobile mob in Town.MilitiaMembers)
                        {
                            if (!(mob is PlayerMobile))
                                continue;

                            if (CitizenshipState.MeetsDispursementRequirements(mob) && ((PlayerMobile)mob).TownsystemPlayerState.KillPoints > 0)
                                Members.Add(mob);
                        }

                        int toGive = Math.Max(1, amount / Math.Max(1, Members.Count));

                        foreach (Mobile m in Members)
                            if (m.BankBox != null)
                            {
                                var bag = new Bag();
                                bag.DropItem(new Gold(toGive));
                                bag.DropItem(new DispursementNote(Town));

                                m.BankBox.AddItem(bag);
                            }

                        Town.WithdrawLog.AddLogEntry(from, amount, TreasuryLogType.MilitiaDispursement);
                        break;
                    }
            }
        }

        public class InternalTimer : Timer
        {
            private Withdraws m_Withdraws;

            public InternalTimer(Withdraws withdraws)
                : base(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1))
            {
                m_Withdraws = withdraws;
                Priority = TimerPriority.OneMinute;
            }

            protected override void OnTick()
            {
                int index = m_Withdraws.Entries.Count - 1;
                if (index < 0)
                {
                    Stop();
                    return;
                }

                var entry = m_Withdraws.Entries[index];

                if (!entry.Given && entry.TimeStamp + GetDelay(entry.WithdrawType) < DateTime.Now)
                {
                    m_Withdraws.CompleteWithdraw(entry);
                    Stop();
                }
            }
        }
    }
}
