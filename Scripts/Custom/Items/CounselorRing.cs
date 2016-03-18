using System;
using Server;

namespace Server.Items
{
    public class CounselorRing : BaseRing
    {
        private AccessLevel m_Level;

        [Constructable]
        public CounselorRing()
            : base(0x108a)
        {
            Weight = 0.1;
            Hue = 0x4F2;
            LootType = LootType.Blessed;
        }

        public CounselorRing(Serial serial)
            : base(serial)
        {
        }

        public override bool OnEquip(Mobile from)
        {
            m_Level = from.AccessLevel;
            if (from.AccessLevel == AccessLevel.Player)
            {
               
                from.AccessLevel = AccessLevel.Counselor;
            }
            return base.OnEquip(from);
        }

        public override void OnRemoved(object parent)
        {
            if (parent is Mobile && m_Level < AccessLevel.Counselor)
            {
                ((Mobile)parent).AccessLevel = m_Level;
                m_Level = AccessLevel.Player;
            }

            base.OnRemoved(parent);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write((int)m_Level);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            m_Level = (AccessLevel)reader.ReadInt();

        }
    }
}