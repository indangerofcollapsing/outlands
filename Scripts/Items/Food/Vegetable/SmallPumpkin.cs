using System;

namespace Server.Items
{
    public class SmallPumpkin : Food
    {
        [Constructable]
        public SmallPumpkin(): this(1)
        {
        }

        [Constructable]
        public SmallPumpkin(int amount): base(0xC6C)
        {
            Name = "small pumkin";

            Stackable = true;
            Weight = 2.0;
            Amount = amount;
        }

        public SmallPumpkin(Serial serial): base(serial)
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