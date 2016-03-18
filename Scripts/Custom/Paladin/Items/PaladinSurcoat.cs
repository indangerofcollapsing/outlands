using System;

namespace Server.Items
{
    [Flipable(0x1ffd, 0x1ffe)]
    public class PaladinSurcoat : BaseMiddleTorso
    {
        public override int PlayerClassCurrencyValue { get { return 2500; } }

        [Constructable]
		public PaladinSurcoat() : this( 0 )
		{
		}

		[Constructable]
        public PaladinSurcoat(int hue): base(0x1FFD, hue)
		{
            Weight = 6.0;
            Name = "Paladin Surcoat";

            PlayerClass = PlayerClass.Paladin;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.PaladinItemHue;

            LootType = Server.LootType.Blessed;            
		}

        public PaladinSurcoat(Serial serial): base(serial)
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