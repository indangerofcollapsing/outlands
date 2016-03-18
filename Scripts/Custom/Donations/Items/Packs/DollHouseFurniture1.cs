using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class DollHouseFurniture1 : Bag
    {
        [Constructable]
        public DollHouseFurniture1()
        {
            Name = "DollHouse Furniture Set";
            PackItems();
        }

        public void PackItems()
        {
            DropItem(new Item(0x3F1F)); // Dresser
            DropItem(new Item(0x3F1E)); // Bed
            DropItem(new Item(0x3F0A)); // Blue Carpet
            DropItem(new Item(0x3F26)); // Covered Chair
            DropItem(new Item(0x3F0C)); // Pink Tree

        }

        public DollHouseFurniture1(Serial serial)
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
