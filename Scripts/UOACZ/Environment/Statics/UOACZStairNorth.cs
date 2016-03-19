using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;

namespace Server.Custom
{
    public class UOACZStairNorth : UOACZStatic
    {
        [Constructable]
        public UOACZStairNorth(): base()
        {
            Name = "stair";

            ItemID = 13835;
            Hue = 2405;
        }

        public UOACZStairNorth(Serial serial): base(serial)
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
