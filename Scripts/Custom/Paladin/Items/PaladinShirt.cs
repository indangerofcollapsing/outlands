using System;

namespace Server.Items
{
    [FlipableAttribute(0x1517, 0x1518)]
    public class PaladinShirt : BaseShirt
    {
        public override int PlayerClassCurrencyValue { get { return 2500; } }

        [Constructable]
		public PaladinShirt() : this( 0 )
		{
		}

		[Constructable]
        public PaladinShirt(int hue): base(0x1517, hue)
		{
			Weight = 1.0;
            Name = "Paladin Shirt";

            PlayerClass = PlayerClass.Paladin;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.PaladinItemHue;

            LootType = Server.LootType.Blessed;            
		}

        public PaladinShirt(Serial serial): base(serial)
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