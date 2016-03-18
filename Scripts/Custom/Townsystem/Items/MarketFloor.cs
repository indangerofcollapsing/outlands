using System;
using Server;
using Server.Items;

namespace Server.Custom.Townsystem
{
	public class MarketFloor : SandFlagstones
	{
        Town m_Town;

        [Constructable]
		public MarketFloor() : base()
		{
        }

        public MarketFloor(Serial serial)
            : base(serial)
		{
		}

        public override void OnMapChange()
        {
            base.OnMapChange();
            if (Location == Point3D.Zero) return;
            Town town = Town.FromRegion(Region.Find(Location, Map));
            if (town == null)
            {
                Delete();
                return;
            }

            m_Town = town;
            if (!town.MarketTiles.Contains(this))
                town.MarketTiles.Add(this);

        }

        public override void OnDelete()
        {
            if (m_Town != null && m_Town.MarketTiles.Contains(this))
                m_Town.MarketTiles.Remove(this);

            base.OnDelete();
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 );

            Town.WriteReference(writer, m_Town);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        m_Town = Town.ReadReference(reader);
                        break;
                    }
            }

		}
	}
}