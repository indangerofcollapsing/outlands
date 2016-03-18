using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    public class ReinforcedJesterHat : BaseArmoredHat
    {
        public override string DefaultName { get { return "reinforced jester hat"; } }

        [Constructable]
        public ReinforcedJesterHat()
            : base(5916)
        {
        }

        public ReinforcedJesterHat(Serial serial)
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
