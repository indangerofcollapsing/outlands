using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    public class ReinforcedWideBrimHat : BaseArmoredHat
    {
        public override string DefaultName { get { return "reinforced wide-brim hat"; } }

        [Constructable]
        public ReinforcedWideBrimHat()
            : base(5908)
        {
        }

        public ReinforcedWideBrimHat(Serial serial)
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
