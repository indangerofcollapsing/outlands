using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    public class LeatherPatchedTricorneHat : BaseArmoredHat
    {
        public override string DefaultName { get { return "leather patched tricorne hat"; } }

        [Constructable]
        public LeatherPatchedTricorneHat()
            : base(5915)
        {
        }

        public LeatherPatchedTricorneHat(Serial serial)
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
