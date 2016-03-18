using System;

using Server.Network;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
    public class PaladinCarpet : Item
	{
        public int HueColor = PlayerClassPersistance.PaladinItemHue;

        public override int PlayerClassCurrencyValue { get { return 200; } }

        [Constructable]
        public PaladinCarpet(): base(0x3F0A)
        {
            Name = "paladin carpet";

            PlayerClass = PlayerClass.Paladin;
            PlayerClassRestricted = false;

            Hue = HueColor;
        }

        public PaladinCarpet(Serial serial): base(serial)
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