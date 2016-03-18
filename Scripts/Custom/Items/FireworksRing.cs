using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server.Items;
namespace Server.Custom.Items
{
	class FireworksRing : GoldRing
	{
		private int m_Charges;

		[CommandProperty(AccessLevel.GameMaster)]
		public int Charges
		{
			get { return m_Charges; }
			set { m_Charges = value; }
		}

		[Constructable]
		public FireworksRing()
			: this(100)
		{
		}

		[Constructable]
		public FireworksRing(int charges)
			: this(charges, 0x496)
		{
		}

		[Constructable]
		public FireworksRing(int charges, int hue)
		{
			m_Charges = charges;
			LootType = LootType.Blessed;
			Hue = hue;
		}

		public override string DefaultName { get { return "a fireworks ring"; } }

		public override void OnSingleClick(Mobile from)
		{
			base.OnSingleClick(from);
			LabelTo(from, String.Format("[Charges: {0}]", m_Charges));
		}

		public FireworksRing(Serial serial)
			: base(serial)
		{
		}

		public override void OnDoubleClick(Mobile from)
		{
			BeginLaunch(from, true);
		}

		public void BeginLaunch(Mobile from, bool useCharges)
		{
			Map map = from.Map;

			if (map == null || map == Map.Internal)
				return;

			if (useCharges)
			{
				if (Charges > 0)
				{
					--Charges;
				}
				else
				{
					from.SendLocalizedMessage(502412); // There are no charges left on that item.
					return;
				}
			}

			Point3D ourLoc = GetWorldLocation();

			Point3D startLoc = new Point3D(ourLoc.X, ourLoc.Y, ourLoc.Z + 10);
			Point3D endLoc = new Point3D(startLoc.X + Utility.RandomMinMax(-2, 2), startLoc.Y + Utility.RandomMinMax(-2, 2), startLoc.Z + 32);

			Effects.SendMovingEffect(new Entity(Serial.Zero, startLoc, map), new Entity(Serial.Zero, endLoc, map),
				0x36E4, 5, 0, false, false, Hue, 0);

			Timer.DelayCall(TimeSpan.FromSeconds(1.0), new TimerStateCallback(FinishLaunch), new object[] { from, endLoc, map });
		}

		private void FinishLaunch(object state)
		{
			object[] states = (object[])state;

			Mobile from = (Mobile)states[0];
			Point3D endLoc = (Point3D)states[1];
			Map map = (Map)states[2];

			int hue = Utility.Random(40);

			if (Hue > 0)
				hue = Hue;
			else if (hue < 8)
				hue = 0x66D;
			else if (hue < 10)
				hue = 0x482;
			else if (hue < 12)
				hue = 0x47E;
			else if (hue < 16)
				hue = 0x480;
			else if (hue < 20)
				hue = 0x47F;
			else
				hue = 0;

			if (Utility.RandomBool())
				hue = Utility.RandomList(0x47E, 0x47F, 0x480, 0x482, 0x66D);

			int renderMode = Utility.RandomList(0, 2, 3, 4, 5, 7);

			Effects.PlaySound(endLoc, map, Utility.Random(0x11B, 4));
			Effects.SendLocationEffect(endLoc, map, 0x373A + (0x10 * Utility.Random(4)), 16, 10, hue, renderMode);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)1); // version

			//version 1
			writer.Write(Hue);

			//version 0

			writer.Write((int)m_Charges);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 1:
					{
						Hue = reader.ReadInt();
						goto case 0;
					}
				case 0:
					{
						m_Charges = reader.ReadInt();
						break;
					}
			}
		}
	}
}
