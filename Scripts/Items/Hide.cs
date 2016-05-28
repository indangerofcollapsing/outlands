using System;
using Server.Items;

namespace Server
{
    public class Hide : Item
    {
        public static int GoldValue = 3;

        [Constructable]
        public Hide(int amount): base(0x1079)
        {
            Name = "hide";

            Stackable = true;
            Weight = .01;

            Amount = amount;
        }

        [Constructable]
        public Hide(): base(0x1079)
        {
            Name = "hide";

            Stackable = true;
            Weight = .01;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "(can be sold)");
        }

        public Hide(Serial serial): base(serial)
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

            //Version 0
            if (version >= 0)
            {
            }
        }
    }
}