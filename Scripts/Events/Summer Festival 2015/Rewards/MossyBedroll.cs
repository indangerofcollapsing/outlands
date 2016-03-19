using System;

namespace Server.Items
{
    public class MossyBedRoll : Bedroll
    {
        [Constructable]
        public MossyBedRoll()
        {
            Name = "mossy bedroll";
            Hue = 2129;
        }

        public MossyBedRoll(Serial serial) : base(serial) 
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}