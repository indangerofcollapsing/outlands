using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class FireEssence : CraftingComponent
    {
        [Constructable]
        public FireEssence(): this(1)
		{
            Name = "fire essence";

            ItemID = 16395; 
            Hue = 2075;
		}

        [Constructable]
        public FireEssence(int amount): base(amount)
		{
            Name = "fire essence";

            ItemID = 16395;
            Hue = 2075;
		}

        public FireEssence(Serial serial): base(serial)
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