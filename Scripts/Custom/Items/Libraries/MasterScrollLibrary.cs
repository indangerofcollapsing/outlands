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
    public class MasterScrollLibrary : Item
    {
        public enum LockedDownAccessLevelType
        {
            Owner,
            CoOwner,
            Friend,
            Anyone
        }

        public static Type[] MasterScrolls = new Type[]
        {
            typeof(ChainLightningScroll),
            typeof(EnergyFieldScroll),
            typeof(FlamestrikeScroll),
            typeof(GateTravelScroll),
            typeof(ManaVampireScroll),
            typeof(MassDispelScroll),
            typeof(MeteorSwarmScroll),
            typeof(PolymorphScroll),

            typeof(SummonAirElementalScroll),
            typeof(SummonEarthElementalScroll),
            typeof(EarthquakeScroll),
            typeof(EnergyVortexScroll),
            typeof(SummonFireElementalScroll),           
            typeof(SummonDaemonScroll),
            typeof(SummonWaterElementalScroll),
            //typeof(ResurrectionScroll),
        };

        public static string GetSpellName(Type type)
        {
            string name = "";

            switch (type.Name)
            {
                case "ChainLightningScroll": name = "Chain Lightning"; break;
                case "EnergyFieldScroll": name = "Energy Field"; break;
                case "FlamestrikeScroll": name = "Flamestrike"; break;
                case "GateTravelScroll": name = "Gate Travel"; break;
                case "ManaVampireScroll": name = "Mana Vampire"; break;
                case "MassDispelScroll": name = "Mass Dispel"; break;
                case "MeteorSwarmScroll": name = "Meteor Swarm"; break;
                case "PolymorphScroll": name = "Polymoprh"; break;

                case "SummonAirElementalScroll": name = "Air Elemental"; break;
                case "SummonEarthElementalScroll": name = "Earth Elemental"; break;
                case "EarthquakeScroll": name = "Earthquake"; break;
                case "EnergyVortexScroll": name = "Energy Vortex"; break;
                case "SummonFireElementalScroll": name = "Fire Elemental"; break;
                case "SummonDaemonScroll": name = "Daemon"; break;
                case "SummonWaterElementalScroll": name = "Water Elemental"; break;
            }

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

        public List<MasterScrollLibraryEntry> m_LibraryEntries = new List<MasterScrollLibraryEntry>();

        public int openGumpSound = 0x055;
        public int changeGumpSound = 0x057;
        public int closeGumpSound = 0x058;
        public int addItemSound = 0x249;

        [Constructable]
        public MasterScrollLibrary(): base(8793)
        {
            Name = "a master scroll library";
            Hue = 2550;
            Weight = 5;

            CreateSpellEntries();
        }

        public MasterScrollLibrary(Serial serial): base(serial)
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

            from.CloseGump(typeof(MasterScrollLibraryGump));
            from.SendGump(new MasterScrollLibraryGump(player, this, 1));
        }        

        public void CreateSpellEntries()
        {
            for (int a = 0; a < MasterScrolls.Length; a++)
            {
                MasterScrollLibraryEntry entry = new MasterScrollLibraryEntry();
                entry.SpellType = MasterScrolls[a];

                m_LibraryEntries.Add(entry);
            }
        }

        public MasterScrollLibraryEntry GetEntryDetail(Type spellType)
        {
            MasterScrollLibraryEntry targetEntry = null;

            foreach (MasterScrollLibraryEntry entry in m_LibraryEntries)
            {
                if (entry.SpellType == spellType)
                    return entry;
            }

            return targetEntry;
        }

        public void AddAllScrollsInPack(Mobile from)
        {
            if (from == null) return;
            if (from.Backpack == null) return;            

            List<SpellScroll> m_MasterScrolls = from.Backpack.FindItemsByType<SpellScroll>();

            int totalCount = 0;

            Queue m_Queue = new Queue();

            foreach (SpellScroll spellScroll in m_MasterScrolls)
            {
                if (spellScroll.MasterStatus != 1)
                    continue;

                m_Queue.Enqueue(spellScroll);                
            }

            while (m_Queue.Count > 0)
            {
                SpellScroll spellScroll = (SpellScroll)m_Queue.Dequeue();
                MasterScrollLibraryEntry entry = GetEntryDetail(spellScroll.GetType());

                if (entry == null)
                    continue;

                entry.Charges += spellScroll.UsesRemaining;
                
                totalCount++;
                spellScroll.Delete();
            }

            if (totalCount > 1)
            {
                from.SendMessage("You add " + totalCount.ToString() + " master scrolls to the library.");
                from.SendSound(addItemSound);
            }

            else if (totalCount == 1)
            {
                from.SendMessage("You add a master scroll to the library.");
                from.SendSound(addItemSound);
            }

            else
                from.SendMessage("You do not have any master scrolls in your backpack.");
        }

        public void EjectScroll(Mobile from, Type spellType, bool removeAll)
        {
            if (from == null || spellType == null)
                return;

            MasterScrollLibraryEntry entry = GetEntryDetail(spellType);

            if (entry == null)
                return;

            if (entry.Charges == 0)
            {
                from.SendMessage("You do not have any charges of that spell currently stored within this library.");
                return;
            }            

            if (from.Backpack == null)
                return;

            if (from.Backpack.TotalItems == from.Backpack.MaxItems)
            {
                from.SendMessage("Your backpack is at capacity. Please remove some items and try again.");
                return;
            }

            int baseCharges = 50;

            if (removeAll)
            {
                int scrollCount = 0;

                for (int a = 0; a < 1000; a++)
                {
                    if (entry.Charges == 0)
                        break;

                    if (from.Backpack.TotalItems == from.Backpack.MaxItems)
                        break;

                    int charges = baseCharges;

                    if (entry.Charges < charges)
                        charges = entry.Charges;

                    SpellScroll spellScroll = (SpellScroll)Activator.CreateInstance(entry.SpellType);

                    if (spellScroll != null)
                    {
                        spellScroll.MasterStatus = 1;
                        spellScroll.UsesRemaining = charges;

                        entry.Charges -= charges;

                        from.Backpack.DropItem(spellScroll);
                        scrollCount++;
                    }
                }

                if (scrollCount > 1)
                {
                    from.SendMessage("You retrieve " + scrollCount.ToString() + " master scrolls from the library.");
                    from.SendSound(addItemSound);
                }

                else if (scrollCount == 1)
                {
                    from.SendMessage("You retrieve a master scroll from the library.");
                    from.SendSound(addItemSound);
                }

                else
                    from.SendMessage("You do not have any scrolls of that in the library.");
            }

            else
            {
                int charges = baseCharges;

                if (entry.Charges < charges)
                    charges = entry.Charges;

                SpellScroll spellScroll = (SpellScroll)Activator.CreateInstance(entry.SpellType);

                if (spellScroll != null)
                {
                    spellScroll.MasterStatus = 1;
                    spellScroll.UsesRemaining = charges;

                    entry.Charges -= charges;

                    from.Backpack.DropItem(spellScroll);
                    from.SendMessage("You retrieve a master scroll from the library.");
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
                MasterScrollLibraryEntry entry = m_LibraryEntries[a];

                writer.Write(entry.SpellType.ToString());

                writer.Write(entry.Charges);
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
                    MasterScrollLibraryEntry entry = new MasterScrollLibraryEntry();

                    Type type = Type.GetType(reader.ReadString());
                    int charges = reader.ReadInt();

                    if (type != null)
                    {
                        entry.SpellType = type;
                        entry.Charges = charges;

                        m_LibraryEntries.Add(entry);
                    }
                }
            }
        }
    }

    public class MasterScrollLibraryEntry
    {
        public Type SpellType = typeof(ChainLightningScroll);
        public int Charges = 0;
    }

    public class MasterScrollLibraryGump : Gump
    {
        PlayerMobile m_Player;
        MasterScrollLibrary m_Library; 

        int m_PageNumber = 1;
        int m_TotalPages = 1;
        int m_TotalEntries = 1;

        int EntriesPerSide = 8;
        int EntriesPerPage = 16;

        int WhiteTextHue = 2499; 

        public MasterScrollLibraryGump(PlayerMobile player, MasterScrollLibrary library, int pageNumber): base(10, 10)
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

            AddLabel(105, 5, 2599, "Master Scroll Library");

            AddLabel(88, 53, WhiteTextHue, "Add All in Backpack into Library");
            AddButton(65, 56, 2118, 2118, 1, GumpButtonType.Reply, 0);            
         
            AddLabel(354, 5, 2615, "Locked Down Access Level");

            string accessName = "Owner";

            switch (m_Library.LockedDownAccessLevel)
            {
                case MasterScrollLibrary.LockedDownAccessLevelType.Owner: accessName = "Owner"; break;
                case MasterScrollLibrary.LockedDownAccessLevelType.CoOwner: accessName = "Co-Owner"; break;
                case MasterScrollLibrary.LockedDownAccessLevelType.Friend: accessName = "Friend"; break;
                case MasterScrollLibrary.LockedDownAccessLevelType.Anyone: accessName = "Anyone"; break;
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
                    MasterScrollLibraryEntry entry = m_Library.m_LibraryEntries[a];                    

                    //Left Side
                    if (entryCount < EntriesPerSide)
                    {
                        AddLabel(60, leftStartY, 2599, MasterScrollLibrary.GetSpellName(entry.SpellType));
                        AddLabel(174, leftStartY, 2615, "Charges");
                        AddButton(231, leftStartY + 3, 2118, 2118, 10 + entryCount, GumpButtonType.Reply, 0);
                        AddLabel(249, leftStartY, WhiteTextHue, entry.Charges.ToString());

                        leftStartY += 38;
                    }

                    //Right Side
                    else
                    {
                        AddLabel(317, rightStartY, 2599, MasterScrollLibrary.GetSpellName(entry.SpellType));
                        AddLabel(431, rightStartY, 2615, "Charges");
                        AddButton(488, rightStartY + 3, 2118, 2118, 10 + entryCount, GumpButtonType.Reply, 0);
                        AddLabel(506, rightStartY, WhiteTextHue, entry.Charges.ToString());                

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
                            case MasterScrollLibrary.LockedDownAccessLevelType.Owner: m_Library.LockedDownAccessLevel = MasterScrollLibrary.LockedDownAccessLevelType.Anyone; break;
                            case MasterScrollLibrary.LockedDownAccessLevelType.CoOwner: m_Library.LockedDownAccessLevel = MasterScrollLibrary.LockedDownAccessLevelType.Owner; break;
                            case MasterScrollLibrary.LockedDownAccessLevelType.Friend: m_Library.LockedDownAccessLevel = MasterScrollLibrary.LockedDownAccessLevelType.CoOwner; break;
                            case MasterScrollLibrary.LockedDownAccessLevelType.Anyone: m_Library.LockedDownAccessLevel = MasterScrollLibrary.LockedDownAccessLevelType.Friend; break;
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
                            case MasterScrollLibrary.LockedDownAccessLevelType.Owner: m_Library.LockedDownAccessLevel = MasterScrollLibrary.LockedDownAccessLevelType.CoOwner; break;
                            case MasterScrollLibrary.LockedDownAccessLevelType.CoOwner: m_Library.LockedDownAccessLevel = MasterScrollLibrary.LockedDownAccessLevelType.Friend; break;
                            case MasterScrollLibrary.LockedDownAccessLevelType.Friend: m_Library.LockedDownAccessLevel = MasterScrollLibrary.LockedDownAccessLevelType.Anyone; break;
                            case MasterScrollLibrary.LockedDownAccessLevelType.Anyone: m_Library.LockedDownAccessLevel = MasterScrollLibrary.LockedDownAccessLevelType.Owner; break;
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

                MasterScrollLibraryEntry entry = m_Library.m_LibraryEntries[index];

                if (entry == null)
                    return;

                bool removeAll = m_Library.RemoveAllOnSelection;

                m_Library.EjectScroll(m_Player, entry.SpellType, removeAll);

                closeGump = false;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(MasterScrollLibraryGump));
                m_Player.SendGump(new MasterScrollLibraryGump(m_Player, m_Library, m_PageNumber));
            }

            else
                m_Player.SendSound(m_Library.closeGumpSound);
        }
    }
}