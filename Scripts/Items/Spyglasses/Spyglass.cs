using System;
using Server;
using Server.Network;
using Server.Multis;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Server.Gumps;
using Server.Mobiles;
using Server.Custom;
using Server.Engines.Craft;

namespace Server.Items
{
    public class Spyglass : SpyglassMapViewItem
    {
        public enum SearchModeType
        {
            Ships,
            Shipwrecks,
            SchoolsOfFish
        }

        public enum SearchLengthType
        {
            Short,
            Medium,
            Long
        }

        public static int BaseViewDistance = 50;
        public static int BaseWidth = 200;
        public static int BaseHeight = 200;

        private int m_Charges = 100;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get { return m_Charges; }
            set
            {
                m_Charges = value;

                if (m_Charges > MaxCharges)
                    m_Charges = MaxCharges;
            }
        }

        private int m_MaxCharges = 100;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxCharges
        {
            get { return m_MaxCharges; }
            set
            {
                m_MaxCharges = value;

                if (m_Charges > MaxCharges)
                    m_Charges = MaxCharges;
            }
        }

        public SearchModeType LastSearchMode = SearchModeType.Ships;
        public SearchLengthType LastSearchLength = SearchLengthType.Short;

        public DateTime NextUsageAllowed;

        [Constructable]
        public Spyglass(): base()
        {
            Name = "a spyglass";

            ItemID = 5365;

            m_Protected = false;
            m_Editable = false;

            SetDisplay(0, 0, 5119, 4095, BaseWidth, BaseHeight);
        }

        public Spyglass(Serial serial): base(serial)
        {
        }

        public void DetermineUses()
        {
            int uses = 50;

            if (Quality == Quality.Exceptional)
                uses += 25;

            if (DisplayCrafter)
                uses += 25;

            switch (Resource)
            {
                case CraftResource.DullCopper: uses += 25; break;
                case CraftResource.ShadowIron: uses += 25; break;
                case CraftResource.Copper: uses += 25; break;
                case CraftResource.Bronze: uses += 25; break;
                case CraftResource.Gold: uses += 25; break;
                case CraftResource.Agapite: uses += 25; break;
                case CraftResource.Verite: uses += 25; break;
                case CraftResource.Valorite: uses += 25; break;
                case CraftResource.Lunite: uses += 25; break;
            }

            MaxCharges = uses;
            Charges = uses;
        }

        public override void QualityChange()
        {
            DetermineUses();
        }

        public override void ResourceChange()
        {
            Hue = CraftResources.GetCraftResourceHue(Resource);

            DetermineUses();
        }  

        public override void OnSingleClick(Mobile from)
        {
            string spyglassName = "a spyglass";

            if (Resource != CraftResource.None)
                spyglassName = "a " + CraftResources.GetCraftResourceName(Resource).ToLower() + " spyglass";

            LabelTo(from, spyglassName);
            LabelTo(from, "(" + m_Charges.ToString() + " Uses Remaining)");
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That item must be in your pack in order to use it.");
                return;
            }

            if (DateTime.UtcNow < NextUsageAllowed)
            {
                player.SendMessage("That spyglass is currently in use.");
                return;
            }

            if (!player.CanBeginAction(typeof(Spyglass)))
            {
                player.SendMessage("You are already using a spyglass at the moment.");
                return;
            }
            
            player.SendSound(0x055);

