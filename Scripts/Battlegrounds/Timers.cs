using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Battlegrounds
{
    public class CTFAutobalancer : Timer
    {
        private CTFBattleground m_Battleground;

        public CTFAutobalancer(CTFBattleground battleground) 
            : base(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1)) 
        {
            m_Battleground = battleground;
            Priority = TimerPriority.OneMinute;
        }

        protected override void OnTick()
        {
            base.OnTick();
            if (!m_Battleground.Active || m_Battleground.Ended)
            {
                Stop();
                return;
            }

            m_Battleground.BalanceTeams();
        }
    }

    public class SiegeAutobalancer : Timer
    {
        private SiegeBattleground m_Battleground;

        public SiegeAutobalancer(SiegeBattleground battleground)
            : base(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1))
        {
            Priority = TimerPriority.OneMinute;
            m_Battleground = battleground;
        }

        protected override void OnTick()
        {
            base.OnTick();
            if (!m_Battleground.Active || m_Battleground.Ended)
            {
                Stop();
                return;
            }

            int difference = Math.Abs(m_Battleground.Offense.Count - m_Battleground.Defense.Count);
            if (difference >= 2)
            {
                m_Battleground.BalanceTeams(difference);
            }
        }
    }

    public class GameOverTimer : Timer
    {
        private Battleground m_Battleground;

        public GameOverTimer(Battleground battleground)
            : base(TimeSpan.Zero, TimeSpan.FromMinutes(1))
        {
            Priority = TimerPriority.OneMinute;
            m_Battleground = battleground;
        }

        protected override void OnTick()
        {
            base.OnTick();
            if (m_Battleground.Active && !m_Battleground.Ended)
            {
                if (m_Battleground.StartTime + m_Battleground.TimeLimit <= DateTime.UtcNow)
                    m_Battleground.OnTimeout();
            }
            else
            {
                Stop();
            }
        }
    }
}
