using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    public class ArmoredTribalMask : BaseArmoredHat
    {
        public override bool CanBeDyed { get { return false; } }
        public override string DefaultName { get { return "an armored tribal mask"; } }

        [Constructable]
        public ArmoredTribalMask()
            : base(0x154B)
        {
        }

        public ArmoredTribalMask(Serial serial)
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
