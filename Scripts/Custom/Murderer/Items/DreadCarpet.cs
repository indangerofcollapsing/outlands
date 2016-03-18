using System;

using Server.Network;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
    public class DreadCarpet : Item
	{
        public int HueColor = 2117;

        public override int PlayerClassCurrencyValue { get { return 200; } }

        [Constructable]
        public DreadCarpet(): base(0x3F0A)
        {
            Name = "dread carpet";

            PlayerClass = PlayerClass.Murderer;
            PlayerClassRestricted = false;

            Hue = HueColor;
        }

        public DreadCarpet(Serial serial): base(serial)
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