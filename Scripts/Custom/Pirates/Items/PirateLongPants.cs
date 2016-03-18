using System;

namespace Server.Items
{
    [FlipableAttribute(0x1539, 0x153a)]
    public class PirateLongPants : BasePants
    {
        public override int PlayerClassCurrencyValue { get { return 2500; } }

        [Constructable]
		public PirateLongPants() : this( 0 )
		{
		}

		[Constructable]
        public PirateLongPants(int hue): base(0x1539, hue)
		{
			Weight = 2.0;
            Name = "Pirate Long Pants";

            PlayerClass = PlayerClass.Pirate;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.PirateItemHue;

            LootType = Server.LootType.Blessed;            
		}

        public PirateLongPants(Serial serial): base(serial)
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