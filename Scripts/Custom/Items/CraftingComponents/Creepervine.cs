using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class Creepervine : CraftingComponent
    {
        [Constructable]
        public Creepervine(): this(1)
		{
            Name = "creepervine";

            ItemID = 22311;
            Hue = 2208;
		}

        [Constructable]
        public Creepervine(int amount): base(amount)
		{
            Name = "creepervine";

            ItemID = 22311;
            Hue = 2208;
		}

        public Creepervine(Serial serial): base(serial)
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