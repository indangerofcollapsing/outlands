using System;

namespace Server.Items
{
    [Flipable(0x1fa1, 0x1fa2)]
    public class PaladinTunic : BaseMiddleTorso
    {
        public override int PlayerClassCurrencyValue { get { return 2500; } }

        [Constructable]
		public PaladinTunic() : this( 0 )
		{
		}

		[Constructable]
        public PaladinTunic(int hue): base(0x1FA1, hue)
		{
            Weight = 5.0;
            Name = "Paladin Tunic";

            PlayerClass = PlayerClass.Paladin;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.PaladinItemHue;

            LootType = Server.LootType.Blessed;            
		}

        public PaladinTunic(Serial serial): base(serial)
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