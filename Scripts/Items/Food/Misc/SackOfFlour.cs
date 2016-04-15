using System;

namespace Server.Items
{
    public class SackOfFlour : Item
    {
        [Constructable]
        public SackOfFlour(): this(1)
        {
        }

        [Constructable]
        public SackOfFlour(int amount): base(4153)
        {
            Name = "sack of flour";

            Stackable = true;
            Weight = 2;
            Amount = amount;
        }

        public SackOfFlour(Serial serial): base(serial)
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