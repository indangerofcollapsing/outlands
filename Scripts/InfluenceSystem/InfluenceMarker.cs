using System;
using Server.Items;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using Server.Accounting;

namespace Server
{
    public class InfluenceMarker : Item
    {        
        public override bool Decays { get { return false; } }

        public enum MarkerBuildingType
        {
            Basic, 
            Vendor,
            City
        }

        public enum MarkerFacing
        {
            South,
            East
        }

        public int BaseInvestmentCost = 500;
        public int InvestmentCostIncrement = 250;

        public double BasicBonusChance = .10;
        public double InvestmentBonusChance = .25;
        public double PrestigeBonusChance = .25;

        public int minimumInfluenceNeededForControlBonus = 5;

        public int BasicBuildingControllerInfluenceBonus = 1;        
        public int VendorBuildingControllerInfluenceBonus = 3;
        public int CityBuildingControllerInfluenceBonus = 5;

        public string m_BuildingName = "";
        [CommandProperty(AccessLevel.Administrator)]
        public string BuildingName
        {
            get { return m_BuildingName; }
            set { m_BuildingName = value; }
        }

        public MarkerBuildingType m_BuildingType = MarkerBuildingType.Basic;
        [CommandProperty(AccessLevel.Administrator)]
        public MarkerBuildingType BuildingType
        {
            get { return m_BuildingType; }
            set { m_BuildingType = value; }
        }

        public IndexedRegionName m_MarkerRegion = IndexedRegionName.Britain;
        [CommandProperty(AccessLevel.Administrator)]
        public IndexedRegionName MarkerRegion
        {
            get { return m_MarkerRegion; }
            set { m_MarkerRegion = value; }
        }

        public MarkerFacing m_Facing = MarkerFacing.South;
        [CommandProperty(AccessLevel.Administrator)]
        public MarkerFacing Facing
        {
            get { return m_Facing; }
            set
            {
                m_Facing = value;

                switch (m_Facing)
                {
                    case MarkerFacing.South: ItemID = 7977; break;
                    case MarkerFacing.East: ItemID = 7976; break;
                }
            }
        }

        public bool m_Enabled = false;
        [CommandProperty(AccessLevel.Administrator)]
        public bool Enabled
        {
            get {return m_Enabled; }
            set
            {
                m_Enabled = value;

                if (m_Enabled)
                {
                    Visible = true;

                    foreach (Item item in m_Items)
                    {
                        if (item != null)
                            item.Visible = true;
                    }
                }

                else
                {
                    Visible = false;

                    foreach (Item item in m_Items)
                    {
                        if (item != null)
                            item.Visible = false;
                    }
                }
            }
        }

        private XmlSpawner m_XMLSpawner;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner XMLSpawner
        {
            get { return m_XMLSpawner; }
            set {
                m_XMLSpawner = value;

                if (m_XMLSpawner != null)
                {
                    if (!m_XMLSpawner.Deleted)
                        m_XMLSpawner.InfluenceMarker = this;
                }
            }
        } 

        public List<Item> m_Items = new List<Item>();
        public List<InfluenceEntry> m_InfluenceEntries = new List<InfluenceEntry>();

        public InfluenceAccountEntry m_Controller;
        public int m_ControllerInfluence = 0;

        [Constructable]
        public InfluenceMarker(): base(7977)
        {
            Name = "an influence marker";
            Hue = 2587;

            Movable = false;
            Visible = m_Enabled;

            InfluencePersistance.m_InfluenceMarkers.Add(this);

            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(AddComponents));
        }

        public InfluenceMarker(Serial serial): base(serial)
        {
        }

        public void AddComponents()
        {
            if (Deleted)
                return;

            GroupItem(new Static(4758), 0, 0, 0);

            Z += 14;
        }

        public virtual void GroupItem(Item item, int xOffset, int yOffset, int zOffset)
        {
            m_Items.Add(item);

            item.Visible = m_Enabled;

            item.MoveToWorld(new Point3D(X + xOffset, Y + yOffset, Z + zOffset), Map);
        }

        public override void OnSingleClick(Mobile from)
        {
            if (from.NetState != null)
            {
                if (BuildingName == "" || BuildingName == null || !InfluencePersistance.Enabled)
                    LabelTo(from, "an influence marker");
                else
                {
                    if (m_Controller != null)
                    {
                        if (m_Controller.m_CurrentCharacter != null)
                        {
                            LabelTo(from, BuildingName);
                            LabelTo(from, "(controlled by " + m_Controller.m_CurrentCharacter.RawName + ")");
                        }

                        else
                        {
                            LabelTo(from, BuildingName);
                            LabelTo(from, "(influence marker)");
                        }
                    }

                    else
                    {
                        LabelTo(from, BuildingName);
                        LabelTo(from, "(influence marker)");
                    }                   
                }                
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from == null)
                return;

            if (!InfluencePersistance.Enabled)
            {
                from.SendMessage("The Influence System is currently disabled.");
                return;
            }

            if (!from.Alive)
            {
                from.SendMessage("You must be alive to use that.");
                return;
            }

            if (!from.InRange(Location, 2) && from.AccessLevel == AccessLevel.Player)
            {
                from.SendMessage("You are too far away to use that.");
                return;
            }

            PlayerMobile pm_From = from as PlayerMobile;

            if (pm_From == null)
                return;

            InfluencePersistance.CheckCreateInfluenceAccountEntry(pm_From);

            if (pm_From.Skills.Begging.Value >= 100 && pm_From.Skills.Camping.Value >= 100)
            {
                if (!InfluencePersistance.m_PlayersGivenFreeRecruitmentDeeds.Contains(pm_From))
                {
                    if (pm_From.BankBox != null)
                    {
                        if (pm_From.BankBox.Items.Count < pm_From.BankBox.MaxItems)
                        {
                            pm_From.BankBox.AddItem(new Custom.HenchmanRecruitmentDeed());

                            InfluencePersistance.m_PlayersGivenFreeRecruitmentDeeds.Add(pm_From);

                            pm_From.SendMessage("One complimentary Henchman Recruitment Deed has been placed in your bankbox for your mastery of Begging and Camping. Further deeds must be acquired from the Influence Rewards Gump through the use of Influence Lottery Tickets.");
                            pm_From.SendSound(0x5AA);

                            return;
                        }

                        else
                        {
                            pm_From.SendMessage("You qualify for receiving one complimentary Henchman Recruitment Deed for your mastery of Begging and Camping, however your bankbox is currently full. Please make some room in your bankbox and reactivate the Influence Marker to receive it.");
                            return;
                        }
                    }                    
                }
            }            

            pm_From.CloseAllGumps();
            pm_From.SendGump(new InfluenceMarkerGump(this, pm_From));
        }       

        public static int NextInvestmentLevelCost(int investmentLevel)
        {
            if (investmentLevel < 0)
                investmentLevel = 0;  

            return (500 + (250 * investmentLevel));
        }

