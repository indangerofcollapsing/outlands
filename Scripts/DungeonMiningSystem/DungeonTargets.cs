using System;
using System.Collections.Generic;

using Server.Targeting;
using Server.Items;
using Server.Mobiles;

namespace Server.PortalSystem
{
    public class PortalsTargeting
    {
        public static readonly string s_targetOutsidePortal = "That point is outside your portal.";
        public static readonly string s_platformUntargetable = "Elements can not be placed atop the platform.";
        public static readonly string s_targetRegionEnd = "Select the end of the region.";
        public static readonly string s_invalidTarget = "That is an invalid target.";
        public static readonly string s_notInInventory = "That element is not in your inventory.";

        public static bool GetTargetPoint(Mobile targeter, object targeted, ref Point3D point)
        {
            if (targeted is LandTarget)
            {
                LandTarget landTarget = targeted as LandTarget;
                point.X = landTarget.X;
                point.Y = landTarget.Y;
                point.Z = landTarget.Z;
                return true;
            }
            else if (targeted is IPortalElement)
            {
                IPortalElement element = targeted as IPortalElement;
                point = element.GetLocation();
                return true;
            }
            else if (targeted is Mobile)
            {
                // As a convenience, allow targeting of self to indicate target point. This can be useful
                // when the player wants to place in a position obstructed by other portal elements.
                Mobile m = targeted as Mobile;
                if (m == targeter)
                {
                    point = m.Location;
                }
                return true;
            }
            return false;
        }
    }

    class InjectControllerTarget : Target
    {
        DungeonInjector m_injector;

        public InjectControllerTarget(DungeonInjector injector)
            : base(-1, false, TargetFlags.None)
        {
            m_injector = injector;
        }
        protected override void OnTarget(Mobile from, object targeted)
        {
            if (targeted is DungeonControl)
            {
                DungeonControl control = targeted as DungeonControl;
                if (!control.m_isActivated)
                {
                    from.SendMessage("This {0} has not been bound yet.", DungeonControl.s_desc);
                    return;
                }
                else if (control.m_username != from.Account.Username)
                {
                    from.SendMessage("This {0} doesn't belong to you.", DungeonControl.s_desc);
                    return;
                }

                PortalPartition partition = PortalsSystem.GetPartitionByUsername(control.m_username);
                if (partition == null)
                    return;

                //if (m_injector is DungeonSingleStaticInjector ||
                //    m_injector is DungeonGroupStaticInjector)
                //{
                //    control.OnStaticInjection(m_injector);
                //}
                //else if (m_injector is DungeonDoorInjector)
                //{
                //    control.OnDoorInjection(m_injector as DungeonDoorInjector);
                //}
                //else if (m_injector is DungeonTestInjector)
                //{
                //    // 10/13/13: Remove this on release.
                //    control.OnTestInjection(m_injector as DungeonTestInjector);
                //}

                // Content was added, so we need to refresh the gump.
                if (from.HasGump(typeof(PortalControlGump)))
                {
                    from.CloseGump(typeof(PortalControlGump));
                    from.SendGump(new PortalControlGump(from));
                }

                from.SendMessage("Your {0} expands!", DungeonControl.s_desc);

                m_injector.Delete();
            }
            else
            {
                from.SendMessage("Target a {0} that is bound to you.", DungeonControl.s_desc);
            }
        }
    }

    class AddPortalElementTarget : Target
    {
        private PortalControlGump m_dcg;

