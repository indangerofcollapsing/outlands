using System;
using Server;

namespace Server.Items
{
	public class StaffStaff : BlackStaff
	{
        private AccessLevel m_Level;

        [Constructable]
		public StaffStaff()
		{
            Hue = 0x4F2;
            Name = "Staff staff";
        }

		public StaffStaff( Serial serial ) : base( serial )
		{
		}

        public override bool OnEquip(Mobile from)
        {
            if (from.AccessLevel > AccessLevel.Player)
            {
                m_Level = from.AccessLevel;
                from.AccessLevel = AccessLevel.Player;
            }
            return base.OnEquip(from);
        }

        public override void OnRemoved(object parent)
        {
            if (parent is Mobile)
            {
                ((Mobile)parent).AccessLevel = m_Level;
                m_Level = AccessLevel.Player;
            }
            
            base.OnRemoved(parent);
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
            writer.Write((int)m_Level);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            m_Level = (AccessLevel)reader.ReadInt();
		}
	}

    public class StaffRing : BaseRing
    {
        private AccessLevel m_Level;

        [Constructable]
        public StaffRing()
            : base(0x108a)
        {
            Weight = 0.1;
            Hue = 0x4F2;
        }

        public StaffRing(Serial serial)
            : base(serial)
        {
        }

        public override bool OnEquip(Mobile from)
        {
            if (from.AccessLevel > AccessLevel.Player)
            {
                m_Level = from.AccessLevel;
                from.AccessLevel = AccessLevel.Player;
            }
            return base.OnEquip(from);
        }

        public override void OnRemoved(object parent)
        {
            if (parent is Mobile)
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