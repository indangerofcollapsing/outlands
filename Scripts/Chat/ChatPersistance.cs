﻿using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Misc;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;

using Server.Accounting;
using System.Linq;
using Server.Custom;
using System.Text;
using Server.Engines.Chat;

namespace Server
{
    public static class ChatPersistance
    {
        public static ChatPersistanceItem PersistanceItem;

        public static TimeSpan ChatMessageCooldown = TimeSpan.FromSeconds(2);

        public static List<WorldChatAccountEntry> m_WorldChatAccountEntries = new List<WorldChatAccountEntry>();
        public static List<PlayerSquelchEntry> m_PlayersSquelched = new List<PlayerSquelchEntry>();

        public static int m_PlayersSquelchedAdminPage = 1;
        public static WorldChatAccountEntry.FilterMode m_AdminFilterMode = WorldChatAccountEntry.FilterMode.Name;

        public static int GlobalTextHue = 2550;
        public static int PrivateTextHue = 2599;

        private static bool m_Enabled = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public static bool Enabled
        {
            get { return m_Enabled; }
            set { m_Enabled = value; }
        }

        public static void Initialize()
        {
            CommandSystem.Register("WorldChatEnable", AccessLevel.Counselor, new CommandEventHandler(WorldChatEnable));             

            CommandSystem.Register("WorldChatSquelchOneDay", AccessLevel.Counselor, new CommandEventHandler(WorldChatSquelchOneDay));
            CommandSystem.Register("WorldChatLiftSquelch", AccessLevel.Counselor, new CommandEventHandler(WorldChatLiftSquelch));

            CommandSystem.Register("WorldChat", AccessLevel.Player, new CommandEventHandler(WorldChat));
            CommandSystem.Register("WorldChatAdmin", AccessLevel.Counselor, new CommandEventHandler(WorldChatAdmin));
            
            Timer.DelayCall(TimeSpan.FromTicks(1), delegate
            {
                if (PersistanceItem == null)
                    PersistanceItem = new ChatPersistanceItem();

                else if (PersistanceItem.Deleted)
                    PersistanceItem = new ChatPersistanceItem();
            });
        }

        [Usage("WorldChatEnable")]
        [Description("Enable or Disable Displaying of WorldChat Messages for All Players")]
        public static void WorldChatEnable(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            if (ChatPersistance.Enabled)
            {
                ChatPersistance.Enabled = false;

                player.SendMessage("World chat system is now offline.");
                return;
            }

            else
            {
                ChatPersistance.Enabled = true;

                player.SendMessage("World chat system is now online.");
                return;
            }
        }   

        [Usage("WorldChatSquelchOneDay")]
        [Description("Squelch player in WorldChat system for one day")]
        public static void WorldChatSquelchOneDay(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            WorldChatAdminSquelch(player, TimeSpan.FromHours(24));
        }

        [Usage("WorldChatSquelchLiftSquelch")]
        [Description("Lifts any squelch timer on player")]
        public static void WorldChatLiftSquelch(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            player.SendMessage("Target the player to unsquelch.");
            player.Target = new AdminLiftSquelchTarget();
        }        

        [Usage("WorldChat")]
        [Description("Opens the WorldChat gump")]
        public static void WorldChat(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            ChatPersistance.CheckAndCreateWorldChatAccountEntry(player);

            player.CloseGump(typeof(WorldChatGump));
            player.SendGump(new WorldChatGump(player));
        }

        [Usage("WorldChatAdmin")]
        [Description("Opens the WorldChat Admingump")]
        public static void WorldChatAdmin(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            ChatPersistance.CheckAndCreateWorldChatAccountEntry(player);

            player.CloseGump(typeof(WorldChatAdminGump));
            player.SendGump(new WorldChatAdminGump(player));
        }

