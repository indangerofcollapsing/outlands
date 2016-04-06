using System;

namespace Server.Items
{
    public class SackOfFlour : Item
    {
        [Constructable]
        public SackOfFlour(): base(0x1039)
        {
            Stackable = true;
            Weight = 5;
        }

        public SackOfFlour(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}