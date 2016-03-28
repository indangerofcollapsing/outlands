using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class CovetousDungeonCore : DungeonCore
    {
        [Constructable]
        public CovetousDungeonCore(): this(1)
		{
            Name = "covetous dungeon core";

            Dungeon = DungeonEnum.Covetous;
		}

        [Constructable]
        public CovetousDungeonCore(int amount): base(amount)
		{
            Name = "covetous dungeon core";

            Dungeon = DungeonEnum.Covetous;
		}

        public CovetousDungeonCore(Serial serial): base(serial)
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