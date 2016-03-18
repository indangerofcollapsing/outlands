using System;

namespace Server.Items
{    
    public class PirateBandana : BaseHat
    {
        public override int PlayerClassCurrencyValue { get { return 2500; } }

        [Constructable]
		public PirateBandana() : this( 0 )
		{
		}

		[Constructable]
        public PirateBandana(int hue): base(0x1540, hue)
		{
			Weight = 1.0;
            Name = "Pirate Bandana";

            PlayerClass = PlayerClass.Pirate;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.PirateItemHue;

            LootType = Server.LootType.Blessed;
		}

        public PirateBandana(Serial serial): base(serial)
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

    public class ArmoredPirateBandana : BaseArmoredHat
    {
		[Constructable]
        public ArmoredPirateBandana()
            : base(0x1540)
		{
			Weight = 1.0;
            Name = "Pirate Bandana";

            PlayerClass = PlayerClass.Pirate;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.PirateItemHue;

            LootType = Server.LootType.Blessed;
		}

        public ArmoredPirateBandana(Serial serial)
            : base(serial)
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
		}
    }
}