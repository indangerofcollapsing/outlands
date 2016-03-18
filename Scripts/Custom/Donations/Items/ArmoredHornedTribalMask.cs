using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Donations.Items
{
    public class ArmoredHornedTribalMask : BaseArmoredHat
    {
        public override bool CanBeDyed { get { return false; } }

        public override string DefaultName { get { return "armored horned tribal mask"; } }

        [Constructable]
        public ArmoredHornedTribalMask()
            : base(0x1549)
        {
        }

        public ArmoredHornedTribalMask(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
