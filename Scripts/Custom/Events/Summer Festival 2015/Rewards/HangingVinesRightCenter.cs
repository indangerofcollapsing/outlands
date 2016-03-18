// Delceri

using System;

namespace Server.Items
{
    public class HangingVinesRightCenter : Item
    {
        [Constructable]
        public HangingVinesRightCenter()
            : base(0x1AA1)
        {
            this.Name = ("hanging vines");
            this.Weight = 1.0;
        }

        public HangingVinesRightCenter(Serial serial) : base(serial) { }

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