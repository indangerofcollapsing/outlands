using System;

namespace Server.Items
{
    public class RawCrab : Item
    {
        [Constructable]
        public RawCrab(): this(1)
        {
        }

        [Constructable]
        public RawCrab(int amount): base(17617)
        {
            Name = "raw crab";
            Hue = 2117;

            Stackable = true;
            Weight = 0.1;
            Amount = amount;
        }

        public RawCrab(Serial serial): base(serial)
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