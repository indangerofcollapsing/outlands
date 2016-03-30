using System;

namespace Server.Items
{
    public class Watermelon : Food
    {
        [Constructable]
        public Watermelon(): this(1)
        {
        }

        [Constructable]
        public Watermelon(int amount): base(amount, 0xC5C)
        {
            Weight = 5.0;
        }

        public Watermelon(Serial serial) : base(serial)
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