using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class ObsidianShard : CraftingComponent
    {
        [Constructable]
        public ObsidianShard(): this(1)
		{
            Name = "obsidian shard";

            ItemID = 11703; 
            Hue = 1102;
		}

        [Constructable]
        public ObsidianShard(int amount): base(amount)
		{
            Name = "obsidian shard";

            ItemID = 11703;
            Hue = 1102;
		}

        public ObsidianShard(Serial serial): base(serial)
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