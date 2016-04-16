﻿using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Accounting;
using Server.Custom;
using Server.Guilds;
using Server.Multis;

namespace Server
{
    public static class GuildDockPersistance
    {
        public static GuildDockPersistanceItem PersistanceItem;

        public static List<GuildDockGuildInfo> m_GuildDockGuildInfoEntries = new List<GuildDockGuildInfo>();
        
        public static void Initialize()
        {     
            Timer.DelayCall(TimeSpan.FromTicks(1), delegate
            {
                if (PersistanceItem == null)
                    PersistanceItem = new GuildDockPersistanceItem();

                else if (PersistanceItem.Deleted)
                    PersistanceItem = new GuildDockPersistanceItem();
            });
        }

        public static void CheckCreateGuildDockGuildInfo(Guild guild)
        {
            bool foundEntry = false;

            if (guild == null)
                return;

            foreach (GuildDockGuildInfo entry in m_GuildDockGuildInfoEntries)
            {
                if (entry == null) continue; 
               
                //Match
                if (guild == entry.m_Guild)
                    return;                
            }

            if (!foundEntry)
                m_GuildDockGuildInfoEntries.Add(new GuildDockGuildInfo(guild));                         
        }        
        
        public static GuildDockGuildInfo GetPlayerGuildDockGuildInfo(Mobile mobile)
        {
            if (mobile == null) return null;
            if (mobile.Guild == null) return null;

            GuildDockPersistance.CheckCreateGuildDockGuildInfo((Guild)mobile.Guild);

            foreach (GuildDockGuildInfo infoEntry in m_GuildDockGuildInfoEntries)
            {
                if (infoEntry == null) continue;

                //Match
                if (mobile.Guild == infoEntry.m_Guild)
                    return infoEntry;
            }

            return null;
        }

        public static GuildDockGuildInfo GetBoatGuildDockGuildInfo(BaseBoat boat)
        {
            if (boat == null)
                return null;

            if (boat.Guild == null)
                return null;

            foreach (GuildDockGuildInfo infoEntry in m_GuildDockGuildInfoEntries)
            {
                if (infoEntry == null) continue;

                //Match
                if (boat.Guild == infoEntry.m_Guild)
                    return infoEntry;
            }

            return null;
        }        

        public static List<GuildDockUpgrade> GetGuildDocksContractsForPlayer(PlayerMobile player)
        {
            List<GuildDockUpgrade> m_Upgrades = new List<GuildDockUpgrade>();

            if (player == null)
                return m_Upgrades;

            GuildDockGuildInfo guildDockGuildInfo = GuildDockPersistance.GetPlayerGuildDockGuildInfo(player);

            if (guildDockGuildInfo == null)
                return m_Upgrades;

            //Bounty Contracts
            if (GuildDockPersistance.PlayerHasGuildDockUpgrade(player, GuildDockUpgradeType.DailyBountyContractEasy))
                m_Upgrades.Add(guildDockGuildInfo.GetDockUpgrade(GuildDockUpgradeType.DailyBountyContractEasy));

            if (GuildDockPersistance.PlayerHasGuildDockUpgrade(player, GuildDockUpgradeType.DailyBountyContractChallenging))
                m_Upgrades.Add(guildDockGuildInfo.GetDockUpgrade(GuildDockUpgradeType.DailyBountyContractChallenging));

            if (GuildDockPersistance.PlayerHasGuildDockUpgrade(player, GuildDockUpgradeType.DailyBountyContractDangerous))
                m_Upgrades.Add(guildDockGuildInfo.GetDockUpgrade(GuildDockUpgradeType.DailyBountyContractDangerous));

            //Transport Contracts
            if (GuildDockPersistance.PlayerHasGuildDockUpgrade(player, GuildDockUpgradeType.DailyTransportContractEasy))
                m_Upgrades.Add(guildDockGuildInfo.GetDockUpgrade(GuildDockUpgradeType.DailyTransportContractEasy));

            if (GuildDockPersistance.PlayerHasGuildDockUpgrade(player, GuildDockUpgradeType.DailyTransportContractChallenging))
                m_Upgrades.Add(guildDockGuildInfo.GetDockUpgrade(GuildDockUpgradeType.DailyTransportContractChallenging));

            if (GuildDockPersistance.PlayerHasGuildDockUpgrade(player, GuildDockUpgradeType.DailyTransportContractDangerous))
                m_Upgrades.Add(guildDockGuildInfo.GetDockUpgrade(GuildDockUpgradeType.DailyTransportContractDangerous));

            return m_Upgrades;
        }

        public static bool PlayerHasGuildDockUpgrade(PlayerMobile player, GuildDockUpgradeType upgradeType)
        {
            if (player == null)
                return false;

            GuildDockGuildInfo infoEntry = GetPlayerGuildDockGuildInfo(player);

            if (infoEntry == null)
                return false;

            GuildDockUpgrade guildDockUpgrade = infoEntry.GetDockUpgrade(upgradeType);

            if (guildDockUpgrade.m_Purchased)
                return true;

            return false;
        }

