using Server.Mobiles;
using Server.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Items
{
    class HythlothBackpackDye : BackpackDye
    {
        [Constructable]
        public HythlothBackpackDye(): base()
        {
            Name = "Hythloth backpack dye";
            DyeHue = 1769;
        }

        public HythlothBackpackDye(Serial serial): base(serial)
        {
        }       

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
