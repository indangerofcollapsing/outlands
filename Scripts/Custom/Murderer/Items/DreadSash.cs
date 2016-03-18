using System;

namespace Server.Items
{
    [Flipable(0x1541, 0x1542)]
    public class DreadSash : BodySash
    {
        public override int PlayerClassCurrencyValue { get { return 2500; } }

        [Constructable]
		public DreadSash() : this( 0 )
		{
		}

		[Constructable]
		public DreadSash( int hue ) : base( hue )
		{
			Weight = 1.0;
            Name = "Dread Sash";

            PlayerClass = PlayerClass.Murderer;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.MurdererItemHue;

            LootType = Server.LootType.Blessed;
		}

        public DreadSash(Serial serial): base(serial)
		{
		}

        public override bool Dye(Mobile from, DyeTub sender)
        {
            return false;
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
		}
    }
}