        public static bool BoatHasGuildDockUpgrade(BaseBoat boat, GuildDockUpgradeType upgradeType)
        {
            if (boat == null)
                return false;

            GuildDockGuildInfo infoEntry = GetBoatGuildDockGuildInfo(boat);

            if (infoEntry == null)
                return false;

            GuildDockUpgrade guildDockUpgrade = infoEntry.GetDockUpgrade(upgradeType);

            if (guildDockUpgrade.m_Purchased)
                return true;

            return false;
        }

        public class GuildDockPersistanceItem : Item
        {
            public override string DefaultName { get { return "GuildDockPersistance"; } }

            public GuildDockPersistanceItem(): base(0x0)
            {
                Movable = false;
            }

            public GuildDockPersistanceItem(Serial serial): base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
                writer.Write((int)0); //Version

                //----

                GuildDockPersistance.Serialize(writer);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                int version = reader.ReadInt();

                Movable = false;

                //---

                GuildDockPersistance.PersistanceItem = this;
                GuildDockPersistance.Deserialize(reader);
            }
        }   
        
        public static void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); //Version

            //Version 0            
            writer.Write(m_GuildDockGuildInfoEntries.Count);
            for (int a = 0; a < m_GuildDockGuildInfoEntries.Count; a++)
            {
                writer.Write(m_GuildDockGuildInfoEntries[a]);
            }
        }

        public static void Deserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            //Version 0
            if (version >= 0)
            {
                //Entries
                int guildDockEntriesCount = reader.ReadInt();
                for (int a = 0; a < guildDockEntriesCount; a++)
                {
                    GuildDockGuildInfo entry = (GuildDockGuildInfo)reader.ReadItem();

                    m_GuildDockGuildInfoEntries.Add(entry);
                }
            }     
        }        
    }

    public class GuildDockGuildInfo : Item
    {
        public Guild m_Guild;
        public int m_Doubloons = 0;

        public List<GuildDockUpgrade> m_Upgrades = new List<GuildDockUpgrade>();

        [Constructable]
        public GuildDockGuildInfo(Guild guild): base(0x0)
        {
            Visible = false;
            Movable = false;

            m_Guild = guild;

            CheckCreateUpgradeEntries();

            GuildDockPersistance.m_GuildDockGuildInfoEntries.Add(this);
        }

        public void CheckCreateUpgradeEntries()
        {
            int upgradesAvailable = Enum.GetNames(typeof(GuildDockUpgradeType)).Length;

            Queue m_Queue = new Queue();

            for (int a = 0; a < upgradesAvailable; a++)
            {
                GuildDockUpgradeType upgradeType = (GuildDockUpgradeType)a;

                bool exists = false;

                foreach (GuildDockUpgrade upgrade in m_Upgrades)
                {
                    if (upgrade.m_UpgradeType == upgradeType)
                    {
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                    m_Queue.Enqueue(upgradeType);
            }

            while (m_Queue.Count > 0)
            {
                GuildDockUpgradeType upgradeType = (GuildDockUpgradeType)m_Queue.Dequeue();

                GuildDockUpgrade upgradeEntry = new GuildDockUpgrade(upgradeType);

                m_Upgrades.Add(upgradeEntry);
            }
        }

        public GuildDockUpgrade GetDockUpgrade(GuildDockUpgradeType guildDockUpgradeType)
        {
            GuildDockUpgrade guildDockUpgrade = null;

            foreach (GuildDockUpgrade upgrade in m_Upgrades)
            {
                if (upgrade.m_UpgradeType == guildDockUpgradeType)
                    return upgrade;
            }

            return guildDockUpgrade;
        }

        public void UnlockUpgrade(GuildDockUpgradeType guildDockUpgradeType)
        {
            //Add Physical Aspects to All Guild Docks in Guild
        }

        public GuildDockGuildInfo(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            //Version 0    
            writer.Write(m_Guild);
            writer.Write(m_Doubloons);

            writer.Write(m_Upgrades.Count);
            for (int a = 0; a < m_Upgrades.Count; a++)
            {
                GuildDockUpgrade guildDockUpgrade = m_Upgrades[a];

                writer.Write((int)guildDockUpgrade.m_UpgradeType);
                writer.Write(guildDockUpgrade.m_Purchased);
                writer.Write(guildDockUpgrade.m_LastUsage);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Guild = (Guild)reader.ReadGuild();
                m_Doubloons = reader.ReadInt();

                int upgradeEntries = reader.ReadInt();

                for (int a = 0; a < upgradeEntries; a++)
                {
                    GuildDockUpgradeType upgradeType = (GuildDockUpgradeType)reader.ReadInt();
                    bool purchased = reader.ReadBool();
                    DateTime lastUsage = reader.ReadDateTime();

                    GuildDockUpgrade guildDockUpgrade = new GuildDockUpgrade(upgradeType);

                    if (guildDockUpgrade != null)
                    {
                        guildDockUpgrade.m_Purchased = purchased;
                        guildDockUpgrade.m_LastUsage = lastUsage;

                        m_Upgrades.Add(guildDockUpgrade);
                    }
                }
            }

            //-----

            CheckCreateUpgradeEntries();

            GuildDockPersistance.m_GuildDockGuildInfoEntries.Add(this);
        }        
    }
}