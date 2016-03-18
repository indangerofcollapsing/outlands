using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    public class ReinforcedFeatheredHat : BaseArmoredHat
    {
        public override string DefaultName { get { return "reinforced feathered hat"; } }

        [Constructable]
        public ReinforcedFeatheredHat()
            : base(5914)
        {
        }

        public ReinforcedFeatheredHat(Serial serial)
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
