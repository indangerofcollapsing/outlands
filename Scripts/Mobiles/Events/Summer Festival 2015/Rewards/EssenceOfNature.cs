using System;

namespace Server.Items
{
    public class EssenceOfNature : Item
    {
        [Constructable]
        public EssenceOfNature(): base(0x1848)
        {
            Name = ("essence of nature");
            Weight = 1.0;
        }

        public EssenceOfNature(Serial serial) : base(serial)
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