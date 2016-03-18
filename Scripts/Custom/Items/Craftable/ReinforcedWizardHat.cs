using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    public class ReinforcedWizardsHat : BaseArmoredHat
    {
        public override string DefaultName { get { return "reinforced wizard's hat"; } }

        [Constructable]
        public ReinforcedWizardsHat()
            : base(5912)
        {
        }

        public ReinforcedWizardsHat(Serial serial)
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
