using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class MongbatStew : Item
    {
        public override string DefaultName { get { return "mongbat stew"; } }

        [Constructable]
        public MongbatStew()
            : base(0x3DB1)
        {
        }

        public MongbatStew(Serial serial)
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
