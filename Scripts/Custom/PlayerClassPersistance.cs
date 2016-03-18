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
using System.Linq;

namespace Server.Items
{
    public static class PlayerClassPersistance
    {
        public static List<PlayerClassRecord> m_LifetimePaladinScores = new List<PlayerClassRecord>();
        public static List<PlayerClassRecord> m_PreviousPaladinScores = new List<PlayerClassRecord>();
        public static List<PlayerClassRecord> m_CurrentPaladinScores = new List<PlayerClassRecord>();

        public static List<PlayerClassRecord> m_LifetimeMurdererScores = new List<PlayerClassRecord>();
        public static List<PlayerClassRecord> m_PreviousMurdererScores = new List<PlayerClassRecord>();
        public static List<PlayerClassRecord> m_CurrentMurdererScores = new List<PlayerClassRecord>();

        public static List<PlayerClassRecord> m_LifetimePirateScores = new List<PlayerClassRecord>();
        public static List<PlayerClassRecord> m_PreviousPirateScores = new List<PlayerClassRecord>();
        public static List<PlayerClassRecord> m_CurrentPirateScores = new List<PlayerClassRecord>();

        //No Longer Used
        public static List<PlayerClassScores> m_PlayerScores = new List<PlayerClassScores>();
        public static DateTime lastScoreCalculation = DateTime.UtcNow;        

        //No Class
        public static string NoClassName = "";

        public static int NoClassItemHue = 0;

        public static Type NoClassCurrency = typeof(Gold);
        public static int NoClassCurrencyItemId = 3823;
        public static int NoClassCurrencyItemHue = 0;

        public static int NoClassGumpBackgroundId = 11014;

        //Paladin
        public static string PaladinClassName = "Paladin";

        public static int PaladinItemHue = 1154;

        public static Type PaladinCurrency = typeof(Platinum);
        public static int PaladinCurrencyItemId = 3823;
        public static int PaladinCurrencyItemHue = 996;

        public static int PaladinGumpBackgroundId = 11009;

        //Murderer
        public static string MurdererClassName = "Murderer";

        public static int MurdererItemHue = 1779; 

        public static Type MurdererCurrency = typeof(DreadCoin);
        public static int MurdererCurrencyItemId = 3823;
        public static int MurdererCurrencyItemHue = 2117;

        public static int MurdererGumpBackgroundId = 11015;

        //Pirate
        public static string PirateClassName = "Pirate";

        public static int PirateItemHue = 2019; 

        public static Type PirateCurrency = typeof(Doubloon);
        public static int PirateCurrencyItemId = 2539;
        public static int PirateCurrencyItemHue = 2125;

        public static int PirateGumpBackgroundId = 11008;

        public static string[] PaladinTitles = new string[] { "Paladin", "Defender", "Knight", "Crusader", "Justicar" };
        public static string[] MurdererTitles = new string[] { "Murderer", "Villain", "Scourge", "Assassin", "Dread Lord" };
        public static string[] PirateTitles = new string[] { "Pirate", "Raider", "Corsair", "Marauder", "Dread Pirate" };
        
        public static double PlayerClassCurrencyBonusPerItem = .03;

        public static int GetPlayerClassArmorItemCount(Mobile from, PlayerClass playerClass)
        {
            int items = 0;

            Item oneHand = from.FindItemOnLayer(Layer.OneHanded);
            Item twoHand = from.FindItemOnLayer(Layer.TwoHanded);

            bool handEquipedItem = false;

            if (oneHand != null)
            {
                if (oneHand.PlayerClass == playerClass)
                    handEquipedItem = true;
            }

            if (twoHand != null)
            {
                if (twoHand.PlayerClass == playerClass)
                    handEquipedItem = true;
            }

            //Equipped Item (Weapon / Shield / Spellbook) Counts For Two Items
            if (handEquipedItem)
                items += 2;

            BaseArmor helm = from.FindItemOnLayer(Layer.Helm) as BaseArmor;
            BaseArmor gorget = from.FindItemOnLayer(Layer.Neck) as BaseArmor;
            BaseArmor chest = from.FindItemOnLayer(Layer.Shirt) as BaseArmor;
            BaseArmor arms = from.FindItemOnLayer(Layer.Arms) as BaseArmor;
            BaseArmor gloves = from.FindItemOnLayer(Layer.Gloves) as BaseArmor;
            BaseArmor legs = from.FindItemOnLayer(Layer.Pants) as BaseArmor;

            if (helm != null) { if (helm.PlayerClass == playerClass) items++; }
            if (gorget != null) { if (gorget.PlayerClass == playerClass) items++; }
            if (chest != null) { if (chest.PlayerClass == playerClass) items++; }
            if (arms != null) { if (arms.PlayerClass == playerClass) items++; }
            if (gloves != null) { if (gloves.PlayerClass == playerClass) items++; }
            if (legs != null) { if (legs.PlayerClass == playerClass) items++; }

            return items;
        }
        
        public static void Initialize()
        {
            CommandSystem.Register("ResetPlayerClassScoreCycle", AccessLevel.GameMaster, new CommandEventHandler(ResetPlayerClassScoreCycle));

            Timer.DelayCall(TimeSpan.FromTicks(1), delegate
            {
                if (PersistanceItem == null)                
                    PersistanceItem = new PlayerClassPersistanceItem();                
            });            
        }

        public static PlayerClassPersistanceItem PersistanceItem;

