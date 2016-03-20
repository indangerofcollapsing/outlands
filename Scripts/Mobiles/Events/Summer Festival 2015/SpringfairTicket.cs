using System;

namespace Server.Items
{  
    public class SpringfairTicket : Item
    {        
        [Constructable]
        public SpringfairTicket() : this(1) 
        {
        }

        [Constructable]
        public SpringfairTicket(int amount) : base(0xFA0)
        {
            Amount = amount;

            Name = "springfair ticket";
            Stackable = true;

            Hue = 1167;
            Weight = 1.0;
        }

        public SpringfairTicket(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}