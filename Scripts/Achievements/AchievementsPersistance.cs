﻿using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using Server.Accounting;

namespace Server.Items
{
    public static class AchievementsPersistance
    {
        public static AchievementPersistanceItem PersistanceItem;

        public static List<AchievementAccountEntry> m_AchievementEntries = new List<AchievementAccountEntry>();
        
        public static void Initialize()
        {
            Timer.DelayCall(TimeSpan.FromTicks(1), delegate
            {
                if (PersistanceItem == null)
                    PersistanceItem = new AchievementPersistanceItem();               
            });
        }

        public static void OnLogin(PlayerMobile player)
        {
            CheckAndCreateAchievementAccountEntry(player);

            if (player.m_AchievementAccountEntry.OnLoginShowUnclaimedRewards)
            {
                int unclaimedRewardCount = Achievements.GetUnclaimedAchievementRewards(player);

                if (unclaimedRewardCount > 0)
                    player.SendMessage(Achievements.AchievementTextHue, "You have " + unclaimedRewardCount.ToString() + " unclaimed achievement rewards.");
            }
        }
        
        public static void CheckAndCreateAchievementAccountEntry(PlayerMobile player)
        {
            if (player == null) return;
            if (player.Account == null) return;

            if (player.m_AchievementAccountEntry == null)
                CreateAchievementAccountEntry(player);

            if (player.m_AchievementAccountEntry.Deleted)
                CreateAchievementAccountEntry(player);            
        }

        public static void CreateAchievementAccountEntry(PlayerMobile player)
        {
            if (player == null)
                return;

            string accountName = player.Account.Username;

            AchievementAccountEntry achievementAccountEntry = null;

            bool foundEntry = false;

            foreach (AchievementAccountEntry entry in m_AchievementEntries)
            {
                if (entry.m_AccountUsername == accountName)
                {
                    player.m_AchievementAccountEntry = entry;
                    foundEntry = true;

                    return;
                }
            }

            if (!foundEntry)
            {
                AchievementAccountEntry newEntry = new AchievementAccountEntry(accountName);

                Account account = player.Account as Account;

                for (int a = 0; a < account.accountMobiles.Length; a++)
                {
                    Mobile mobile = account.accountMobiles[a] as Mobile;

                    if (mobile != null)
                    {
                        PlayerMobile pm_Mobile = mobile as PlayerMobile;

                        if (pm_Mobile != null)
                            pm_Mobile.m_AchievementAccountEntry = newEntry;
                    }
                }
            }
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

    public class AchievementPersistanceItem : Item
    {
        public override string DefaultName { get { return "AchievementPersistance"; } }

        public AchievementPersistanceItem(): base(0x0)
        {
            Movable = false;
        }

        public AchievementPersistanceItem(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            AchievementsPersistance.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Movable = false;

            AchievementsPersistance.PersistanceItem = this;
            AchievementsPersistance.Deserialize(reader);
        }
    }

    public class AchievementAccountEntry : Item
    {
        public string m_AccountUsername = "";

        public List<AchievementEntry> m_Achievements = new List<AchievementEntry>();

        public bool OnLoginShowUnclaimedRewards = true;
        public bool AnnounceAchievementsToGuildMembers = true;
        public bool AnnounceAchievementsToNonGuildMembers = true;
        public bool ShowGuildMemberAchievementAnnoucements = true;
        public bool ShowNonGuildMemberAchievementAnnoucements = true;
        public bool AudioEnabled = true;
        public bool PopupEnabled = true;
        
        [Constructable]
        public AchievementAccountEntry(string accountName): base(0x0)
        {
            Visible = false;
            Movable = false;

            m_AccountUsername = accountName;

            //-----

            AchievementsPersistance.m_AchievementEntries.Add(this);
        }

        public AchievementAccountEntry(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            //Version 0
            writer.Write(m_AccountUsername);

            writer.Write(OnLoginShowUnclaimedRewards);
            writer.Write(AnnounceAchievementsToGuildMembers);
            writer.Write(AnnounceAchievementsToNonGuildMembers);
            writer.Write(ShowGuildMemberAchievementAnnoucements);
            writer.Write(ShowNonGuildMemberAchievementAnnoucements);
            writer.Write(AudioEnabled);
            writer.Write(PopupEnabled);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();           

            //Version 0
            if (version >= 0)
            {
                m_AccountUsername = reader.ReadString();

                OnLoginShowUnclaimedRewards = reader.ReadBool();
                AnnounceAchievementsToGuildMembers = reader.ReadBool();
                AnnounceAchievementsToNonGuildMembers = reader.ReadBool();
                ShowGuildMemberAchievementAnnoucements = reader.ReadBool();
                ShowNonGuildMemberAchievementAnnoucements = reader.ReadBool();
                AudioEnabled = reader.ReadBool();
                PopupEnabled = reader.ReadBool();
            }

            //-----

            AchievementsPersistance.m_AchievementEntries.Add(this);
        }
    }
}
