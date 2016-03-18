using System;

namespace Server.Items
{
    [FlipableAttribute( 0x1eff, 0x1f00 )]
    public class PirateFancyDress : BaseOuterTorso
    {
        public override int PlayerClassCurrencyValue { get { return 2500; } }

        [Constructable]
		public PirateFancyDress() : this( 0 )
		{
		}

		[Constructable]
		public PirateFancyDress( int hue ) : base( 0x1EFF, hue )
		{
			Weight = 3.0;
            Name = "Pirate Fancy Dress";

            PlayerClass = PlayerClass.Pirate;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.PirateItemHue;

            LootType = Server.LootType.Blessed;            
		}

        public PirateFancyDress(Serial serial): base(serial)
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