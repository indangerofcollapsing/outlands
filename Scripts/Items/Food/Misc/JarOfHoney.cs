using System;

namespace Server.Items
{
    public class JarOfHoney : Item
    {
        [Constructable]
        public JarOfHoney(): base(0x9ec)
        {
            Stackable = true;
            Weight = 1;
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