using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class ShameDungeonCore : DungeonCore
    {
        [Constructable]
        public ShameDungeonCore(): this(1)
		{
            Name = "shame dungeon core";

            Dungeon = DungeonEnum.Shame;
		}

        [Constructable]
        public ShameDungeonCore(int amount): base(amount)
		{
            Name = "shame dungeon core";

            Dungeon = DungeonEnum.Shame;
		}

        public ShameDungeonCore(Serial serial): base(serial)
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