        public static void OnLogin(PlayerMobile player)
        {
            if (player == null)
                return;

            Account account = player.Account as Account;

            if (account == null)
                return;

            player.m_WorldChatAccountEntry = ChatPersistance.FindWorldChatAccountEntry(player);

            if (player.m_WorldChatAccountEntry == null)
                player.m_WorldChatAccountEntry = new WorldChatAccountEntry(account.Username);

            if (player.m_WorldChatAccountEntry.Deleted)
                player.m_WorldChatAccountEntry = new WorldChatAccountEntry(account.Username);

            Timer.DelayCall(TimeSpan.FromSeconds(2), delegate
            {
                if (!ChatPersistance.Enabled)
                    return;

                if (player.m_WorldChatAccountEntry.Enabled)                
                    player.SendMessage(ChatPersistance.GlobalTextHue, "WorldChat is currently enabled. Type [WorldChat to access worldchat options.");                
            });
        }

        public static void AdminSortSquelchedPlayersByName()
        {
            List<PlayerSquelchEntry> m_TempEntries = new List<PlayerSquelchEntry>();

            foreach (PlayerSquelchEntry entry in ChatPersistance.m_PlayersSquelched)
            {
                m_TempEntries.Add(entry);
            }

            ChatPersistance.m_PlayersSquelched.Clear();

            for (int a = 0; a < m_TempEntries.Count; a++)
            {
                if (m_TempEntries[a].m_Player == null) continue;
                if (m_TempEntries[a].m_Player.RawName == null) continue;

                string tempPlayer = m_TempEntries[a].m_Player.RawName.ToLower();

                int index = -1;

                if (ChatPersistance.m_PlayersSquelched.Count > 0)
                    index = 0;

                for (int b = 0; b < ChatPersistance.m_PlayersSquelched.Count; b++)
                {
                    if (ChatPersistance.m_PlayersSquelched[b] == null) continue;
                    if (ChatPersistance.m_PlayersSquelched[b].m_Player == null) continue;

                    string squelchedPlayer = ChatPersistance.m_PlayersSquelched[b].m_Player.RawName.ToLower();

                    int compareValue = tempPlayer.CompareTo(squelchedPlayer);

                    if (compareValue > 0)
                        index = b + 1;
                }

                if (index == -1)
                    ChatPersistance.m_PlayersSquelched.Add(m_TempEntries[a]);

                else
                {
                    if (index >= ChatPersistance.m_PlayersSquelched.Count)
                        ChatPersistance.m_PlayersSquelched.Add(m_TempEntries[a]);
                    else
                        ChatPersistance.m_PlayersSquelched.Insert(index, m_TempEntries[a]);
                }
            }
        }

        public static void AdminSortSquelchedPlayersByAccount()
        {
            List<PlayerSquelchEntry> m_TempEntries = new List<PlayerSquelchEntry>();

            foreach (PlayerSquelchEntry entry in ChatPersistance.m_PlayersSquelched)
            {
                m_TempEntries.Add(entry);
            }

            ChatPersistance.m_PlayersSquelched.Clear();

            for (int a = 0; a < m_TempEntries.Count; a++)
            {
                if (m_TempEntries[a].m_Player == null) continue;
                if (m_TempEntries[a].m_Player.Account == null) continue;

                Account playerAccount = m_TempEntries[a].m_Player.Account as Account;

                if (playerAccount == null) 
                    continue;

                string tempPlayerAccount = playerAccount.Username;

                int index = -1;

                if (ChatPersistance.m_PlayersSquelched.Count > 0)
                    index = 0;

                for (int b = 0; b < ChatPersistance.m_PlayersSquelched.Count; b++)
                {
                    if (ChatPersistance.m_PlayersSquelched[b] == null) continue;
                    if (ChatPersistance.m_PlayersSquelched[b].m_Player == null) continue;
                    if (ChatPersistance.m_PlayersSquelched[b].m_Player.Account == null) continue;

                    Account squelchedPlayerAccount = ChatPersistance.m_PlayersSquelched[b].m_Player.Account as Account;

                    if (squelchedPlayerAccount == null)
                        continue;

                    string squelchedAccount = squelchedPlayerAccount.Username;

                    int compareValue = tempPlayerAccount.CompareTo(squelchedAccount);

                    if (compareValue > 0)
                        index = b + 1;
                }

                if (index == -1)
                    ChatPersistance.m_PlayersSquelched.Add(m_TempEntries[a]);

                else
                {
                    if (index >= ChatPersistance.m_PlayersSquelched.Count)
                        ChatPersistance.m_PlayersSquelched.Add(m_TempEntries[a]);
                    else
                        ChatPersistance.m_PlayersSquelched.Insert(index, m_TempEntries[a]);
                }
            }
        }

