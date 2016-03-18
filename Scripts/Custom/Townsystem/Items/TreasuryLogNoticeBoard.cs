using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server;
using Server.Network;
using Server.Items;
using Server.Gumps;

namespace Server.Custom.Townsystem
{
	[Flipable(0x1E5E, 0x1E5F)]
	public class TreasuryLogNoticeboard : Item
	{
		private Town m_Town;
		[CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
		public Town Town
		{
			get { return m_Town; }
			set { m_Town = value; }
		}

		[Constructable]
		public TreasuryLogNoticeboard()
			: base(0x1E5E)
		{
			Movable = false;
			Hue = 2213;
		}

		public TreasuryLogNoticeboard(Serial serial)
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
			Name = String.Format("The {0} Treasury Log", Town.Definition.FriendlyName);
		}

		public override void OnDoubleClick(Mobile from)
		{
			if(Town != null)
				Town.WithdrawLog.DisplayTo(from);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0);

			Town.WriteReference(writer, m_Town);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			Town = Town.ReadReference(reader);
		}
	}
}
