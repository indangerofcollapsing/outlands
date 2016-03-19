using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class BlackCatStatue : Item
    {
        public override string DefaultName { get { return "Black Cat Statue"; } }

        [Constructable]
        public BlackCatStatue()
            : base(0x211B)
        {
            Hue = 2051;
        }

        public BlackCatStatue(Serial serial)
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
