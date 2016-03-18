using System;
using System.Collections.Generic;
using System.Diagnostics;

using Server;
using Server.Items;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using Server.Targeting;
using Server.Accounting;
using Server.Commands;
using Server.Spells;
using Server.Regions;
using Server.Multis;

namespace Server.PortalSystem
{
    public class DungeonControlReplacement : Item
    {
        [Constructable]
        public DungeonControlReplacement()
            : base(DungeonControl.s_gid)
        {
            Hue = 10;
        }
        public DungeonControlReplacement(Serial serial)
            : base(serial)
        {
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
        }
        public override string DefaultName
        {
            get
            {
                return "A dungeon control replacement tool";
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            PortalPartition partition = PortalsSystem.GetPartitionByUsername(from.Account.Username);
            if (partition != null)
            {
                DungeonControl replacementController = new DungeonControl();
                replacementController.m_isActivated = true;
                replacementController.m_username = partition.m_username;

                from.Backpack.AddItem(replacementController);

                from.SendMessage("You have been issued a new controller for your dungeon.");

                this.Delete();
            }
            else
            {
                from.SendMessage("You aren't a dungeon owner. This is useless to you.");
            }
        }
    }

    // This control can produce a gump to manage the inventory, and their dungeon.
    public class DungeonControl : Item
    {
        public static int s_gid = 0x2254;
        public static string s_desc = "portal journeybook";
        public bool m_isActivated;
        public string m_username;

        [Constructable]
        public DungeonControl()
            : base(s_gid)
        {
            m_isActivated = false;
            m_username = "";

            Hue = 0x501;
            Weight = 1.0;
            LootType = LootType.Blessed;
        }
        public DungeonControl(Serial serial)
            : base(serial)
        {
        }
        public static void Initialize()
        {
            CommandSystem.Register("portal", AccessLevel.Player, new CommandEventHandler(OnDoubleClickProxy));
            EventSink.VirtueGumpRequest += new VirtueGumpRequestEventHandler(OnPaperDollClickProxy);
        }
        public override string DefaultName
        {
            get
            {
                return s_desc;
            }
        }
        public void BindToAccount(string username)
        {
            m_isActivated = true;
            m_username = username;
        }
        public PortalsItemization.PortalInventory GetInventory()
        {
            PortalPartition partition = PortalsSystem.GetPartitionByUsername(m_username);
            return partition != null ? partition.m_inventory : null;
        }
        public void FlushInventory()
        {
            PortalPartition partition = PortalsSystem.GetPartitionByUsername(m_username);
            if (partition != null)
            {
                //partition.m_inventory.FlushInventory();
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

            writer.Write(m_isActivated);
            writer.Write(m_username);
        }
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch (version)
            {
                case 0:
                    {
                        m_isActivated = reader.ReadBool();
                        m_username = reader.ReadString();
                        break;
                    }
            }
        }
        // The controller can be moved around the root backpack, and no other location.
        public override bool OnDroppedInto(Mobile from, Container target, Point3D p)
        {
            if (!(target == from.Backpack))
            {
                return false;
            }
            return base.OnDroppedInto(from, target, p);
        }

