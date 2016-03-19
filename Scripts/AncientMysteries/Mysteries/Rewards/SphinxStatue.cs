using System;
using Server.Items;
using Server;
using Server.Misc;
using Server.Custom;

namespace Server
{
    [FlipableAttribute(6473, 6474)]
    public class SphinxStatue : Item
    {  
        [Constructable]
        public SphinxStatue(): base(6473)
        {
            Name = "a strange statue";
            Hue = 2599;
            Weight = 10.0;           
        }

        public SphinxStatue(Serial serial): base(serial)
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