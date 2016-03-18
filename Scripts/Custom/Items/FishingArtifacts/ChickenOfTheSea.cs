using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class ChickenOfTheSea : ChickenLeg
    {
        public override string DefaultName { get { return "Chicken of the Sea"; } }

        [Constructable]
        public ChickenOfTheSea()
        {
        }

        public ChickenOfTheSea(Serial serial)
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
