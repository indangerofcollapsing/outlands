using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class GhoulStatue : Item
    {
        public override string DefaultName { get { return "Ghoul Statue"; } }

        [Constructable]
        public GhoulStatue()
            : base(0x2109)
        {
            Hue = 0;
        }

        public GhoulStatue(Serial serial)
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
