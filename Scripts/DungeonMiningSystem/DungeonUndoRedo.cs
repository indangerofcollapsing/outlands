using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Server.Items;

namespace Server.PortalSystem
{
    public class ControlActionManager
    {
        private static bool AddToPartition(PortalPartition partition, int gid, int entryKey, Point3D point, bool inventoryIgnored = false)
        {
            if (!inventoryIgnored)
                partition.m_inventory.Remove(gid, entryKey, 1);
            IPortalElement element = PortalUtility.CreateIPortalElement(gid);
            element.MoveToWorldWrapper(point, PortalsSystem.s_map);

            return true;
        }
        private static bool AddToPartition(PortalPartition partition, int[] gids, int[] entryKeys, Point3D[] points, bool inventoryIgnored = false)
        {
            int count = gids.Length;
            for (int i = 0; i < count; ++i)
            {
                if (!inventoryIgnored)
                    partition.m_inventory.Remove(gids[i], entryKeys[i], 1);
                IPortalElement element = PortalUtility.CreateIPortalElement(gids[i]);
                element.MoveToWorldWrapper(points[i], PortalsSystem.s_map);
            }

            return true;
        }
        private static bool RemoveFromPartition(PortalPartition partition, int gid, int entryKey, Point3D point, bool inventoryIgnored = false)
        {
            IEntity deleteEntity = null;
            List<IPortalElement> deleteList = new List<IPortalElement>();
            IPooledEnumerable pool = PortalsSystem.s_map.GetItemsInBounds(new Rectangle2D(point.X, point.Y, 1, 1));
            foreach (IEntity obj in pool)
            {
                if (obj is PortalStatic || obj is PortalDoor)
                {
                    IPortalElement element = obj as IPortalElement;
                    if (element.GetLocation() == point && element.GetGid() == gid)
                    {
                        if (!inventoryIgnored)
                            partition.m_inventory.Add(gid, entryKey, 1);
                        deleteEntity = obj;
                        break;
                    }
                }
            }
            pool.Free();

            if (deleteEntity != null)
                deleteEntity.Delete();
            return true;
        }
        private static bool RemoveFromPartition(PortalPartition partition, int[] gids, Point3D[] points, bool inventoryIgnored = false)
        {
            List<IEntity> deleteList = new List<IEntity>();
            int count = gids.Length;
            for (int i = 0; i < count; ++i)
            {
                // This is not ideal, however, maintaining the references is a complicated issue.
                IPooledEnumerable pool = PortalsSystem.s_map.GetItemsInBounds(new Rectangle2D(points[i].X, points[i].Y, 1, 1));
                foreach (IEntity obj in pool)
                {
                    if (obj is PortalStatic || obj is PortalDoor)
                    {
                        IPortalElement element = obj as IPortalElement;
                        if (element.GetLocation() == points[i] && element.GetGid() == gids[i])
                        {
                            if (!inventoryIgnored)
                                partition.m_inventory.Add(element.GetGid(), element.GetEntryKey(), 1);
                            deleteList.Add(obj);
                            break;
                        }
                    }
                }
                pool.Free();
            }

            foreach (IEntity entity in deleteList)
                entity.Delete();
            return true;
        }

        public interface IControlAction
        {
            bool Undo(PortalPartition partition);
            bool Redo(PortalPartition partition);
        }

        public class PlaceElementControlAction : IControlAction
        {
            private int m_gid;
            private int m_entryKey;
            private Point3D m_point;

            public PlaceElementControlAction(int gid, int entryKey, Point3D p)
            {
                m_gid = gid;
                m_entryKey = entryKey;
                m_point = p;
            }
            public bool Undo(PortalPartition partition)
            {
                return RemoveFromPartition(partition, m_gid, m_entryKey, m_point);
            }
            public bool Redo(PortalPartition partition)
            {
                return AddToPartition(partition, m_gid, m_entryKey, m_point);
            }
        }
        public class PlaceElementsControlAction : IControlAction
        {
            public int[] m_gids;
            public int m_entryKey;
            public Point3D[] m_points;

            public PlaceElementsControlAction(int[] gids, int entryKey, Point3D[] points)
            {
                m_gids = gids;
                m_entryKey = entryKey;
                m_points = points;
            }
            public bool Undo(PortalPartition partition)
            {
                return RemoveFromPartition(partition, m_gids, m_points);
            }
            public bool Redo(PortalPartition partition)
            {
                int count = m_gids.Length;
                int[] entryKeys = new int[count];
                for (int i = 0; i < count; ++i)
                    entryKeys[i] = m_entryKey;
                return AddToPartition(partition, m_gids, entryKeys, m_points);
            }
        }
        public class ReclaimElementControlAction : IControlAction
        {
            private int m_gid;
            private int m_entryKey;
            private Point3D m_point;

