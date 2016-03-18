// Delceri

using System;

namespace Server.Items
{
    public class HangingPurpleFlowersVinesEast : Item
    {
        [Constructable]
        public HangingPurpleFlowersVinesEast()
            : base(0x2CF9)
        {
            this.Name = ("hanging vines");
            this.Weight = 1.0;
        }

        public HangingPurpleFlowersVinesEast(Serial serial) : base(serial) { }

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