        public AddPortalElementTarget(PortalControlGump dcg)
            : base(-1, true, TargetFlags.None, false)
        {
            m_dcg = dcg;
        }
        protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
        {
            from.SendGump(new PortalControlGump(m_dcg));
        }
        protected override void OnTarget(Mobile from, object targeted)
        {
            PortalPartition partition = PortalsSystem.GetPartitionByUsername(from.Account.Username);
            if (partition == null)
                return;

            Point3D p = new Point3D(0, 0, 0);
            if (!PortalsTargeting.GetTargetPoint(from, targeted, ref p))
            {
                from.SendMessage(PortalsTargeting.s_invalidTarget);
                from.SendGump(new PortalControlGump(m_dcg));
                return;
            }

            if (!PortalsSystem.IsValidLocationForElement(from.Account.Username, p))
            {
                from.SendMessage(PortalsTargeting.s_invalidTarget);
                from.SendGump(new PortalControlGump(m_dcg));
                return;
            }

            from.Animate(9, 7, 1, true, false, 0);
            //float time = PortalUtility.GetItemTransitTime(p, from);
            //PortalUtility.CreateItemTransitFromSourceEffect(p, from, gid, time);

            int gid = m_dcg.GetSelectedInventoryGid();
            IPortalElement element = PortalUtility.CreateIPortalElement(gid);

            if (partition.IsUsingRelativeHeight())
                p.Z = from.Location.Z;
            else
                p = PortalUtility.CalculateElementLocation(from, element as Item, targeted);

            element.MoveToWorldWrapper(p, PortalsSystem.s_map);

            //element.SetVisibility(false);
            //RevealElementTimer timer = new RevealElementTimer(time, element);
            //timer.Start();

            ControlActionManager.IControlAction action = new ControlActionManager.PlaceElementControlAction(element.GetGid(), element.GetEntryKey(), p);
            partition.ControlActionManager.Commit(action);

            PortalsItemization.PartitionPortalInventory inventory = partition.GetInventory();
            inventory.Remove(gid, element.GetEntryKey(), 1);
            if (inventory.GetCount(gid, element.GetEntryKey()) <= 0)
            {
                m_dcg.SetSelectedInventoryGid(0);
                from.SendGump(new PortalControlGump(m_dcg));
            }
            else  // Target continuation.
                from.Target = new AddPortalElementTarget(m_dcg);
        }
    }

    class AddPortalElementsTarget : Target
    {
        private PortalControlGump m_dcg;
        private Point3D m_firstPoint;

        public AddPortalElementsTarget(PortalControlGump dcg, Point3D firstPoint = default(Point3D))
            : base(-1, true, TargetFlags.None, false)
        {
            m_dcg = dcg;
            m_firstPoint = firstPoint;
        }

        protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
        {
            from.SendGump(new PortalControlGump(m_dcg));
        }
        protected override void OnTarget(Mobile from, object targeted)
        {
            Point3D p = new Point3D(0, 0, 0);
            if (!PortalsTargeting.GetTargetPoint(from, targeted, ref p))
            {
                from.SendMessage(PortalsTargeting.s_invalidTarget);
                from.SendGump(new PortalControlGump(m_dcg));
                return;
            }

            PortalPartition partition = PortalsSystem.GetPartitionByUsername(from.Account.Username);
            if (partition == null)
                return;

            if (m_firstPoint == default(Point3D))
            {
                if (!PortalsSystem.IsValidLocationForElement(from.Account.Username, p))
                {
                    from.SendMessage(PortalsTargeting.s_targetOutsidePortal);
                    from.SendGump(new PortalControlGump(m_dcg));
                    return;
                }

                from.SendMessage(PortalsTargeting.s_targetRegionEnd);
                from.Target = new AddPortalElementsTarget(m_dcg, p);
            }
            else
            {
                if (!PortalsSystem.IsValidLocationForElement(from.Account.Username, p))
                {
                    from.SendMessage(PortalsTargeting.s_targetOutsidePortal);
                    from.Target = new AddPortalElementsTarget(m_dcg, m_firstPoint);
                    return;
                }

                // Resolve the points into a new rect that is aligned from the most positive x and y points.
                Rectangle2D rect = new Rectangle2D(m_firstPoint.X < p.X ? m_firstPoint.X : p.X,
                    m_firstPoint.Y < p.Y ? m_firstPoint.Y : p.Y,
                    Math.Abs(m_firstPoint.X - p.X),
                    Math.Abs(m_firstPoint.Y - p.Y));

                PortalsItemization.PartitionPortalInventory inventory = partition.GetInventory();
                int gid = m_dcg.GetSelectedInventoryGid();
                PortalContentEntry entry = PortalsItemization.InventoryLibrary.GetPortalContentEntryWithGid(gid);

                List<Point3D> createdPointsList = new List<Point3D>();

                int quantityAvailable = inventory.GetCount(gid, entry.m_key);
                int quantityRemaining = quantityAvailable;
                for (int x = 0; x <= rect.Width && quantityRemaining > 0; x++)
                {
                    for (int y = 0; y <= rect.Height && quantityRemaining > 0; y++)

                    {
                        Point3D xyp = new Point3D(rect.X + x, rect.Y + y, p.Z);
                        if (PortalsSystem.IsValidLocationForElement(from.Account.Username, xyp))
                        {
                            --quantityRemaining;

                            PortalStatic portalStatic = new PortalStatic(gid, entry.m_key, entry.m_name, entry.m_category);

                            if (partition.IsUsingRelativeHeight())
                                xyp.Z = from.Location.Z;
                            else
                                xyp.Z = PortalUtility.CalculateElementLocation(from, portalStatic as Item, targeted).Z;

                            portalStatic.MoveToWorld(xyp, PortalsSystem.s_map);

                            //float time = PortalUtility.GetItemTransitTime(p, from);
                            //PortalUtility.CreateItemTransitFromSourceEffect(p, from, gid, time);
                            //portalStatic.Visible = false;

                            //RevealElementTimer timer = new RevealElementTimer(time, portalStatic);
                            //timer.Start();

                            createdPointsList.Add(xyp);
                        }
                    }
                }

                int quantityUsed = quantityAvailable - quantityRemaining;
                if (quantityUsed > 0)
                {
                    from.Animate(9, 7, 1, true, false, 0);
                    inventory.Remove(gid, entry.m_key, quantityUsed);

                    int[] gids = new int[quantityUsed];
                    for (int i = 0; i < quantityUsed; ++i)
                        gids[i] = gid;
                    Point3D[] points = createdPointsList.ToArray();
                    ControlActionManager.IControlAction action = new ControlActionManager.PlaceElementsControlAction(gids, entry.m_key, points);
                    partition.ControlActionManager.Commit(action);
                }

                if (quantityRemaining > 0) // Target continuation.
                    from.Target = new AddPortalElementsTarget(m_dcg);
                else
                {
                    m_dcg.SetSelectedInventoryGid(0);
                    from.SendGump(new PortalControlGump(m_dcg));
                }
            }
        }
    }

    //public class RevealElementTimer : Timer
    //{
    //    private IPortalElement m_element;

    //    public RevealElementTimer(float time, IPortalElement element)
    //        : base(TimeSpan.FromSeconds(time))
    //    {
    //        m_element = element;
    //        Priority = TimerPriority.TwentyFiveMS;
    //    }
    //    protected override void OnTick()
    //    {
    //        m_element.SetVisibility(true);
    //    }
    //}

    class DeleteDungeonElementTarget : Target
    {
        private PortalControlGump m_dcg;

        public DeleteDungeonElementTarget(PortalControlGump dcg)
            : base(-1, false, TargetFlags.None, false)
        {
            m_dcg = dcg;
        }

        protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
        {
            from.SendGump(new PortalControlGump(m_dcg));
        }
        protected override void OnTarget(Mobile from, object targeted)
        {
            string username = from.Account.Username;
            PortalPartition partition = PortalsSystem.GetPartitionByUsername(username);
            if (partition == null)
                return;

            if (!(targeted is IPortalElement))
            {
                from.SendMessage(PortalsTargeting.s_invalidTarget);
                from.SendGump(new PortalControlGump(m_dcg));
                return;
            }

            IPortalElement element = targeted as IPortalElement;
            Point3D p = element.GetLocation();

            if (!PortalsSystem.IsValidLocationForElement(username, p))
            {
                from.SendMessage(PortalsTargeting.s_targetOutsidePortal);
                from.SendGump(new PortalControlGump(m_dcg));
            }

            // Invalidate the selections if a delete is performed.
            m_dcg.ClearSelectedPortalElements();

            ControlActionManager.IControlAction action = new ControlActionManager.DeleteElementControlAction(element.GetGid(), element.GetEntryKey(), p);
            partition.ControlActionManager.Commit(action);

            element.Delete();


            // Target continuation
            from.Target = new DeleteDungeonElementTarget(m_dcg);
            return;
        }
    }

