using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class FishHeads : Item
    {
        public override string DefaultName { get { return "fish heads"; } }

        [Constructable]
        public FishHeads() : base(7707)
        {
        }

        public FishHeads(Serial serial)
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
