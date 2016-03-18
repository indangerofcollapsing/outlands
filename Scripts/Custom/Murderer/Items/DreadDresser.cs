using System;

namespace Server.Items
{
    [DynamicFliping]
    [Flipable(0x280D, 0x280E)]
    public class DreadDresser : LockableContainer
    {
        public override int PlayerClassCurrencyValue { get { return 5000; } }

		[Constructable]
        public DreadDresser(): base(0x280D)
		{			
            Name = "Dread Dresser";

            PlayerClass = PlayerClass.Murderer;
            PlayerClassRestricted = false;

            Hue = 2117;
            GumpID = 0x109;
		}

        public DreadDresser(Serial serial): base(serial)
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

            PlayerClass = PlayerClass.Murderer;
            PlayerClassRestricted = false;
		}
    }
}