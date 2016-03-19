using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class ChefsPack : Bag
    {
        [Constructable]
        public ChefsPack()
        {
            Name = "Chef's Pack";
	
            PackItems();
        }

        public void PackItems()
        {
            DropItem(new Item(2541)); //Large Kettle
            DropItem(new Item(2547)); //Pan
            DropItem(new Item(2516)); //Silverware
            DropItem(new Item(2517)); //Silverware            
            DropItem(new Item(2590)); //Bowl of Flour
            DropItem(new Item(2450)); //Tray
            DropItem(new Item(2476)); //Bushel
            DropItem(new Item(15793)); //Cauldron of Stew
        }

        public ChefsPack(Serial serial): base(serial)
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
