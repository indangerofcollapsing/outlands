using System;

namespace Server.Items
{    
    public class PirateTricorneHat : BaseHat
    {
        public override int PlayerClassCurrencyValue { get { return 2500; } }

        [Constructable]
		public PirateTricorneHat() : this( 0 )
		{
		}

		[Constructable]
        public PirateTricorneHat(int hue): base(0x171B, hue)
		{
			Weight = 1.0;
            Name = "Pirate Tricorne Hat";

            PlayerClass = PlayerClass.Pirate;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.PirateItemHue;

            LootType = Server.LootType.Blessed;
		}

        public PirateTricorneHat(Serial serial): base(serial)
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

    public class ArmoredPirateTricorneHat : BaseArmoredHat
    {
        [Constructable]
        public ArmoredPirateTricorneHat()
            : base(0x171B)
        {
            Weight = 1.0;
            Name = "Pirate Tricorne Hat";

            PlayerClass = PlayerClass.Pirate;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.PirateItemHue;

            LootType = Server.LootType.Blessed;
        }

        public ArmoredPirateTricorneHat(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}