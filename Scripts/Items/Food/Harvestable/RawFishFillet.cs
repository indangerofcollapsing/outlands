using System;

namespace Server.Items
{
    public class RawFishFillet : Item
    {
        [Constructable]
        public RawFishFillet(): base(7701)
        {
            Name = "raw fish fillet";

            Stackable = true;
            Weight = 0.1;
        }

        public RawFishFillet(Serial serial): base(serial)
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