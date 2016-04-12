using System;
using Server;
using Server.Mobiles;
using System.Collections;

namespace Server.Items
{
    public class DeDOSKynDragonStatue : RareStatue
    {
        public override string DisplayName { get { return "trophy: a kyn dragon"; } }
        public override int DisplayItemId { get { return 17062; } }
        public override int DisplayItemHue { get { return 2603; } }
        public override int ClickSound { get { return Utility.RandomList(0x4FC, 0x4FD, 0x4FC, 0x4EC, 0x4EA); } }

        [Constructable]
        public DeDOSKynDragonStatue(): base()
        {
        }

        public DeDOSKynDragonStatue(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}