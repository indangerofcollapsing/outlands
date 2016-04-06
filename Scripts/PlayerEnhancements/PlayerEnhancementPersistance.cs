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

namespace Server.Items
{
    public static class PlayerEnhancementPersistance
    {
        public static PlayerEnhancementPersistanceItem PersistanceItem;

        public static bool Enabled = false;

        public static List<PlayerEnhancementAccountEntry> m_PlayerEnhancementAccountEntries = new List<PlayerEnhancementAccountEntry>();
        
        public static void Initialize()
        {
            CommandSystem.Register("WipeAllPlayerEnhancements", AccessLevel.GameMaster, new CommandEventHandler(WipeAllPlayerEnhancements));            

            Timer.DelayCall(TimeSpan.FromTicks(1), delegate
            {
                if (PersistanceItem == null)
                    PersistanceItem = new PlayerEnhancementPersistanceItem();               
            });
        }

        [Usage("WipeAllPlayerEnhancements")]
        [Description("Completely Wipes All Player Enhancements from All Players")]
        public static void WipeAllPlayerEnhancements(CommandEventArgs arg)
        {
            //WARNING: THIS WILL WIPE ALL PURCHASED PLAYER ENHANCEMENTS FOR ALL PLAYERS

            PlayerMobile pm_Mobile = arg.Mobile as PlayerMobile;

            if (pm_Mobile == null)
                return;

            pm_Mobile.SendMessage("Wiping Player Enhancements....");

            Queue m_Queue = new Queue();

            int playerEnhancementEntries = 0;

            foreach (Item item in World.Items.Values)
            {
                if (item is PlayerEnhancementAccountEntry)
                {
                    playerEnhancementEntries++;
                    m_Queue.Enqueue(item);
                }
            }

            while (m_Queue.Count > 0)
            {
                Item item = (Item)m_Queue.Dequeue();
                item.Delete();
            }

            Console.Write("Deleted " + playerEnhancementEntries.ToString() + " player enhancement entries.");

            foreach (Account account in Accounts.GetAccounts())
            {
                for (int i = 0; i < account.Length; i++)
				{
                    Mobile mobile = account[i];

					if (mobile == null)
                        continue;

                    PlayerMobile player = mobile as PlayerMobile;

                    if (player == null)
                        continue;

                    player.m_PlayerEnhancementAccountEntry = null;
                }  
            }

            PlayerEnhancementPersistance.m_PlayerEnhancementAccountEntries = new List<PlayerEnhancementAccountEntry>();

            pm_Mobile.SendMessage("Player Enhancements Wiped.");
        }

        public static void CheckAndCreatePlayerEnhancementAccountEntry(PlayerMobile player)
        {
            if (player == null) return;
            if (player.Account == null) return;

            string accountName = player.Account.Username;

            PlayerEnhancementAccountEntry playerEnhancementAccountEntry = null;

            bool foundPlayerEnhancementEntry = false;

            foreach (PlayerEnhancementAccountEntry entry in m_PlayerEnhancementAccountEntries)
            {
                if (entry.m_AccountUsername == accountName)
                {
                    player.m_PlayerEnhancementAccountEntry = entry;
                    foundPlayerEnhancementEntry = true;

                    return;
                }
            }

            if (!foundPlayerEnhancementEntry)
            {
                PlayerEnhancementAccountEntry newEntry = new PlayerEnhancementAccountEntry(accountName);
                m_PlayerEnhancementAccountEntries.Add(newEntry);

                Account account = player.Account as Account;

                for (int i = 0; i < (account.Length - 1); i++)
                {
                    Mobile mobile = account.accountMobiles[i] as Mobile;

                    if (mobile != null)
                    {
                        PlayerMobile pm_Mobile = mobile as PlayerMobile;

                        if (pm_Mobile != null)
                            pm_Mobile.m_PlayerEnhancementAccountEntry = newEntry;
                    }
                }
            }
        }
        
        public static PlayerEnhancementAccountEntry FindPlayerEnhancementAccountEntry(PlayerMobile player)
        {
            if (player == null) return null;
            if (player.Account == null) return null;

            string accountName = player.Account.Username;

            PlayerEnhancementAccountEntry playerEnhancementAccountEntry = null;

            foreach (PlayerEnhancementAccountEntry entry in m_PlayerEnhancementAccountEntries)
            {
                if (entry.m_AccountUsername == accountName)
                {
                    playerEnhancementAccountEntry = entry;
                    break;
                }
            }

            return playerEnhancementAccountEntry;
        }

