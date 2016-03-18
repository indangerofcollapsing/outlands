using System;
using Server.Items;
using Server;
using Server.Misc;
using Server.Custom;

namespace Server
{
    public class MiniatureHouse : Item
    {  
        [Constructable]
        public MiniatureHouse(): base(8928)
        {
            Name = "a miniature house";
            Hue = 2599;
            Weight = 1.0;           
        }

        public MiniatureHouse(Serial serial): base(serial)
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