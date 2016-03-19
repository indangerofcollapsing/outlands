using System;
using System.Collections.Generic;
using Server;
using Server.Commands;
using System.IO;

namespace Server.Custom
{
    public static class CurrencyTracking
    {
        public static double m_TotalGold = 0;
        public static double m_TotalPlatinum = 0;
        public static double m_TotalDreadCoin = 0;
        public static double m_TotalDoubloons = 0;
        public static double m_TotalGhostCoin = 0;
        public static double m_TotalMonsterCoin = 0;

        public static void Initialize()
        {
            CommandSystem.Register("TotalCurrency", AccessLevel.Counselor, new CommandEventHandler(TotalCurrency_OnCommand));
            new CurrencyTracker();
        }

        [Usage("TotalCurrency")]
        [Description("Returns the total amount of Currency coin the world currently contains.")]
        public static void TotalCurrency_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            from.SendMessage(String.Format("There is currently {0} gold coin, {1} platinum coin, {2} dread coin, {3} doubloon, {4} ghost coins and {5} monster coins in the world.", m_TotalGold, m_TotalPlatinum, m_TotalDreadCoin, m_TotalDoubloons, m_TotalGhostCoin, m_TotalMonsterCoin));
        }

        #region Currency Register/Delete

        //Gold
        public static void RegisterGold(long amount)
        {
            m_TotalGold += amount;
        }

        public static void DeleteGold(long amount)
        {
            m_TotalGold -= amount;
        }

        //Platinum
        public static void RegisterPlatinum(long amount)
        {
            m_TotalPlatinum += amount;
        }

        public static void DeletePlatinum(long amount)
        {
            m_TotalPlatinum -= amount;
        }

        //Dread Coin
        public static void RegisterDreadCoin(long amount)
        {
            m_TotalDreadCoin += amount;
        }

        public static void DeleteDreadCoin(long amount)
        {
            m_TotalDreadCoin -= amount;
        }

        //Doubloons
        public static void RegisterDoubloons(long amount)
        {
            m_TotalDoubloons += amount;
        }

        public static void DeleteDoubloons(long amount)
        {
            m_TotalDoubloons -= amount;
        }

        //Ghost Coin
        public static void RegisterGhostCoins(long amount)
        {
            m_TotalGhostCoin += amount;
        }

        public static void DeleteGhostCoins(long amount)
        {
            m_TotalGhostCoin -= amount;
        }

        //Monster Coin
        public static void RegisterMonsterCoin(long amount)
        {
            m_TotalMonsterCoin += amount;
        }

        public static void DeleteMonsterCoin(long amount)
        {
            m_TotalMonsterCoin -= amount;
        }

        #endregion

        public static void Save()
        {
            /*
            using (StreamWriter writer = new StreamWriter("CurrencyLog.txt", true))
            {
                writer.WriteLine("{0},{1},{2},{3},{4},{5}", DateTime.UtcNow.ToShortDateString(),
                    m_TotalGold, m_TotalPlatinum, m_TotalDreadCoin, m_TotalDoubloons, m_TotalGhostCoin, m_TotalMonsterCoin);
            }

            CurrencyTracker._dateLastSaved = DateTime.UtcNow;
             */
        }

        public class CurrencyTracker : Item
        {
            private static CurrencyTracker m_Instance;
            public static DateTime _dateLastSaved;

            public override string DefaultName { get { return "Currency Tracker - Internal"; } }
            public CurrencyTracker()
                : base(1)
            {
                Movable = false;

                if (m_Instance == null || m_Instance.Deleted)
                    m_Instance = this;
                else
                    base.Delete();
            }

            public CurrencyTracker(Serial serial)
                : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                if (_dateLastSaved.Date < DateTime.UtcNow.Date)
                    Save();

                base.Serialize(writer);
                writer.WriteEncodedInt((int)0);
                writer.Write((DateTime)_dateLastSaved);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                int version = reader.ReadEncodedInt();
                _dateLastSaved = reader.ReadDateTime();

                m_Instance = this;
            }
        }
    }
}
