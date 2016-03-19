using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Multis;
using Server.Targeting;
using Server.Commands;
using Server.Gumps;
using System.Reflection;


namespace Server.Items
{
    public class ShipParts : Item
    {
        [Constructable]
        public ShipParts() : base(0x14F3)
        {
            Weight = 2.0;
            Name = "ship parts";

            //Stackable = true; //Need to Edit TileData.mul to set TileFlag = Generic
        }

        public ShipParts(Serial serial): base(serial)
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