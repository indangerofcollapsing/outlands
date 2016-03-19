using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class RuinedHomePack : Bag
    {
        [Constructable]
        public RuinedHomePack()
        {
            Name = "Ruined Home Pack";
	
            PackItems();
        }

        public void PackItems()
        {            
            DropItem(new Item(6913)); //Floor Cracks
            DropItem(new Item(6914)); //Floor Cracks
            DropItem(new Item(6915)); //Floor Cracks
            DropItem(new Item(6916)); //Floor Cracks
            DropItem(new Item(6917)); //Floor Cracks
            DropItem(new Item(6918)); //Floor Cracks
            DropItem(new Item(6919)); //Floor Cracks
            DropItem(new Item(6920)); //Floor Cracks
            DropItem(new Item(6913)); //Floor Cracks
            DropItem(new Item(6914)); //Floor Cracks
            DropItem(new Item(6915)); //Floor Cracks
            DropItem(new Item(6916)); //Floor Cracks
            DropItem(new Item(6917)); //Floor Cracks
            DropItem(new Item(6918)); //Floor Cracks
            DropItem(new Item(6919)); //Floor Cracks
            DropItem(new Item(6920)); //Floor Cracks
            DropItem(new Item(6943)); //Leaves
            DropItem(new Item(6944)); //Leaves
            DropItem(new Item(6947)); //Leaves
            DropItem(new Item(3270)); //Grasses
            DropItem(new Item(3270)); //Grasses
            DropItem(new Item(3270)); //Grasses
            DropItem(new Item(7045)); //Twigs
            DropItem(new Item(7046)); //Twigs
            DropItem(new Item(7065)); //Twigs
            DropItem(new Item(7064)); //Twigs
        }

        public RuinedHomePack(Serial serial): base(serial)
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
