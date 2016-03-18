using System;
using System.Collections;
using System.Collections.Generic;
using Server.Guilds;
using Server.Multis;
using Server.Mobiles;
using Server.Gumps;
using Server.Network;
using Server.Custom;
using Server.Multis;
using Server.Guilds;

namespace Server.Items
{
    public class GuildDockContractsGump : Gump
    {
        public PlayerMobile m_Player;
        public BaseGuildDock m_GuildDock;

        public int ContractsPerPage = 3;

        public int m_CurrentPage = 1;

        public TimeSpan ContractCooldown = TimeSpan.FromDays(24);

        public GuildDockContractsGump(PlayerMobile player, BaseGuildDock guildDock, int currentPage): base(50, 50)
        {
            if (player == null) return;
            if (guildDock == null) return;
            if (guildDock.Deleted) return;

            m_Player = player;
            m_GuildDock = guildDock;

            m_CurrentPage = currentPage;

            GuildDockGuildInfo guildDockInfo = GuildDockPersistance.GetPlayerGuildDockGuildInfo(m_Player);

            if (guildDockInfo == null) return;
            if (guildDockInfo.m_Guild == null) return;
            if (m_Player.AccessLevel == AccessLevel.Player && m_Player.Guild != guildDockInfo.m_Guild) return;

            List<GuildDockUpgrade> m_Upgrades = GuildDockPersistance.GetGuildDocksContractsForPlayer(m_Player);
            
            int TotalEntries = m_Upgrades.Count;
            int TotalPages = (int)(Math.Ceiling((double)m_Upgrades.Count / (double)ContractsPerPage));

            if (TotalPages == 0)
                TotalPages = 1;

            if (m_CurrentPage < 1)            
                m_CurrentPage = 1;               
            

            if (m_CurrentPage > TotalPages)            
                m_CurrentPage = TotalPages;            

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int WhiteTextHue = 2655;
            int GreenTextHue = 0x3F;

            AddImage(2, 0, 103);
            AddImage(131, 0, 103);
            AddImage(260, 0, 103);
            AddImage(260, 87, 103);
            AddImage(261, 180, 103);
            AddImage(141, 180, 103);
            AddImage(2, 180, 103);
            AddImage(2, 85, 103);
            AddBackground(14, 12, 377, 257, 3000);
                       
            //-----

            AddLabel(127, 15, 149, "Contracts Available for");
            AddLabel(134, 35, 2599, m_Player.Guild.Name + "(" + m_Player.Guild.Abbreviation + ")");

            int startY = 70;
            int rowSpacing = 58;

            if (m_Upgrades.Count > 0)
            {
                int startIndex = (m_CurrentPage - 1) * ContractsPerPage;
                int endIndex = startIndex + ContractsPerPage;

                if (endIndex > TotalEntries)
                    endIndex = TotalEntries;

                for (int a = startIndex; a < endIndex; a++)
                {
                    if (a < m_Upgrades.Count)
                    {
                        GuildDockUpgrade guildDockUpgrade = m_Upgrades[a];
                        GuildDockUpgradeDetail upgradeDetail = GuildDockUpgrades.GetUpgradeDetail(guildDockUpgrade.m_UpgradeType);
                        
                        bool available = true;

                        if (guildDockUpgrade.m_LastUsage + ContractCooldown >= DateTime.UtcNow)
                            available = false;      

                        AddItem(40 + upgradeDetail.m_IconOffsetX, startY + upgradeDetail.m_IconOffsetY, upgradeDetail.m_IconItemID, upgradeDetail.m_IconHue);

                        if (available)
                            AddButton(91, startY + 5, 2151, 2154, 10 + a, GumpButtonType.Reply, 0);
                        else
                            AddButton(91, startY + 5, 9721, 9721, 10 + a, GumpButtonType.Reply, 0);

                        AddLabel(125, startY, 0, upgradeDetail.m_Name);
                        
                        if (available)
                            AddLabel(125, startY + 20, 2600, "Available");

                        else
                        {
                            string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, DateTime.UtcNow + ContractCooldown, true, true, true, true, false);
                            AddLabel(125, startY + 20, 2550, "Available in " + timeRemaining);
                        }
                        
                        AddButton(285, startY + 23, 2118, 2117, 20 + a, GumpButtonType.Reply, 0);
                        AddLabel(306, startY, 2599, "Info");

                        startY += rowSpacing;
                    }
                }
            }  

            //Previous Page
            if (m_CurrentPage > 1)            
                AddButton(156, 238, 4014, 4016, 1, GumpButtonType.Reply, 0);

            //Next Page
            if (m_CurrentPage < TotalPages)            
                AddButton(204, 238, 4005, 4007, 2, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null) return;
            if (m_GuildDock == null) return;
            if (m_GuildDock.Deleted) return;

            GuildDockGuildInfo guildDockInfo = GuildDockPersistance.GetPlayerGuildDockGuildInfo(m_Player);

            if (guildDockInfo == null) return;
            if (guildDockInfo.m_Guild == null) return;
            if (m_Player.AccessLevel == AccessLevel.Player && m_Player.Guild != guildDockInfo.m_Guild) return;

            List<GuildDockUpgrade> m_Upgrades = GuildDockPersistance.GetGuildDocksContractsForPlayer(m_Player);

            int TotalEntries = m_Upgrades.Count;
            int TotalPages = (int)(Math.Ceiling((double)m_Upgrades.Count / (double)ContractsPerPage));

            if (TotalPages == 0)
                TotalPages = 1;

