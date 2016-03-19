using System;

namespace Server.Items
{
    public class OldBranch : GnarledStaff
    {
        public override int InitMinHits { get { return 500; } }
        public override int InitMaxHits { get { return 500; } }

        [Constructable]
        public OldBranch()
        {
            Name = "an old branch";
            Hue = 2001;           
        }

        public OldBranch(Serial serial) : base(serial)
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