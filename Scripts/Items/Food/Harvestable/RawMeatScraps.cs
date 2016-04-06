using System;

namespace Server.Items
{
    public class RawMeatScraps : Item
    {
        [Constructable]
        public RawMeatScraps(): base(7820)
        {
            Name = "raw meat scraps";

            Stackable = true;
            Weight = 0.1;
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