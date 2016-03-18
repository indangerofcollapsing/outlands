using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    public class ReinforcedTallStrawHat : BaseArmoredHat
    {
        public override string DefaultName { get { return "reinforced tall straw hat"; } }

        [Constructable]
        public ReinforcedTallStrawHat()
            : base(5910)
        {
        }

        public ReinforcedTallStrawHat(Serial serial)
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
