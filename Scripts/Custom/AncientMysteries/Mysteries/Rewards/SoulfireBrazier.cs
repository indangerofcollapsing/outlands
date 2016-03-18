using System;
using Server.Items;
using Server;
using Server.Misc;
using Server.Custom;

namespace Server
{    
    public class SoulfireBrazier : Item
    {  
        [Constructable]
        public SoulfireBrazier(): base(11736)
        {
            Name = "a soulfire brazier";
            Hue = 2637;
            Weight = 10.0;           
        }

        public SoulfireBrazier(Serial serial): base(serial)
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