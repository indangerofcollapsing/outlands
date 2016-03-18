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
    public enum VengeanceGumpMode
    {
        PlayerOnly,
        PlayerSelected,
        PublicSelected
    }

    public enum VengeanceTimeRemaining
    {
        Short,
        Medium,
        Long
    }

    public static class Vengeance
    {
        public static int DeathPoints = 10;
        public static int AttackingPoints = 1;
        public static int CreatureDeathPointsPerControlSlot = 2;
        public static int PeacemakingPoints = 1;
        public static int ProvocationPoints = 1;
        public static int DiscordancePoints = 1;
        public static int StealingPoints = 1;

        public static List<VengeanceEntry> m_VengeanceEntries = new List<VengeanceEntry>();

        public static void Initialize()
        {
            Timer.DelayCall(TimeSpan.FromTicks(1), delegate
            {
                if (PersistanceItem == null)
                    PersistanceItem = new VengeancePersistance();
            });
        }

        public static VengeancePersistance PersistanceItem;

        public static bool HasVengeanceAgainstTarget(Mobile from, Mobile target)
        {
            if (from == null || target == null)
                return false;

            PlayerMobile pm_From = from as PlayerMobile;
            BaseCreature bc_From = from as BaseCreature;

            PlayerMobile pm_Target = target as PlayerMobile;
            BaseCreature bc_Target = target as BaseCreature;

            //Acting Creature is Controlled By a Player
            if (bc_From != null)
            {
                if (bc_From.Controlled && bc_From.ControlMaster is PlayerMobile)
                    pm_From = bc_From.ControlMaster as PlayerMobile;
            }

            //Target Creature is Controlled By a Player
            if (bc_Target != null)
            {
                if (bc_Target.Controlled && bc_Target.ControlMaster is PlayerMobile)
                    pm_Target = bc_Target.ControlMaster as PlayerMobile;
            }

            if (pm_From != null && pm_Target != null)
            {
                //Can Already Attack Target
                if (NotorietyHandlers.DetermineMobileNotoriety(pm_From, pm_Target, false) != Notoriety.Innocent)                
                    return false;                

                //Can Attack Target From Vengeance
                if (pm_From.FindVengeanceEntry(pm_Target))                
                    return true;                
            }

            return false;
        }  

        public static void AuditPlayerVengeanceEntries(PlayerMobile pm_Player)
        {
            if (pm_Player == null)
                return;

            List<VengeanceEntry> m_EntriesToDelete = new List<VengeanceEntry>();

            foreach (VengeanceEntry entry in pm_Player.m_VengeanceEntries)
            {
                bool deleteEntry = false;

                //Owner Doesn't Exist
                if (entry.m_Owner == null)
                    deleteEntry = true;

                else
                {
                    //Owner is Deleted
                    if (entry.m_Owner.Deleted)
                        deleteEntry = true;
                }

                //Offender Doesn't Exist
                if (entry.m_Offender == null)
                    deleteEntry = true;

                else
                {
                    //Offender is Deleted
                    if (entry.m_Offender.Deleted)
                        deleteEntry = true;
                }

                //No Points Remaining
                if (entry.m_PointsRemaining <= 0)
                    deleteEntry = true;

                //Contract Has Expired
                if (entry.m_CreationDate != DateTime.MinValue && entry.m_CreationDate + VengeanceEntry.ExpirationDuration <= DateTime.UtcNow)
                    deleteEntry = true;                

                if (deleteEntry)
                    m_EntriesToDelete.Add(entry);                
            }

            for (int a = 0; a < m_EntriesToDelete.Count; a++)
            {
                pm_Player.m_VengeanceEntries.Remove(m_EntriesToDelete[a]);
            }
        }

        public static void AuditPublicVengeanceEntries()
        {
            List<VengeanceEntry> m_EntriesToDelete = new List<VengeanceEntry>();

            foreach (VengeanceEntry entry in m_VengeanceEntries)
            {
                bool deleteEntry = false;

                PlayerMobile pm_Owner = entry.m_Owner as PlayerMobile;
                PlayerMobile pm_Offender = entry.m_Offender as PlayerMobile;

                //Owner Doesn't Exist
                if (pm_Owner == null)
                    deleteEntry = true;

                else
                {
                    //Owner is Deleted
                    if (pm_Owner.Deleted)
                        deleteEntry = true;
                }

                //Offender Doesn't Exist
                if (pm_Offender == null)
                    deleteEntry = true;

                else
                {
                    //Offender is Deleted
                    if (pm_Offender.Deleted)
                        deleteEntry = true;
                }  
              
                //No Points Remaining
                if (entry.m_PointsRemaining <= 0)
                    deleteEntry = true;

                //Contract Has Expired
                if (entry.m_CreationDate + VengeanceEntry.ExpirationDuration <= DateTime.UtcNow)
                    deleteEntry = true;

                if (deleteEntry)
                    m_EntriesToDelete.Add(entry);
            }

            for (int a = 0; a < m_EntriesToDelete.Count; a++)
            {
                m_VengeanceEntries.Remove(m_EntriesToDelete[a]);
            }
        }

        public static bool AddPublicVengeanceEntry(VengeanceEntry entryToAdd)
        {
            if (m_VengeanceEntries == null || entryToAdd == null)
                return false;

            if (entryToAdd.m_Owner == null || entryToAdd.m_Offender == null)
                return false;

            if (entryToAdd.m_Owner.Deleted || entryToAdd.m_Offender.Deleted)
                return false;

            VengeanceEntry targetEntry = null;

            foreach (VengeanceEntry entry in m_VengeanceEntries)
            {
                if (entry.m_Owner == entryToAdd.m_Owner && entry.m_Offender == entryToAdd.m_Offender)
                    targetEntry = entry;
            }

            //Entry Doesn't Exist Yet
            if (targetEntry == null)
            {
                m_VengeanceEntries.Add(entryToAdd);
                return true;
            }

            return false;
        }

        public static bool UpdatePublicVengeanceEntry(VengeanceEntry entryToUpdate, bool createNewEntry)
        {
            if (m_VengeanceEntries == null || entryToUpdate == null)
                return false;

            if (entryToUpdate.m_Owner == null || entryToUpdate.m_Offender == null)
                return false;

            if (entryToUpdate.m_Owner.Deleted || entryToUpdate.m_Offender.Deleted)
                return false;

            VengeanceEntry targetEntry = null;

            foreach (VengeanceEntry entry in m_VengeanceEntries)
            {
                if (entry.m_Owner == entryToUpdate.m_Owner && entry.m_Offender == entryToUpdate.m_Offender)
                    targetEntry = entry;
            }

            //New Entry
            if (targetEntry == null)
            {
                if (createNewEntry)
                    m_VengeanceEntries.Add(entryToUpdate);

                return true;
            }

            //Updating Old Entry
            else
            {
                targetEntry.m_Purchasable = entryToUpdate.m_Purchasable;
                targetEntry.m_GoldCost = entryToUpdate.m_GoldCost;
                targetEntry.m_Message = entryToUpdate.m_Message;

                targetEntry.m_CreationDate = entryToUpdate.m_CreationDate;
                targetEntry.m_PointsRemaining = entryToUpdate.m_PointsRemaining;

                return true;
            }

            return false;
        }

        public static bool RemovePublicVengeanceEntry(VengeanceEntry entryToRemove)
        {
            if (m_VengeanceEntries == null || entryToRemove == null)
                return false;

            if (entryToRemove.m_Owner == null || entryToRemove.m_Offender == null)
                return false;

            VengeanceEntry targetEntry = null;

            foreach (VengeanceEntry entry in m_VengeanceEntries)
            {
                if (entry.m_Owner == entryToRemove.m_Owner && entry.m_Offender == entryToRemove.m_Offender)
                    targetEntry = entry;
            }

            //Deleting Entry
            if (targetEntry != null)
            {
                m_VengeanceEntries.Remove(targetEntry);
                return true;
            }

            return false;
        }

        public static void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(1); //Version

            //Version 0
            writer.Write((int)m_VengeanceEntries.Count);   
         
            foreach (VengeanceEntry entry in m_VengeanceEntries)
            {
                writer.Write(entry.m_Owner);
                writer.Write(entry.m_Offender);

                writer.Write(entry.m_Purchasable);
                writer.Write(entry.m_GoldCost);
                writer.Write(entry.m_Message);

                //Version 1
                writer.Write(entry.m_CreationDate);
                writer.Write(entry.m_PointsRemaining);               
            }
        }

        public static void Deserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            if (version >= 0)
            {
                int entriesCount = reader.ReadInt();

                m_VengeanceEntries = new List<VengeanceEntry>();

                for (int i = 0; i < entriesCount; ++i)
                {
                    PlayerMobile pm_Owner = reader.ReadMobile() as PlayerMobile;
                    PlayerMobile pm_Offender = reader.ReadMobile() as PlayerMobile;

                    bool m_Purchasable = reader.ReadBool();
                    int m_GoldCost = reader.ReadInt();
                    string m_Message = reader.ReadString();

                    DateTime m_CreationDate = DateTime.UtcNow;
                    int m_PointsRemaining = VengeanceEntry.PointsRemainingDefault;

                    if (version >= 1)
                    {
                        m_CreationDate = reader.ReadDateTime();
                        m_PointsRemaining = reader.ReadInt();
                    }      
 
                    bool allowEntry = true;
                    bool forceCreationDate = false;
             
                    if (m_CreationDate == DateTime.MinValue)
                    {
                        m_CreationDate = DateTime.UtcNow;
                        forceCreationDate = true;
                    }                    

                    //Owner No Longer Exists
                    if (pm_Owner == null)
                        allowEntry = false;

                    else
                    {
                        //Owner is Deleted
                        if (pm_Owner.Deleted)
                            allowEntry = false;
                    }

                    //Offender No Longer Exists
                    if (pm_Offender == null)
                        allowEntry = false;

                    else
                    {
                        //Offender is Deleted
                        if (pm_Offender.Deleted)
                            allowEntry = false;
                    }
                    
                    //Contract Has Expired
                    if (m_CreationDate != DateTime.MinValue && m_CreationDate + VengeanceEntry.ExpirationDuration <= DateTime.UtcNow)
                        allowEntry = false;

                    if (allowEntry)
                    {
                        VengeanceEntry entry = new VengeanceEntry(pm_Owner, pm_Offender, m_Purchasable, m_GoldCost, m_Message, m_CreationDate, m_PointsRemaining);
                        
                        m_VengeanceEntries.Add(entry);

                        Timer.DelayCall(TimeSpan.FromMilliseconds(1000), delegate
                        {
                            if (entry != null && forceCreationDate)
                            {
                                if (entry.m_Purchasable && pm_Owner != null)                                
                                    pm_Owner.UpdateVengeanceEntry(entry, false);                                
                            }
                        }); 
                    }
                }
            }
        }
    }

    public class VengeancePersistance : Item
    {
        public override string DefaultName { get { return "Vengeance Persistance"; } }

        public VengeancePersistance(): base(0x0)
        {
            Movable = false;
        }

        public VengeancePersistance(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            Vengeance.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Vengeance.PersistanceItem = this;
            Movable = false;
            Vengeance.Deserialize(reader);
        }
    }

    public class VengeanceFilter
    {
        public string m_PlayerNameFilter;
        public string m_GuildAbbreviationFilter;

        public VengeanceTimeRemaining m_LengthFilter;       

        public string m_CostMinimum;
        public string m_CostMaxmimum;

        public bool m_ShowOnlyMyPublicEntries;
        public bool m_ShowMurderers;

        public static int ShortFilterDurationDays = 15;
        public static int MediumFilterDurationDays = 30;
        public static int LongFilterDurationDays = 45;

        public VengeanceFilter()
        {
            m_PlayerNameFilter = "";
            m_GuildAbbreviationFilter = "";

            m_LengthFilter = VengeanceTimeRemaining.Long;

            m_CostMinimum = "";
            m_CostMaxmimum = "";

            m_ShowOnlyMyPublicEntries = false;
            m_ShowMurderers = false;
        }
    }

    public class VengeanceEntry
    {
        public PlayerMobile m_Owner;
        public PlayerMobile m_Offender;

        public bool m_Purchasable = false;
        public int m_GoldCost = 0;
        public string m_Message = "";
                
        public static TimeSpan ExpirationDuration = TimeSpan.FromDays(45);

        public static int PointsRemainingDefault = 10;

        public DateTime m_CreationDate = DateTime.MinValue;
        public int m_PointsRemaining = PointsRemainingDefault;

        public static void Initialize()
        {
            CommandSystem.Register("Vengeance", AccessLevel.Player, new CommandEventHandler(PlayerVengeanceList));
            CommandSystem.Register("VengeanceList", AccessLevel.Player, new CommandEventHandler(PlayerVengeanceList));
            CommandSystem.Register("PublicVengeanceList", AccessLevel.Seer, new CommandEventHandler(PublicVengeanceList));
        }

        [Usage("VengeanceList")]
        [Description("Opens Player's Vengeance List")]
        public static void PlayerVengeanceList(CommandEventArgs arg)
        {
            PlayerMobile pm_Mobile = arg.Mobile as PlayerMobile;

            if (pm_Mobile == null)
                return;

            pm_Mobile.CloseGump(typeof(VengeanceGump));
            pm_Mobile.SendGump(new VengeanceGump(VengeanceGumpMode.PlayerOnly, arg.Mobile, new VengeanceFilter(), 0));
        }

        [Usage("PublicVengeanceList")]
        [Description("Opens The Public Vengeance List")]
        public static void PublicVengeanceList(CommandEventArgs arg)
        {
            PlayerMobile pm_Mobile = arg.Mobile as PlayerMobile;

            if (pm_Mobile == null)
                return;

            pm_Mobile.CloseGump(typeof(VengeanceGump));
            pm_Mobile.SendGump(new VengeanceGump(VengeanceGumpMode.PublicSelected, arg.Mobile, new VengeanceFilter(), 0));
        }

        public VengeanceEntry(PlayerMobile owner, PlayerMobile offender, bool purchasable, int goldCost, string message, DateTime creationDate, int pointsRemaining)
        {
            m_Owner = owner;
            m_Offender = offender;

            m_Purchasable = purchasable;
            m_GoldCost = goldCost;
            m_Message = message;

            m_CreationDate = creationDate;
            m_PointsRemaining = pointsRemaining;  
        }

        public class VengeanceGump : Server.Gumps.Gump
        {
            public VengeanceGumpMode m_VengeanceGumpMode;

            public Mobile m_From;
            public VengeanceFilter m_VengeanceFilter = new VengeanceFilter();
            public List<VengeanceEntry> m_FilteredVengeanceEntries = new List<VengeanceEntry>();

            public int m_Page = 0;

            public const int m_EntriesPerPage = 10;

            public VengeanceGump(VengeanceGumpMode vengeanceGumpMode, Mobile from, VengeanceFilter vengeanceFilter, int page): base(0, 0)
            {
                if (vengeanceGumpMode == null || from == null || vengeanceFilter == null)
                    return;

                m_VengeanceGumpMode = vengeanceGumpMode;

                m_From = from;
                m_VengeanceFilter = vengeanceFilter;
                m_Page = page;

                PlayerMobile pm_From = m_From as PlayerMobile;

                if (pm_From == null)
                    return;

                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;

                this.AddPage(0);

                this.AddImage(23, 44, 206);

                this.AddImageTiled(127, 147, 21, 21, 200);
                this.AddImageTiled(67, 85, 539, 424, 200);

                this.AddImage(66, 44, 201);
                this.AddImage(178, 44, 201);
                this.AddImage(605, 82, 203);
                this.AddImage(605, 465, 205);
                this.AddImage(605, 149, 203);
                this.AddImage(22, 465, 204);
                this.AddImage(604, 44, 207);
                this.AddImage(178, 465, 233);
                this.AddImage(66, 465, 233);
                this.AddImage(23, 88, 202);
                this.AddImage(23, 149, 202);
                this.AddImage(615, 77, 10441);

                this.AddBackground(192, 100, 423, 350, 9200);

                int textHue = 2036;

                int blueTextHue = 0x59;
                int redTextHue = 0x22;
                int greenTextHue = 0x3F;

                //FILTERS------------
                AddLabel(100, 75, textHue, @"Filter By");

                AddLabel(45, 95, textHue, @"Offender Name");
                AddImage(45, 115, 2501);
                AddTextEntry(53, 115, 127, 20, textHue, 3, m_VengeanceFilter.m_PlayerNameFilter);

                AddLabel(45, 135, textHue, @"Guild Abbreviation");
                AddImage(45, 155, 2501);
                AddTextEntry(53, 155, 127, 20, textHue, 4, m_VengeanceFilter.m_GuildAbbreviationFilter, 3);

                AddLabel(45, 175, textHue, @"Expires Within");

                AddGroup(0);

                AddRadio(45, 195, 2151, 2153, m_VengeanceFilter.m_LengthFilter == VengeanceTimeRemaining.Short, 5);
                AddLabel(80, 200, textHue, VengeanceFilter.ShortFilterDurationDays.ToString() + " Days");

                AddRadio(45, 225, 2151, 2153, m_VengeanceFilter.m_LengthFilter == VengeanceTimeRemaining.Medium, 6);
                AddLabel(80, 230, textHue, VengeanceFilter.MediumFilterDurationDays.ToString() + " Days");

                AddRadio(45, 255, 2151, 2153, m_VengeanceFilter.m_LengthFilter == VengeanceTimeRemaining.Long, 7);
                AddLabel(80, 260, textHue, VengeanceFilter.LongFilterDurationDays.ToString() + " Days");

                AddLabel(45, 290, textHue, @"Cost Minimum");
                AddImage(45, 310, 2501);
                AddTextEntry(53, 310, 127, 20, textHue, 8, m_VengeanceFilter.m_CostMinimum, 7);

                AddLabel(45, 330, textHue, @"Cost Maximum");
                AddImage(45, 350, 2501);
                AddTextEntry(53, 350, 127, 20, textHue, 9, m_VengeanceFilter.m_CostMaxmimum, 7);

                AddCheck(45, 375, 2151, 2153, m_VengeanceFilter.m_ShowOnlyMyPublicEntries, 10);
                AddLabel(80, 375, textHue, @"Show Only Entries");
                AddLabel(80, 395, textHue, @"Sold By Me");

                AddCheck(45, 420, 2151, 2153, m_VengeanceFilter.m_ShowMurderers, 11);
                AddLabel(80, 423, textHue, @"Show Murderers");

                AddButton(90, 455, 238, 240, 15, GumpButtonType.Reply, 0); //Apply  

                //Filter Vengeance List Entries----------------------
                bool usePublicEntries = true;

                if (m_VengeanceGumpMode == VengeanceGumpMode.PlayerOnly || m_VengeanceGumpMode == VengeanceGumpMode.PlayerSelected)
                    usePublicEntries = false;

                List<VengeanceEntry> m_TempVengeanceEntries;

                //Use Public Entries
                if (usePublicEntries)
                {
                    if (Vengeance.m_VengeanceEntries == null)
                        return;

                    Vengeance.AuditPublicVengeanceEntries();

                    m_TempVengeanceEntries = Vengeance.m_VengeanceEntries;

                    AddLabel(340, 75, textHue, @"Public Vengeance Listings");
                }

                //Use Only Player Entries
                else
                {
                    if (pm_From.m_VengeanceEntries == null)
                        return;

                    Vengeance.AuditPlayerVengeanceEntries(pm_From);

                    m_TempVengeanceEntries = pm_From.m_VengeanceEntries;

                    AddLabel(280, 75, textHue, @"Personal Vengeance Listings for " + pm_From.RawName);
                }

                foreach (VengeanceEntry entry in m_TempVengeanceEntries)
                {
                    bool addToFilter = true;

                    //Different Player Name
                    if (m_VengeanceFilter.m_PlayerNameFilter != "")
                    {
                        string offenderName = entry.m_Offender.RawName.ToLower();
                        string searchString = m_VengeanceFilter.m_PlayerNameFilter.ToLower();
                        int indexOf = offenderName.IndexOf(searchString);

                        if (indexOf == -1)
                            addToFilter = false;
                    }

                    //Different Guild Abbreviation
                    if (m_VengeanceFilter.m_GuildAbbreviationFilter != "")
                    {
                        if (entry.m_Offender.Guild != null)
                        {
                            string offenderGuildAbbreviationName = entry.m_Offender.Guild.Abbreviation.ToLower();
                            string searchString = m_VengeanceFilter.m_GuildAbbreviationFilter.ToLower();
                            int indexOf = offenderGuildAbbreviationName.IndexOf(searchString);

                            if (indexOf == -1)
                                addToFilter = false;
                        }

                        else
                            addToFilter = false;
                    }

                    DateTime contractExpiration = entry.m_CreationDate + VengeanceEntry.ExpirationDuration;

                    int daysRemaining = contractExpiration.Subtract(DateTime.UtcNow).Days;

                    if (contractExpiration <= DateTime.UtcNow)
                         addToFilter = false;

                    //Contract Expiration Filter
                    switch (m_VengeanceFilter.m_LengthFilter)
                    {
                        case VengeanceTimeRemaining.Short:
                            if (VengeanceFilter.ShortFilterDurationDays <= daysRemaining)
                                addToFilter = false;
                        break;

                        case VengeanceTimeRemaining.Medium:
                            if (VengeanceFilter.MediumFilterDurationDays <= daysRemaining)
                                addToFilter = false;
                        break;

                        case VengeanceTimeRemaining.Long:
                            if (VengeanceFilter.LongFilterDurationDays <= daysRemaining)
                                addToFilter = false;
                        break;
                    }

                    //Cost Minimum
                    if (m_VengeanceFilter.m_CostMinimum != "")
                    {
                        int costMinimum = 0;

                        try { costMinimum = Convert.ToInt32(m_VengeanceFilter.m_CostMinimum); }
                        catch (Exception e) { costMinimum = 0; }

                        if (entry.m_GoldCost < costMinimum)
                            addToFilter = false;
                    }

                    //Cost Maximum
                    if (m_VengeanceFilter.m_CostMaxmimum != "")
                    {
                        int costMaximum = 0;

                        try { costMaximum = Convert.ToInt32(m_VengeanceFilter.m_CostMaxmimum); }
                        catch (Exception e) { costMaximum = 0; }

                        if (entry.m_GoldCost > costMaximum)
                            addToFilter = false;
                    }

                    //Show Only Mine For Sale
                    if (m_VengeanceFilter.m_ShowOnlyMyPublicEntries)
                    {
                        if (entry.m_Owner != pm_From)
                            addToFilter = false;

                        if (!entry.m_Purchasable)
                            addToFilter = false;
                    }

                    //Show Murderers
                    if (!m_VengeanceFilter.m_ShowMurderers)
                    {
                        if (entry.m_Offender.Murderer)
                            addToFilter = false;
                    }

                    //Entry Still Valid After Applying Filters
                    if (addToFilter)
                        m_FilteredVengeanceEntries.Add(entry);
                }

                //Vengeance List Titles
                AddLabel(240, 115, 149, @"Offender Name");
                AddLabel(360, 115, 149, @"Guild ");
                AddLabel(420, 115, 149, @"Gold Accepted");
                AddLabel(520, 115, 149, @"Expires In");

                //Build Page Based on Entries and Page Index
                AddPage(1);

                int totalEntries = m_FilteredVengeanceEntries.Count;

                if (totalEntries <= 0)
                    return;

                //Decrease Page if 
                if ((m_Page * m_EntriesPerPage) > totalEntries)
                    m_Page = (int)(Math.Floor((double)totalEntries / 10));

                int startingEntryIndex = m_Page * m_EntriesPerPage;
                int endingEntryIndex;

                if (totalEntries > startingEntryIndex + m_EntriesPerPage)
                    endingEntryIndex = startingEntryIndex + m_EntriesPerPage;
                else
                    endingEntryIndex = totalEntries;

                int yStart = 145;               

                //Vengeance Entries
                for (int a = startingEntryIndex; a < endingEntryIndex; a++)
                {
                    VengeanceEntry entry = m_FilteredVengeanceEntries[a];

                    int textColor = textHue;

                    Account playerAccount = pm_From.Account as Account;
                    Account targetAccount = entry.m_Offender.Account as Account;

                    //Character is on Player's Account
                    if (playerAccount == targetAccount)
                        textColor = greenTextHue;

                    //Character is Murderer
                    else if (entry.m_Offender.Murderer)
                        textColor = redTextHue;
                    
                    else
                        textColor = textHue;

                    AddButton(200, yStart, 4029, 4031, 20 + a, GumpButtonType.Reply, 0); //Details Button
                    AddLabel(240, yStart, textColor, entry.m_Offender.RawName); //Player Name

                    if (entry.m_Offender.Guild != null)                    
                        AddLabel(360, yStart, textColor, entry.m_Offender.Guild.Abbreviation); //Guild Abbv.                    

                    int goldOffered = entry.m_GoldCost;
                    string strGoldOffered = goldOffered.ToString();

                    AddLabel(420, yStart, textColor, strGoldOffered); //Gold Offered

                    DateTime contractExpiration = entry.m_CreationDate + VengeanceEntry.ExpirationDuration;

                    int daysRemaining = contractExpiration.Subtract(DateTime.UtcNow).Days;
                    int hoursRemaining = contractExpiration.Subtract(DateTime.UtcNow).Hours;
                    int minutesRemaining = contractExpiration.Subtract(DateTime.UtcNow).Minutes;

                    if (daysRemaining < 0)
                        daysRemaining = 0;

                    if (hoursRemaining < 0)
                        hoursRemaining = 0;

                    if (minutesRemaining < 0)
                        minutesRemaining = 0;

                    string contractTimeRemaining = daysRemaining.ToString() + "d " + hoursRemaining.ToString() + "h " + minutesRemaining.ToString() + "m";

                    AddLabel(520, yStart, textColor, contractTimeRemaining); //Contract Expiration

                    yStart += 28;
                }

                if (m_Page > 0)
                {
                    //Previous Page
                    AddButton(275, 462, 9909, 9909, 16, GumpButtonType.Reply, 0);
                    AddLabel(305, 462, textHue, @"Previous Page");
                }

                if (totalEntries > endingEntryIndex)
                {
                    //Next Page
                    AddButton(500, 462, 9903, 9903, 17, GumpButtonType.Reply, 0);
                    AddLabel(430, 462, textHue, @"Next Page");
                }
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (sender.Mobile == null || info == null || m_VengeanceFilter == null || m_VengeanceGumpMode == null || m_Page == null)
                    return;

                PlayerMobile pm_From = sender.Mobile as PlayerMobile;

                if (pm_From == null)
                    return;

                string textPlayerName = "";
                string textGuildAbbreviation = "";
                string textCostMinimum = "";
                string textCostMaximum = "";

                //Player Name
                if (info.GetTextEntry(3) != null)
                {
                    TextRelay textRelayPlayerName = info.GetTextEntry(3);
                    textPlayerName = textRelayPlayerName.Text.Trim();
                }

                if (textPlayerName == null || textPlayerName == "")
                    m_VengeanceFilter.m_PlayerNameFilter = "";
                else
                    m_VengeanceFilter.m_PlayerNameFilter = textPlayerName;

                //Guild Abbreviation
                if (info.GetTextEntry(4) != null)
                {
                    TextRelay textRelayGuildAbbreviation = info.GetTextEntry(4);
                    textGuildAbbreviation = textRelayGuildAbbreviation.Text.Trim();
                }

                if (textGuildAbbreviation == null || textGuildAbbreviation == "")
                    m_VengeanceFilter.m_GuildAbbreviationFilter = "";
                else
                    m_VengeanceFilter.m_GuildAbbreviationFilter = textGuildAbbreviation;

                if (info.IsSwitched(5))
                    m_VengeanceFilter.m_LengthFilter = VengeanceTimeRemaining.Short;

                if (info.IsSwitched(6))
                    m_VengeanceFilter.m_LengthFilter = VengeanceTimeRemaining.Medium;

                if (info.IsSwitched(7))
                    m_VengeanceFilter.m_LengthFilter = VengeanceTimeRemaining.Long;

                //Cost Minimum
                if (info.GetTextEntry(8) != null)
                {
                    TextRelay textRelayCostMinimum = info.GetTextEntry(8);
                    textCostMinimum = textRelayCostMinimum.Text.Trim();
                }

                if (textCostMinimum == null || textCostMinimum == "")
                    m_VengeanceFilter.m_CostMinimum = "";
                else
                    m_VengeanceFilter.m_CostMinimum = textCostMinimum;

                //Cost Maximum
                if (info.GetTextEntry(9) != null)
                {
                    TextRelay textRelayCostMaximum = info.GetTextEntry(9);
                    textCostMaximum = textRelayCostMaximum.Text.Trim();
                }

                if (textCostMaximum == null || textCostMaximum == "")
                    m_VengeanceFilter.m_CostMaxmimum = "";
                else
                    m_VengeanceFilter.m_CostMaxmimum = textCostMaximum;

                //Show Only Mine for Sale
                m_VengeanceFilter.m_ShowOnlyMyPublicEntries = info.IsSwitched(10);

                //Show Murderers
                m_VengeanceFilter.m_ShowMurderers = info.IsSwitched(11);

                //Clicked a Vengeance Entry
                if (info.ButtonID >= 20 && m_FilteredVengeanceEntries != null)
                {
                    int arrayIndex = info.ButtonID - 20;

                    if (m_FilteredVengeanceEntries.Count <= arrayIndex)
                        return;

                    VengeanceEntry entry = m_FilteredVengeanceEntries[arrayIndex];

                    if (entry == null)
                        return;

                    //Public Vengeance List
                    if (m_VengeanceGumpMode == VengeanceGumpMode.PublicSelected)
                    {
                        if (entry.m_Owner != null)
                        {
                            //Contract Belongs to The Viewing Player
                            if (entry.m_Owner == pm_From)
                            {
                                pm_From.CloseGump(typeof(VengeanceGump));
                                pm_From.CloseGump(typeof(PlayerVengeanceEntryGump));

                                pm_From.SendGump(new VengeanceGump(m_VengeanceGumpMode, pm_From, m_VengeanceFilter, m_Page));
                                pm_From.SendGump(new PlayerVengeanceEntryGump(m_VengeanceGumpMode, pm_From, m_VengeanceFilter, entry, m_Page));
                            }

                            //Public Vengeance Entry
                            else
                            {
                                pm_From.CloseGump(typeof(VengeanceGump));
                                pm_From.CloseGump(typeof(PublicVengeanceEntryGump));

                                pm_From.SendGump(new VengeanceGump(m_VengeanceGumpMode, pm_From, m_VengeanceFilter, m_Page));
                                pm_From.SendGump(new PublicVengeanceEntryGump(m_VengeanceGumpMode, pm_From, m_VengeanceFilter, entry, m_Page));
                            }
                        }
                    }

                    //Player Vengeance List
                    else
                    {
                        pm_From.CloseGump(typeof(VengeanceGump));
                        pm_From.CloseGump(typeof(PlayerVengeanceEntryGump));

                        pm_From.SendGump(new VengeanceGump(m_VengeanceGumpMode, pm_From, m_VengeanceFilter, m_Page));
                        pm_From.SendGump(new PlayerVengeanceEntryGump(m_VengeanceGumpMode, pm_From, m_VengeanceFilter, entry, m_Page));
                    }
                }

                //Handle Button Press
                switch (info.ButtonID)
                {
                    //My Vengeance List
                    case 0:
                        break;

                    //Public Vengeance Contracts
                    case 1:
                        break;

                    //Restitution Fees
                    case 2:
                        break;

                    case 15: //Apply Filters: Resend Gump After Updating Filters
                        pm_From.CloseGump(typeof(VengeanceGump));
                        pm_From.SendGump(new VengeanceGump(m_VengeanceGumpMode, pm_From, m_VengeanceFilter, m_Page));
                        break;

                    case 16:
                        //Previous Page  
                        if (m_Page > 0)
                            m_Page--;

                        pm_From.CloseGump(typeof(VengeanceGump));
                        pm_From.SendGump(new VengeanceGump(m_VengeanceGumpMode, pm_From, m_VengeanceFilter, m_Page));
                        break;

                    case 17:
                        //Next Page                        
                        m_Page++;

                        pm_From.CloseGump(typeof(VengeanceGump));
                        pm_From.SendGump(new VengeanceGump(m_VengeanceGumpMode, pm_From, m_VengeanceFilter, m_Page));
                        break;
                }
            }
        }

        public class PlayerVengeanceEntryGump : Server.Gumps.Gump
        {
            VengeanceGumpMode m_VengeanceGumpMode;

            Mobile m_From;
            VengeanceFilter m_VengeanceFilter;
            VengeanceEntry m_VengeanceEntry;

            int m_Index;

            int initialGoldCost;
            bool initialPurchaseable;
            string initialMessage;
            int initialPointsRemaining;

            public PlayerVengeanceEntryGump(VengeanceGumpMode vengeanceGumpMode, Mobile from, VengeanceFilter vengeanceFilter, VengeanceEntry vengeanceEntry, int index): base(150, 75)
            {
                PlayerMobile pm_From = from as PlayerMobile;

                if (pm_From == null)
                    return;

                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;

                m_VengeanceGumpMode = vengeanceGumpMode;

                m_From = from;
                m_VengeanceFilter = vengeanceFilter;
                m_VengeanceEntry = vengeanceEntry;
                m_Index = index;

                AddPage(0);
                AddImage(8, 8, 1248);

                int textHue = 2036;

                int blueTextHue = 0x59;
                int redTextHue = 0x22;
                int greenTextHue = 0x3F;
                int messageTextHue = 159;

                AddLabel(100, 20, textHue, @"Vengeance Contract");

                //Owner
                Account playerAccount = pm_From.Account as Account;
                Account ownerAccount = vengeanceEntry.m_Owner.Account as Account;

                int ownerTextHue = textHue;

                if (playerAccount == ownerAccount)
                    ownerTextHue = greenTextHue;

                AddLabel(45, 55, textHue, @"Owner:");
                AddLabel(160, 55, ownerTextHue, vengeanceEntry.m_Owner.RawName);

                //Offender
                Account offenderAccount = vengeanceEntry.m_Offender.Account as Account;

                int offenderTextHue = textHue;

                if (playerAccount == offenderAccount)
                    offenderTextHue = greenTextHue;

                else if (vengeanceEntry.m_Offender.Murderer)
                    offenderTextHue = redTextHue;

                else
                    offenderTextHue = textHue;

                AddLabel(45, 75, textHue, @"Offender:");
                AddLabel(160, 75, offenderTextHue, vengeanceEntry.m_Offender.RawName);

                AddLabel(45, 95, textHue, @"Offender's Guild:");

                if (vengeanceEntry.m_Offender.Guild != null)
                    AddLabel(160, 95, textHue, vengeanceEntry.m_Offender.Guild.Abbreviation);
                else
                    AddLabel(160, 95, textHue, "None");

                AddLabel(45, 115, textHue, @"Expires In:");

                DateTime contractExpiration = vengeanceEntry.m_CreationDate + VengeanceEntry.ExpirationDuration;

                int daysRemaining = contractExpiration.Subtract(DateTime.UtcNow).Days;
                int hoursRemaining = contractExpiration.Subtract(DateTime.UtcNow).Hours;
                int minutesRemaining = contractExpiration.Subtract(DateTime.UtcNow).Minutes;

                if (daysRemaining < 0)
                    daysRemaining = 0;

                if (hoursRemaining < 0)
                    hoursRemaining = 0;

                if (minutesRemaining < 0)
                    minutesRemaining = 0;

                string contractTimeRemaining = daysRemaining.ToString() + "d " + hoursRemaining.ToString() + "h " + minutesRemaining.ToString() + "m";

                AddLabel(160, 115, textHue, contractTimeRemaining);

                AddLabel(45, 140, textHue, @"Points Remaining:");
                AddLabel(160, 140, textHue, vengeanceEntry.m_PointsRemaining.ToString());
                
                AddLabel(45, 165, textHue, @"Gold Accepted:");
                AddImage(137, 165, 2501);
                AddTextEntry(145, 165, 127, 20, textHue, 3, vengeanceEntry.m_GoldCost.ToString(), 7);

                AddLabel(45, 195, textHue, @"Make Available for Purchase:");
                AddCheck(235, 192, 9721, 9724, vengeanceEntry.m_Purchasable, 4);

                AddLabel(45, 220, textHue, @"Message:");
                AddTextEntry(45, 240, 235, 180, messageTextHue, 5, vengeanceEntry.m_Message, 200);

                AddLabel(76, 374, textHue, @"Confirm");
                AddButton(43, 370, 2151, 2152, 1, GumpButtonType.Reply, 0);

                AddLabel(185, 374, textHue, @"Delete Entry");
                AddButton(267, 371, 2472, 2472, 2, GumpButtonType.Reply, 0);

                initialGoldCost = vengeanceEntry.m_GoldCost;
                initialPurchaseable = vengeanceEntry.m_Purchasable;
                initialMessage = vengeanceEntry.m_Message;
                initialPointsRemaining = vengeanceEntry.m_PointsRemaining;                 
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                PlayerMobile pm_From = sender.Mobile as PlayerMobile;

                if (pm_From == null)
                    return;

                if (pm_From.m_VengeanceEntries == null)
                    return;

                if (m_VengeanceEntry == null)
                    return;

                if (m_VengeanceEntry.m_Owner == null)
                    return;

                int totalPurchasable = 0;
                bool detailsChanged = false;
                bool expired = false;

                VengeanceEntry playerEntry = null;
                Vengeance.AuditPlayerVengeanceEntries(pm_From);

                //Find Player's Version in their Player Vengeance Lise
                foreach (VengeanceEntry entry in pm_From.m_VengeanceEntries)
                {
                    //Found Correct Owner + Offender Match
                    if (entry.m_Owner == m_VengeanceEntry.m_Owner && entry.m_Offender == m_VengeanceEntry.m_Offender && entry.m_Offender != null)
                        playerEntry = entry;

                    if (entry.m_Purchasable)
                        totalPurchasable++;
                }

                //Can't Find Original Entry Anymore
                if (playerEntry == null)
                    detailsChanged = true;

                else
                {
                    if (playerEntry.m_CreationDate + VengeanceEntry.ExpirationDuration <= DateTime.UtcNow)
                    {
                        detailsChanged = true;
                        expired = true;
                    }

                    if (playerEntry.m_Offender.Deleted)
                        detailsChanged = true;

                    if (playerEntry.m_GoldCost != initialGoldCost)
                        detailsChanged = true;

                    if (playerEntry.m_Purchasable != initialPurchaseable)
                        detailsChanged = true;

                    if (playerEntry.m_Message != initialMessage)
                        detailsChanged = true;

                    if (playerEntry.m_PointsRemaining != initialPointsRemaining)
                        detailsChanged = true;                    
                }

                //If Any Details Have Changed
                if (detailsChanged == true)
                {
                    if (expired)
                        sender.Mobile.SendMessage("This vengeance contract has expired.");
                    else
                        sender.Mobile.SendMessage("Details of this vengeance contract have changed since initial viewing.");

                    sender.Mobile.CloseGump(typeof(PlayerVengeanceEntryGump));
                    sender.Mobile.CloseGump(typeof(VengeanceGump));
                    sender.Mobile.SendGump(new VengeanceGump(m_VengeanceGumpMode, sender.Mobile, m_VengeanceFilter, m_Index));

                    return;
                }

                //Public Entry
                VengeanceEntry publicEntry = null;
                Vengeance.AuditPublicVengeanceEntries();

                //Find Public Version of Entry in Public List
                foreach (VengeanceEntry entry in Vengeance.m_VengeanceEntries)
                {
                    //Found Correct Owner + Offender Match
                    if (entry.m_Owner == m_VengeanceEntry.m_Owner && entry.m_Offender == m_VengeanceEntry.m_Offender && entry.m_Offender != null)
                        publicEntry = entry;
                }

                switch (info.ButtonID)
                {
                    //Confirm
                    case 1:
                        bool goldValid = true;
                        int goldAccepted = 0;

                        TextRelay textRelayGoldAccepted = info.GetTextEntry(3);
                        string textGoldAccepted = textRelayGoldAccepted.Text.Trim();

                        try { goldAccepted = Convert.ToInt32(textGoldAccepted); }

                        catch (Exception e)
                        {
                            goldAccepted = 0;
                            sender.Mobile.SendMessage("That is not a valid gold amount.");
                            goldValid = false;
                        }

                        if (!goldValid || goldAccepted < 0)
                            return;

                        //Message
                        TextRelay textRelayMessage = info.GetTextEntry(5);
                        string textMessage = textRelayMessage.Text.Trim();

                        if (textMessage == null)
                        {
                            sender.Mobile.SendMessage("That is not valid text for the message.");
                            return;
                        }

                        //Attempting to Make 6th Vengeance Contract Purchasable
                        if (totalPurchasable >= 5)
                        {
                            if (playerEntry.m_Purchasable == false && info.IsSwitched(4))
                            {
                                sender.Mobile.SendMessage("You may only have 5 vengeance contracts available for purchase. You must deactivate another before making this contract purchasable.");
                                return;
                            }
                        }

                        int goldCost = goldAccepted;
                        bool purchasable = info.IsSwitched(4);                       

                        if (!purchasable)
                            goldCost = 0;

                        //Update Root Entry in Player's Vengeance List
                        playerEntry.m_GoldCost = goldCost;     
                        playerEntry.m_Purchasable = purchasable;
                        playerEntry.m_Message = textMessage;

                        //No Match In Public List Found
                        if (publicEntry == null)
                        {
                            //Need New Entry
                            if (playerEntry.m_Purchasable)
                                Vengeance.AddPublicVengeanceEntry(playerEntry);
                        }

                        //Found Match in Public List
                        else
                        {
                            //Need to Remove from Public List
                            if (!playerEntry.m_Purchasable)
                                Vengeance.RemovePublicVengeanceEntry(publicEntry);

                            //Update Public Entry Info
                            else
                                Vengeance.UpdatePublicVengeanceEntry(publicEntry, true);
                        }

                        pm_From.SendMessage("You update the vengeance entry.");

                        pm_From.CloseGump(typeof(VengeanceGump));
                        pm_From.CloseGump(typeof(PlayerVengeanceEntryGump));

                        pm_From.SendGump(new VengeanceGump(m_VengeanceGumpMode, pm_From, m_VengeanceFilter, m_Index));

                    break;

                    //Delete Entry
                    case 2:
                        m_VengeanceEntry.m_Owner.RemoveVengeanceEntry(playerEntry.m_Offender);
                        Vengeance.RemovePublicVengeanceEntry(publicEntry);

                        pm_From.SendMessage("You delete the vengeance entry.");

                        pm_From.CloseGump(typeof(PlayerVengeanceEntryGump));
                        pm_From.CloseGump(typeof(VengeanceGump));
                        pm_From.SendGump(new VengeanceGump(m_VengeanceGumpMode, pm_From, m_VengeanceFilter, m_Index));
                    break;
                }
            }
        }

        public class PublicVengeanceEntryGump : Server.Gumps.Gump
        {
            VengeanceGumpMode m_VengeanceGumpMode;

            Mobile m_From;
            VengeanceFilter m_VengeanceFilter;
            VengeanceEntry m_VengeanceEntry;

            int page;

            int initialGoldCost;
            bool initialPurchaseable;
            string initialMessage;
            int initialPointsRemaining;

            public PublicVengeanceEntryGump(VengeanceGumpMode vengeanceGumpMode, Mobile from, VengeanceFilter vengeanceFilter, VengeanceEntry vengeanceEntry, int page): base(150, 75)
            {
                if (vengeanceGumpMode == null || from == null || vengeanceFilter == null || vengeanceEntry == null)
                    return;

                PlayerMobile pm_From = from as PlayerMobile;

                if (pm_From == null)
                    return;

                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;

                m_VengeanceGumpMode = vengeanceGumpMode;

                m_From = from;
                m_VengeanceFilter = vengeanceFilter;
                m_VengeanceEntry = vengeanceEntry;
                page = page;

                AddPage(0);
                AddImage(8, 8, 1248);

                int textHue = 2036;

                int blueTextHue = 0x59;
                int redTextHue = 0x22;
                int greenTextHue = 0x3F;
                int messageTextHue = 159;

                AddLabel(100, 20, textHue, @"Vengeance Contract");

                Account playerAccount = pm_From.Account as Account;
                Account ownerAccount = vengeanceEntry.m_Owner.Account as Account;

                int ownerTextHue = textHue;

                if (playerAccount == ownerAccount)
                    ownerTextHue = greenTextHue;

                AddLabel(100, 20, textHue, @"Vengeance Contract");

                AddLabel(45, 55, textHue, @"Owner:");
                AddLabel(160, 55, ownerTextHue, vengeanceEntry.m_Owner.RawName);

                Account offenderAccount = vengeanceEntry.m_Offender.Account as Account;

                int offenderTextHue = textHue;

                if (playerAccount == offenderAccount)
                    offenderTextHue = greenTextHue;

                else if (vengeanceEntry.m_Offender.Murderer)
                    offenderTextHue = redTextHue;

                else
                    offenderTextHue = textHue;

                AddLabel(45, 75, textHue, @"Offender:");
                AddLabel(160, 75, offenderTextHue, vengeanceEntry.m_Offender.RawName);

                AddLabel(45, 95, textHue, @"Offender's Guild:");

                if (vengeanceEntry.m_Offender.Guild != null)
                    AddLabel(160, 95, textHue, vengeanceEntry.m_Offender.Guild.Abbreviation);
                else
                    AddLabel(160, 95, textHue, "None");

                AddLabel(45, 115, textHue, @"Expires In:");

                DateTime contractExpiration = vengeanceEntry.m_CreationDate + VengeanceEntry.ExpirationDuration;

                int daysRemaining = contractExpiration.Subtract(DateTime.UtcNow).Days;
                int hoursRemaining = contractExpiration.Subtract(DateTime.UtcNow).Hours;
                int minutesRemaining = contractExpiration.Subtract(DateTime.UtcNow).Minutes;

                if (daysRemaining < 0)
                    daysRemaining = 0;

                if (hoursRemaining < 0)
                    hoursRemaining = 0;

                if (minutesRemaining < 0)
                    minutesRemaining = 0;

                string contractTimeRemaining = daysRemaining.ToString() + "d " + hoursRemaining.ToString() + "h " + minutesRemaining.ToString() + "m";

                AddLabel(160, 115, textHue, contractTimeRemaining);

                AddLabel(45, 140, textHue, @"Points Remaining:");
                AddLabel(160, 140, textHue, vengeanceEntry.m_PointsRemaining.ToString());

                AddLabel(45, 165, textHue, @"Gold Cost:");
                AddLabel(160, 165, textHue, vengeanceEntry.m_GoldCost.ToString());

                AddLabel(45, 195, textHue, @"Message:");

                AddHtml(45, 215, 235, 180, vengeanceEntry.m_Message, false, false);

                AddLabel(76, 374, textHue, @"Purchase");
                AddButton(43, 370, 2151, 2152, 1, GumpButtonType.Reply, 0);   
             
                initialGoldCost = vengeanceEntry.m_GoldCost;
                initialPurchaseable = vengeanceEntry.m_Purchasable;
                initialMessage = m_VengeanceEntry.m_Message;
                initialPointsRemaining = m_VengeanceEntry.m_PointsRemaining;
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                PlayerMobile pm_From = sender.Mobile as PlayerMobile;

                if (pm_From == null) return;
                if (pm_From.m_VengeanceEntries == null) return;
                if (m_VengeanceEntry == null) return;
                if (m_VengeanceEntry.m_Owner == null) return;

                bool detailsChanged = false;
                bool expired = false;

                VengeanceEntry masterEntry = null;
                Vengeance.AuditPublicVengeanceEntries();

                //Find Public Version of Entry in Public Vengeance List
                foreach (VengeanceEntry entry in Vengeance.m_VengeanceEntries)
                {
                    //Found Correct Owner + Offender Match
                    if (entry.m_Owner == m_VengeanceEntry.m_Owner && entry.m_Offender == m_VengeanceEntry.m_Offender && entry.m_Offender != null)
                    {
                        masterEntry = entry;
                        break;
                    }
                }

                //Can't Find Public Entry Anymore
                if (masterEntry == null)
                    detailsChanged = true;

                else
                {
                    //Owner No Longer Exists
                    if (masterEntry.m_Owner == null)
                        detailsChanged = true;

                    else
                    {
                        //Owner is Deleted
                        if (masterEntry.m_Owner.Deleted)
                            detailsChanged = true;
                    }

                    //Offender No Longer Exists
                    if (masterEntry.m_Offender == null)
                        detailsChanged = true;

                    else
                    {
                        //Offender is Deleted
                        if (masterEntry.m_Offender.Deleted)
                            detailsChanged = true;
                    }

                    //Contract Has Expired
                    if (masterEntry.m_CreationDate + VengeanceEntry.ExpirationDuration <= DateTime.UtcNow)
                    {
                        detailsChanged = true;
                        expired = true;
                    }

                    if (masterEntry.m_GoldCost != initialGoldCost)
                        detailsChanged = true;

                    if (masterEntry.m_Purchasable != initialPurchaseable)
                        detailsChanged = true;

                    if (masterEntry.m_Message != initialMessage)
                        detailsChanged = true;

                    if (masterEntry.m_PointsRemaining != initialPointsRemaining)
                        detailsChanged = true;
                }

                VengeanceEntry playerEntry = null;

                if (!detailsChanged)
                {
                    //Find Player-Owned Version of Entry in Player's Vengeance List
                    foreach (VengeanceEntry entry in m_VengeanceEntry.m_Owner.m_VengeanceEntries)
                    {
                        //Found Correct Owner + Offender Match
                        if (entry.m_Owner == m_VengeanceEntry.m_Owner && entry.m_Offender == m_VengeanceEntry.m_Offender && entry.m_Owner != null && entry.m_Offender != null)
                        {
                            playerEntry = entry;
                            break;
                        }
                    }

                    if (playerEntry == null)
                        detailsChanged = true;
                }

                switch (info.ButtonID)
                {
                    //Purchase
                    case 1:
                        //If Any Details Have Changed
                        if (detailsChanged == true)
                        {
                            if (expired)
                                pm_From.SendMessage("This vengeance contract has expired.");
                            else
                                pm_From.SendMessage("Details of this vengeance contract have changed since initial viewing.");

                            pm_From.CloseGump(typeof(PublicVengeanceEntryGump));
                            pm_From.CloseGump(typeof(VengeanceGump));
                            pm_From.SendGump(new VengeanceGump(m_VengeanceGumpMode, sender.Mobile, m_VengeanceFilter, page));

                            return;
                        }

                        //See if Player Already Has A Contract for This Offender
                        if (pm_From.FindVengeanceEntry(m_VengeanceEntry.m_Offender))
                        {
                            pm_From.SendMessage("You already have a vengeance contract against the offender.");

                            pm_From.CloseGump(typeof(PublicVengeanceEntryGump));
                            pm_From.CloseGump(typeof(VengeanceGump));
                            pm_From.SendGump(new VengeanceGump(m_VengeanceGumpMode, sender.Mobile, m_VengeanceFilter, page));

                            return;
                        }

                        bool hasEnoughGold = Banker.GetBalance(pm_From) >= m_VengeanceEntry.m_GoldCost;

                        if (hasEnoughGold)
                        {
                            Banker.Withdraw(pm_From, m_VengeanceEntry.m_GoldCost);

                            if (m_VengeanceEntry.m_Owner.BankBox != null)
                                m_VengeanceEntry.m_Owner.BankBox.AddItem(new Gold(m_VengeanceEntry.m_GoldCost));

                            m_VengeanceEntry.m_Owner.SendMessage(pm_From.RawName + " has purchased your vengeance contract against " + m_VengeanceEntry.m_Offender.RawName + " for " + m_VengeanceEntry.m_GoldCost.ToString() + " gold.");

                            //Remove Owner's Entry
                            m_VengeanceEntry.m_Owner.RemoveVengeanceEntry(playerEntry.m_Offender);

                            //Remove Public Entry
                            Vengeance.RemovePublicVengeanceEntry(masterEntry);

                            //Player is the Contract Offender: Do Not Add to Vengeance List (Delete It)
                            if (pm_From == m_VengeanceEntry.m_Offender)
                                pm_From.SendMessage("You purchase the vengeance contract against yourself and void it.");

                            //Offender is Another Character: Buy the Contract
                            else
                            {
                                pm_From.SendMessage("You purchase the vengeance contract.");

                                //Create New Player Entry
                                pm_From.AddVengeanceEntry(m_VengeanceEntry.m_Offender, m_VengeanceEntry.m_CreationDate, m_VengeanceEntry.m_PointsRemaining);
                            }

                            pm_From.CloseGump(typeof(PublicVengeanceEntryGump));
                            pm_From.CloseGump(typeof(VengeanceGump));
                            pm_From.SendGump(new VengeanceGump(m_VengeanceGumpMode, sender.Mobile, m_VengeanceFilter, page));

                            return;
                        }

                        else
                        {
                            pm_From.SendMessage("You lack the neccessary funds to purchase this vengeance contract.");

                            pm_From.CloseGump(typeof(PublicVengeanceEntryGump));
                            pm_From.CloseGump(typeof(VengeanceGump));
                            pm_From.SendGump(new VengeanceGump(m_VengeanceGumpMode, sender.Mobile, m_VengeanceFilter, page));

                            return;
                        }
                    break;
                }
            }
        }
    }
}
