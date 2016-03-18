using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server.Network;
using Server.Items;
using Server.Gumps;
namespace Server.Custom.Townsystem
{

	// Alliance banner EAST
	class OCBAllianceBannerEast : TownDecorationItem
	{
		[Constructable]
		public OCBAllianceBannerEast()
			: base(0x42CA)
		{
			Movable = false;
		}

		public OCBAllianceBannerEast(Serial s)
			: base(s)
		{
			Movable = false;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}

		public override void OnTownUpdated(Town t)
		{
			if (t == null)
			{
				ItemID = 0x42CA;// UO Banner
				Hue = 0;
				Name = "banner";
			}

            ItemID = 0x159F;
			Hue = t.ControllingTown.HomeFaction.Definition.HuePrimary;
            Name = String.Format("{0} flag", t.ControllingTown.Definition.FriendlyName);

			Visible = true;
		}
	}

	// Alliance banner SOUTH
	class OCBAllianceBannerSouth : TownDecorationItem
	{
		[Constructable]
		public OCBAllianceBannerSouth()
			: base(0x42CA)
		{
			Movable = false;
		}

		public OCBAllianceBannerSouth(Serial s)
			: base(s)
		{
			Movable = false;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}

		public override void OnTownUpdated(Town t)
        {
            if (t == null)
            {
                ItemID = 0x42CA;// UO Banner
                Hue = 0;
                Name = "banner";
            }

            ItemID = 0x159F;
            Hue = t.ControllingTown.HomeFaction.Definition.HuePrimary;
            Name = String.Format("{0} flag", t.ControllingTown.Definition.FriendlyName);

			Visible = true;
		}
	}

	// Town Capture Banner
	public class TownCaptureBannerEast : TownDecorationItem
	{
		private Item m_Banner1;
		private Item m_Banner2;

		[Constructable]
		public TownCaptureBannerEast()
			: base(0x0A20)
		{
			Movable = false;
		}

		public TownCaptureBannerEast(Serial s)
			: base(s)
		{
		}

		public override string DefaultName {get { return "a town banner"; }	}

		public override void OnDelete()
		{
			if( m_Banner1 != null )
				m_Banner1.Delete();
			if (m_Banner2 != null)
				m_Banner2.Delete();
			base.OnDelete();
		}

		public override void Serialize(GenericWriter writer)
		{
			// safety in case someone decides to delete the banners and not the pole..
			FactionDefinition towndef = m_Town.HomeFaction.Definition;
			if (m_Town.ControllingTown != null)
				towndef = m_Town.ControllingTown.HomeFaction.Definition;
			if (m_Banner1 == null || m_Banner1.Deleted)
				CreateBanner1(towndef);
			if (m_Banner2 == null || m_Banner2.Deleted)
				CreateBanner2(towndef);


			base.Serialize(writer);
			writer.Write((int)0);

			// version 0
			writer.Write(m_Banner1);
			writer.Write(m_Banner2);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			
			// version 0
			m_Banner1 = reader.ReadItem();
			m_Banner2 = reader.ReadItem();
		}

		private void CreateBanner1(FactionDefinition facdef)
		{
			m_Banner1 = new Item(facdef.CapFlagID1);
			m_Banner1.Movable = false;
			m_Banner1.MoveToWorld(new Point3D(this.X + 1, this.Y + 1, this.Z + 7), this.Map);
			m_Banner1.ItemID = facdef.CapFlagID1;
			m_Banner1.Name = String.Format("{0} currently controls this town", facdef.FriendlyName);
			m_Banner1.Hue = facdef.HuePrimary;
			m_Banner1.Movable = false;
		}

		private void CreateBanner2(FactionDefinition facdef)
		{
			m_Banner2 = new Item(facdef.CapFlagID2);
			m_Banner2.Movable = false;
			m_Banner2.MoveToWorld(new Point3D(this.X + 1, this.Y, this.Z + 7), this.Map);
			m_Banner2.ItemID = facdef.CapFlagID2;
			m_Banner2.Name = String.Format("{0} currently controls this town", facdef.FriendlyName);
			m_Banner2.Hue = facdef.HuePrimary;
			m_Banner2.Movable = false;
		}

		public override void OnTownUpdated(Town t)
		{
			FactionDefinition facdef = t.HomeFaction.Definition;
			if (t.ControllingTown != null)
				facdef = t.ControllingTown.HomeFaction.Definition;

			if (m_Banner1 == null || m_Banner1.Deleted)
				CreateBanner1(facdef);
			if (m_Banner2 == null || m_Banner2.Deleted)
				CreateBanner2(facdef);

			m_Banner1.ItemID = facdef.CapFlagID1;
			m_Banner1.Name = String.Format("{0} currently controls this town", facdef.FriendlyName);
			m_Banner1.Hue = facdef.HuePrimary;

			m_Banner2.ItemID = facdef.CapFlagID2;
			m_Banner2.Name = String.Format("{0} currently controls this town", facdef.FriendlyName);
			m_Banner2.Hue = facdef.HuePrimary;
		}
	}
}

