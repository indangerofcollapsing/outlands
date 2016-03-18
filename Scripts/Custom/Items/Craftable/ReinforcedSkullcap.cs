using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    public class ReinforcedSkullcap : BaseArmoredHat
    {
        public override string DefaultName { get { return "reinforced skullcap"; } }

        [Constructable]
        public ReinforcedSkullcap()
            : base(5444)
        {
        }

        public ReinforcedSkullcap(Serial serial)
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
