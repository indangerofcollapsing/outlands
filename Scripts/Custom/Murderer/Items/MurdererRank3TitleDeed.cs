using System;
using System.Collections.Generic;
using Server;
using Server.Multis;
using Server.Targeting;
using Server.ContextMenus;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Items
{
	public class MurdererRank3TitleDeed : PlayerClassTitleDeed
	{
        public override int PlayerClassCurrencyValue { get { return 2500; } }

        public override string Title { get { return PlayerClassPersistance.MurdererTitles[2]; } }  

		[Constructable]
        public MurdererRank3TitleDeed(): base()
		{           
			Weight = 1.0;

            PlayerClass = PlayerClass.Murderer;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.MurdererItemHue;
		}

        public MurdererRank3TitleDeed(Serial serial) : base(serial)
		{
		}

        public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write((int)0); //Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}