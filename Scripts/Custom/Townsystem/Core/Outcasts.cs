using System;
using System.Collections.Generic;
using Server;

namespace Server.Custom.Townsystem
{
    public class OutcastEntry: IComparable
    {
        private Town m_Town;
        private Mobile m_Poster;
        private Mobile m_Outcast;
        private int m_Count;
        private DateTime m_StartDate;
        private string m_Title;
        private string m_Description;
        private List<Mobile> m_Voters;

        public Town Town { get { return m_Town; } }
        public Mobile Poster { get { return m_Poster; } }
        public Mobile Outcast { get { return m_Outcast; } }
        public int Count { get { return m_Count; } set { m_Count = value; } }
        public DateTime StartDate { get { return m_StartDate; } }
        public string Title { get { return m_Title; } }
        public string Description { get { return m_Description; } }
        public List<Mobile> Voters { get { return m_Voters; } set { m_Voters = value; } }

        public OutcastEntry( Mobile poster, Mobile outcast, Town town, string title, string desc)
		{
            m_Poster = poster;
            m_Outcast = outcast;
            m_Town = town;
			m_StartDate = DateTime.Now;
            m_Count = 0;
            m_Title = title;
            m_Description = desc;
            m_Voters = new List<Mobile>();
		}

        public int CompareTo(object obj)
        {
            return (((OutcastEntry)obj).m_Count - m_Count);
        }

        public OutcastEntry(GenericReader reader)
		{
			int version = reader.ReadEncodedInt();

			switch ( version )
			{
				case 0:
				{
                    m_Poster = reader.ReadMobile();
                    m_Outcast = reader.ReadMobile();
                    m_Town = Town.ReadReference(reader);
                    m_StartDate = reader.ReadDateTime();
                    m_Count = reader.ReadEncodedInt();
                    m_Title = reader.ReadString();
                    m_Description = reader.ReadString();

                    m_Voters = new List<Mobile>();
                    int count = reader.ReadEncodedInt();
                    for (int i = 0; i < count; ++i)
                        m_Voters.Add((Mobile)reader.ReadMobile());
				}break;
			}
		}

        public void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)0); // version
            writer.WriteMobile(m_Poster);
            writer.WriteMobile(m_Outcast);
            Town.WriteReference(writer, m_Town);
            writer.Write((DateTime)m_StartDate);
            writer.WriteEncodedInt((int)m_Count);
            writer.Write((string)m_Title);
            writer.Write((string)m_Description);

            //Braziers
            writer.WriteEncodedInt((int)m_Voters.Count);
            for (int i = 0; i < m_Voters.Count; ++i)
                writer.WriteMobile((Mobile)m_Voters[i]);

        }
    }

    public class Outcasts
    {
        public static Mobile FromName(string strName)
        {
            string match = strName.Trim().ToLower();

            foreach (Mobile i in World.Mobiles.Values)
                if (i.Account != null && String.Compare(i.RawName,match,true) == 0)
                    return i;

            return null;
        }
    }
}
