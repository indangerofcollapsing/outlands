using System;

namespace Server.Items
{
    [FlipableAttribute(0xC66, 0xC67)]
    public class GreenGourd : Food
    {
        [Constructable]
        public GreenGourd(): this(1)
        {
        }

        [Constructable]
        public GreenGourd(int amount): base(0xC66)
        {
            Name = "green gourd";

            Stackable = true;
            Weight = 1.0;
            Amount = amount;
        }

        public GreenGourd(Serial serial): base(serial)
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