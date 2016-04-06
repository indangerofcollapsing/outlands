using System;

namespace Server.Items
{
    public class RawBacon : Item
    {
        [Constructable]
        public RawBacon(): base(2422)
        {
            Name = "raw bacon";

            Stackable = true;
            Weight = 0.1;
        }

        public RawBacon(Serial serial): base(serial)
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