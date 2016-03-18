using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    public class ReinforcedCap : BaseArmoredHat
    {
        public override string DefaultName { get { return "reinforced cap"; } }

        [Constructable]
        public ReinforcedCap()
            : base(5909)
        {
        }

        public ReinforcedCap(Serial serial)
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
