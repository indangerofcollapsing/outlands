using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    public class ReinforcedBonnet : BaseArmoredHat
    {
        public override string DefaultName { get { return "reinforced bonnet"; } }

        [Constructable]
        public ReinforcedBonnet()
            : base(5913)
        {
        }

        public ReinforcedBonnet(Serial serial)
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
