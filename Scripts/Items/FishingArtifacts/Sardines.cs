using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class Sardines : Item
    {
        public override string DefaultName { get { return "sardines"; } }

        [Constructable]
        public Sardines() : base(3544)
        {
            Hue = 2106;
        }

        public Sardines(Serial serial)
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
