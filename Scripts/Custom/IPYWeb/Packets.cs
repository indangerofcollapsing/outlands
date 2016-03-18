using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Network;
using Server.Accounting;
using Server.Commands;
using Server.Custom.Townsystem;
using System.Text;

namespace Server.IPYWeb
{
	public enum WebAckResponse : byte
	{
		NoUser = 0,
		BadIP,
		BadPass,
		NoAccess,
        Banned,
        AlreadyRegistered,
		OK
	}
	
	public sealed class PasswordChangeAckPacket : Packet
	{
        public PasswordChangeAckPacket(WebAckResponse resp)
            : base(0x02, 2)
		{
			m_Stream.Write( (byte)resp );
		}
	}

    public sealed class RegisterEmailAckPacket : Packet
    {
        public RegisterEmailAckPacket(WebAckResponse resp)
            : base(0x02, 2)
        {
            m_Stream.Write((byte)resp);
        }
    }

    public sealed class IPYServerStatisticsPacket : Packet
    {
        public IPYServerStatisticsPacket()
            : base(0x02, 13+Town.Towns.Count*231)
        {
            m_Stream.Write(Server.RemoteAdmin.ServerInfo.NetStateCount());
			m_Stream.WriteAsciiFixed(FormatServerUptime(DateTime.UtcNow - Clock.ServerStart), 8);
            Town t;
            int count = Town.Towns.Count;
            for (int i = 0; i < count; i++ )
            {
                t = Town.Towns[i];
                m_Stream.WriteAsciiFixed(t.Definition.FriendlyName, 30);
                m_Stream.WriteAsciiFixed(t.King == null ? "the ex-King" : t.King.RawName, 30);
                m_Stream.Write(t.King == null ? true : !t.King.Female);
				m_Stream.WriteAsciiFixed(t.HomeFaction == null ? "Independent" : t.HomeFaction.Alliance == null ? "Independent" : t.HomeFaction.Alliance.Name, 30);
				m_Stream.WriteAsciiFixed(t.ControllingTown == null ? "Uncontrolled" : t.ControllingTown.Definition.FriendlyName, 30);
                m_Stream.WriteAsciiFixed(TownBuff.GetBuffName(t.PrimaryCitizenshipBuff), 30);
                m_Stream.WriteAsciiFixed(t.Election == null ? "Undetermined" : FormatLongTime(t.Election.NextElection), 35);
                m_Stream.WriteAsciiFixed(t.GuardState.ToString(), 30);
                m_Stream.Write((int)(t.SalesTax * 100));
                m_Stream.Write((int)t.ActiveCitizens);
                m_Stream.Write((int)t.ActiveMilitia);
                m_Stream.Write((int)t.Exiles.Count);
                m_Stream.Write((int)t.Treasury);
            }
        }

        public static string FormatServerUptime(TimeSpan ts)
        {
            if (ts.TotalDays > 1)
                return String.Format("{0:D}d {1:D}h", ts.Days, ts.Hours % 24);
            else if (ts.TotalHours > 1)
                return String.Format("{0:D}h {1:D}m", ts.Hours % 24, ts.Minutes % 60);
            else
                return String.Format("{0:D}m", ts.Minutes % 60);
        }

        public static string FormatLongTime(TimeSpan ts)
        {
            var sb = new StringBuilder();

            if (ts.TotalDays > 7)
                return String.Format("{0:D} weeks {1:D} days and {2:D} hours", (int)(ts.Days/7), ts.Days % 7, ts.Hours % 24);
            if (ts.TotalDays > 1)
                return String.Format("{0:D} days and {1:D} hours", ts.Days, ts.Hours % 24);
            else if (ts.TotalHours > 1)
                return String.Format("{0:D} hours and {1:D} minutes", ts.Hours % 24, ts.Minutes % 60);
            else
                return String.Format("{0:D} minutes", ts.Minutes % 60);
        }
    }
}
