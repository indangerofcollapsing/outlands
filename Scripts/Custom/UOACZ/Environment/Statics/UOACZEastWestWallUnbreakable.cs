using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;

namespace Server.Custom
{
    public class UOACZEastWestWallUnbreakable : UOACZStatic
    {
        [Constructable]
        public UOACZEastWestWallUnbreakable(): base()
        {
            Name = "wall";

            ItemID = 13883;
            Hue = 2405; 
        }

        public UOACZEastWestWallUnbreakable(Serial serial): base(serial)
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
