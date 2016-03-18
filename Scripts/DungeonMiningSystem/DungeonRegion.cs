using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.DungeonMiningSystem
{
    /// <summary>
    /// Represents a volume of space beneath the portals plane. A player can (due to technical problems)
    /// fall from a portal, landing in space we do not want the player to reach. Any mobile that enters this
    /// volume will have the movement rejected by the server.
    /// </summary>
    public class PortalsCatchRegion : Region
    {
        public PortalsCatchRegion(string name, Map map, Rectangle3D area)
            : base(name, map, 1000, area)
        {
        }
        public override void OnLocationChanged(Mobile m, Point3D oldLocation)
        {
            m.Location = oldLocation;
        }
        public override bool OnMoveInto(Mobile m, Direction d, Point3D newLocation, Point3D oldLocation)
        {
            // The player has fallen beneath a portal.
            return false;
        }
    }
}
