using System;
using Server;
using Server.Network;

namespace Server.Items
{
    public class StaffBracelet : GoldBracelet
    {
        private AccessLevel m_Level;

        [Constructable]
        public StaffBracelet(): base()
        {
            Name = "staff bracelet";
            Hue = 2587;
        }

        public StaffBracelet(Serial serial): base(serial)
        {
        }

        public override bool OnEquip(Mobile from)
        {
            if (from.AccessLevel > AccessLevel.Player)
            {
                m_Level = from.AccessLevel;
                from.AccessLevel = AccessLevel.Player;

                from.Send(SpeedControl.Disable);
            }

            return base.OnEquip(from);
        }

        public override void OnRemoved(object parent)
        {
            if (parent is Mobile)
            {
                Mobile from = parent as Mobile;

                from.AccessLevel = m_Level;
                m_Level = AccessLevel.Player;

                if (from.AccessLevel > AccessLevel.Player)
                    from.Send(SpeedControl.MountSpeed);
            }

            base.OnRemoved(parent);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            //Version 0
            writer.Write((int)m_Level);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Level = (AccessLevel)reader.ReadInt();
            }
        }
    }
}