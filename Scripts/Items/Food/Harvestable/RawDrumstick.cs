using System;

namespace Server.Items
{
    public class RawDrumstick : Item
    {
        [Constructable]
        public RawDrumstick(): base(5639)
        {
            Name = "raw drumstick";

            Stackable = true;
            Weight = 0.1;
        }

        public RawDrumstick(Serial serial): base(serial)
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