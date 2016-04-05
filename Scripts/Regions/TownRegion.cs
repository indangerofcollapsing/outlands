using System;
using System.Xml;
using Server.Mobiles;
using Server.Gumps;
using Server;
using Server.Items;

namespace Server.Regions
{
	public class TownRegion : GuardedRegion
	{
		public TownRegion( XmlElement xml, Map map, Region parent ) : base( xml, map, parent )
		{
		}

        public override bool OnBeginSpellCast(Mobile mobile, ISpell s)
        {
            return base.OnBeginSpellCast(mobile, s);
        }

        public override void OnEnter(Mobile mobile)
        {
            base.OnEnter(mobile);

            Town town = Towns.FromRegion(this);

            if (town == null)
                return;

            //TEST
            mobile.Say("Entering Town: " + town.TownName);
        }

        public override void OnExit(Mobile mobile)
        {
            base.OnExit(mobile);

            Town town = Towns.FromRegion(this);

            if (town == null)
                return;

            //TEST
            mobile.Say("Exiting Town: " + town.TownName);
        }
	}
}