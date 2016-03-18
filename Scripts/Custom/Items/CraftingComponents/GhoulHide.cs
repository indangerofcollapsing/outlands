using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class GhoulHide : CraftingComponent
    {
        [Constructable]
        public GhoulHide() : this(1)
		{
            Name = "ghoul hide";

            ItemID = 12677; 
            Hue = 2610;
		}

        [Constructable]
        public GhoulHide(int amount): base(amount)
		{
            Name = "ghoul hide";

            ItemID = 12677;
            Hue = 2610;
		}

        public GhoulHide(Serial serial): base(serial)
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