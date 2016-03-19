using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class SpookyHedge : Item
    {
        public override string DefaultName { get { return "Spooky Hedge"; } }

        [Constructable]
        public SpookyHedge()
            : base(3218)
        {
            Hue = 2105;
        }

        public SpookyHedge(Serial serial)
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
