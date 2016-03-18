using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Server.Targeting;

namespace Server.PortalSystem
{
    /// <summary>
    /// Provides a convenient way of logging the item contents within a rect.
    /// </summary>
    public class SelectItemsToDumpTarget : Target
    {
        private Point3D m_firstPoint;

        public SelectItemsToDumpTarget(Point3D firstPoint = default(Point3D))
            : base(-1, true, TargetFlags.None, false)
        {
            m_firstPoint = firstPoint;
        }

        protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
        {
        }
        protected override void OnTarget(Mobile from, object targeted)
        {
            if (m_firstPoint == default(Point3D))
            {
                Point3D p;
                if (targeted is LandTarget)
                {
                    LandTarget landTarget = targeted as LandTarget;
                    p = landTarget.Location;
                }
                else if (targeted is IPortalElement)
                {
                    IPortalElement element = targeted as IPortalElement;
                    p = element.GetLocation();
                }
                else
                {
                    return;
                }

                from.Target = new SelectItemsToDumpTarget(p);
            }
            else
            {
                Point3D p;
                if (targeted is LandTarget)
                {
                    LandTarget landTarget = targeted as LandTarget;
                    p = landTarget.Location;
                }
                else if (targeted is IPortalElement)
                {
                    IPortalElement element = targeted as IPortalElement;
                    p = element.GetLocation();
                }
                else
                {
                    return;
                }

                string directory = "PortalSystemDebug";
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                string date = DateTime.Today.ToString("d");
                date = date.Replace("/", "_");
                string file = directory + "\\portalLog_" + date + ".txt";

                StreamWriter writer = null;
                if (!File.Exists(file))
                {
                    writer = File.CreateText(file);
                }
                else
                {
                    writer = new StreamWriter(file, true);
                }

                Rectangle2D rect = new Rectangle2D(m_firstPoint, p);
                IPooledEnumerable penum = PortalsSystem.s_map.GetItemsInBounds(rect);

                // Find the actual bounds.
                Point3D center = new Point3D(m_firstPoint.X + ((p.X - m_firstPoint.X) / 2), m_firstPoint.Y + ((p.Y - m_firstPoint.Y) / 2), m_firstPoint.Z);

                foreach (Item item in penum)
                {
                    if (item is IPortalElement)
                    {
                        //writer.WriteLine("{0},{1},{2},{3}", item.ItemID, item.Location.X, item.Location.Y, item.Location.Z);
                        //Point3D relativeAnchor = from.Location;
                        Point3D relativeAnchor = new Point3D(19, 30, 0);
                        Point3D displacement = new Point3D(item.Location.X - relativeAnchor.X,
                             item.Location.Y - relativeAnchor.Y, item.Location.Z - relativeAnchor.Z);
                        writer.WriteLine("arenaPieces.Add(new Pair<int,Point3D>({0},new Point3D({1},{2},{3})));", item.ItemID, displacement.X, displacement.Y, displacement.Z);
                        //writer.WriteLine("From a center of {0}, {1}", center.ToString(), item.Location.ToString());
                    }
                }
                penum.Free();
                writer.WriteLine("");
                writer.Close();
            }
        }
    }
}