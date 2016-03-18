using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class UOACZSackOfFlour : Item
    {
        [Constructable]
        public UOACZSackOfFlour(): base(4165)
        {
            Name = "a sack of flour";
            Weight = 3;
        }

        public UOACZSackOfFlour(Serial serial): base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            from.SendMessage("Use a skillet to turn this into a large bowl of dough.");
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