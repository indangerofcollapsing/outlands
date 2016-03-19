using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class PirateGlassEye : Item
    {
        public override string DefaultName { get { return "pirate's glass eye"; } }

        [Constructable]
        public PirateGlassEye() : base(3975)
        {
            Hue = 2101;
        }

        public PirateGlassEye(Serial serial)
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
