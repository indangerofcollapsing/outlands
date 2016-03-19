using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;
using Server.Multis;
using Server.Accounting;
using Server.Gumps;

namespace Server.Items
{
    public class PowerScrollLibrary : Item
    {
        public enum LockedDownAccessLevelType
        {
            Owner,
            CoOwner,
            Friend,
            Anyone
        }

        public static string GetSkillName(SkillName skillName)
        {
            string name = "";

            #region Skills

            switch (skillName)
            {
                case SkillName.Alchemy: name = "Alchemy"; break;
                case SkillName.Anatomy: name = "Anatomy"; break;
                case SkillName.AnimalLore: name = "Animal Lore"; break;
                case SkillName.AnimalTaming: name = "Animal Taming"; break;
                case SkillName.Archery: name = "Archery"; break;
                case SkillName.ArmsLore: name = "Arms Lore"; break;
                case SkillName.Begging: name = "Begging"; break;
                case SkillName.Blacksmith: name = "Blacksmithy"; break;
                case SkillName.Camping: name = "Camping"; break;
                case SkillName.Carpentry: name = "Carpentry"; break;
                case SkillName.Cartography: name = "Cartography"; break;
                case SkillName.Cooking: name = "Cooking"; break;
                case SkillName.DetectHidden: name = "Detect Hidden"; break;
                case SkillName.Discordance: name = "Discordance"; break;
                case SkillName.EvalInt: name = "Eval Int"; break;
                case SkillName.Fencing: name = "Fencing"; break;
                case SkillName.Fishing: name = "Fishing"; break;
                case SkillName.Fletching: name = "Fletching"; break;
                case SkillName.Forensics: name = "Forensic Eval"; break;
                case SkillName.Healing: name = "Healing"; break;
                case SkillName.Herding: name = "Herding"; break;
                case SkillName.Hiding: name = "Hiding"; break;
                case SkillName.Inscribe: name = "Inscription"; break;
                case SkillName.ItemID: name = "Item Id"; break;
                case SkillName.Lockpicking: name = "Lockpicking"; break;
                case SkillName.Lumberjacking: name = "Lumberjacking"; break;
                case SkillName.Macing: name = "Macing"; break;
                case SkillName.Magery: name = "Magery"; break;
                case SkillName.MagicResist: name = "Magic Resist"; break;
                case SkillName.Meditation: name = "Meditation"; break;
                case SkillName.Mining: name = "Mining"; break;
                case SkillName.Musicianship: name = "Musicianship"; break;
                case SkillName.Parry: name = "Parrying"; break;
                case SkillName.Peacemaking: name = "Peacemaking"; break;
                case SkillName.Poisoning: name = "Poisoning"; break;
                case SkillName.Provocation: name = "Provocation"; break;
                case SkillName.RemoveTrap: name = "Remove Trap"; break;
                case SkillName.Snooping: name = "Snooping"; break;
                case SkillName.SpiritSpeak: name = "Spirit Speak"; break;
                case SkillName.Stealing: name = "Stealing"; break;
                case SkillName.Stealth: name = "Stealth"; break;
                case SkillName.Swords: name = "Swordsmanship"; break;
                case SkillName.Tactics: name = "Tactics"; break;
                case SkillName.Tailoring: name = "Tailoring"; break;
                case SkillName.TasteID: name = "Taste Id"; break;
                case SkillName.Tinkering: name = "Tinkering"; break;
                case SkillName.Tracking: name = "Tracking"; break;
                case SkillName.Veterinary: name = "Veterinary"; break;
                case SkillName.Wrestling: name = "Wrestling"; break;
            }

            #endregion

            return name;            
        }

        private LockedDownAccessLevelType m_LockedDownAccessLevel = LockedDownAccessLevelType.Owner;
        [CommandProperty(AccessLevel.GameMaster)]
        public LockedDownAccessLevelType LockedDownAccessLevel
        {
            get { return m_LockedDownAccessLevel; }
            set { m_LockedDownAccessLevel = value; }
        }

