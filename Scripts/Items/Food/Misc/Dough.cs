using System;

namespace Server.Items
{
    public class Dough : Item
    {
        [Constructable]
        public Dough(): this(1)
        {
        }

        [Constructable]
        public Dough(int amount): base(0x103d)
        {
            Name = "dough";

            Stackable = true;
            Weight = 1;
            Amount = amount;
        }

        public Dough(Serial serial): base(serial)
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