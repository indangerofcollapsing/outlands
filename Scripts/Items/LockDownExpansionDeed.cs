using System; 
using Server.Items;

using Server.Multis;

namespace Server.Items
{
	public class LockDownExpansionDeed : Item
	{
		public override string DefaultName 
		{ 
			get { return "a Lock Down Expansion Deed"; }  
		}

		[Constructable]
		public LockDownExpansionDeed(): base ( 0x14F0 )
		{
			Name="a Lock Down Expansion Deed";
			Weight=1.0;
		}

		public LockDownExpansionDeed( Serial serial ) : base( serial )
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

			int version = reader.ReadInt();
			if ( Weight == 0.0 )
			{
				Weight = 1.0;
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
            else
            {
            	BaseHouse house = BaseHouse.FindHouseAt( from );

            	if (house == null)
            	{
            		from.SendMessage("You must be in a house to expand the maximum number of locked down items.");
            	}
            	else if ( !house.IsOwner( from ) )
            	{
            		from.SendMessage("You must be the House owner to expand the maximum number of locked down items.");
            	}
                else if (house is HouseFoundation)
                {
                    house.BonusLockdowns += 250;
                    from.SendMessage("You have increased the maximum number of locked down items by 250.");
                    Delete();
                }
            	else
            	{
                    house.MaxLockDowns +=250;

                    from.SendMessage("You have increased the maximum number of locked down items by 250.");

                    Delete();
            	}
            }
		}
	}
}