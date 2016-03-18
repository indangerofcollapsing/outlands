using System;

using Server.Network;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
    public class PirateCarpet : Item
	{
        public int HueColor = 1102;

        public override int PlayerClassCurrencyValue { get { return 500; } }

        [Constructable]
        public PirateCarpet(): base(0x3F0A)
        {
            Name = "pirate carpet";

            PlayerClass = PlayerClass.Pirate;
            PlayerClassRestricted = false;

            Hue = HueColor;
        }

        public PirateCarpet(Serial serial): base(serial)
		{
		}

        public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( 0 ); // version
		}

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();  
          
            //-----

            Hue = HueColor;
        }
    }
}