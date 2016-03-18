using System;

namespace Server.Items
{
    [Flipable(0x1f7b, 0x1f7c)]
    public class DreadDoublet : BaseMiddleTorso
    {
        public override int PlayerClassCurrencyValue { get { return 2500; } }

        [Constructable]
		public DreadDoublet() : this( 0 )
		{
		}

		[Constructable]
        public DreadDoublet(int hue) : base(0x1F7B, hue)
		{
			Weight = 3.0;
            Name = "Dread Doublet";

            PlayerClass = PlayerClass.Murderer;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.MurdererItemHue;

            LootType = Server.LootType.Blessed;          
		}

        public DreadDoublet(Serial serial)
            : base(serial)
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