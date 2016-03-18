using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Server.Custom.Townsystem;
using Server;
using Server.Items;

namespace Server.Custom.Townsystem
{

	public abstract class TownDecorationItem : Item
	{
		protected Town m_Town;

		public abstract void OnTownUpdated(Town t);

		public TownDecorationItem(int itemId) : base(itemId)
		{
		}
		
		public TownDecorationItem(Serial s) : base(s)
		{
		}

		public override void OnMapChange()
		{
			if( m_Town != null )
				UnregisterItem(this, m_Town);

			base.OnMapChange();

			m_Town = Town.FromRegion(Region.Find(Location, Map));
			if (m_Town == null)
			{
				Delete();
				return;
			}

			RegisterItem(this, m_Town);
			OnTownUpdated(m_Town);
		}

		public override void OnDelete()
		{
			if( m_Town != null )
				UnregisterItem(this, m_Town);
			base.OnDelete();
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
			m_Town = Town.ReadReference(reader);
			RegisterItem(this, m_Town);
		}




		// STATIC COLLECTION OF ALL ITEMS FOR QUICK LOOKUP
		public static List<TownDecorationItem>[] AllTownDecoItems = new List<TownDecorationItem>[(int)ETownId.NumTowns];
		static TownDecorationItem()
		{
			for (int i = 0; i < (int)ETownId.NumTowns; ++i)
				AllTownDecoItems[i] = new List<TownDecorationItem>();
		}

		public static void RegisterItem(TownDecorationItem item, Town town)
		{
			Debug.Assert(AllTownDecoItems[(int)town.Definition.TownId].Contains(item) == false);
			AllTownDecoItems[(int)town.Definition.TownId].Add(item);
		}

		public static void UnregisterItem(TownDecorationItem item, Town town)
		{
			bool ok = AllTownDecoItems[(int)town.Definition.TownId].Remove(item);
			Debug.Assert(ok);
		}

		public static List<TownDecorationItem> GetAllItemsFor(Town town)
		{
			return AllTownDecoItems[(int)town.Definition.TownId];
		}

		public static void OnTownChanged(Town town)
		{
			foreach (TownDecorationItem tdi in GetAllItemsFor(town))
			{
				tdi.OnTownUpdated(town);
			}
		}

	}
}
