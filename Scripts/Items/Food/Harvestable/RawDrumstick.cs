using System;

namespace Server.Items
{
    public class RawDrumstick : Item
    {
        [Constructable]
        public RawDrumstick() : this(1)
        {
        }

        [Constructable]
        public RawDrumstick(int amount): base(5639)
        {
            Name = "raw drumstick";

            Stackable = true;
            Weight = 0.1;
            Amount = amount;
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