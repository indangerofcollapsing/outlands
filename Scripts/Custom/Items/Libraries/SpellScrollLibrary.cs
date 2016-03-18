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
    public class SpellScrollLibrary : Item
    {
        public enum LockedDownAccessLevelType
        {
            Owner,
            CoOwner,
            Friend,
            Anyone
        }

        public static Type[] SpellScrolls = new Type[]
        {
            #region Spells 

            //First
            typeof(ClumsyScroll),
            typeof(CreateFoodScroll),
            typeof(FeeblemindScroll),
            typeof(HealScroll),
            typeof(MagicArrowScroll),
            typeof(NightSightScroll),
            typeof(ReactiveArmorScroll),
            typeof(WeakenScroll),

            //Second
            typeof(AgilityScroll),
            typeof(CunningScroll),
            typeof(CureScroll),
            typeof(HarmScroll),
            typeof(MagicTrapScroll),
            typeof(ProtectionScroll),
            typeof(MagicUnTrapScroll),
            typeof(StrengthScroll),

            //Third
            typeof(BlessScroll),
            typeof(FireballScroll),
            typeof(MagicLockScroll),
            typeof(PoisonScroll),
            typeof(TelekinisisScroll),
            typeof(TeleportScroll),
            typeof(UnlockScroll),
            typeof(WallOfStoneScroll),

            //Fourth
            typeof(ArchCureScroll),
            typeof(ArchProtectionScroll),
            typeof(CurseScroll),
            typeof(FireFieldScroll),
            typeof(GreaterHealScroll),
            typeof(LightningScroll),
            typeof(ManaDrainScroll),
            typeof(RecallScroll),

            //Fifth
            typeof(BladeSpiritsScroll),
            typeof(DispelFieldScroll),
            typeof(IncognitoScroll),
            typeof(MagicReflectScroll),
            typeof(MindBlastScroll),
            typeof(ParalyzeScroll),
            typeof(PoisonFieldScroll),
            typeof(SummonCreatureScroll),

            //Sixth
            typeof(DispelScroll),
            typeof(EnergyBoltScroll),
            typeof(ExplosionScroll),
            typeof(InvisibilityScroll),
            typeof(MarkScroll),
            typeof(MassCurseScroll),
            typeof(ParalyzeFieldScroll),
            typeof(RevealScroll),

            //Seventh
            typeof(ChainLightningScroll),
            typeof(EnergyFieldScroll),
            typeof(FlamestrikeScroll),
            typeof(GateTravelScroll),
            typeof(ManaVampireScroll),
            typeof(MassDispelScroll),
            typeof(MeteorSwarmScroll),
            typeof(PolymorphScroll),

            //Eighth
            typeof(SummonAirElementalScroll),
            typeof(SummonEarthElementalScroll),
            typeof(EarthquakeScroll),
            typeof(EnergyVortexScroll),
            typeof(SummonFireElementalScroll),
            typeof(ResurrectionScroll),
            typeof(SummonDaemonScroll),
            typeof(SummonWaterElementalScroll),

            #endregion
        };

        public static string GetSpellName(Type type)
        {
            string name = "";

            #region Spells

            switch (type.Name)
            {             
                case "ClumsyScroll": name = "Clumsy"; break;
                case "CreateFoodScroll": name = "Create Food"; break;
                case "FeeblemindScroll": name = "Feeblemind"; break;
                case "HealScroll": name = "Heal"; break;
                case "MagicArrowScroll": name = "Magic Arrow"; break;
                case "NightSightScroll": name = "Night Sight"; break;
                case "ReactiveArmorScroll": name = "Reactive Armor"; break;
                case "WeakenScroll": name = "Weaken"; break;

                case "AgilityScroll": name = "Agility"; break;
                case "CunningScroll": name = "Cunning"; break;
                case "CureScroll": name = "Cure"; break;
                case "HarmScroll": name = "Harm"; break;
                case "MagicTrapScroll": name = "Magic Trap"; break;
                case "ProtectionScroll": name = "Protection"; break;
                case "MagicUnTrapScroll": name = "Unlock"; break;
                case "StrengthScroll": name = "Strength"; break;

                case "BlessScroll": name = "Bless"; break;
                case "FireballScroll": name = "Fireball"; break;
                case "MagicLockScroll": name = "Magic Lock"; break;
                case "PoisonScroll": name = "Poison"; break;
                case "TelekinisisScroll": name = "Telekinisis"; break;
                case "TeleportScroll": name = "Teleport"; break;
                case "UnlockScroll": name = "Unlock"; break;
                case "WallOfStoneScroll": name = "Wall of Stone"; break;

                case "ArchCureScroll": name = "Arch Cure"; break;
                case "ArchProtectionScroll": name = "Arch Protection"; break;
                case "CurseScroll": name = "Curse"; break;
                case "FireFieldScroll": name = "Fire Field"; break;
                case "GreaterHealScroll": name = "Greater Heal"; break;
                case "LightningScroll": name = "Lightning"; break;
                case "ManaDrainScroll": name = "Mana Drain"; break;
                case "RecallScroll": name = "Recall"; break;

                case "BladeSpiritsScroll": name = "Blade Spirits"; break;
                case "DispelFieldScroll": name = "Dispel Field"; break;
                case "IncognitoScroll": name = "Incognito"; break;
                case "MagicReflectScroll": name = "Magic Reflect"; break;
                case "MindBlastScroll": name = "Mind Blast"; break;
                case "ParalyzeScroll": name = "Paralyze"; break;
                case "PoisonFieldScroll": name = "Poison Field"; break;
                case "SummonCreatureScroll": name = "Summon Creature"; break;

                case "DispelScroll": name = "Blade Spirits"; break;
                case "EnergyBoltScroll": name = "Energy Bolt"; break;
                case "ExplosionScroll": name = "Explosion"; break;
                case "InvisibilityScroll": name = "Invisibility"; break;
                case "MarkScroll": name = "Mark"; break;
                case "MassCurseScroll": name = "Mass Curse"; break;
                case "ParalyzeFieldScroll": name = "Paralyze Field"; break;
                case "RevealScroll": name = "Reveal"; break;

                case "ChainLightningScroll": name = "Chain Lightning"; break;
                case "EnergyFieldScroll": name = "Energy Field"; break;
                case "FlamestrikeScroll": name = "Flamestrike"; break;
                case "GateTravelScroll": name = "Gate Travel"; break;
                case "ManaVampireScroll": name = "Mana Vampire"; break;
                case "MassDispelScroll": name = "Mass Dispel"; break;
                case "MeteorSwarmScroll": name = "Meteor Swarm"; break;
                case "PolymorphScroll": name = "Polymorph"; break;

                case "SummonAirElementalScroll": name = "Air Elemental"; break;
                case "SummonEarthElementalScroll": name = "Earth Elemental"; break;
                case "EarthquakeScroll": name = "Earthquake"; break;
                case "EnergyVortexScroll": name = "Energy Vortex"; break;
                case "SummonFireElementalScroll": name = "Fire Elemental"; break;
                case "ResurrectionScroll": name = "Resurrection"; break;
                case "SummonDaemonScroll": name = "Daemon"; break;
                case "SummonWaterElementalScroll": name = "Water Elemental"; break;
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

        public List<SpellScrollLibraryEntry> m_LibraryEntries = new List<SpellScrollLibraryEntry>();

        public int openGumpSound = 0x055;
        public int changeGumpSound = 0x057;
        public int closeGumpSound = 0x058;
        public int addItemSound = 0x249;

        [Constructable]
        public SpellScrollLibrary(): base(8793)
        {
            Name = "a spell scroll library";
            Hue = 2117;
            Weight = 5;

            CreateSpellEntries();
        }

        public SpellScrollLibrary(Serial serial): base(serial)
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
            
            from.CloseGump(typeof(SpellScrollLibraryGump));
            from.SendGump(new SpellScrollLibraryGump(player, this, 1));
        }        

        public void CreateSpellEntries()
        {
            for (int a = 0; a < SpellScrolls.Length; a++)
            {
                SpellScrollLibraryEntry entry = new SpellScrollLibraryEntry();
                entry.SpellType = SpellScrolls[a];

                m_LibraryEntries.Add(entry);
            }
        }

        public SpellScrollLibraryEntry GetEntryDetail(Type spellType)
        {
            SpellScrollLibraryEntry targetEntry = null;

            foreach (SpellScrollLibraryEntry entry in m_LibraryEntries)
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
                if (spellScroll.MasterStatus != 0)
                    continue;

                m_Queue.Enqueue(spellScroll);                
            }

            while (m_Queue.Count > 0)
            {
                SpellScroll spellScroll = (SpellScroll)m_Queue.Dequeue();
                SpellScrollLibraryEntry entry = GetEntryDetail(spellScroll.GetType());

                if (entry == null)
                    continue;

                entry.Count += spellScroll.Amount;

                totalCount += spellScroll.Amount;
                spellScroll.Delete();
            }

            if (totalCount > 1)
            {
                from.SendMessage("You add " + totalCount.ToString() + " spells scrolls to the library.");
                from.SendSound(addItemSound);
            }

            else if (totalCount == 1)
            {
                from.SendMessage("You add a spell scroll to the library.");
                from.SendSound(addItemSound);
            }

            else
                from.SendMessage("You do not have any spell scrolls in your backpack.");
        }

        public void EjectScroll(Mobile from, Type spellType, bool ejectAll)
        {
            if (from == null)
                return;

            SpellScrollLibraryEntry entry = GetEntryDetail(spellType);

            if (entry == null)
                return;

            if (entry.Count == 0)
            {
                from.SendMessage("You do not have any copies of that spell currently stored within this library.");
                return;
            }            

            if (from.Backpack == null)
                return;

            if (from.Backpack.TotalItems == from.Backpack.MaxItems)
            {
                from.SendMessage("Your backpack is at maximum capacity. Please remove some items and try again.");
                return;
            }
           
            if (ejectAll)
            {
                int amount = entry.Count;

                SpellScroll spellScroll = (SpellScroll)Activator.CreateInstance(entry.SpellType);

                if (spellScroll != null)
                {
                    spellScroll.Amount = amount;
                    entry.Count = 0;

                    from.Backpack.DropItem(spellScroll);
                }

                else
                    amount = 0;                

                if (amount > 1)
                {
                    from.SendMessage("You retrieve several spell scrolls from the library.");
                    from.SendSound(addItemSound);
                }

                else if (amount == 1)
                {
                    from.SendMessage("You retrieve a spell scroll from the library.");
                    from.SendSound(addItemSound);
                }

                else
                    from.SendMessage("You do not have any scrolls of that in the library.");
            }

            else
            {                
                SpellScroll spellScroll = (SpellScroll)Activator.CreateInstance(entry.SpellType);

                if (spellScroll != null)
                {
                    spellScroll.Amount = 1;
                    entry.Count--;

                    from.Backpack.DropItem(spellScroll);
                    from.SendSound(addItemSound);
                    from.SendMessage("You retrieve a spell scroll from the library.");                        
                }

                else
                    from.SendMessage("You do not have any scrolls of that in the library.");                
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
                SpellScrollLibraryEntry entry = m_LibraryEntries[a];

                writer.Write(entry.SpellType.ToString());

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
                    SpellScrollLibraryEntry entry = new SpellScrollLibraryEntry();

                    Type type = Type.GetType(reader.ReadString());
                    int count = reader.ReadInt();

                    if (type != null)
                    {
                        entry.SpellType = type;
                        entry.Count = count;

                        m_LibraryEntries.Add(entry);
                    }
                }
            }
        }
    }

    public class SpellScrollLibraryEntry
    {
        public Type SpellType = typeof(ChainLightningScroll);
        public int Count = 0;
    }

    public class SpellScrollLibraryGump : Gump
    {
        PlayerMobile m_Player;
        SpellScrollLibrary m_Library;

        int m_PageNumber = 1;
        int m_TotalPages = 1;
        int m_TotalEntries = 1;

        int EntriesPerSide = 8;
        int EntriesPerPage = 16;

        int WhiteTextHue = 2499;

        public SpellScrollLibraryGump(PlayerMobile player, SpellScrollLibrary library, int pageNumber): base(10, 10)
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

            AddImage(205, 193, 11015);
            AddImage(204, 1, 11015);
            AddImage(3, 192, 11015);
            AddImage(3, 1, 11015);
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
            AddItem(152, 29, 8002);
            AddImage(49, 80, 3001, 2615);
            AddImage(56, 80, 3001, 2615);
            AddImage(306, 80, 3001, 2615);
            AddImage(315, 80, 3001, 2615);

            AddLabel(115, 5, 2560, "Spell Scroll Library");

            AddLabel(88, 53, WhiteTextHue, "Add All in Backpack into Library");
            AddButton(65, 56, 2118, 2118, 1, GumpButtonType.Reply, 0);
            
            AddLabel(354, 5, 2615, "Locked Down Access Level");

            string accessName = "Owner";

            switch (m_Library.LockedDownAccessLevel)
            {
                case SpellScrollLibrary.LockedDownAccessLevelType.Owner: accessName = "Owner"; break;
                case SpellScrollLibrary.LockedDownAccessLevelType.CoOwner: accessName = "Co-Owner"; break;
                case SpellScrollLibrary.LockedDownAccessLevelType.Friend: accessName = "Friend"; break;
                case SpellScrollLibrary.LockedDownAccessLevelType.Anyone: accessName = "Anyone"; break;
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
                    SpellScrollLibraryEntry entry = m_Library.m_LibraryEntries[a];

                    //Left Side
                    if (entryCount < EntriesPerSide)
                    {
                        AddLabel(60, leftStartY, 2560, SpellScrollLibrary.GetSpellName(entry.SpellType));
                        AddButton(231, leftStartY + 3, 2118, 2118, 10 + entryCount, GumpButtonType.Reply, 0);
                        AddLabel(249, leftStartY, WhiteTextHue, entry.Count.ToString());

                        leftStartY += 38;
                    }

                    //Right Side
                    else
                    {
                        AddLabel(317, rightStartY, 2560, SpellScrollLibrary.GetSpellName(entry.SpellType));
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
                            case SpellScrollLibrary.LockedDownAccessLevelType.Owner: m_Library.LockedDownAccessLevel = SpellScrollLibrary.LockedDownAccessLevelType.Anyone; break;
                            case SpellScrollLibrary.LockedDownAccessLevelType.CoOwner: m_Library.LockedDownAccessLevel = SpellScrollLibrary.LockedDownAccessLevelType.Owner; break;
                            case SpellScrollLibrary.LockedDownAccessLevelType.Friend: m_Library.LockedDownAccessLevel = SpellScrollLibrary.LockedDownAccessLevelType.CoOwner; break;
                            case SpellScrollLibrary.LockedDownAccessLevelType.Anyone: m_Library.LockedDownAccessLevel = SpellScrollLibrary.LockedDownAccessLevelType.Friend; break;
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
                            case SpellScrollLibrary.LockedDownAccessLevelType.Owner: m_Library.LockedDownAccessLevel = SpellScrollLibrary.LockedDownAccessLevelType.CoOwner; break;
                            case SpellScrollLibrary.LockedDownAccessLevelType.CoOwner: m_Library.LockedDownAccessLevel = SpellScrollLibrary.LockedDownAccessLevelType.Friend; break;
                            case SpellScrollLibrary.LockedDownAccessLevelType.Friend: m_Library.LockedDownAccessLevel = SpellScrollLibrary.LockedDownAccessLevelType.Anyone; break;
                            case SpellScrollLibrary.LockedDownAccessLevelType.Anyone: m_Library.LockedDownAccessLevel = SpellScrollLibrary.LockedDownAccessLevelType.Owner; break;
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

                SpellScrollLibraryEntry entry = m_Library.m_LibraryEntries[index];

                if (entry == null)
                    return;

                bool removeAll = m_Library.RemoveAllOnSelection;

                m_Library.EjectScroll(m_Player, entry.SpellType, removeAll);

                closeGump = false;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(SpellScrollLibraryGump));
                m_Player.SendGump(new SpellScrollLibraryGump(m_Player, m_Library, m_PageNumber));
            }

            else
                m_Player.SendSound(m_Library.closeGumpSound);
        }
    }
}