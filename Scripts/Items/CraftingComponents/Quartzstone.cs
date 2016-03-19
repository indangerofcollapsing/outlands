using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class Quartzstone : CraftingComponent
    {
        [Constructable]
        public Quartzstone(): this(1)
		{
            Name = "quartzstone";

            ItemID = 5925; 
            Hue = 2507;
		}

        [Constructable]
        public Quartzstone(int amount): base(amount)
		{
            Name = "quartzstone";

            ItemID = 5925;
            Hue = 2507;
		}

        public Quartzstone(Serial serial): base(serial)
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