using System;
using Server;
using Server.Network;

namespace Server.Items
{
    public class TreeStalkerVine : Item
    {
        [Constructable]
        public TreeStalkerVine() : base(Utility.RandomMinMax(0x4792, 0x4795))
        {
            Name = "an old vine";           
            Weight = 5;
        }

        public TreeStalkerVine(Serial serial) : base(serial) 
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}