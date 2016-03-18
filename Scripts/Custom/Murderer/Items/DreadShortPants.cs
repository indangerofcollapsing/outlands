using System;

namespace Server.Items
{
    [FlipableAttribute(0x152e, 0x152f)]
    public class DreadShortPants : BasePants
    {
        public override int PlayerClassCurrencyValue { get { return 2500; } }

        [Constructable]
		public DreadShortPants() : this( 0 )
		{
		}

		[Constructable]
        public DreadShortPants(int hue): base(0x152E, hue)
		{
			Weight = 2.0;
            Name = "Dread Short Pants";

            PlayerClass = PlayerClass.Murderer;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.MurdererItemHue;

            LootType = Server.LootType.Blessed;            
		}

        public DreadShortPants(Serial serial): base(serial)
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