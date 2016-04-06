using System;

namespace Server.Items
{
    public class RawLobster : Item
    {
        [Constructable]
        public RawLobster(): this(1)
        {
        }

        [Constructable]
        public RawLobster(int amount): base(0x44D3)
        {
            Name = "raw lobster";

            Stackable = true;
            Weight = 0.1;
            Amount = amount;
        }

        public RawLobster(Serial serial): base(serial)
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