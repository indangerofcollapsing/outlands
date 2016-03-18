using System;
using Server.Targeting;
using Server.Gumps;

namespace Server.Items
{
	public class PaladinRunebookDyeTub : RunebookDyeTub
	{
        public override bool Redyable { get { return false; } }
        public override int PermanentColor { get { return PlayerClassPersistance.PaladinItemHue; } }

        public override int PlayerClassCurrencyValue { get { return 1000; } }

        [Constructable]
        public PaladinRunebookDyeTub(): base()
        {
            Name = "paladin runebook dye tub";

            if (PermanentColor != -1)
                DyeColor = PermanentColor;

            PlayerClass = PlayerClass.Paladin;
            PlayerClassRestricted = false;

            Weight = 10;
        }

        public PaladinRunebookDyeTub(Serial serial): base(serial)
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

            PlayerClass = PlayerClass.Paladin;
            PlayerClassRestricted = false;
		}
	}
}