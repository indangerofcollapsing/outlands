using System;
using System.Collections.Generic;
using Server;
using Server.Regions;
using Server.Items;

namespace Scripts.Custom.Misc
{
    public class OrcFort
    {
        public class OrcFortRegion : BaseRegion
        {
            public override bool AllowHousing(Mobile from, Point3D p)
            {
                return false;
            }

            public OrcFortRegion(String name, Map map, int priority, Rectangle2D area)
                : base(name, map, priority, area)
            {
            }

            public override void OnEnter(Mobile m)
            {
                base.OnEnter(m);

                if (m.NetState != null && m_Multi != null && !m_Multi.Deleted)
                {
                    m_Multi.SendInfoTo(m.NetState);
                    Timer.DelayCall(TimeSpan.FromSeconds(2.0), delegate { m_Multi.SendInfoTo(m.NetState); });
                }
                    
            }
        }

        private static OrcFortRegion m_Region;
        private static readonly Rectangle2D m_Area = new Rectangle2D(new Point2D(1716, 999), new Point2D(1796, 1159));
        private static Item m_Multi;

        public static void Initialize()
        {
                if (m_Region == null)
                {
                    m_Region = new OrcFortRegion("OrcFortRegion", Map.Felucca, 5, m_Area);
                    m_Region.Register();
                }
        }

        public class OrcFortMulti : BaseMulti
        {
            [Constructable]
            public OrcFortMulti()
                : base(0x15B)
            {
                if (m_Multi != null && !m_Multi.Deleted)
                    base.Delete();
                else
                    m_Multi = this;

            }

            public OrcFortMulti(Serial serial)
                : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
                writer.WriteEncodedInt(0);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                int version = reader.ReadEncodedInt();

                m_Multi = this;
            }
        }
    }
}
