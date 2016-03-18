using System;

namespace Server.Items
{
    [DynamicFliping]
    [Flipable(0x280F, 0x2810)]
    public class PaladinDresser : LockableContainer
    {
        public override int PlayerClassCurrencyValue { get { return 5000; } }

		[Constructable]
        public PaladinDresser(): base(0x280F)
		{			
            Name = "Paladin Dresser";

            PlayerClass = PlayerClass.Paladin;
            PlayerClassRestricted = false;

            Hue = 2120;
            GumpID = 0x10A;
		}

        public PaladinDresser(Serial serial): base(serial)
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

            PlayerClass = PlayerClass.Paladin;
            PlayerClassRestricted = false;
		}
    }
}