        private bool m_RemoveAllOnSelection = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool RemoveAllOnSelection
        {
            get { return m_RemoveAllOnSelection; }
            set { m_RemoveAllOnSelection = value; }
        }

        public override bool AlwaysAllowDoubleClick { get { return true; } }

        public List<PowerScrollLibraryEntry> m_LibraryEntries = new List<PowerScrollLibraryEntry>();

        public int openGumpSound = 0x055;
        public int changeGumpSound = 0x057;
        public int closeGumpSound = 0x058;
        public int addItemSound = 0x249;

        [Constructable]
        public PowerScrollLibrary(): base(8793)
        {
            Name = "a power scroll library";
            Hue = 2500;
            Weight = 5;

            CheckSkillEntries();
        }

        public PowerScrollLibrary(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (IsLockedDown)
            {
                switch (m_LockedDownAccessLevel)
                {
                    case LockedDownAccessLevelType.Owner: LabelTo(from, "(owner accessable)"); break;
                    case LockedDownAccessLevelType.CoOwner: LabelTo(from, "(co-owner accessable)"); break;
                    case LockedDownAccessLevelType.Friend: LabelTo(from, "(friend accessable)"); break;
                    case LockedDownAccessLevelType.Anyone: LabelTo(from, "(freely access)"); break;
                }
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            //base.OnDoubleClick(from);

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            if (!CanUse(player))
                return;

            player.SendSound(openGumpSound);
            
            from.CloseGump(typeof(PowerScrollLibraryGump));
            from.SendGump(new PowerScrollLibraryGump(player, this, 1));
        }        

        public void CheckSkillEntries()
        {
            Queue m_Queue = new Queue();

            foreach (SkillName skillName in PowerScroll.Skills)
            {
                bool foundMatch = false;

                foreach (PowerScrollLibraryEntry entry in m_LibraryEntries)
                {
                    if (entry.SkillName == skillName)
                    {
                        foundMatch = true;
                        break;
                    }
                }

                if (!foundMatch)
                    m_Queue.Enqueue(skillName);
            }

            while (m_Queue.Count > 0)
            {
                SkillName skillName = (SkillName)m_Queue.Dequeue();

                PowerScrollLibraryEntry entry = new PowerScrollLibraryEntry();
                entry.SkillName = skillName;

                m_LibraryEntries.Add(entry);
            }
        }

        public PowerScrollLibraryEntry GetEntryDetail(SkillName skillName)
        {
            PowerScrollLibraryEntry targetEntry = null;

            foreach (PowerScrollLibraryEntry entry in m_LibraryEntries)
            {
                if (entry.SkillName == skillName)
                    return entry;
            }

            return targetEntry;
        }

        public void AddAllScrollsInPack(Mobile from)
        {
            if (from == null) return;
            if (from.Backpack == null) return;

            List<PowerScroll> m_PowerScrolls = from.Backpack.FindItemsByType<PowerScroll>();

            int totalCount = 0;

            Queue m_Queue = new Queue();

            foreach (PowerScroll powerScroll in m_PowerScrolls)
            {
                m_Queue.Enqueue(powerScroll);                
            }

            while (m_Queue.Count > 0)
            {
                PowerScroll powerScroll = (PowerScroll)m_Queue.Dequeue();
                PowerScrollLibraryEntry entry = GetEntryDetail(powerScroll.Skill);

                if (entry == null)
                    continue;

                switch ((int)powerScroll.Value)
                {
                    case 105: entry.Value105++; break;
                    case 110: entry.Value110++; break;
                    case 115: entry.Value115++; break;
                    case 120: entry.Value120++; break;
                }

                totalCount++;
                powerScroll.Delete();
            }

            if (totalCount > 1)
            {
                from.SendMessage("You add " + totalCount.ToString() + " power scrolls to the library.");
                from.SendSound(addItemSound);
            }

            else if (totalCount == 1)
            {
                from.SendMessage("You add a power scroll to the library.");
                from.SendSound(addItemSound);
            }

            else
                from.SendMessage("You do not have any power scrolls in your backpack.");
        }

        public void EjectScroll(Mobile from, SkillName skillName, int value, bool removeAll)
        {
            if (from == null)
                return;

            PowerScrollLibraryEntry entry = GetEntryDetail(skillName);

            if (entry == null)
                return;

            switch (value)
            {
                case 105:
                    if (entry.Value105 == 0)
                    {
                        from.SendMessage("The are no power scrolls of that type currently stored within.");
                        return;
                    }
                break;

                case 110:
                    if (entry.Value110 == 0)
                    {
                        from.SendMessage("The are no power scrolls of that type currently stored within.");
                        return;
                    }
                break;

                case 115:
                    if (entry.Value115 == 0)
                    {
                        from.SendMessage("The are no power scrolls of that type currently stored within.");
                        return;
                    }
                break;

                case 120:
                    if (entry.Value120 == 0)
                    {
                        from.SendMessage("The are no power scrolls of that type currently stored within.");
                        return;
                    }
                break;

                default:
                    return;
                break;
            }

            if (from.Backpack == null)
                return;

            if (from.Backpack.TotalItems == from.Backpack.MaxItems)
            {
                from.SendMessage("Your backpack is at maximum capacity. Please remove some items and try again.");
                return;
            }

            if (removeAll)
            {
                int scrollCount = 0;

                for (int a = 0; a < 1000; a++)
                {
                    bool outOfScrolls = false;

                    switch (value)
                    {
                        case 105: if (entry.Value105 == 0) outOfScrolls = true; break;
                        case 110: if (entry.Value110 == 0) outOfScrolls = true; break;
                        case 115: if (entry.Value115 == 0) outOfScrolls = true; break;
                        case 120: if (entry.Value120 == 0) outOfScrolls = true; break;
                    }

                    if (from.Backpack.TotalItems == from.Backpack.MaxItems)
                        break;

                    if (outOfScrolls)
                        break;

                    PowerScroll powerScroll = new PowerScroll(skillName, (double)value);

                    if (powerScroll != null)
                    {
                        switch (value)
                        {
                            case 105: entry.Value105--; break;
                            case 110: entry.Value110--; break;
                            case 115: entry.Value115--; break;
                            case 120: entry.Value120--; break;
                        }

                        from.Backpack.DropItem(powerScroll);                        
                    }

                    scrollCount++;
                }

                if (scrollCount > 1)
                {
                    from.SendMessage("You retrieve " + scrollCount.ToString() + " power scrolls from the library.");
                    from.SendSound(addItemSound);
                }

                else if (scrollCount == 1)
                {
                    from.SendMessage("You retrieve a power scroll from the library.");
                    from.SendSound(addItemSound);
                }

                else
                    from.SendMessage("You do not have any scrolls of that value in the library.");
            }

            else
            {
                PowerScroll powerScroll = new PowerScroll(skillName, (double)value);

                if (powerScroll != null)
                {
                    switch (value)
                    {
                        case 105: entry.Value105--; break;
                        case 110: entry.Value110--; break;
                        case 115: entry.Value115--; break;
                        case 120: entry.Value120--; break;
                    }

                    from.Backpack.DropItem(powerScroll);
                    from.SendSound(addItemSound);
                    from.SendMessage("You retrieve a power scroll from the library.");
                }
            }             
        }

        public bool CanUse(Mobile from)
        {
            if (from == null)
                return false;

            if (from.AccessLevel > AccessLevel.Player)
                return true;

            if (!from.Alive)
                return false;

            if (IsChildOf(from.Backpack) || IsChildOf(from.BankBox))
                return true;

            if (from.Map != Map || !from.InRange(GetWorldLocation(), 2))
            {
                from.SendMessage("That is too far away to use.");
                return false;
            }

            if (!from.InLOS(this))
            {
                from.SendMessage("That item is out of your line of sight.");
                return false;
            }

            if (IsLockedDown)
            {
                BaseHouse house = BaseHouse.FindHouseAt(Location, Map, 64);

                if (house == null)
                {
                    from.SendMessage("That is not accessible.");
                    return false;
                }

                switch (m_LockedDownAccessLevel)
                {
                    case LockedDownAccessLevelType.Owner:
                        if (house.Owner != null)
                        {
                            Account ownerAccount = house.Owner.Account as Account;
                            Account playerAccount = from.Account as Account;

                            if (ownerAccount != null && playerAccount != null && ownerAccount == playerAccount)
                                return true;

                            else
                            {
                                from.SendMessage("You do not have the neccessary access rights to use that.");
                                return false;
                            }
                        }
                        break;

                    case LockedDownAccessLevelType.CoOwner:
                        if (house.IsCoOwner(from))
                            return true;
                        else
                        {
                            from.SendMessage("You do not have the neccessary access rights to use that.");
                            return false;
                        }
                        break;

                    case LockedDownAccessLevelType.Friend:
                        if (house.IsFriend(from))
                            return true;

                        else
                        {
                            from.SendMessage("You do not have the neccessary access rights to use that.");
                            return false;
                        }

                        break;

                    case LockedDownAccessLevelType.Anyone:
                        return true;
                        break;
                }
            }

            if (RootParent is PlayerMobile && RootParent != from)
            {
                from.SendMessage("That is not accessible.");
                return false;
            }

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version 

            writer.Write((int)m_LockedDownAccessLevel);
            writer.Write(m_RemoveAllOnSelection);

            //Version 0
            writer.Write(m_LibraryEntries.Count);
            for (int a = 0; a < m_LibraryEntries.Count; a++)
            {
                PowerScrollLibraryEntry entry = m_LibraryEntries[a];

                writer.Write((int)entry.SkillName);

                writer.Write(entry.Value105);
                writer.Write(entry.Value110);
                writer.Write(entry.Value115);
                writer.Write(entry.Value120);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version
            if (version >= 0)
            {
                m_LockedDownAccessLevel = (LockedDownAccessLevelType)reader.ReadInt();
                m_RemoveAllOnSelection = reader.ReadBool();

                int libraryEntryCount = reader.ReadInt();

                for (int a = 0; a < libraryEntryCount; a++)
                {
                    PowerScrollLibraryEntry entry = new PowerScrollLibraryEntry();

                    entry.SkillName = (SkillName)reader.ReadInt();

                    entry.Value105 = reader.ReadInt();
                    entry.Value110 = reader.ReadInt();
                    entry.Value115 = reader.ReadInt();
                    entry.Value120 = reader.ReadInt();

                    m_LibraryEntries.Add(entry);
                }
            }

            //-------
            CheckSkillEntries();
        }
    }

    public class PowerScrollLibraryEntry
    {
        public SkillName SkillName = SkillName.Alchemy;
        public int Value105 = 0;
        public int Value110 = 0;
        public int Value115 = 0;
        public int Value120 = 0;
    }

    public class PowerScrollLibraryGump : Gump
    {
        PlayerMobile m_Player;
        PowerScrollLibrary m_Library;

        int m_PageNumber = 1;
        int m_TotalPages = 1;
        int m_TotalEntries = 1;

        int EntriesPerSide = 4;
        int EntriesPerPage = 8;

        int WhiteTextHue = 2499;

        public PowerScrollLibraryGump(PlayerMobile player, PowerScrollLibrary library, int pageNumber): base(10, 10)
        {
            if (player == null) return;
            if (library == null) return;
            if (library.Deleted) return;

            m_Player = player;
            m_Library = library;
            m_PageNumber = pageNumber;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddImage(205, 193, 11015, 2499);
            AddImage(204, 1, 11015, 2499);
            AddImage(3, 192, 11015, 2499);
            AddImage(3, 1, 11015, 2499);
            AddImage(302, 75, 2081, 2499);
            AddImage(300, 270, 2081, 2499);
            AddImage(301, 141, 2081, 2499);
            AddImage(301, 5, 2081, 2499);
            AddImage(301, 206, 2081, 2499);
            AddImage(299, 338, 2081, 2499);
            AddImage(44, 6, 2081, 2499);
            AddImage(44, 75, 2081, 2499);
            AddImage(43, 141, 2081, 2499);
            AddImage(43, 206, 2081, 2499);
            AddImage(41, 335, 2081);
            AddImage(43, 274, 2081, 2499);
            AddImageTiled(301, 2, 6, 405, 2701);
            AddImage(41, 338, 2081, 2499);            
            AddImage(49, 80, 3001, 2615);
            AddImage(56, 80, 3001, 2615);
            AddImage(306, 80, 3001, 2615);
            AddImage(315, 80, 3001, 2615);

            AddItem(153, 24, 5360, 1152);

            AddLabel(111, 5, 2590, "Power Scroll Library");

            AddLabel(88, 53, WhiteTextHue, "Add All in Backpack into Library");
            AddButton(65, 56, 2118, 2118, 1, GumpButtonType.Reply, 0);

            AddLabel(354, 5, 2615, "Locked Down Access Level");

            string accessName = "Owner";

            switch (m_Library.LockedDownAccessLevel)
            {
                case PowerScrollLibrary.LockedDownAccessLevelType.Owner: accessName = "Owner"; break;
                case PowerScrollLibrary.LockedDownAccessLevelType.CoOwner: accessName = "Co-Owner"; break;
                case PowerScrollLibrary.LockedDownAccessLevelType.Friend: accessName = "Friend"; break;
                case PowerScrollLibrary.LockedDownAccessLevelType.Anyone: accessName = "Anyone"; break;
            }

            AddLabel(Utility.CenteredTextOffset(435, accessName), 25, 2562, accessName);

            AddButton(366, 28, 2223, 2223, 2, GumpButtonType.Reply, 0);
            AddButton(488, 29, 2224, 2224, 3, GumpButtonType.Reply, 0);

            AddLabel(347, 53, WhiteTextHue, "Remove All Possible on Selection");
            if (m_Library.RemoveAllOnSelection)
                AddButton(313, 48, 2154, 2151, 4, GumpButtonType.Reply, 0);
            else
                AddButton(313, 48, 2151, 2154, 4, GumpButtonType.Reply, 0);  

            //-----

            m_TotalEntries = m_Library.m_LibraryEntries.Count;
            m_TotalPages = (int)(Math.Ceiling((double)m_TotalEntries / (double)EntriesPerPage));

            if (m_TotalPages == 0)
                m_TotalPages = 1;

            if (m_PageNumber < 1)
                m_PageNumber = 1;

            if (m_PageNumber > m_TotalPages)
                m_PageNumber = m_TotalPages;
            
            int startIndex = (m_PageNumber - 1) * EntriesPerPage;
            int endIndex = startIndex + EntriesPerPage;

            if (endIndex > m_TotalEntries)
                endIndex = m_TotalEntries;

            int leftStartY = 95;
            int rightStartY = 95;

            int entryCount = 0;

            for (int a = startIndex; a < endIndex; a++)
            {
                if (a < m_Library.m_LibraryEntries.Count)
                {
                    PowerScrollLibraryEntry entry = m_Library.m_LibraryEntries[a];

                    //Left Side
                    if (entryCount < EntriesPerSide)
                    {
                        string skillName = PowerScrollLibrary.GetSkillName(entry.SkillName);

                        AddLabel(Utility.CenteredTextOffset(180, skillName), leftStartY, 2590, skillName);

                        AddLabel(80, leftStartY + 20, 2615, "105");
                        AddButton(60, leftStartY + 43, 2118, 2118, (10 * entryCount) + 10, GumpButtonType.Reply, 0);
                        AddLabel(80, leftStartY + 40, WhiteTextHue, entry.Value105.ToString());

                        AddLabel(140, leftStartY + 20, 2615, "110");
                        AddButton(121, leftStartY + 43, 2118, 2118, (10 * entryCount) + 11, GumpButtonType.Reply, 0);
                        AddLabel(140, leftStartY + 40, WhiteTextHue, entry.Value110.ToString());

                        AddLabel(200, leftStartY + 20, 2615, "115");
                        AddButton(180, leftStartY + 43, 2118, 2118, (10 * entryCount) + 12, GumpButtonType.Reply, 0);
                        AddLabel(200, leftStartY + 40, WhiteTextHue, entry.Value115.ToString());

                        AddLabel(260, leftStartY + 20, 2615, "120");
                        AddButton(241, leftStartY + 43, 2118, 2118, (10 * entryCount) + 13, GumpButtonType.Reply, 0);
                        AddLabel(260, leftStartY + 40, WhiteTextHue, entry.Value120.ToString());  

                        leftStartY += 67;
                    }

                    //Right Side
                    else
                    {
                        string skillName = PowerScrollLibrary.GetSkillName(entry.SkillName);

                        AddLabel(Utility.CenteredTextOffset(435, skillName), rightStartY, 2590, skillName);

                        AddLabel(340, rightStartY + 20, 2615, "105");
                        AddButton(320, rightStartY + 43, 2118, 2118, (10 * entryCount) + 10, GumpButtonType.Reply, 0);
                        AddLabel(340, rightStartY + 40, WhiteTextHue, entry.Value105.ToString());

                        AddLabel(400, rightStartY + 20, 2615, "110");
                        AddButton(380, rightStartY + 43, 2118, 2118, (10 * entryCount) + 11, GumpButtonType.Reply, 0);
                        AddLabel(400, rightStartY + 40, WhiteTextHue, entry.Value110.ToString());

                        AddLabel(460, rightStartY + 20, 2615, "115");
                        AddButton(440, rightStartY + 43, 2118, 2118, (10 * entryCount) + 12, GumpButtonType.Reply, 0);
                        AddLabel(460, rightStartY + 40, WhiteTextHue, entry.Value115.ToString());

                        AddLabel(520, rightStartY + 20, 2615, "120");
                        AddButton(500, rightStartY + 43, 2118, 2118, (10 * entryCount) + 13, GumpButtonType.Reply, 0);
                        AddLabel(520, rightStartY + 40, WhiteTextHue, entry.Value120.ToString());

                        rightStartY += 67;
                    }

                    entryCount++;
                }
            }

            if (m_PageNumber > 1)
                AddButton(160, 380, 4014, 4016, 5, GumpButtonType.Reply, 0);

            if (m_PageNumber < m_TotalPages)
                AddButton(415, 380, 4005, 4007, 6, GumpButtonType.Reply, 0);    
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null) return;
            if (m_Player.Backpack == null) return;

            if (m_Library == null) return;
            if (m_Library.Deleted) return;

            if (!m_Library.CanUse(m_Player))
                return;
            
            m_TotalEntries = m_Library.m_LibraryEntries.Count;
            m_TotalPages = (int)(Math.Ceiling((double)m_TotalEntries / (double)EntriesPerPage));

            if (m_TotalPages == 0)
                m_TotalPages = 1;

            if (m_PageNumber < 1)
                m_PageNumber = 1;

            if (m_PageNumber > m_TotalPages)
                m_PageNumber = m_TotalPages;

            bool closeGump = true;

            //-----

            switch (info.ButtonID)
            {
                case 1:
                    //Add All From Backpack
                    m_Library.AddAllScrollsInPack(m_Player);

                    closeGump = false;
                break;

                case 2:
                    //Previous Access Level
                    if (m_Library.IsLockedDown && m_Player.AccessLevel == AccessLevel.Player)    
                        m_Player.SendMessage("You may not change the access level of this item while it is currently locked down.");

                    else
                    {
                        switch (m_Library.LockedDownAccessLevel)
                        {
                            case PowerScrollLibrary.LockedDownAccessLevelType.Owner: m_Library.LockedDownAccessLevel = PowerScrollLibrary.LockedDownAccessLevelType.Anyone; break;
                            case PowerScrollLibrary.LockedDownAccessLevelType.CoOwner: m_Library.LockedDownAccessLevel = PowerScrollLibrary.LockedDownAccessLevelType.Owner; break;
                            case PowerScrollLibrary.LockedDownAccessLevelType.Friend: m_Library.LockedDownAccessLevel = PowerScrollLibrary.LockedDownAccessLevelType.CoOwner; break;
                            case PowerScrollLibrary.LockedDownAccessLevelType.Anyone: m_Library.LockedDownAccessLevel = PowerScrollLibrary.LockedDownAccessLevelType.Friend; break;
                        }
                    }

                    closeGump = false;
                break;

                case 3:
                    //Next Access Level
                    if (m_Library.IsLockedDown && m_Player.AccessLevel == AccessLevel.Player)    
                        m_Player.SendMessage("You may not change the access level of this item while it is currently locked down.");

                    else
                    {
                        switch (m_Library.LockedDownAccessLevel)
                        {
                            case PowerScrollLibrary.LockedDownAccessLevelType.Owner: m_Library.LockedDownAccessLevel = PowerScrollLibrary.LockedDownAccessLevelType.CoOwner; break;
                            case PowerScrollLibrary.LockedDownAccessLevelType.CoOwner: m_Library.LockedDownAccessLevel = PowerScrollLibrary.LockedDownAccessLevelType.Friend; break;
                            case PowerScrollLibrary.LockedDownAccessLevelType.Friend: m_Library.LockedDownAccessLevel = PowerScrollLibrary.LockedDownAccessLevelType.Anyone; break;
                            case PowerScrollLibrary.LockedDownAccessLevelType.Anyone: m_Library.LockedDownAccessLevel = PowerScrollLibrary.LockedDownAccessLevelType.Owner; break;
                        }
                    }

                    closeGump = false;
                break;

                case 4:
                    //Remove All Possible on Selection
                    m_Library.RemoveAllOnSelection = !m_Library.RemoveAllOnSelection;

                    closeGump = false;
                break;

                case 5:
                    //Previous Page
                    if (m_PageNumber > 1)
                        m_PageNumber--;

                    m_Player.SendSound(m_Library.changeGumpSound);

                    closeGump = false;
                break;

                case 6:
                    //Next Page
                    if (m_PageNumber < m_TotalPages)
                        m_PageNumber++;

                    m_Player.SendSound(m_Library.changeGumpSound);

                    closeGump = false;
                break;
            }

            //Eject Items
            if (info.ButtonID >= 10)
            {
                int rootIndex = info.ButtonID - 10;
                int baseIndex = (int)(Math.Floor((double)rootIndex / 10));
                int remainder = info.ButtonID % 10;                
                int value = 0;

                switch (remainder)
                {
                    case 0: value = 105; break;
                    case 1: value = 110; break;
                    case 2: value = 115; break;
                    case 3: value = 120; break;                        
                }

                if (value == 0)
                    return;

                int index = ((m_PageNumber - 1) * EntriesPerPage) + baseIndex;

                if (index >= m_Library.m_LibraryEntries.Count || index < 0)
                    return;

                PowerScrollLibraryEntry entry = m_Library.m_LibraryEntries[index];

                if (entry == null)
                    return;

                bool removeAll = m_Library.RemoveAllOnSelection;

                m_Library.EjectScroll(m_Player, entry.SkillName, value, removeAll);

                closeGump = false;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(PowerScrollLibraryGump));
                m_Player.SendGump(new PowerScrollLibraryGump(m_Player, m_Library, m_PageNumber));
            }

            else
                m_Player.SendSound(m_Library.closeGumpSound);
        }
    }
}