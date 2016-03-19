using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class AnimalHandlerPack : Bag
    {
        [Constructable]
        public AnimalHandlerPack()
        {
            Name = "Animal Handler's Pack";
	
            PackItems();
        }

        public void PackItems()
        {
            DropItem(new Item(4022)); //Horseshoes
            DropItem(new Item(3892)); //Hay
            DropItem(new Item(3892)); //Hay
            DropItem(new Item(3894)); //Hay Bundle
            DropItem(new Item(3894)); //Hay Bundle
            DropItem(new Item(3895)); //Old Saddle
            DropItem(new Item(3896)); //New Saddle
            DropItem(new Item(3900)); //Small Dung
        }

        public AnimalHandlerPack(Serial serial): base(serial)
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
