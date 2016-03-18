using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class BlacksmithDecoBag : Bag
    {
        [Constructable]
        public BlacksmithDecoBag()
        {
            Name = "Grandmaster Smiths's Decoration Bag";
            PackItems();
        }

        public void PackItems()
        {
            DropItem(new Item(5404)); // Decorate Armor Statue
            DropItem(new Item(5472)); // Decorative Weapons
            DropItem(new Item(5480)); // Decorative Weapons
            DropItem(new Item(5095)); // Hanging Ringmail Chest
            DropItem(new Item(5097)); // Hanging Ringmail Arms
            DropItem(new Item(7155)); // Ingots
            DropItem(new Item(6586)); // Iron Ore
            DropItem(new LargeForgeEastDeed());
            DropItem(new ForgedMetal());


        }

        public BlacksmithDecoBag(Serial serial)
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
