using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class FlowstonesPack : Bag
    {
        [Constructable]
        public FlowstonesPack()
        {
            Name = "Flowstones Pack";
	
            PackItems();
        }

        public void PackItems()
        {
            DropItem(new Item(13446)); //Small Flowstone
            DropItem(new Item(13484)); //Small Flowstone
            DropItem(new Item(13451)); //Medium Flowstone
            DropItem(new Item(13488)); //Medium Flowstone
            DropItem(new Item(13345)); //Large Flowstone
        }

        public FlowstonesPack(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}
