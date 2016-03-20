// Delceri

using System;

namespace Server.Items
{
    public class HangingVinesLeftCenter : Item
    {
        [Constructable]
        public HangingVinesLeftCenter()
            : base(0x1A9F)
        {
            this.Name = ("hanging vines");
            this.Weight = 1.0;
        }

        public HangingVinesLeftCenter(Serial serial) : base(serial) { }

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