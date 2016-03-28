using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class DespiseDungeonCore : DungeonCore
    {
        [Constructable]
        public DespiseDungeonCore(): this(1)
		{
            Name = "despise dungeon core";

            Dungeon = DungeonEnum.Despise;
		}

        [Constructable]
        public DespiseDungeonCore(int amount): base(amount)
		{
            Name = "despise dungeon core";

            Dungeon = DungeonEnum.Despise;
		}

        public DespiseDungeonCore(Serial serial): base(serial)
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