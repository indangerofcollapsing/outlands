using System;
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

namespace Server
{    
    public static class PetBattlePersistance
    {
        public static bool Enabled = false;

        public static List<PetBattleCreatureCollection> m_PetBattleCreatureCollection = new List<PetBattleCreatureCollection>();       

        public static void Initialize()
        {
            Timer.DelayCall(TimeSpan.FromTicks(1), delegate
            {
                if (PersistanceItem == null)
                    PersistanceItem = new PetBattlePersistanceItem();
            });
        }

        public static PetBattlePersistanceItem PersistanceItem;

        public static PetBattleCreatureCollection GetPlayerPetBattleCreatureCollection(PlayerMobile player)
        {
            bool foundAccount = false;

            PetBattleCreatureCollection playerCollection = null;

            if (m_PetBattleCreatureCollection == null)
                return null;

            foreach (PetBattleCreatureCollection collection in m_PetBattleCreatureCollection)
            {
                if (collection.m_AccountUsername == player.Account.Username)
                {
                    foundAccount = true;
                    return collection;
                }
            }

            if (!foundAccount)
            {
                playerCollection = new PetBattleCreatureCollection();
                playerCollection.m_AccountUsername = player.Account.Username;

                PopulatePetBattleAccount(playerCollection);               

                m_PetBattleCreatureCollection.Add(playerCollection);
            }

            return playerCollection;            
        }

        public static void PopulatePetBattleAccount(PetBattleCreatureCollection petBattleCreatureCollection)
        {
            List<Type> m_CreatureTypes = new List<Type>();

            m_CreatureTypes.Add(typeof(PetBattleDesertOstard));
            m_CreatureTypes.Add(typeof(PetBattleGiantSpider));
            m_CreatureTypes.Add(typeof(PetBattleGorilla));
            m_CreatureTypes.Add(typeof(PetBattlePanther));

            petBattleCreatureCollection.m_CreatureEntries.Add(new PetBattleCreatureEntry(m_CreatureTypes[Utility.RandomMinMax(0, m_CreatureTypes.Count - 1)]));
        }

        public static bool AddPetBattleCreature(PlayerMobile player, Type creatureType)
        {
            bool foundAccount = false;
            bool foundCreature = false;

            if (player == null || creatureType == null)
                return true;

            if (m_PetBattleCreatureCollection == null)
                return true;

            PetBattleCreatureCollection playerCollection = null;

            foreach (PetBattleCreatureCollection collection in m_PetBattleCreatureCollection)
            {
                if (collection.m_AccountUsername == player.Account.Username)
                {
                    playerCollection = collection; 
                    foundAccount = true;

                    break;
                }
            }

            if (!foundAccount)
            {
                playerCollection = new PetBattleCreatureCollection();
                playerCollection.m_AccountUsername = player.Account.Username;

                PopulatePetBattleAccount(playerCollection);

                m_PetBattleCreatureCollection.Add(playerCollection);
            }

            foreach (PetBattleCreatureEntry entry in playerCollection.m_CreatureEntries)
            {
                if (entry.m_Type == creatureType)
                {
                    foundCreature = true;
                    return true;
                }
            }

            if (!foundCreature)
                playerCollection.m_CreatureEntries.Add(new PetBattleCreatureEntry(creatureType));

            return false;
        }

        public static bool UpdatePlayerCreatureEntry(PlayerMobile pm, PetBattleCreatureEntry entry)
        {
            if (m_PetBattleCreatureCollection == null || pm == null || entry == null)
                return false;
            
            foreach (PetBattleCreatureCollection collection in m_PetBattleCreatureCollection)
            {                
                if (collection.m_AccountUsername == pm.Account.Username)
                {                   
                    foreach (PetBattleCreatureEntry collectionCreatureEntry in collection.m_CreatureEntries)
                    {                       
                        if (collectionCreatureEntry.m_Type == entry.m_Type)
                        {
                            collectionCreatureEntry.m_Level = entry.m_Level;
                            collectionCreatureEntry.m_Experience = entry.m_Experience;
                            collectionCreatureEntry.m_OffensivePower = entry.m_OffensivePower;
                            collectionCreatureEntry.m_DefensivePower = entry.m_DefensivePower;

                            collectionCreatureEntry.m_Wins = entry.m_Wins;
                            collectionCreatureEntry.m_Ties = entry.m_Ties;
                            collectionCreatureEntry.m_Losses = entry.m_Losses;

                            break;
                        }
                    }
                }
            }

            return false;
        }

