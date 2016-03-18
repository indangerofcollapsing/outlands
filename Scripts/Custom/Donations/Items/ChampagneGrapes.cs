using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class ChampagneGrapes : Grapes
    {
        public override string DefaultName { get { return "champagne grapes"; } }

        [Constructable]
        public ChampagneGrapes()
        {
            Hue = 2425;
        }

        public ChampagneGrapes(Serial serial)
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
