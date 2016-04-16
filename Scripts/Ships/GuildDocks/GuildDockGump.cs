using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;
using Server.Gumps;
using Server.Guilds;
using Server.Custom;

namespace Server.Items
{    
    public class GuildDockGump : Gump
    {
        PlayerMobile m_Player;

        int m_PageNumber = 1;

        int m_TotalPages = 1;
        int m_TotalEntries = 1;

        int EntriesPerPage = 5;

        public int openGumpSound = 0x055;
        public int changeGumpSound = 0x057;
        public int closeGumpSound = 0x058;
        public int addItemSound = 0x249;
        
        public GuildDockGump(PlayerMobile player, int pageNumber): base(10, 10)
        {
            if (player == null) return;
            if (player.Deleted) return;

            m_Player = player;
            m_PageNumber = pageNumber;
            
            GuildDockGuildInfo guildDockInfo = GuildDockPersistance.GetPlayerGuildDockGuildInfo(m_Player);
                        
            if (guildDockInfo == null) return;            
            if (guildDockInfo.m_Guild == null) return;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddImage(397, 462, 103);
            AddImage(266, 462, 103);
            AddImage(133, 462, 103);
            AddImage(4, 463, 103);
            AddImage(2, 0, 103);
            AddImage(131, 0, 103);
            AddImage(264, 0, 103);
            AddImage(397, 0, 103);
            AddImage(397, 88, 103);
            AddImage(397, 181, 103);
            AddImage(397, 276, 103);
            AddImage(2, 86, 103);
            AddImage(2, 180, 103);
            AddImage(2, 277, 103);
            AddImage(3, 372, 103);
            AddImage(397, 371, 103);
            AddBackground(14, 13, 515, 537, 3000);
            AddItem(477, 459, 16645);
            AddItem(468, 499, 2538);
            AddItem(475, 496, 2554);
            AddItem(480, 504, 2539);
            AddItem(477, 494, 2554);
            
            int WhiteTextHue = 2499;
            int GreenTextHue = 0x3F;               
            
            int m_TotalPages = 1;
            int m_TotalEntries = 1;

            int EntriesPerPage = 5;

            m_TotalEntries = guildDockInfo.m_Upgrades.Count;
            m_TotalPages = (int)(Math.Ceiling((double)m_TotalEntries / (double)EntriesPerPage));

            if (m_TotalPages == 0)
                m_TotalPages = 1;

            if (m_PageNumber < 1)
                m_PageNumber = 1;

            if (m_PageNumber > m_TotalPages)
                m_PageNumber = m_TotalPages;

            string guildName = guildDockInfo.m_Guild.Name + " (" + guildDockInfo.m_Guild.Abbreviation + ")";

            AddLabel(231, 17, 149, "Guild Docks for");
            AddLabel(Utility.CenteredTextOffset(285, guildName), 37, 2599, guildName);

            //Implement Later
            /*
            AddLabel(42, 65, GreenTextHue, "Dock Upgrades");
            AddButton(69, 92, 4018, 248, 1, GumpButtonType.Reply, 0);
            AddLabel(69, 117, GreenTextHue, "View");

            AddLabel(426, 65, WhiteTextHue, "Guild Info");
            AddButton(441, 92, 4017, 248, 2, GumpButtonType.Reply, 0);
            AddLabel(440, 117, WhiteTextHue, "View");
            */

            AddLabel(229, 143, 2603, "Dock Upgrades");

            int iStartY = 140;
            int itemSpacing = 60;

            //Upgrades
            if (m_TotalEntries > 0)
            {
                int startIndex = (m_PageNumber - 1) * EntriesPerPage;
                int endIndex = startIndex + EntriesPerPage;

                if (endIndex > m_TotalEntries)
                    endIndex = m_TotalEntries;

                int entryCount = 0;

                for (int a = startIndex; a < endIndex; a++)
                {
                    if (a < m_TotalEntries)
                    {
                        GuildDockUpgrade guildDockUpgrade = guildDockInfo.m_Upgrades[a];

                        GuildDockUpgradeDetail guildDockUpgradeDetail = GuildDockUpgrades.GetUpgradeDetail(guildDockUpgrade.m_UpgradeType);

                        if (guildDockUpgradeDetail == null)
                            continue;

                        AddItem(109 + guildDockUpgradeDetail.m_IconOffsetX, iStartY + guildDockUpgradeDetail.m_IconOffsetY, guildDockUpgradeDetail.m_IconItemID, guildDockUpgradeDetail.m_IconHue);

                        if (guildDockUpgrade.m_Purchased)
                            AddButton(159, iStartY + 32, 9724, 9724, (10 * entryCount) + 10, GumpButtonType.Reply, 0);
                        else
                            AddButton(159, iStartY + 32, 2151, 2151, (10 * entryCount) + 10, GumpButtonType.Reply, 0);

                        AddLabel(194, iStartY + 28, WhiteTextHue, guildDockUpgradeDetail.m_Name);

                        if (guildDockUpgrade.m_Purchased)
                            AddLabel(195, iStartY + 48, 2550, "Purchased");
                        else
                            AddLabel(195, iStartY + 48, 149, Utility.CreateCurrencyString(guildDockUpgradeDetail.m_Cost) + " Doubloons");

                        AddButton(325, iStartY + 51, 2118, 2118, (10 * entryCount) + 11, GumpButtonType.Reply, 0);
                        AddLabel(344, iStartY + 48, 2599, "Info");

                        iStartY += itemSpacing;

                        entryCount++;
                    }
                }
            }

            AddButton(25, 488, 2151, 9724, 3, GumpButtonType.Reply, 0);
            AddLabel(62, 492, WhiteTextHue, "Deposit");
            AddItem(100, 495, 2539);
            AddLabel(132, 493, 149, "500 Doubloons");
            AddLabel(229, 493, WhiteTextHue, "From Your");
            AddLabel(84, 513, WhiteTextHue, "Bank Into The Guild Treasury");

            AddLabel(387, 473, 149, "Guild Treasury");
            AddItem(374, 496, 2539);
            AddLabel(407, 493, 149, Utility.CreateCurrencyString(guildDockInfo.m_Doubloons));
            AddLabel(402, 513, 149, "Doubloons");

            if (m_PageNumber > 1)
                AddButton(225, 454, 4014, 4016, 4, GumpButtonType.Reply, 0); //Previous Page

            if (m_PageNumber < m_TotalPages)
                AddButton(276, 453, 4005, 4007, 5, GumpButtonType.Reply, 0); //Next Page
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null) return;
            if (m_Player.Deleted) return;
            if (m_Player.Guild == null) return;

