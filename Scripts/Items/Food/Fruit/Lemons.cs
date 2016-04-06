using System;

namespace Server.Items
{
    public class Lemons : Food
    {
        [Constructable]
        public Lemons(): this(1)
        {
        }

        [Constructable]
        public Lemons(int amount): base( 0x1729)
        {
            Name = "lemons";

            Weight = 1.0;
            Amount = amount;
        }

        public Lemons(Serial serial): base(serial)
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