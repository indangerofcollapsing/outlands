using System;

namespace Server.Items
{
    [FlipableAttribute(0xC7F, 0xC81)]
    public class EarOfCorn : Food
    {
        [Constructable]
        public EarOfCorn(): this(1)
        {
        }

        [Constructable]
        public EarOfCorn(int amount): base(0xC81)
        {
            Name = "ear of corn";

            Stackable = true;
            Weight = 1.0;
            Amount = amount;
        }

        public EarOfCorn(Serial serial): base(serial)
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