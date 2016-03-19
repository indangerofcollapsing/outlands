using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class HalloweenPack : Bag
    {
        [Constructable]
        public HalloweenPack()
        {
            Name = "Halloween Pack";
            PackItems();
        }

        public void PackItems()
        {

            DropItem(new Item(0x4691)); 
            DropItem(new Item(0x4694)); 
            DropItem(new Item(0x468C)); 
            DropItem(new Item(0x468D)); 
            DropItem(new Item(0x1E34)); 
            DropItem(new Item(0x0EE3));
            DropItem(new Item(0x1E34)); 
            DropItem(new Item(0x0EE3)); 
            DropItem(new Item(0x0EE0));
            DropItem(new Item(0x0EE8)); 
            DropItem(new Item(0x0ED7));  




        }

        public HalloweenPack(Serial serial)
            : base(serial)
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
