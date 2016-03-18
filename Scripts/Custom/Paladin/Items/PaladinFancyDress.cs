using System;

namespace Server.Items
{
    [FlipableAttribute( 0x1eff, 0x1f00 )]
    public class PaladinFancyDress : BaseOuterTorso
    {
        public override int PlayerClassCurrencyValue { get { return 2500; } }

        [Constructable]
		public PaladinFancyDress() : this( 0 )
		{
		}

		[Constructable]
		public PaladinFancyDress( int hue ) : base( 0x1EFF, hue )
		{
			Weight = 3.0;
            Name = "Paladin Fancy Dress";

            PlayerClass = PlayerClass.Paladin;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.PaladinItemHue;

            LootType = Server.LootType.Blessed;            
		}

        public PaladinFancyDress(Serial serial): base(serial)
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