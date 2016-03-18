using System;
using Server;
using Server.Mobiles;
using Server.Spells;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
    public class BrownOrcKinPaint : KinPaint
	{
		[Constructable]
		public BrownOrcKinPaint() : base()
		{
            Name = "brown orcish kin paint";
            Hue = 2417;
			Weight = 1.0;

            PaintHue = 2416; 
		}

        public BrownOrcKinPaint(Serial serial): base(serial)
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