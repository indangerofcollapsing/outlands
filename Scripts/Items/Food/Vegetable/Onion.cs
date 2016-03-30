using System;

namespace Server.Items
{
    [FlipableAttribute(0xc6d, 0xc6e)]
    public class Onion : Food
    {
        [Constructable]
        public Onion() : this(1)
        {
        }

        [Constructable]
        public Onion(int amount) : base(amount, 0xc6d)
        {
            Weight = 1.0;
        }

        public Onion(Serial serial): base(serial)
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