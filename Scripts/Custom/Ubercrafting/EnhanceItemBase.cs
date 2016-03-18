using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server;
using Server.Items;
using Server.Gumps;
using Server.Network;
using Server.Targeting;
using Server.Targets;

namespace Server.Custom.Ubercrafting
{
	public abstract class EnhanceItemBase : Item
	{
		public EnhanceItemBase(int id) : base(id) { Weight = 1.0; }
		public EnhanceItemBase(Serial serial) : base(serial) { }

		public abstract SkillName RequiredSkill(Item i);
		public abstract double RequiredSkillLevel(Item i);
		public abstract bool ValidTarget(Item i);
		public abstract bool Enhance(Item i, Mobile enhancer);

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.WriteEncodedInt(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadEncodedInt();
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (this.Deleted)
				return;

			if ( !this.IsChildOf( from.Backpack ) ) 
			{
				from.SendLocalizedMessage( 1042010 ); //You must have the object in your backpack to use it.
				return;
			}
			if ( !from.InRange( this.GetWorldLocation(), 1 ) )
			{
				from.LocalOverheadMessage(MessageType.Regular, 906, 1019045); // I can't reach that.
				return;
			}
			
			// new target
			from.ClearTarget();
			from.SendMessage("Which item do you wish to permanently enhance?");
			from.Target = new EnhanceItemTarget(this);
		}

		private class EnhanceItemTarget : Target
		{
			EnhanceItemBase m_Enhancer;

			public EnhanceItemTarget(EnhanceItemBase enhancer)
				: base(1, false, TargetFlags.None)
			{
				m_Enhancer = enhancer;
			}

			protected override void OnTarget(Mobile from, object t)
			{
				Item targeted_item = t as Item;
				if (m_Enhancer == null || m_Enhancer.Deleted || targeted_item == null)
					return;

				if (!m_Enhancer.ValidTarget(targeted_item))
				{
					from.SendMessage("That can not be enhanced using this item");
					return;
				}
				if (m_Enhancer.RequiredSkillLevel(targeted_item) > from.Skills[m_Enhancer.RequiredSkill(targeted_item)].Base)
				{
					from.SendMessage("You are not skilled enough to enhance this item. (requires {0} : {1})", from.Skills[m_Enhancer.RequiredSkill(targeted_item)].Name, m_Enhancer.RequiredSkillLevel(targeted_item));
					return;
				}
				if (m_Enhancer.Enhance(targeted_item, from))
				{
					m_Enhancer.Delete();
					from.SendMessage("You successfully enhance the item");
				}
			}
		}
	}
}