        [Usage("ResetPlayerClassScoreCycle")]
        [Description("Manually Resets Current Cycle Scores")]
        public static void ResetPlayerClassScoreCycle(CommandEventArgs arg)
        {
            CycleComplete();

            arg.Mobile.SendMessage("Manually Reset Player Class Score Cycle.");
        }
        
        public static void AddTitle(PlayerMobile player, string[] titles, String newTitle)
        {
            if (player.TitlesPrefix.Contains(newTitle))
                return;

            foreach (String title in titles)
            {
                if (player.CurrentPrefix == title)
                    player.CurrentPrefix = "";

                player.TitlesPrefix.Remove(title);
            }

            player.TitlesPrefix.Add(newTitle);
            player.SendMessage("You have earned the title of *" + newTitle + "* and your title options have been updated.");
        }

        public static void RemoveTitles(PlayerMobile player, string[] titles)
        {
            foreach (String title in titles)
            {
                if (player.CurrentPrefix == title)
                    player.CurrentPrefix = "";
            }
        }

        public static void RemovePlayerClassEquipment(PlayerMobile player, PlayerClass playerClass)
        {
            bool movedPaladinItemsToBackpack = false;

            foreach (Layer layer in m_EquipmentLayers)
            {
                Item item = player.FindItemOnLayer(layer);

                if (item != null)
                {
                    if (item.PlayerClass == playerClass)
                    {
                        //Weapon Loses Hue If It Had One
                        if (item is BaseWeapon)
                        {
                            item.PlayerClass = PlayerClass.None;
                            item.PlayerClassOwner = null;
                            item.Hue = 0;
                        }

                        //Item Drops to Backpack
                        else
                        {
                            player.PlaceInBackpack(item);
                            movedPaladinItemsToBackpack = true;
                        }
                    }
                }
            }

            if (movedPaladinItemsToBackpack)
                player.SendMessage("You find that your items no longer adhere to you and drop to your backpack.");
        }

        public static bool IsPlayerClass(Mobile from, PlayerClass playerClass)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return false;

            switch (playerClass)
            {
                case PlayerClass.None: return true; break;
                case PlayerClass.Paladin: return player.Paladin; break;
                case PlayerClass.Murderer: return player.Murderer; break;
                case PlayerClass.Pirate: return player.Pirate; break;
            }

            return false;
        }

        public static bool PlayerClassCanUse(Mobile from, Item item)
        {
            return PlayerClassCanEquip(from, item);
        }

        public static bool PlayerClassCanEquip(Mobile from, Item item)
        {
            if (from == null || item == null)
                return false;

            PlayerMobile player = from as PlayerMobile;
            BaseCreature bc_Creature = from as BaseCreature;

            //Allow NPCs to Wear PlayerClass Items
            if (player == null || bc_Creature != null)
                return true;

            //Fix for PlayerClass Item Instances Where They Have Lost Their PlayerClassOwner But Not Their Hue 
            if (player != null && (item is BaseWeapon || item is Spellbook))
            {
                if (item.Hue == PlayerClassPersistance.PaladinItemHue || item.Hue == PlayerClassPersistance.MurdererItemHue || item.Hue == PlayerClassPersistance.PirateItemHue)
                {
                    if (item.PlayerClassOwner == null || item.PlayerClass == PlayerClass.None)
                    {
                        item.PlayerClass = PlayerClass.None;
                        item.PlayerClassOwner = null;
                        item.PlayerClassRestricted = false;

                        item.Hue = 0;

                        from.LocalOverheadMessage(Server.Network.MessageType.Regular, 0x3B2, false, "That item is not bound to you and loses it's hue as you equip it.");
                    }
                }
            }

            if (item.PlayerClass == PlayerClass.None || item.PlayerClassRestricted == false)
                return true;

            bool playerClassMatch = false;

            if (item.PlayerClass == PlayerClass.Paladin && player.Paladin)
                playerClassMatch = true;

            if (item.PlayerClass == PlayerClass.Murderer && player.Murderer)
                playerClassMatch = true;

            if (item.PlayerClass == PlayerClass.Pirate && player.Pirate)
                playerClassMatch = true;

            //Player is the Item's Owner
            if (playerClassMatch && player == item.PlayerClassOwner)
                return true;

            //Item is Weapon with an Owner: Player Can Equip But Item Loses Hue and Previous Owner
            if (item is BaseWeapon)
            {
                Effects.SendLocationEffect(from.Location, from.Map, 0x3728, 13);
                from.LocalOverheadMessage(Server.Network.MessageType.Regular, 0x3B2, false, "That item is not bound to you and loses it's hue as you equip it.");

                item.PlayerClass = PlayerClass.None;
                item.PlayerClassOwner = null;
                item.PlayerClassRestricted = false;

                item.Hue = 0;

                return true;
            }

            from.LocalOverheadMessage(Server.Network.MessageType.Regular, 0x3B2, false, "That item is not bound to you.");

            return false;
        }

        public static void PlayerClassSingleClick(Item item, Mobile from)
        {
            if (item.PlayerClass != PlayerClass.None && item.PlayerClassRestricted)
            {
                if (item.PlayerClassOwner != null)
                {
                    NetState ns = from.NetState;

                    if (ns != null)
                        ns.Send(new UnicodeMessage(item.Serial, item.ItemID, MessageType.Label, 0, 3, "ENU", "", "[Bound to " + item.PlayerClassOwner.RawName + "]"));
                }
            }
        }

