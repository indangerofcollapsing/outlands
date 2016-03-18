using System;

namespace Server.Items
{
    [FlipableAttribute(0x1517, 0x1518)]
    public class PirateShirt : BaseShirt
    {
        public override int PlayerClassCurrencyValue { get { return 2500; } }

        [Constructable]
		public PirateShirt() : this( 0 )
		{
		}

		[Constructable]
        public PirateShirt(int hue): base(0x1517, hue)
		{
			Weight = 1.0;
            Name = "Pirate Shirt";

            PlayerClass = PlayerClass.Pirate;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.PirateItemHue;

            LootType = Server.LootType.Blessed;            
		}

        public PirateShirt(Serial serial): base(serial)
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