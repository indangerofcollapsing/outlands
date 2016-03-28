using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class IceDungeonCore : DungeonCore
    {
        [Constructable]
        public IceDungeonCore(): this(1)
		{
            Name = "ice dungeon core";

            Dungeon = DungeonEnum.Ice;
		}

        [Constructable]
        public IceDungeonCore(int amount): base(amount)
		{
            Name = "ice dungeon core";

            Dungeon = DungeonEnum.Ice;
		}

        public IceDungeonCore(Serial serial): base(serial)
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