using System;
using Server;

namespace Server.Items
{
    public class Eggs : Item
    {  
        [Constructable]
        public Eggs(): this(1)
        {
        }

        [Constructable]
        public Eggs(int amount): base(2485)
        {
            Name = "eggs";

            Stackable = true;
            Weight = .1;
            Amount = amount;
        }

        public Eggs(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}