using System;

namespace Server.Items
{
    [Flipable(0x1541, 0x1542)]
    public class PaladinSash : BodySash
    {
        public override int PlayerClassCurrencyValue { get { return 2500; } }

        [Constructable]
		public PaladinSash() : this( 0 )
		{
		}

		[Constructable]
		public PaladinSash( int hue ) : base( hue )
		{
			Weight = 1.0;
            Name = "Paladin Sash";

            PlayerClass = PlayerClass.Paladin;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.PaladinItemHue;

            LootType = Server.LootType.Blessed;
		}

        public PaladinSash(Serial serial): base(serial)
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