using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class BluecapMushroom : CraftingComponent
    {
        [Constructable]
        public BluecapMushroom(): this(1)
		{
            Name = "bluecap mushroom";

            ItemID = 3350; 
            Hue = 2599;
		}

        [Constructable]
        public BluecapMushroom(int amount): base(amount)
		{
            Name = "bluecap mushroom";

            ItemID = 3350;
            Hue = 2599;
		}

        public BluecapMushroom(Serial serial): base(serial)
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