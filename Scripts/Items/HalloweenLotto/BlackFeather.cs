using System;
using System.Collections.Generic;
using Server.Custom;

namespace Server.Items
{
    public class BlackFeather : Item
    {
        public override string DefaultName { get { return "a black feather"; } }

        [Constructable]
        public BlackFeather()
            : base(4129)
        {
            DonationItem = true;
            Hue = 2051;
        }

        public BlackFeather(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Deleted)
                return;
        }

        private bool CheckLuck()
        {
            return Utility.RandomBool();
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