        public static void AdminSortSquelchedPlayersByDate()
        {
            List<PlayerSquelchEntry> m_TempEntries = new List<PlayerSquelchEntry>();

            foreach (PlayerSquelchEntry entry in ChatPersistance.m_PlayersSquelched)
            {
                m_TempEntries.Add(entry);
            }

            ChatPersistance.m_PlayersSquelched.Clear();

            for (int a = 0; a < m_TempEntries.Count; a++)
            {
                if (m_TempEntries[a].m_Player == null) continue;
                if (m_TempEntries[a].m_Player.RawName == null) continue;

                int index = -1;

                if (ChatPersistance.m_PlayersSquelched.Count > 0)
                    index = 0;

                for (int b = 0; b < ChatPersistance.m_PlayersSquelched.Count; b++)
                {
                    if (ChatPersistance.m_PlayersSquelched[b] == null) continue;
                    if (ChatPersistance.m_PlayersSquelched[b].m_Player == null) continue;

                    if (m_TempEntries[a].m_SquelchExpiration > ChatPersistance.m_PlayersSquelched[b].m_SquelchExpiration)
                        index = b + 1;
                }

                if (index == -1)
                    ChatPersistance.m_PlayersSquelched.Add(m_TempEntries[a]);

                else
                {
                    if (index >= ChatPersistance.m_PlayersSquelched.Count)
                        ChatPersistance.m_PlayersSquelched.Add(m_TempEntries[a]);
                    else
                        ChatPersistance.m_PlayersSquelched.Insert(index, m_TempEntries[a]);
                }
            }
        }
        
        public static void SortSquelchedPlayersByName(WorldChatAccountEntry accountEntry)
        {
            List<PlayerSquelchEntry> m_TempEntries = new List<PlayerSquelchEntry>();

            foreach (PlayerSquelchEntry entry in accountEntry.m_PlayersSquelched)
            {
                m_TempEntries.Add(entry);
            }

            accountEntry.m_PlayersSquelched.Clear();

            for (int a = 0; a < m_TempEntries.Count; a++)
            {
                if (m_TempEntries[a].m_Player == null) continue;
                if (m_TempEntries[a].m_Player.RawName == null) continue;

                string tempPlayer = m_TempEntries[a].m_Player.RawName.ToLower();
                
                int index = -1;

                if (accountEntry.m_PlayersSquelched.Count > 0)
                    index = 0;

                for (int b = 0; b < accountEntry.m_PlayersSquelched.Count; b++)
                {
                    if (accountEntry.m_PlayersSquelched[b] == null) continue;
                    if (accountEntry.m_PlayersSquelched[b].m_Player == null) continue;

                    string squelchedPlayer = accountEntry.m_PlayersSquelched[b].m_Player.RawName.ToLower();
                    
                    int compareValue = tempPlayer.CompareTo(squelchedPlayer);
                    
                    if (compareValue > 0)
                        index = b + 1;
                }
                
                if (index == -1)
                    accountEntry.m_PlayersSquelched.Add(m_TempEntries[a]);

                else
                {
                    if (index >= accountEntry.m_PlayersSquelched.Count)
                        accountEntry.m_PlayersSquelched.Add(m_TempEntries[a]);
                    else
                        accountEntry.m_PlayersSquelched.Insert(index, m_TempEntries[a]);
                }

            }
        }

