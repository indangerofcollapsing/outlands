using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;


namespace Server.Items
{
	public class JewelryBlessTarget : Target
	{
		private JewelryBlessDeed m_Deed;

		public JewelryBlessTarget( JewelryBlessDeed deed ) : base( 1, false, TargetFlags.None )
		{
			m_Deed = deed;
		}

		protected override void OnTarget( Mobile from, object target )
		{
			if ( m_Deed.Deleted || m_Deed.RootParent != from )
				return;

            if (target is Beads || target is BaseBracelet || target is BaseEarrings || target is BaseNecklace || target is BaseRing)
			{
				Item item = (Item)target;

				if ( item.LootType == LootType.Blessed || item.BlessedFor == from || (Mobile.InsuranceEnabled && item.Insured) ) // Check if its already newbied (blessed)
				{
					from.SendLocalizedMessage( 1045113 ); // That item is already blessed
				}
				else if ( item.LootType != LootType.Regular )
				{
					from.SendLocalizedMessage( 1045114 ); // You can not bless that item
				}
				else if ( item.RootParent != from )
				{
					from.SendLocalizedMessage( 500509 ); // You cannot bless that object
				}
				else
				{
					item.LootType = LootType.Blessed;
					from.SendLocalizedMessage( 1010026 ); // You bless the item....

					m_Deed.Delete(); // Delete the bless deed
				}
			}
            else
            {
                from.SendLocalizedMessage(500509); // You cannot bless that object
            }
		}
	}

	public class JewelryBlessDeed : Item // Create the item class which is derived from the base item class
	{
		public override string DefaultName
		{
			get { return "a jewelry bless deed"; }
		}

		[Constructable]
		public JewelryBlessDeed() : base( 0x14F0 )
		{
			Weight = 1.0;
			LootType = LootType.Blessed;

            Hue = 2595;
		}

		public JewelryBlessDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			LootType = LootType.Blessed;

			int version = reader.ReadInt();
		}

		public override bool DisplayLootType{ get{ return false; } }

		public override void OnDoubleClick( Mobile from ) // Override double click of the deed to call our target
		{
			if ( !IsChildOf( from.Backpack ) ) // Make sure its in their pack
			{
				 from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else
			{
				from.SendLocalizedMessage( 1005018 ); // What would you like to bless? (Clothes Only)
				from.Target = new JewelryBlessTarget( this ); // Call our target
			 }
		}
	}
}