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
using Server.Achievements;
using Server.Accounting;
using Server.Custom;

namespace Server
{
    public static class MHSPersistance
    {
        public static MHSPersistanceItem PersistanceItem;

        public static List<MHSPlayerEntry> m_PlayerEntries = new List<MHSPlayerEntry>();

        public static double RareChance = .002;
        public static int RareHue = 2500;        

        public static void Initialize()
        {        
            Timer.DelayCall(TimeSpan.FromTicks(1), delegate
            {
                if (PersistanceItem == null)
                    PersistanceItem = new MHSPersistanceItem();

                else if (PersistanceItem.Deleted)
                    PersistanceItem = new MHSPersistanceItem();

                Timer.DelayCall(TimeSpan.FromSeconds(1), delegate
                {                    
                });
            });
        }

        public static void CheckAndCreateMHSAccountEntry(PlayerMobile player)
        {
            if (player == null) return;
            if (player.Deleted) return;

            if (player.m_MHSPlayerEntry == null)
                CreateMHSAccountEntry(player);

            if (player.m_MHSPlayerEntry.Deleted)
                CreateMHSAccountEntry(player);
        }

        public static void CreateMHSAccountEntry(PlayerMobile player)
        {            
            if (player == null)
                return;
            
            foreach (MHSPlayerEntry playerEntry in m_PlayerEntries)
            {
                if (playerEntry == null) continue;
                if (playerEntry.Deleted) continue;

                if (playerEntry.m_PlayerMobile == player)
                {
                    player.m_MHSPlayerEntry = playerEntry;
                    return;
                }
            }

            player.m_MHSPlayerEntry = new MHSPlayerEntry(player);
        }

        public static void RefreshGumps(PlayerMobile player)
        {
            if (player.HasGump(typeof(MHSTableOfContentsGump)))
            {
                player.CloseGump(typeof(MHSTableOfContentsGump));
                player.SendGump(new MHSTableOfContentsGump(player));
            }

            if (player.HasGump(typeof(MHSCreatureEntryGump)))
            {
                player.CloseGump(typeof(MHSCreatureEntryGump));
                player.SendGump(new MHSCreatureEntryGump(player));
            }
        }

        public static int GetGlobalRanking(PlayerMobile player)
        {
            int globalRanking = 0;

            if (player == null)
                return globalRanking;

            CheckAndCreateMHSAccountEntry(player);

            int totalPointsEarned = player.m_MHSPlayerEntry.m_TotalPointsEarned;

            int totalEntries = MHSPersistance.m_PlayerEntries.Count;
            int currentPosition = totalEntries;

            for (int a = 0; a < totalEntries; a++)
            {
                MHSPlayerEntry entry = MHSPersistance.m_PlayerEntries[a];

                if (entry.m_PlayerMobile == player)
                    continue;

                if (totalPointsEarned > entry.m_TotalPointsEarned)
                    currentPosition--;
            }

            globalRanking = currentPosition;

            return globalRanking;
        }

        public static int GetGlobalEntryCount()
        {
            return MHSPersistance.m_PlayerEntries.Count;
        }

        public static void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); //Version            
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

    public class MHSPersistanceItem : Item
    {
        public override string DefaultName { get { return "MHSPersistance"; } }

        public MHSPersistanceItem(): base(0x0)
        {
            Movable = false;
        }

        public MHSPersistanceItem(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            MHSPersistance.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Movable = false;

            MHSPersistance.PersistanceItem = this;
            MHSPersistance.Deserialize(reader);
        }
    }

    public class MHSPlayerEntry : Item
    {
        public enum GrimoirePage
        {
            Main,
            Boss,
            Champion,
            LoHBoss,
            Rare,
            Paragon
        }

        public PlayerMobile m_PlayerMobile;

        public GrimoirePage m_GrimoirePage = GrimoirePage.Main;

        public int m_BossPageNumber = 1;
        public int m_ChampionPageNumber = 1;
        public int m_LoHBossPageNumber = 1;
        public int m_RarePageNumber = 1;
        public int m_ParagonPageNumber = 1;

        public int m_TotalPointsEarned = 0;
        public int m_AvailablePoints = 0;

        public List<MHSCreaturePlayerEntry> m_CreatureEntries = new List<MHSCreaturePlayerEntry>();       

        [Constructable]
        public MHSPlayerEntry(PlayerMobile player): base(0x0)
        {
            Visible = false;
            Movable = false;

            m_PlayerMobile = player;   
         
            MHSPersistance.m_PlayerEntries.Add(this);
        }