        public static bool PlayerClassDoubleClick(Item item, Mobile from)
        {
            if (item.PlayerClass != PlayerClass.None && item.PlayerClassRestricted)
                return PlayerClassCanUse(from, item);

            return false;
        }

        public static Layer[] m_EquipmentLayers = new Layer[]
		{
			Layer.Cloak,		Layer.Bracelet,     Layer.Ring,			Layer.Shirt,	    Layer.Pants,
			Layer.InnerLegs,	Layer.Shoes,	    Layer.Arms,			Layer.InnerTorso,   Layer.MiddleTorso,
			Layer.OuterLegs,	Layer.Neck,		    Layer.Waist,		Layer.Gloves,		Layer.OuterTorso,
			Layer.OneHanded,    Layer.TwoHanded,    Layer.FacialHair,	Layer.Hair,			Layer.Helm,
			Layer.Talisman
		};

        public enum PlayerClassScoreType
        {
            Paladin,
            Murderer,
            Pirate
        }

        public class PlayerClassRecord : Item
        {
            public PlayerClassScoreType m_PlayerClassScoreType = PlayerClassScoreType.Paladin;
            public PlayerMobile pm_Player = null;
            public int m_Score = 0;

            [Constructable]
            public PlayerClassRecord(PlayerClassScoreType playerClassScoreType, PlayerMobile player, int score)
            {
                m_PlayerClassScoreType = playerClassScoreType;
                pm_Player = player;
                m_Score = score;
            }

            public PlayerClassRecord(Serial serial): base(serial)
		    {
		    }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
                writer.WriteEncodedInt(0); // version

                writer.Write((int)m_PlayerClassScoreType);
                writer.Write(pm_Player);
                writer.Write(m_Score);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                int version = reader.ReadEncodedInt();