        public static void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); //Version

            writer.Write((int)m_PetBattleCreatureCollection.Count);

            foreach (PetBattleCreatureCollection collection in m_PetBattleCreatureCollection)
            {
                writer.Write(collection.m_AccountUsername);
                writer.Write((int)collection.m_CreatureEntries.Count);
                
                foreach (PetBattleCreatureEntry entry in collection.m_CreatureEntries)
                {
                    writer.Write(entry.m_Type.ToString());

                    writer.Write(entry.m_Level);
                    writer.Write(entry.m_OffensivePower);
                    writer.Write(entry.m_DefensivePower);

                    writer.Write(entry.m_Experience);
                    writer.Write(entry.m_Wins);
                    writer.Write(entry.m_Ties);
                    writer.Write(entry.m_Losses);
                }
            }
        }

        public static void Deserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();                       

            m_PetBattleCreatureCollection = new List<PetBattleCreatureCollection>();

            switch (version)
            {
                case 0:
                    {
                        int collectionCount = reader.ReadInt();
                        for (int a = 0; a < collectionCount; ++a)
                        {
                            string m_AccountUsername = reader.ReadString();                           

                            PetBattleCreatureCollection collection = new PetBattleCreatureCollection();
                            collection.m_AccountUsername = m_AccountUsername;

                            collection.m_CreatureEntries = new List<PetBattleCreatureEntry>();

                            int entriesCount = reader.ReadInt();
                            for (int b = 0; b < entriesCount; ++b)
                            {
                                string str = reader.ReadString();
                                Type type = Type.GetType(str); 
                                
                                PetBattleCreatureEntry creatureEntry = new PetBattleCreatureEntry(type);

                                creatureEntry.m_Level = reader.ReadInt();
                                creatureEntry.m_OffensivePower = reader.ReadInt();
                                creatureEntry.m_DefensivePower = reader.ReadInt();

                                creatureEntry.m_Experience = reader.ReadInt();
                                creatureEntry.m_Wins = reader.ReadInt();
                                creatureEntry.m_Ties = reader.ReadInt();
                                creatureEntry.m_Losses = reader.ReadInt();
                               
                                collection.m_CreatureEntries.Add(creatureEntry);
                            }

                            m_PetBattleCreatureCollection.Add(collection);
                        }
                    }
                break;
            }             
        }
    }

    public class PetBattleCreatureCollection
    {        
        public string m_AccountUsername = "";
        public List<PetBattleCreatureEntry> m_CreatureEntries = new List<PetBattleCreatureEntry>();

        [Constructable]
        public PetBattleCreatureCollection()
        {           
        }

        public PetBattleCreatureEntry GetCreatureInCollection(Type type)
        {
            foreach (PetBattleCreatureEntry creatureEntry in m_CreatureEntries)
            {
                if (creatureEntry.m_Type == type)
                {
                    return creatureEntry;
                    break;
                }
            }

            return null;
        }

        public bool FindCreatureInCollection(Type type)
        {
            bool foundCreature = false;

            foreach (PetBattleCreatureEntry creatureEntry in m_CreatureEntries)
            {
                if (creatureEntry.m_Type == type)
                {
                    foundCreature = true;
                    break;
                }
            }

            return foundCreature;
        }

        public void AddCreatureEntry(Type type)
        {
            if (!FindCreatureInCollection(type))
                m_CreatureEntries.Add(new PetBattleCreatureEntry(type));
        }
    }

    public class PetBattleCreatureEntry
    {
        public Type m_Type;

        public int m_Level = 1;
        public int m_OffensivePower = 1;
        public int m_DefensivePower = 1;

        public int m_Experience = 0;
        public int m_Wins = 0;
        public int m_Ties = 0;
        public int m_Losses = 0;

        public PetBattleCreatureEntry(Type type)
        {
            m_Type = type;
        }
    }

    public class PetBattlePersistanceItem : Item
    {
        public override string DefaultName { get { return "PetBattlePersistance"; } }

        public PetBattlePersistanceItem(): base(0x0)
        {
            Movable = false;
        }

        public PetBattlePersistanceItem(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version
            PetBattlePersistance.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            Movable = false;

            PetBattlePersistance.PersistanceItem = this;
            PetBattlePersistance.Deserialize(reader);
        }
    }
}
