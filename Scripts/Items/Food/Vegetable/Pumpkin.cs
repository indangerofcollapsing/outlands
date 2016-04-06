using System;

namespace Server.Items
{
    [FlipableAttribute(0xC6A, 0xC6B)]
    public class Pumpkin : Food
    {
        [Constructable]
        public Pumpkin(): this(1)
        {
        }

        [Constructable]
        public Pumpkin(int amount): base(0xC6A)
        {
            Name = "pumpkin";

            Stackable = true;
            Weight = 4;
            Amount = amount;
        }

        public Pumpkin(Serial serial): base(serial)
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