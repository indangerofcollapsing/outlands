using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    public class ReinforcedBandana : BaseArmoredHat
    {
        public override string DefaultName { get { return "reinforced bandana"; } }

        [Constructable]
        public ReinforcedBandana()
            : base(5440)
        {
        }

        public ReinforcedBandana(Serial serial)
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
