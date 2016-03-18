using System;

namespace Server.Items
{
    [FlipableAttribute(0x1539, 0x153a)]
    public class PaladinLongPants : BasePants
    {
        public override int PlayerClassCurrencyValue { get { return 2500; } }

        [Constructable]
		public PaladinLongPants() : this( 0 )
		{
		}

		[Constructable]
        public PaladinLongPants(int hue): base(0x1539, hue)
		{
			Weight = 2.0;
            Name = "Paladin Long Pants";

            PlayerClass = PlayerClass.Paladin;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.PaladinItemHue;

            LootType = Server.LootType.Blessed;             
		}

        public PaladinLongPants(Serial serial): base(serial)
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