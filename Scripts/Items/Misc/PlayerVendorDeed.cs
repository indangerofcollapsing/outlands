using System;
using Server;
using Server.Mobiles;
using Server.Multis;
using Server.Custom.Townsystem;
using Server.Regions;

namespace Server.Items
{
	public class ContractOfEmployment : Item
	{
		public override int LabelNumber{ get{ return 1041243; } } // a contract of employment

		[Constructable]
		public ContractOfEmployment() : base( 0x14F0 )
		{
			Weight = 1.0;
			//LootType = LootType.Blessed;
		}

		public ContractOfEmployment( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); //version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

        private static bool CheckExistance(Point3D loc, Map facet, Type type)
        {
            foreach (Item item in facet.GetItemsInRange(loc, 0))
            {
                if (type.IsAssignableFrom(item.GetType()))
                    return true;
            }

            return false;
        }

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else if ( from.AccessLevel >= AccessLevel.GameMaster )
			{
				from.SendLocalizedMessage( 503248 ); // Your godly powers allow you to place this vendor whereever you wish.

				Mobile v = new PlayerVendor( from, BaseHouse.FindHouseAt( from ) );

				v.Direction = from.Direction & Direction.Mask;
				v.MoveToWorld( from.Location, from.Map );

				v.SayTo( from, 503246 ); // Ah! it feels good to be working again.

				this.Delete();
			}
			else
			{
				BaseHouse house = BaseHouse.FindHouseAt( from );

				if ( house == null )
				{
                    if (CheckExistance(from.Location, from.Map, typeof(MarketFloor)))
                    {
                        BankBox b = from.BankBox;
                        bool vendor, contract;
					    BaseHouse.IsThereVendor( from.Location, from.Map, out vendor, out contract );
                        Town town = Town.FromRegion(Region.Find(from.Location, from.Map));

                        if (b.TotalGold < 5000)
                        {
                            from.SendMessage("You must have at least 5000 gold in your bank account to place a vendor in the town market.");
                        }
                        else if (town == null)
                        {
                            from.SendMessage("Vendors can only be placed in houses or in designated town markets.");
                        }
                        else if (town != Town.CheckCitizenship(from))
                        {
                            from.SendMessage(String.Format("You are not a citizen of {0}!", town.Definition.FriendlyName));
                        }
                        else if (vendor)
                        {
                            from.SendLocalizedMessage(1062677); // You cannot place a vendor or barkeep at this location.
                        }
                        else if (contract)
                        {
                            from.SendLocalizedMessage(1062678); // You cannot place a vendor or barkeep on top of a rental contract!
                        }
                        else
                        {
                            b.ConsumeUpTo(typeof(Gold), 5000);
                            from.SendMessage("5000 gold has been removed from your bank box.");

                            Mobile v = new PlayerVendor(from, house);
                            v.Direction = from.Direction & Direction.Mask;
                            v.MoveToWorld(from.Location, from.Map);

                            v.SayTo(from, 503246); // Ah! it feels good to be working again.

                            this.Delete();
                        }
                    }
                    else
                    {
                        from.SendMessage("Vendors can only be placed in houses or in designated town markets.");//from.SendLocalizedMessage( 503240 ); // Vendors can only be placed in houses.	
                    }
				}
				else if ( !BaseHouse.NewVendorSystem && !house.IsFriend( from ) )
				{
					from.SendLocalizedMessage( 503242 ); // You must ask the owner of this building to name you a friend of the household in order to place a vendor here.
				}
				else if ( BaseHouse.NewVendorSystem && !house.IsOwner( from ) )
				{
					from.SendLocalizedMessage( 1062423 ); // Only the house owner can directly place vendors.  Please ask the house owner to offer you a vendor contract so that you may place a vendor in this house.
				}
				else if ( !house.Public || !house.CanPlaceNewVendor() ) 
				{
					from.SendLocalizedMessage( 503241 ); // You cannot place this vendor or barkeep.  Make sure the house is public and has sufficient storage available.
				}
				else
				{
					bool vendor, contract;
					BaseHouse.IsThereVendor( from.Location, from.Map, out vendor, out contract );

                    GuardedRegion guardedRegion = (GuardedRegion) from.Region.GetRegion( typeof( GuardedRegion ) );

                    if (guardedRegion != null)
                    {
                        from.SendMessage("You cannot place a vendor inside of a house in a guarded region.");
                    }
					else if ( vendor )
					{
						from.SendLocalizedMessage( 1062677 ); // You cannot place a vendor or barkeep at this location.
					}
					else if ( contract )
					{
						from.SendLocalizedMessage( 1062678 ); // You cannot place a vendor or barkeep on top of a rental contract!
					}
					else
					{
						Mobile v = new PlayerVendor( from, house );

						v.Direction = from.Direction & Direction.Mask;
						v.MoveToWorld( from.Location, from.Map );

						v.SayTo( from, 503246 ); // Ah! it feels good to be working again.

						this.Delete();
					}
				}
			}
		}
	}
}