        public void IncreaseInvestment(PlayerMobile player, int amount)
        {
            if (player == null) return;
            if (player.m_InfluenceAccountEntry == null) return;

            if (player.m_InfluenceAccountEntry == null)
                InfluencePersistance.CheckCreateInfluenceAccountEntry(player);

            if (player.m_InfluenceAccountEntry == null) return;

            player.m_InfluenceAccountEntry.m_CurrentCharacter = player;

            bool foundPlayer = false;

            int investmentIncrease = amount;

            string message = "You increase your Investment level at this location.";
            int sound = 0x034;

            switch (m_BuildingType)
            {
                case MarkerBuildingType.Basic:
                    if (Utility.RandomDouble() <= BasicBonusChance)
                    {
                        investmentIncrease++;

                        sound = 0x036;
                        message += " Additionally, your popularity has also granted you a bonus level of Investment here!";
                    }
                break;

                case MarkerBuildingType.Vendor:
                    if (Utility.RandomDouble() <= InvestmentBonusChance)
                    {
                        investmentIncrease++;

                        sound = 0x036;
                        message += " Additionally, your wise investment has also yielded you a bonus level of Investment here!";
                    }
                break;

                case MarkerBuildingType.City:
                break;
            }

            player.SendMessage(message);
            player.SendSound(sound);

            InfluenceEntry influenceEntry = null;

            foreach (InfluenceEntry entry in m_InfluenceEntries)
            {
                if (entry == null)
                    continue;

                if (entry.m_InfluenceAccountEntry == player.m_InfluenceAccountEntry && player.m_InfluenceAccountEntry != null)
                {
                    influenceEntry = entry;
                    foundPlayer = true;

                    break;
                }
            }

            if (!foundPlayer)
                m_InfluenceEntries.Add(new InfluenceEntry(player.m_InfluenceAccountEntry, investmentIncrease, 0));

            else
                influenceEntry.m_InvestmentLevel += investmentIncrease;

            UpdatePlayerInfluence(player.m_InfluenceAccountEntry, investmentIncrease, this);
            UpdateMarkerValues();
        }

        public void IncreasePrestige(PlayerMobile player, int amount)
        {
            if (player == null) return;
            if (player.m_InfluenceAccountEntry == null) return;

            if (player.m_InfluenceAccountEntry == null)
                InfluencePersistance.CheckCreateInfluenceAccountEntry(player);

            if (player.m_InfluenceAccountEntry == null) return;

            player.m_InfluenceAccountEntry.m_CurrentCharacter = player;

            bool foundPlayer = false;

            int prestigeIncrease = amount;

            string message = "You increase your Prestige level at this location.";
            int sound = 0x5B4;

            switch (m_BuildingType)
            {
                case MarkerBuildingType.Basic:
                    if (Utility.RandomDouble() <= BasicBonusChance)
                    {
                        prestigeIncrease++;

                        sound = 0x5B5;
                        message += " Additionally, your popularity has also granted you a bonus level of Prestige here!";
                    }
                    break;

                case MarkerBuildingType.Vendor:                   
                break;

                case MarkerBuildingType.City:
                    if (Utility.RandomDouble() <= PrestigeBonusChance)
                    {
                        prestigeIncrease++;

                        sound = 0x5B5;
                        message +=  " Additionally, your reputation has granted you a bonus level of Prestige here!";
                    }
                break;
            }

            player.SendMessage(message);
            player.SendSound(sound);

            InfluenceEntry influenceEntry = null;

            foreach (InfluenceEntry entry in m_InfluenceEntries)
            {
                if (entry == null)
                    continue;

                if (entry.m_InfluenceAccountEntry == player.m_InfluenceAccountEntry && player.m_InfluenceAccountEntry != null)
                {
                    influenceEntry = entry;
                    foundPlayer = true;

                    break;
                }
            }

            if (!foundPlayer)
                m_InfluenceEntries.Add(new InfluenceEntry(player.m_InfluenceAccountEntry, 0, prestigeIncrease));

            else
                influenceEntry.m_PrestigeLevel += prestigeIncrease;

            UpdatePlayerInfluence(player.m_InfluenceAccountEntry, prestigeIncrease, this);
            UpdateMarkerValues();
        }

        public void DetermineMarkerController()
        {
            int controllerInfluence = 0;
            List<InfluenceEntry> m_HighestEntries = new List<InfluenceEntry>();

            foreach (InfluenceEntry entry in m_InfluenceEntries)
            {
                if (entry == null) continue;
                if (entry.m_InfluenceAccountEntry == null) continue;

                int entryTotalInfluence = entry.m_PrestigeLevel + entry.m_InvestmentLevel;

                if (entryTotalInfluence < minimumInfluenceNeededForControlBonus)
                    continue;

                if (controllerInfluence == 0)
                {
                    controllerInfluence = entryTotalInfluence;
                    m_HighestEntries.Add(entry);
                }

                else
                {
                    if (entryTotalInfluence == controllerInfluence)
                        m_HighestEntries.Add(entry);

                    else if (entryTotalInfluence > controllerInfluence)
                    {
                        m_HighestEntries.Clear();

                        m_HighestEntries.Add(entry);
                        controllerInfluence = entryTotalInfluence;
                    }
                }                
            }

            m_ControllerInfluence = 0;

            if (controllerInfluence > 0)
            {
                m_ControllerInfluence = controllerInfluence;

                if (m_HighestEntries.Count == 1)
                    m_Controller = m_HighestEntries[0].m_InfluenceAccountEntry;
            }
        }

        public void UpdateMarkerValues()
        {
            int influenceBonus = InfluenceMarker.GetControllerInfluenceBonus(this);

            InfluenceAccountEntry previousController = m_Controller;         

            DetermineMarkerController();

            if (m_Controller != null)
            {
                if (previousController != null)
                {
                    if (m_Controller != previousController)
                    {
                        UpdatePlayerInfluence(m_Controller, influenceBonus, this);
                        UpdatePlayerInfluence(previousController, influenceBonus * -1, this);

                        Account controllerAccount = Accounts.GetAccount(m_Controller.m_AccountUsername) as Account;

                        if (controllerAccount != null)
                        {
                            if (controllerAccount.accountMobiles != null)
                            {
                                for (int a = 0; a < controllerAccount.accountMobiles.Length; a++)
                                {
                                    PlayerMobile pm_Mobile = controllerAccount.accountMobiles[a] as PlayerMobile;

                                    if (pm_Mobile != null)
                                    {
                                        pm_Mobile.SendSound(0x5A7);
                                        pm_Mobile.SendMessage("You have won influence control over: " + BuildingName + ".");
                                    }
                                }
                            }
                        }

                        Account previousControllerAccount = Accounts.GetAccount(previousController.m_AccountUsername) as Account;

                        if (previousControllerAccount != null)
                        {
                            if (previousControllerAccount.accountMobiles != null)
                            {
                                for (int a = 0; a < previousControllerAccount.accountMobiles.Length; a++)
                                {
                                    PlayerMobile pm_Mobile = previousControllerAccount.accountMobiles[a] as PlayerMobile;

                                    if (pm_Mobile != null)
                                    {
                                        pm_Mobile.SendSound(0x5B3);
                                        pm_Mobile.SendMessage("You have lost influence control over: " + BuildingName + ".");
                                    }
                                }
                            }
                        }
                    }
                }

                else
                {   
                    UpdatePlayerInfluence(m_Controller, influenceBonus, this);

                    Account controllerAccount = Accounts.GetAccount(m_Controller.m_AccountUsername) as Account;

                    if (controllerAccount != null)
                    {
                        if (controllerAccount.accountMobiles != null)
                        {
                            for (int a = 0; a < controllerAccount.accountMobiles.Length; a++)
                            {
                                PlayerMobile pm_Mobile = controllerAccount.accountMobiles[a] as PlayerMobile;

                                if (pm_Mobile != null)
                                {
                                    pm_Mobile.SendSound(0x5A7);
                                    pm_Mobile.SendMessage("You have won influence control over: " + BuildingName + ".");
                                }
                            }
                        }
                    }
                }                    
            }

            else
            {
                if (previousController != null)
                {
                    UpdatePlayerInfluence(previousController, influenceBonus * -1, this);

                    Account account = Accounts.GetAccount(previousController.m_AccountUsername) as Account;
                    
                    if (account != null)
                    {
                        if (account.accountMobiles != null)
                        {
                            for (int a = 0; a < account.accountMobiles.Length; a++)
                            {
                                PlayerMobile pm_Mobile = account.accountMobiles[a] as PlayerMobile;

                                if (pm_Mobile != null)
                                {
                                    pm_Mobile.SendSound(0x5B3);
                                    pm_Mobile.SendMessage("You have lost influence control over: " + BuildingName + ".");                                    
                                }
                            }
                        }
                    }                   
                }
            }
        } 

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            int bonusInfluence = InfluenceMarker.GetControllerInfluenceBonus(this);

