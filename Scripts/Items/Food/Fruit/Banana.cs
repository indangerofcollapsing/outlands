using System;

namespace Server.Items
{
    [FlipableAttribute(0x171f, 0x1720)]
    public class Banana : Food
    {
        [Constructable]
        public Banana(): this(1)
        {
        }

        [Constructable]
        public Banana(int amount): base(0x171f)
        {
            Name = "banana";

            Weight = 1.0;
            Amount = amount;
        }

        public Banana(Serial serial): base(serial)
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