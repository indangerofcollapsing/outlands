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

        public override void QualityChange()
        {
        }

        public override void ResourceChange()
        {
        }  

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "(" + m_Charges.ToString() + " Uses Remaining)");
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

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

            AddPage(0);

            AddImage(3, 3, 1247);

            AddLabel(173, 27, 2603, "Spyglass");

            string usesText = spyglass.Charges.ToString() + " Uses Remaining";

            AddLabel(Utility.CenteredTextOffset(200, usesText), 47, WhiteTextHue, usesText);

            string searchText = "";

            int iconItemId = 0;
            int iconHue = 0;
            int iconOffsetX = 0;
            int iconOffsetY = 0;

            //Guide
            AddButton(14, 9, 2094, 2095, 1, GumpButtonType.Reply, 0);
            AddLabel(40, 11, 149, "Guide");           

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

            AddLabel(Utility.CenteredTextOffset(205, searchText), 79, 2550, searchText);
            AddButton(114, 122, 4014, 4016, 2, GumpButtonType.Reply, 0);
            AddItem(186 + iconOffsetX, 110 + iconOffsetY, iconItemId, iconHue);
            AddButton(245, 122, 4005, 4007, 3, GumpButtonType.Reply, 0);

            //Search Length
            AddButton(138, 201, 2223, 2223, 4, GumpButtonType.Reply, 0);

            string lengthText = "";

            switch (searchLengthType)
            {
                case Spyglass.SearchLengthType.Short:
                    lengthText = "Short Search";

                    AddLabel(Utility.CenteredTextOffset(200, lengthText), 176, 2599, lengthText);

                    AddItem(172, 198, 5365);
                break;

                case Spyglass.SearchLengthType.Medium:
                    lengthText = "Medium Search";

                    AddLabel(Utility.CenteredTextOffset(200, lengthText), 176, 2599, lengthText);

                    AddItem(165, 198, 5365);
                    AddItem(178, 198, 5365);
                break;

                case Spyglass.SearchLengthType.Long:
                    lengthText = "Long Search";

                    AddLabel(Utility.CenteredTextOffset(200, lengthText), 176, 2599, lengthText);

                    AddItem(159, 198, 5365);
                    AddItem(172, 198, 5365);
                    AddItem(185, 198, 5365);
                break;
            }

            AddButton(232, 201, 2224, 2224, 5, GumpButtonType.Reply, 0);

            //Begin Search
            AddLabel(173, 234, 2600, "Search");
            AddButton(180, 252, 2151, 2151, 6, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Spyglass == null || m_Player == null) return;
            if (m_Spyglass.Deleted) return;

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
                    m_Spyglass.LastSearchMode = m_SearchModeType;
                    m_Spyglass.LastSearchLength = m_SearchLengthType;

                    m_Spyglass.StartSearch(m_Player, m_SearchModeType, m_SearchLengthType);                    
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