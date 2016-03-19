using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class Snakeskin : CraftingComponent
    {
        [Constructable]
        public Snakeskin(): this(1)
		{
            Name = "snakeskin";

            ItemID = 22340; 
            Hue = 2515;
		}

        [Constructable]
        public Snakeskin(int amount): base(amount)
		{
            Name = "snakeskin";

            ItemID = 22340;
            Hue = 2515;
		}

        public Snakeskin(Serial serial): base(serial)
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