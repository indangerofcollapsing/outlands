using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class ClothMapDecoration : Item
    {
        public override string DefaultName { get { return "UO Cloth Map"; } }

        [Constructable]
        public ClothMapDecoration()
            : base(15287)
        {
            Hue = 0;
        }

        public ClothMapDecoration(Serial serial)
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