    /// <summary>
    /// EyedropElementTarget selects the targeted element in inventory, if it exists.
    /// </summary>
    class EyedropElementTarget : Target
    {
        private PortalControlGump m_dcg;

        public EyedropElementTarget(PortalControlGump dcg)
            : base(-1, true, TargetFlags.None, false)
        {
            m_dcg = dcg;
        }
        protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
        {
            from.SendGump(new PortalControlGump(m_dcg));
        }
        protected override void OnTarget(Mobile from, object targeted)
        {
            IPortalElement element = targeted as IPortalElement;
            if (element == null)
            {
                from.SendMessage(PortalsTargeting.s_invalidTarget);
                from.SendGump(new PortalControlGump(m_dcg));
                return;
            }

            string username = from.Account.Username;
            PortalPartition partition = PortalsSystem.GetPartitionByUsername(username);
            if (partition == null)
                return;

            PortalsItemization.PartitionPortalInventory inventory = partition.GetInventory();
            bool foundInInventory = inventory.GetCount(element.GetGid(), element.GetEntryKey()) > 0;
            if (!foundInInventory)
            {
                from.SendMessage(PortalsTargeting.s_notInInventory);
                from.SendGump(new PortalControlGump(m_dcg));
                return;
            }

            m_dcg.SetSelectedInventoryGid(element.GetGid());
            from.SendGump(new PortalControlGump(m_dcg, PortalControlGump.EActiveScreen.eAP_Selection));
        }
    }

    class SelectPortalElementTarget : Target
    {
        private PortalControlGump m_dcg;

        public SelectPortalElementTarget(PortalControlGump dcg)
            : base(-1, false, TargetFlags.None, false)
        {
            m_dcg = dcg;
        }
        protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
        {
            from.SendGump(new PortalControlGump(m_dcg));
        }
        protected override void OnTarget(Mobile from, object targeted)
        {
            PortalPartition partition = PortalsSystem.GetPartitionByUsername(from.Account.Username);
            if (partition == null)
                return;

            IPortalElement target = targeted as IPortalElement;
            if (target == null)
            {
                from.SendMessage(PortalsTargeting.s_invalidTarget);
                from.SendGump(new PortalControlGump(m_dcg));
                return;
            }

            Point3D p = target.GetLocation();
            if (!PortalsSystem.IsValidLocationForElement(from.Account.Username, p))
            {
                from.SendMessage(PortalsTargeting.s_targetOutsidePortal);
                from.SendGump(new PortalControlGump(m_dcg));
                return;
            }

            m_dcg.SetSelectedPortalElement(target);
            from.SendGump(new PortalControlGump(m_dcg, PortalControlGump.EActiveScreen.eAP_Selection));
        }
    }

    class SelectPortalElementsTarget : Target
    {
        private PortalControlGump m_dcg;
        private Point3D m_point;

