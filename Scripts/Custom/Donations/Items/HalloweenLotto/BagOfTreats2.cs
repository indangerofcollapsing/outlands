using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class BagOfTreats2 : Bag
    {
        [Constructable]
        public BagOfTreats2()
        {
            Name = "Bag of Treats";
            PackItems();
        }

        public void PackItems()
        {

            DropItem(new Item(0x469E)); 
            DropItem(new Item(0x469E)); 
            DropItem(new Item(0x469D)); 
            DropItem(new Item(0x469D)); 
            DropItem(new Item(0x468C)); 
            DropItem(new Item(0x468C));




        }

        public BagOfTreats2(Serial serial)
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
