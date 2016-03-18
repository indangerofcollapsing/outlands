// Delceri

using System;

namespace Server.Items
{
    public class HangingPinkFlowersVinesEast : Item
    {
        [Constructable]
        public HangingPinkFlowersVinesEast()
            : base(0x2CFA)
        {
            this.Name = ("hanging vines");
            this.Weight = 1.0;
        }

        public HangingPinkFlowersVinesEast(Serial serial) : base(serial) {}

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