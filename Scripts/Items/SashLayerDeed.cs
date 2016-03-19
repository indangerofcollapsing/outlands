using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;


namespace Server.Items
{
	public class SashLayerTarget : Target
	{
		private SashLayerDeed m_Deed;

		public SashLayerTarget( SashLayerDeed deed ) : base( 1, false, TargetFlags.None )
		{
			m_Deed = deed;
		}

		protected override void OnTarget( Mobile from, object target )
		{
			if ( m_Deed.Deleted )
				return;
            
            if ( target is BodySash)
			{
                Item item = (Item)target;
                
				if ( item.Layer == Layer.Earrings ) // Check if its already layered
				{
					from.SendMessage( "That Body Sash is already Layered" );
				}

				else
				{
					if( item.RootParent != from ) // Make sure its in their pack or they are wearing it
					{
						from.SendMessage( "Make sure the item is in your backpack" );
					}
					else
					{
						item.Layer = Layer.Earrings;
						if ( item.Name != null )
						{
							item.Name = item.Name + " [Layered]";
						}
						else
						{
							item.Name = "a body sash [Layered]";
						}
						from.SendMessage( "You layer the sash!" );
						m_Deed.Delete(); // Delete the sash layer deed
					}
				}
			}
			else
			{
				from.SendMessage( "You cannot layer that object" );
			}
		}
	}

	public class SashLayerDeed : Item // Create the item class which is derived from the base item class
	{
		[Constructable]
		public SashLayerDeed() : base( 0x14F0 )
		{
			Weight = 1.0;
			Name = "a sash layering deed";
			LootType = LootType.Blessed;
			Hue = 1266;	
		}

		public SashLayerDeed( Serial serial ) : base( serial )
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
				from.SendMessage( "What sash would you like to layer?" ); 
				from.Target = new SashLayerTarget( this ); // Call our target
			 }
		}	
	}
}