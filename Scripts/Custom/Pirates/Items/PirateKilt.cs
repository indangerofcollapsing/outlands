using System;

namespace Server.Items
{
    [Flipable(0x1537, 0x1538)]
    public class PirateKilt : BaseOuterLegs
    {
        public override int PlayerClassCurrencyValue { get { return 2500; } }

        [Constructable]
		public PirateKilt() : this( 0 )
		{
		}

		[Constructable]
        public PirateKilt(int hue): base(0x1537, hue)
		{
			Weight = 2.0;
            Name = "Pirate Kilt";

            PlayerClass = PlayerClass.Pirate;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.PirateItemHue;

            LootType = Server.LootType.Blessed;
		}

        public PirateKilt(Serial serial): base(serial)
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