using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class WrongDungeonCore : DungeonCore
    {
        [Constructable]
        public WrongDungeonCore(): this(1)
		{
            Name = "wrong dungeon core";

            Dungeon = BaseDungeonArmor.DungeonEnum.Wrong;
		}

        [Constructable]
        public WrongDungeonCore(int amount): base(amount)
		{
            Name = "wrong dungeon core";

            Dungeon = BaseDungeonArmor.DungeonEnum.Wrong;
		}

        public WrongDungeonCore(Serial serial): base(serial)
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