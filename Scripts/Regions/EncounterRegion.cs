using System;
using System.Xml;
using Server;
using Server.Spells;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Spells.Fifth;
using Server.Spells.Fourth;
using Server.Spells.Third;
//using Server.Custom.Encounters;

namespace Server.Regions
{
	public class EncounterRegion : DungeonRegion
	{
        public EncounterRegion(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
		{
		}
        public override bool OnBeginSpellCast( Mobile m, ISpell s )
		{
			if ( ( s is GateTravelSpell || s is RecallSpell || s is MarkSpell || s is TeleportSpell) && m.AccessLevel == AccessLevel.Player )
			{
				m.SendMessage( "You cannot cast that spell here." );
				return false;
			}
			else
			{
				return base.OnBeginSpellCast( m, s );
            }
        }

        public override bool OnBeforeDeath(Mobile m)
        {
            return true;
        }
	}
}