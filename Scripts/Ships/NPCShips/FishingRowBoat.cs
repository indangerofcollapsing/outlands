using System;
using Server;
using Server.Items;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;

namespace Server.Multis
{
	public class FishingRowBoat : RowBoat
	{
        public override void ConfigureShip()
        {
            base.ConfigureShip();

            PPlank.Visible = false;
            SPlank.Visible = false; 
        }

		[Constructable]
		public FishingRowBoat()
		{
		}

        public FishingRowBoat(Serial serial): base(serial)
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );
		}
	}
}