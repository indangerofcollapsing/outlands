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
    public enum RestitutionActivityWithin
    {
        ThreeDays,
        ThreeWeeks,
        ThreeMonths
    }

    public static class Restitution
    {
        public static List<RestitutionEntry> m_RestitutionEntries = new List<RestitutionEntry>();        
        
        public static void Initialize()
        {
            Timer.DelayCall(TimeSpan.FromTicks(1), delegate
            {
                if (PersistanceItem == null)
                    PersistanceItem = new RestitutionPersistance();
            });
        }

        public static RestitutionPersistance PersistanceItem;

        public static void AuditPublicRestitutionEntries()
        {
            List<RestitutionEntry> m_EntriesToDelete = new List<RestitutionEntry>();

            foreach (RestitutionEntry entry in m_RestitutionEntries)
            {
                bool deleteEntry = false;                

                if (entry.m_Offender == null)
                    deleteEntry = true;
                else
                {
                    if (entry.m_Offender.Deleted)
                        deleteEntry = true;
                }

                if (deleteEntry)
                    m_EntriesToDelete.Add(entry);
            }

            for (int a = 0; a < m_EntriesToDelete.Count; a++)
            {
                m_RestitutionEntries.Remove(m_EntriesToDelete[a]);
            }
        }

        public static bool AddPublicRestitutionEntry(RestitutionEntry entryToAdd)
        {
            if (m_RestitutionEntries == null || entryToAdd == null)
                return false;

            if (entryToAdd.m_Offender == null)
                return false;

            if (entryToAdd.m_Offender.Deleted)
                return false;

            RestitutionEntry targetEntry = null;

            foreach (RestitutionEntry entry in m_RestitutionEntries)
            {
                if (entry.m_Offender == entryToAdd.m_Offender)
                    targetEntry = entry;
            }

            //Entry Doesn't Exist Yet
            if (targetEntry == null)
            {
                m_RestitutionEntries.Add(entryToAdd);
                return true;
            }

            return false;
        }

        public static bool UpdatePublicRestitutionEntry(RestitutionEntry entryToUpdate)
        {
            if (m_RestitutionEntries == null || entryToUpdate == null)
                return false;

            if (entryToUpdate.m_Offender == null)
                return false;

            if (entryToUpdate.m_Offender.Deleted)
                return false;

            RestitutionEntry targetEntry = null;

            foreach (RestitutionEntry entry in m_RestitutionEntries)
            {
                if (entry.m_Offender == entryToUpdate.m_Offender)
                    targetEntry = entry;
            }

            //New Entry
            if (targetEntry == null)
            {
                m_RestitutionEntries.Add(entryToUpdate);
                return true;
            }

            //Updating Old Entry
            else
            {                
                targetEntry.m_GoldCost = entryToUpdate.m_GoldCost;                

                return true;
            }

            return false;
        }

        public static bool RemovePublicRestitutionEntry(RestitutionEntry entryToRemove)
        {
            if (m_RestitutionEntries == null || entryToRemove == null)
                return false;

            if (entryToRemove.m_Offender == null)
                return false;

            RestitutionEntry targetEntry = null;

            foreach (RestitutionEntry entry in m_RestitutionEntries)
            {
                if (entry.m_Offender == entryToRemove.m_Offender)
                    targetEntry = entry;
            }

            //Deleting Entry
            if (targetEntry != null)
            {
                m_RestitutionEntries.Remove(targetEntry);
                return true;
            }

            return false;
        } 

        public static void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); //version

            writer.Write((int)m_RestitutionEntries.Count);

            foreach (RestitutionEntry entry in m_RestitutionEntries)
            {               
                writer.Write(entry.m_Offender);
                writer.Write(entry.m_GoldCost);
            }
        }

        public static void Deserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            switch (version)
            {
                case 0:
                    {
                        int entriesCount = reader.ReadInt();

                        m_RestitutionEntries = new List<RestitutionEntry>();

                        for (int i = 0; i < entriesCount; ++i)
                        {                           
                            PlayerMobile pm_Offender = reader.ReadMobile() as PlayerMobile;
                           
                            int m_GoldCost = reader.ReadInt();
                           
                            bool allowEntry = true;

                            if (pm_Offender == null)
                                allowEntry = false;
                            else
                            {
                                if (pm_Offender.Deleted)
                                    allowEntry = false;
                            }

                            if (allowEntry)
                            {
                                RestitutionEntry entry = new RestitutionEntry(pm_Offender, m_GoldCost);
                                m_RestitutionEntries.Add(entry);
                            }
                        } 
                    } 
                break;
            }
        }
    }    

    public class RestitutionPersistance : Item
    {
        public override string DefaultName { get { return "Restitution Persistance"; } }

        public RestitutionPersistance(): base(0x0)
        {
            Movable = false;
        }

        public RestitutionPersistance(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            Restitution.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            Movable = false;
            Restitution.PersistanceItem = this;
            Restitution.Deserialize(reader);
        }
    }   

    public class RestitutionFilter
    {   
        public string m_PlayerNameFilter;
        public string m_GuildAbbreviationFilter;

        public RestitutionActivityWithin m_LengthFilter;

        public string m_CostMinimum;
        public string m_CostMaxmimum;

        public bool m_ShowOnlyMyPublicEntires;

        public RestitutionFilter()
        {            
            m_PlayerNameFilter = "";
            m_GuildAbbreviationFilter = "";

            m_LengthFilter = RestitutionActivityWithin.ThreeMonths;

            m_CostMinimum = "";
            m_CostMaxmimum = "";

            m_ShowOnlyMyPublicEntires = false;
        }
    }

    public class RestitutionEntry
    {             
        public PlayerMobile m_Offender; 
     
        public int m_GoldCost = 0;

        public static void Initialize()
        {          
            CommandSystem.Register("RestitutionFeeList", AccessLevel.Seer, new CommandEventHandler(RestitutionFeeList));
            CommandSystem.Register("ConsiderSins", AccessLevel.Player, new CommandEventHandler(ConsiderSins));            
        }

        [Usage("RestitutionFeeList")]
        [Description("Opens The Restitution Fee List")]
        public static void RestitutionFeeList(CommandEventArgs arg)
        {
            PlayerMobile pm_Mobile = arg.Mobile as PlayerMobile;

            if (pm_Mobile == null)
                return;

            pm_Mobile.CloseGump(typeof(RestitutionGump));
            pm_Mobile.SendGump(new RestitutionGump(arg.Mobile, new RestitutionFilter(), 0));
        }

        [Usage("ConsiderSins")]
        [Description("Acts as if player said 'I Must Consider My Sins'")]
        public static void ConsiderSins(CommandEventArgs arg)
        {
            PlayerMobile pm_Mobile = arg.Mobile as PlayerMobile;

            if (pm_Mobile == null)
                return;

            pm_Mobile.ConsiderSins();
        }        

        public RestitutionEntry(PlayerMobile offender, int goldCost)
        {       
            m_Offender = offender;    
          
            m_GoldCost = goldCost;           
        }

        public class RestitutionGump : Server.Gumps.Gump
        {   
            public Mobile m_From;
            public RestitutionFilter m_RestitutionFilter = new RestitutionFilter();
            public List<RestitutionEntry> m_FilteredRestitutionEntries = new List<RestitutionEntry>();
          
            public int m_Page = 0;

            public const int m_EntriesPerPage = 10;            

            public RestitutionGump(Mobile from, RestitutionFilter RestitutionFilter, int page): base(0, 0)
            {
                if (from == null || RestitutionFilter == null)
                    return;
                
                m_From = from;               
                m_RestitutionFilter = RestitutionFilter;
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
                int activeTextHue = 159;

                //FILTERS------------
                AddLabel(100, 75, textHue, @"Filter By");

                AddLabel(45, 95, textHue, @"Player Name");
                AddImage(45, 115, 2501);
                AddTextEntry(53, 115, 127, 20, textHue, 3, m_RestitutionFilter.m_PlayerNameFilter);

                AddLabel(45, 135, textHue, @"Guild Abbreviation");
                AddImage(45, 155, 2501);
                AddTextEntry(53, 155, 127, 20, textHue, 4, m_RestitutionFilter.m_GuildAbbreviationFilter, 3);     
               
                AddLabel(45, 175, textHue, @"Active Within");

                AddGroup(0);

                AddRadio(45, 195, 2151, 2153, m_RestitutionFilter.m_LengthFilter == RestitutionActivityWithin.ThreeDays, 5);
                AddLabel(85, 200, textHue, @"3 Days");

                AddRadio(45, 225, 2151, 2153, m_RestitutionFilter.m_LengthFilter == RestitutionActivityWithin.ThreeWeeks, 6);
                AddLabel(85, 230, textHue, @"3 Weeks");

                AddRadio(45, 255, 2151, 2153, m_RestitutionFilter.m_LengthFilter == RestitutionActivityWithin.ThreeMonths, 7);
                AddLabel(85, 260, textHue, @"3 Months");                

                AddLabel(45, 290, textHue, @"Cost Minimum");
                AddImage(45, 310, 2501);
                AddTextEntry(53, 310, 127, 20, textHue, 8, m_RestitutionFilter.m_CostMinimum, 7);

                AddLabel(45, 330, textHue, @"Cost Maximum");
                AddImage(45, 350, 2501);
                AddTextEntry(53, 350, 127, 20, textHue, 9, m_RestitutionFilter.m_CostMaxmimum, 7);

                AddCheck(45, 375, 2151, 2153, m_RestitutionFilter.m_ShowOnlyMyPublicEntires, 10);
                AddLabel(85, 375, textHue, @"Show Only My");
                AddLabel(85, 395, textHue, @"Account Entries");

                AddButton(85, 450, 238, 240, 15, GumpButtonType.Reply, 0); //Apply  
               
                //Filter Restitution List Entries----------------------
                bool usePublicEntries = true;

                List<RestitutionEntry> m_TempRestitutionEntries;              

                if (Restitution.m_RestitutionEntries == null)
                    return;

                Restitution.AuditPublicRestitutionEntries();

                m_TempRestitutionEntries = Restitution.m_RestitutionEntries;  

                foreach (RestitutionEntry entry in m_TempRestitutionEntries)
                {
                    bool addToFilter = true;

                    //Different Player Name
                    if (m_RestitutionFilter.m_PlayerNameFilter != "")
                    {
                        string offenderName = entry.m_Offender.RawName.ToLower();
                        string searchString = m_RestitutionFilter.m_PlayerNameFilter.ToLower();
                        int indexOf = offenderName.IndexOf(searchString);

                        if (indexOf == -1)
                            addToFilter = false;
                    }

                    //Different Guild Abbreviation
                    if (m_RestitutionFilter.m_GuildAbbreviationFilter != "")
                    {
                        if (entry.m_Offender.Guild != null)
                        {
                            string offenderGuildAbbreviationName = entry.m_Offender.Guild.Abbreviation.ToLower();
                            string searchString = m_RestitutionFilter.m_GuildAbbreviationFilter.ToLower();
                            int indexOf = offenderGuildAbbreviationName.IndexOf(searchString);

                            if (indexOf == -1)
                                addToFilter = false;
                        }

                        else
                            addToFilter = false;
                    }

                    DateTime lastLoggedOn = entry.m_Offender.LastOnline;

                    DateTime threeDaysAgo = DateTime.UtcNow.Subtract(TimeSpan.FromDays(3));
                    DateTime threeWeeksAgo = DateTime.UtcNow.Subtract(TimeSpan.FromDays(21));
                    DateTime threeMonthsAgo = DateTime.UtcNow.Subtract(TimeSpan.FromDays(90));

                    //Last Activity Date
                    switch (m_RestitutionFilter.m_LengthFilter)
                    {
                        case RestitutionActivityWithin.ThreeDays:
                            if (lastLoggedOn < threeDaysAgo)
                                addToFilter = false;
                            break;

                        case RestitutionActivityWithin.ThreeWeeks:
                            if (lastLoggedOn < threeWeeksAgo)
                                addToFilter = false;
                            break;

                        case RestitutionActivityWithin.ThreeMonths:
                            if (lastLoggedOn < threeMonthsAgo)
                                addToFilter = false;
                            break;
                    }

                    //Cost Minimum
                    if (m_RestitutionFilter.m_CostMinimum != "")
                    {
                        int costMinimum = 0;

                        try { costMinimum = Convert.ToInt32(m_RestitutionFilter.m_CostMinimum); }
                        catch (Exception e) { costMinimum = 0; }

                        if (entry.m_GoldCost < costMinimum)
                            addToFilter = false;
                    }

                    //Cost Maximum
                    if (m_RestitutionFilter.m_CostMaxmimum != "")
                    {
                        int costMaximum = 0;

                        try { costMaximum = Convert.ToInt32(m_RestitutionFilter.m_CostMaxmimum); }
                        catch (Exception e) { costMaximum = 0; }

                        if (entry.m_GoldCost > costMaximum)
                            addToFilter = false;
                    }

                    //Show Only My Account Entries
                    if (m_RestitutionFilter.m_ShowOnlyMyPublicEntires)
                    {
                        //Character Isn't On My Account
                        if (entry.m_Offender.Account != pm_From.Account)
                            addToFilter = false;                        
                    }

                    //Entry Still Valid After Applying Filters
                    if (addToFilter)
                        m_FilteredRestitutionEntries.Add(entry);
                }

                //Restitution List Titles
                AddLabel(350, 75, textHue, @"Restitution Fee Listings");

                AddLabel(240, 115, 149, @"Player Name");
                AddLabel(360, 115, 149, @"Guild ");
                AddLabel(420, 115, 149, @"Fee Amount");
                AddLabel(520, 115, 149, @"Active Within");

                //Build Page Based on Entries and Page Index
                AddPage(1);

                int totalEntries = m_FilteredRestitutionEntries.Count;

                if (totalEntries <= 0)
                    return;

                //Decrease Page if 
                if ((m_Page * m_EntriesPerPage) > totalEntries)
                    m_Page = (int)(Math.Floor((double)totalEntries / 10));

                if (m_Page >= totalEntries)
                    m_Page = totalEntries - 1;

                int startingEntryIndex = m_Page * m_EntriesPerPage;
                int endingEntryIndex;

                if (totalEntries > startingEntryIndex + m_EntriesPerPage)
                    endingEntryIndex = startingEntryIndex + m_EntriesPerPage;
                else
                    endingEntryIndex = totalEntries;

                int yStart = 145;

                //Restitution Entries
                for (int a = startingEntryIndex; a < endingEntryIndex; a++)
                {
                    RestitutionEntry entry = m_FilteredRestitutionEntries[a];

                    int textColor = textHue;
                   
                    //On Same Account
                    if (entry.m_Offender.Account == pm_From.Account)
                        textColor = activeTextHue;                    

                    AddButton(200, yStart, 4029, 4031, 20 + a, GumpButtonType.Reply, 0); //Details Button
                    AddLabel(240, yStart, textColor, entry.m_Offender.RawName); //Player Name

                    if (entry.m_Offender.Guild != null)
                    {
                        AddLabel(360, yStart, textColor, entry.m_Offender.Guild.Abbreviation); //Guild Abbv.
                    }

                    AddLabel(420, yStart, textColor, entry.m_GoldCost.ToString()); //Gold Offered

                    string activeWithin = "1 Year";

                    DateTime lastLoggedOn = entry.m_Offender.LastOnline;

                    DateTime threeDaysAgo = DateTime.UtcNow.Subtract(TimeSpan.FromDays(3));
                    DateTime threeWeeksAgo = DateTime.UtcNow.Subtract(TimeSpan.FromDays(21));
                    DateTime threeMonthsAgo = DateTime.UtcNow.Subtract(TimeSpan.FromDays(90));

                    if (lastLoggedOn > threeDaysAgo)
                        activeWithin = "3 Days";

                    else if (lastLoggedOn > threeWeeksAgo)
                        activeWithin = "3 Weeks";

                    else if (lastLoggedOn > threeMonthsAgo)
                        activeWithin = "3 Months";

                    AddLabel(520, yStart, textColor, activeWithin); //Activity

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
                if (sender.Mobile == null || info == null || m_RestitutionFilter == null || m_Page == null)
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
                    m_RestitutionFilter.m_PlayerNameFilter = "";
                else
                    m_RestitutionFilter.m_PlayerNameFilter = textPlayerName;

                //Guild Abbreviation
                if (info.GetTextEntry(4) != null)
                {
                    TextRelay textRelayGuildAbbreviation = info.GetTextEntry(4);
                    textGuildAbbreviation = textRelayGuildAbbreviation.Text.Trim();
                }

                if (textGuildAbbreviation == null || textGuildAbbreviation == "")
                    m_RestitutionFilter.m_GuildAbbreviationFilter = "";
                else
                    m_RestitutionFilter.m_GuildAbbreviationFilter = textGuildAbbreviation;

                if (info.IsSwitched(5))
                    m_RestitutionFilter.m_LengthFilter = RestitutionActivityWithin.ThreeDays;

                if (info.IsSwitched(6))
                    m_RestitutionFilter.m_LengthFilter = RestitutionActivityWithin.ThreeWeeks;

                if (info.IsSwitched(7))
                    m_RestitutionFilter.m_LengthFilter = RestitutionActivityWithin.ThreeMonths;

                //Cost Minimum
                if (info.GetTextEntry(8) != null)
                {
                    TextRelay textRelayCostMinimum = info.GetTextEntry(8);
                    textCostMinimum = textRelayCostMinimum.Text.Trim();
                }

                if (textCostMinimum == null || textCostMinimum == "")
                    m_RestitutionFilter.m_CostMinimum = "";
                else
                    m_RestitutionFilter.m_CostMinimum = textCostMinimum;

                //Cost Maximum
                if (info.GetTextEntry(9) != null)
                {
                    TextRelay textRelayCostMaximum = info.GetTextEntry(9);
                    textCostMaximum = textRelayCostMaximum.Text.Trim();
                }

                if (textCostMaximum == null || textCostMaximum == "")
                    m_RestitutionFilter.m_CostMaxmimum = "";
                else
                    m_RestitutionFilter.m_CostMaxmimum = textCostMaximum;

                //Show Only My Active
                m_RestitutionFilter.m_ShowOnlyMyPublicEntires = info.IsSwitched(10);

                //Clicked a Restitution Entry
                if (info.ButtonID >= 20 && m_FilteredRestitutionEntries != null)
                {
                    int arrayIndex = info.ButtonID - 20;

                    if (m_FilteredRestitutionEntries.Count <= arrayIndex)
                        return;

                    RestitutionEntry entry = m_FilteredRestitutionEntries[arrayIndex];

                    if (entry == null)
                        return;

                    pm_From.CloseGump(typeof(RestitutionGump));
                    pm_From.CloseGump(typeof(PublicRestitutionEntryGump));

                    pm_From.SendGump(new RestitutionGump(pm_From, m_RestitutionFilter, m_Page));
                    pm_From.SendGump(new PublicRestitutionEntryGump(pm_From, m_RestitutionFilter, entry, m_Page));
                    
                }

                //Handle Button Press
                switch (info.ButtonID)
                {
                    //My Restitution List
                    case 0:                        
                    break;

                    //Public Restitution Contracts
                    case 1:                    
                    break;

                    //Restitution Fees
                    case 2:
                    break;

                    case 15: //Apply Filters: Resend Gump After Updating Filters
                        pm_From.CloseGump(typeof(RestitutionGump));
                        pm_From.SendGump(new RestitutionGump(pm_From, m_RestitutionFilter, m_Page));
                    break;

                    case 16:
                        //Previous Page  
                        if (m_Page > 0)
                            m_Page--;

                        pm_From.CloseGump(typeof(RestitutionGump));
                        pm_From.SendGump(new RestitutionGump(pm_From, m_RestitutionFilter, m_Page));
                    break;

                    case 17:
                        //Next Page                        
                        m_Page++;

                        pm_From.CloseGump(typeof(RestitutionGump));
                        pm_From.SendGump(new RestitutionGump(pm_From, m_RestitutionFilter, m_Page));
                    break;
                }
            }
        }

        public class PublicRestitutionEntryGump : Server.Gumps.Gump
        {   
            Mobile m_From;
            RestitutionFilter m_RestitutionFilter;
            RestitutionEntry m_RestitutionEntry;

            int m_Page;

            public PublicRestitutionEntryGump(Mobile from, RestitutionFilter RestitutionFilter, RestitutionEntry RestitutionEntry, int page)
                : base(150, 75)
            {
                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;

                int textHue = 2036;               

                m_From = from;
                m_RestitutionFilter = RestitutionFilter;
                m_RestitutionEntry = RestitutionEntry;
                m_Page = page;

                AddPage(0);
                AddImage(8, 8, 1248);

                AddLabel(100, 20, textHue, @"Restitution Fee Payment");               

                AddLabel(45, 55, textHue, @"Offender:");
                AddLabel(150, 55, 38, RestitutionEntry.m_Offender.RawName);

                AddLabel(45, 75, textHue, @"Offender's Guild:");

                if (RestitutionEntry.m_Offender.Guild != null)
                    AddLabel(150, 75, 38, RestitutionEntry.m_Offender.Guild.Abbreviation);
                else
                    AddLabel(150, 75, 38, "None");

                AddLabel(45, 95, textHue, @"Active Within:");

                string activeWithin = "1 Year";

                DateTime lastLoggedOn = RestitutionEntry.m_Offender.LastOnline;

                DateTime threeDaysAgo = DateTime.UtcNow.Subtract(TimeSpan.FromDays(3));
                DateTime threeWeeksAgo = DateTime.UtcNow.Subtract(TimeSpan.FromDays(21));
                DateTime threeMonthsAgo = DateTime.UtcNow.Subtract(TimeSpan.FromDays(90));

                if (lastLoggedOn > threeDaysAgo)
                    activeWithin = "3 Days";
                else if (lastLoggedOn > threeWeeksAgo)
                    activeWithin = "3 Weeks";
                else if (lastLoggedOn > threeMonthsAgo)
                    activeWithin = "3 Months";

                AddLabel(150, 95, 38, activeWithin);

                AddLabel(45, 115, textHue, @"Restitution Fee:");
                AddLabel(150, 115, textHue, RestitutionEntry.m_GoldCost.ToString());

                AddLabel(45, 140, textHue, @"Make Payment:");
                AddImage(137, 140, 2501);
                AddTextEntry(145, 140, 127, 20, textHue, 3, "0", 7);

                AddLabel(76, 374, textHue, @"Confirm Payment");
                AddButton(43, 370, 2151, 2152, 1, GumpButtonType.Reply, 0);                              
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {                
                PlayerMobile pm_From = sender.Mobile as PlayerMobile;

                if (pm_From == null)
                    return;                          
                
                if (m_RestitutionEntry == null)
                    return;
               
                bool detailsChanged = false;

                RestitutionEntry masterEntry = null;
                Restitution.AuditPublicRestitutionEntries();

                //Find Public Version of Entry in Public Restitution List
                foreach (RestitutionEntry entry in Restitution.m_RestitutionEntries)
                {
                    //Found Correct Owner + Offender Match
                    if (entry.m_Offender == m_RestitutionEntry.m_Offender && entry.m_Offender != null)
                        masterEntry = entry;                   
                }  

                //Can't Find Public Entry Anymore
                if (masterEntry == null)
                    detailsChanged = true;

                else
                {                       
                    if (masterEntry.m_Offender.Deleted)
                        detailsChanged = true;

                    if (masterEntry.m_GoldCost != m_RestitutionEntry.m_GoldCost)
                        detailsChanged = true;                  
                }

                //If Any Details Have Changed
                if (detailsChanged == true)
                {   
                    pm_From.SendMessage("Details of this Restitution Fee listing have changed since initial viewing.");
                                        
                    pm_From.CloseGump(typeof(PublicRestitutionEntryGump));
                    pm_From.CloseGump(typeof(RestitutionGump)); 
                    pm_From.SendGump(new RestitutionGump(sender.Mobile, m_RestitutionFilter, m_Page));   

                    return;
                }

                PlayerMobile pm_Offender = m_RestitutionEntry.m_Offender;

                switch (info.ButtonID)
                {
                    //Make Payment
                    case 1:
                        bool goldValid = true;
                        int goldPayment = 0;

                        TextRelay textRelayGoldAccepted = info.GetTextEntry(3);
                        string textGoldAccepted = textRelayGoldAccepted.Text.Trim();

                        try { goldPayment = Convert.ToInt32(textGoldAccepted); }

                        catch (Exception e)
                        {
                            goldPayment = 0;
                            pm_From.SendMessage("That is not a valid gold amount.");
                            goldValid = false;
                        }

                        if (!goldValid)
                            return;

                        if (goldPayment <= 0)
                        {
                            pm_From.SendMessage("Payments must be of at least 1 gold.");
                            return;
                        }

                        if (goldPayment > m_RestitutionEntry.m_GoldCost)
                        {
                            pm_From.SendMessage("That is more gold than required for this Restitution Fee listing.");
                            return;
                        }

                        bool hasEnoughGold = Banker.GetBalance(pm_From) >= goldPayment;
                        
                        if (hasEnoughGold)
                        {
                            //Full Payment
                            if (goldPayment == m_RestitutionEntry.m_GoldCost)
                            {
                                Banker.Withdraw(pm_From, m_RestitutionEntry.m_GoldCost);                                

                                pm_Offender.RestitutionFee = 0;

                                Restitution.RemovePublicRestitutionEntry(masterEntry);   
   
                                if (pm_From != pm_Offender)
                                    pm_From.SendMessage("Your payment fulfills the outstanding Restitution Fee for that individual.");
                                                               
                                pm_Offender.SendMessage("Your Restitution Fee has been paid in full!");

                                //Find Longest Current Penance Timer on Offender's Account
                                DateTime longestPenance = DateTime.UtcNow;

                                Account acc = pm_Offender.Account as Account;

                                for (int i = 0; i < (acc.Length - 1); i++)
                                {
                                    Mobile m = acc.accountMobiles[i] as Mobile;

                                    if (m != null)
                                    {
                                        PlayerMobile player = m as PlayerMobile;

                                        if (player != null)
                                        {
                                            if (player.PenanceExpiration > DateTime.UtcNow && player.PenanceExpiration > longestPenance)
                                                longestPenance = player.PenanceExpiration;

                                            break;
                                        }
                                    }
                                }

                                int days = Math.Abs((pm_Offender.PenanceExpiration - DateTime.UtcNow).Days);
                                int hours = Math.Abs((pm_Offender.PenanceExpiration - DateTime.UtcNow).Hours);
                                int minutes = Math.Abs((pm_Offender.PenanceExpiration - DateTime.UtcNow).Minutes);

                                if (minutes >= 60)
                                    hours++;

                                if (hours >= 24)
                                    days++;

                                string sTime = "";

                                if (days > 1)
                                    sTime += days.ToString() + " days ";

                                else if (days == 1)
                                    sTime += days.ToString() + " day ";

                                if (hours > 1)
                                    sTime += hours.ToString() + " hours ";

                                else if (hours == 1)
                                    sTime += hours.ToString() + " hour ";

                                if (minutes > 1)
                                    sTime += minutes.ToString() + " minutes ";

                                else if (minutes == 1)
                                    sTime += minutes.ToString() + " minute ";

                                sTime = sTime.Trim();

                                if (sTime != "")
                                {
                                    if (longestPenance > DateTime.UtcNow)
                                        pm_Offender.SendMessage("Your account is under the risk of temporary statloss for another " + sTime + " if you enter any dungeon or contested area.");
                                }

                                //Disburse Restitution Fees to Victims
                                PaladinEvents.DisburseRestitutionFees(pm_Offender);
                            }

                            //Partial Payment
                            else
                            {
                                Banker.Withdraw(pm_From, goldPayment);
                                
                                masterEntry.m_GoldCost -= goldPayment;
                                pm_Offender.RestitutionFee -= goldPayment;

                                Restitution.UpdatePublicRestitutionEntry(masterEntry);

                                pm_From.SendMessage("You make a payment towards the Restitution Fee for that character.");
                            }

                            pm_From.CloseGump(typeof(PublicRestitutionEntryGump));
                            pm_From.CloseGump(typeof(RestitutionGump));
                            pm_From.SendGump(new RestitutionGump(sender.Mobile, m_RestitutionFilter, m_Page));

                            return;
                        }

                        else
                        {
                            pm_From.SendMessage("You do not have enough gold to make that payment amount.");                            

                            return;
                        }
                    break;                              
                }
            }            
        }
    }
}
