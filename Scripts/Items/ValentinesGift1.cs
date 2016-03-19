using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class ValentinesGift1 : GiftBoxAngel
    {
        [Constructable]
        public ValentinesGift1()
        {
            Name = "Valentine's Gift";
            PackItems();
        }

        public void PackItems()
        {
            AddItem(new BlueBook());

            var candle = new CandleLong();
            candle.Hue = 2619;
            candle.Name = "Be My Valentine";
            DropItem(candle);

            DropItem(new IcePoinsettia());

            DropItem(new PinkGoldEarrings());

        }

        public ValentinesGift1(Serial serial)
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
