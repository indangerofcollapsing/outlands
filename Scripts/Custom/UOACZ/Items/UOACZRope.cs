using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class UOACZRope : Item
    {
        [Constructable]
        public UOACZRope(): base(5368)
		{
            Name = "rope";

            Stackable = true;
            Amount = 1;
            Weight = 1;
		}

        [Constructable]
        public UOACZRope(int amount): base(5368)
		{
            Name = "rope";

            Stackable = true;
            Amount = amount;
            Weight = 1;
		}

        public UOACZRope(Serial serial): base(serial)
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