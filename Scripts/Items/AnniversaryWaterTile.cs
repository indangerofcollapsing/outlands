using System;
using System.Collections.Generic;

namespace Server.Items
{
    [FlipableAttribute(0x1797, 0x1797)]
    public class AnniversaryWaterTile : Item
    {
        public override string DefaultName { get { return "a water tile"; } }

        [Constructable]
        public AnniversaryWaterTile()
            : base(6039)
        {
            Hue = 0;

        }

        public AnniversaryWaterTile(Serial serial)
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
