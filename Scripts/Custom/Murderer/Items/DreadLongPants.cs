using System;

namespace Server.Items
{
    [FlipableAttribute(0x1539, 0x153a)]
    public class DreadLongPants : BasePants
    {
        public override int PlayerClassCurrencyValue { get { return 2500; } }

        [Constructable]
		public DreadLongPants() : this( 0 )
		{
		}

		[Constructable]
        public DreadLongPants(int hue): base(0x1539, hue)
		{
			Weight = 2.0;
            Name = "Dread Long Pants";

            PlayerClass = PlayerClass.Murderer;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.MurdererItemHue;

            LootType = Server.LootType.Blessed;        
		}

        public DreadLongPants(Serial serial): base(serial)
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