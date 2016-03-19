using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class PirateDecoBag : Bag
    {
        [Constructable]
        public PirateDecoBag()
        {
            Name = "Pirate's Bag";
            PackItems();
        }

        public void PackItems()
        {
            DropItem(new Item(5367)); //anchor
            DropItem(new Item(3542)); // small fish
            DropItem(new Item(4099)); //spittoon
            DropItem(new Item(4003)); //cards
            DropItem(new Item(4104)); //wash basin
            DropItem(new Item(0x1ea5)); //net
            DropItem(new Item(0x4224)); //cannonball
        }

        public PirateDecoBag(Serial serial)
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
