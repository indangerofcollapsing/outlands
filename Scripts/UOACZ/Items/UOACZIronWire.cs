using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class UOACZIronWire : Item
    {
        [Constructable]
        public UOACZIronWire(): base(6262)
		{
            Name = "iron wire";

            Stackable = true;
            Amount = 1;
            Weight = 1;
		}

        [Constructable]
        public UOACZIronWire(int amount): base(6262)
		{
            Name = "iron wire";

            Stackable = true;
            Amount = amount;
            Weight = 1;
		}

        public UOACZIronWire(Serial serial): base(serial)
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