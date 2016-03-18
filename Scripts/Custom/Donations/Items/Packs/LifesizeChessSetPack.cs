using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class LifesizeChessSetPack : Bag
    {
        [Constructable]
        public LifesizeChessSetPack()
        {
            Name = "Lifesize Chess Set Pack";
	
            PackItems();
        }

        public void PackItems()
        {
            //White
            DropItem(new Item(13701)); //Bishop
            DropItem(new Item(13701)); //Bishop
            DropItem(new Item(13702)); //Rook
            DropItem(new Item(13702)); //Rook
            DropItem(new Item(13704)); //Knight
            DropItem(new Item(13704)); //Knight
            DropItem(new Item(13703)); //Queen
            DropItem(new Item(13706)); //King
            DropItem(new Item(13705)); //Pawn
            DropItem(new Item(13705)); //Pawn
            DropItem(new Item(13705)); //Pawn
            DropItem(new Item(13705)); //Pawn
            DropItem(new Item(13705)); //Pawn
            DropItem(new Item(13705)); //Pawn
            DropItem(new Item(13705)); //Pawn
            DropItem(new Item(13705)); //Pawn

            //Black
            DropItem(new Item(13708)); //Bishop
            DropItem(new Item(13708)); //Bishop
            DropItem(new Item(13709)); //Rook
            DropItem(new Item(13709)); //Rook
            DropItem(new Item(13711)); //Knight
            DropItem(new Item(13711)); //Knight
            DropItem(new Item(13710)); //Queen
            DropItem(new Item(13713)); //King
            DropItem(new Item(13712)); //Pawn
            DropItem(new Item(13712)); //Pawn
            DropItem(new Item(13712)); //Pawn
            DropItem(new Item(13712)); //Pawn
            DropItem(new Item(13712)); //Pawn
            DropItem(new Item(13712)); //Pawn
            DropItem(new Item(13712)); //Pawn
            DropItem(new Item(13712)); //Pawn
        }

        public LifesizeChessSetPack(Serial serial): base(serial)
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
