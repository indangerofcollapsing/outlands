using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class CampionFlowerPatch : Item
    {
        public override string DefaultName { get { return "Campion Flower Patch"; } }

        [Constructable]
        public CampionFlowerPatch()
            : base(0x0C83)
        {
            Hue = 0;
        }

        public CampionFlowerPatch(Serial serial)
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