            player.CloseGump(typeof(SpyglassGump));
            player.SendGump(new SpyglassGump(this, player, LastSearchMode, LastSearchLength));
        }

        public void StartSearch(PlayerMobile player, SearchModeType searchMode, SearchLengthType searchLength)
        {
            player.SendMessage("You begin searching.");

            int searchIntervals = 1;
            double intervalDuration = 5;

            player.BeginAction(typeof(Spyglass));

            switch (searchLength)
            {
                case SearchLengthType.Short: 
                    NextUsageAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(5);
                    searchIntervals = 1;
                break;
                case SearchLengthType.Medium: 
                    NextUsageAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(15);
                    searchIntervals = 3;
                break;

                case SearchLengthType.Long: 
                    NextUsageAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(30);
                    searchIntervals = 6;
                break;
            }

            for (int a = 0; a < searchIntervals; a++)
            {
                if (a == 0)
                {
                    if (player.NetState != null)
                        player.PrivateOverheadMessage(MessageType.Regular, player.SpeechHue, false, "*begins scanning the horizon*", player.NetState);
                }

                else
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(a * intervalDuration), delegate
                    {
                        if (player != null)
                        {
                            if (player.NetState != null)
                                player.PrivateOverheadMessage(MessageType.Regular, player.SpeechHue, false, "*continues scanning*", player.NetState);
                        }
                    });
                }
            }

            Timer.DelayCall(TimeSpan.FromSeconds(searchIntervals * intervalDuration), delegate
            {
                if (player == null)
                    return;

                    player.EndAction(typeof(Spyglass));

                switch (searchMode)
                {
                    case SearchModeType.Ships: player.Say("Ships");
                    break;

                    case SearchModeType.Shipwrecks: player.Say("Shipwrecks"); 
                    break;

                    case SearchModeType.SchoolsOfFish: player.Say("SchoolsOfFish"); 
                    break;
                }
            });

            Charges--;

            if (Charges <= 0)
                Delete();
        }

        public int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue)
        {
            Quality = (Quality)quality;

            if (makersMark)
                DisplayCrafter = true;

            return quality;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            //Version 0
            writer.Write(m_Charges);
            writer.Write((int)LastSearchMode);
            writer.Write((int)LastSearchLength);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                Charges = reader.ReadInt();
                LastSearchMode = (SearchModeType)reader.ReadInt();
                LastSearchLength = (SearchLengthType)reader.ReadInt();
            }
        }
    }

    public class SpyglassGump : Gump
    {
        public Spyglass m_Spyglass;
        public PlayerMobile m_Player;

        public Spyglass.SearchModeType m_SearchModeType;
        public Spyglass.SearchLengthType m_SearchLengthType;

        public int openGumpSound = 0x055;

        public SpyglassGump(Spyglass spyglass, PlayerMobile player, Spyglass.SearchModeType searchModeType, Spyglass.SearchLengthType searchLengthType): base(10, 10)
        {
            if (spyglass == null || player == null) return;
            if (spyglass.Deleted) return;

            m_Spyglass = spyglass;
            m_Player = player;

            m_SearchModeType = searchModeType;
            m_SearchLengthType = searchLengthType;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int WhiteTextHue = 2655;

            AddImage(3, 3, 1248);               

            string spyglassName = "Spyglass";

            if (spyglass.Resource != CraftResource.None)            
                spyglassName = CraftResources.GetCraftResourceName(spyglass.Resource) + " Spyglass";

            string usesText = spyglass.Charges.ToString() + " Uses Remaining";
            int spyglassHue = spyglass.Hue;
            
            double spyglassQualityRangeBonus = 0;
            double spyglassQualityAccuracyBonus = 0;

            double trackingSkillRangeBonus = 0;
            double trackingSkillAccuracyBonus = 0;

            double cartographySkillRangeBonus = 0;
            double cartographySkillAccuracyBonus = 0;

            double searchLengthRangeBonus = 0;
            double searchLengthAccuracyBonus = 0;

            double totalRangeModifiers = 0;
            double totalAccuracyModifiers = 0;

            if (spyglass.Quality == Quality.Exceptional)
            {
                spyglassQualityRangeBonus += .05;
                spyglassQualityAccuracyBonus += .05;
            }

            switch (spyglass.Resource)
            {
                case CraftResource.DullCopper: 
                    spyglassQualityRangeBonus += .05;
                    spyglassQualityAccuracyBonus += .05;
                break;

                case CraftResource.ShadowIron:
                    spyglassQualityRangeBonus += .10;
                    spyglassQualityAccuracyBonus += .10;
                break;

                case CraftResource.Copper:
                    spyglassQualityRangeBonus += .15;
                    spyglassQualityAccuracyBonus += .15;
                break;

                case CraftResource.Bronze:
                    spyglassQualityRangeBonus += .20;
                    spyglassQualityAccuracyBonus += .20;
                break;

                case CraftResource.Gold:
                    spyglassQualityRangeBonus += .25;
                    spyglassQualityAccuracyBonus += .25;
                break;

                case CraftResource.Agapite:
                    spyglassQualityRangeBonus += .30;
                    spyglassQualityAccuracyBonus += .30;
                break;

                case CraftResource.Verite:
                    spyglassQualityRangeBonus += .35;
                    spyglassQualityAccuracyBonus += .35;
                break;

                case CraftResource.Valorite:
                    spyglassQualityRangeBonus += .40;
                    spyglassQualityAccuracyBonus += .40;
                break;

                case CraftResource.Lunite:
                    spyglassQualityRangeBonus += .45;
                    spyglassQualityAccuracyBonus += .45;
                break; 
            }

            trackingSkillRangeBonus = (player.Skills[SkillName.Tracking].Value / 100) * .25;
            trackingSkillAccuracyBonus = (player.Skills[SkillName.Tracking].Value / 100) * .25;

            cartographySkillRangeBonus = (player.Skills[SkillName.Cartography].Value / 100) * .25;
            cartographySkillAccuracyBonus = (player.Skills[SkillName.Cartography].Value / 100) * .25;

            switch (searchLengthType)
            {
                case Spyglass.SearchLengthType.Short:
                    searchLengthRangeBonus = 0;
                    searchLengthAccuracyBonus = -.25;
                break;

                case Spyglass.SearchLengthType.Medium:
                    searchLengthRangeBonus = .25;
                    searchLengthAccuracyBonus = -.50;
                break;

                case Spyglass.SearchLengthType.Long:
                    searchLengthRangeBonus = .50;
                    searchLengthAccuracyBonus = -.75;
                break;
            }

            //Guide
            AddButton(14, 9, 2094, 2095, 1, GumpButtonType.Reply, 0);
            AddLabel(40, 11, 149, "Guide");

            AddLabel(Utility.CenteredTextOffset(165, spyglassName), 15, spyglassHue, spyglassName);
            AddLabel(Utility.CenteredTextOffset(165, usesText), 40, WhiteTextHue, usesText);

            //Search Type
            string searchText = "";

            int iconItemId = 0;
            int iconHue = 0;
            int iconOffsetX = 0;
            int iconOffsetY = 0;

            //Search Type
            switch (m_SearchModeType)
            {
                case Spyglass.SearchModeType.Ships:
                    searchText = "Search for Ships";

                    iconItemId = 5363;
                    iconHue = 0;
                    iconOffsetX = -8;
                    iconOffsetY = 7;
                break;

                case Spyglass.SearchModeType.Shipwrecks:
                    searchText = "Search for Shipwrecks";

                    iconItemId = 5367;
                    iconHue = 0;
                    iconOffsetX = 0;
                    iconOffsetY = 0;
                break;

                case Spyglass.SearchModeType.SchoolsOfFish:
                    searchText = "Search for Schools of Fish";

                    iconItemId = 3544;
                    iconHue = 0;
                    iconOffsetX = -30;
                    iconOffsetY = 0;
                break;
            }

            AddLabel(Utility.CenteredTextOffset(165, searchText), 70, 2550, searchText);
            AddButton(70, 105, 4014, 4016, 2, GumpButtonType.Reply, 0);
            AddItem(145 + iconOffsetX, 95 + iconOffsetY, iconItemId, iconHue);
            AddButton(210, 105, 4005, 4007, 3, GumpButtonType.Reply, 0);

            //Search Variable
            AddButton(95, 185, 2223, 2223, 4, GumpButtonType.Reply, 0);

            string lengthText = "";

            switch (searchLengthType)
            {
                case Spyglass.SearchLengthType.Short:
                    lengthText = "Short Search";

                    AddLabel(Utility.CenteredTextOffset(160, lengthText), 157, 2599, lengthText);

                    AddItem(130, 182, 5365);
                break;

                case Spyglass.SearchLengthType.Medium:
                    lengthText = "Medium Search";

                    AddLabel(Utility.CenteredTextOffset(160, lengthText), 157, 2599, lengthText);

                    AddItem(122, 182, 5365);
                    AddItem(137, 182, 5365);
                break;

                case Spyglass.SearchLengthType.Long:
                    lengthText = "Large Search";

                    AddLabel(Utility.CenteredTextOffset(160, lengthText), 157, 2599, lengthText);

                    AddItem(115, 182, 5365);
                    AddItem(130, 182, 5365);
                    AddItem(145, 182, 5365);
                break;
            }

            AddButton(195, 185, 2224, 2224, 5, GumpButtonType.Reply, 0);

            //Modifiers Text
            AddLabel(41, 230, 149, "Spyglass Quality");  
            AddLabel(58, 250, 149, "Tracking Skill");
            AddLabel(34, 270, 149, "Cartography Skill");
            AddLabel(51, 290, 149, "Search Length");
            AddLabel(45, 310, 52, "Total Modifiers");

            AddLabel(169, 210, 2114, "Range");
            AddLabel(225, 210, 2577, "Accuracy");

            totalRangeModifiers = spyglassQualityRangeBonus + trackingSkillRangeBonus + cartographySkillRangeBonus + searchLengthRangeBonus;
            totalAccuracyModifiers = spyglassQualityAccuracyBonus + trackingSkillAccuracyBonus + cartographySkillAccuracyBonus + searchLengthAccuracyBonus;

            string spyglassQualityRangeBonusText = "";
            if (spyglassQualityRangeBonus >= 0)
                spyglassQualityRangeBonusText = "+";           
            spyglassQualityRangeBonusText += Utility.CreateDecimalPercentageString(spyglassQualityRangeBonus, 1);

            string spyglassQualityAccuracyBonusText = "";
            if (spyglassQualityAccuracyBonus >= 0)
                spyglassQualityAccuracyBonusText = "+";
            spyglassQualityAccuracyBonusText += Utility.CreateDecimalPercentageString(spyglassQualityAccuracyBonus, 1);

            string trackingSkillRangeBonusText = "";
            if (trackingSkillRangeBonus >= 0)
                trackingSkillRangeBonusText = "+";
            trackingSkillRangeBonusText += Utility.CreateDecimalPercentageString(trackingSkillRangeBonus, 1);

            string trackingSkillAccuracyBonusText = "";
            if (trackingSkillAccuracyBonus >= 0)
                trackingSkillAccuracyBonusText = "+";
            trackingSkillAccuracyBonusText += Utility.CreateDecimalPercentageString(trackingSkillAccuracyBonus, 1);

            string cartographySkillRangeBonusText = "";
            if (cartographySkillRangeBonus >= 0)
                cartographySkillRangeBonusText = "+";
            cartographySkillRangeBonusText += Utility.CreateDecimalPercentageString(cartographySkillRangeBonus, 1);

            string cartographySkillAccuracyBonusText = "";
            if (cartographySkillAccuracyBonus >= 0)
                cartographySkillAccuracyBonusText = "+";
            cartographySkillAccuracyBonusText += Utility.CreateDecimalPercentageString(cartographySkillAccuracyBonus, 1);

            string searchLengthRangeBonusText = "";
            if (searchLengthRangeBonus >= 0)
                searchLengthRangeBonusText = "+";
            searchLengthRangeBonusText += Utility.CreateDecimalPercentageString(searchLengthRangeBonus, 1);

            string searchLengthAccuracyBonusText = "";
            if (searchLengthAccuracyBonus >= 0)
                searchLengthAccuracyBonusText = "+";
            searchLengthAccuracyBonusText += Utility.CreateDecimalPercentageString(searchLengthAccuracyBonus, 1);

            //Total
            string totalRangeModifiersText = "";
            if (totalRangeModifiers >= 0)
                totalRangeModifiersText = "+";
            totalRangeModifiersText += Utility.CreateDecimalPercentageString(totalRangeModifiers, 1);

            string totalAccuracyModifiersText = "";
            if (totalAccuracyModifiers >= 0)
                totalAccuracyModifiersText = "+";
            totalAccuracyModifiersText += Utility.CreateDecimalPercentageString(totalAccuracyModifiers, 1);

            AddLabel(168, 230, WhiteTextHue, spyglassQualityRangeBonusText);
            AddLabel(233, 230, WhiteTextHue, spyglassQualityAccuracyBonusText);

            AddLabel(168, 250, WhiteTextHue, trackingSkillRangeBonusText);
            AddLabel(233, 250, WhiteTextHue, trackingSkillAccuracyBonusText);

            AddLabel(168, 270, WhiteTextHue, cartographySkillRangeBonusText);
            AddLabel(233, 270, WhiteTextHue, cartographySkillAccuracyBonusText);

            AddLabel(168, 290, WhiteTextHue, searchLengthRangeBonusText);
            AddLabel(233, 290, WhiteTextHue, searchLengthAccuracyBonusText);

            AddLabel(168, 310, 52, totalRangeModifiersText);
            AddLabel(233, 310, 52, totalAccuracyModifiersText);

            //Begin Search
            AddLabel(115, 344, 169, "Begin Search");
            AddItem(101, 371, 5365, 0);           
            AddButton(140, 366, 2151, 2151, 6, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Spyglass == null || m_Player == null) return;
            if (m_Spyglass.Deleted) return;
            if (!m_Player.Alive) return;

            bool closeGump = true;

            switch (info.ButtonID)
            {
                //Guide
                case 1:
                    closeGump = false;
                break;

                //Previous Search Type
                case 2:
                    switch(m_SearchModeType)
                    {
                        case Spyglass.SearchModeType.Ships: m_SearchModeType = Spyglass.SearchModeType.SchoolsOfFish; break;
                        case Spyglass.SearchModeType.Shipwrecks: m_SearchModeType = Spyglass.SearchModeType.Ships; break;
                        case Spyglass.SearchModeType.SchoolsOfFish: m_SearchModeType = Spyglass.SearchModeType.Shipwrecks; break;
                    }

                    m_Player.SendSound(0x3E6);

                    closeGump = false;
                break;

                //Next Search Type
                case 3:
                    switch(m_SearchModeType)
                    {
                        case Spyglass.SearchModeType.Ships: m_SearchModeType = Spyglass.SearchModeType.Shipwrecks; break;
                        case Spyglass.SearchModeType.Shipwrecks: m_SearchModeType = Spyglass.SearchModeType.SchoolsOfFish; break;
                        case Spyglass.SearchModeType.SchoolsOfFish: m_SearchModeType = Spyglass.SearchModeType.Ships; break;
                    }

                    m_Player.SendSound(0x3E6);

                    closeGump = false;
                break;

                //Previous Search Length
                case 4:
                    switch (m_SearchLengthType)
                    {
                        case Spyglass.SearchLengthType.Short: m_SearchLengthType = Spyglass.SearchLengthType.Long; break;
                        case Spyglass.SearchLengthType.Medium: m_SearchLengthType = Spyglass.SearchLengthType.Short; break;
                        case Spyglass.SearchLengthType.Long: m_SearchLengthType = Spyglass.SearchLengthType.Medium; break;
                    }

                    m_Player.SendSound(0x3E4);

                    closeGump = false;
                break;

                //Next Search Length
                case 5:
                    switch (m_SearchLengthType)
                    {
                        case Spyglass.SearchLengthType.Short: m_SearchLengthType = Spyglass.SearchLengthType.Medium; break;
                        case Spyglass.SearchLengthType.Medium: m_SearchLengthType = Spyglass.SearchLengthType.Long; break;
                        case Spyglass.SearchLengthType.Long: m_SearchLengthType = Spyglass.SearchLengthType.Short; break;
                    }

                    m_Player.SendSound(0x3E4);

                    closeGump = false;
                break;

                //Begin Search
                case 6:
                    if (m_Player.Backpack != null)
                    {
                        if (!m_Spyglass.IsChildOf(m_Player.Backpack))
                        {
                            m_Player.SendMessage("That item must be in your pack in order to use it.");

                            closeGump = false;
                        }

                        else
                        {
                            m_Spyglass.LastSearchMode = m_SearchModeType;
                            m_Spyglass.LastSearchLength = m_SearchLengthType;

                            m_Spyglass.StartSearch(m_Player, m_SearchModeType, m_SearchLengthType);
                        }
                    }
                break;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(SpyglassGump));
                m_Player.SendGump(new SpyglassGump(m_Spyglass, m_Player, m_SearchModeType, m_SearchLengthType));
            }

            else
            {
                m_Player.SendSound(0x058);
            }
        }
    }
}