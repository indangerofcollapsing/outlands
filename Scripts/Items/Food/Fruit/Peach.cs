using System;

namespace Server.Items
{
    public class Peach : Food
    {
        [Constructable]
        public Peach(): this(1)
        {
        }

        [Constructable]
        public Peach(int amount): base( 0x9D2)
        {
            Name = "peach";

            Weight = 1.0;
            Amount = amount;
        }

        public Peach(Serial serial): base(serial)
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