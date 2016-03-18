using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class Ghostweed : CraftingComponent
    {
        [Constructable]
        public Ghostweed(): this(1)
		{
            Name = "ghostweed";

            ItemID = 731; 
            Hue = 2498;
		}

        [Constructable]
        public Ghostweed(int amount): base(amount)
		{
            Name = "ghostweed";

            ItemID = 731;
            Hue = 2498;
		}

        public Ghostweed(Serial serial): base(serial)
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