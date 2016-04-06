using System;

namespace Server.Items
{
    public class OpenCoconut : Food
    {
        [Constructable]
        public OpenCoconut(): this(1)
        {
        }

        [Constructable]
        public OpenCoconut(int amount): base( 0x1723)
        {
            Name = "open coconut";

            Weight = 1.0;
            Amount = amount;
        }

        public OpenCoconut(Serial serial): base(serial)
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