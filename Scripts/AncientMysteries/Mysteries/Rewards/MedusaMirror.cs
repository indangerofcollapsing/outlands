using System;
using Server.Items;
using Server;
using Server.Misc;
using Server.Custom;

namespace Server
{
    [FlipableAttribute(10875, 10877)]
    public class MedusaMirror : Item
    {  
        [Constructable]
        public MedusaMirror(): base(10875)
        {
            Name = "a mirror";
            Hue = 2602;
            Weight = 10.0;           
        }

        public MedusaMirror(Serial serial): base(serial)
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