using System;
using Server.Items;
using Server;
using Server.Misc;
using Server.Custom;

namespace Server
{
    public class VampireCoffin : Item
    {  
        [Constructable]
        public VampireCoffin(): base(16142)
        {
            Name = "a coffin";
            Weight = 10.0;           
        }

        public VampireCoffin(Serial serial): base(serial)
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