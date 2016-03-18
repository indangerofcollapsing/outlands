using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class BlackCatStatueRare : Item
    {
        public override string DefaultName { get { return "Black Cat Statue"; } }

        [Constructable]
        public BlackCatStatueRare()
            : base(0x4688)
        {
            Hue = 0;
        }

        public BlackCatStatueRare(Serial serial)
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
