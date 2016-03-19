using System;
using Server;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
    public class UOACZGlass : Item
    {
		[Constructable]
        public UOACZGlass(): base(8065)
		{
            Name = "glass";
            Hue = 2500;
		}

        public UOACZGlass(Serial serial): base(serial)
        {
        }
                
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}