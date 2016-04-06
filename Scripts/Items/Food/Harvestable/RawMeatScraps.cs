using System;

namespace Server.Items
{
    public class RawMeatScraps : Item
    {
        //TEST: NOT STACKABLE ITEMID

        [Constructable]
        public RawMeatScraps(): this(1)
        {
        }

        [Constructable]
        public RawMeatScraps(int amount): base(7820)
        {
            Name = "raw meat scraps";

            Stackable = true;
            Weight = 0.1;
            Amount = amount;
        }

        public RawMeatScraps(Serial serial): base(serial)
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