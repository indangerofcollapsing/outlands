using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    public class ReinforcedTricorneHat : BaseArmoredHat
    {
        public override string DefaultName { get { return "reinforced tricorne hat"; } }

        [Constructable]
        public ReinforcedTricorneHat()
            : base(5915)
        {
        }

        public ReinforcedTricorneHat(Serial serial)
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
