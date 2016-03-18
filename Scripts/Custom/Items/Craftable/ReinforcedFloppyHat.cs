using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    public class ReinforcedFloppyHat : BaseArmoredHat
    {
        public override string DefaultName { get { return "reinforced floppy hat"; } }

        [Constructable]
        public ReinforcedFloppyHat()
            : base(5907)
        {
        }

        public ReinforcedFloppyHat(Serial serial)
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