            public ReclaimElementControlAction(int gid, int entryKey, Point3D p)
            {
                m_gid = gid;
                m_entryKey = entryKey;
                m_point = p;
            }
            public bool Undo(PortalPartition partition)
            {
                return AddToPartition(partition, m_gid, m_entryKey, m_point);
            }
            public bool Redo(PortalPartition partition)
            {
                return RemoveFromPartition(partition, m_gid, m_entryKey, m_point);
            }
        }
        public class ReclaimElementsControlAction : IControlAction
        {
            public int[] m_gids;
            public int[] m_entryKeys;
            public Point3D[] m_points;

            public ReclaimElementsControlAction(int[] gids, int[] entryKeys, Point3D[] points)
            {
                m_gids = gids;
                m_entryKeys = entryKeys;
                m_points = points;
            }
            public bool Undo(PortalPartition partition)
            {
                return AddToPartition(partition, m_gids, m_entryKeys, m_points);
            }
            public bool Redo(PortalPartition partition)
            {
                return RemoveFromPartition(partition, m_gids, m_points);
            }
        }
        public class DeleteElementControlAction : IControlAction
        {
            private int m_gid;
            private int m_entryKey;
            private Point3D m_point;

            public DeleteElementControlAction(int gid, int entryKey, Point3D p)
            {
                m_gid = gid;
                m_entryKey = entryKey;
                m_point = p;
            }
            public bool Undo(PortalPartition partition)
            {
                return AddToPartition(partition, m_gid, m_entryKey, m_point, true);
            }
            public bool Redo(PortalPartition partition)
            {
                return RemoveFromPartition(partition, m_gid, m_entryKey, m_point, true);
            }
        }
        public class SwapElementsControlAction : IControlAction
        {
            private int m_gidA;
            private Point3D m_pointA;
            private int m_gidB;
            private Point3D m_pointB;

            public SwapElementsControlAction(int gidA, Point3D pointA, int gidB, Point3D pointB)
            {
                m_gidA = gidA;
                m_pointA = pointA;
                m_gidB = gidB;
                m_pointB = pointB;
            }
            public bool Undo(PortalPartition partition)
            {
                return Swap(true);
            }
            public bool Redo(PortalPartition partition)
            {
                return Swap(false);
            }
            private bool Swap(bool undo)
            {
                IPortalElement element1 = null;
                IPortalElement element2 = null;
                Point3D findP1 = undo ? m_pointA : m_pointB;
                Point3D findP2 = undo ? m_pointB : m_pointA;

                IPooledEnumerable pool1 = PortalsSystem.s_map.GetItemsInBounds(new Rectangle2D(findP1.X, findP1.Y, 1, 1));
                foreach (IEntity obj in pool1)
                {
                    if (obj is PortalStatic || obj is PortalDoor)
                    {
                        IPortalElement element = obj as IPortalElement;
                        if (element.GetLocation().Z == findP1.Z && element.GetGid() == m_gidA)
                        {
                            element1 = element;
                            break;
                        }
                    }
                }
                pool1.Free();

                IPooledEnumerable pool2 = PortalsSystem.s_map.GetItemsInBounds(new Rectangle2D(findP2.X, findP2.Y, 1, 1));
                foreach (IEntity obj in pool2)
                {
                    if (obj is PortalStatic || obj is PortalDoor)
                    {
                        IPortalElement element = obj as IPortalElement;
                        if (element.GetLocation().Z == findP2.Z && element.GetGid() == m_gidB)
                        {
                            element2 = element;
                            break;
                        }
                    }
                }
                pool2.Free();

                if (element1 == null || element2 == null)
                    return false;

                if (undo)
                {
                    element1.MoveToWorldWrapper(m_pointB);
                    element2.MoveToWorldWrapper(m_pointA);
                }
                else
                {
                    element1.MoveToWorldWrapper(m_pointA);
                    element2.MoveToWorldWrapper(m_pointB);
                }
                return true;
            }
        }
        public class MoveElementControlAction : IControlAction
        {
            private int m_gid;
            private Point3D m_origin;
            private Point3D m_destination;