        public SelectPortalElementsTarget(PortalControlGump dcg, Point3D firstPoint = default(Point3D))
            : base(-1, true, TargetFlags.None, false)
        {
            m_dcg = dcg;
            m_point = firstPoint;
        }
        protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
        {
            from.SendGump(new PortalControlGump(m_dcg));
        }
        protected override void OnTarget(Mobile from, object targeted)
        {
            PortalPartition partition = PortalsSystem.GetPartitionByUsername(from.Account.Username);
            if (partition == null)
                return;

            Point3D p = new Point3D(0, 0, 0);
            if (!PortalsTargeting.GetTargetPoint(from, targeted, ref p))
            {
                from.SendMessage(PortalsTargeting.s_invalidTarget);
                from.SendGump(new PortalControlGump(m_dcg));
                return;
            }
            p.Z = Math.Max(p.Z, 1);

            if (m_point == default(Point3D))
            {
                if (!PortalsSystem.IsValidLocationForElement(from.Account.Username, p))
                {
                    from.SendMessage(PortalsTargeting.s_invalidTarget);
                    from.SendGump(new PortalControlGump(m_dcg));
                    return;
                }
                else
                {
                    from.SendMessage(PortalsTargeting.s_targetRegionEnd);
                    from.Target = new SelectPortalElementsTarget(m_dcg, p);
                    return;
                }
            }
            else
            {
                if (!PortalsSystem.IsValidLocationForElement(from.Account.Username, p))
                {
                    from.SendMessage(PortalsTargeting.s_invalidTarget);
                    from.Target = new SelectPortalElementsTarget(m_dcg, m_point);
                    return;
                }

                // Resolve the points into a new rect that is aligned from the most positive x and y points.
                Rectangle2D rect = new Rectangle2D(m_point.X < p.X ? m_point.X : p.X,
                    m_point.Y < p.Y ? m_point.Y : p.Y,
                    Math.Abs(m_point.X - p.X) + 1, Math.Abs(m_point.Y - p.Y) + 1);


                IPooledEnumerable pool = PortalsSystem.s_map.GetObjectsInBounds(rect);
                List<IPortalElement> selectedList = new List<IPortalElement>();
                foreach (IEntity obj in pool)
                {
                    if (obj is IPortalElement)
                    {
                        IPortalElement element = obj as IPortalElement;
                        Point3D elementPos = element.GetLocation();
                        if (PortalsSystem.IsValidLocationForElement(from.Account.Username, elementPos))
                            selectedList.Add((IPortalElement)obj);
                    }
                }
                pool.Free();

                m_dcg.SetSelectedPortalElements(selectedList);
                from.SendGump(new PortalControlGump(m_dcg, PortalControlGump.EActiveScreen.eAP_Selection));
            }
        }
    }

    class ReclaimDungeonElementTarget : Target
    {
        private PortalControlGump m_dcg;

        public ReclaimDungeonElementTarget(PortalControlGump dcg)
            : base(-1, false, TargetFlags.None, false)
        {
            m_dcg = dcg;
        }
        protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
        {
            from.SendGump(new PortalControlGump(m_dcg));
        }
        protected override void OnTarget(Mobile from, object targeted)
        {
            PortalPartition partition = PortalsSystem.GetPartitionByUsername(from.Account.Username);
            if (partition == null)
                return;

            IPortalElement target = targeted as IPortalElement;
            if (target == null)
            {
                from.SendMessage(PortalsTargeting.s_invalidTarget);
                from.SendGump(new PortalControlGump(m_dcg));
                return;
            }

            Point3D p = target.GetLocation();

            if (!PortalsSystem.IsValidLocationForElement(from.Account.Username, p))
            {
                from.SendMessage(PortalsTargeting.s_invalidTarget);
                from.SendGump(new PortalControlGump(m_dcg));
                return;
            }

            from.Animate(9, 7, 1, true, false, 0);
            //float time = PortalUtility.GetItemTransitTime(p, from);
            //PortalUtility.CreateItemTransitToSourceEffect(p, from, target.GetGid(), time);

            PortalsItemization.PartitionPortalInventory inventory = partition.GetInventory();
            inventory.Add(target.GetGid(), target.GetEntryKey(), 1);

            // Auto-select the reclaimed element
            m_dcg.SetSelectedInventoryGid(target.GetGid());

            ControlActionManager.IControlAction action = new ControlActionManager.ReclaimElementControlAction(target.GetGid(), target.GetEntryKey(), p);
            partition.ControlActionManager.Commit(action);

            target.Delete();

            // Target continuation.
            from.Target = new ReclaimDungeonElementTarget(m_dcg);
            //from.SendGump(new PortalControlGump(m_dcg));
        }
    }

    class ReclaimDungeonElementsTarget : Target
    {
        private PortalControlGump m_dcg;
        private Point3D m_point;

        public ReclaimDungeonElementsTarget(PortalControlGump dcg, Point3D firstPoint = default(Point3D))
            : base(-1, true, TargetFlags.None, false)
        {
            m_dcg = dcg;
            m_point = firstPoint;
        }

        protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
        {
            from.SendGump(new PortalControlGump(m_dcg));
        }
        protected override void OnTarget(Mobile from, object targeted)
        {
            PortalPartition partition = PortalsSystem.GetPartitionByUsername(from.Account.Username);
            if (partition == null)
                return;

            Point3D p = new Point3D(0, 0, 0);
            if (!PortalsTargeting.GetTargetPoint(from, targeted, ref p))
            {
                from.SendMessage(PortalsTargeting.s_invalidTarget);
                from.SendGump(new PortalControlGump(m_dcg));
                return;
            }
            p.Z = Math.Max(p.Z, 1);

            if (m_point == default(Point3D))
            {
                if (!PortalsSystem.IsValidLocationForElement(from.Account.Username, p))
                {
                    from.SendMessage(PortalsTargeting.s_targetOutsidePortal);
                    from.SendGump(new PortalControlGump(m_dcg));
                    return;
                }
                else
                {
                    from.SendMessage(PortalsTargeting.s_targetRegionEnd);
                    from.Target = new ReclaimDungeonElementsTarget(m_dcg, p);
                    return;
                }
            }
            else
            {
                if (!PortalsSystem.IsValidLocationForElement(from.Account.Username, p))
                {
                    from.SendMessage(PortalsTargeting.s_targetOutsidePortal);
                    from.Target = new ReclaimDungeonElementsTarget(m_dcg, m_point);
                    return;
                }

                // Resolve the points into a new rect that is aligned from the most positive x and y points.
                Rectangle2D rect = new Rectangle2D(m_point.X < p.X ? m_point.X : p.X,
                    m_point.Y < p.Y ? m_point.Y : p.Y,
                    Math.Abs(m_point.X - p.X) + 1,
                    Math.Abs(m_point.Y - p.Y) + 1);


                IPooledEnumerable pool = PortalsSystem.s_map.GetObjectsInBounds(rect);
                List<IPortalElement> toReclaim = new List<IPortalElement>();
                foreach (IEntity obj in pool)
                {
                    if (obj is IPortalElement)
                    {
                        IPortalElement element = obj as IPortalElement;
                        Point3D elementPos = element.GetLocation();
                        if (PortalsSystem.IsValidLocationForElement(from.Account.Username, elementPos))
                            toReclaim.Add(obj as IPortalElement);
                    }
                }
                pool.Free();


                int reclaimCount = toReclaim.Count;
                int[] gids = new int[reclaimCount];
                int[] entryKeys = new int[reclaimCount];
                Point3D[] points = new Point3D[reclaimCount];
                for (int i = 0; i < reclaimCount; ++i)
                {
                    gids[i] = toReclaim[i].GetGid();
                    entryKeys[i] = toReclaim[i].GetEntryKey();
                    points[i] = toReclaim[i].GetLocation();
                }
                ControlActionManager.IControlAction action = new ControlActionManager.ReclaimElementsControlAction(gids, entryKeys, points);
                partition.ControlActionManager.Commit(action);

                if (toReclaim.Count > 0)
                {
                    from.Animate(17, 7, 1, true, false, 0);
                    IPortalElementsUtility.ReclaimIActiveDungeonElements(from, toReclaim, partition);
                }

                // Target continuation.
                from.Target = new ReclaimDungeonElementsTarget(m_dcg);
            }
        }
    }
    class MovePortalElementTarget : Target
    {
        private PortalControlGump m_dcg;
        private IPortalElement m_element;

