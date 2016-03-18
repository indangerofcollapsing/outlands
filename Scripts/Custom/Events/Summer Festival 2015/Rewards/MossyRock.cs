using System;
using Server;
using Server.Network;

namespace Server.Items
{
    public class MossyRock : Item
    {
        public override bool AlwaysAllowDoubleClick { get { return true; } }

        [Constructable]
        public MossyRock(): base(Utility.RandomMinMax(0x1363, 0x136D))
        {
            Name = "a mossy rock";           
            Hue = 2001;

            Weight = 5;
        }

        public override void OnDoubleClick(Mobile from)
        {            
            from.SendMessage("It appears to be a rock. It also appears to be mossy.");
        }

        public MossyRock(Serial serial): base(serial)
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