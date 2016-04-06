using System;

namespace Server.Items
{
    public class Dates : Food
    {
        [Constructable]
        public Dates(): this(1)
        {
        }

        [Constructable]
        public Dates(int amount): base( 0x1727)
        {
            Name = "dates";

            Weight = 1.0;
            Amount = amount;
        }

        public Dates(Serial serial): base(serial)
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