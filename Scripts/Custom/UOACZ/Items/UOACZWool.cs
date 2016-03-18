using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class UOACZWool : Item
    {
        [Constructable]
        public UOACZWool(): base(3576)
		{
            Name = "wool";

            Stackable = true;
            Amount = 1;
            Weight = 1;
		}

        [Constructable]
        public UOACZWool(int amount): base(3576)
		{
            Name = "wool";

            Stackable = true;
            Amount = amount;
            Weight = 1;
		}

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            from.SendMessage("Use a sewing kit to spin this cotton into a bolt of cloth.");
        }

        public UOACZWool(Serial serial): base(serial)
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