            GuildDockGuildInfo guildDockInfo = GuildDockPersistance.GetPlayerGuildDockGuildInfo(m_Player);

            if (guildDockInfo == null)
                return;

            int m_TotalPages = 1;
            int m_TotalEntries = 1;

            int EntriesPerPage = 5;

            m_TotalEntries = guildDockInfo.m_Upgrades.Count;
            m_TotalPages = (int)(Math.Ceiling((double)m_TotalEntries / (double)EntriesPerPage));

            if (m_TotalPages == 0)
                m_TotalPages = 1;

            if (m_PageNumber < 1)
                m_PageNumber = 1;

            if (m_PageNumber > m_TotalPages)
                m_PageNumber = m_TotalPages;

            //------

            bool closeGump = true;

            switch (info.ButtonID)
            {
                case 1: 
                    //View Upgrades
                    closeGump = false;
                break;

                case 2:
                    //View Guild Info
                    closeGump = false;
                break;

                case 3:
                    //Add to Treasury
                    int doubloonsInBank = Banker.GetUniqueCurrencyBalance(m_Player, typeof(Doubloon));

                    if (doubloonsInBank < 500)                    
                        m_Player.SendMessage("You do not have 500 doubloons in your bank.");
                    
                    else
                    {
                        if (Banker.WithdrawUniqueCurrency(m_Player, typeof(Doubloon), 500))
                        {               
                            string message = "You deposit 500 doubloons into the guild treasury from your bank.";

                            int doubloonsRemaining = Banker.GetUniqueCurrencyBalance(m_Player, typeof(Doubloon));

                            message += " You have " + doubloonsRemaining.ToString() + " doubloons remaining.";

                            m_Player.SendMessage(message);
                            m_Player.SendSound(0x2E6);
                        }

                        else
                            m_Player.SendMessage("You do not have 500 doubloons in your bank.");
                    }

                    closeGump = false;
                break;

                case 4:
                    //Previous Page
                    if (m_PageNumber > 1)
                        m_PageNumber--;

                    m_Player.SendSound(changeGumpSound);

                    closeGump = false;
                break;

                case 5:
                    //Next Page
                    if (m_PageNumber < m_TotalPages)
                        m_PageNumber++;

                    m_Player.SendSound(changeGumpSound);

                    closeGump = false;
                break;
            }

            if (info.ButtonID >= 10)
            {
                int rootIndex = info.ButtonID - 10;
                int baseIndex = (int)(Math.Floor((double)rootIndex / 10));
                int remainder = info.ButtonID % 10;

                int index = ((m_PageNumber - 1) * EntriesPerPage) + baseIndex;

                if (index >= guildDockInfo.m_Upgrades.Count || index < 0)
                    return;

                GuildDockUpgrade upgradeEntry = guildDockInfo.m_Upgrades[index];

                if (upgradeEntry == null)
                    return;

                switch (remainder)
                {
                    case 0:
                        if (!upgradeEntry.m_Purchased)
                        {
                        }
                    break; //Purchase

                    case 1:
                    break; //Info               
                }

                closeGump = false;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(GuildDockGump));
                m_Player.SendGump(new GuildDockGump(m_Player, m_PageNumber));
            }

