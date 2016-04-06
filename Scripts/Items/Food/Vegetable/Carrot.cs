using System;

namespace Server.Items
{
    [FlipableAttribute(0xc77, 0xc78)]
    public class Carrot : Food
    {
        [Constructable]
        public Carrot(): this(1)
        {
        }

        [Constructable]
        public Carrot(int amount): base(0xc78)
        {
            Name = "carrot";

            Stackable = true;
            Weight = 1.0;
            Amount = amount;
        }

        public Carrot(Serial serial): base(serial)
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