using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class ValentinesGift : GiftBoxAngel
    {
        [Constructable]
        public ValentinesGift()
        {
            Name = "Valentine's Gift";
            PackItems();
        }

        public void PackItems()
        {
            AddItem(new BlueBook());

            var candle = new CandleLong();
            candle.Hue = 34;
            candle.Name = "Be My Valentine";
            DropItem(candle);

            var book = new BlueBook();
            book.Hue = 30;
            DropItem(book);

            DropItem(new RedPoinsettia());
        }

        public ValentinesGift(Serial serial)
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

            //-----

            Name = "Valentine's Gift";
        }
    }
}
