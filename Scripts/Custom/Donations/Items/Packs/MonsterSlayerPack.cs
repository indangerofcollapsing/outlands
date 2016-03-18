using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class MonsterSlayerPack : Bag
    {
        [Constructable]
        public MonsterSlayerPack()
        {
            Name = "Monster Slayer's Pack";
	
            PackItems();
        }

        public void PackItems()
        {
            DropItem(new Item(7779)); //Mounted Troll
            DropItem(new Item(7780)); //Mounted Orc
            DropItem(new Item(7782)); //Mounted Ogre
            DropItem(new Item(12633)); //Mounted Dreadhorn
            DropItem(new Item(5472)); //Decorative Bardiche
        }

        public MonsterSlayerPack(Serial serial): base(serial)
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
