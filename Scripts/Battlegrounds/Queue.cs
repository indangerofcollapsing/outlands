using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Battlegrounds
{
    public class Queue
    {
        private List<PlayerMobile> m_Players;
        private Battleground m_Battleground;

        public Queue(Battleground battleground)
        {
            m_Players = new List<PlayerMobile>();
            m_Battleground = battleground;
        }

        public void Join(PlayerMobile player)
        {
            if (AlreadyQueued(player))
                return;
            
            m_Players.Add(player);
            player.SendMessage(string.Format("You have joined the {0} queue.", m_Battleground.Name));

            m_Battleground.TryStart();
        }

        public void Leave(PlayerMobile player)
        {
            if (m_Players.Contains(player))
                m_Players.Remove(player);
        }

        public bool AlreadyQueued(PlayerMobile player)
        {
            return m_Players.Contains(player);
        }

        public List<PlayerMobile> Players { get { Clean(); return m_Players; } }

        private void Clean()
        {
            for (int i = 0; i < m_Players.Count; i++)
                if (m_Players[i].NetState == null)
                    m_Players.RemoveAt(i--);
        }

        public int Count
        {
            get { return m_Players.Count; }
        }
    }
}
