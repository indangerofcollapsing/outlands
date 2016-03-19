using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class HalloweenSandals : Sandals
    {
        public override string DefaultName { get { return "Halloween Sandals"; } }

        [Constructable]
        public HalloweenSandals()

        {
            Hue = 2621;
        }

        public HalloweenSandals(Serial serial)
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
