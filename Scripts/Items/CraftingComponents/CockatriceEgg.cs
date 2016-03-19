using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class CockatriceEgg : CraftingComponent
    {
        [Constructable]
        public CockatriceEgg(): this(1)
		{
            Name = "cockatrice egg";

            ItemID = 10249;
            Hue = 2589;
		}

        [Constructable]
        public CockatriceEgg(int amount): base(amount)
		{
            Name = "cockatrice egg";
            
            ItemID = 10249;
            Hue = 2589;
		}

        public CockatriceEgg(Serial serial): base(serial)
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