        public static void SortSquelchedPlayersByDate(WorldChatAccountEntry accountEntry)
        {
            List<PlayerSquelchEntry> m_TempEntries = new List<PlayerSquelchEntry>();

            foreach (PlayerSquelchEntry entry in accountEntry.m_PlayersSquelched)
            {
                m_TempEntries.Add(entry);
            }

            accountEntry.m_PlayersSquelched.Clear();

            for (int a = 0; a < m_TempEntries.Count; a++)
            {
                if (m_TempEntries[a].m_Player == null) continue;
                if (m_TempEntries[a].m_Player.RawName == null) continue;

                int index = -1;

                if (accountEntry.m_PlayersSquelched.Count > 0)
                    index = 0;

                for (int b = 0; b < accountEntry.m_PlayersSquelched.Count; b++)
                {
                    if (accountEntry.m_PlayersSquelched[b] == null) continue;
                    if (accountEntry.m_PlayersSquelched[b].m_Player == null) continue;

                    if (m_TempEntries[a].m_SquelchExpiration > accountEntry.m_PlayersSquelched[b].m_SquelchExpiration)
                        index = b + 1;
                }

                if (index == -1)
                    accountEntry.m_PlayersSquelched.Add(m_TempEntries[a]);

                else
                {
                    if (index >= accountEntry.m_PlayersSquelched.Count)
                        accountEntry.m_PlayersSquelched.Add(m_TempEntries[a]);
                    else
                        accountEntry.m_PlayersSquelched.Insert(index, m_TempEntries[a]);
                }
            }
        }

        public static void WorldChatAdminSquelch(Mobile from, TimeSpan timeSpan)
        {
            from.SendMessage("Target the player to squelch.");
            from.Target = new AdminSquelchTarget(timeSpan);
        }

        public class AdminSquelchTarget : Target
        {
            private IEntity targetLocation;
            private TimeSpan m_TimeSpan;

            public AdminSquelchTarget(TimeSpan timeSpan): base(25, false, TargetFlags.None, false)
            {
                m_TimeSpan = timeSpan;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile pm_From = from as PlayerMobile;

                if (pm_From == null) 
                    return;

                PlayerMobile playerTarget = target as PlayerMobile;

                if (playerTarget == null)
                {
                    pm_From.SendMessage("That is not a player.");
                    return;
                }

                if (pm_From.AccessLevel <= playerTarget.AccessLevel)
                {
                    pm_From.SendMessage("You do not have enough access to Squelch that player.");
                    return;
                }

                ChatPersistance.CheckAndCreateWorldChatAccountEntry(playerTarget);

                Account playerAccount = playerTarget.Account as Account;

                if (playerAccount == null)
                    return;

                string accountName = "";

                PlayerSquelchEntry entryToRemove = null;

                foreach (PlayerSquelchEntry squelchEntry in ChatPersistance.m_PlayersSquelched)
                {
                    if (squelchEntry.m_Player == null) continue;
                    if (squelchEntry.m_Player.Account == null) continue;

                    Account squelchAccount = squelchEntry.m_Player.Account as Account;

                    if (squelchAccount == null) continue;

                    if (playerAccount == squelchAccount)
                    {
                        accountName = playerAccount.Username;
                        entryToRemove = squelchEntry;

                        break;
                    }
                }

                if (entryToRemove != null)
                {
                    if (entryToRemove.m_SquelchExpiration >= DateTime.UtcNow + TimeSpan.FromHours(24))
                        pm_From.SendMessage("That player's account (" + accountName + ") is already squelched for longer than 24 hours. Use [WorldChatAdmin to manage current Squelches.");

                    else
                    {
                        ChatPersistance.m_PlayersSquelched.Remove(entryToRemove);
                        ChatPersistance.m_PlayersSquelched.Add(new PlayerSquelchEntry(playerTarget, true, DateTime.UtcNow + TimeSpan.FromHours(24)));

                        pm_From.SendMessage("Squelch for " + playerTarget.Name + " set for 24 hours.");
                    }
                }

                else
                {
                    ChatPersistance.m_PlayersSquelched.Add(new PlayerSquelchEntry(playerTarget, true, DateTime.UtcNow + TimeSpan.FromHours(24)));

                    pm_From.SendMessage("Squelch for " + playerTarget.Name + " set for 24 hours.");
                }

                UpdateAdminGumps();

                pm_From.CloseGump(typeof(WorldChatAdminGump));
                pm_From.SendGump(new WorldChatAdminGump(pm_From));
            }
        }

