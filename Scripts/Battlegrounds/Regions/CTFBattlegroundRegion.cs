using Server.Custom.Battlegrounds.Items;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Server.Custom.Battlegrounds.Regions
{
    class CTFBattlegroundRegion : BattlegroundRegion
    {
        public CTFBattlegroundRegion(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        {

        }
    }
}
