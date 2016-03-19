using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class UOACZShaftBundle : Item
    {
        [Constructable]
        public UOACZShaftBundle(): base(7126)
        {
            Name = "a shaft bundle";

            Weight = 1;
        }

        public UOACZShaftBundle(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "(double click to separate)");
        }
        
        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That item must be in your pack in order to use it.");
                return;
            }

            from.SendMessage("You separate the shaft bundle and its contents are placed in your backpack.");

            Item[] shafts = from.Backpack.FindItemsByType(typeof(Shaft));

            Shaft largestItem = null;
            int highestCount = 0;

            foreach (Item item in shafts)
            {
                Shaft shaft = item as Shaft;

                if (shaft.Amount > highestCount)
                    largestItem = shaft;
            }

            if (largestItem != null)
                largestItem.Amount += 25;

            else
                from.Backpack.DropItem(new Shaft(25));

            Delete();
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