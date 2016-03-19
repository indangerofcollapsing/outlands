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
    public class PrestigeScrollLibrary : Item
    {
        public enum LockedDownAccessLevelType
        {
            Owner,
            CoOwner,
            Friend,
            Anyone
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

        public List<PrestigeScrollLibraryEntry> m_LibraryEntries = new List<PrestigeScrollLibraryEntry>();

        public int openGumpSound = 0x055;
        public int changeGumpSound = 0x057;
        public int closeGumpSound = 0x058;
        public int addItemSound = 0x249;

        [Constructable]
        public PrestigeScrollLibrary(): base(8793)
        {
            Name = "a prestige scroll library";
            Hue = 2587;
            Weight = 5;

            CreateLocationEntries();
        }

        public PrestigeScrollLibrary(Serial serial): base(serial)
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
            
            from.CloseGump(typeof(PrestigeScrollLibraryGump));
            from.SendGump(new PrestigeScrollLibraryGump(player, this, 1));
        }        

        public void CreateLocationEntries()
        {
            for (int a = 0; a < InfluencePersistance.Regions.Length; a++)
            {
                PrestigeScrollLibraryEntry entry = new PrestigeScrollLibraryEntry();
                entry.regionName = InfluencePersistance.Regions[a];

                m_LibraryEntries.Add(entry);
            }
        }

        public PrestigeScrollLibraryEntry GetEntryDetail(IndexedRegionName regionName)
        {
            PrestigeScrollLibraryEntry targetEntry = null;

            foreach (PrestigeScrollLibraryEntry entry in m_LibraryEntries)
            {
                if (entry.regionName == regionName)
                    return entry;
            }

            return targetEntry;
        }

        public void AddAllScrollsInPack(Mobile from)
        {
            if (from == null) return;
            if (from.Backpack == null) return;

            List<PrestigeScroll> m_PrestigeScrolls = from.Backpack.FindItemsByType<PrestigeScroll>();

            int totalCount = 0;

            Queue m_Queue = new Queue();

            foreach (PrestigeScroll prestigeScroll in m_PrestigeScrolls)
            {
                m_Queue.Enqueue(prestigeScroll);                
            }

            while (m_Queue.Count > 0)
            {
                PrestigeScroll prestigeScroll = (PrestigeScroll)m_Queue.Dequeue();
                PrestigeScrollLibraryEntry entry = GetEntryDetail(prestigeScroll.Region);

                if (entry == null)
                    continue;

                switch ((int)prestigeScroll.InfluenceAmount)
                {
                    case 1: entry.Value1++; break;
                    case 2: entry.Value2++; break;
                    case 3: entry.Value3++; break;
                }

                totalCount++;
                prestigeScroll.Delete();
            }

            if (totalCount > 1)
            {
                from.SendMessage("You add " + totalCount.ToString() + " prestige scrolls to the library.");
                from.SendSound(addItemSound);
            }

            else if (totalCount == 1)
            {
                from.SendMessage("You add a prestige scroll to the library.");
                from.SendSound(addItemSound);
            }

            else
                from.SendMessage("You do not have any prestige scrolls in your backpack.");
        }

        public void EjectScroll(Mobile from, IndexedRegionName regionName, int value, bool removeAll)
        {
            if (from == null)
                return;

            PrestigeScrollLibraryEntry entry = GetEntryDetail(regionName);

            if (entry == null)
                return;

            switch (value)
            {
                case 1:
                    if (entry.Value1 == 0)
                    {
                        from.SendMessage("The are no prestige scrolls of that type currently stored within.");
                        return;
                    }
                break;

                case 2:
                    if (entry.Value2 == 0)
                    {
                        from.SendMessage("The are no prestige scrolls of that type currently stored within.");
                        return;
                    }
                break;

                case 3:
                    if (entry.Value3 == 0)
                    {
                        from.SendMessage("The are no prestige scrolls of that type currently stored within.");
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
                        case 1: if (entry.Value1 == 0) outOfScrolls = true; break;
                        case 2: if (entry.Value2 == 0) outOfScrolls = true; break;
                        case 3: if (entry.Value3 == 0) outOfScrolls = true; break;                       
                    }

                    if (from.Backpack.TotalItems == from.Backpack.MaxItems)
                        break;

                    if (outOfScrolls)
                        break;

                    PrestigeScroll prestigeScroll = new PrestigeScroll(regionName, value);

                    if (prestigeScroll != null)
                    {
                        switch (value)
                        {
                            case 1: entry.Value1--; break;
                            case 2: entry.Value2--; break;
                            case 3: entry.Value3--; break;
                        }

                        from.Backpack.DropItem(prestigeScroll);
                    }

                    scrollCount++;
                }

                if (scrollCount > 1)
                {
                    from.SendMessage("You retrieve " + scrollCount.ToString() + " prestige scrolls from the library.");
                    from.SendSound(addItemSound);
                }

                else if (scrollCount == 1)
                {
                    from.SendMessage("You retrieve a prestige scroll from the library.");
                    from.SendSound(addItemSound);
                }

                else
                    from.SendMessage("You do not have any scrolls of that value in the library.");
            }

            else
            {
                PrestigeScroll prestigeScroll = new PrestigeScroll(regionName, value);

                if (prestigeScroll != null)
                {


                    switch (value)
                    {
                        case 1: entry.Value1--; break;
                        case 2: entry.Value2--; break;
                        case 3: entry.Value3--; break;
                    }

                    from.Backpack.DropItem(prestigeScroll);
                    from.SendSound(addItemSound);
                    from.SendMessage("You retrieve a prestige scroll from the library.");
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
                PrestigeScrollLibraryEntry entry = m_LibraryEntries[a];

                writer.Write((int)entry.regionName);

                writer.Write(entry.Value1);
                writer.Write(entry.Value2);
                writer.Write(entry.Value3);
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
                    PrestigeScrollLibraryEntry entry = new PrestigeScrollLibraryEntry();

                    entry.regionName = (IndexedRegionName)reader.ReadInt();

                    entry.Value1 = reader.ReadInt();
                    entry.Value2 = reader.ReadInt();
                    entry.Value3 = reader.ReadInt();

                    m_LibraryEntries.Add(entry);
                }
            }
        }
    }

    public class PrestigeScrollLibraryEntry
    {
        public IndexedRegionName regionName = InfluencePersistance.Regions[0];

        public int Value1 = 0;
        public int Value2 = 0;
        public int Value3 = 0;
    }

    public class PrestigeScrollLibraryGump : Gump
    {
        PlayerMobile m_Player;
        PrestigeScrollLibrary m_Library;

        int m_PageNumber = 1;
        int m_TotalPages = 1;
        int m_TotalEntries = 1;

        int EntriesPerSide = 4;
        int EntriesPerPage = 8;

        int WhiteTextHue = 2499;

        public PrestigeScrollLibraryGump(PlayerMobile player, PrestigeScrollLibrary library, int pageNumber): base(10, 10)
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

            AddImage(205, 193, 11015, 2587);
            AddImage(204, 1, 11015, 2587);
            AddImage(3, 192, 11015, 2587);
            AddImage(3, 1, 11015, 2587);
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
            AddItem(152, 29, 8002, 2615);
            AddImage(49, 80, 3001, 2615);
            AddImage(56, 80, 3001, 2615);
            AddImage(306, 80, 3001, 2615);
            AddImage(315, 80, 3001, 2615);

            AddLabel(105, 5, 2615, "Prestige Scroll Library");

            AddLabel(88, 53, WhiteTextHue, "Add All in Backpack into Library");
            AddButton(65, 56, 2118, 2118, 1, GumpButtonType.Reply, 0);

            AddLabel(354, 5, 2615, "Locked Down Access Level");

            string accessName = "Owner";

            switch (m_Library.LockedDownAccessLevel)
            {
                case PrestigeScrollLibrary.LockedDownAccessLevelType.Owner: accessName = "Owner"; break;
                case PrestigeScrollLibrary.LockedDownAccessLevelType.CoOwner: accessName = "Co-Owner"; break;
                case PrestigeScrollLibrary.LockedDownAccessLevelType.Friend: accessName = "Friend"; break;
                case PrestigeScrollLibrary.LockedDownAccessLevelType.Anyone: accessName = "Anyone"; break;
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
                    PrestigeScrollLibraryEntry entry = m_Library.m_LibraryEntries[a];

                    //Left Side
                    if (entryCount < EntriesPerSide)
                    {
                        string regionName = InfluencePersistance.GetRegionName(entry.regionName);

                        AddLabel(Utility.CenteredTextOffset(180, regionName), leftStartY, 2615, regionName);

                        AddLabel(110, leftStartY + 20, 149, "1");
                        AddButton(90, leftStartY + 43, 2118, 2118, (10 * entryCount) + 10, GumpButtonType.Reply, 0);
                        AddLabel(110, leftStartY + 40, WhiteTextHue, entry.Value1.ToString());

                        AddLabel(170, leftStartY + 20, 149, "2");
                        AddButton(150, leftStartY + 43, 2118, 2118, (10 * entryCount) + 11, GumpButtonType.Reply, 0);
                        AddLabel(170, leftStartY + 40, WhiteTextHue, entry.Value2.ToString());

                        AddLabel(230, leftStartY + 20, 149, "3");
                        AddButton(210, leftStartY + 43, 2118, 2118, (10 * entryCount) + 12, GumpButtonType.Reply, 0);
                        AddLabel(230, leftStartY + 40, WhiteTextHue, entry.Value3.ToString());

                        leftStartY += 67;
                    }

                    //Right Side
                    else
                    {
                        string regionName = InfluencePersistance.GetRegionName(entry.regionName);

                        AddLabel(Utility.CenteredTextOffset(435, regionName), rightStartY, 2615, regionName);

                        AddLabel(370, rightStartY + 20, 149, "1");
                        AddButton(350, rightStartY + 43, 2118, 2118, (10 * entryCount) + 10, GumpButtonType.Reply, 0);
                        AddLabel(370, rightStartY + 40, WhiteTextHue, entry.Value1.ToString());

                        AddLabel(430, rightStartY + 20, 149, "2");
                        AddButton(410, rightStartY + 43, 2118, 2118, (10 * entryCount) + 11, GumpButtonType.Reply, 0);
                        AddLabel(430, rightStartY + 40, WhiteTextHue, entry.Value2.ToString());

                        AddLabel(490, rightStartY + 20, 149, "3");
                        AddButton(470, rightStartY + 43, 2118, 2118, (10 * entryCount) + 12, GumpButtonType.Reply, 0);
                        AddLabel(490, rightStartY + 40, WhiteTextHue, entry.Value3.ToString());

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
                            case PrestigeScrollLibrary.LockedDownAccessLevelType.Owner: m_Library.LockedDownAccessLevel = PrestigeScrollLibrary.LockedDownAccessLevelType.Anyone; break;
                            case PrestigeScrollLibrary.LockedDownAccessLevelType.CoOwner: m_Library.LockedDownAccessLevel = PrestigeScrollLibrary.LockedDownAccessLevelType.Owner; break;
                            case PrestigeScrollLibrary.LockedDownAccessLevelType.Friend: m_Library.LockedDownAccessLevel = PrestigeScrollLibrary.LockedDownAccessLevelType.CoOwner; break;
                            case PrestigeScrollLibrary.LockedDownAccessLevelType.Anyone: m_Library.LockedDownAccessLevel = PrestigeScrollLibrary.LockedDownAccessLevelType.Friend; break;
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
                            case PrestigeScrollLibrary.LockedDownAccessLevelType.Owner: m_Library.LockedDownAccessLevel = PrestigeScrollLibrary.LockedDownAccessLevelType.CoOwner; break;
                            case PrestigeScrollLibrary.LockedDownAccessLevelType.CoOwner: m_Library.LockedDownAccessLevel = PrestigeScrollLibrary.LockedDownAccessLevelType.Friend; break;
                            case PrestigeScrollLibrary.LockedDownAccessLevelType.Friend: m_Library.LockedDownAccessLevel = PrestigeScrollLibrary.LockedDownAccessLevelType.Anyone; break;
                            case PrestigeScrollLibrary.LockedDownAccessLevelType.Anyone: m_Library.LockedDownAccessLevel = PrestigeScrollLibrary.LockedDownAccessLevelType.Owner; break;
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
                    case 0: value = 1; break;
                    case 1: value = 2; break;
                    case 2: value = 3; break;
                }

                if (value == 0)
                    return;

                int index = ((m_PageNumber - 1) * EntriesPerPage) + baseIndex;

                if (index >= m_Library.m_LibraryEntries.Count || index < 0)
                    return;

                PrestigeScrollLibraryEntry entry = m_Library.m_LibraryEntries[index];

                if (entry == null)
                    return;

                bool removeAll = m_Library.RemoveAllOnSelection;

                m_Library.EjectScroll(m_Player, entry.regionName, value, removeAll);

                closeGump = false;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(PrestigeScrollLibraryGump));
                m_Player.SendGump(new PrestigeScrollLibraryGump(m_Player, m_Library, m_PageNumber));
            }

            else
                m_Player.SendSound(m_Library.closeGumpSound);
        }
    }
}