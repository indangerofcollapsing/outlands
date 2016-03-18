using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class MageDecoBag : Bag
    {
        [Constructable]
        public MageDecoBag()
        {
            Name = "Mage's Bag";
            PackItems();
        }

        public void PackItems()
        {
            DropItem(new GreenDriedFlowers());
            DropItem(new WhiteDriedFlowers());
            DropItem(new Candelabra());
            DropItem(new Item(0xf03)); //bottle
            DropItem(new Item(0xe29)); //bottle2
            DropItem(new AniRedRibbedFlask());
            DropItem(new AniLargeVioletFlask());
            DropItem(new SmallYellowFlask());

            var bookcase = new RuinedBookcase();
            bookcase.Movable = true;
            DropItem(bookcase); 

            var pentagram = new PentagramDeed();
            double rand = Utility.RandomDouble();

            if (rand < 0.1)
                pentagram.Hue = 2405;
            else if (rand < 0.4)
                pentagram.Hue = 2414;
            else
                pentagram.Hue = 2101;

            DropItem(pentagram);
        }

        public MageDecoBag(Serial serial)
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
