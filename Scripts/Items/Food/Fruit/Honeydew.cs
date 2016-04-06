using System;

namespace Server.Items
{
    [FlipableAttribute(0xC74, 0xC75)]
    public class Honeydew : Food
    {
        [Constructable]
        public Honeydew() : this(1)
        {
        }

        [Constructable]
        public Honeydew(int amount): base(0xC74)
        {
            Name = "honeydew";

            Weight = 1.0;
            Amount = amount;
        }

        public Honeydew(Serial serial): base(serial)
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