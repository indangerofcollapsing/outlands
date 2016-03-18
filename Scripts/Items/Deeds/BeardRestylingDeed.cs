using System;
using Server.Mobiles;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using Server.Gumps;

namespace Server.Items
{
	public class BeardRestylingDeed : Item
	{
		[Constructable]
		public BeardRestylingDeed() : base( 0x14F0 )
		{
			Weight = 1.0;
			LootType = LootType.Blessed;
		}

        public override string DefaultName
        {
            get
            {
                return "a coupon for a free beard restyling";
            }
        }

		public BeardRestylingDeed( Serial serial ) : base( serial )
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
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			else
				from.SendGump( new InternalGump( from, this ) );
		}

		private class InternalGump : Gump
		{
			private Mobile m_From;
			private BeardRestylingDeed m_Deed;

			public InternalGump( Mobile from, BeardRestylingDeed deed ) : base( 200, 50 )
			{
				m_From = from;
				m_Deed = deed;

				from.CloseGump( typeof( InternalGump ) );

				AddBackground( 100, 10, 430, 385, 0xA28 );

                AddLabel(150, 25, 255, "FACIAL HAIR SELECTION MENU");
				AddButton( 175, 340, 0xFA5, 0xFA7, 0x0, GumpButtonType.Reply, 0 ); // CANCEL

				AddHtmlLocalized( 210, 342, 90, 35, 1011012, false, false );// <CENTER>BEARDSTYLE SELECTION MENU</center>

				AddBackground( 230, 60, 50, 50, 0xA3C );
                AddBackground( 230, 115, 50, 50, 0xA3C);
                AddBackground( 230, 170, 50, 50, 0xA3C);
                AddBackground( 230, 225, 50, 50, 0xA3C);
				AddBackground( 445, 60, 50, 50, 0xA3C );
				AddBackground( 445, 115, 50, 50, 0xA3C );
				AddBackground( 445, 170, 50, 50, 0xA3C );
				AddBackground( 445, 225, 50, 50, 0xA3C );

                AddLabel(150, 75, 255, "Long Beard");
                AddLabel(150, 130, 255, "Short Beard");
                AddLabel(150, 185, 255, "Goatee");
                AddLabel(150, 240, 255, "Mustache");
                AddLabel(355, 75, 255, "Medium Beard");
                AddLabel(355, 130, 255, "Full Beard");
                AddLabel(355, 185, 255, "Vandyke");
                AddLabel(355, 240, 255, "Remove Beard");

				AddImage( 163, 15, 0xC671 );
				AddImage( 163, 70, 0xC672 );
				AddImage( 163, 120, 0xC670 );
				AddImage( 163, 180, 0xC673 );
				AddImage( 378, 18, 0xC6D8 );
				AddImage( 378, 65, 0xC6D9 );
				AddImage( 378, 120, 0xC674 );

				AddButton( 118, 73, 0xFA5, 0xFA7, 2, GumpButtonType.Reply, 0 );
				AddButton( 118, 128, 0xFA5, 0xFA7, 3, GumpButtonType.Reply, 0 );
				AddButton( 118, 183, 0xFA5, 0xFA7, 4, GumpButtonType.Reply, 0 );
				AddButton( 118, 238, 0xFA5, 0xFA7, 5, GumpButtonType.Reply, 0 );
				AddButton( 323, 73, 0xFA5, 0xFA7, 7, GumpButtonType.Reply, 0 );
				AddButton( 323, 128, 0xFA5, 0xFA7, 6, GumpButtonType.Reply, 0 );
				AddButton( 323, 183, 0xFA5, 0xFA7, 8, GumpButtonType.Reply, 0 );
				AddButton( 323, 238, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0 );
			}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				if ( m_Deed.Deleted )
					return;

				if ( info.ButtonID > 0 )
				{
					int itemID = 0;

				switch ( info.ButtonID )
				{

                    case 2: itemID = 0x203E; break; //LongBeard
                    case 3: itemID = 0x203F; break; //ShortBeard
                    case 4: itemID = 0x2040; break; //Goatee
                    case 5: itemID = 0x2041; break; //Mustache
                    case 6: itemID = 0x204C; break; //MediumBeard
                    case 7: itemID = 0x204B; break; //Full Beard
                    case 8: itemID = 0x204D; break; //Vandyke     
				}

				if ( m_From is PlayerMobile )
				{
					PlayerMobile pm = (PlayerMobile)m_From;

					//pm.SetBeardMods( -1, -1 ); // clear any hairmods (disguise kit, incognito)
				}

					m_From.FacialHairItemID = itemID;

					m_Deed.Delete();
				}
			}
		}
	}
}