        public class AdminLiftSquelchTarget : Target
        {
            private IEntity targetLocation;

            public AdminLiftSquelchTarget(): base(25, false, TargetFlags.None, false)
            {              
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (from == null)
                    return;

                PlayerMobile playerTarget = target as PlayerMobile;

                if (playerTarget == null)
                {
                    from.SendMessage("That is not a player.");
                    return;
                }

                ChatPersistance.CheckAndCreateWorldChatAccountEntry(playerTarget);

                Account playerAccount = playerTarget.Account as Account;

                if (playerAccount == null)
                    return;

                PlayerSquelchEntry entryToRemove = null;

                foreach (PlayerSquelchEntry squelchEntry in ChatPersistance.m_PlayersSquelched)
                {
                    if (squelchEntry.m_Player == null) continue;
                    if (squelchEntry.m_Player.Account == null) continue;

                    Account squelchAccount = squelchEntry.m_Player.Account as Account;

                    if (squelchAccount == null) continue;

                    if (playerAccount == squelchAccount)
                    {
                        entryToRemove = squelchEntry;
                        break;
                    }
                }

                if (entryToRemove != null)
                {
                    ChatPersistance.m_PlayersSquelched.Remove(entryToRemove);
                    from.SendMessage("Squelch for " + playerTarget.Name + " has been lifted.");
                }

                else                
                    from.SendMessage("No Squelch entry found for that player's account.");

                UpdateAdminGumps();
                
            }
        }

        public static void CheckAndCreateWorldChatAccountEntry(PlayerMobile player)
        {
            if (player == null) return;
            if (player.Deleted) return;

            if (player.m_WorldChatAccountEntry == null)
                CreateWorldChatAccountEntry(player);

            if (player.m_WorldChatAccountEntry.Deleted)
                CreateWorldChatAccountEntry(player);
        }

        public static void CreateWorldChatAccountEntry(PlayerMobile player)
        {            
            if (player == null) return;
            if (player.Account == null) return;

            string accountName = player.Account.Username;

            WorldChatAccountEntry WorldChatAccountEntry = null;

            foreach (WorldChatAccountEntry entry in m_WorldChatAccountEntries)
            {
                if (entry.m_AccountUsername == accountName && !entry.Deleted)
                {
                    player.m_WorldChatAccountEntry = entry;
                    return;
                }
            }

            WorldChatAccountEntry newEntry = new WorldChatAccountEntry(accountName);

            Account account = player.Account as Account;

            if (account == null) return;
            if (account.accountMobiles == null) return;

            for (int a = 0; a < account.accountMobiles.Length; a++)
            {
                Mobile mobile = account.accountMobiles[a] as Mobile;

                if (mobile != null)
                {
                    PlayerMobile pm_Mobile = mobile as PlayerMobile;

                    if (pm_Mobile != null)                    
                        pm_Mobile.m_WorldChatAccountEntry = newEntry;
                }
            }            
        }

        public static WorldChatAccountEntry FindWorldChatAccountEntryByAccountName(string accountName)
        {            
            foreach (WorldChatAccountEntry entry in m_WorldChatAccountEntries)
            {
                if (entry.m_AccountUsername == accountName && !entry.Deleted)
                {
                    return entry;
                }
            }

            return null;
        }

        public static WorldChatAccountEntry FindWorldChatAccountEntry(PlayerMobile player)
        {            
            if (player == null) return null;
            if (player.Account == null) return null;

            string accountName = player.Account.Username;

            WorldChatAccountEntry WorldChatAccountEntry = null;

            foreach (WorldChatAccountEntry entry in m_WorldChatAccountEntries)
            {
                if (entry.m_AccountUsername == accountName && !entry.Deleted)
                {
                    WorldChatAccountEntry = entry;
                    break;
                }
            }

            return WorldChatAccountEntry;
        }

