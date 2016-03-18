using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;

namespace Server.Custom
{
    public class UOACZStairSouth : UOACZStatic
    {       
        [Constructable]
        public UOACZStairSouth(): base()
        {
            Name = "stair";

            ItemID = 13837;
            Hue = 2405;
        }

        public UOACZStairSouth(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version          
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
