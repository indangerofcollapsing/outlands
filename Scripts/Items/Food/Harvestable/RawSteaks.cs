using System;

namespace Server.Items
{
    public class RawSteaks : Item
    {
        [Constructable]
        public RawSteaks(): base(7711)
        {
            Name = "raw steaks";

            Stackable = true;
            Weight = 0.1;
        }

        public RawSteaks(Serial serial): base(serial)
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