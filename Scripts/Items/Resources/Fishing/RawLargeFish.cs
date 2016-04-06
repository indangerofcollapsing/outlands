using System;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
    public class RawLargeFish : Item, ICarvable
    {
        [Constructable]
        public RawLargeFish(): this(1)
		{
		}

        [Constructable]
        public RawLargeFish(int amount): base(0x09CC)
        {
            Name = "raw large fish";
            Hue = 2543;

            Stackable = true;
            Weight = 1;
            Amount = amount;
        }

        public RawLargeFish(Serial serial): base(serial)
        {
        }

        public void Carve(Mobile from, Item item)
        {
            base.ScissorHelper(from, new RawFishFillet(), 1);
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