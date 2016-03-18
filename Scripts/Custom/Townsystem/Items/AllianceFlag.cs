using System;
using Server;
using Server.Items;

namespace Server.Custom.Townsystem
{
    [FlipableAttribute(5681, 5531)]
	public class AllianceFlag : Item
	{
        private Town m_Town;
        
        [CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
        public Town Town
        {
            get { return m_Town; }
            set { m_Town = value; Invalidate(); }
        }

        [Constructable]
		public AllianceFlag() : base(5681)
		{
            Movable = false;
        }

        public AllianceFlag(Serial serial)
            : base(serial)
		{
		}

        public override void OnMapChange()
        {
            base.OnMapChange();
            if (Location == Point3D.Zero) return;
            Town = Town.FromRegion(Region.Find(Location, Map));
            if (Town == null)
            {
                Delete();
                return;
            }
            Town.AllianceFlags.Add(this);
            Invalidate();
        }

        public override void OnDelete()
        {
            if (m_Town != null && m_Town.AllianceFlags.Contains(this))
                m_Town.AllianceFlags.Remove(this);

            base.OnDelete();
        }

        public void Invalidate()
        {
            if (m_Town == null) 
			{
                Delete();
            }
			else if (m_Town.HomeFaction == null || m_Town.HomeFaction.Alliance == null) 
			{
                Visible = false;
            }
            else 
			{
				Name = String.Format("{0} Alliance Flag", m_Town.HomeFaction.Alliance.Name);
				Hue = m_Town.HomeFaction.Alliance.BannerHue;
				ItemID = m_Town.HomeFaction.Alliance.BannerID;
                Visible = true;
            }
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );

            Town.WriteReference(writer, m_Town);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            m_Town = Town.ReadReference(reader);

            Invalidate();
		}
	}
}