            public MoveElementControlAction(int gid, Point3D origin, Point3D destination)
            {
                m_gid = gid;
                m_origin = origin;
                m_destination = destination;
            }
            public bool Undo(PortalPartition partition)
            {
                return Move(true);
            }
            public bool Redo(PortalPartition partition)
            {
                return Move(false);
            }
            private bool Move(bool undo)
            {
                Point3D queryLocation = undo ? m_destination : m_origin;
                Point3D resultLocation = undo ? m_origin : m_destination;

                IPooledEnumerable pool = PortalsSystem.s_map.GetItemsInBounds(new Rectangle2D(queryLocation.X, queryLocation.Y, 1, 1));
                foreach (IEntity obj in pool)
                {
                    if (obj is PortalStatic || obj is PortalDoor)
                    {
                        IPortalElement element = obj as IPortalElement;
                        if (element.GetLocation().Z == queryLocation.Z && element.GetGid() == m_gid)
                        {
                            element.MoveToWorldWrapper(resultLocation);
                            break;
                        }
                    }
                }
                pool.Free();
                return true;
            }
        }
        public class AdjustElementsControlAction : IControlAction
        {
            public int[] m_gids;
            public Point3D[] m_points;
            public Point3D m_adjustment;

            public AdjustElementsControlAction(int[] gids, Point3D[] points, Point3D adjustment)
            {
                m_gids = gids;
                m_points = points;
                m_adjustment = adjustment;
            }
            public bool Undo(PortalPartition partition)
            {
                return Adjust(true);
            }
            public bool Redo(PortalPartition partition)
            {
                return Adjust(false);
            }
            private bool Adjust(bool undo)
            {
                int multiplier = undo ? -1 : 1;
                int count = m_gids.Length;
                for (int i = 0; i < count; ++i)
                {
                    Point3D findPoint = m_points[i];
                    if (undo)
                    {
                        findPoint.X += m_adjustment.X;
                        findPoint.Y += m_adjustment.Y;
                        findPoint.Z += m_adjustment.Z;
                    }

                    // This is not ideal, however, maintaining the references is a complicated issue.
                    IPooledEnumerable pool = PortalsSystem.s_map.GetItemsInBounds(new Rectangle2D(findPoint.X, findPoint.Y, 1, 1));
                    foreach (IEntity obj in pool)
                    {
                        if (obj is PortalStatic || obj is PortalDoor)
                        {
                            IPortalElement element = obj as IPortalElement;
                            Point3D location = element.GetLocation();
                            if (location == findPoint && element.GetGid() == m_gids[i])
                            {
                                if (undo)
                                    element.MoveToWorldWrapper(m_points[i]);
                                else
                                {
                                    Point3D pos = new Point3D(m_points[i].X + m_adjustment.X, m_points[i].Y + m_adjustment.Y, m_points[i].Z + m_adjustment.Z);
                                    element.MoveToWorldWrapper(pos);
                                }
                                
                                break;
                            }
                        }
                    }
                    pool.Free();
                }

                return true;
            }
        }


        private static int S_MAX_ACTION_HISTORY = 10;
        private static int S_ACTION_INVALID = -1;
        public List<IControlAction> m_controlActionList;
        public int m_actionIdx; // points to an undo

        public PortalPartition m_partition;

        public ControlActionManager(PortalPartition partition)
        {
            m_controlActionList = new List<IControlAction>();
            m_controlActionList.Capacity = S_MAX_ACTION_HISTORY;
            m_partition = partition;
            m_actionIdx = S_ACTION_INVALID;
        }
        public bool Undo()
        {
            if (!CanUndo())
                return false;

            return m_controlActionList[m_actionIdx--].Undo(m_partition);
        }
        public bool Redo()
        {
            if (!CanRedo())
                return false;

            return m_controlActionList[++m_actionIdx].Redo(m_partition);
        }
        public void Flush()
        {
            m_controlActionList.Clear();
            m_actionIdx = S_ACTION_INVALID;
        }
        public bool CanUndo()
        {
            return m_actionIdx > S_ACTION_INVALID;
        }
        public bool CanRedo()
        {
            return m_actionIdx < m_controlActionList.Count - 1;
        }
        public void Commit(IControlAction action)
        {
            // Any entries beyond the action marker are effectively discarded.
            if (m_controlActionList.Count > m_actionIdx + 1)
                m_controlActionList = m_controlActionList.GetRange(0, m_actionIdx + 1);
            m_controlActionList.Add(action);
            m_actionIdx = m_controlActionList.Count - 1;
        }
    }
}
