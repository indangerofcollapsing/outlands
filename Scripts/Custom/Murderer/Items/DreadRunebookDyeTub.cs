using System;
using Server.Targeting;
using Server.Gumps;

namespace Server.Items
{
	public class DreadRunebookDyeTub : RunebookDyeTub
	{
        public override bool Redyable { get { return false; } }
        public override int PermanentColor { get { return PlayerClassPersistance.MurdererCurrencyItemHue; } }

        public override int PlayerClassCurrencyValue { get { return 1000; } }

        [Constructable]
        public DreadRunebookDyeTub(): base()
        {
            Name = "dread runebook dye tub";

            if (PermanentColor != -1)
                DyeColor = PermanentColor;

            PlayerClass = PlayerClass.Murderer;
            PlayerClassRestricted = false;

            Weight = 10;
        }

        public DreadRunebookDyeTub(Serial serial): base(serial)
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

            PlayerClass = PlayerClass.Murderer;
            PlayerClassRestricted = false;
		}
	}
}