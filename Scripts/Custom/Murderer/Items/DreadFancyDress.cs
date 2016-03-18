using System;

namespace Server.Items
{
    [FlipableAttribute( 0x1eff, 0x1f00 )]
    public class DreadFancyDress : BaseOuterTorso
    {
        public override int PlayerClassCurrencyValue { get { return 2500; } }

        [Constructable]
		public DreadFancyDress() : this( 0 )
		{
		}

		[Constructable]
		public DreadFancyDress( int hue ) : base( 0x1EFF, hue )
		{
			Weight = 3.0;
            Name = "Dread Fancy Dress";

            PlayerClass = PlayerClass.Murderer;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.MurdererItemHue;

            LootType = Server.LootType.Blessed;           
		}

        public DreadFancyDress(Serial serial): base(serial)
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