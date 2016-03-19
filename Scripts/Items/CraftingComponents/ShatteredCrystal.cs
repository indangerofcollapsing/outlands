using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class ShatteredCrystal : CraftingComponent
    {
        [Constructable]
        public ShatteredCrystal() : this(1)
		{
            Name = "shattered crystal";

            ItemID = 22328; 
            Hue = 84;
		}

        [Constructable]
        public ShatteredCrystal(int amount): base(amount)
		{
            Name = "shattered crystal";

            ItemID = 22328;
            Hue = 84;
		}

        public ShatteredCrystal(Serial serial): base(serial)
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