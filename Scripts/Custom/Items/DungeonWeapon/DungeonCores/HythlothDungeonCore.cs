using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class HythlothDungeonCore : DungeonCore
    {
        [Constructable]
        public HythlothDungeonCore(): this(1)
		{
            Name = "hythloth dungeon core";

            Dungeon = BaseDungeonArmor.DungeonEnum.Hythloth;
		}

        [Constructable]
        public HythlothDungeonCore(int amount): base(amount)
		{
            Name = "hythloth dungeon core";

            Dungeon = BaseDungeonArmor.DungeonEnum.Hythloth;
		}

        public HythlothDungeonCore(Serial serial): base(serial)
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