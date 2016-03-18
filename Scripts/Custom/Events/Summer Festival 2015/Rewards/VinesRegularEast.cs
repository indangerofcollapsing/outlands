// Delceri

using System;

namespace Server.Items
{
    public class VinesRegularEast : Item
    {
        [Constructable]
        public VinesRegularEast()
            : base(0xCF0)
        {
            this.Name = ("vines");
            this.Weight = 1.0;
        }

        public VinesRegularEast(Serial serial) : base(serial) { }

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