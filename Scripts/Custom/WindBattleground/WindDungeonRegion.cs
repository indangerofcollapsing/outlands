using System;
using Server;
using Server.Custom;
using System.Xml;
using Server.Items;

namespace Server.Regions
{
    public class WindDungeonRegion : DungeonRegion
    {
        public WindDungeonRegion(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        {
            WindBattleground.Region = this;
        }

        public override void OnEnter(Mobile m)
        {
            WindBattleground.OnEnterDungeon(m);

            base.OnEnter(m);
        }

        public override void OnExit(Mobile m)
        {

            WindBattleground.OnExitDungeon(m);
            base.OnExit(m);
        }
    }
}