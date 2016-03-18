/***************************************************************************
 *                            SellingPersistance.cs
 *                          ------------------------
 *   begin                : August 1, 2010
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/
using System;
using Server.Commands;
using System.Collections;
using System.Collections.Generic;
using Server.Engines.Reports;
using System.Net;

namespace Server.Custom
{
    public class SellingPersistance : Item
    {
        private static SellingPersistance m_Instance;
        private static DateTime m_DateLastReset;
        private static Dictionary<string, int> m_TableUsername = new Dictionary<string,int>();
        private static Dictionary<IPAddress, int> m_TableIP = new Dictionary<IPAddress,int>();
        public static Int32 m_TotalGoldPerDay = 5000; //TOTAL GOLD PER IP PER DAY

        public override string DefaultName
        {
            get { return "Selling Persistance - Internal"; }
        }


        public SellingPersistance()
            : base(1)
		{
			Movable = false;

			if ( m_Instance == null || m_Instance.Deleted )
				m_Instance = this;
			else
				base.Delete();
		}


        public SellingPersistance( Serial serial ) : base( serial )
		{
		}

        public static void Initialize()
        {
            CommandSystem.Register("SetTotalGold", AccessLevel.Administrator, new CommandEventHandler(SetTotalGold_OnCommand));
            CommandSystem.Register("GetTotalGold", AccessLevel.Counselor, new CommandEventHandler(GetTotalGold_OnCommand));
        }

        [Usage("SetTotalGold")]
        [Description("Sets the total gold allotted for players to sell items to vendors per day")]
        public static void SetTotalGold_OnCommand(CommandEventArgs e)
        {
            new SellingPersistance();

            Mobile from = e.Mobile;
            if (e.Length == 1)
            {
                try
                {
                    int goldAmt = Int32.Parse(e.GetString(0).Trim());
                    m_TotalGoldPerDay = goldAmt;
                }
                catch { from.SendMessage("Bad Format: SetTotalGold ####"); }


            }
            else
            {
                e.Mobile.SendMessage(0x25, "Bad Format: SetTotalGold ####");
            }
        }

        [Usage("GetTotalGold")]
        [Description("Gets the total gold allotted for players to sell items to vendors per day")]
        public static void GetTotalGold_OnCommand(CommandEventArgs e)
        {
            new SellingPersistance();

            Mobile from = e.Mobile;
            e.Mobile.SendMessage(0x25, String.Format("Total Gold Allotted Per Day Per Person: {0}", m_TotalGoldPerDay));
        }

        public static bool PlayerRequestSell(Mobile from, Int32 amount)
        {
            if (amount > m_TotalGoldPerDay)
                return false;

            string UNkey = GetAccountFromPlayer(from);
            IPAddress IPkey = GetIPFromPlayer(from);

            if (m_TableUsername.ContainsKey(UNkey) && m_TableIP.ContainsKey(IPkey))
            {
                if ((m_TableUsername[UNkey] + amount) > m_TotalGoldPerDay || (m_TableIP[IPkey] + amount) > m_TotalGoldPerDay)
                    return false;
                else
                {
                    m_TableUsername[UNkey] += amount;
                    m_TableIP[IPkey] += amount;
                    return true;
                }
            }
            else if (m_TableUsername.ContainsKey(UNkey))
            {
               if ((m_TableUsername[UNkey] + amount) > m_TotalGoldPerDay)
                   return false;
               else
               {
                    m_TableUsername[UNkey] += amount;
                    m_TableIP[IPkey] = amount;
                    return true;
               }
            }
            else if (m_TableIP.ContainsKey(IPkey))
            {
                if ((m_TableIP[IPkey] + amount) > m_TotalGoldPerDay)
                    return false;
                else
                {
                    m_TableUsername[UNkey] = amount;
                    m_TableIP[IPkey] += amount;
                    return true;
                }
            }
            else
            {
                m_TableUsername[UNkey] = amount;
                m_TableIP[IPkey] = amount;
                return true;
            }
        }

        private static void Reset()
        {
            m_TableUsername.Clear();
            m_TableIP.Clear();
            m_DateLastReset = DateTime.UtcNow;
        }

        private static string GetAccountFromPlayer(Mobile from)
        {
            return from.Account.Username;
        }

        private static IPAddress GetIPFromPlayer(Mobile from)
        {
            return from.NetState.Address;
        }

        public override void Serialize(GenericWriter writer)
        {
            if (m_DateLastReset.Date < DateTime.UtcNow.Date)
                Reset();

            base.Serialize(writer);
            writer.WriteEncodedInt(0);

            writer.Write((int)m_TotalGoldPerDay);
            writer.Write((DateTime)m_DateLastReset);

            writer.Write((int)m_TableUsername.Count);            
            foreach (KeyValuePair<string, int> m in m_TableUsername)
            {
                writer.Write((string)m.Key);
                writer.Write((int)m.Value);
            }

            writer.Write(m_TableIP.Count);
            foreach (KeyValuePair<IPAddress, int> m in m_TableIP)
            {
                writer.Write(m.Key);
                writer.Write(m.Value);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();

            m_TotalGoldPerDay = reader.ReadInt();
			
			// !!!!!!!!!!!!!!!!!!!
			m_TotalGoldPerDay = 5000;
			// !!!!!!!!!!!!!!!!!!!

            m_DateLastReset = reader.ReadDateTime();

            int count = reader.ReadInt();

            for (int i = 0; i < count; ++i)
            {
                string key = reader.ReadString();
                int value = reader.ReadInt();

                m_TableUsername[key] = value;
            }

            int count2 = reader.ReadInt();
            for (int i = 0; i < count2; ++i)
            {
                IPAddress key = reader.ReadIPAddress();
                int value = reader.ReadInt();

                m_TableIP[key] = value;
            }


            m_Instance = this;
        }
    }
}
