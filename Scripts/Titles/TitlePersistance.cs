using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    public class TitlePersistance
    {
        public static TitlePersistanceItem PersistanceItem;

        public static List<TitleCollection> m_TitleCollections = new List<TitleCollection>();

        public static Timer m_Timer;

        public static void Initialize()
        {
            Timer.DelayCall(TimeSpan.FromTicks(1), delegate
            {
                if (PersistanceItem == null)
                    PersistanceItem = new TitlePersistanceItem();

                m_Timer = new TitleTimer();
                m_Timer.Start();
            });
        }

        public class TitleTimer : Timer
        {
            public TitleTimer(): base(TimeSpan.Zero, TimeSpan.FromHours(1))
            {
                Priority = TimerPriority.OneMinute;
            }

            protected override void OnTick()
            {
                foreach (NetState state in NetState.Instances)
                {
                    PlayerMobile player = state.Mobile as PlayerMobile;

                    if (player == null)
                        continue;

                    AuditTitles(player);
                }
            }
        }

        public static void OnLogin(PlayerMobile player)
        {
            CheckAndCreateTitleCollection(player);

            AuditTitles(player);
        }

        public static bool HasTitle(PlayerMobile player, string title)
        {
            if (player == null)
                return false;

            CheckAndCreateTitleCollection(player);

            foreach (TitleEntry entry in player.m_TitleCollection.m_Titles)
            {
                if (entry.m_TitleName == title)
                    return true;
            }

            return false;
        }

        public static TitleEntry GetTitle(PlayerMobile player, string title)
        {
            if (player == null)
                return null;

            CheckAndCreateTitleCollection(player);

            foreach (TitleEntry entry in player.m_TitleCollection.m_Titles)
            {
                if (entry.m_TitleName == title)
                    return entry;
            }

            return null;
        }

        public static bool AddTitle(PlayerMobile player, string title, bool expires, DateTime expiration, bool message)
        {
            if (player == null)
                return false;

            CheckAndCreateTitleCollection(player);

            if (HasTitle(player, title))
            {
                TitleEntry oldEntry = GetTitle(player, title);
                oldEntry.m_DateExpires = expiration;

                return true;
            }

            TitleEntry entry = new TitleEntry(title);

            entry.m_Expires = expires;
            entry.m_DateExpires = expiration;

            if (message)
            {
                if (expires)
                {
                    string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, expiration, false, true, true, false, false);

                    player.SendMessage("You have earned the title: " + title + " (expires in " + timeRemaining + ").");
                }

                else
                    player.SendMessage("You have earned the title: " + title + ".");
            }

            return true;            
        }

        public static bool RemoveTitle(PlayerMobile player, string title, bool message)
        {
            if (player == null)
                return false;

            CheckAndCreateTitleCollection(player);

            TitleEntry entryToRemove = null;

            foreach (TitleEntry entry in player.m_TitleCollection.m_Titles)
            {
                if (entry.m_TitleName == title)
                    entryToRemove = entry;
            }

            if (entryToRemove != null)
            {
                player.m_TitleCollection.m_Titles.Remove(entryToRemove);

                if (message)
                    player.SendMessage("You have lost the title: " + title + ".");

                return true;
            }

            return false;
        }

        public static bool DeactivateTitle(PlayerMobile player, string title)
        {
            if (player == null)
                return false;

            CheckAndCreateTitleCollection(player);

            if (HasTitle(player, title))
                return false;

            foreach (TitleEntry entry in player.m_TitleCollection.m_Titles)
            {
                if (entry.m_TitleName == title)
                {
                    entry.m_Activated = false;
                    return true;
                }
            }

            return true;
        }

        public static bool ActivateTitle(PlayerMobile player, string title)
        {
            if (player == null)
                return false;

            CheckAndCreateTitleCollection(player);

            if (HasTitle(player, title))
                return false;

            foreach (TitleEntry entry in player.m_TitleCollection.m_Titles)
            {
                if (entry.m_TitleName == title)                
                    entry.m_Activated = true;

                else
                    entry.m_Activated = false;        
            }

            return true;
        }        

        public static void AuditTitles(PlayerMobile player)
        {
            CheckAndCreateTitleCollection(player);

            Queue m_Queue = new Queue();

            foreach (TitleEntry entry in player.m_TitleCollection.m_Titles)
            {
                if (!ValidateTitle(player, entry.m_TitleName, true))
                {
                    m_Queue.Enqueue(entry);
                    continue;
                }

                if (!entry.m_Expires)
                    continue;

                if (DateTime.UtcNow >= entry.m_DateExpires)
                {
                    if (DeactivateTitle(player, entry.m_TitleName))
                        player.SendMessage("Your title of '" + entry.m_TitleName + "' has expired.");

                    continue;
                }

                else if (DateTime.UtcNow + TimeSpan.FromHours(1) >= entry.m_DateExpires)
                {
                    TimeSpan timeRemaining = entry.m_DateExpires - DateTime.UtcNow;

                    Timer.DelayCall(timeRemaining, delegate
                    {
                        if (player == null)
                            return;
                        
                        if (DeactivateTitle(player, entry.m_TitleName))
                            player.SendMessage("Your title of '" + entry.m_TitleName + "' has expired.");
                    });

                    continue;
                }
            }

            while (m_Queue.Count > 0)
            {
                TitleEntry entry = (TitleEntry)m_Queue.Dequeue();

                RemoveTitle(player, entry.m_TitleName, true);
            }
        }

        public static bool ValidateTitle(PlayerMobile player, string title, bool message)
        {
            if (player == null)
                return false;

            if (message)
            {
                //Reason for Losing Title
            }

            return true;
        }

        public static void CheckAndCreateTitleCollection(PlayerMobile player)
        {
            if (player.m_TitleCollection == null)
                CreateAchievementAccountEntry(player);

            if (player.m_TitleCollection.Deleted)
                CreateAchievementAccountEntry(player);
        }

        public static void CreateAchievementAccountEntry(PlayerMobile player)
        {
            player.m_TitleCollection = new TitleCollection(player);
        }

        public static void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); //Version

            //Version 0
        }

        public static void Deserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            //Version 0
            if (version >= 0)
            {
            }
        }
    }

    public class TitlePersistanceItem : Item
    {
        public override string DefaultName { get { return "TitlePersistance"; } }

        public TitlePersistanceItem(): base(0x0)
        {
            Movable = false;
        }

        public TitlePersistanceItem(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            TitlePersistance.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Movable = false;

            TitlePersistance.PersistanceItem = this;
            TitlePersistance.Deserialize(reader);
        }
    }

    public class TitleCollection : Item
    {
        public PlayerMobile m_Player;

        public bool m_NotificationForNonSelectedTitleExpirations = true;

        public List<int> m_UnlockedHues = new List<int>();
        public List<TitleEntry> m_Titles = new List<TitleEntry>();        

        [Constructable]
        public TitleCollection(PlayerMobile player): base(0x0)
        {
            Visible = false;
            Movable = false;

            m_Player = player;

            TitlePersistance.m_TitleCollections.Add(this);
        }

        public TitleCollection(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            //Version 0
            writer.Write(m_Player);
            writer.Write(m_NotificationForNonSelectedTitleExpirations);

            writer.Write(m_UnlockedHues.Count);
            for (int a = 0; a < m_UnlockedHues.Count; a++)
            {
                writer.Write(m_UnlockedHues[a]);
            }

            writer.Write(m_Titles.Count);
            for (int a = 0; a < m_Titles.Count; a++)
            {
                writer.Write(m_Titles[a].m_Activated);
                writer.Write(m_Titles[a].m_TitleName);
                writer.Write(m_Titles[a].m_CurrentHue);
                writer.Write(m_Titles[a].m_DateEarned);
                writer.Write(m_Titles[a].m_Expires);
                writer.Write(m_Titles[a].m_DateExpires);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Player = (PlayerMobile)reader.ReadMobile();
                m_NotificationForNonSelectedTitleExpirations = reader.ReadBool();

                int hueCount = reader.ReadInt();
                for (int a = 0; a < hueCount; a++)
                {
                    m_UnlockedHues.Add(reader.ReadInt());
                }

                int titleCount = reader.ReadInt();
                for (int a= 0; a < titleCount; a++)
                {
                    string titleName = reader.ReadString();
                    bool activated = reader.ReadBool();                    
                    int currentHue = reader.ReadInt();
                    DateTime dateEarned = reader.ReadDateTime();
                    bool expires = reader.ReadBool();
                    DateTime dateExpires = reader.ReadDateTime();

                    TitleEntry titleEntry = new TitleEntry(titleName);

                    titleEntry.m_Activated = activated;
                    titleEntry.m_CurrentHue = currentHue;
                    titleEntry.m_DateEarned = dateEarned;
                    titleEntry.m_Expires = expires;
                    titleEntry.m_DateExpires = dateExpires;

                    m_Titles.Add(titleEntry);
                }
            }

            //-----

            TitlePersistance.m_TitleCollections.Add(this);
        }
    }

    public class TitleEntry
    {
        public string m_TitleName = "";
        public bool m_Activated = false;       
        public int m_CurrentHue = 0;

        public DateTime m_DateEarned = DateTime.UtcNow;
        public bool m_Expires = false;
        public DateTime m_DateExpires = DateTime.UtcNow;

        public TitleEntry(string titleName)
        {
            m_TitleName = titleName;
        }
    }
}