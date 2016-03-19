using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class GrapeSandals : Sandals
    {
        public override string DefaultName { get { return "Grape-Stained Sandals"; } }

        [Constructable]
        public GrapeSandals()

        {
            Hue = 512;
        }

        public GrapeSandals(Serial serial)
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
