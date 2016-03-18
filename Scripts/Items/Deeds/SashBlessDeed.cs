using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server.Targeting;

namespace Server.Items.Deeds
{
	class BodysashBlessDeed : SpecificTypeBlessDeed
	{
		[Constructable]
		public BodysashBlessDeed() : base(typeof(BodySash)) { }
		public override string DefaultName { get { return "a body sash bless deed"; } }

		public BodysashBlessDeed(Serial serial) : base(serial) { }
		public override void Serialize(GenericWriter writer){ base.Serialize(writer); }
		public override void Deserialize(GenericReader reader) { base.Deserialize(reader); }
	}

	class BasehatBlessDeed : SpecificTypeBlessDeed
	{
		[Constructable]
		public BasehatBlessDeed() : base(typeof(BaseHat)) { }
		public override string DefaultName { get { return "a hat bless deed"; } }

		public BasehatBlessDeed(Serial serial) : base(serial) { }
		public override void Serialize(GenericWriter writer) { base.Serialize(writer); }
		public override void Deserialize(GenericReader reader) { base.Deserialize(reader); }
	}

	class RobeBlessDeed : SpecificTypeBlessDeed
	{
		[Constructable]
		public RobeBlessDeed() : base(typeof(Robe)) { }
		public override string DefaultName { get { return "a robe bless deed"; } }

		public RobeBlessDeed(Serial serial) : base(serial) { }
		public override void Serialize(GenericWriter writer) { base.Serialize(writer); }
		public override void Deserialize(GenericReader reader) { base.Deserialize(reader); }
	}

	public class SpecificTypeBlessTarget : Target // Create our targeting class (which we derive from the base target class)
	{
		private SpecificTypeBlessDeed m_Deed;
		private Type m_TargetType;

		public SpecificTypeBlessTarget(SpecificTypeBlessDeed deed, Type required_target_type)
			: base(1, false, TargetFlags.None)
		{
			m_Deed = deed;
			m_TargetType = required_target_type;
		}

		protected override void OnTarget(Mobile from, object target) // Override the protected OnTarget() for our feature
		{
			if (m_Deed.Deleted || m_Deed.RootParent != from)
				return;

			bool is_correct_type = m_TargetType.IsAssignableFrom(target.GetType());
			if (!is_correct_type)
			{
				from.SendMessage("This blessing deed only works with another kind of items.");
				return;
			}

			Item item = (Item)target;

			if (item.LootType == LootType.Blessed || item.BlessedFor == from || (Mobile.InsuranceEnabled && item.Insured)) // Check if its already newbied (blessed)
			{
				from.SendLocalizedMessage(1045113); // That item is already blessed
			}
			else if (item.LootType != LootType.Regular)
			{
				from.SendLocalizedMessage(1045114); // You can not bless that item
			}
			else if (item.RootParent != from)
			{
				from.SendLocalizedMessage(500509); // You cannot bless that object
			}
			else
			{
				item.LootType = LootType.Blessed;
				from.SendLocalizedMessage(1010026); // You bless the item....

				m_Deed.Delete(); // Delete the bless deed
			}
		}
	}

	public class SpecificTypeBlessDeed : Item
	{
		// The type that can be blessed. typeof(BodySash) for example
		private Type m_TargetItemType;

		[Constructable]
		public SpecificTypeBlessDeed(Type required_target_type)
			: base(0x14F0)
		{
			Weight = 1.0;
			LootType = LootType.Blessed;
			m_TargetItemType = required_target_type;
		}

		public SpecificTypeBlessDeed(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0); // version

			// item type
			writer.Write(m_TargetItemType.ToString());
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			LootType = LootType.Blessed;

			int version = reader.ReadInt();

			// version 0
			try 
			{ 
				m_TargetItemType = Type.GetType(reader.ReadString());
			}
			catch 
			{ 
				m_TargetItemType = null; 
			}

		}

		public override bool DisplayLootType { get { return false; } }

		public override void OnDoubleClick(Mobile from) // Override double click of the deed to call our target
		{
			if (!IsChildOf(from.Backpack)) // Make sure its in their pack
			{
				from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
			}
			else
			{
				from.SendMessage("What would you like to bless?");
				from.Target = new SpecificTypeBlessTarget(this, m_TargetItemType); // Call our target
			}
		}
	}
}
