using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class ChristmasPack : Bag
    {
        public enum ChristmasPackType
        {
            Green, 
            Red,
            White
        }

        public override string DefaultName {
            get {
                return "christmas pack";
            }
        }

        [Constructable]
        public ChristmasPack()
        {
            PackItems((ChristmasPackType)Utility.Random(3));
        }

        [Constructable]
        public ChristmasPack(ChristmasPackType type) {
            PackItems(type);
        }

        public void PackItems(ChristmasPackType type)
        {
            switch (type) {
                case ChristmasPackType.Green:
                    DropItem(new WreathDeed { Hue = 73 });
                    DropItem(new MistletoeDeed { Name = "Green Mistletoe (rare)", Hue = 73 });
                    DropItem(new Item {ItemID = 0x5738, Name = "ice shards", Hue = 0 }); //crystal
                    DropItem(new GreenStocking {Name = "green christmas stocking", Hue = 0 });
                    DropItem(new Item {ItemID = 0x46EC, Name = "green nutcracker", Hue = 2963 }); 
                    DropItem(new RedPoinsettia {Name = "green poinsettia", Hue = 1267 });
                    DropItem(new Item {ItemID = 0x236E, Name = "green holiday candle", Hue = 73 });
                    break;
                case ChristmasPackType.Red:
                    DropItem(new WreathDeed { Hue = 133 });
                    DropItem(new MistletoeDeed { Name = "Red Mistletoe (extremely rare)", Hue = 133 });
                    DropItem(new Item {ItemID = 0x5738, Name = "ice shards", Hue = 0 }); //crystal
                    DropItem(new RedStocking {Name = "red christmas stocking", Hue = 0 }); 
                    DropItem(new Item {ItemID = 0x46EC, Name = "red nutcracker", Hue = 1645 });
                   DropItem(new RedPoinsettia {Name = "red poinsettia", Hue = 0 });
                    DropItem(new Item {ItemID = 0x236E, Name = "red holiday candle", Hue = 133 });
                    break;
                case ChristmasPackType.White:
                    DropItem(new WreathDeed {  Hue = 1150 });
                    DropItem(new MistletoeDeed { Name = "White Mistletoe (ultra rare)", Hue = 1150 });
                    DropItem(new Item {ItemID = 0x5738, Name = "ice shards", Hue = 0 }); //crystal
                    DropItem(new RedStocking { Name = "white christmas stocking", Hue = 1150 }); 
                    DropItem(new Item {ItemID = 0x46EC, Name = "white nutcracker", Hue = 1150 });
                   DropItem(new WhitePoinsettia {Name = "white poinsettia", Hue = 0 });
                    DropItem(new Item {ItemID = 0x236E, Name = "white holiday candle", Hue = 1150 });
                    break;

            }
        }

        public ChristmasPack(Serial serial)
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
