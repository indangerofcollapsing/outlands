using System;

namespace Server.Items
{
    public class Grapes : Food
    {
        [Constructable]
        public Grapes(): this(1)
        {
        }

        [Constructable]
        public Grapes(int amount): base( 0x9D1)
        {
            Name = "grapes";

            Weight = 1.0;
            Amount = amount;
        }

        public Grapes(Serial serial): base(serial)
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