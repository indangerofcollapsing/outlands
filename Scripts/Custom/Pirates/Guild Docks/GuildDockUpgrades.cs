using System;
using Server;
using Server.Regions;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using Server.Multis;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Accounting;
using Server.Achievements;

namespace Server
{
    public enum GuildDockUpgradeType
    {
        DailyBountyContractEasy,
        DailyBountyContractChallenging,
        DailyBountyContractDangerous,

        DailyTransportContractEasy,
        DailyTransportContractChallenging,
        DailyTransportContractDangerous,

        Seamstress,
        Carpenter,
        Gunsmith,
        Quartermaster,
        MunitionsSpecialist,        

        SecureContainer1,
        SecureContainer2,
        SecureContainer3,

        AdditionalDock2,
        AdditionalDock3,
        AdditionalDock4,
        AdditionalDock5,
    }

    public static class GuildDockUpgrades
    {
        public static GuildDockUpgradeDetail GetUpgradeDetail(GuildDockUpgradeType upgradeType)
        {
            GuildDockUpgradeDetail upgradeDetail = new GuildDockUpgradeDetail();

            switch (upgradeType)
            {
                case GuildDockUpgradeType.DailyBountyContractEasy:
                    upgradeDetail.m_Name = "Daily Bounty Contract: Easy";
                    upgradeDetail.m_Description = "Unlocks the ability to request an easy ship bounty contract from your dockmaster once per day by saying 'contract' while near them.";

                    upgradeDetail.m_Cost = 1000;

                    upgradeDetail.m_IconItemID = 710;
                    upgradeDetail.m_IconHue = 0;
                    upgradeDetail.m_IconOffsetX = 0;
                    upgradeDetail.m_IconOffsetY = 0;
                break;

                case GuildDockUpgradeType.DailyBountyContractChallenging:
                    upgradeDetail.m_Name = "Daily Bounty Contract: Challenging";
                    upgradeDetail.m_Description = "Unlocks the ability to request a challenging ship bounty contract from your dockmaster once per day by saying 'contract' while near them.";

                    upgradeDetail.m_Cost = 5000;

                    upgradeDetail.m_IconItemID = 710;
                    upgradeDetail.m_IconHue = 2408;
                    upgradeDetail.m_IconOffsetX = 0;
                    upgradeDetail.m_IconOffsetY = 0;
                break;

                case GuildDockUpgradeType.DailyBountyContractDangerous:
                    upgradeDetail.m_Name = "Daily Bounty Contract: Dangerous";
                    upgradeDetail.m_Description = "Unlocks the ability to request a dangerous ship bounty contract from your dockmaster once per day by saying 'contract' while near them.";

                    upgradeDetail.m_Cost = 10000;

                    upgradeDetail.m_IconItemID = 710;
                    upgradeDetail.m_IconHue = 2213;
                    upgradeDetail.m_IconOffsetX = 0;
                    upgradeDetail.m_IconOffsetY = 0;
                break;

                case GuildDockUpgradeType.DailyTransportContractEasy:
                    upgradeDetail.m_Name = "Daily Transport Contract: Easy";
                    upgradeDetail.m_Description = "Unlocks the ability to request an easy ship transport from your dockmaster once per day by saying 'contract' while near them.";

                    upgradeDetail.m_Cost = 1000;

                    upgradeDetail.m_IconItemID = 3644;
                    upgradeDetail.m_IconHue = 0;
                    upgradeDetail.m_IconOffsetX = 0;
                    upgradeDetail.m_IconOffsetY = 23;
                break;

                case GuildDockUpgradeType.DailyTransportContractChallenging:
                    upgradeDetail.m_Name = "Daily Transport Contract: Challenging";
                    upgradeDetail.m_Description = "Unlocks the ability to request a challenging ship transport from your dockmaster once per day by saying 'contract' while near them.";

                    upgradeDetail.m_Cost = 5000;

                    upgradeDetail.m_IconItemID = 3644;
                    upgradeDetail.m_IconHue = 2408;
                    upgradeDetail.m_IconOffsetX = 0;
                    upgradeDetail.m_IconOffsetY = 23;
                break;

                case GuildDockUpgradeType.DailyTransportContractDangerous:
                    upgradeDetail.m_Name = "Daily Transport Contract: Dangerous";
                    upgradeDetail.m_Description = "Unlocks the ability to request a dangerous ship transport contract from your dockmaster once per day by saying 'contract' while near them.";

                    upgradeDetail.m_Cost = 10000;

                    upgradeDetail.m_IconItemID = 3644;
                    upgradeDetail.m_IconHue = 2213;
                    upgradeDetail.m_IconOffsetX = 0;
                    upgradeDetail.m_IconOffsetY = 23;
                break;

                case GuildDockUpgradeType.Seamstress:
                    upgradeDetail.m_Name = "Seamstress";
                    upgradeDetail.m_Description = "Your ships launched from your guild docks receive +10% to Max Sail Points.";

                    upgradeDetail.m_Cost = 5000;

                    upgradeDetail.m_IconItemID = 3644;
                    upgradeDetail.m_IconHue = 2213;
                    upgradeDetail.m_IconOffsetX = 0;
                    upgradeDetail.m_IconOffsetY = 23;
                break;

                case GuildDockUpgradeType.Carpenter:
                    upgradeDetail.m_Name = "Carpenter";
                    upgradeDetail.m_Description = "Your ships launched from your guild docks receive +5% to Max Hull Points.";

                    upgradeDetail.m_Cost = 5000;

                    upgradeDetail.m_IconItemID = 3644;
                    upgradeDetail.m_IconHue = 2213;
                    upgradeDetail.m_IconOffsetX = 0;
                    upgradeDetail.m_IconOffsetY = 23;
                break;

                case GuildDockUpgradeType.Gunsmith:
                    upgradeDetail.m_Name = "Gunsmith";
                    upgradeDetail.m_Description = "Your ships launched from your guild docks receive +10% to Max Gun Points.";

                    upgradeDetail.m_Cost = 5000;

                    upgradeDetail.m_IconItemID = 3644;
                    upgradeDetail.m_IconHue = 2213;
                    upgradeDetail.m_IconOffsetX = 0;
                    upgradeDetail.m_IconOffsetY = 23;
                break;

                case GuildDockUpgradeType.Quartermaster:
                    upgradeDetail.m_Name = "Quartermaster";
                    upgradeDetail.m_Description = "Your ships launched from your guild docks receive +10% to Doubloons earned.";

                    upgradeDetail.m_Cost = 5000;

                    upgradeDetail.m_IconItemID = 3644;
                    upgradeDetail.m_IconHue = 2213;
                    upgradeDetail.m_IconOffsetX = 0;
                    upgradeDetail.m_IconOffsetY = 23;
                break;

                case GuildDockUpgradeType.MunitionsSpecialist:
                    upgradeDetail.m_Name = "Munitions Specialist";
                    upgradeDetail.m_Description = "Your ships launched from your guild docks have their Epic Ability cooldowns reduced by 10%.";

                    upgradeDetail.m_Cost = 5000;

                    upgradeDetail.m_IconItemID = 3644;
                    upgradeDetail.m_IconHue = 2213;
                    upgradeDetail.m_IconOffsetX = 0;
                    upgradeDetail.m_IconOffsetY = 23;
                break;

                case GuildDockUpgradeType.SecureContainer1:
                    upgradeDetail.m_Name = "Secure Container";
                    upgradeDetail.m_Description = "Creates a guild-accessible secure container at each of your guild docks.";

                    upgradeDetail.m_Cost = 5000;

                    upgradeDetail.m_IconItemID = 3644;
                    upgradeDetail.m_IconHue = 2213;
                    upgradeDetail.m_IconOffsetX = 0;
                    upgradeDetail.m_IconOffsetY = 23;
                break;

                case GuildDockUpgradeType.SecureContainer2:
                    upgradeDetail.m_Name = "Secure Container";
                    upgradeDetail.m_Description = "Creates a guild-accessible secure container at each of your guild docks.";

                    upgradeDetail.m_Cost = 5000;

                    upgradeDetail.m_IconItemID = 3644;
                    upgradeDetail.m_IconHue = 2213;
                    upgradeDetail.m_IconOffsetX = 0;
                    upgradeDetail.m_IconOffsetY = 23;
                break;

                case GuildDockUpgradeType.SecureContainer3:
                    upgradeDetail.m_Name = "Secure Container";
                    upgradeDetail.m_Description = "Creates a guild-accessible secure container at each of your guild docks.";

                    upgradeDetail.m_Cost = 5000;

                    upgradeDetail.m_IconItemID = 3644;
                    upgradeDetail.m_IconHue = 2213;
                    upgradeDetail.m_IconOffsetX = 0;
                    upgradeDetail.m_IconOffsetY = 23;
                break;

                case GuildDockUpgradeType.AdditionalDock2:
                    upgradeDetail.m_Name = "Additional Dock";
                    upgradeDetail.m_Description = "Allows 1 additional dock to be placed by another member of your guild." ;

                    upgradeDetail.m_Cost = 5000;

                    upgradeDetail.m_IconItemID = 3644;
                    upgradeDetail.m_IconHue = 2213;
                    upgradeDetail.m_IconOffsetX = 0;
                    upgradeDetail.m_IconOffsetY = 23;
                break;

                case GuildDockUpgradeType.AdditionalDock3:
                    upgradeDetail.m_Name = "Additional Dock";
                    upgradeDetail.m_Description ="Allows 1 additional dock to be placed by another member of your guild.";

                    upgradeDetail.m_Cost = 5000;

                    upgradeDetail.m_IconItemID = 3644;
                    upgradeDetail.m_IconHue = 2213;
                    upgradeDetail.m_IconOffsetX = 0;
                    upgradeDetail.m_IconOffsetY = 23;
                break;

                case GuildDockUpgradeType.AdditionalDock4:
                    upgradeDetail.m_Name = "Additional Dock";
                    upgradeDetail.m_Description = "Allows 1 additional dock to be placed by another member of your guild.";

                    upgradeDetail.m_Cost = 5000;

                    upgradeDetail.m_IconItemID = 3644;
                    upgradeDetail.m_IconHue = 2213;
                    upgradeDetail.m_IconOffsetX = 0;
                    upgradeDetail.m_IconOffsetY = 23;
                break;

                case GuildDockUpgradeType.AdditionalDock5:
                    upgradeDetail.m_Name = "Additional Dock";
                    upgradeDetail.m_Description = "Allows 1 additional dock to be placed by another member of your guild.";

                    upgradeDetail.m_Cost = 5000;

                    upgradeDetail.m_IconItemID = 3644;
                    upgradeDetail.m_IconHue = 2213;
                    upgradeDetail.m_IconOffsetX = 0;
                    upgradeDetail.m_IconOffsetY = 23;
                break;
            }

            return upgradeDetail;
        }        
    }

    public class GuildDockUpgradeDetail
    {
        public string m_Name = "Daily Bounty Contract: Easy";
        public string m_Description = "";

        public int m_Cost = 2500;

        public int m_IconItemID = 710;
        public int m_IconHue = 0;
        public int m_IconOffsetX = 0;
        public int m_IconOffsetY = 0;
    }

    public class GuildDockUpgrade
    {
        public GuildDockUpgradeType m_UpgradeType = GuildDockUpgradeType.DailyBountyContractEasy;
        public bool m_Purchased = false;
        public DateTime m_LastUsage = DateTime.MinValue;       

        public GuildDockUpgrade(GuildDockUpgradeType upgradeType)
        {
            m_UpgradeType = upgradeType;
        }
    }
}