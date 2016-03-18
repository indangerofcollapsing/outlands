using System; 
using Server.Items;
using Server.Guilds;
using Server.Multis;

namespace Server.Items
{
	public class SecureExpansionDeed : Item
	{
		public override string DefaultName 
		{ 
			get { return "a Secure Container Expansion Deed"; }  
		}

		[Constructable]
		public SecureExpansionDeed(): base ( 0x14F0 )
		{
			Name="a Secure Container Expansion Deed";
			Weight=1.0;
		}

		public SecureExpansionDeed( Serial serial ) : base( serial )
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
            		from.SendMessage("You must be in a house to expand the maximum allowed secure containers.");
            	}
            	else if ( !house.IsOwner( from ) )
            	{
            		from.SendMessage("You must be the House owner to expand the maximum allowed secure containers.");
            	}
                else if (house is HouseFoundation)
                {
                    house.BonusStorage += 125;
                    from.SendMessage("You have increased the maximum number of secure containers by 1 and the maximum number of locked down items by 125.");
                    Delete();
                }
            	else
            	{
                    house.MaxSecures += 1;
                    house.MaxLockDowns +=125;

                    from.SendMessage("You have increased the maximum number of secure containers by 1 and the maximum number of locked down items by 125.");
                    Delete();
            	}
            }
		}
	}
}