        public override bool OnDroppedOnto(Mobile from, Item target, Point3D p)
        {
            return false;
        }
        public override bool OnDroppedOnto(Mobile from, Item target)
        {
            return false;
        }
        public override bool OnDroppedToMobile(Mobile from, Mobile target)
        {
            return false;
        }
        public override bool OnDroppedToWorld(Mobile from, Point3D p)
        {
            return false;
        }
        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is PortalInjector)
            {
                if (!m_isActivated)
                {
                    from.SendMessage("This {0} has not been activated.", DungeonControl.s_desc);
                    return false;
                }
                else if (m_username != from.Account.Username)
                {
                    from.SendMessage("This {0} doesn't belong to you.", DungeonControl.s_desc);
                    return false;
                }

                bool result = OnInjection(dropped as PortalInjector);

                // Content was added.
                if (from.HasGump(typeof(PortalControlGump)))
                {
                    from.CloseGump(typeof(PortalControlGump));
                    from.SendGump(new PortalControlGump(from));
                }

                from.SendMessage("Your {0} has expanded!", DungeonControl.s_desc);
                from.Send(new PlaySound(0x249, GetWorldLocation()));

                dropped.Delete();
            }
            return false;
        }
        public bool OnInjection(PortalInjector injector)
        {
            PortalPartition partition = PortalsSystem.GetPartitionByUsername(m_username);
            if (partition == null)
                return false;

            return partition.GetInventory().Inject(injector);
        }
        public static void OnDoubleClickProxy(CommandEventArgs e)
        {
            if (e.Length == 0)
            {
                PortalPartition partition = PortalsSystem.GetPartitionByUsername(e.Mobile.Account.Username);
                if (partition != null)
                {
                    DungeonControl controller = e.Mobile.Backpack.FindItemByType(typeof(DungeonControl)) as DungeonControl;
                    if (controller != null && controller.RootParent == e.Mobile)
                    {
                        controller.OnDoubleClick(e.Mobile);
                    }
                }
            }
            else
            {
                e.Mobile.SendMessage("Format: [portal <no arguments>");
            }
        }
        public static void OnPaperDollClickProxy(VirtueGumpRequestEventArgs e)
        {
            PortalPartition partition = PortalsSystem.GetPartitionByUsername(e.Beholder.Account.Username);
            if (partition != null)
                StartControlGump(e.Beholder);
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (!m_isActivated)
                from.SendGump(new PortalsAgreementGump(from, this));
            else
            {
                if (!(this.RootParent == from))
                {
                    from.SendMessage("The {0} must be accessed from your backpack.", s_desc);
                    return;
                }
                PortalPartition partitionAtLocation = PortalsSystem.GetPartitionAtPoint(from.Location);
                if (partitionAtLocation != null && partitionAtLocation.m_username != from.Account.Username)
                {
                    from.SendMessage("You can not access your {0} from here.", s_desc);
                    return;
                }

                PortalPartition controllersPartition = PortalsSystem.GetPartitionByUsername(this.m_username);
                if (controllersPartition == null)
                {
                    // Under some bizare circumstance, it's possible that the controller's dungeon was wiped, but the
                    // controller remains in the game world. In this instance, it should be removed.
                    this.Delete();
                    from.SendMessage("The {0} disintegrates in your hands!", s_desc);
                    return;
                }
                if (m_username != from.Account.Username)
                {
                    // The controller is not in possession of it's owner.
                    from.SendMessage("You do not have permission to use this.");
                    return;
                }

                StartControlGump(from);
            }
        }
        public static void StartControlGump(Mobile from)
        {
            if (from.HasGump(typeof(PortalControlGump)))
                from.CloseGump(typeof(PortalControlGump));

            from.SendGump(new PortalControlGump(from));
        }
    }

    public class PortalControlGump : Gump
    {
        public static int s_entry_description = 1;

        public Mobile m_from { get; private set; }

        protected Rectangle2D m_inventoryWindowBounds;

        // Navigation
        protected string m_category;
        protected string m_subcategory;
        protected int m_entryId;
        protected int m_selectedInventoryGid; // gid

        // Virtual inventory selections
        public void SetSelectedInventoryGid(int gid)
        {
            PortalPartition partition = PortalsSystem.GetPartitionByUsername(m_from.Account.Username);
            if (partition == null)
                return;

            ClearSelectedPortalElements();

            m_selectedInventoryGid = gid;

            if (gid > 0)
            {
                PortalsItemization.PartitionPortalInventory inventory = partition.GetInventory();
                inventory.AddSelectionHistory(gid);
            }
        }
        public int GetSelectedInventoryGid() { return m_selectedInventoryGid; }

        protected EActiveScreen m_activeScreen;

        // Partition selections
        private List<List<IPortalElement>> m_filterList;
        public List<IPortalElement> m_selectedPortalElements;
        public void SetSelectedPortalElement(IPortalElement element)
        {
            m_selectedPortalElements.Clear();
            m_selectedPortalElements.Add(element);

            PortalPartition partition = PortalsSystem.GetPartitionByUsername(m_from.Account.Username);
            if (partition == null)
                return;

            PortalsItemization.PartitionPortalInventory inventory = partition.GetInventory();
            inventory.AddSelectionHistory(element.GetGid());
        }
        public void SetSelectedPortalElements(List<IPortalElement> elements)
        {
            SetSelectedInventoryGid(0);

            m_selectedPortalElements.Clear();
            m_selectedPortalElements = elements;
        }
        public void ClearSelectedPortalElements()
        {
            m_selectedPortalElements.Clear();
        }
        public List<IPortalElement> GetSelectedPortalElements()
        {
            // copy
            return new List<IPortalElement>(m_selectedPortalElements);
        }

        protected struct ButtonDesc
        {
            public GumpButtonType m_type;
            public string m_name;
            public GumpPages m_pageId;
            public GumpActions m_actionId;
            public int m_upState;
            public int m_downState;
            public ButtonDesc(GumpButtonType type, string name, GumpPages pageId, GumpActions actionId, int upState, int downState)
            {
                m_type = type;
                m_name = name;
                m_pageId = pageId;
                m_actionId = actionId;
                m_upState = upState;
                m_downState = downState;
            }
        }
        protected enum GumpActions
        {
            // 8829-8899 reserved, do not use 0. 8829-8899 is not arbitrary, it's a section of "unused" items.
            GA_Reserve = 8829,
            GA_AddElement,
            GA_AddMultipleElements,
            GA_AdjustElementNegXNegY,
            GA_AdjustElementNegY,
            GA_AdjustElementPosXNegY,
            GA_AdjustElementPosX,
            GA_AdjustElementPosXPosY,
            GA_AdjustElementPosY,
            GA_AdjustElementNegXPosY,
            GA_AdjustElementNegX,
            GA_AdjustElementPosZ,
            GA_AdjustElementNegZ,
            GA_AdjustElementPosZ5,
            GA_AdjustElementNegZ5,
            GA_AdjustElementPosZ20,
            GA_AdjustElementNegZ20,
            GA_NavInventory,
            GA_NavSelection,
            GA_ApplyFilter,
            GA_Teleport,
            GA_EnterPortal,
            GA_LeavePortal,
            GA_EyedropSelectElement,
            GA_SelectInventoryElement,
            GA_SelectPartitionElement,
            GA_SelectMultiplePartitionElement,
            GP_SelectionHistory,
            GA_DeleteElement,
            GA_ReclaimElement,
            GA_ReclaimMultipleElements,
            GA_ReclaimSelection,
            GA_SwapElements,
            GA_MoveElement,
            GA_Config,
            GA_Cancel,
            GA_ConfirmSurrenderPortal,
            GA_ConfirmEraseToInventory,
            GA_ConfirmEraseAndDiscard,
            GA_ConfirmChangeDescription,
            GA_ToggleAvailability,
            GA_ToggleRelativeHeight,
            GA_UndoLastAction,
            GA_RedoLastAction,
            GA_EndReserve = 8899,
        };
        protected enum GumpPages
        {
            GP_ErasePortalToInventory = 9000,
            GP_ErasePortal,
            GP_SurrenderPortal,
            GP_ChangeDescription,
        }
        public enum EActiveScreen
        {
            eAP_Selection, // initial page
            eAP_Inventory,
            eAP_History,
            eAP_Config,
        }

        // Standard constructor
        public PortalControlGump(Mobile from)
            : base(0, 0)
        {
            m_category = "";
            m_subcategory = "";
            m_entryId = 0;
            m_selectedInventoryGid = 0;
            m_selectedPortalElements = new List<IPortalElement>();
            m_filterList = new List<List<IPortalElement>>();
            m_activeScreen = EActiveScreen.eAP_Selection;

            _DungeonControlGump(from);
        }

        public PortalControlGump(PortalControlGump dcg, EActiveScreen activeScreen = EActiveScreen.eAP_Selection)
            : base(0, 0)
        {
            m_category = dcg.m_category;
            m_subcategory = dcg.m_subcategory;
            m_entryId = dcg.m_entryId;
            m_selectedInventoryGid = dcg.m_selectedInventoryGid;
            m_selectedPortalElements = dcg.m_selectedPortalElements;
            m_filterList = dcg.m_filterList;
            m_activeScreen = activeScreen;

            _DungeonControlGump(dcg.m_from);
        }

        protected void CreateContentWindow()
        {
            PortalInventoryGumpUtility.CreateBoundary(m_inventoryWindowBounds, this);
            switch (m_activeScreen)
            {
                case EActiveScreen.eAP_Inventory:
                    {
                        CreateInventory();
                        break;
                    }
                case EActiveScreen.eAP_Selection:
                    {
                        if (m_selectedInventoryGid > 0)
                            CreateInventorySelectionView();
                        else if (m_selectedPortalElements.Count > 0)
                            CreatePortalSelectionView();
                        else
                            PortalInventoryGumpUtility.BuildNavigationPath(m_inventoryWindowBounds, this, "No selection.");
                        break;
                    }
                case EActiveScreen.eAP_History:
                    {
                        CreateSelectionHistory();
                        break;
                    }
                case EActiveScreen.eAP_Config:
                    {
                        CreateConfig();
                        break;
                    }
            }
        }
        public void CreateConfig()
        {
            PortalPartition partition = PortalsSystem.GetPartitionByUsername(m_from.Account.Username);
            if (partition == null)
                return;


            AddPage(1);

            PortalInventoryGumpUtility.BuildNavigationPath(m_inventoryWindowBounds, this, "Config");

            int startButtonX = m_inventoryWindowBounds.X + 5;
            int startButtonY = m_inventoryWindowBounds.Y + 32;
            int buttonX = startButtonX;
            int labelX = buttonX + 35;
            int nextButtonY = startButtonY;
            const int buttonSeparator = 23;

            // Status
            AddButton(buttonX, nextButtonY, 4011, 4013, 0, GumpButtonType.Page, (int)GumpPages.GP_ChangeDescription);
            AddLabel(labelX, nextButtonY, 0xFF, "Change portal description.");
            nextButtonY += buttonSeparator;

            AddButton(buttonX, nextButtonY, 4011, 4013, (int)GumpActions.GA_ToggleAvailability, GumpButtonType.Reply, 0);
            AddLabel(labelX, nextButtonY, 0xFF, partition.IsAccessible() ? "Close the portal to the public." : "Open the portal to the public.");
            nextButtonY += buttonSeparator;

            bool atPortal = partition.InteriorContainsPoint(m_from.Location);
            if (atPortal)
            {
                AddButton(buttonX, nextButtonY, 4011, 4013, (int)GumpActions.GA_ToggleRelativeHeight, GumpButtonType.Reply, 0);
                AddLabel(labelX, nextButtonY, 0xFF, partition.IsUsingRelativeHeight() ? "Place material relative to your character's height." : "Place material relative to cursor target's height.");
                nextButtonY += buttonSeparator;

                AddButton(buttonX, nextButtonY, 4011, 4013, 0, GumpButtonType.Page, (int)GumpPages.GP_ErasePortal);
                AddLabel(labelX, nextButtonY, 0xFF, "Discard placed materials.");
                nextButtonY += buttonSeparator;

                AddButton(buttonX, nextButtonY, 4011, 4013, 0, GumpButtonType.Page, (int)GumpPages.GP_ErasePortalToInventory);
                AddLabel(labelX, nextButtonY, 0xFF, "Return placed materials to inventory.");
                nextButtonY += buttonSeparator;
            }

            AddButton(buttonX, nextButtonY, 4011, 4013, 0, GumpButtonType.Page, (int)GumpPages.GP_SurrenderPortal);
            AddLabel(labelX, nextButtonY, 0xFF, "Abandon and destroy portal.");
            nextButtonY += buttonSeparator;

            // Confirms
            int[] pages = new int[3] {
                (int)GumpPages.GP_ErasePortal,
                (int)GumpPages.GP_ErasePortalToInventory,
                (int)GumpPages.GP_SurrenderPortal,
            };
            int[] actions = new int[3] {
                (int)GumpActions.GA_ConfirmEraseAndDiscard,
                (int)GumpActions.GA_ConfirmEraseToInventory,
                (int)GumpActions.GA_ConfirmSurrenderPortal,
            };
            string[] descriptions = new string[3] {
                "You are about to destroy all materials within the portal. Are you sure?",
                "You are about to reclaim all materials within the portal. Are you sure?",
                "You are about to surrender your portal. Are you sure?"
            };
            string[] headers = new string[3] {
                "Erase and discard",
                "Erase to inventory",
                "Surrender portal"
            };

            startButtonY += 50;
            for (int i = 0; i < 3; ++i)
            {
                AddPage(pages[i]);

                PortalInventoryGumpUtility.BuildNavigationPath(m_inventoryWindowBounds, this, headers[i]);

                AddHtml(m_inventoryWindowBounds.X + 5, m_inventoryWindowBounds.Y + 30, m_inventoryWindowBounds.Width - 10, 50, descriptions[i], true, false);

                buttonX = startButtonX;
                nextButtonY = startButtonY;

                AddButton(buttonX, nextButtonY, 4011, 4013, actions[i], GumpButtonType.Reply, 0);
                AddLabel(labelX, nextButtonY, 0xFF, "Confirm");
                nextButtonY += buttonSeparator;
                AddButton(buttonX, nextButtonY, 4011, 4013, (int)GumpActions.GA_Cancel, GumpButtonType.Reply, 0);
                AddLabel(labelX, nextButtonY, 0xFF, "Cancel");
            }

            // Status
            AddPage((int)GumpPages.GP_ChangeDescription);

            PortalInventoryGumpUtility.BuildNavigationPath(m_inventoryWindowBounds, this, "Change description");

            string header = "To change the description, type the desired text into the empty field below, then confirm.";
            AddHtml(m_inventoryWindowBounds.X + 5, m_inventoryWindowBounds.Y + 30, m_inventoryWindowBounds.Width - 10, 50, header, true, false);

            // current description
            AddLabel(m_inventoryWindowBounds.X + 5, m_inventoryWindowBounds.Y + 80, 0xFF, "Existing description:");
            AddHtml(m_inventoryWindowBounds.X + 5, m_inventoryWindowBounds.Y + 100, m_inventoryWindowBounds.Width - 10, 90, partition.Description, true, false);

            // entry field
            AddLabel(m_inventoryWindowBounds.X + 5, m_inventoryWindowBounds.Y + 190, 0xFF, "Type a new description:");
            AddBackground(m_inventoryWindowBounds.X + 5, m_inventoryWindowBounds.Y + 210, m_inventoryWindowBounds.Width - 10, 90, PortalInventoryGumpUtility.s_backgroundStyle3);
            AddTextEntry(m_inventoryWindowBounds.X + 10, m_inventoryWindowBounds.Y + 210, m_inventoryWindowBounds.Width - 20, 80, 0xFF, s_entry_description, "");

            buttonX = startButtonX;
            nextButtonY = startButtonY + 230;

            AddButton(buttonX, nextButtonY, 4011, 4013, (int)GumpActions.GA_ConfirmChangeDescription, GumpButtonType.Reply, 0);
            AddLabel(labelX, nextButtonY, 0xFF, "Confirm");
            nextButtonY += buttonSeparator;
            AddButton(buttonX, nextButtonY, 4011, 4013, (int)GumpActions.GA_Cancel, GumpButtonType.Reply, 0);
            AddLabel(labelX, nextButtonY, 0xFF, "Cancel");
        }
        public void CreateSelectionHistory()
        {
            PortalPartition partition = PortalsSystem.GetPartitionByUsername(m_from.Account.Username);
            if (partition == null)
                return;

            PortalsItemization.PartitionPortalInventory inventory = partition.GetInventory();
            List<Pair<int, int>> elementsInventoried = inventory.GetSelectionHistory();

            string currentNavPath = String.Format("Record of last {0} selections available in your inventory.", elementsInventoried.Count);
            PortalInventoryGumpUtility.BuildNavigationPath(m_inventoryWindowBounds, this, currentNavPath);

            int entryStartX = m_inventoryWindowBounds.X + PortalInventoryGumpUtility.s_entryStartX;
            int entryStartY = m_inventoryWindowBounds.Y + PortalInventoryGumpUtility.s_entryStartY;
            int x = entryStartX;
            int y = entryStartY;

            foreach (Pair<int, int> pair in elementsInventoried)
            {
                int buttonId = (pair.First << 16) + (int)GumpActions.GA_SelectInventoryElement;
                AddButton(x, y, PortalInventoryGumpUtility.s_buttonSelectUp, PortalInventoryGumpUtility.s_buttonSelectDown, buttonId, GumpButtonType.Reply, 0);
                AddItem(x, y + 20, pair.First);
                AddLabel(x + 20, y, PortalInventoryGumpUtility.s_fontColor, String.Format("x{0}", inventory.GetCount(pair.First, pair.Second)));
                x += PortalInventoryGumpUtility.s_entryColumnSeparation;
                if (x + PortalInventoryGumpUtility.s_entryColumnSeparation > m_inventoryWindowBounds.X + m_inventoryWindowBounds.Width)
                {
                    x = entryStartX;
                    y += PortalInventoryGumpUtility.s_entryRowSeparation;
                }
            }
        }
        public void CreateInventory()
        {
            string username = m_from.Account.Username;
            PortalPartition partition = PortalsSystem.GetPartitionByUsername(username);
            if (partition == null)
                return;

            if (m_category == "")
                PortalInventoryGumpUtility.CreateCategoryPage(m_inventoryWindowBounds, this, partition.GetInventory());
            else if (m_subcategory == "")
                PortalInventoryGumpUtility.CreateSubcategoryPage(m_inventoryWindowBounds, this, partition.GetInventory(), m_category);
            else if (m_entryId == 0)
                PortalInventoryGumpUtility.CreateSubcategoryEntriesPage(m_inventoryWindowBounds, this, partition.GetInventory(), m_category, m_subcategory);
            else
            {
                // Custom logic for inventory selection
                PortalsItemization.PartitionPortalInventory inventory = partition.GetInventory();

                PortalContentEntry entry = PortalsItemization.InventoryLibrary.GetPortalContentEntry(m_entryId);
                int entryId = entry.m_key;

                List<int> entryContents = new List<int>();
                if (entry is PortalRangedContentEntry)
                {
                    PortalRangedContentEntry rangedEntry = entry as PortalRangedContentEntry;
                    foreach (IntervalTree.Interval<int> interval in rangedEntry.m_intervals)
                    {
                        for (int i = interval.Start; i <= interval.End - 1; ++i)
                        {
                            if (inventory.GetCount(i, entryId) > 0)
                                entryContents.Add(i);
                        }
                    }
                }
                else if (entry is PortalSingleContentEntry)
                {
                    PortalSingleContentEntry singleEntry = entry as PortalSingleContentEntry;
                    if (inventory.GetCount(entryId) > 0)
                        entryContents.Add(entryId);
                }

                int relativePage = 1;
                AddPage(relativePage);

                string currentNavPath = String.Format("{0}->{1}->{2}", m_category, m_subcategory, entry.m_name);
                PortalInventoryGumpUtility.BuildNavigationPath(m_inventoryWindowBounds, this, String.Format("{0} (pg.1)", currentNavPath));

                PortalInventoryGumpUtility.BuildInitialReturnButton(m_inventoryWindowBounds, this);

                bool atPortal = partition.InteriorContainsPoint(m_from.Location);

                int entryStartX = m_inventoryWindowBounds.X + PortalInventoryGumpUtility.s_entryStartX;
                int entryStartY = m_inventoryWindowBounds.Y + PortalInventoryGumpUtility.s_entryStartY;
                int x = entryStartX;
                int y = entryStartY;

                for (int i = 0; i < entryContents.Count; ++i)
                {
                    int gid = entryContents[i];
                    int buttonId = (gid << 16) + (int)GumpActions.GA_SelectInventoryElement;

                    if (atPortal)
                        AddButton(x, y, PortalInventoryGumpUtility.s_buttonSelectUp, PortalInventoryGumpUtility.s_buttonSelectDown, buttonId, GumpButtonType.Reply, 0);

                    AddItem(x, y + 20, gid);
                    AddLabel(x + 20, y, PortalInventoryGumpUtility.s_fontColor, String.Format("x{0}", inventory.GetCount(gid, entry.m_key)));
                    x += PortalInventoryGumpUtility.s_entryColumnSeparation;
                    if (x + PortalInventoryGumpUtility.s_entryColumnSeparation > m_inventoryWindowBounds.X + m_inventoryWindowBounds.Width)
                    {
                        x = entryStartX;
                        y += PortalInventoryGumpUtility.s_entryRowSeparation;
                    }
                    if (y + PortalInventoryGumpUtility.s_entryRowSeparation >= m_inventoryWindowBounds.Y + m_inventoryWindowBounds.Height)
                    {
                        x = entryStartX;
                        y = entryStartY;

                        // Complete page, and create the following page's return button and navigation path.
                        PortalInventoryGumpUtility.BuildNextPage(m_inventoryWindowBounds, this, ref relativePage, currentNavPath);
                    }
                }
            }
        }

        public void CreatePortalSelectionView()
        {
            string username = m_from.Account.Username;
            PortalPartition partition = PortalsSystem.GetPartitionByUsername(username);
            if (partition == null)
                return;

            // Controls
            int anchorX = m_inventoryWindowBounds.X + 25;
            int anchorY = m_inventoryWindowBounds.Y + 33;

            AddBackground(m_inventoryWindowBounds.X, anchorY - 7, m_inventoryWindowBounds.Width, 200, 2620);
            AddBackground(m_inventoryWindowBounds.X, anchorY - 7, 320, 200, 2620); // z adjustment
            AddBackground(m_inventoryWindowBounds.X, anchorY - 7, 200, 200, 2620); // xy adjustment
            AddBackground(m_inventoryWindowBounds.X, anchorY - 7, m_inventoryWindowBounds.Width, 33, 2620); // headers

            int headerY = anchorY;
            int xAlignXY = anchorX;
            int xAlignZ = anchorX + 180;
            int xAlignActions = anchorX + 300;
            AddLabel(xAlignXY, headerY, 0xFF, "X/Y Adjust");
            AddLabel(xAlignZ, headerY, 0xFF, "Z Adjust");
            AddLabel(xAlignActions, headerY, 0xFF, "Actions");

            // xy
            int contentY = anchorY + 25;
            int xyButtonBase = 0x1194;
            int baseXYDirectionEnum = (int)GumpActions.GA_AdjustElementNegXNegY;
            int separatorXY = 25;
            List<Pair<int, int>> positions = new List<Pair<int, int>>();
            positions.Add(new Pair<int, int>(separatorXY * 2, 0));
            positions.Add(new Pair<int, int>(separatorXY * 3, separatorXY * 1));
            positions.Add(new Pair<int, int>(separatorXY * 4, separatorXY * 2));
            positions.Add(new Pair<int, int>(separatorXY * 3, separatorXY * 3));
            positions.Add(new Pair<int, int>(separatorXY * 2, separatorXY * 4));
            positions.Add(new Pair<int, int>(separatorXY * 1, separatorXY * 3));
            positions.Add(new Pair<int, int>(0, separatorXY * 2));
            positions.Add(new Pair<int, int>(separatorXY * 1, separatorXY * 1));
            for (int i = 0; i < 8; ++i)
            {
                int xyButton = xyButtonBase + i;
                AddButton(anchorX + positions[i].First, contentY + positions[i].Second, xyButton, xyButton, baseXYDirectionEnum + i, GumpButtonType.Reply, 0);
            }

            // z
            contentY += 10;
            int baseZUpDirectionMod = (int)GumpActions.GA_AdjustElementPosZ;
            int baseZDownDirectionMod = (int)GumpActions.GA_AdjustElementNegZ;
            int zButtonDown = 0x578C;
            int zButtonUp = 0x578F;

            const int zMods = 3;
            int[] zAmounts = new int[zMods] { 1, 5, 20 };
            for (int i = 0; i < zMods; ++i)
            {
                int modX = xAlignZ + (35 * i);
                AddLabel(modX, contentY, 0xFF, String.Format("h:{0}", zAmounts[i].ToString()));
                AddButton(modX, contentY + 25, zButtonUp, zButtonUp + 1, baseZUpDirectionMod + i * 2, GumpButtonType.Reply, 0);
                AddButton(modX, contentY + 75, zButtonDown, zButtonDown + 1, baseZDownDirectionMod + i * 2, GumpButtonType.Reply, 0);
                AddLabel(modX, contentY + 120, 0xFF, String.Format("h:-{0}", zAmounts[i].ToString()));
            }

            // actions
            int buttonX = m_inventoryWindowBounds.X + 325;
            int buttonY = m_inventoryWindowBounds.Y + 58;
            int buttonSeparator = 23;
            AddLabel(buttonX + 35, buttonY, 0xFF, "Reclaim");
            AddButton(buttonX, buttonY, 4011, 4013, (int)GumpActions.GA_ReclaimSelection, GumpButtonType.Reply, 0);
            buttonY += buttonSeparator;
            if (m_selectedPortalElements.Count > 1)
            {
                AddLabel(buttonX + 35, buttonY, 0xFF, "Apply filters");
                AddButton(buttonX, buttonY, 4023, 4025, (int)GumpActions.GA_ApplyFilter, GumpButtonType.Reply, 0);
            }

            int relativePage = 1;
            AddPage(relativePage);

            string currentNavPath = String.Format("{0} elements selected.", m_selectedPortalElements.Count);
            PortalInventoryGumpUtility.BuildNavigationPath(m_inventoryWindowBounds, this, String.Format("{0} {1}",
                currentNavPath, m_selectedPortalElements.Count > 20 ? "(pg.1)" : ""));

            int entryStartX = m_inventoryWindowBounds.X + PortalInventoryGumpUtility.s_entryStartX;
            int entryStartY = m_inventoryWindowBounds.Y + 150 + PortalInventoryGumpUtility.s_entryStartY;
            int x = entryStartX;
            int y = entryStartY;

            if (m_selectedPortalElements.Count > 1)
            {
                // Filters
                Dictionary<int, List<IPortalElement>> typesMap = new Dictionary<int, List<IPortalElement>>();
                foreach (IPortalElement element in m_selectedPortalElements)
                {
                    int gid = element.GetGid();
                    if (!typesMap.ContainsKey(gid))
                    {
                        List<IPortalElement> typeList = new List<IPortalElement>();
                        typeList.Add(element);
                        typesMap.Add(gid, typeList);
                    }
                    else
                    {
                        typesMap[gid].Add(element);
                    }
                }

                List<List<IPortalElement>> completeList = new List<List<IPortalElement>>();

                // Sort list contents by z
                foreach (KeyValuePair<int, List<IPortalElement>> pair in typesMap)
                {
                    List<IPortalElement> typeList = pair.Value;
                    Dictionary<int, List<IPortalElement>> sortedTypesMap = new Dictionary<int, List<IPortalElement>>();
                    foreach (IPortalElement element in typeList)
                    {
                        int z = element.GetLocation().Z;
                        if (!sortedTypesMap.ContainsKey(z))
                        {
                            List<IPortalElement> listByZ = new List<IPortalElement>();
                            listByZ.Add(element);
                            sortedTypesMap.Add(z, listByZ);
                        }
                        else
                            sortedTypesMap[z].Add(element);
                    }

                    // Add every type list by z to the master list.
                    foreach (KeyValuePair<int, List<IPortalElement>> pair2 in sortedTypesMap)
                        completeList.Add(pair2.Value);

                }

                m_filterList = completeList;

                int radioId = 0;
                foreach (List<IPortalElement> subList in completeList)
                {
                    int gid = subList[0].GetGid();
                    int z = subList[0].GetLocation().Z;
                    int len = subList.Count;

                    AddItem(x, y + 20, gid);
                    AddLabel(x, y, 0xFF, String.Format("{0} h:{1}", len.ToString(), z));

                    // z value is packed. z is increemented by 1 so it is non-zero so it can be evaluated.
                    int buttonId = (((z + 1) << 16) + gid);
                    AddCheck(x, y + 20, 0x938, 0x939, true, radioId++);

                    x += PortalInventoryGumpUtility.s_entryColumnSeparation;
                    if (x + PortalInventoryGumpUtility.s_entryColumnSeparation > m_inventoryWindowBounds.X + m_inventoryWindowBounds.Width)
                    {
                        x = entryStartX;
                        y += PortalInventoryGumpUtility.s_entryRowSeparation;
                    }
                    if (y + PortalInventoryGumpUtility.s_entryRowSeparation > m_inventoryWindowBounds.Y + m_inventoryWindowBounds.Height)
                    {
                        x = entryStartX;
                        y = entryStartY;

                        // Complete page, and create the following page's return button and navigation path.
                        PortalInventoryGumpUtility.BuildNextPage(m_inventoryWindowBounds, this, ref relativePage, currentNavPath);
                    }
                }
            }
        }
        public void CreateInventorySelectionView()
        {
            string username = m_from.Account.Username;
            PortalPartition partition = PortalsSystem.GetPartitionByUsername(username);
            if (partition == null)
                return;

            PortalContentEntry entry = PortalsItemization.InventoryLibrary.GetPortalContentEntryWithGid(m_selectedInventoryGid);
            string currentNavPath = String.Format("{0}->{1}->{2}", entry.m_category, entry.m_subcategory, entry.m_name);
            PortalInventoryGumpUtility.BuildNavigationPath(m_inventoryWindowBounds, this, currentNavPath);

            PortalsItemization.PartitionPortalInventory inventory = partition.GetInventory();
            int entryStartX = m_inventoryWindowBounds.X + PortalInventoryGumpUtility.s_entryStartX;
            int entryStartY = m_inventoryWindowBounds.Y + PortalInventoryGumpUtility.s_entryStartY;
            AddItem(entryStartX, entryStartY + 20, m_selectedInventoryGid);
            AddLabel(entryStartX + 20, entryStartY, PortalInventoryGumpUtility.s_fontColor, String.Format("x{0}", inventory.GetCount(m_selectedInventoryGid, entry.m_key)));

            bool atPortal = partition.InteriorContainsPoint(m_from.Location);
            if (atPortal)
            {
                // Selection buttons
                ButtonDesc[] invSelectionButtons =
                {
                    new ButtonDesc(GumpButtonType.Reply, "Place", 0, GumpActions.GA_AddElement, 0xFA5, 0xFA7),
                    new ButtonDesc(GumpButtonType.Reply, "Place X", 0, GumpActions.GA_AddMultipleElements, 0xFA5, 0xFA7)
                };

                int buttonX = entryStartX - 8;
                int labelX = buttonX + 35;
                int nextButtonY = entryStartY + 150;
                for (int i = 0; i < invSelectionButtons.Length; ++i)
                {
                    AddButton(buttonX, nextButtonY, invSelectionButtons[i].m_upState, invSelectionButtons[i].m_downState, (int)invSelectionButtons[i].m_actionId, GumpButtonType.Reply, 0);
                    AddLabel(labelX, nextButtonY, 0xFF, invSelectionButtons[i].m_name);
                    nextButtonY += 23;

                    // Doors can only be placed one at a time.
                    if (entry.m_behavior == PortalContentEntry.s_behavior_door)
                        break;
                }
            }
        }
        public void _DungeonControlGump(Mobile from)
        {
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            m_from = from;

            string username = from.Account.Username;
            PortalPartition partition = PortalsSystem.GetPartitionByUsername(username);
            if (partition == null)
                return;

            // Possession of control is required.
            Item item = from.Backpack.FindItemByType(typeof(DungeonControl));
            if (item == null)
                return;
            DungeonControl control = item as DungeonControl;

            string header = "";

            AddPage(0);
            AddBackground(0, 0, 700, 600, 83);
            AddHtml(10, 10, 680, 60, header, true, false);
            m_inventoryWindowBounds = new Rectangle2D(160, 70, 530, 520);

            ButtonDesc[] generalButtons =
            {
                new ButtonDesc(GumpButtonType.Reply, "Config", 0, GumpActions.GA_Config, 0xFAB, 0xFAD),
                new ButtonDesc(GumpButtonType.Reply, "Inventory", 0, GumpActions.GA_NavInventory, 0xFAB, 0xFAD),
                new ButtonDesc(GumpButtonType.Reply, "Selection", 0, GumpActions.GA_NavSelection, 0xFAB, 0xFAD),
                new ButtonDesc(GumpButtonType.Reply, "Teleport", 0, GumpActions.GA_Teleport, 0xFBA, 0xFBC),
                new ButtonDesc(GumpButtonType.Reply, "Select", 0, GumpActions.GA_SelectPartitionElement, 0xFBD, 0xFBF),
                new ButtonDesc(GumpButtonType.Reply, "Select X", 0, GumpActions.GA_SelectMultiplePartitionElement, 0xFBD, 0xFBF),
                new ButtonDesc(GumpButtonType.Reply, "Swap", 0, GumpActions.GA_SwapElements, 0xFBD, 0xFBF),
                new ButtonDesc(GumpButtonType.Reply, "Move", 0, GumpActions.GA_MoveElement, 0xFBD, 0xFBF),
                new ButtonDesc(GumpButtonType.Reply, "Eyedropper", 0, GumpActions.GA_EyedropSelectElement, 0xFBD, 0xFBF),
                new ButtonDesc(GumpButtonType.Reply, "Reclaim", 0, GumpActions.GA_ReclaimElement,0xFB1,0xFB3),
                new ButtonDesc(GumpButtonType.Reply, "Reclaim X", 0, GumpActions.GA_ReclaimMultipleElements,0xFB1,0xFB3),
                new ButtonDesc(GumpButtonType.Reply, "Delete", 0, GumpActions.GA_DeleteElement,0xFB1,0xFB3),
            };


            // Standard options
            const int buttonX = 10;
            const int buttonYStart = 75;
            const int labelX = buttonX + 35;
            const int buttonSeparator = 23;
            int nextButtonY = buttonYStart;

            bool atPortal = partition.InteriorContainsPoint(from.Location);
            int maxButtons = atPortal ? generalButtons.Length : 2;
            for (int i = 0; i < maxButtons; ++i)
            {
                AddButton(buttonX, nextButtonY, generalButtons[i].m_upState, generalButtons[i].m_downState, (int)generalButtons[i].m_actionId, GumpButtonType.Reply, 0);
                AddLabel(labelX, nextButtonY, 0xFF, generalButtons[i].m_name);
                nextButtonY += buttonSeparator;
            }

            PortalsItemization.PartitionPortalInventory inventory = partition.GetInventory();
            if (inventory.GetSelectionHistory().Count > 0)
            {
                AddLabel(labelX, nextButtonY, 0xFF, "History");
                AddButton(buttonX, nextButtonY, 0xFAB, 0xFAD, (int)GumpActions.GP_SelectionHistory, GumpButtonType.Reply, 0);
                nextButtonY += buttonSeparator;
            }

            bool canUndo = partition.ControlActionManager.CanUndo();
            bool canRedo = partition.ControlActionManager.CanRedo();
            if (canUndo || canRedo)
                nextButtonY += buttonSeparator;

            if (canUndo)
            {
                AddLabel(labelX, nextButtonY, 0xFF, "Undo");
                AddButton(buttonX, nextButtonY, 0xFAE, 0xFB0, (int)GumpActions.GA_UndoLastAction, GumpButtonType.Reply, 0);
                nextButtonY += buttonSeparator;
            }
            if (canRedo)
            {
                AddLabel(labelX, nextButtonY, 0xFF, "Redo");
                AddButton(buttonX, nextButtonY, 0xFA5, 0xFA7, (int)GumpActions.GA_RedoLastAction, GumpButtonType.Reply, 0);
                nextButtonY += buttonSeparator;
            }

            nextButtonY += buttonSeparator;
            if (atPortal)
            {
                AddLabel(labelX, nextButtonY, 0xFF, "Leave portal");
                AddButton(buttonX, nextButtonY, 0xFB4, 0xFB6, (int)GumpActions.GA_LeavePortal, GumpButtonType.Reply, 0);
                nextButtonY += buttonSeparator;
            }
            else
            {
                AddLabel(labelX, nextButtonY, 0xFF, "Enter portal");
                AddButton(buttonX, nextButtonY, 0xFBD, 0xFBE, (int)GumpActions.GA_EnterPortal, GumpButtonType.Reply, 0);
                nextButtonY += buttonSeparator;
            }


            CreateContentWindow();
        }
        public override void OnResponse(NetState sender, RelayInfo info)
        {
            PortalPartition partition = PortalsSystem.GetPartitionByUsername(sender.Account.Username);
            if (partition == null)
                return;
            int buttonId = (info.ButtonID << 16) >> 16;
            int gid = info.ButtonID >> 16;

            if (buttonId == 0)
                return;

            switch (buttonId)
            {
                case (int)PortalInventoryGumpUtility.GumpActions.GA_GotoCategory:
                    {
                        PortalContentEntry entry = PortalsItemization.InventoryLibrary.GetPortalContentEntry(gid);
                        this.m_category = entry.m_category;
                        sender.Mobile.SendGump(new PortalControlGump(this, EActiveScreen.eAP_Inventory));
                        return;
                    }
                case (int)PortalInventoryGumpUtility.GumpActions.GA_GotoSubcategory:
                    {
                        PortalContentEntry entry = PortalsItemization.InventoryLibrary.GetPortalContentEntry(gid);
                        this.m_subcategory = entry.m_subcategory;
                        sender.Mobile.SendGump(new PortalControlGump(this, EActiveScreen.eAP_Inventory));
                        return;
                    }
                case (int)PortalInventoryGumpUtility.GumpActions.GA_GotoEntry:
                    {
                        PortalContentEntry entry = PortalsItemization.InventoryLibrary.GetPortalContentEntry(gid);
                        this.m_entryId = entry.m_key;
                        sender.Mobile.SendGump(new PortalControlGump(this, EActiveScreen.eAP_Inventory));
                        return;
                    }
                case (int)PortalInventoryGumpUtility.GumpActions.GA_Back:
                    {
                        // Discard the furthest progression in inventory path
                        if (this.m_entryId > 0)
                            this.m_entryId = 0;
                        else if (this.m_subcategory != "")
                            this.m_subcategory = "";
                        else if (this.m_category != "")
                            this.m_category = "";

                        sender.Mobile.SendGump(new PortalControlGump(this, EActiveScreen.eAP_Inventory));
                        return;
                    }
            }

            if (buttonId >= (int)GumpActions.GA_Reserve || buttonId <= (int)GumpActions.GA_EndReserve)
            {
                //sender.Mobile.SendMessage(((GumpActions)buttonId).ToString());

                switch (buttonId)
                {
                    case (int)GumpActions.GA_EnterPortal:
                        {
                            // In case the control is still up and the user is already in the portal.
                            if (partition.InteriorContainsPoint(m_from.Location))
                                return;

                            // Origination restrictions
                            GuardedRegion reg = (GuardedRegion)Region.Find(m_from.Location, m_from.Map).GetRegion(typeof(GuardedRegion));
                            bool inGuardedRegion = reg != null && !reg.IsDisabled();

                            BaseHouse house = BaseHouse.FindHouseAt(m_from);
                            bool inFriendlyHouse = (house != null &&
                                (house.Owner.Account.Username == m_from.Account.Username ||
                                house.Friends.Contains(m_from) || house.CoOwners.Contains(m_from)));

                            if (inGuardedRegion || inFriendlyHouse)
                            {
                                MagerySpell spell = new PortalTransportSpell(sender.Mobile, partition, true);
                                spell.Cast();
                            }
                            else
                                sender.Mobile.SendMessage("You must be under the protection of the guards, or a friendly home before a passage can be opened.");

                            break;
                        }
                    case (int)GumpActions.GA_LeavePortal:
                        {
                            // In case the control is still up and the user has already exited the portal.
                            if (!partition.InteriorContainsPoint(m_from.Location))
                                return;

                            MagerySpell spell = new PortalTransportSpell(m_from, partition, false);
                            spell.Cast();

                            break;
                        }
                    case (int)GumpActions.GA_NavInventory:
                        {
                            sender.Mobile.SendGump(new PortalControlGump(this, EActiveScreen.eAP_Inventory));
                            return;
                        }
                    case (int)GumpActions.GA_NavSelection:
                        {
                            sender.Mobile.SendGump(new PortalControlGump(this));
                            return;
                        }
                    case (int)GumpActions.GA_SelectInventoryElement:
                        {
                            SetSelectedInventoryGid(gid);
                            sender.Mobile.SendGump(new PortalControlGump(this));
                            return;
                        }
                    case (int)GumpActions.GP_SelectionHistory:
                        {
                            sender.Mobile.SendGump(new PortalControlGump(this, EActiveScreen.eAP_History));
                            break;
                        }
                    case (int)GumpActions.GA_AddElement:
                        {
                            sender.Mobile.Target = new AddPortalElementTarget(this);
                            break;
                        }
                    case (int)GumpActions.GA_AddMultipleElements:
                        {
                            sender.Mobile.Target = new AddPortalElementsTarget(this);
                            break;
                        }
                    case (int)GumpActions.GA_AdjustElementNegX:
                        {
                            AdjustPortalElementsPosition(sender.Mobile, -1, 0, 0);
                            break;
                        }
                    case (int)GumpActions.GA_AdjustElementPosX:
                        {
                            AdjustPortalElementsPosition(sender.Mobile, 1, 0, 0);
                            break;
                        }
                    case (int)GumpActions.GA_AdjustElementNegY:
                        {
                            AdjustPortalElementsPosition(sender.Mobile, 0, -1, 0);
                            break;
                        }
                    case (int)GumpActions.GA_AdjustElementPosY:
                        {
                            AdjustPortalElementsPosition(sender.Mobile, 0, 1, 0);
                            break;
                        }
                    case (int)GumpActions.GA_AdjustElementNegXNegY:
                        {
                            AdjustPortalElementsPosition(sender.Mobile, -1, -1, 0);
                            break;
                        }
                    case (int)GumpActions.GA_AdjustElementNegXPosY:
                        {
                            AdjustPortalElementsPosition(sender.Mobile, -1, 1, 0);
                            break;
                        }
                    case (int)GumpActions.GA_AdjustElementPosXNegY:
                        {
                            AdjustPortalElementsPosition(sender.Mobile, 1, -1, 0);
                            break;
                        }
                    case (int)GumpActions.GA_AdjustElementPosXPosY:
                        {
                            AdjustPortalElementsPosition(sender.Mobile, 1, 1, 0);
                            break;
                        }
                    case (int)GumpActions.GA_AdjustElementPosZ:
                        {
                            AdjustPortalElementsPosition(sender.Mobile, 0, 0, 1);
                            break;
                        }
                    case (int)GumpActions.GA_AdjustElementNegZ:
                        {
                            AdjustPortalElementsPosition(sender.Mobile, 0, 0, -1);
                            break;
                        }
                    case (int)GumpActions.GA_AdjustElementPosZ5:
                        {
                            AdjustPortalElementsPosition(sender.Mobile, 0, 0, 5);
                            break;
                        }
                    case (int)GumpActions.GA_AdjustElementNegZ5:
                        {
                            AdjustPortalElementsPosition(sender.Mobile, 0, 0, -5);
                            break;
                        }
                    case (int)GumpActions.GA_AdjustElementPosZ20:
                        {
                            AdjustPortalElementsPosition(sender.Mobile, 0, 0, 20);
                            break;
                        }
                    case (int)GumpActions.GA_AdjustElementNegZ20:
                        {
                            AdjustPortalElementsPosition(sender.Mobile, 0, 0, -20);
                            break;
                        }
                    case (int)GumpActions.GA_Teleport:
                        {
                            sender.Mobile.Target = new TeleportTarget(this);
                            break;
                        }
                    case (int)GumpActions.GA_EyedropSelectElement:
                        {
                            sender.Mobile.Target = new EyedropElementTarget(this);
                            break;
                        }
                    case (int)GumpActions.GA_ApplyFilter:
                        {
                            int filterCount = m_filterList.Count;
                            for (int i = 0; i < filterCount; ++i)
                            {
                                if (!info.IsSwitched(i))
                                {
                                    // Remove unchecked
                                    List<IPortalElement> filteredList = m_filterList[i];
                                    m_selectedPortalElements.RemoveAll(x =>
                                        x.GetGid() == filteredList[0].GetGid() &&
                                        x.GetLocation().Z == filteredList[0].GetLocation().Z);
                                }
                            }
                            sender.Mobile.SendGump(new PortalControlGump(this));
                            break;
                        }
                    case (int)GumpActions.GA_SelectPartitionElement:
                        {
                            sender.Mobile.Target = new SelectPortalElementTarget(this);
                            break;
                        }
                    case (int)GumpActions.GA_SelectMultiplePartitionElement:
                        {
                            sender.Mobile.Target = new SelectPortalElementsTarget(this);
                            break;
                        }
                    case (int)GumpActions.GA_DeleteElement:
                        {
                            sender.Mobile.Target = new DeleteDungeonElementTarget(this);
                            break;
                        }
                    case (int)GumpActions.GA_ReclaimElement:
                        {
                            sender.Mobile.Target = new ReclaimDungeonElementTarget(this);
                            break;
                        }
                    case (int)GumpActions.GA_ReclaimMultipleElements:
                        {
                            sender.Mobile.Target = new ReclaimDungeonElementsTarget(this);
                            break;
                        }
                    case (int)GumpActions.GA_ReclaimSelection:
                        {
                            IPortalElementsUtility.ReclaimIActiveDungeonElements(m_from, m_selectedPortalElements, partition);

                            m_selectedPortalElements.Clear();
                            m_filterList.Clear();
                            sender.Mobile.SendGump(new PortalControlGump(this));
                            break;
                        }
                    case (int)GumpActions.GA_SwapElements:
                        {
                            sender.Mobile.Target = new SwapDungeonElementsTarget(this);
                            break;
                        }
                    case (int)GumpActions.GA_MoveElement:
                        {
                            sender.Mobile.Target = new MovePortalElementTarget(this);
                            return;
                        }
                    case (int)GumpActions.GA_Config:
                        {
                            sender.Mobile.SendGump(new PortalControlGump(this, EActiveScreen.eAP_Config));
                            break;
                        }
                    case (int)GumpActions.GA_Cancel:
                        {
                            sender.Mobile.SendGump(new PortalControlGump(this, EActiveScreen.eAP_Config));
                            break;
                        }
                    case (int)GumpActions.GA_ConfirmEraseToInventory:
                        {
                            partition.Clean(false, true);
                            partition.ControlActionManager.Flush();

                            sender.Mobile.SendGump(new PortalControlGump(this));

                            PortalUtility.ReturnPlayersToPlatform(partition);
                            break;
                        }
                    case (int)GumpActions.GA_ConfirmEraseAndDiscard:
                        {
                            partition.Clean(false);
                            partition.ControlActionManager.Flush();

                            sender.Mobile.SendGump(new PortalControlGump(this));

                            PortalUtility.ReturnPlayersToPlatform(partition);
                            break;
                        }
                    case (int)GumpActions.GA_ConfirmSurrenderPortal:
                        {
                            PortalUtility.ReturnPlayersToPortalPoint(partition);

                            sender.Mobile.SendMessage("The portal is stripped from your control...");

                            PortalsSystem.DestroyPartition(partition.m_partitionId);
                            break;
                        }
                    case (int)GumpActions.GA_ConfirmChangeDescription:
                        {
                            TextRelay entry = info.GetTextEntry(s_entry_description);
                            if (entry != null)
                                partition.Description = entry.Text;
                            sender.Mobile.SendGump(new PortalControlGump(this, EActiveScreen.eAP_Config));
                            break;
                        }
                    case (int)GumpActions.GA_ToggleAvailability:
                        {
                            partition.ToggleAccessibility();
                            sender.Mobile.SendGump(new PortalControlGump(this, EActiveScreen.eAP_Config));
                            break;
                        }
                    case (int)GumpActions.GA_ToggleRelativeHeight:
                        {
                            partition.ToggleDynamicHeight();
                            sender.Mobile.SendGump(new PortalControlGump(this, EActiveScreen.eAP_Config));
                            break;
                        }
                    case (int)GumpActions.GA_UndoLastAction:
                        {
                            partition.ControlActionManager.Undo();
                            ClearSelectedPortalElements();

                            sender.Mobile.SendGump(new PortalControlGump(this));
                            break;
                        }
                    case (int)GumpActions.GA_RedoLastAction:
                        {
                            partition.ControlActionManager.Redo();
                            ClearSelectedPortalElements();

                            sender.Mobile.SendGump(new PortalControlGump(this));
                            break;
                        }
                    default:
                        {
                            // unimplemented
                            sender.Mobile.SendGump(new PortalControlGump(this));
                            return;
                        }
                }
                return;
            }
        }
        private void AdjustPortalElementsPosition(Mobile from, int dx, int dy, int dz)
        {
            PortalPartition partition = PortalsSystem.GetPartitionByUsername(from.Account.Username);
            if (partition == null)
                return;

            int listLen = m_selectedPortalElements.Count;
            bool contained = true;
            for (int i = 0; i < listLen; ++i)
            {
                // Optimize this later to avoid interior checks.
                Point3D p = m_selectedPortalElements[i].GetLocation();
                p.X += dx;
                p.Y += dy;
                p.Z += dz;
                if (!PortalsSystem.IsValidLocationForElement(m_from.Account.Username, p))
                {
                    // An adjustment would result in an element being placed out of bounds.
                    contained = false;
                    break;
                }
            }

            if (contained)
            {
                int[] gids = new int[listLen];
                Point3D[] points = new Point3D[listLen];
                for (int i = 0; i < listLen; ++i)
                {
                    gids[i] = m_selectedPortalElements[i].GetGid();
                    points[i] = m_selectedPortalElements[i].GetLocation();
                }
                ControlActionManager.IControlAction action = new ControlActionManager.AdjustElementsControlAction(gids, points, new Point3D(dx, dy, dz));
                partition.ControlActionManager.Commit(action);


                for (int i = 0; i < listLen; ++i)
                {
                    IPortalElement element = m_selectedPortalElements[i];
                    Point3D p = element.GetLocation();
                    p.X += dx;
                    p.Y += dy;
                    p.Z += dz;
                    element.MoveToWorldWrapper(p);
                }
            }
            else
                m_from.SendMessage("The final position of the elements must remain in bounds, and not obstruct the platform.");

            m_from.SendGump(new PortalControlGump(this, EActiveScreen.eAP_Selection));
        }
    }
    //

}
