﻿using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class DeceitDungeonCore : DungeonCore
    {
        [Constructable]
        public DeceitDungeonCore(): this(1)
		{
            Name = "deceit dungeon core";

            Dungeon = DungeonEnum.Deceit;
		}

        [Constructable]
        public DeceitDungeonCore(int amount): base(amount)
		{
            Name = "deceit dungeon core";

            Dungeon = DungeonEnum.Deceit;
		}

        public DeceitDungeonCore(Serial serial): base(serial)
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