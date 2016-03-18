using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    public class LeatherPatchedFloppyHat : BaseArmoredHat
    {
        public override string DefaultName { get { return "leather patched floppy hat"; } }

        [Constructable]
        public LeatherPatchedFloppyHat()
            : base(5907)
        {
        }

        public LeatherPatchedFloppyHat(Serial serial)
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
