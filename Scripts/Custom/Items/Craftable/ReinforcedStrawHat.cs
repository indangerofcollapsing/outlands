using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    public class ReinforcedStrawHat : BaseArmoredHat
    {
        public override string DefaultName { get { return "reinforced straw hat"; } }

        [Constructable]
        public ReinforcedStrawHat()
            : base(5911)
        {
        }

        public ReinforcedStrawHat(Serial serial)
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
