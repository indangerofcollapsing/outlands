// Delceri

using System;

namespace Server.Items
{
    public class VinesLeafySouth : Item
    {
        [Constructable]
        public VinesLeafySouth()
            : base(0xCF2)
        {
            this.Name = ("hanging vines");
            this.Weight = 1.0;
        }

        public VinesLeafySouth(Serial serial) : base(serial) { }

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