        public static bool IsPlayerSquelched(PlayerMobile pm_Speaker, PlayerMobile pm_Listener)
        {
            bool squelched = false;

            if (pm_Speaker == null || pm_Listener == null) 
                return squelched;

            CheckAndCreateWorldChatAccountEntry(pm_Listener);           
            
            foreach (PlayerSquelchEntry entry in pm_Listener.m_WorldChatAccountEntry.m_PlayersSquelched)
            {
                if (entry.m_Player == null) continue;
                if (entry.m_Player == pm_Speaker)
                    return true;

                if (!entry.m_SquelchEntireAccount)
                    continue;
                
                Account entryAccount = entry.m_Player.Account as Account;

                if (entryAccount == null)
                    continue;

                Account speakerAccount = pm_Speaker.Account as Account;

                if (speakerAccount == null) continue;
                if (speakerAccount == entryAccount)
                    return true;
            }

            return squelched;
        }

        public static void ClearExpiredSquelchEntries()
        {
            bool removedSquelch = false;

            Queue m_Queue = new Queue();

            foreach (PlayerSquelchEntry squelchEntry in ChatPersistance.m_PlayersSquelched)
            {
                if (DateTime.UtcNow >= squelchEntry.m_SquelchExpiration)
                {
                    m_Queue.Enqueue(squelchEntry);

                    removedSquelch = true;
                }
            }           

            while (m_Queue.Count > 0)
            {
                PlayerSquelchEntry squelchEntry = (PlayerSquelchEntry)m_Queue.Dequeue();
                ChatPersistance.m_PlayersSquelched.Remove(squelchEntry);
            }

            if (removedSquelch)
                ChatPersistance.UpdateAdminGumps();
        }

        public static void UpdateAdminGumps()
        {
            ChatPersistance.ClearExpiredSquelchEntries();
            
            foreach (NetState state in NetState.Instances)
            {
                Mobile mobile = state.Mobile;
                PlayerMobile player = mobile as PlayerMobile;

                if (player == null)
                    continue;

                if (player.HasGump(typeof(WorldChatAdminGump)))
                {
                    player.CloseGump(typeof(WorldChatAdminGump));
                    player.SendGump(new WorldChatAdminGump(player));
                }               
            }
        }

        public static void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); //Version

            //Version 0   
            writer.Write(m_Enabled);

            writer.Write(m_PlayersSquelchedAdminPage);
            writer.Write((int)(m_AdminFilterMode));

