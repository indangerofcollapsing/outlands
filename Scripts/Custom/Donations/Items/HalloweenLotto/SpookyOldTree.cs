using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class SpookyOldTree : Item
    {
        public override string DefaultName { get { return "Spooky Old Tree"; } }

        [Constructable]
        public SpookyOldTree()
            : base(0x0CE0)
        {
            Hue = 0;
        }

        public SpookyOldTree(Serial serial)
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
