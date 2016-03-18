using System;
using Server;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
    public class UOACZTub : Item
    {
		[Constructable]
        public UOACZTub(): base(3715)
		{
            Name = "empty water tub";
            Hue = 0;
		}

        public UOACZTub(Serial serial): base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            from.SendMessage("Double click on fresh ground water to fill this item.");
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