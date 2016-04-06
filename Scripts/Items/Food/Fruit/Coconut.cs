using System;

namespace Server.Items
{
    public class Coconut : Food
    {
        [Constructable]
        public Coconut(): this(1)
        {
        }

        [Constructable]
        public Coconut(int amount): base( 0x1726)
        {
            Name = "coconut";

            Weight = 1.0;
            Amount = amount;
        }

        public Coconut(Serial serial): base(serial)
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