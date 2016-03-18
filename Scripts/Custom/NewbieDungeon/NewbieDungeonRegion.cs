using Server.Mobiles;
using Server.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Server.Regions
{
    public class NewbieDungeonRegion : DungeonRegion
    {
        public static double SkillGainCap = 65.0;

        public NewbieDungeonRegion(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent) 
        {
        }

        public override void OnEnter(Mobile m)
        {
            base.OnEnter(m);
            if (m.Player)
            {
                m.SendMessage("You will see reduced gold drops and fail to gain skill past 65 in this region.");
            }
        }

        public override bool AllowHarmful(Mobile from, Mobile target)
        {
            if (target is PlayerMobile && from is PlayerMobile)
            {
                // allow players to defend themselves
                bool attackedBy = from.Aggressors.Any(a => a.Attacker == target || a.Defender == target);
                bool attacked = from.Aggressed.Any(a => a.Attacker == target || a.Defender == target);
                if (target.Criminal || target.ShortTermMurders >= 5 || attackedBy || attacked)
                {
                    return true;
                }
                else
                {
                    from.SendMessage("You cannot do that in the new player dungeon.");
                    return false;
                }
            }
            return base.AllowHarmful(from, target);
        }
    }
}
