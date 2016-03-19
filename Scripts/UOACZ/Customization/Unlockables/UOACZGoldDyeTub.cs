using System;
using System.Collections.Generic;
using Server;
using Server.Multis;
using Server.Targeting;
using Server.ContextMenus;
using Server.Gumps;

namespace Server.Items
{
    public class UOACZGoldDyeTub : UOACZDyeTub
	{
        [Constructable]
        public UOACZGoldDyeTub(): base()
        {
            Name = "a gold dye tub";           

            DyedHue = 2125;
            Redyable = false;
        }

        public UOACZGoldDyeTub(Serial serial): base(serial)
        {
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
            writer.Write((int)0); // version          
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();           
		}	
	}
}