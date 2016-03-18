using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server;
using Server.Mobiles;
using Server.Items;

namespace Server.PortalSystem
{
    // Macro prevention. Players' actions are restricted when in a portal that is not "open". 
    public class DungeonPortalsRegion : Region
    {
        private static string s_actionsProhibited = "You can not perform actions here at this time.";
        private static string s_regionName = "PortalsRegion";
        public DungeonPortalsRegion()
            : base(s_regionName, PortalsSystem.s_map, 1000, GetPortalsArea())
        {
        }
        private static Rectangle2D[] GetPortalsArea()
        {
            return new Rectangle2D[] { new Rectangle2D(0, 0, PortalsSystem.s_map.Width, PortalsSystem.s_map.Height) };
        }
        private bool IsActionAllowed(Mobile m)
        {
            if (!(m is PlayerMobile))
                return true;

            PlayerMobile pm = (PlayerMobile)m;
            PortalPartition partition = PortalsSystem.GetPartitionAtPoint(pm.Location);
            if (partition == null)
                return true;

            if (pm.AccessLevel >= AccessLevel.Player)
                return true;

            return PortalsSystem.AccessManager.IsPortalActive(partition.m_partitionId);
        }
        public override bool AllowBeneficial(Mobile from, Mobile target)
        {
            if (!IsActionAllowed(from))
            {
                from.SendMessage(s_actionsProhibited);
                return false;
            }
            return true;
        }
        public override bool AllowHarmful(Mobile from, Mobile target)
        {
            if (!IsActionAllowed(from))
            {
                from.SendMessage(s_actionsProhibited);
                return false;
            }
            return true;
        }
        public override bool OnSkillUse(Mobile from, int Skill)
        {
            if (!IsActionAllowed(from))
            {
                from.SendMessage(s_actionsProhibited);
                return false;
            }
            return true;
        }
        public override bool OnBeginSpellCast(Mobile m, ISpell s)
        {
            if (s is PortalTransportSpell)
                return true;

            if (!IsActionAllowed(m))
            {
                m.SendMessage(s_actionsProhibited);
                return false;
            }
            return true;
        }
        public override bool OnDoubleClick(Mobile m, object o)
        {
            // Spellbook can be opened (to prevent login problems), but spells can't be cast.
            if (!(o is Container || o is DungeonControl || o is GhostTrap || o is Spellbook || IsActionAllowed(m)))
            {
                m.SendMessage(s_actionsProhibited);
                return false;
            }
            return true;
        }
        public override bool AllowHousing(Mobile from, Point3D p)
        {
            // Housing is never allowed.
            return false;
        }
    }
}
