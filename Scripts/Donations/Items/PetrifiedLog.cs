using System;
using System.Collections.Generic;

namespace Server.Items
{
    [FlipableAttribute(0x1bdd, 0x1be0)]
    public class PetrifiedLog : Item
    {
        public override string DefaultName { get { return "a petrified log"; } }

        [Constructable]
        public PetrifiedLog()
            : base(7133)
        {
            Hue = 2404;
            Stackable = true;
        }

        public PetrifiedLog(Serial serial)
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
