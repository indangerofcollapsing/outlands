using System;
using Server.Items;
using Server;
using Server.Misc;
using Server.Custom;

namespace Server
{
    public class MiniatureTree : Item
    {  
        [Constructable]
        public MiniatureTree(): base(16140)
        {
            Name = "a miniature tree";
            Hue = 2599;
            Weight = 1.0;           
        }

        public MiniatureTree(Serial serial): base(serial)
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