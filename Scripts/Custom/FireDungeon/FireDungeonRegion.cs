using System;
using Server;
using Server.Custom;
using System.Xml;
using Server.Items;


namespace Server.Regions
{
    public class FireDungeonRegion : DungeonRegion
    {
        public FireDungeonRegion(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent) 
        {
            if (map == Map.Felucca)
                FireDungeon.RegionOne = this;
            else
                FireDungeon.RegionTwo = this;
        }

        public override void OnDeath(Mobile m)
        {
            if (m is Server.Mobiles.BaseCreature)
                DousingGuild.RegionDeath(m);

            base.OnDeath(m);
        }

        public override void OnEnter(Mobile m) {
            base.OnEnter(m);
        }

        public override void OnExit(Mobile m) {
            base.OnExit(m);
        }
    }
}