            for (int a = 0; a < m_InfluenceEntries.Count; a++)
            {
                InfluenceEntry entry = m_InfluenceEntries[a];

                if (entry == null)
                    continue;

                if (entry.m_InfluenceAccountEntry != null)
                {
                    int influenceAmount = entry.m_InvestmentLevel + entry.m_PrestigeLevel;

                    if (m_Controller == entry.m_InfluenceAccountEntry && m_ControllerInfluence >= minimumInfluenceNeededForControlBonus)
                        influenceAmount += bonusInfluence;

                    influenceAmount *= -1;

                    UpdatePlayerInfluence(entry.m_InfluenceAccountEntry, influenceAmount, this);
                }
            }

            if (InfluencePersistance.m_InfluenceMarkers.Contains(this))
                InfluencePersistance.m_InfluenceMarkers.Remove(this);

            for (int i = 0; i < m_Items.Count; ++i)
                m_Items[i].Delete();
        }

        public static void UpdatePlayerInfluence(InfluenceAccountEntry influenceAccountEntry, int influenceAmount, InfluenceMarker influenceMarker)
        {
            influenceAccountEntry.RealmInfluenceEarnedPerDay += influenceAmount;

            switch (influenceMarker.MarkerRegion)
            {
                case IndexedRegionName.Britain: influenceAccountEntry.BritainInfluenceEarnedPerDay += influenceAmount; break;
                case IndexedRegionName.BuccaneeersDen: influenceAccountEntry.BuccaneersDenInfluenceEarnedPerDay += influenceAmount; break;
                case IndexedRegionName.Cove: influenceAccountEntry.CoveInfluenceEarnedPerDay += influenceAmount; break;
                case IndexedRegionName.Jhelom: influenceAccountEntry.JhelomInfluenceEarnedPerDay += influenceAmount; break;
                case IndexedRegionName.Magincia: influenceAccountEntry.MaginciaInfluenceEarnedPerDay += influenceAmount; break;
                case IndexedRegionName.Minoc: influenceAccountEntry.MinocInfluenceEarnedPerDay += influenceAmount; break;
                case IndexedRegionName.Moonglow: influenceAccountEntry.MoonglowInfluenceEarnedPerDay += influenceAmount; break;
                case IndexedRegionName.Nujelm: influenceAccountEntry.NujelmInfluenceEarnedPerDay += influenceAmount; break;
                case IndexedRegionName.Occlo: influenceAccountEntry.OccloInfluenceEarnedPerDay += influenceAmount; break;
                case IndexedRegionName.SerpentsHold: influenceAccountEntry.SerpentsHoldInfluenceEarnedPerDay += influenceAmount; break;
                case IndexedRegionName.SkaraBrae: influenceAccountEntry.SkaraBraeInfluenceEarnedPerDay += influenceAmount; break;
                case IndexedRegionName.Trinsic: influenceAccountEntry.TrinsicInfluenceEarnedPerDay += influenceAmount; break;
                case IndexedRegionName.Vesper: influenceAccountEntry.VesperInfluenceEarnedPerDay += influenceAmount; break;
                case IndexedRegionName.Yew: influenceAccountEntry.YewInfluenceEarnedPerDay += influenceAmount; break;
            }   
        }

        public static int GetControllerInfluenceBonus(InfluenceMarker influenceMarker)
        {
            int bonus = 0;

            switch (influenceMarker.BuildingType)
            {
                case MarkerBuildingType.Basic: bonus = influenceMarker.BasicBuildingControllerInfluenceBonus; break;                
                case MarkerBuildingType.Vendor: bonus = influenceMarker.VendorBuildingControllerInfluenceBonus; break;
                case MarkerBuildingType.City: bonus = influenceMarker.CityBuildingControllerInfluenceBonus; break;
            }

            return bonus;
        }

        public void GetPlayerLevels(PlayerMobile player, out int prestigeLevel, out int investmentLevel)
        {
            prestigeLevel = 0;
            investmentLevel = 0;

            if (player == null) return;
            if (player.m_InfluenceAccountEntry == null) return;

            foreach (InfluenceEntry influenceEntry in m_InfluenceEntries)
            {
                if (influenceEntry.m_InfluenceAccountEntry == player.m_InfluenceAccountEntry)
                {
                    prestigeLevel = influenceEntry.m_PrestigeLevel;
                    investmentLevel = influenceEntry.m_InvestmentLevel;

                    break; 
                }
            }
        }

        public static string GetBuildingTypeName(InfluenceMarker influenceMarker)
        {
            string buildingTypeName = "";

            switch (influenceMarker.BuildingType)
            {
                case InfluenceMarker.MarkerBuildingType.Basic: buildingTypeName = "Basic"; break;                
                case InfluenceMarker.MarkerBuildingType.Vendor: buildingTypeName = "Vendor"; break;
                case InfluenceMarker.MarkerBuildingType.City: buildingTypeName = "City"; break;
            }

            return buildingTypeName;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            //Version 0
            writer.Write(m_BuildingName);
            writer.Write((int)m_BuildingType);
            writer.Write((int)m_MarkerRegion);
            writer.Write((int)m_Facing);
            writer.Write(m_Enabled);

            writer.Write((int)m_Items.Count);
            foreach (Item item in m_Items)
            {
                writer.Write(item);
            }

            writer.Write(m_InfluenceEntries.Count);
            foreach (InfluenceEntry influenceEntry in m_InfluenceEntries)
            {
                writer.Write(influenceEntry.m_InfluenceAccountEntry);
                writer.Write(influenceEntry.m_InvestmentLevel);
                writer.Write(influenceEntry.m_PrestigeLevel);
            }

            writer.Write(m_Controller);
            writer.Write(m_ControllerInfluence);

            writer.Write(m_XMLSpawner);            
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
           
            m_InfluenceEntries = new List<InfluenceEntry>();
            m_Items = new List<Item>();

            //Version 0
            if (version >= 0)
            {
                m_BuildingName = reader.ReadString();
                m_BuildingType = (MarkerBuildingType)reader.ReadInt();
                m_MarkerRegion = (IndexedRegionName)reader.ReadInt();
                m_Facing = (MarkerFacing)reader.ReadInt();
                m_Enabled = reader.ReadBool();

                int itemsCount = reader.ReadInt();

                for (int i = 0; i < itemsCount; ++i)
                {
                    m_Items.Add(reader.ReadItem());
                } 

                int influenceEntries = reader.ReadInt();
                for (int a = 0; a < influenceEntries; ++a)
                {
                    InfluenceAccountEntry influenceAccountEntry = reader.ReadItem() as InfluenceAccountEntry;

                    int investmentLevel = reader.ReadInt();
                    int prestigeLevel = reader.ReadInt();

                    if (influenceAccountEntry == null) continue;

                    m_InfluenceEntries.Add(new InfluenceEntry(influenceAccountEntry, investmentLevel, prestigeLevel));
                }

                m_Controller = reader.ReadItem()as InfluenceAccountEntry;
                m_ControllerInfluence = reader.ReadInt();

                m_XMLSpawner = (XmlSpawner)reader.ReadItem();
            }
        }
    }

    public class InfluenceEntry
    {
        public InfluenceAccountEntry m_InfluenceAccountEntry;
        public int m_InvestmentLevel;
        public int m_PrestigeLevel;

        public InfluenceEntry(InfluenceAccountEntry influenceAccountEntry, int investmentLevel, int prestigeLevel)
        {
            m_InfluenceAccountEntry = influenceAccountEntry;
            m_InvestmentLevel = investmentLevel;
            m_PrestigeLevel = prestigeLevel;
        }
    }

    public class InfluenceMarkerGump : Gump
    {
        InfluenceMarker m_Marker;
        PlayerMobile pm_From;

        public InfluenceMarkerGump(InfluenceMarker marker, Mobile from): base(0, 0)
        {
            m_Marker = marker;
            pm_From = from as PlayerMobile;

            if (marker == null || from == null || pm_From == null)
                return;

            if (pm_From.m_InfluenceAccountEntry == null)
                return;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int boldHue = 149;
            int textHue = 2036;
            int greenTextHue = 0x3F;
            
            AddPage(0);

            #region Background

            AddImage(10, 10, 103);
            AddImage(139, 10, 103);
            AddImage(272, 10, 103);
            AddImage(405, 10, 103);
            AddImage(405, 98, 103);
            AddImage(405, 192, 103);
            AddImage(405, 286, 103);
            AddImage(10, 96, 103);
            AddImage(10, 191, 103);
            AddImage(10, 287, 103);
            AddImage(11, 382, 103);
            AddImage(141, 382, 103);
            AddImage(274, 382, 103);
            AddImage(405, 382, 103);

            AddBackground(21, 22, 515, 450, 3000);
            AddImage(485, 6, 10440, 2500);   

            #endregion

            #region Images

            AddItem(372, 271, 10784);
            AddItem(351, 251, 10784);
            AddItem(331, 231, 10784);
            AddItem(291, 189, 10784);
            AddItem(311, 210, 10784);
            AddItem(284, 172, 12586);
            AddItem(411, 145, 10784);
            AddItem(432, 166, 10784);
            AddItem(414, 184, 10784);
            AddItem(392, 165, 10784);
            AddItem(352, 166, 10784);
            AddItem(392, 206, 10784);
            AddItem(373, 187, 10784);
            AddItem(333, 146, 10784);
            AddItem(388, 120, 10784);
            AddItem(370, 80, 8979);
            AddItem(393, 89, 8980);
            AddItem(349, 90, 8981);
            AddItem(371, 146, 8982);
            AddItem(331, 188, 10784);
            AddItem(372, 227, 10784);
            AddItem(352, 209, 10784);
            AddItem(312, 168, 10784);
            AddItem(336, 167, 8905);
            AddItem(352, 177, 16150);
            AddItem(338, 223, 8900);
            AddItem(382, 197, 8950);
            AddItem(312, 136, 12584);
            AddItem(455, 185, 10784);
            AddItem(436, 204, 10784);
            AddItem(413, 227, 10784);
            AddItem(394, 247, 10784);
            AddItem(426, 177, 8961);
            AddItem(404, 160, 8962);
            AddItem(414, 205, 12585);
            AddItem(404, 224, 8949);
            AddItem(454, 169, 16140);
            AddItem(314, 195, 8929);
            AddItem(363, 247, 16135);
            AddItem(374, 114, 16153);  
            AddItem(96, 132, 5609);
            AddItem(51, 142, 5402, 2600);
            AddItem(69, 179, 12245);
            AddItem(72, 165, 4810, 2600);
            AddItem(195, 181, 3823);
            AddItem(177, 168, 3823);
            AddItem(163, 187, 3823);
            AddItem(186, 181, 3649);
            AddItem(208, 190, 9245);
            AddItem(164, 202, 7147);
            AddItem(174, 216, 2458);

            #endregion
                                    
            int prestigeLevel = 0;
            int investmentLevel = 0;
            int baseBuildingInfluence = 0;
            int controllerInfluenceBonus = 0;
            int totalBuildingInfluence = 0;

            double totalRegionInfluence = 0;

            int buildingControlBonusValue = InfluenceMarker.GetControllerInfluenceBonus(m_Marker);

            for (int a = 0; a < m_Marker.m_InfluenceEntries.Count; a++)
            {
                InfluenceEntry entry = m_Marker.m_InfluenceEntries[a];

                if (entry == null)
                    continue;

                if (entry.m_InfluenceAccountEntry == pm_From.m_InfluenceAccountEntry)
                {
                    prestigeLevel = entry.m_PrestigeLevel;
                    investmentLevel = entry.m_InvestmentLevel;

                    baseBuildingInfluence += prestigeLevel + investmentLevel;

                    break;
                }
            }

            if (m_Marker.m_Controller == pm_From.m_InfluenceAccountEntry && baseBuildingInfluence >= m_Marker.minimumInfluenceNeededForControlBonus)
                controllerInfluenceBonus = buildingControlBonusValue;

            totalBuildingInfluence = baseBuildingInfluence + controllerInfluenceBonus;

            totalRegionInfluence = InfluencePersistance.GetPlayerInfluenceEarnedPerDay(pm_From, m_Marker.MarkerRegion);            

            string controllerString = "Uncontrolled (control needs 5 influence)";

            if (m_Marker.m_Controller != null && m_Marker.m_ControllerInfluence > 0)
            {
                if (m_Marker.m_Controller.m_CurrentCharacter != null)
                    controllerString = m_Marker.m_Controller.m_CurrentCharacter.RawName + " (via " + m_Marker.m_ControllerInfluence.ToString() + " Influence)";
            }

            if (m_Marker.m_Controller == null && m_Marker.m_ControllerInfluence > 0)
                controllerString = "Contested at " + m_Marker.m_ControllerInfluence.ToString() + " Influence";

            string buildingType = InfluenceMarker.GetBuildingTypeName(m_Marker);
            string regionName = InfluencePersistance.GetRegionName(m_Marker.MarkerRegion);

            int buildingNameOffsetX = (15 - m_Marker.BuildingName.Length) * 4;

            AddLabel(235 + buildingNameOffsetX, 28, boldHue, m_Marker.BuildingName);

            AddLabel(49, 78, 2120, @"Controller:");

            if (m_Marker.m_Controller == pm_From.m_InfluenceAccountEntry)
                AddLabel(125, 78, greenTextHue, controllerString);
            else
                AddLabel(125, 78, textHue, controllerString);

            AddLabel(49, 53, 2426, @"Building Type:");
            AddLabel(140, 53, textHue, buildingType);

            AddLabel(49, 103, 2611, @"Control Bonus:");
            if (m_Marker.m_Controller == pm_From.m_InfluenceAccountEntry && m_Marker.m_ControllerInfluence >= m_Marker.minimumInfluenceNeededForControlBonus)
                AddLabel(148, 103, greenTextHue, buildingControlBonusValue.ToString() + " Influence (earned)");
            else
                AddLabel(148, 103, textHue, buildingControlBonusValue.ToString() + " Influence");

            AddLabel(75, 242, boldHue, @"Your");
            AddLabel(47, 262, boldHue, @"Prestige Here");
            AddLabel(84, 282, textHue, prestigeLevel.ToString());

            AddLabel(185, 240, boldHue, @"Your");
            AddLabel(149, 260, boldHue, @"Investment Here");
            AddLabel(194, 280, textHue, investmentLevel.ToString());

            AddLabel(70, 321, boldHue, @"Your Daily Influence From");
            AddLabel(95, 341, boldHue, @"This Building");

            if (baseBuildingInfluence > 0)
            {
                if (m_Marker.m_Controller == pm_From.m_InfluenceAccountEntry && baseBuildingInfluence >= m_Marker.minimumInfluenceNeededForControlBonus)
                    AddLabel(135, 361, greenTextHue, totalBuildingInfluence.ToString() + "  ( " + prestigeLevel.ToString() + " + " + investmentLevel.ToString() + " + " + controllerInfluenceBonus.ToString() + " )");
                else
                    AddLabel(135, 361, textHue, totalBuildingInfluence.ToString() + "  ( " + prestigeLevel.ToString() + " + " + investmentLevel.ToString() + " )");
            }

            else
                AddLabel(135, 361, textHue, baseBuildingInfluence.ToString());

            int regionNameOffsetX = regionName.Length * 3;

            AddLabel(390 - regionNameOffsetX, 197, textHue, regionName);

            AddLabel(321, 321, boldHue, @"Your Daily Influence In");
            AddLabel(390 - regionNameOffsetX, 341, boldHue, regionName);
            AddLabel(387, 361, textHue, totalRegionInfluence.ToString());

            AddLabel(39, 405, textHue, @"Influence This Building");
            AddItem(62, 423, 8928);  
            AddButton(97, 430, 9721, 9723, 1, GumpButtonType.Reply, 0);

            AddLabel(221, 405, textHue, @"View World Influence");
            AddItem(247, 423, 4167); 
            AddButton(284, 430, 9721, 9723, 2, GumpButtonType.Reply, 0);

            AddLabel(391, 405, textHue, @"Collect Rewards");
            AddItem(394, 435, 0x14ED, 2616);
            AddButton(432, 430, 9721, 9723, 3, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (m_Marker == null) return;
            if (m_Marker.Deleted) return;
            if (pm_From == null) return;
            if (pm_From.Deleted) return;

            //Influence This Building
            if (info.ButtonID == 1)
            {
                pm_From.CloseAllGumps();
                pm_From.SendGump(new InfluenceMarkerInfluenceBuildingGump(m_Marker, pm_From));
            }

            //View World Influence
            if (info.ButtonID == 2)
            {
                pm_From.CloseAllGumps();
                pm_From.SendGump(new InfluenceMarkerWorldInfluenceGump(m_Marker, pm_From, true));
            }

            //Collect Rewards
            if (info.ButtonID == 3)
            {
                pm_From.CloseAllGumps();
                pm_From.SendGump(new InfluenceMarkerCollectRewardsGump(m_Marker, pm_From));
            }
        }
    }

    public class InfluenceMarkerInfluenceBuildingGump : Gump
    {
        InfluenceMarker m_Marker;
        PlayerMobile pm_From;

        public InfluenceMarkerInfluenceBuildingGump(InfluenceMarker marker, Mobile from): base(0, 0)
        {
            m_Marker = marker;
            pm_From = from as PlayerMobile;

            if (marker == null || from == null || pm_From == null)
                return;

            if (pm_From.m_InfluenceAccountEntry == null)
                return;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int boldHue = 149;
            int textHue = 2036;
            int greenTextHue = 0x3F;

            AddPage(0);

            #region Background            
            
            AddImage(10, 10, 103);
            AddImage(139, 10, 103);
            AddImage(272, 10, 103);
            AddImage(405, 10, 103);
            AddImage(405, 98, 103);
            AddImage(405, 192, 103);
            AddImage(405, 286, 103);
            AddImage(10, 96, 103);
            AddImage(10, 191, 103);
            AddImage(10, 287, 103);
            AddImage(11, 382, 103);
            AddImage(141, 382, 103);
            AddImage(274, 382, 103);
            AddImage(405, 382, 103);

            AddBackground(21, 22, 515, 450, 3000);
            AddImage(485, 6, 10440, 2500);

            #endregion

            #region Images

            AddItem(266, 238, 3823);
            AddItem(248, 225, 3823);
            AddItem(234, 244, 3823);
            AddItem(257, 238, 3649);
            AddItem(279, 247, 9245);
            AddItem(235, 259, 7147);
            AddItem(245, 273, 2458);
            AddItem(106, 192, 5609);
            AddItem(61, 202, 5402, 2600);
            AddItem(79, 239, 12245);
            AddItem(82, 225, 4810, 2600);

            #endregion
            
            int prestigeLevel = 0;
            int investmentLevel = 0;
            int baseBuildingInfluence = 0;
            int controllerInfluenceBonus = 0;
            int totalBuildingInfluence = 0;

            double totalRegionInfluence = 0;

            int buildingControlBonusValue = InfluenceMarker.GetControllerInfluenceBonus(m_Marker);
                      
            m_Marker.GetPlayerLevels(pm_From, out prestigeLevel, out investmentLevel); 

            baseBuildingInfluence += prestigeLevel + investmentLevel;

            int regionPrestigeAvailable = InfluencePersistance.GetPlayerPrestigeAvailable(pm_From, m_Marker.MarkerRegion);
            int investmentNextLevelCost = InfluenceMarker.NextInvestmentLevelCost(investmentLevel);

            if (m_Marker.m_Controller == pm_From.m_InfluenceAccountEntry && baseBuildingInfluence >= m_Marker.minimumInfluenceNeededForControlBonus)
                controllerInfluenceBonus = buildingControlBonusValue;

            totalBuildingInfluence = baseBuildingInfluence + controllerInfluenceBonus;

            totalRegionInfluence = InfluencePersistance.GetPlayerInfluenceEarnedPerDay(pm_From, m_Marker.MarkerRegion);    

            string regionName = InfluencePersistance.GetRegionName(m_Marker.MarkerRegion);

            int buildingNameOffsetX = (15 - m_Marker.BuildingName.Length) * 4;
            int regionNameOffsetX = (15 - regionName.Length) * 4;           

            string controllerString = "Uncontrolled (control needs 5 influence)";

            if (m_Marker.m_Controller != null && m_Marker.m_ControllerInfluence > 0)
            {
                if (m_Marker.m_Controller.m_CurrentCharacter != null)
                    controllerString = m_Marker.m_Controller.m_CurrentCharacter.RawName + " (via " + m_Marker.m_ControllerInfluence.ToString() + " Influence)";
            }

            if (m_Marker.m_Controller == null && m_Marker.m_ControllerInfluence > 0)
                controllerString = "Contested at " + m_Marker.m_ControllerInfluence.ToString() + " Influence";

            string buildingType = InfluenceMarker.GetBuildingTypeName(m_Marker);

            AddLabel(235 + buildingNameOffsetX, 28, boldHue, m_Marker.BuildingName);
            AddLabel(235 + regionNameOffsetX, 47, 2622, regionName);

            AddLabel(40, 73, 2120, @"Controller:");
            if (m_Marker.m_Controller == pm_From.m_InfluenceAccountEntry)
                AddLabel(116, 73, greenTextHue, controllerString); 
            else
                AddLabel(116, 73, textHue, controllerString); 

            AddLabel(40, 98, 2611, @"Control Bonus:");
            if (m_Marker.m_Controller == pm_From.m_InfluenceAccountEntry && m_Marker.m_ControllerInfluence >= m_Marker.minimumInfluenceNeededForControlBonus)
                AddLabel(139, 98, greenTextHue, buildingControlBonusValue.ToString() + " Influence (earned)");
            else
                AddLabel(139, 98, textHue, buildingControlBonusValue.ToString() + " Influence");

            AddLabel(40, 126, 2426, @"Building Type:");
            AddLabel(133, 126, textHue, buildingType);

            string buildingTypeLine1 = "";
            string buildingTypeLine2 = "";

            switch (m_Marker.BuildingType)
            {
                case InfluenceMarker.MarkerBuildingType.Basic:
                    buildingTypeLine1 = "Basic buildings have a 10% chance to gain an additional level of";
                    buildingTypeLine2 = "Prestige or Investment when players increase either here";
                break;

                case InfluenceMarker.MarkerBuildingType.Vendor:
                buildingTypeLine1 = "Vendor buildings have a 25% chance to gain an additional level of";
                    buildingTypeLine2 = "Investment when players increase their Investment here";
                break;

                case InfluenceMarker.MarkerBuildingType.City:
                buildingTypeLine1 = "City buildings have a 25% chance to gain an additional level of";
                    buildingTypeLine2 = "Prestige when players increase their Prestige here";
                break;
            }

            AddLabel(40, 146, textHue, buildingTypeLine1);
            AddLabel(40, 166, textHue, buildingTypeLine2);

            AddLabel(85, 302, boldHue, @"Your");
            AddLabel(57, 322, boldHue, @"Prestige Here");
            AddLabel(98, 342, textHue, prestigeLevel.ToString());

            AddLabel(257, 297, boldHue, @"Your");
            AddLabel(221, 317, boldHue, @"Investment Here");
            AddLabel(270, 337, textHue, investmentLevel.ToString());

            regionNameOffsetX = regionName.Length * 3;

            AddLabel(351, 208, boldHue, @"Your Daily Influence In");
            AddLabel(420 - regionNameOffsetX, 228, boldHue, regionName);
            AddLabel(417, 248, textHue, totalRegionInfluence.ToString());

            AddLabel(354, 297, boldHue, @"Your Daily Influence From");
            AddLabel(383, 317, boldHue, @"This Building");

            if (m_Marker.m_Controller == pm_From.m_InfluenceAccountEntry)
                AddLabel(419, 337, greenTextHue, totalBuildingInfluence.ToString());
            else
                AddLabel(419, 337, textHue, totalBuildingInfluence.ToString());

            AddLabel(41, 385, 2615, @"Increase Prestige Here");
            AddLabel(43, 405, textHue, @"(" + regionPrestigeAvailable + " Prestige Available)");
            AddItem(62, 435, 8827, 2616);
            AddButton(97, 430, 9721, 9724, 1, GumpButtonType.Reply, 0);

            AddLabel(213, 385, 2615, @"Increase Investment Here");
            AddLabel(213, 405, textHue, @"(" + investmentNextLevelCost + " Cost to Increase)");
            AddItem(243, 430, 3823);
            AddButton(284, 430, 9721, 9724, 2, GumpButtonType.Reply, 0);

            AddLabel(425, 405, textHue, @"Return");
            AddButton(432, 430, 9721, 9724, 3, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (m_Marker == null) return;
            if (m_Marker.Deleted) return;
            if (pm_From == null) return;
            if (pm_From.Deleted) return;

            int prestigeLevel = 0;
            int investmentLevel = 0;

            m_Marker.GetPlayerLevels(pm_From, out prestigeLevel, out investmentLevel);

            int regionPrestigeAvailable = InfluencePersistance.GetPlayerPrestigeAvailable(pm_From, m_Marker.MarkerRegion);
            int investmentNextLevelCost = InfluenceMarker.NextInvestmentLevelCost(investmentLevel);

            //Increase Prestige
            if (info.ButtonID == 1)
            {
                if (regionPrestigeAvailable > 0)
                {
                    m_Marker.IncreasePrestige(pm_From, 1);
                    InfluencePersistance.UpdatePlayerPrestigeAvailable(pm_From, m_Marker.MarkerRegion, -1);                    
                }

                else                
                    pm_From.SendMessage("You do not have any available Prestige to spend in this city.");                

                pm_From.CloseAllGumps();
                pm_From.SendGump(new InfluenceMarkerInfluenceBuildingGump(m_Marker, pm_From));
            }

            //Increase Investment
            if (info.ButtonID == 2)
            {
                int bankBalance = Banker.GetBalance(pm_From);

                if (bankBalance >= investmentNextLevelCost)
                {
                    Banker.Withdraw(pm_From, investmentNextLevelCost);
                    m_Marker.IncreaseInvestment(pm_From, 1);                  
                }

                else
                    pm_From.SendMessage("You do not have the neccesary funds in your bank account to increase your Investment Level here.");

                pm_From.CloseAllGumps();
                pm_From.SendGump(new InfluenceMarkerInfluenceBuildingGump(m_Marker, pm_From));
            }

            //Return
            if (info.ButtonID == 3)
            {
                pm_From.CloseAllGumps();
                pm_From.SendGump(new InfluenceMarkerGump(m_Marker, pm_From));
            }
        }
    }

    public class InfluenceMarkerWorldInfluenceGump : Gump
    {
        InfluenceMarker m_Marker;
        PlayerMobile pm_From;

        public bool m_ShowCurrentCycle = true;

        public InfluenceMarkerWorldInfluenceGump(InfluenceMarker marker, Mobile from, bool showCurrentCycle): base(0, 0)
        {
            m_Marker = marker;
            pm_From = from as PlayerMobile;
            m_ShowCurrentCycle = showCurrentCycle;

            if (marker == null || from == null || pm_From == null)
                return;

            if (pm_From.m_InfluenceAccountEntry == null)
                return;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int boldHue = 149;
            int textHue = 2036;
            int greenTextHue = 0x3F;

            AddPage(0);

            #region Background

            AddImage(10, 10, 103);
            AddImage(139, 10, 103);
            AddImage(272, 10, 103);
            AddImage(405, 10, 103);
            AddImage(405, 98, 103);
            AddImage(405, 192, 103);
            AddImage(405, 286, 103);
            AddImage(10, 96, 103);
            AddImage(10, 191, 103);
            AddImage(10, 287, 103);
            AddImage(11, 382, 103);
            AddImage(141, 382, 103);
            AddImage(274, 382, 103);
            AddImage(405, 382, 103);

            AddBackground(21, 22, 515, 450, 3000);
            AddImage(485, 6, 10440, 2500);

            #endregion

            #region Images
            #endregion

            AddLabel(250, 25, boldHue, @"World Influence");

            string cycleText = "";

            if (m_ShowCurrentCycle)
            {
                if (DateTime.UtcNow >= InfluencePersistance.NextCycleCompletion)
                    AddLabel(197, 41, textHue, "Current Cycle: Calculating Results");
                else
                    AddLabel(197, 41, textHue, "Current Cycle: Ends in " + Utility.CreateTimeRemainingString(DateTime.UtcNow, InfluencePersistance.NextCycleCompletion, true, true, true, true, false));
            }

            else
            {
                if (InfluencePersistance.PreviousCycleCompletion != DateTime.MinValue)
                {
                    cycleText = InfluencePersistance.PreviousCycleCompletion.Month.ToString() + "-" + InfluencePersistance.PreviousCycleCompletion.Day.ToString() + "-" + InfluencePersistance.PreviousCycleCompletion.Year.ToString();
                    AddLabel(170, 41, textHue, "Results of Previous Cycle Ending on " + cycleText);
                }

                else
                    AddLabel(170, 41, textHue, "Results of Previous Cycle: No Previous Cycle Yet");
            }

            AddLabel(35, 70, boldHue, @"Region");
            AddLabel(155, 70, boldHue, @"Controller");
            AddLabel(240, 70, boldHue, @"Controller Influence");
            AddLabel(370, 70, boldHue, @"Your Influence");

            List<IndexedRegionName> m_Cities = new List<IndexedRegionName>();

            m_Cities.Add(IndexedRegionName.Britain);
            m_Cities.Add(IndexedRegionName.BuccaneeersDen);
            m_Cities.Add(IndexedRegionName.Cove);
            m_Cities.Add(IndexedRegionName.Jhelom);
            m_Cities.Add(IndexedRegionName.Magincia);
            m_Cities.Add(IndexedRegionName.Minoc);
            m_Cities.Add(IndexedRegionName.Moonglow);
            m_Cities.Add(IndexedRegionName.Nujelm);
            m_Cities.Add(IndexedRegionName.Occlo);
            m_Cities.Add(IndexedRegionName.SerpentsHold);
            m_Cities.Add(IndexedRegionName.SkaraBrae);
            m_Cities.Add(IndexedRegionName.Trinsic);
            m_Cities.Add(IndexedRegionName.Vesper);
            m_Cities.Add(IndexedRegionName.Yew);     
                       
            //Current Cycle
            if (showCurrentCycle)
            {
                //Realm Controller
                int realmTextHue = textHue;

                string realmControllerName = "?";
                string realmControllerInfluence = "?";
                string realmPlayerInfluence = ((int)(Math.Ceiling(pm_From.m_InfluenceAccountEntry.RealmCurrentInfluence))).ToString();
                string realmPlayerInfluencePerDay = "(" + pm_From.m_InfluenceAccountEntry.RealmInfluenceEarnedPerDay.ToString() + "/day)";                

                if (InfluencePersistance.RealmController == pm_From.m_InfluenceAccountEntry)
                {
                    if (pm_From.m_InfluenceAccountEntry.m_CurrentCharacter != null)
                        realmControllerName = pm_From.m_InfluenceAccountEntry.m_CurrentCharacter.RawName;

                    realmControllerInfluence = ((int)(Math.Ceiling(InfluencePersistance.RealmControllerCurrentInfluence))).ToString();
                    realmTextHue = greenTextHue;
                }

                else
                {
                    if (InfluencePersistance.RealmController != null)
                    {
                        if (InfluencePersistance.RealmController.m_CurrentCharacter != null)
                            realmControllerName = InfluencePersistance.RealmController.m_CurrentCharacter.RawName;

                        if (pm_From.AccessLevel > AccessLevel.Player)
                            realmControllerInfluence = ((int)(Math.Ceiling(InfluencePersistance.RealmControllerCurrentInfluence))).ToString();
                    }
                }                

                AddLabel(40, 90, realmTextHue, @"The Realm");
                AddLabel(165, 90, realmTextHue, realmControllerName);
                AddLabel(280, 90, realmTextHue, realmControllerInfluence);

                AddLabel(370, 90, greenTextHue, realmPlayerInfluence);
                AddLabel(420, 90, greenTextHue, realmPlayerInfluencePerDay);

                //City Control
                int startY = 125;
                int intervalY = 20;

                for (int a = 0; a < m_Cities.Count; a++)
                {
                    string sController = "?";
                    string sInfluence = "?";
                    string sPlayerInfluence = "";
                    string sPlayerInfluencePerDay = "";

                    int cityTextHue = textHue;

                    InfluenceAccountEntry controller = InfluencePersistance.GetCityController(m_Cities[a]);
                    double controllerInfluence = InfluencePersistance.GetCityControllerInfluence(m_Cities[a]);

                    if (controller != null)
                    {
                        if (controller.m_CurrentCharacter != null)
                            sController = controller.m_CurrentCharacter.RawName;

                        if (controller == pm_From.m_InfluenceAccountEntry)
                        {
                            sInfluence = ((int)(Math.Ceiling(controllerInfluence))).ToString();
                            cityTextHue = greenTextHue;
                        }

                        if (pm_From.AccessLevel > AccessLevel.Player)
                            sInfluence = ((int)(Math.Ceiling(controllerInfluence))).ToString();
                    }

                    int playerCityInfluence = (int)(Math.Ceiling(InfluencePersistance.GetPlayerCityInfluence(pm_From, m_Cities[a])));
                    int playerCityInfluencePerDay = InfluencePersistance.GetPlayerCityInfluencePerDay(pm_From, m_Cities[a]);

                    sPlayerInfluence = playerCityInfluence.ToString();
                    sPlayerInfluencePerDay = "(" + playerCityInfluencePerDay.ToString() + "/day)";

                    AddLabel(40, startY + (a * intervalY), cityTextHue, InfluencePersistance.GetRegionName(m_Cities[a]));
                    AddLabel(165, startY + (a * intervalY), cityTextHue, sController);
                    AddLabel(280, startY + (a * intervalY), cityTextHue, sInfluence);
                    AddLabel(370, startY + (a * intervalY), greenTextHue, sPlayerInfluence);
                    AddLabel(420, startY + (a * intervalY), greenTextHue, sPlayerInfluencePerDay);
                }
            }

            //Previous Cycle
            else
            {
                //Realm Controller
                int realmTextHue = textHue;

                string realmControllerName = InfluencePersistance.PreviousRealmControllerName;
                string realmControllerInfluence = ((int)(Math.Ceiling(InfluencePersistance.PreviousRealmControllerInfluence))).ToString();
                string realmPlayerInfluence = ((int)(Math.Ceiling(pm_From.m_InfluenceAccountEntry.RealmPreviousInfluence))).ToString();

                if (InfluencePersistance.PreviousRealmController == pm_From.m_InfluenceAccountEntry)
                    realmTextHue = greenTextHue;

                AddLabel(40, 90, realmTextHue, @"The Realm");
                AddLabel(165, 90, realmTextHue, realmControllerName);
                AddLabel(280, 90, realmTextHue, realmControllerInfluence);

                AddLabel(370, 90, greenTextHue, realmPlayerInfluence);

                //City Control
                int startY = 125;
                int intervalY = 20;

                for (int a = 0; a < m_Cities.Count; a++)
                {
                    int cityTextHue = textHue;

                    InfluenceAccountEntry previousController = InfluencePersistance.GetPreviousCityController(m_Cities[a]);
                    string previousControllerName = InfluencePersistance.GetPreviousCityControllerName(m_Cities[a]);
                    int previousControllerInfluence = (int)(Math.Ceiling(InfluencePersistance.GetPreviousCityControllerInfluence(m_Cities[a])));
                    int playerInfluence = (int)(Math.Ceiling(InfluencePersistance.GetPreviousPlayerCityInfluence(pm_From, m_Cities[a])));

                    if (previousController == pm_From.m_InfluenceAccountEntry)
                        cityTextHue = greenTextHue;                                   

                    AddLabel(40, startY + (a * intervalY), cityTextHue, InfluencePersistance.GetRegionName(m_Cities[a]));
                    AddLabel(165, startY + (a * intervalY), cityTextHue, previousControllerName);
                    AddLabel(280, startY + (a * intervalY), cityTextHue, previousControllerInfluence.ToString());
                    AddLabel(370, startY + (a * intervalY), greenTextHue, playerInfluence.ToString());
                }
            }

            if (m_ShowCurrentCycle)
            {
                AddLabel(60, 405, textHue, @"Show Last Cycle");
                AddButton(97, 430, 9721, 9724, 1, GumpButtonType.Reply, 0);
            }

            else
            {
                AddLabel(60, 405, boldHue, @"Show Last Cycle");
                AddButton(97, 430, 9724, 9721, 1, GumpButtonType.Reply, 0);
            }


            if (m_ShowCurrentCycle)
            {
                AddLabel(240, 405, boldHue, @"Show Current Cycle");
                AddButton(284, 430, 9724, 9721, 2, GumpButtonType.Reply, 0);
            }

            else
            {
                AddLabel(240, 405, textHue, @"Show Current Cycle");
                AddButton(284, 430, 9721, 9724, 2, GumpButtonType.Reply, 0);
            }

            AddLabel(425, 405, textHue, @"Return");
            AddButton(432, 430, 9721, 9724, 3, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (m_Marker == null) return;
            if (m_Marker.Deleted) return;
            if (pm_From == null) return;
            if (pm_From.Deleted) return;

            //Show Last Cycle
            if (info.ButtonID == 1)
            {
                m_ShowCurrentCycle = false;

                pm_From.CloseAllGumps();
                pm_From.SendGump(new InfluenceMarkerWorldInfluenceGump(m_Marker, pm_From, m_ShowCurrentCycle));
            }

            //Show Current Cycle
            if (info.ButtonID == 2)
            {
                m_ShowCurrentCycle = true;

                pm_From.CloseAllGumps();
                pm_From.SendGump(new InfluenceMarkerWorldInfluenceGump(m_Marker, pm_From, m_ShowCurrentCycle));
            }

            //Return
            if (info.ButtonID == 3)
            {
                pm_From.CloseAllGumps();
                pm_From.SendGump(new InfluenceMarkerGump(m_Marker, pm_From));
            }
        }
    }

    public class InfluenceMarkerCollectRewardsGump : Gump
    {
        InfluenceMarker m_Marker;
        PlayerMobile pm_From;

        public InfluenceMarkerCollectRewardsGump(InfluenceMarker marker, Mobile from): base(0, 0)
        {
            m_Marker = marker;
            pm_From = from as PlayerMobile;

            if (marker == null || from == null || pm_From == null)
                return;

            if (pm_From.m_InfluenceAccountEntry == null)
                return;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int boldHue = 149;
            int textHue = 2036;
            int greenTextHue = 0x3F;

            AddPage(0);

            #region Background

            AddImage(10, 10, 103);
            AddImage(139, 10, 103);
            AddImage(272, 10, 103);
            AddImage(405, 10, 103);
            AddImage(405, 98, 103);
            AddImage(405, 192, 103);
            AddImage(405, 286, 103);
            AddImage(10, 96, 103);
            AddImage(10, 191, 103);
            AddImage(10, 287, 103);
            AddImage(11, 382, 103);
            AddImage(141, 382, 103);
            AddImage(274, 382, 103);
            AddImage(405, 382, 103);

            AddBackground(21, 22, 515, 450, 3000);
            AddImage(485, 6, 10440, 2500);

            #endregion

            #region Images
            
            AddItem(260, 239, 10784);
            AddItem(239, 219, 10784);
            AddItem(219, 199, 10784);
            AddItem(179, 157, 10784);
            AddItem(199, 178, 10784);
            AddItem(299, 113, 10784);
            AddItem(321, 135, 10784);
            AddItem(302, 152, 10784);
            AddItem(280, 133, 10784);
            AddItem(240, 134, 10784);
            AddItem(280, 174, 10784);
            AddItem(261, 155, 10784);
            AddItem(221, 114, 10784);
            AddItem(220, 156, 10784);
            AddItem(260, 195, 10784);
            AddItem(240, 177, 10784);
            AddItem(200, 136, 10784);
            AddItem(343, 156, 10784);
            AddItem(324, 174, 10784);
            AddItem(302, 195, 10784);
            AddItem(283, 216, 10784);
            AddItem(238, 261, 10784);
            AddItem(217, 241, 10784);
            AddItem(197, 221, 10784);
            AddItem(157, 179, 10784);
            AddItem(177, 200, 10784);
            AddItem(217, 282, 10784);
            AddItem(196, 262, 10784);
            AddItem(176, 242, 10784);
            AddItem(136, 200, 10784);
            AddItem(156, 221, 10784);
            AddItem(282, 259, 10784);
            AddItem(365, 176, 10784);
            AddItem(347, 195, 10784);
            AddItem(325, 216, 10784);
            AddItem(305, 236, 10784);
            AddItem(260, 281, 10784);
            AddItem(239, 302, 10784);
            AddItem(303, 279, 10784);
            AddItem(387, 197, 10784);
            AddItem(368, 215, 10784);
            AddItem(346, 236, 10784);
            AddItem(326, 256, 10784);
            AddItem(281, 301, 10784);
            AddItem(260, 322, 10784);
            AddItem(260, 73, 10784);
            AddItem(240, 95, 10784);
            AddItem(280, 94, 10784);
            AddItem(259, 114, 10784);

            AddItem(260, 134, 8979);
            AddItem(283, 143, 8980);
            AddItem(239, 144, 8981);
            AddItem(261, 200, 8982);

            AddItem(367, 224, 86);
            AddItem(345, 245, 86);
            AddItem(323, 267, 86);
            AddItem(303, 287, 86);
            AddItem(282, 307, 86);
            AddItem(157, 230, 85);
            AddItem(179, 251, 85);
            AddItem(197, 269, 85);
            AddItem(217, 288, 85);
            AddItem(237, 307, 85);
            AddItem(260, 332, 82);
            AddItem(261, 324, 82);
            AddItem(320, 99, 85);
            AddItem(299, 78, 85);
            AddItem(342, 120, 85);
            AddItem(360, 138, 85);
            AddItem(285, 63, 85);
            AddItem(380, 157, 85);
            AddItem(403, 179, 85);
            AddItem(390, 201, 86);
            AddItem(206, 104, 86);
            AddItem(184, 125, 86);
            AddItem(162, 147, 86);
            AddItem(142, 167, 86);
            AddItem(121, 187, 86);
            AddItem(136, 209, 85);
            AddItem(245, 66, 86);
            AddItem(229, 81, 86);
            AddItem(348, 175, 8957);
            AddItem(369, 191, 8956);
            AddItem(163, 206, 8962);
            AddItem(185, 223, 8961);
            AddItem(251, 110, 8929);
            AddItem(238, 231, 8900);
            AddItem(329, 134, 8949);
            AddItem(209, 159, 8905);
            AddItem(328, 219, 8900);
            AddItem(264, 111, 4648, 2500);
            AddItem(264, 165, 4644);
            AddItem(264, 64, 12584);
            AddItem(248, 275, 8905);
            AddItem(293, 237, 8950);
            AddItem(276, 278, 16135);
            AddItem(196, 186, 12585);
            AddItem(312, 181, 16150);
            AddItem(202, 112, 16140);
            AddItem(346, 218, 12586);

            #endregion

            int lotteryTicketsAvailable = pm_From.m_InfluenceAccountEntry.InfluenceLotteryTickets;

            AddLabel(208, 25, boldHue, "Collect Influence Rewards");

            AddLabel(50, 385, 2615, "Collect Lottery Ticket");
            AddLabel(55, 405, textHue, "(" + lotteryTicketsAvailable.ToString() + " Tickets Earned)");
            AddItem(66, 435, 5357, 2616);
            AddButton(97, 430, 9721, 9724, 1, GumpButtonType.Reply, 0);
            
            AddLabel(195, 385, 2615, "Purchase Henchman Recruitment Deed");
            AddLabel(240, 405, textHue, "(Costs 2 Tickets)");
            AddItem(242, 430, 0x14F0, 2587);
            AddButton(284, 430, 9721, 9724, 2, GumpButtonType.Reply, 0);

            AddLabel(425, 405, textHue, "Return");
            AddButton(432, 430, 9721, 9724, 3, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (m_Marker == null) return;
            if (m_Marker.Deleted) return;
            if (pm_From == null) return;
            if (pm_From.Deleted) return;

            if (pm_From.m_InfluenceAccountEntry == null) return;

            int lotteryTicketsAvailable = pm_From.m_InfluenceAccountEntry.InfluenceLotteryTickets;

            //Collect Lottery Ticket
            if (info.ButtonID == 1)
            {
                if (lotteryTicketsAvailable > 0)
                {
                    if (pm_From.TotalItems < 125 && pm_From.TotalWeight < pm_From.MaxWeight)
                    {
                        pm_From.SendMessage("You redeem an Influence Lottery Ticket.");
                        pm_From.m_InfluenceAccountEntry.InfluenceLotteryTickets--;

                        pm_From.SendSound(0x5BC);

                        pm_From.AddToBackpack(new InfluenceLotteryTicket());
                    }

                    else                    
                        pm_From.SendMessage("You would not be able to carry that. You must clear some space in your backpack.");                    
                }

                else
                    pm_From.SendMessage("You do not have any Influence Lottery Tickets available to redeem.");

                pm_From.CloseAllGumps();
                pm_From.SendGump(new InfluenceMarkerCollectRewardsGump(m_Marker, pm_From));
            }

            //Collect Henchman Recruitment Deed
            if (info.ButtonID == 2)
            {
                if (lotteryTicketsAvailable >= 2)
                {
                    if (pm_From.TotalItems < 125 && pm_From.TotalWeight < pm_From.MaxWeight)
                    {
                        pm_From.SendMessage("You convert 2 Lottery Tickets into a Henchman Recruitment Deed.");
                        pm_From.m_InfluenceAccountEntry.InfluenceLotteryTickets -= 2;

                        pm_From.SendSound(0x5BC);

                        pm_From.AddToBackpack(new Custom.HenchmanRecruitmentDeed());
                    }

                    else
                        pm_From.SendMessage("You would not be able to carry that. You must clear some space in your backpack.");
                }

                else
                    pm_From.SendMessage("You do not have enough Influence Lottery Tickets available to convert to a Henchman Recruitment Deed.");

                pm_From.CloseAllGumps();
                pm_From.SendGump(new InfluenceMarkerCollectRewardsGump(m_Marker, pm_From));
            }

            //Return
            if (info.ButtonID == 3)
            {
                pm_From.CloseAllGumps();
                pm_From.SendGump(new InfluenceMarkerGump(m_Marker, pm_From));
            }
        }
    }
}