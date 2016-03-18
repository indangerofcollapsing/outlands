using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class VineyardPack : Bag
    {
        [Constructable]
        public VineyardPack()
        {
            Name = "Vineyard Pack";
            PackItems();
        }

        public void PackItems()
        {

            DropItem(new Item(0x0D1A)); 
            DropItem(new Item(0x0D1B)); 
            DropItem(new Item(0x0D1C)); 
            DropItem(new Item(0x0D1D)); 
            DropItem(new Item(0x0D1E)); 
            DropItem(new Item(0x0D1F)); 
        }

        public VineyardPack(Serial serial): base(serial)
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