        public static SpellHueEntry GetSpellHueEntry(PlayerMobile player, HueableSpell hueableSpell)
        {
            SpellHueEntry spellHueEntry = null;

            if (player == null)
                return spellHueEntry;

            if (player.m_PlayerEnhancementAccountEntry == null)
                return spellHueEntry;

            foreach (SpellHueEntry entry in player.m_PlayerEnhancementAccountEntry.m_SpellHueEntries)
            {
                if (entry.m_HueableSpell == hueableSpell)
                {
                    spellHueEntry = entry;
                    break;
                }
            }

            return spellHueEntry;
        }

        public static bool IsCustomizationEntryActive(Mobile mobile, CustomizationType customization)
        {
            if (mobile == null) return false;
            if (!(mobile is PlayerMobile)) return false;

            PlayerMobile player = mobile as PlayerMobile;
            PlayerCustomizationEntry customizationEntry = null;

            if (player == null)
                return false;

            if (player.m_PlayerEnhancementAccountEntry == null)
                return false;

            foreach (PlayerCustomizationEntry entry in player.m_PlayerEnhancementAccountEntry.m_CustomizationEntries)
            {
                if (entry.m_CustomizationType == customization)
                {
                    customizationEntry = entry;
                    break;
                }
            }

            if (customizationEntry != null)
            {
                if (customizationEntry.m_Active)
                    return true;
            }

            return false;
        }

        public static PlayerCustomizationEntry GetCustomizationEntry(PlayerMobile player, CustomizationType customization)
        {
            PlayerCustomizationEntry customizationEntry = null;

            if (player == null)
                return customizationEntry;

            if (player.m_PlayerEnhancementAccountEntry == null)
                return customizationEntry;

            foreach (PlayerCustomizationEntry entry in player.m_PlayerEnhancementAccountEntry.m_CustomizationEntries)
            {
                if (entry.m_CustomizationType == customization)
                {
                    customizationEntry = entry;
                    break;
                }
            }

            return customizationEntry;
        }  

        public static int GetSpellHueFor(Mobile mobile, HueableSpell hueableSpell)
        {
            int hue = 0;

            BaseCreature bc_Creature = mobile as BaseCreature;

            if (bc_Creature != null)            
                return bc_Creature.SpellHue;            

            PlayerMobile player = mobile as PlayerMobile;

            if (player == null)
                return hue;

            SpellHueEntry entry = GetSpellHueEntry(player, hueableSpell);

            if (entry == null)
                return hue;

            return SpellHue.GetSpellHue(entry.m_ActiveHue);
        }

        public static void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); //Version

            //Version 0   
            writer.Write(Enabled);