                //Version 0
                if (version >= 0)
                {
                    m_PlayerClassScoreType = (PlayerClassScoreType)reader.ReadInt();
                    pm_Player = reader.ReadMobile() as PlayerMobile;
                    m_Score = reader.ReadInt();
                }
            }
        }

        //No Longer Used
        public class PlayerClassScores
        {
            public PlayerMobile m_Player;

            public int PaladinScore = 0;
            public int MurdererScore = 0;
            public int PirateScore = 0;
            public int ThiefScore = 0;
            public int DetectiveScore = 0;
            public int CrafterScore = 0;
            public int TamerScore = 0;
            public int MonsterHunterScore = 0;
            public int TreasureHunterScore = 0;

            [Constructable]
            public PlayerClassScores(PlayerMobile player)
            {
                m_Player = player;
            }
        }

        public static void InitializeLifetimeScores()
        {   
            int index = -1;

            //Paladin
            if (m_LifetimePaladinScores.Count == 0)
            {
                foreach (Account account in Accounts.GetAccounts())
                {
                    if (account == null)
                        continue;

                    foreach (Mobile mobile in account.accountMobiles)
                    {
                        if (mobile == null)
                            continue;

                        PlayerMobile pm_Mobile = mobile as PlayerMobile;

                        if (pm_Mobile != null)
                        {
                            if (pm_Mobile.PaladinScore > 0)
                            {
                                if (m_LifetimePaladinScores.Count == 0)
                                    m_LifetimePaladinScores.Add(new PlayerClassRecord(PlayerClassScoreType.Paladin, pm_Mobile, pm_Mobile.PaladinScore));
                                
                                else
                                {
                                    index = -1;

                                    for (int a = 0; a < m_LifetimePaladinScores.Count; a++)
                                    {
                                        if (pm_Mobile.PaladinScore >= m_LifetimePaladinScores[a].m_Score)
                                        {
                                            index = a;
                                            m_LifetimePaladinScores.Insert(index, new PlayerClassRecord(PlayerClassScoreType.Paladin, pm_Mobile, pm_Mobile.PaladinScore));
                                            
                                            break;
                                        }
                                    }

                                    if (index == -1)
                                        m_LifetimePaladinScores.Add(new PlayerClassRecord(PlayerClassScoreType.Paladin, pm_Mobile, pm_Mobile.PaladinScore));
                                }
                            }
                        }
                    }
                }
            }

            //Murderer
            if (m_LifetimeMurdererScores.Count == 0)
            {
                foreach (Account account in Accounts.GetAccounts())
                {
                    if (account == null)
                        continue;

                    foreach (Mobile mobile in account.accountMobiles)
                    {
                        if (mobile == null)
                            continue;

                        PlayerMobile pm_Mobile = mobile as PlayerMobile;

                        if (pm_Mobile != null)
                        {
                            if (pm_Mobile.MurdererScore > 0)
                            {
                                if (m_LifetimeMurdererScores.Count == 0)                               
                                    m_LifetimeMurdererScores.Add(new PlayerClassRecord(PlayerClassScoreType.Murderer, pm_Mobile, pm_Mobile.MurdererScore));
                                
                                else
                                {
                                    index = -1;

                                    for (int a = 0; a < m_LifetimeMurdererScores.Count; a++)
                                    {
                                        if (pm_Mobile.MurdererScore >= m_LifetimeMurdererScores[a].m_Score)
                                        {
                                            index = a;
                                            m_LifetimeMurdererScores.Insert(index, new PlayerClassRecord(PlayerClassScoreType.Murderer, pm_Mobile, pm_Mobile.MurdererScore));
                                            
                                            break;
                                        }
                                    }

                                    if (index == -1)
                                        m_LifetimeMurdererScores.Add(new PlayerClassRecord(PlayerClassScoreType.Murderer, pm_Mobile, pm_Mobile.MurdererScore));
                                }
                            }
                        }
                    }
                }
            }

            //Pirate
            if (m_LifetimePirateScores.Count == 0)
            {
                foreach (Account account in Accounts.GetAccounts())
                {
                    if (account == null)
                        continue;

                    foreach (Mobile mobile in account.accountMobiles)
                    {
                        if (mobile == null)
                            continue;

                        PlayerMobile pm_Mobile = mobile as PlayerMobile;

                        if (pm_Mobile != null)
                        {
                            if (pm_Mobile.PirateScore > 0)
                            {
                                if (m_LifetimePirateScores.Count == 0)
                                    m_LifetimePirateScores.Add(new PlayerClassRecord(PlayerClassScoreType.Pirate, pm_Mobile, pm_Mobile.PirateScore));
                                else
                                {
                                    index = -1;

                                    for (int a = 0; a < m_LifetimePirateScores.Count; a++)
                                    {
                                        if (pm_Mobile.PirateScore >= m_LifetimePirateScores[a].m_Score)
                                        {
                                            index = a;
                                            m_LifetimePirateScores.Insert(index, new PlayerClassRecord(PlayerClassScoreType.Pirate, pm_Mobile, pm_Mobile.PirateScore));

                                            break;
                                        }
                                    }

                                    if (index == -1)
                                        m_LifetimePirateScores.Add(new PlayerClassRecord(PlayerClassScoreType.Pirate, pm_Mobile, pm_Mobile.PirateScore));
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void UpdatePlayerScore(PlayerClassScoreType playerClassScoreType, PlayerMobile player, int scoreChange)
        {
            if (player == null)
                return;

            //Initialize Lifetime Scores (Should Only Happen Once)
            InitializeLifetimeScores();

            PlayerClassRecord record = null;
            int index = -1;            

            switch (playerClassScoreType)
            {
                case PlayerClassScoreType.Paladin:
                    //Lifetime
                    record = null;
                    index = -1;

                    for (int a = 0; a < m_LifetimePaladinScores.Count; a++)
                    {
                        if (m_LifetimePaladinScores[a].pm_Player == player)
                        {
                            record = m_LifetimePaladinScores[a];
                            record.m_Score += scoreChange;
                            index = a;

                            break;
                        }
                    }

                    if (index != -1)
                        m_LifetimePaladinScores.RemoveAt(index);

                    if (record == null)                    
                        record = new PlayerClassRecord(playerClassScoreType, player, player.PaladinScore);                    

                    for (int a = 0; a < m_LifetimePaladinScores.Count; a++)
                    {
                        if (record.m_Score >= m_LifetimePaladinScores[a].m_Score)
                        {
                            index = a;
                            break;
                        }
                    }

                    if (index != -1)
                        m_LifetimePaladinScores.Insert(index, record);
                    else
                        m_LifetimePaladinScores.Add(record);

                    //Current
                    record = null;
                    index = -1;

                    for (int a = 0; a < m_CurrentPaladinScores.Count; a++)
                    {
                        if (m_CurrentPaladinScores[a].pm_Player == player)
                        {
                            record = m_CurrentPaladinScores[a];
                            record.m_Score += scoreChange;
                            index = a;

                            break;
                        }
                    }

                    if (index != -1)
                        m_CurrentPaladinScores.RemoveAt(index);

                    if (record == null)
                        record = new PlayerClassRecord(playerClassScoreType, player, scoreChange);

                    for (int a = 0; a < m_CurrentPaladinScores.Count; a++)
                    {
                        if (record.m_Score >= m_CurrentPaladinScores[a].m_Score)
                        {
                            index = a;
                            break;
                        }
                    }

                    if (index != -1)
                        m_CurrentPaladinScores.Insert(index, record);
                    else
                        m_CurrentPaladinScores.Add(record);
                break;   
                   
                case PlayerClassScoreType.Murderer:
                    //Lifetime
                    record = null;
                    index = -1;

                    for (int a = 0; a < m_LifetimeMurdererScores.Count; a++)
                    {
                        if (m_LifetimeMurdererScores[a].pm_Player == player)
                        {
                            record = m_LifetimeMurdererScores[a];
                            record.m_Score += scoreChange;
                            index = a;

                            break;
                        }
                    }

                    if (index != -1)
                        m_LifetimeMurdererScores.RemoveAt(index);

                    if (record == null)
                        record = new PlayerClassRecord(playerClassScoreType, player, player.MurdererScore);

                    index = -1;

                    for (int a = 0; a < m_LifetimeMurdererScores.Count; a++)
                    {
                        if (record != null && m_LifetimeMurdererScores[a] != null)
                        {
                            if (record.m_Score >= m_LifetimeMurdererScores[a].m_Score)
                            {
                                index = a;
                                break;
                            }
                        }
                    }

                    if (index != -1)
                        m_LifetimeMurdererScores.Insert(index, record);

                    else
                        m_LifetimeMurdererScores.Add(record);

                    //Current
                    record = null;
                    index = -1;

                    for (int a = 0; a < m_CurrentMurdererScores.Count; a++)
                    {
                        if (m_CurrentMurdererScores[a] != null)
                        {
                            if (m_CurrentMurdererScores[a].pm_Player == player)
                            {
                                record = m_CurrentMurdererScores[a];
                                record.m_Score += scoreChange;
                                index = a;

                                break;
                            }
                        }
                    }

                    if (index != -1)
                        m_CurrentMurdererScores.RemoveAt(index);

                    if (record == null)
                        record = new PlayerClassRecord(playerClassScoreType, player, scoreChange);

                    index = -1;

                    for (int a = 0; a < m_CurrentMurdererScores.Count; a++)
                    {
                        if (record != null && m_CurrentMurdererScores[a] != null)
                        {
                            if (record.m_Score >= m_CurrentMurdererScores[a].m_Score)
                            {
                                index = a;
                                break;
                            }
                        }
                    }

                    if (index != -1)
                        m_CurrentMurdererScores.Insert(index, record);
                    else
                        m_CurrentMurdererScores.Add(record);
                break;

                case PlayerClassScoreType.Pirate:                    
                    //Lifetime
                    record = null;
                    index = -1;

                    for (int a = 0; a < m_LifetimePirateScores.Count; a++)
                    {
                        if (m_LifetimePirateScores[a].pm_Player == player)
                        {
                            record = m_LifetimePirateScores[a];
                            record.m_Score += scoreChange;
                            index = a;

                            break;
                        }
                    }

                    if (index != -1)
                        m_LifetimePirateScores.RemoveAt(index);

                    if (record == null)
                        record = new PlayerClassRecord(playerClassScoreType, player, player.PirateScore);

                    index = -1;

                    for (int a = 0; a < m_LifetimePirateScores.Count; a++)
                    {
                        if (record != null && m_LifetimePirateScores[a] != null)
                        {
                            if (record.m_Score >= m_LifetimePirateScores[a].m_Score)
                            {
                                index = a;
                                break;
                            }
                        }
                    }

                    if (index != -1)
                        m_LifetimePirateScores.Insert(index, record);

                    else
                        m_LifetimePirateScores.Add(record);

                    //Current
                    record = null;
                    index = -1;

                    for (int a = 0; a < m_CurrentPirateScores.Count; a++)
                    {
                        if (m_CurrentPirateScores[a] != null)
                        {
                            if (m_CurrentPirateScores[a].pm_Player == player)
                            {
                                record = m_CurrentPirateScores[a];
                                record.m_Score += scoreChange;
                                index = a;

                                break;
                            }
                        }
                    }

                    if (index != -1)
                        m_CurrentPirateScores.RemoveAt(index);

                    if (record == null)
                        record = new PlayerClassRecord(playerClassScoreType, player, scoreChange);

                    index = -1;

                    for (int a = 0; a < m_CurrentPirateScores.Count; a++)
                    {
                        if (record != null && m_CurrentPirateScores[a] != null)
                        {
                            if (record.m_Score >= m_CurrentPirateScores[a].m_Score)
                            {
                                index = a;
                                break;
                            }
                        }
                    }

                    if (index != -1)
                        m_CurrentPirateScores.Insert(index, record);
                    else
                        m_CurrentPirateScores.Add(record);
                break;
            }
        }

        public static void CycleComplete()
        {
            List<NetState> m_States = NetState.Instances;

            for (int i = 0; i < m_States.Count; ++i)
            {
                PlayerMobile player = m_States[i].Mobile as PlayerMobile;

                if (player == null)
                    continue;

                player.CloseGump(typeof(Custom.PlayerClassScoresGump));
            }

            m_PreviousPaladinScores.Clear();
            m_PreviousMurdererScores.Clear();
            m_PreviousPirateScores.Clear();

            //Paladin
            int validPaladins = 0;
            int currentPaladinIndex = 0;
            int totalPaladinScore = 0;

            for (int a = 0; a < m_CurrentPaladinScores.Count; a++)
            {
                PlayerClassRecord record = m_CurrentPaladinScores[a];

                if (record == null) continue;
                if (record.pm_Player == null) continue;
                if (record.pm_Player.AccessLevel > AccessLevel.Player) continue;

                validPaladins++;
            }
            
            for (int a = 0; a < m_CurrentPaladinScores.Count; a++)
            {
                PlayerClassRecord record = m_CurrentPaladinScores[a];

                if (record == null) continue;
                if (record.pm_Player == null) continue;
                if (record.pm_Player.AccessLevel > AccessLevel.Player) continue;

                currentPaladinIndex++;

                m_PreviousPaladinScores.Add(record);

                record.pm_Player.SendMessage("Paladin scores for the week have been tabulated. You have ranked " + currentPaladinIndex.ToString() + " out of " + validPaladins.ToString() + " paladins with a score of " + record.m_Score.ToString() + " points.");
            }

            //Murderer
            int validMurderers = 0;
            int validMurdererIndex = 0;
            int totalMurdererScore = 0;

            for (int a = 0; a < m_CurrentMurdererScores.Count; a++)
            {
                PlayerClassRecord record = m_CurrentMurdererScores[a];

                if (record == null) continue;
                if (record.pm_Player == null) continue;
                if (record.pm_Player.AccessLevel > AccessLevel.Player) continue;

                validMurderers++;
            }

            for (int a = 0; a < m_CurrentMurdererScores.Count; a++)
            {
                PlayerClassRecord record = m_CurrentMurdererScores[a];

                if (record == null) continue;
                if (record.pm_Player == null) continue;
                if (record.pm_Player.AccessLevel > AccessLevel.Player) continue;

                validMurdererIndex++;

                m_PreviousMurdererScores.Add(record);

                record.pm_Player.SendMessage("Murderer scores for the week have been tabulated. You have ranked " + validMurdererIndex.ToString() + " out of " + validMurderers.ToString() + " murderers with a score of " + record.m_Score.ToString() + " points.");
            }

            //Pirate
            int validPirates = 0;
            int validPirateIndex = 0;
            int totalPirateScore = 0;

            for (int a = 0; a < m_CurrentPirateScores.Count; a++)
            {
                PlayerClassRecord record = m_CurrentPirateScores[a];

                if (record == null) continue;
                if (record.pm_Player == null) continue;
                if (record.pm_Player.AccessLevel > AccessLevel.Player) continue;

                validPirates++;
            }

            for (int a = 0; a < m_CurrentPirateScores.Count; a++)
            {
                PlayerClassRecord record = m_CurrentPirateScores[a];

                if (record == null) continue;
                if (record.pm_Player == null) continue;
                if (record.pm_Player.AccessLevel > AccessLevel.Player) continue;

                validPirateIndex++;

                m_PreviousPirateScores.Add(record);

                record.pm_Player.SendMessage("Pirate scores for the week have been tabulated. You have ranked " + validPirateIndex.ToString() + " out of " + validPirates.ToString() + " pirates with a score of " + record.m_Score.ToString() + " points.");
            }

            m_CurrentPaladinScores.Clear();
            m_CurrentMurdererScores.Clear();
            m_CurrentPirateScores.Clear();
        }

        public static void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(2); //Version 

            //Version 0           
            writer.Write((DateTime)lastScoreCalculation);

            //No Longer Being Used
            writer.Write((int)m_PlayerScores.Count);
            foreach (PlayerClassScores score in m_PlayerScores)
            {
                writer.Write(score.m_Player);

                writer.Write(score.PaladinScore);
                writer.Write(score.MurdererScore);
                writer.Write(score.PirateScore);
                writer.Write(score.ThiefScore);
                writer.Write(score.DetectiveScore);
                writer.Write(score.CrafterScore);
                writer.Write(score.TamerScore);
                writer.Write(score.MonsterHunterScore);
                writer.Write(score.TreasureHunterScore);
            }

            //Version 2

            //Paladin Scores
            writer.Write((int)m_LifetimePaladinScores.Count);
            for (int a = 0; a < m_LifetimePaladinScores.Count; a++)
            {
                PlayerClassRecord record = m_LifetimePaladinScores[a];

                writer.Write((int)record.m_PlayerClassScoreType);
                writer.Write(record.pm_Player);
                writer.Write(record.m_Score);
            }

            writer.Write((int)m_PreviousPaladinScores.Count);
            for (int a = 0; a < m_PreviousPaladinScores.Count; a++)
            {
                PlayerClassRecord record = m_PreviousPaladinScores[a];

                writer.Write((int)record.m_PlayerClassScoreType);
                writer.Write(record.pm_Player);
                writer.Write(record.m_Score);
            }

            writer.Write((int)m_CurrentPaladinScores.Count);
            for (int a = 0; a < m_CurrentPaladinScores.Count; a++)
            {
                PlayerClassRecord record = m_CurrentPaladinScores[a];

                writer.Write((int)record.m_PlayerClassScoreType);
                writer.Write(record.pm_Player);
                writer.Write(record.m_Score);
            }

            //Murderer Scores
            writer.Write((int)m_LifetimeMurdererScores.Count);
            for (int a = 0; a < m_LifetimeMurdererScores.Count; a++)
            {
                PlayerClassRecord record = m_LifetimeMurdererScores[a];

                writer.Write((int)record.m_PlayerClassScoreType);
                writer.Write(record.pm_Player);
                writer.Write(record.m_Score);
            }

            writer.Write((int)m_PreviousMurdererScores.Count);
            for (int a = 0; a < m_PreviousMurdererScores.Count; a++)
            {
                PlayerClassRecord record = m_PreviousMurdererScores[a];

                writer.Write((int)record.m_PlayerClassScoreType);
                writer.Write(record.pm_Player);
                writer.Write(record.m_Score);
            }

            writer.Write((int)m_CurrentMurdererScores.Count);
            for (int a = 0; a < m_CurrentMurdererScores.Count; a++)
            {
                PlayerClassRecord record = m_CurrentMurdererScores[a];

                writer.Write((int)record.m_PlayerClassScoreType);
                writer.Write(record.pm_Player);
                writer.Write(record.m_Score);
            }

            //Pirate
            writer.Write((int)m_LifetimePirateScores.Count);
            for (int a = 0; a < m_LifetimePirateScores.Count; a++)
            {
                PlayerClassRecord record = m_LifetimePirateScores[a];

                writer.Write((int)record.m_PlayerClassScoreType);
                writer.Write(record.pm_Player);
                writer.Write(record.m_Score);
            }

            writer.Write((int)m_PreviousPirateScores.Count);
            for (int a = 0; a < m_PreviousPirateScores.Count; a++)
            {
                PlayerClassRecord record = m_PreviousPirateScores[a];

                writer.Write((int)record.m_PlayerClassScoreType);
                writer.Write(record.pm_Player);
                writer.Write(record.m_Score);
            }

            writer.Write((int)m_CurrentPirateScores.Count);
            for (int a = 0; a < m_CurrentPirateScores.Count; a++)
            {
                PlayerClassRecord record = m_CurrentPirateScores[a];

                writer.Write((int)record.m_PlayerClassScoreType);
                writer.Write(record.pm_Player);
                writer.Write(record.m_Score);
            }
        }

        public static void Deserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            m_LifetimePaladinScores = new List<PlayerClassRecord>();
            m_PreviousPaladinScores = new List<PlayerClassRecord>();
            m_CurrentPaladinScores = new List<PlayerClassRecord>();

            m_LifetimeMurdererScores = new List<PlayerClassRecord>();
            m_PreviousMurdererScores = new List<PlayerClassRecord>();
            m_CurrentMurdererScores = new List<PlayerClassRecord>();

            m_LifetimePirateScores = new List<PlayerClassRecord>();
            m_PreviousPirateScores = new List<PlayerClassRecord>();
            m_CurrentPirateScores = new List<PlayerClassRecord>();

            //Version 0           
            lastScoreCalculation = reader.ReadDateTime();
           
            //No Longer Being Used
            int scoreEntries = reader.ReadInt();
            for (int a = 0; a < scoreEntries; ++a)
            {
                PlayerMobile pm_Player = reader.ReadMobile() as PlayerMobile;
                PlayerClassScores scoreEntry = new PlayerClassScores(pm_Player);

                scoreEntry.PaladinScore = reader.ReadInt();
                scoreEntry.MurdererScore = reader.ReadInt();
                scoreEntry.PirateScore = reader.ReadInt();
                scoreEntry.ThiefScore = reader.ReadInt();
                scoreEntry.DetectiveScore = reader.ReadInt();
                scoreEntry.CrafterScore = reader.ReadInt();
                scoreEntry.TamerScore = reader.ReadInt();
                scoreEntry.MonsterHunterScore = reader.ReadInt();
                scoreEntry.TreasureHunterScore = reader.ReadInt();                
            }      
      
            //Version 1: Note: This is only for EXACTLY Version 1
            if (version == 1)
            {
                //Paladin
                int lifetimePaladinScoresCount = reader.ReadInt();
                for (int a = 0; a < lifetimePaladinScoresCount; a++)
                {
                    PlayerClassRecord record = reader.ReadItem() as PlayerClassRecord;
                    m_LifetimePaladinScores.Add(record);
                }

                int previousPaladinScoresCount = reader.ReadInt();
                for (int a = 0; a < previousPaladinScoresCount; a++)
                {
                    PlayerClassRecord record = reader.ReadItem() as PlayerClassRecord;
                    m_PreviousPaladinScores.Add(record);
                }

                int currentPaladinScoresCount = reader.ReadInt();
                for (int a = 0; a < currentPaladinScoresCount; a++)
                {
                    PlayerClassRecord record = reader.ReadItem() as PlayerClassRecord;

                    if (record.pm_Player != null)
                    {
                        if (!record.pm_Player.Deleted)
                            m_CurrentPaladinScores.Add(record);
                    }
                }

                //Murderer
                int lifetimeMurdererScoresCount = reader.ReadInt();
                for (int a = 0; a < lifetimeMurdererScoresCount; a++)
                {
                    PlayerClassRecord record = reader.ReadItem() as PlayerClassRecord;
                    m_LifetimeMurdererScores.Add(record);
                }

                int previousMurdererScoresCount = reader.ReadInt();
                for (int a = 0; a < previousMurdererScoresCount; a++)
                {
                    PlayerClassRecord record = reader.ReadItem() as PlayerClassRecord;
                    m_PreviousMurdererScores.Add(record);
                }

                int currentMurdererScoresCount = reader.ReadInt();
                for (int a = 0; a < currentMurdererScoresCount; a++)
                {
                    PlayerClassRecord record = reader.ReadItem() as PlayerClassRecord;

                    if (record.pm_Player != null)
                    {
                        if (!record.pm_Player.Deleted)
                            m_CurrentMurdererScores.Add(record);
                    }
                }

                //Pirate
                int lifetimePirateScoresCount = reader.ReadInt();
                for (int a = 0; a < lifetimePirateScoresCount; a++)
                {
                    PlayerClassRecord record = reader.ReadItem() as PlayerClassRecord;
                    m_LifetimePirateScores.Add(record);
                }

                int previousPirateScoresCount = reader.ReadInt();
                for (int a = 0; a < previousPirateScoresCount; a++)
                {
                    PlayerClassRecord record = reader.ReadItem() as PlayerClassRecord;
                    m_PreviousPirateScores.Add(record);
                }

                int currentPirateScoresCount = reader.ReadInt();
                for (int a = 0; a < currentPirateScoresCount; a++)
                {
                    PlayerClassRecord record = reader.ReadItem() as PlayerClassRecord;

                    if (record.pm_Player != null)
                    {
                        if (!record.pm_Player.Deleted)
                            m_CurrentPirateScores.Add(record);
                    }
                }
            }

            //Version 2
            if (version >= 2)
            {
                //Paladin
                int lifetimePaladinScoresCount = reader.ReadInt();
                for (int a = 0; a < lifetimePaladinScoresCount; a++)
                {
                    PlayerClassScoreType playerClassScoreType = (PlayerClassScoreType)reader.ReadInt();
                    PlayerMobile player = (PlayerMobile)reader.ReadMobile();
                    int score = reader.ReadInt();

                    PlayerClassRecord record = new PlayerClassRecord(playerClassScoreType, player, score);
                    m_LifetimePaladinScores.Add(record);
                }

                int previousPaladinScoresCount = reader.ReadInt();
                for (int a = 0; a < previousPaladinScoresCount; a++)
                {
                    PlayerClassScoreType playerClassScoreType = (PlayerClassScoreType)reader.ReadInt();
                    PlayerMobile player = (PlayerMobile)reader.ReadMobile();
                    int score = reader.ReadInt();

                    PlayerClassRecord record = new PlayerClassRecord(playerClassScoreType, player, score);
                    m_PreviousPaladinScores.Add(record);
                }

                int currentPaladinScoresCount = reader.ReadInt();
                for (int a = 0; a < currentPaladinScoresCount; a++)
                {
                    PlayerClassScoreType playerClassScoreType = (PlayerClassScoreType)reader.ReadInt();
                    PlayerMobile player = (PlayerMobile)reader.ReadMobile();
                    int score = reader.ReadInt();

                    PlayerClassRecord record = new PlayerClassRecord(playerClassScoreType, player, score);
                    m_CurrentPaladinScores.Add(record);
                }

                //Murderer
                int lifetimeMurdererScoresCount = reader.ReadInt();
                for (int a = 0; a < lifetimeMurdererScoresCount; a++)
                {
                    PlayerClassScoreType playerClassScoreType = (PlayerClassScoreType)reader.ReadInt();
                    PlayerMobile player = (PlayerMobile)reader.ReadMobile();
                    int score = reader.ReadInt();

                    PlayerClassRecord record = new PlayerClassRecord(playerClassScoreType, player, score);
                    m_LifetimeMurdererScores.Add(record);
                }

                int previousMurdererScoresCount = reader.ReadInt();
                for (int a = 0; a < previousMurdererScoresCount; a++)
                {
                    PlayerClassScoreType playerClassScoreType = (PlayerClassScoreType)reader.ReadInt();
                    PlayerMobile player = (PlayerMobile)reader.ReadMobile();
                    int score = reader.ReadInt();

                    PlayerClassRecord record = new PlayerClassRecord(playerClassScoreType, player, score);
                    m_PreviousMurdererScores.Add(record);
                }

                int currentMurdererScoresCount = reader.ReadInt();
                for (int a = 0; a < currentMurdererScoresCount; a++)
                {
                    PlayerClassScoreType playerClassScoreType = (PlayerClassScoreType)reader.ReadInt();
                    PlayerMobile player = (PlayerMobile)reader.ReadMobile();
                    int score = reader.ReadInt();

                    PlayerClassRecord record = new PlayerClassRecord(playerClassScoreType, player, score);
                    m_CurrentMurdererScores.Add(record);
                }

                //Pirate
                int lifetimePirateScoresCount = reader.ReadInt();
                for (int a = 0; a < lifetimePirateScoresCount; a++)
                {
                    PlayerClassScoreType playerClassScoreType = (PlayerClassScoreType)reader.ReadInt();
                    PlayerMobile player = (PlayerMobile)reader.ReadMobile();
                    int score = reader.ReadInt();

                    PlayerClassRecord record = new PlayerClassRecord(playerClassScoreType, player, score);
                    m_LifetimePirateScores.Add(record);
                }

                int previousPirateScoresCount = reader.ReadInt();
                for (int a = 0; a < previousPirateScoresCount; a++)
                {
                    PlayerClassScoreType playerClassScoreType = (PlayerClassScoreType)reader.ReadInt();
                    PlayerMobile player = (PlayerMobile)reader.ReadMobile();
                    int score = reader.ReadInt();

                    PlayerClassRecord record = new PlayerClassRecord(playerClassScoreType, player, score);
                    m_PreviousPirateScores.Add(record);
                }                

                int currentPirateScoresCount = reader.ReadInt();

                for (int a = 0; a < currentPirateScoresCount; a++)
                {
                    PlayerClassScoreType playerClassScoreType = (PlayerClassScoreType)reader.ReadInt();
                    PlayerMobile player = (PlayerMobile)reader.ReadMobile();
                    int score = reader.ReadInt();

                    PlayerClassRecord record = new PlayerClassRecord(playerClassScoreType, player, score);
                    m_CurrentPirateScores.Add(record);
                }
            }
        }
    }    

    public class PlayerClassPersistanceItem : Item
    {
        public override string DefaultName { get { return "PlayerClassPersistance"; } }

        public PlayerClassPersistanceItem(): base(0x0)
        {
            Movable = false;
        }

        public PlayerClassPersistanceItem(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            PlayerClassPersistance.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            PlayerClassPersistance.PersistanceItem = this;
            PlayerClassPersistance.Deserialize(reader);
        }
    }

    public class PlayerClassItemRansomEntry
    {
        public Mobile m_Ransomer;
        public Type m_ItemType;
        public int m_GoldCost = 0;

        public PlayerClassItemRansomEntry(Mobile ransomer, Type itemType, int goldCost)
        {
            m_Ransomer = ransomer;
            m_ItemType = itemType;
            m_GoldCost = goldCost;           
        } 
    }
}
