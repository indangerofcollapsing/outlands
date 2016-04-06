using System;

namespace Server.Items
{
    public class Lime : Food
    {
        [Constructable]
        public Lime(): this(1)
        {
        }

        [Constructable]
        public Lime(int amount): base( 0x172a)
        {
            Name = "lime";

            Weight = 1.0;
            Amount = amount;
        }

        public Lime(Serial serial): base(serial)
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