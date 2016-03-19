using System;
using System.Collections.Generic;
using Server;
using Server.Multis;
using Server.Targeting;
using Server.ContextMenus;
using Server.Gumps;

namespace Server.Items
{
    public class UOACZPinkDyeTub : UOACZDyeTub
	{
        [Constructable]
        public UOACZPinkDyeTub(): base()
        {
            Name = "a pink dye tub";           

            DyedHue = 2622;
            Redyable = false;
        }

        public UOACZPinkDyeTub(Serial serial): base(serial)
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