            else
                m_Player.SendSound(closeGumpSound);
        }        
    }

    public class GuildDocksUpgradePurchaseConfirmationGump : Gump
    {
        public PlayerMobile m_Player;
        public Guild m_Guild;
        GuildDockUpgradeType m_UpgradeType;

        public GuildDocksUpgradePurchaseConfirmationGump(PlayerMobile player, Guild guild, GuildDockUpgradeType upgradeType): base(10, 10)
        {
            if (player == null || guild == null)
                return;

            m_Player = player;
            m_Guild = guild;
            m_UpgradeType = upgradeType;

            GuildDockGuildInfo guildDockInfo = GuildDockPersistance.GetPlayerGuildDockGuildInfo(m_Player);
            GuildDockUpgradeDetail upgradeDetail = GuildDockUpgrades.GetUpgradeDetail(m_UpgradeType);

            if (guildDockInfo == null)
                return;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddBackground(8, 8, 484, 228, 9200);
            AddBackground(19, 43, 459, 66, 3000);

            /*
            int textHue = 2036;
            int boldTextHue = 149;

            int iStartY = 43;

            AddLabel(167, 17, boldTextHue, "Unlock Player Customization");

            AddItem(25 + customizationDetail.m_IconOffsetX, iStartY + 13 + customizationDetail.m_IconOffsetY, customizationDetail.m_IconItemId, customizationDetail.m_IconHue); //Icon
            AddLabel(260 - (customizationDetail.m_Name.Length * 3), iStartY + 3, 149, customizationDetail.m_Name);

            if (customizationDetail.m_Description != null)
            {
                for (int a = 0; a < customizationDetail.m_Description.Length; a++)
                {
                    AddLabel(260 - (customizationDetail.m_Description[a].Length * 3), iStartY + ((a + 1) * 20), textHue, customizationDetail.m_Description[a]);
                }
            }

            //AddLabel(260 - (customizationDetail.m_Description[0].Length * 3), iStartY + 20, textHue, customizationDetail.m_Description[0]);
            //AddLabel(260 - (customizationDetail.m_Description[1].Length * 3), iStartY + 40, textHue, customizationDetail.m_Description[1]);

            AddLabel(37, 119, textHue, "This will unlock this Player Customization for all characters on this ");
            AddLabel(29, 139, textHue, "account and will withdraw the following amount of gold from your bank");

            AddItem(185, 163, 3823);
            AddLabel(230, 167, textHue, Utility.CreateCurrencyString(customizationDetail.m_Cost));

            AddLabel(112, 199, textHue, "Confirm");
            AddButton(73, 195, 2152, 2154, 1, GumpButtonType.Reply, 0);

            AddLabel(376, 197, textHue, "Cancel");
            AddButton(337, 193, 2474, 2472, 2, GumpButtonType.Reply, 0);
            */
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null) return;
            if (m_Player.Deleted) return;
            if (m_Guild == null) return;

            //Confirm
            if (info.ButtonID == 1)
            {
                GuildDockGuildInfo guildDockInfo = GuildDockPersistance.GetPlayerGuildDockGuildInfo(m_Player);

                if (guildDockInfo == null)
                {
                    m_Player.SendMessage("That guild no longer exists.");
                    return;
                }

                GuildDockUpgrade guildDockUpgrade = guildDockInfo.GetDockUpgrade(m_UpgradeType);
                GuildDockUpgradeDetail upgradeDetail = GuildDockUpgrades.GetUpgradeDetail(m_UpgradeType);

                if (guildDockUpgrade == null || upgradeDetail == null)
                    return;

                if (guildDockUpgrade.m_Purchased)
                {
                    m_Player.SendMessage("That upgrade has already been purchased.");
                    return;
                }

                if (upgradeDetail.m_Cost > guildDockInfo.m_Doubloons)
                {
                    m_Player.SendMessage("Your guild does not have enough doubloons to purchase that upgrade.");
                    return;
                }

                else
                {
                    guildDockInfo.m_Doubloons -= upgradeDetail.m_Cost;
                    guildDockUpgrade.m_Purchased = true;
                    
                    m_Player.SendMessage("You unlock the upgrade: " + upgradeDetail.m_Name);

                    m_Player.SendSound(0x2E6);

                    guildDockInfo.UnlockUpgrade(m_UpgradeType);
                }

                m_Player.CloseGump(typeof(GuildDockGump));
                m_Player.CloseGump(typeof(GuildDocksUpgradePurchaseConfirmationGump));

                m_Player.SendGump(new GuildDockGump(m_Player, 1));

                return;
            }

            //Cancel
            if (info.ButtonID == 2)
            {
                m_Player.CloseGump(typeof(GuildDocksUpgradePurchaseConfirmationGump));

                return;
            }
        }
    }
}