using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class HangingFilets : Item
    {
        public override string DefaultName { get { return "hanging filets"; } }

        [Constructable]
        public HangingFilets() : base(5160)
        {
        }

        public HangingFilets(Serial serial)
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
