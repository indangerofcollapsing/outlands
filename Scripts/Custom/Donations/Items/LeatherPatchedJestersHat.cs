using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    public class LeatherPatchedJesterHat : BaseArmoredHat
    {
        public override string DefaultName { get { return "leather patched jester hat"; } }

        [Constructable]
        public LeatherPatchedJesterHat()
            : base(5916)
        {
        }

        public LeatherPatchedJesterHat(Serial serial)
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