        public MovePortalElementTarget(PortalControlGump dcg, IPortalElement element = null)
            : base(-1, element != null ? true : false, TargetFlags.None, false)
        {
            m_dcg = dcg;
            m_element = element;
        }
        protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
        {
            from.SendGump(new PortalControlGump(m_dcg));
        }
        protected override void OnTarget(Mobile from, object targeted)
        {
            PortalPartition partition = PortalsSystem.GetPartitionByUsername(from.Account.Username);
            if (partition == null)
                return;

            if (m_element == null)
            {
                // First target
                if (targeted is IPortalElement)
                {
                    IPortalElement element = targeted as IPortalElement;
                    if (PortalsSystem.IsValidLocationForElement(from.Account.Username, element.GetLocation()))
                    {
                        from.SendMessage("Pick a new location.");
                        from.Target = new MovePortalElementTarget(m_dcg, element);
                    }
                    else
                    {
                        from.SendMessage(PortalsTargeting.s_targetOutsidePortal);
                        from.SendGump(new PortalControlGump(m_dcg));
                    }
                }
                else
                {
                    from.SendMessage(PortalsTargeting.s_invalidTarget);
                    from.SendGump(new PortalControlGump(m_dcg));
                    return;
                }
            }
            else
            {
                Point3D p = new Point3D(0, 0, 0);
                if (!PortalsTargeting.GetTargetPoint(from, targeted, ref p))
                {
                    from.SendMessage(PortalsTargeting.s_invalidTarget);
                    from.SendGump(new PortalControlGump(m_dcg));
                    return;
                }

                if (!PortalsSystem.IsValidLocationForElement(from.Account.Username, p))
                {
                    from.SendMessage(PortalsTargeting.s_targetOutsidePortal);
                    from.Target = new MovePortalElementTarget(m_dcg, m_element);
                    return;
                }

                if (partition.IsUsingRelativeHeight())
                    p.Z = from.Location.Z;
                else
                    p = PortalUtility.CalculateElementLocation(from, m_element as Item, targeted);

                ControlActionManager.IControlAction action = new ControlActionManager.MoveElementControlAction(m_element.GetGid(), m_element.GetLocation(), p);
                partition.ControlActionManager.Commit(action);

                m_element.MoveToWorldWrapper(p);

                // Target continuation.
                from.Target = new MovePortalElementTarget(m_dcg);
            }
        }
    }

    class SwapDungeonElementsTarget : Target
    {
        private PortalControlGump m_dcg;
        private Point3D m_point;
        private IPortalElement m_firstElement;

        public SwapDungeonElementsTarget(PortalControlGump dcg, Point3D firstPoint = default(Point3D), IPortalElement firstElement = null)
            : base(-1, true, TargetFlags.None, false)
        {
            m_dcg = dcg;
            m_point = firstPoint;
            m_firstElement = firstElement;
        }
        protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
        {
            from.SendGump(new PortalControlGump(m_dcg));
        }
        protected override void OnTarget(Mobile from, object targeted)
        {
            PortalPartition partition = PortalsSystem.GetPartitionByUsername(from.Account.Username);
            if (partition == null)
                return;

            if (m_firstElement == null)
            {
                IPortalElement element = targeted as IPortalElement;
                if (element == null)
                {
                    from.SendMessage(PortalsTargeting.s_invalidTarget);
                    from.SendGump(new PortalControlGump(m_dcg));
                    return;
                }

                Point3D p = element.GetLocation();
                if (!PortalsSystem.IsValidLocationForElement(from.Account.Username, p))
                {
                    from.SendMessage(PortalsTargeting.s_targetOutsidePortal);
                    from.SendGump(new PortalControlGump(m_dcg));
                    return;
                }
                else
                {

                    from.SendMessage("Select the first element to swap positions.");
                    from.Target = new SwapDungeonElementsTarget(m_dcg, p, element);
                    return;
                }
            }
            else
            {
                IPortalElement element = targeted as IPortalElement;
                if (element == null)
                {
                    from.SendMessage(PortalsTargeting.s_invalidTarget);
                    from.Target = new SwapDungeonElementsTarget(m_dcg, m_point, m_firstElement);
                    return;
                }

                Point3D p = element.GetLocation();
                if (!PortalsSystem.IsValidLocationForElement(from.Account.Username, p))
                {
                    from.SendMessage(PortalsTargeting.s_targetOutsidePortal);
                    from.Target = new SwapDungeonElementsTarget(m_dcg, m_point, m_firstElement);
                    return;
                }

                ControlActionManager.IControlAction action = new ControlActionManager.SwapElementsControlAction(m_firstElement.GetGid(), p, element.GetGid(), m_point);
                partition.ControlActionManager.Commit(action);

                // Exchange locations.
                m_firstElement.MoveToWorldWrapper(p);
                element.MoveToWorldWrapper(m_point);


                // Animate the player once.
                from.Animate(16, 7, 1, true, false, 0);

                // Target continuation.
                from.Target = new SwapDungeonElementsTarget(m_dcg);
            }
        }
    }

