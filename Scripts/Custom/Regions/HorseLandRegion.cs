using Server.Mobiles;
using Server.Network;
using Server.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Server.Custom.Regions
{
    class HorseLandRegion : DungeonRegion
    {
        private static int[] m_HorseValues = new int[] 
            { 
                116, 178, 179, 
                0xC8,
				0xE2,
				0xE4,
				0xCC,
                0x7A, 0xBE,
                0x77, 0x78,
                0x79, 0x76,
                793, 0x90,
                0x75, 
            };

        public HorseLandRegion(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent) 
        {
        }

        public override void OnEnter(Mobile m)
        {
            base.OnEnter(m);
            if (m.Player)
            {
                m.BodyMod = Utility.RandomList(m_HorseValues);
                if (m.AccessLevel == AccessLevel.Player)
                    m.Send(SpeedControl.MountSpeed);
            }
        }

        public override void OnExit(Mobile m)
        {
            base.OnExit(m);
            if (m.Player)
            {
                m.BodyMod = 0;
                if (m.AccessLevel == AccessLevel.Player)
                    m.Send(SpeedControl.Disable);
            }
        }
    }
}
