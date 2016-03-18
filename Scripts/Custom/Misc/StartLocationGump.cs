// Author: CEO
// Released: 12/08/07
using System;
using System.Collections;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Custom.Townsystem;
using System.Collections.Generic;

namespace Server.Gumps
{
    public class StartLocationGump : Gump
    {
        private Mobile m_Mobile;
        private Map m_Map;
        private int m_Index;

        private Dictionary<Town, Tuple<int, int, Point3D>> m_TownInfo; // tuple is x, y, point3d for map and movement

        public StartLocationGump(Mobile mobile, Map map, int index)
            : base(100, 100)
        {            
            initializeTowns();
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
                var destination = m_TownInfo[Town.Towns[m_Index]].Item3;
                m_Mobile.MoveToWorld(destination, m_Map);
                Effects.PlaySound(destination, m_Map, 0x1FE);
            }
        }

        private void initializeTowns()
        {
            m_TownInfo = new Dictionary<Town, Tuple<int, int, Point3D>>();
            m_TownInfo.Add(Town.Parse("Britain"), Tuple.Create(149, 188, new Point3D( 1504, 1620, 21)));
            m_TownInfo.Add(Town.Parse("Minoc"), Tuple.Create(224, 79, new Point3D( 2477, 399, 15)));
            m_TownInfo.Add(Town.Parse("Vesper"), Tuple.Create(249, 124, new Point3D( 2778, 971, 0)));
            m_TownInfo.Add(Town.Parse("Cove"), Tuple.Create(224, 142, new Point3D( 2235, 1214, 0)));
            m_TownInfo.Add(Town.Parse("Yew"), Tuple.Create(80, 130, new Point3D( 533, 983, 0)));
            m_TownInfo.Add(Town.Parse("Ocllo"), Tuple.Create(315, 274, new Point3D( 3661, 2627, 0)));
            m_TownInfo.Add(Town.Parse("Magincia"), Tuple.Create(312, 234, new Point3D( 3714, 2220, 20)));
            m_TownInfo.Add(Town.Parse("Serpent's Hold"), Tuple.Create(264, 354, new Point3D( 2980, 3406, 15)));
            m_TownInfo.Add(Town.Parse("Nujel'm"), Tuple.Create(316, 159, new Point3D( 3744, 1315, 0)));
            m_TownInfo.Add(Town.Parse("Moonglow"), Tuple.Create(372, 149, new Point3D( 4403, 1155, 0)));
            m_TownInfo.Add(Town.Parse("Skara Brae"), Tuple.Create(75, 233, new Point3D( 612, 2242, 0)));
            m_TownInfo.Add(Town.Parse("Jhelom"), Tuple.Create(137, 380, new Point3D( 1364, 3822, 0)));
            m_TownInfo.Add(Town.Parse("Trinsic"), Tuple.Create(177, 293, new Point3D( 1840, 2729, 0)));
        }
    }
}