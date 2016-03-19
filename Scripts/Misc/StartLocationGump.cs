// Author: CEO
// Released: 12/08/07
using System;
using System.Collections;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;

using System.Collections.Generic;

namespace Server.Gumps
{
    public class StartLocationGump : Gump
    {
        private Mobile m_Mobile;
        private Map m_Map;
        private int m_Index;

        public StartLocationGump(Mobile mobile, Map map, int index): base(100, 100)
        {                       
            Closable = false;
            Dragable = false;

            m_Mobile = mobile;
            m_Map = map;
            m_Index = index;

            AddPage(0);

            AddBackground(0, 0, 615, 480, 0x2436);

            AddImage(0, 0, 0x157C);
            AddImage(35, 35, 0x1598);
            AddButton(355, 430, 0x992, 0x993, 1, GumpButtonType.Reply, 0);

            /*
            for (int i = 0; i < Town.Towns.Count; i++)
            {
                var town = Town.Towns[i];
                var tuple = m_TownInfo[town];
                bool selected = i == index;
                var hue = selected ? 1153 : town.HomeFaction.Definition.HuePrimary;
                var name = town.HomeFaction.Definition.FriendlyName;


                AddLabel(tuple.Item1 - 3, tuple.Item2 - 17, hue - 1, name);
                AddButton(tuple.Item1, tuple.Item2, 0x845, 0x846, 100 + i, GumpButtonType.Reply, 0);

                if (selected)
                {
                    AddLabel(420, 40, town.HomeFaction.Definition.HuePrimary - 1, name);
                    AddLabel(430, 55, 1153, String.Format("Sales Tax: {0}%", (town.SalesTax * 100).ToString()));
                    AddLabel(430, 70, 1153, String.Format("Guards: {0}", town.GuardState.ToString()));
                    AddLabel(430, 85, 1153, String.Format("Citizens: {0}", town.ActiveCitizens));
                    AddLabel(430, 100, 1153, String.Format("Miltia: {0}", town.ActiveMilitia));
                    AddLabel(430, 115, 1153, "Town Buffs:");
                    AddLabel(440, 130, 1153, TownBuff.GetBuffName(town.PrimaryCitizenshipBuff));
                    int y = 145;
                    foreach (var b in town.SecondaryCitizenshipBuffs)
                    {
                        AddLabel(440, y, 1153, TownBuff.GetBuffName(b));
                        y += 15;
                    }
                }
            }
            */

            AddHtmlLocalized(80, 430, 250, 20, 1010591, 0xffffff, false, false);
        }


        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info.ButtonID == 0) // close/cancel
                return;

            else if (info.ButtonID >= 100 && info.ButtonID <= 113)
                m_Mobile.SendGump(new StartLocationGump(m_Mobile, m_Map, info.ButtonID - 100));

            else if (info.ButtonID == 1)
            {
                Point3D destination = new Point3D(1504, 1620, 21);
                Map map = Map.Felucca;

                m_Mobile.MoveToWorld(destination, map);
                Effects.PlaySound(destination, map, 0x1FE);
            }
        }
    }
}