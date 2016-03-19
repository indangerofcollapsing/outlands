using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class DestardDungeonCore : DungeonCore
    {
        [Constructable]
        public DestardDungeonCore(): this(1)
		{
            Name = "destard dungeon core";

            Dungeon = BaseDungeonArmor.DungeonEnum.Destard;
		}

        [Constructable]
        public DestardDungeonCore(int amount): base(amount)
		{
            Name = "destard dungeon core";

            Dungeon = BaseDungeonArmor.DungeonEnum.Destard;
		}

        public DestardDungeonCore(Serial serial): base(serial)
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