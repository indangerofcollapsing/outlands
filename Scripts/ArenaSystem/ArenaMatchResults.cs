using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.ArenaSystem
{
    public class LimitedArenaMatchResult
    {
        public string m_team1Name;
        public string m_team2Name;
        public ArenaMatch.EMatchEndType m_endType;
        public string m_winnerName;
        public int m_team1ScoreDelta;
        public int m_team2ScoreDelta;
        public string m_seasonName;

        public LimitedArenaMatchResult(string team1Name, string team2Name, ArenaMatch.EMatchEndType endType,
            string winnerName, int team1ScoreDelta, int team2ScoreDelta, string seasonName)
        {
            m_team1Name = team1Name;
            m_team2Name = team2Name;
            m_endType = endType;
            m_winnerName = winnerName;
            m_team1ScoreDelta = team1ScoreDelta;
            m_team2ScoreDelta = team2ScoreDelta;
            m_seasonName = seasonName;
        }
        public LimitedArenaMatchResult(GenericReader reader)
        {
            int version = reader.ReadInt();

            if (version >= 1)
            {
                m_team1Name = reader.ReadString();
                m_team2Name = reader.ReadString();
                m_endType = (ArenaMatch.EMatchEndType)reader.ReadInt();
                m_winnerName = reader.ReadString();
                m_team1ScoreDelta = reader.ReadInt();
                m_team2ScoreDelta = reader.ReadInt();
                m_seasonName = reader.ReadString();
            }
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(1);

            // Version 1
            writer.Write(m_team1Name);
            writer.Write(m_team2Name);
            writer.Write((int)m_endType);
            writer.Write(m_winnerName);
            writer.Write(m_team1ScoreDelta);
            writer.Write(m_team2ScoreDelta);
            writer.Write(m_seasonName);
        }
    }
}
