using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class SosarianTapestry : Item
    {
        public override string DefaultName { get { return "Sosarian Tapestry"; } }

        [Constructable]
        public SosarianTapestry()
            : base(0x234F)
        {
            Hue = 0;
        }

        public SosarianTapestry(Serial serial)
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
