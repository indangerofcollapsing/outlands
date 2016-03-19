using System;
using Server;
using Server.Items;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;

namespace Server.Multis
{
	public class LargeFishingBoat : LargeBoat
	{
        public override List<Point3D> m_CannonLocations()
        {
            List<Point3D> list = new List<Point3D>();           

            return list;
        }

		[Constructable]
        public LargeFishingBoat()
		{
		}

        public LargeFishingBoat(Serial serial): base(serial)
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