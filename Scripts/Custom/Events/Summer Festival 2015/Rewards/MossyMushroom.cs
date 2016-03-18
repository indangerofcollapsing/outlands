using System;
using Server;
using Server.Network;

namespace Server.Items
{
    public class MossyMushroom : Item
    {
        public override bool AlwaysAllowDoubleClick { get { return true; } }

        [Constructable]
        public MossyMushroom(): base(Utility.RandomMinMax(0x0D0C, 0x0D19))
        {
            Name = "mushroom";           
            Hue = 2001;

            Weight = 1;
        }

        public override void OnDoubleClick(Mobile from)
        {            
            from.SendMessage("Yup. It's a mushroom.");
        }

        public MossyMushroom(Serial serial) : base(serial)
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