using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class RawFish : Item
    {
        public override string DefaultName { get { return "a raw fish"; } }

        [Constructable]
        public RawFish() : base(7702)
        {
        }

        public RawFish(Serial serial)
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
