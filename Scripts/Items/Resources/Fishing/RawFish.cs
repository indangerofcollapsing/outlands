using System;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
    public class RawFish : Item, ICarvable
    {
        [Constructable]
        public RawFish(): this(1)
		{
		}

        [Constructable]
        public RawFish(int amount): base(7710)
        {
            Name = "raw fish";
            Hue = 2543;

            Stackable = true;
            Weight = 0.1;
            Amount = amount;
        }

        public RawFish(Serial serial): base(serial)
        {
        }

        public void Carve(Mobile from, Item item)
        {
            base.ScissorHelper(from, new RawFishSteak(), 1);
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