using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class UOACZCotton : Item
    {
        [Constructable]
        public UOACZCotton(): base(3577)
		{
            Name = "cotton";

            Stackable = true;
            Amount = 1;
            Weight = 1;
		}

        [Constructable]
        public UOACZCotton(int amount): base(3577)
		{
            Name = "cotton";

            Stackable = true;
            Amount = amount;
            Weight = 1;
		}

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            from.SendMessage("Use a sewing kit to spin this cotton into a bolt of cloth.");
        }

        public UOACZCotton(Serial serial): base(serial)
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