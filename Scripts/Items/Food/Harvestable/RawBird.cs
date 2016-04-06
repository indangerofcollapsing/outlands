using System;

namespace Server.Items
{
    public class RawBird : Item
    {
        [Constructable]
        public RawBird(): base(2489)
        {
            Name = "raw bird";

            Stackable = true;
            Weight = 0.1;
        }

        public RawBird(Serial serial): base(serial)
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