using System;

namespace Server.Items
{
    [Flipable(0x1fa1, 0x1fa2)]
    public class PirateTunic : BaseMiddleTorso
    {
        public override int PlayerClassCurrencyValue { get { return 2500; } }

        [Constructable]
		public PirateTunic() : this( 0 )
		{
		}

		[Constructable]
        public PirateTunic(int hue): base(0x1FA1, hue)
		{
            Weight = 5.0;
            Name = "Pirate Tunic";

            PlayerClass = PlayerClass.Pirate;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.PirateItemHue;

            LootType = Server.LootType.Blessed;            
		}

        public PirateTunic(Serial serial): base(serial)
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