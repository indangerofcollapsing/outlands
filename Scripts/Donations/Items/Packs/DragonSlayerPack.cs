using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class DragonSlayerPack : Bag
    {
        [Constructable]
        public DragonSlayerPack()
        {
            Name = "Dragon Slayer's Pack";
	
            PackItems();
        }

        public void PackItems()
        {
            DropItem(new Item(8756)); 
            DropItem(new Item(5488));
            DropItem(new Item(5504)); 
            DropItem(new Item(7111));
            DropItem(new Item(8758)); 
            DropItem(new Item(5095)); 
            DropItem(new Item(5687));
            DropItem(new WallBannerDeed());
        }

        public DragonSlayerPack(Serial serial)
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
