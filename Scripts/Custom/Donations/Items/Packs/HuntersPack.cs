using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class HuntersPack : Bag
    {
        [Constructable]
        public HuntersPack()
        {
            Name = "Hunter's Pack";
	
            PackItems();
        }

        public void PackItems()
        {   
            DropItem(new Item(7776)); //Mounted Bear
            DropItem(new Item(7777)); //Mounted Deer
            DropItem(new Item(7778)); //Mounted Fish
            DropItem(new Item(7781)); //Mounted Polar Bear

            DropItem(new Item(11671) { Name = "a hunting companion: weasel", Hue = 2514 });

            //DropItem(new Item(11670) { Name = "a hunting companion: wolf", Hue = 2401 });            
            //DropItem(new Item(11672) { Name = "a hunting companion: badger", Hue = 2503 });
        }

        public HuntersPack(Serial serial): base(serial)
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
