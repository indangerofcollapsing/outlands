using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class SpookyTreeStatue : Item
    {
        public override string DefaultName { get { return "Spooky Tree Statue"; } }

        [Constructable]
        public SpookyTreeStatue()
            : base(0x20FA)
        {
            Hue = 2501;
        }

        public SpookyTreeStatue(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}