            writer.Write(m_PlayersSquelched.Count);
            for (int a = 0; a < m_PlayersSquelched.Count; a++)
            {
                PlayerSquelchEntry squelchEntry = m_PlayersSquelched[a];

                writer.Write(squelchEntry.m_Player);
                writer.Write(squelchEntry.m_SquelchEntireAccount);
                writer.Write(squelchEntry.m_SquelchExpiration);
            }
        }

        public static void Deserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            //Version 0
            if (version >= 0)
            {
                m_Enabled = reader.ReadBool();               
            }

            m_PlayersSquelchedAdminPage = reader.ReadInt();
            m_AdminFilterMode = (WorldChatAccountEntry.FilterMode)reader.ReadInt();

            int squelchedPlayerCount = reader.ReadInt();
            for (int a = 0; a < squelchedPlayerCount; a++)
            {
                PlayerMobile player = (PlayerMobile)reader.ReadMobile();
                bool squelchEntireAccount = reader.ReadBool();
                DateTime dateSquelched = reader.ReadDateTime();

                PlayerSquelchEntry squelchEntry = new PlayerSquelchEntry(player, squelchEntireAccount, dateSquelched);

                m_PlayersSquelched.Add(squelchEntry);
            } 

            //-----            
        }
    }

    public class ChatPersistanceItem : Item
    {
        public override string DefaultName { get { return "ChatPersistance"; } }

        public ChatPersistanceItem(): base(0x0)
        {
            Movable = false;
        }

        public ChatPersistanceItem(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            ChatPersistance.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Movable = false;

            ChatPersistance.PersistanceItem = this;
            ChatPersistance.Deserialize(reader);
        }
    }

    public class WorldChatAccountEntry : Item
    {
        public enum FilterMode
        {
            Name,
            Date,
            Account
        }

        public string m_AccountUsername = "";

        private bool m_Enabled = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Enabled
        {
            get { return m_Enabled; }
            set { m_Enabled = value; }
        }
        
        private DateTime m_LastMessageSent = DateTime.UtcNow;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastMessageSent
        {
            get { return m_LastMessageSent; }
            set { m_LastMessageSent = value; }
        }

        public int m_PlayersSquelchedPage = 1;
        public FilterMode m_FilterMode = FilterMode.Name;       

        private int m_GlobalTextHue = -1;
        [CommandProperty(AccessLevel.GameMaster)]
        public int GlobalTextHue
        {
            get { return m_GlobalTextHue; }
            set { m_GlobalTextHue = value; }
        }

        private int m_PrivateTextHue = -1;
        [CommandProperty(AccessLevel.GameMaster)]
        public int PrivateTextHue
        {
            get { return m_PrivateTextHue; }
            set { m_PrivateTextHue = value; }
        }

        public List<PlayerSquelchEntry> m_PlayersSquelched = new List<PlayerSquelchEntry>();

        [Constructable]
        public WorldChatAccountEntry(string accountName): base(0x0)
        {
            Visible = false;
            Movable = false;

            m_AccountUsername = accountName;

            ChatPersistance.m_WorldChatAccountEntries.Add(this);
        }

        public WorldChatAccountEntry(Serial serial): base(serial)
        {
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); //Version

            //Version 0
            writer.Write(m_AccountUsername);
            writer.Write(m_Enabled);           
            writer.Write(m_LastMessageSent);
            writer.Write(m_PlayersSquelchedPage);
            writer.Write((int)m_FilterMode);
            writer.Write(m_GlobalTextHue);
            writer.Write(m_PrivateTextHue);

            writer.Write(m_PlayersSquelched.Count);
            for (int a = 0; a < m_PlayersSquelched.Count; a++)
            {
                PlayerSquelchEntry squelchEntry = m_PlayersSquelched[a];

                writer.Write(squelchEntry.m_Player);
                writer.Write(squelchEntry.m_SquelchEntireAccount);
                writer.Write(squelchEntry.m_SquelchExpiration);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_AccountUsername = reader.ReadString();
                m_Enabled = reader.ReadBool();
                m_LastMessageSent = reader.ReadDateTime();
                m_PlayersSquelchedPage = reader.ReadInt();
                m_FilterMode = (FilterMode)reader.ReadInt();
                m_GlobalTextHue = reader.ReadInt();
                m_PrivateTextHue = reader.ReadInt();
                
                int squelchedPlayerCount = reader.ReadInt();
                for (int a = 0; a < squelchedPlayerCount; a++)
                {
                    PlayerMobile player = (PlayerMobile)reader.ReadMobile();
                    bool squelchEntireAccount = reader.ReadBool();
                    DateTime dateSquelched = reader.ReadDateTime();

                    PlayerSquelchEntry squelchEntry = new PlayerSquelchEntry(player, squelchEntireAccount, dateSquelched);

                    m_PlayersSquelched.Add(squelchEntry);
                }                
            }

            //---------

            ChatPersistance.m_WorldChatAccountEntries.Add(this);
        }
    }

    public class PlayerSquelchEntry
    {
        public PlayerMobile m_Player;
        public bool m_SquelchEntireAccount = true;
        public DateTime m_SquelchExpiration = DateTime.UtcNow;

        public PlayerSquelchEntry(PlayerMobile player, bool squelchEntireAccount, DateTime dateSquelched)
        {
            m_Player = player;
            m_SquelchEntireAccount = squelchEntireAccount;
            m_SquelchExpiration = dateSquelched;
        }
    }
}
