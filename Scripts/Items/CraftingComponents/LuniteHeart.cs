using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class LuniteHeart : CraftingComponent
    {
        [Constructable]
        public LuniteHeart(): this(1)
		{
            Name = "lunite heart";

            ItemID = 12126; 
            Hue = 2605;
		}

        [Constructable]
        public LuniteHeart(int amount): base(amount)
		{
            Name = "lunite heart";

            ItemID = 12126;
            Hue = 2605;
		}

        public LuniteHeart(Serial serial): base(serial)
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