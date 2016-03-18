using System;

namespace Server.Items
{
    [DynamicFliping]
    [Flipable(0x2811, 0x2812)]
    public class PirateFootlocker : LockableContainer
    {
        public override int PlayerClassCurrencyValue { get { return 5000; } }

		[Constructable]
        public PirateFootlocker(): base(0x2811)
		{			
            Name = "Pirate Footlocker";

            PlayerClass = PlayerClass.Pirate;
            PlayerClassRestricted = false;

            GumpID = 0x10B;
		}

        public PirateFootlocker(Serial serial): base(serial)
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

            PlayerClass = PlayerClass.Pirate;
            PlayerClassRestricted = false;
		}
    }
}