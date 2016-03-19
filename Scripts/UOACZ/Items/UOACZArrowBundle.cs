using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class UOACZArrowBundle : Item
    {
        [Constructable]
        public UOACZArrowBundle(): base(3905)
        {
            Name = "an arrow bundle";
            Weight = 3;
        }

        public UOACZArrowBundle(Serial serial): base(serial)
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

            from.SendMessage("You separate the arrow bundle and its contents are placed in your backpack.");

            Item[] arrows = from.Backpack.FindItemsByType(typeof(Arrow));

            Arrow largestItem = null;
            int highestCount = 0;

            foreach (Item item in arrows)
            {
                Arrow arrow = item as Arrow;

                if (arrow.Amount > highestCount)
                    largestItem = arrow;
            }

            if (largestItem != null)
                largestItem.Amount += 25;

            else
                from.Backpack.DropItem(new Arrow(25));

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