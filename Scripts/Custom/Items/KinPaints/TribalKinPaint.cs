using System;
using Server;
using Server.Mobiles;
using Server.Spells;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
    public class TribalKinPaint : KinPaint
	{
        [Constructable]
		public TribalKinPaint() : base()
		{
            Name = "tribal kin paint";
            Hue = 1619;
			Weight = 1.0;

            PaintHue = 1619;
		}

        public TribalKinPaint(Serial serial): base(serial)
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