using System;

namespace Server.Items
{
    public class RawFishFillet : Item
    {
        [Constructable]
        public RawFishFillet(): this(1)
        {
        }

        [Constructable]
        public RawFishFillet(int amount): base(7708)
        {
            Name = "raw fish fillet";

            Stackable = true;
            Weight = 0.1;
            Amount = amount;
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