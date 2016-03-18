using System;
using System.Xml;
using Server;
using Server.Custom.Townsystem;
using Server.Spells;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Spells.Fifth;
using Server.Spells.Fourth;
using Server.Spells.Third;
//using Server.Custom.Encounters;

namespace Server.Regions
{
	public class TreasuryRegion : Region
	{
        public TreasuryRegion(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
		{
		}

		public TreasuryRegion(string name, Rectangle3D area, Region parent, Map map)
			: base(name, map, parent, area)
		{
		}

        public override void OnEnter(Mobile m)
        {
			m.SendMessage("You have entered the Kings treasury");
        }

		public override void OnExit(Mobile m)
		{
			m.SendMessage("You have left the Kings treasury");
			base.OnExit(m);
		}

		public override bool OnBeginSpellCast(Mobile m, ISpell s)
		{
			if (s is GateTravelSpell)
			{
				m.SendMessage("You can not use that spell here.");
				return false;
			}
			return true;
		}
	}
}