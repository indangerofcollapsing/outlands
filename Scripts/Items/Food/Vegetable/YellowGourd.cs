using System;

namespace Server.Items
{
    [FlipableAttribute(0xC64, 0xC65)]
    public class YellowGourd : Food
    {
        [Constructable]
        public YellowGourd(): this(1)
        {
        }

        [Constructable]
        public YellowGourd(int amount): base( 0xC64)
        {
            Name = "yellow gourd";

            Stackable = true;
            Weight = 1.0;
            Amount = amount;
        }

        public YellowGourd(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}