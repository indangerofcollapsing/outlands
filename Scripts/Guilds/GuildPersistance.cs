﻿using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using Server.Accounting;
using Server.Multis;

namespace Server.Items
{
    public static class GuildPersistance
    {
        public static GuildPersistanceItem PersistanceItem;

        public static List<Guild> m_Guilds = new List<Guild>();
        
        public static void Initialize()
        {
            Timer.DelayCall(TimeSpan.FromTicks(1), delegate
            {
                if (PersistanceItem == null)
                    PersistanceItem = new GuildPersistanceItem();               
            });
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

    public class GuildPersistanceItem : Item
    {
        public override string DefaultName { get { return "GuildPersistance"; } }

        public GuildPersistanceItem(): base(0x0)
        {
            Movable = false;
        }

        public GuildPersistanceItem(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            GuildPersistance.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Movable = false;

            GuildPersistance.PersistanceItem = this;
            GuildPersistance.Deserialize(reader);
        }
    }

    public enum GuildMemberRank
    {        
        Initiate,
        Veteran,
        Officer,
        Guildmaster
    }

    public class GuildMemberEntry
    {
        public PlayerMobile m_Player;
        public GuildMemberRank m_Rank = GuildMemberRank.Initiate;
        public DateTime m_JoinDate = DateTime.UtcNow;

        public GuildMemberEntry(PlayerMobile player, GuildMemberRank rank, DateTime joinDate)
        {
            m_Player = player;
            m_Rank = m_Rank;
            m_JoinDate = joinDate;
        }
    }

    public class Guild : Item
    {
        public string m_Abbreviation = "";

        public int m_Icon = 2583;
        public int m_IconHue = 0;

        public string m_Website = "";

        public BaseHouse m_Guildhouse;

        public Faction m_Faction = null;

        public List<GuildMemberEntry> m_Members = new List<GuildMemberEntry>();  

        public List<Guild> m_Allies = new List<Guild>();
        public List<Guild> m_AlliesRequestsSent = new List<Guild>();
        public List<Guild> m_AlliesRequestsReceived = new List<Guild>();

        public List<Guild> m_Enemies = new List<Guild>();
        public List<Guild> m_EnemiesRequestsSent = new List<Guild>();
        public List<Guild> m_EnemiesRequestsReceived = new List<Guild>();

        public List<PlayerMobile> m_Candidates = new List<PlayerMobile>();    

        [Constructable]
        public Guild(string name): base(0x0)
        {
            Visible = false;
            Movable = false;

            Name = name;

            GuildPersistance.m_Guilds.Add(this);
        }

        public Guild(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            //Version 0
            writer.Write(m_Abbreviation);
            writer.Write(m_Icon);
            writer.Write(m_IconHue);
            writer.Write(m_Website);
            writer.Write(m_Guildhouse);
            writer.Write(m_Faction);

            writer.Write(m_Members.Count);
            for (int a = 0; a < m_Members.Count; a++)
            {
                writer.Write(m_Members[a].m_Player);
                writer.Write((int)m_Members[a].m_Rank);
                writer.Write(m_Members[a].m_JoinDate);
            }

            writer.Write(m_Allies.Count);
            for (int a = 0; a < m_Allies.Count; a++)
            {
                writer.Write(m_Allies[a]);
            }

            writer.Write(m_AlliesRequestsSent.Count);
            for (int a = 0; a < m_AlliesRequestsSent.Count; a++)
            {
                writer.Write(m_AlliesRequestsSent[a]);
            }

            writer.Write(m_AlliesRequestsReceived.Count);
            for (int a = 0; a < m_AlliesRequestsReceived.Count; a++)
            {
                writer.Write(m_AlliesRequestsReceived[a]);
            }

            writer.Write(m_Enemies.Count);
            for (int a = 0; a < m_Enemies.Count; a++)
            {
                writer.Write(m_Enemies[a]);
            }

            writer.Write(m_EnemiesRequestsSent.Count);
            for (int a = 0; a < m_EnemiesRequestsSent.Count; a++)
            {
                writer.Write(m_EnemiesRequestsSent[a]);
            }

            writer.Write(m_EnemiesRequestsReceived.Count);
            for (int a = 0; a < m_EnemiesRequestsReceived.Count; a++)
            {
                writer.Write(m_EnemiesRequestsReceived[a]);
            }

            writer.Write(m_Candidates.Count);
            for (int a = 0; a < m_Candidates.Count; a++)
            {
                writer.Write(m_Candidates[a]);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Abbreviation = reader.ReadString();
                m_Icon = reader.ReadInt();
                m_IconHue = reader.ReadInt();
                m_Website = reader.ReadString();
                m_Guildhouse = reader.ReadItem() as BaseHouse;
                m_Faction = reader.ReadItem() as Faction;

                int membersCount = reader.ReadInt();
                for (int a = 0; a < membersCount; a++)
                {
                    PlayerMobile player = reader.ReadMobile() as PlayerMobile;
                    GuildMemberRank rank = (GuildMemberRank)reader.ReadInt();
                    DateTime joinDate = reader.ReadDateTime();

                    GuildMemberEntry guildMemberEntry = new GuildMemberEntry(player, rank, joinDate);

                    m_Members.Add(guildMemberEntry);
                }

                int alliesCount = reader.ReadInt();
                for (int a = 0; a < alliesCount; a++)
                {
                    Guild entry = reader.ReadItem() as Guild;
                    m_Allies.Add(entry);
                }

                int alliesRequestsSentCount = reader.ReadInt();
                for (int a = 0; a < alliesRequestsSentCount; a++)
                {
                    Guild entry = reader.ReadItem() as Guild;
                    m_AlliesRequestsSent.Add(entry);
                }

                int alliesRequestsReceivedCount = reader.ReadInt();
                for (int a = 0; a < alliesRequestsReceivedCount; a++)
                {
                    Guild entry = reader.ReadItem() as Guild;
                    m_AlliesRequestsReceived.Add(entry);
                }

                int enemiesCount = reader.ReadInt();
                for (int a = 0; a < enemiesCount; a++)
                {
                    Guild entry = reader.ReadItem() as Guild;
                    m_Enemies.Add(entry);
                }

                int enemiesRequestsSentCount = reader.ReadInt();
                for (int a = 0; a < enemiesRequestsSentCount; a++)
                {
                    Guild entry = reader.ReadItem() as Guild;
                    m_EnemiesRequestsSent.Add(entry);
                }

                int enemiesRequestsReceivedCount = reader.ReadInt();
                for (int a = 0; a < enemiesRequestsReceivedCount; a++)
                {
                    Guild entry = reader.ReadItem() as Guild;
                    m_EnemiesRequestsReceived.Add(entry);
                }

                int candidatesCount = reader.ReadInt();
                for (int a = 0; a < candidatesCount; a++)
                {                    
                   m_Candidates.Add(reader.ReadMobile() as PlayerMobile);
                }
            }

            //-----

            GuildPersistance.m_Guilds.Add(this);
        }
    }
}
