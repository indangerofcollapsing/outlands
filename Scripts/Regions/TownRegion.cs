using System;
using System.Xml;
using Server.Mobiles;
using Server.Custom.Townsystem;
using Server.Gumps;
using Server;

namespace Server.Regions
{
	public class TownRegion : GuardedRegion
	{
		public TownRegion( XmlElement xml, Map map, Region parent ) : base( xml, map, parent )
		{
		}

        public override bool OnBeginSpellCast(Mobile m, ISpell s)
        {
            //find the town
            Town town = Town.FromRegion(this);

            if (town != null && OCTimeSlots.IsActiveTown(town) && s is Server.Spells.Third.WallOfStoneSpell)
                return true;
            else
                return base.OnBeginSpellCast(m, s);
        }

        public override void OnEnter(Mobile m)
        {
            base.OnEnter(m);

            if (!(m is PlayerMobile))
                return;

            PlayerMobile pm = (PlayerMobile)m;
            Town town = Town.FromRegion(this);

            if (town == null)
                return;

            string msg = "";
            switch (town.GuardState)
            {
                case GuardStates.None: msg = "You see no guards whatsoever as you enter the town."; break;
                case GuardStates.Lax: msg = "You notice guards scattered sparsely about, barely paying attention."; break;
                case GuardStates.Standard: msg = "You feel comfortable entering the town with a welcoming line of guards."; break;
                case GuardStates.Strong: msg = "You feel very comfortable entering the town, as the nearby guards are keeping a close watch on the citizens."; break;
                case GuardStates.Exceptional: msg = "You feel extremely safe, as the guards are both attentive and numerous."; break;
            }

            m.SendMessage(msg);

            if (town.Exiles.Contains(m))
            {
                m.SendMessage("You have been exiled from this town! You are not welcome here.");
                return;
            }
            
            if (pm.Citizenship != null)
                return;

            if (pm.SquelchCitizenship)
                return;

            if (pm.HasGump(typeof(CitizenshipGump)))
                pm.CloseGump(typeof(CitizenshipGump));

            pm.SendGump(new CitizenshipGump(town));

        }

        public override bool CheckVendorAccess(BaseVendor vendor, Mobile from)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster || IsDisabled())
                return true;

            Town town = Town.FromRegion(this);

            return ((town != null && town.GuardState == GuardStates.None) || from.ShortTermMurders < 5);
        }
	}
}