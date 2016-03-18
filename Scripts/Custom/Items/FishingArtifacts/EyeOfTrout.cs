using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class EyeOfTrout : Item
    {
        public override string DefaultName { get { return "eye of trout"; } }

        [Constructable]
        public EyeOfTrout() : base(3975)
        {
        }

        public EyeOfTrout(Serial serial)
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
