using System;
using System.Collections.Generic;

using Server.Gumps;
using Server.Commands;
using Server.Network;

using Server.Mobiles;

namespace Server.PortalSystem
{
    public class PortalsAgreementGump : Gump
    {
        private Mobile m_from;
        private DungeonControl m_control;

        enum GumpActions
        {
            GA_Cancel,
            GA_Confirm,
        };

        public PortalsAgreementGump(Mobile from, DungeonControl control)
            : base(100, 100)
        {
            Closable = false;
            Dragable = false;
            m_from = from;
            m_control = control;

            AddPage(0);
            AddBackground(0, 0, 500, 400, 83);

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<BASEFONT COLOR=#000000>Before being issued a portal, you must agree to the following rules:\n\n");
            sb.Append("<BASEFONT COLOR=#BB0000>1. You may not macro inside any portal.\n");
            sb.Append("<BASEFONT COLOR=#BB0000>2. You may not place ghost traps with [hostile] creatures in a location where the creature can not reach an interacting player. (i.e. placing creatures with the intent to free-farm archery or magic resistance)\n");
            sb.Append("<BASEFONT COLOR=#BB0000>3. You agree not to maliciously obstruct players from accessing the portal (i.e. surrounding the portal platform with walls)\n");
            sb.Append("<BASEFONT COLOR=#BB0000>4. You agree not to utilize portals to circumvent any UOAC shard policies, or to use the portal space for personal character gains (i.e. skills, stats, crafting, or financial)\n");
            sb.Append("<BASEFONT COLOR=#BB0000>\nFailure to comply with these rules will result in the termination of your account(s).");
            AddHtml(25, 25, 450, 300, sb.ToString(), true, true);

            AddButton(300, 350, 2453, 2454, (int)GumpActions.GA_Cancel, GumpButtonType.Reply, 0);
            AddButton(370, 350, 2450, 2451, (int)GumpActions.GA_Confirm, GumpButtonType.Reply, 0);
        }
        public override void OnResponse(NetState sender, RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case (int)GumpActions.GA_Cancel:
                    {
                        break;
                    }
                case (int)GumpActions.GA_Confirm:
                    {
                        if (PortalsSystem.RequestPartition(m_from, m_control))
                        {
                            m_from.SendMessage("You are now a portal architect!");
                            DungeonControl.StartControlGump(m_from);
                        }
                        break;
                    }
            }
        }
    }

    public class PartitionsDisplayGump : Gump
    {
        private int m_partitionId;

        enum GumpActions
        {
            GA_GoToPartition = 1,
            GA_ToggleAccessibility,
            GA_DestroyPartition,
        }

        public PartitionsDisplayGump(Mobile from, int partitionId)
            : base(100, 100)
        {
            Closable = true;

            m_partitionId = partitionId;
            Build();
        }
        public PartitionsDisplayGump(Mobile from)
            : base(100, 100)
        {
            Closable = true;

            m_partitionId = -1;
            Build();
        }
        private void Build()
        {
            AddPage(0);
            AddBackground(0, 0, 500, 600, 83);

            // Anchor the segment information relative to the window.
            int anchorX = 30;
            int anchorY = 150;
            int graphicWidth = 19;
            int graphicHeight = 20;

            for (int i = 0; i < PortalsSystem.s_partitionColumnsMax; i++)
                AddLabel(anchorX + i * graphicWidth + 3, anchorY - graphicWidth, 0xFF, i.ToString());
            for (int i = 0; i < PortalsSystem.s_partitionRowsMax; i++)
                AddLabel(anchorX - graphicHeight + 2, anchorY + i * graphicHeight, 0xFF, i.ToString());

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("Every partition is represented by a block. Colored blocks indicate an occupied partition. Formatted by [column],[row].");

            int hue = 1;
            for (int i = 0; i < PortalsSystem.s_partitionsMax; i++)
            {
                int col = i % PortalsSystem.s_partitionColumnsMax;
                int row = (int)(i / PortalsSystem.s_partitionColumnsMax);
                int x = col * graphicWidth + anchorX;
                int y = row * graphicHeight + anchorY;

                PortalPartition p = PortalsSystem.GetPartitionById(i);
                if (p == null)
                    AddImage(x, y, 0xD2);
                else
                {
                    AddImage(x, y, 0xD2, hue++);
                    AddButton(x + 3, y + 3, 0x2716, 0x2716, i + 500, GumpButtonType.Reply, 0);
                    sb.Append("\n");
                    sb.Append(col.ToString());
                    sb.Append(",");
                    sb.Append(row.ToString());
                    sb.Append(" owned by ");
                    sb.Append(p.m_username);
                    sb.Append(" (partitionId: ");
                    sb.Append(p.m_partitionId);
                    sb.Append(")");
                }
            }


            if (m_partitionId > -1)
            {
                PortalPartition p = PortalsSystem.GetPartitionById(m_partitionId);
                if (p == null)
                    return;

                sb.Clear();
                sb.Append(String.Format("Partition owned by {0}.\n", p.m_username));
                sb.Append(String.Format("Ratings: {0}, Average rating: {1}.\n", p.GetRatingCount(), p.GetAverageRating()));
                sb.Append(String.Format("Clients in parition: {0}.\n", p.GetClientsInBounds().Count));
                sb.Append(String.Format("Description:\n{0}\n", p.Description));
                AddHtml(25, 25, 440, 100, sb.ToString(), true, true);

                int buttonX = 20;
                int buttonY = 500;
                int buttonSeparator = 23;
                AddButton(buttonX, buttonY, 4011, 4013, (int)GumpActions.GA_GoToPartition, GumpButtonType.Reply, 0);
                AddLabel(buttonX + 35, buttonY, 0xFF, "Go to partition.");
                buttonY += buttonSeparator;
                AddButton(buttonX, buttonY, 4011, 4013, (int)GumpActions.GA_ToggleAccessibility, GumpButtonType.Reply, 0);
                AddLabel(buttonX + 35, buttonY, 0xFF, p.IsAccessible() ? "Disable this partition." : "Enable this partition.");
                buttonY += buttonSeparator;
                AddButton(buttonX, buttonY, 4011, 4013, (int)GumpActions.GA_DestroyPartition, GumpButtonType.Reply, 0);
                AddLabel(buttonX + 35, buttonY, 0xFF, "Destroy the partition.");
                buttonY += buttonSeparator;
            }
            else
                AddHtml(25, 25, 440, 100, sb.ToString(), true, true);
        }
        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID >= 500 && info.ButtonID <= 1000)
            {
                int partitionId = info.ButtonID - 500;
                PortalPartition partition = PortalsSystem.GetPartitionById(partitionId);
                if (partition == null)
                    sender.Mobile.SendGump(new PartitionsDisplayGump(sender.Mobile));
                else
                    sender.Mobile.SendGump(new PartitionsDisplayGump(sender.Mobile, partitionId));
                return;
            }
            else
            {
                switch (info.ButtonID)
                {
                    case (int)GumpActions.GA_GoToPartition:
                        {
                            PortalPartition partition = PortalsSystem.GetPartitionById(m_partitionId);
                            if (partition == null)
                                sender.Mobile.SendGump(new PartitionsDisplayGump(sender.Mobile));
                            else
                            {
                                Point3D p = partition.GetEntryPosition();
                                sender.Mobile.MoveToWorld(p, PortalsSystem.s_map);
                                sender.Mobile.SendGump(this);
                            }
                            break;
                        }
                    case (int)GumpActions.GA_ToggleAccessibility:
                        {
                            PortalPartition partition = PortalsSystem.GetPartitionById(m_partitionId);
                            if (partition == null)
                                sender.Mobile.SendGump(new PartitionsDisplayGump(sender.Mobile));
                            else
                            {
                                partition.ToggleAccessibility();
                                sender.Mobile.SendGump(new PartitionsDisplayGump(sender.Mobile, m_partitionId));
                            }
                            break;
                        }
                    case (int)GumpActions.GA_DestroyPartition:
                        {
                            PortalsSystem.DestroyPartition(m_partitionId);
                            sender.Mobile.SendGump(new PartitionsDisplayGump(sender.Mobile));
                            break;
                        }
                }
            }
        }
    }

    public class PortalsAdminGump : Gump
    {
        enum GumpPages
        {
            GP_ViewGeneral = 1,
            GP_ViewModifiers,
            GP_ViewStatistics,
            GP_ViewPortalPoints,
            GP_ViewSetup,
            GP_ViewPortalQueue,
            GP_ViewItemization,
            GP_ViewTroubleshooting
        }
        enum GumpActions
        {
            GA_TogglePortals = 1,
            GA_ViewItemization,
            GA_GoToPartition,
            GA_ToggleDungeonAccessibility,
            GA_AddPortalPoint,
            GA_RemovePortalPoint,
            GA_TogglePortalSystem,
            GA_DestroyDungeon,
            GA_CreatePortalVendor,
            GA_CreatePortalSupplierVendor,
            GA_ViewPortalMap,
            GA_ReplaceController,
            GA_ToggleLocalLogging,
            GA_ErasePartitions,
            GA_Confirm,
            GA_Cancel,
        }
        public PortalsAdminGump(Mobile from)
            : base(100, 100)
        {
            int width = 695;
            int height = 512;

            // Background
            AddPage(0);
            AddBackground(44, 47, width, height, 83);

            // Window
            int windowAnchorX = 257;
            int windowAnchorY = 170;
            int windowWidth = 460;
            int windowHeight = 370;
            AddBackground(windowAnchorX, windowAnchorY + 10, 40, windowHeight - 20, 2620); // Graphic patch
            AddBackground(windowAnchorX, windowAnchorY, windowWidth, windowHeight, 2620);

            // Standard options.
            const int buttonX = 68;
            const int labelX = 106;
            const int buttonSeparator = 20;
            int nextButtonY = 68;

            // Standard options.
            AddButton(buttonX, nextButtonY, 4008, 4010, 0, GumpButtonType.Page, (int)GumpPages.GP_ViewGeneral);
            AddLabel(labelX, nextButtonY, 0xFF, "View general");
            nextButtonY += buttonSeparator;
            AddButton(buttonX, nextButtonY, 4008, 4010, 0, GumpButtonType.Page, (int)GumpPages.GP_ViewModifiers);
            AddLabel(labelX, nextButtonY, 0xFF, "View nodifiers");
            nextButtonY += buttonSeparator;
            AddButton(buttonX, nextButtonY, 4008, 4010, 0, GumpButtonType.Page, (int)GumpPages.GP_ViewStatistics);
            AddLabel(labelX, nextButtonY, 0xFF, "View statistics");
            nextButtonY += buttonSeparator;
            AddButton(buttonX, nextButtonY, 4008, 4010, 0, GumpButtonType.Page, (int)GumpPages.GP_ViewPortalPoints);
            AddLabel(labelX, nextButtonY, 0xFF, "View portal points");
            nextButtonY += buttonSeparator;
            AddButton(buttonX, nextButtonY, 4011, 4013, 0, GumpButtonType.Page, (int)GumpPages.GP_ViewSetup);
            AddLabel(labelX, nextButtonY, 0xFF, "View setup");
            nextButtonY += buttonSeparator;
            AddButton(buttonX, nextButtonY, 4011, 4013, (int)GumpActions.GA_ViewPortalMap, GumpButtonType.Reply, 0);
            AddLabel(labelX, nextButtonY, 0xFF, "View partitions");
            nextButtonY += buttonSeparator;
            AddButton(buttonX, nextButtonY, 4011, 4013, (int)GumpActions.GA_ViewItemization, GumpButtonType.Reply, 0);
            AddLabel(labelX, nextButtonY, 0xFF, "View itemization");
            nextButtonY += buttonSeparator;
            AddButton(buttonX, nextButtonY, 4011, 4013, 0, GumpButtonType.Page, (int)GumpPages.GP_ViewTroubleshooting);
            AddLabel(labelX, nextButtonY, 0xFF, "View troubleshooting");

            AddButton(buttonX + 25, 510, 2453, 2454, (int)GumpActions.GA_Cancel, GumpButtonType.Reply, 0);
            AddButton(buttonX + 95, 510, 2450, 2451, (int)GumpActions.GA_Confirm, GumpButtonType.Reply, 0);


            CreateGeneralView(windowAnchorX, windowAnchorY, windowWidth, windowHeight);
            CreateItemizationView(windowAnchorX,windowAnchorY,windowWidth,windowHeight);
            CreateTroubleshootingView(windowAnchorX, windowAnchorY, windowWidth, windowHeight);
            CreateModifersView(windowAnchorX, windowAnchorY, windowWidth, windowHeight);
            CreateStatisticsView(windowAnchorX, windowAnchorY, windowWidth, windowHeight);
            CreatePortalPointsView(windowAnchorX, windowAnchorY, windowWidth, windowHeight);
            CreateSetupView(windowAnchorX, windowAnchorY, windowWidth, windowHeight);
        }

        private void CreateGeneralView(int windowAnchorX, int windowAnchorY, int windowWidth, int windowHeight)
        {
            AddPage(1);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("General customization options.\n");
            sb.Append("Portal frequency: the amount of time in minutes until portal generation.\n");
            sb.Append("Difficulty factor: the % bonus applied per point of difficulty.\n");

            AddHtml(windowAnchorX, 64, windowWidth, 102, sb.ToString(), true, true);

            AddLabel(windowAnchorX + 23, windowAnchorY + 23, 0xFF, "Portal frequency:");
            AddBackground(windowAnchorX + 20, windowAnchorY + 43, 40, 20, 3000);

            int textEntryId = 10;
            AddTextEntry(windowAnchorX + 23, windowAnchorY + 43, 40, 20, 0xFF, textEntryId++, PortalsSystem.AccessManager.m_portalFrequency.ToString());
            AddLabel(windowAnchorX + 143, windowAnchorY + 23, 0xFF, "Difficulty Factor:");
            AddBackground(windowAnchorX + 140, windowAnchorY + 43, 40, 20, 3000);
            AddTextEntry(windowAnchorX + 143, windowAnchorY + 43, 40, 20, 0xFF, textEntryId++, PortalsSystem.AccessManager.m_difficultyFactor.ToString());
        }
        private void CreateItemizationView(int windowAnchorX, int windowAnchorY, int windowWidth, int windowHeight)
        {
            AddPage((int)GumpPages.GP_ViewItemization);

            // Background
            AddBackground(windowAnchorX, windowAnchorY + 10, 40, windowHeight - 20, 2620); // Graphic patch
            AddBackground(windowAnchorX, windowAnchorY, windowWidth, windowHeight, 2620);

            // Header
            string header = "Access to testing kits, random dungeon injectors, and dungeon mining vendors are provided here.";
            AddHtml(windowAnchorX, 64, windowWidth, 102, header, true, true);

            AddButton(windowAnchorX + 23, windowAnchorY + 103, 4029, 4031, (int)GumpActions.GA_CreatePortalVendor, GumpButtonType.Reply, 0);
            AddLabel(windowAnchorX + 77, windowAnchorY + 103, 0xFF, "Create a portal vendor.");
            AddButton(windowAnchorX + 23, windowAnchorY + 123, 4029, 4031, (int)GumpActions.GA_CreatePortalSupplierVendor, GumpButtonType.Reply, 0);
            AddLabel(windowAnchorX + 77, windowAnchorY + 123, 0xFF, "Create a portal supplier vendor.");
        }
        private void CreateTroubleshootingView(int windowAnchorX, int windowAnchorY, int windowWidth, int windowHeight)
        {
            AddPage((int)GumpPages.GP_ViewTroubleshooting);

            // Background
            AddBackground(windowAnchorX, windowAnchorY + 10, 40, windowHeight - 20, 2620); // Graphic patch
            AddBackground(windowAnchorX, windowAnchorY, windowWidth, windowHeight, 2620);

            // Header
            string header = "Troubleshooting tools to address problems with DMS.";
            AddHtml(windowAnchorX, 64, windowWidth, 102, header, true, true);

            // Content
            AddButton(windowAnchorX + 23, windowAnchorY + 23, 4029, 4031, (int)GumpActions.GA_ReplaceController, GumpButtonType.Reply, 0);
            AddLabel(windowAnchorX + 77, windowAnchorY + 23, 0xFF, "Replace a dungeon owner's controller.");
            AddButton(windowAnchorX + 23, windowAnchorY + 43, 4029, 4031, (int)GumpActions.GA_ErasePartitions, GumpButtonType.Reply, 0);
            AddLabel(windowAnchorX + 77, windowAnchorY + 43, 0xFF, "Removes all portals from the system.");
        }
        private void CreateModifersView(int windowAnchorX, int windowAnchorY, int windowWidth, int windowHeight)
        {
            AddPage((int)GumpPages.GP_ViewModifiers);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Clear();
            sb.Append("Region modifiers.\n");
            sb.Append("Difficulty: scalar used by difficulty factor to determine % bonus.\n");
            sb.Append("Base: the base % to spawn a portal.\n");
            sb.Append("Portals Max: the maximum number of simultaneous portals.\n");
            AddHtml(windowAnchorX, 64, windowWidth, 102, sb.ToString(), true, true);

            AddLabel(windowAnchorX + 23, windowAnchorY + 23, 0xFF, "Region:");
            AddLabel(windowAnchorX + 103, windowAnchorY + 23, 0xFF, "Difficulty:");
            AddLabel(windowAnchorX + 183, windowAnchorY + 23, 0xFF, "Base %:");
            AddLabel(windowAnchorX + 263, windowAnchorY + 23, 0xFF, "Portals Max:");

            int baseTextEntry = 20;
            int row = 1;
            foreach (KeyValuePair<PortalAccessManager.ERegion, PortalAccessManager.RegionDetails> kvp in PortalsSystem.AccessManager.m_regionDetailsMap)
            {
                int y = row * 20;
                PortalAccessManager.RegionDetails details = kvp.Value;
                AddLabel(windowAnchorX + 23, windowAnchorY + 23 + y, 0xFF, details.m_region);
                AddBackground(windowAnchorX + 100, windowAnchorY + 23 + y, 30, 20, 3000);
                AddTextEntry(windowAnchorX + 103, windowAnchorY + 23 + y, 30, 20, 0xFF, baseTextEntry++, details.m_difficulty.ToString());
                AddBackground(windowAnchorX + 180, windowAnchorY + 23 + y, 30, 20, 3000);
                AddTextEntry(windowAnchorX + 183, windowAnchorY + 23 + y, 30, 20, 0xFF, baseTextEntry++, details.m_portalBase.ToString());
                AddBackground(windowAnchorX + 260, windowAnchorY + 23 + y, 30, 20, 3000);
                AddTextEntry(windowAnchorX + 263, windowAnchorY + 23 + y, 30, 20, 0xFF, baseTextEntry++, details.m_maxPortals.ToString());

                ++row;
            }
        }
        private void CreateStatisticsView(int windowAnchorX, int windowAnchorY, int windowWidth, int windowHeight)
        {
            AddPage((int)GumpPages.GP_ViewStatistics);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Clear();
            sb.Append("Region statistics.\n");
            sb.Append("T. Kills: total kills accumulated in this region.\n");
            sb.Append("T. Points: total points accumulated in this region.\n");
            sb.Append("S. Kills: kills accumulated during the current session.\n");
            sb.Append("S. Points: points accumulated during the current session.\n");
            sb.Append("%: The current chance for portals to be opened.\n");
            AddHtml(windowAnchorX, 64, windowWidth, 102, sb.ToString(), true, true);

            AddLabel(windowAnchorX + 23, windowAnchorY + 23, 0xFF, "Region:");
            AddLabel(windowAnchorX + 103, windowAnchorY + 23, 0xFF, "T. Kills:");
            AddLabel(windowAnchorX + 183, windowAnchorY + 23, 0xFF, "T. Points:");
            AddLabel(windowAnchorX + 263, windowAnchorY + 23, 0xFF, "S. Kills:");
            AddLabel(windowAnchorX + 343, windowAnchorY + 23, 0xFF, "S. Points:");
            AddLabel(windowAnchorX + 423, windowAnchorY + 23, 0xFF, "%:");

            int row = 1;
            foreach (KeyValuePair<PortalAccessManager.ERegion, PortalAccessManager.RegionStatistic> kvp in PortalsSystem.AccessManager.m_regionStatisticsMap)
            {
                int y = row * 20;
                PortalAccessManager.RegionStatistic statistics = kvp.Value;
                AddLabel(windowAnchorX + 23, windowAnchorY + 23 + y, 0xFF, statistics.m_region);
                AddLabel(windowAnchorX + 103, windowAnchorY + 23 + y, 0xFF, statistics.m_totalKills.ToString());
                AddLabel(windowAnchorX + 183, windowAnchorY + 23 + y, 0xFF, statistics.m_totalPoints.ToString());
                AddLabel(windowAnchorX + 263, windowAnchorY + 23 + y, 0xFF, statistics.m_sessionKills.ToString());
                AddLabel(windowAnchorX + 343, windowAnchorY + 23 + y, 0xFF, statistics.m_sessionPoints.ToString());
                AddLabel(windowAnchorX + 423, windowAnchorY + 23 + y, 0xFF, statistics.m_currentChance.ToString());

                ++row;
            }
        }
        private void CreatePortalPointsView(int windowAnchorX, int windowAnchorY, int windowWidth, int windowHeight)
        {
            AddPage((int)GumpPages.GP_ViewPortalPoints);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Clear();
            sb.Append("Portal points.\n");
            sb.Append("Portals: total portal points in the region.\n");
            AddHtml(windowAnchorX, 64, windowWidth, 102, sb.ToString(), true, true);

            AddLabel(windowAnchorX + 23, windowAnchorY + 23, 0xFF, "Region:");
            AddLabel(windowAnchorX + 103, windowAnchorY + 23, 0xFF, "Portals:");

            int row = 1;
            int idx = 0;
            foreach (List<PortalPoint> portalPointList in PortalsSystem.AccessManager.m_portalPointsLists)
            {
                int y = row * 20;
                string region = PortalsSystem.AccessManager.m_regions[idx];
                AddLabel(windowAnchorX + 23, windowAnchorY + 23 + y, 0xFF, region);
                AddLabel(windowAnchorX + 103, windowAnchorY + 23 + y, 0xFF, portalPointList.Count.ToString());

                ++idx;
                ++row;
            }
        }
        private void CreateSetupView(int windowAnchorX, int windowAnchorY, int windowWidth, int windowHeight)
        {
            AddPage((int)GumpPages.GP_ViewSetup);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            string header = "Setup. Dungeons available for use by portal points are shown in the format [partitionId],[account],[size],[rating].";
            AddHtml(windowAnchorX, 64, windowWidth, 102, header, true, true);

            AddButton(windowAnchorX + 23, windowAnchorY + 23, 4029, 4031, (int)GumpActions.GA_TogglePortals, GumpButtonType.Reply, 0);
            string label = PortalsSystem.AccessManager.m_enabled ? "Disable the portal queue." : "Enable the portal queue.";
            AddLabel(windowAnchorX + 77, windowAnchorY + 23, 0xFF, label);
            AddButton(windowAnchorX + 23, windowAnchorY + 43, 4029, 4031, (int)GumpActions.GA_AddPortalPoint, GumpButtonType.Reply, 0);
            AddLabel(windowAnchorX + 77, windowAnchorY + 43, 0xFF, "Add a portal point.");
            AddButton(windowAnchorX + 23, windowAnchorY + 63, 4029, 4031, (int)GumpActions.GA_RemovePortalPoint, GumpButtonType.Reply, 0);
            AddLabel(windowAnchorX + 77, windowAnchorY + 63, 0xFF, "Remove a portal point.");

            List<PortalPartition> queueList = PortalsSystem.AccessManager.m_eligiblePortals;
            sb.Clear();
            sb.Append("Number of dungeons in the queue: ");
            sb.Append(queueList.Count);
            sb.Append("\n");

            foreach (PortalPartition p in queueList)
            {
                sb.Append("\n");
                sb.Append(p.m_partitionId);
                sb.Append(", ");
                sb.Append(p.m_username);
                sb.Append(", ");
                sb.Append(p.GetAverageRating().ToString("n2"));
            }

            AddHtml(windowAnchorX + 23, windowAnchorY + 95, windowWidth - 45, windowHeight - 160, sb.ToString(), false, true);
        }
        public override void OnResponse(NetState sender, RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case (int)GumpActions.GA_ViewPortalMap:
                    {
                        sender.Mobile.SendGump(this);
                        sender.Mobile.SendGump(new PartitionsDisplayGump(sender.Mobile));
                        break;
                    }
                case (int)GumpActions.GA_ViewItemization:
                    {
                        sender.Mobile.SendGump(this);
                        sender.Mobile.SendGump(new PortalVendorInventoryBrowserGump(PortalsItemization.GetSystemInventory(), String.Empty, String.Empty, 0));
                        break;
                    }
                case (int)GumpActions.GA_TogglePortals:
                    {
                        PortalsSystem.AccessManager.m_enabled = !PortalsSystem.AccessManager.m_enabled;
                        sender.Mobile.SendGump(new PortalsAdminGump(sender.Mobile));
                        break;
                    }

                case (int)GumpActions.GA_AddPortalPoint:
                    {
                        sender.Mobile.Target = new AddPortalPointTarget();
                        break;
                    }
                case (int)GumpActions.GA_RemovePortalPoint:
                    {
                        sender.Mobile.Target = new RemovePortalPointTarget();
                        break;
                    }
                case (int)GumpActions.GA_ErasePartitions:
                    {
                        PortalsSystem.ErasePartitions();
                        sender.Mobile.SendGump(this);
                        break;
                    }
                case (int)GumpActions.GA_ReplaceController:
                    {
                        sender.Mobile.Target = new ReplaceControllerTarget();
                        sender.Mobile.SendGump(this);
                        break;
                    }
                case (int)GumpActions.GA_CreatePortalVendor:
                    {
                        PortalVendor vendor = new PortalVendor();

                        Point3D issuedFrom = sender.Mobile.Location;
                        Map map = sender.Mobile.Map;
                        vendor.MoveToWorld(sender.Mobile.Location, map);

                        sender.Mobile.SendGump(this);
                        break;
                    }
                case (int)GumpActions.GA_CreatePortalSupplierVendor:
                    {
                        PortalSupplierVendor vendor = new PortalSupplierVendor();

                        Point3D issuedFrom = sender.Mobile.Location;
                        Map map = sender.Mobile.Map;
                        vendor.MoveToWorld(sender.Mobile.Location, map);

                        sender.Mobile.SendGump(this);
                        break;
                    }
                case (int)GumpActions.GA_Confirm:
                    {
                        // Frequency
                        TextRelay entry = info.GetTextEntry(10);
                        int iresult = PortalsSystem.AccessManager.m_portalFrequency;
                        if (int.TryParse(entry.Text, out iresult))
                        {
                            PortalsSystem.AccessManager.m_portalFrequency = iresult;
                        }
                        // Frequency
                        entry = info.GetTextEntry(11);
                        float fresult = PortalsSystem.AccessManager.m_difficultyFactor;
                        if (float.TryParse(entry.Text, out fresult))
                        {
                            PortalsSystem.AccessManager.m_difficultyFactor = fresult;
                        }

                        int baseTextEntry = 20;
                        foreach (KeyValuePair<PortalAccessManager.ERegion, PortalAccessManager.RegionDetails> kvp in PortalsSystem.AccessManager.m_regionDetailsMap)
                        {
                            PortalAccessManager.RegionDetails details = kvp.Value;

                            entry = info.GetTextEntry(baseTextEntry);
                            iresult = details.m_difficulty;
                            if (int.TryParse(entry.Text, out iresult))
                            {
                                details.m_difficulty = iresult;
                            }
                            entry = info.GetTextEntry(baseTextEntry+1);
                            iresult = details.m_portalBase;
                            if (int.TryParse(entry.Text, out iresult))
                            {
                                details.m_portalBase = iresult;
                            }
                            entry = info.GetTextEntry(baseTextEntry+2);
                            iresult = details.m_maxPortals;
                            if (int.TryParse(entry.Text, out iresult))
                            {
                                details.m_maxPortals = iresult;
                            }

                            baseTextEntry += 3;
                        }

                        sender.Mobile.SendGump(new PortalsAdminGump(sender.Mobile));
                        break;
                    }
                case (int)GumpActions.GA_Cancel:
                    {
                        break;
                    }
            }
        }
    }



    public class DungeonEnteredGump : Gump
    {
        public struct SDungeonEnteredParams
        {
            public Mobile m_from;
            public Point3D m_returnPoint;
            public Map m_returnMap;
            public string m_partitionAccount;

            public SDungeonEnteredParams(Mobile from, string partitionAccount, Point3D returnPoint, Map returnMap)
            {
                m_from = from;
                m_returnPoint = returnPoint;
                m_returnMap = returnMap;
                m_partitionAccount = partitionAccount;
            }
        };

        public Mobile m_from { get; private set; }
        public Point3D m_returnPoint { get; private set; }
        public Map m_returnMap { get; private set; }
        public string m_partitionAccount { get; private set; }

        enum GumpActions
        {
            RequestLeave = 0,
        };

        public DungeonEnteredGump(SDungeonEnteredParams param)
            : base(380, 400)
        {
            m_from = param.m_from;
            m_returnPoint = param.m_returnPoint;
            m_returnMap = param.m_returnMap;
            m_partitionAccount = param.m_partitionAccount;
            Closable = false;

            PortalPartition partition = PortalsSystem.GetPartitionByUsername(param.m_partitionAccount);
            if (partition == null)
                return;

            string header = partition.Description + "\n\n";

            List<Survey> surveyList = partition.GetSurveys();
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(header);
            foreach (Survey survey in surveyList)
            {
                stringBuilder.Append(survey.m_submitter + "'s review (" + survey.m_rating.ToString() + "/5):\n");
                stringBuilder.Append(survey.m_comment + "\n\n");
            }

            AddPage(0);
            AddBackground(0, 0, 420, 197, 83);
            AddHtml(21, 23, 383, 120, stringBuilder.ToString(), true, true);
            AddLabel(68, 154, 0xFF, @"Leave dungeon. (Requires 5 seconds)");
            AddButton(22, 154, 4026, 4028, (int)GumpActions.RequestLeave, GumpButtonType.Reply, 0);
        }
        public override void OnResponse(NetState sender, RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case (int)GumpActions.RequestLeave:
                    {
                        LeaveTimer timer = new LeaveTimer(m_from, m_returnPoint, m_returnMap);
                        timer.Start();
                        break;
                    }
            }
        }

        private class LeaveTimer : Timer
        {
            private const float m_time = 5.0f;
            private Mobile m_activator;
            private Point3D m_returnPoint;
            private Map m_returnMap;

            public LeaveTimer(Mobile from, Point3D returnPoint, Map returnMap)
                : base(TimeSpan.FromSeconds(m_time))
            {
                m_activator = from;
                m_returnPoint = returnPoint;
                m_returnMap = returnMap;
                Priority = TimerPriority.OneSecond;

                m_activator.SendMessage("You will leave the dungeon in {0} seconds.", m_time);
            }

            protected override void OnTick()
            {
                Point3D dungeonLocation = m_activator.Location;
                Map map = m_activator.Map;

                // Ensure that the player is still in the dungeon.
                if (map == PortalsSystem.s_map)
                {
                    PlayerMobile pm = (PlayerMobile)m_activator;

                    BaseCreature.TeleportPets(m_activator, m_returnPoint, m_returnMap);
                    m_activator.MoveToWorld(m_returnPoint, m_returnMap);
                    m_activator.SendMessage("You have left the dungeon!");
                }

                PortalPartition partition = PortalsSystem.GetPartitionAtPoint(dungeonLocation);
                if (partition != null && partition.m_username != m_activator.Account.Username)
                {
                    DungeonExitedGump gump = new DungeonExitedGump(m_activator, partition.m_username);
                    m_activator.SendGump(gump);
                }
            }
        }
    }

    public class DungeonExitedGump : Gump
    {
        public Mobile m_from { get; private set; }
        public string m_partitionAccount { get; private set; }

        enum GumpActions
        {
            GA_SubmitVote,
        };

        public DungeonExitedGump(Mobile from, string partitionAccount)
            : base(100, 100)
        {
            m_from = from;
            m_partitionAccount = partitionAccount;
            Closable = false;

            AddPage(0);
            AddBackground(0, 0, 420, 197, 83);
            AddBackground(21, 23, 383, 90, 3000);
            AddLabel(68, 114, 0xFF, "Please rate your experience with the dungeon.");

            PortalPartition partition = PortalsSystem.GetPartitionByUsername(m_partitionAccount);
            if (partition == null)
            {
                return;
            }

            // If the player already has a review for this dungeon, replicate the state. Otherwise, provide the
            // default screen.
            Survey survey = partition.GetSurveyByAccount(from.Account.ToString());
            byte lastRating;
            string lastComment;
            if (survey != null)
            {
                lastRating = survey.m_rating;
                lastComment = survey.m_comment;
            }
            else
            {
                lastRating = 0;
                lastComment = "[Please replace this with feedback!]";
            }
            int[] hues = { 0x21, 0x30, 0x35, 0x49, 0x5D };
            for (int i = 1; i < 6; i++)
            {
                AddRadio(60 + i * 40, 137, 9720, 9723, i == lastRating ? true : false, i);
                AddLabel(62 + i * 40, 133, hues[i - 1], i.ToString());
            }
            AddTextEntry(25, 23, 375, 90, 0, 0, lastComment);
            AddButton(300, 140, 2450, 2451, (int)GumpActions.GA_SubmitVote, GumpButtonType.Reply, 0);

        }
        public override void OnResponse(NetState sender, RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case (int)GumpActions.GA_SubmitVote:
                    {
                        TextRelay textRelay = info.GetTextEntry(0);
                        string comment = textRelay.Text.ToString();
                        int voteWeight = 0;
                        for (int i = 1; i < 6; i++)
                        {
                            if (info.IsSwitched(i))
                            {
                                voteWeight = i;
                                break;
                            }
                        }
                        PortalPartition partition = PortalsSystem.GetPartitionByUsername(m_partitionAccount);
                        if (partition == null)
                        {
                            return;
                        }
                        Survey survey = new Survey(m_from.Account.ToString(), m_from.Name, (byte)voteWeight, comment);
                        partition.AddSurvey(survey);

                        break;
                    }
            }

        }
    }

    public class AdminMiniGump : Gump
    {
        public AdminMiniGump()
            : base(20, 680)
        {
            Closable = false;

            AddPage(0);
            AddBackground(0, 0, 40, 52, 9200);
            AddLabel(4, 5, 0, "P");
			AddLabel(4, 27, 0, "A");
            AddButton(19, 7, 2117, 2118, 1, GumpButtonType.Reply, 0);
			AddButton(19, 29, 2117, 2118, 2, GumpButtonType.Reply, 0);
        }
        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (sender.Mobile.AccessLevel < AccessLevel.GameMaster)
                return;
            switch (info.ButtonID)
            {
                case 1:
                    {
                        Commands.CommandSystem.Handle(sender.Mobile, "[portals_admin");
                        break;
                    }
				case 3:
					{
                        Commands.CommandSystem.Handle(sender.Mobile, "[as_admin");
						break;
					}
            }
            sender.Mobile.SendGump(new AdminMiniGump());
        }

        public static void Initialize()
        {
            EventSink.Login += new LoginEventHandler(OnLogin);
        }
        public static void OnLogin(LoginEventArgs e)
        {
            Mobile m = e.Mobile;
            if (m.AccessLevel >= AccessLevel.GameMaster)
                m.SendGump(new AdminMiniGump());
        }

    }
}
