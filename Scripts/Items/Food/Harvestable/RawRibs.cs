using System;

namespace Server.Items
{
    public class RawRibs : Item
    {
        [Constructable]
        public RawRibs(): base(2545)
        {
            Name = "raw ribs";

            Stackable = true;
            Weight = 0.1;
        }

        public RawRibs(Serial serial): base(serial)
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