            writer.Write((int)m_PlayerEnhancementAccountEntries.Count);
            foreach (PlayerEnhancementAccountEntry entry in m_PlayerEnhancementAccountEntries)
            {
                writer.Write(entry);
            }
        }

        public static void Deserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            m_PlayerEnhancementAccountEntries = new List<PlayerEnhancementAccountEntry>();            

            if (version >= 0)
            {
                Enabled = reader.ReadBool();

                int playerEnhancementAccountEntries = reader.ReadInt();
                for (int a = 0; a < playerEnhancementAccountEntries; ++a)
                {
                    PlayerEnhancementAccountEntry entry = reader.ReadItem() as PlayerEnhancementAccountEntry;

                    if (entry != null)
                        m_PlayerEnhancementAccountEntries.Add(entry);
                }
            }
        }
    }  

    public class PlayerEnhancementPersistanceItem : Item
    {
        public override string DefaultName { get { return "PlayerEnhancementPersistance"; } }

        public PlayerEnhancementPersistanceItem(): base(0x0)
        {
            Movable = false;
        }

        public PlayerEnhancementPersistanceItem(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            PlayerEnhancementPersistance.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Movable = false;

            PlayerEnhancementPersistance.PersistanceItem = this;
            PlayerEnhancementPersistance.Deserialize(reader);
        }
    }

    public class PlayerEnhancementAccountEntry : Item
    {
        public string m_AccountUsername = "";

        public List<PlayerCustomizationEntry> m_CustomizationEntries = new List<PlayerCustomizationEntry>();
        public List<SpellHueEntry> m_SpellHueEntries = new List<SpellHueEntry>();

        [Constructable]
        public PlayerEnhancementAccountEntry(string accountName): base(0x0)
        {
            Visible = false;
            Movable = false;

            m_AccountUsername = accountName;

            AddCustomizationEntries();
            AddSpellHueEntries();
        }

        public void AddCustomizationEntries()
        {
             int customizationCount = Enum.GetNames(typeof(CustomizationType)).Length;

             for (int a = 0; a < customizationCount; a++)
             {
                 CustomizationType customization = (CustomizationType)a;
                 m_CustomizationEntries.Add(new PlayerCustomizationEntry(customization, false, false, true));
             }
        }

        public void AddSpellHueEntries()
        {
            int spellHueCount = Enum.GetNames(typeof(HueableSpell)).Length;

             for (int a = 0; a < spellHueCount; a++)
             {
                HueableSpell hueableSpell = (HueableSpell)a;
                SpellHueEntry newEntry = new SpellHueEntry(hueableSpell, SpellHueType.Basic);

                m_SpellHueEntries.Add(newEntry);
             }
        }

        public void AuditCustomizationEntries()
        {
            List<CustomizationType> m_CustomizationsToAdd = new List<CustomizationType>();

            int customizationCount = Enum.GetNames(typeof(CustomizationType)).Length;

            for (int a = 0; a < customizationCount; a++)
            {
                CustomizationType customization = (CustomizationType)a;
                
                bool foundCustomizationEntry = false;

                foreach (PlayerCustomizationEntry entry in m_CustomizationEntries)
                {
                    if (entry.m_CustomizationType == customization)
                    {
                        foundCustomizationEntry = true;
                        break;
                    }            
                }

                if (!foundCustomizationEntry)
                    m_CustomizationsToAdd.Add(customization);
            }

            foreach (CustomizationType customization in m_CustomizationsToAdd)
            {
                m_CustomizationEntries.Add(new PlayerCustomizationEntry(customization, false, false, true));
            }
        }

        public void AuditSpellHueEntries()
        {
            List<HueableSpell> m_SpellHuesToAdd = new List<HueableSpell>();

            int spellHuesCount = Enum.GetNames(typeof(HueableSpell)).Length;

            for (int a = 0; a < spellHuesCount; a++)
            {
                HueableSpell hueableSpell = (HueableSpell)a;

                bool foundSpellHueEntry = false;

                foreach (SpellHueEntry entry in m_SpellHueEntries)
                {
                    if (entry.m_HueableSpell == hueableSpell)
                    {
                        foundSpellHueEntry = true;
                        break;
                    }
                }

                if (!foundSpellHueEntry)
                    m_SpellHuesToAdd.Add(hueableSpell);
            }

            foreach (HueableSpell hueableSpell in m_SpellHuesToAdd)
            {
                m_SpellHueEntries.Add(new SpellHueEntry(hueableSpell, SpellHueType.Basic));
            }
        }              

        public PlayerEnhancementAccountEntry(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            //Version 0
            writer.Write(m_AccountUsername);
            
            writer.Write(m_CustomizationEntries.Count);
            foreach (PlayerCustomizationEntry customizationEntry in m_CustomizationEntries)
            {
                writer.Write((int)customizationEntry.m_CustomizationType);
                writer.Write(customizationEntry.m_Unlocked);
                writer.Write(customizationEntry.m_Active);             
            }            
            
            writer.Write(m_SpellHueEntries.Count);
            foreach (SpellHueEntry spellHueEntry in m_SpellHueEntries)
            {
                writer.Write((int)spellHueEntry.m_HueableSpell);
                writer.Write((int)spellHueEntry.m_ActiveHue);                
                
                writer.Write(spellHueEntry.m_UnlockedHues.Count);
                foreach (SpellHueType spellHueType in spellHueEntry.m_UnlockedHues)
                {
                    writer.Write((int)spellHueType);
                }                 
            }            
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_CustomizationEntries = new List<PlayerCustomizationEntry>();
            m_SpellHueEntries = new List<SpellHueEntry>();

            if (version >= 0)
            {
                m_AccountUsername = reader.ReadString();
                
                int customizationEntries = reader.ReadInt();                
                for (int a = 0; a < customizationEntries; a++)
                {
                    CustomizationType customizationType = (CustomizationType)reader.ReadInt();

                    bool unlocked = reader.ReadBool();
                    bool active = reader.ReadBool();

                    m_CustomizationEntries.Add(new PlayerCustomizationEntry(customizationType, unlocked, active, true));
                }
                
                int spellHueEntries = reader.ReadInt();
                for (int a = 0; a < spellHueEntries; a++)
                {
                    HueableSpell spellHueType = (HueableSpell)reader.ReadInt();
                    SpellHueType activeType = (SpellHueType)reader.ReadInt();                    
                    
                    SpellHueEntry spellHueEntry = new SpellHueEntry(spellHueType, activeType);

                    int unlockedHueCount = reader.ReadInt();
                    for (int b = 0; b < unlockedHueCount; b++)
                    {
                        SpellHueType unlockedHue = (SpellHueType)reader.ReadInt();
                        spellHueEntry.m_UnlockedHues.Add(unlockedHue);
                    }

                    m_SpellHueEntries.Add(spellHueEntry);                    
                }                
            }
        }
    }
}
