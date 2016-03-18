// Delceri

using System;

namespace Server.Items
{
    public class VinesBramblySouth : Item
    {
        [Constructable]
        public VinesBramblySouth()
            : base(0xCED)
        {
            this.Name = ("vines");
            this.Weight = 1.0;
        }

        public VinesBramblySouth(Serial serial) : base(serial) { }

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