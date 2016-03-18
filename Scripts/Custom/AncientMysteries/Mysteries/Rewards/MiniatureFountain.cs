using System;
using Server.Items;
using Server;
using Server.Misc;
using Server.Custom;

namespace Server
{
    public class MiniatureFountain : Item
    {  
        [Constructable]
        public MiniatureFountain(): base(16144)
        {
            Name = "a miniature fountain";
            Hue = 2599;
            Weight = 2.0;           
        }

        public MiniatureFountain(Serial serial): base(serial)
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