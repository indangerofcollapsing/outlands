using System;

namespace Server.Items
{
    [Flipable( 0x1516, 0x1531 )]
    public class DreadSkirt : BaseOuterLegs
    {
        public override int PlayerClassCurrencyValue { get { return 2500; } }

        [Constructable]
		public DreadSkirt() : this( 0 )
		{
		}

		[Constructable]
		public DreadSkirt( int hue ) : base( 0x1516, hue )
		{
			Weight = 4.0;
            Name = "Dread Skirt";

            PlayerClass = PlayerClass.Murderer;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.MurdererItemHue;

            LootType = Server.LootType.Blessed;            
		}

        public DreadSkirt(Serial serial): base(serial)
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