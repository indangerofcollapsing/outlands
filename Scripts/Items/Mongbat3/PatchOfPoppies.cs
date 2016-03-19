using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class PatchOfPoppies : Item
    {
        public override string DefaultName { get { return "Patch Of Poppies"; } }

        [Constructable]
        public PatchOfPoppies()
            : base(0x0CBF)
        {
            Hue = 0;
        }

        public PatchOfPoppies(Serial serial)
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
