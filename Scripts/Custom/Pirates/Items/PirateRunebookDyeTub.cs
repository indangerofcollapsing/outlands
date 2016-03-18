using System;
using Server.Targeting;
using Server.Gumps;

namespace Server.Items
{
	public class PirateRunebookDyeTub : RunebookDyeTub
	{
        public override bool Redyable { get { return false; } }
        public override int PermanentColor { get { return PlayerClassPersistance.PirateItemHue; } }

        public override int PlayerClassCurrencyValue { get { return 1000; } }

        [Constructable]
        public PirateRunebookDyeTub(): base()
        {
            Name = "pirate runebook dye tub";

            if (PermanentColor != -1)
                DyeColor = PermanentColor;

            PlayerClass = PlayerClass.Pirate;
            PlayerClassRestricted = false;

            Weight = 10;
        }

        public PirateRunebookDyeTub(Serial serial): base(serial)
        {
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
            writer.Write((int)0); //version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            PlayerClass = PlayerClass.Pirate;
            PlayerClassRestricted = false;
		}
	}
}