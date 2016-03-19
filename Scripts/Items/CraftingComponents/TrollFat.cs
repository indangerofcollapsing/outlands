using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class TrollFat : CraftingComponent
    {
        [Constructable]
        public TrollFat(): this(1)
		{
            Name = "troll fat";

            ItemID = 5163;
            Hue = 2612;
		}

        [Constructable]
        public TrollFat(int amount): base(amount)
		{
            Name = "troll fat";

            ItemID = 5163;
            Hue = 2612;
		}

        public TrollFat(Serial serial): base(serial)
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