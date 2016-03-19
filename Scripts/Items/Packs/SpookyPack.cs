using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class SpookyPack : Bag
    {
        [Constructable]
        public SpookyPack()
        {
            Name = "Spooky Pack";
	
            PackItems();
        }

        public void PackItems()
        {
            DropItem(new Item(3807)); //Grave Dirt
            DropItem(new Item(3808)); //Grave Dirt
            DropItem(new Item(3809)); //Grave Dirt
            DropItem(new Item(3810)); //Grave Dirt          
            DropItem(new Item(3811)); //Spiderwebs
            DropItem(new Item(3812)); //Spiderwebs
            DropItem(new Item(3813)); //Spiderwebs
            DropItem(new Item(3814)); //Spiderwebs
            DropItem(new Item(4453)); //Gravestone
            DropItem(new Item(4454)); //Gravestone
            DropItem(new Item(7732)); //Scarecrow
        }

        public SpookyPack(Serial serial): base(serial)
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