    class ReplaceControllerTarget : Target
    {
        public ReplaceControllerTarget()
            : base(-1, true, TargetFlags.None, false)
        {
        }
        protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
        {
            from.SendGump(new PortalsAdminGump(from));
        }
        protected override void OnTarget(Mobile from, object targeted)
        {
            // Target must be a player.
            if (targeted is PlayerMobile)
            {
                PlayerMobile m = targeted as PlayerMobile;

                // The player must have an account registered to a dungeon.
                string username = m.Account.Username;
                PortalPartition partition = PortalsSystem.GetPartitionByUsername(username);
                if (partition == null)
                {
                    from.SendMessage("This user doesn't have a portal associated with them.");
                    return;
                }

                // Assign a new controller, and update the partition controller reference.
                DungeonControl replacementController = new DungeonControl();
                replacementController.BindToAccount(username);

                m.Backpack.AddItem(replacementController);

                from.SendMessage("{0} under account {1} has been issued a new controller for their portal.", m.Name, m.Account.Username);
            }
            else
            {
                from.SendMessage("You must select an existing portal owner.");
            }

            from.SendGump(new PortalsAdminGump(from));
        }
    }

    class TeleportTarget : Target
    {
        private PortalControlGump m_dcg;

        public TeleportTarget(PortalControlGump dcg)
            : base(-1, true, TargetFlags.None, false)
        {
            m_dcg = dcg;
        }
        protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
        {
            from.SendGump(new PortalControlGump(from));
        }
        protected override void OnTarget(Mobile from, object targeted)
        {
            PortalPartition partition = PortalsSystem.GetPartitionByUsername(from.Account.Username);
            if (partition == null)
                return;

            Point3D p = new Point3D(0, 0, 0);
            if (!PortalsTargeting.GetTargetPoint(from, targeted, ref p))
            {
                from.SendMessage(PortalsTargeting.s_invalidTarget);
                from.Target = new TeleportTarget(m_dcg);
                return;
            }

            // Avoid z merge
            p.Z += 1;

            if (targeted is IPortalElement)
            {
                IPortalElement target = targeted as IPortalElement;
                Point3D surface = target.GetSurfaceTopWrapper();

                if (surface.Z <= p.Z)
                    p.Z = surface.Z + 1; // Avoid z merge
            }

            if (!partition.InteriorContainsPoint(p))
            {
                from.SendMessage(PortalsTargeting.s_targetOutsidePortal);
                from.Target = new TeleportTarget(m_dcg);
                return;
            }

            Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
            Effects.SendLocationParticles(EffectItem.Create(p, from.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023);
            from.PlaySound(0x1FE);

            from.MoveToWorld(p, from.Map);
            from.Target = new TeleportTarget(m_dcg);
        }
    }

    public class IPortalElementsUtility
    {
        public static void ReclaimIActiveDungeonElements(Mobile from,
            List<IPortalElement> elementList,
            PortalPartition partition)
        {
            PortalsItemization.PartitionPortalInventory inventory = partition.GetInventory();

            int elementListLen = elementList.Count;
            for (int i = 0; i < elementListLen; i++)
            {
                IPortalElement element = elementList[i] as IPortalElement;
                inventory.Add(element.GetGid(), element.GetEntryKey(), 1);

                Point3D xyp = element.GetLocation();
                //float time = PortalUtility.GetItemTransitTime(xyp, from);
                //PortalUtility.CreateItemTransitToSourceEffect(xyp, from, element.GetGid(), time);

                element.Delete();
            }
        }
    }
}