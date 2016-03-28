using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class FireDungeonCore : DungeonCore
    {
        [Constructable]
        public FireDungeonCore(): this(1)
		{
            Name = "fire dungeon core";

            Dungeon = DungeonEnum.Fire;
		}

        [Constructable]
        public FireDungeonCore(int amount): base(amount)
		{
            Name = "fire dungeon core";

            Dungeon = DungeonEnum.Fire;
		}

        public FireDungeonCore(Serial serial): base(serial)
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