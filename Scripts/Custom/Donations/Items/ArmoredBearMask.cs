using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    [Flipable(0x1545, 0x1546)]
    public class ArmoredBearMask : BaseArmoredHat
    {
        public override bool CanBeDyed { get { return false; } }
        public override string DefaultName { get { return "an armored bear mask"; } }

        [Constructable]
        public ArmoredBearMask()
            : base(0x1545)
        {
        }

        public ArmoredBearMask(Serial serial)
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
