using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class BagOfTreats1 : Bag
    {
        [Constructable]
        public BagOfTreats1()
        {
            Name = "Bag of Treats";
            PackItems();
        }

        public void PackItems()
        {

            DropItem(new Item(0x468C)); 
            DropItem(new Item(0x468D)); 
            DropItem(new Item(0x468E)); 
            DropItem(new Item(0x468C)); 
            DropItem(new Item(0x468D)); 
            DropItem(new Item(0x468E)); 





        }

        public BagOfTreats1(Serial serial)
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
