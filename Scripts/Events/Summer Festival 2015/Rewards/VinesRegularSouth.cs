// Delceri

using System;

namespace Server.Items
{
    public class VinesRegularSouth : Item
    {
        [Constructable]
        public VinesRegularSouth()
            : base(0xCEC)
        {
            this.Name = ("vines");
            this.Weight = 1.0;
        }

        public VinesRegularSouth(Serial serial) : base(serial) { }

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