        public MHSPlayerEntry(Serial serial): base(serial)
        {
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            //Version 0
            writer.Write(m_PlayerMobile);

            writer.Write((int)m_GrimoirePage);
            writer.Write(m_BossPageNumber);
            writer.Write(m_ChampionPageNumber);
            writer.Write(m_LoHBossPageNumber);
            writer.Write(m_RarePageNumber);
            writer.Write(m_ParagonPageNumber);

            writer.Write(m_TotalPointsEarned);
            writer.Write(m_AvailablePoints);

            writer.Write(m_CreatureEntries.Count);
            for (int a = 0; a < m_CreatureEntries.Count; a++)
            {
                MHSCreaturePlayerEntry creatureEntry = m_CreatureEntries[a];

                writer.Write((string)creatureEntry.m_CreatureType.ToString());
                writer.Write((int)creatureEntry.m_GroupType);
                writer.Write(creatureEntry.m_TimesKilled);
                writer.Write(creatureEntry.m_LastKilled);

                writer.Write(creatureEntry.m_Tasks.Count);
                for (int b = 0; b < creatureEntry.m_Tasks.Count; b++)
                {
                    MHSCreatureTaskPlayerEntry taskEntry = creatureEntry.m_Tasks[b];

                    writer.Write((int)taskEntry.m_TaskType);
                    writer.Write(taskEntry.m_TimesCompleted);
                    writer.Write(taskEntry.m_LastTimeCompleted);
                }
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_PlayerMobile = (PlayerMobile)reader.ReadMobile();

                m_GrimoirePage = (GrimoirePage)reader.ReadInt();
                m_BossPageNumber = reader.ReadInt();
                m_ChampionPageNumber = reader.ReadInt();
                m_LoHBossPageNumber = reader.ReadInt();
                m_RarePageNumber = reader.ReadInt();
                m_ParagonPageNumber = reader.ReadInt();

                m_TotalPointsEarned = reader.ReadInt();
                m_AvailablePoints = reader.ReadInt();

                int creatureEntriesCount = reader.ReadInt();
                for (int a = 0; a < creatureEntriesCount; a++)
                {
                    Type creatureType = Type.GetType(reader.ReadString());
                    MHSGroupType groupType = (MHSGroupType)reader.ReadInt();
                    int timesKilled = reader.ReadInt();
                    DateTime lastKilled = reader.ReadDateTime();

                    MHSCreaturePlayerEntry creaturePlayerEntry = new MHSCreaturePlayerEntry(creatureType, groupType);
                    creaturePlayerEntry.m_TimesKilled = timesKilled;
                    creaturePlayerEntry.m_LastKilled = lastKilled;

                    int taskCount = reader.ReadInt();
                    for (int b = 0; b < taskCount; b++)
                    {
                        MHSTaskType taskType = (MHSTaskType)reader.ReadInt();
                        int timesCompleted = reader.ReadInt();
                        DateTime lastCompleted = reader.ReadDateTime();

                        MHSCreatureTaskPlayerEntry taskEntry = new MHSCreatureTaskPlayerEntry(taskType);
                        taskEntry.m_TimesCompleted = timesCompleted;
                        taskEntry.m_LastTimeCompleted = lastCompleted;

                        creaturePlayerEntry.m_Tasks.Add(taskEntry);
                    }

                    m_CreatureEntries.Add(creaturePlayerEntry);                    
                }
            }

            //---------

            MHSPersistance.m_PlayerEntries.Add(this);
        }
    }

    public class MHSCreaturePlayerEntry
    {
        public Type m_CreatureType = typeof(EmperorDragon);
        public MHSGroupType m_GroupType = MHSGroupType.Boss;

        public int m_TimesKilled = 0;
        public DateTime m_LastKilled = DateTime.MaxValue;
        public List<MHSCreatureTaskPlayerEntry> m_Tasks = new List<MHSCreatureTaskPlayerEntry>();

        public MHSCreaturePlayerEntry(Type type, MHSGroupType groupType)
        {
            m_CreatureType = type;
            m_GroupType = groupType;
        }
    }

    public class MHSCreatureTaskPlayerEntry
    {
        public MHSTaskType m_TaskType = MHSTaskType.Damage5PercentOfTotal;
        public int m_TimesCompleted = 0;
        public DateTime m_LastTimeCompleted = DateTime.MaxValue;

        public MHSCreatureTaskPlayerEntry(MHSTaskType taskType)
        {
            m_TaskType = taskType;
        }
    }
}