            if (m_CurrentPage < 1)
                m_CurrentPage = 1;

            if (m_CurrentPage > TotalPages)
                m_CurrentPage = TotalPages; 

            bool closeGump = true;

            switch (info.ButtonID)
            {
                case 1:
                    //Previous Page
                    if (m_CurrentPage > 1)
                    {
                        m_CurrentPage--;
                        closeGump = false;
                    }
                break;

                case 2:
                    //Next Page
                    if (m_CurrentPage < TotalPages)        
                    {
                        m_CurrentPage++;
                        closeGump = false;
                    }
                break;
            }

            //Select Contract
            if (info.ButtonID >= 10 && info.ButtonID <= 10 + ContractsPerPage)
            {
                int index = info.ButtonID - 10;

                if (m_Upgrades.Count > 0)
                {
                    int startIndex = (m_CurrentPage - 1) * ContractsPerPage;
                    int upgradeIndex = startIndex + index;

                    if (upgradeIndex < m_Upgrades.Count)
                    {
                        GuildDockUpgrade guildDockUpgrade = m_Upgrades[upgradeIndex];
                        GuildDockUpgradeDetail upgradeDetail = GuildDockUpgrades.GetUpgradeDetail(guildDockUpgrade.m_UpgradeType);

                        bool available = true;

                        if (guildDockUpgrade.m_LastUsage + ContractCooldown >= DateTime.UtcNow)
                            available = false;

                        if (available)
                        {
                            if (m_Player.Backpack == null)
                            {
                            }

                            else if (m_Player.Backpack.TotalItems >= m_Player.Backpack.MaxItems)
                                m_Player.SendMessage("You have too many items in your backpack and must make some room to proceed.");

                            else
                            {

                                ShipBountyContract bountyContract;
                                ShipTransportContract transportContract;

                                switch (guildDockUpgrade.m_UpgradeType)
                                {
                                    //Bounty Contracts
                                    case GuildDockUpgradeType.DailyBountyContractEasy:
                                        bountyContract = new ShipBountyContract(ShipBountyContract.BountyContractType.DailyEasy, m_Player, (Guild)m_Player.Guild);

                                        m_Player.Backpack.DropItem(bountyContract);
                                        m_Player.SendMessage("You receive an easy bounty contract.");
                                    break;

                                    case GuildDockUpgradeType.DailyBountyContractChallenging:
                                        bountyContract = new ShipBountyContract(ShipBountyContract.BountyContractType.DailyChallenging, m_Player, (Guild)m_Player.Guild);

                                        m_Player.Backpack.DropItem(bountyContract);
                                        m_Player.SendMessage("You receive a challenging bounty contract.");
                                    break;

                                    case GuildDockUpgradeType.DailyBountyContractDangerous:
                                        bountyContract = new ShipBountyContract(ShipBountyContract.BountyContractType.DailyDangerous, m_Player, (Guild)m_Player.Guild);

                                        m_Player.Backpack.DropItem(bountyContract);
                                        m_Player.SendMessage("You receive a dangerous bounty contract.");
                                    break;

                                    //Transport Contract                               
                                    case GuildDockUpgradeType.DailyTransportContractEasy:
                                        transportContract = new ShipTransportContract(ShipTransportContract.TransportContractType.DailyEasy, m_Player, (Guild)m_Player.Guild);

                                        m_Player.Backpack.DropItem(transportContract);
                                        m_Player.SendMessage("You receive an easy transport contract.");
                                    break;

                                    case GuildDockUpgradeType.DailyTransportContractChallenging:
                                        transportContract = new ShipTransportContract(ShipTransportContract.TransportContractType.DailyChallenging, m_Player, (Guild)m_Player.Guild);

                                        m_Player.Backpack.DropItem(transportContract);
                                        m_Player.SendMessage("You receive a challenging transport contract.");
                                    break;

                                    case GuildDockUpgradeType.DailyTransportContractDangerous:
                                        transportContract = new ShipTransportContract(ShipTransportContract.TransportContractType.DailyDangerous, m_Player, (Guild)m_Player.Guild);

                                        m_Player.Backpack.DropItem(transportContract);
                                        m_Player.SendMessage("You receive a dangerous transport contract.");
                                    break;
                                }

                                guildDockUpgrade.m_LastUsage = DateTime.UtcNow;
                            }                            
                        }

                        else
                        {
                            string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, DateTime.UtcNow + ContractCooldown, false, true, true, true, false);
                            m_Player.SendMessage("You must wait another + " + timeRemaining + " before you may request another contract of that type.");
                        }
                    }
                }

                closeGump = false;
            }

            //Contract Info Event
            if (info.ButtonID >= 20 && info.ButtonID <= 20 + ContractsPerPage)
            {
                int index = info.ButtonID - 20;

                if (m_Upgrades.Count > 0)
                {
                    int startIndex = (m_CurrentPage - 1) * ContractsPerPage;
                    int upgradeIndex = startIndex + index;

                    if (upgradeIndex < m_Upgrades.Count)
                    {
                        GuildDockUpgrade guildDockUpgrade = m_Upgrades[upgradeIndex];
                        GuildDockUpgradeDetail upgradeDetail = GuildDockUpgrades.GetUpgradeDetail(guildDockUpgrade.m_UpgradeType);

                        m_Player.SendMessage(upgradeDetail.m_Description);
                    }
                }

                closeGump = false;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(GuildDockContractsGump));
                m_Player.SendGump(new GuildDockContractsGump(m_Player, m_GuildDock, m_CurrentPage));
            }
        }
    }
}