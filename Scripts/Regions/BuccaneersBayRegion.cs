using System;
using System.Xml;
using Server;
using Server.Custom.Pirates;

namespace Server.Regions
{
	public class BuccsBayRegion : BaseRegion
	{
        public BuccsBayRegion(String name, Map map, int priority, Rectangle2D[] area): base(name, map, priority, area)
		{
		}

        public override void OnEnter(Mobile m)
        {
            base.OnEnter(m);
        }
	}
}