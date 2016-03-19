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
using Server.Custom;

namespace Server.Items
{
    public class DungeonCoreLibrary : Item
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

        public List<DungeonCoreLibraryEntry> m_LibraryEntries = new List<DungeonCoreLibraryEntry>();

        public int openGumpSound = 0x055;
        public int changeGumpSound = 0x057;
        public int closeGumpSound = 0x058;
        public int addItemSound = 0x249;

        [Constructable]
        public DungeonCoreLibrary(): base(8793)
        {
            Name = "a dungeon core library";
            Hue = 1106;
            Weight = 5;

            CreateDungeonEntries();
        }

        public DungeonCoreLibrary(Serial serial): base(serial)
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

            from.CloseGump(typeof(DungeonCoreLibraryGump));
            from.SendGump(new DungeonCoreLibraryGump(player, this, 1));
        }        

        public void CreateDungeonEntries()
        {
            int dungeonCount = Enum.GetNames(typeof(BaseDungeonArmor.DungeonEnum)).Length;

            for (int a = 1; a < dungeonCount; a++)
            {
                DungeonCoreLibraryEntry entry = new DungeonCoreLibraryEntry();
                entry.Dungeon = (BaseDungeonArmor.DungeonEnum)a;

                m_LibraryEntries.Add(entry);
            }
        }

        public DungeonCoreLibraryEntry GetEntryDetail(BaseDungeonArmor.DungeonEnum dungeon)
        {
            DungeonCoreLibraryEntry targetEntry = null;

            foreach (DungeonCoreLibraryEntry entry in m_LibraryEntries)
            {
                if (entry.Dungeon == dungeon)
                    return entry;
            }

            return targetEntry;
        }

        public void AddAllDungeonCoresInPack(Mobile from)
        {
            if (from == null) return;
            if (from.Backpack == null) return;

            List<DungeonCore> m_DungeonCores = from.Backpack.FindItemsByType<DungeonCore>();

            int totalCount = 0;

            Queue m_Queue = new Queue();

            foreach (DungeonCore dungeonCore in m_DungeonCores)
            {
                m_Queue.Enqueue(dungeonCore);                
            }

            while (m_Queue.Count > 0)
            {
                DungeonCore dungeonCore = (DungeonCore)m_Queue.Dequeue();
                DungeonCoreLibraryEntry entry = GetEntryDetail(dungeonCore.Dungeon);

                if (entry == null)
                    continue;

                entry.Count++;                
                totalCount++;

                dungeonCore.Delete();
            }

            if (totalCount > 1)
            {
                from.SendMessage("You add " + totalCount.ToString() + " dungeon cores to the library.");
                from.SendSound(addItemSound);
            }

            else if (totalCount == 1)
            {
                from.SendMessage("You add a dungeon core to the library.");
                from.SendSound(addItemSound);
            }

            else
                from.SendMessage("You do not have any dungeon cores in your backpack.");
        }

        public void EjectDungeonCore(Mobile from, BaseDungeonArmor.DungeonEnum dungeon, bool removeAll)
        {
            if (from == null || dungeon == null)
                return;

            DungeonCoreLibraryEntry entry = GetEntryDetail(dungeon);

            if (entry == null)
                return;

            if (entry.Count == 0)
            {
                from.SendMessage("You do not have any of that dungeon core currently stored within this library.");
                return;
            }            

            if (from.Backpack == null)
                return;

            if (from.Backpack.TotalItems == from.Backpack.MaxItems)
            {
                from.SendMessage("Your backpack is at capacity. Please remove some items and try again.");
                return;
            }            

            if (removeAll)
            {
                int itemCount = 0;

                for (int a = 0; a < 1000; a++)
                {
                    if (entry.Count == 0)
                        break;

                    if (from.Backpack.TotalItems == from.Backpack.MaxItems)
                        break;

                    DungeonCore dungeonCore = (DungeonCore)Activator.CreateInstance(typeof(DungeonCore));
                    dungeonCore.Dungeon = dungeon;

                    if (dungeonCore != null)
                    {
                        entry.Count--;

                        from.Backpack.DropItem(dungeonCore);
                        itemCount++;
                    }
                }

                if (itemCount > 1)
                {
                    from.SendMessage("You retrieve " + itemCount.ToString() + " dungeon cores from the library.");
                    from.SendSound(addItemSound);
                }

                else if (itemCount == 1)
                {
                    from.SendMessage("You retrieve a dungeon core from the library.");
                    from.SendSound(addItemSound);
                }

                else
                    from.SendMessage("You do not have any dungeon cores of that in the library.");
            }

            else
            {

                DungeonCore dungeonCore = (DungeonCore)Activator.CreateInstance(typeof(DungeonCore));
                dungeonCore.Dungeon = dungeon;

                if (dungeonCore != null)
                {
                    entry.Count--;

                    from.Backpack.DropItem(dungeonCore);
                    from.SendMessage("You retrieve a dungeon core from the library.");
                    from.SendSound(addItemSound);
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

            if (from.Map != Map || !from.InRange(GetWorldLocation(), 2) )
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
                DungeonCoreLibraryEntry entry = m_LibraryEntries[a];

                writer.Write((int)entry.Dungeon);
                writer.Write(entry.Count);
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
                    DungeonCoreLibraryEntry entry = new DungeonCoreLibraryEntry();

                    entry.Dungeon = (BaseDungeonArmor.DungeonEnum)reader.ReadInt();
                    entry.Count = reader.ReadInt();

                    m_LibraryEntries.Add(entry);                    
                }
            }
        }
    }

    public class DungeonCoreLibraryEntry
    {
        public BaseDungeonArmor.DungeonEnum Dungeon = BaseDungeonArmor.DungeonEnum.Shame;
        public int Count = 0;
    }

    public class DungeonCoreLibraryGump : Gump
    {
        PlayerMobile m_Player;
        DungeonCoreLibrary m_Library; 

        int m_PageNumber = 1;
        int m_TotalPages = 1;
        int m_TotalEntries = 1;

        int EntriesPerSide = 8;
        int EntriesPerPage = 16;

        int WhiteTextHue = 2499; 

        public DungeonCoreLibraryGump(PlayerMobile player, DungeonCoreLibrary library, int pageNumber): base(10, 10)
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

            AddImage(205, 193, 11015, 2550);
            AddImage(204, 1, 11015, 2550);
            AddImage(3, 192, 11015, 2550);
            AddImage(3, 1, 11015, 2550);
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
            AddItem(152, 29, 8002, 2550);
            AddImage(49, 80, 3001, 2615);
            AddImage(56, 80, 3001, 2615);
            AddImage(306, 80, 3001, 2615);
            AddImage(315, 80, 3001, 2615);

            //TEST: FINISH AFTER THIS

            AddLabel(105, 5, 2599, "Dungeon Core Library");

            AddLabel(88, 53, WhiteTextHue, "Add All in Backpack into Library");
            AddButton(65, 56, 2118, 2118, 1, GumpButtonType.Reply, 0);            
         
            AddLabel(354, 5, 2615, "Locked Down Access Level");

            string accessName = "Owner";

            switch (m_Library.LockedDownAccessLevel)
            {
                case DungeonCoreLibrary.LockedDownAccessLevelType.Owner: accessName = "Owner"; break;
                case DungeonCoreLibrary.LockedDownAccessLevelType.CoOwner: accessName = "Co-Owner"; break;
                case DungeonCoreLibrary.LockedDownAccessLevelType.Friend: accessName = "Friend"; break;
                case DungeonCoreLibrary.LockedDownAccessLevelType.Anyone: accessName = "Anyone"; break;
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

            int leftStartY = 88;
            int rightStartY = 88;

            int entryCount = 0;

            for (int a = startIndex; a < endIndex; a++)
            {
                if (a < m_Library.m_LibraryEntries.Count)
                {
                    DungeonCoreLibraryEntry entry = m_Library.m_LibraryEntries[a];                    

                    //Left Side
                    if (entryCount < EntriesPerSide)
                    {
                        AddLabel(60, leftStartY, 2599, BaseDungeonArmor.GetDungeonName(entry.Dungeon));
                        AddLabel(174, leftStartY, 2615, "Count");
                        AddButton(231, leftStartY + 3, 2118, 2118, 10 + entryCount, GumpButtonType.Reply, 0);
                        AddLabel(249, leftStartY, WhiteTextHue, entry.Count.ToString());

                        leftStartY += 38;
                    }

                    //Right Side
                    else
                    {
                        AddLabel(317, rightStartY, 2599, BaseDungeonArmor.GetDungeonName(entry.Dungeon));
                        AddLabel(431, rightStartY, 2615, "Count");
                        AddButton(488, rightStartY + 3, 2118, 2118, 10 + entryCount, GumpButtonType.Reply, 0);
                        AddLabel(506, rightStartY, WhiteTextHue, entry.Count.ToString());                

                        rightStartY += 38;
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
                    m_Library.AddAllDungeonCoresInPack(m_Player);

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
                            case DungeonCoreLibrary.LockedDownAccessLevelType.Owner: m_Library.LockedDownAccessLevel = DungeonCoreLibrary.LockedDownAccessLevelType.Anyone; break;
                            case DungeonCoreLibrary.LockedDownAccessLevelType.CoOwner: m_Library.LockedDownAccessLevel = DungeonCoreLibrary.LockedDownAccessLevelType.Owner; break;
                            case DungeonCoreLibrary.LockedDownAccessLevelType.Friend: m_Library.LockedDownAccessLevel = DungeonCoreLibrary.LockedDownAccessLevelType.CoOwner; break;
                            case DungeonCoreLibrary.LockedDownAccessLevelType.Anyone: m_Library.LockedDownAccessLevel = DungeonCoreLibrary.LockedDownAccessLevelType.Friend; break;
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
                            case DungeonCoreLibrary.LockedDownAccessLevelType.Owner: m_Library.LockedDownAccessLevel = DungeonCoreLibrary.LockedDownAccessLevelType.CoOwner; break;
                            case DungeonCoreLibrary.LockedDownAccessLevelType.CoOwner: m_Library.LockedDownAccessLevel = DungeonCoreLibrary.LockedDownAccessLevelType.Friend; break;
                            case DungeonCoreLibrary.LockedDownAccessLevelType.Friend: m_Library.LockedDownAccessLevel = DungeonCoreLibrary.LockedDownAccessLevelType.Anyone; break;
                            case DungeonCoreLibrary.LockedDownAccessLevelType.Anyone: m_Library.LockedDownAccessLevel = DungeonCoreLibrary.LockedDownAccessLevelType.Owner; break;
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
                int index = ((m_PageNumber - 1) * EntriesPerPage) + (info.ButtonID - 10);
                
                if (index >= m_Library.m_LibraryEntries.Count || index < 0)
                    return;

                DungeonCoreLibraryEntry entry = m_Library.m_LibraryEntries[index];

                if (entry == null)
                    return;

                bool removeAll = m_Library.RemoveAllOnSelection;

                m_Library.EjectDungeonCore(m_Player, entry.Dungeon, removeAll);

                closeGump = false;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(DungeonCoreLibraryGump));
                m_Player.SendGump(new DungeonCoreLibraryGump(m_Player, m_Library, m_PageNumber));
            }

            else
                m_Player.SendSound(m_Library.closeGumpSound);
        }
    }
}