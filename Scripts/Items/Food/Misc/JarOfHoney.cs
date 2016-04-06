using System;

namespace Server.Items
{
    public class JarOfHoney : Item
    {
        [Constructable]
        public JarOfHoney(): this(1)
        {
        }

        [Constructable]
        public JarOfHoney(int amount): base(0x9ec)
        {
            Name = "jar of honey";

            Stackable = true;
            Weight = 1;
            Amount = amount;
        }

        public JarOfHoney(Serial serial): base(serial)
        {
        }

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