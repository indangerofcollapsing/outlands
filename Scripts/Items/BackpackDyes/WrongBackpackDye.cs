using Server.Mobiles;
using Server.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Items
{
    class WrongBackpackDye : BackpackDye
    {
        [Constructable]
        public WrongBackpackDye(): base()
        {
            Name = "Wrong backpack dye";
            DyeHue = 2675;
        }

        public WrongBackpackDye(Serial serial): base(serial)
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
