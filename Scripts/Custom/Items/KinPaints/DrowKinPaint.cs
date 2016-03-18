using System;
using Server;
using Server.Mobiles;
using Server.Spells;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
    public class DrowKinPaint : KinPaint
	{
		[Constructable]
		public DrowKinPaint() : base()
		{
            Name = "drow kin paint";
            Hue = 1107;
			Weight = 1.0;

            PaintHue = 1107;
		}

        public DrowKinPaint(Serial serial): base(serial)
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