using System;
using System.Collections.Generic;
using Server;
using Server.Multis;
using Server.Targeting;
using Server.ContextMenus;
using Server.Gumps;

namespace Server.Items
{
    public class UOACZFallonUndeadDye : UOACZUndeadDye
	{
        [Constructable]
        public UOACZFallonUndeadDye(): base()
        {
            Name = "a fallon undead dye";           

            DyedHue = 1266;
        }

        public UOACZFallonUndeadDye(Serial serial): base(serial)
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