using System;
using Server;

namespace Server.Items
{
	public class DonationScroll : BaseLight
	{
		public override int LitItemID { get { return 0x0E34; } }
		
		[Constructable]
		public DonationScroll() : base( 0x0E34 )
		{
			Movable = true;
			Duration = TimeSpan.Zero; // Never burnt out
			Burning = true;
			Light = LightType.Circle225;
			Weight = 1.0;
		}

		public DonationScroll( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}