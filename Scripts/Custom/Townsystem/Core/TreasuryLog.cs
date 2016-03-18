using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;

namespace Server.Custom.Townsystem
{
    public enum TreasuryLogType
    {
        Withdraw,
        Theft,
        TreasuryCapture,
        Pirating,
        GuardPurchase,
        GuardUpkeep,
        TreasuryWall,
        TreasuryWallRefund,
        CitizenDispursement,
        MilitiaDispursement,
        PersonalRequestDispursment,
        CheckDeposit,
        CitizenBuffs,
    }

    public class TreasuryLog
    {
        List<TreasuryLogEntry> Entries = new List<TreasuryLogEntry>();
        public static readonly TimeSpan EntryDisplayLength = TimeSpan.FromDays(14); //how long to logs appear in the treasury?

        public TreasuryLog() { }


        public void AddLogEntry(Mobile from, long amount, TreasuryLogType type) {
            Entries.Add(new TreasuryLogEntry(from, amount, type)); }


        public void DisplayTo(Mobile who)
        {
            //clean old log entries
            int count = Entries.Count;
            for (int i = 0; i < count; i++)
            {
                TreasuryLogEntry le = Entries[i];
				if (le.Recipient == null || le.When + EntryDisplayLength < DateTime.Now)
                {
                    Entries.RemoveAt(i--);
                    count--;
                }
            }

            if (!who.HasGump(typeof(TreasuryLogGump)))
                who.SendGump(new TreasuryLogGump(Entries));
        }

        public void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); //version

            writer.Write((int)Entries.Count);
            foreach (TreasuryLogEntry le in Entries)
            {
                writer.Write(le.Recipient);
                writer.Write(le.When);
                writer.Write(le.Amount);
                writer.Write((byte)le.WithdrawType);
            }
        }

        public TreasuryLog(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();
            switch (version)
            {
                case 0:
                    {
                        int count = reader.ReadInt();
                        for (int i = 0; i < count; i++)
                        {
                            Mobile who = reader.ReadMobile();
                            DateTime when = reader.ReadDateTime();
                            long amount = reader.ReadLong();
                            TreasuryLogType type = (TreasuryLogType)reader.ReadByte();

                            if (who != null)
                                Entries.Add(new TreasuryLogEntry(who, amount, when, type));
                        }
                        break;
                    }
            }
        }
    }

    public class TreasuryLogEntry
    {
        public Mobile Recipient { get; private set; }
        public DateTime When { get; private set; }
        public long Amount { get; private set; }
        public TreasuryLogType WithdrawType { get; private set; }

        public TreasuryLogEntry(Mobile from, long amount, TreasuryLogType type)
			: this(from, amount, DateTime.Now, type)
        {
        }

        public TreasuryLogEntry(Mobile from, long amount, DateTime when, TreasuryLogType type)
        {
            When = when;
            Recipient = from;
            Amount = amount;
            WithdrawType = type;
        }
    }

    public class TreasuryLogGump : Gump
    {
        public TreasuryLogGump(List<TreasuryLogEntry> entries)
            : base(50, 100)
        {
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
            this.AddPage(0);
            this.AddBackground(0, 0, 600, 300, 9200);


            string log = "";

            log = String.Concat(log, "<center><h2>Treasury Activity Log</h2></center><br><br>");
            for (int i = entries.Count - 1; i >= 0; i--)
            {
                TreasuryLogEntry le = entries[i];
                switch (le.WithdrawType)
                {
                    case TreasuryLogType.Withdraw: log = String.Concat(log, String.Format("{3}.   {0} gold withdrawn by {1} on {2}.<br>", le.Amount.ToString(), le.Recipient.Name, le.When.ToShortDateString(), i + 1)); break;
                    case TreasuryLogType.GuardPurchase: log = String.Concat(log, String.Format("{3}.   {0} gold removed for guard purchase by {1} on {2}.<br>", le.Amount.ToString(), le.Recipient.Name, le.When.ToShortDateString(), i + 1)); break;
                    case TreasuryLogType.GuardUpkeep: log = String.Concat(log, String.Format("{2}.   {0} gold removed for guard upkeep on {1}.<br>", le.Amount.ToString(), le.When.ToShortDateString(), i + 1)); break;
                    case TreasuryLogType.Theft: log = String.Concat(log, String.Format("{1}.   The treasury was stolen on {0}!<br>", le.When.ToShortDateString(), i + 1)); break;
                    case TreasuryLogType.TreasuryWall: log = String.Concat(log, String.Format("{3}.   {0} gold spent on treasury walls by {1} on {2}.<br>", le.Amount.ToString(), le.Recipient.Name, le.When.ToShortDateString(), i + 1)); break;
                    case TreasuryLogType.TreasuryWallRefund: log = String.Concat(log, String.Format("{3}.   {0} gold refunded from treasury walls by {1} on {2}.<br>", le.Amount.ToString(), le.Recipient.Name, le.When.ToShortDateString(), i + 1)); break;
                    case TreasuryLogType.TreasuryCapture: log = String.Concat(log, String.Format("{2}.   {0} gold stolen for the town on {1}.<br>", le.Amount.ToString(), le.When.ToShortDateString(), i + 1)); break;
                    case TreasuryLogType.MilitiaDispursement: log = String.Concat(log, String.Format("{3}.   {0} gold distributed to the militia of the town by {1} on {2}.<br>", le.Amount.ToString(), le.Recipient.Name, le.When.ToShortDateString(), i + 1)); break;
                    case TreasuryLogType.CitizenDispursement: log = String.Concat(log, String.Format("{3}.   {0} gold distributed to the citizens of the town by {1} on {2}.<br>", le.Amount.ToString(), le.Recipient.Name, le.When.ToShortDateString(), i + 1)); break;
                    case TreasuryLogType.PersonalRequestDispursment: log = String.Concat(log, String.Format("{3}.   {0} gold requested from the treasury by {1} on {2}.<br>", le.Amount.ToString(), le.Recipient.Name, le.When.ToShortDateString(), i + 1)); break;
                    case TreasuryLogType.CheckDeposit: log = String.Concat(log, String.Format("{3}.   {0} gold deposited into the treasury by {1} on {2}.<br>", le.Amount.ToString(), le.Recipient.Name, le.When.ToShortDateString(), i + 1)); break;
                }
                
            }

            this.AddHtml(11, 11, 575, 279, log, (bool)true, (bool)true);
        }
    }
}
