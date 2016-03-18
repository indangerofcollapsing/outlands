using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    public class LeatherPatchedTallStrawHat : BaseArmoredHat
    {
        public override string DefaultName { get { return "leather patched tall straw hat"; } }

        [Constructable]
        public LeatherPatchedTallStrawHat()
            : base(5910)
        {
        }

        public LeatherPatchedTallStrawHat(Serial serial)
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
