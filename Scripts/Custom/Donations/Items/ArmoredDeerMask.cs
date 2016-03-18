using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    [Flipable(0x1547, 0x1548)]
    public class ArmoredDeerMask : BaseArmoredHat
    {
        public override bool CanBeDyed { get { return false; } }
        public override string DefaultName { get { return "an armored deer mask"; } }

        [Constructable]
        public ArmoredDeerMask()
            : base(0x1547)
        {
        }

        public ArmoredDeerMask(Serial serial)
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
